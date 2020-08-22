using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using Dungeonator;
using SpecialItemPack.AdaptedSynergyStuff;

namespace SpecialItemPack
{
    class BatLauncher : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Bullat Launcher", "batlauncher");
            Game.Items.Rename("outdated_gun_mods:bullat_launcher", "spapi:bullat_launcher");
            GunExt.SetShortDescription(gun, "Fires Bullats");
            GunExt.SetLongDescription(gun, "Shoots angry bullats.\n\nCatching Bullats is fairly easy and every good gundead hunter has to have a Bullat collection. Frifle through had an idea to weaponize his collection, and made this gun.");
            GunExt.SetupSprite(gun, null, "batlauncher_idle_001", 16);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Charged;
            AdvancedGunBehaviour advancedGunBehaviour = gun.gameObject.AddComponent<AdvancedGunBehaviour>();
            advancedGunBehaviour.preventNormalFireAudio = true;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            AIActor bullat = EnemyDatabase.GetOrLoadByGuid("2feb50a6a40f4f50982e89fd276f6f15");
            Projectile toInstantiate = null;
            foreach(AttackBehaviorBase behav in bullat.behaviorSpeculator.AttackBehaviors)
            {
                if(behav is AttackBehaviorGroup)
                {
                    foreach(AttackBehaviorGroup.AttackGroupItem item in (behav as AttackBehaviorGroup).AttackBehaviors)
                    {
                        if(item.Behavior is SuicideShotBehavior)
                        {
                            toInstantiate = bullat.bulletBank.GetBullet((item.Behavior as SuicideShotBehavior).bulletBankName).BulletObject.GetComponent<Projectile>();
                        }
                    }
                }
                else if(behav is SuicideShotBehavior)
                {
                    toInstantiate = bullat.bulletBank.GetBullet((behav as SuicideShotBehavior).bulletBankName).BulletObject.GetComponent<Projectile>();
                }
            }
            Projectile projectile = UnityEngine.Object.Instantiate(toInstantiate);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.chargeProjectiles = new List<ProjectileModule.ChargeProjectile>
            {
                new ProjectileModule.ChargeProjectile
                {
                    ChargeTime = 1.5f,
                    Projectile = projectile
                }
            };
            projectile.transform.parent = gun.barrelOffset;
            projectile.collidesWithEnemies = true;
            projectile.baseData.damage = 20f;
            gun.gunHandedness = GunHandedness.OneHanded;
            projectile.name = "Bullat_Projectile";
            gun.DefaultModule.cooldownTime = 0.5f;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "finished small";
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.reloadTime = 1.5f;
            gun.SetBaseMaxAmmo(50);
            gun.quality = PickupObject.ItemQuality.C;
            gun.barrelOffset.transform.localPosition = new Vector3(1.25f, 1.0625f, 0f);
            gun.encounterTrackable.EncounterGuid = "batlauncher";
            gun.gunClass = GunClass.CHARGE;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).frames[1].FrameToDefinition().MakeOffset(new Vector2(-0.0625f, 0.0625f));
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).frames[3].FrameToDefinition().MakeOffset(new Vector2(-0.0625f, 0.0625f));
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_ENM_bullat_tackle_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).frames[0].eventAudio = "Play_ENM_bullat_charge_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).frames[0].triggerEvent = true;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 2;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToTrorkShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(22);
            AdvancedInfiniteAmmoSynergyProcessor processor = gun.gameObject.AddComponent<AdvancedInfiniteAmmoSynergyProcessor>();
            processor.RequiredSynergy = "#KING_BULLAT_SHOOTER";
            processor.PreventsReload = false;
        }
    }
}
