using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoMod.RuntimeDetour;
using System.Reflection;
using UnityEngine;

namespace SpecialItemPack
{
    class SpecialBlankModificationItem : BlankModificationItem
    {
        public static void InitHooks()
        {
            Hook hook = new Hook(
                typeof(SilencerInstance).GetMethod("ProcessBlankModificationItemAdditionalEffects", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(SpecialBlankModificationItem).GetMethod("BlankModificationHook")
            );
        }

        public static void BlankModificationHook(Action<SilencerInstance, BlankModificationItem, Vector2, PlayerController> orig, SilencerInstance self, BlankModificationItem bmi, Vector2 centerPoint, PlayerController user)
        {
            orig(self, bmi, centerPoint, user);
            if(bmi is SpecialBlankModificationItem)
            {
                (bmi as SpecialBlankModificationItem).OnBlank(centerPoint, user);
            }
        }

        protected virtual void OnBlank(Vector2 centerPoint, PlayerController user)
        {

        }
    }
}
