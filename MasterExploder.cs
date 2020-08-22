using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;
using System.Collections;

namespace SpecialItemPack
{
    class MasterExploder : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Master Exploder";
            string resourceName = "SpecialItemPack/Resources/PeashooterSeeds";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<MasterExploder>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Wondawondawonder!";
            string longDesc = "A transformative spell of incredible power.\n\nThe wizard Alben Smallbore theorized that the more power was put into a spell, the less could be known about its outcome. This spell is immensely powerful.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 150f);
            item.consumable = false;
            item.quality = ItemQuality.S;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(250);
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
        }

        protected override void DoActiveEffect(PlayerController user)
        {

            base.DoActiveEffect(user);
        }
    }
}