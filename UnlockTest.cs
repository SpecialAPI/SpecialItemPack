using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;

namespace SpecialItemPack
{
    class UnlockTest : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Unlock Test!!!";
            string resourceName = "SpecialItemPack/Resources/Evo6Bullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<UnlockTest>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "It worked!!!";
            string longDesc = "It's an unlock test.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.C;
            item.PlaceItemInAmmonomiconAfterItemById(293);
            item.SetupUnlockOnFlag(GungeonFlags.TUTORIAL_COMPLETED, true);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.RandomizeDamage;
            player.OnReloadedGun += this.RerollRanges;
        }

        private void RandomizeDamage(Projectile proj, float f)
        {
            float increase = UnityEngine.Random.Range(1 - this.range, 1 + this.range);
            proj.baseData.damage *= increase;
        }

        private void RerollRanges(PlayerController player, Gun gun)
        {
            AkSoundEngine.PostEvent("Play_OBJ_Chest_Synergy_Slots_01", base.gameObject);
            this.range = UnityEngine.Random.Range(0.1f, 0.5f);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.RandomizeDamage;
            player.OnReloadedGun -= this.RerollRanges;
            return base.Drop(player);
        }

        private float range = 0.25f;
    }
}
