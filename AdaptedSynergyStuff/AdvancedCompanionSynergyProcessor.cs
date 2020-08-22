using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialItemPack.AdaptedSynergyStuff
{
    public class AdvancedCompanionSynergyProcessor : MonoBehaviour
    {
        public GameObject ExtantCompanion
        {
            get
            {
                return this.m_extantCompanion;
            }
        }

        private void CreateCompanion(PlayerController owner)
        {
            if (this.PreventRespawnOnFloorLoad)
            {
                return;
            }
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(this.CompanionGuid);
            Vector3 position = owner.transform.position;
            GameObject extantCompanion = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, position, Quaternion.identity);
            this.m_extantCompanion = extantCompanion;
            CompanionController orAddComponent = this.m_extantCompanion.GetOrAddComponent<CompanionController>();
            orAddComponent.Initialize(owner);
            if (orAddComponent.specRigidbody)
            {
                PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody, null, false);
            }
        }

        private void DestroyCompanion()
        {
            if (!this.m_extantCompanion)
            {
                return;
            }
            UnityEngine.Object.Destroy(this.m_extantCompanion);
            this.m_extantCompanion = null;
        }

        private void Awake()
        {
            this.m_gun = base.GetComponent<Gun>();
            this.m_item = base.GetComponent<PassiveItem>();
            this.m_activeItem = base.GetComponent<PlayerItem>();
        }

        public void Update()
        {
            PlayerController playerController = this.ManuallyAssignedPlayer;
            if (!playerController && this.m_item)
            {
                playerController = this.m_item.Owner;
            }
            if (!playerController && this.m_activeItem && this.m_activeItem.PickedUp && this.m_activeItem.LastOwner)
            {
                playerController = this.m_activeItem.LastOwner;
            }
            if (!playerController && this.m_gun && this.m_gun.CurrentOwner is PlayerController)
            {
                playerController = (this.m_gun.CurrentOwner as PlayerController);
            }
            if (playerController && (this.RequiresNoSynergy || playerController.PlayerHasActiveSynergy(this.RequiredSynergy)) && !this.m_active)
            {
                this.m_active = true;
                this.m_cachedPlayer = playerController;
                this.ActivateSynergy(playerController);
            }
            else if (!playerController || (!this.RequiresNoSynergy && !playerController.PlayerHasActiveSynergy(this.RequiredSynergy) && this.m_active))
            {
                this.DeactivateSynergy();
                this.m_active = false;
            }
        }

        private void OnDisable()
        {
            if (this.m_active && !this.PersistsOnDisable)
            {
                this.DeactivateSynergy();
                this.m_active = false;
            }
            else if (this.m_active && this.m_cachedPlayer)
            {
                this.m_cachedPlayer.StartCoroutine(this.HandleDisabledChecks());
            }
        }

        private IEnumerator HandleDisabledChecks()
        {
            yield return null;
            while (this && this.m_cachedPlayer && !this.isActiveAndEnabled && this.m_active)
            {
                if (!this.m_cachedPlayer.PlayerHasActiveSynergy(this.RequiredSynergy))
                {
                    this.DeactivateSynergy();
                    this.m_active = false;
                    yield break;
                }
                yield return null;
            }
            yield break;
        }

        private void OnDestroy()
        {
            if (this.m_active)
            {
                this.DeactivateSynergy();
                this.m_active = false;
            }
        }

        public void ActivateSynergy(PlayerController player)
        {
            player.OnNewFloorLoaded = (Action<PlayerController>)Delegate.Combine(player.OnNewFloorLoaded, new Action<PlayerController>(this.HandleNewFloor));
            this.CreateCompanion(player);
        }

        private void HandleNewFloor(PlayerController obj)
        {
            this.DestroyCompanion();
            if (!this.PreventRespawnOnFloorLoad)
            {
                this.CreateCompanion(obj);
            }
        }

        public void DeactivateSynergy()
        {
            if (this.m_cachedPlayer != null)
            {
                PlayerController cachedPlayer = this.m_cachedPlayer;
                cachedPlayer.OnNewFloorLoaded = (Action<PlayerController>)Delegate.Remove(cachedPlayer.OnNewFloorLoaded, new Action<PlayerController>(this.HandleNewFloor));
                this.m_cachedPlayer = null;
            }
            this.DestroyCompanion();
        }

        [LongNumericEnum]
        public string RequiredSynergy;

        public bool RequiresNoSynergy;

        public bool PersistsOnDisable;

        [EnemyIdentifier]
        public string CompanionGuid;

        [NonSerialized]
        public bool PreventRespawnOnFloorLoad;

        private Gun m_gun;

        private PassiveItem m_item;

        private PlayerItem m_activeItem;

        [NonSerialized]
        public PlayerController ManuallyAssignedPlayer;

        private GameObject m_extantCompanion;

        private bool m_active;

        private PlayerController m_cachedPlayer;
    }

}
