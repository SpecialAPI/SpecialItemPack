using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;
using Dungeonator;

namespace SpecialItemPack
{
    class Muscle : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Muscle";
            string resourceName = "SpecialItemPack/Resources/Muscle";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Muscle>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Bringing in the Big Guns!";
            string longDesc = "Allows the owner to take an enemy and throw it.\n\nThese big muscles actually aren't realy, they are made out of metal and painted so that it matches the body color. Probably that's the reason they are so strong.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 100);
            item.consumable = false;
            item.quality = ItemQuality.C;
            item.AddToCursulaShop();
            item.AddToTrorkShop();
            item.PlaceItemInAmmonomiconAfterItemById(440);
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            AkSoundEngine.PostEvent("Play_BOSS_doormimic_turn_01", base.gameObject);
            AIActor nearestEnemy = this.GetNearestEnemy();
            nearestEnemy.Mount(user, tk2dBaseSprite.Anchor.UpperCenter, tk2dBaseSprite.Anchor.LowerCenter, EnemyWorks.MountedBehaviour.LayeringType.ABOVE, 1.5f);
            if(nearestEnemy.behaviorSpeculator != null)
            {
                nearestEnemy.behaviorSpeculator.Stun(1f);
            }
            this.m_currentlyHeldEnemy = nearestEnemy;
        }

        protected override void OnPreDrop(PlayerController user)
        {
            if (this.m_isCurrentlyActive)
            {
                this.DoActiveEffect(user);
            }
            base.OnPreDrop(user);
        }

        protected override void DoActiveEffect(PlayerController user)
        {
            base.DoActiveEffect(user);
            if(user.healthHaver != null)
            {
                user.healthHaver.TriggerInvulnerabilityPeriod(0.5f);
            }
            this.m_currentlyHeldEnemy.Dismount();
            AIActor aiActor = this.m_currentlyHeldEnemy;
            KnockbackDoer knockbackDoer = aiActor.knockbackDoer;
            if (aiActor)
            {
                if(aiActor.specRigidbody != null)
                {
                    aiActor.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox));
                    aiActor.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleHitEnemyHitEnemy);
                    aiActor.specRigidbody.OnPreTileCollision += new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.HandleHitTile);
                }
            }
            if (knockbackDoer)
            {
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(GameManager.Instance.PrimaryPlayer.PlayerIDX);
                bool flag2 = instanceForPlayer == null;
                if (!flag2)
                {
                    Vector2 vector = GameManager.Instance.PrimaryPlayer.unadjustedAimPoint.XY() - user.CenterPosition;
                    Vector2 direction = BraveMathCollege.DegreesToVector(BraveMathCollege.Atan2Degrees(vector), 1);
                    knockbackDoer.ApplyKnockback(direction, 800f, true);
                }
            }
            this.m_currentlyHeldEnemy = null;
        }

        private void HandleHitEnemyHitEnemy(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody && otherRigidbody.aiActor && myRigidbody && myRigidbody.healthHaver)
            {
                AIActor aiActor = otherRigidbody.aiActor;
                myRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleHitEnemyHitEnemy);
                myRigidbody.OnPreTileCollision -= new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.HandleHitTile);
                if (aiActor.IsNormalEnemy && aiActor.healthHaver)
                {
                    aiActor.healthHaver.ApplyDamage(myRigidbody.healthHaver.GetMaxHealth() * 2f, myRigidbody.Velocity, "Kinetic Power", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                }
                AIActor aiActor2 = myRigidbody.aiActor;
                myRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleHitEnemyHitEnemy);
                if (aiActor2.IsNormalEnemy && aiActor2.healthHaver)
                {
                    aiActor2.healthHaver.ApplyDamage(30, myRigidbody.Velocity, "Kinetic Power", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                }
            }
        }

        public void HandleHitTile(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            if (myRigidbody && myRigidbody.healthHaver)
            {
                AIActor aiActor = myRigidbody.aiActor;
                myRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleHitEnemyHitEnemy);
                myRigidbody.OnPreTileCollision -= new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.HandleHitTile);
                if (aiActor.IsNormalEnemy && aiActor.healthHaver)
                {
                    aiActor.healthHaver.ApplyDamage(30, myRigidbody.Velocity, "Kinetic Power", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                }
            }
        }

        public AIActor GetNearestEnemy()
        {
            AIActor aiactor = null;
            float nearestDistance = float.MaxValue;
            if (this.LastOwner.CenterPosition.GetAbsoluteRoom() == null)
            {
                return null;
            }
            List<AIActor> activeEnemies = this.LastOwner.CenterPosition.GetAbsoluteRoom().GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            AIActor result;
            if (activeEnemies == null)
            {
                return null;
            }
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                AIActor aiactor2 = activeEnemies[i];
                if (aiactor2 != null && aiactor2.healthHaver != null)
                {
                    bool flag3 = !aiactor2.healthHaver.IsDead;
                    if (flag3 && !aiactor2.healthHaver.IsBoss)
                    {
                        float num = Vector2.Distance(this.LastOwner.CenterPosition, aiactor2.CenterPosition);
                        bool flag5 = num < nearestDistance;
                        if (flag5)
                        {
                            nearestDistance = num;
                            aiactor = aiactor2;
                        }
                    }
                }
            }
            result = aiactor;
            if (result == null)
            {
                return null;
            }
            return result;
        }

        public override void Update()
        {
            base.Update();
            this.m_isCurrentlyActive = this.m_currentlyHeldEnemy != null;
        }
        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && this.GetNearestEnemy() != null && (this.GetNearestEnemy().CenterPosition - user.CenterPosition).sqrMagnitude < 3f * 3f;
        }

        private AIActor m_currentlyHeldEnemy;
    }
}
