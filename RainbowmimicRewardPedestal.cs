using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace SpecialItemPack
{
    class RainbowmimicRewardPedestal : BreachShrineController
    {
        public IEnumerator Start()
        {
            while (Foyer.DoIntroSequence || Foyer.DoMainMenu)
            {
                yield return null;
            }
            this.GetComponent<tk2dBaseSprite>().HeightOffGround = -1f;
            this.GetComponent<tk2dBaseSprite>().UpdateZDepth();
            if (AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BOSS_RAINBOWMIMIC_DEFEATED))
            {
                this.GetComponent<tk2dBaseSprite>().spriteId = rewardSpriteId;
            }
            yield break;
        }

        public override void Interact(PlayerController interactor)
        {
            base.Interact(interactor);
            if (TextBoxManager.HasTextBox(this.talkPoint))
            {
                return;
            }
            string displayKey = AdvancedGameStatsManager.Instance.GetFlag(CustomDungeonFlags.BOSS_RAINBOWMIMIC_DEFEATED) ? "#RAINBOWMIMIC_REWARD_PEDESTAL" : "#RAINBOWMIMIC_REWARD_PEDESTAL_EMPTY";
            TextBoxManager.ShowStoneTablet(this.talkPoint.position, this.talkPoint, -1f, StringTableManager.GetLongString(displayKey), true, false);
        }

        public override void OnExitRange(PlayerController interactor)
        {
            base.OnExitRange(interactor);
            if (TextBoxManager.HasTextBox(this.talkPoint))
            {
                TextBoxManager.ClearTextBox(this.talkPoint);
            }
        }

        public Transform talkPoint;
        public static int rewardSpriteId;
    }
}
