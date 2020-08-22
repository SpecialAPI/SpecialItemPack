using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class SreaperShrine : AdvancedCustomShrineController
    {
        public SreaperShrine()
        {
            this.AcceptKey = "#SHRINE_SREAPER_ACCEPT";
            this.DeclineKey = "#SHRINE_SREAPER_DECLINE";
            this.SpentKey = "#SHRINE_SREAPER_SPENT";
            this.StoneTableKey = "#SHRINE_SREAPER_TABLE";
        }

        public override void Start()
        {
            base.Start();
            this.stoneSreaper = UnityEngine.Object.Instantiate(SpecialItemModule.sreaperShrine.GetComponent<SreaperShrine>().stoneSreaper, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter, Quaternion.identity, base.transform);
            this.stoneSreaper.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(base.sprite.WorldTopCenter + new Vector2(-10f / 16, -8f / 16), tk2dBaseSprite.Anchor.LowerCenter);
        }

        protected override void DoShrineEffect(PlayerController player)
        {
            base.DoShrineEffect(player);
            IntVector2? randomAvailableCell = base.transform.position.GetAbsoluteRoom().GetRandomAvailableCell(new IntVector2?(IntVector2.One * 4), new CellTypes?(CellTypes.FLOOR), false, null);
            IntVector2? intVector = (randomAvailableCell == null) ? null : new IntVector2?(randomAvailableCell.GetValueOrDefault() + IntVector2.One);
            if (intVector != null)
            {
                Chest chest = Chest.Spawn(GameManager.Instance.RewardManager.S_Chest, intVector.Value);
                chest.lootTable.lootTable = (UnityEngine.Random.value <= 0.5f ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable);
                chest.ForceUnlock();
            }
            else
            {
                Chest chest = Chest.Spawn(GameManager.Instance.RewardManager.S_Chest, base.transform.position.GetAbsoluteRoom().GetBestRewardLocation(new IntVector2(3, 3), RoomHandler.RewardLocationStyle.Original, true) + IntVector2.Up);
                chest.lootTable.lootTable = (UnityEngine.Random.value <= 0.5f ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable);
                chest.ForceUnlock();
            }
            SpawnManager.SpawnVFX(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData.effect, base.sprite.WorldTopCenter + new Vector2(0f, -8f / 16), Quaternion.identity);
            AkSoundEngine.PostEvent("Play_WPN_grenade_blast_01", base.gameObject);
            if (this.stoneSreaper != null)
            {
                this.stoneSreaper.GetComponent<tk2dBaseSprite>().renderer.enabled = false;
            }
            GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetString("#SHRINE_SREAPER_HEADER"), StringTableManager.GetString("#SHRINE_SREAPER_TEXT"), SpriteBuilder.itemCollection, 
                spriteId, UINotificationController.NotificationColor.SILVER, false, false);
            player.gameObject.AddComponent<CursedPlayerBehaviour>();
            if (GameManager.HasInstance && GameManager.Instance.Dungeon != null && !GameManager.Instance.Dungeon.CurseReaperActive)
            {
                GameManager.Instance.Dungeon.CurseReaperActive = true;
                GameObject superReaper = PrefabDatabase.Instance.SuperReaper;
                Vector2 vector = base.sprite.WorldTopCenter - new Vector2(5, 3);
                Instantiate(superReaper, vector.ToVector3ZUp(0f), Quaternion.identity);
            }
        }

        public GameObject stoneSreaper;
        public static int spriteId;
    }

    public class CursedPlayerBehaviour : BraveBehaviour
    {
        private void Update()
        {
            if(GameManager.Instance != null && GameManager.Instance.Dungeon != null)
            {
                if (!GameManager.Instance.Dungeon.CurseReaperActive)
                {
                    GameManager.Instance.Dungeon.SpawnCurseReaper();
                }
            }
        }
    }
}
