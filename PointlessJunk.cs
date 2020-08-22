using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class PointlessJunk : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Pointless Junk";
            string resourceName = "SpecialItemPack/Resources/PointlessJunk";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<PointlessJunk>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Next Time Don't Play With Chaos";
            string longDesc = "The item inside the chest was completelly downgraded by the Key of Chaos and now became this pointless piece of junk.\n\nDoesn't do anything.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.SPECIAL;
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(127);
            SpecialItemIds.PointlessJunk = item.PickupObjectId;
        }
    }
}
