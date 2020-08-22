using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using Dungeonator;
using SpecialItemPack.AdaptedSynergyStuff;

namespace SpecialItemPack
{
    class SuperReapersScythe : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Scythe of the Jammed", "jamm_scythe");
            Game.Items.Rename("outdated_gun_mods:scythe_of_the_jammed", "spapi:scythe_of_the_jammed");
            gun.gameObject.AddComponent<SuperReapersScythe>();
            GunExt.SetShortDescription(gun, "Jammed's Favorite Gun");
            GunExt.SetLongDescription(gun, "Shoots a lot of bullets. Grants flight.\n\nThis half-scythe-half-gun is the weapon of the Jammed himself. Legends say this weapon provides the powers of the Jammed to anyone when held...");
            GunExt.SetupSprite(gun, null, "jamm_scythe_idle_001", 8);
            for (int i = 0; i < 12; i++)
            {
                GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            }
            int g = 0;
            foreach (ProjectileModule mod in gun.Volley.projectiles)
            {
                mod.shootStyle = ProjectileModule.ShootStyle.Charged;
                Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(51) as Gun).DefaultModule.projectiles[0]);
                projectile.gameObject.SetActive(false);
                FakePrefab.MarkAsFakePrefab(projectile.gameObject);
                UnityEngine.Object.DontDestroyOnLoad(projectile);
                mod.projectiles[0] = projectile;
                projectile.transform.parent = gun.barrelOffset;
                mod.angleVariance = 0f;
                projectile.sprite.usesOverrideMaterial = true;
                projectile.sprite.renderer.material.SetFloat("_BlackBullet", 1f);
                projectile.sprite.renderer.material.SetFloat("_EmissivePower", -40f);
                projectile.gameObject.name = "Jamm_Scythe_Projectile";
                projectile.ignoreDamageCaps = true;
                projectile.baseData.range = float.MaxValue;
                projectile.baseData.damage = 12.5f;
                mod.angleFromAim = g * 30;
                mod.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
                ProjectileModule.ChargeProjectile chargeProj = new ProjectileModule.ChargeProjectile
                {
                    Projectile = projectile,
                    ChargeTime = 1f
                };
                mod.chargeProjectiles = new List<ProjectileModule.ChargeProjectile> { chargeProj };
                if (mod != gun.DefaultModule)
                {
                    mod.ammoCost = 0;
                }
                mod.cooldownTime = 0.35f;
                mod.numberOfShotsInClip = 5;
                g++;
            }
            AdvancedHoveringGunSynergyProcessor processor = gun.gameObject.AddComponent<AdvancedHoveringGunSynergyProcessor>();
            processor.RequiredSynergy = "#MINE_TOO!";
            processor.TargetGunID = 365;
            processor.UsesMultipleGuns = false;
            processor.PositionType = HoveringGunController.HoverPosition.OVERHEAD;
            processor.AimType = HoveringGunController.AimType.PLAYER_AIM;
            processor.FireType = HoveringGunController.FireType.ON_RELOAD;
            processor.FireCooldown = 1.35f;
            processor.FireDuration = 0f;
            processor.OnlyOnEmptyReload = false;
            processor.ShootAudioEvent = "";
            processor.OnEveryShotAudioEvent = "";
            processor.FinishedShootingAudioEvent = "";
            processor.Trigger = AdvancedHoveringGunSynergyProcessor.TriggerStyle.CONSTANT;
            processor.NumToTrigger = 1;
            processor.TriggerDuration = 0f;
            processor.ConsumesTargetGunAmmo = false;
            processor.ChanceToConsumeTargetGunAmmo = 0f;
            gun.reloadTime = 1.35f;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation).loopStart = 9;
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventAudio = "Play_ENM_cannonball_launch_01";
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].triggerEvent = true;
            gun.GetComponent<tk2dSpriteAnimator>().MuteAudio = false;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.S;
            gun.barrelOffset.transform.localPosition = new Vector3(1.7f, 0.65f, 0f);
            gun.encounterTrackable.EncounterGuid = "jamm_scythe";
            gun.gunClass = GunClass.CHARGE;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(336);
            gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[0].eventInfo = "damageEnemies";
            for (int i = 0; i < EncounterDatabase.Instance.Entries.Count; i++)
            {
                if (EncounterDatabase.Instance.Entries[i].journalData.PrimaryDisplayName == "#SREAPER_ENCNAME")
                {
                    gun.SetupUnlockOnEncounter(EncounterDatabase.Instance.Entries[i].myGuid, DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN, 0);
                    break;
                }
            }
        }

        public override void Start()
        {
            base.Start();
            gun.GetComponent<tk2dSpriteAnimator>().AnimationEventTriggered += this.AnimationEventTriggered;
        }

        public void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameIdx)
        {
            if(clip.GetFrame(frameIdx).eventInfo == "damageEnemies" && this.Player != null && this.Player.PlayerHasActiveSynergy("#GRIM_SUPERREAPER"))
            {
                GameObject obj = SpawnManager.SpawnVFX(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, gun.sprite.WorldCenter, Quaternion.identity);
                obj.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(gun.sprite.WorldCenter, tk2dBaseSprite.Anchor.LowerCenter);
                obj.GetComponent<tk2dBaseSprite>().renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
                GameManager.Instance.StartCoroutine(this.DelayedDamageToEnemy());
            }
        }

        public IEnumerator DelayedDamageToEnemy()
        {
            yield return new WaitForSeconds(0.5f);
            if (this.gun != null && this.Player != null && this.Player.CurrentRoom != null && this.Player.CurrentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.All))
            {
                AIActor aiactor = this.Player.CurrentRoom.GetRandomActiveEnemy(true);
                if(aiactor != null)
                {
                    aiactor.healthHaver.ApplyDamage(aiactor.IsBlackPhantom ? 30f : 10f, Vector2.zero, aiactor.IsBlackPhantom ? "The Curse" : "The Jammed's Curse", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                    aiactor.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero, true, false, false);
                }
            }
            yield break;
        }

        protected override void OnPickedUpByPlayer(PlayerController player)
        {
            base.OnPickup(player);
            player.GunChanged += this.OnGunChanged;
            this.CheckGunFlightStatus(player.CurrentGun);
        }

        private void OnGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            this.CheckGunFlightStatus(newGun);
        }

        private void CheckGunFlightStatus(Gun current)
        {
            if(current == this.gun && !this.GaveFlight)
            {
                this.Player.SetIsFlying(true, "reaper", true, false);
                this.Player.AdditionalCanDodgeRollWhileFlying.AddOverride("reaper");
                this.Player.SetOverrideShader(ShaderCache.Acquire("Brave/LitCutoutUberPhantom"));
                this.GaveFlight = true;
            }
            else if(current != this.gun && this.GaveFlight)
            {
                this.Player.SetIsFlying(false, "reaper", true, false);
                this.Player.AdditionalCanDodgeRollWhileFlying.RemoveOverride("reaper");
                this.Player.ClearOverrideShader();
                this.GaveFlight = false;
            }
        }

        protected override void Update()
        {
            if (this.PickedUp && this.Player != null)
            {
                if (GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.LOW && GameManager.Options.ShaderQuality != GameOptions.GenericHighMedLowOption.VERY_LOW)
                {
                    Vector3 vector = this.Owner.sprite.WorldBottomLeft.ToVector3ZisY(0f);
                    Vector3 vector2 = this.Owner.sprite.WorldTopRight.ToVector3ZisY(0f);
                    float num = (vector2.y - vector.y) * (vector2.x - vector.x);
                    float num2 = 50f * num;
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
                if (GameManager.Instance != null && GameManager.Instance.Dungeon != null && !GameManager.Instance.Dungeon.CurseReaperActive)
                {
                    GameManager.Instance.Dungeon.CurseReaperActive = true;
                }
                if (SuperReaperController.Instance != null && SuperReaperController.Instance.gameObject != null)
                {
                    Destroy(SuperReaperController.Instance.gameObject);
                }
            }
            if (!this.gun.PreventNormalFireAudio)
            {
                this.gun.PreventNormalFireAudio = true;
            }
            if (!this.gun.IsReloading && !this.HasReloaded)
            {
                this.HasReloaded = true;
            }
            base.Update();
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            base.OnReloadPressed(player, gun, bSOMETHING);
            if(gun.IsReloading && this.HasReloaded)
            {
                AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                if(gun.ClipShotsRemaining <= 0)
                {
                    GameObject obj = SpawnManager.SpawnVFX(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, gun.sprite.WorldCenter, Quaternion.identity);
                    obj.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(gun.sprite.WorldCenter, tk2dBaseSprite.Anchor.LowerCenter);
                    obj.GetComponent<tk2dBaseSprite>().renderer.material.shader = ShaderCache.Acquire("Brave/LitCutoutUberPhantom");
                    GameManager.Instance.StartCoroutine(this.DelayedDamageToEnemies());
                }
                else
                {
                    AkSoundEngine.PostEvent("Play_ENM_beholster_teleport_02", base.gameObject);
                }
                this.HasReloaded = false;
            }
        }

        public IEnumerator DelayedDamageToEnemies()
        {
            yield return new WaitForSeconds(0.5f);
            if(this.gun != null && this.Player != null && this.Player.CurrentRoom != null && this.Player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
            {
                foreach(AIActor aiactor in this.Player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                {
                    if(aiactor != null && aiactor.healthHaver != null)
                    {
                        aiactor.healthHaver.ApplyDamage(aiactor.IsBlackPhantom ? 30f : 10f, Vector2.zero, aiactor.IsBlackPhantom ? "The Curse" : "The Jammed's Curse", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                        aiactor.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero, true, false, false);
                    }
                }
            }
            yield break;
        }

        protected override void OnPostDroppedByPlayer(PlayerController player)
        {
            base.OnPostDrop(player);
            player.GunChanged -= this.OnGunChanged;
            this.CheckGunFlightStatus(player.CurrentGun);
            if(GameManager.Instance != null && GameManager.Instance.Dungeon != null)
            {
                GameManager.Instance.Dungeon.CurseReaperActive = false;
            }
            player.stats.RecalculateStats(player, true, false);
        }

        private bool HasReloaded = true;
        private bool GaveFlight = false;
    }
}