using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class PokeballItem : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Pokebullet";
            string resourceName = "SpecialItemPack/Resources/PokeBullet";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<PokeballItem>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "I chose you!";
            string longDesc = "Shoots a bullet, and if it hits the enemy, it will became a companion. Returns if misses. Sadly, they will run away in elevators, because of the fear.\n\nIf it wasn't Gungeon, the \"bullet inside a bullet\" thing would be impossible, but you " +
                "know it's gungeon - here even impossible is possible!";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None, int.MaxValue);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.consumable = false;
            item.quality = ItemQuality.A;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(300);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            GameManager.Instance.OnNewLevelFullyLoaded += this.RechargePerFloor;
        }

        private void RechargePerFloor()
        {
            this.thrown = false;
        }

        protected override void OnPreDrop(PlayerController user)
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= this.RechargePerFloor;
            base.OnPreDrop(user);
        }

        protected override void DoEffect(PlayerController user)
        {
            this.thrown = true;
            BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.LastOwner.PlayerIDX);
            bool flag2 = instanceForPlayer == null;
            float z = 0;
            if (!flag2)
            {
                bool flag3 = instanceForPlayer.IsKeyboardAndMouse(false);
                Vector2 a;
                if (flag3)
                {
                    a = this.LastOwner.unadjustedAimPoint.XY() - base.sprite.WorldCenter;
                }
                else
                {
                    bool flag4 = instanceForPlayer.ActiveActions == null;
                    if (flag4)
                    {
                        return;
                    }
                    a = instanceForPlayer.ActiveActions.Aim.Vector;
                }
                a.Normalize();
                z = BraveMathCollege.Atan2Degrees(a);
            }
            GameObject obj = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(38) as Gun).DefaultModule.projectiles[0].gameObject, user.sprite.WorldCenter, Quaternion.Euler(0, 0, z));
            Projectile proj = obj.GetComponent<Projectile>();
            if(proj != null)
            {
                proj.Owner = user;
                proj.Shooter = user.specRigidbody;
                proj.baseData.damage = 0;
                proj.baseData.force = 0;
                CatchProjectileBehaviour pokemonBehaviour = proj.gameObject.AddComponent<CatchProjectileBehaviour>();
                pokemonBehaviour.thrower = user;
                pokemonBehaviour.parentItem = this;
                pokemonBehaviour.hasEvolutionSynergy = user.PlayerHasActiveSynergy("#GEN_2");
            }
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && !this.thrown;
        }

        public bool thrown = false;

        public class CatchProjectileBehaviour : MonoBehaviour
        {
            private void Start()
            {
                this.proj = base.GetComponent<Projectile>();
                this.proj.OnHitEnemy += this.OnHitEnemy;
                this.proj.OnDestruction += this.OnDestroy;
            }

            private void OnDestroy(Projectile proj)
            {
                if(this.catchedEnemy != null)
                {
                    string guid = this.catchedEnemy.EnemyGuid;
                    Vector2 pos = this.catchedEnemy.sprite.WorldBottomLeft;
                    Vector2 blankPos = this.catchedEnemy.sprite.WorldCenter;
                    bool isBlackPhantom = this.catchedEnemy.IsBlackPhantom;
                    this.catchedEnemy.EraseFromExistence(true);
                    AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, pos, Quaternion.identity);
                    CompanionController orAddComponent = gameObject.GetOrAddComponent<CompanionController>();
                    if (isBlackPhantom)
                    {
                        gameObject.GetComponent<AIActor>().BecomeBlackPhantom();
                    }
                    orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
                    orAddComponent.Initialize(this.thrower);
                    orAddComponent.behaviorSpeculator.MovementBehaviors.Add(new CompanionFollowPlayerBehavior());
                    AIActor aiactor = gameObject.GetComponent<AIActor>();
                    if(orAddComponent.healthHaver != null)
                    {
                        orAddComponent.healthHaver.PreventAllDamage = true;
                    }
                    if(orAddComponent.bulletBank != null)
                    {
                        orAddComponent.bulletBank.OnProjectileCreated += CatchProjectileBehaviour.OnPostProcessProjectile;
                    }
                    if(orAddComponent.aiShooter != null)
                    {
                        orAddComponent.aiShooter.PostProcessProjectile += CatchProjectileBehaviour.OnPostProcessProjectile;
                    }
                }
                else
                {
                    if(this.parentItem != null)
                    {
                        parentItem.thrown = false;
                    }
                }
            }

            public static void OnPostProcessProjectile(Projectile proj)
            {
                if(proj.Owner is AIActor && (proj.Owner as AIActor).IsBlackPhantom)
                {
                    proj.baseData.damage = 15f;
                }
                else
                {
                    proj.baseData.damage = 10f;
                }
            }

            private void OnHitEnemy(Projectile proj, SpeculativeRigidbody enemyRigidbody, bool fatal)
            {
                if(enemyRigidbody.aiActor != null && enemyRigidbody.aiActor.healthHaver != null && !enemyRigidbody.aiActor.healthHaver.IsBoss)
                {
                    catchedEnemy = enemyRigidbody.aiActor;
                }
            }

            private AIActor catchedEnemy;
            public PokeballItem parentItem = null;
            private Projectile proj;
            public PlayerController thrower;
            public bool hasEvolutionSynergy = false;
        }
    }
}