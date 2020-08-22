using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;

namespace SpecialItemPack
{
    class StormEyeController : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Eye of the Storm", "eye");
            Game.Items.Rename("outdated_gun_mods:eye_of_the_storm", "spapi:eye_of_the_storm");
            gun.gameObject.AddComponent<StormEyeController>();
            GunExt.SetShortDescription(gun, "Legend of the Gungeon");
            GunExt.SetLongDescription(gun, "Legendary weapon, made using a framgent of the great bullet. Shoots powerful electrical bolts, and if they collide...\n\nGun from the past, now existing only as a clip, still of great power." +
                " Because of the overuse of the colliding, the gun collapsed, unleashing a great storm to the Gungeon.\n\nBeing made from the metal of the great bullet, from which all the gundead came, it will ignore any limitations and will hurt your " +
                "enemies without any mercy.");
            GunExt.SetupSprite(gun, null, "eye_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 32);
            GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 16);
            for(int i=0; i<3; i++)
            {
                GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            }
            bool upOrDown = false;
            Projectile lastProj = null;
            foreach(ProjectileModule mod in gun.Volley.projectiles)
            {
                mod.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
                mod.angleVariance = 0;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(47) as Gun).DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                mod.projectiles[0] = projectile;
                projectile.transform.parent = gun.barrelOffset;
                projectile.baseData.damage = 7f;
                mod.cooldownTime = 0.6f;
                if (mod != gun.DefaultModule)
                {
                    mod.ammoCost = 0;
                    upOrDown = !upOrDown;
                }
                PierceProjModifier penetrateMod = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
                penetrateMod.penetratesBreakables = true;
                penetrateMod.penetration = 999;
                penetrateMod.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
                penetrateMod.preventPenetrationOfActors = false;
                projectile.ignoreDamageCaps = true;
                projectile.hitEffects = (PickupObjectDatabase.GetById(38) as Gun).DefaultModule.projectiles[0].hitEffects;
                lastProj = projectile;
                mod.ammoType = GameUIAmmoType.AmmoType.BLUE_SHOTGUN;
                mod.customAmmoType = (PickupObjectDatabase.GetById(156) as Gun).DefaultModule.customAmmoType;
                mod.numberOfShotsInClip = 10;
                projectile.name = "StormEye_Secondary_Projectile";
            }
            Gun gun2 = (Gun)ETGMod.Databases.Items["plasma_cell_gun"];
            gun.DefaultModule.customAmmoType = gun2.DefaultModule.customAmmoType;
            gun.Volley.projectiles[0].projectiles[0].SetProjectileSpriteRight("eye_proj_up_001", 8, 5);
            gun.Volley.projectiles[1].projectiles[0].SetProjectileSpriteRight("eye_proj_down_001", 8, 5);
            gun.Volley.projectiles[2].projectiles[0].SetProjectileSpriteRight("eye_proj_middle_001", 9, 5);
            lastProj.HasDefaultTint = true;
            lastProj.damageTypes = CoreDamageTypes.Electric;
            lastProj.name = "StormEye_Main_Projectile";
            gun.reloadTime = 2f;
            gun.SetBaseMaxAmmo(100);
            gun.quality = PickupObject.ItemQuality.S;
            gun.encounterTrackable.EncounterGuid = "eye_of_the_storm";
            gun.gunClass = GunClass.RIFLE;
            gun.barrelOffset.transform.localPosition = new Vector3(2.05f, 0.80f, 0f);
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            StormEyeController.goopDefs = new List<GoopDefinition>();
            foreach (string text in StormEyeController.goops)
            {
                GoopDefinition goopDefinition;
                try
                {
                    GameObject gameObject2 = assetBundle.LoadAsset(text) as GameObject;
                    goopDefinition = gameObject2.GetComponent<GoopDefinition>();
                }
                catch
                {
                    goopDefinition = (assetBundle.LoadAsset(text) as GoopDefinition);
                }
                goopDefinition.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
                StormEyeController.goopDefs.Add(goopDefinition);
            }
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(13);
            gun.SetupUnlockOnFlag(GungeonFlags.ACHIEVEMENT_CONSTRUCT_BULLET, true);
        }

        public override void OnInitializedWithOwner(GameActor actor)
        {
            base.OnInitializedWithOwner(actor);
            if(actor is PlayerController)
            {
                GameManager.Instance.OnNewLevelFullyLoaded += this.RefillAmmo;
            }
        }

        public void RefillAmmo()
        {
            this.gun.GainAmmo(this.gun.AdjustedMaxAmmo);
        }

        public override void OnDropped()
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= this.RefillAmmo;
            base.OnDropped();
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            float length = 0;
            BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer((this.gun.CurrentOwner as PlayerController).PlayerIDX);
            bool flag2 = instanceForPlayer == null;
            if (!flag2)
            {
                bool flag3 = instanceForPlayer.IsKeyboardAndMouse(false);
                if (flag3)
                {
                    Vector2 a;
                    a = (this.gun.CurrentOwner as PlayerController).unadjustedAimPoint.XY();
                    a.Normalize();
                    length = Vector2.Distance(projectile.sprite.WorldCenter, a) * 8f;
                    float magnitude = (projectile.sprite.WorldCenter - a).magnitude;
                }
                else
                {
                    length = 8f;
                }
            }
            bool isMiddleProj = false;
            if (this.lastUp == null)
            {
                projectile.OverrideMotionModule = new HelixProjectileMotionModule
                {
                    helixAmplitude = 2f,
                    helixWavelength = 8f,
                    ForceInvert = false
                };
                projectile.gameObject.AddComponent<EyeProjUp>();
                this.lastUp = projectile;
            }
            else if(this.lastDown == null)
            {
                projectile.OverrideMotionModule = new HelixProjectileMotionModule
                {
                    helixAmplitude = 2f,
                    helixWavelength = 8f,
                    ForceInvert = true
                };
                projectile.gameObject.AddComponent<EyeProjDown>();
                this.lastDown = projectile;
            }
            else
            {
                projectile.gameObject.AddComponent<EyeProjMain>().boundUp = this.lastUp;
                projectile.gameObject.GetComponent<EyeProjMain>().boundDown = this.lastDown;
                projectile.gameObject.GetComponent<EyeProjMain>().ownerHasThunderstormSynergy = this.gun.CurrentOwner is PlayerController && (this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("#GUNDERSTORM");
                if(this.gun.CurrentOwner != null && this.gun.CurrentOwner is PlayerController)
                {
                    projectile.gameObject.GetComponent<EyeProjMain>().divider = (this.gun.CurrentOwner as PlayerController).stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
                }
                this.lastUp = null;
                this.lastDown = null;
                isMiddleProj = true;
            }
            if (this.gun.CurrentOwner is PlayerController && (this.gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy("#GUNDERSTORM"))
            {
                ComplexProjectileModifier shockRounds = PickupObjectDatabase.GetById(298) as ComplexProjectileModifier;
                ChainLightningModifier orAddComponent = projectile.gameObject.GetOrAddComponent<ChainLightningModifier>();
                orAddComponent.LinkVFXPrefab = shockRounds.ChainLightningVFX;
                orAddComponent.damageTypes = shockRounds.ChainLightningDamageTypes;
                orAddComponent.maximumLinkDistance = shockRounds.ChainLightningMaxLinkDistance;
                orAddComponent.damagePerHit = shockRounds.ChainLightningDamagePerHit;
                orAddComponent.damageCooldown = shockRounds.ChainLightningDamageCooldown;
                if (isMiddleProj)
                {
                    orAddComponent.maximumLinkDistance *= 3f;
                    orAddComponent.CanChainToAnyProjectile = true;
                }
                if (shockRounds.ChainLightningDispersalParticles != null)
                {
                    orAddComponent.UsesDispersalParticles = true;
                    orAddComponent.DispersalParticleSystemPrefab = shockRounds.ChainLightningDispersalParticles;
                    orAddComponent.DispersalDensity = shockRounds.ChainLightningDispersalDensity;
                    orAddComponent.DispersalMinCoherency = shockRounds.ChainLightningDispersalMinCoherence;
                    orAddComponent.DispersalMaxCoherency = shockRounds.ChainLightningDispersalMaxCoherence;
                }
                else
                {
                    orAddComponent.UsesDispersalParticles = false;
                }
            }
        }

        private void Update()
        {
            if (!this.gun.PreventNormalFireAudio)
            {
                this.gun.PreventNormalFireAudio = true;
            }
            if(this.gun.CurrentOwner != null && this.gun.CurrentOwner is PlayerController)
            {
                lastOwner = this.gun.CurrentOwner as PlayerController;
                if(this.electricityImmunity == null)
                {
                    this.electricityImmunity = new DamageTypeModifier
                    {
                        damageMultiplier = 0,
                        damageType = CoreDamageTypes.Electric
                    };
                    this.lastOwner.healthHaver.damageTypeModifiers.Add(this.electricityImmunity);
                }
            }
            else
            {
                if (this.electricityImmunity != null)
                {
                    this.lastOwner.healthHaver.damageTypeModifiers.Remove(this.electricityImmunity);
                    this.electricityImmunity = null;
                }
            }
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            AkSoundEngine.PostEvent("Play_WPN_spellactionrevolver_shot_01", base.gameObject);
        }

        private PlayerController lastOwner = null;
        private DamageTypeModifier electricityImmunity = null;
        private Projectile lastDown = null;
        private Projectile lastUp = null;

        private static string[] goops = new string[]
        {
            "assets/data/goops/water goop.asset"
        };

        private static List<GoopDefinition> goopDefs;

        private class EyeProjUp : MonoBehaviour
        {  
        }

        private class EyeProjDown : MonoBehaviour
        {
        }

        private class EyeProjMain : MonoBehaviour
        {
            public void Start()
            {
                this._proj = base.GetComponent<Projectile>();
                this._player = (this._proj.Owner as PlayerController);
                Projectile proj = this._proj;
                this.immuneToCollision = true;
                proj.collidesWithProjectiles = true;
                proj.collidesWithEnemies = true;
                GameManager.Instance.StartCoroutine(this.JunkCoroutine());
            }

            private void Awake()
            {
                this._proj = base.GetComponent<Projectile>();
                Projectile proj = this._proj;
                SpeculativeRigidbody specRigidbody = this._proj.specRigidbody;
                proj.collidesWithProjectiles = true;
                this.immuneToCollision = false;
                specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandlePreCollision));
            }

            private void HandlePreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
            {
                if (otherRigidbody && otherRigidbody.projectile)
                {
                    if (otherRigidbody.projectile.Owner is PlayerController)
                    {
                        Projectile projectile = otherRigidbody.projectile;
                        if (projectile.gameObject.GetComponent<EyeProjUp>() != null || projectile.gameObject.GetComponent<EyeProjDown>() != null)
                        {
                            if (!this.immuneToCollision)
                            {
                                Gun voidCannon = (PickupObjectDatabase.GetById(593) as Gun);
                                Gun heroine = (PickupObjectDatabase.GetById(41) as Gun);
                                ExplosionData data0 = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
                                ExplosionData data = new ExplosionData
                                {
                                    doDamage = true,
                                    damageRadius = this.ownerHasThunderstormSynergy ? 5f : 3.75f,
                                    damageToPlayer = 0,
                                    damage = 25,
                                    breakSecretWalls = true,
                                    secretWallsRadius = this.ownerHasThunderstormSynergy ? 4.75f : 3.25f,
                                    doDestroyProjectiles = true,
                                    doForce = true,
                                    pushRadius = this.ownerHasThunderstormSynergy ? 4.75f : 3.25f,
                                    force = data0.force,
                                    debrisForce = data0.debrisForce,
                                    preventPlayerForce = true,
                                    explosionDelay = data0.explosionDelay,
                                    usesComprehensiveDelay = data0.usesComprehensiveDelay,
                                    comprehensiveDelay = data0.comprehensiveDelay,
                                    effect = data0.effect,
                                    doScreenShake = true,
                                    ss = data0.ss,
                                    doStickyFriction = data0.doStickyFriction,
                                    doExplosionRing = data0.doExplosionRing,
                                    isFreezeExplosion = false,
                                    freezeEffect = null,
                                    playDefaultSFX = true,
                                    IsChandelierExplosion = false,
                                    rotateEffectToNormal = data0.rotateEffectToNormal,
                                    ignoreList = new List<SpeculativeRigidbody>(),
                                    overrideRangeIndicatorEffect = data0.overrideRangeIndicatorEffect
                                };
                                if(voidCannon.DefaultModule.projectiles[0].GetComponent<ExplosiveModifier>() != null)
                                {
                                    data.effect = voidCannon.DefaultModule.projectiles[0].GetComponent<ExplosiveModifier>().explosionData.effect;
                                }
                                Exploder.Explode(this._proj.sprite.sprite.WorldCenter, data, new Vector2(), null, false, CoreDamageTypes.Electric, true);
                                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(StormEyeController.goopDefs[0]).AddGoopCircle(this._proj.sprite.WorldCenter, this.ownerHasThunderstormSynergy ? 5f : 3.75f);
                                for (int i = 0; i < StaticReferenceManager.AllGoops.Count; i++)
                                {
                                    StaticReferenceManager.AllGoops[i].ElectrifyGoopCircle(this._proj.sprite.WorldBottomCenter, 1f);
                                }
                                projectile.DieInAir(true);
                                this._proj.DieInAir(true);
                                if (this.boundUp != null)
                                {
                                    this.boundUp.DieInAir(true);
                                }
                                if (this.boundDown != null)
                                {
                                    this.boundDown.DieInAir(true);
                                }
                            }
                        }
                    }
                    PhysicsEngine.SkipCollision = true;
                }
            }

            private IEnumerator JunkCoroutine()
            {
                yield return new WaitForSeconds(0.2f / divider);
                this.immuneToCollision = false;
                yield break;
            }

            private Projectile _proj;
            private PlayerController _player;
            private bool immuneToCollision;
            public Projectile boundUp;
            public Projectile boundDown;
            public bool ownerHasThunderstormSynergy = false;
            public float divider = 1;
        }
    }
}
