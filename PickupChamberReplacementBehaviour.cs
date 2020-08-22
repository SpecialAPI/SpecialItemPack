using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialItemPack
{
    class PickupChamberReplacementBehaviour : BraveBehaviour
    {
        private void Start()
        {
            if (GameManager.HasInstance && GameManager.Instance.Dungeon != null && GameManager.Instance.Dungeon.tileIndices != null && GameManager.Instance.Dungeon.tileIndices.tilesetId == (GlobalDungeonData.ValidTilesets)CustomValidTilesets.CHAMBERGEON)
            {
                if (this.destroysSpriteAnimator && base.spriteAnimator)
                {
                    Destroy(base.spriteAnimator);
                }
                if (this.stopAnimation && base.spriteAnimator)
                {
                    base.spriteAnimator.Stop();
                    base.spriteAnimator.playAutomatically = false;
                }
                if (setsSprite)
                {
                    base.sprite.SetSprite(newCollection, newSpriteId);
                }
                if (this.overridesAnimations && base.spriteAnimator)
                {
                    foreach (string name in this.overrideAnimations.Keys)
                    {
                        if (base.spriteAnimator.GetClipByName(name) != null)
                        {
                            Toolbox.Remove(ref base.spriteAnimator.Library.clips, base.spriteAnimator.GetClipByName(name));
                            Toolbox.Add(ref base.spriteAnimator.Library.clips, this.overrideAnimations[name]);
                        }
                    }
                }
                if (this.addsAnimations && base.spriteAnimator)
                {
                    foreach (tk2dSpriteAnimationClip clip in this.animationsToAdd.Values)
                    {
                        Toolbox.Add(ref base.spriteAnimator.Library.clips, clip);
                    }
                }
                if (addsOutlineToSprite)
                {
                    SpriteOutlineManager.AddOutlineToSprite(base.sprite, outlineColor);
                }
            }
        }

        private void Update()
        {
            if (GameManager.HasInstance && GameManager.Instance.Dungeon != null && GameManager.Instance.Dungeon.tileIndices != null && GameManager.Instance.Dungeon.tileIndices.tilesetId == (GlobalDungeonData.ValidTilesets)CustomValidTilesets.CHAMBERGEON)
            {
                if (this.addsOutlineToSprite && this.attemptToAddOutlineAfterCreation && !SpriteOutlineManager.HasOutline(base.sprite))
                {
                    SpriteOutlineManager.AddOutlineToSprite(base.sprite, outlineColor);
                }
            }
        }

        public bool setsSprite = false;
        public int newSpriteId = -1;
        public tk2dSpriteCollectionData newCollection = null;
        public bool stopAnimation = true;
        public bool playsAnimation = false;
        public string animationToPlay = "";
        public bool overridesAnimations = false;
        public Dictionary<string, tk2dSpriteAnimationClip> overrideAnimations = new Dictionary<string, tk2dSpriteAnimationClip>();
        public bool addsAnimations = false;
        public Dictionary<string, tk2dSpriteAnimationClip> animationsToAdd = new Dictionary<string, tk2dSpriteAnimationClip>();
        public bool addsOutlineToSprite = true;
        public Color outlineColor = Color.black;
        public bool destroysSpriteAnimator = false;
        public bool attemptToAddOutlineAfterCreation = false;
    }
}
