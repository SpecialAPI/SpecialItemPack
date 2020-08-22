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
    class EnterTheGungeon : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("The Gungeon", "gungeon");
            Game.Items.Rename("outdated_gun_mods:the_gungeon", "spapi:the_gungeon");
            gun.gameObject.AddComponent<EnterTheGungeon>();
            GunExt.SetShortDescription(gun, "EEeeeeeeeeeee...");
            GunExt.SetLongDescription(gun, "The Gungeon from an alternate universe, converted into this by a mystic magician. Mysterious music can be heard from inside.\n\n ...eeeeeeeeeeeenter The Gungeon (Enter the gun)\n" +
                "ENTER THE GUNGEON (Enter the gun) \n\nBy Beholster eye," +
                " Dragun tooth,\nGungeon wind do whisper one truth,\n" +
                "body the floor, bullet through roof\nammo willin', PastKillin' gun shall shoot\n\nNon stop hammercock\nviolent mannered shots land a lot\nkeep cannon hot, can it not cease,\ngranted rot peace rest handed\ngot beast, best last man standin" +
                "gun speak, fleece boss head handed,\n\nHE\ngun'd it up gungeon down\ngun'd it up gungeon down\ngun'd it up gungeon down\n\nSHE\ngun'd it up gungeon down\ngun'd it up gungeon down\ngun'd it up gungeon down\n\nWE...\ndead em and down," +
                "\nim chopping round after round after round\n" +
                "gun'd it up gungeon down\ngun'd it up gungeon down\ngun'd it up gungeon down\n\n" +
                "WE...\ndead em and down,\nim chopping round after round after round\ngun'd it up gungeon down\ngun'd it up gungeon down\ngun'd it up gungeon down\n\nWE...\ndead em and down,\nim chopping round after round after round\n\n" +
                "ENTER THE GUNGEON (Enter the gun)\nENTER THE GUNGEON (Enter the gun)\nENTER THE GUNGEON (Enter the gun)\n\n" +
                "gun'd it up gungeon down\ngun'd it up gungeon down\ngun'd it up gungeon down\n\ngun'd it up gungeon down\ngun'd it up gungeon down\ngun'd it up gungeon down\n\nDeath down barrel at you only once wink.\n" +
                "Blur gone bullets life at brink in a blink.\nCaseing killing, floor clink\nfought boss, blood drink\n" +
                "too wired to die, too under fire to think\n\nYou'd better run...\nGimmie that gun...\n\nYou'd better run...\nGimmie that gun...\n\nGo the jaws of the shot\nGo the keys to the lock\nand what I got...\n\n" +
                "The Tangler weapon, what a shot - gun full of hate\nLamp, Cold 45, Grass chopper to your face\nPea shooter, Rusty blooper,\nRogue Special, Vindicator,\nBee hive to your temple,\nBee hive to your temple,\n" +
                "Zorgun, Disintegrator,\nThe Glacier, Law maker,\nDemon head for the hater,\nUnicorn horn, Plague gauge,\nGun bow, Ice breaker\n\nDungeon eagle, Gungeon ant\nZilla, Robot's right hand\nand Uzi spine millimeter go BLAM!\n\n" +
                "ENTER THE GUNGEON (Enter the gun)\nENTER THE GUNGEON (Enter the gun)\nENTER THE GUNGEON (Enter the gun)");
            GunExt.SetupSprite(gun, null, "gungeon_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 6);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(47) as Gun).DefaultModule.projectiles[0]);
            gun.DefaultModule.angleVariance = 0;
            projectile.specRigidbody.UpdateCollidersOnRotation = true;
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.gameObject.AddComponent<EnterTheGungeonProjectile>();
            projectile.specRigidbody.PrimaryPixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            projectile.transform.parent = gun.barrelOffset;
            gun.DefaultModule.numberOfShotsInClip = 900;
            projectile.baseData.damage = 12f;
            gun.DefaultModule.angleVariance /= 2.5f;
            gun.DefaultModule.cooldownTime = 0.05f;
            projectile.BossDamageMultiplier = 1.35f;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            gun.reloadTime = 1f;
            gun.SetBaseMaxAmmo(900);
            gun.barrelOffset.transform.localPosition = new Vector3(1.05f, 0.1f, 0f);
            gun.quality = PickupObject.ItemQuality.S;
            gun.encounterTrackable.EncounterGuid = "enter_the_gungeon";
            gun.gunClass = GunClass.RIFLE;
            gun.barrelOffset.transform.localPosition = new Vector3(1.6f, 0.65f, 0f);
            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        private void Update()
        {
            if(this.gun.CurrentOwner != null && !this.TriggeredMusic)
            {
                AkSoundEngine.PostEvent("Stop_MUS_All", base.gameObject);
                AkSoundEngine.PostEvent("Play_MUS_Anthem_Winner_Short_01", base.gameObject);
                this.TriggeredMusic = true;
            }
            else if(this.gun.CurrentOwner == null && this.TriggeredMusic)
            {
                AkSoundEngine.PostEvent("Stop_MUS_All", base.gameObject);
                AkSoundEngine.PostEvent(this.ResetMusic(GameManager.Instance.Dungeon), GameManager.Instance.gameObject);
                this.TriggeredMusic = false;
            }
        }

        private bool TriggeredMusic = false;

        public string ResetMusic(Dungeon d)
        {
            string m_cachedMusicEventCore;
            bool flag = !string.IsNullOrEmpty(d.musicEventName);
            if (flag)
            {
                m_cachedMusicEventCore = d.musicEventName;
            }
            else
            {
                m_cachedMusicEventCore = "Play_MUS_Dungeon_Theme_01";
            }
            return m_cachedMusicEventCore;
        }

        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            PlayerController player = this.gun.CurrentOwner as PlayerController;
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
        }
    }
}