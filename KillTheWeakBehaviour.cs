using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace SpecialItemPack
{
    class KillTheWeakBehaviour : BraveBehaviour
    {
        private void Start()
        {
            base.projectile.OnHitEnemy += this.ProjectileHitEnemy;
        }

        private void ProjectileHitEnemy(Projectile source, SpeculativeRigidbody target, bool fatal)
        {
            if (!fatal && target != null && target.aiActor != null && target.healthHaver != null)
            {
                if (target.healthHaver.GetMaxHealth() <= maximumWeakHealthAmount)
                {
                    this.EatEnemy(target.aiActor);
                    base.projectile.DieInAir();
                }
            }
        }

        private void EatEnemy(AIActor enemy)
        {
            if (!enemy)
            {
                return;
            }
            if (!enemy.healthHaver.IsBoss)
            {
                if (enemy.behaviorSpeculator)
                {
                    enemy.behaviorSpeculator.Stun(1f, true);
                }
                if (enemy.knockbackDoer)
                {
                    enemy.knockbackDoer.SetImmobile(true, "KillTheWeakBehaviour");
                }
            }
            GameObject gameObject = enemy.PlayEffectOnActor(KaliberAffliction.EraseVFX, new Vector3(0f, -1f, 0f), enemy.healthHaver.IsBoss, false, false);
            enemy.StartCoroutine(this.DelayedDestroyEnemy(enemy, gameObject.GetComponent<tk2dSpriteAnimator>()));
        }

        private IEnumerator DelayedDestroyEnemy(AIActor enemy, tk2dSpriteAnimator vfxAnimator)
        {
            if (vfxAnimator)
            {
                vfxAnimator.sprite.IsPerpendicular = false;
                vfxAnimator.sprite.HeightOffGround = -1f;
            }
            while (enemy && vfxAnimator && vfxAnimator.sprite.GetCurrentSpriteDef().name != "kthuliber_tentacles_010")
            {
                vfxAnimator.sprite.UpdateZDepth();
                yield return null;
            }
            if (vfxAnimator)
            {
                vfxAnimator.sprite.IsPerpendicular = true;
                vfxAnimator.sprite.HeightOffGround = 1.5f;
                vfxAnimator.sprite.UpdateZDepth();
            }
            if (enemy)
            {
                if (enemy.healthHaver.IsBoss)
                {
                    AkSoundEngine.PostEvent("Play_WPN_kthulu_blast_01", enemy.gameObject);
                    enemy.healthHaver.ApplyDamage(this.damageToBosses, Vector2.zero, "Being too Weak", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, this.bossDamageIgnoresDamageCap);
                }
                else
                {
                    enemy.EraseFromExistence(false);
                }
            }
            yield break;
        }

        public float maximumWeakHealthAmount = 50f;
        public float damageToBosses = 200f;
        public bool bossDamageIgnoresDamageCap = true;
    }
}
