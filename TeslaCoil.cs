using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;
using Gungeon;

namespace SpecialItemPack
{
    class TeslaCoil : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Tesla Coil (SpecialItemPack)";
            string resourceName = "SpecialItemPack/Resources/TeslaCoil";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<TeslaCoil>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Controllable Lightning";
            string longDesc = "Electrifies nearby enemies.\n\nA prototype of a tesla coil, this is basically just a remade version of the Thunderclap.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.S;
            item.AddToBlacksmithShop();
            item.AddToTrorkShop();
            item.PlaceItemInAmmonomiconAfterItemById(287);
            Game.Items.Rename("spapi:tesla_coil_(specialitempack)", "spapi:tesla_coil");
            item.SetName("Tesla Coil");
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            GameObject obj = SpawnManager.SpawnProjectile(Toolbox.GetGunById(38).DefaultModule.projectiles[0].gameObject, this.m_owner.sprite.WorldCenter, Quaternion.identity);
            Projectile proj = obj.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.sprite.renderer.enabled = false;
                proj.specRigidbody.CollideWithOthers = false;
                proj.specRigidbody.CollideWithTileMap = false;
                proj.baseData.damage = 0;
                proj.baseData.range = float.MaxValue;
                proj.baseData.speed = 0;
                proj.Owner = this.m_owner;
                proj.Shooter = this.m_owner.specRigidbody;
                proj.gameObject.AddComponent<ZapperMainProjectileBehaviour>().manuallyAssignedPlayer = this.m_owner;
                ComplexProjectileModifier projModifier = PickupObjectDatabase.GetById(298) as ComplexProjectileModifier;
                ChainLightningModifier orAddComponent = obj.gameObject.GetOrAddComponent<ChainLightningModifier>();
                orAddComponent.LinkVFXPrefab = projModifier.ChainLightningVFX;
                orAddComponent.damageTypes = projModifier.ChainLightningDamageTypes;
                orAddComponent.maximumLinkDistance = projModifier.ChainLightningMaxLinkDistance;
                orAddComponent.damagePerHit = projModifier.ChainLightningDamagePerHit;
                orAddComponent.damageCooldown = projModifier.ChainLightningDamageCooldown;
                if (projModifier.ChainLightningDispersalParticles != null)
                {
                    orAddComponent.UsesDispersalParticles = true;
                    orAddComponent.DispersalParticleSystemPrefab = projModifier.ChainLightningDispersalParticles;
                    orAddComponent.DispersalDensity = projModifier.ChainLightningDispersalDensity;
                    orAddComponent.DispersalMinCoherency = projModifier.ChainLightningDispersalMinCoherence;
                    orAddComponent.DispersalMaxCoherency = projModifier.ChainLightningDispersalMaxCoherence;
                }
                else
                {
                    orAddComponent.UsesDispersalParticles = false;
                }
            }
            this.zapperMainProjectile = proj;
            this.m_elecImmunity = new DamageTypeModifier
            {
                damageMultiplier = 0,
                damageType = CoreDamageTypes.Electric
            };
            player.healthHaver.damageTypeModifiers.Add(this.m_elecImmunity);
            player.PostProcessProjectile += this.OnPostProcessProjectile;
        }

        private void OnPostProcessProjectile(Projectile proj, float f)
        {
            if (this.m_owner.PlayerHasActiveSynergy("#DYNAMO-MACHINE"))
            {
                this.shotsFired++;
                if(this.shotsFired > 10)
                {
                    this.shotsFired = 0;
                }
                if(shotsFired < 10)
                {
                    proj.damageTypes |= CoreDamageTypes.Electric;
                }
                else
                {
                    if(proj.GetAnySprite() != null)
                    {
                        GoopModifier mod = proj.gameObject.AddComponent<GoopModifier>();
                        mod.goopDefinition = Toolbox.DefaultWaterGoop;
                        mod.IsSynergyContingent = false;
                        mod.SpawnGoopInFlight = true;
                        mod.InFlightSpawnFrequency = 0f;
                        mod.InFlightSpawnRadius = 0.5f;
                        mod.SpawnGoopOnCollision = false;
                        mod.SpawnAtBeamEnd = false;
                        mod.spawnOffset = Vector2.zero;
                        mod.UsesInitialDelay = false;
                        mod.SynergyViable = false;
                    }
                    else
                    {
                        proj.damageTypes |= CoreDamageTypes.Electric;
                    }
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            if(this.m_pickedUp && this.m_owner != null)
            {
                if(this.m_owner.CurrentRoom != null && this.m_owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                {
                    foreach (AIActor aiactor in this.m_owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                    {
                        if(aiactor.GetComponent<ZappedEnemyBehaviour>() == null)
                        {
                            GameObject obj = SpawnManager.SpawnProjectile(Toolbox.GetGunById(38).DefaultModule.projectiles[0].gameObject, aiactor.sprite.WorldCenter, Quaternion.identity);
                            Projectile proj = obj.GetComponent<Projectile>();
                            if(proj != null)
                            {
                                proj.sprite.renderer.enabled = false;
                                proj.specRigidbody.CollideWithOthers = false;
                                proj.specRigidbody.CollideWithTileMap = false;
                                proj.baseData.damage = 0;
                                proj.baseData.range = float.MaxValue;
                                proj.baseData.speed = 0;
                                proj.Owner = this.m_owner;
                                proj.Shooter = this.m_owner.specRigidbody;
                                proj.gameObject.AddComponent<ZapperProjectileBehaviour>().manuallyAssignedEnemy = aiactor;
                                ComplexProjectileModifier projModifier = PickupObjectDatabase.GetById(298) as ComplexProjectileModifier;
                                ChainLightningModifier orAddComponent = obj.gameObject.GetOrAddComponent<ChainLightningModifier>();
                                orAddComponent.LinkVFXPrefab = projModifier.ChainLightningVFX;
                                orAddComponent.damageTypes = projModifier.ChainLightningDamageTypes;
                                orAddComponent.maximumLinkDistance = projModifier.ChainLightningMaxLinkDistance;
                                orAddComponent.damagePerHit = projModifier.ChainLightningDamagePerHit;
                                orAddComponent.damageCooldown = projModifier.ChainLightningDamageCooldown;
                                if (projModifier.ChainLightningDispersalParticles != null)
                                {
                                    orAddComponent.UsesDispersalParticles = true;
                                    orAddComponent.DispersalParticleSystemPrefab = projModifier.ChainLightningDispersalParticles;
                                    orAddComponent.DispersalDensity = projModifier.ChainLightningDispersalDensity;
                                    orAddComponent.DispersalMinCoherency = projModifier.ChainLightningDispersalMinCoherence;
                                    orAddComponent.DispersalMaxCoherency = projModifier.ChainLightningDispersalMaxCoherence;
                                }
                                else
                                {
                                    orAddComponent.UsesDispersalParticles = false;
                                }
                            }
                            aiactor.gameObject.AddComponent<ZappedEnemyBehaviour>();
                        }
                    }
                }
                if(this.zapperMainProjectile == null)
                {
                    GameObject obj = SpawnManager.SpawnProjectile(Toolbox.GetGunById(38).DefaultModule.projectiles[0].gameObject, this.m_owner.sprite.WorldCenter, Quaternion.identity);
                    Projectile proj = obj.GetComponent<Projectile>();
                    if (proj != null)
                    {
                        proj.sprite.renderer.enabled = false;
                        proj.specRigidbody.CollideWithOthers = false;
                        proj.specRigidbody.CollideWithTileMap = false;
                        proj.baseData.damage = 0;
                        proj.baseData.range = float.MaxValue;
                        proj.baseData.speed = 0;
                        proj.Owner = this.m_owner;
                        proj.Shooter = this.m_owner.specRigidbody;
                        proj.gameObject.AddComponent<ZapperMainProjectileBehaviour>().manuallyAssignedPlayer = this.m_owner;
                        ComplexProjectileModifier projModifier = PickupObjectDatabase.GetById(298) as ComplexProjectileModifier;
                        ChainLightningModifier orAddComponent = obj.gameObject.GetOrAddComponent<ChainLightningModifier>();
                        orAddComponent.LinkVFXPrefab = projModifier.ChainLightningVFX;
                        orAddComponent.damageTypes = projModifier.ChainLightningDamageTypes;
                        orAddComponent.maximumLinkDistance = projModifier.ChainLightningMaxLinkDistance;
                        orAddComponent.damagePerHit = projModifier.ChainLightningDamagePerHit;
                        orAddComponent.damageCooldown = projModifier.ChainLightningDamageCooldown;
                        if (projModifier.ChainLightningDispersalParticles != null)
                        {
                            orAddComponent.UsesDispersalParticles = true;
                            orAddComponent.DispersalParticleSystemPrefab = projModifier.ChainLightningDispersalParticles;
                            orAddComponent.DispersalDensity = projModifier.ChainLightningDispersalDensity;
                            orAddComponent.DispersalMinCoherency = projModifier.ChainLightningDispersalMinCoherence;
                            orAddComponent.DispersalMaxCoherency = projModifier.ChainLightningDispersalMaxCoherence;
                        }
                        else
                        {
                            orAddComponent.UsesDispersalParticles = false;
                        }
                    }
                    this.zapperMainProjectile = proj;
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.OnPostProcessProjectile;
            if (this.zapperMainProjectile != null)
            {
                UnityEngine.Object.Destroy(this.zapperMainProjectile.gameObject);
            }
            player.healthHaver.damageTypeModifiers.Remove(this.m_elecImmunity);
            this.m_elecImmunity = null;
            return base.Drop(player);
        }

        private Projectile zapperMainProjectile = null;
        private int shotsFired = 0;
        private DamageTypeModifier m_elecImmunity = null;

        public class ZappedEnemyBehaviour : BraveBehaviour
        {
        }

        public class ZapperProjectileBehaviour : BraveBehaviour
        {
            private void Update()
            {
                if(this.manuallyAssignedEnemy == null || (this.manuallyAssignedEnemy.healthHaver != null && this.manuallyAssignedEnemy.healthHaver.IsDead))
                {
                    UnityEngine.Object.Destroy(base.gameObject);
                }
                else
                {
                    base.transform.position = this.manuallyAssignedEnemy.sprite.WorldCenter.ToVector3ZisY(0f);
                    base.specRigidbody.Reinitialize();
                }
            }

            public AIActor manuallyAssignedEnemy;
        }

        public class ZapperMainProjectileBehaviour : BraveBehaviour
        {
            private void Update()
            {
                if (this.manuallyAssignedPlayer == null || (this.manuallyAssignedPlayer.healthHaver != null && this.manuallyAssignedPlayer.healthHaver.IsDead))
                {
                    UnityEngine.Object.Destroy(base.gameObject);
                }
                else
                {
                    base.transform.position = this.manuallyAssignedPlayer.sprite.WorldCenter.ToVector3ZisY(0f);
                    base.specRigidbody.Reinitialize();
                }
            }

            public PlayerController manuallyAssignedPlayer;
        }
    }
}
