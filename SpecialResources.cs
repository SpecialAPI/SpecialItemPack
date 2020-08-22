using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MonoMod.RuntimeDetour;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using SpecialItemPack.Shapeshifting;

namespace SpecialItemPack
{
    class SpecialResources
    {
        public static void InitRainbowchestMimic()
        {
            Chest rainbowMimicChest = UnityEngine.Object.Instantiate<Chest>(GameManager.Instance.RewardManager.A_Chest);
            rainbowMimicChest.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(rainbowMimicChest.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(rainbowMimicChest);
            rainbowMimicChest.gameObject.AddComponent<SpecialItemModule.BecomeRainbowmimicBehaviour>();
            SpecialItemModule.rainbowMimicChest = rainbowMimicChest;
            rainbowMimicChest.overrideMimicChance = 100f;
            rainbowMimicChest.MimicGuid = "boss_rainbowchest_mimic";
            typeof(Chest).GetField("m_isMimic", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(rainbowMimicChest, true);
            rainbowMimicChest.BecomeRainbowChest();
        }

        public static void InitGrimShrine()
        {
            GameObject grimShrine = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/ShrinePedestal", new GameObject("GrimShrine"));
            grimShrine.SetActive(false);
            FakePrefab.MarkAsFakePrefab(grimShrine);
            UnityEngine.Object.DontDestroyOnLoad(grimShrine);
            Shader shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutout");
            grimShrine.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(0, 0), new IntVector2(26, 31)).PrimaryPixelCollider.CollisionLayer = CollisionLayer.HighObstacle;//31
            grimShrine.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.shader = shader;
            grimShrine.GetComponent<tk2dSprite>().GetCurrentSpriteDef().materialInst.shader = shader;
            grimShrine.GetComponent<tk2dSprite>().HeightOffGround = -0.7f;
            grimShrine.GetComponent<tk2dSprite>().UpdateZDepth();
            GrimShrine shrine = grimShrine.gameObject.AddComponent<GrimShrine>();
            SpecialItemModule.grimShrine = grimShrine;
            Transform transform = new GameObject("talkpoint").transform;
            transform.position = grimShrine.transform.position + new Vector3(0.5f, 1f, 0f);
            transform.SetParent(grimShrine.transform);
            shrine.talkPoint = transform;
            GameObject stoneGrim = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/ShrineReaper", new GameObject("StoneGrim"));
            stoneGrim.SetActive(false);
            FakePrefab.MarkAsFakePrefab(stoneGrim);
            UnityEngine.Object.DontDestroyOnLoad(stoneGrim);
            stoneGrim.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.shader = shader;
            stoneGrim.GetComponent<tk2dSprite>().GetCurrentSpriteDef().materialInst.shader = shader;
            stoneGrim.GetComponent<tk2dSprite>().HeightOffGround = 1.0f;
            stoneGrim.GetComponent<tk2dSprite>().UpdateZDepth();
            stoneGrim.transform.parent = grimShrine.transform;
            GrimShrine.spriteId = SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/ShrineReaperIcon", SpriteBuilder.itemCollection);
            shrine.stoneGrim = stoneGrim;
        }

        public static void InitGorbShrine()
        {
            GameObject gorbShrine = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/ShrinePedestal", new GameObject("GorbShrine"));
            gorbShrine.SetActive(false);
            FakePrefab.MarkAsFakePrefab(gorbShrine);
            UnityEngine.Object.DontDestroyOnLoad(gorbShrine);
            Shader shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutout");
            gorbShrine.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(0, 0), new IntVector2(26, 31)).PrimaryPixelCollider.CollisionLayer = CollisionLayer.HighObstacle;//31
            gorbShrine.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.shader = shader;
            gorbShrine.GetComponent<tk2dSprite>().GetCurrentSpriteDef().materialInst.shader = shader;
            gorbShrine.GetComponent<tk2dSprite>().HeightOffGround = -0.7f;
            gorbShrine.GetComponent<tk2dSprite>().UpdateZDepth();
            ShrineGorb shrine = gorbShrine.gameObject.AddComponent<ShrineGorb>();
            SpecialItemModule.gorbShrine = gorbShrine;
            Transform transform = new GameObject("talkpoint").transform;
            transform.position = gorbShrine.transform.position + new Vector3(0.5f, 1f, 0f);
            transform.SetParent(gorbShrine.transform);
            shrine.talkPoint = transform;
            GameObject statue = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/ShrineGorb", new GameObject("GorbStatue"));
            statue.SetActive(false);
            FakePrefab.MarkAsFakePrefab(statue);
            UnityEngine.Object.DontDestroyOnLoad(statue);
            statue.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.shader = shader;
            statue.GetComponent<tk2dSprite>().GetCurrentSpriteDef().materialInst.shader = shader;
            statue.GetComponent<tk2dSprite>().HeightOffGround = 1.0f;
            statue.GetComponent<tk2dSprite>().UpdateZDepth();
            statue.transform.parent = gorbShrine.transform;
            ShrineGorb.spriteId = SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/ShrineGorbIcon", SpriteBuilder.itemCollection);
            shrine.statue = statue;
        }

        public static void InitAspidShrine()
        {
            GameObject shrineObj = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/ShrinePedestal", new GameObject("AspidShrine"));
            shrineObj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(shrineObj);
            UnityEngine.Object.DontDestroyOnLoad(shrineObj);
            Shader shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutout");
            shrineObj.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(0, 0), new IntVector2(26, 31)).PrimaryPixelCollider.CollisionLayer = CollisionLayer.HighObstacle;//31
            shrineObj.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.shader = shader;
            shrineObj.GetComponent<tk2dSprite>().GetCurrentSpriteDef().materialInst.shader = shader;
            shrineObj.GetComponent<tk2dSprite>().HeightOffGround = -0.7f;
            shrineObj.GetComponent<tk2dSprite>().UpdateZDepth();
            ShrinePrimalAspid shrine = shrineObj.gameObject.AddComponent<ShrinePrimalAspid>();
            SpecialItemModule.aspidShrine = shrineObj;
            Transform transform = new GameObject("talkpoint").transform;
            transform.position = shrineObj.transform.position + new Vector3(0.5f, 1f, 0f);
            transform.SetParent(shrineObj.transform);
            shrine.talkPoint = transform;
            GameObject statue = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/ShrinePrimalAspid", new GameObject("AspidStatue"));
            statue.SetActive(false);
            FakePrefab.MarkAsFakePrefab(statue);
            UnityEngine.Object.DontDestroyOnLoad(statue);
            statue.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.shader = shader;
            statue.GetComponent<tk2dSprite>().GetCurrentSpriteDef().materialInst.shader = shader;
            statue.GetComponent<tk2dSprite>().HeightOffGround = 1.0f;
            statue.GetComponent<tk2dSprite>().UpdateZDepth();
            statue.transform.parent = shrineObj.transform;
            shrine.statue = statue;
        }

        public static void InitSreaperShrine()
        {
            GameObject sreaperShrine = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/ShrinePedestal", new GameObject("SreaperShrine"));
            sreaperShrine.SetActive(false);
            FakePrefab.MarkAsFakePrefab(sreaperShrine);
            UnityEngine.Object.DontDestroyOnLoad(sreaperShrine);
            Shader shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutout");
            sreaperShrine.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(0, 0), new IntVector2(26, 31)).PrimaryPixelCollider.CollisionLayer = CollisionLayer.HighObstacle;//31
            sreaperShrine.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.shader = shader;
            sreaperShrine.GetComponent<tk2dSprite>().GetCurrentSpriteDef().materialInst.shader = shader;
            sreaperShrine.GetComponent<tk2dSprite>().HeightOffGround = -0.7f;
            sreaperShrine.GetComponent<tk2dSprite>().UpdateZDepth();
            SreaperShrine shrine = sreaperShrine.gameObject.AddComponent<SreaperShrine>();
            SpecialItemModule.sreaperShrine = sreaperShrine;
            Transform transform = new GameObject("talkpoint").transform;
            transform.position = sreaperShrine.transform.position + new Vector3(0.5f, 1f, 0f);
            transform.SetParent(sreaperShrine.transform);
            shrine.talkPoint = transform;
            GameObject stoneSreaper = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/ShrineSuperreaper", new GameObject("StoneSreaper"));
            stoneSreaper.SetActive(false);
            FakePrefab.MarkAsFakePrefab(stoneSreaper);
            UnityEngine.Object.DontDestroyOnLoad(stoneSreaper);
            stoneSreaper.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.shader = shader;
            stoneSreaper.GetComponent<tk2dSprite>().GetCurrentSpriteDef().materialInst.shader = shader;
            stoneSreaper.GetComponent<tk2dSprite>().HeightOffGround = 1.0f;
            stoneSreaper.GetComponent<tk2dSprite>().UpdateZDepth();
            SreaperShrine.spriteId = SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/ShrineSreaperIcon", SpriteBuilder.itemCollection);
            stoneSreaper.transform.parent = sreaperShrine.transform;
            shrine.stoneSreaper = stoneSreaper;
        }

        public static void InitShapeshifterShrine()
        {
            GameObject shapeshifterShrine = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/SrineShapeshifter", new GameObject("ShapeshifterShrine"));
            shapeshifterShrine.SetActive(false);
            FakePrefab.MarkAsFakePrefab(shapeshifterShrine);
            UnityEngine.Object.DontDestroyOnLoad(shapeshifterShrine);
            Shader shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutout");
            shapeshifterShrine.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(2, 0), new IntVector2(14, 20)).PrimaryPixelCollider.CollisionLayer = CollisionLayer.HighObstacle;//31
            shapeshifterShrine.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.shader = shader;
            shapeshifterShrine.GetComponent<tk2dSprite>().GetCurrentSpriteDef().materialInst.shader = shader;
            shapeshifterShrine.GetComponent<tk2dSprite>().HeightOffGround = -0.7f;
            shapeshifterShrine.GetComponent<tk2dSprite>().UpdateZDepth();
            ShrineShapeshifter shrine = shapeshifterShrine.gameObject.AddComponent<ShrineShapeshifter>();
            Transform transform = new GameObject("talkpoint").transform;
            transform.position = shapeshifterShrine.transform.position + new Vector3(0.5f, 1f, 0f);
            transform.SetParent(shapeshifterShrine.transform);
            shrine.talkPoint = transform;
            shrine.offset = new Vector2(48, 38);
            ShrineShapeshifter.spriteId = SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/NotificationIconShapeshifter", SpriteBuilder.itemCollection);
            Toolbox.RegisterBreachShrine("spapi:shapeshifter_shrine", shapeshifterShrine);
        }

        public static void InitRainbowmimicRewardPedestal()
        {
            GameObject pedestal = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/ThisPedestalIsEmpty", new GameObject("RainbowMimicRewardPedestal"));
            pedestal.SetActive(false);
            FakePrefab.MarkAsFakePrefab(pedestal);
            UnityEngine.Object.DontDestroyOnLoad(pedestal);
            Shader shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutout");
            pedestal.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(1, 0), new IntVector2(14, 20)).PrimaryPixelCollider.CollisionLayer = CollisionLayer.HighObstacle;//31
            pedestal.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.shader = shader;
            pedestal.GetComponent<tk2dSprite>().GetCurrentSpriteDef().materialInst.shader = shader;
            pedestal.GetComponent<tk2dSprite>().HeightOffGround = -1f;
            pedestal.GetComponent<tk2dSprite>().UpdateZDepth();
            RainbowmimicRewardPedestal shrine = pedestal.gameObject.AddComponent<RainbowmimicRewardPedestal>();
            Transform transform = new GameObject("talkpoint").transform;
            transform.position = pedestal.transform.position + new Vector3(0.5f, 1f, 0f);
            transform.SetParent(pedestal.transform);
            shrine.talkPoint = transform;
            shrine.offset = new Vector2(29.1875f, 32.5f);
            RainbowmimicRewardPedestal.rewardSpriteId = SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RainbowMimicRewardPedestal", SpriteBuilder.itemCollection);
            tk2dSpriteDefinition def = SpriteBuilder.itemCollection.spriteDefinitions[RainbowmimicRewardPedestal.rewardSpriteId];
            def.material.shader = shader;
            def.materialInst.shader = shader;
            Toolbox.RegisterBreachShrine("spapi:rainbowmimic_reward_pedestal", pedestal);
        }

        public static void InitHKPlaceable()
        {
            GameObject hkPlaceable = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/hk_chained_001", new GameObject("HKPlaceable")).ProcessGameObject();
            SpecialItemModule.hkPlaceable = hkPlaceable;
        }

        public static void InitScaryRedChamber()
        {
            GameObject scaryRedChamber = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/RedChamber_idle_001", new GameObject("ScaryRedChamber")).ProcessGameObject();
            List<int> spriteIds = new List<int>
            {
                scaryRedChamber.GetComponent<tk2dBaseSprite>().spriteId,
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_001", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_002", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_003", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_004", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_005", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_006", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_007", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_008", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_009", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_010", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_011", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_012", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_013", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_014", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_015", SpriteBuilder.itemCollection),
            };
            List<int> spriteIds2 = new List<int>
            {
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_016", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_017", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_018", SpriteBuilder.itemCollection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RedChamber_melt_019", SpriteBuilder.itemCollection),
            };
            tk2dSpriteAnimator animator = scaryRedChamber.AddComponent<tk2dSpriteAnimator>();
            animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animator.Library.clips = new tk2dSpriteAnimationClip[0];
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip { fps = 15, frames = new tk2dSpriteAnimationFrame[0], name = "melt", wrapMode = tk2dSpriteAnimationClip.WrapMode.Once };
            for (int i = 0; i < spriteIds.Count; i++)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = spriteIds[i], spriteCollection = SpriteBuilder.itemCollection };
                clip.frames = clip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library.clips = animator.Library.clips.Concat(new tk2dSpriteAnimationClip[] { clip }).ToArray();
            tk2dSpriteAnimationClip clip2 = new tk2dSpriteAnimationClip { fps = 15, frames = new tk2dSpriteAnimationFrame[0], name = "melt_more", wrapMode = tk2dSpriteAnimationClip.WrapMode.Once };
            for (int i = 0; i < spriteIds2.Count; i++)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = spriteIds2[i], spriteCollection = SpriteBuilder.itemCollection };
                clip2.frames = clip2.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library.clips = animator.Library.clips.Concat(new tk2dSpriteAnimationClip[] { clip2 }).ToArray();
            tk2dSpriteAnimationClip clip1 = new tk2dSpriteAnimationClip { fps = 15, frames = new tk2dSpriteAnimationFrame[0], name = "unmelt", wrapMode = tk2dSpriteAnimationClip.WrapMode.Once };
            for (int i = spriteIds2.Count - 1; i > -1; i--)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = spriteIds2[i], spriteCollection = SpriteBuilder.itemCollection };
                clip1.frames = clip1.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            for (int i = spriteIds.Count - 1; i > -1; i--)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = spriteIds[i], spriteCollection = SpriteBuilder.itemCollection };
                clip1.frames = clip1.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library.clips = animator.Library.clips.Concat(new tk2dSpriteAnimationClip[] { clip1 }).ToArray();
            scaryRedChamber.AddComponent<ScaryRedChamberBehaviour>();
            SpecialItemModule.scaryRedChamber = scaryRedChamber;
        }

        public static void InitVeryComfortableCarpet()
        {
            GameObject comfortableCarpet = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/ComfortableCarpet", new GameObject("ComfortableCarpet"));
            comfortableCarpet.SetActive(false);
            FakePrefab.MarkAsFakePrefab(comfortableCarpet);
            UnityEngine.Object.DontDestroyOnLoad(comfortableCarpet);
            Shader shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTiltedCutout");
            comfortableCarpet.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(0, 0), new IntVector2(110, 80)).CollideWithOthers = false;//31
            comfortableCarpet.GetComponent<tk2dSprite>().GetCurrentSpriteDef().material.shader = shader;
            comfortableCarpet.GetComponent<tk2dSprite>().GetCurrentSpriteDef().materialInst.shader = shader;
            comfortableCarpet.GetComponent<tk2dSprite>().HeightOffGround = -10f;
            comfortableCarpet.GetComponent<tk2dSprite>().UpdateZDepth();
            ComfortableCarpet carpet = comfortableCarpet.gameObject.AddComponent<ComfortableCarpet>();
            SpecialItemModule.comfortableCarpet = comfortableCarpet;
        }

        public static void Init()
        {
            var braveSETypes = new Type[]
            {
                typeof(string),
                typeof(string),
            };
            Hook braveLoad = new Hook(
                typeof(BraveResources).GetMethod("Load", BindingFlags.Public | BindingFlags.Static, null, braveSETypes, null),
                typeof(SpecialResources).GetMethod("GetSpecialResource")
            );
            InitSreaperShrine();
            InitGrimShrine();
            InitGorbShrine();
            InitAspidShrine();
            InitVeryComfortableCarpet();
            InitShapeshifterShrine();
            InitRainbowchestMimic();
            InitHKPlaceable();
            InitRainbowmimicRewardPedestal();
            InitScaryRedChamber();
        }

        public static UnityEngine.Object GetSpecialResource(Func<string, string, UnityEngine.Object> orig, string path, string extension = ".prefab")
        {
            if (resources.ContainsKey(path))
            {
                return resources[path];
            }
            return orig(path, extension);
        }

        public static Dictionary<string, UnityEngine.Object> resources = new Dictionary<string, UnityEngine.Object>();
    }
}
