using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using System.Collections;

namespace SpecialItemPack
{
    class ScaryRedChamberBehaviour : DungeonPlaceableBehaviour, IPlaceConfigurable, IPlayerInteractable
    {
        public void ConfigureOnPlacement(RoomHandler room)
        {
        }

        public void Start()
        {
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        public void Interact(PlayerController interactor)
        {
            if (this.isDoingSuck)
            {
                return;
            }
            this.isDoingSuck = true;
            base.StartCoroutine(this.DoSuck(interactor));
        }

        private IEnumerator DoSuck(PlayerController player)
        {
            GameManager.Instance.MainCameraController.SetManualControl(true, true);
            GameManager.Instance.MainCameraController.OverridePosition = base.sprite.WorldCenter;
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            AkSoundEngine.PostEvent("Play_ENM_darken_world_01", base.gameObject);
            base.sprite.UpdateZDepth();
            player.SetInputOverride("scary red chamber");
            GameUIRoot.Instance.ToggleUICamera(false);
            base.spriteAnimator.Play("melt");
            while (base.spriteAnimator.IsPlaying("melt"))
            {
                yield return null;
            }
            float ela = 0f;
            while(ela < 1f)
            {
                ela += BraveTime.DeltaTime;
                yield return null;
            }
            base.spriteAnimator.Play("melt_more");
            player.IsVisible = false;
            Transform copySprite = this.CreateEmptySprite(player);
            Vector3 startPosition = copySprite.transform.position;
            float elapsed = 0f;
            float duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                Vector3 position = base.sprite.WorldCenter;
                float t = elapsed / duration * (elapsed / duration);
                copySprite.position = Vector3.Lerp(startPosition, position, t);
                copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                yield return null;
            }
            if (copySprite)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
            }
            while (base.spriteAnimator.IsPlaying("melt_more"))
            {
                yield return null;
            }
            AkSoundEngine.PostEvent("Play_ENM_lighten_world_01", base.gameObject);
            base.spriteAnimator.Play("unmelt");
            while (base.spriteAnimator.IsPlaying("unmelt"))
            {
                yield return null;
            }
            Pixelator.Instance.FadeToBlack(0.5f, false, 0f);
            yield return new WaitForSeconds(0.5f);
            player.ClearInputOverride("scary red chamber");
            GameManager.Instance.MainCameraController.SetManualControl(false, true);
            GameManager.Instance.LoadCustomLevel("spapi_chamber");
            yield break;
        }

        private Transform CreateEmptySprite(PlayerController target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            return gameObject2.transform;
        }

        public void OnExitRange(PlayerController interactor)
        {
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            base.sprite.UpdateZDepth();
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite)
            {
                return 1000f;
            }
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2));
        }

        public float GetOverrideMaxDistance()
        {
            return -1f;
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        private bool isDoingSuck = false;
    }
}
