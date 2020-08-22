using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;

namespace SpecialItemPack
{
    class ScrollOfWonder : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Scroll of Wonder";
            string resourceName = "SpecialItemPack/Resources/ScrollOfWonder";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ScrollOfWonder>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Wondawondawonder!";
            string longDesc = "A transformative spell of incredible power.\n\nThe wizard Alben Smallbore theorized that the more power was put into a spell, the less could be known about its outcome. This spell is immensely powerful.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 5500);
            item.consumable = false;
            item.quality = ItemQuality.S;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(250);
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            foreach(AIActor aiactor in user.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All))
            {
                if (aiactor != null && aiactor.healthHaver != null && aiactor.healthHaver.IsAlive && !aiactor.healthHaver.IsBoss && !aiactor.IsMimicEnemy && !aiactor.IsHarmlessEnemy)
                {
                    int num = UnityEngine.Random.Range(1, 5);
                    ItemQuality quality;
                    if (num == 1)
                    {
                        quality = ItemQuality.D;
                    }
                    else if (num == 2)
                    {
                        quality = ItemQuality.C;
                    }
                    else if(num == 3)
                    {
                        quality = ItemQuality.B;
                    }
                    else if(num == 4)
                    {
                        quality = ItemQuality.A;
                    }
                    else
                    {
                        quality = ItemQuality.S;
                    }
                    GenericLootTable lootTable = UnityEngine.Random.value <= 0.5f ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable;
                    bool spawnsExtraItem = false;
                    if(UnityEngine.Random.value < 0.5f && user.PlayerHasActiveSynergy("#WONDA-WONDER_WONDER-WONDA!!!"))
                    {
                        spawnsExtraItem = true;
                        GenericLootTable singleItemRewardTable = GameManager.Instance.RewardManager.CurrentRewardData.SingleItemRewardTable;
                        LootEngine.SpawnItem(singleItemRewardTable.SelectByWeight(false), aiactor.sprite.WorldCenter, Vector2.right, 1f, true, false, false);
                    }
                    PickupObject po = LootEngine.GetItemOfTypeAndQuality<PickupObject>(quality, lootTable);
                    LootEngine.SpawnItem(po.gameObject, aiactor.sprite.WorldCenter, spawnsExtraItem ? Vector2.left : Vector2.zero, spawnsExtraItem ? 1f : 0f, true, false, false);
                    LootEngine.DoDefaultItemPoof(aiactor.sprite.WorldCenter);
                    aiactor.EraseFromExistence(true);
                }
            }
        }

        private bool CheckOnEnemies(RoomHandler room)
        {
            bool result = false;
            foreach(AIActor aiactor in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
            {
                if(aiactor != null && aiactor.healthHaver != null && aiactor.healthHaver.IsAlive && !aiactor.healthHaver.IsBoss && !aiactor.IsMimicEnemy && !aiactor.IsHarmlessEnemy)
                {
                    result = true;
                }
            }
            return result;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user.CurrentRoom != null && user.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All) && user.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null && CheckOnEnemies(user.CurrentRoom);
        }
    }
}
