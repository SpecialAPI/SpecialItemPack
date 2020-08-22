using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using Dungeonator;
using System.Reflection;

namespace SpecialItemPack
{
    class InfinityController : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Infinity", "infinity");
            Game.Items.Rename("outdated_gun_mods:infinity", "spapi:infinity");
            gun.gameObject.AddComponent<InfinityController>();
            GunExt.SetShortDescription(gun, "[sprite \"infinite-big\"]");
            GunExt.SetLongDescription(gun, "A small piece of never ending time matter, this gun might be older than the universe itself.\nEven though it isn't very powerful, it has a great potential...");
            GunExt.SetupSprite(gun, null, "infinity_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "infinity";
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(35) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.transform.parent = gun.barrelOffset;
            projectile.baseData.damage = 12.123412342348102398f;
            projectile.baseData.speed /= 4.412387401289374012438523f;
            gun.DefaultModule.cooldownTime = 0.369817263493402345209387f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.reloadTime = 0f;
            gun.InfiniteAmmo = false;
            gun.quality = PickupObject.ItemQuality.C;
            gun.barrelOffset.transform.localPosition = new Vector3(1.7f, 0.65f, 0f);
            gun.encounterTrackable.EncounterGuid = "infinity";
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToTrorkShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(22);
            //Toolbox.CreateAmmoType("", "");
            //gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            //gun.DefaultModule.customAmmoType = "ExampleUIClip";
        }

        protected override void Update()
        {
            base.Update();
            if(this.gun.RuntimeModuleData != null)
            {
                foreach(ModuleShootData data in this.gun.RuntimeModuleData.Values)
                {
                    if(data.numberShotsFired >= 0)
                    {
                        data.numberShotsFired = -1;
                    }
                }
            }
        }
    }
}
