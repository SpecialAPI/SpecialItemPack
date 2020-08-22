using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;

namespace SpecialItemPack
{
    class StoneJunk : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Stone Junk";
            string resourceName = "SpecialItemPack/Resources/StoneJunk";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<StoneJunk>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "May Be Worth Keeping Around";
            string longDesc = "A nearly-perfect replica of a sack of junk, made out of stone. Also it has some sort of sentience, and will eat normal junk, returning useful things.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 0.5f);
            item.consumable = false;
            item.quality = ItemQuality.C;
            item.PlaceItemInAmmonomiconAfterItemById(580);
        }

        protected override void DoEffect(PlayerController user)
        {
            foreach (PassiveItem passive in user.passiveItems)
            {
                if (passive is BasicStatPickup)
                {
                    if ((passive as BasicStatPickup).IsJunk && passive.PickupObjectId != SpecialItemIds.JunkUp)
                    {
                        bool isLies = passive.PickupObjectId == 148;
                        bool isGoldJunk = (passive as BasicStatPickup).GivesCurrency;
                        user.RemovePassiveItem(passive.PickupObjectId);
                        LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(120).gameObject, user);
                        if (isLies)
                        {
                            LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(120).gameObject, user);
                            if(user.CurrentGun != null)
                            {
                                user.CurrentGun.GainAmmo(user.CurrentGun.AdjustedMaxAmmo);
                            }
                            user.ownerlessStatModifiers.Add(Toolbox.SetupStatModifier(PlayerStats.StatType.Damage, 0.05f, StatModifier.ModifyMethod.ADDITIVE));
                            user.stats.RecalculateStats(user, true, false);
                        }
                        if (isGoldJunk)
                        {
                            user.carriedConsumables.KeyBullets += 15;
                            user.carriedConsumables.Currency += 50;
                            user.ownerlessStatModifiers.Add(Toolbox.SetupStatModifier(PlayerStats.StatType.GlobalPriceMultiplier, 0f, StatModifier.ModifyMethod.MULTIPLICATIVE));
                            user.stats.RecalculateStats(user, true, false);
                        }
                        if (user.PlayerHasActiveSynergy("#CANT_TALK_BUT_STILL_FUN"))
                        {
                            user.AcquirePassiveItemPrefabDirectlyForFakePrefabs(PickupObjectDatabase.GetById(SpecialItemIds.JunkUp) as PassiveItem);
                            user.AcquirePassiveItemPrefabDirectlyForFakePrefabs(PickupObjectDatabase.GetById(SpecialItemIds.JunkUp) as PassiveItem);
                        }
                        AkSoundEngine.PostEvent("Play_NPC_BabyDragun_Munch_01", this.gameObject);
                        break;
                    }
                }
            }
        }

        public bool HasJunk(PlayerController owner)
        {
            foreach(PassiveItem passive in owner.passiveItems)
            {
                if(passive is BasicStatPickup)
                {
                    if((passive as BasicStatPickup).IsJunk && passive.PickupObjectId != SpecialItemIds.JunkUp)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return this.HasJunk(user) && base.CanBeUsed(user);
        }
    }
}
