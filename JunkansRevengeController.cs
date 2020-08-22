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
    class JunkansRevengeController : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Junkan's Revenge", "junken");
            Game.Items.Rename("outdated_gun_mods:junkan's_revenge", "spapi:junkans_revenge");
            gun.gameObject.AddComponent<JunkansRevengeController>();
            GunExt.SetShortDescription(gun, "Next Time, Show No Mercy!");
            GunExt.SetLongDescription(gun, "Shoots Ser Junkan's Squires, that will promise to tear apart your enemies!");
            GunExt.SetupSprite(gun, null, "junken_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(35) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.name = "Junkan_Projectile";
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.transform.parent = gun.barrelOffset;
            projectile.SetProjectileSpriteRight("junken_proj_001", 12, 13, false, tk2dBaseSprite.Anchor.LowerLeft, false, false, 8, 10, 5, 1);
            projectile.gameObject.AddComponent<SpawnJunkanProjectileBehaviour>();
            projectile.baseData.damage = 6f;
            gun.clipObject = Toolbox.CreateCustomClip("junken_clip_001", 12, 13);
            gun.reloadClipLaunchFrame = 0;
            gun.DefaultModule.cooldownTime = 0.625f;
            gun.DefaultModule.numberOfShotsInClip = 6;
            gun.reloadTime = 1.75f;
            gun.SetBaseMaxAmmo(250);
            gun.quality = PickupObject.ItemQuality.S;
            gun.barrelOffset.transform.localPosition = new Vector3(1.65f, 0.55f, 0f);
            gun.muzzleFlashEffects = Toolbox.GetGunById(81).muzzleFlashEffects;
            gun.gunSwitchGroup = Toolbox.GetGunById(150).gunSwitchGroup;
            gun.encounterTrackable.EncounterGuid = "djunkans_rhevenge";
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.SetupUnlockOnFlag(GungeonFlags.ITEMSPECIFIC_SER_JUNKAN_MAXLVL, true);
            gun.PlaceItemInAmmonomiconAfterItemById(376);
        }

        protected override void OnPickedUpByPlayer(PlayerController player)
        {
            base.OnPickup(player);
            if(this.gun.GetComponent<CantGiveJunkBehaviour>() == null)
            {
                LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(127).gameObject, player);
                this.gun.gameObject.AddComponent<CantGiveJunkBehaviour>();
            }
        }

        private class SpawnJunkanProjectileBehaviour : BraveBehaviour
        {
            private void Start()
            {
                base.projectile.OnDestruction += this.SpawnJunkan;
            }

            private void SpawnJunkan(Projectile proj)
            {
                if(proj.Owner == null || !(proj.Owner is PlayerController))
                {
                    return;
                }
                PlayerController player = proj.Owner as PlayerController;
                if(player.CurrentGun.GetComponent<JunkansRevengeController>() == null)
                {
                    return;
                }
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("c6c8e59d0f5d41969c74e802c9d67d07");
                Vector3 vector = proj.sprite.WorldCenter.ToVector3ZUp(0f);
                GameObject enemyObj = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, vector, Quaternion.identity);
                if(enemyObj.GetComponent<AIActor>() != null)
                {
                    if(enemyObj.GetComponent<AIActor>().sprite != null)
                    {
                        enemyObj.GetComponent<AIActor>().sprite.PlaceAtPositionByAnchor(vector, tk2dBaseSprite.Anchor.MiddleCenter);
                        if (enemyObj.GetComponent<AIActor>().specRigidbody != null)
                        {
                            enemyObj.GetComponent<AIActor>().specRigidbody.Reinitialize();
                        }
                    }
                }
                CompanionController orAddComponent = enemyObj.GetOrAddComponent<CompanionController>();
                orAddComponent.Initialize(player);
                enemyObj.AddComponent<TempraryJunkanBehaviour>();
            }

            private class TempraryJunkanBehaviour : BraveBehaviour
            {
                private void Start()
                {
                    base.aiActor.StartCoroutine(this.HandleDuration());
                }

                private IEnumerator HandleDuration()
                {
                    float duration = 2f;
                    float elapsed = 0f;
                    while(elapsed < duration)
                    {
                        elapsed += BraveTime.DeltaTime;
                        if(base.aiActor.CompanionOwner == null || base.aiActor.CompanionOwner.CurrentGun == null || base.aiActor.CompanionOwner.CurrentGun.GetComponent<JunkansRevengeController>() == null)
                        {
                            break;
                        }
                        yield return null;
                    }
                    LootEngine.DoDefaultItemPoof(base.aiActor.sprite.WorldCenter);
                    base.aiActor.EraseFromExistence(true);
                    yield break;
                }
            }
        }

        private class CantGiveJunkBehaviour : MonoBehaviour
        {
        }
    }
}