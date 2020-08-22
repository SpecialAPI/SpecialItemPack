using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class TakeableEnemy : PassiveItem
    {
        public static void Init()
        {
            string itemName = "TakeableEnemy";
            string resourceName = "SpecialItemPack/Resources/djunkan_thonk";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<TakeableEnemy>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "";
            string longDesc = "";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.EXCLUDED;
        }

        public void DelayedInitialization()
        {
            if(this.m_owner != null)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid(this.enemyguid).gameObject, this.m_owner.transform.position, Quaternion.identity);
                CompanionController orAddComponent = gameObject.GetOrAddComponent<CompanionController>();
                orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
                orAddComponent.Initialize(this.m_owner);
                orAddComponent.behaviorSpeculator.MovementBehaviors.Add(new CompanionFollowPlayerBehavior());
                AIActor aiactor = gameObject.GetComponent<AIActor>();
                aiactor.HitByEnemyBullets = true;
                aiactor.healthHaver.ModifyDamage += TakeableEnemy.ModifyDamageForCompanions;
                if(aiactor.bulletBank != null)
                {
                    orAddComponent.bulletBank.OnProjectileCreated += TakeableEnemy.OnCompanionPostProcessProjectile;
                }
                if(orAddComponent.aiShooter != null)
                {
                    orAddComponent.aiShooter.PostProcessProjectile += TakeableEnemy.OnCompanionPostProcessProjectile;
                }
                this.extantCompanion = gameObject;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (this.m_owner != null)
            {
                if(this.extantCompanion == null || this.extantCompanion.GetComponent<HealthHaver>() == null || this.extantCompanion.GetComponent<HealthHaver>().IsDead)
                {
                    TakeableEnemy.RemovePassiveItem(this.m_owner, this);
                }
            }
        }

        public static void RemovePassiveItem(PlayerController self, PassiveItem passive)
        {
            int num = self.passiveItems.IndexOf(passive);
            if (num >= 0)
            {
                TakeableEnemy.RemovePassiveItemAt(self, num);
            }
        }

        public static void RemovePassiveItemAt(PlayerController self, int index)
        {
            PassiveItem passiveItem = self.passiveItems[index];
            self.passiveItems.RemoveAt(index);
            GameUIRoot.Instance.RemovePassiveItemFromDock(passiveItem);
            UnityEngine.Object.Destroy(passiveItem);
            self.stats.RecalculateStats(self, false, false);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            if (this.extantCompanion)
            {
                Destroy(this.extantCompanion);
            }
            this.extantCompanion = null;
            DebrisObject obj = base.Drop(player);
            obj.OnGrounded += TakeableEnemy.OnGrounded;
            return obj;
        }

        public static void OnCompanionPostProcessProjectile(Projectile proj)
        {
            proj.baseData.damage = 10f;
        }

        private static void ModifyDamageForCompanions(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == HealthHaver.ModifyDamageEventArgs.Empty)
            {
                return;
            }
            args.ModifiedDamage *= 5f;
        }

        public static void OnGrounded(DebrisObject obj)
        {
            if(obj.GetComponent<TakeableEnemy>() != null)
            {
                GameObject enemy = UnityEngine.Object.Instantiate<GameObject>(EnemyDatabase.GetOrLoadByGuid(obj.GetComponent<TakeableEnemy>().enemyguid).gameObject, obj.transform.position, Quaternion.identity);
                if(enemy.GetComponent<AIActor>() != null)
                {
                    enemy.GetComponent<AIActor>().HasBeenEngaged = true;
                }
                Destroy(obj.gameObject);
            }
        }

        public string enemyguid;
        private GameObject extantCompanion;
    }
}
