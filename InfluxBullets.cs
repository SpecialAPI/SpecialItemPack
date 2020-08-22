using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;
using System.Collections;
using Dungeonator;

namespace SpecialItemPack
{
    class InfluxBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Influx Bullets";
            string resourceName = "SpecialItemPack/Resources/InfluxBullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<InfluxBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Alien Technology";
            string longDesc = "Bullets create more bullets when hitting an enemy.\n\nBullets created on Mars, they were used by aliens.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.A;
            item.AddToTrorkShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(407);
            projectileToSpawn = (PickupObjectDatabase.GetById(531) as ComplexProjectileModifier).CollisionSpawnProjectile;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.DoInflux;
        }

        private void DoInflux(Projectile proj, float f)
        {
            proj.OnHitEnemy += this.SpawnAdditionalProjectile;
        }

        public void SpawnAdditionalProjectile(Projectile proj, SpeculativeRigidbody hitEnemy, bool fatal)
        {
            if (UnityEngine.Random.value <= 0.35f)
            {
                float randomAngle = UnityEngine.Random.Range(0f, 360f);
                Vector2 spawnPosition = hitEnemy.UnitCenter + BraveMathCollege.DegreesToVector(randomAngle, 3f);
                float spawnAngle = randomAngle - 180f;
                GameObject obj = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, spawnPosition, Quaternion.Euler(0f, 0f, spawnAngle), true);
                Projectile proj2 = obj.GetComponent<Projectile>();
                if (proj2 != null)
                {
                    proj2.Owner = proj.Owner;
                    proj2.Shooter = proj.Shooter;
                    if (proj.sprite != null)
                    {
                        proj2.shouldRotate = proj.shouldRotate;
                        proj2.shouldFlipHorizontally = proj.shouldFlipHorizontally;
                        proj2.shouldFlipVertically = proj.shouldFlipVertically;
                        proj2.sprite.SetSprite(proj.sprite.Collection, proj.sprite.spriteId);
                        Vector2 vector = proj2.transform.position.XY() - proj2.sprite.WorldCenter;
                        proj2.transform.position += vector.ToVector3ZUp(0f);
                        proj2.specRigidbody.Reinitialize();
                    }
                    proj2.baseData.damage = proj.baseData.damage;
                    proj2.baseData.speed = proj.baseData.speed;
                    proj2.baseData.force = proj.baseData.force;
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.DoInflux;
            return base.Drop(player);
        }

        public static Projectile projectileToSpawn;
    }
}
