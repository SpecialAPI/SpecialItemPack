using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;

namespace SpecialItemPack
{
    class GlassChamber : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Glass Chamber (SpecialItemPack)";
            string resourceName = "SpecialItemPack/Resources/glass_chamber";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<GlassChamber>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Fleeting Fire";
            string longDesc = "Increases firerate, max ammo, and negates reload speed, but breaks itself, and the current gun when owner takes damage.\n\nItem from the Glass Kingdom, the home of Lady of Pane. It was made for glass people, mainly warriors, " +
                "to increase their gun efficiency. Through, nobody thought someone except glass people would use it, and so they didn't prevent it's brittleness and because guns in the Glass Kingdom are made out of glass too, they also didn't prevent the " +
                "gun fragility too.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RateOfFire, 0.5f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AmmoCapacityMultiplier, 1.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ReloadSpeed, 0f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            item.quality = ItemQuality.C;
            item.SetName("Glass Chamber");
            Game.Items.Rename("spapi:glass_chamber_(specialitempack)", "spapi:glass_chamber");
            item.PlaceItemInAmmonomiconAfterItemById(499);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += this.BreakOnDamage;
        }

        public void BreakOnDamage(PlayerController player)
        {
            if (!player.PlayerHasActiveSynergy("#NO_TIME_TO_BREAK") || (UnityEngine.Random.value <= 0.15f) && !player.PlayerHasActiveSynergy("#DIAMONDER_GUON_STONE"))
            {
                if (!player.CurrentGun.InfiniteAmmo)
                {
                    player.inventory.DestroyCurrentGun();
                }
                player.DropPassiveItem(this);
                player.OnReceivedDamage -= this.BreakOnDamage;
            }
        }

        protected override void OnDestroy()
        {
            if ((PickupObjectDatabase.GetById(565) as PlayerOrbitalItem).BreakVFX && this.sprite != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(565) as PlayerOrbitalItem).BreakVFX, this.sprite.WorldCenter.ToVector3ZisY(0f), Quaternion.identity);
                }
                tk2dBaseSprite sprite = SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(538) as SilverBulletsPassiveItem).SynergyPowerVFX, this.sprite.WorldCenter.ToVector3ZisY(0f), Quaternion.identity).GetComponent<tk2dBaseSprite>();
                sprite.PlaceAtPositionByAnchor(this.sprite.WorldCenter.ToVector3ZisY(0f), tk2dBaseSprite.Anchor.MiddleCenter);
            }
            base.OnDestroy();
        }

        public void Break()
        {
            this.m_pickedUp = true;
            UnityEngine.Object.Destroy(base.gameObject, 1f);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            GlassChamber component = debrisObject.GetComponent<GlassChamber>();
            player.OnReceivedDamage -= component.BreakOnDamage;
            component.m_pickedUpThisRun = true;
            component.Break();
            return debrisObject;
        }
    }
}
