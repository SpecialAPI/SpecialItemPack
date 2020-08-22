using System;
using System.Collections;
using Dungeonator;
using SpecialItemPack.ItemAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Gungeon;

namespace SpecialItemPack
{
    public class WoodenGuon : AdvancedPlayerOrbitalItem
    {
        public static void Init()
        {
            string name = "Wooden Guon Stone (pavlov.andrei.d)";
            string resourcePath = "SpecialItemPack/Resources/WoodenGuonStone";
            GameObject gameObject = new GameObject(name);
            var item = gameObject.AddComponent<WoodenGuon>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject, true);
            string shortDesc = "Lazy Work";
            string longDesc = "Blocks bullets. That's it.\n\nIn difference of all other guon stones, that are made out of crystals or stone and enchanted with magic, this one is made out of wood and enchanted only with the lazyness of it's creator.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = PickupObject.ItemQuality.D;
            WoodenGuon.BuildPrefab();
            WoodenGuon.BuildSynergyPrefab();
            item.OrbitalPrefab = WoodenGuon.orbitalPrefab;
            Game.Items.Rename("spapi:wooden_guon_stone_(pavlov.andrei.d)", "spapi:wooden_guon_stone");
            item.SetName("Wooden Guon Stone");
            item.PlaceItemInAmmonomiconAfterItemById(270);
            item.HasAdvancedUpgradeSynergy = true;
            item.AdvancedUpgradeSynergy = "#WOODENER_GUON_STONE";
            item.AdvancedUpgradeOrbitalPrefab = WoodenGuon.upgradeOrbitalPrefab.gameObject;
        }

        public static void BuildPrefab()
        {
            bool flag = WoodenGuon.orbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonOrbitals/WoodenGuonStoneOrbital", null, true);
                gameObject.name = "Wooden Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(8, 8));
                WoodenGuon.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                WoodenGuon.orbitalPrefab.shouldRotate = false;
                WoodenGuon.orbitalPrefab.orbitRadius = 2.5f;
                WoodenGuon.orbitalPrefab.orbitDegreesPerSecond = 90f;
                WoodenGuon.orbitalPrefab.orbitDegreesPerSecond = 120f;
                WoodenGuon.orbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public static void BuildSynergyPrefab()
        {
            bool flag = WoodenGuon.upgradeOrbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonStrongOrbitals/WoodenGuonStrongOrbital", null, true);
                gameObject.name = "Synergy Wooden Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(12, 12));
                WoodenGuon.upgradeOrbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                WoodenGuon.upgradeOrbitalPrefab.shouldRotate = false;
                WoodenGuon.upgradeOrbitalPrefab.orbitRadius = 2.5f;
                WoodenGuon.upgradeOrbitalPrefab.orbitDegreesPerSecond = 90f;
                WoodenGuon.upgradeOrbitalPrefab.orbitDegreesPerSecond = 120f;
                WoodenGuon.upgradeOrbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public static PlayerOrbital orbitalPrefab;
        public static PlayerOrbital upgradeOrbitalPrefab;
    }
}
