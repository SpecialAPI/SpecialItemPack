using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;
using Dungeonator;

namespace SpecialItemPack
{
    class EboniteAmmolet : SpecialBlankModificationItem
    {
        public static void Init()
        {
            string itemName = "Ebonstone Ammolet";
            string resourceName = "SpecialItemPack/Resources/EbonstoneAmmolet";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<EboniteAmmolet>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Blanks Curse";
            string longDesc = "Used blanks will ignite your enemies with a cursed flame. Sadly, cursed flames don't work on jammed enemies.\n\nAmmolet made out of weird stone, that provides a blazing blight aura. Using blanks seems to affect it " +
                " and force it to expand it's aura.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = ItemQuality.A;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.AddToOldRedShop();
            item.PlaceItemInAmmonomiconAfterItemById(344);
            item.AddItemToSynergy(CustomSynergyType.RELODESTAR);
            item.BlankStunTime = 0;
        }

        protected override void OnBlank(Vector2 centerPoint, PlayerController user)
        {
            List<AIActor> activeEnemies = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(centerPoint.ToIntVector2(VectorConversions.Round)).GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (activeEnemies != null)
            {
                for (int j = 0; j < activeEnemies.Count; j++)
                {
                    AIActor aiactor = activeEnemies[j];
                    if (aiactor != null && !aiactor.IsBlackPhantom)
                    {
                        aiactor.ApplyEffect((PickupObjectDatabase.GetById(722) as Gun).DefaultModule.projectiles[0].fireEffect);
                    }
                }
            }
            if(user != null && user.PlayerHasActiveSynergy("#FORBIDDEN_AMMOLET_OF_RESISTANCE"))
            {
                if (user.healthHaver != null)
                {
                    user.healthHaver.TriggerInvulnerabilityPeriod(3);
                }
            }
        }
    }
}
