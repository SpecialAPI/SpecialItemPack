using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class CyberkineticSuperbattery : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Cyberkinetic Superbattery";
            string resourceName = "SpecialItemPack/Resources/CyberkineticSuperbattery";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<CyberkineticSuperbattery>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "UNLIMITED POWER!!!";
            string longDesc = "Charges when the player is moving and shocks nearby enemies when charged.\n\nThis battery contains a Mini-Sprun inside, and is capable of creating 1,2000 volts per second with only a little help.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.A;
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(155);
            item.SetupUnlockOnStat(TrackedStats.RUNS_PLAYED_POST_FTA, DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN, 6);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            for(int i=0; i<2; i++)
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
                    if (zapperMainProjectile != null)
                    {
                        proj.gameObject.AddComponent<ZapperSecondaryProjectileBehaviour>().manuallyAssignedEnemy = null;
                        proj.gameObject.GetComponent<ZapperSecondaryProjectileBehaviour>().manuallyAssignedPlayer = this.m_owner;
                    }
                    else
                    {
                        proj.gameObject.AddComponent<TeslaCoil.ZapperMainProjectileBehaviour>().manuallyAssignedPlayer = this.m_owner;
                    }
                    ComplexProjectileModifier projModifier = PickupObjectDatabase.GetById(298) as ComplexProjectileModifier;
                    ChainLightningModifier orAddComponent = obj.gameObject.GetOrAddComponent<ChainLightningModifier>();
                    orAddComponent.LinkVFXPrefab = Toolbox.GetGunById(682).DefaultModule.projectiles[0].GetComponent<ChainLightningModifier>().LinkVFXPrefab;
                    orAddComponent.damageTypes = projModifier.ChainLightningDamageTypes;
                    orAddComponent.maximumLinkDistance = projModifier.ChainLightningMaxLinkDistance;
                    orAddComponent.damagePerHit = projModifier.ChainLightningDamagePerHit;
                    orAddComponent.damageCooldown = projModifier.ChainLightningDamageCooldown / 5;
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
                if(zapperMainProjectile != null)
                {
                    this.zapperSecondaryProjectile = proj;
                }
                else
                {
                    this.zapperMainProjectile = proj;
                }
            }
            player.OnReceivedDamage += this.OnPlayerReceivedDamage;
        }

        private void OnPlayerReceivedDamage(PlayerController player)
        {
            this.m_movementElapsed = 0f;
            if (this.m_isCurrentlyActive)
            {
                this.DisableVfx();
                this.m_isCurrentlyActive = false;
            }
        }

        protected override void Update()
        {
            base.Update();
            if(this.m_pickedUp && this.m_owner != null)
            {
                if(this.m_owner.Velocity.magnitude > 0.05f && !this.m_owner.IsDodgeRolling)
                {
                    this.m_movementElapsed += BraveTime.DeltaTime;
                    if(this.m_movementElapsed > 2f && !this.m_isCurrentlyActive)
                    {
                        this.EnableVfx();
                        this.m_isCurrentlyActive = true;
                    }
                    if(this.m_isCurrentlyActive && this.m_currentSprunSpinVfx == null)
                    {
                        this.m_currentSprunSpinVfx = Toolbox.CreateSprenSpun(this.m_owner.CenterPosition);
                        this.m_currentSprunSpinVfx.transform.parent = this.m_owner.transform;
                        this.m_currentSprunSpinVfx.GetComponent<Toolbox.SprenSpunBehaviour>().ChangeDirection(Toolbox.SprenSpunBehaviour.SprenSpunRotateType.BACKWARDS);
                    }
                    if(this.m_isCurrentlyActive && this.m_owner.CurrentRoom != null)
                    {
                        float num = -1f;
                        AIActor aiactor = this.m_owner.CurrentRoom.GetNearestEnemy(this.m_owner.CenterPosition, out num, true, true);
                        if(aiactor != null && this.zapperSecondaryProjectile != null)
                        {
                            this.zapperSecondaryProjectile.GetComponent<ZapperSecondaryProjectileBehaviour>().manuallyAssignedEnemy = aiactor;
                        }
                        else if(aiactor == null && this.zapperSecondaryProjectile != null)
                        {
                            this.zapperSecondaryProjectile.GetComponent<ZapperSecondaryProjectileBehaviour>().manuallyAssignedEnemy = null;
                        }
                    }
                }
                else
                {
                    this.m_movementElapsed = 0f;
                    if (this.m_isCurrentlyActive)
                    {
                        this.DisableVfx();
                        this.m_isCurrentlyActive = false;
                    }
                    if (this.zapperSecondaryProjectile != null)
                    {
                        this.zapperSecondaryProjectile.GetComponent<ZapperSecondaryProjectileBehaviour>().manuallyAssignedEnemy = null;
                    }
                }
                if(this.zapperMainProjectile == null || this.zapperSecondaryProjectile == null)
                {
                    if (this.zapperMainProjectile != null)
                    {
                        UnityEngine.Object.Destroy(this.zapperMainProjectile.gameObject);
                    }
                    if (this.zapperSecondaryProjectile != null)
                    {
                        UnityEngine.Object.Destroy(this.zapperSecondaryProjectile.gameObject);
                    }
                    for (int i = 0; i < 2; i++)
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
                            if (zapperMainProjectile != null)
                            {
                                proj.gameObject.AddComponent<ZapperSecondaryProjectileBehaviour>().manuallyAssignedEnemy = null;
                                proj.gameObject.GetComponent<ZapperSecondaryProjectileBehaviour>().manuallyAssignedPlayer = this.m_owner;
                            }
                            else
                            {
                                proj.gameObject.AddComponent<TeslaCoil.ZapperMainProjectileBehaviour>().manuallyAssignedPlayer = this.m_owner;
                            }
                            ComplexProjectileModifier projModifier = PickupObjectDatabase.GetById(298) as ComplexProjectileModifier;
                            ChainLightningModifier orAddComponent = obj.gameObject.GetOrAddComponent<ChainLightningModifier>();
                            orAddComponent.LinkVFXPrefab = Toolbox.GetGunById(682).DefaultModule.projectiles[0].GetComponent<ChainLightningModifier>().LinkVFXPrefab;
                            orAddComponent.damageTypes = projModifier.ChainLightningDamageTypes;
                            orAddComponent.maximumLinkDistance = projModifier.ChainLightningMaxLinkDistance;
                            orAddComponent.damagePerHit = projModifier.ChainLightningDamagePerHit;
                            orAddComponent.damageCooldown = projModifier.ChainLightningDamageCooldown / 5;
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
                        if (zapperMainProjectile != null)
                        {
                            this.zapperSecondaryProjectile = proj;
                        }
                        else
                        {
                            this.zapperMainProjectile = proj;
                        }
                    }
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            if (this.zapperMainProjectile != null)
            {
                UnityEngine.Object.Destroy(this.zapperMainProjectile.gameObject);
            }
            if (this.zapperSecondaryProjectile != null)
            {
                UnityEngine.Object.Destroy(this.zapperSecondaryProjectile.gameObject);
            }
            player.OnReceivedDamage -= this.OnPlayerReceivedDamage;
            return base.Drop(player);
        }

        private void EnableVfx()
        {
            AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Active_01", this.gameObject);
            this.m_currentSprunSpinVfx = Toolbox.CreateSprenSpun(this.m_owner.CenterPosition);
            this.m_currentSprunSpinVfx.transform.parent = this.m_owner.transform;
            this.m_currentSprunSpinVfx.GetComponent<Toolbox.SprenSpunBehaviour>().ChangeDirection(Toolbox.SprenSpunBehaviour.SprenSpunRotateType.BACKWARDS);
            Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(this.m_owner.sprite);
            if (outlineMaterial != null)
            {
                outlineMaterial.SetColor("_OverrideColor", new Color(144f, 246f, 180f));
            }
        }

        private void DisableVfx()
        {
            AkSoundEngine.PostEvent("Play_ITM_Macho_Brace_Fade_01", this.gameObject);
            if(this.m_currentSprunSpinVfx != null)
            {
                Destroy(this.m_currentSprunSpinVfx);
            }
            Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(this.m_owner.sprite);
            if (outlineMaterial != null)
            {
                outlineMaterial.SetColor("_OverrideColor", new Color(0f, 0f, 0f));
            }
        }

        private float m_movementElapsed = 0f;
        private bool m_isCurrentlyActive = false;
        private GameObject m_currentSprunSpinVfx;
        private Projectile zapperMainProjectile = null;
        private Projectile zapperSecondaryProjectile = null;

        public class ZapperSecondaryProjectileBehaviour : BraveBehaviour
        {
            private void Update()
            {
                if (this.manuallyAssignedEnemy == null || (this.manuallyAssignedEnemy.healthHaver != null && this.manuallyAssignedEnemy.healthHaver.IsDead))
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
                else
                {
                    base.transform.position = this.manuallyAssignedEnemy.sprite.WorldCenter.ToVector3ZisY(0f);
                    base.specRigidbody.Reinitialize();
                }
            }

            public AIActor manuallyAssignedEnemy;
            public PlayerController manuallyAssignedPlayer;
        }
    }
}