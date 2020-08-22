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
    class SequencerController : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("The Sequencer", "sequencer");
            gun.idleAnimation = "sequencer1_idle";
            gun.shootAnimation = "sequencer1_fire";
            Game.Items.Rename("outdated_gun_mods:the_sequencer", "spapi:the_sequencer");
            SequencerController controller = gun.gameObject.AddComponent<SequencerController>();
            GunExt.SetShortDescription(gun, "Change Your Mind");
            GunExt.SetLongDescription(gun, "A worn gun with an enchanted chamber. It's enchantment makes the gun change it's mood (as well as projectiles) very quickly. Like after every shot.\n\nThis gun was once worn by a famous gungeoneer and handcrafted " +
                "by him too. At first it was a normal gun, but a gunjurer accidentaly threw a magical spell on the chamber, enchanting it with the elements.");
            GunExt.SetupSprite(gun, null, "sequencer1_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 16);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.cooldownTime = 0.50f;
            gun.DefaultModule.angleVariance = 0;
            //setting up fire proj.
            Projectile fireProjectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            fireProjectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(fireProjectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(fireProjectile);
            fireProjectile.transform.parent = gun.barrelOffset;
            fireProjectile.baseData.damage = 20f;
            fireProjectile.baseData.force /= 2f;
            fireProjectile.baseData.range = 9999;
            fireProjectile.DefaultTintColor = Color.red;
            fireProjectile.HasDefaultTint = true;
            fireProjectile.AppliesFire = true;
            fireProjectile.FireApplyChance = 100;
            fireProjectile.fireEffect = (PickupObjectDatabase.GetById(295) as BulletStatusEffectItem).FireModifierEffect;
            fireProjectile.damageTypes = CoreDamageTypes.Fire;
            fireProjectile.name = "Sequencer_FireProjectile";
            SequencerController.replacementProjectiles.Add(fireProjectile);
            //setting up poison proj;
            Projectile poisonProjectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            poisonProjectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(poisonProjectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(poisonProjectile);
            poisonProjectile.transform.parent = gun.barrelOffset;
            poisonProjectile.baseData.damage = 20f;
            poisonProjectile.baseData.force /= 2f;
            poisonProjectile.baseData.range = 9999;
            poisonProjectile.DefaultTintColor = Color.green;
            poisonProjectile.HasDefaultTint = true;
            poisonProjectile.AppliesPoison = true;
            poisonProjectile.PoisonApplyChance = 100;
            poisonProjectile.healthEffect = (PickupObjectDatabase.GetById(204) as BulletStatusEffectItem).HealthModifierEffect;
            poisonProjectile.damageTypes = CoreDamageTypes.Poison;
            poisonProjectile.name = "Sequencer_PoisonProjectile";
            SequencerController.replacementProjectiles.Add(poisonProjectile);
            //setting up electric proj;
            Projectile electricProjectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            electricProjectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(electricProjectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(electricProjectile);
            electricProjectile.transform.parent = gun.barrelOffset;
            electricProjectile.baseData.damage = 28f;
            electricProjectile.baseData.force /= 2f;
            electricProjectile.baseData.range = 9999;
            electricProjectile.DefaultTintColor = Color.blue;
            electricProjectile.HasDefaultTint = true;
            ComplexProjectileModifier chainBullets = PickupObjectDatabase.GetById(298) as ComplexProjectileModifier;
            ChainLightningModifier orAddComponent = electricProjectile.gameObject.GetOrAddComponent<ChainLightningModifier>();
            orAddComponent.LinkVFXPrefab = chainBullets.ChainLightningVFX;
            orAddComponent.damageTypes = chainBullets.ChainLightningDamageTypes;
            orAddComponent.maximumLinkDistance = chainBullets.ChainLightningMaxLinkDistance;
            orAddComponent.damagePerHit = chainBullets.ChainLightningDamagePerHit;
            orAddComponent.damageCooldown = chainBullets.ChainLightningDamageCooldown;
            if (chainBullets.ChainLightningDispersalParticles != null)
            {
                orAddComponent.UsesDispersalParticles = true;
                orAddComponent.DispersalParticleSystemPrefab = chainBullets.ChainLightningDispersalParticles;
                orAddComponent.DispersalDensity = chainBullets.ChainLightningDispersalDensity;
                orAddComponent.DispersalMinCoherency = chainBullets.ChainLightningDispersalMinCoherence;
                orAddComponent.DispersalMaxCoherency = chainBullets.ChainLightningDispersalMaxCoherence;
            }
            else
            {
                orAddComponent.UsesDispersalParticles = false;
            }
            electricProjectile.OnDestruction += controller.AddElectricGoop;
            electricProjectile.damageTypes = CoreDamageTypes.Electric;
            electricProjectile.name = "Sequencer_ElectricProjectile";
            SequencerController.replacementProjectiles.Add(electricProjectile);
            gun.reloadTime = 1f;
            gun.DefaultModule.numberOfShotsInClip = 3;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.SetBaseMaxAmmo(200);
            gun.DefaultModule.projectiles = new List<Projectile> { fireProjectile };
            gun.quality = PickupObject.ItemQuality.S;
            gun.encounterTrackable.EncounterGuid = "sequencer_gun";
            gun.gunClass = GunClass.PISTOL;
            gun.barrelOffset.transform.localPosition = new Vector3(1.15f, 0.45f, 0f);
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(341);
        }

        public override void OnAutoReload(PlayerController player, Gun gun)
        {
            base.OnAutoReload(player, gun);
            this.forme = 1;
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            base.OnReloadPressed(player, gun, bSOMETHING);
            if (this.gun.IsReloading)
            {
                this.forme = 1;
                if (this.reloaded)
                {
                    AkSoundEngine.PostEvent("Stop_WPN_All", base.gameObject);
                    AkSoundEngine.PostEvent("Play_WPN_plasmacell_reload_01", base.gameObject);
                    this.reloaded = false;
                }
            }
        }

        private void AddElectricGoop(Projectile proj)
        {
            DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(Toolbox.DefaultWaterGoop).TimedAddGoopCircle(proj.sprite.WorldCenter, 1.5f, 0.5f, false);
            for (int i = 0; i < StaticReferenceManager.AllGoops.Count; i++)
            {
                StaticReferenceManager.AllGoops[i].ElectrifyGoopCircle(proj.sprite.WorldBottomCenter, 1f);
            }
        }

        public override Projectile OnPreFireProjectileModifier(Gun gun, Projectile projectile)
        {
            return SequencerController.replacementProjectiles[this.forme - 1];
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            if(this.m_owner != null)
            {
                if(this.forme == 1 && this.m_owner.HasPickupID(295) || this.m_owner.PlayerHasCompletionItem())
                {
                    projectile.baseData.damage *= 1.33f;
                }
                if(this.forme == 2 && this.m_owner.HasPickupID(204) || this.m_owner.PlayerHasCompletionItem())
                {
                    projectile.baseData.damage *= 1.33f;
                }
                if(this.forme == 3 && this.m_owner.HasPickupID(298) || this.m_owner.PlayerHasCompletionItem())
                {
                    projectile.baseData.damage *= 1.33f;
                }
            }
            if(this.forme == 3)
            {
                projectile.OnDestruction += this.AddElectricGoop;
            }
        }

        public PlayerController m_owner
        {
            get
            {
                if(this.gun.CurrentOwner != null && this.gun.CurrentOwner is PlayerController)
                {
                    return this.gun.CurrentOwner as PlayerController;
                }
                return null;
            }
        }

        private void Update()
        {
            if (!this.gun.PreventNormalFireAudio)
            {
                this.gun.PreventNormalFireAudio = true;
            }
            if (this.forme == 1)
            {
                this.gun.DefaultModule.customAmmoType = "burning hand";
                this.gun.DefaultModule.projectiles[0] = SequencerController.replacementProjectiles[0];
                this.gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(384) as Gun).muzzleFlashEffects;
            }
            else if (this.forme == 2)
            {
                this.gun.DefaultModule.customAmmoType = "poison_blob";
                this.gun.DefaultModule.projectiles[0] = SequencerController.replacementProjectiles[1];
                this.gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(207) as Gun).muzzleFlashEffects;
            }
            else
            {
                this.gun.DefaultModule.customAmmoType = "ghost_small";
                this.gun.DefaultModule.projectiles[0] = SequencerController.replacementProjectiles[2];
                this.gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(156) as Gun).muzzleFlashEffects;
            }
            if (!this.gun.IsReloading)
            {
                this.reloaded = true;
            }
            if (this.gun.CurrentOwner != null && this.gun.CurrentOwner is PlayerController)
            {
                lastOwner = this.gun.CurrentOwner as PlayerController;
                if (this.electricityImmunity == null)
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
            this.gun.idleAnimation = SequencerController.UpdateSequencerAnimation(this, "idle", null, false);
            this.gun.shootAnimation = SequencerController.UpdateSequencerAnimation(this, "fire", null, true);
        }

        public static string UpdateSequencerAnimation(SequencerController controller, string name, tk2dSpriteCollectionData collection = null, bool returnToIdle = false)
        {
            Gun gun = controller.gun;
            collection = (collection ?? ETGMod.Databases.Items.WeaponCollection);
            string text = gun.name + controller.forme + "_" + name;
            string text2 = text + "_";
            int prefixLength = text2.Length;
            List<tk2dSpriteAnimationFrame> list = new List<tk2dSpriteAnimationFrame>();
            for (int i = 0; i < collection.spriteDefinitions.Length; i++)
            {
                tk2dSpriteDefinition tk2dSpriteDefinition = collection.spriteDefinitions[i];
                if (tk2dSpriteDefinition.Valid && tk2dSpriteDefinition.name.StartsWithInvariant(text2))
                {
                    list.Add(new tk2dSpriteAnimationFrame
                    {
                        spriteCollection = collection,
                        spriteId = i
                    });
                }
            }
            if (list.Count == 0)
            {
                return null;
            }
            tk2dSpriteAnimationClip tk2dSpriteAnimationClip = gun.spriteAnimator.Library.GetClipByName(text);
            if (tk2dSpriteAnimationClip == null)
            {
                tk2dSpriteAnimationClip = new tk2dSpriteAnimationClip();
                tk2dSpriteAnimationClip.name = text;
                tk2dSpriteAnimationClip.fps = 15f;
                if (returnToIdle)
                {
                    tk2dSpriteAnimationClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
                }
                Array.Resize<tk2dSpriteAnimationClip>(ref gun.spriteAnimator.Library.clips, gun.spriteAnimator.Library.clips.Length + 1);
                gun.spriteAnimator.Library.clips[gun.spriteAnimator.Library.clips.Length - 1] = tk2dSpriteAnimationClip;
            }
            list.Sort((tk2dSpriteAnimationFrame x, tk2dSpriteAnimationFrame y) => int.Parse(collection.spriteDefinitions[x.spriteId].name.Substring(prefixLength)) - int.Parse(collection.spriteDefinitions[y.spriteId].name.Substring(prefixLength)));
            tk2dSpriteAnimationClip.frames = list.ToArray();
            return text;
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            AkSoundEngine.PostEvent("Play_WPN_plasmacell_shot_01", base.gameObject);
            this.forme += 1;
            if (this.forme > 3)
            {
                this.forme = 1;
            }
        }

        private int forme = 1;
        private static List<Projectile> replacementProjectiles = new List<Projectile>();
        private bool reloaded;
        private PlayerController lastOwner = null;
        private DamageTypeModifier electricityImmunity = null;
    }
}