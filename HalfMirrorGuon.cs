using System;
using System.Collections;
using Dungeonator;
using SpecialItemPack.ItemAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace SpecialItemPack
{
    public class HalfMirrorGuon : AdvancedPlayerOrbitalItem
    {
        public static void Init()
        {
            string name = "Polarizing Guon Stone";
            string resourcePath = "SpecialItemPack/Resources/HalfMirrorGuonStone";
            GameObject gameObject = new GameObject(name);
            var item = gameObject.AddComponent<HalfMirrorGuon>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject, true);
            string shortDesc = "Transparent And Not";
            string longDesc = "Reflects normal bullets. Doesn't block jammed shots.\n\nGuon Stone specifically enchanted in the Glass Kingdom, imperfect version of the fabled Mirror Guon Stone. The materaial it is made of with the enchantment can reflect" +
                " only specific types of matter, and others just pierce through it.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = PickupObject.ItemQuality.A;
            HalfMirrorGuon.BuildPrefab();
            HalfMirrorGuon.BuildSynergyPrefab();
            item.OrbitalPrefab = HalfMirrorGuon.orbitalPrefab;
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(270);
            item.HasAdvancedUpgradeSynergy = true;
            item.AdvancedUpgradeSynergy = "#POLARIZINGER_GUON_STONE";
            item.AdvancedUpgradeOrbitalPrefab = HalfMirrorGuon.upgradeOrbitalPrefab.gameObject;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            if(this.m_extantOrbital != null)
            {
                this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.MaybeReflect);
            }
        }

        private void RecoverAbility(PlayerController player)
        {
            if (this.m_extantOrbital != null)
            {
                this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.MaybeReflect);
            }
        }

        protected override void Update()
        {
            base.Update();
            if(this.m_extantOrbital != null)
            {
                if((this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody.OnPreRigidbodyCollision - new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.MaybeReflect)) ==
                    this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody.OnPreRigidbodyCollision)
                {
                    this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.MaybeReflect);
                }
            }
        }

        private void MaybeReflect(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
        {
            if(otherRigidbody.projectile != null)
            {
                Projectile proj = otherRigidbody.projectile;
                if (!proj.IsBlackBullet)
                {
                    PassiveReflectItem.ReflectBullet(proj, true, this.m_owner, 20f, 1, 1.25f, 0);
                }
                else if (this.m_advancedSynergyUpgradeActive)
                {
                    PassiveReflectItem.ReflectBullet(proj, true, this.m_owner, 10f, 1f, 1f, 0f);
                }
                PhysicsEngine.SkipCollision = true;
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            if(this.m_extantOrbital != null)
            {
                this.m_extantOrbital.GetComponent<PlayerOrbital>().specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.MaybeReflect);
            }
            player.OnNewFloorLoaded -= this.RecoverAbility;
            return base.Drop(player);
        }

        public static void BuildPrefab()
        {
            bool flag = HalfMirrorGuon.orbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonOrbitals/HalfMirrorGuonStoneOrbital", null, true);
                gameObject.name = "Half Mirror Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(15, 15));
                HalfMirrorGuon.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                HalfMirrorGuon.orbitalPrefab.shouldRotate = false;
                HalfMirrorGuon.orbitalPrefab.orbitRadius = 2.5f;
                HalfMirrorGuon.orbitalPrefab.orbitDegreesPerSecond = 90f;
                HalfMirrorGuon.orbitalPrefab.orbitDegreesPerSecond = 120f;
                HalfMirrorGuon.orbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public static void BuildSynergyPrefab()
        {
            bool flag = HalfMirrorGuon.upgradeOrbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonStrongOrbitals/HalfMirrorGuonStrongOrbital", null, true);
                gameObject.name = "Synergy Half Mirror Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(17, 17));
                HalfMirrorGuon.upgradeOrbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                HalfMirrorGuon.upgradeOrbitalPrefab.shouldRotate = false;
                HalfMirrorGuon.upgradeOrbitalPrefab.orbitRadius = 2.5f;
                HalfMirrorGuon.upgradeOrbitalPrefab.orbitDegreesPerSecond = 90f;
                HalfMirrorGuon.upgradeOrbitalPrefab.orbitDegreesPerSecond = 120f;
                HalfMirrorGuon.upgradeOrbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public static PlayerOrbital orbitalPrefab;
        public static PlayerOrbital upgradeOrbitalPrefab;
    }
}
