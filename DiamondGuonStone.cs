using System;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;
using SpecialItemPack.ItemAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace SpecialItemPack
{
    public class DiamondGuonStone : AdvancedPlayerOrbitalItem
    {
        public static void Init()
        {
            string name = "Diamond Guon Stone";
            string resourcePath = "SpecialItemPack/Resources/DiamondGuonStone";
            GameObject gameObject = new GameObject(name);
            var item = gameObject.AddComponent<DiamondGuonStone>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject, true);
            string shortDesc = "Shiny!!!";
            string longDesc = "Prevents almost all glass guon stones from breaking. Also increases chance of getting Glass Guon Stones upon clearing a room.\n\nThis Guon Stone was owned by the elite knight from the Glass Kingdom. Glass Guon Stones respect this one very much, and will try their all to not break.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = PickupObject.ItemQuality.A;
            DiamondGuonStone.BuildPrefab();
            DiamondGuonStone.BuildSynergyPrefab();
            item.OrbitalPrefab = DiamondGuonStone.orbitalPrefab;
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(270);
            item.HasAdvancedUpgradeSynergy = true;
            item.AdvancedUpgradeSynergy = "#DIAMONDER_GUON_STONE";
            item.AdvancedUpgradeOrbitalPrefab = DiamondGuonStone.upgradeOrbitalPrefab.gameObject;
        }

        protected void LateUpdate()
        {
            if(this.m_pickedUp && this.m_owner != null)
            {
                int count = 0;
                foreach(PassiveItem passive in this.m_owner.passiveItems)
                {
                    if(passive is IounStoneOrbitalItem)
                    {
                        if((passive as IounStoneOrbitalItem).BreaksUponOwnerDamage)
                        {
                            count += 1;
                        }
                    }
                }
                this.guonCount = count;
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += this.RecoverGuons;
            player.OnRoomClearEvent += this.MagnetGuons;
        }

        private void MagnetGuons(PlayerController player)
        {
            if (!player.CurrentRoom.PlayerHasTakenDamageInThisRoom)
            {
                if(UnityEngine.Random.value <= (this.m_advancedSynergyUpgradeActive ? 0.1f : 0.05f))
                {
                    LootEngine.SpawnItem(PickupObjectDatabase.GetById(565).gameObject, player.CenterPosition, Vector2.up, 1f, true, false, false);
                }
            }
        }

        private void RecoverGuons(PlayerController player)
        {
            GameManager.Instance.StartCoroutine(this.RecoverGuonsCoroutine(player, this.guonCount));
        }

        private IEnumerator RecoverGuonsCoroutine(PlayerController player, int count)
        {
            yield return null;
            if(UnityEngine.Random.value <= 0.15f && !this.m_advancedSynergyUpgradeActive)
            {
                count -= 1;
            }
            if(count <= 0)
            {
                yield break;
            }
            for(int i=0; i<count; i++)
            {
                player.AcquirePassiveItemPrefabDirectly(PickupObjectDatabase.GetById(565) as PassiveItem);
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnReceivedDamage -= this.RecoverGuons;
            player.OnRoomClearEvent -= this.MagnetGuons;
            return base.Drop(player);
        }

        public static void BuildPrefab()
        {
            bool flag = DiamondGuonStone.orbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonOrbitals/DiamondGuonStoneOrbital", null, true);
                gameObject.name = "Diamond Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(7, 10));
                DiamondGuonStone.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                DiamondGuonStone.orbitalPrefab.shouldRotate = false;
                DiamondGuonStone.orbitalPrefab.orbitRadius = 2.5f;
                DiamondGuonStone.orbitalPrefab.orbitDegreesPerSecond = 90f;
                DiamondGuonStone.orbitalPrefab.orbitDegreesPerSecond = 120f;
                DiamondGuonStone.orbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public static void BuildSynergyPrefab()
        {
            bool flag = DiamondGuonStone.upgradeOrbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonStrongOrbitals/DiamondGuonStrongOrbital", null, true);
                gameObject.name = "Synergy Diamond Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(9, 15));
                DiamondGuonStone.upgradeOrbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                DiamondGuonStone.upgradeOrbitalPrefab.shouldRotate = false;
                DiamondGuonStone.upgradeOrbitalPrefab.orbitRadius = 2.5f;
                DiamondGuonStone.upgradeOrbitalPrefab.orbitDegreesPerSecond = 90f;
                DiamondGuonStone.upgradeOrbitalPrefab.orbitDegreesPerSecond = 120f;
                DiamondGuonStone.upgradeOrbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        private int guonCount = 0;
        public static PlayerOrbital orbitalPrefab;
        public static PlayerOrbital upgradeOrbitalPrefab;
    }
}
