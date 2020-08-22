using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using SpecialItemPack.AdaptedSynergyStuff;

namespace SpecialItemPack
{
    class BigGunController : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Big Gun", "big_gun");
            Game.Items.Rename("outdated_gun_mods:big_gun", "spapi:big_gun");
            gun.gameObject.AddComponent<BigGunController>();
            GunExt.SetShortDescription(gun, "A Gun That's Big");
            GunExt.SetLongDescription(gun, "A big gun.\n\nIn comparison with the Magnum, this one is bigger.");
            GunExt.SetupSprite(gun, null, "big_gun_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.AddProjectileModuleFrom(gun, PickupObjectDatabase.GetById(601) as Gun, true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(601) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.transform.parent = gun.barrelOffset;
            projectile.baseData.damage *= 2f;
            projectile.baseData.speed *= 2f;
            projectile.name = "BigGun_Projectile";
            gun.DefaultModule.cooldownTime = 0.15f;
            gun.DefaultModule.numberOfShotsInClip = 3;
            gun.reloadTime = 2.1f;
            gun.SetBaseMaxAmmo(60);
            gun.quality = PickupObject.ItemQuality.S;
            gun.barrelOffset.transform.localPosition = new Vector3(1.90f, 1.0f, 0f);
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            gun.encounterTrackable.EncounterGuid = "oh_look_this_gun_is_big";
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToTrorkShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(601);
            AdvancedDualWieldSynergyProcessor dualWieldController = gun.gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            dualWieldController.SynergyNameToCheck = "#BIG_GUN_SHOTGUN-GUN";
            dualWieldController.PartnerGunID = 601;
        }

        private void LateUpdate()
        {
            if (this.gun && this.gun.IsReloading && this.gun.CurrentOwner is PlayerController)
            {
                PlayerController playerController = this.gun.CurrentOwner as PlayerController;
                if (playerController.CurrentRoom != null)
                {
                    playerController.CurrentRoom.ApplyActionToNearbyEnemies(playerController.CenterPosition, 8f, new Action<AIActor, float>(this.ProcessEnemy));
                }
            }
        }

        private void ProcessEnemy(AIActor target, float distance)
        {
            for (int i = 0; i < this.TargetEnemies.Count; i++)
            {
                if (target.EnemyGuid == this.TargetEnemies[i])
                {
                    GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(target));
                    target.EraseFromExistence(true);
                    break;
                }
            }
        }

        private IEnumerator HandleEnemySuck(AIActor target)
        {
            Transform copySprite = this.CreateEmptySprite(target);
            Vector3 startPosition = copySprite.transform.position;
            float elapsed = 0f;
            float duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                if (this.gun && copySprite)
                {
                    Vector3 position = this.gun.PrimaryHandAttachPoint.position;
                    float t = elapsed / duration * (elapsed / duration);
                    copySprite.position = Vector3.Lerp(startPosition, position, t);
                    copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                    copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                }
                yield return null;
            }
            if (copySprite)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
            }
            if (this.gun)
            {
                this.gun.GainAmmo(1);
            }
            yield break;
        }

        private Transform CreateEmptySprite(AIActor target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            if (target.optionalPalette != null)
            {
                tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
            }
            return gameObject2.transform;
        }

        public List<string> TargetEnemies = new List<string>
        {
            "db35531e66ce41cbb81d507a34366dfe",
            "01972dee89fc4404a5c408d50007dad5",
            "70216cae6c1346309d86d4a0b4603045",
            "88b6b6a93d4b4234a67844ef4728382c",
            "df7fb62405dc4697b7721862c7b6b3cd",
            "47bdfec22e8e4568a619130a267eab5b",
            "3cadf10c489b461f9fb8814abc1a09c1",
            "8bb5578fba374e8aae8e10b754e61d62",
            "e5cffcfabfae489da61062ea20539887",
            "1a78cfb776f54641b832e92c44021cf2",
            "d4a9836f8ab14f3fadd0f597438b1f1f",
            "5f3abc2d561b4b9c9e72b879c6f10c7e",
            "844657ad68894a4facb1b8e1aef1abf9",
            "2feb50a6a40f4f50982e89fd276f6f15"
        };
    }
}