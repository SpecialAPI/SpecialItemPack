using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class ModdedMagnetItem : PassiveItem
    {
        public static void InitItems()
        {
            string itemName = "Modded Magnet (Items)";
            string resourceName = "SpecialItemPack/Resources/ModdedItemsMagnet";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ModdedMagnetItem>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Magnets Unusual Items";
            string longDesc = "Increases the loot weight of modded items. Weight increase can be changed through Mod the Gungeon Console's 'changeitemweight' command.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.EXCLUDED;
            item.type = ModdedMagnetItemType.ITEMS;
        }

        public static void InitGuns()
        {
            string itemName = "Modded Magnet (Guns)";
            string resourceName = "SpecialItemPack/Resources/ModdedGunsMagnet";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ModdedMagnetItem>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Magnets Unusual Guns";
            string longDesc = "Increases the loot weight of modded guns. Weight increase can be changed through Mod the Gungeon Console's 'changegunweight' command.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.EXCLUDED;
            item.type = ModdedMagnetItemType.GUNS;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            if(this.type == ModdedMagnetItemType.GUNS)
            {
                this.weight = SpecialItemModule.ModGunWeight;
                foreach(WeightedGameObject obj in GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.elements)
                {
                    if(obj.pickupId > 823 || obj.pickupId < 0)
                    {
                        obj.weight *= this.weight;
                    }
                }
            }
            else if (this.type == ModdedMagnetItemType.ITEMS)
            {
                this.weight = SpecialItemModule.ModItemWeight;
                foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.elements)
                {
                    if (obj.pickupId > 823 || obj.pickupId < 0)
                    {
                        obj.weight *= this.weight;
                    }
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            if (this.type == ModdedMagnetItemType.GUNS)
            {
                this.weight = SpecialItemModule.ModGunWeight;
                foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.elements)
                {
                    if (obj.pickupId > 823 || obj.pickupId < 0)
                    {
                        obj.weight /= this.weight;
                    }
                }
            }
            else if (this.type == ModdedMagnetItemType.ITEMS)
            {
                this.weight = SpecialItemModule.ModItemWeight;
                foreach (WeightedGameObject obj in GameManager.Instance.RewardManager.ItemsLootTable.defaultItemDrops.elements)
                {
                    if (obj.pickupId > 823 || obj.pickupId < 0)
                    {
                        obj.weight /= this.weight;
                    }
                }
            }
            return base.Drop(player);
        }

        public ModdedMagnetItem.ModdedMagnetItemType type;
        public float weight;

        public enum ModdedMagnetItemType
        {
            ITEMS,
            GUNS
        }
    }
}
