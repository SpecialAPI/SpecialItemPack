using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using Dungeonator;

namespace SpecialItemPack.Shapeshifting
{
    class WallOfFleshBehaviour : ShapeShiftBehaviour
    {
        public static void SetupObjects()
        {
            GameObject eye = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/WallEye");
            eye.SetActive(false);
            FakePrefab.MarkAsFakePrefab(eye);
            UnityEngine.Object.DontDestroyOnLoad(eye);
            eye.GetComponent<tk2dSprite>().GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, eye.GetComponent<tk2dSprite>().GetCurrentSpriteDef().position3);
            EyePrefab = eye;
        }

        public override void OnApplied()
        {
            player.SetFlagForPlayer(typeof(HeavyBootsItem));
            player.SetFlagForPlayer(typeof(LiveAmmoItem));
            player.MovementModifiers += this.ProcessMovement;
            GameObject instanceVFX = player.PlayEffectOnActor((PickupObjectDatabase.GetById(662) as CheeseWheelItem).TransformationVFX, Vector3.zero, true, true, false);
            instanceVFX.transform.localPosition = instanceVFX.transform.localPosition.QuantizeFloor(0.0625f);
            this.instanceVFX = instanceVFX;
            tk2dSprite instanceVFXSprite = instanceVFX.GetComponent<tk2dSprite>();
            tk2dSpriteAnimator instanceVFXAnimator = instanceVFX.GetComponent<tk2dSpriteAnimator>();
            instanceVFXAnimator.MuteAudio = true;
            player.ToggleShadowVisiblity(true);
            player.SetIsFlying(true, "wall", false, false);
            player.AdditionalCanDodgeRollWhileFlying.SetOverride("wall", true);
            player.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePrerigidbodyCollision);
            player.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
            this.firstEye = Instantiate(EyePrefab, player.CenterPosition, Quaternion.identity);
            this.secondEye = Instantiate(EyePrefab, player.CenterPosition, Quaternion.identity);
            player.OnTriedToInitiateAttack += this.Fire;
        }

        private void Fire(PlayerController player)
        {
            if(cooldown <= 0f)
            {
                GameObject obj = SpawnManager.SpawnProjectile(Toolbox.GetGunById(58).DefaultModule.projectiles[0].gameObject, firstEye.transform.position, firstEye.transform.rotation);
                Projectile proj = obj.GetComponent<Projectile>();
                if (proj != null)
                {
                    proj.Owner = player;
                    proj.Shooter = player.specRigidbody;
                }
                GameObject obj2 = SpawnManager.SpawnProjectile(Toolbox.GetGunById(58).DefaultModule.projectiles[0].gameObject, secondEye.transform.position, secondEye.transform.rotation);
                Projectile proj2 = obj2.GetComponent<Projectile>();
                if (proj2 != null)
                {
                    proj2.Owner = player;
                    proj2.Shooter = player.specRigidbody;
                }
                if(player.characterIdentity == PlayableCharacters.Robot)
                {
                    cooldown = 0.25f;
                }
                else
                {
                    cooldown = player.healthHaver.GetCurrentHealthPercentage() / 2f;
                }
            }
        }

        protected Vector2 FindExpectedEndPoint(Vector2 direction)
        {
            Dungeon dungeon = GameManager.Instance.Dungeon;
            Vector2 unitCenter = base.specRigidbody.UnitCenter;
            Vector2 b = unitCenter + direction.normalized * float.MaxValue;
            RoomHandler room = unitCenter.GetAbsoluteRoom();
            bool flag = false;
            Vector2 vector = unitCenter;
            IntVector2 intVector = vector.ToIntVector2(VectorConversions.Floor);
            if (dungeon.data.CheckInBoundsAndValid(intVector))
            {
                flag = dungeon.data[intVector].isExitCell;
            }
            float num = b.x - unitCenter.x;
            float num2 = b.y - unitCenter.y;
            float num3 = Mathf.Sign(b.x - unitCenter.x);
            float num4 = Mathf.Sign(b.y - unitCenter.y);
            bool flag2 = num3 > 0f;
            bool flag3 = num4 > 0f;
            int num5 = 0;
            Vector2 toReturn = unitCenter;
            while (Vector2.Distance(vector, b) > 0.1f && num5 < 10000)
            {
                num5++;
                toReturn += direction.normalized / 2;
                if(dungeon.data.CheckInBoundsAndValid(toReturn.ToIntVector2(VectorConversions.Floor)) && dungeon.data.cellData[toReturn.ToIntVector2(VectorConversions.Floor).x][toReturn.ToIntVector2(VectorConversions.Floor).y].type == CellType.WALL)
                {
                    break;
                }
                if(dungeon.data.CheckInBoundsAndValid(toReturn.ToIntVector2(VectorConversions.Floor)) && dungeon.data[toReturn.ToIntVector2(VectorConversions.Floor)].isExitCell)
                {
                    break;
                }
            }
            return toReturn;
        }

        private void HandlePrerigidbodyCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody && otherRigidbody.healthHaver && otherRigidbody.healthHaver.IsDead)
            {
                PhysicsEngine.SkipCollision = true;
            }
        }
        
        private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
        {
            AIActor component = rigidbodyCollision.OtherRigidbody.GetComponent<AIActor>();
            bool flag = false;
            if (component && component.IsNormalEnemy && component.healthHaver && component.healthHaver.IsVulnerable)
            {
                if (component.FlagToSetOnDeath == GungeonFlags.BOSSKILLED_DEMONWALL)
                {
                    flag = true;
                    component.healthHaver.ApplyDamage(35f, rigidbodyCollision.Normal * -1f, "nom nom", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                }
                else if (component.healthHaver.IsBoss)
                {
                    flag = true;
                    component.healthHaver.ApplyDamage(35f, rigidbodyCollision.Normal * -1f, "nom nom", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                }
                else
                {
                    flag = true;
                    component.healthHaver.ApplyDamage(20f, rigidbodyCollision.Normal * -1f, "nom nom", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                }
            }
            else
            {
                MajorBreakable component3 = rigidbodyCollision.OtherRigidbody.GetComponent<MajorBreakable>();
                BodyPartController component4 = rigidbodyCollision.OtherRigidbody.GetComponent<BodyPartController>();
                if (component4 && component3)
                {
                    flag = true;
                    Vector2 normalized = (rigidbodyCollision.MyRigidbody.UnitCenter - rigidbodyCollision.OtherRigidbody.UnitCenter).normalized;
                    component3.ApplyDamage(20f, normalized * -1f, false, false, false);
                    if (component3.healthHaver)
                    {
                        component3.healthHaver.ApplyDamage(20f, normalized * -1f, "nom nom", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
                    }
                }
            }
            if (flag)
            {
                rigidbodyCollision.MyRigidbody.RegisterTemporaryCollisionException(rigidbodyCollision.OtherRigidbody, 0.5f, null);
            }
        }

        private void Update()
        {
            if(player != null)
            {
                if(this.instanceVFX == null && !this.isDead)
                {
                    GameObject instanceVFX = player.PlayEffectOnActor((PickupObjectDatabase.GetById(662) as CheeseWheelItem).TransformationVFX, Vector3.zero, true, true, false);
                    instanceVFX.transform.localPosition = instanceVFX.transform.localPosition.QuantizeFloor(0.0625f);
                    this.instanceVFX = instanceVFX;
                    tk2dSpriteAnimator instanceVFXAnimator2 = instanceVFX.GetComponent<tk2dSpriteAnimator>();
                    instanceVFXAnimator2.MuteAudio = true;
                }
                if(cooldown > 0f)
                {
                    cooldown -= BraveTime.DeltaTime;
                }
                player.IsOnFire = false;
                player.CurrentPoisonMeterValue = 0f;
                player.IsGunLocked = true;
                if (player.IsVisible)
                {
                    player.IsVisible = false;
                    player.ToggleShadowVisiblity(true);
                }
                if (player.specRigidbody.Velocity != Vector2.zero)
                {
                    this.lastVel = player.specRigidbody.Velocity;
                }
                if (this.instanceVFX != null)
                {
                    tk2dSpriteAnimator instanceVFXAnimator = instanceVFX.GetComponent<tk2dSpriteAnimator>();
                    instanceVFXAnimator.MuteAudio = true;
                    if (instanceVFXAnimator)
                    {
                        if (this.isDead)
                        {
                            instanceVFXAnimator.UpdateAnimation(GameManager.INVARIANT_DELTA_TIME);
                        }
                        else if (player.spriteAnimator != null && player.spriteAnimator.IsPlaying((!player.UseArmorlessAnim) ? "death_shot" : "death_shot_armorless") && !instanceVFXAnimator.IsPlaying("Resourceful_Rat_pac_outro"))
                        {
                            instanceVFXAnimator.PlayAndDestroyObject("Resourceful_Rat_pac_outro");
                            this.isDead = true;
                        }
                        else
                        {
                            if (instanceVFXAnimator.IsPlaying("Resourceful_Rat_pac_intro"))
                            {
                                instanceVFXAnimator.Stop();
                                instanceVFXAnimator.Play("Resourceful_Rat_pac_player");
                            }
                        }
                    }
                    if (instanceVFX)
                    {
                        instanceVFX.transform.localRotation = Quaternion.Euler(0f, 0f, (player.unadjustedAimPoint.XY() - player.CenterPosition).ToAngle());
                        instanceVFX.GetComponent<tk2dSprite>().ForceRotationRebuild();
                    }
                    if (player.specRigidbody.Velocity != Vector2.zero)
                    {
                        float z = player.specRigidbody.Velocity.ToAngle();
                    }
                }
            }
        }

        private void LateUpdate()
        {
            ProcessEye(this.firstEye);
            ProcessEye(this.secondEye, -90f);
        }

        private void ProcessEye(GameObject eye, float angleOffset = 90f)
        {
            Vector2 vector = BraveMathCollege.DegreesToVector((player.unadjustedAimPoint.XY() - player.CenterPosition).ToAngle() + angleOffset, 1f);
            float lerp = 0f;
            Vector2 exceptedEndPoint = this.FindExpectedEndPoint(vector);
            if (angleOffset == 90f)
            {
                float a = 0.5f - (Vector2.Distance(eye.transform.position, player.CenterPosition) / Vector2.Distance(exceptedEndPoint, player.CenterPosition));
                a = a.Normalize();
                this.lerp = (Vector2.Distance(eye.transform.position, player.CenterPosition) / Vector2.Distance(exceptedEndPoint, player.CenterPosition));
                /*bool flag5 = Mathf.Abs(Toolbox.Distance(0.5f, (Vector2.Distance(eye.transform.position, player.CenterPosition) / Vector2.Distance(exceptedEndPoint, player.CenterPosition)))) < 0.02f;
                if (flag5)
                {
                    this.velocity = Mathf.Lerp(this.velocity, 0f, 0.5f);
                }
                else
                {
                }*/
                this.velocity = Mathf.Lerp(this.velocity, a * eyeMoveSpeed, 0.1f);
                this.lerp += this.velocity * BraveTime.DeltaTime;
                lerp = this.lerp;
            }
            else if(angleOffset == -90f)
            {
                float a = 0.5f - (Vector2.Distance(eye.transform.position, player.CenterPosition) / Vector2.Distance(exceptedEndPoint, player.CenterPosition));
                a = a.Normalize();
                this.lerpRight = (Vector2.Distance(eye.transform.position, player.CenterPosition) / Vector2.Distance(exceptedEndPoint, player.CenterPosition));
                /*bool flag5 = Mathf.Abs(Toolbox.Distance(0.5f, (Vector2.Distance(eye.transform.position, player.CenterPosition) / Vector2.Distance(exceptedEndPoint, player.CenterPosition)))) < 0.02f;
                if (flag5)
                {
                    this.velocityRight = Mathf.Lerp(this.velocityRight, 0f, 0.5f);
                }
                else
                {
                }*/
                this.velocityRight = Mathf.Lerp(this.velocityRight, a * eyeMoveSpeed, 0.3f);
                this.lerpRight += this.velocityRight * BraveTime.DeltaTime;
                lerp = this.lerpRight;
            }
            Vector2 vector2 = Vector2.LerpUnclamped(player.CenterPosition, 
                player.CenterPosition + BraveMathCollege.DegreesToVector((player.unadjustedAimPoint.XY() - player.CenterPosition).ToAngle() + angleOffset, Vector2.Distance(exceptedEndPoint, player.CenterPosition)), lerp);
            eye.transform.position = vector2;
            AimAt(eye, player.unadjustedAimPoint.XY());
        }

        private void AimAt(GameObject obj, Vector2 point)
        {
            Vector2 v = point - obj.transform.position.XY();
            float currentAimTarget = BraveMathCollege.Atan2Degrees(v);
            obj.transform.rotation = Quaternion.Euler(0f, 0f, currentAimTarget);
        }

        private void ProcessMovement(ref Vector2 movementComponent, ref Vector2 knockbackComponent)
        {
            knockbackComponent = Vector2.zero;
            if (!player.usingForcedInput)
            {
                float x = movementComponent.x;
                float y = movementComponent.y;
                float absX = Mathf.Abs(x);
                float absY = Mathf.Abs(y);
                if (absX > absY)
                {
                    movementComponent.y = 0f;
                }
                else if (absY > absX)
                {
                    movementComponent.x = 0f;
                }
                else
                {
                    //This is possible?
                    movementComponent.x = 0f;
                }
            }
        }

        public override void ClearEffect()
        {
            player.UnsetFlagForPlayer(typeof(HeavyBootsItem));
            player.UnsetFlagForPlayer(typeof(LiveAmmoItem));
            player.MovementModifiers -= this.ProcessMovement;
            player.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePrerigidbodyCollision);
            player.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
            player.SetIsFlying(false, "wall", false, false);
            player.AdditionalCanDodgeRollWhileFlying.SetOverride("wall", false);
            player.IsVisible = true;
            if (instanceVFX)
            {
                SpawnManager.Despawn(instanceVFX);
                instanceVFX = null;
            }
            player.IsGunLocked = false;
        }

        public override List<StatModifier> shapeShiftModifiers => new List<StatModifier>();
        private GameObject instanceVFX;
        private bool isDead;
        private GameObject firstEye;
        private GameObject secondEye;
        private Vector2 lastVel = Vector2.down;
        private static GameObject EyePrefab;
        private float cooldown;
        private float velocity;
        private float lerp = 0.2f;
        private float velocityRight;
        private float lerpRight = 0.2f;
        private static readonly float eyeMoveSpeed = 0.5f;
    }
}
