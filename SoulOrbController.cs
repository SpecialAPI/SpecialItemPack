using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using System.Reflection;
using Dungeonator;

namespace SpecialItemPack
{
    class SoulOrbController : AdvancedGunBehaviour
    {
        public SoulOrbController()
        {
            this.charges = 0;
        }

        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Soul Orb", "soul_orb");
            Game.Items.Rename("outdated_gun_mods:soul_orb", "spapi:soul_orb");
            SoulOrbController component = gun.gameObject.AddComponent<SoulOrbController>();
            GunExt.SetShortDescription(gun, "Contains SOUL");
            GunExt.SetLongDescription(gun, "Charges up. Firing consumes 3 charges and casts a powerful spell. Reloading swaps the spell and holding \"reload\" consumes 3 charge to heal the owner.\n\nSOUL is the essence of life in a far away kingdom. " +
                "It somehow works here too, even through gundead do not contain it.");
            GunExt.SetupSprite(gun, null, "soul_orb_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.SetBaseMaxAmmo(1);
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            Projectile oldProjectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(47) as Gun).DefaultModule.projectiles[0]);
            SoulProjectileBehaviour projectile = Toolbox.CopyFields<SoulProjectileBehaviour>(oldProjectile);
            Destroy(oldProjectile);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.transform.parent = gun.barrelOffset;
            projectile.AnimateProjectile(new List<string> {
                "vs_001",
                "vs_002",
                "vs_003",
                "vs_004",
                "vs_005"
            }, 16, true, new List<IntVector2> {
                new IntVector2(30, 12),
                new IntVector2(30, 12),
                new IntVector2(26, 12),
                new IntVector2(27, 14),
                new IntVector2(29, 12)
            }, Toolbox.ConstructListOfSameValues(true, 5), Toolbox.ConstructListOfSameValues(tk2dBaseSprite.Anchor.MiddleCenter, 5), Toolbox.ConstructListOfSameValues(true, 5), Toolbox.ConstructListOfSameValues(true, 5), 
            Toolbox.ConstructListOfSameValues<Vector3?>(null, 5), Toolbox.ConstructListOfSameValues<IntVector2?>(null, 5), Toolbox.ConstructListOfSameValues<IntVector2?>(null, 5), Toolbox.ConstructListOfSameValues<Projectile>(null, 5));
            gun.DefaultModule.cooldownTime = 0f;
            component.preventNormalFireAudio = true;
            component.overrideNormalFireAudio = "Play_WPN_bsg_shot_01";
            projectile.AdditionalScaleMultiplier = 2f;
            projectile.hitEffects = Toolbox.GetGunById(417).DefaultModule.projectiles[0].hitEffects;
            projectile.ignoreDamageCaps = true;
            projectile.baseData.damage = 100f;
            gun.InfiniteAmmo = true;
            gun.DefaultModule.numberOfShotsInClip = 1;
            gun.reloadTime = 0f;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "white";
            gun.quality = PickupObject.ItemQuality.S;
            gun.barrelOffset.transform.localPosition = new Vector3(1.7f, 0.65f, 0f);
            gun.encounterTrackable.EncounterGuid = "soul_orb";
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToTrorkShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(22);
            SoulFocusVFX = (PickupObjectDatabase.GetById(538) as SilverBulletsPassiveItem).SynergyPowerVFX;
        }

        

        protected override void OnPickedUpByPlayer(PlayerController player)
        {
            base.OnPickup(player);
            player.OnDealtDamageContext += this.CountUp;
        }

        public override void MidGameSerialize(List<object> data, int dataIndex)
        {
            base.MidGameSerialize(data, dataIndex);
            data.Add(this.charges);
            data.Add(this.damageToGoUp);
        }

        public override void InheritData(Gun source)
        {
            base.InheritData(source);
            SoulOrbController component = source.GetComponent<SoulOrbController>();
            if(component != null)
            {
                this.charges = component.charges;
                this.damageToGoUp = component.damageToGoUp;
            }
        }

        public override void MidGameDeserialize(List<object> data, ref int dataIndex)
        {
            base.MidGameDeserialize(data, ref dataIndex);
            this.charges = (int)data[dataIndex];
            this.damageToGoUp = (int)data[dataIndex + 1];
            dataIndex += 2;
        }

        protected override void Update()
        {
            base.Update();
            if(this.charges > this.chargesMax)
            {
                this.charges = this.chargesMax;
            }
            this.gun.idleAnimation = this.gun.UpdateAnimation("idle" + ((this.charges > 0) ? this.charges.ToString() : ""), null, false);
            this.gun.PlayIdleAnimation();
            bool isFocusing = false;
            if(this.Player != null)
            {
                BraveInput input = BraveInput.GetInstanceForPlayer(this.Player.PlayerIDX);
                if(input != null && input.ActiveActions != null)
                {
                    InControl.PlayerAction action = input.ActiveActions.ReloadAction;
                    if (action.IsPressed)
                    {
                        if(this.charges >= 3)
                        {
                            reloadHoldTime += BraveTime.DeltaTime;
                            if (reloadHoldTime >= 1.5f)
                            {
                                reloadHoldTime = 0f;
                                this.Owner.healthHaver.ApplyHealing(0.5f);
                                this.Owner.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Healing_Sparkles_001") as GameObject, Vector3.zero, true, false, false);
                                AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", base.gameObject);
                                this.charges -= 3;
                                Toolbox.GetGunById(417).DefaultModule.projectiles[0].hitEffects.HandleEnemyImpact(this.gun.sprite.WorldCenter, 0f, this.gun.transform, Vector3.zero, Vector2.zero, false, false);
                            }
                            else if (reloadHoldTime >= 0.5f)
                            {
                                this.Owner.PlayEffectOnActor(SoulFocusVFX, new Vector3(0f, -0.5f, 0f), true, false, false);
                            }
                            if (reloadHoldTime >= 0.1f)
                            {
                                isFocusing = true;
                            }
                        }
                    }
                    if(!action.IsPressed && actionWasPressed)
                    {
                        if(reloadHoldTime <= 0.1f)
                        {
                            //Do stuff.
                        }
                        reloadHoldTime = 0f;
                    }
                    this.actionWasPressed = action.IsPressed;
                }
            }
            if (this.cooldownTimer > 0f)
            {
                this.cooldownTimer -= BraveTime.DeltaTime;
            }
            this.gun.RuntimeModuleData[this.gun.DefaultModule].onCooldown = this.cooldownTimer > 0 || isFocusing || charges < 3;
        }

        private void CountUp(PlayerController player, float damage, bool fatal, HealthHaver victim)
        {
            if(victim == null)
            {
                return;
            }
            if(this.charges < this.chargesMax)
            {
                this.damageToGoUp += damage;
                if (this.damageToGoUp >= this.damageNeededToGoUp)
                {
                    this.damageToGoUp = 0;
                    this.charges += 1;
                    if(this.Owner.CurrentGun == this.gun)
                    {
                        Toolbox.GetGunById(417).DefaultModule.projectiles[0].hitEffects.HandleEnemyImpact(this.gun.sprite.WorldCenter, 0f, this.gun.transform, Vector3.zero, Vector2.zero, false, false);
                    }
                }
            }
        }

        protected override void OnPostDroppedByPlayer(PlayerController player)
        {
            player.OnDealtDamageContext -= this.CountUp;
            base.OnPostDrop(player);
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            this.gun.ClipShotsRemaining = 2;
            this.cooldownTimer = 1.5f;
            this.charges -= 3;
            this.gun.RuntimeModuleData[this.gun.DefaultModule].onCooldown = true;
        }

        private int charges;
        private readonly int chargesMax = 9;
        private float damageToGoUp;
        private readonly float damageNeededToGoUp = 50f;
        private bool actionWasPressed = false;
        private float reloadHoldTime = 0f;
        public static GameObject SoulFocusVFX;
        private float cooldownTimer = 0f;
        private class SoulProjectileBehaviour : Projectile
        {
            public override void Start()
            {
                base.Start();
                base.OwnerName = "Soul Orb Projectile";
            }

            protected override HandleDamageResult HandleDamage(SpeculativeRigidbody rigidbody, PixelCollider hitPixelCollider, out bool killedTarget, PlayerController player, bool alreadyPlayerDelayed = false)
            {
                killedTarget = false;
                if (rigidbody.ReflectProjectiles)
                {
                    return Projectile.HandleDamageResult.NO_HEALTH;
                }
                if (!rigidbody.healthHaver)
                {
                    return Projectile.HandleDamageResult.NO_HEALTH;
                }
                if (!alreadyPlayerDelayed && Projectile.s_delayPlayerDamage && player)
                {
                    return Projectile.HandleDamageResult.HEALTH;
                }
                if (rigidbody.spriteAnimator != null && rigidbody.spriteAnimator.QueryInvulnerabilityFrame())
                {
                    return Projectile.HandleDamageResult.HEALTH;
                }
                bool flag = !rigidbody.healthHaver.IsDead;
                float num = this.ModifiedDamage;
                if (this.Owner is AIActor && rigidbody && rigidbody.aiActor && (this.Owner as AIActor).IsNormalEnemy)
                {
                    num = ProjectileData.FixedFallbackDamageToEnemies;
                    if (rigidbody.aiActor.HitByEnemyBullets)
                    {
                        num /= 4f;
                    }
                }
                FieldInfo info = typeof(Projectile).GetField("m_healthHaverHitCount", BindingFlags.NonPublic | BindingFlags.Instance);
                int m_healthHaverHitCount = (int)info.GetValue(this as Projectile);
                if (this.Owner is PlayerController && this.m_hasPierced && m_healthHaverHitCount >= 1)
                {
                    int num2 = Mathf.Clamp(m_healthHaverHitCount - 1, 0, GameManager.Instance.PierceDamageScaling.Length - 1);
                    num *= GameManager.Instance.PierceDamageScaling[num2];
                }
                if (this.OnWillKillEnemy != null && num >= rigidbody.healthHaver.GetCurrentHealth())
                {
                    this.OnWillKillEnemy(this, rigidbody);
                }
                if (rigidbody.healthHaver.IsBoss)
                {
                    num *= this.BossDamageMultiplier;
                }
                if (this.BlackPhantomDamageMultiplier != 1f && rigidbody.aiActor && rigidbody.aiActor.IsBlackPhantom)
                {
                    num *= this.BlackPhantomDamageMultiplier;
                }
                bool flag2 = false;
                if (this.DelayedDamageToExploders)
                {
                    flag2 = (rigidbody.GetComponent<ExplodeOnDeath>() && rigidbody.healthHaver.GetCurrentHealth() <= num);
                }
                if (!flag2)
                {
                    HealthHaver healthHaver = rigidbody.healthHaver;
                    float damage = num;
                    Vector2 velocity = base.specRigidbody.Velocity;
                    CoreDamageTypes coreDamageTypes = this.damageTypes;
                    DamageCategory damageCategory = (!this.IsBlackBullet) ? DamageCategory.Normal : DamageCategory.BlackBullet;
                    if (healthHaver.IsBoss)
                    {
                        damage *= (base.Owner as PlayerController).stats.GetStatValue(PlayerStats.StatType.DamageToBosses);
                    }
                    healthHaver.ApplyDamage(damage, velocity, "Soul Orb Projectile", coreDamageTypes, damageCategory, false, hitPixelCollider, this.ignoreDamageCaps);
                    if (player && player.OnHitByProjectile != null)
                    {
                        player.OnHitByProjectile(this, player);
                    }
                }
                else
                {
                    rigidbody.StartCoroutine(this.HandleDelayedDamage(rigidbody, num, base.specRigidbody.Velocity, hitPixelCollider));
                }
                if (this.Owner && this.Owner is AIActor && player)
                {
                    (this.Owner as AIActor).HasDamagedPlayer = true;
                }
                killedTarget = (flag && rigidbody.healthHaver.IsDead);
                if (!killedTarget && rigidbody.gameActor != null)
                {
                    if (this.AppliesPoison && UnityEngine.Random.value < this.PoisonApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(this.healthEffect, 1f, null);
                    }
                    if (this.AppliesSpeedModifier && UnityEngine.Random.value < this.SpeedApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(this.speedEffect, 1f, null);
                    }
                    if (this.AppliesCharm && UnityEngine.Random.value < this.CharmApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(this.charmEffect, 1f, null);
                    }
                    if (this.AppliesFreeze && UnityEngine.Random.value < this.FreezeApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(this.freezeEffect, 1f, null);
                    }
                    if (this.AppliesCheese && UnityEngine.Random.value < this.CheeseApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(this.cheeseEffect, 1f, null);
                    }
                    if (this.AppliesBleed && UnityEngine.Random.value < this.BleedApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(this.bleedEffect, -1f, this);
                    }
                    if (this.AppliesFire && UnityEngine.Random.value < this.FireApplyChance)
                    {
                        rigidbody.gameActor.ApplyEffect(this.fireEffect, 1f, null);
                    }
                    if (this.AppliesStun && UnityEngine.Random.value < this.StunApplyChance && rigidbody.gameActor.behaviorSpeculator)
                    {
                        rigidbody.gameActor.behaviorSpeculator.Stun(this.AppliedStunDuration, true);
                    }
                    for (int i = 0; i < this.statusEffectsToApply.Count; i++)
                    {
                        rigidbody.gameActor.ApplyEffect(this.statusEffectsToApply[i], 1f, null);
                    }
                }
                m_healthHaverHitCount++;
                info.SetValue(this as Projectile, m_healthHaverHitCount);
                return (!killedTarget) ? Projectile.HandleDamageResult.HEALTH : Projectile.HandleDamageResult.HEALTH_AND_KILLED;
            }

            private IEnumerator HandleDelayedDamage(SpeculativeRigidbody targetRigidbody, float damage, Vector2 damageVec, PixelCollider hitPixelCollider)
            {
                yield return new WaitForSeconds(0.5f);
                if (targetRigidbody && targetRigidbody.healthHaver)
                {
                    HealthHaver healthHaver = targetRigidbody.healthHaver;
                    string ownerName = this.OwnerName;
                    CoreDamageTypes coreDamageTypes = this.damageTypes;
                    DamageCategory damageCategory = (!this.IsBlackBullet) ? DamageCategory.Normal : DamageCategory.BlackBullet;
                    healthHaver.ApplyDamage(damage, damageVec, ownerName, coreDamageTypes, damageCategory, false, hitPixelCollider, this.ignoreDamageCaps);
                }
                yield break;
            }
        }
    }
}