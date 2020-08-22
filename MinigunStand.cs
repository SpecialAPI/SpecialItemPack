using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;
using System.Collections;

namespace SpecialItemPack
{
    class MinigunStand : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Minigun Stand";
            string resourceName = "SpecialItemPack/Resources/MinigunStand";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<MinigunStand>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "That's What I'm Talking 'Bout!";
            string longDesc = "Places a heavy minigun.\n\nThis stand was created for the Vulcan Cannon, the favored weapon of a predatory bird. Because the owner of the minigun was strong enough to carry it, he didn't need the stand, and thrown it away.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 150f);
            MinigunStand.BuildPrefab();
            item.consumable = false;
            item.quality = ItemQuality.B;
            item.AddToBlacksmithShop();
            item.AddToTrorkShop();
            item.PlaceItemInAmmonomiconAfterItemById(490);

        }

        private static void BuildPrefab()
        {
            GameObject gameObject = new GameObject("Minigun");
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            MinigunBehaviour behaviour = gameObject.AddComponent<MinigunBehaviour>();
            if (behaviour.spriteAnimator == null)
            {
                behaviour.spriteAnimator = behaviour.gameObject.AddComponent<tk2dSpriteAnimator>();
            }
            behaviour.spriteAnimator.Library = Toolbox.GetGunById(707).GetComponent<tk2dSpriteAnimator>().Library;
            behaviour.idleAnimation = Toolbox.GetGunById(707).idleAnimation;
            behaviour.shootAnimation = Toolbox.GetGunById(707).shootAnimation;
            behaviour.reloadAnimation = Toolbox.GetGunById(707).reloadAnimation;
            behaviour.GunSwitchGroup = Toolbox.GetGunById(707).gunSwitchGroup;
            behaviour.CooldownTime = Toolbox.GetGunById(707).DefaultModule.cooldownTime * 1.25f;
            behaviour.AngleVariance = Toolbox.GetGunById(707).DefaultModule.angleVariance;
            behaviour.ProjectileToShoot = Toolbox.GetGunById(707).DefaultModule.projectiles[0];
            behaviour.ReloadTime = Toolbox.GetGunById(707).reloadTime;
            tk2dSprite.SetSprite(Toolbox.GetGunById(707).sprite.Collection, Toolbox.GetGunById(707).sprite.spriteId);
            behaviour.spriteId = Toolbox.GetGunById(707).sprite.spriteId;
            behaviour.collection = Toolbox.GetGunById(707).sprite.Collection;
            gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.GameObject.DontDestroyOnLoad(gameObject);
            MinigunPrefab = gameObject;
            GameObject obj = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/stand");
            obj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(obj);
            UnityEngine.GameObject.DontDestroyOnLoad(obj);
            StandPrefab = obj;
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            GameObject obj = UnityEngine.Object.Instantiate(MinigunPrefab, user.sprite.WorldCenter, Quaternion.identity);
            MinigunBehaviour behaviour = obj.GetComponent<MinigunBehaviour>();
            behaviour.Owner = user;
            behaviour.playerCanCarryMe = user.PlayerHasActiveSynergy("#IM_THE_BOSS_HERE");
            behaviour.DelayedInitialization();
            base.StartCoroutine(ItemBuilder.HandleDuration(this, 10, user, null));
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user);
        }

        public static GameObject MinigunPrefab;
        public static GameObject StandPrefab;

        public class MinigunBehaviour : BraveBehaviour
        {
            public void DelayedInitialization()
            {
                this.m_cooldownRemaining = 0;
                this.m_reloadRemaining = 0;
                //this.m_extantStand = Instantiate(MinigunStand.StandPrefab, base.sprite.WorldCenter, Quaternion.identity);
                //this.m_extantStand.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(base.sprite.WorldBottomLeft, tk2dBaseSprite.Anchor.UpperCenter);
                AkSoundEngine.SetSwitch("WPN_Guns", this.GunSwitchGroup, base.gameObject);
                base.StartCoroutine(this.HandleDuration());
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black);
                base.StartCoroutine(this.HandleGeneration());
            }

            private IEnumerator HandleGeneration()
            {
                yield return null;
                Projectile proj = Toolbox.GetGunById(761).DefaultModule.projectiles[0];
                if (proj != null && proj.GetComponent<KthuliberProjectileController>() != null && proj.GetComponent<KthuliberProjectileController>().ExplodeVFX != null)
                {
                    SpawnManager.SpawnVFX(proj.GetComponent<KthuliberProjectileController>().ExplodeVFX, base.sprite.WorldCenter, Quaternion.identity);
                    AkSoundEngine.PostEvent("Play_OBJ_item_spawn_01", base.gameObject);
                }
                yield break;
            }

            private IEnumerator HandleDuration()
            {
                float elapsed = 0;
                while (elapsed < 10)
                {
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                Projectile proj = Toolbox.GetGunById(761).DefaultModule.projectiles[0];
                if (proj != null && proj.GetComponent<KthuliberProjectileController>() != null && proj.GetComponent<KthuliberProjectileController>().ExplodeVFX != null)
                {
                    SpawnManager.SpawnVFX(proj.GetComponent<KthuliberProjectileController>().ExplodeVFX, base.sprite.WorldCenter, Quaternion.identity);
                    AkSoundEngine.PostEvent("Play_OBJ_item_spawn_01", base.gameObject);
                }
                if(this.m_extantStand != null)
                {
                    Destroy(this.m_extantStand);
                }
                UnityEngine.Object.Destroy(base.gameObject);
                this.Owner.IsGunLocked = false;
                yield break;
            }

            private void LateUpdate()
            {
                if (this.m_reloadRemaining > 0)
                {
                }
                else
                {
                    if (this.m_cooldownRemaining > 0)
                    {
                    }
                    else
                    {
                        if (this.OwnerAvailable())
                        {
                            BraveInput input2 = BraveInput.GetInstanceForPlayer(this.Owner.PlayerIDX);
                            if (input2 != null && input2.ActiveActions.GetActionFromType(GungeonActions.GungeonActionType.Shoot).IsPressed)
                            {
                                GameObject obj = SpawnManager.SpawnProjectile(this.ProjectileToShoot.gameObject, this.sprite.WorldCenter,
                                    Quaternion.Euler(0, 0, BraveMathCollege.Atan2Degrees(this.GetAimPoint(this.Owner)) + UnityEngine.Random.Range(-this.AngleVariance, this.AngleVariance)));
                                Projectile proj = obj.GetComponent<Projectile>();
                                if (proj != null)
                                {
                                    if (this.Owner != null)
                                    {
                                        proj.Owner = this.Owner;
                                        proj.Shooter = this.Owner.specRigidbody;
                                    }
                                }
                                AkSoundEngine.PostEvent("Play_WPN_gun_shot_01", base.gameObject);
                                this.spriteAnimator.Play(this.shootAnimation);
                                base.StartCoroutine(this.HandleCooldown());
                            }
                        }
                    }
                }
                BraveInput input = BraveInput.GetInstanceForPlayer(this.Owner.PlayerIDX);
                if (this.m_cooldownRemaining <= 0 && (!this.OwnerAvailable() || input == null || !input.ActiveActions.GetActionFromType(GungeonActions.GungeonActionType.Shoot).IsPressed))
                {
                    this.spriteAnimator.Stop();
                    this.sprite.SetSprite(this.collection, this.spriteId);
                }
                if (this.OwnerAvailable())
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, BraveMathCollege.ClampAngle180(BraveMathCollege.Atan2Degrees(this.GetAimPoint(this.Owner))));
                    bool flag2 = this.transform.rotation.eulerAngles.z > 90f && this.transform.rotation.eulerAngles.z < 270f;
                    if (flag2 && !base.sprite.FlipY)
                    {
                        base.sprite.FlipY = true;
                    }
                    else if (!flag2 && base.sprite.FlipY)
                    {
                        base.sprite.FlipY = false;
                    }
                    this.Owner.IsGunLocked = true;
                }
                else
                {
                    this.Owner.IsGunLocked = false;
                }
                if(this.playerCanCarryMe && this.Owner != null)
                {
                    this.transform.position = this.Owner.sprite.WorldTopCenter + Vector2.down;
                }
                this.sprite.HeightOffGround = 0.075f;
                this.sprite.UpdateZDepth();
            }

            public bool OwnerAvailable()
            {
                if (this.Owner.Velocity.magnitude > 0.05f)
                {
                    return false;
                }
                float num = MinigunBehaviour.PlayerDetectRadius * MinigunBehaviour.PlayerDetectRadius;
                bool flag = MinigunBehaviour.PlayerDetectRadius < 0f;
                Vector2 vector = this.Owner.CenterPosition - this.transform.position.XY();
                if (!flag)
                {
                    flag = (vector.sqrMagnitude < num);
                }
                return flag;
            }

            public Vector2 GetAimPoint(PlayerController player)
            {
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(player.PlayerIDX);
                bool flag2 = instanceForPlayer == null;
                Vector2 a = Vector2.zero;
                if (!flag2)
                {
                    bool flag3 = instanceForPlayer.IsKeyboardAndMouse(false);
                    if (flag3)
                    {
                        a = player.unadjustedAimPoint.XY() - (base.transform.position.XY() + base.sprite.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.MiddleLeft).Rotate(base.transform.eulerAngles.z));
                    }
                    else
                    {
                        bool flag4 = instanceForPlayer.ActiveActions == null;
                        if (flag4)
                        {
                            return a;
                        }
                        a = instanceForPlayer.ActiveActions.Aim.Vector;
                    }
                }
                return a;
            }

            public IEnumerator HandleCooldown()
            {
                this.m_cooldownRemaining = this.CooldownTime;
                while (this.m_cooldownRemaining > 0)
                {
                    this.m_cooldownRemaining -= BraveTime.DeltaTime;
                    yield return null;
                }
                yield break;
            }

            public float CooldownTime;
            private float m_cooldownRemaining;
            public float AngleVariance;
            public Projectile ProjectileToShoot;
            public float ReloadTime;
            private float m_reloadRemaining;
            public int spriteId;
            public tk2dSpriteCollectionData collection;
            public string GunSwitchGroup;
            public PlayerController Owner;
            public string shootAnimation;
            public string reloadAnimation;
            public string idleAnimation;
            public float CurrentAngle = 0;
            public static float PlayerDetectRadius = 2f;
            private GameObject m_extantStand;
            public bool playerCanCarryMe;
        }
    }
}
