using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using Dungeonator;
using SpecialItemPack.AdaptedSynergyStuff;

namespace SpecialItemPack
{
    class AegisGunController : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Aegis Gun", "aegis");
            Game.Items.Rename("outdated_gun_mods:aegis_gun", "spapi:aegis_gun");
            gun.gameObject.AddComponent<AegisGunController>();
            GunExt.SetShortDescription(gun, "En Guarde!");
            GunExt.SetLongDescription(gun, "Reloading shields enemy bullets.\n\nThis handgun was made out of a sword. Through, the melee abilities of the sword it was made from were completelly nullified, so this weapon doesn't anger the" +
                " jammed.");
            GunExt.SetupSprite(gun, null, "aegis_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(464) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.transform.parent = gun.barrelOffset;
            projectile.DefaultTintColor = new Color(1, 1, 1).WithAlpha(0.45f);
            projectile.HasDefaultTint = true;
            projectile.name = "Explosive_Aegis_Shell";
            ExplosionData data = (PickupObjectDatabase.GetById(81) as Gun).DefaultModule.projectiles[0].GetComponent<ExplosiveModifier>().explosionData.CopyExplosionData();
            data.doDestroyProjectiles = false;
            data.doForce = false;
            data.damage *= 0.35f;
            ExplosiveModifier mod = projectile.gameObject.GetOrAddComponent<ExplosiveModifier>();
            mod.explosionData = data;
            gun.reloadClipLaunchFrame = 0;
            gun.DefaultModule.cooldownTime = 0.2f;
            gun.DefaultModule.numberOfShotsInClip = 4;
            gun.reloadTime = 1.4f;
            gun.SetBaseMaxAmmo(150);
            gun.quality = PickupObject.ItemQuality.B;
            gun.gunSwitchGroup = Toolbox.GetGunById(380).gunSwitchGroup;
            gun.muzzleFlashEffects = Toolbox.GetGunById(334).muzzleFlashEffects;
            gun.barrelOffset.transform.localPosition = new Vector3(0.9f, 0.55f, 0f);
            gun.encounterTrackable.EncounterGuid = "aegis_junk";
            AdvancedDualWieldSynergyProcessor dualWieldController = gun.gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            dualWieldController.SynergyNameToCheck = "#SHIELD_BROS";
            dualWieldController.PartnerGunID = 380;
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToCursulaShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(380);
        }

        protected override void Update()
        {
            base.Update();
            if(this.gun.CurrentOwner != null)
            {
                Vector2 cachedSlashOffset = this.gun.PrimaryHandAttachPoint.position.XY() - this.Owner.CenterPosition;
                Vector2 arcOrigin = this.gun.CurrentOwner.CenterPosition + cachedSlashOffset;
                float num = 1;
                float num2 = 45f;
                float num3 = num * num;
                if (this.gun.IsReloading)
                {
                    ReadOnlyCollection<Projectile> allProjectiles2 = StaticReferenceManager.AllProjectiles;
                    for (int j = allProjectiles2.Count - 1; j >= 0; j--)
                    {
                        Projectile projectile2 = allProjectiles2[j];
                        if (projectile2 && (!(projectile2.Owner is PlayerController) || projectile2.ForcePlayerBlankable))
                        {
                            if (!(projectile2.Owner is AIActor) || (projectile2.Owner as AIActor).IsNormalEnemy)
                            {
                                Vector2 worldCenter2 = projectile2.sprite.WorldCenter;
                                float num5 = Vector2.SqrMagnitude(worldCenter2 - arcOrigin);
                                if (num5 < num3)
                                {
                                    float target2 = BraveMathCollege.Atan2Degrees(worldCenter2 - arcOrigin);
                                    if (Mathf.DeltaAngle(this.gun.CurrentAngle, target2) < num2)
                                    {
                                        projectile2.DieInAir(false, true, true, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}