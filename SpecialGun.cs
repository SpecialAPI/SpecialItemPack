using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using Dungeonator;

namespace SpecialItemPack
{
    class SpecialGun
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("SpecialGUN", "special_gun");
            Game.Items.Rename("outdated_gun_mods:specialgun", "spapi:specialgun");
            GunExt.SetShortDescription(gun, "GunAPI When?");
            GunExt.SetLongDescription(gun, "Gun made by a sentient golden lock. Smites all things unworthy of living in Gungeon.\n\nThe word \"BAD\" can be heard from inside.");
            GunExt.SetupSprite(gun, null, "special_gun_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 11);
            GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 11);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            AdvancedGunBehaviour behav = gun.gameObject.AddComponent<AdvancedGunBehaviour>();
            behav.preventNormalReloadAudio = true;
            behav.overrideNormalReloadAudio = "Play_WPN_magnum_reload_01";
            gun.gunSwitchGroup = "BeholsterEye";
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            SpecialGunProjectile projectile = Toolbox.CopyFields<SpecialGunProjectile>(UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(35) as Gun).DefaultModule.projectiles[0]));
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.transform.parent = gun.barrelOffset;
            projectile.baseData.damage = 10f;
            projectile.baseData.speed = 20f;
            gun.carryPixelOffset += new IntVector2(1, 0);
            gun.DefaultModule.cooldownTime = 0.15f;
            gun.DefaultModule.numberOfShotsInClip = 7;
            gun.reloadTime = 0.78f;
            gun.SetBaseMaxAmmo(100);
            gun.muzzleFlashEffects = Toolbox.GetGunById(53).muzzleFlashEffects;
            gun.quality = PickupObject.ItemQuality.S;
            gun.barrelOffset.transform.localPosition = new Vector3(1.7f, 0.65f, 0f);
            gun.encounterTrackable.EncounterGuid = "special_gun";
            gun.gunClass = GunClass.PISTOL;
            int index = 0;
            foreach (tk2dSpriteAnimationFrame frame in gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation).frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                if (def != null)
                {
                    def.MakeOffset(offsets[0][index]);
                }
                index++;
            }
            index = 0;
            foreach (tk2dSpriteAnimationFrame frame in gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                if (def != null)
                {
                    def.MakeOffset(offsets[1][index]);
                }
                index++;
            }
            index = 0;
            foreach (tk2dSpriteAnimationFrame frame in gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                if (def != null)
                {
                    def.MakeOffset(offsets[2][index]);
                }
                index++;
            }
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation).frames = new tk2dSpriteAnimationFrame[0];
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames = new tk2dSpriteAnimationFrame[0];
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames = new tk2dSpriteAnimationFrame[0];
            index = 0;
            foreach (string name in spriteNames[0])
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = ETGMod.Databases.Items.WeaponCollection.GetSpriteIdByName(name), spriteCollection = ETGMod.Databases.Items.WeaponCollection };
                Toolbox.Add(ref gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation).frames, frame);
                index++;
            }
            index = 0;
            foreach (string name in spriteNames[1])
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = ETGMod.Databases.Items.WeaponCollection.GetSpriteIdByName(name), spriteCollection = ETGMod.Databases.Items.WeaponCollection };
                Toolbox.Add(ref gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames, frame);
                index++;
            }
            index = 0;
            foreach (string name in spriteNames[2])
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = ETGMod.Databases.Items.WeaponCollection.GetSpriteIdByName(name), spriteCollection = ETGMod.Databases.Items.WeaponCollection };
                Toolbox.Add(ref gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames, frame);
                index++;
            }
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[3].eventAudio = "Play_WPN_SAA_spin_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[3].triggerEvent = true;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToTrorkShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(22);
            GameObject obj = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/rad_vfx_bad_intro_001", new GameObject("SmiteVFX")).ProcessGameObject();
            tk2dSpriteAnimator animator = obj.AddComponent<tk2dSpriteAnimator>();
            animator.Library = obj.AddComponent<tk2dSpriteAnimation>();
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip { name = "idle", fps = 7, frames = new tk2dSpriteAnimationFrame[0], wrapMode = tk2dSpriteAnimationClip.WrapMode.Once };
            foreach(string spritePath in smiteVFXSpritePaths)
            {
                int id = SpriteBuilder.AddSpriteToCollection(spritePath, SpriteBuilder.itemCollection);
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = id, spriteCollection = SpriteBuilder.itemCollection };
                frame.FrameToDefinition().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, frame.FrameToDefinition().position3);
                Toolbox.Add(ref clip.frames, frame);
            }
            animator.Library.clips = new tk2dSpriteAnimationClip[] { clip };
            animator.playAutomatically = true;
            animator.DefaultClipId = animator.GetClipIdByName("idle");
            SpriteAnimatorKiller kill = animator.gameObject.AddComponent<SpriteAnimatorKiller>();
            kill.fadeTime = -1f;
            kill.animator = animator;
            kill.delayDestructionTime = -1f;
            SmiteVFX = obj;
            GameObject obj2 = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/rad_vfx_bad_intro_001", new GameObject("SmiteVFX")).ProcessGameObject();
            tk2dSpriteAnimator animator2 = obj2.AddComponent<tk2dSpriteAnimator>();
            animator2.Library = obj2.AddComponent<tk2dSpriteAnimation>();
            tk2dSpriteAnimationClip clip2 = new tk2dSpriteAnimationClip { name = "idle", fps = 7, frames = new tk2dSpriteAnimationFrame[0], wrapMode = tk2dSpriteAnimationClip.WrapMode.Once };
            foreach (string spritePath in notSmiteVFXSpritePaths)
            {
                int id = SpriteBuilder.AddSpriteToCollection(spritePath, SpriteBuilder.itemCollection);
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = id, spriteCollection = SpriteBuilder.itemCollection };
                frame.FrameToDefinition().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, frame.FrameToDefinition().position3);
                Toolbox.Add(ref clip2.frames, frame);
            }
            animator2.Library.clips = new tk2dSpriteAnimationClip[] { clip2 };
            animator2.playAutomatically = true;
            animator2.DefaultClipId = animator2.GetClipIdByName("idle");
            SpriteAnimatorKiller kill2 = animator2.gameObject.AddComponent<SpriteAnimatorKiller>();
            kill2.fadeTime = -1f;
            kill2.animator = animator2;
            kill2.delayDestructionTime = -1f;
            NotSmiteVFX = obj2;
        }

        public static List<List<Vector3>> offsets = new List<List<Vector3>>
        {
            new List<Vector3> 
            { 
                new Vector3(0f, 0f) 
            },
            new List<Vector3>
            {
                new Vector3(-0.0625f, 0f),
                new Vector3(-0.0625f, 0.125f),
                new Vector3(0.0625f, -0.0625f),
                new Vector3(0f, 0f)
            },
            new List<Vector3>
            {
                new Vector3(-0.125f, 0.0625f),
                new Vector3(0f, -0.1875f),
                new Vector3(0f, -0.25f),
                new Vector3(0.0625f, -0.25f),
                new Vector3(0.0625f, -0.125f),
                new Vector3(0.0625f, 0.0f),
                new Vector3(0.0625f, -0.125f),
                new Vector3(0.0625f, -0.0625f),
            }
        };

        public static List<List<string>> spriteNames = new List<List<string>>
        {
            new List<string>
            {
                "special_gun_idle_001"
            },
            new List<string>
            {
                "special_gun_fire_001",
                "special_gun_fire_002",
                "special_gun_fire_003",
                "special_gun_fire_004",
                "special_gun_idle_001"
            },
            new List<string>
            {
                "special_gun_reload_001",
                "special_gun_reload_002",
                "special_gun_reload_003",
                "special_gun_reload_004",
                "special_gun_reload_005",
                "special_gun_reload_006",
                "special_gun_reload_007",
                "special_gun_reload_008",
                "special_gun_reload_002",
                "special_gun_reload_003",
                "special_gun_fire_004",
                "special_gun_idle_001"
            }
        };

        public static List<string> smiteVFXSpritePaths = new List<string>
        {
            "SpecialItemPack/Resources/rad_vfx_bad_intro_001",
            "SpecialItemPack/Resources/rad_vfx_bad_intro_002",
            "SpecialItemPack/Resources/rad_vfx_bad_001",
            "SpecialItemPack/Resources/rad_vfx_bad_002",
            "SpecialItemPack/Resources/rad_vfx_bad_001",
            "SpecialItemPack/Resources/rad_vfx_bad_002",
            "SpecialItemPack/Resources/rad_vfx_bad_outro_001",
            "SpecialItemPack/Resources/rad_vfx_bad_outro_002",
            "SpecialItemPack/Resources/rad_vfx_bad_outro_003",
            "SpecialItemPack/Resources/rad_vfx_bad_outro_004",
        };

        public static List<string> notSmiteVFXSpritePaths = new List<string>
        {
            "SpecialItemPack/Resources/rad_vfx_bad_intro_001",
            "SpecialItemPack/Resources/rad_vfx_bad_intro_002",
            "SpecialItemPack/Resources/rad_vfx_good_001",
            "SpecialItemPack/Resources/rad_vfx_good_002",
            "SpecialItemPack/Resources/rad_vfx_good_001",
            "SpecialItemPack/Resources/rad_vfx_good_002",
            "SpecialItemPack/Resources/rad_vfx_bad_outro_001",
            "SpecialItemPack/Resources/rad_vfx_bad_outro_002",
            "SpecialItemPack/Resources/rad_vfx_bad_outro_003",
            "SpecialItemPack/Resources/rad_vfx_bad_outro_004",
        };

        public static GameObject SmiteVFX;
        public static GameObject NotSmiteVFX;

        public class SpecialGunProjectile : Projectile
        {
            public override void Start()
            {
                base.Start();
            }

            protected override void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
            {
                if (otherRigidbody != null && otherRigidbody.aiActor != null && BadGuids.Contains(otherRigidbody.aiActor.EnemyGuid))
                {
                    GameManager.Instance.StartCoroutine(this.HandleEnemyDeath(otherRigidbody.aiActor));
                }
                else if (otherRigidbody != null && otherRigidbody.aiActor != null && GoodGuids.Contains(otherRigidbody.aiActor.EnemyGuid))
                {
                    tk2dBaseSprite sprite = Instantiate(NotSmiteVFX, otherRigidbody.aiActor.CenterPosition, Quaternion.identity).GetComponent<tk2dBaseSprite>();
                    sprite.HeightOffGround = 20f;
                    sprite.UpdateZDepth();
                    string guid = otherRigidbody.aiActor.EnemyGuid;
                    Vector2 pos = otherRigidbody.transform.position;
                    bool isBlackPhantom = otherRigidbody.aiActor.IsBlackPhantom;
                    otherRigidbody.aiActor.EraseFromExistence(true);
                    AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
                    GameObject gameObject = Instantiate(orLoadByGuid.gameObject, pos, Quaternion.identity);
                    CompanionController orAddComponent = gameObject.GetOrAddComponent<CompanionController>();
                    if (isBlackPhantom)
                    {
                        gameObject.GetComponent<AIActor>().BecomeBlackPhantom();
                    }
                    orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
                    orAddComponent.Initialize(this.Owner as PlayerController);
                    orAddComponent.behaviorSpeculator.MovementBehaviors.Add(new CompanionFollowPlayerBehavior());
                    AIActor aiactor = gameObject.GetComponent<AIActor>();
                    if (orAddComponent.healthHaver != null)
                    {
                        orAddComponent.healthHaver.PreventAllDamage = true;
                    }
                    if (orAddComponent.bulletBank != null)
                    {
                        orAddComponent.bulletBank.OnProjectileCreated += PokeballItem.CatchProjectileBehaviour.OnPostProcessProjectile;
                    }
                    if (orAddComponent.aiShooter != null)
                    {
                        orAddComponent.aiShooter.PostProcessProjectile += PokeballItem.CatchProjectileBehaviour.OnPostProcessProjectile;
                    }
                    List<Tuple<int, MirrorImageBehavior>> toReplace = new List<Tuple<int, MirrorImageBehavior>>();
                    Dictionary<int, Dictionary<int, MirrorImageBehavior>> toReplace2 = new Dictionary<int, Dictionary<int, MirrorImageBehavior>>();
                    int i = 0;
                    ETGModConsole.Log("ADDING...");
                    foreach(AttackBehaviorBase behav in aiactor.behaviorSpeculator.AttackBehaviors)
                    {
                        ETGModConsole.Log(behav.GetType().ToString());
                        if(behav is MirrorImageBehavior)
                        {
                            toReplace.Add(new Tuple<int, MirrorImageBehavior>(i, behav as MirrorImageBehavior));
                            ETGModConsole.Log("ADDED A MIRRORIMAGEBEHAVIOUR WITH AN INDEX " + i);
                        }
                        else if(behav is AttackBehaviorGroup)
                        {
                            Dictionary<int, MirrorImageBehavior> list = new Dictionary<int, MirrorImageBehavior>();
                            int j = 0;
                            foreach(AttackBehaviorGroup.AttackGroupItem item in (behav as AttackBehaviorGroup).AttackBehaviors)
                            {
                                if(item.Behavior is MirrorImageBehavior)
                                {
                                    list.Add(j, item.Behavior as MirrorImageBehavior);
                                }
                                j++;
                            }
                            toReplace2.Add(i, list);
                            ETGModConsole.Log("ADDED SOMETHING COMPLEX WITH AN INDEX " + i);
                        }
                        i++;
                    }
                    ETGModConsole.Log("REPLACING...");
                    foreach(Tuple<int, MirrorImageBehavior> tuple in toReplace)
                    {
                        aiactor.behaviorSpeculator.AttackBehaviors[tuple.First] = new FriendlyMirrorImageBehavior
                        {
                            NumImages = tuple.Second.NumImages,
                            MaxImages = tuple.Second.MaxImages,
                            MirrorHealth = tuple.Second.MirrorHealth,
                            SpawnDelay = tuple.Second.SpawnDelay,
                            SplitDelay = tuple.Second.SplitDelay,
                            SplitDistance = tuple.Second.SplitDistance,
                            Anim = tuple.Second.Anim,
                            AnimRequiresTransparency = tuple.Second.AnimRequiresTransparency,
                            MirrorDeathAnim = tuple.Second.MirrorDeathAnim,
                            MirroredAnims = tuple.Second.MirroredAnims,
                            AccumulateHealthThresholds = tuple.Second.AccumulateHealthThresholds,
                            AttackCooldown = tuple.Second.AttackCooldown,
                            Cooldown = tuple.Second.Cooldown,
                            CooldownVariance = tuple.Second.CooldownVariance,
                            GlobalCooldown = tuple.Second.GlobalCooldown,
                            GroupCooldown = tuple.Second.GroupCooldown,
                            GroupName = tuple.Second.GroupName,
                            HealthThresholds = tuple.Second.HealthThresholds,
                            InitialCooldown = tuple.Second.InitialCooldown,
                            InitialCooldownVariance = tuple.Second.InitialCooldownVariance,
                            IsBlackPhantom = tuple.Second.IsBlackPhantom,
                            MaxEnemiesInRoom = tuple.Second.MaxEnemiesInRoom,
                            MaxHealthThreshold = tuple.Second.MaxHealthThreshold,
                            MaxUsages = tuple.Second.MaxUsages,
                            MinHealthThreshold = tuple.Second.MinHealthThreshold,
                            MinRange = tuple.Second.MinRange,
                            MinWallDistance = tuple.Second.MinWallDistance,
                            Range = tuple.Second.Range,
                            RequiresLineOfSight = tuple.Second.RequiresLineOfSight,
                            resetCooldownOnDamage = tuple.Second.resetCooldownOnDamage,
                            targetAreaStyle = tuple.Second.targetAreaStyle
                        };
                        ETGModConsole.Log("REPLACED ATTACK BEHAVIOUR #" + tuple.First + " WITH A FRIENDLYMIRRORIMAGEBEHAVIOUR.");
                    }
                    foreach(KeyValuePair<int, Dictionary<int, MirrorImageBehavior>> valuePair in toReplace2)
                    {
                        foreach(KeyValuePair<int, MirrorImageBehavior> valuePair2 in valuePair.Value)
                        {
                            (aiactor.behaviorSpeculator.AttackBehaviors[valuePair.Key] as AttackBehaviorGroup).AttackBehaviors[valuePair2.Key].Behavior = new FriendlyMirrorImageBehavior
                            {
                                NumImages = valuePair2.Value.NumImages,
                                MaxImages = valuePair2.Value.MaxImages,
                                MirrorHealth = valuePair2.Value.MirrorHealth,
                                SpawnDelay = valuePair2.Value.SpawnDelay,
                                SplitDelay = valuePair2.Value.SplitDelay,
                                SplitDistance = valuePair2.Value.SplitDistance,
                                Anim = valuePair2.Value.Anim,
                                AnimRequiresTransparency = valuePair2.Value.AnimRequiresTransparency,
                                MirrorDeathAnim = valuePair2.Value.MirrorDeathAnim,
                                MirroredAnims = valuePair2.Value.MirroredAnims,
                                AccumulateHealthThresholds = valuePair2.Value.AccumulateHealthThresholds,
                                AttackCooldown = valuePair2.Value.AttackCooldown,
                                Cooldown = valuePair2.Value.Cooldown,
                                CooldownVariance = valuePair2.Value.CooldownVariance,
                                GlobalCooldown = valuePair2.Value.GlobalCooldown,
                                GroupCooldown = valuePair2.Value.GroupCooldown,
                                GroupName = valuePair2.Value.GroupName,
                                HealthThresholds = valuePair2.Value.HealthThresholds,
                                InitialCooldown = valuePair2.Value.InitialCooldown,
                                InitialCooldownVariance = valuePair2.Value.InitialCooldownVariance,
                                IsBlackPhantom = valuePair2.Value.IsBlackPhantom,
                                MaxEnemiesInRoom = valuePair2.Value.MaxEnemiesInRoom,
                                MaxHealthThreshold = valuePair2.Value.MaxHealthThreshold,
                                MaxUsages = valuePair2.Value.MaxUsages,
                                MinHealthThreshold = valuePair2.Value.MinHealthThreshold,
                                MinRange = valuePair2.Value.MinRange,
                                MinWallDistance = valuePair2.Value.MinWallDistance,
                                Range = valuePair2.Value.Range,
                                RequiresLineOfSight = valuePair2.Value.RequiresLineOfSight,
                                resetCooldownOnDamage = valuePair2.Value.resetCooldownOnDamage,
                                targetAreaStyle = valuePair2.Value.targetAreaStyle
                            };
                        }
                    }
                    AdditionalCompanionOwner additionalCompanionOwner = this.Owner.gameObject.AddComponent<AdditionalCompanionOwner>();
                    additionalCompanionOwner.CompanionGuid = guid;
                    additionalCompanionOwner.IsBlackPhantom = isBlackPhantom;
                    PhysicsEngine.SkipCollision = true;
                    return;
                }
                base.OnPreCollision(myRigidbody, myCollider, otherRigidbody, otherCollider);
            }

            private IEnumerator HandleEnemyDeath(AIActor target)
            {
                tk2dBaseSprite sprite = Instantiate(SmiteVFX, target.CenterPosition, Quaternion.identity).GetComponent<tk2dBaseSprite>();
                sprite.HeightOffGround = 20f;
                sprite.UpdateZDepth();
                target.EraseFromExistenceWithRewards(false);
                Transform copyTransform = this.CreateEmptySprite(target);
                tk2dSprite copySprite = copyTransform.GetComponentInChildren<tk2dSprite>();
                GameObject gameObject = Instantiate(Toolbox.GetGunById(519).alternateVolley.projectiles[0].projectiles[0].GetComponent<CombineEvaporateEffect>().ParticleSystemToSpawn, 
                    copySprite.WorldCenter.ToVector3ZisY(0f), Quaternion.identity);
                gameObject.transform.parent = copyTransform;
                float elapsed = 0f;
                float duration = 2.5f;
                copySprite.renderer.material.DisableKeyword("TINTING_OFF");
                copySprite.renderer.material.EnableKeyword("TINTING_ON");
                copySprite.renderer.material.DisableKeyword("EMISSIVE_OFF");
                copySprite.renderer.material.EnableKeyword("EMISSIVE_ON");
                copySprite.renderer.material.DisableKeyword("BRIGHTNESS_CLAMP_ON");
                copySprite.renderer.material.EnableKeyword("BRIGHTNESS_CLAMP_OFF");
                copySprite.renderer.material.SetFloat("_EmissiveThresholdSensitivity", 5f);
                copySprite.renderer.material.SetFloat("_EmissiveColorPower", 1f);
                int emId = Shader.PropertyToID("_EmissivePower");
                while (elapsed < duration)
                {
                    elapsed += BraveTime.DeltaTime;
                    float t = elapsed / duration;
                    copySprite.renderer.material.SetFloat(emId, Mathf.Lerp(1f, 10f, t));
                    copySprite.renderer.material.SetFloat("_BurnAmount", t);
                    copyTransform.position += Vector3.up * BraveTime.DeltaTime * 1f;
                    yield return null;
                }
                Destroy(copyTransform.gameObject);
                yield break;
            }

            private Transform CreateEmptySprite(AIActor target)
            {
                GameObject gameObject = new GameObject("suck image");
                gameObject.layer = target.gameObject.layer;
                tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
                gameObject.transform.parent = SpawnManager.Instance.VFX;
                tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
                tk2dSprite.transform.position = target.sprite.transform.position;
                GameObject gameObject2 = new GameObject("image parent");
                gameObject2.transform.position = tk2dSprite.WorldCenter;
                tk2dSprite.transform.parent = gameObject2.transform;
                tk2dSprite.usesOverrideMaterial = true;
                bool flag = target.optionalPalette != null;
                bool flag2 = flag;
                if (flag2)
                {
                    tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
                }
                bool flag3 = tk2dSprite.renderer.material.shader.name.Contains("ColorEmissive");
                bool flag4 = flag3;
                if (flag4)
                {
                }
                return gameObject2.transform;
            }

            private static readonly List<string> BadGuids = new List<string>
            {
                "fc809bd43a4d41738a62d7565456622c",
                "57255ed50ee24794b7aac1ac3cfb8a95",
                "1cec0cdf383e42b19920787798353e46",
                "8b913eea3d174184be1af362d441910d",
                "6450d20137994881aff0ddd13e3d40c8",
                "465da2bb086a4a88a803f79fe3a27677",
                "05b8afe0b6cc4fffa9dc6036fa24c8ec",
                "9d50684ce2c044e880878e86dbada919",
                "699cd24270af4cd183d671090d8323a1"
            };

            private static readonly List<string> GoodGuids = new List<string>
            {
                "98fdf153a4dd4d51bf0bafe43f3c77ff",
                "a9cc6a4e9b3d46ea871e70a03c9f77d4",
                "d4f4405e0ff34ab483966fd177f2ece3",
                "534f1159e7cf4f6aa00aeea92459065e",
            };
        }
    }
}
