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
    class SuperBatLauncher : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Super Bullat Launcher", "superbatlauncher");
            Game.Items.Rename("outdated_gun_mods:super_bullat_launcher", "spapi:bullat_launcher+king_bullat_shooter");
            GunExt.SetShortDescription(gun, "Fires Bullats");
            GunExt.SetLongDescription(gun, "Shoots angry bullats.\n\nCatching Bullats is fairly easy and every good gundead hunter has to have a Bullat collection. Frifle through had an idea to weaponize his collection, and made this gun.");
            GunExt.SetupSprite(gun, null, "superbatlauncher_idle_001", 12);
            for(int i = 0; i < 12; i++)
            {
                GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            }
            int j = 0;
            foreach(ProjectileModule mod in gun.Volley.projectiles)
            {
                mod.angleVariance = 0;
                mod.shootStyle = ProjectileModule.ShootStyle.Charged;
                mod.cooldownTime = 0.5f;
                mod.numberOfShotsInClip = 10;
                Projectile projectile = Instantiate(Toolbox.GetGunById(38).DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                DontDestroyOnLoad(projectile);
                projectile.transform.parent = gun.barrelOffset;
                projectile.baseData.damage = 15f;
                projectile.name = "SuperBullat_Projectile";
                BounceProjModifier modifier = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
                modifier.numberOfBounces = 1;
                mod.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
                {
                    new ProjectileModule.ChargeProjectile
                    {
                        ChargeTime = 1.5f,
                        Projectile = projectile
                    }
                };
                mod.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
                mod.customAmmoType = "finished small";
                mod.angleFromAim = j * 30;
                j++;
            }
            AdvancedGunBehaviour advancedGunBehaviour = gun.gameObject.AddComponent<AdvancedGunBehaviour>();
            advancedGunBehaviour.preventNormalFireAudio = true;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.reloadTime = 1.5f;
            gun.gunHandedness = GunHandedness.OneHanded;
            gun.SetBaseMaxAmmo(50);
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.barrelOffset.transform.localPosition = new Vector3(1.625f, 1.5625f, 0f);
            gun.encounterTrackable.EncounterGuid = "superbatlauncher";
            gun.gunClass = GunClass.CHARGE;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_ENM_bullat_tackle_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).frames[0].eventAudio = "Play_ENM_bullat_charge_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).frames[0].triggerEvent = true;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 1;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToTrorkShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(22);
        }
    }
}
