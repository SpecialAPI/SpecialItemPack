using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;

namespace SpecialItemPack
{
    class GlassBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Glass Bullets (SpecialItemPack)";
            string resourceName = "SpecialItemPack/Resources/GlassBullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<GlassBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Fleeting Damage";
            string longDesc = "Increases damage, but will be shattered upon taking damage.\n\nThe bullets made in Glass Kingdom, the home of the Lady of Pane. The bullets were designed to grant a decent amount of damage, but nobody thought that non-glass" +
                "people would use it, so creators of the item did not prevent the fragility of theese bullets.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 0.5f, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = ItemQuality.D;
            item.PlaceItemInAmmonomiconAfterItemById(662);
            item.SetName("Glass Bullets");
            Game.Items.Rename("spapi:glass_bullets_(specialitempack)", "spapi:glass_bullets");
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReceivedDamage += this.BreakOnDamage;
        }

        public void BreakOnDamage(PlayerController player)
        {
            if(!player.PlayerHasActiveSynergy("#NO_TIME_TO_BREAK") || (UnityEngine.Random.value <= 0.15f) && !player.PlayerHasActiveSynergy("#DIAMONDER_GUON_STONE"))
            {
                player.DropPassiveItem(this);
            }
        }

        protected override void OnDestroy()
        {
            if ((PickupObjectDatabase.GetById(565) as PlayerOrbitalItem).BreakVFX && this.sprite != null)
            {
                for(int i = 0; i < 5; i++)
                {
                    SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(565) as PlayerOrbitalItem).BreakVFX, this.sprite.WorldCenter.ToVector3ZisY(0f), Quaternion.identity);
                }
                SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(538) as SilverBulletsPassiveItem).SynergyPowerVFX, this.sprite.WorldCenter.ToVector3ZisY(0f), Quaternion.identity).GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(
                    this.sprite.WorldCenter.ToVector3ZisY(0f), tk2dBaseSprite.Anchor.MiddleCenter);
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
            GlassBullets component = debrisObject.GetComponent<GlassBullets>();
            player.OnReceivedDamage -= component.BreakOnDamage;
            component.m_pickedUpThisRun = true;
            component.Break();
            return debrisObject;
        }
    }
}
