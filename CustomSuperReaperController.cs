using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    public class CustomSuperReaperController : BraveBehaviour
    {
        public CustomSuperReaperController()
        {
            this.ShootTimer = 3f;
            this.MinSpeed = 3f;
            this.MaxSpeed = 10f;
            this.MinSpeedDistance = 10f;
            this.MaxSpeedDistance = 50f;
            this.c_particlesPerSecond = 50;
        }

        private void Start()
        {
            this.m_shootTimer = this.ShootTimer;
            SpeculativeRigidbody specRigidbody = base.specRigidbody;
            specRigidbody.OnEnterTrigger = (SpeculativeRigidbody.OnTriggerDelegate)Delegate.Combine(specRigidbody.OnEnterTrigger, new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTriggerEntered));
            base.aiAnimator.PlayUntilCancelled("idle", false, null, -1f, false);
            base.aiAnimator.PlayUntilFinished("intro", false, null, -1f, false);
            tk2dSpriteAnimator spriteAnimator = base.spriteAnimator;
            spriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(spriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.HandleAnimationEvent));
        }

        public static void Build()
        {
            GameObject reaper = UnityEngine.Object.Instantiate(PrefabDatabase.Instance.SuperReaper);
            reaper.SetActive(false);
            FakePrefab.MarkAsFakePrefab(reaper);
            UnityEngine.Object.DontDestroyOnLoad(reaper);
            SuperReaperController controller = reaper.GetComponent<SuperReaperController>();
            CustomSuperReaperController customController = reaper.AddComponent<CustomSuperReaperController>();
            customController.PreventShooting = false;
            customController.BulletScript = controller.BulletScript;
            customController.ShootPoint = controller.ShootPoint;
            customController.ShootTimer = controller.ShootTimer;
            customController.MinSpeed = controller.MinSpeed;
            customController.MaxSpeed = controller.MaxSpeed;
            customController.MinSpeedDistance = controller.MinSpeedDistance;
            customController.MaxSpeedDistance = controller.MaxSpeedDistance;
            customController.knockbackComponent = controller.knockbackComponent;
            Destroy(controller);
            List<AIBulletBank.Entry> entries = new List<AIBulletBank.Entry>();
            foreach(AIBulletBank.Entry bullet in customController.bulletBank.Bullets)
            {
                AIBulletBank.Entry entry = bullet.Clone(true, true, false, false);
                entries.Add(entry);
            }
            customController.bulletBank.Bullets.Clear();
            foreach (AIBulletBank.Entry bullet in entries)
            {
                customController.bulletBank.Bullets.Add(bullet);
            }
            customController.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().BulletScriptSettings.preventPooling = true;
            customController.bulletBank.Bullets[0].ProjectileData.damage = 10f;
            customController.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().GetAnySprite().SetSprite(ETGMod.Databases.Items.ProjectileCollection, ETGMod.Databases.Items.ProjectileCollection.GetSpriteIdByName("10x10_player_projectile_dark_002"));
            ETGMod.Databases.Items.ProjectileCollection.spriteDefinitions[customController.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().GetAnySprite().spriteId] =
                PrefabDatabase.Instance.SuperReaper.GetComponent<SuperReaperController>().bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().GetAnySprite().GetCurrentSpriteDef().CopyDefinitionFrom();
            Material material = Toolbox.GetGunById(38).DefaultModule.projectiles[0].GetAnySprite().GetCurrentSpriteDef().material;
            ETGMod.Databases.Items.ProjectileCollection.spriteDefinitions[customController.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().GetAnySprite().spriteId].material = material;
            ETGMod.Databases.Items.ProjectileCollection.spriteDefinitions[customController.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().GetAnySprite().spriteId].materialInst = material;
            ETGMod.Databases.Items.ProjectileCollection.spriteDefinitions[customController.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().GetAnySprite().spriteId].name = "10x10_player_projectile_dark_002";
            tk2dBaseSprite sprite = PrefabDatabase.Instance.SuperReaper.GetComponent<SuperReaperController>().bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().GetAnySprite();
            PixelCollider pcollider = PrefabDatabase.Instance.SuperReaper.GetComponent<SuperReaperController>().bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().specRigidbody.PrimaryPixelCollider;
            tk2dSpriteDefinition tk2dSpriteDefinition;
            if (pcollider.BagleUseFirstFrameOnly && !string.IsNullOrEmpty(pcollider.SpecifyBagelFrame))
            {
                tk2dSpriteDefinition = sprite.Collection.GetSpriteDefinition(pcollider.SpecifyBagelFrame);
            }
            else
            {
                tk2dSpriteDefinition = sprite.GetTrueCurrentSpriteDef();
            }
            int num = (tk2dSpriteDefinition != null) ? sprite.GetSpriteIdByName(tk2dSpriteDefinition.name) : -1;
            BagelColliderData data = Toolbox.GetBagelColliders(num, sprite.Collection);
            ETGMod.Databases.Items.ProjectileCollection.SpriteIDsWithBagelColliders.Add(customController.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().GetAnySprite().spriteId);
            ETGMod.Databases.Items.ProjectileCollection.SpriteDefinedBagelColliders.Add(data);
            customController.bulletBank.Bullets[0].BulletObject.GetComponent<Projectile>().specRigidbody.PrimaryPixelCollider.SpecifyBagelFrame = "";
            CustomSuperReaperController.friendlyReaperPrefab = reaper;
        }

        public void BindWithOwner(PlayerController player)
        {
            this.owner = player;
            if (base.bulletBank)
            {
                AIBulletBank bulletBank2 = base.bulletBank;
                bulletBank2.OnProjectileCreated += this.HandleCompanionPostProcessProjectile;
            }
        }

        private void HandleCompanionPostProcessProjectile(Projectile obj)
        {
            obj.IsBulletScript = false;
            if (obj)
            {
                obj.collidesWithPlayer = false;
                obj.collidesWithEnemies = true;
                obj.TreatedAsNonProjectileForChallenge = true;
                obj.UpdateCollisionMask();
            }
            if (this.owner)
            {
                if (PassiveItem.IsFlagSetForCharacter(this.owner, typeof(BattleStandardItem)))
                {
                    obj.baseData.damage *= BattleStandardItem.BattleStandardCompanionDamageMultiplier;
                }
                if (this.owner.CurrentGun && this.owner.CurrentGun.LuteCompanionBuffActive)
                {
                    obj.baseData.damage *= 2f;
                }
                if (this.owner.PlayerHasActiveSynergy("#GRIM_SUPERREAPER"))
                {
                    obj.baseData.damage *= 2f;
                }
                this.owner.DoPostProcessProjectile(obj);
            }

        }

        private void HandleTriggerEntered(SpeculativeRigidbody targetRigidbody, SpeculativeRigidbody sourceSpecRigidbody, CollisionData collisionData)
        {
            Projectile projectile = targetRigidbody.projectile;
            if (projectile)
            {
                projectile.HandleKnockback(base.specRigidbody, targetRigidbody.GetComponent<PlayerController>(), false, false);
            }
        }

        private void HandleAnimationEvent(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2, int arg3)
        {
            tk2dSpriteAnimationFrame frame = arg2.GetFrame(arg3);
            if (frame.eventInfo == "fire")
            {
                this.SpawnProjectiles();
            }
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
            {
                return;
            }
            if (BossKillCam.BossDeathCamRunning || GameManager.Instance.PreventPausing)
            {
                return;
            }
            if (TimeTubeCreditsController.IsTimeTubing)
            {
                base.gameObject.SetActive(false);
                return;
            }
            this.HandleMotion();
            if (!this.PreventShooting)
            {
                this.HandleAttacks();
            }
            this.UpdateBlackPhantomParticles();
        }

        private void HandleAttacks()
        {
            if (base.aiAnimator.IsPlaying("intro"))
            {
                return;
            }
            CellData cellData = GameManager.Instance.Dungeon.data[this.ShootPoint.position.IntXY(VectorConversions.Floor)];
            if (cellData != null && cellData.type != CellType.WALL && this.GetNearestEnemy() != null)
            {
                this.m_shootTimer -= BraveTime.DeltaTime;
                if (this.m_shootTimer <= 0f)
                {
                    base.aiAnimator.PlayUntilFinished("attack", false, null, -1f, false);
                    this.m_shootTimer = this.ShootTimer;
                }
            }
        }

        private void SpawnProjectiles()
        {
            if (GameManager.Instance.PreventPausing || BossKillCam.BossDeathCamRunning)
            {
                return;
            }
            if (this.PreventShooting)
            {
                return;
            }
            CellData cellData = GameManager.Instance.Dungeon.data[this.ShootPoint.position.IntXY(VectorConversions.Floor)];
            if (cellData == null || cellData.type == CellType.WALL)
            {
                return;
            }
            if (!this.m_bulletSource)
            {
                this.m_bulletSource = this.ShootPoint.gameObject.GetOrAddComponent<BulletScriptSource>();
            }
            this.m_bulletSource.BulletManager = base.bulletBank;
            this.m_bulletSource.BulletScript = this.BulletScript;
            this.m_bulletSource.Initialize();
        }

        private void UpdateBlackPhantomParticles()
        {
            if (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.LOW && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.VERY_LOW && !base.aiAnimator.IsPlaying("intro"))
            {
                Vector3 vector = base.specRigidbody.UnitBottomLeft.ToVector3ZisY(0f);
                Vector3 vector2 = base.specRigidbody.UnitTopRight.ToVector3ZisY(0f);
                float num = (vector2.y - vector.y) * (vector2.x - vector.x);
                float num2 = (float)this.c_particlesPerSecond * num;
                int num3 = Mathf.CeilToInt(Mathf.Max(1f, num2 * BraveTime.DeltaTime));
                int num4 = num3;
                Vector3 minPosition = vector;
                Vector3 maxPosition = vector2;
                Vector3 up = Vector3.up;
                float angleVariance = 120f;
                float magnitudeVariance = 0.5f;
                float? startLifetime = new float?(UnityEngine.Random.Range(1f, 1.65f));
                GlobalSparksDoer.DoRandomParticleBurst(num4, minPosition, maxPosition, up, angleVariance, magnitudeVariance, null, startLifetime, null, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
            }
        }

        private void HandleMotion()
        {
            base.specRigidbody.Velocity = Vector2.zero;
            if (base.aiAnimator.IsPlaying("intro"))
            {
                return;
            }
            if (this.owner == null)
            {
                return;
            }
            if (this.owner.healthHaver.IsDead || this.owner.IsGhost)
            {
                return;
            }
            if(this.GetNearestEnemy() == null)
            {
                if(Vector2.Distance(base.specRigidbody.UnitCenter, this.owner.CenterPosition) > CustomSuperReaperController.IdealRadius)
                {
                    Vector2 centerPosition = this.owner.CenterPosition;
                    Vector2 vector = centerPosition - base.specRigidbody.UnitCenter;
                    float magnitude = vector.magnitude;
                    float d = Mathf.Lerp(this.MinSpeed, this.MaxSpeed, (magnitude - this.MinSpeedDistance) / (this.MaxSpeedDistance - this.MinSpeedDistance));
                    base.specRigidbody.Velocity = vector.normalized * d;
                    base.specRigidbody.Velocity += this.knockbackComponent;
                }
            }
            else
            {
                Vector2 centerPosition = this.GetNearestEnemy().CenterPosition;
                Vector2 vector = centerPosition - base.specRigidbody.UnitCenter;
                float magnitude = vector.magnitude;
                if(magnitude < 0.1f)
                {
                    vector = Vector2.zero;
                    magnitude = vector.magnitude;
                }
                float d = Mathf.Lerp(this.MinSpeed, this.MaxSpeed, (magnitude - this.MinSpeedDistance) / (this.MaxSpeedDistance - this.MinSpeedDistance));
                base.specRigidbody.Velocity = vector.normalized * d;
                base.specRigidbody.Velocity += this.knockbackComponent;
            }
        }

        public AIActor GetNearestEnemy()
        {
            AIActor aiactor = null;
            float nearestDistance = float.MaxValue;
            if (base.specRigidbody.UnitCenter.GetAbsoluteRoom() == null)
            {
                return null;
            }
            if(this.owner != null)
            {
                if (base.specRigidbody.UnitCenter.GetAbsoluteRoom() != this.owner.CurrentRoom)
                {
                    return null;
                }
            }
            List<AIActor> activeEnemies = base.specRigidbody.UnitCenter.GetAbsoluteRoom().GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            AIActor result;
            if (activeEnemies == null)
            {
                return null;
            }
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                AIActor aiactor2 = activeEnemies[i];
                if(aiactor2 != null && aiactor2.healthHaver != null)
                {
                    bool flag3 = !aiactor2.healthHaver.IsDead;
                    if (flag3)
                    {
                        float num = Vector2.Distance(base.specRigidbody.UnitCenter, aiactor2.CenterPosition);
                        bool flag5 = num < nearestDistance;
                        if (flag5)
                        {
                            nearestDistance = num;
                            aiactor = aiactor2;
                        }
                    }
                }
            }
            result = aiactor;
            if(result == null)
            {
                return null;
            }
            return result;
        }

        public bool PreventShooting;
        public BulletScriptSelector BulletScript;
        public Transform ShootPoint;
        public float ShootTimer;
        public float MinSpeed;
        public float MaxSpeed;
        public float MinSpeedDistance;
        public float MaxSpeedDistance;
        public Vector2 knockbackComponent;
        private BulletScriptSource m_bulletSource;
        private float m_shootTimer;
        private int c_particlesPerSecond;
        private PlayerController owner;
        public static GameObject friendlyReaperPrefab;
        public static float IdealRadius = 4f;
    }
}
