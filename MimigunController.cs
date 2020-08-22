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
    class MimigunController : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Mimigun", "mimigun");
            Game.Items.Rename("outdated_gun_mods:mimigun", "spapi:mimigun");
            gun.gameObject.AddComponent<MimigunController>();
            GunExt.SetShortDescription(gun, "Baby Good Mimigun");
            GunExt.SetLongDescription(gun, "Despite most mimics being evil, and trying to stop gungeoneers in any ways, this one is good and will try to help it's owner as it can. It isn't very powerful, but as all mimics, it has the ability to mimic anything" +
                " it sees. It one will try to use it to help.");
            GunExt.SetupSprite(gun, null, "mimigun_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.AddProjectileModuleFrom(gun, Toolbox.GetGunById(734), true, false);
            gun.reloadTime = 2.0f;
            gun.SetBaseMaxAmmo(900);
            gun.quality = PickupObject.ItemQuality.A;
            gun.barrelOffset.transform.localPosition = new Vector3(1.85f, 0.6f, 0f);
            gun.encounterTrackable.EncounterGuid = "mimigun_very_cute";
            gun.gunClass = GunClass.SILLY;
            gun.gunSwitchGroup = Toolbox.GetGunById(734).gunSwitchGroup;
            gun.muzzleFlashEffects = Toolbox.GetGunById(734).muzzleFlashEffects;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToCursulaShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(369);
            gun.SetupUnlockOnFlag(GungeonFlags.ITEMSPECIFIC_HAS_BEEN_PEDESTAL_MIMICKED, true);
            mimigunId = gun.PickupObjectId;
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            if(this.Player != null && this.Player.PlayerHasActiveSynergy("#DEADLY_SURPRISE") && this.gun.GetComponent<CantTransformBehaviour>() != null)
            {
                projectile.baseData.damage *= 1.5f;
            }
        }

        protected override void Update()
        {
            base.Update();
            if(this.Owner != null && this.gun.GetComponent<CantTransformBehaviour>() != null && this.gun.GetComponent<CantTransformBehaviour>().detransformed)
            {
                this.gun.TransformToTargetGun(Toolbox.GetGunById(this.gun.GetComponent<CantTransformBehaviour>().transformGunId));
                this.gun.TransformToTargetGun(Toolbox.GetGunById(this.gun.GetComponent<CantTransformBehaviour>().transformGunId));
                this.gun.InfiniteAmmo = Toolbox.GetGunById(this.gun.GetComponent<CantTransformBehaviour>().transformGunId).InfiniteAmmo;
                this.gun.UsesRechargeLikeActiveItem = Toolbox.GetGunById(this.gun.GetComponent<CantTransformBehaviour>().transformGunId).UsesRechargeLikeActiveItem;
                this.gun.ActiveItemStyleRechargeAmount = Toolbox.GetGunById(this.gun.GetComponent<CantTransformBehaviour>().transformGunId).ActiveItemStyleRechargeAmount;
                FieldInfo info2 = typeof(Gun).GetField("m_cachedGunHandedness", BindingFlags.NonPublic | BindingFlags.Instance);
                info2.SetValue(this.gun, new GunHandedness?(Toolbox.GetGunById(this.gun.GetComponent<CantTransformBehaviour>().transformGunId).Handedness));
                if(this.Player != null)
                {
                    this.Player.ProcessHandAttachment();
                }
                this.gun.GetComponent<CantTransformBehaviour>().detransformed = false;
            }
            if(this.Owner == null && this.gun.GetComponent<CantTransformBehaviour>() != null && !this.gun.GetComponent<CantTransformBehaviour>().detransformed)
            {
                this.gun.TransformToTargetGun(Toolbox.GetGunById(mimigunId));
                this.gun.GetComponent<CantTransformBehaviour>().detransformed = true;
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            base.OnReloadPressed(player, gun, bSOMETHING);
            if (!gun.IsReloading && gun.GetComponent<CantTransformBehaviour>() == null)
            {
                IPlayerInteractable nearestInteractable = player.CurrentRoom.GetNearestInteractable(player.CenterPosition, 1f, player);
                if(nearestInteractable != null && nearestInteractable is Gun)
                {
                    Gun gun2 = Toolbox.GetGunById((nearestInteractable as Gun).PickupObjectId);
                    if(gun2 != null)
                    {
                        gun.TransformToTargetGun(gun2);
                        gun.InfiniteAmmo = gun2.InfiniteAmmo;
                        gun.UsesRechargeLikeActiveItem = gun2.UsesRechargeLikeActiveItem;
                        gun.ActiveItemStyleRechargeAmount = gun2.ActiveItemStyleRechargeAmount;
                        this.gun.gameObject.AddComponent<CantTransformBehaviour>().transformGunId = gun2.PickupObjectId;
                        FieldInfo info2 = typeof(Gun).GetField("m_cachedGunHandedness", BindingFlags.NonPublic | BindingFlags.Instance);
                        info2.SetValue(this.gun, new GunHandedness?(gun2.Handedness));
                        player.ProcessHandAttachment();
                    }
                }
            }
        }

        public static int mimigunId;

        private class CantTransformBehaviour : GunBehaviour
        {
            public int transformGunId;
            public bool detransformed = false;
        }
    }
}