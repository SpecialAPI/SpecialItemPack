using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class UglyChest : Chest
    {
        public static void Init()
        {
            GameObject obj = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/UglyChest/chest_ugly_idle_001", new GameObject("UglyRedChest"));
            obj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(obj);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            tk2dSprite sprite = obj.GetComponent<tk2dSprite>();
            SpeculativeRigidbody body = sprite.SetUpSpeculativeRigidbody(new IntVector2(0, 0), new IntVector2(12, 12));
            body.PrimaryPixelCollider.CollisionLayer = CollisionLayer.HighObstacle;
            tk2dSpriteAnimator animator = obj.AddComponent<tk2dSpriteAnimator>();
            animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animator.Library.clips = new tk2dSpriteAnimationClip[0];
            animator.Library.enabled = true;
            List<int> appearIds = new List<int>();
            List<int> breakIds = new List<int>();
            List<int> openIds = new List<int>();
            string zeroHpName = "";
            foreach (string text in UglyChest.spritePaths)
            {
                if (text.Contains("appear"))
                {
                    appearIds.Add(SpriteBuilder.AddSpriteToCollection(text, obj.GetComponent<tk2dBaseSprite>().Collection));
                }
                else if (text.Contains("break"))
                {
                    if (text.EndsWith("001"))
                    {
                        int id = SpriteBuilder.AddSpriteToCollection(text, obj.GetComponent<tk2dBaseSprite>().Collection);
                        zeroHpName = obj.GetComponent<tk2dBaseSprite>().Collection.inst.spriteDefinitions[id].name;
                    }
                    else
                    {
                        breakIds.Add(SpriteBuilder.AddSpriteToCollection(text, obj.GetComponent<tk2dBaseSprite>().Collection));
                    }
                }
                else if(text.Contains("open"))
                {
                    openIds.Add(SpriteBuilder.AddSpriteToCollection(text, obj.GetComponent<tk2dBaseSprite>().Collection));
                }
                else
                {
                    appearIds.Add(SpriteBuilder.AddSpriteToCollection(text, obj.GetComponent<tk2dBaseSprite>().Collection));
                }
            }
            appearIds.Add(sprite.spriteId);
            tk2dSpriteAnimationClip openClip = new tk2dSpriteAnimationClip { name = "open", fps = 10, frames = new tk2dSpriteAnimationFrame[0] };
            foreach (int id in openIds)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame
                {
                    spriteId = id,
                    spriteCollection = obj.GetComponent<tk2dBaseSprite>().Collection
                };
                openClip.frames = openClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            openClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library.clips = animator.Library.clips.Concat(new tk2dSpriteAnimationClip[] { openClip }).ToArray();
            tk2dSpriteAnimationClip appearClip = new tk2dSpriteAnimationClip { name = "appear", fps = 10, frames = new tk2dSpriteAnimationFrame[0] };
            foreach (int id in appearIds)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame
                {
                    spriteId = id,
                    spriteCollection = obj.GetComponent<tk2dBaseSprite>().Collection
                };
                appearClip.frames = appearClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            appearClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library.clips = animator.Library.clips.Concat(new tk2dSpriteAnimationClip[] { appearClip }).ToArray();
            tk2dSpriteAnimationClip breakClip = new tk2dSpriteAnimationClip { name = "break", fps = 10, frames = new tk2dSpriteAnimationFrame[0] };
            foreach (int id in breakIds)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame
                {
                    spriteId = id,
                    spriteCollection = obj.GetComponent<tk2dBaseSprite>().Collection
                };
                breakClip.frames = breakClip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            breakClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library.clips = animator.Library.clips.Concat(new tk2dSpriteAnimationClip[] { breakClip }).ToArray();
            Chest chest = obj.AddComponent<Chest>();
            chest.spawnCurve = new AnimationCurve { keys = new Keyframe[] { new Keyframe { time = 0f, value = 0f, inTangent = 3.562501f, outTangent = 3.562501f }, new Keyframe { time = 1f, value = 1.0125f, inTangent = 0.09380959f, 
                outTangent = 0.09380959f } } };
            UglyChest.UglyRedChest = chest;
            chest.openAnimName = "open";
            chest.spawnAnimName = "appear";
            chest.majorBreakable = obj.AddComponent<MajorBreakable>();
            chest.majorBreakable.spriteNameToUseAtZeroHP = zeroHpName;
            chest.majorBreakable.usesTemporaryZeroHitPointsState = true;
            chest.breakAnimName = "break";
            chest.VFX_GroundHit = new GameObject("example thingy");
            chest.VFX_GroundHit.transform.parent = chest.transform;
            chest.VFX_GroundHit.SetActive(false);
            chest.groundHitDelay = 2f / 10f;
        }

        public static string[] spritePaths = new string[]
        {
            "SpecialItemPack/Resources/UglyChest/chest_ugly_appear_001",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_appear_002",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_appear_003",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_appear_004",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_appear_005",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_break_001",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_break_002",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_break_003",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_break_004",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_open_001",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_open_002",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_open_003",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_open_004",
            "SpecialItemPack/Resources/UglyChest/chest_ugly_open_005",
        };

        public static Chest UglyRedChest;
    }
}
