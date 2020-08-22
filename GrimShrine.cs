using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class GrimShrine : AdvancedCustomShrineController
    {
        public GrimShrine()
        {
            this.AcceptKey = "#SHRINE_GRIM_ACCEPT";
            this.DeclineKey = "#SHRINE_GRIM_DECLINE";
            this.SpentKey = "#SHRINE_SREAPER_SPENT";
            this.StoneTableKey = "#SHRINE_GRIM_TABLE";
        }

        public override void Start()
        {
            base.Start();
            this.stoneGrim = UnityEngine.Object.Instantiate(SpecialItemModule.grimShrine.GetComponent<GrimShrine>().stoneGrim, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter, Quaternion.identity, base.transform);
            this.stoneGrim.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(base.sprite.WorldTopCenter + new Vector2(-10f / 16, -8f / 16), tk2dBaseSprite.Anchor.LowerCenter);
        }

        protected override bool CheckCosts(PlayerController interactor)
        {
            return interactor.healthHaver != null && interactor.healthHaver.GetMaxHealth() > 2f;
        }

        protected override void DoShrineEffect(PlayerController player)
        {
            player.ownerlessStatModifiers.Add(Toolbox.SetupStatModifier(PlayerStats.StatType.Health, -2f, StatModifier.ModifyMethod.ADDITIVE, false));
            player.ownerlessStatModifiers.Add(Toolbox.SetupStatModifier(PlayerStats.StatType.Curse, 2f, StatModifier.ModifyMethod.ADDITIVE, false));
            player.stats.RecalculateStats(player, true, false);
            GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetString("#SHRINE_GRIM_HEADER"), StringTableManager.GetString("#SHRINE_GRIM_TEXT"), SpriteBuilder.itemCollection,
                spriteId, UINotificationController.NotificationColor.SILVER, false, false);
            player.gameObject.AddComponent<FriendsWithGrimBehaviour>();
            AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", base.gameObject);
        }

        public GameObject stoneGrim;
        public static int spriteId;
    }

    class FriendsWithGrimBehaviour : BraveBehaviour
    {
        public void Start()
        {
            if(base.gameActor is PlayerController)
            {
                this.m_player = base.gameActor as PlayerController;
                this.m_player.OnEnteredCombat += this.FriendsWithGrim;
            }
        }

        public void FriendsWithGrim()
        {
            if (this.m_player != null)
            {
                AIActor grim = EnemyDatabase.GetOrLoadByGuid("88f037c3f93b4362a040a87b30770407");
                IntVector2? vector = this.m_player.CurrentRoom.GetRandomAvailableCell(grim.Clearance, grim.PathableTiles, false, null);
                AIActor.Spawn(grim, vector != null ? vector.Value : this.m_player.CurrentRoom.GetBestRewardLocation(grim.Clearance, RoomHandler.RewardLocationStyle.Original, false), this.m_player.CurrentRoom, true, AIActor.AwakenAnimationType.Default, 
                    true).ApplyEffect(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultPermanentCharmEffect, 1, null);
            }
        }

        private PlayerController m_player;
    }
}
