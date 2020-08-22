using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class CrownOfTheJammed : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Crown of the Jammed";
            string resourceName = "SpecialItemPack/Resources/CrownOfTheJammed";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<CrownOfTheJammed>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Brings the Jammed to His Knees";
            string longDesc = "Allows the bearer control over the Sentiel beyond the Curtain.\n\nThis crown was created the same way the Jammed was, and was enchanted with a powerful spell, that can give the bearer control over the victim of it." +
                "\n\nThe Jammed tried to keep the crown with him, but failed...";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.S;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(214);
            for (int i = 0; i < EncounterDatabase.Instance.Entries.Count; i++)
            {
                if (EncounterDatabase.Instance.Entries[i].journalData.PrimaryDisplayName == "#SREAPER_ENCNAME")
                {
                    item.SetupUnlockOnEncounter(EncounterDatabase.Instance.Entries[i].myGuid, DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN, 0);
                    break;
                }
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            GameObject obj = Instantiate(CustomSuperReaperController.friendlyReaperPrefab, player.CenterPosition, Quaternion.identity);
            obj.GetComponent<CustomSuperReaperController>().BindWithOwner(player);
            this.m_extantReaper = obj;
            player.OnNewFloorLoaded += this.RegenerateCompanion;
        }

        private void RegenerateCompanion(PlayerController player)
        {
            if (this.m_extantReaper != null)
            {
                ETGModConsole.Log("Destroying");
                Destroy(this.m_extantReaper.gameObject);
                this.m_extantReaper = null;
            }
            GameObject obj = Instantiate(CustomSuperReaperController.friendlyReaperPrefab, player.CenterPosition, Quaternion.identity);
            obj.GetComponent<CustomSuperReaperController>().BindWithOwner(player);
            this.m_extantReaper = obj;
        }

        protected override void Update()
        {
            base.Update();
            if(this.m_pickedUp && this.m_owner != null)
            {
                if(GameManager.Instance != null && GameManager.Instance.Dungeon != null && !GameManager.Instance.Dungeon.CurseReaperActive)
                {
                    GameManager.Instance.Dungeon.CurseReaperActive = true;
                }
                if(SuperReaperController.Instance != null && SuperReaperController.Instance.gameObject != null)
                {
                    Destroy(SuperReaperController.Instance.gameObject);
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            if(this.m_extantReaper != null)
            {
                Destroy(this.m_extantReaper.gameObject);
                this.m_extantReaper = null;
            }
            GameManager.Instance.Dungeon.CurseReaperActive = false;
            player.OnNewFloorLoaded -= this.RegenerateCompanion;
            return base.Drop(player);
        }

        private GameObject m_extantReaper;
    }
}