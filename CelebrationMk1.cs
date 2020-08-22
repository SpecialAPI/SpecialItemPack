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
    class CelebrationMk1 : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Celebration", "mk1");
            Game.Items.Rename("outdated_gun_mods:celebration", "spapi:celebration");
            gun.gameObject.AddComponent<CelebrationMk1>();
            GunExt.SetShortDescription(gun, "Great For Gunparties!");
            GunExt.SetLongDescription(gun, "Shoots fireworks. Yes, fireworks.\n\nGun originally designed for being in highest-tier chests, but because of hate it got it was moved to lower-tier chests.");
            GunExt.SetupSprite(gun, null, "mk1_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            for(int i=0; i<3; i++)
            {
                GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            }
            foreach(ProjectileModule mod in gun.Volley.projectiles)
            {
                mod.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                mod.angleVariance /= 1.25f;
                mod.ammoType = GameUIAmmoType.AmmoType.GRENADE;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(275) as Gun).DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                mod.projectiles[0] = projectile;
                projectile.transform.parent = gun.barrelOffset;
                projectile.name = "Firework_Rocket";
                ExplosiveModifier explosiveMod = projectile.gameObject.AddComponent<ExplosiveModifier>();
                explosiveMod.explosionData = new ExplosionData();
                explosiveMod.explosionData.CopyFrom((PickupObjectDatabase.GetById(593) as Gun).DefaultModule.projectiles[0].GetComponent<ExplosiveModifier>().explosionData);
                explosiveMod.explosionData.effect = EnemyDatabase.GetOrLoadByGuid(RnGEnemyDatabase.GetRnGEnemyGuid(RnGEnemyDatabase.RnGEnemyType.DynaM80_Guy)).GetComponent<ExplodeOnDeath>().explosionData.effect;
                explosiveMod.IgnoreQueues = true;
                projectile.SuppressHitEffects = true;
                projectile.damageTypes = CoreDamageTypes.None;
                projectile.AppliesFire = false;
                mod.cooldownTime = 1f;
                mod.numberOfShotsInClip = 1;
                if(mod != gun.DefaultModule)
                {
                    mod.ammoCost = 0;
                }
            }
            gun.Volley.UsesShotgunStyleVelocityRandomizer = true;
            gun.Volley.DecreaseFinalSpeedPercentMin = -10f;
            gun.Volley.IncreaseFinalSpeedPercentMax = 10f;
            gun.reloadTime = 2.5f;
            gun.gunSwitchGroup = Toolbox.GetGunById(39).gunSwitchGroup;
            gun.SetBaseMaxAmmo(75);
            gun.quality = PickupObject.ItemQuality.A;
            gun.barrelOffset.transform.localPosition = new Vector3(1f, 0.35f, 0f);
            gun.encounterTrackable.EncounterGuid = "celebration_mk1";
            gun.gunClass = GunClass.EXPLOSIVE;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToTrorkShop();
            gun.AddToCursulaShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(275);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            if(this.Player != null && this.Player.PlayerHasActiveSynergy("#FIREWORK_MASTER"))
            {
                projectile.GetComponent<ExplosiveModifier>().explosionData.damage *= 1.25f;
            }
        }
    }
}