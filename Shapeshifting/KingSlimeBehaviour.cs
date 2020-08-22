using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialItemPack.Shapeshifting
{
    class KingSlimeBehaviour : ShapeShiftBehaviour
    {
        public override void ClearEffect()
        {
            player.OnPreDodgeRoll -= AntiRoll;
            player.healthHaver.damageTypeModifiers.Remove(fireWeakness);
            fireWeakness = null;
        }

        private void Update()
        {
            BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(player.PlayerIDX);
            if (instanceForPlayer.GetButtonDown(GungeonActions.GungeonActionType.DodgeRoll))
            {
                instanceForPlayer.ConsumeButtonDown(GungeonActions.GungeonActionType.DodgeRoll);
                Vector2 vector = Vector2.zero;
                if (player.CurrentInputState != PlayerInputState.NoMovement)
                {
                    vector = this.AdjustInputVector(instanceForPlayer.ActiveActions.Move.Vector, BraveInput.MagnetAngles.movementCardinal, BraveInput.MagnetAngles.movementOrdinal);
                }
                if (vector.magnitude > 1f)
                {
                    vector.Normalize();
                }
                if (!m_isJumping)
                {
                    player.StartCoroutine(this.Jump(vector));
                }
            }
            if (player.IsOnFire)
            {
                if (this.IsJumping)
                {
                    player.IncreaseFire(-(BraveTime.DeltaTime * 0.466666f));
                }
            }
        }

        private Vector2 AdjustInputVector(Vector2 rawInput, float cardinalMagnetAngle, float ordinalMagnetAngle)
        {
            float num = BraveMathCollege.ClampAngle360(BraveMathCollege.Atan2Degrees(rawInput));
            float num2 = num % 90f;
            float num3 = (num + 45f) % 90f;
            float num4 = 0f;
            if (cardinalMagnetAngle > 0f)
            {
                if (num2 < cardinalMagnetAngle)
                {
                    num4 = -num2;
                }
                else if (num2 > 90f - cardinalMagnetAngle)
                {
                    num4 = 90f - num2;
                }
            }
            if (ordinalMagnetAngle > 0f)
            {
                if (num3 < ordinalMagnetAngle)
                {
                    num4 = -num3;
                }
                else if (num3 > 90f - ordinalMagnetAngle)
                {
                    num4 = 90f - num3;
                }
            }
            num += num4;
            return (Quaternion.Euler(0f, 0f, num) * Vector3.right).XY() * rawInput.magnitude;
        }

        private IEnumerator Jump(Vector2 direction)
        {
            m_isJumping = true;
            if (player.CurrentFireMeterValue > 0f)
            {
                player.CurrentFireMeterValue = Mathf.Max(0f, player.CurrentFireMeterValue -= 0.5f);
                if (player.CurrentFireMeterValue == 0f)
                {
                    player.IsOnFire = false;
                }
            }
            Func<bool, bool> noFallsForYou = ((bool b) => false);
            AnimationCurve jumpCurve = new AnimationCurve(new Keyframe[] { new Keyframe { time = 0, value = 6f }, new Keyframe { time = (0.5f * player.rollStats.GetModifiedTime(player)), value = 0f }, new Keyframe { time = 
                player.rollStats.GetModifiedTime(player), value = -6f } });
            player.SetInputOverride("king slime's jump");
            player.ToggleGunRenderers(false, "king slime's jump");
            player.ToggleHandRenderers(false, "king slime's jump");
            player.OnAboutToFall += noFallsForYou;
            player.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.Projectile));
            player.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox));
            player.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyCollider));
            float ela = 0f;
            while(ela < (player.rollStats.GetModifiedTime(player)))
            {
                ela = BraveTime.DeltaTime + ela;
                player.sprite.transform.position = new Vector2(player.sprite.transform.position.x, Mathf.Max(player.sprite.transform.position.y + (jumpCurve.Evaluate(ela) * BraveTime.DeltaTime), player.ShadowObject.GetAnyComponent<tk2dBaseSprite>().WorldCenter.y));
                float time = Mathf.Clamp01((ela - BraveTime.DeltaTime) / player.rollStats.GetModifiedTime(player));
                float time2 = Mathf.Clamp01(ela / player.rollStats.GetModifiedTime(player));
                float num = (Mathf.Clamp01(player.rollStats.speed.Evaluate(time2)) - Mathf.Clamp01(player.rollStats.speed.Evaluate(time))) * player.rollStats.GetModifiedDistance(player);
                player.specRigidbody.Velocity = direction.normalized * (num / BraveTime.DeltaTime) + player.knockbackComponent + player.immutableKnockbackComponent;
                yield return null;
            }
            player.specRigidbody.Velocity = Vector2.zero;
            player.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.Projectile));
            player.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox));
            player.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyCollider));
            player.OnAboutToFall -= noFallsForYou;
            player.ToggleGunRenderers(true, "king slime's jump");
            player.ToggleHandRenderers(true, "king slime's jump");
            player.ClearInputOverride("king slime's jump");
            Exploder.DoRadialPush(player.sprite.WorldBottomCenter.ToVector3ZUp(player.sprite.WorldBottomCenter.y), 22f, 1.5f);
            Exploder.DoRadialKnockback(player.sprite.WorldBottomCenter.ToVector3ZUp(player.sprite.WorldBottomCenter.y), 100f, 1.5f);
            Exploder.DoRadialDamage(5f * player.stats.GetStatValue(PlayerStats.StatType.DodgeRollDamage), player.sprite.WorldBottomCenter.ToVector3ZUp(player.sprite.WorldBottomCenter.y), 1.5f, false, true, false, null);
            GameObject gameObject = SpawnManager.SpawnVFX(BraveResources.Load<GameObject>("Global VFX/VFX_DBZ_Charge", ".prefab"), false);
            gameObject.transform.position = player.specRigidbody.UnitCenter;
            tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
            component.HeightOffGround = -1f;
            component.UpdateZDepth();
            m_isJumping = false;
            yield break;
        }

        private IEnumerator ClearStationary(bool cachedStationary)
        {
            yield return null;
            player.IsStationary = cachedStationary;
            yield break;
        }

        private void AntiRoll(PlayerController player)
        {
            bool cachedStationary = player.IsStationary;
            player.IsStationary = true;
            player.StartCoroutine(this.ClearStationary(cachedStationary));
        }

        public override void OnApplied()
        {
            player.OnPreDodgeRoll += AntiRoll;
            fireWeakness = new DamageTypeModifier
            {
                damageMultiplier = 2f,
                damageType = CoreDamageTypes.Fire
            };
            player.healthHaver.damageTypeModifiers.Add(fireWeakness);
        }

        public bool IsJumping
        {
            get
            {
                return m_isJumping;
            }
        }

        private DamageTypeModifier fireWeakness;
        private bool m_isJumping;

        public override List<StatModifier> shapeShiftModifiers => new List<StatModifier> { 
            Toolbox.SetupStatModifier(PlayerStats.StatType.Health, -1f, StatModifier.ModifyMethod.ADDITIVE, false), 
            Toolbox.SetupStatModifier(PlayerStats.StatType.MovementSpeed, 0.75f, StatModifier.ModifyMethod.MULTIPLICATIVE, false) 
        };
    }
}
