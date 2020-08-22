using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class JunkUp : BasicStatPickup
    {
        public static void Init()
        {
            string itemName = "Junk Up";
            string resourceName = "SpecialItemPack/Resources/HappyJunk";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<JunkUp>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Happy!";
            string longDesc = "Empowers your fellow Junk Companion.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.SPECIAL;
            item.PlaceItemInAmmonomiconAfterItemById(580);
            item.modifiers = new List<StatModifier>();
            item.ArmorToGive = 0;
            item.ModifiesDodgeRoll = false;
            item.IsJunk = true;
            item.GivesCurrency = false;
            item.IsMasteryToken = false;
            SpecialItemIds.JunkUp = item.PickupObjectId;
        }
    }
}
