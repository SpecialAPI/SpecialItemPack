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
    class RedChamberGun
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Red Chamber", "red_chamber");
            Game.Items.Rename("outdated_gun_mods:red_chamber", "spapi:red_chamber");
            GunExt.SetShortDescription(gun, "Product of Environment");
            GunExt.SetLongDescription(gun, "Transforms based on Chamber.\n\nThis gun uses an advanced shape - memory alloy. When exposed to different environmental stimuli it reacts with the salts in Gungeoneer’s bodies and transforms into a location - " +
                "specific gun. Master rounds can also influence the shape of this curious weapon.");
            GunExt.SetupSprite(gun, null, "red_chamber_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 10);
            GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 10);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            Projectile projectile = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(761) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.transform.parent = gun.barrelOffset;
            projectile.RemoveComponent<KthuliberProjectileController>();
            projectile.gameObject.AddComponent<KillTheWeakBehaviour>();
            projectile.name = "RedChamber_Projectile";
            Projectile projectile2 = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(761) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            projectile2.transform.parent = gun.barrelOffset;
            projectile2.RemoveComponent<KthuliberProjectileController>();
            projectile2.gameObject.AddComponent<KillTheWeakBehaviour>().maximumWeakHealthAmount = float.MaxValue;
            projectile2.name = "RedChamber_StrongProjectile";
            AfterImageTrailController trailController = projectile2.GetAnySprite().gameObject.AddComponent<AfterImageTrailController>();
            trailController.dashColor = Color.yellow;
            trailController.spawnShadows = true;
            trailController.shadowTimeDelay = 0.05f;
            trailController.shadowLifetime = 0.3f;
            trailController.minTranslation = 0.05f;
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                new ProjectileModule.ChargeProjectile
                {
                    Projectile = projectile,
                    ChargeTime = 1.6f
                },
                new ProjectileModule.ChargeProjectile
                {
                    Projectile = projectile2,
                    ChargeTime = 3.2f,
                    UsedProperties = ProjectileModule.ChargeProjectileProperties.ammo | ProjectileModule.ChargeProjectileProperties.depleteAmmo | ProjectileModule.ChargeProjectileProperties.shootAnim,
                    AmmoCost = 60,
                }
            };
            gun.reloadClipLaunchFrame = 0;
            gun.DefaultModule.cooldownTime = 2f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.reloadTime = 2f;
            gun.DefaultModule.ammoCost = 6;
            gun.SetBaseMaxAmmo(666);
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.barrelOffset.transform.localPosition = new Vector3(1.3f, 0.65f, 0f);
            string superShootAnimation = gun.UpdateAnimation("fire2", null, true);
            gun.DefaultModule.chargeProjectiles[1].OverrideShootAnimation = superShootAnimation;
            GunExt.SetAnimationFPS(gun, superShootAnimation, 10);
            gun.gunSwitchGroup = "Kthulu";
            gun.encounterTrackable.EncounterGuid = "red_chamber";
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.gunClass = GunClass.PISTOL;
            gun.gunHandedness = GunHandedness.OneHanded;
            tk2dSpriteAnimator animator = gun.GetComponent<tk2dSpriteAnimator>();
            for (int i = animator.GetClipByName(gun.chargeAnimation).frames.Length - 2; i > -1; i--)
            {
                tk2dSpriteAnimationFrame frame2 = animator.GetClipByName(gun.chargeAnimation).frames[i];
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = frame2.spriteId, spriteCollection = frame2.spriteCollection };
                animator.GetClipByName(gun.shootAnimation).frames = animator.GetClipByName(gun.shootAnimation).frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            for (int i = animator.GetClipByName(gun.chargeAnimation).frames.Length - 6; i > -1; i--)
            {
                tk2dSpriteAnimationFrame frame2 = animator.GetClipByName(gun.chargeAnimation).frames[i];
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = frame2.spriteId, spriteCollection = frame2.spriteCollection };
                animator.GetClipByName(superShootAnimation).frames = animator.GetClipByName(superShootAnimation).frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            List<string> spriteNames = new List<string>
            {
                "red_chamber_charge_016",
                "red_chamber_charge_016",
                "red_chamber_charge_016",
                "red_chamber_charge_016",
                "red_chamber_charge_016",
                "red_chamber_charge_016",
                "red_chamber_charge_016",
                "red_chamber_charge_016",
                "red_chamber_charge_016",
                "red_chamber_charge_016",
                "red_chamber_charge_016",
                "red_chamber_charge_015",
                "red_chamber_charge2_001",
                "red_chamber_charge2_002",
                "red_chamber_charge2_003",
                "red_chamber_charge2_004",
            };
            foreach(string spriteName in spriteNames)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = ETGMod.Databases.Items.WeaponCollection.GetSpriteIdByName(spriteName), spriteCollection = ETGMod.Databases.Items.WeaponCollection };
                animator.GetClipByName(gun.chargeAnimation).frames = animator.GetClipByName(gun.chargeAnimation).frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            animator.GetClipByName(gun.chargeAnimation).loopStart = 31;
            animator.GetClipByName(gun.shootAnimation).frames[0].FrameToDefinition().MakeOffset(new Vector2(-0.125f, 0.125f));
            animator.GetClipByName(gun.shootAnimation).frames[1].FrameToDefinition().MakeOffset(new Vector2(0.0625f, -0.0625f));
            animator.GetClipByName(gun.shootAnimation).frames[2].FrameToDefinition().MakeOffset(new Vector2(-0.0625f, 0.0625f));
            animator.GetClipByName(superShootAnimation).frames[0].FrameToDefinition().MakeOffset(new Vector2(-0.125f, 0.125f));
            animator.GetClipByName(superShootAnimation).frames[1].FrameToDefinition().MakeOffset(new Vector2(0.0625f, -0.0625f));
            animator.GetClipByName(superShootAnimation).frames[2].FrameToDefinition().MakeOffset(new Vector2(-0.0625f, 0.0625f));
            animator.GetClipByName(superShootAnimation).frames[0].eventAudio = "Play_WPN_kthulu_soul_01";
            animator.GetClipByName(superShootAnimation).frames[0].triggerEvent = true;
            animator.MuteAudio = false;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            SpecialItemIds.RedChamber = gun.PickupObjectId;
        }
    }
}
