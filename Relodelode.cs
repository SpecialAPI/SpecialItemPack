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
    class Relodelode : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Relode-Lode";
            string resourceName = "SpecialItemPack/Resources/Relodelode";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Relodelode>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Just Reload It";
            string longDesc = "This cylinder shows that reloading at a full clip is possible, you all were just too lazy to do it.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.C;
            item.AddToTrorkShop();
            item.PlaceItemInAmmonomiconAfterItemById(135);
            Game.Items.Rename("spapi:relode-lode", "spapi:relode_lode");
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReloadPressed += this.ReloadIt;
        }

        public void ReloadIt(PlayerController player, Gun gun)
        {
            base.StartCoroutine(this.DelayedTryReload(player, gun));
        }

        public IEnumerator DelayedTryReload(PlayerController player, Gun gun)
        {
            yield return null;
            if (!gun.IsReloading)
            {
                int clipshotsremainingLast = gun.ClipShotsRemaining;
                gun.ClipShotsRemaining = (gun.DefaultModule.numberOfShotsInClip - 1);
                gun.Reload();
                gun.ClipShotsRemaining = clipshotsremainingLast;
            }
            yield break;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnReloadPressed -= this.ReloadIt;
            return base.Drop(player);
        }
    }
}
