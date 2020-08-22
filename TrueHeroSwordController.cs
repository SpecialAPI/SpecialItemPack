using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;

namespace SpecialItemPack
{
    class TrueHeroSwordController : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Hero's Sword", "hero");
            Game.Items.Rename("outdated_gun_mods:hero's_sword", "spapi:heros_sword");
            gun.gameObject.AddComponent<TrueHeroSwordController>();
            GunExt.SetShortDescription(gun, "Enter the Swordgeon");
            GunExt.SetLongDescription(gun, "An elegant weapon. Can never be obsolete!\n\nSwords like this were used by hero bullets a long time ago, when the Gungeon was still named Swordgeon. After all the time that has passed, it's still a reliable " +
                "weapon!");
            GunExt.SetupSprite(gun, null, "hero_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 12);
            GunExt.AddProjectileModuleFrom(gun, "wonderboy", true, false);
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((ETGMod.Databases.Items["wonderboy"] as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.DefaultTintColor = Color.white;
            projectile.HasDefaultTint = true;
            projectile.baseData.damage *= 2f;
            projectile.gameObject.AddComponent<HeroSwordSpunBehaviour>();
            PierceProjModifier mod = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            mod.penetration = 999;
            mod.penetratesBreakables = true;
            mod.preventPenetrationOfActors = false;
            mod.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
            gun.DefaultModule.projectiles[0] = projectile;
            gun.DefaultModule.angleVariance = 0f;
            gun.reloadTime = 1.0f;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;
            gun.barrelOffset.transform.localPosition = new Vector3(1.750f, 0.3f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(574) as Gun).muzzleFlashEffects;
            gun.encounterTrackable.EncounterGuid = "true_hero_sword";
            gun.OverrideNormalFireAudioEvent = "Play_WPN_blasphemy_shot_01";
            gun.gunClass = GunClass.SILLY;
            gun.gunSwitchGroup = "Blasphemy";
            gun.IsHeroSword = true;
            gun.HeroSwordDoesntBlank = false;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.blankReloadRadius = 3f;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(417);
            SpecialItemIds.HerosSword = gun.PickupObjectId;
        }

        public override void OnHeroSwordCooldownStarted(PlayerController player, Gun gun)
        {
            base.OnHeroSwordCooldownStarted(player, gun);
            AkSoundEngine.PostEvent("Play_WPN_blasphemy_shot_01", gun.gameObject);
        }

        public class HeroSwordSpunBehaviour : BraveBehaviour
        {
            public void Start()
            {
                if(base.projectile != null)
                {
                    base.projectile.OnPostUpdate += this.OnPostUpdate;
                }
            }

            private void OnPostUpdate(Projectile proj)
            {
                if(proj != null && proj.transform != null)
                {
                    proj.transform.rotation = Quaternion.Euler(0, 0, proj.transform.rotation.eulerAngles.z + 720 * BraveTime.DeltaTime);
                    if(proj.specRigidbody != null)
                    {
                        proj.specRigidbody.UpdateCollidersOnRotation = true;
                    }
                }
            }
        }
    }
}
