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
    class ArmorHeartFriendship : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Armor-Heart Friendship";
            string resourceName = "SpecialItemPack/Resources/ArmorHeartFriendship";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ArmorHeartFriendship>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Armor Hearts and Heart Armor!";
            string longDesc = "This figure represents friendship between health and armor. If one of them is tired, and can't do it's job right now, another one can just replace them!";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 0.5f);
            item.consumable = true;
            item.quality = ItemQuality.S;
            item.AddToCursulaShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(450);
            Game.Items.Rename("spapi:armor-heart_friendship", "spapi:armor_heart_friendship");
        }

        protected override void DoEffect(PlayerController user)
        {
            user.PlayEffectOnActor((GameObject)ResourceCache.Acquire("Global VFX/VFX_AltGunShrine"), Vector3.zero, true, false, false);
            GameManager.Instance.StartCoroutine(this.ChangeCoroutine(user));
        }

        public IEnumerator ChangeCoroutine(PlayerController user)
        {
            yield return new WaitForSeconds(0.75f);
            user.HealthAndArmorSwapped = !user.HealthAndArmorSwapped;
            if(!user.PlayerHasActiveSynergy("#IM_HELPING!"))
            {
                if (user.characterIdentity == PlayableCharacters.Robot)
                {
                    if (user.healthHaver)
                    {
                        user.healthHaver.Armor = Mathf.Max(1, user.healthHaver.Armor - 1);
                    }
                }
                else
                {
                    user.ownerlessStatModifiers.Add(Toolbox.SetupStatModifier(PlayerStats.StatType.Health, -1f));
                }
            }
            if(user.characterIdentity != PlayableCharacters.Robot)
            {
                if (user.healthHaver)
                {
                    user.healthHaver.FullHeal();
                }
            }
            else
            {
                user.ownerlessStatModifiers.Add(Toolbox.SetupStatModifier(PlayerStats.StatType.Damage, 0.05f));
            }
            user.stats.RecalculateStats(user, true, false);
            yield break;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user);
        }
    }
}
