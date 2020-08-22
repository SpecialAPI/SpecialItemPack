using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;
using Gungeon;
using MonoMod.RuntimeDetour;

namespace SpecialItemPack
{
    class ReallySpecialGramophone : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Really Special Gramophone";
            string resourceName = "SpecialItemPack/Resources/ReallySpecialGramophone";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ReallySpecialGramophone>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Old-Fashioned";
            string longDesc = "Energizes companionable listeners.\n\nThis gramophone plays a beautiful melody. Everyone likes it, and will try to fight harder when it plays.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.C;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(119);
            Hook hook = new Hook(
                typeof(Gun).GetProperty("LuteCompanionBuffActive").GetGetMethod(),
                typeof(ReallySpecialGramophone).GetMethod("LuteCompanionBuffActiveHook")
            );
        }

        public static bool LuteCompanionBuffActiveHook(Func<Gun, bool> orig, Gun self)
        {
            if(self.CurrentOwner is PlayerController)
            {
                foreach(PassiveItem passive in (self.CurrentOwner as PlayerController).passiveItems)
                {
                    if(passive is ReallySpecialGramophone)
                    {
                        return true;
                    }
                }
            }
            return orig(self);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            AkSoundEngine.SetSwitch("WPN_Guns", "Lute", base.gameObject);
            AkSoundEngine.PostEvent("Play_WPN_gun_shot_01", base.gameObject);
            player.OnNewFloorLoaded += this.ResetMusic;
        }

        private void ResetMusic(PlayerController player)
        {
            AkSoundEngine.SetSwitch("WPN_Guns", "Lute", base.gameObject);
            AkSoundEngine.PostEvent("Play_WPN_gun_shot_01", base.gameObject);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnNewFloorLoaded -= this.ResetMusic;
            AkSoundEngine.PostEvent("Stop_WPN_gun_loop_01", base.gameObject);
            return base.Drop(player);
        }
    }
}
