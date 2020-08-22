using System;
using System.Collections;
using Dungeonator;
using System.Collections.Generic;
using SpecialItemPack.ItemAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace SpecialItemPack
{
    public class DarkGreenGuonStone : AdvancedPlayerOrbitalItem
    {
        public static void Init()
        {
            string name = "Dark Green Guon Stone";
            string resourcePath = "SpecialItemPack/Resources/DarkGreenGuonStone";
            GameObject gameObject = new GameObject(name);
            var item = gameObject.AddComponent<DarkGreenGuonStone>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject, true);
            string shortDesc = "Full of Thorns";
            string longDesc = "Throws deadly spikes at nearby enemies.\n\nThis Guon Stone was secretally grown in the Kaliber's private garden by the king of the Undiscovered Chamber. The plant it was grew on was some sort of combination of cactus and " +
                "rose, and because both plants have spikes, the spikes of this Guon Stone are so sharp, literally no one can survive multiple spikes at a time.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = PickupObject.ItemQuality.A;
            DarkGreenGuonStone.BuildPrefab();
            DarkGreenGuonStone.BuildSynergyPrefab();
            item.OrbitalPrefab = DarkGreenGuonStone.orbitalPrefab;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1.5f, StatModifier.ModifyMethod.ADDITIVE);
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(270);
            item.HasAdvancedUpgradeSynergy = true;
            item.AdvancedUpgradeSynergy = "#DARKER_GREEN_GUON_STONE";
            item.AdvancedUpgradeOrbitalPrefab = DarkGreenGuonStone.upgradeOrbitalPrefab.gameObject;
        }

        public static void BuildPrefab()
        {
            bool flag = DarkGreenGuonStone.orbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonOrbitals/DarkGreenGuonStoneOrbital", null, true);
                gameObject.name = "Dark Green Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(8, 8));
                DarkGreenGuonStone.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                DarkGreenGuonStone.orbitalPrefab.shouldRotate = false;
                DarkGreenGuonStone.orbitalPrefab.orbitRadius = 2.5f;
                DarkGreenGuonStone.orbitalPrefab.orbitDegreesPerSecond = 90f;
                DarkGreenGuonStone.orbitalPrefab.orbitDegreesPerSecond = 120f;
                DarkGreenGuonStone.orbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public static void BuildSynergyPrefab()
        {
            bool flag = DarkGreenGuonStone.upgradeOrbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonStrongOrbitals/DarkGreenGuonStrongOrbital", null, true);
                gameObject.name = "Synergy Dark Green Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(12, 12));
                DarkGreenGuonStone.upgradeOrbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                DarkGreenGuonStone.upgradeOrbitalPrefab.shouldRotate = false;
                DarkGreenGuonStone.upgradeOrbitalPrefab.orbitRadius = 2.5f;
                DarkGreenGuonStone.upgradeOrbitalPrefab.orbitDegreesPerSecond = 90f;
                DarkGreenGuonStone.upgradeOrbitalPrefab.orbitDegreesPerSecond = 120f;
                DarkGreenGuonStone.upgradeOrbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        protected override void Update()
        {
            base.Update();
            if(this.m_extantOrbital != null)
            {
                this.cooldown -= BraveTime.DeltaTime;
                if(this.m_extantOrbital.transform.position.GetAbsoluteRoom() != null && this.m_extantOrbital.transform.position.GetAbsoluteRoom().HasActiveEnemies(RoomHandler.ActiveEnemyType.All) && cooldown <= 0f)
                {
                    AIActor aiactor = null;
                    float nearestDistance = float.MaxValue;
                    List<AIActor> activeEnemies = this.m_extantOrbital.transform.position.GetAbsoluteRoom().GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                    AIActor result;
                    for (int i = 0; i < activeEnemies.Count; i++)
                    {
                        AIActor aiactor2 = activeEnemies[i];;
                        bool flag3 = !aiactor2.healthHaver.IsDead;
                        if (flag3)
                        {
                            float num = Vector2.Distance(this.m_extantOrbital.transform.position, aiactor2.CenterPosition);
                            bool flag5 = num < nearestDistance;
                            if (flag5)
                            {
                                nearestDistance = num;
                                aiactor = aiactor2;
                            }
                        }
                    }
                    result = aiactor;
                    if(result != null)
                    {
                        GameObject obj = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(this.m_advancedSynergyUpgradeActive ? 156 : 124) as Gun).DefaultModule.projectiles[0].gameObject, this.m_extantOrbital.GetComponent<tk2dSprite>().WorldCenter,
                            Quaternion.Euler(0, 0, BraveMathCollege.Atan2Degrees(result.sprite.WorldCenter - this.m_extantOrbital.GetComponent<tk2dSprite>().WorldCenter)));
                        Projectile proj = obj.GetComponent<Projectile>();
                        if(proj != null)
                        {
                            if(this.m_owner != null)
                            {
                                proj.Owner = this.m_owner;
                                proj.Shooter = this.m_owner.specRigidbody;
                            }
                            proj.baseData.damage *= (this.m_advancedSynergyUpgradeActive ? 0.15f : 0.65f);
                        }
                        this.cooldown = 0.2f;
                    }
                }
            }
        }

        public static PlayerOrbital orbitalPrefab;
        public static PlayerOrbital upgradeOrbitalPrefab;
        private float cooldown = 0;
    }
}
