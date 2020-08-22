using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using Gungeon;
using MonoMod.RuntimeDetour;
using SpecialItemPack.ItemAPI;
using FullInspector;

namespace SpecialItemPack
{
    class SpecialEnemies
    {
        public static void CreateRainbowchestMimicPrefab()
        {
            AIActor aiactor = UnityEngine.Object.Instantiate<AIActor>(EnemyDatabase.GetOrLoadByGuid("d8fd592b184b4ac9a3be217bc70912a2"));
            aiactor.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(aiactor.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(aiactor);
            aiactor.healthHaver.bossHealthBar = HealthHaver.BossBarType.MainBar;
            MeshRenderer component = aiactor.GetComponent<MeshRenderer>();
            Material[] sharedMaterials = component.sharedMaterials;
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = UnityEngine.Object.Instantiate<Material>(new Material(ShaderCache.Acquire("Brave/Internal/RainbowChestShader")));
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;
            aiactor.gameObject.AddComponent<RainbowmimicAwakenBehaviour>();
            foreach (AttackBehaviorBase attackBehav in aiactor.behaviorSpeculator.AttackBehaviors)
            {
                if (attackBehav is AttackBehaviorGroup)
                {
                    foreach (AttackBehaviorGroup.AttackGroupItem item in (attackBehav as AttackBehaviorGroup).AttackBehaviors)
                    {
                        if (item != null && item.Behavior != null)
                        {
                            if (item.Behavior is ShootBehavior)
                            {
                                (item.Behavior as ShootBehavior).BulletScript = new Toolbox.CustomBulletScriptSelector(typeof(MimicRainbowMiniguns1));
                            }
                        }
                    }
                }
            }
            if (aiactor.healthHaver != null)
            {
                aiactor.healthHaver.SetHealthMaximum(1000, null, true);
            }
            if (aiactor.bulletBank != null)
            {
                foreach (AIBulletBank.Entry entry in aiactor.bulletBank.Bullets)
                {
                    if (entry.Name == "default")
                    {
                        entry.BulletObject = EnemyDatabase.GetOrLoadByGuid("044a9f39712f456597b9762893fbc19c").bulletBank.bulletBank.GetBullet("gross").BulletObject;
                        entry.Name = "gross";
                    }
                    if (entry.Name == "bigBullet")
                    {
                        entry.BulletObject = EnemyDatabase.GetOrLoadByGuid("044a9f39712f456597b9762893fbc19c").bulletBank.bulletBank.GetBullet("gross").BulletObject;
                        entry.Name = "bigGross";
                    }
                }
                aiactor.bulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("044a9f39712f456597b9762893fbc19c").bulletBank.bulletBank.GetBullet("default"));
            }
            aiactor.ActorName = "Rainbow Chest Mimic";
            aiactor.EnemyGuid = "boss_rainbowchest_mimic";
            customEnemies.Add(aiactor);
            SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/BossRainbowmimicIcon", SpriteBuilder.ammonomiconCollection);
            if(aiactor.GetComponent<EncounterTrackable>() != null)
            {
                UnityEngine.Object.Destroy(aiactor.GetComponent<EncounterTrackable>());
            }
            aiactor.encounterTrackable = aiactor.gameObject.AddComponent<EncounterTrackable>();
            aiactor.encounterTrackable.journalData = new JournalEntry();
            aiactor.encounterTrackable.EncounterGuid = aiactor.EnemyGuid;
            aiactor.encounterTrackable.prerequisites = new DungeonPrerequisite[0];
            aiactor.encounterTrackable.journalData.SuppressKnownState = false;
            aiactor.encounterTrackable.journalData.IsEnemy = true;
            aiactor.encounterTrackable.journalData.SuppressInAmmonomicon = false;
            aiactor.encounterTrackable.ProxyEncounterGuid = "";
            aiactor.encounterTrackable.journalData.AmmonomiconSprite = "BossRainbowmimicIcon";
            aiactor.encounterTrackable.journalData.enemyPortraitSprite = ResourceExtractor.GetTextureFromResource("SpecialItemPack/Resources/AmmonomiconPortraitRainbowmimic.png");
            aiactor.OverrideDisplayName = "#SPECIAL_RAINBOWCHEST_MIMIC";
            SpecialItemModule.Strings.Enemies.Set("#SPECIAL_RAINBOWCHEST_MIMIC", "Rainbow Chest Mimic");
            SpecialItemModule.Strings.Enemies.Set("#SPECIAL_RAINBOWCHEST_MIMIC_SHORTDESC", "Deadly Surprise");
            SpecialItemModule.Strings.Enemies.Set("#SPECIAL_RAINBOWCHEST_MIMIC_LONGDESC", "Evolution gave this mimic ability to change colors. With that ability, it can mimic valuable rainbow chests, that attract foolish gungeoneers so much. " +
                "Their miniguns are filled with flak shells and they are tougher than other mimics. Truly a terrifying foe to face.");
            aiactor.encounterTrackable.journalData.PrimaryDisplayName = "#SPECIAL_RAINBOWCHEST_MIMIC";
            aiactor.encounterTrackable.journalData.NotificationPanelDescription = "#SPECIAL_RAINBOWCHEST_MIMIC_SHORTDESC";
            aiactor.encounterTrackable.journalData.AmmonomiconFullEntry = "#SPECIAL_RAINBOWCHEST_MIMIC_LONGDESC";
            AddEnemyToDatabase(aiactor.gameObject, "boss_rainbowchest_mimic", true, true, true);
            EnemyDatabase.GetEntry("boss_rainbowchest_mimic").ForcedPositionInAmmonomicon = 24;
            EnemyDatabase.GetEntry("boss_rainbowchest_mimic").isInBossTab = true;
        }

        public static AIActor SetupAIActorDummy(string name, IntVector2 colliderOffset, IntVector2 colliderDimensions)
        {
            GameObject obj = new GameObject(name);
            obj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(obj);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            tk2dSprite sprite = obj.AddComponent<tk2dSprite>();
            SpeculativeRigidbody body = sprite.SetUpSpeculativeRigidbody(colliderOffset, colliderDimensions);
            PixelCollider pixelCollider = new PixelCollider();
            pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            pixelCollider.CollisionLayer = CollisionLayer.EnemyCollider;
            pixelCollider.ManualWidth = colliderDimensions.x;
            pixelCollider.ManualHeight = colliderDimensions.y;
            pixelCollider.ManualOffsetX = colliderOffset.x;
            pixelCollider.ManualOffsetY = colliderOffset.y;
            body.PixelColliders.Add(pixelCollider);
            body.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyCollider;
            AIActor aiactor = obj.AddComponent<AIActor>();
            HealthHaver hh = obj.AddComponent<HealthHaver>();
            hh.SetHealthMaximum(10f);
            hh.ForceSetCurrentHealth(10f);
            BehaviorSpeculator spec = obj.AddComponent<BehaviorSpeculator>();
            ((ISerializedObject)spec).SerializedObjectReferences = new List<UnityEngine.Object>(0);
            ((ISerializedObject)spec).SerializedStateKeys = new List<string> { "OverrideBehaviors", "OtherBehaviors", "TargetBehaviors", "AttackBehaviors", "MovementBehaviors" };
            ((ISerializedObject)spec).SerializedStateValues = new List<string> { "", "", "", "", "" };
            return aiactor;
        }

        public static AIAnimator GenerateBlankAIAnimator(GameObject targetObject)
        {
            AIAnimator m_CachedAIAnimator = targetObject.AddComponent<AIAnimator>();
            m_CachedAIAnimator.facingType = AIAnimator.FacingType.Default;
            m_CachedAIAnimator.faceSouthWhenStopped = false;
            m_CachedAIAnimator.faceTargetWhenStopped = false;
            m_CachedAIAnimator.AnimatedFacingDirection = -90;
            m_CachedAIAnimator.directionalType = AIAnimator.DirectionalType.Sprite;
            m_CachedAIAnimator.RotationQuantizeTo = 0;
            m_CachedAIAnimator.RotationOffset = 0;
            m_CachedAIAnimator.ForceKillVfxOnPreDeath = false;
            m_CachedAIAnimator.SuppressAnimatorFallback = false;
            m_CachedAIAnimator.IsBodySprite = true;
            m_CachedAIAnimator.IdleAnimation = new DirectionalAnimation()
            {
                Type = DirectionalAnimation.DirectionType.None,
                Prefix = string.Empty,
                AnimNames = new string[0],
                Flipped = new DirectionalAnimation.FlipType[0]
            };
            m_CachedAIAnimator.MoveAnimation = new DirectionalAnimation()
            {
                Type = DirectionalAnimation.DirectionType.None,
                Prefix = string.Empty,
                AnimNames = new string[0],
                Flipped = new DirectionalAnimation.FlipType[0]
            };
            m_CachedAIAnimator.FlightAnimation = new DirectionalAnimation()
            {
                Type = DirectionalAnimation.DirectionType.None,
                Prefix = string.Empty,
                AnimNames = new string[0],
                Flipped = new DirectionalAnimation.FlipType[0]
            };
            m_CachedAIAnimator.HitAnimation = new DirectionalAnimation()
            {
                Type = DirectionalAnimation.DirectionType.None,
                Prefix = string.Empty,
                AnimNames = new string[0],
                Flipped = new DirectionalAnimation.FlipType[0]
            };
            m_CachedAIAnimator.TalkAnimation = new DirectionalAnimation()
            {
                Type = DirectionalAnimation.DirectionType.None,
                Prefix = string.Empty,
                AnimNames = new string[0],
                Flipped = new DirectionalAnimation.FlipType[0]
            };
            m_CachedAIAnimator.OtherAnimations = new List<AIAnimator.NamedDirectionalAnimation>(0);
            m_CachedAIAnimator.OtherVFX = new List<AIAnimator.NamedVFXPool>(0);
            m_CachedAIAnimator.OtherScreenShake = new List<AIAnimator.NamedScreenShake>(0);
            m_CachedAIAnimator.IdleFidgetAnimations = new List<DirectionalAnimation>(0);
            m_CachedAIAnimator.HitReactChance = 1;
            m_CachedAIAnimator.HitType = AIAnimator.HitStateType.Basic;
            return m_CachedAIAnimator;
        }

        public static void GenerateSpriteAnimator(GameObject targetObject, tk2dSpriteAnimation library = null, int DefaultClipId = 0, float AdditionalCameraVisibilityRadius = 0, bool AnimateDuringBossIntros = false, bool AlwaysIgnoreTimeScale = false, bool ignoreTimeScale = false, bool ForceSetEveryFrame = false, bool playAutomatically = false, bool IsFrameBlendedAnimation = false, float clipTime = 0, float ClipFps = 15, bool deferNextStartClip = false, bool alwaysUpdateOffscreen = false, bool maximumDeltaOneFrame = false)
        {

            if (targetObject.GetComponent<tk2dSpriteAnimator>()) { UnityEngine.Object.Destroy(targetObject.GetComponent<tk2dSpriteAnimator>()); }

            tk2dSpriteAnimator newAnimator = targetObject.AddComponent<tk2dSpriteAnimator>();
            newAnimator.Library = library;
            newAnimator.DefaultClipId = DefaultClipId;
            newAnimator.AdditionalCameraVisibilityRadius = AdditionalCameraVisibilityRadius;
            newAnimator.AnimateDuringBossIntros = AnimateDuringBossIntros;
            newAnimator.AlwaysIgnoreTimeScale = AlwaysIgnoreTimeScale;
            newAnimator.ignoreTimeScale = ignoreTimeScale;
            newAnimator.ForceSetEveryFrame = ForceSetEveryFrame;
            newAnimator.playAutomatically = playAutomatically;
            newAnimator.IsFrameBlendedAnimation = IsFrameBlendedAnimation;
            newAnimator.clipTime = clipTime;
            newAnimator.ClipFps = ClipFps;
            newAnimator.deferNextStartClip = deferNextStartClip;
            newAnimator.alwaysUpdateOffscreen = alwaysUpdateOffscreen;
            newAnimator.maximumDeltaOneFrame = maximumDeltaOneFrame;

            return;
        }

        public static void InitButterflyPrefab()
        {
            AIActor aiactor = SetupAIActorDummy("Butterfly", new IntVector2(0, 0), new IntVector2(10, 10));
            List<string> leftAnim = new List<string>
            {
                "SpecialItemPack/Resources/Butterfly/butterfly_idle_left_001",
                "SpecialItemPack/Resources/Butterfly/butterfly_idle_left_002",
                "SpecialItemPack/Resources/Butterfly/butterfly_idle_left_003"
            };
            List<string> rightAnim = new List<string>
            {
                "SpecialItemPack/Resources/Butterfly/butterfly_idle_left_001",
                "SpecialItemPack/Resources/Butterfly/butterfly_idle_left_002",
                "SpecialItemPack/Resources/Butterfly/butterfly_idle_left_003"
            };
            GenerateSpriteAnimator(aiactor.gameObject, null, 0, 0, playAutomatically: true, clipTime: 0, ClipFps: 0);
            tk2dSpriteAnimator spriteAnim = aiactor.gameObject.GetComponent<tk2dSpriteAnimator>();
            spriteAnim.playAutomatically = true;
            spriteAnim.Library = aiactor.gameObject.AddComponent<tk2dSpriteAnimation>();
            spriteAnim.Library.clips = new tk2dSpriteAnimationClip[0];
            spriteAnim.DefaultClipId = 0;
            tk2dSpriteAnimationClip leftClip = new tk2dSpriteAnimationClip { fps = 5, frames = new tk2dSpriteAnimationFrame[0], name = "idle_left" };
            foreach (string path in leftAnim)
            {
                int id = SpriteBuilder.AddSpriteToCollection(path, SpriteBuilder.itemCollection);
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = id, spriteCollection = SpriteBuilder.itemCollection };
                leftClip.frames = leftClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            aiactor.sprite.SetSprite(leftClip.frames[0].spriteCollection, leftClip.frames[0].spriteId);
            spriteAnim.Library.clips = spriteAnim.Library.clips.Concat(new tk2dSpriteAnimationClip[] { leftClip }).ToArray();
            tk2dSpriteAnimationClip rightClip = new tk2dSpriteAnimationClip { fps = 5, frames = new tk2dSpriteAnimationFrame[0], name = "idle_right" };
            foreach (string path in rightAnim)
            {
                int id = SpriteBuilder.AddSpriteToCollection(path, SpriteBuilder.itemCollection);
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = id, spriteCollection = SpriteBuilder.itemCollection };
                rightClip.frames = rightClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            spriteAnim.Library.clips = spriteAnim.Library.clips.Concat(new tk2dSpriteAnimationClip[] { rightClip }).ToArray();
            AIAnimator aiAnim = GenerateBlankAIAnimator(aiactor.gameObject);
            aiAnim.aiAnimator.facingType = AIAnimator.FacingType.Default;
            aiAnim.aiAnimator.faceSouthWhenStopped = false;
            aiAnim.aiAnimator.faceTargetWhenStopped = false;
            aiAnim.aiAnimator.HitType = AIAnimator.HitStateType.Basic;
            aiAnim.IdleAnimation = new DirectionalAnimation
            {
                Type = DirectionalAnimation.DirectionType.TwoWayHorizontal,
                Flipped = new DirectionalAnimation.FlipType[2],
                AnimNames = new string[]
                {
                    "idle_right",
                    "idle_left"
                }
            };
            aiAnim.IdleAnimation = aiAnim.MoveAnimation;
            BehaviorSpeculator chickSpec = EnemyDatabase.GetOrLoadByGuid("76bc43539fc24648bff4568c75c686d1").behaviorSpeculator;
            SpeculativeRigidbody body = aiactor.specRigidbody;
            body.PixelColliders.RemoveAt(1);
            body.PrimaryPixelCollider.CollisionLayer = CollisionLayer.BulletBlocker;
            BehaviorSpeculator spec = aiactor.behaviorSpeculator;
            spec.OverrideBehaviors = chickSpec.OverrideBehaviors;
            spec.OtherBehaviors = chickSpec.OtherBehaviors;
            spec.TargetBehaviors = chickSpec.TargetBehaviors;
            spec.AttackBehaviors = chickSpec.AttackBehaviors;
            spec.MovementBehaviors = chickSpec.MovementBehaviors;
            spec.InstantFirstTick = chickSpec.InstantFirstTick;
            spec.TickInterval = chickSpec.TickInterval;
            spec.PostAwakenDelay = chickSpec.PostAwakenDelay;
            spec.RemoveDelayOnReinforce = chickSpec.RemoveDelayOnReinforce;
            spec.OverrideStartingFacingDirection = chickSpec.OverrideStartingFacingDirection;
            spec.StartingFacingDirection = chickSpec.StartingFacingDirection;
            spec.SkipTimingDifferentiator = chickSpec.SkipTimingDifferentiator;
            aiactor.SetIsFlying(true, "butter fly", true, true);
            Game.Enemies.Add("spapi:butterfly", aiactor);
        }

        public static void Init()
        {
            CreateRainbowchestMimicPrefab();
            InitButterflyPrefab();
            Hook getorLoadByGuidHook = new Hook(
                typeof(EnemyDatabase).GetMethod("GetOrLoadByGuid", BindingFlags.Public | BindingFlags.Static),
                typeof(SpecialEnemies).GetMethod("GetOrLoadByGuidHook")
            );
            Hook getorLoadByNameHook = new Hook(
                typeof(EnemyDatabase).GetMethod("GetOrLoadByName", BindingFlags.Public | BindingFlags.Static),
                typeof(SpecialEnemies).GetMethod("GetOrLoadByNameHook")
            );
        }

        public static AIActor GetOrLoadByGuidHook(Func<string, AIActor> orig, string guid)
        {
            foreach(AIActor aiactor in customEnemies)
            {
                if(aiactor.EnemyGuid == guid)
                {
                    return aiactor;
                }
            }
            return orig(guid);
        }

        public static AIActor GetOrLoadByNameHook(Func<string, AIActor> orig, string name)
        {
            foreach (AIActor aiactor in customEnemies)
            {
                if (aiactor.ActorName == name)
                {
                    return aiactor;
                }
            }
            return orig(name);
        }

        public static void AddEnemyToDatabase(GameObject EnemyPrefab, string EnemyGUID, bool isInBossTab = false, bool IsNormalEnemy = true, bool AddToMTGSpawnPool = true)
        {
            EnemyDatabaseEntry item = new EnemyDatabaseEntry
            {
                myGuid = EnemyGUID,
                placeableWidth = 2,
                placeableHeight = 2,
                isNormalEnemy = IsNormalEnemy,
                path = EnemyGUID,
                isInBossTab = isInBossTab,
                encounterGuid = EnemyGUID
            };
            EnemyDatabase.Instance.Entries.Add(item);
            SpecialResources.resources.Add(EnemyGUID, EnemyPrefab);
            EncounterDatabaseEntry encounterDatabaseEntry = new EncounterDatabaseEntry(EnemyPrefab.GetComponent<AIActor>().encounterTrackable)
            {
                path = EnemyGUID,
                myGuid = EnemyPrefab.GetComponent<AIActor>().encounterTrackable.EncounterGuid
            };
            EncounterDatabase.Instance.Entries.Add(encounterDatabaseEntry);
            if (AddToMTGSpawnPool && !string.IsNullOrEmpty(EnemyPrefab.GetComponent<AIActor>().ActorName))
            {
                string EnemyName = "spapi:" + EnemyPrefab.GetComponent<AIActor>().ActorName.Replace(" ", "_").Replace("(", "_").Replace(")", string.Empty).ToLower();
                if (!Game.Enemies.ContainsID(EnemyName)) { Game.Enemies.Add(EnemyName, EnemyPrefab.GetComponent<AIActor>()); }
            }
        }

        public static List<AIActor> customEnemies = new List<AIActor>();

        public class RainbowmimicAwakenBehaviour : BraveBehaviour
        {
            private void Awake()
            {
                if(base.aiActor != null)
                {
                    base.aiActor.HasBeenEngaged = true;
                }
                if(GameManager.Instance.PrimaryPlayer != null)
                {
                    if(GameManager.Instance.PrimaryPlayer.CurrentRoom != base.aiActor.GetAbsoluteParentRoom())
                    {
                        GameManager.Instance.PrimaryPlayer.AttemptTeleportToRoom(base.aiActor.GetAbsoluteParentRoom(), true, false);
                        GameManager.Instance.PrimaryPlayer.ForceChangeRoom(base.aiActor.GetAbsoluteParentRoom());
                    }
                }
                base.aiActor.GetAbsoluteParentRoom().SealRoom();
                base.healthHaver.OnDeath += this.Die;
            }

            private void Die(Vector2 finalDamageDirection)
            {
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.BOSS_RAINBOWMIMIC_DEFEATED, true);
            }
        }
    }
}
