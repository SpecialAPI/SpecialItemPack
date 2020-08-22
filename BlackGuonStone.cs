using System;
using System.Collections;
using Dungeonator;
using SpecialItemPack.ItemAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace SpecialItemPack
{
    public class BlackGuonStone : AdvancedPlayerOrbitalItem
    {
        public static void Init()
        {
            string name = "Black Guon Stone";
            string resourcePath = "SpecialItemPack/Resources/BlackGuonStone";
            GameObject gameObject = new GameObject(name);
            var item = gameObject.AddComponent<BlackGuonStone>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject, true);
            string shortDesc = "Damned Rock";
            string longDesc = "Emits an aura of death. Enemies that die in it will also kill another enemies because of the strong voodoo magic.\n\nThe most powerful Guon Stone. In fact so powerful, that the Jammed itself drained it's color. " +
                "It still contains souls of those who have fallen by it's power.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = PickupObject.ItemQuality.S;
            BlackGuonStone.BuildPrefab();
            BlackGuonStone.BuildSynergyPrefab();
            item.OrbitalPrefab = BlackGuonStone.orbitalPrefab;
            item.AddToCursulaShop();
            item.AddToBlacksmithShop();
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 3, StatModifier.ModifyMethod.ADDITIVE);
            item.PlaceItemInAmmonomiconAfterItemById(270);
            item.HasAdvancedUpgradeSynergy = true;
            item.AdvancedUpgradeSynergy = "#BLACKER_GUON_STONE";
            item.AdvancedUpgradeOrbitalPrefab = BlackGuonStone.upgradeOrbitalPrefab.gameObject;
        }

        public static void BuildPrefab()
        {
            bool flag = BlackGuonStone.orbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonOrbitals/BlackGuonStoneOrbital", null, true);
                gameObject.name = "Black Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(10, 10));
                BlackGuonStone.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                BlackGuonStone.orbitalPrefab.shouldRotate = false;
                BlackGuonStone.orbitalPrefab.orbitRadius = 2.5f;
                BlackGuonStone.orbitalPrefab.orbitDegreesPerSecond = 90f;
                BlackGuonStone.orbitalPrefab.orbitDegreesPerSecond = 120f;
                BlackGuonStone.orbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public static void BuildSynergyPrefab()
        {
            bool flag = BlackGuonStone.upgradeOrbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonStrongOrbitals/BlackGuonStrongOrbital", null, true);
                gameObject.name = "Synergy Black Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(14, 14));
                BlackGuonStone.upgradeOrbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                BlackGuonStone.upgradeOrbitalPrefab.shouldRotate = false;
                BlackGuonStone.upgradeOrbitalPrefab.orbitRadius = 2.5f;
                BlackGuonStone.upgradeOrbitalPrefab.orbitDegreesPerSecond = 90f;
                BlackGuonStone.upgradeOrbitalPrefab.orbitDegreesPerSecond = 120f;
                BlackGuonStone.upgradeOrbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        private void HandleRadialIndicator(bool isOnPlayer)
        {
            if (isOnPlayer)
            {
                if (!this.m_playerRadialIndicatorActive)
                {
                    this.m_playerRadialIndicatorActive = true;
                    this.m_playerRadialIndicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), this.m_owner.sprite.WorldCenter.ToVector3ZisY(0f),
                        Quaternion.identity, this.m_owner.transform)).GetComponent<HeatIndicatorController>();
                    this.m_playerRadialIndicator.IsFire = false;
                    this.m_playerRadialIndicator.CurrentColor = new Color(0.25f, 0.25f, 0.25f);
                }
            }
            else
            {
                if (!this.m_radialIndicatorActive)
                {
                    this.m_radialIndicatorActive = true;
                    this.m_radialIndicator = ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), this.m_extantOrbital.GetComponent<tk2dBaseSprite>().WorldCenter.ToVector3ZisY(0f),
                        Quaternion.identity, this.m_extantOrbital.transform)).GetComponent<HeatIndicatorController>();
                    this.m_radialIndicator.IsFire = false;
                    this.m_radialIndicator.CurrentColor = new Color(0.25f, 0.25f, 0.25f);
                }
            }
        }

        private void UnhandleRadialIndicator(bool isOnPlayer)
        {
            if (isOnPlayer)
            {
                if (this.m_playerRadialIndicatorActive)
                {
                    this.m_playerRadialIndicatorActive = false;
                    if (this.m_playerRadialIndicator)
                    {
                        this.m_playerRadialIndicator.EndEffect();
                    }
                    this.m_playerRadialIndicator = null;
                }
            }
            else
            {
                if (this.m_radialIndicatorActive)
                {
                    this.m_radialIndicatorActive = false;
                    if (this.m_radialIndicator)
                    {
                        this.m_radialIndicator.EndEffect();
                    }
                    this.m_radialIndicator = null;
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            if (this.m_extantOrbital != null)
            {
                if (this.m_radialIndicator == null && this.m_radialIndicatorActive)
                {
                    this.m_radialIndicatorActive = false;
                }
                if (!this.m_radialIndicatorActive)
                {
                    this.HandleRadialIndicator(false);
                }
                this.m_radialIndicator.CurrentRadius = this.m_advancedSynergyUpgradeActive ? 3.5f : 2.5f;
                this.m_extantOrbital.transform.position.GetAbsoluteRoom().ApplyActionToNearbyEnemies(this.m_extantOrbital.GetComponent<tk2dBaseSprite>().WorldCenter, this.m_advancedSynergyUpgradeActive ? 3.5f : 2.5f, this.Die);
            }
            else
            {
                if (this.m_radialIndicatorActive)
                {
                    this.UnhandleRadialIndicator(false);
                }
            }
            if(this.m_owner != null)
            {
                if(this.m_playerRadialIndicator == null && this.m_playerRadialIndicatorActive)
                {
                    this.m_playerRadialIndicatorActive = false;
                }
                if (!this.m_playerRadialIndicatorActive)
                {
                    this.HandleRadialIndicator(true);
                }
                this.m_playerRadialIndicator.CurrentRadius = this.m_advancedSynergyUpgradeActive ? 2.5f : 1.5f;
                this.m_owner.CurrentRoom.ApplyActionToNearbyEnemies(this.m_owner.sprite.WorldCenter, this.m_advancedSynergyUpgradeActive ? 2.5f : 1.5f, this.Die);
            }
            else
            {
                if (this.m_playerRadialIndicatorActive)
                {
                    this.UnhandleRadialIndicator(true);
                }
            }
        }

        private void Die(AIActor target, float f)
        {
            if(target == null)
            {
                return;
            }
            if(UnityEngine.Random.value <= 0.15f)
            {
                if(target != null && target.healthHaver != null && !target.healthHaver.IsBoss && target.healthHaver.IsAlive)
                {
                    target.healthHaver.ApplyDamage(float.MaxValue, Vector2.zero, "Terrible Fate", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                    if (target.ParentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All) && UnityEngine.Random.value <= 0.5f)
                    {
                        AIActor target2 = target.ParentRoom.GetRandomActiveEnemy(true);
                        if(target2 != null && target2.healthHaver != null && !target2.healthHaver.IsBoss && target2.healthHaver.IsAlive)
                        {
                            target2.healthHaver.ApplyDamage(float.MaxValue, Vector2.zero, "Voodoo", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                        }
                        else if (target2 != null && target2.healthHaver != null && target2.healthHaver.IsBoss && target2.healthHaver.IsAlive)
                        {
                            target2.healthHaver.ApplyDamage(target2.healthHaver.GetCurrentHealth()/2, Vector2.zero, "Voodoo", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                        }
                        if (target2.ParentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All) && UnityEngine.Random.value <= 0.15f)
                        {
                            AIActor target3 = target2.ParentRoom.GetRandomActiveEnemy(true);
                            if (target3 != null && target3.healthHaver != null && !target3.healthHaver.IsBoss && target3.healthHaver.IsAlive)
                            {
                                target3.healthHaver.ApplyDamage(float.MaxValue, Vector2.zero, "Voodoo", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                            }
                            else if (target3 != null && target3.healthHaver != null && target3.healthHaver.IsBoss && target3.healthHaver.IsAlive)
                            {
                                target3.healthHaver.ApplyDamage(target2.healthHaver.GetCurrentHealth() / 2, Vector2.zero, "Voodoo", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                            }
                        }
                    }
                }
            }
        }

        public static PlayerOrbital orbitalPrefab;
        public static PlayerOrbital upgradeOrbitalPrefab;
        private bool m_radialIndicatorActive;
        private bool m_playerRadialIndicatorActive;
        private HeatIndicatorController m_radialIndicator;
        private HeatIndicatorController m_playerRadialIndicator;
    }
}
