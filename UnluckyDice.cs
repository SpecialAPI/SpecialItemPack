using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;

namespace SpecialItemPack
{
    class UnluckyDice : PassiveItem
    {
        public static void Init()
        {
            string itemName = "(Un)Lucky Dice";
            string resourceName = "SpecialItemPack/Resources/UnluckyDice";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<UnluckyDice>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Maybe You Should Get Lucky";
            string longDesc = "Randomizes damage. Reloading rerolls the range.\n\nThis ancient dice was used to solve bets, before it was cursed. It was lost, later until someone VERY (un)lucky found it.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = ItemQuality.B;
            Game.Items.Rename("spapi:(un)lucky_dice", "spapi:unlucky_dice");
            item.PlaceItemInAmmonomiconAfterItemById(293);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.RandomizeDamage;
            player.OnReloadedGun += this.RerollRanges;
        }

        private void RandomizeDamage(Projectile proj, float f)
        {
            float increase = UnityEngine.Random.Range(1 - (this.m_owner.PlayerHasActiveSynergy("#?LUCKY_DICE") ? this.range/2 : this.range), 1 + this.range);
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
