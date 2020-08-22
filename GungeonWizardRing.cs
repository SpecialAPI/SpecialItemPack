using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;

namespace SpecialItemPack
{
    class GungeonWizardRing : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Gunjurer Ring";
            string resourceName = "SpecialItemPack/Resources/Purple_ring";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<GungeonWizardRing>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Full of Magic";
            string longDesc = "Transmogrifies enemies upon use of active item and saves consumable items.\n\nA ring that Apprentice Gunjurers wear while studiying magic. That ring boosts magic abilities and " +
                "may draw it's magic even from normal active items!\n\nDespite being so powerful, this ring is only worn by Apprentice Gunjurers while studying. Gunjurers who wear that ring actually in combat are forever banished from the Ammomancers Cult." +
                " Gunjurers have their pride, after all.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.S;
            item.AddToCursulaShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(158);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnUsedPlayerItem += this.OnActiveItemUsed;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnUsedPlayerItem -= this.OnActiveItemUsed;
            return base.Drop(player);
        }



        public void OnActiveItemUsed(PlayerController user, PlayerItem item)
        {
            if (this.m_owner.IsInCombat)
            {
                RoomHandler currentroom = user.CurrentRoom;
                AIActor randomenemy = currentroom.GetRandomActiveEnemy(false);
                if(randomenemy != null)
                {
                    randomenemy.Transmogrify(EnemyDatabase.GetOrLoadByGuid("76bc43539fc24648bff4568c75c686d1"), (GameObject)ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"));
                }
                if(user.PlayerHasActiveSynergy("#GUNGEON_WIZARDRY_SCHOOL"))
                {
                    RoomHandler currentroom2 = user.CurrentRoom;
                    AIActor randomenemy2 = currentroom.GetRandomActiveEnemy(true);
                    if (randomenemy2 != null)
                    {
                        randomenemy2.ApplyEffect((PickupObjectDatabase.GetById(295) as BulletStatusEffectItem).FireModifierEffect);
                    }
                }
            }
            if (item.consumable)
            {
                GameManager.Instance.StartCoroutine(this.ItemUseCoroutine(user, item));
            }
        }

        public IEnumerator ItemUseCoroutine(PlayerController user, PlayerItem item)
        {
            yield return new WaitForSeconds(0.1f);
            LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(item.PickupObjectId).gameObject, user);
            foreach (PlayerItem item2 in user.activeItems)
            {
                if (item2.PickupObjectId == item.PickupObjectId)
                {
                    item2.timeCooldown = -1f;
                    item2.roomCooldown = -1;
                    item2.damageCooldown = 1700f;
                    item2.ForceApplyCooldown(user);
                }
            }
            yield break;
        }
    }
}
