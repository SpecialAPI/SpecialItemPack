using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;
using System.Collections;
using Dungeonator;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace SpecialItemPack
{
    class IgnoreRounds : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Ignore Rounds";
            string resourceName = "SpecialItemPack/Resources/IgnoreRounds";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<IgnoreRounds>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "I'm Not Even Listening";
            string longDesc = "These bullets will ignore all sorts of protection.\n\nThey don't care if the enemy is an immortal being, a demon from hell or is immune to fire, they hit everyone equally hard.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.A;
            item.AddToTrorkShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(407);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.OnPostProcessProjectile;
        }

        private void OnPostProcessProjectile(Projectile proj, float f)
        {
            proj.gameObject.AddComponent<IgnoreBehaviour>();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.OnPostProcessProjectile;
            return base.Drop(player);
        }
    }
}
