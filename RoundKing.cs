using System;
using System.Collections;
using System.Collections.Generic;
using MonoMod.RuntimeDetour;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;
using Dungeonator;
using Brave.BulletScript;

namespace SpecialItemPack
{
    class RoundKing : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Round King (SpecialItemPack)";
            string resourceName = "SpecialItemPack/Resources/RoundKingItem";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<RoundKing>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "King K. Round";
            string longDesc = "Summons the Round King to help the user in combat.\n\nThis crown powers the Round King, to be the Round King and not the Round Clown. Someone took that crown and kept it as a trophy, however when he left it on the ground, " +
                "some theif stole it. He carried it in his sack, but it turns out it had some holes in it...";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 255f);
            item.consumable = false;
            item.quality = ItemQuality.B;
            item.AddToCursulaShop();
            item.AddToBlacksmithShop();
            RoundKing.BuildPrefab();
            item.PlaceItemInAmmonomiconAfterItemById(214);
            item.SetName("Round King");
            Game.Items.Rename("spapi:round_king_(specialitempack)", "spapi:round_king");
        }

        public static void BuildPrefab()
        {
            GameObject toy = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/RoundKing/RoundKingJump2");
            toy.SetActive(false);
            FakePrefab.MarkAsFakePrefab(toy);
            UnityEngine.Object.DontDestroyOnLoad(toy);
            GameObject rKing = new GameObject("KingKRound");
            tk2dSprite sprite = rKing.AddComponent<tk2dSprite>();
            sprite.SetSprite(toy.GetComponent<tk2dBaseSprite>().Collection, toy.GetComponent<tk2dBaseSprite>().spriteId);
            spriteIds.Add(SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/RoundKing/RoundKingJump1", sprite.Collection));
            sprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
            spriteIds.Add(sprite.spriteId);
            rKing.SetActive(false);
            sprite.SetSprite(spriteIds[0]);
            sprite.GetCurrentSpriteDef().material.shader = ShaderCache.Acquire("Brave/PlayerShader");
            sprite.SetSprite(spriteIds[1]);
            FakePrefab.MarkAsFakePrefab(rKing);
            UnityEngine.Object.DontDestroyOnLoad(rKing);
            roundKingPrefab = rKing;
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            this.m_extantRoundKing = UnityEngine.Object.Instantiate(roundKingPrefab, user.CenterPosition, Quaternion.identity);
            if(user.CurrentRoom != null)
            {
                AIActor randomenemy = user.CurrentRoom.GetRandomActiveEnemy(true);
                this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(randomenemy.sprite.WorldBottomCenter + new Vector2(0, 50), tk2dBaseSprite.Anchor.LowerCenter);
                base.StartCoroutine(this.HandleAttack(randomenemy, user.CurrentRoom));
            }
        }

        public IEnumerator HandleAttack(AIActor enemyToStartWith ,RoomHandler room)
        {
            this.m_isCurrentlyActive = true;
            AIActor currentEnemy = enemyToStartWith;
            RoundKingState currentState = RoundKingState.Falling;
            Vector2 targetPoint = currentEnemy.sprite.WorldBottomCenter;
            Vector2 velocity = default(Vector2);
            float cooldownTime = 0f;
            float chaseTime = 0f;
            Vector2 lastTargetPoint = Vector2.zero;
            while(true)
            {
                if (!this.m_pickedUp || this.LastOwner == null)
                {
                    break;
                }
                if(currentState == RoundKingState.Falling)
                {
                    if(this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().WorldBottomCenter.y > targetPoint.y)
                    {
                        this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().WorldBottomCenter + new Vector2(0, -(30f * BraveTime.DeltaTime)), tk2dBaseSprite.Anchor.LowerCenter);
                    }
                    else
                    {
                        Exploder.DoDefaultExplosion(this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().WorldBottomCenter, new Vector2());
                        if(this.LastOwner != null && this.LastOwner.PlayerHasActiveSynergy("#TRUE_KING"))
                        {
                            for(int i=0; i<6; i++)
                            {
                                GameObject obj = SpawnManager.SpawnProjectile(Toolbox.GetGunById(52).DefaultModule.chargeProjectiles[0].Projectile.gameObject, this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().WorldBottomCenter,
                                    Quaternion.Euler(0, 0, i * 60));
                                Projectile proj = obj.GetComponent<Projectile>();
                                if (proj != null)
                                {
                                    proj.Owner = this.LastOwner;
                                    proj.Shooter = this.LastOwner.specRigidbody;
                                    proj.DefaultTintColor = Color.white;
                                    proj.HasDefaultTint = true;
                                    if(proj.GetComponent<SpawnProjModifier>() != null)
                                    {
                                        Destroy(proj.GetComponent<SpawnProjModifier>());
                                    }
                                    PierceProjModifier mod = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
                                    mod.penetration = 999;
                                    mod.preventPenetrationOfActors = true;
                                    mod.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
                                }
                            }
                        }
                        currentState = RoundKingState.CoolingDown;
                        cooldownTime = 1f;
                    }
                }
                else if(currentState == RoundKingState.CoolingDown)
                {
                    if(cooldownTime > 0)
                    {
                        cooldownTime -= BraveTime.DeltaTime;
                    }
                    else
                    {
                        if (room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
                        {
                            currentState = RoundKingState.Chasing;
                            this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().SetSprite(spriteIds[0]);
                            chaseTime = 2f;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if(currentState == RoundKingState.Chasing)
                {
                    if (room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
                    {
                        if (currentEnemy == null || currentEnemy.sprite == null || (currentEnemy.healthHaver != null && currentEnemy.healthHaver.IsDead))
                        {
                            currentEnemy = room.GetRandomActiveEnemy(true);
                        }
                        Vector2 a = (currentEnemy.sprite.WorldTopCenter + new Vector2(0, 1) - this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().WorldBottomCenter);
                        a.Normalize();
                        bool flag5 = Vector2.Distance((currentEnemy.sprite.WorldTopCenter + new Vector2(0, 1)), base.sprite.WorldBottomCenter) < 0.2f;
                        if (flag5)
                        {
                            velocity = Vector2.Lerp(velocity, Vector2.zero, 0.5f);
                        }
                        else
                        {
                            velocity = Vector2.Lerp(velocity, a * 10, 0.1f);
                        }
                        this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().WorldBottomCenter + (BraveTime.DeltaTime * velocity), tk2dBaseSprite.Anchor.LowerCenter);
                        lastTargetPoint = (this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().WorldBottomCenter + new Vector2(0, -1f - currentEnemy.sprite.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.UpperCenter).y));
                        if (chaseTime > 0)
                        {
                            chaseTime -= BraveTime.DeltaTime;
                        }
                        else
                        {
                            targetPoint = (this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().WorldBottomCenter + new Vector2(0, -1f - currentEnemy.sprite.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.UpperCenter).y));
                            this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().SetSprite(spriteIds[1]);
                            currentState = RoundKingState.Falling;
                        }
                    }
                    else
                    {
                        targetPoint = lastTargetPoint;
                        this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().SetSprite(spriteIds[1]);
                        currentState = RoundKingState.Falling;
                    }
                }
                yield return null;
            }
            this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().SetSprite(spriteIds[0]);
            float ela = 0f;
            while(ela < 2f)
            {
                this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(this.m_extantRoundKing.GetComponent<tk2dBaseSprite>().WorldBottomCenter + new Vector2(0, (30f * BraveTime.DeltaTime)), tk2dBaseSprite.Anchor.LowerCenter);
                ela += BraveTime.DeltaTime;
                yield return null;
            }
            Destroy(this.m_extantRoundKing);
            this.m_isCurrentlyActive = false;
            yield break;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return user != null && user.CurrentRoom != null && user.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All) && base.CanBeUsed(user);
        }

        private GameObject m_extantRoundKing;
        public static GameObject roundKingPrefab;
        public static List<int> spriteIds = new List<int>();
        public enum RoundKingState
        {
            Falling,
            Chasing,
            CoolingDown
        }
    }
}
