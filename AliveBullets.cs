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
    class AliveBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Alive Bullets";
            string resourceName = "SpecialItemPack/Resources/AliveBullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<AliveBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Friendly Fire";
            string longDesc = "Bullets have a slight chance to actually become alive.\n\nTheese bullets were enchanted by Alben Smallbore to be alive, but the expirement... kinda failed. Theese bullets almost don't have any intellegence and will just shoot " +
                "anything bullet-like that comes in sight and because the lack of intellegence they will die after a short time. They will also explode if there's no bullet-like creatures next to it.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.C;
            item.AddToCursulaShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(241);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.PostProcessProjectile;
        }

        protected override void Update()
        {
            base.Update();
            List<AIActor> actorsToRemove = new List<AIActor>();
            foreach(AIActor aiactor in this.smarterMinions)
            {
                if(aiactor != null)
                {
                    if(aiactor.healthHaver != null)
                    {
                        if (!aiactor.healthHaver.IsAlive)
                        {
                            actorsToRemove.Add(aiactor);
                        }
                    }
                    else
                    {
                        actorsToRemove.Add(aiactor);
                    }
                }
                else
                {
                    actorsToRemove.Add(aiactor);
                }
            }
            foreach(AIActor aiactor in actorsToRemove)
            {
                this.smarterMinions.Remove(aiactor);
            }
        }

        public void PostProcessProjectile(Projectile proj, float f)
        {
            if (proj.Owner is AIActor && (proj.Owner as AIActor).GetComponent<SmarterDumbEnemyBehavior>() != null)
            {
                return;
            }
            float value = 0.25f;
            if (this.m_owner.PlayerHasActiveSynergy("#DUMBER_DUMB_BULLETS"))
            {
                value -= 0.1f;
            }
            if(UnityEngine.Random.value < value)
            {
                proj.specRigidbody.OnPreTileCollision += new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreTileCollision);
            }
            if (UnityEngine.Random.value < 0.05f && this.m_owner.PlayerHasActiveSynergy("#DUMBER_DUMB_BULLETS"))
            {
                proj.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
                if(proj.PossibleSourceGun != null && proj.PossibleSourceGun.DefaultModule != null)
                {
                    NewUsesProjectileSynergyController projectileSynergyController = proj.gameObject.AddComponent<NewUsesProjectileSynergyController>();
                    projectileSynergyController.ammoType = proj.PossibleSourceGun.DefaultModule.ammoType;
                    projectileSynergyController.customAmmoType = proj.PossibleSourceGun.DefaultModule.customAmmoType;
                }
            }
        }

        public void OnPreRigidbodyCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if(myRigidbody.projectile != null && myRigidbody.sprite != null && this.m_owner.PlayerHasActiveSynergy("#BE_SERIOUS"))
            {
                if (!this.m_owner.IsInCombat || this.smarterMinions.Count >= 4)
                {
                    return;
                }
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
                IntVector2? nearestAvailableCell = null;
                if (this.m_owner.CurrentRoom != null)
                {
                    nearestAvailableCell = this.m_owner.CurrentRoom.GetNearestAvailableCell(myRigidbody.projectile.sprite.WorldCenter, new IntVector2?(orLoadByGuid.Clearance), new CellTypes?(CellTypes.FLOOR), false, null);
                }
                Vector3 vector = nearestAvailableCell != null ? (nearestAvailableCell.Value.ToVector2()).ToVector3ZUp(0f) : myRigidbody.projectile.sprite.WorldCenter.ToVector3ZUp(0f);
                GameObject enemyObj = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, vector, Quaternion.identity);
                CompanionController orAddComponent = enemyObj.GetOrAddComponent<CompanionController>();
                orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
                orAddComponent.Initialize(this.m_owner);
                orAddComponent.behaviorSpeculator.MovementBehaviors.Add(new CompanionFollowPlayerBehavior());
                AIActor aiactor = enemyObj.GetComponent<AIActor>();
                aiactor.HitByEnemyBullets = true;
                aiactor.healthHaver.ModifyDamage += this.ModifyDamageForCompanions;
                aiactor.StartCoroutine(this.InvisibleCoroutine(aiactor));
                aiactor.procedurallyOutlined = false;
                aiactor.CorpseObject = null;
                aiactor.ImmuneToAllEffects = true;
                aiactor.SetIsFlying(true, "I'm a bullet too!");
                SmarterDumbEnemyBehavior smartBehavior = aiactor.gameObject.AddComponent<SmarterDumbEnemyBehavior>();
                smartBehavior.layer = myRigidbody.projectile.gameObject.layer;
                smartBehavior.collection = myRigidbody.projectile.sprite.Collection;
                smartBehavior.spriteId = myRigidbody.projectile.sprite.spriteId;
                foreach (AIBulletBank.Entry entry in orAddComponent.bulletBank.Bullets)
                {
                    if (aiactor.IsBlackPhantom)
                    {
                        entry.BulletObject.GetComponent<Projectile>().baseData.damage = 15;
                    }
                    else
                    {
                        entry.BulletObject.GetComponent<Projectile>().baseData.damage = 10;
                    }
                }
                foreach (AttackBehaviorBase behavior in aiactor.behaviorSpeculator.AttackBehaviors)
                {
                    if ((behavior as ShootGunBehavior) != null)
                    {
                        if (aiactor.IsBlackPhantom)
                        {
                            aiactor.aiShooter.GetBulletEntry((behavior as ShootGunBehavior).OverrideBulletName).ProjectileData.damage = 15;
                        }
                        else
                        {
                            aiactor.aiShooter.GetBulletEntry((behavior as ShootGunBehavior).OverrideBulletName).ProjectileData.damage = 10;
                        }
                    }
                }
                this.smarterMinions.Add(aiactor);
            }
            else if (myRigidbody.projectile != null && myRigidbody.sprite != null)
            {
                AIActor aiactor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5"), myRigidbody.projectile.sprite.WorldCenter, myRigidbody.transform.position.GetAbsoluteRoom(), true, AIActor.AwakenAnimationType.Default, true);
                GameObject gameObject = new GameObject("suck image");
                gameObject.layer = myRigidbody.projectile.gameObject.layer;
                tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
                gameObject.transform.parent = SpawnManager.Instance.Projectiles;
                gameObject.transform.position = aiactor.sprite.WorldCenter;
                tk2dSprite.SetSprite(myRigidbody.projectile.sprite.Collection, myRigidbody.projectile.sprite.spriteId);
                tk2dSprite.transform.parent = aiactor.transform;
                aiactor.sprite.renderer.enabled = false;
                aiactor.procedurallyOutlined = false;
                aiactor.CanTargetEnemies = true;
                aiactor.CanTargetPlayers = false;
                aiactor.CorpseObject = null;
                aiactor.ImmuneToAllEffects = true;
                aiactor.SetIsFlying(true, "I'm a bullet too!");
                aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.PlayerCollider));
                if (aiactor.IgnoreForRoomClear)
                {
                    aiactor.IgnoreForRoomClear = false;
                    if (aiactor.ParentRoom != null)
                    {
                        aiactor.ParentRoom.DeregisterEnemy(aiactor);
                    }
                }
                DumbEnemyBehavior dumbEnemyBehavior = aiactor.gameObject.AddComponent<DumbEnemyBehavior>();
                if(myRigidbody.projectile.GetComponent<NewUsesProjectileSynergyController>() != null)
                {
                    dumbEnemyBehavior.sourceAmmoType = myRigidbody.projectile.GetComponent<NewUsesProjectileSynergyController>().ammoType;
                    dumbEnemyBehavior.customAmmoType = myRigidbody.projectile.GetComponent<NewUsesProjectileSynergyController>().customAmmoType;
                    dumbEnemyBehavior.usesNewUsesSynergy = true;
                }
                myRigidbody.projectile.DieInAir();
            }
        }

        public void OnPreTileCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            if (myRigidbody.projectile != null && myRigidbody.sprite != null && this.m_owner.PlayerHasActiveSynergy("#BE_SERIOUS"))
            {
                if (!this.m_owner.IsInCombat || this.smarterMinions.Count >= 4)
                {
                    return;
                }
                AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5");
                IntVector2? nearestAvailableCell = null;
                if (this.m_owner.CurrentRoom != null)
                {
                    nearestAvailableCell = this.m_owner.CurrentRoom.GetNearestAvailableCell(myRigidbody.projectile.sprite.WorldCenter, new IntVector2?(orLoadByGuid.Clearance), new CellTypes?(CellTypes.FLOOR), false, null);
                }
                Vector3 vector = nearestAvailableCell != null ? (nearestAvailableCell.Value.ToVector2()).ToVector3ZUp(0f) : myRigidbody.projectile.sprite.WorldCenter.ToVector3ZUp(0f);
                GameObject enemyObj = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, vector, Quaternion.identity);
                CompanionController orAddComponent = enemyObj.GetOrAddComponent<CompanionController>();
                orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
                orAddComponent.Initialize(this.m_owner);
                orAddComponent.behaviorSpeculator.MovementBehaviors.Add(new CompanionFollowPlayerBehavior());
                AIActor aiactor = enemyObj.GetComponent<AIActor>();
                aiactor.HitByEnemyBullets = true;
                aiactor.healthHaver.ModifyDamage += this.ModifyDamageForCompanions;
                aiactor.StartCoroutine(this.InvisibleCoroutine(aiactor));
                aiactor.procedurallyOutlined = false;
                aiactor.CorpseObject = null;
                aiactor.ImmuneToAllEffects = true;
                aiactor.SetIsFlying(true, "I'm a bullet too!");
                SmarterDumbEnemyBehavior smartBehavior = aiactor.gameObject.AddComponent<SmarterDumbEnemyBehavior>();
                smartBehavior.layer = myRigidbody.projectile.gameObject.layer;
                smartBehavior.collection = myRigidbody.projectile.sprite.Collection;
                smartBehavior.spriteId = myRigidbody.projectile.sprite.spriteId;
                foreach (AIBulletBank.Entry entry in orAddComponent.bulletBank.Bullets)
                {
                    if (aiactor.IsBlackPhantom)
                    {
                        entry.BulletObject.GetComponent<Projectile>().baseData.damage = 15;
                    }
                    else
                    {
                        entry.BulletObject.GetComponent<Projectile>().baseData.damage = 10;
                    }
                }
                foreach (AttackBehaviorBase behavior in aiactor.behaviorSpeculator.AttackBehaviors)
                {
                    if ((behavior as ShootGunBehavior) != null)
                    {
                        if (aiactor.IsBlackPhantom)
                        {
                            aiactor.aiShooter.GetBulletEntry((behavior as ShootGunBehavior).OverrideBulletName).ProjectileData.damage = 15;
                        }
                        else
                        {
                            aiactor.aiShooter.GetBulletEntry((behavior as ShootGunBehavior).OverrideBulletName).ProjectileData.damage = 10;
                        }
                    }
                }
                this.smarterMinions.Add(aiactor);
            }
            else if(myRigidbody.projectile != null && myRigidbody.sprite != null)
            {
                AIActor aiactor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid("01972dee89fc4404a5c408d50007dad5"), myRigidbody.projectile.sprite.WorldCenter, myRigidbody.transform.position.GetAbsoluteRoom(), true, AIActor.AwakenAnimationType.Default, true);
                GameObject gameObject = new GameObject("suck image");
                gameObject.layer = myRigidbody.projectile.gameObject.layer;
                tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
                gameObject.transform.parent = SpawnManager.Instance.Projectiles;
                gameObject.transform.position = aiactor.sprite.WorldCenter;
                tk2dSprite.SetSprite(myRigidbody.projectile.sprite.Collection, myRigidbody.projectile.sprite.spriteId);
                tk2dSprite.transform.parent = aiactor.transform;
                aiactor.sprite.renderer.enabled = false;
                aiactor.procedurallyOutlined = false;
                aiactor.CanTargetEnemies = true;
                aiactor.CanTargetPlayers = false;
                aiactor.CorpseObject = null;
                aiactor.SetIsFlying(true, "I'm a bullet too!");
                aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.PlayerHitBox, CollisionLayer.PlayerCollider));
                if (aiactor.IgnoreForRoomClear)
                {
                    aiactor.IgnoreForRoomClear = false;
                    if (aiactor.ParentRoom != null)
                    {
                        aiactor.ParentRoom.DeregisterEnemy(aiactor);
                    }
                }
                DumbEnemyBehavior dumbEnemyBehavior = aiactor.gameObject.AddComponent<DumbEnemyBehavior>();
                if (myRigidbody.projectile.GetComponent<NewUsesProjectileSynergyController>() != null)
                {
                    dumbEnemyBehavior.sourceAmmoType = myRigidbody.projectile.GetComponent<NewUsesProjectileSynergyController>().ammoType;
                    dumbEnemyBehavior.customAmmoType = myRigidbody.projectile.GetComponent<NewUsesProjectileSynergyController>().customAmmoType;
                    dumbEnemyBehavior.usesNewUsesSynergy = true;
                }
                myRigidbody.projectile.DieInAir();
            }
        }


        private IEnumerator InvisibleCoroutine(AIActor aiactor)
        {
            yield return new WaitForSeconds(0.05f);
            aiactor.sprite.renderer.enabled = false;
            yield break;
        }

        private void ModifyDamageForCompanions(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == HealthHaver.ModifyDamageEventArgs.Empty)
            {
                return;
            }
            args.ModifiedDamage *= 5f;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProcessProjectile;
            return base.Drop(player);
        }

        private List<AIActor> smarterMinions = new List<AIActor>();

        public class DumbEnemyBehavior : MonoBehaviour
        {
            private void Awake()
            {
                this.owner = base.GetComponent<AIActor>();
                this.owner.StartCoroutine(this.DoStuff());
            }

            private IEnumerator DoStuff()
            {
                yield return new WaitForSeconds(10f);
                this.owner.healthHaver.ApplyDamage(9999, Vector2.zero, "Lack of Intellegence", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                yield break;
            }

            private bool CheckForEnemies()
            {
                bool result = true;
                foreach(AIActor aiactor in this.owner.ParentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.RoomClear))
                {
                    if(aiactor.GetComponent<DumbEnemyBehavior>() == null)
                    {
                        result = false;
                    }
                }
                return result;
            }

            private void Update()
            {
                if (this.owner.ParentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.RoomClear) != null && CheckForEnemies())
                {
                    if (!this.PreventExploding)
                    {
                        this.owner.healthHaver.ApplyDamage(9999, Vector2.zero, "Lack of Intellegence", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                        Exploder.DoDefaultExplosion(this.owner.sprite.WorldCenter, new Vector2());
                        this.PreventExploding = true;
                    }
                }
            }
            private AIActor owner;
            private bool PreventExploding = false;
            public GameUIAmmoType.AmmoType sourceAmmoType;
            public string customAmmoType;
            public bool usesNewUsesSynergy = false;
        }

        public class SmarterDumbEnemyBehavior : MonoBehaviour
        {
            private void Update()
            {
                base.GetComponent<AIActor>().sprite.renderer.enabled = false;
                if (this.projectileAttached == null)
                {
                    GameObject gameObject = new GameObject("suck image");
                    gameObject.layer = this.layer;
                    tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
                    gameObject.transform.parent = SpawnManager.Instance.Projectiles;
                    gameObject.transform.position = base.GetComponent<AIActor>().sprite.WorldCenter;
                    tk2dSprite.SetSprite(this.collection, this.spriteId);
                    tk2dSprite.transform.parent = base.GetComponent<AIActor>().transform;
                    this.projectileAttached = gameObject.transform;
                }
            }

            public Transform projectileAttached = null;
            public tk2dSpriteCollectionData collection;
            public int spriteId;
            public int layer;
        }

        public class NewUsesProjectileSynergyController : MonoBehaviour
        {
            public GameUIAmmoType.AmmoType ammoType;
            public string customAmmoType;
        }
    }
}
