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
    class LockonBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Lock-On Bullets";
            string resourceName = "SpecialItemPack/Resources/SmartBullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<LockonBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Don't Lose Focus";
            string longDesc = "Firing at an enemy will lock-on them and bullets fire later will home onto it.\n\nTheese bullets trained their focus for years, and will focus on one enemy and try to hit it no matter what.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.A;
            item.AddToBlacksmithShop();
            item.AddToTrorkShop();
            Game.Items.Rename("spapi:lock-on_bullets", "spapi:lock_on_bullets");
            item.PlaceItemInAmmonomiconAfterItemById(284);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.PostProcessProjectile;
        }

        protected override void Update()
        {
            base.Update();
            if(this.m_pickedUp && this.m_owner != null)
            {
                if (this.m_owner.PlayerHasActiveSynergy("#THE_SMART_COMPANIONS"))
                {
                    foreach(AIActor enemy in StaticReferenceManager.AllEnemies)
                    {
                        if (enemy.GetEffect("charm") != null && enemy.GetComponent<AliveBullets.DumbEnemyBehavior>() == null && enemy.GetComponent<CompanionController>() == null)
                        {
                            string guid = enemy.EnemyGuid;
                            Vector2 pos = enemy.sprite.WorldBottomLeft;
                            bool isBlackPhantom = enemy.IsBlackPhantom;
                            enemy.EraseFromExistence(true);
                            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, pos, Quaternion.identity);
                            CompanionController orAddComponent = gameObject.GetOrAddComponent<CompanionController>();
                            if (isBlackPhantom)
                            {
                                gameObject.GetComponent<AIActor>().BecomeBlackPhantom();
                            }
                            orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
                            orAddComponent.Initialize(this.m_owner);
                            orAddComponent.behaviorSpeculator.MovementBehaviors.Add(new CompanionFollowPlayerBehavior());
                            AIActor aiactor = gameObject.GetComponent<AIActor>();
                            aiactor.HitByEnemyBullets = true;
                            aiactor.healthHaver.ModifyDamage += this.ModifyDamageForCompanions;
                            if (orAddComponent.bulletBank)
                            {
                                orAddComponent.bulletBank.OnProjectileCreated += this.PreventInfection;
                            }
                            if (orAddComponent.aiShooter)
                            {
                                orAddComponent.aiShooter.PostProcessProjectile += this.PreventInfection;
                            }
                            foreach (AIBulletBank.Entry entry in orAddComponent.bulletBank.Bullets)
                            {
                                if (aiactor.IsBlackPhantom)
                                {
                                    entry.BulletObject.GetComponent<Projectile>().baseData.damage = 15;
                                }
                                else
                                {
                                    entry.BulletObject.GetComponent<Projectile>().baseData.damage = 10;
                                }
                            }
                            foreach (AttackBehaviorBase behavior in aiactor.behaviorSpeculator.AttackBehaviors)
                            {
                                if ((behavior as ShootGunBehavior) != null)
                                {
                                    if (aiactor.IsBlackPhantom)
                                    {
                                        aiactor.aiShooter.GetBulletEntry((behavior as ShootGunBehavior).OverrideBulletName).ProjectileData.damage = 15;
                                    }
                                    else
                                    {
                                        aiactor.aiShooter.GetBulletEntry((behavior as ShootGunBehavior).OverrideBulletName).ProjectileData.damage = 10;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void PreventInfection(Projectile proj)
        {
            if (proj.AppliesCharm)
            {
                proj.AppliesCharm = false;
                proj.CharmApplyChance = 0;
            }
        }

        private void ModifyDamageForCompanions(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == HealthHaver.ModifyDamageEventArgs.Empty)
            {
                return;
            }
            args.ModifiedDamage *= 5f;
        }

        public void PostProcessProjectile(Projectile proj, float f)
        {
            if (this.lockonEnemy != null && (this.lockonEnemy.healthHaver == null || !this.lockonEnemy.healthHaver.IsAlive))
            {
                this.lockonEnemy = null;
            }
            if (lockonEnemy != null)
            {
                LockOnHomingModifier lockonMod = proj.gameObject.AddComponent<LockOnHomingModifier>();
                lockonMod.AssignTargetManually(this.lockonEnemy);
                lockonMod.HomingRadius = 10;
                lockonMod.AngularVelocity = 420;
            }
            proj.OnHitEnemy += this.AssignEnemy;
        }

        private void AssignEnemy(Projectile proj, SpeculativeRigidbody enemyRigidbody, bool fatal)
        {
            if(enemyRigidbody == null || enemyRigidbody.aiActor == null)
            {
                return;
            }
            if(enemyRigidbody != null && enemyRigidbody.aiActor != null)
            {
                AIActor aiactor = enemyRigidbody.aiActor;
                if(aiactor.healthHaver != null && aiactor.healthHaver.IsAlive)
                {
                    this.lockonEnemy = aiactor;
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProcessProjectile;
            return base.Drop(player);
        }

        private AIActor lockonEnemy = null;
    }
}
