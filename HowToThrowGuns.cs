using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class HowToThrowGuns : PlayerItem
    {
        public static void Init()
        {
            string itemName = "How To Throw Guns";
            string resourceName = "SpecialItemPack/Resources/howtothrowbooks";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<HowToThrowGuns>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "An Advanced Guide";
            string longDesc = "Makes the user throw their current gun.\n\nThis guide contains the forgotten technique of throwing non-empty guns.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 0.5f);
            item.consumable = false;
            item.quality = ItemQuality.D;
            item.PlaceItemInAmmonomiconAfterItemById(814);
        }

        protected override void DoEffect(PlayerController user)
        {
            if(user.CurrentGun != null && !user.CurrentGun.InfiniteAmmo)
            {
                user.CurrentGun.PrepGunForThrow();
                typeof(Gun).GetField("m_prepThrowTime", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(user.CurrentGun, 999);
                user.CurrentGun.CeaseAttack();
            }
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return user.CurrentGun != null && !user.CurrentGun.InfiniteAmmo && base.CanBeUsed(user);
        }
    }
}
