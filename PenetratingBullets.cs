using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class PenetratingBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Penetrating Bullets";
            string resourceName = "SpecialItemPack/Resources/PenetratingBullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<PenetratingBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Forward, No Matter What";
            string longDesc = "Increases bullet penetration to further beyond.\n\nMade by a crazy gungeoneer who was not satisfied with how Ghost Bullets pierce, so he made this.\n\nTheir pointed slugs will help them penetrate almost everything and fly as far" +
                " as possible, while their neverchanging serious expression will help them not lose anything during penetration.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.A;
            item.AddToBlacksmithShop();
            item.AddToTrorkShop();
            item.PlaceItemInAmmonomiconAfterItemById(172);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.AntiCaught;
        }

        public void AntiCaught(Projectile proj, float f)
        {
            PierceProjModifier penetrateMod = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
            penetrateMod.penetratesBreakables = true;
            penetrateMod.penetration = 999;
            penetrateMod.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
            penetrateMod.preventPenetrationOfActors = false;
            proj.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnCollision);
            proj.baseData.range *= 999;
        }

        public void OnCollision(CollisionData data)
        {
            Projectile proj = data.MyRigidbody.projectile;
            if(proj != null)
            {
                FieldInfo info = typeof(Projectile).GetField("m_hasPierced", BindingFlags.NonPublic | BindingFlags.Instance);
                info.SetValue(proj, false);
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.AntiCaught;
            return base.Drop(player);
        }
    }
}
