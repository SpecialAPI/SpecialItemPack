using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using System.Collections;

namespace SpecialItemPack
{
    class UnlockGreenChamberBehaviour : DungeonPlaceableBehaviour, IPlaceConfigurable
    {
        private void Update()
        {
            for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
            {
                PlayerController playerController2 = GameManager.Instance.AllPlayers[j];
                if (playerController2 && playerController2.IsFlying && !playerController2.IsGhost && playerController2.CurrentRoom != null && playerController2.CurrentRoom == this.parentRoom)
                {
                    CellData cell = playerController2.CenterPosition.GetCell();
                    if (cell != null && cell.type == CellType.PIT && playerController2.IsFlying)
                    {
                        playerController2.ForceFall();
                    }
                }
            }
        }

        public static void BuildRewardPedestal()
        {
            Dungeon d = ItemBuilder.ForgeDungeonPrefab;
            foreach(WeightedGameObject weightedobj in d.sharedSettingsPrefab.ChestsForBosses.elements)
            {
                if(weightedobj != null && weightedobj.gameObject != null && weightedobj.gameObject.GetComponent<RewardPedestal>() != null)
                {
                    GameObject obj = Instantiate(weightedobj.gameObject);
                    obj.SetActive(false);
                    FakePrefab.MarkAsFakePrefab(obj);
                    DontDestroyOnLoad(obj);
                    GreenChamberRewardPedestal = obj;
                    RewardPedestal pedestal = obj.GetComponent<RewardPedestal>();
                    pedestal.MimicGuid = "";
                    pedestal.ReturnCoopPlayerOnLand = false;
                    pedestal.UsesSpecificItem = true;
                    pedestal.SpecificItemId = SpecialItemIds.GreenChamber;
                    pedestal.spawnAnimName = "";
                    pedestal.VFX_PreSpawn = null;
                    pedestal.SpawnsTertiarySet = false;
                    break;
                }
            }
        }

        public RoomHandler AddRuntimeRoom()
        {
            IntVector2 dimensions = new IntVector2(24, 24);
            Dungeon d = GameManager.Instance.Dungeon;
            IntVector2 intVector = new IntVector2(d.data.Width + 10, 10);
            int newWidth = d.data.Width + 10 + dimensions.x;
            int newHeight = Mathf.Max(d.data.Height, dimensions.y + 10);
            CellData[][] array = BraveUtility.MultidimensionalArrayResize<CellData>(d.data.cellData, d.data.Width, d.data.Height, newWidth, newHeight);
            CellArea cellArea = new CellArea(intVector, dimensions, 0);
            cellArea.IsProceduralRoom = true;
            d.data.cellData = array;
            d.data.ClearCachedCellData();
            RoomHandler roomHandler = new RoomHandler(cellArea);
            for (int i = 0; i < dimensions.x; i++)
            {
                for (int j = 0; j < dimensions.y; j++)
                {
                    IntVector2 p = new IntVector2(i, j) + intVector;
                    CellData cellData = new CellData(p, CellType.FLOOR);
                    cellData.parentArea = cellArea;
                    cellData.parentRoom = roomHandler;
                    cellData.nearestRoom = roomHandler;
                    array[p.x][p.y] = cellData;
                    roomHandler.RuntimeStampCellComplex(p.x, p.y, CellType.FLOOR, DiagonalWallType.NONE);
                }
            }
            d.data.rooms.Add(roomHandler);
            GameObject obj = Instantiate((GameObject)BraveResources.Load("Global Prefabs/CreepyEye_Room", ".prefab"), new Vector3((float)intVector.x, (float)intVector.y, 0f), Quaternion.identity);
            List<Component> children = obj.GetComponentsInChildren<Component>().ToList();
            foreach(Component component in obj.GetComponents<Component>())
            {
                if (children.Contains(component))
                {
                    children.Remove(component);
                }
            }
            foreach(Component child in children)
            {
                Destroy(child);
            }
            DeadlyDeadlyGoopManager.ReinitializeData();
            roomHandler.Entered += delegate (PlayerController p)
            {
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.ITEMSPECIFIC_GREEN_CHAMBER, true);
                for (int i = parentRoom.area.basePosition.x; i < parentRoom.area.basePosition.x + parentRoom.area.dimensions.x; i++)
                {
                    for (int j = parentRoom.area.basePosition.y; j < parentRoom.area.basePosition.y + parentRoom.area.dimensions.y; j++)
                    {
                        CellData cellData = GameManager.Instance.Dungeon.data[i, j];
                        if (cellData != null && cellData.parentRoom == parentRoom && !cellData.isExitCell)
                        {
                            cellData.type = CellType.FLOOR;
                        }
                    }
                }
                enteredGeneratedRoom = true;
                parentRoom.AddProceduralTeleporterToRoom();
                Destroy(eye);
            };
            Transform parent = new GameObject("room's parent").transform;
            parent.position = new Vector3((float)intVector.x, (float)intVector.y, 0f);
            roomHandler.hierarchyParent = parent;
            Transform arrivalPoint = new GameObject("Arrival").transform;
            arrivalPoint.name = "Arrival";
            arrivalPoint.position = new Vector3((float)intVector.x + (float)dimensions.x/2, (float)intVector.y + (float)dimensions.x / 4, 0f);
            arrivalPoint.parent = parent;
            if(GreenChamberRewardPedestal != null)
            {
                RewardPedestal.Spawn(GreenChamberRewardPedestal.GetComponent<RewardPedestal>(), new IntVector2((intVector.x + dimensions.x / 2) - 1, (intVector.y + dimensions.x / 2) - 3));
            }
            roomHandler.PreventMinimapUpdates = true;
            return roomHandler;
        }

        public void ConfigureOnPlacement(RoomHandler parentRoom)
        {
            this.parentRoom = parentRoom;
            eye = Instantiate(GreenChamberEyeController.eyePrefab, parentRoom.GetCenterCell().ToVector3(), Quaternion.identity);
            GameManager.Instance.StartCoroutine(this.DelayedStart());
        }

        private IEnumerator DelayedStart()
        {
            while (Dungeon.IsGenerating)
            {
                yield return null;
            }
            RoomHandler generatedRoom = this.AddRuntimeRoom();
            this.parentRoom.TargetPitfallRoom = generatedRoom;
            yield break;
        }

        private GameObject eye;
        private RoomHandler parentRoom;
        private static GameObject GreenChamberRewardPedestal;
        private float m_timeHovering = 0f;
        private bool enteredGeneratedRoom = false;
    }
}
