using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialItemPack
{
    class AdditionalCompanionOwner : BraveBehaviour
    {
        public void Initialize()
        {
            (base.gameActor as PlayerController).OnNewFloorLoaded += this.RegenerateCompanion;
        }

        private void RegenerateCompanion(PlayerController owner)
        {
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(CompanionGuid);
            GameObject gameObject = Instantiate(orLoadByGuid.gameObject, owner.transform.position, Quaternion.identity);
            CompanionController orAddComponent = gameObject.GetOrAddComponent<CompanionController>();
            if (IsBlackPhantom)
            {
                gameObject.GetComponent<AIActor>().BecomeBlackPhantom();
            }
            orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
            orAddComponent.Initialize(owner);
            orAddComponent.behaviorSpeculator.MovementBehaviors.Add(new CompanionFollowPlayerBehavior());
            AIActor aiactor = gameObject.GetComponent<AIActor>();
            if (orAddComponent.healthHaver != null)
            {
                orAddComponent.healthHaver.PreventAllDamage = true;
            }
            if (orAddComponent.bulletBank != null)
            {
                orAddComponent.bulletBank.OnProjectileCreated += PokeballItem.CatchProjectileBehaviour.OnPostProcessProjectile;
            }
            if (orAddComponent.aiShooter != null)
            {
                orAddComponent.aiShooter.PostProcessProjectile += PokeballItem.CatchProjectileBehaviour.OnPostProcessProjectile;
            }
        }

        public string CompanionGuid;
        public bool IsBlackPhantom;
    }
}
