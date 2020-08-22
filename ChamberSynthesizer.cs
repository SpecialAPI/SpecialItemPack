using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;
using System.Collections;
using Dungeonator;

namespace SpecialItemPack
{
    class ChamberSynthesizer : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Chamber Synthesizer";
            string resourceName = "SpecialItemPack/Resources/BigChamber";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<BigChamber>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Play Well, Get Chambers";
            string longDesc = "Occasionally produces lesser chambers. Less effective if rattled.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.S;
            item.AddToTrorkShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(570);
        }
    }
}
