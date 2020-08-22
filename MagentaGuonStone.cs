using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dungeonator;
using SpecialItemPack.ItemAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace SpecialItemPack
{
    public class MagentaGuonStone : AdvancedPlayerOrbitalItem
    {
        public static void Init()
        {
            string name = "Magenta Guon Stone";
            string resourcePath = "SpecialItemPack/Resources/MagentaGuonStone";
            GameObject gameObject = new GameObject(name);
            var item = gameObject.AddComponent<MagentaGuonStone>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject, true);
            string shortDesc = "Immaculately Balanced";
            string longDesc = "Having curse increases firerate, while not having curse increases damage.\n\nThis guon stone was made upon guidance of the Jammed, and blessed by Kaliber herself afterwards. It connects in itself curse and blessing, two " +
                "opposite elements, and transforms both of them into useful benefits.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = PickupObject.ItemQuality.B;
            MagentaGuonStone.BuildPrefab();
            MagentaGuonStone.BuildSynergyPrefab();
            item.OrbitalPrefab = MagentaGuonStone.orbitalPrefab;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(270);
            item.HasAdvancedUpgradeSynergy = true;
            item.AdvancedUpgradeSynergy = "#MAGENTER_GUON_STONE";
            item.AdvancedUpgradeOrbitalPrefab = MagentaGuonStone.upgradeOrbitalPrefab.gameObject;
        }

        public static void BuildPrefab()
        {
            bool flag = MagentaGuonStone.orbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonOrbitals/MagentaGuonStoneOrbital", null, true);
                gameObject.name = "Magenta Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(5, 8));
                MagentaGuonStone.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                MagentaGuonStone.orbitalPrefab.shouldRotate = false;
                MagentaGuonStone.orbitalPrefab.orbitRadius = 2.5f;
                MagentaGuonStone.orbitalPrefab.orbitDegreesPerSecond = 90f;
                MagentaGuonStone.orbitalPrefab.orbitDegreesPerSecond = 120f;
                MagentaGuonStone.orbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public static void BuildSynergyPrefab()
        {
            bool flag = MagentaGuonStone.upgradeOrbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonStrongOrbitals/MagentaGuonStrongOrbital", null, true);
                gameObject.name = "Synergy Magenta Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(9, 12));
                MagentaGuonStone.upgradeOrbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                MagentaGuonStone.upgradeOrbitalPrefab.shouldRotate = false;
                MagentaGuonStone.upgradeOrbitalPrefab.orbitRadius = 2.5f;
                MagentaGuonStone.upgradeOrbitalPrefab.orbitDegreesPerSecond = 90f;
                MagentaGuonStone.upgradeOrbitalPrefab.orbitDegreesPerSecond = 120f;
                MagentaGuonStone.upgradeOrbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        protected override void Update()
        {
            base.Update();
            if(this.m_pickedUp && this.m_owner != null)
            {
                this.curse = this.m_owner.stats.GetStatValue(PlayerStats.StatType.Curse);
                if(this.curse != this.lastCurse)
                {
                    this.RemoveStat(PlayerStats.StatType.Damage);
                    this.RemoveStat(PlayerStats.StatType.RateOfFire);
                    this.RemoveStat(PlayerStats.StatType.ReloadSpeed);
                    float actualCurse = curse;
                    if(actualCurse > 10f)
                    {
                        actualCurse = 10f;
                    }
                    if (actualCurse < 0f)
                    {
                        actualCurse = 0f;
                    }
                    float damage = (-actualCurse + 10f) / 30f;
                    float gunSpeed = actualCurse / 20f;
                    this.AddStat(PlayerStats.StatType.Damage, damage, StatModifier.ModifyMethod.ADDITIVE);
                    this.AddStat(PlayerStats.StatType.ReloadSpeed, -gunSpeed, StatModifier.ModifyMethod.ADDITIVE);
                    this.AddStat(PlayerStats.StatType.RateOfFire, gunSpeed, StatModifier.ModifyMethod.ADDITIVE);
                    this.m_owner.stats.RecalculateStats(this.m_owner, true, false);
                    this.lastCurse = this.curse;
                }
            }
            if(this.m_extantOrbital != null && this.m_advancedSynergyUpgradeActive)
            {
                if(this.m_extantOrbital.transform.position.GetAbsoluteRoom() != null)
                {
                    float num = -1f;
                    AIActor aiactor = this.m_extantOrbital.transform.position.GetAbsoluteRoom().GetNearestEnemy(this.m_extantOrbital.GetComponent<tk2dBaseSprite>().WorldCenter, out num, true, true);
                    if(aiactor != null)
                    {
                        if(this.cooldownRemaining <= 0)
                        {
                            Gun gun = Toolbox.GetGunById(MagentaGunIds[UnityEngine.Random.Range(0, MagentaGunIds.Count)]);
                            if(gun != null)
                            {
                                Projectile proj = null;
                                if(gun.alternateVolley != null)
                                {
                                    proj = gun.alternateVolley.projectiles[0].projectiles[0];
                                }
                                else if(gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Charged)
                                {
                                    proj = (UnityEngine.Random.value <= 0.5f ? gun.DefaultModule.chargeProjectiles[0].Projectile : gun.DefaultModule.chargeProjectiles[1].Projectile);
                                }
                                else
                                {
                                    proj = gun.DefaultModule.GetCurrentProjectile();
                                }
                                if(gun.DefaultModule.shootStyle == ProjectileModule.ShootStyle.Beam)
                                {
                                    GameManager.Instance.StartCoroutine(this.HandleFireShortBeam(proj, 2.5f));
                                }
                                else
                                {
                                    GameObject obj = SpawnManager.SpawnProjectile(proj.gameObject, this.m_extantOrbital.GetComponent<tk2dBaseSprite>().WorldCenter,
                                        Quaternion.Euler(0, 0, BraveMathCollege.Atan2Degrees(aiactor.sprite.WorldCenter - this.m_extantOrbital.GetComponent<tk2dSprite>().WorldCenter)));
                                    Projectile proj2 = obj.GetComponent<Projectile>();
                                    if(proj2 != null)
                                    {
                                        if(this.m_owner != null)
                                        {
                                            proj2.Owner = this.m_owner;
                                            proj2.Shooter = this.m_owner.specRigidbody;
                                            float actualCurse = curse;
                                            if (actualCurse > 10f)
                                            {
                                                actualCurse = 10f;
                                            }
                                            if (actualCurse < 0f)
                                            {
                                                actualCurse = 0f;
                                            }
                                            float damage = (-actualCurse + 10f) / 30f;
                                            proj2.baseData.damage *= damage;
                                        }
                                    }
                                }
                                this.cooldownRemaining = 10f;
                                if(this.m_owner != null)
                                {
                                    float actualCurse = curse;
                                    if (actualCurse > 10f)
                                    {
                                        actualCurse = 10f;
                                    }
                                    if (actualCurse < 0f)
                                    {
                                        actualCurse = 0f;
                                    }
                                    float gunSpeed = actualCurse / 20f;
                                    this.cooldownRemaining /= (1 + gunSpeed);
                                }
                            }
                        }
                    }
                    this.cooldownRemaining = Mathf.Max(0, this.cooldownRemaining - BraveTime.DeltaTime);
                }
            }
        }

        private IEnumerator HandleFireShortBeam(Projectile projectileToSpawn, float duration)
        {
            float elapsed = 0f;
            float num2 = -1f;
            AIActor aiactor2 = this.m_extantOrbital.transform.position.GetAbsoluteRoom().GetNearestEnemy(this.m_extantOrbital.GetComponent<tk2dBaseSprite>().WorldCenter, out num2, true, true);
            BeamController beam = this.BeginFiringBeam(projectileToSpawn, this.m_owner, BraveMathCollege.Atan2Degrees(aiactor2.sprite.WorldCenter - this.m_extantOrbital.GetComponent<tk2dSprite>().WorldCenter), 
                this.m_extantOrbital.GetComponent<tk2dBaseSprite>().WorldCenter);
            yield return null;
            while (elapsed < duration)
            {
                if (!this || !this.m_extantOrbital || this.m_extantOrbital.transform.position.GetAbsoluteRoom() == null)
                {
                    break;
                }
                float num = -1f;
                AIActor aiactor = this.m_extantOrbital.transform.position.GetAbsoluteRoom().GetNearestEnemy(this.m_extantOrbital.GetComponent<tk2dBaseSprite>().WorldCenter, out num, true, true);
                if(aiactor == null)
                {
                    break;
                }
                elapsed += BraveTime.DeltaTime;
                this.ContinueFiringBeam(beam, BraveMathCollege.Atan2Degrees(aiactor.sprite.WorldCenter - this.m_extantOrbital.GetComponent<tk2dSprite>().WorldCenter), this.m_extantOrbital.GetComponent<tk2dBaseSprite>().WorldCenter);
                yield return null;
            }
            this.CeaseBeam(beam);
            yield break;
        }

        private BeamController BeginFiringBeam(Projectile projectileToSpawn, PlayerController player, float targetAngle, Vector2 overrideSpawnPoint)
        {
            Vector2 vector = overrideSpawnPoint;
            GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, vector, Quaternion.identity, true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.Owner = player;
            BeamController component2 = gameObject.GetComponent<BeamController>();
            component2.Owner = player;
            component2.HitsPlayers = false;
            component2.HitsEnemies = true;
            Vector3 v = BraveMathCollege.DegreesToVector(targetAngle, 1f);
            component2.Direction = v;
            component2.Origin = vector;
            return component2;
        }

        private void ContinueFiringBeam(BeamController beam, float angle, Vector2 overrideSpawnPoint)
        {
            Vector2 vector = overrideSpawnPoint;
            beam.Direction = BraveMathCollege.DegreesToVector(angle, 1f);
            beam.Origin = vector;
            beam.LateUpdatePosition(vector);
        }

        private void CeaseBeam(BeamController beam)
        {
            beam.CeaseAttack();
        }

        private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier statModifier = new StatModifier();
            statModifier.amount = amount;
            statModifier.statToBoost = statType;
            statModifier.modifyType = method;
            foreach (StatModifier statModifier2 in this.passiveStatModifiers)
            {
                bool flag = statModifier2.statToBoost == statType;
                if (flag)
                {
                    return;
                }
            }
            bool flag2 = this.passiveStatModifiers == null;
            if (flag2)
            {
                this.passiveStatModifiers = new StatModifier[]
                {
                    statModifier
                };
                return;
            }
            this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[]
            {
                statModifier
            }).ToArray<StatModifier>();
        }

        private void RemoveStat(PlayerStats.StatType statType)
        {
            List<StatModifier> list = new List<StatModifier>();
            for (int i = 0; i < this.passiveStatModifiers.Length; i++)
            {
                bool flag = this.passiveStatModifiers[i].statToBoost != statType;
                if (flag)
                {
                    list.Add(this.passiveStatModifiers[i]);
                }
            }
            this.passiveStatModifiers = list.ToArray();
        }

        public static PlayerOrbital orbitalPrefab;
        public static PlayerOrbital upgradeOrbitalPrefab;
        private float curse = 0;
        private float lastCurse = -90909049202;
        private float cooldownRemaining = 0f;
        public static List<int> MagentaGunIds = new List<int>
        {
            395,
            475,
            504,
            670,
            595,
            52
        };
    }
}
