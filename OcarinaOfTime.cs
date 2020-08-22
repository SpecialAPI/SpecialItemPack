using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;
using Dungeonator;
using UnityEngine.SceneManagement;

namespace SpecialItemPack
{
    class OcarinaOfTime : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Ocarina of Time";
            string resourceName = "SpecialItemPack/Resources/OcarinaOfTime";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<OcarinaOfTime>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc =  "Like we could turn time back...";
            string longDesc = "Playing this ocarina can send the owner back in time. After that it disappears.\n\nOcarinas like this were used by hero bullets a long time ago, when the Gungeon was still named Swordgeon. For some reason, they were using " +
                "swords as their weapons. Guns are better, right?";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 0.5f);
            item.consumable = true;
            item.quality = ItemQuality.A;
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(69);
        }

        protected override void DoEffect(PlayerController user)
        {
            GameManager.Instance.PauseRaw(true);
            BraveTime.RegisterTimeScaleMultiplier(0f, GameManager.Instance.gameObject);
            AkSoundEngine.PostEvent("Stop_SND_All", GameManager.Instance.gameObject);
            GameManager.Instance.StartCoroutine(OcarinaOfTime.HandleReturn(user));
            AkSoundEngine.PostEvent("Play_UI_gameover_start_01", GameManager.Instance.gameObject);
            base.DoEffect(user);
        }

        private static IEnumerator HandleReturn(PlayerController user)
        {
            Pixelator.Instance.DoFinalNonFadedLayer = true;
            if (user.CurrentGun)
            {
                user.CurrentGun.CeaseAttack(false, null);
            }
            user.CurrentInputState = PlayerInputState.NoInput;
            GameManager.Instance.MainCameraController.SetManualControl(true, false);
            Transform cameraTransform = GameManager.Instance.MainCameraController.transform;
            Vector3 cameraStartPosition = cameraTransform.position;
            Vector3 cameraEndPosition = user.CenterPosition;
            GameManager.Instance.MainCameraController.OverridePosition = cameraStartPosition;
            if (user.CurrentGun)
            {
                user.CurrentGun.DespawnVFX();
            }
            yield return null;
            if (user.CurrentGun)
            {
                user.CurrentGun.DespawnVFX();
            }
            user.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Unfaded"));
            GameUIRoot.Instance.ForceClearReload(user.PlayerIDX);
            GameUIRoot.Instance.notificationController.ForceHide();
            float elapsed = 0f;
            float duration = 0.8f;
            tk2dBaseSprite spotlightSprite = ((GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("DeathShadow", ".prefab"), user.specRigidbody.UnitCenter, Quaternion.identity)).GetComponent<tk2dBaseSprite>();
            spotlightSprite.spriteAnimator.ignoreTimeScale = true;
            spotlightSprite.spriteAnimator.Play();
            tk2dSpriteAnimator whooshAnimator = spotlightSprite.transform.GetChild(0).GetComponent<tk2dSpriteAnimator>();
            whooshAnimator.ignoreTimeScale = true;
            whooshAnimator.Play();
            Pixelator.Instance.CustomFade(0.6f, 0f, Color.white, Color.black, 0.1f, 0.5f);
            Pixelator.Instance.LerpToLetterbox(0.35f, 0.8f);
            BraveInput.AllowPausedRumble = true;
            user.DoVibration(Vibration.Time.Normal, Vibration.Strength.Hard);
            while (elapsed < duration)
            {
                if (GameManager.INVARIANT_DELTA_TIME == 0f)
                {
                    elapsed += 0.05f;
                }
                elapsed += GameManager.INVARIANT_DELTA_TIME;
                float t = elapsed / duration;
                GameManager.Instance.MainCameraController.OverridePosition = Vector3.Lerp(cameraStartPosition, cameraEndPosition, t);
                user.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                spotlightSprite.color = new Color(1f, 1f, 1f, t);
                Pixelator.Instance.saturation = Mathf.Clamp01(1f - t);
                yield return null;
            }
            spotlightSprite.color = Color.white;
            yield return user.StartCoroutine(OcarinaOfTime.InvariantWait(user, 0.4f));
            Transform clockhairTransform = ((GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("Clockhair", ".prefab"))).transform;
            ClockhairController clockhair = clockhairTransform.GetComponent<ClockhairController>();
            elapsed = 0f;
            duration = clockhair.ClockhairInDuration;
            Vector3 clockhairTargetPosition = user.CenterPosition;
            Vector3 clockhairStartPosition = clockhairTargetPosition + new Vector3(-20f, 5f, 0f);
            clockhair.renderer.enabled = false;
            clockhair.spriteAnimator.Play("clockhair_intro");
            clockhair.hourAnimator.Play("hour_hand_intro");
            clockhair.minuteAnimator.Play("minute_hand_intro");
            clockhair.secondAnimator.Play("second_hand_intro");
            bool hasWobbled = false;
            while (elapsed < duration)
            {
                if (GameManager.INVARIANT_DELTA_TIME == 0f)
                {
                    elapsed += 0.05f;
                }
                elapsed += GameManager.INVARIANT_DELTA_TIME;
                float t2 = elapsed / duration;
                float smoothT = Mathf.SmoothStep(0f, 1f, t2);
                Vector3 currentPosition = Vector3.Slerp(clockhairStartPosition, clockhairTargetPosition, smoothT);
                clockhairTransform.position = currentPosition.WithZ(0f);
                if (t2 > 0.5f)
                {
                    clockhair.renderer.enabled = true;
                    clockhair.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                }
                if (t2 > 0.75f)
                {
                    clockhair.hourAnimator.GetComponent<Renderer>().enabled = true;
                    clockhair.minuteAnimator.GetComponent<Renderer>().enabled = true;
                    clockhair.secondAnimator.GetComponent<Renderer>().enabled = true;
                    clockhair.hourAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    clockhair.minuteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                    clockhair.secondAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                }
                if (!hasWobbled && clockhair.spriteAnimator.CurrentFrame == clockhair.spriteAnimator.CurrentClip.frames.Length - 1)
                {
                    clockhair.spriteAnimator.Play("clockhair_wobble");
                    hasWobbled = true;
                }
                clockhair.sprite.UpdateZDepth();
                user.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                yield return null;
            }
            if (!hasWobbled)
            {
                clockhair.spriteAnimator.Play("clockhair_wobble");
            }
            clockhair.SpinToSessionStart(clockhair.ClockhairSpinDuration);
            elapsed = 0f;
            duration = clockhair.ClockhairSpinDuration + clockhair.ClockhairPauseBeforeShot;
            while (elapsed < duration)
            {
                if (GameManager.INVARIANT_DELTA_TIME == 0f)
                {
                    elapsed += 0.05f;
                }
                elapsed += GameManager.INVARIANT_DELTA_TIME;
                clockhair.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                yield return null;
            }
            clockhair.spriteAnimator.Play("clockhair_fire");
            clockhair.hourAnimator.GetComponent<Renderer>().enabled = false;
            clockhair.minuteAnimator.GetComponent<Renderer>().enabled = false;
            clockhair.secondAnimator.GetComponent<Renderer>().enabled = false;
            Pixelator.Instance.FadeToBlack(0.5f, false, 0f);
            Pixelator.Instance.saturation = 1f;
            Pixelator.Instance.LerpToLetterbox(1f, 0.25f);
            UnityEngine.Object.Destroy(spotlightSprite.gameObject);
            Pixelator.Instance.DoFinalNonFadedLayer = false;
            user.CurrentInputState = PlayerInputState.AllInput;
            user.gameObject.SetLayerRecursively(LayerMask.NameToLayer("FG_Reflection"));
            GameManager.Instance.ForceUnpause();
            GameManager.Instance.PreventPausing = false;
            user.healthHaver.IsVulnerable = true;
            user.healthHaver.TriggerInvulnerabilityPeriod(-1f);
            Pixelator.Instance.FadeToBlack(0.5f, false, 0f);
            GameUIRoot.Instance.ToggleUICamera(false);
            GameManager.Instance.SetNextLevelIndex(OcarinaOfTime.GetTargetLevelIndexFromSavedTileset(GameManager.Instance.Dungeon.tileIndices.tilesetId));
            if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH)
            {
                GameManager.Instance.DelayedLoadBossrushFloor(0.5f);
            }
            else
            {
                GameManager.Instance.DelayedLoadNextLevel(0.5f);
            }
            if(user.inventory != null && user.PlayerHasActiveSynergy("#SWORDGEON_AWAITS_%PLAYER_NICK"))
            {
                user.inventory.AddGunToInventory(Toolbox.GetGunById(SpecialItemIds.HerosSword), true);
            }
            yield break;
        }

        public static int GetTargetLevelIndexFromSavedTileset(GlobalDungeonData.ValidTilesets tilesetID)
        {
            switch (tilesetID)
            {
                case GlobalDungeonData.ValidTilesets.GUNGEON:
                    return 1;
                case GlobalDungeonData.ValidTilesets.CASTLEGEON:
                    return 1;
                default:
                    if (tilesetID == GlobalDungeonData.ValidTilesets.MINEGEON)
                    {
                        return 2;
                    }
                    if (tilesetID == GlobalDungeonData.ValidTilesets.CATACOMBGEON)
                    {
                        return 3;
                    }
                    if (tilesetID == GlobalDungeonData.ValidTilesets.FORGEGEON)
                    {
                        return 4;
                    }
                    if (tilesetID == GlobalDungeonData.ValidTilesets.HELLGEON)
                    {
                        return 5;
                    }
                    if (tilesetID == GlobalDungeonData.ValidTilesets.OFFICEGEON)
                    {
                        return 4;
                    }
                    if (tilesetID == GlobalDungeonData.ValidTilesets.FINALGEON)
                    {
                        return 5;
                    }
                    if (tilesetID != GlobalDungeonData.ValidTilesets.RATGEON)
                    {
                        return 3;
                    }
                    return 3;
                case GlobalDungeonData.ValidTilesets.SEWERGEON:
                    return 1;
                case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
                    return 2;
            }
        }

        private static IEnumerator InvariantWait(PlayerController player, float delay)
        {
            float elapsed = 0f;
            while (elapsed < delay)
            {
                if (GameManager.INVARIANT_DELTA_TIME == 0f)
                {
                    elapsed += 0.05f;
                }
                elapsed += GameManager.INVARIANT_DELTA_TIME;
                player.spriteAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                yield return null;
            }
            yield break;
        }
    }
}
