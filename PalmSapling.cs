using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;

namespace SpecialItemPack
{
    class PalmSapling : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Palm Sapling";
            string resourceName = "SpecialItemPack/Resources/PalmSapling";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ScrollOfWonder>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Wondawondawonder!";
            string longDesc = "A transformative spell of incredible power.\n\nThe wizard Alben Smallbore theorized that the more power was put into a spell, the less could be known about its outcome. This spell is immensely powerful.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 100);
            PalmSapling.BuildPrefab();
            item.consumable = false;
            item.quality = ItemQuality.S;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(250);
        }

        public static void BuildPrefab()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/PalmTree", null, true);
            gameObject.name = "Palm Tree";
            SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(new IntVector2(15, 0), new IntVector2(10, 101));
            speculativeRigidbody.CollideWithTileMap = false;
            speculativeRigidbody.CollideWithOthers = true;
            speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
            speculativeRigidbody.PrimaryPixelCollider.CollisionLayer |= CollisionLayer.BulletBlocker;
            PixelCollider pixelCollider = new PixelCollider();
            pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            pixelCollider.CollisionLayer = CollisionLayer.PlayerBlocker;
            pixelCollider.CollisionLayer |= CollisionLayer.EnemyBlocker;
            pixelCollider.ManualWidth = 10;
            pixelCollider.ManualHeight = 10;
            pixelCollider.ManualOffsetX = 15;
            pixelCollider.ManualOffsetY = 0;
            speculativeRigidbody.PixelColliders.Add(pixelCollider);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            FakePrefab.MarkAsFakePrefab(gameObject);
            gameObject.SetActive(false);
            PalmPrefab = gameObject;
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user);
        }

        public static GameObject PalmPrefab;
    }
}
