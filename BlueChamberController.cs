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
    class BlueChamberController : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Blue Chamber", "blue_chamber");
            Game.Items.Rename("outdated_gun_mods:blue_chamber", "spapi:blue_chamber");
            gun.gameObject.AddComponent<BlueChamberController>();
            GunExt.SetShortDescription(gun, "\"it shoots stuff\"");
            GunExt.SetLongDescription(gun, "Even through this gun seems a chamber, it's actually a gun.\n\nKills the weak.");
            GunExt.SetupSprite(gun, null, "blue_chamber_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 10);
            GunExt.SetAnimationFPS(gun, gun.chargeAnimation, 10);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(761) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                new ProjectileModule.ChargeProjectile
                {
                    Projectile = projectile, 
                    ChargeTime = 1.6f
                }
            };
            projectile.transform.parent = gun.barrelOffset;
            projectile.RemoveComponent<KthuliberProjectileController>();
            projectile.gameObject.AddComponent<KillTheWeakBehaviour>();
            projectile.name = "BlueChamber_Projectile";
            gun.reloadClipLaunchFrame = 0;
            gun.DefaultModule.cooldownTime = 2f;
            gun.DefaultModule.numberOfShotsInClip = 10;
            gun.reloadTime = 2f;
            gun.DefaultModule.ammoCost = 6;
            gun.SetBaseMaxAmmo(666);
            gun.quality = PickupObject.ItemQuality.C;
            gun.barrelOffset.transform.localPosition = new Vector3(1.3f, 0.65f, 0f);
            gun.gunSwitchGroup = "Kthulu";
            gun.encounterTrackable.EncounterGuid = "blue_chamber";
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.gunClass = GunClass.PISTOL;
            ItemBuilder.AddPassiveStatModifier(gun, PlayerStats.StatType.Curse, 2f);
            ItemBuilder.AddPassiveStatModifier(gun, PlayerStats.StatType.Health, 2f);
            ItemBuilder.AddPassiveStatModifier(gun, PlayerStats.StatType.RateOfFire, 0.15f);
            tk2dSpriteAnimator animator = gun.GetComponent<tk2dSpriteAnimator>();
            for (int i = animator.GetClipByName(gun.chargeAnimation).frames.Length - 2; i > -1; i--)
            {
                tk2dSpriteAnimationFrame frame2 = animator.GetClipByName(gun.chargeAnimation).frames[i];
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = frame2.spriteId, spriteCollection = frame2.spriteCollection };
                animator.GetClipByName(gun.shootAnimation).frames = animator.GetClipByName(gun.shootAnimation).frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            animator.GetClipByName(gun.chargeAnimation).loopStart = 15;
            animator.GetClipByName(gun.shootAnimation).frames[0].FrameToDefinition().MakeOffset(new Vector2(-0.125f, 0.125f));
            animator.GetClipByName(gun.shootAnimation).frames[1].FrameToDefinition().MakeOffset(new Vector2(0.0625f, -0.0625f));
            animator.GetClipByName(gun.shootAnimation).frames[2].FrameToDefinition().MakeOffset(new Vector2(-0.0625f, 0.0625f));
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToTrorkShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(22);
        }
    }
}
