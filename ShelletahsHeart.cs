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
    class ShelletahsHeart : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Shell'tan's Heart";
            string resourceName = "SpecialItemPack/Resources/ShelletahsHeart";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ShelletahsHeart>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Ammoazing Thing!";
            string longDesc = "Reffils ammo, at a cost...\n\nThe heart of Shell'tan the Ammo Elemental. It alone does it's job pretty well...";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 0.5f);
            item.consumable = false;
            item.quality = ItemQuality.A;
            item.AddToCursulaShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(134);
            Game.Items.Rename("spapi:shell'tan's_heart", "spapi:shelltans_heart");
        }

        protected override void DoEffect(PlayerController user)
        {
            foreach(Gun gun in this.GetAllAcceptableGuns(user))
            {
                gun.GainAmmo(gun.AdjustedMaxAmmo);
            }
            user.ownerlessStatModifiers.Add(Toolbox.SetupStatModifier(PlayerStats.StatType.Curse, user.PlayerHasActiveSynergy("#PROSTHETIC_HEART") ? 1.75f : 3.5f));
            user.stats.RecalculateStats(user, true);
        }

        public List<Gun> GetAllAcceptableGuns(PlayerController player)
        {
            List<Gun> result = new List<Gun>();
            if(player != null && player.inventory != null && player.inventory.AllGuns != null)
            {
                foreach(Gun gun in player.inventory.AllGuns)
                {
                    if (!gun.InfiniteAmmo && gun.ammo < gun.AdjustedMaxAmmo && gun.CanGainAmmo)
                    {
                        result.Add(gun);
                    }
                }
            }
            return result;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return this.GetAllAcceptableGuns(user).Count > 0 && base.CanBeUsed(user);
        }
    }
}
