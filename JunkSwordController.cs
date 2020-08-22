using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using System.Reflection;
using MonoMod.RuntimeDetour;

namespace SpecialItemPack
{
    class JunkSwordController : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Junk Sword", "junk1_sword");
            Game.Items.Rename("outdated_gun_mods:junk_sword", "spapi:junk_sword");
            gun.gameObject.AddComponent<JunkSwordController>();
            GunExt.SetShortDescription(gun, "Next Time... What is This?");
            GunExt.SetLongDescription(gun, "Just a sword made out of junk. Some of the junk seems enchanted and may react different the more normal junk is around.");
            GunExt.SetupSprite(gun, null, "junk1_sword_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 8);
            GunExt.AddProjectileModuleFrom(gun, "wonderboy", true, true);
            gun.DefaultModule.projectiles[0].baseData.damage = 5;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            gun.DefaultModule.numberOfShotsInClip = 999;
            gun.reloadTime = 0;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.C;
            gun.barrelOffset.transform.localPosition = new Vector3(1.750f, 0.3f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(574) as Gun).muzzleFlashEffects;
            gun.encounterTrackable.EncounterGuid = "junkan1_sword";
            gun.OverrideNormalFireAudioEvent = "Play_WPN_blasphemy_shot_01";
            gun.gunClass = GunClass.SILLY;
            gun.gunSwitchGroup = "Shotgun";
            gun.IsHeroSword = true;
            gun.HeroSwordDoesntBlank = true;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToTrorkShop();
            gun.AddToBlacksmithShop();
            JunkSwordController.NormalSwordId = gun.PickupObjectId;
            JunkSwordController.InitFormes();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(417);
        }

        public override void OnHeroSwordCooldownStarted(PlayerController player, Gun gun)
        {
            base.OnHeroSwordCooldownStarted(player, gun);
            AkSoundEngine.PostEvent("Play_WPN_blasphemy_shot_01", gun.gameObject);
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            if(projectile != null)
            {
                if(this.m_forme < 7)
                {
                    UnityEngine.GameObject.Destroy(projectile.gameObject);
                }
                else if(this.m_forme > 8)
                {
                    ProjectileModule projectileModule = null;
                    if (this.gun.Volley != null)
                    {
                        for (int i = 0; i < this.gun.Volley.projectiles.Count; i++)
                        {
                            for (int j = 0; j < this.gun.Volley.projectiles[j].projectiles.Count; j++)
                            {
                                if (projectile.name.Contains(this.gun.Volley.projectiles[j].projectiles[j].name))
                                {
                                    projectileModule = this.gun.Volley.projectiles[j];
                                    break;
                                }
                            }
                            if (projectileModule != null)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k < this.gun.singleModule.projectiles.Count; k++)
                        {
                            if (projectile.name.Contains(this.gun.singleModule.projectiles[k].name))
                            {
                                projectileModule = this.gun.singleModule;
                                break;
                            }
                        }
                    }
                    if(projectileModule == this.gun.DefaultModule)
                    {
                        UnityEngine.GameObject.Destroy(projectile.gameObject);
                    }
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            if(this.gun && this.gun.CurrentOwner != null && this.gun.CurrentOwner is PlayerController)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;
                int junkAmount = 1;
                bool hasGoldenJunk = false;
                bool hasJunkan = false;
                foreach(PassiveItem passive in player.passiveItems)
                {
                    if(passive.PickupObjectId == 127 || passive.PickupObjectId == 148)
                    {
                        junkAmount += 1;
                    }
                    else if(passive.PickupObjectId == 641)
                    {
                        hasGoldenJunk = true;
                    }
                    else if(passive.PickupObjectId == 580)
                    {
                        hasJunkan = true;
                    }
                }
                if(junkAmount > 8)
                {
                    junkAmount = 8;
                }
                if(junkAmount < 1)
                {
                    junkAmount = 1;
                }
                int forme = junkAmount;
                if (hasGoldenJunk)
                {
                    forme = 9;
                }
                this.m_forme = forme;
                if(this.m_forme != this.m_lastForme)
                {
                    this.ChangeForme(this.m_forme);
                    this.m_lastForme = this.m_forme;
                }
                this.m_hasJunkan = hasJunkan;
                if(this.m_hasJunkan != this.m_hasJunkanLast)
                {
                    this.UpdateDescription(this.m_hasJunkan);
                    this.m_hasJunkanLast = this.m_hasJunkan;
                }
            }
        }

        private void ChangeForme(int forme)
        {
            int id = 0;
            if(forme <= 1)
            {
                id = JunkSwordController.NormalSwordId;
            }
            else if (forme == 2)
            {
                id = JunkSwordController.SquireSwordId;
            }
            else if (forme == 3)
            {
                id = JunkSwordController.HedgeSwordId;
            }
            else if (forme == 4)
            {
                id = JunkSwordController.KnightSwordId;
            }
            else if (forme == 5)
            {
                id = JunkSwordController.MoreKnightSwordID;
            }
            else if (forme == 6)
            {
                id = JunkSwordController.CommanderSwordID;
            }
            else if (forme == 7)
            {
                id = JunkSwordController.HolySwordID;
            }
            else if (forme == 8)
            {
                id = JunkSwordController.AngelSwordID;
            }
            else
            {
                id = JunkSwordController.LightSaberId;
            }
            this.gun.TransformToTargetGun(PickupObjectDatabase.GetById(id) as Gun);
            if (forme == 8)
            {
                this.gun.IsHeroSword = false;
                if (this.gun.OverrideAnimations)
                {
                }
                else
                {
                    if (this.gun.usesDirectionalAnimator && this.gun.aiAnimator.HasDirectionalAnimation(name) && !this.gun.aiAnimator.IsPlaying(name))
                    {
                        this.gun.aiAnimator.PlayUntilFinished(name, false, null, -1f, false);
                    }
                    else
                    {
                        tk2dSpriteAnimationClip clipByName = this.gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(name);
                        if (clipByName != null && !this.gun.GetComponent<tk2dSpriteAnimator>().IsPlaying(clipByName))
                        {
                            this.gun.GetComponent<tk2dSpriteAnimator>().Play(clipByName);
                        }
                    }
                }
            }
            else
            {
                this.gun.IsHeroSword = true;
            }
            if (this.gun.IsHeroSword)
            {
                if(forme < 4)
                {
                    this.gun.HeroSwordDoesntBlank = true;
                }
                else
                {
                    this.gun.HeroSwordDoesntBlank = false;
                }
            }
        }

        private void UpdateDescription(bool hasJunkan)
        {
            if (hasJunkan)
            {
                this.gun.SetName("Ser Junkan's Sword");
                this.gun.SetShortDescription("Knightly Weapon");
                this.gun.SetLongDescription("The sword of Ser Junkan, one of the Gungeon's greatest knights. It will increase it's power the more junk it's owner carries.");
            }
            else
            {
                this.gun.SetName("Junk Sword");
                this.gun.SetShortDescription("Next Time... What is This?");
                this.gun.SetLongDescription("Just a sword made out of junk. Some of the junk seems enchanted and may react different the more normal junk is around.");
            }
        }

        private static void InitFormes()
        {
            JunkSwordController.InitForme2();
            JunkSwordController.InitForme3();
            JunkSwordController.InitForme4();
            JunkSwordController.InitForme5();
            JunkSwordController.InitForme6();
            JunkSwordController.InitForme7();
            JunkSwordController.InitForme8();
            JunkSwordController.InitForme9();
        }

        private static void InitForme2()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Junk Sword", "junk2_sword");
            Game.Items.Rename("outdated_gun_mods:junk_sword", "spapi:junk_sword+junk_2");
            gun.gameObject.AddComponent<JunkSwordController>();
            GunExt.SetShortDescription(gun, "Next Time... What is This?");
            GunExt.SetLongDescription(gun, "Just a sword made out of junk. Some of the junk seems enchanted and may react different the more normal junk is around.");
            GunExt.SetupSprite(gun, null, "junk2_sword_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 8);
            GunExt.AddProjectileModuleFrom(gun, "wonderboy", true, true);
            gun.DefaultModule.projectiles[0].baseData.damage = 7;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            gun.DefaultModule.numberOfShotsInClip = 999;
            gun.reloadTime = 0;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.barrelOffset.transform.localPosition = new Vector3(1.750f, 0.3f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(574) as Gun).muzzleFlashEffects;
            gun.encounterTrackable.EncounterGuid = "junkan2_sword";
            gun.gunClass = GunClass.SILLY;
            gun.gunSwitchGroup = "Shotgun";
            gun.IsHeroSword = true;
            gun.HeroSwordDoesntBlank = true;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            JunkSwordController.SquireSwordId = gun.PickupObjectId;
        }
        
        private static void InitForme3()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Junk Sword", "junk3_sword");
            Game.Items.Rename("outdated_gun_mods:junk_sword", "spapi:junk_sword+junk_3");
            gun.gameObject.AddComponent<JunkSwordController>();
            GunExt.SetShortDescription(gun, "Next Time... What is This?");
            GunExt.SetLongDescription(gun, "Just a sword made out of junk. Some of the junk seems enchanted and may react different the more normal junk is around.");
            GunExt.SetupSprite(gun, null, "junk3_sword_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 8);
            GunExt.AddProjectileModuleFrom(gun, "wonderboy", true, true);
            gun.DefaultModule.projectiles[0].baseData.damage = 9;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            gun.DefaultModule.numberOfShotsInClip = 999;
            gun.reloadTime = 0;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.barrelOffset.transform.localPosition = new Vector3(1.750f, 0.3f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(574) as Gun).muzzleFlashEffects;
            gun.encounterTrackable.EncounterGuid = "junkan3_sword";
            gun.gunClass = GunClass.SILLY;
            gun.gunSwitchGroup = "Shotgun";
            gun.IsHeroSword = true;
            gun.HeroSwordDoesntBlank = true;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            JunkSwordController.HedgeSwordId = gun.PickupObjectId;
        }
        
        private static void InitForme4()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Junk Sword", "junk4_sword");
            Game.Items.Rename("outdated_gun_mods:junk_sword", "spapi:junk_sword+junk_4");
            gun.gameObject.AddComponent<JunkSwordController>();
            GunExt.SetShortDescription(gun, "Next Time... What is This?");
            GunExt.SetLongDescription(gun, "Just a sword made out of junk. Some of the junk seems enchanted and may react different the more normal junk is around.");
            GunExt.SetupSprite(gun, null, "junk4_sword_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 8);
            GunExt.AddProjectileModuleFrom(gun, "wonderboy", true, true);
            gun.DefaultModule.projectiles[0].baseData.damage = 11;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            gun.DefaultModule.numberOfShotsInClip = 999;
            gun.reloadTime = 0;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.barrelOffset.transform.localPosition = new Vector3(1.750f, 0.3f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(574) as Gun).muzzleFlashEffects;
            gun.encounterTrackable.EncounterGuid = "junkan4_sword";
            gun.gunClass = GunClass.SILLY;
            gun.gunSwitchGroup = "Shotgun";
            gun.IsHeroSword = true;
            gun.HeroSwordDoesntBlank = false;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            JunkSwordController.KnightSwordId = gun.PickupObjectId;
        }
        
        private static void InitForme5()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Junk Sword", "junk5_sword");
            Game.Items.Rename("outdated_gun_mods:junk_sword", "spapi:junk_sword+junk_5");
            gun.gameObject.AddComponent<JunkSwordController>();
            GunExt.SetShortDescription(gun, "Next Time... What is This?");
            GunExt.SetLongDescription(gun, "Just a sword made out of junk. Some of the junk seems enchanted and may react different the more normal junk is around.");
            GunExt.SetupSprite(gun, null, "junk5_sword_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 8);
            GunExt.AddProjectileModuleFrom(gun, "wonderboy", true, true);
            gun.DefaultModule.projectiles[0].baseData.damage = 13;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            gun.DefaultModule.numberOfShotsInClip = 999;
            gun.reloadTime = 0;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.barrelOffset.transform.localPosition = new Vector3(1.750f, 0.3f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(574) as Gun).muzzleFlashEffects;
            gun.encounterTrackable.EncounterGuid = "junkan5_sword";
            gun.gunClass = GunClass.SILLY;
            gun.gunSwitchGroup = "Shotgun";
            gun.IsHeroSword = true;
            gun.HeroSwordDoesntBlank = false;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            JunkSwordController.MoreKnightSwordID = gun.PickupObjectId;
        }

        private static void InitForme6()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Junk Sword", "junk6_sword");
            Game.Items.Rename("outdated_gun_mods:junk_sword", "spapi:junk_sword+junk_6");
            gun.gameObject.AddComponent<JunkSwordController>();
            GunExt.SetShortDescription(gun, "Next Time... What is This?");
            GunExt.SetLongDescription(gun, "Just a sword made out of junk. Some of the junk seems enchanted and may react different the more normal junk is around.");
            GunExt.SetupSprite(gun, null, "junk6_sword_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 8);
            GunExt.AddProjectileModuleFrom(gun, "wonderboy", true, true);
            gun.DefaultModule.projectiles[0].baseData.damage = 16;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            gun.DefaultModule.numberOfShotsInClip = 999;
            gun.reloadTime = 0;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.barrelOffset.transform.localPosition = new Vector3(1.750f, 0.3f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(574) as Gun).muzzleFlashEffects;
            gun.encounterTrackable.EncounterGuid = "junkan6_sword";
            gun.gunClass = GunClass.SILLY;
            gun.gunSwitchGroup = "Shotgun";
            gun.IsHeroSword = true;
            gun.HeroSwordDoesntBlank = false;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            JunkSwordController.CommanderSwordID = gun.PickupObjectId;
        }

        private static void InitForme7()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Junk Sword", "junk7_sword");
            Game.Items.Rename("outdated_gun_mods:junk_sword", "spapi:junk_sword+junk_7");
            gun.gameObject.AddComponent<JunkSwordController>();
            GunExt.SetShortDescription(gun, "Next Time... What is This?");
            GunExt.SetLongDescription(gun, "Just a sword made out of junk. Some of the junk seems enchanted and may react different the more normal junk is around.");
            GunExt.SetupSprite(gun, null, "junk7_sword_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 8);
            GunExt.AddProjectileModuleFrom(gun, "wonderboy", true, false);
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(574) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.DefaultTintColor = new Color(1, 1, 1);
            projectile.name = "JunkSword_HolyForm_Projectile";
            projectile.HasDefaultTint = true;
            gun.DefaultModule.projectiles[0] = projectile;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            gun.DefaultModule.numberOfShotsInClip = 999;
            gun.reloadTime = 0;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.barrelOffset.transform.localPosition = new Vector3(1.750f, 0.3f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(574) as Gun).muzzleFlashEffects;
            gun.encounterTrackable.EncounterGuid = "junkan7_sword";
            gun.gunClass = GunClass.SILLY;
            gun.gunSwitchGroup = "Shotgun";
            gun.IsHeroSword = true;
            gun.HeroSwordDoesntBlank = false;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            JunkSwordController.HolySwordID = gun.PickupObjectId;
        }

        private static void InitForme8()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Junk Sword", "junk8_sword");
            Game.Items.Rename("outdated_gun_mods:junk_sword", "spapi:junk_sword+junk_8");
            gun.gameObject.AddComponent<JunkSwordController>();
            GunExt.SetShortDescription(gun, "Next Time... What is This?");
            GunExt.SetLongDescription(gun, "Just a sword made out of junk. Some of the junk seems enchanted and may react different the more normal junk is around.");
            GunExt.SetupSprite(gun, null, "junk8_sword_idle_001", 8);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(EnemyDatabase.GetOrLoadByGuid("c6c8e59d0f5d41969c74e802c9d67d07").bulletBank.GetBullet("angel").BulletObject.GetComponent<Projectile>());
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            projectile.name = "JunkSword_AngelForm_Projectile";
            gun.DefaultModule.projectiles[0] = projectile;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
            gun.DefaultModule.angleVariance = 30;
            gun.DefaultModule.cooldownTime = 0.05f;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            gun.DefaultModule.customAmmoType = "white";
            gun.DefaultModule.numberOfShotsInClip = 999;
            gun.reloadTime = 0;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.barrelOffset.transform.localPosition = new Vector3(0.65f, 1.7f, 0f);
            gun.encounterTrackable.EncounterGuid = "junkan8_sword";
            gun.gunClass = GunClass.SILLY;
            gun.gunSwitchGroup = "Sack";
            gun.muzzleFlashEffects.type = VFXPoolType.None;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            JunkSwordController.AngelSwordID = gun.PickupObjectId;
        }

        private static void InitForme9()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Junk Sword", "junk9_sword");
            Game.Items.Rename("outdated_gun_mods:junk_sword", "spapi:junk_sword+junk_9");
            gun.gameObject.AddComponent<JunkSwordController>();
            GunExt.SetShortDescription(gun, "Next Time... What is This?");
            GunExt.SetLongDescription(gun, "Just a sword made out of junk. Some of the junk seems enchanted and may react different the more normal junk is around.");
            GunExt.SetupSprite(gun, null, "junk9_sword_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 8);
            GunExt.AddProjectileModuleFrom(gun, "wonderboy", true, true);
            ProjectileModule mod = GunExt.AddProjectileModuleFrom(gun, (PickupObjectDatabase.GetById(345) as Gun), true, false);
            mod.angleVariance = 0;
            mod.cooldownTime = 0;
            mod.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.projectiles[0].baseData.damage = 90;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            gun.DefaultModule.numberOfShotsInClip = 999;
            gun.reloadTime = 0;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.EXCLUDED;
            gun.barrelOffset.transform.localPosition = new Vector3(1.750f, 0.3f, 0f);
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(574) as Gun).muzzleFlashEffects;
            gun.encounterTrackable.EncounterGuid = "junkan9_sword";
            gun.gunClass = GunClass.SILLY;
            gun.PreventNormalFireAudio = true;
            gun.IsHeroSword = true;
            gun.HeroSwordDoesntBlank = false;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            JunkSwordController.LightSaberId = gun.PickupObjectId;
        }

        public static int NormalSwordId;
        public static int SquireSwordId;
        public static int HedgeSwordId;
        public static int KnightSwordId;
        public static int MoreKnightSwordID;
        public static int CommanderSwordID;
        public static int HolySwordID;
        public static int AngelSwordID;
        public static int LightSaberId;

        private int m_forme = 0;
        private int m_lastForme = 0;
        private bool m_hasJunkan = false;
        private bool m_hasJunkanLast = false;
    }
}