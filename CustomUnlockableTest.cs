using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class CustomUnlockableTest : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Custom Flag Tests";
            string resourceName = "SpecialItemPack/Resources/Evo2Bullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<CustomUnlockableTest>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "blah blah blah";
            string longDesc = "pheas";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.C;
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(822);
        }
    }
}
