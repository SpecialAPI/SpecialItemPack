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
using UnityEngine.SceneManagement;
using GungeonAPI;

namespace SpecialItemPack
{
    class RedChamberDungeon : SpecialDungeon
    {
        public override Dungeon BuildDungeon(Dungeon dungeon)
        {
            dungeon.gameObject.name = "RedChamber";
            dungeon.LevelOverrideType = GameManager.LevelOverrideState.NONE;
            dungeon.contentSource = ContentSource.BASE;
            dungeon.DungeonShortName = "#SPAPI_DUNGEON_RED_CHAMBER_SHORT";
            dungeon.DungeonFloorName = "#SPAPI_DUNGEON_RED_CHAMBER";
            dungeon.DungeonFloorLevelTextOverride = "#SPAPI_DUNGEON_RED_CHAMBER_FLOOR_TEXT";
            SpecialItemModule.Strings.Core.Set("#SPAPI_DUNGEON_RED_CHAMBER", "\"in the chamber\"");
            SpecialItemModule.Strings.Core.Set("#SPAPI_DUNGEON_RED_CHAMBER_FLOOR_TEXT", "Red Chamber");
            SpecialItemModule.Strings.Core.Set("#SPAPI_DUNGEON_RED_CHAMBER_SHORT", "Chamber");
            dungeon.PatternSettings = new SemioticDungeonGenSettings
            {
                DEBUG_RENDER_CANVASES_SEPARATELY = dungeon.PatternSettings.DEBUG_RENDER_CANVASES_SEPARATELY,
                flows = new List<DungeonFlow> { RedChamberDungeonFlow.BuildFlow() },
                mandatoryExtraRooms = dungeon.PatternSettings.mandatoryExtraRooms,
                MAX_GENERATION_ATTEMPTS = dungeon.PatternSettings.MAX_GENERATION_ATTEMPTS,
                optionalExtraRooms = dungeon.PatternSettings.optionalExtraRooms
            };
            dungeon.tileIndices.tilesetId = (GlobalDungeonData.ValidTilesets)CustomValidTilesets.CHAMBERGEON;
            return dungeon;
        }

        public override float BossDPSCap => 999f;
        public override float DamageCap => 99f;
        public override float EnemyHealthMultiplier => 99f;
        public override List<DungeonFlowLevelEntry> FlowEntries => base.FlowEntries;
        public override string PrefabPath => "Red_Chamber";
        public override float PriceMultiplier => 99f;
        public override string SceneName => "spapi_chamber";
        public override float SecretDoorHealthMultiplier => 99f;
    }

    class RedChamberDungeonFlow
    {
        public static DungeonFlow BuildFlow()
        {
            DungeonFlow flow = ScriptableObject.CreateInstance<DungeonFlow>();
            flow.name = "RedChamberDungeonFlow";
            flow.fallbackRoomTable = null;
            flow.phantomRoomTable = null;
            flow.subtypeRestrictions = new List<DungeonFlowSubtypeRestriction>(0);
            flow.flowInjectionData = new List<ProceduralFlowModifierData>(0);
            flow.sharedInjectionData = new List<SharedInjectionData>() { Toolbox.shared_auto_002.LoadAsset<SharedInjectionData>("Base Shared Injection Data") };
            DungeonFlowNode node = Toolbox.GenerateFlowNode(flow, PrototypeDungeonRoom.RoomCategory.ENTRANCE, RoomFactory.BuildFromResource("SpecialItemPack/RedChamberRooms/RedChamber_Entrance.room").room);
            DungeonFlowNode node2 = Toolbox.GenerateFlowNode(flow, PrototypeDungeonRoom.RoomCategory.EXIT, RoomFactory.BuildFromResource("SpecialItemPack/RedChamberRooms/RedChamber_Exit.room").room);
            flow.Initialize();
            flow.AddNodeToFlow(node, null);
            flow.AddNodeToFlow(node2, node);
            flow.FirstNode = node;
            return flow;
        }
    }
}
