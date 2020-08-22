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
    class LichSlayer : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Lich Slayer 9000", "lich_slayer");
            Game.Items.Rename("outdated_gun_mods:lich_slayer_9000", "spapi:lich_slayer_9000");
            gun.gameObject.AddComponent<LichSlayer>();
            GunExt.SetShortDescription(gun, "Banger!");
            GunExt.SetLongDescription(gun, "This gun shoots bones that give lich an osteoporosis.\n\nMade in Zilla Industries for the most talented gungeoneer. Legends say he killed Lich, the Gungeon Master over 9000 times...");
            GunExt.SetupSprite(gun, null, "lich_slayer_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 6);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(812) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.transform.parent = gun.barrelOffset;
            gun.DefaultModule.numberOfShotsInClip = 900;
            projectile.baseData.damage = 12f;
            gun.DefaultModule.angleVariance /= 2.5f;
            gun.DefaultModule.cooldownTime = 0.05f;
            projectile.BossDamageMultiplier = 1.35f;
            projectile.name = "Lich_Slayer_Projectile";
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.SKULL;
            gun.reloadTime = 1f;
            gun.SetBaseMaxAmmo(900);
            gun.quality = PickupObject.ItemQuality.S;
            gun.encounterTrackable.EncounterGuid = "lich_slayer_9001";
            gun.gunClass = GunClass.FULLAUTO;
            gun.barrelOffset.transform.localPosition = new Vector3(2.05f, 0.60f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(274) as Gun).muzzleFlashEffects;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(16);
            gun.SetupUnlockOnStat(TrackedStats.TIMES_CLEARED_BULLET_HELL, DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN, 9);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            projectile.OnHitEnemy += this.OnHit;
            PlayerController player = this.gun.CurrentOwner as PlayerController;
            if ((UnityEngine.Random.value < 0.35f) || ((GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.HELLGEON)))
            {
                projectile.ignoreDamageCaps = true;
                if (player.PlayerHasActiveSynergy("#LICHES_NO_MORE"))
                {
                    projectile.AppliesPoison = true;
                    projectile.PoisonApplyChance = 100;
                    projectile.healthEffect = (PickupObjectDatabase.GetById(204) as BulletStatusEffectItem).HealthModifierEffect;
                }
                if (GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.HELLGEON)
                {
                    this.gun.DefaultModule.ammoCost = 2;
                }
                else
                {
                    this.gun.DefaultModule.ammoCost = 1;
                }
            }
            else
            {
                this.gun.DefaultModule.ammoCost = 1;
            }
        }

        private void Update()
        {
            if (!this.gun.PreventNormalFireAudio)
            {
                //this.gun.PreventNormalFireAudio = true;
            }
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
        }

        private void OnHit(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
        {
            if(enemy == null || enemy.aiActor == null)
            {
                return;
            }
            if(enemy != null && enemy.aiActor != null && (enemy.aiActor.EnemyGuid == "cd88c3ce60c442e9aa5b3904d31652bc" || enemy.aiActor.EnemyGuid == "68a238ed6a82467ea85474c595c49c6e" || enemy.aiActor.EnemyGuid == "7c5d5f09911e49b78ae644d2b50ff3bf"))
            {
                enemy.aiActor.healthHaver.ApplyDamage(proj.baseData.damage *= 1.5f, proj.specRigidbody.Velocity, "Lich Slayer 9000", CoreDamageTypes.SpecialBossDamage, DamageCategory.Normal, true, null, true);
            }
        }
    }
}