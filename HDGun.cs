using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using Dungeonator;
using SpecialItemPack.AdaptedSynergyStuff;

namespace SpecialItemPack
{
    class HDGun : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("High-Quality Gun", "hd");
            Game.Items.Rename("outdated_gun_mods:high-quality_gun", "spapi:high_quality_gun");
            gun.gameObject.AddComponent<HDGun>();
            GunExt.SetShortDescription(gun, "HD!");
            GunExt.SetLongDescription(gun, "Bullets charm enemies.\n\nThis gun is just magnum, but but has 4 times more pixels. Because of that, it seems very weird among all other guns, but at the same time looks kinda cool!");
            GunExt.SetupSprite(gun, null, "hd_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 13);
            GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 12);
            GunExt.AddProjectileModuleFrom(gun, Toolbox.GetGunById(38), true, false);
            gun.gunSwitchGroup = "Magnum";
            VFXPool pool = new VFXPool();
            pool.type = VFXPoolType.All;
            VFXComplex complex = new VFXComplex();
            VFXObject vfObj = new VFXObject();
            GameObject obj = new GameObject("hd_smoke");
            obj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(obj);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            tk2dSprite sprite = obj.AddComponent<tk2dSprite>();
            tk2dSpriteAnimator animator = obj.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
            GameObject origEffect = Toolbox.GetGunById(38).muzzleFlashEffects.effects[0].effects[0].effect;
            VFXObject origVfEffect = Toolbox.GetGunById(38).muzzleFlashEffects.effects[0].effects[0];
            clip.fps = origEffect.GetAnyComponent<tk2dSpriteAnimator>().DefaultClip.fps;
            clip.frames = new tk2dSpriteAnimationFrame[0];
            for(int i = 1; i < 4; i++)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteId = Toolbox.VFXCollection.GetSpriteIdByName("hd_smoke_00" + i);
                Toolbox.VFXCollection.inst.spriteDefinitions[frame.spriteId] = Toolbox.VFXCollection.inst.spriteDefinitions[origEffect.GetAnyComponent<tk2dSpriteAnimator>().DefaultClip.frames[i - 1].spriteId].CopyDefinitionFrom();
                frame.spriteCollection = Toolbox.VFXCollection;
                clip.frames = clip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            clip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            clip.name = "start";
            animator.spriteAnimator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animator.spriteAnimator.Library.clips = new tk2dSpriteAnimationClip[] { clip };
            animator.playAutomatically = true;
            animator.DefaultClipId = animator.GetClipIdByName("start");
            SpriteAnimatorKiller kill = animator.gameObject.AddComponent<SpriteAnimatorKiller>();
            kill.fadeTime = -1f;
            kill.animator = animator;
            kill.delayDestructionTime = -1f;
            obj.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
            vfObj.orphaned = origVfEffect.orphaned;
            vfObj.attached = origVfEffect.attached;
            vfObj.persistsOnDeath = origVfEffect.persistsOnDeath;
            vfObj.usesZHeight = origVfEffect.usesZHeight;
            vfObj.zHeight = origVfEffect.zHeight;
            vfObj.alignment = origVfEffect.alignment;
            vfObj.destructible = origVfEffect.destructible;
            vfObj.effect = obj;
            VFXObject vfObj2 = new VFXObject();
            GameObject obj2 = new GameObject("hd_flare");
            obj2.SetActive(false);
            FakePrefab.MarkAsFakePrefab(obj2);
            UnityEngine.Object.DontDestroyOnLoad(obj2);
            tk2dSprite sprite2 = obj2.AddComponent<tk2dSprite>();
            tk2dSpriteAnimator animator2 = obj2.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip clip2 = new tk2dSpriteAnimationClip();
            GameObject origEffect2 = Toolbox.GetGunById(38).muzzleFlashEffects.effects[0].effects[1].effect;
            VFXObject origVfEffect2 = Toolbox.GetGunById(38).muzzleFlashEffects.effects[0].effects[1];
            clip2.fps = origEffect2.GetAnyComponent<tk2dSpriteAnimator>().DefaultClip.fps;
            clip2.frames = new tk2dSpriteAnimationFrame[0];
            for (int i = 1; i < 6; i++)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteId = Toolbox.VFXCollection.GetSpriteIdByName("hd_flare_00" + i);
                Toolbox.VFXCollection.inst.spriteDefinitions[frame.spriteId] = Toolbox.VFXCollection.inst.spriteDefinitions[origEffect2.GetAnyComponent<tk2dSpriteAnimator>().DefaultClip.frames[i - 1].spriteId].CopyDefinitionFrom();
                frame.spriteCollection = Toolbox.VFXCollection;
                clip2.frames = clip2.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            clip2.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            clip2.name = "start";
            animator2.spriteAnimator.Library = animator2.gameObject.AddComponent<tk2dSpriteAnimation>();
            animator2.spriteAnimator.Library.clips = new tk2dSpriteAnimationClip[] { clip2 };
            animator2.playAutomatically = true;
            animator2.DefaultClipId = animator2.GetClipIdByName("start");
            SpriteAnimatorKiller kill2 = animator2.gameObject.AddComponent<SpriteAnimatorKiller>();
            kill2.fadeTime = -1f;
            kill2.animator = animator2;
            kill2.delayDestructionTime = -1f;
            obj2.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
            vfObj2.orphaned = origVfEffect2.orphaned;
            vfObj2.attached = origVfEffect2.attached;
            vfObj2.persistsOnDeath = origVfEffect2.persistsOnDeath;
            vfObj2.usesZHeight = origVfEffect2.usesZHeight;
            vfObj2.zHeight = origVfEffect2.zHeight;
            vfObj2.alignment = origVfEffect2.alignment;
            vfObj2.destructible = origVfEffect2.destructible;
            vfObj2.effect = obj2;
            complex.effects = new VFXObject[] { vfObj, vfObj2 };
            pool.effects = new VFXComplex[] { complex };
            gun.muzzleFlashEffects = pool;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(38) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            gun.OverrideAngleSnap = null;
            projectile.transform.parent = gun.barrelOffset;
            projectile.AppliesCharm = true;
            projectile.CharmApplyChance = 100f;
            projectile.baseData.damage = 20f;
            projectile.charmEffect = (PickupObjectDatabase.GetById(527) as BulletStatusEffectItem).CharmModifierEffect;
            projectile.GetAnySprite().spriteId = ETGMod.Databases.Items.ProjectileCollection.inst.GetSpriteIdByName("hd_bullet_001");
            ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[projectile.GetAnySprite().spriteId] = 
                ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[(PickupObjectDatabase.GetById(38) as Gun).DefaultModule.projectiles[0].GetAnySprite().spriteId].CopyDefinitionFrom();
            gun.DefaultModule.numberOfShotsInClip = 8;
            projectile.name = "Charmingly_HD_Projectile";
            gun.reloadTime = 1.0f;
            gun.SetBaseMaxAmmo(280);
            gun.quality = PickupObject.ItemQuality.S;
            gun.encounterTrackable.EncounterGuid = "hd";
            gun.gunClass = GunClass.PISTOL;
            GameObject shellCasing = UnityEngine.Object.Instantiate(Toolbox.GetGunById(38).shellCasing);
            shellCasing.SetActive(false);
            FakePrefab.MarkAsFakePrefab(shellCasing);
            tk2dSpriteAnimator gunAnim = gun.GetComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip reloadClip = gunAnim.GetClipByName(gun.reloadAnimation);
            reloadClip.frames = new tk2dSpriteAnimationFrame[0];
            foreach(string text in reloadFrames)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteCollection = ETGMod.Databases.Items.WeaponCollection, spriteId = ETGMod.Databases.Items.WeaponCollection.GetSpriteIdByName(text) };
                reloadClip.frames = reloadClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            UnityEngine.Object.DontDestroyOnLoad(shellCasing);
            shellCasing.GetAnyComponent<tk2dBaseSprite>().SetSprite(Toolbox.VFXCollection, Toolbox.VFXCollection.GetSpriteIdByName("hd_shell_001"));
            Toolbox.VFXCollection.spriteDefinitions[shellCasing.GetAnyComponent<tk2dBaseSprite>().spriteId] =
                Toolbox.GetGunById(38).shellCasing.GetAnyComponent<tk2dBaseSprite>().Collection.spriteDefinitions[Toolbox.GetGunById(38).shellCasing.GetAnyComponent<tk2dBaseSprite>().spriteId].CopyDefinitionFrom();
            shellCasing.AddComponent<AlwaysUnpixeledBehaviour>();
            gun.shellCasing = shellCasing;
            gun.shellsToLaunchOnFire = 0;
            gun.shellsToLaunchOnReload = 6;
            gun.reloadShellLaunchFrame = 2;
            gun.barrelOffset.transform.localPosition = new Vector3(1.0625f, 0.5625f, 0f);
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToCursulaShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(38);
            ItemBuilder.AddPassiveStatModifier(gun, PlayerStats.StatType.Coolness, 3, StatModifier.ModifyMethod.ADDITIVE);
            tk2dSpriteDefinition ammonomiconDef = AmmonomiconController.ForceInstance.EncounterIconCollection.spriteDefinitions[AmmonomiconController.ForceInstance.EncounterIconCollection.GetSpriteIdByName("hd_idle_001")];
            ammonomiconDef.position1 = new Vector3(ammonomiconDef.position1.x / 2, ammonomiconDef.position1.y / 2, ammonomiconDef.position1.z / 2);
            ammonomiconDef.position2 = new Vector3(ammonomiconDef.position2.x / 2, ammonomiconDef.position2.y / 2, ammonomiconDef.position2.z / 2);
            ammonomiconDef.position3 = new Vector3(ammonomiconDef.position3.x / 2, ammonomiconDef.position3.y / 2, ammonomiconDef.position3.z / 2);
            ammonomiconDef.boundsDataCenter = new Vector3(ammonomiconDef.boundsDataCenter.x / 2, ammonomiconDef.boundsDataCenter.y / 2, ammonomiconDef.boundsDataCenter.z / 2);
            ammonomiconDef.boundsDataExtents = new Vector3(ammonomiconDef.boundsDataExtents.x / 2, ammonomiconDef.boundsDataExtents.y / 2, ammonomiconDef.boundsDataExtents.z / 2);
            ammonomiconDef.untrimmedBoundsDataCenter = new Vector3(ammonomiconDef.untrimmedBoundsDataCenter.x / 2, ammonomiconDef.untrimmedBoundsDataCenter.y / 2, ammonomiconDef.untrimmedBoundsDataCenter.z / 2);
            ammonomiconDef.untrimmedBoundsDataExtents = new Vector3(ammonomiconDef.untrimmedBoundsDataExtents.x / 2, ammonomiconDef.untrimmedBoundsDataExtents.y / 2, ammonomiconDef.untrimmedBoundsDataExtents.z / 2);
            List<tk2dSpriteDefinition> affected = new List<tk2dSpriteDefinition>();
            foreach (tk2dSpriteAnimationClip clip3 in gun.GetGunAnimationClips())
            {
                if (clip3.frames != null)
                {
                    foreach (tk2dSpriteAnimationFrame frame in clip3.frames)
                    {
                        if (frame != null && frame.spriteCollection != null)
                        {
                            tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                            if (def != null && !affected.Contains(def))
                            {
                                def.position1 = new Vector3(def.position1.x / 2, def.position1.y / 2, def.position1.z / 2);
                                def.position2 = new Vector3(def.position2.x / 2, def.position2.y / 2, def.position2.z / 2);
                                def.position3 = new Vector3(def.position3.x / 2, def.position3.y / 2, def.position3.z / 2);
                                def.boundsDataCenter = new Vector3(def.boundsDataCenter.x / 2, def.boundsDataCenter.y / 2, def.boundsDataCenter.z / 2);
                                def.boundsDataExtents = new Vector3(def.boundsDataExtents.x / 2, def.boundsDataExtents.y / 2, def.boundsDataExtents.z / 2);
                                def.untrimmedBoundsDataCenter = new Vector3(def.untrimmedBoundsDataCenter.x / 2, def.untrimmedBoundsDataCenter.y / 2, def.untrimmedBoundsDataCenter.z / 2);
                                def.untrimmedBoundsDataExtents = new Vector3(def.untrimmedBoundsDataExtents.x / 2, def.untrimmedBoundsDataExtents.y / 2, def.untrimmedBoundsDataExtents.z / 2);
                                def.RemoveOffset();
                                affected.Add(def);
                            }
                        }
                    }
                }
            }
            affected.Clear();
            int index = 0;
            foreach (tk2dSpriteAnimationFrame frame in gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation).frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                if(def != null && !affected.Contains(def))
                {
                    def.MakeOffset(offsets[0][index]);
                    affected.Add(def);
                }
                index++;
            }
            index = 0;
            foreach (tk2dSpriteAnimationFrame frame in gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                if (def != null && !affected.Contains(def))
                {
                    def.MakeOffset(offsets[1][index]);
                    affected.Add(def);
                }
                index++;
            }
            index = 0;
            foreach (tk2dSpriteAnimationFrame frame in gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames)
            {
                tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                if (def != null && !affected.Contains(def))
                {
                    def.MakeOffset(offsets[2][index]);
                    affected.Add(def);
                }
                index++;
            }
            AdvancedHoveringGunSynergyProcessor processor = gun.gameObject.AddComponent<AdvancedHoveringGunSynergyProcessor>();
            processor.RequiredSynergy = "#LOW-QUALITY_HELP";
            processor.TargetGunID = 38;
            processor.UsesMultipleGuns = false;
            processor.PositionType = HoveringGunController.HoverPosition.CIRCULATE;
            processor.AimType = HoveringGunController.AimType.PLAYER_AIM;
            processor.FireType = HoveringGunController.FireType.ON_RELOAD;
            processor.FireCooldown = 0.2f;
            processor.FireDuration = 0f;
            processor.OnlyOnEmptyReload = false;
            processor.ShootAudioEvent = "";
            processor.OnEveryShotAudioEvent = "";
            processor.FinishedShootingAudioEvent = "";
            processor.Trigger = AdvancedHoveringGunSynergyProcessor.TriggerStyle.CONSTANT;
            processor.NumToTrigger = 1;
            processor.TriggerDuration = 0f;
            processor.ConsumesTargetGunAmmo = false;
            processor.ChanceToConsumeTargetGunAmmo = 0f;
        }

        protected override void OnPostDroppedByPlayer(PlayerController player)
        {
            base.OnPostDroppedByPlayer(player);
            player.HighAccuracyAimMode = false;
        }

        protected override void OnPickedUpByPlayer(PlayerController player)
        {
            base.OnPickedUpByPlayer(player);
            player.HighAccuracyAimMode = true;
        }

        protected override void Update()
        {
            base.Update();
            if(base.gameObject.layer != LayerMask.NameToLayer("Unpixelated"))
            {
                base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
            }
            if(this.Player != null)
            {
                this.Player.HighAccuracyAimMode = true;
            }
        }

        private void OnEnable()
        {
            base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
        }

        public static List<string> reloadFrames = new List<string>
        {
            "hd_reload_001",
            "hd_reload_002",
            "hd_reload_002",
            "hd_reload_002",
            "hd_reload_002",
            "hd_fire_003"
        };

        public static List<List<Vector3>> offsets = new List<List<Vector3>>
        {
            new List<Vector3> { new Vector3(0f, 0f) },
            new List<Vector3>
            {
                new Vector3(0f, 0f),
                new Vector3(-0.125f, 0.0625f),
                new Vector3(-0.0625f, 0.0f),
                new Vector3(0.0625f, -0.0625f)
            },
            new List<Vector3>
            {
                new Vector3(-0.125f, 0.0f),
                new Vector3(-0.125f, -0.0625f),
                new Vector3(-0.125f, -0.0625f),
                new Vector3(-0.125f, -0.0625f),
                new Vector3(-0.125f, -0.0625f),
                new Vector3(-0.0625f, 0.0f),
            }
        };
    }

    public class AlwaysUnpixeledBehaviour : MonoBehaviour
    {
        private void Update()
        {
            if (base.gameObject.layer != LayerMask.NameToLayer("Unpixelated"))
            {
                base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unpixelated"));
            }
        }
    }
}