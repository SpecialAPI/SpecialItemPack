using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using System.Reflection;

namespace SpecialItemPack
{
    class ComfortableCarpet : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
    {
        public void Interact(PlayerController interactor)
        {
            if (TextBoxManager.HasTextBox(interactor.transform))
            {
                return;
            }
            base.StartCoroutine(this.HandleShrineConversation(interactor));
        }

        private IEnumerator HandleShrineConversation(PlayerController interactor)
        {
            string targetDisplayKey = theCarpetIsntThatComfortableAnymore ? "#INTERACTABLE_CARPET_NOTCOMFORTABLE" : "#INTERACTABLE_CARPET_COMFORTABLE";
            TextBoxManager.ShowThoughtBubble(interactor.sprite.WorldTopCenter, interactor.transform, -1f, StringTableManager.GetLongString(targetDisplayKey), true, false);
            int selectedResponse = -1;
            interactor.SetInputOverride("shrineConversation");
            yield return null;
            GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, StringTableManager.GetString("#INTERACTABLE_CARPET_ACCEPT"), StringTableManager.GetString("#INTERACTABLE_CARPET_DECLINE"));
            while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
            {
                yield return null;
            }
            interactor.ClearInputOverride("shrineConversation");
            TextBoxManager.ClearTextBox(interactor.transform);
            if (selectedResponse == 0)
            {
                base.StartCoroutine(this.HandlePlayerWalkIn(interactor));
            }
            yield break;
        }

        private IEnumerator HandlePlayerWalkIn(PlayerController interactor)
        {
            interactor.SetInputOverride("walking");
            interactor.usingForcedInput = true;
            interactor.ForceIdleFacePoint(this.sprite.WorldCenter - interactor.sprite.WorldBottomCenter, false);
            while (Vector2.Distance(interactor.sprite.WorldBottomCenter, this.sprite.WorldCenter) > 0.2f)
            {
                float num2 = 1f;
                if (!interactor.IsInCombat && GameManager.Options.IncreaseSpeedOutOfCombat)
                {
                    bool flag = true;
                    List<AIActor> allEnemies = StaticReferenceManager.AllEnemies;
                    if (allEnemies != null)
                    {
                        for (int l = 0; l < allEnemies.Count; l++)
                        {
                            AIActor aiactor = allEnemies[l];
                            if (aiactor && aiactor.IsMimicEnemy && !aiactor.IsGone)
                            {
                                float num3 = Vector2.Distance(aiactor.CenterPosition, interactor.CenterPosition);
                                if (num3 < 40f)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        num2 *= 1.5f;
                    }
                }
                interactor.forcedInput = Vector2.Lerp(interactor.forcedInput, (this.sprite.WorldCenter - interactor.sprite.WorldBottomCenter), 0.1f) / num2;
                interactor.ForceStaticFaceDirection(interactor.forcedInput);
                yield return null;
            }
            interactor.usingForcedInput = false;
            interactor.forcedInput = Vector2.zero;
            interactor.specRigidbody.Velocity = Vector2.zero;
            float ela = 0f;
            while(ela < 1f)
            {
                ela += BraveTime.DeltaTime;
                yield return null;
            }
            FieldInfo info = typeof(PlayerController).GetField("m_handlingQueuedAnimation", BindingFlags.NonPublic | BindingFlags.Instance);
            info.SetValue(interactor, true);
            interactor.ToggleGunRenderers(false, "carpet");
            interactor.ToggleHandRenderers(false, "carpet");
            string anim = (!interactor.UseArmorlessAnim) ? "death_shot" : "death_shot_armorless";
            interactor.spriteAnimator.Play(anim);
            while (interactor.spriteAnimator.IsPlaying(anim))
            {
                yield return null;
            }
            info.SetValue(interactor, true);
            tk2dSpriteAnimationClip clip = interactor.spriteAnimator.GetClipByName(anim);
            interactor.spriteAnimator.Stop();
            interactor.sprite.SetSprite(clip.frames[clip.frames.Length - 1].spriteCollection, clip.frames[clip.frames.Length - 1].spriteId);
            interactor.ClearInputOverride("walking");
            interactor.CurrentInputState = PlayerInputState.OnlyMovement;
            float healingTimer = 0.75f;
            while(interactor.specRigidbody.Velocity.magnitude < 0.05f)
            {
                if (!this.theCarpetIsntThatComfortableAnymore)
                {
                    if (interactor.healthHaver.GetCurrentHealthPercentage() < 1f)
                    {
                        healingTimer -= BraveTime.DeltaTime;
                    }
                    if (healingTimer <= 0f)
                    {
                        interactor.healthHaver.ApplyHealing(0.5f);
                        AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", base.gameObject);
                        interactor.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Healing_Sparkles_001") as GameObject, Vector3.zero, true, false, false);
                        healingTimer = 0.75f;
                    }
                    if (interactor.CurrentItem != null)
                    {
                        if (interactor.CurrentItem.IsOnCooldown)
                        {
                            if (interactor.CurrentItem.damageCooldown > 0)
                            {
                                interactor.CurrentItem.CurrentDamageCooldown = Mathf.Max(interactor.CurrentItem.CurrentDamageCooldown - 1f, 0f);
                            }
                        }
                    }
                }
                yield return null;
            }
            interactor.CurrentInputState = PlayerInputState.AllInput;
            interactor.ToggleGunRenderers(true, "carpet");
            interactor.ToggleHandRenderers(true, "carpet");
            info.SetValue(interactor, false);
            this.theCarpetIsntThatComfortableAnymore = true;
            yield break;
        }

        public virtual void ConfigureOnPlacement(RoomHandler room)
        {
            room.OptionalDoorTopDecorable = (ResourceCache.Acquire("Global Prefabs/Purple_Lantern") as GameObject);
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (base.sprite == null)
            {
                return 100f;
            }
            Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, base.specRigidbody.UnitBottomLeft, base.specRigidbody.UnitDimensions);
            return Vector2.Distance(point, v) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return -1f;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
        }

        public void OnExitRange(PlayerController interactor)
        {
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
        }

        public bool theCarpetIsntThatComfortableAnymore = false;
    }
}
