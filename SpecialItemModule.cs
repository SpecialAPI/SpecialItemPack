using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using Gungeon;
using Dungeonator;
using MonoMod.RuntimeDetour;
using UnityEngine;
using SpecialItemPack.AdaptedSynergyStuff;
using GungeonAPI;
using System.Net;
using System.IO;
using SpecialItemPack.Shapeshifting;
using FakePrefab = SpecialItemPack.ItemAPI.FakePrefab;
using ItemAPI = SpecialItemPack.ItemAPI;
using Ionic.Zip;

namespace SpecialItemPack
{
    public class SpecialItemModule : ETGModule
    {
        public override void Init()
        {
            AdvancedGameStatsManager.AdvancedGameSave = new SaveManager.SaveType
            {
                filePattern = "Slot{0}.advancedSave",
                encrypted = true,
                backupCount = 3,
                backupPattern = "Slot{0}.advancedBackup.{1}",
                backupMinTimeMin = 45,
                legacyFilePattern = "advancedGameStatsSlot{0}.txt"
            };
            for (int i = 0; i < 3; i++)
            {
                SaveManager.SaveSlot saveSlot = (SaveManager.SaveSlot)i;
                Toolbox.SafeMove(Path.Combine(SaveManager.OldSavePath, string.Format(AdvancedGameStatsManager.AdvancedGameSave.legacyFilePattern, saveSlot)), Path.Combine(SaveManager.OldSavePath,
                    string.Format(AdvancedGameStatsManager.AdvancedGameSave.filePattern, saveSlot)), false);
                Toolbox.SafeMove(Path.Combine(SaveManager.OldSavePath, string.Format(AdvancedGameStatsManager.AdvancedGameSave.filePattern, saveSlot)), Path.Combine(SaveManager.OldSavePath,
                    string.Format(AdvancedGameStatsManager.AdvancedGameSave.filePattern, saveSlot)), false);
                Toolbox.SafeMove(Toolbox.PathCombine(SaveManager.SavePath, "01", string.Format(AdvancedGameStatsManager.AdvancedGameSave.filePattern, saveSlot)), Path.Combine(SaveManager.SavePath, 
                    string.Format(AdvancedGameStatsManager.AdvancedGameSave.filePattern, saveSlot)), true);
            }
            Hook mainMenuAwakeHook = new Hook(
                typeof(MainMenuFoyerController).GetMethod("InitializeMainMenu", BindingFlags.Public | BindingFlags.Instance),
                typeof(SpecialItemModule).GetMethod("MainMenuAwakeHook")
            );
            Toolbox.specialeverything = this.LoadAssetBundleFromLiterallyAnywhere();
            AdvancedGameStatsManager.Init();
        }

        public AssetBundle LoadAssetBundleFromLiterallyAnywhere()
        {
            AssetBundle assetBundle = null;
            if (File.Exists(this.Metadata.Archive))
            {
                ZipFile ModZIP = ZipFile.Read(this.Metadata.Archive);
                if (ModZIP != null && ModZIP.Entries.Count > 0)
                {
                    foreach (ZipEntry entry in ModZIP.Entries)
                    {
                        if (entry.FileName == "specialeverything")
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                entry.Extract(ms);
                                ms.Seek(0, SeekOrigin.Begin);
                                assetBundle = AssetBundle.LoadFromStream(ms);
                                break;
                            }
                        }
                    }
                }
            }
            else if (File.Exists(this.Metadata.Directory + "/specialeverything"))
            {
                try 
                { 
                    assetBundle = AssetBundle.LoadFromFile(this.Metadata.Directory + "/specialeverything"); 
                } 
                catch (Exception ex) 
                {
                    Debug.LogError("Failed loading asset bundle from file.");
                    Debug.LogError(ex.ToString());
                }
            }
            else
            {
                Debug.LogError("AssetBundle NOT FOUND!");
            }
            return assetBundle;
        }

        public override void Start()
        {
            ItemAPI.FakePrefabHooks.Init();
            ItemAPI.ItemBuilder.Init();
            SpecialItemModule.Strings = new AdvancedStringDB();
            Toolbox.Init();
            Gun highDragunfire = PickupObjectDatabase.GetById(670) as Gun;
            highDragunfire.SetBaseMaxAmmo(1200);
            highDragunfire.GainAmmo(1200);
            highDragunfire.reloadTime = 0.8f;
            highDragunfire.DefaultModule.cooldownTime = 0.025f;
            Projectile proj = highDragunfire.DefaultModule.projectiles[0];
            proj.FireApplyChance = 100f;
            proj.ignoreDamageCaps = true;
            proj.baseData.damage = 10f;
            proj.baseData.force /= 3f;
            PierceProjModifier penetrateMod = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
            penetrateMod.penetratesBreakables = true;
            penetrateMod.penetration = 999;
            penetrateMod.preventPenetrationOfActors = false;
            penetrateMod.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;

            Toolbox.GetGunById(543).DefaultModule.projectiles[0].gameObject.AddComponent<IgnoreBehaviour>();

            EnemyDatabase.GetOrLoadByGuid("7c5d5f09911e49b78ae644d2b50ff3bf").gameObject.AddComponent<CustomInfinilichDeathBehaviour>();

            SpecialBlankModificationItem.InitHooks();
            MysteriousChest.Init();
            KeyOfChaosItem.Init();
            SynergracingBullets.Init();
            SprenThing.Init();
            GlassHeart.Init();
            GlassBullets.Init();
            GlassChamber.Init();
            QuantumBullets.Init();
            GungeonWizardRing.Init();
            PenetratingBullets.Init();
            InvisibleBullets.Init();
            AliveBullets.Init();
            GungeonCompass.Init();
            StormEyeController.Init();
            RagingBullets.Init();
            BigGunController.Init();
            HotDogController.Init();
            BowBowController.Init();
            WoodenGuon.Init();
            DiamondGuonStone.Init();
            MagentaGuonStone.Init();
            DarkGreenGuonStone.Init();
            PurpleGuonStone.Init();
            BlackGuonStone.Init();
            HalfMirrorGuon.Init();
            PhilosophersGuonStone.Init();
            AmmoFlower.Init();
            ModdedMagnetItem.InitGuns();
            ModdedMagnetItem.InitItems();
            PastkillersPlan.Init();
            UnluckyDice.Init();
            LichSlayer.Init();
            BigChamber.Init();
            PokeballItem.Init();
            //EnterTheGungeon.Init();
            RingOfAmmunition.Init();
            SequencerController.Init();
            ScrollOfWonder.Init();
            CarBattery.Init();
            UglyGun.Init();
            LockonBullets.Init();
            EboniteAmmolet.Init();
            LichsFavoriteController.Init();
            JunkSwordController.Init();
            CrimtaneAmmolet.Init();
            PointlessJunk.Init();
            CelebrationMk1.Init();
            ArmorHeartFriendship.Init();
            StoneJunk.Init();
            ShelletahsHeart.Init();
            Fireworks.Init();
            Relodelode.Init();
            MimigunController.Init();
            HowToThrowGuns.Init();
            SoulOrbController.Init();
            JunkansRevengeController.Init();
            ShieldOfGunkh.Init();
            JunkArmor.Init();
            AegisGunController.Init();
            TeslaCoil.Init();
            MagicBeans.Init();
            FlatBullets.Init();
            PeashooterSeeds.Init();
            MahogunySapling.Init();
            YourselfBullets.Init();
            MinigunStand.Init();
            CustomCreepyEyeController.Build();
            GreenChamberEyeController.Build();
            KaliberAffliction.Init();
            CustomSuperReaperController.Build();
            CrownOfTheJammed.Init();
            TakeableEnemy.Init();
            StraightBullets.Init();
            Muscle.Init();
            SuperReapersScythe.Init();
            RoundKing.Init();
            OcarinaOfTime.Init();
            CyberkineticSuperbattery.Init();
            JunkUp.Init();
            GiantsPlaytoyllets.Init();
            TrueHeroSwordController.Init();
            ReallySpecialGramophone.Init();
            HDGun.Init();
            Decagun.Init();
            RedirectRounds.Init();
            GreenChamberItem.Init();
            BlueChamberController.Init();
            ElevatorSwitch.Init();
            MasterChamber.Init();
            //InfinityController.Init();
            InfluxBullets.Init();
            IgnoreRounds.Init();
            TableTechChaos.Init();
            TableTechTableflip.Init();
            RedChamberGun.Init();
            SpecialGun.Init();
            BatLauncher.Init();
            SuperBatLauncher.Init();

            Hook getValueHook = new Hook(
                typeof(dfLanguageManager).GetMethod("GetValue", BindingFlags.Public | BindingFlags.Instance),
                typeof(SpecialItemModule).GetMethod("GetValueHook")
            );

            SpecialDungeon.Init();

            UglyChest.Init();
            SpecialResources.Init();

            WallOfFleshBehaviour.SetupObjects();

            PickupChamberReplacementBehaviour behav = PickupObjectDatabase.GetById(120).gameObject.AddComponent<PickupChamberReplacementBehaviour>();
            behav.setsSprite = true;
            behav.stopAnimation = true;
            behav.newSpriteId = ItemAPI.SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/ArmorChamber", ItemAPI.SpriteBuilder.itemCollection);
            behav.newCollection = ItemAPI.SpriteBuilder.itemCollection;

            PickupChamberReplacementBehaviour behav2 = PickupObjectDatabase.GetById(78).gameObject.AddComponent<PickupChamberReplacementBehaviour>();
            behav2.setsSprite = true;
            behav2.stopAnimation = true;
            behav2.newSpriteId = ItemAPI.SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/AmmoChamber", ItemAPI.SpriteBuilder.itemCollection);
            behav2.newCollection = ItemAPI.SpriteBuilder.itemCollection;

            PickupChamberReplacementBehaviour behav3 = PickupObjectDatabase.GetById(224).gameObject.AddComponent<PickupChamberReplacementBehaviour>();
            behav3.setsSprite = true;
            behav3.stopAnimation = true;
            behav3.newSpriteId = ItemAPI.SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/BlankChamber", ItemAPI.SpriteBuilder.itemCollection);
            behav3.newCollection = ItemAPI.SpriteBuilder.itemCollection;
            behav3.attemptToAddOutlineAfterCreation = true;

            PickupChamberReplacementBehaviour behav4 = PickupObjectDatabase.GetById(600).gameObject.AddComponent<PickupChamberReplacementBehaviour>();
            behav4.setsSprite = true;
            behav4.stopAnimation = true;
            behav4.newSpriteId = ItemAPI.SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/SpreadAmmoChamber", ItemAPI.SpriteBuilder.itemCollection);
            behav4.newCollection = ItemAPI.SpriteBuilder.itemCollection;

            PickupChamberReplacementBehaviour behav5 = PickupObjectDatabase.GetById(67).gameObject.AddComponent<PickupChamberReplacementBehaviour>();
            behav5.setsSprite = true;
            behav5.stopAnimation = true;
            behav5.newSpriteId = ItemAPI.SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/KeyChamber", ItemAPI.SpriteBuilder.itemCollection);
            behav5.newCollection = ItemAPI.SpriteBuilder.itemCollection;
            behav5.destroysSpriteAnimator = true;

            PickupChamberReplacementBehaviour behav6 = PickupObjectDatabase.GetById(85).gameObject.AddComponent<PickupChamberReplacementBehaviour>();
            behav6.setsSprite = true;
            behav6.stopAnimation = true;
            behav6.newSpriteId = ItemAPI.SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/HealthChamber", ItemAPI.SpriteBuilder.itemCollection);
            behav6.newCollection = ItemAPI.SpriteBuilder.itemCollection;
            behav6.overridesAnimations = true;
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip { name = "heart_big_teleport", fps = 10, frames = new tk2dSpriteAnimationFrame[0], wrapMode = tk2dSpriteAnimationClip.WrapMode.Once };
            behav6.overrideAnimations.Add("heart_big_teleport", clip);

            PickupChamberReplacementBehaviour behav7 = PickupObjectDatabase.GetById(73).gameObject.AddComponent<PickupChamberReplacementBehaviour>();
            behav7.setsSprite = true;
            behav7.stopAnimation = true;
            behav7.newSpriteId = ItemAPI.SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/HalfHealthChamber", ItemAPI.SpriteBuilder.itemCollection);
            behav7.newCollection = ItemAPI.SpriteBuilder.itemCollection;
            behav7.overridesAnimations = true;
            tk2dSpriteAnimationClip clip2 = new tk2dSpriteAnimationClip { name = "heart_big_teleport", fps = 10, frames = new tk2dSpriteAnimationFrame[0], wrapMode = tk2dSpriteAnimationClip.WrapMode.Once };
            behav7.overrideAnimations.Add("heart_small_teleport", clip2);

            UnlockGreenChamberBehaviour.BuildRewardPedestal();

            AdvancedDualWieldSynergyProcessor dualWieldController = Toolbox.GetGunById(601).gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            dualWieldController.SynergyNameToCheck = "#BIG_GUN_SHOTGUN-GUN";
            dualWieldController.PartnerGunID = ETGMod.Databases.Items["big_gun"].PickupObjectId;

            AdvancedDualWieldSynergyProcessor dualWieldController2 = Toolbox.GetGunById(380).gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            dualWieldController2.SynergyNameToCheck = "#SHIELD_BROS";
            dualWieldController2.PartnerGunID = ETGMod.Databases.Items["aegis"].PickupObjectId;

            Toolbox.AddDualWieldSynergyProcessor(Toolbox.GetModdedItemId("ugly_gun"), Toolbox.GetModdedItemId("hd"), "#THE_SUCCESS_AND_THE_FAILURE");

            Toolbox.GetGunById(122).gameObject.AddComponent<IPlantedYouSynergyProcessor>();

            AdvancedHoveringGunSynergyProcessor processor = Toolbox.GetGunById(365).gameObject.AddComponent<AdvancedHoveringGunSynergyProcessor>();
            processor.RequiredSynergy = "#MINE_TOO!";
            processor.TargetGunID = ETGMod.Databases.Items["jamm_scythe"].PickupObjectId;
            processor.UsesMultipleGuns = false;
            processor.PositionType = HoveringGunController.HoverPosition.CIRCULATE;
            processor.AimType = HoveringGunController.AimType.PLAYER_AIM;
            processor.FireType = HoveringGunController.FireType.ON_FIRED_GUN;
            processor.FireCooldown = 1f;
            processor.FireDuration = 0f;
            processor.OnlyOnEmptyReload = false;
            processor.ShootAudioEvent = "Play_ENM_cannonball_launch_01";
            processor.OnEveryShotAudioEvent = "";
            processor.FinishedShootingAudioEvent = "";
            processor.Trigger = AdvancedHoveringGunSynergyProcessor.TriggerStyle.CONSTANT;
            processor.NumToTrigger = 1;
            processor.TriggerDuration = 0f;
            processor.ConsumesTargetGunAmmo = false;
            processor.ChanceToConsumeTargetGunAmmo = 0f;

            AdvancedTransformGunSynergyProcessor synergyProcessor = ETGMod.Databases.Items["batlauncher"].gameObject.AddComponent<AdvancedTransformGunSynergyProcessor>();
            synergyProcessor.NonSynergyGunId = Toolbox.GetModdedItemId("batlauncher");
            synergyProcessor.SynergyGunId = Toolbox.GetModdedItemId("superbatlauncher");
            synergyProcessor.SynergyToCheck = "#KING_BULLAT_SHOOTER";

            /*foreach(string text in Directory.GetFiles(ETGMod.ModsDirectory))
            {
                string modId = "";
                foreach(char chr in Path.GetFileName(text))
                {
                    if(chr.ToString() != null)
                    {
                        string text2 = chr.ToString();
                        if(text2 == "_")
                        {
                            break;
                        }
                        else
                        {
                            modId += text2;
                        }
                    }
                }
                Application.OpenURL("https://modworkshop.net/mod/" + modId);
            }*/

            SpecialItemModule.Strings.Core.Set("#SHRINE_SREAPER_ACCEPT", "<Kneel at the Altar>");
            SpecialItemModule.Strings.Core.Set("#SHRINE_SREAPER_DECLINE", "<Walk away>");
            SpecialItemModule.Strings.Core.Set("#SHRINE_SREAPER_SPENT", "The spirits that once inhabited this shrine have departed.");
            SpecialItemModule.Strings.Core.Set("#SHRINE_SREAPER_TABLE", "A shrine to Lord of the Jammed, the sentinel beyond the Curtain.");
            SpecialItemModule.Strings.Core.Set("#SHRINE_SREAPER_HEADER", "Gift at a Cost");
            SpecialItemModule.Strings.Core.Set("#SHRINE_SREAPER_TEXT", "You feel unsafe.");

            SpecialItemModule.Strings.Core.Set("#SHRINE_GRIM_ACCEPT", "<Sacrifice your soul>");
            SpecialItemModule.Strings.Core.Set("#SHRINE_GRIM_DECLINE", "<Walk away>");
            SpecialItemModule.Strings.Core.Set("#SHRINE_GRIM_TABLE", "A shrine to Gunreapers, will of Kaliber incarnate. Perhaps, they can help you if you give them what they want?");
            SpecialItemModule.Strings.Core.Set("#SHRINE_GRIM_HEADER", "Sacrifice Accepted");
            SpecialItemModule.Strings.Core.Set("#SHRINE_GRIM_TEXT", "Friends with Gunreapers.");

            SpecialItemModule.Strings.Core.Set("#SHRINE_GORB_ACCEPT", "<Ascend! Ascend! Ascend ascend ascend with Gorb!>");
            SpecialItemModule.Strings.Core.Set("#SHRINE_GORB_DECLINE", "<Walk away>");
            SpecialItemModule.Strings.Core.Set("#SHRINE_GORB_TABLE", "A shrine to Gorb, the great mind. Bow! Bow bow bow bow to Gorb!");
            SpecialItemModule.Strings.Core.Set("#SHRINE_GORB_HEADER", "Ascended");
            SpecialItemModule.Strings.Core.Set("#SHRINE_GORB_TEXT", "Bow to Gorb.");

            SpecialItemModule.Strings.Core.Set("#SHRINE_ASPID_ACCEPT", "<Stay (No! Don't do that!)>");
            SpecialItemModule.Strings.Core.Set("#SHRINE_ASPID_DECLINE", "<Walk away (Yes, do that as fast as possible!)>");
            SpecialItemModule.Strings.Core.Set("#SHRINE_ASPID_TABLE", "A shrine to Primal Aspid God. For some reason, you don't want to be near it.");
            SpecialItemModule.Strings.Core.Set("#SHRINE_ASPID_SPENT", "The spirits that once inhabited this shrine have departed (phew).");
            SpecialItemModule.Strings.Core.Set("#SHRINE_ASPID_DECLINE_SPENT", "<Walk away in relief>");

            SpecialItemModule.Strings.Core.Set("#INTERACTABLE_CARPET_COMFORTABLE", "This carpet seems very comfortable.");
            SpecialItemModule.Strings.Core.Set("#INTERACTABLE_CARPET_NOTCOMFORTABLE", "This carpet doesn't seem that comfortable anymore.");
            SpecialItemModule.Strings.Core.Set("#INTERACTABLE_CARPET_ACCEPT", "<Lie down and rest>");
            SpecialItemModule.Strings.Core.Set("#INTERACTABLE_CARPET_DECLINE", "<Walk away>");


            SpecialItemModule.Strings.Core.Set("#SHRINE_SHAPESHIFTER_TABLET", "A shrine to the legendary Shapeshifter, gungeoneer that learned how to change forms.");
            SpecialItemModule.Strings.Core.Set("#SHRINE_SHAPESHIFTER_CLEAR", "<Become yourself again>");
            SpecialItemModule.Strings.Core.Set("#SHRINE_SHAPESHIFTER_DECLINE", "<Walk away>");
            SpecialItemModule.Strings.Core.Set("#SHRINE_SHAPESHIFTER_SHAPESHIFTED", "Shapeshifted");
            SpecialItemModule.Strings.Core.Set("#SHRINE_SHAPESHIFTER_CUR_SHAPESHIFT", "Current Shapeshift: ");
            SpecialItemModule.Strings.Core.Set("#SHRINE_SHAPESHIFTER_NORMAL_SHAPESHIFT", "Normal");

            SpecialItemModule.Strings.Enemies.Set("#ENEMY_PRIMALASPIDS_REVENGE", "Primal Aspid God's Revenge");

            SpecialItemModule.Strings.Core.Set("#RAINBOWMIMIC_REWARD_PEDESTAL_EMPTY", "This pedestal is empty.");
            SpecialItemModule.Strings.Core.Set("#RAINBOWMIMIC_REWARD_PEDESTAL", "The trophy of royal mimics.");

            SpecialItemModule.Strings.Core.Set("#SHAPESHIFT_KING_SLIME_ACCEPT", "<Become the ruler of the slimes>");
            SpecialItemModule.Strings.Core.Set("#SHAPESHIFT_KING_SLIME", "King Slime");
            SpecialItemModule.Strings.Core.Set("#SHAPESHIFT_WOF_ACCEPT", "<Become the hungry abomination>");
            SpecialItemModule.Strings.Core.Set("#SHAPESHIFT_WOF", "Wall of Flesh");

            ShrineShapeshifter.shapeshifts.Add(new Shapeshift { name = "#SHAPESHIFT_KING_SLIME", acceptKey = "#SHAPESHIFT_KING_SLIME_ACCEPT", behaviour = typeof(KingSlimeBehaviour) });
            ShrineShapeshifter.shapeshifts.Add(new Shapeshift { name = "#SHAPESHIFT_WOF", acceptKey = "#SHAPESHIFT_WOF_ACCEPT", behaviour = typeof(WallOfFleshBehaviour) });

            AdvancedCompanionSynergyProcessor companionSynergyProcessor = PickupObjectDatabase.GetById(580).gameObject.AddComponent<AdvancedCompanionSynergyProcessor>();
            companionSynergyProcessor.RequiredSynergy = "#BROTHERS_IN_ARMS";
            companionSynergyProcessor.RequiresNoSynergy = false;
            companionSynergyProcessor.PersistsOnDisable = false;
            companionSynergyProcessor.CompanionGuid = "c6c8e59d0f5d41969c74e802c9d67d07";
            companionSynergyProcessor.PreventRespawnOnFloorLoad = false;
            companionSynergyProcessor.ManuallyAssignedPlayer = null;

            ETGModConsole.Commands.AddGroup("spapi");
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("spawndragonchest", SpawnDragunChest);
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("mount_test", delegate (string[] args)
            {
                RoomHandler room = GameManager.Instance.PrimaryPlayer.CurrentRoom;
                if (room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                {
                    List<AIActor> activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    if (activeEnemies[0] != null && activeEnemies[1] != null)
                    {
                        activeEnemies[0].Mount(activeEnemies[1]);
                    }
                }
            });
            ETGModConsole.Commands.GetGroup("spawn").AddUnit("item", delegate (string[] args)
            {
                if (!ETGModConsole.ArgCount(args, 1, 2))
                {
                    return;
                }
                if (!GameManager.Instance.PrimaryPlayer)
                {
                    ETGModConsole.Log("Couldn't access Player Controller", false);
                    return;
                }
                string text = args[0];
                if (!Game.Items.ContainsID(text))
                {
                    ETGModConsole.Log(string.Format("Invalid item ID {0}!", text), false);
                    return;
                }
                ETGModConsole.Log(string.Concat(new object[]
                {
                    "Attempting to spawn item ID ",
                    args[0],
                    " (numeric ",
                    text,
                    "), class ",
                    Game.Items.Get(text).GetType()
                }), false);
                if (args.Length == 2)
                {
                    int num = int.Parse(args[1]);
                    for (int i = 0; i < num; i++)
                    {
                        Game.PrimaryPlayer.GiveItem(text);
                    }
                    return;
                }
                LootEngine.SpawnItem(Game.Items[text].gameObject, GameManager.Instance.PrimaryPlayer.sprite.WorldBottomCenter + Vector2.down, Vector2.down, 1f);
            }, new AutocompletionSettings(delegate (string input)
            {
                List<string> list = new List<string>();
                foreach (string text in Game.Items.IDs)
                {
                    if (text.AutocompletionMatch(input.ToLower()))
                    {
                        Console.WriteLine(string.Format("INPUT {0} KEY {1} MATCH!", input, text));
                        list.Add(text.Replace("gungeon:", ""));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("INPUT {0} KEY {1} NO MATCH!", input, text));
                    }
                }
                return list.ToArray();
            }));
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("invisible", delegate (string[] args)
            {
                GameManager.Instance.PrimaryPlayer.sprite.renderer.enabled = !GameManager.Instance.PrimaryPlayer.sprite.renderer.enabled;
                SpriteOutlineManager.ToggleOutlineRenderers(GameManager.Instance.PrimaryPlayer.sprite, GameManager.Instance.PrimaryPlayer.sprite.renderer.enabled);
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("lock_merchant", delegate (string[] args)
            {
                GameObject theBestNpc = RoomFactory.GetAnythingFromBundles("Merchant_Key");
                Toolbox.LogComponents(theBestNpc, true, true, false);
                ETGModConsole.Log("---------------------CHILDREN:-------------------");
                foreach (Component a in theBestNpc.GetComponentsInChildren<Component>())
                {
                    ETGModConsole.Log(a.gameObject.name + ": " + a.GetType().ToString());
                }
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("chamber", delegate (string[] args)
            {
                GameManager.Instance.LoadCustomLevel("spapi_chamber");
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("asset_bundle_objects", delegate (string[] args)
            {
                foreach(string str in Toolbox.specialeverything.GetAllAssetNames())
                {
                    ETGModConsole.Log(str);
                }
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("asset_exists", delegate (string[] args)
            {
                if(args.Length > 0)
                {
                    ETGModConsole.Log(args[0] + " isn't null: " + (Toolbox.specialeverything.LoadAsset(args[0]) != null));
                }
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("no_cooldowns", delegate (string[] args)
            {
                GameManager.Instance.PrimaryPlayer.CurrentItem.damageCooldown = -1f;
                GameManager.Instance.PrimaryPlayer.CurrentItem.timeCooldown = -1f;
                GameManager.Instance.PrimaryPlayer.CurrentItem.roomCooldown = -1;
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("synergies", delegate (string[] args)
            {
                foreach (AdvancedSynergyEntry entry in SynergiesHub.synergies)
                {
                    ETGModConsole.Log(entry.NameKey);
                }
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("sreaper", delegate (string[] args)
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(UnityEngine.Object.Instantiate(SpecialItemModule.sreaperShrine, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter,
                    Quaternion.identity).GetComponent<SreaperShrine>());
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("timescale", delegate (string[] args)
            {
                if (args.Length <= 0)
                {
                    ETGModConsole.Log("<color=#ff0000ff>spapi timescale [timescale]</color>: sets the timescale to a certain amount. Usage: <color=#ff0000ff>spapi timescale [timescale]</color>.");
                }
                else
                {
                    if (timeScaleChanger == null)
                    {
                        timeScaleChanger = new GameObject("TimeScaleChanger");
                        UnityEngine.Object.DontDestroyOnLoad(timeScaleChanger);
                    }
                    BraveTime.SetTimeScaleMultiplier(float.Parse(args[0]), timeScaleChanger);
                }
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("grim", delegate (string[] ar)
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(UnityEngine.Object.Instantiate(SpecialItemModule.grimShrine, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter,
                    Quaternion.identity).GetComponent<GrimShrine>());
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("slime", delegate (string[] args)
            {
                ShapeShiftBehaviour.AddToTarget(typeof(KingSlimeBehaviour), GameManager.Instance.PrimaryPlayer);
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("wall", delegate (string[] args)
            {
                ShapeShiftBehaviour.AddToTarget(typeof(WallOfFleshBehaviour), GameManager.Instance.PrimaryPlayer);
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("ascend", delegate (string[] args)
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(UnityEngine.Object.Instantiate(SpecialItemModule.gorbShrine, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter,
                    Quaternion.identity).GetComponent<ShrineGorb>());
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("aspid", delegate (string[] args)
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(UnityEngine.Object.Instantiate(SpecialItemModule.aspidShrine, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter,
                    Quaternion.identity).GetComponent<AdvancedCustomShrineController>());
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("red_chamber", delegate (string[] args)
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(UnityEngine.Object.Instantiate(SpecialItemModule.scaryRedChamber, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter,
                    Quaternion.identity).GetComponent<ScaryRedChamberBehaviour>());
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("carpet", delegate (string[] args)
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(UnityEngine.Object.Instantiate(SpecialItemModule.comfortableCarpet, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter,
                    Quaternion.identity).GetComponent<ComfortableCarpet>());
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("hk", delegate (string[] args)
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(UnityEngine.Object.Instantiate(SpecialItemModule.hkPlaceable, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter,
                    Quaternion.identity).GetComponent<IPlayerInteractable>());
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("chest", delegate (string[] args)
            {
                Chest.Spawn(SpecialItemModule.rainbowMimicChest, (GameManager.Instance.PrimaryPlayer.CenterPosition + new Vector2(1f, 0f)).ToIntVector2(VectorConversions.Round));
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("kill", delegate (string[] args)
            {
                GameManager.Instance.PrimaryPlayer.healthHaver.ApplyDamage(99999, Vector2.zero, "Forced Kill", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("mount_test2", delegate (string[] args)
            {
                RoomHandler room = GameManager.Instance.PrimaryPlayer.CurrentRoom;
                if (room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                {
                    List<AIActor> activeEnemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    AIActor previousAiactor = null;
                    foreach (AIActor aiactor in activeEnemies)
                    {
                        if (previousAiactor != null)
                        {
                            aiactor.Mount(previousAiactor, tk2dBaseSprite.Anchor.UpperCenter, tk2dBaseSprite.Anchor.LowerCenter);
                        }
                        previousAiactor = aiactor;
                    }
                }
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("lock", delegate (string[] args)
            {
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.CUSTOMFLAG_OPENED_BLACK_CHEST, false);
                ETGModConsole.Log(AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.CUSTOMFLAG_OPENED_BLACK_CHEST).ToString());
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("unlock", delegate (string[] args)
            {
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.CUSTOMFLAG_OPENED_BLACK_CHEST, true);
                ETGModConsole.Log(AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.CUSTOMFLAG_OPENED_BLACK_CHEST).ToString());
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("value", delegate (string[] args)
            {
                ETGModConsole.Log(AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.CUSTOMFLAG_OPENED_BLACK_CHEST).ToString());
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("prerequisite", delegate (string[] args)
            {
                ETGModConsole.Log(ETGMod.Databases.Items["Mysterious Chest"].encounterTrackable.prerequisites[0].CheckConditionsFulfilled().ToString());
                ETGModConsole.Log((ETGMod.Databases.Items["Mysterious Chest"].encounterTrackable.prerequisites[0] as AdvancedDungeonPrerequisite).CheckConditionsFulfilled().ToString());
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("enemynames", delegate (string[] args)
            {
                foreach (EnemyDatabaseEntry enemyDatabaseEntry in EnemyDatabase.Instance.Entries)
                {
                    ETGModConsole.Log(enemyDatabaseEntry.GetPrefab<AIActor>().name);
                }
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("customammotypes", CustomAmmoTypes);
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("fianlcustomammotypes", CustomFinalAmmoTypes);
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("getnumberforname", delegate (string[] args)
            {
                CreepyEyeController eye = UnityEngine.Object.Instantiate<CreepyEyeController>(((GameObject)BraveResources.Load("Global Prefabs/CreepyEye_Room", ".prefab")).GetComponentInChildren<CreepyEyeController>(),
                    GameManager.Instance.PrimaryPlayer.CenterPosition + new Vector2(0, 5), Quaternion.identity);//.GetComponentInChildren<CreepyEyeController>();
                Toolbox.LogComponents(eye.gameObject);
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("spawncustomcreepyeye", delegate (string[] args)
            {
                GameObject obj = UnityEngine.Object.Instantiate(CustomCreepyEyeController.eyePrefab, GameManager.Instance.PrimaryPlayer.CenterPosition + new Vector2(0, 5), Quaternion.identity);
                obj.GetComponent<CustomCreepyEyeController>().ChangeScale(0.5f);
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("spawncustomreaper", delegate (string[] args)
            {
                GameObject obj = UnityEngine.Object.Instantiate(CustomSuperReaperController.friendlyReaperPrefab, GameManager.Instance.PrimaryPlayer.CenterPosition + new Vector2(0, 5), Quaternion.identity);
                obj.GetComponent<CustomSuperReaperController>().BindWithOwner(GameManager.Instance.PrimaryPlayer);
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("changegunweight", delegate (string[] args)
            {
                if (args.Length <= 0)
                {
                    ETGModConsole.Log("Default value is 6.");
                }
                else
                {
                    SpecialItemModule.ModGunWeight = float.Parse(args[0]);
                    ETGModConsole.Log("Gun weight modifier was changed to: " + SpecialItemModule.ModGunWeight);
                }
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("changeitemweight", delegate (string[] args)
            {
                if (args.Length <= 0)
                {
                    ETGModConsole.Log("Default value is 3.");
                }
                else
                {
                    SpecialItemModule.ModItemWeight = float.Parse(args[0]);
                    ETGModConsole.Log("Item weight modifier was changed to: " + SpecialItemModule.ModItemWeight);
                }
            });
            ETGModConsole.Commands.GetGroup("spapi").AddUnit("sayiteminfo", this.ItemInfo, new AutocompletionSettings(delegate (string input)
            {
                List<string> list = new List<string>();
                foreach (string text in Game.Items.IDs)
                {
                    if (text.AutocompletionMatch(input.ToLower()))
                    {
                        Console.WriteLine(string.Format("INPUT {0} KEY {1} MATCH!", input, text));
                        list.Add(text.Replace("gungeon:", ""));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("INPUT {0} KEY {1} NO MATCH!", input, text));
                    }
                }
                return list.ToArray();
            }));
            Hook startDodgerollHook = new Hook(
                typeof(PlayerController).GetMethod("HandleStartDodgeRoll", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(SpecialItemModule).GetMethod("StartDodgeRollHook")
            );
            SynergiesHub.Init();
            SpecialEnemies.Init();
            GungeonAPI.GungeonAPI.Init();
            SpecialRooms.Init();
            ETGMod.StartGlobalCoroutine(this.DelayedStartCR());
            ETGModConsole.Log("SpecialAPI's Stuff " + Version + " initialized.");
        }

        public void DelayedStart()
        {
            SynergiesHub.AddModdedSynergies();
            GameObject obj = new GameObject("InfinityAmmoTypeSprite").ProcessGameObject();
            dfTiledSprite sprite = obj.AddComponent<dfTiledSprite>();
            sprite.Atlas = ((dfFont)typeof(UINotificationController).GetField("EnglishFont", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameUIRoot.Instance.notificationController)).Atlas;
            sprite.IsLocalized = false;
            sprite.Size = Vector2.zero;
            sprite.SpriteName = "infinite-big";
            GameUIAmmoType uiammotype = new GameUIAmmoType
            {
                ammoBarBG = sprite,
                ammoBarFG = sprite,
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = "infinity"
            };
            addedAmmoTypes.Add(uiammotype);
            foreach (GameUIAmmoController uiammocontroller in GameUIRoot.Instance.ammoControllers)
            {
                Toolbox.Add(ref uiammocontroller.ammoTypes, uiammotype);
            }
            Component componentToDestroy = null;
            bool isExpandComponent = false;
            foreach (Component component in PickupObjectDatabase.GetById(647).GetComponents<Component>())
            {
                if (component.GetType().AssemblyQualifiedName.Contains("ExpandChamberGunProcessor") || component is ChamberGunProcessor)
                {
                    componentToDestroy = component;
                    isExpandComponent = component.GetType().AssemblyQualifiedName.Contains("ExpandChamberGunProcessor");
                    break;
                }
            }
            SpecialChamberGunProcessor processor = PickupObjectDatabase.GetById(647).gameObject.AddComponent<SpecialChamberGunProcessor>();
            if (componentToDestroy != null)
            {
                if (isExpandComponent)
                {
                    try
                    {
                        Type type = componentToDestroy.GetType();
                        processor.CastleGunID = (int)type.GetField("CastleGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.OublietteGunID = (int)type.GetField("OublietteGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.GungeonGunID = (int)type.GetField("GungeonGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.AbbeyGunID = (int)type.GetField("AbbeyGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.MinesGunID = (int)type.GetField("MinesGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.RatgeonGunID = (int)type.GetField("RatgeonGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.HollowGunID = (int)type.GetField("HollowGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.OfficeGunID = (int)type.GetField("OfficeGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.ForgeGunID = (int)type.GetField("ForgeGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.HellGunID = (int)type.GetField("HellGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.PhobosGunID = (int)type.GetField("PhobosGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.OldWestGunID = (int)type.GetField("OldWestGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.JungleGunID = (int)type.GetField("JungleGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.BellyGunID = (int)type.GetField("BellyGunID", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.RefillsOnFloorChange = (bool)type.GetField("RefillsOnFloorChange", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                        processor.JustActiveReloaded = (bool)type.GetField("JustActiveReloaded", BindingFlags.Public | BindingFlags.Instance).GetValue(componentToDestroy);
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError("Failed setting fields.");
                        Debug.LogError(ex.ToString());
                        processor.CastleGunID = 647;
                        processor.OublietteGunID = 657;
                        processor.GungeonGunID = 660;
                        processor.AbbeyGunID = 806;
                        processor.MinesGunID = 807;
                        processor.RatgeonGunID = 808;
                        processor.HollowGunID = 659;
                        processor.OfficeGunID = 823;
                        processor.ForgeGunID = 658;
                        processor.HellGunID = 763;
                        processor.PhobosGunID = 734;
                        processor.OldWestGunID = 734; 
                        processor.JungleGunID = 368;
                        processor.BellyGunID = 734;
                        processor.RefillsOnFloorChange = true;
                        processor.JustActiveReloaded = false;
                    }
                }
                else if(componentToDestroy is ChamberGunProcessor)
                {
                    ChamberGunProcessor processor2 = componentToDestroy as ChamberGunProcessor;
                    processor.CastleGunID = processor2.CastleGunID;
                    processor.OublietteGunID = processor2.OublietteGunID;
                    processor.GungeonGunID = processor2.GungeonGunID;
                    processor.AbbeyGunID = processor2.AbbeyGunID;
                    processor.MinesGunID = processor2.MinesGunID;
                    processor.RatgeonGunID = processor2.RatgeonGunID;
                    processor.HollowGunID = processor2.HollowGunID;
                    processor.OfficeGunID = processor2.OfficeGunID;
                    processor.ForgeGunID = processor2.OfficeGunID;
                    processor.HellGunID = processor2.HellGunID;
                    processor.RefillsOnFloorChange = processor2.RefillsOnFloorChange;
                    processor.JustActiveReloaded = processor2.JustActiveReloaded;
                }
                else
                {
                    Debug.Log("ChamberGunProcessor is an unknown component. Setting fields to default ones...");
                    processor.CastleGunID = 647;
                    processor.OublietteGunID = 657;
                    processor.GungeonGunID = 660;
                    processor.AbbeyGunID = 806;
                    processor.MinesGunID = 807;
                    processor.RatgeonGunID = 808;
                    processor.HollowGunID = 659;
                    processor.OfficeGunID = 823;
                    processor.ForgeGunID = 658;
                    processor.HellGunID = 763;
                    processor.RefillsOnFloorChange = true;
                    processor.JustActiveReloaded = false;
                }
                UnityEngine.Object.Destroy(componentToDestroy);
            }
            else
            {
                Debug.Log("Didn't find any ChamberGun components. Setting fields to default ones...");
                processor.CastleGunID = 647;
                processor.OublietteGunID = 657;
                processor.GungeonGunID = 660;
                processor.AbbeyGunID = 806;
                processor.MinesGunID = 807;
                processor.RatgeonGunID = 808;
                processor.HollowGunID = 659;
                processor.OfficeGunID = 823;
                processor.ForgeGunID = 658;
                processor.HellGunID = 763;
                processor.RefillsOnFloorChange = true;
                processor.JustActiveReloaded = false;
            }
            processor.ChamberGunID = SpecialItemIds.RedChamber;
            ETGModConsole.Log("SpecialAPI's Stuff " + Version + " fully initialized.");
        }

        public IEnumerator DelayedStartCR()
        {
            yield return null;
            this.DelayedStart();
            yield break;
        }

        public static bool StartDodgeRollHook(Func<PlayerController, Vector2, bool> orig, PlayerController self, Vector2 dir)
        {
            if(self.GetComponent<KingSlimeBehaviour>() != null)
            {
                return false;
            }
            return orig(self, dir);
        }

        public static void MainMenuAwakeHook(Action<MainMenuFoyerController> orig, MainMenuFoyerController self)
        {
            orig(self);
            self.VersionLabel.Text = self.VersionLabel.Text + " | " + Version;
        }

        public static string GetValueHook(Func<dfLanguageManager, string, string> orig, dfLanguageManager self, string key)
        {
            if (key.StartsWith("#SPAPI"))
            {
                return StringTableManager.GetString(key);
            }
            return orig(self, key);
        }

        public static AmmonomiconPokedexEntry.EncounterState CheckEncounterState(EncounterTrackable encounterTrackableOriginal)
        {
            AmmonomiconPokedexEntry.EncounterState state;
            EncounterDatabaseEntry encounterTrackable = EncounterDatabase.GetEntry(encounterTrackableOriginal.EncounterGuid);
            if (GameStatsManager.Instance.QueryEncounterable(encounterTrackable) == 0)
            {
                if (encounterTrackable.PrerequisitesMet() && !encounterTrackable.journalData.SuppressKnownState && !encounterTrackable.journalData.IsEnemy)
                {
                    state = AmmonomiconPokedexEntry.EncounterState.KNOWN;
                }
                else
                {
                    state = AmmonomiconPokedexEntry.EncounterState.UNKNOWN;
                }
            }
            else if (encounterTrackable.PrerequisitesMet())
            {
                state = AmmonomiconPokedexEntry.EncounterState.ENCOUNTERED;
            }
            else
            {
                state = AmmonomiconPokedexEntry.EncounterState.UNKNOWN;
            }
            return state;
        }

        public void SpawnDragunChest(string[] args)
        {
            Chest dragunchest = Chest.Spawn(GameManager.Instance.RewardManager.A_Chest, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter.ToIntVector2());
            dragunchest.forceContentIds = new List<int>{ 670 };
        }

        public void CustomFinalAmmoTypes(string[] args)
        {
            List<string> d = new List<string>();
            Dictionary<string, List<Gun>> b = new Dictionary<string, List<Gun>>();
            for (int i = 0; i < ETGMod.Databases.Items.Count; i++)
            {
                Gun gun = ETGMod.Databases.Items[i] as Gun;
                if (gun != null)
                {
                    if (gun.DefaultModule.ammoType == GameUIAmmoType.AmmoType.CUSTOM)
                    {
                        if (!d.Contains(gun.DefaultModule.finalCustomAmmoType))
                        {
                            d.Add(gun.DefaultModule.finalCustomAmmoType);
                            b.Add(gun.DefaultModule.finalCustomAmmoType, new List<Gun>());
                        }
                        b[gun.DefaultModule.finalCustomAmmoType].Add(gun);
                    }
                }
            }
            foreach (string v in d)
            {
                string text = "<color=#ff0000>" + v + "</color> -";
                bool first = true;
                foreach (Gun gun2 in b[v])
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        text = text + ",";
                    }
                    text = text + " " + gun2.DisplayName;
                }
                ETGModConsole.Log(text);
            }
        }

        public void CustomAmmoTypes(string[] args)
        {
            List<string> d = new List<string>();
            Dictionary<string, List<Gun>> b = new Dictionary<string, List<Gun>>();
            for (int i = 0; i < ETGMod.Databases.Items.Count; i++)
            {
                Gun gun = ETGMod.Databases.Items[i] as Gun;
                if (gun != null)
                {
                    if(gun.DefaultModule.ammoType == GameUIAmmoType.AmmoType.CUSTOM)
                    {
                        if (!d.Contains(gun.DefaultModule.customAmmoType))
                        {
                            d.Add(gun.DefaultModule.customAmmoType);
                            b.Add(gun.DefaultModule.customAmmoType, new List<Gun>());
                        }
                        b[gun.DefaultModule.customAmmoType].Add(gun);
                    }
                }
            }
            foreach (string v in d)
            {
                string text = "<color=#ff0000>" + v + "</color> -";
                bool first = true;
                foreach(Gun gun2 in b[v])
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        text = text + ",";
                    }
                    text = text + " " + gun2.DisplayName;
                }
                ETGModConsole.Log(text);
            }
        }

        private void ItemInfo(string[] args)
        {
            PickupObject pickupObject = Game.Items.Get(args[0]);
            if (pickupObject == null)
            {
                ETGModConsole.Log("The item " + args[0] + " does not exist.");
                return;
            }
            else if (pickupObject is PassiveItem)
            {
                PassiveItem passive = pickupObject as PassiveItem;
                ETGModConsole.Log(passive.DisplayName + ", " + passive.quality.ToString() + " teir passive.");
                ETGModConsole.Log("Game name: " + passive.name);
                ETGModConsole.Log("Encounter trackable name: " + passive.encounterTrackable.name);
                ETGModConsole.Log("Class: " + passive.GetType());
            }
            else if (pickupObject is PlayerItem)
            {
                PlayerItem active = pickupObject as PlayerItem;
                ETGModConsole.Log(active.DisplayName + ", " + active.quality.ToString() + " teir active.");
                ETGModConsole.Log("Game name: " + active.name);
                ETGModConsole.Log("Encounter trackable name: " + active.encounterTrackable.name);
                ETGModConsole.Log("Class: " + active.GetType());
                ETGModConsole.Log("Cooldowns: timed - " + active.timeCooldown + ", damage - " + active.damageCooldown + ", room clear - " + active.roomCooldown);
            }
            else if (pickupObject is Gun)
            {
                Gun gun = pickupObject as Gun;
                ETGModConsole.Log(gun.DisplayName + ", " + gun.quality.ToString() + " teir gun.");
                ETGModConsole.Log("Game name: " + gun.name);
                ETGModConsole.Log("Encounter trackable name: " + gun.encounterTrackable.name);
                string idleName = gun.idleAnimation;
                if (idleName == null)
                {
                    idleName = "null";
                }
                string fireName = gun.shootAnimation;
                if (fireName == null)
                {
                    fireName = "null";
                }
                string reloadName = gun.reloadAnimation;
                if (reloadName == null)
                {
                    reloadName = "null";
                }
                ETGModConsole.Log("Base animation names: idle - " + idleName + ", reload - " + reloadName + ", shoot - " + fireName);
                string introName = gun.introAnimation;
                if (introName == null)
                {
                    introName = "null";
                }
                string outName = gun.outOfAmmoAnimation;
                if (outName == null)
                {
                    outName = "null";
                }
                string chargeName = gun.chargeAnimation;
                if (chargeName == null)
                {
                    chargeName = "null";
                }
                string disName = gun.dischargeAnimation;
                if (disName == null)
                {
                    disName = "null";
                }
                string criticName = gun.criticalFireAnimation;
                if (criticName == null)
                {
                    criticName = "null";
                }
                string emptyName = gun.emptyAnimation;
                if (emptyName == null)
                {
                    emptyName = "null";
                }
                string emptyReloadName = gun.emptyReloadAnimation;
                if (emptyReloadName == null)
                {
                    emptyReloadName = "null";
                }
                string finalName = gun.finalShootAnimation;
                if (finalName == null)
                {
                    finalName = "null";
                }
                ETGModConsole.Log("Advanced animation names: intro - " + introName + ", out of ammo - " + outName + ", charge - " + chargeName + ", discharge - " + disName + ", ciritcal shot - " + criticName + ", empty - " + emptyName + ", empty reload - " + emptyReloadName + ", final shot - " + finalName);
                string altIdleName = gun.alternateIdleAnimation;
                if (altIdleName == null)
                {
                    altIdleName = "null";
                }
                string altFireName = gun.alternateShootAnimation;
                if (altFireName == null)
                {
                    altFireName = "null";
                }
                string altReloadName = gun.alternateReloadAnimation;
                if (altReloadName == null)
                {
                    altReloadName = "null";
                }
                ETGModConsole.Log("Alt animation names: idle - " + altIdleName + ", shoot - " + altFireName + ", reload - " + altReloadName);
                ETGModConsole.Log("GunSwitchGroup: " + gun.gunSwitchGroup);
            }
        }

        public override void Exit()
        {
        }

        public static Chest rainbowMimicChest;
        public static GameObject sreaperShrine;
        public static GameObject gorbShrine;
        public static GameObject grimShrine;
        public static GameObject aspidShrine;
        public static GameObject comfortableCarpet;
        public static GameObject hkPlaceable;
        public static GameObject scaryRedChamber;
        public static GameObject timeScaleChanger;
        public static float ModGunWeight = 6;
        public static float ModItemWeight = 3;
        public static AdvancedStringDB Strings;
        public static string Version = "v3.0";
        public static List<GameUIAmmoType> addedAmmoTypes = new List<GameUIAmmoType>();
        public static Dictionary<StringTableManager.GungeonSupportedLanguages, string> convictNames = new Dictionary<StringTableManager.GungeonSupportedLanguages, string>();
        public static readonly List<string> alphabet = new List<string>
        {
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g",
            "h",
            "i",
            "j",
            "k",
            "l",
            "m",
            "n",
            "o",
            "p",
            "q",
            "r",
            "s",
            "t",
            "u",
            "v",
            "w",
            "x",
            "y",
            "z"
        };

        public class BecomeRainbowmimicBehaviour : BraveBehaviour
        {
            public void Start()
            {
                typeof(Chest).GetField("m_isMimic", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(base.GetComponent<Chest>(), true);
                if (base.gameObject.activeInHierarchy)
                {
                    base.StartCoroutine((IEnumerator)typeof(Chest).GetMethod("MimicIdleAnimCR", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(base.GetComponent<Chest>(), new object[0]));
                }
            }
        }

        public class CustomInfinilichDeathBehaviour : BraveBehaviour
        {
            public void Start()
            {
                base.healthHaver.ManualDeathHandling = true;
                base.healthHaver.OnPreDeath += this.OnBossDeath;
                base.healthHaver.SuppressContinuousKillCamBulletDestruction = true;
            }

            private void OnBossDeath(Vector2 direction)
            {
            }
        }
    }
}
