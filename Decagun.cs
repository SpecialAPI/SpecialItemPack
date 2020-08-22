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
    class Decagun : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Decagun", "decagun");
            Game.Items.Rename("outdated_gun_mods:decagun", "spapi:decagun");
            gun.gameObject.AddComponent<Decagun>();
            GunExt.SetShortDescription(gun, "A Regular Decagun");
            GunExt.SetLongDescription(gun, "In gunometry, a decagun is a ten-sided polygun or 10-gun.");
            GunExt.SetupSprite(gun, null, "decagun_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 13);
            GunExt.SetAnimationFPS(gun, gun.introAnimation, 16);
            for(int i = 0; i < 10; i++)
            {
                GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            }
            int index = 0;
            foreach(ProjectileModule mod in gun.Volley.projectiles)
            {
                mod.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                mod.angleVariance = 0;
                mod.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(38) as Gun).DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                mod.projectiles[0] = projectile;
                projectile.transform.parent = gun.barrelOffset;
                projectile.baseData.damage = 5.10f;
                projectile.baseData.speed = 10;
                projectile.baseData.force = 10;
                mod.cooldownTime = 1;
                mod.numberOfShotsInClip = 10;
                mod.positionOffset += new Vector3(projOffsets[index].x, projOffsets[index].y, 0f);
                if(mod != gun.DefaultModule)
                {
                    mod.ammoCost = 0;
                }
                else
                {
                    mod.ammoCost = 10;
                }
                index++;
            }
            gun.reloadTime = 1.10f;
            gun.SetBaseMaxAmmo(1000);
            gun.quality = PickupObject.ItemQuality.A;
            gun.barrelOffset.transform.localPosition = new Vector3(1.7f, 0.65f, 0f);
            gun.encounterTrackable.EncounterGuid = "decagun";
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(385);
        }

        public static List<Vector2> projOffsets = new List<Vector2>
        {
            new Vector2(0, 0),
            new Vector2(0.25f, 0.7f),
            new Vector2(0.25f, -0.7f),
            new Vector2(0.85f, -1.3f),
            new Vector2(0.85f, 1.3f),
            new Vector2(2.55f, 0.7f),
            new Vector2(2.55f, -0.7f),
            new Vector2(2.05f, -1.3f),
            new Vector2(2.05f, 1.3f),
            new Vector2(2.8f, 0)
        };
    }
}