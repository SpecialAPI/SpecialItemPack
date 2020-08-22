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
    class TableTechTableflip
    {
        public static void Init()
        {
            string itemName = "Table Tech Tableflip";
            string resourceName = "SpecialItemPack/Resources/TableTechTableflip";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<TableFlipItem>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Flipping Flips";
            string longDesc = "This ancient technique flips tables when a table is flipped.\n\nThe tenth chapter of the \"Tabla Sutra.\" The ultimate tableflip technique ever known.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = PickupObject.ItemQuality.D;
            item.AddToTrorkShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(407);
            item.AddItemToSynergy(CustomSynergyType.PAPERWORK);
            item.TableFlocking = true;
        }
    }
}
