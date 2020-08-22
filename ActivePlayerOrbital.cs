using System;
using UnityEngine;
using System.Collections.Generic;

namespace SpecialItemPack
{
    public class ActivePlayerOrbital : PlayerItem
    {
        public static GameObject CreateOrbital(PlayerController owner, GameObject targetOrbitalPrefab, bool isFollower)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(targetOrbitalPrefab, owner.transform.position, Quaternion.identity);
            if (!isFollower)
            {
                PlayerOrbital component = gameObject.GetComponent<PlayerOrbital>();
                component.Initialize(owner);
                component.SourceItem = null;
            }
            else
            {
                PlayerOrbitalFollower component2 = gameObject.GetComponent<PlayerOrbitalFollower>();
                if (component2)
                {
                    component2.Initialize(owner);
                }
            }
            return gameObject;
        }

        private void CreateOrbital(PlayerController owner)
        {
            GameObject targetOrbitalPrefab = (!(this.OrbitalPrefab != null)) ? this.OrbitalFollowerPrefab.gameObject : this.OrbitalPrefab.gameObject;
            if (this.HasUpgradeSynergy && this.m_synergyUpgradeActive)
            {
                targetOrbitalPrefab = ((!(this.UpgradeOrbitalPrefab != null)) ? this.UpgradeOrbitalFollowerPrefab.gameObject : this.UpgradeOrbitalPrefab.gameObject);
            }
            if (this.HasAdvancedUpgradeSynergy && this.m_advancedSynergyUpgradeActive)
            {
                targetOrbitalPrefab = ((!(this.AdvancedUpgradeOrbitalPrefab != null)) ? this.AdvancedUpgradeOrbitalFollowerPrefab.gameObject : this.AdvancedUpgradeOrbitalPrefab.gameObject);
            }
            this.m_extantOrbital = ActivePlayerOrbital.CreateOrbital(owner, targetOrbitalPrefab, this.OrbitalFollowerPrefab != null);
            if (this.BreaksUponContact && this.m_extantOrbital)
            {
                SpeculativeRigidbody component = this.m_extantOrbital.GetComponent<SpeculativeRigidbody>();
                if (component)
                {
                    SpeculativeRigidbody speculativeRigidbody = component;
                    speculativeRigidbody.OnRigidbodyCollision = (SpeculativeRigidbody.OnRigidbodyCollisionDelegate)Delegate.Combine(speculativeRigidbody.OnRigidbodyCollision, new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleBreakOnCollision));
                }
            }
            if (this.BreaksUponOwnerDamage && owner)
            {
                owner.OnReceivedDamage += this.HandleBreakOnOwnerDamage;
            }
        }

        private void HandleBreakOnOwnerDamage(PlayerController arg1)
        {
            if (!this)
            {
                return;
            }
            if (this.BreakVFX && this.m_extantOrbital && this.m_extantOrbital.GetComponentInChildren<tk2dSprite>())
            {
                SpawnManager.SpawnVFX(this.BreakVFX, this.m_extantOrbital.GetComponentInChildren<tk2dSprite>().WorldCenter.ToVector3ZisY(0f), Quaternion.identity);
            }
            if (this.LastOwner)
            {
                this.LastOwner.RemoveActiveItem(this.PickupObjectId);
                this.LastOwner.OnReceivedDamage -= this.HandleBreakOnOwnerDamage;
            }
            UnityEngine.Object.Destroy(base.gameObject);
        }

        private void HandleBreakOnCollision(CollisionData rigidbodyCollision)
        {
            if (this.LastOwner)
            {
                this.LastOwner.RemoveActiveItem(this.PickupObjectId);
            }
            UnityEngine.Object.Destroy(base.gameObject);
        }

        public void DecoupleOrbital()
        {
            this.m_extantOrbital = null;
            if (this.BreaksUponOwnerDamage && this.LastOwner)
            {
                this.LastOwner.OnReceivedDamage -= this.HandleBreakOnOwnerDamage;
            }
        }

        private void DestroyOrbital()
        {
            if (!this.m_extantOrbital)
            {
                return;
            }
            if (this.BreaksUponOwnerDamage && this.LastOwner)
            {
                this.LastOwner.OnReceivedDamage -= this.HandleBreakOnOwnerDamage;
            }
            UnityEngine.Object.Destroy(this.m_extantOrbital.gameObject);
            this.m_extantOrbital = null;
        }

        public override void Update()
        {
            base.Update();
            if (this.HasUpgradeSynergy)
            {
                if (this.m_synergyUpgradeActive && (!this.LastOwner || !this.LastOwner.HasActiveBonusSynergy(this.UpgradeSynergy, false)))
                {
                    if (this.LastOwner)
                    {
                        for (int i = 0; i < this.synergyModifiers.Length; i++)
                        {
                            this.LastOwner.healthHaver.damageTypeModifiers.Remove(this.synergyModifiers[i]);
                        }
                    }
                    this.m_synergyUpgradeActive = false;
                    this.DestroyOrbital();
                    if (this.LastOwner)
                    {
                        this.CreateOrbital(this.LastOwner);
                    }
                }
                else if (!this.m_synergyUpgradeActive && this.LastOwner && this.LastOwner.HasActiveBonusSynergy(this.UpgradeSynergy, false))
                {
                    this.m_synergyUpgradeActive = true;
                    this.DestroyOrbital();
                    if (this.LastOwner)
                    {
                        this.CreateOrbital(this.LastOwner);
                    }
                    for (int j = 0; j < this.synergyModifiers.Length; j++)
                    {
                        this.LastOwner.healthHaver.damageTypeModifiers.Add(this.synergyModifiers[j]);
                    }
                }
            }
            if (this.HasAdvancedUpgradeSynergy)
            {
                if (this.m_advancedSynergyUpgradeActive && (!this.LastOwner || !this.LastOwner.PlayerHasActiveSynergy(this.AdvancedUpgradeSynergy)))
                {
                    if (this.LastOwner)
                    {
                        for (int i = 0; i < this.advancedSynergyModifiers.Count; i++)
                        {
                            this.LastOwner.healthHaver.damageTypeModifiers.Remove(this.advancedSynergyModifiers[i]);
                        }
                    }
                    this.m_advancedSynergyUpgradeActive = false;
                    this.DestroyOrbital();
                    if (this.LastOwner)
                    {
                        this.CreateOrbital(this.LastOwner);
                    }
                }
                else if (!this.m_advancedSynergyUpgradeActive && this.LastOwner && this.LastOwner.PlayerHasActiveSynergy(this.AdvancedUpgradeSynergy))
                {
                    this.m_advancedSynergyUpgradeActive = true;
                    this.DestroyOrbital();
                    if (this.LastOwner)
                    {
                        this.CreateOrbital(this.LastOwner);
                    }
                    for (int j = 0; j < this.advancedSynergyModifiers.Count; j++)
                    {
                        this.LastOwner.healthHaver.damageTypeModifiers.Add(this.advancedSynergyModifiers[j]);
                    }
                }
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnNewFloorLoaded = (Action<PlayerController>)Delegate.Combine(player.OnNewFloorLoaded, new Action<PlayerController>(this.HandleNewFloor));
            for (int i = 0; i < this.modifiers.Length; i++)
            {
                player.healthHaver.damageTypeModifiers.Add(this.modifiers[i]);
            }
            this.CreateOrbital(player);
        }

        private void HandleNewFloor(PlayerController obj)
        {
            this.DestroyOrbital();
            this.CreateOrbital(obj);
        }

        protected override void OnPreDrop(PlayerController player)
        {
            this.DestroyOrbital();
            player.OnNewFloorLoaded = (Action<PlayerController>)Delegate.Remove(player.OnNewFloorLoaded, new Action<PlayerController>(this.HandleNewFloor));
            for (int i = 0; i < this.modifiers.Length; i++)
            {
                player.healthHaver.damageTypeModifiers.Remove(this.modifiers[i]);
            }
            for (int j = 0; j < this.synergyModifiers.Length; j++)
            {
                player.healthHaver.damageTypeModifiers.Remove(this.synergyModifiers[j]);
            }
            base.OnPreDrop(player);
        }

        protected override void OnDestroy()
        {
            if (this.LastOwner != null)
            {
                PlayerController owner = this.LastOwner;
                owner.OnNewFloorLoaded = (Action<PlayerController>)Delegate.Remove(owner.OnNewFloorLoaded, new Action<PlayerController>(this.HandleNewFloor));
                for (int i = 0; i < this.modifiers.Length; i++)
                {
                    this.LastOwner.healthHaver.damageTypeModifiers.Remove(this.modifiers[i]);
                }
                for (int j = 0; j < this.synergyModifiers.Length; j++)
                {
                    this.LastOwner.healthHaver.damageTypeModifiers.Remove(this.synergyModifiers[j]);
                }
                this.LastOwner.OnReceivedDamage -= this.HandleBreakOnOwnerDamage;
            }
            this.DestroyOrbital();
            base.OnDestroy();
        }

        public PlayerOrbital OrbitalPrefab;
        public PlayerOrbitalFollower OrbitalFollowerPrefab;
        public bool HasUpgradeSynergy;
        [LongNumericEnum]
        public CustomSynergyType UpgradeSynergy;
        public GameObject UpgradeOrbitalPrefab;
        public GameObject UpgradeOrbitalFollowerPrefab;
        public bool CanBeMimicked;
        [Header("Random Stuff, probably for Ioun Stones")]
        public DamageTypeModifier[] modifiers;
        public DamageTypeModifier[] synergyModifiers;
        public bool BreaksUponContact;
        public bool BreaksUponOwnerDamage;
        public GameObject BreakVFX;
        protected GameObject m_extantOrbital;
        protected bool m_synergyUpgradeActive;
        public bool HasAdvancedUpgradeSynergy;
        public string AdvancedUpgradeSynergy;
        public GameObject AdvancedUpgradeOrbitalPrefab;
        public GameObject AdvancedUpgradeOrbitalFollowerPrefab;
        public List<DamageTypeModifier> advancedSynergyModifiers = new List<DamageTypeModifier>();
        protected bool m_advancedSynergyUpgradeActive;
    }
}
