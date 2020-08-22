using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SpecialItemPack
{
    class IgnoreBehaviour : BraveBehaviour
    {
        public void Start()
        {
            base.projectile.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
            this.ClearDamageResistance(base.projectile.healthEffect);
            this.ClearDamageResistance(base.projectile.speedEffect);
            this.ClearDamageResistance(base.projectile.charmEffect);
            this.ClearDamageResistance(base.projectile.freezeEffect);
            this.ClearDamageResistance(base.projectile.fireEffect);
            this.ClearDamageResistance(base.projectile.cheeseEffect);
            foreach (GameActorEffect effect in base.projectile.statusEffectsToApply)
            {
                this.ClearDamageResistance(effect);
            }
        }

        private void ClearDamageResistance(GameActorEffect effect)
        {
            if (effect != null)
            {
                effect.resistanceType = EffectResistanceType.None;
                effect.effectIdentifier = "Setting This To Something Different Is Needed, So...";
            }
        }

        private void OnPreRigidbodyCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody.aiActor != null)
            {
                otherRigidbody.StartCoroutine(this.RigidbodyCollision(otherRigidbody));
            }
        }

        private IEnumerator RigidbodyCollision(SpeculativeRigidbody otherRigidbody)
        {
            AIActor aiactor = otherRigidbody.aiActor;
            bool reflectedProjectiles = otherRigidbody.ReflectProjectiles;
            List<DamageTypeModifier> immuneDamageTypes = aiactor.healthHaver.damageTypeModifiers;
            bool wasVulnerable = aiactor.healthHaver.PreventAllDamage;
            tk2dSpriteAnimationFrame frame = null;
            bool wasInvulnerableThisFrame = false;
            if (aiactor.spriteAnimator != null && aiactor.spriteAnimator.CurrentClip != null && aiactor.spriteAnimator.CurrentFrame >= 0 && aiactor.spriteAnimator.CurrentFrame < aiactor.spriteAnimator.CurrentClip.frames.Length)
            {
                frame = aiactor.spriteAnimator.CurrentClip.frames[aiactor.spriteAnimator.CurrentFrame];
                wasInvulnerableThisFrame = frame.invulnerableFrame;
                frame.invulnerableFrame = false;
            }
            otherRigidbody.ReflectProjectiles = false;
            aiactor.healthHaver.damageTypeModifiers.Clear();
            aiactor.healthHaver.PreventAllDamage = false;
            yield return null;
            otherRigidbody.ReflectProjectiles = reflectedProjectiles;
            aiactor.healthHaver.damageTypeModifiers = immuneDamageTypes;
            aiactor.healthHaver.PreventAllDamage = wasVulnerable;
            if(frame != null)
            {
                frame.invulnerableFrame = wasInvulnerableThisFrame;
            }
            yield break;
        }
    }
}
