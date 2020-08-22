using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;

namespace SpecialItemPack
{
    class ShrinePrimalAspid : AdvancedCustomShrineController
    {
        public ShrinePrimalAspid()
        {
            this.AcceptKey = "#SHRINE_ASPID_ACCEPT";
            this.DeclineKey = "#SHRINE_ASPID_DECLINE";
            this.SpentKey = "#SHRINE_ASPID_SPENT";
            this.StoneTableKey = "#SHRINE_ASPID_TABLE";
        }

        public override void Start()
        {
            base.Start();
            this.statue = UnityEngine.Object.Instantiate(SpecialItemModule.aspidShrine.GetComponent<ShrinePrimalAspid>().statue, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter, Quaternion.identity, base.transform);
            this.statue.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(base.sprite.WorldTopCenter + new Vector2(0f, -8f / 16f), tk2dBaseSprite.Anchor.LowerCenter);
        }

        protected override bool CheckCosts(PlayerController interactor)
        {
            return true;
        }

        protected override void DoShrineEffect(PlayerController player)
        {
            this.IsActive = true;
            this.SpawnWave();
        }

        private void Update()
        {
            if(this.IsActive && !this.GetAbsoluteParentRoom().HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
            {
                this.SpawnWave();
            }
        }

        private void SpawnWave()
        {
            CurrentWave++;
            if(CurrentWave < waves.Count)
            {
                foreach (string guid in waves[CurrentWave])
                {
                    AIActor prefab = EnemyDatabase.GetOrLoadByGuid(guid);
                    IntVector2? vector = this.GetAbsoluteParentRoom().GetRandomAvailableCell(prefab.Clearance, new CellTypes?(CellTypes.FLOOR | CellTypes.PIT), false, null);
                    AIActor aiactor = AIActor.Spawn(prefab, vector != null ? vector.Value : this.GetAbsoluteParentRoom().GetBestRewardLocation(prefab.Clearance, RoomHandler.RewardLocationStyle.Original, false), this.GetAbsoluteParentRoom(), false, AIActor.AwakenAnimationType.Default,
                        true);
                    aiactor.SetIsFlying(true, "because why not", false, true);
                    aiactor.encounterTrackable.journalData.PrimaryDisplayName = "#ENEMY_PRIMALASPIDS_REVENGE";
                    this.GetAbsoluteParentRoom().SealRoom();
                }
            }
            else
            {
                SpawnManager.SpawnVFX(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData.effect, this.statue.GetComponent<tk2dBaseSprite>().WorldBottomCenter, Quaternion.identity);
                AkSoundEngine.PostEvent("Play_WPN_grenade_blast_01", base.gameObject);
                this.statue.GetComponent<tk2dBaseSprite>().renderer.enabled = false;
                Chest chest = GameManager.Instance.RewardManager.Rainbow_Chest;
                IntVector2? vector = this.GetAbsoluteParentRoom().GetRandomAvailableCell(new IntVector2?(IntVector2.One * 4), new CellTypes?(CellTypes.FLOOR | CellTypes.PIT), false, null);
                Chest.Spawn(chest, vector != null ? vector.Value : this.GetAbsoluteParentRoom().GetBestRewardLocation(IntVector2.One * 4, RoomHandler.RewardLocationStyle.Original, false), this.GetAbsoluteParentRoom(), true);
                this.IsActive = false;
                this.DeclineKey = "#SHRINE_ASPID_DECLINE_SPENT";
            }
        }

        public GameObject statue;
        public bool IsActive = false;
        public int CurrentWave = -1;
        public static List<List<string>> waves = new List<List<string>>
        {
            new List<string> 
            {
                "044a9f39712f456597b9762893fbc19c",
                "044a9f39712f456597b9762893fbc19c",
                "044a9f39712f456597b9762893fbc19c",
                "2752019b770f473193b08b4005dc781f",
                "2752019b770f473193b08b4005dc781f",
                "2752019b770f473193b08b4005dc781f",
                "2752019b770f473193b08b4005dc781f",
                "044a9f39712f456597b9762893fbc19c"
            },
            new List<string>
            {
                "ba657723b2904aa79f9e51bce7d23872",
                "c50a862d19fc4d30baeba54795e8cb93",
                "116d09c26e624bca8cca09fc69c714b3",
                "116d09c26e624bca8cca09fc69c714b3",
                "116d09c26e624bca8cca09fc69c714b3",
                "062b9b64371e46e195de17b6f10e47c8",
                "062b9b64371e46e195de17b6f10e47c8"
            },
            new List<string>
            {
                "b70cbd875fea498aa7fd14b970248920",
                "b70cbd875fea498aa7fd14b970248920",
                "ba657723b2904aa79f9e51bce7d23872",
                "ba657723b2904aa79f9e51bce7d23872",
                "c50a862d19fc4d30baeba54795e8cb93",
                "cd4a4b7f612a4ba9a720b9f97c52f38c"
            },
            new List<string>
            {
                "cd4a4b7f612a4ba9a720b9f97c52f38c",
                "cd4a4b7f612a4ba9a720b9f97c52f38c",
                "cd4a4b7f612a4ba9a720b9f97c52f38c",
                "ba657723b2904aa79f9e51bce7d23872",
                "ba657723b2904aa79f9e51bce7d23872",
                "c50a862d19fc4d30baeba54795e8cb93"
            },
            new List<string>
            {
                "cd4a4b7f612a4ba9a720b9f97c52f38c",
                "cd4a4b7f612a4ba9a720b9f97c52f38c",
                "cd4a4b7f612a4ba9a720b9f97c52f38c",
                "3f2026dc3712490289c4658a2ba4a24b",
                "3f2026dc3712490289c4658a2ba4a24b",
                "3f2026dc3712490289c4658a2ba4a24b",
                "ba657723b2904aa79f9e51bce7d23872",
                "c50a862d19fc4d30baeba54795e8cb93"
            }
        };
    }
}
