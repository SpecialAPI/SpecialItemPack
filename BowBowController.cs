using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;

namespace SpecialItemPack
{
    class BowBowController : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Suspicious Shotgun", "jail_npc_shotgun");
            Game.Items.Rename("outdated_gun_mods:suspicious_shotgun", "spapi:suspicious_shotgun");
            gun.gameObject.AddComponent<BowBowController>();
            GunExt.SetShortDescription(gun, "Covered in rainbow sparkles?");
            GunExt.SetLongDescription(gun, "A weird shotgun. Internal part of the barrel was painted in rainbow paint. A name \"Bow-Bow\" is also engraved on the barrel.\n\nThis strange shotgun was found in a cell. Probably someone forgot it there while they were moving to breach.");
            GunExt.SetupSprite(gun, null, "jail_npc_shotgun_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 8);
            for (int i = 0; i < 5; i++)
            {
                GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            }
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {
                mod.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(51) as Gun).DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                mod.projectiles[0] = projectile;
                projectile.transform.parent = gun.barrelOffset;
                mod.numberOfShotsInClip = 7;
                projectile.baseData.damage = 7f;
                mod.cooldownTime = 0.6f;
                projectile.baseData.range /= 2.25f;
                projectile.name = "Suspicious_Shotgun_Projectile";
                mod.ammoType = GameUIAmmoType.AmmoType.SHOTGUN;
                if(mod != gun.DefaultModule)
                {
                    mod.ammoCost = 0;
                }
            }
            gun.shellCasing = Toolbox.GetGunById(202).shellCasing;
            gun.reloadShellLaunchFrame = 5;
            gun.shellCasingOnFireFrameDelay = 0;
            gun.shellsToLaunchOnFire = 1;
            gun.shellsToLaunchOnReload = 3;
            gun.reloadTime = 1f;
            gun.SetBaseMaxAmmo(200);
            gun.quality = PickupObject.ItemQuality.B;
            gun.encounterTrackable.EncounterGuid = "boiled_bowler_shotga";
            gun.gunClass = GunClass.SHOTGUN;
            gun.barrelOffset.transform.localPosition = new Vector3(1.4f, 0.2f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(51) as Gun).muzzleFlashEffects;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToCursulaShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(100);
            gun.SetupUnlockOnFlag(GungeonFlags.BOWLER_ACTIVE_IN_FOYER, true);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            projectile.OnWillKillEnemy += this.OnKill;
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            if (player != null && player.PlayerHasActiveSynergy("#MORE_RAINBOW?"))
            {
                projectile.baseData.damage *= 1.35f;
                projectile.AdditionalScaleMultiplier *= 1.35f;
                projectile.baseData.range *= 1.35f;
            }
            if (GameStatsManager.Instance.IsRainbowRun)
            {
                projectile.baseData.damage *= 1.35f;
                projectile.AdditionalScaleMultiplier *= 1.35f;
                projectile.baseData.range *= 1.35f;
            }
        }

        private void Update()
        {
            if (!this.gun.PreventNormalFireAudio)
            {
                this.gun.PreventNormalFireAudio = true;
            }
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            AkSoundEngine.PostEvent("Play_WPN_shotgun_shot_01", base.gameObject);
        }

        private void OnKill(Projectile proj, SpeculativeRigidbody enemy)
        {
            if(enemy == null || enemy.aiActor == null)
            {
                return;
            }
            int maxChance = 10000;
            if (GameStatsManager.Instance.IsRainbowRun)
            {
                maxChance /= 5;
            }
            if(UnityEngine.Random.Range(1, maxChance) <= 1)
            {
                Chest rainbowBox = Chest.Spawn(GameManager.Instance.RewardManager.Rainbow_Chest, enemy.aiActor.sprite.WorldTopCenter.ToIntVector2(), enemy.aiActor.sprite.WorldTopCenter.GetAbsoluteRoom(), true);
            }
        }
    }
}