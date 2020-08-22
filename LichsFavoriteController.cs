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
    class LichsFavoriteController : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Lich's Faithful Gun", "lichs_favorite");
            Game.Items.Rename("outdated_gun_mods:lich's_faithful_gun", "spapi:lichs_faithful_gun");
            gun.gameObject.AddComponent<LichsFavoriteController>();
            GunExt.SetShortDescription(gun, "Iron Fights");
            GunExt.SetLongDescription(gun, "Doesn't shoot. Once used by the lord of the Sixth Chamber as his favorite weapon.\n\nThis gun radiates unfathomable magical power.");
            GunExt.SetupSprite(gun, null, "lichs_favorite_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            gun.DefaultModule.cooldownTime = 0;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.S;
            gun.barrelOffset.transform.localPosition = new Vector3(1.7f, 0.65f, 0f);
            gun.encounterTrackable.EncounterGuid = "lichs_favorite_gun";
            gun.gunClass = GunClass.PISTOL;
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(198);
            gun.SetupUnlockOnFlag(GungeonFlags.GUNSLINGER_PAST_KILLED, true);
        }

        protected override void Update()
        {
            base.Update();
            if (!this.gun.PreventNormalFireAudio)
            {
                this.gun.PreventNormalFireAudio = true;
            }
            if (!this.gun.RuntimeModuleData[this.gun.DefaultModule].onCooldown)
            {
                this.gun.RuntimeModuleData[this.gun.DefaultModule].onCooldown = true;
            }
            if(this.Player != null && this.Player.CharacterUsesRandomGuns)
            {
                this.Player.ChangeToRandomGun();
            }
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            UnityEngine.Object.Destroy(projectile.gameObject);
            base.PostProcessProjectile(projectile);
            this.gun.ClipShotsRemaining = 2;
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
        }
    }
}