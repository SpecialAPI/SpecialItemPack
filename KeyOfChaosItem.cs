using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MonoMod.RuntimeDetour;
using System.Reflection;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class KeyOfChaosItem : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Key of Chaos";
            string resourceName = "SpecialItemPack/Resources/KeyOfChaos_001";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<KeyOfChaosItem>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/KeyOfChaos_002", item.sprite.Collection);
            SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/KeyOfChaos_003", item.sprite.Collection);
            string shortDesc = "What a Mess!";
            string longDesc = "Disorganizes chests, but adds a chance to not consume a key.\n\nItem made by nearly a god, who didn't like to sort his storage. He wished he could open all chest without any keys, and decided to make this item. He almost" +
                "succeeded, but a part of his disorganized nature is now in this key, messing up everything it touches.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.B;
            Hook hook = new Hook (
                typeof(Chest).GetMethod("Interact", BindingFlags.Public | BindingFlags.Instance),
                typeof(KeyOfChaosItem).GetMethod("OnInteract")
            );
            Hook hook2 = new Hook(
                typeof(Chest).GetMethod("orig_Open", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(KeyOfChaosItem).GetMethod("OnOpen")
            );
            item.AddToFlyntShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(166);
        }

        public static void OnOpen(Action<Chest,PlayerController> orig, Chest self, PlayerController player)
        {
            if (self.breakAnimName.Contains("black"))
            {
                AdvancedGameStatsManager.Instance.SetFlag(CustomDungeonFlags.CUSTOMFLAG_OPENED_BLACK_CHEST, true);
            }
            if (ETGMod.Databases.Items["Suspiscious Strongbox"] != null)
            {
                bool hasSynergy = player.PlayerHasActiveSynergy("#BOWLERS_APPROVAL");
                if (hasSynergy && GameStatsManager.Instance.IsRainbowRun && !self.IsRainbowChest)
                {
                    List<PickupObject> d = self.PredictContents(player);
                    List<int> b = new List<int>();
                    foreach (PickupObject po in d)
                    {
                        b.Add(po.PickupObjectId);
                    }
                    self.BecomeRainbowChest();
                    self.forceContentIds = b;
                }
            }
            bool hasCloversFlowersSynergy = player.PlayerHasActiveSynergy("#CLOVERS_FLOWERS");
            if (hasCloversFlowersSynergy)
            {
                self.PredictContents(player);
                self.contents.Add(UnityEngine.Random.value < 0.5f ? PickupObjectDatabase.GetById(78) : PickupObjectDatabase.GetById(600));
            }
            foreach (PassiveItem passive in player.passiveItems)
            {
                if (passive is KeyOfChaosItem)
                {
                    if (!self.IsLocked && !self.IsLockBroken && !self.IsSealed)
                    {
                        if(player.PlayerHasActiveSynergy("#KEY_OF_BATTLE"))
                        {
                            player.ownerlessStatModifiers.Add(Toolbox.SetupStatModifier(PlayerStats.StatType.Damage, 1.75f, StatModifier.ModifyMethod.MULTIPLICATIVE, true));
                        }
                        List<PickupObject> d = self.PredictContents(player);
                        if (d != null)
                        {
                            List<int> contentIds = new List<int>();
                            foreach (PickupObject po in d)
                            {
                                PickupObject.ItemQuality quality = po.quality;
                                float value = UnityEngine.Random.value;
                                bool doubleChaos = player.PlayerHasActiveSynergy("#THERE_IS_ONLY_CHAOS") && UnityEngine.Random.value < 0.5f;
                                bool dropJunk = false;
                                bool isChaosJunk = false;
                                if (value < 0.25f)
                                {
                                    if (po.quality != ItemQuality.SPECIAL && po.quality != ItemQuality.EXCLUDED && po.quality != ItemQuality.COMMON)
                                    {
                                        if ((po.quality == ItemQuality.D && !doubleChaos) || (po.quality == ItemQuality.C && doubleChaos))
                                        {
                                            dropJunk = true;
                                        }
                                        else if(po.quality == ItemQuality.D && doubleChaos)
                                        {
                                            dropJunk = true;
                                            isChaosJunk = true;
                                        }
                                        else
                                        {
                                            quality -= doubleChaos ? 2 : 1;
                                        }
                                    }
                                }
                                else if (value < 0.5f)
                                {
                                    if (po.quality != ItemQuality.SPECIAL && po.quality != ItemQuality.EXCLUDED && po.quality != ItemQuality.COMMON)
                                    {
                                        if ((po.quality == ItemQuality.S && !doubleChaos) || (po.quality == ItemQuality.A && doubleChaos))
                                        {
                                            contentIds.Add(LootEngine.GetItemOfTypeAndQuality<PickupObject>(ItemQuality.D, UnityEngine.Random.value < 0.5f ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable).PickupObjectId);
                                        }
                                        if (po.quality == ItemQuality.S && doubleChaos)
                                        {
                                            contentIds.Add(LootEngine.GetItemOfTypeAndQuality<PickupObject>(ItemQuality.C , UnityEngine.Random.value < 0.5f ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable).PickupObjectId);
                                        }
                                        else
                                        {
                                            quality += doubleChaos ? 2 : 1;
                                        }
                                    }
                                }
                                if (!dropJunk)
                                {
                                    PickupObject item;
                                    if (quality != ItemQuality.COMMON && quality != ItemQuality.EXCLUDED && quality != ItemQuality.SPECIAL)
                                    {
                                        item = LootEngine.GetItemOfTypeAndQuality<PickupObject>(quality, UnityEngine.Random.value < 0.5f ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable);
                                    }
                                    else
                                    {
                                        item = po;
                                    }
                                    contentIds.Add(item.PickupObjectId);
                                }
                                else
                                {
                                    contentIds.Add(isChaosJunk ? SpecialItemIds.PointlessJunk : GlobalItemIds.Junk);
                                }
                            }
                            self.contents = null;
                            self.forceContentIds = contentIds;
                            self.ChestType = (Chest.GeneralChestType)UnityEngine.Random.Range(1, 2);
                        }
                    }
                }
            }
            orig(self, player);
        }

        public static void OnInteract(Action<Chest, PlayerController> orig, Chest self, PlayerController player)
        {
            bool locked = false;
            foreach (PassiveItem passive in player.passiveItems)
            {
                if (passive is KeyOfChaosItem)
                {
                    locked = self.IsLocked && !self.IsLockBroken && !self.IsSealed;
                }
            }
            orig(self, player);
            if (!self.IsLocked && locked)
            {
                if (UnityEngine.Random.value < 0.35f)
                {
                    player.carriedConsumables.KeyBullets += 1;
                }
            }
        }
    }
}
