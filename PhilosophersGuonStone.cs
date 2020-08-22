using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;
using MonoMod.RuntimeDetour;

namespace SpecialItemPack
{
    class PhilosophersGuonStone : ActivePlayerOrbital
    {
        public static void Init()
        {
            string itemName = "Philosopher's Guon Stone";
            string resourceName = "SpecialItemPack/Resources/PhilosophersGuonStone";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<PhilosophersGuonStone>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Pure Transmutation";
            string longDesc = "Turns blocked enemy bullets into ammo while active.\n\nAncient Guon Stone, many years searched by almost all treasure hunters. It has a divine effect, that no one can refuse - it turns bullets into more bullets!";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 300);
            item.consumable = false;
            item.quality = ItemQuality.C;
            PhilosophersGuonStone.BuildPrefab();
            PhilosophersGuonStone.BuildSynergyPrefab();
            item.OrbitalPrefab = PhilosophersGuonStone.orbitalPrefab;
            Game.Items.Rename("spapi:philosopher's_guon_stone", "spapi:philosophers_guon_stone");
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(270);
            item.HasAdvancedUpgradeSynergy = true;
            item.AdvancedUpgradeSynergy = "#PHILOSOPHERS_GREAT_GUON_STONE";
            item.AdvancedUpgradeOrbitalPrefab = PhilosophersGuonStone.upgradeOrbitalPrefab.gameObject;
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            this.StartCoroutine(ItemBuilder.HandleDuration(this, 20, user, null));
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            if (this.m_extantOrbital != null)
            {
                this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.RefillAmmo);
            }
            player.OnNewFloorLoaded += this.RecoverAbility;
        }

        private void RecoverAbility(PlayerController player)
        {
            if (this.m_extantOrbital != null)
            {
                this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.RefillAmmo);
            }
        }

        private void RefillAmmo(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
        {
            if (otherRigidbody.projectile != null)
            {
                if (this.m_isCurrentlyActive)
                {
                    this.LastOwner.CurrentGun.GainAmmo(this.m_advancedSynergyUpgradeActive ? 7 : 3);
                    if (this.m_advancedSynergyUpgradeActive)
                    {
                        this.LastOwner.CurrentGun.MoveBulletsIntoClip(3);
                    }
                }
            }
        }

        protected override void OnPreDrop(PlayerController player)
        {
            if (this.m_extantOrbital != null)
            {
                this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.RefillAmmo);
            }
            player.OnNewFloorLoaded -= this.RecoverAbility;
            base.OnPreDrop(player);
        }

        public override void Update()
        {
            base.Update();
            if (this.m_extantOrbital != null)
            {
                if ((this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody.OnPreRigidbodyCollision - new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.RefillAmmo)) ==
                    this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody.OnPreRigidbodyCollision)
                {
                    this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.RefillAmmo);
                }
            }
        }
        public static void BuildPrefab()
        {
            bool flag = PhilosophersGuonStone.orbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonOrbitals/PhilosophersGuonStoneOrbital", null, true);
                gameObject.name = "Philosophers Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(5, 8));
                PhilosophersGuonStone.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                PhilosophersGuonStone.orbitalPrefab.shouldRotate = false;
                PhilosophersGuonStone.orbitalPrefab.orbitRadius = 2.5f;
                PhilosophersGuonStone.orbitalPrefab.orbitDegreesPerSecond = 90f;
                PhilosophersGuonStone.orbitalPrefab.orbitDegreesPerSecond = 120f;
                PhilosophersGuonStone.orbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public static void BuildSynergyPrefab()
        {
            bool flag = PhilosophersGuonStone.upgradeOrbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonStrongOrbitals/PhilosophersGuonStrongOrbital", null, true);
                gameObject.name = "Synergy Philosophers Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(11, 19));
                PhilosophersGuonStone.upgradeOrbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                PhilosophersGuonStone.upgradeOrbitalPrefab.shouldRotate = false;
                PhilosophersGuonStone.upgradeOrbitalPrefab.orbitRadius = 2.5f;
                PhilosophersGuonStone.upgradeOrbitalPrefab.orbitDegreesPerSecond = 90f;
                PhilosophersGuonStone.upgradeOrbitalPrefab.orbitDegreesPerSecond = 120f;
                PhilosophersGuonStone.upgradeOrbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user);
        }

        public static PlayerOrbital orbitalPrefab;
        public static PlayerOrbital upgradeOrbitalPrefab;
    }
}
