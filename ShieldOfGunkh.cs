using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;

namespace SpecialItemPack
{
    class ShieldOfGunkh : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Shield of Gunkh";
            string resourceName = "SpecialItemPack/Resources/ShieldOfGunkh";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ShieldOfGunkh>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "What Else Can You Wish For?";
            string longDesc = "Grants fire, electricity, poison and pit immunity, gives the owner a chance to block damage and acts like an armor itself.\n\nThis sacred shield was created by the Cult of the Aim. It was a gift to Kaliber, the " +
                "goddes of the Gungeon. As this shield couldn't be transmitted to the Kaliber's realm, it was just kept by the High Priest, the First of the Order until someone very skilled stole it...";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.A;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(65);
        }

        protected override void Update()
        {
            base.Update();
            /*if(this.m_pickedUp && this.m_owner != null)
            {
                foreach(Projectile proj in StaticReferenceManager.AllProjectiles)
                {
                    if(proj != null && proj.GetComponent<ProjectileInteractableBehaviour>() == null)
                    {
                        proj.transform.position.GetAbsoluteRoom().RegisterInteractable(proj.gameObject.AddComponent<ProjectileInteractableBehaviour>());
                    }
                }
                foreach (AIActor aiactor in StaticReferenceManager.AllEnemies)
                {
                    if (aiactor != null && aiactor.GetComponent<EnemyInteractableBehaviour>() == null)
                    {
                        aiactor.transform.position.GetAbsoluteRoom().RegisterInteractable(aiactor.gameObject.AddComponent<EnemyInteractableBehaviour>());
                    }
                }
            }*/
        }

        public override void Pickup(PlayerController player)
        {
            if(player.healthHaver != null && !this.m_pickedUpThisRun)
            {
                player.healthHaver.Armor += 3f;
            }
            base.Pickup(player);
            this.m_fireImmunity = new DamageTypeModifier
            {
                damageMultiplier = 0,
                damageType = CoreDamageTypes.Fire
            };
            this.m_poisonImmunity = new DamageTypeModifier
            {
                damageMultiplier = 0,
                damageType = CoreDamageTypes.Poison
            };
            this.m_elecImmunity = new DamageTypeModifier
            {
                damageMultiplier = 0,
                damageType = CoreDamageTypes.Electric
            };
            if (player.healthHaver != null)
            {
                player.healthHaver.damageTypeModifiers.Add(this.m_fireImmunity);
                player.healthHaver.damageTypeModifiers.Add(this.m_poisonImmunity);
                player.healthHaver.damageTypeModifiers.Add(this.m_elecImmunity);
                player.healthHaver.ModifyDamage += this.ModifyIncomingDamage;
            }
            player.ImmuneToPits.AddOverride("shield of gunkh");
        }

        private void ModifyIncomingDamage(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == EventArgs.Empty)
            {
                return;
            }
            if (UnityEngine.Random.value < ((source.gameActor.CurrentGun.PickupObjectId == 380 || source.gameActor.CurrentGun.PickupObjectId == ETGMod.Databases.Items["aegis"].PickupObjectId) ? 0.3f : 0.15f))
            {
                args.ModifiedDamage = 0f;
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            if (player.healthHaver != null)
            {
                player.healthHaver.damageTypeModifiers.Remove(this.m_fireImmunity);
                player.healthHaver.damageTypeModifiers.Remove(this.m_poisonImmunity);
                player.healthHaver.damageTypeModifiers.Remove(this.m_elecImmunity);
                player.healthHaver.ModifyDamage -= this.ModifyIncomingDamage;
            }
            this.m_fireImmunity = null;
            this.m_poisonImmunity = null;
            this.m_elecImmunity = null;
            player.ImmuneToPits.RemoveOverride("shield of gunkh");
            return base.Drop(player);
        }

        

        private DamageTypeModifier m_fireImmunity = null;
        private DamageTypeModifier m_poisonImmunity = null;
        private DamageTypeModifier m_elecImmunity = null;

        private class ProjectileInteractableBehaviour : BraveBehaviour, IPlayerInteractable
        {
            public void OnEnteredRange(PlayerController player)
            {
                if(base.sprite != null)
                {
                    SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
                }
            }

            public void OnExitRange(PlayerController player)
            {
                if(base.sprite != null)
                {
                    SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite);
                }
            }

            public void Interact(PlayerController player)
            {
                if(player != null && player.CurrentGun != null)
                {
                    player.CurrentGun.GainAmmo(1);
                    this.transform.position.GetAbsoluteRoom().DeregisterInteractable(this); 
                    if ((PickupObjectDatabase.GetById(78) as AmmoPickup).pickupVFX != null)
                    {
                        player.PlayEffectOnActor((PickupObjectDatabase.GetById(78) as AmmoPickup).pickupVFX, Vector3.zero, true, false, false);
                    }
                    UnityEngine.Object.Destroy(base.gameObject);
                    AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", base.gameObject);
                }
            }

            public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
            {
                shouldBeFlipped = false;
                return string.Empty;
            }

            public float GetOverrideMaxDistance()
            {
                return -1f;
            }

            public float GetDistanceToPoint(Vector2 point)
            {
                if (!base.sprite)
                {
                    return 1000f;
                }
                Bounds bounds = base.sprite.GetBounds();
                Vector2 vector = base.transform.position.XY() + (base.transform.rotation * bounds.min).XY();
                Vector2 a = vector + (base.transform.rotation * bounds.size).XY();
                return BraveMathCollege.DistToRectangle(point, vector, a - vector);
            }
        }

        private class EnemyInteractableBehaviour : BraveBehaviour, IPlayerInteractable
        {
            public void OnEnteredRange(PlayerController player)
            {
                if (base.sprite != null)
                {
                    if (SpriteOutlineManager.HasOutline(base.sprite))
                    {
                        SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite);
                    }
                    SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
                }
            }

            public void OnExitRange(PlayerController player)
            {
                if (base.sprite != null)
                {
                    if (SpriteOutlineManager.HasOutline(base.sprite))
                    {
                        SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite);
                    }
                    SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
                }
            }

            public void Interact(PlayerController player)
            {
                if (player != null)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(ETGMod.Databases.Items["TakeableEnemy"].gameObject, Vector2.zero, Quaternion.identity);
                    TakeableEnemy component = gameObject.GetComponent<TakeableEnemy>();
                    EncounterTrackable component2 = component.GetComponent<EncounterTrackable>();
                    if (component2 != null)
                    {
                        component2.DoNotificationOnEncounter = false;
                    }
                    component.suppressPickupVFX = true;
                    component.Pickup(player);
                    component.enemyguid = base.aiActor.EnemyGuid;
                    component.DelayedInitialization();
                    component.sprite.SetSprite(base.sprite.Collection, base.sprite.spriteId);
                    component.encounterTrackable.journalData.PrimaryDisplayName = base.encounterTrackable.journalData.PrimaryDisplayName;
                    component.encounterTrackable.journalData.NotificationPanelDescription = base.encounterTrackable.journalData.NotificationPanelDescription;
                    component.encounterTrackable.journalData.AmmonomiconFullEntry = base.encounterTrackable.journalData.AmmonomiconFullEntry;
                    Destroy(base.gameObject);
                }
            }

            public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
            {
                shouldBeFlipped = false;
                return string.Empty;
            }

            public float GetOverrideMaxDistance()
            {
                return -1f;
            }

            public float GetDistanceToPoint(Vector2 point)
            {
                if (!base.sprite)
                {
                    return 1000f;
                }
                Bounds bounds = base.sprite.GetBounds();
                Vector2 vector = base.transform.position.XY() + (base.transform.rotation * bounds.min).XY();
                Vector2 a = vector + (base.transform.rotation * bounds.size).XY();
                return BraveMathCollege.DistToRectangle(point, vector, a - vector);
            }
        }
    }
}
