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
    class CrimtaneAmmolet : SpecialBlankModificationItem
    {
        public static void Init()
        {
            string itemName = "Crimstone Ammolet";
            string resourceName = "SpecialItemPack/Resources/CrimstoneAmmolet";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<CrimtaneAmmolet>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Blanks Weaken";
            string longDesc = "Used blanks will weaken your enemies. Sadly, the weakening don't work on jammed enemies.\n\nAmmolet made out of weird stone, that provides a strong blight aura. Using blanks seems to affect it " +
                " and force it to expand it's aura.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalBlanksPerFloor, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = ItemQuality.B;
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
                    AIActorDebuffEffect debuffEffect = null;
                    foreach (AttackBehaviorBase attackBehaviour in EnemyDatabase.GetOrLoadByGuid((PickupObjectDatabase.GetById(492) as CompanionItem).CompanionGuid).behaviorSpeculator.AttackBehaviors)
                    {
                        if (attackBehaviour is WolfCompanionAttackBehavior)
                        {
                            debuffEffect = (attackBehaviour as WolfCompanionAttackBehavior).EnemyDebuff;
                        }
                    }
                    if (debuffEffect != null && !aiactor.IsBlackPhantom)
                    {
                        aiactor.ApplyEffect(debuffEffect, 1, null);
                    }
                    if (user != null && user.PlayerHasActiveSynergy("#FORBIDDEN_AMMOLET_OF_CONFUSION"))
                    {
                        if (aiactor.behaviorSpeculator != null)
                        {
                            aiactor.behaviorSpeculator.Stun(3, true);
                        }
                    }
                }
            }
        }
    }
}
