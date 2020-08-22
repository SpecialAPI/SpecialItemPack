using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class QuantumBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Quantum Bullets";
            string resourceName = "SpecialItemPack/Resources/QuantumBullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<QuantumBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Hi-Tech Bullets";
            string longDesc = "Bullets have a chance to do... something to enemies?\n\nThe bullets from the future, they take the atoms of the enemies and split them complettely. This leaves them with 2 little \"clones\" of themselves, and one of them can't even do" +
                " anything!";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.S;
            item.AddToTrorkShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(661);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.PostProjectile;
        }

        public void PostProjectile(Projectile proj, float f)
        {
            if(UnityEngine.Random.value <= 0.15f)
            {
                proj.AdjustPlayerProjectileTint(new Color(70f / 255f, 227f / 255f, 174f / 255f).WithAlpha(0.75f), 1, 0);
                proj.OnHitEnemy += this.OnProjectileHitEnemy;
            }
            if (this.m_owner.PlayerHasActiveSynergy("#HI_TECH"))
            {
                proj.OnHitEnemy += delegate (Projectile proj2, SpeculativeRigidbody enemyRigidbody, bool fatal)
                {
                    if(enemyRigidbody.aiActor != null)
                    {
                        AIActor aiactor = enemyRigidbody.aiActor;
                        if (aiactor.gameObject.GetComponent<SlicedBehavior>() != null)
                        {
                            aiactor.ApplyEffect((PickupObjectDatabase.GetById(204) as BulletStatusEffectItem).HealthModifierEffect);
                        }
                    }
                };
            }
        }

        public void OnProjectileHitEnemy(Projectile proj, SpeculativeRigidbody enemy, bool fatal)
        {
            if(enemy != null)
            {
                if (enemy.aiActor != null)
                {
                    AIActor aiactor = enemy.aiActor;
                    //oh god this if section is nightmare.
                    if (aiactor.gameObject.GetComponent<SlicedBehavior>() == null && aiactor.EnemyGuid != "f3b04a067a65492f8b279130323b41f0" && aiactor.EnemyGuid != "465da2bb086a4a88a803f79fe3a27677" && aiactor.EnemyGuid != "05b8afe0b6cc4fffa9dc6036fa24c8ec" &&
                        aiactor.EnemyGuid != "cd88c3ce60c442e9aa5b3904d31652bc" && aiactor.EnemyGuid != "68a238ed6a82467ea85474c595c49c6e" && aiactor.EnemyGuid != "7c5d5f09911e49b78ae644d2b50ff3bf" && aiactor.EnemyGuid != "da797878d215453abba824ff902e21b4" &&
                        aiactor.EnemyGuid != "cd4a4b7f612a4ba9a720b9f97c52f38c" && aiactor.EnemyGuid != "249db525a9464e5282d02162c88e0357" && aiactor.gameObject.GetComponent<AliveBullets.DumbEnemyBehavior>() == null)
                    {
                        if ((aiactor.healthHaver.IsBoss && UnityEngine.Random.value < 0.05f) || !aiactor.healthHaver.IsBoss)
                        {
                            AkSoundEngine.PostEvent("Play_WPN_spellactionrevolver_shot_01", base.gameObject);
                            AIActor aiactor2 = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(aiactor.EnemyGuid), aiactor.sprite.WorldTopRight, aiactor.transform.position.GetAbsoluteRoom(), true, AIActor.AwakenAnimationType.Default, true);
                            GameManager.Instance.StartCoroutine(this.DecreaseEnemyScale(aiactor2));
                            aiactor.EnemyScale *= 0.75f;
                            aiactor2.healthHaver.SetHealthMaximum(aiactor.healthHaver.GetMaxHealth() * 0.5f, null, true);
                            aiactor.healthHaver.SetHealthMaximum(aiactor.healthHaver.GetMaxHealth() * 0.5f, null, true);
                            if (aiactor2.behaviorSpeculator != null)
                            {
                                aiactor2.behaviorSpeculator.TargetBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.TargetBehaviors;
                                aiactor2.behaviorSpeculator.MovementBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.MovementBehaviors;
                                aiactor2.behaviorSpeculator.OtherBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.OtherBehaviors;
                                aiactor2.behaviorSpeculator.AttackBehaviors = EnemyDatabase.GetOrLoadByGuid("b08ec82bef6940328c7ecd9ffc6bd16c").behaviorSpeculator.AttackBehaviors;
                            }
                            if (aiactor2.CurrentGun != null)
                            {
                                aiactor2.aiShooter.Inventory.DestroyAllGuns();
                            }
                            aiactor.gameObject.AddComponent<SlicedBehavior>();
                            aiactor.gameObject.GetComponent<SlicedBehavior>().clone = aiactor2;
                            aiactor2.gameObject.AddComponent<SlicedBehavior>();
                        }
                    }
                }
            }
        }

        public IEnumerator DecreaseEnemyScale(AIActor enemy)
        {
            yield return new WaitForSeconds(0.25f);
            enemy.EnemyScale *= 0.75f;
            yield break;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProjectile;
            return base.Drop(player);
        }

        private class SlicedBehavior : MonoBehaviour
        {
            private void Awake()
            {
                this.aiactor = base.GetComponent<AIActor>();
                if (this.aiactor.healthHaver.IsBoss)
                {
                    this.aiactor.healthHaver.OnDeath += this.KillClone;
                }
            }

            private void KillClone(Vector2 vector)
            {
                if(this.clone != null && this.clone.healthHaver != null)
                {
                    this.clone.healthHaver.PreventAllDamage = false;
                    this.clone.healthHaver.IsVulnerable = true;
                    this.clone.healthHaver.ApplyDamage(int.MaxValue, Vector2.zero, "Quantum Bullets", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                }
            }

            private AIActor aiactor;
            public AIActor clone;
        }
    }
}
