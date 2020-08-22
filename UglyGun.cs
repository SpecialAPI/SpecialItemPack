using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using Dungeonator;

namespace SpecialItemPack
{
    class UglyGun : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Ugly Gun", "ugly_gun");
            Game.Items.Rename("outdated_gun_mods:ugly_gun", "spapi:ugly_gun");
            gun.gameObject.AddComponent<UglyGun>();
            GunExt.SetShortDescription(gun, "Ew!");
            GunExt.SetLongDescription(gun, "A very ugly gun. It doesn't even have a clip, how the heck does it shoot!?\n\nThis gun is a result of a mage trying to smith his own gun. That mage was very expirienced in magic and " +
                "enchantments, but he suck at gunsmithing.\n\nThis gun turned out to be a total fail. It has some parts missing, and light doesn't even affect it. Why the hell did you pick it up???");
            GunExt.SetupSprite(gun, null, "ugly_gun_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(35) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.transform.parent = gun.barrelOffset;
            projectile.baseData.damage = 12.123412342348102398f;
            projectile.baseData.speed /= 4.412387401289374012438523f;
            projectile.SetProjectileSpriteRight("ugly_projectile_001", 11, 11);
            projectile.name = "uGLy_PR0jeCt1L3";
            gun.clipObject = Toolbox.CreateCustomClip("ugly_clip_001", 14, 10);
            gun.muzzleFlashEffects = Toolbox.CreateMuzzleflash("UglyMuzzleflash", new List<string> { "kill_me", "kill_me2" }, 1, new List<IntVector2> { new IntVector2(64, 46), new IntVector2(64, 46) }, new List<tk2dBaseSprite.Anchor> {
                tk2dBaseSprite.Anchor.MiddleLeft, tk2dBaseSprite.Anchor.MiddleLeft }, new List<Vector2> { Vector2.zero, Vector2.zero }, false, false, false, false, 0, VFXAlignment.Fixed, true, new List<float> { 100f, 100f }, new List<Color> { Color.red,
                Color.red});
            gun.reloadClipLaunchFrame = 0;
            gun.DefaultModule.cooldownTime = 0.369817263493402345209387f;
            gun.DefaultModule.numberOfShotsInClip = 7;
            gun.reloadTime = 1.34871023948710239481f;
            gun.SetBaseMaxAmmo(513);
            gun.quality = PickupObject.ItemQuality.C;
            gun.barrelOffset.transform.localPosition = new Vector3(1.7f, 0.65f, 0f);
            gun.encounterTrackable.EncounterGuid = "very_ugly_gun";
            gun.gunClass = GunClass.PISTOL;
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.AddToTrorkShop();
            gun.AddToBlacksmithShop();
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(22);
        }

        protected override void Update()
        {
            if(this.gun.CurrentOwner != null && this.gun.CurrentOwner is PlayerController)
            {
                PlayerController player = this.gun.CurrentOwner as PlayerController;
                if (player.CurrentRoom != null && player.PlayerHasActiveSynergy("#UGLY_BY_NATURE"))
                {
                    if (player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                    {
                        foreach (AIActor aiactor in player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                        {
                            if(aiactor.behaviorSpeculator != null)
                            {
                                if(aiactor.behaviorSpeculator.FleePlayerData == null || aiactor.behaviorSpeculator.FleePlayerData.Player != player)
                                {
                                    aiactor.behaviorSpeculator.FleePlayerData = new FleePlayerData
                                    {
                                        Player = player,
                                        StartDistance = 12f,
                                        DeathDistance = 18f,
                                        StopDistance = 24f
                                    };
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}