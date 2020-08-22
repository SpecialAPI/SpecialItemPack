using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class MysteriousChest : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Mysterious Chest";
            string resourceName = "SpecialItemPack/Resources/MysteriousChest"; 
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<MysteriousChest>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Contains Wonder..?";
            string longDesc = "A mysterious chest hardly locked up. You don't exactly know what's inside, but it has to be something very special.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi"); 
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 15000);
            item.consumable = true;
            item.quality = ItemQuality.S;
            item.AddToFlyntShop();
            item.PlaceItemInAmmonomiconAfterItemById(525);
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.CUSTOMFLAG_OPENED_BLACK_CHEST, true);
        }

        public override void Pickup(PlayerController player)
        {
            if (!this.m_pickedUpThisRun)
            {
                this.ApplyCooldown(player);
            }
            base.Pickup(player);
        }

        protected override void DoEffect(PlayerController user)
        {
            Chest dragunchest = Chest.Spawn(GameManager.Instance.RewardManager.A_Chest, user.sprite.WorldTopCenter.ToIntVector2(), user.CurrentRoom, true);
            dragunchest.overrideMimicChance = 0;
            if (GameStatsManager.Instance.IsRainbowRun)
            {
                dragunchest.BecomeRainbowChest();
            }
            List<int> possibleContents = new List<int> { 146, 670, 677 };
            dragunchest.forceContentIds = new List<int> { possibleContents[UnityEngine.Random.Range(0, possibleContents.Count)] };
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user);
        }
    }
}
