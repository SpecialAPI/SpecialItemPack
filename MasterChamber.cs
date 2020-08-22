using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;

namespace SpecialItemPack
{
    class MasterChamber
    {
        public static void Init()
        {
            string itemName = "Master Chamber";
            string resourceName = "SpecialItemPack/Resources/MasterChamber";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<BasicStatPickup>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Red Chamber";
            string longDesc = "This forgotten artifact indicates mastery of the red chamber.\n\nBecause of a paradox, this Master Round was turned into a chamber.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = PickupObject.ItemQuality.SPECIAL;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, 1f, StatModifier.ModifyMethod.ADDITIVE);
            EncounterDatabase.GetEntry(item.encounterTrackable.EncounterGuid).usesPurpleNotifications = true;
            item.IsMasteryToken = true;
            item.PlaceItemInAmmonomiconAfterItemById(467);
            item.AddItemToSynergy(CustomSynergyType.MASTERS_CHAMBERS);
            SpecialItemIds.MasterChamber = item.PickupObjectId;
        }
    }
}
