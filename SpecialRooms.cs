using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GungeonAPI;
using UnityEngine;
using Dungeonator;
using FakePrefab = SpecialItemPack.ItemAPI.FakePrefab;

namespace SpecialItemPack
{
    class SpecialRooms
    {
        public static void InitRainbowchestMimicRoom()
        {
            var data = RoomFactory.BuildFromResource("SpecialItemPack/Resources/Rooms/Secret_RainbowmimicRoom.room");
            PrototypeDungeonRoom protoroom = data.room;
            AddPlaceable(protoroom, new Vector2(-2f, 0f), new IntVector2(3, 2), SpecialItemModule.rainbowMimicChest.gameObject);
            RoomFactory.rooms.Add("spapi:rainbowchestmimic_room", data);
            DungeonHandler.Register(data);
        }

        public static void InitSkullShrineRoom()
        {
            var data = RoomFactory.BuildFromResource("SpecialItemPack/Resources/Rooms/Room_ReaperShrine.room");
            PrototypeDungeonRoom protoroom = data.room;
            AddPlaceables(protoroom, new Vector2(-1f, 0f), new IntVector2(2, 2), new List<GameObject> { SpecialItemModule.sreaperShrine, SpecialItemModule.grimShrine });
            RoomFactory.rooms.Add("spapi:skullshrine_room", data);
            DungeonHandler.Register(data);
        }

        public static void InitGorbShrineRoom()
        {
            var data = RoomFactory.BuildFromResource("SpecialItemPack/Resources/Rooms/Room_GorbShrine.room");
            PrototypeDungeonRoom protoroom = data.room;
            AddPlaceable(protoroom, new Vector2(-1f, 0f), new IntVector2(2, 2), SpecialItemModule.gorbShrine);
            RoomFactory.rooms.Add("spapi:gorbshrine_room", data);
            DungeonHandler.Register(data);
        }

        public static void InitComfortableCarpetRoom()
        {
            var data = RoomFactory.BuildFromResource("SpecialItemPack/Resources/Rooms/Room_ComfortableCarpet.room");
            PrototypeDungeonRoom protoroom = data.room;
            AddPlaceable(protoroom, new Vector2(-3f, 0f), new IntVector2(5, 5), SpecialItemModule.comfortableCarpet);
            RoomFactory.rooms.Add("spapi:comfortablecarpet_room", data);
            DungeonHandler.Register(data);
        }

        public static void InitGreenchamberPitRoom()
        {
            var data = RoomFactory.BuildFromResource("SpecialItemPack/Resources/Rooms/GreenChamberRoom_Part1.room");
            PrototypeDungeonRoom protoroom = data.room;
            GameObject obj = new GameObject("UnlockGreenchamberObject");
            obj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(obj);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.AddComponent<UnlockGreenChamberBehaviour>();
            AddPlaceable(protoroom, new Vector2(0f, 0f), new IntVector2(0, 0), obj);
            RoomFactory.rooms.Add("spapi:greenchamber_pit_room", data);
            DungeonHandler.Register(data);
        }

        public static void AddPlaceables(PrototypeDungeonRoom room, Vector2 offset, IntVector2 size, List<GameObject> placeables)
        {
            Vector2 position = new Vector2((float)room.Width / 2 + offset.x, (float)room.Height / 2 + offset.y);
            room.placedObjectPositions.Add(position);
            var placeableContents = ScriptableObject.CreateInstance<DungeonPlaceable>();
            placeableContents.width = size.x;
            placeableContents.height = size.y;
            placeableContents.respectsEncounterableDifferentiator = true;
            placeableContents.variantTiers = new List<DungeonPlaceableVariant>() { };
            foreach(GameObject obj in placeables)
            {
                placeableContents.variantTiers.Add(
                    new DungeonPlaceableVariant()
                    {
                        percentChance = 1,
                        nonDatabasePlaceable = obj,
                        prerequisites = new DungeonPrerequisite[0],
                        materialRequirements = new DungeonPlaceableRoomMaterialRequirement[0]
                    }
                );
            }
            room.placedObjects.Add(new PrototypePlacedObjectData()
            {
                contentsBasePosition = position,
                fieldData = new List<PrototypePlacedObjectFieldData>(),
                instancePrerequisites = new DungeonPrerequisite[0],
                linkedTriggerAreaIDs = new List<int>(),
                placeableContents = placeableContents
            });
        }

        public static void AddPlaceable(PrototypeDungeonRoom room, Vector2 offset, IntVector2 size, GameObject placeable)
        {
            Vector2 position = new Vector2((float)room.Width / 2 + offset.x, (float)room.Height / 2 + offset.y);
            room.placedObjectPositions.Add(position);
            var placeableContents = ScriptableObject.CreateInstance<DungeonPlaceable>();
            placeableContents.width = size.x;
            placeableContents.height = size.y;
            placeableContents.respectsEncounterableDifferentiator = true;
            placeableContents.variantTiers = new List<DungeonPlaceableVariant>()
            {
                new DungeonPlaceableVariant()
                {
                    percentChance = 1,
                    nonDatabasePlaceable = placeable,
                    prerequisites = new DungeonPrerequisite[0],
                    materialRequirements= new DungeonPlaceableRoomMaterialRequirement[0]
                }
            };
            room.placedObjects.Add(new PrototypePlacedObjectData()
            {
                contentsBasePosition = position,
                fieldData = new List<PrototypePlacedObjectFieldData>(),
                instancePrerequisites = new DungeonPrerequisite[0],
                linkedTriggerAreaIDs = new List<int>(),
                placeableContents = placeableContents
            });
        }

        public static void Init()
        {
            InitRainbowchestMimicRoom();
            InitSkullShrineRoom();
            InitGorbShrineRoom();
            InitComfortableCarpetRoom();
            InitGreenchamberPitRoom();
        }
    }
}
