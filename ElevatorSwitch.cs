using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;
using Pathfinding;

namespace SpecialItemPack
{
    class ElevatorSwitch : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Mysterious Switch";
            string resourceName = "SpecialItemPack/Resources/howtothrowbooks";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ElevatorSwitch>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "What Does It Do?";
            string longDesc = "A Mysterious Switch. You don't know what does it do, but you probably should give it a go and flip it.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 0.5f);
            item.consumable = false;
            item.quality = ItemQuality.D;
            item.PlaceItemInAmmonomiconAfterItemById(814);
            GameObject platformOfDeath = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/PlatformOfDeath");
            platformOfDeath.SetActive(false);
            FakePrefab.MarkAsFakePrefab(platformOfDeath);
            UnityEngine.Object.DontDestroyOnLoad(platformOfDeath);
            PlatformOfDeath = platformOfDeath;
            GameObject bigPlatformOfDeath = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/BigPlatformOfDeath");
            bigPlatformOfDeath.SetActive(false);
            FakePrefab.MarkAsFakePrefab(bigPlatformOfDeath);
            UnityEngine.Object.DontDestroyOnLoad(bigPlatformOfDeath);
            BigPlatformOfDeath = bigPlatformOfDeath;
            GameObject speedEffect = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/SpeedEffect");
            speedEffect.SetActive(false);
            FakePrefab.MarkAsFakePrefab(speedEffect);
            UnityEngine.Object.DontDestroyOnLoad(speedEffect);
            speedEffect.AddComponent<SpeedEffectBehaviour>();
            SpeedEffect = speedEffect;
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            RoomHandler room = this.AddRuntimeRoom();
            Pathfinder.Instance.InitializeRegion(GameManager.Instance.Dungeon.data, room.area.basePosition, room.area.dimensions);
            user.WarpToPoint((room.area.basePosition + new IntVector2(12, 4)).ToVector2(), false, false);
            this.m_isCurrentlyActive = true;
        }

        public override void Update()
        {
            base.Update();
            if (this.m_isCurrentlyActive)
            {
                Vector2 v = GameManager.Instance.MainCameraController.transform.position.XY() + UnityEngine.Random.insideUnitCircle.normalized * GameManager.Instance.MainCameraController.Camera.orthographicSize * 2f;
                GameObject obj = Instantiate(SpeedEffect, v, Quaternion.identity);
                obj.GetComponent<tk2dBaseSprite>().HeightOffGround = -35f;
                obj.GetComponent<tk2dBaseSprite>().UpdateZDepth();
                obj.GetComponent<SpeedEffectBehaviour>().platform = this.m_currentPlatform;

            }
        }

        public RoomHandler AddRuntimeRoom()
        {
            IntVector2 dimensions = new IntVector2(15, 15);
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
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    CellType cellType = (x >= 4 && x <= 9 && y >= 4 && y <= 9) ? CellType.FLOOR : CellType.PIT;
                    IntVector2 p = new IntVector2(x, y) + intVector;
                    CellData cellData = new CellData(p, cellType);
                    cellData.parentArea = cellArea;
                    cellData.parentRoom = roomHandler;
                    cellData.nearestRoom = roomHandler;
                    array[p.x][p.y] = cellData;
                    roomHandler.RuntimeStampCellComplex(p.x, p.y, cellType, DiagonalWallType.NONE);
                }
            }
            d.data.rooms.Add(roomHandler);
            GameObject obj = Instantiate(PlatformOfDeath, new Vector3((float)intVector.x + 5, (float)intVector.y + 3, 0f), Quaternion.identity);
            obj.GetComponent<tk2dBaseSprite>().HeightOffGround = -30f;
            obj.GetComponent<tk2dBaseSprite>().UpdateZDepth();
            GameObject obj2 = Instantiate(BigPlatformOfDeath, new Vector3((float)intVector.x, (float)intVector.y - 2f, 0f), Quaternion.identity);
            obj2.GetComponent<tk2dBaseSprite>().HeightOffGround = -30f;
            obj2.GetComponent<tk2dBaseSprite>().UpdateZDepth();
            this.m_currentPlatform = obj;
            DeadlyDeadlyGoopManager.ReinitializeData();
            roomHandler.PreventMinimapUpdates = true;
            return roomHandler;
        }

        public static GameObject PlatformOfDeath;
        public static GameObject BigPlatformOfDeath;
        public static GameObject SpeedEffect;
        private GameObject m_currentPlatform;

        public class SpeedEffectBehaviour : MonoBehaviour
        {
            private void Update()
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 50 * BraveTime.DeltaTime, this.transform.position.z);
                if(platform != null)
                {
                    tk2dBaseSprite sprite = this.GetComponent<tk2dBaseSprite>();
                    tk2dBaseSprite platformSprite = platform.GetComponent<tk2dBaseSprite>();
                    if(sprite.WorldBottomLeft.x >= platformSprite.WorldBottomLeft.x && sprite.WorldBottomRight.x <= platformSprite.WorldBottomRight.x)
                    {
                        if(sprite.WorldBottomCenter.y < platformSprite.WorldBottomCenter.y)
                        {
                            Destroy(this.gameObject);
                        }
                    }
                }
            }

            public GameObject platform;
        }
    }
}
