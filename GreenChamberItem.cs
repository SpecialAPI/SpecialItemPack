using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;
using System.Reflection;

namespace SpecialItemPack
{
    class GreenChamberItem : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Green Chamber";
            string resourceName = "SpecialItemPack/Resources/GreenChamberItem/GreenChamber";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<GreenChamberItem>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "\"the world burning\"";
            string longDesc = "Etched into its chambers is a single worn glyph.\n\nBurn everything.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 300f);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 2f);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RateOfFire, 0.15f);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, 2f);
            item.consumable = false;
            item.quality = ItemQuality.S;
            item.PlaceItemInAmmonomiconAfterItemById(209);
            List<int> spriteIds = new List<int>
            {
                item.sprite.spriteId,
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_001", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_002", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_003", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_004", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_005", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_006", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_007", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_008", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_009", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_010", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_011", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_012", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_013", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_014", item.sprite.Collection),
                SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GreenChamberItem/GreenChamber_melt_015", item.sprite.Collection),
            };
            tk2dSpriteAnimator animator = obj.AddComponent<tk2dSpriteAnimator>();
            animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animator.Library.clips = new tk2dSpriteAnimationClip[0];
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip { fps = 15, frames = new tk2dSpriteAnimationFrame[0], name = "melt", wrapMode = tk2dSpriteAnimationClip.WrapMode.Once };
            for (int i = 0; i < spriteIds.Count; i++)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = spriteIds[i], spriteCollection = item.sprite.Collection };
                clip.frames = clip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library.clips = animator.Library.clips.Concat(new tk2dSpriteAnimationClip[] { clip }).ToArray();
            tk2dSpriteAnimationClip clip1 = new tk2dSpriteAnimationClip { fps = 15, frames = new tk2dSpriteAnimationFrame[0], name = "unmelt", wrapMode = tk2dSpriteAnimationClip.WrapMode.Once };
            for (int i = spriteIds.Count - 1; i > -1; i--)
            {
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame { spriteId = spriteIds[i], spriteCollection = item.sprite.Collection };
                clip1.frames = clip1.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            animator.Library.clips = animator.Library.clips.Concat(new tk2dSpriteAnimationClip[] { clip1 }).ToArray();
            item.SetupUnlockOnCustomFlag(CustomDungeonFlags.ITEMSPECIFIC_GREEN_CHAMBER, true);
            SpecialItemIds.GreenChamber = item.PickupObjectId;
        }

        protected override void OnPreDrop(PlayerController user)
        {
            if (this.m_isCurrentlyActive)
            {
                this.EndEffect(user);
            }
            user.OnEnteredCombat += this.ProcessSynergies;
            base.OnPreDrop(user);
        }

        public void ProcessSynergies()
        {
            if (this.LastOwner.PlayerHasActiveSynergy("#\"THE_SYNERGY\""))
            {
                if(UnityEngine.Random.value <= 0.95f)
                {
                    this.LastOwner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref this.m_enemyList);
                    for (int i = 0; i < this.m_enemyList.Count; i++)
                    {
                        AIActor aiactor = this.m_enemyList[i];
                        if (!aiactor || !aiactor.IsNormalEnemy || !aiactor.healthHaver || aiactor.healthHaver.IsBoss || aiactor.IsHarmlessEnemy)
                        {
                            this.m_enemyList.RemoveAt(i);
                            i--;
                        }
                    }
                    if (this.m_enemyList.Count > 1)
                    {
                        AIActor aiactor2 = this.m_enemyList[UnityEngine.Random.Range(0, this.m_enemyList.Count)];
                        aiactor2.ApplyEffect(Toolbox.GetGunById(722).DefaultModule.projectiles[0].fireEffect, 1f, null);
                    }
                }
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnEnteredCombat += this.ProcessSynergies;
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            base.StartCoroutine(ItemBuilder.HandleDuration(this, 15f, user, this.EndEffect));
            AkSoundEngine.PostEvent("Play_ENM_darken_world_01", user.gameObject);
            GameManager.Instance.StartCoroutine(this.HandleColorLerp());
            this.spriteAnimator.Play("melt");
            this.LastOwner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All, ref this.m_enemyList);
            for (int i = 0; i < this.m_enemyList.Count; i++)
            {
                AIActor aiactor = this.m_enemyList[i];
                if (!aiactor || !aiactor.IsNormalEnemy || !aiactor.healthHaver || aiactor.healthHaver.IsBoss || aiactor.IsHarmlessEnemy)
                {
                    this.m_enemyList.RemoveAt(i);
                    i--;
                }
            }
            if (this.m_enemyList.Count > 1)
            {
                this.EatCharmedEnemy();
                AIActor aiactor2 = this.m_enemyList[UnityEngine.Random.Range(0, this.m_enemyList.Count)];
                aiactor2.IgnoreForRoomClear = true;
                aiactor2.ParentRoom.ResetEnemyHPPercentage();
                aiactor2.ApplyEffect(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultPermanentCharmEffect, 1f, null);
                this.m_currentlyCharmedEnemy = aiactor2;
            }
        }

        private void EatCharmedEnemy()
        {
            if (!this.m_currentlyCharmedEnemy)
            {
                return;
            }
            if (this.m_currentlyCharmedEnemy.behaviorSpeculator)
            {
                this.m_currentlyCharmedEnemy.behaviorSpeculator.Stun(1f, true);
            }
            if (this.m_currentlyCharmedEnemy.knockbackDoer)
            {
                this.m_currentlyCharmedEnemy.knockbackDoer.SetImmobile(true, "GreenChamberItem");
            }
            GameObject gameObject = this.m_currentlyCharmedEnemy.PlayEffectOnActor(KaliberAffliction.EraseVFX, new Vector3(0f, -1f, 0f), false, false, false);
            this.m_currentlyCharmedEnemy.StartCoroutine(this.DelayedDestroyEnemy(this.m_currentlyCharmedEnemy, gameObject.GetComponent<tk2dSpriteAnimator>()));
            this.m_currentlyCharmedEnemy = null;
        }

        private IEnumerator DelayedDestroyEnemy(AIActor enemy, tk2dSpriteAnimator vfxAnimator)
        {
            if (vfxAnimator)
            {
                vfxAnimator.sprite.IsPerpendicular = false;
                vfxAnimator.sprite.HeightOffGround = -1f;
            }
            while (enemy && vfxAnimator && vfxAnimator.sprite.GetCurrentSpriteDef().name != "kthuliber_tentacles_010")
            {
                vfxAnimator.sprite.UpdateZDepth();
                yield return null;
            }
            if (vfxAnimator)
            {
                vfxAnimator.sprite.IsPerpendicular = true;
                vfxAnimator.sprite.HeightOffGround = 1.5f;
                vfxAnimator.sprite.UpdateZDepth();
            }
            if (enemy)
            {
                enemy.EraseFromExistence(false);
            }
            yield break;
        }

        private IEnumerator HandleColorLerp()
        {
            float ela = 0f;
            while(ela < 1f)
            {
                ela += BraveTime.DeltaTime;
                RenderSettings.ambientLight = Color.Lerp(Color.white, Color.green, ela);
                if(GameManager.Instance.PrimaryPlayer != null)
                {
                    GameManager.Instance.PrimaryPlayer.baseFlatColorOverride = Color.green.WithAlpha(ela);
                }
                if(GameManager.Instance.SecondaryPlayer != null)
                {
                    GameManager.Instance.SecondaryPlayer.baseFlatColorOverride = Color.green.WithAlpha(ela);
                }
                yield return null;
            }
            while (this != null && this.m_isCurrentlyActive)
            {
                RenderSettings.ambientLight = Color.green;
                if (GameManager.Instance.PrimaryPlayer != null)
                {
                    GameManager.Instance.PrimaryPlayer.baseFlatColorOverride = Color.green;
                }
                if (GameManager.Instance.SecondaryPlayer != null)
                {
                    GameManager.Instance.SecondaryPlayer.baseFlatColorOverride = Color.green;
                }
                yield return null;
            }
            ela = 1f;
            while (ela > 0f)
            {
                ela -= BraveTime.DeltaTime;
                RenderSettings.ambientLight = Color.Lerp(Color.white, Color.green, ela);
                if (GameManager.Instance.PrimaryPlayer != null)
                {
                    GameManager.Instance.PrimaryPlayer.baseFlatColorOverride = Color.green.WithAlpha(ela);
                }
                if (GameManager.Instance.SecondaryPlayer != null)
                {
                    GameManager.Instance.SecondaryPlayer.baseFlatColorOverride = Color.green.WithAlpha(ela);
                }
                yield return null;
            }
            yield break;
        }

        public override void Update()
        {
            base.Update();
            if(this.m_pickedUp && this.LastOwner != null)
            {
                if (this.m_currentlyCharmedEnemy && (this.LastOwner.CurrentRoom == null || this.LastOwner.CurrentRoom.GetActiveEnemiesCount(RoomHandler.ActiveEnemyType.RoomClear) <= 0))
                {
                    this.EatCharmedEnemy();
                }
                if (this.m_isCurrentlyActive)
                {
                    if(this.LastOwner.CurrentRoom != null && this.LastOwner.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
                    {
                        foreach(AIActor aiactor in this.LastOwner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                        {
                            if(aiactor == null || (aiactor.healthHaver != null && aiactor.healthHaver.IsDead || (aiactor.GetEffect("charm") != null && aiactor.GetEffect("charm").duration >= 999f))) { continue; }
                            Vector2 vector;
                            if(aiactor.sprite != null)
                            {
                                vector = aiactor.sprite.WorldBottomCenter;
                            }
                            else if(aiactor.specRigidbody != null)
                            {
                                vector = aiactor.specRigidbody.UnitBottomCenter;
                            }
                            else
                            {
                                vector = aiactor.transform.position;
                            }
                            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Toolbox.DefaultGreenFireGoop).AddGoopCircle(vector, 1f);
                        }
                    }
                    if (GameManager.Instance.PrimaryPlayer != null)
                    {
                        Vector2 vector2;
                        if (this.LastOwner.sprite != null)
                        {
                            vector2 = this.LastOwner.sprite.WorldBottomCenter;
                        }
                        else if (this.LastOwner.specRigidbody != null)
                        {
                            vector2 = this.LastOwner.specRigidbody.UnitBottomCenter;
                        }
                        else
                        {
                            vector2 = this.LastOwner.transform.position;
                        }
                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Toolbox.DefaultGreenFireGoop).AddGoopCircle(vector2, 1f);
                    }
                    if (GameManager.Instance.SecondaryPlayer != null)
                    {
                        Vector2 vector2;
                        if (this.LastOwner.sprite != null)
                        {
                            vector2 = this.LastOwner.sprite.WorldBottomCenter;
                        }
                        else if (this.LastOwner.specRigidbody != null)
                        {
                            vector2 = this.LastOwner.specRigidbody.UnitBottomCenter;
                        }
                        else
                        {
                            vector2 = this.LastOwner.transform.position;
                        }
                        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Toolbox.DefaultGreenFireGoop).AddGoopCircle(vector2, 1f);
                    }
                }
            }
        }

        private void EndEffect(PlayerController player)
        {
            if (UnityEngine.Random.value < this.m_exhaustionChance)
            {
                player.StartCoroutine(this.EndEffectCR(player));
            }
            if (UnityEngine.Random.value < this.m_exhaustionChance)
            {
                List<RoomHandler> rooms = new List<RoomHandler>();
                foreach (RoomHandler room in GameManager.Instance.Dungeon.data.rooms)
                {
                    if (!room.hasEverBeenVisited)
                    {
                        rooms.Add(room);
                    }
                }
                if (rooms.Count > 0)
                {
                    RoomHandler randomRoom = BraveUtility.RandomElement(rooms);
                    IntVector2? randomAvailableCell = randomRoom.GetRandomAvailableCell(new IntVector2?(GreenChamberEyeController.hallucinationEyePrefab.GetComponent<GreenChamberEyeController>().specRigidbody.UnitDimensions.ToIntVector2(VectorConversions.Ceil)),
                        new CellTypes?(CellTypes.FLOOR | CellTypes.PIT), false, null);
                    if (randomAvailableCell.HasValue)
                    {
                        Instantiate(GreenChamberEyeController.hallucinationEyePrefab, randomAvailableCell.Value.ToVector3(0), Quaternion.identity).GetComponent<GreenChamberEyeController>().BindWithRoom(randomRoom);
                    }
                }
            }
            this.m_exhaustionChance = Mathf.Min(this.m_exhaustionChance += 0.01f, 0.05f);
            AkSoundEngine.PostEvent("Play_ENM_lighten_world_01", player.gameObject);
            this.spriteAnimator.Play("unmelt");
        }

        private IEnumerator EndEffectCR(PlayerController player)
        {
            if (player.IsDodgeRolling)
            {
                player.ForceStopDodgeRoll();
            }
            player.SetInputOverride("green chamber");
            FieldInfo info = typeof(PlayerController).GetField("m_handlingQueuedAnimation", BindingFlags.NonPublic | BindingFlags.Instance);
            info.SetValue(player, true);
            player.ToggleGunRenderers(false, "green chamber");
            player.ToggleHandRenderers(false, "green chamber");
            string anim = (!player.UseArmorlessAnim) ? "death_shot" : "death_shot_armorless";
            player.spriteAnimator.Play(anim);
            while (player.spriteAnimator.IsPlaying(anim))
            {
                player.healthHaver.TriggerInvulnerabilityPeriod(0.1f);
                yield return null;
            }
            info.SetValue(player, true);
            tk2dSpriteAnimationClip clip = player.spriteAnimator.GetClipByName(anim);
            player.spriteAnimator.Stop();
            player.sprite.SetSprite(clip.frames[clip.frames.Length - 1].spriteCollection, clip.frames[clip.frames.Length - 1].spriteId);
            player.healthHaver.TriggerInvulnerabilityPeriod(0.25f);
            float ela = 0f;
            while(ela <= 0.25f)
            {
                ela += BraveTime.DeltaTime;
                yield return null;
            }
            player.ClearInputOverride("green chamber");
            player.CurrentInputState = PlayerInputState.OnlyMovement;
            while (player.specRigidbody.Velocity.magnitude < 0.05f)
            {
                yield return null;
            }
            player.CurrentInputState = PlayerInputState.AllInput;
            info.SetValue(player, false);
            player.ToggleGunRenderers(true, "green chamber");
            player.ToggleHandRenderers(true, "green chamber");
            yield break;
        }

        private float m_exhaustionChance = 0f;
        private List<AIActor> m_enemyList = new List<AIActor>();
        private AIActor m_currentlyCharmedEnemy;
        private List<AIActor> excludedEnemies;
    }
}
