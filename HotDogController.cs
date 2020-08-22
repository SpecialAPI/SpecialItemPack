using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using SpecialItemPack.ItemAPI;
using UnityEngine;
using Dungeonator;
using Pathfinding;

namespace SpecialItemPack
{
    class HotDogController : AdvancedGunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Hot Dog", "hot_dog");
            Game.Items.Rename("outdated_gun_mods:hot_dog", "spapi:hot_dog");
            gun.gameObject.AddComponent<HotDogController>();
            GunExt.SetShortDescription(gun, "How Do The Things Work?");
            GunExt.SetLongDescription(gun, "A simple hot dog. Smells pretty tasty.");
            GunExt.SetupSprite(gun, null, "hot_dog_idle_001", 8);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 16);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            //Setting up main projectile.
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(12) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            HotDogController.hotDogBasicProjectile = projectile;
            projectile.hitEffects = Toolbox.GetGunById(47).DefaultModule.projectiles[0].hitEffects;
            projectile.onDestroyEventName = Toolbox.GetGunById(33).DefaultModule.projectiles[0].onDestroyEventName;
            projectile.enemyImpactEventName = Toolbox.GetGunById(33).DefaultModule.projectiles[0].enemyImpactEventName;
            projectile.objectImpactEventName = Toolbox.GetGunById(33).DefaultModule.projectiles[0].objectImpactEventName;
            projectile.hitEffects = (PickupObjectDatabase.GetById(38) as Gun).DefaultModule.projectiles[0].hitEffects;
            projectile.transform.parent = gun.barrelOffset;
            projectile.SetProjectileSpriteRight("not_a_hot_dog_001", 17, 5, false);
            projectile.baseData.damage = 3f;
            projectile.name = "Not_A_Hot_Dog";
            //Setting up additional synergy projectile.
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>((PickupObjectDatabase.GetById(12) as Gun).DefaultModule.projectiles[0]);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            HotDogController.literallySynergyReplacementProjectile = projectile2;
            projectile2.hitEffects = Toolbox.GetGunById(47).DefaultModule.projectiles[0].hitEffects;
            projectile2.onDestroyEventName = "Play_PET_wolf_bark_01";
            projectile2.enemyImpactEventName = "Play_PET_wolf_bite_01";
            projectile2.objectImpactEventName = "Play_PET_wolf_bark_01";
            projectile2.hitEffects = (PickupObjectDatabase.GetById(38) as Gun).DefaultModule.projectiles[0].hitEffects;
            projectile2.transform.parent = gun.barrelOffset;
            projectile2.AppliesFire = true;
            projectile2.FireApplyChance = 100f;
            projectile2.fireEffect = (PickupObjectDatabase.GetById(295) as BulletStatusEffectItem).FireModifierEffect;
            projectile2.SetProjectileSpriteRight("a_hot_dog_001", 16, 11);
            projectile2.DefaultTintColor = new Color(1, 0, 0).WithAlpha(0.5f);
            projectile2.gameObject.AddComponent<FieryParticlesBehaviour>();
            projectile2.HasDefaultTint = true;
            projectile2.baseData.damage = 3f;
            projectile2.name = "A_Hot_Dog";
            gun.reloadTime = 0.5f;
            gun.DefaultModule.cooldownTime *= 2f;
            gun.SetBaseMaxAmmo(300);
            gun.gunSwitchGroup = (PickupObjectDatabase.GetById(510) as Gun).gunSwitchGroup;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.quality = PickupObject.ItemQuality.D;
            gun.encounterTrackable.EncounterGuid = "nom_nom_hotdog";
            gun.gunClass = GunClass.SILLY;
            gun.barrelOffset.transform.localPosition = new Vector3(1.6f, 0.65f, 0f);
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.RemovePeskyQuestionmark();
            gun.PlaceItemInAmmonomiconAfterItemById(478);
        }

        public static IEnumerator HandleSeamlessTransitionToCombatRoom(RoomHandler sourceRoom)
        {
            Dungeon d = GameManager.Instance.Dungeon;
            int tmapExpansion = 13;
            RoomHandler newRoom = d.RuntimeDuplicateChunk(sourceRoom.area.basePosition, sourceRoom.area.dimensions, tmapExpansion, sourceRoom, true);
            newRoom.CompletelyPreventLeaving = true;
            List<Transform> movedObjects = new List<Transform>();
            Vector2 oldPlayerPosition = GameManager.Instance.BestActivePlayer.transform.position.XY();
            Vector2 playerOffset = oldPlayerPosition - sourceRoom.area.basePosition.ToVector2();
            Vector2 newPlayerPosition = newRoom.area.basePosition.ToVector2() + playerOffset;
            Pixelator.Instance.FadeToColor(0.25f, Color.white, true, 0.125f);
            Pathfinder.Instance.InitializeRegion(d.data, newRoom.area.basePosition, newRoom.area.dimensions);
            GameManager.Instance.BestActivePlayer.WarpToPoint(newPlayerPosition, false, false);
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                GameManager.Instance.GetOtherPlayer(GameManager.Instance.BestActivePlayer).ReuniteWithOtherPlayer(GameManager.Instance.BestActivePlayer, false);
            }
            yield return null;
            for (int j = 0; j < GameManager.Instance.AllPlayers.Length; j++)
            {
                GameManager.Instance.AllPlayers[j].WarpFollowersToPlayer(false);
                GameManager.Instance.AllPlayers[j].WarpCompanionsToPlayer(false);
            }
            yield return d.StartCoroutine(HandleCombatRoomExpansion(sourceRoom, newRoom, null));
            /*this.DisappearDrillPoof.SpawnAtPosition(spawnedSprite.WorldBottomLeft + new Vector2(-0.0625f, 0.25f), 0f, null, null, null, new float?(3f), false, null, null, false);
            UnityEngine.Object.Destroy(spawnedVFX.gameObject);
            sourceChest.ForceUnlock();
            AkSoundEngine.PostEvent("Stop_OBJ_paydaydrill_loop_01", GameManager.Instance.gameObject);
            AkSoundEngine.PostEvent("Play_OBJ_item_spawn_01", GameManager.Instance.gameObject);
            bool goodToGo = false;
            while (!goodToGo)
            {
                goodToGo = true;
                for (int k = 0; k < GameManager.Instance.AllPlayers.Length; k++)
                {
                    float num = Vector2.Distance(sourceChest.specRigidbody.UnitCenter, GameManager.Instance.AllPlayers[k].CenterPosition);
                    if (num > 3f)
                    {
                        goodToGo = false;
                    }
                }
                yield return null;
            }
            GameManager.Instance.MainCameraController.SetManualControl(true, true);
            GameManager.Instance.MainCameraController.OverridePosition = GameManager.Instance.BestActivePlayer.CenterPosition;
            for (int l = 0; l < GameManager.Instance.AllPlayers.Length; l++)
            {
                GameManager.Instance.AllPlayers[l].SetInputOverride("shrinkage");
            }
            yield return d.StartCoroutine(this.HandleCombatRoomShrinking(newRoom));
            for (int m = 0; m < GameManager.Instance.AllPlayers.Length; m++)
            {
                GameManager.Instance.AllPlayers[m].ClearInputOverride("shrinkage");
            }
            Pixelator.Instance.FadeToColor(0.25f, Color.white, true, 0.125f);
            AkSoundEngine.PostEvent("Play_OBJ_paydaydrill_end_01", GameManager.Instance.gameObject);
            GameManager.Instance.MainCameraController.SetManualControl(false, false);
            GameManager.Instance.BestActivePlayer.WarpToPoint(oldPlayerPosition, false, false);
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                GameManager.Instance.GetOtherPlayer(GameManager.Instance.BestActivePlayer).ReuniteWithOtherPlayer(GameManager.Instance.BestActivePlayer, false);
            }
            this.MoveObjectBetweenRooms(sourceChest.transform, newRoom, sourceRoom);
            for (int n = 0; n < movedObjects.Count; n++)
            {
                this.MoveObjectBetweenRooms(movedObjects[n], newRoom, sourceRoom);
            }
            sourceRoom.RegisterInteractable(sourceChest);
            this.m_inEffect = false;*/
            yield break;
        }

        public static IEnumerator HandleCombatRoomExpansion(RoomHandler sourceRoom, RoomHandler targetRoom, Chest sourceChest)
        {
            yield return new WaitForSeconds(1);
            float duration = 5.5f;
            float elapsed = 0f;
            int numExpansionsDone = 0;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime * 9f;
                while (elapsed > (float)numExpansionsDone)
                {
                    numExpansionsDone++;
                    ExpandRoom(targetRoom);
                    AkSoundEngine.PostEvent("Play_OBJ_rock_break_01", GameManager.Instance.gameObject);
                }
                yield return null;
            }
            Dungeon d = GameManager.Instance.Dungeon;
            Pathfinder.Instance.InitializeRegion(d.data, targetRoom.area.basePosition + new IntVector2(-5, -5), targetRoom.area.dimensions + new IntVector2(10, 10));
            yield break;
        }

        public static void ExpandRoom(RoomHandler r)
        {
            Dungeon dungeon = GameManager.Instance.Dungeon;
            AkSoundEngine.PostEvent("Play_OBJ_stone_crumble_01", GameManager.Instance.gameObject);
            tk2dTileMap tk2dTileMap = null;
            HashSet<IntVector2> hashSet = new HashSet<IntVector2>();
            for (int i = -5; i < r.area.dimensions.x + 5; i++)
            {
                for (int j = -5; j < r.area.dimensions.y + 5; j++)
                {
                    IntVector2 intVector = r.area.basePosition + new IntVector2(i, j);
                    CellData cellData = (!dungeon.data.CheckInBoundsAndValid(intVector)) ? null : dungeon.data[intVector];
                    if (cellData != null && cellData.type == CellType.WALL && cellData.HasTypeNeighbor(dungeon.data, CellType.FLOOR))
                    {
                        hashSet.Add(cellData.position);
                    }
                }
            }
            foreach (IntVector2 key in hashSet)
            {
                CellData cellData2 = dungeon.data[key];
                cellData2.breakable = true;
                cellData2.occlusionData.overrideOcclusion = true;
                cellData2.occlusionData.cellOcclusionDirty = true;
                tk2dTileMap = dungeon.DestroyWallAtPosition(key.x, key.y, true);
                r.Cells.Add(cellData2.position);
                r.CellsWithoutExits.Add(cellData2.position);
                r.RawCells.Add(cellData2.position);
            }
            Pixelator.Instance.MarkOcclusionDirty();
            Pixelator.Instance.ProcessOcclusionChange(r.Epicenter, 1f, r, false);
            if (tk2dTileMap)
            {
                dungeon.RebuildTilemap(tk2dTileMap);
            }
        }

        protected override void Update()
        {
            base.Update();
            if (this.everPickedUpByPlayer && this.Owner == null && this.canEat)
            {
                this.fiveSecondsTimer -= BraveTime.DeltaTime;
                if (this.fiveSecondsTimer <= 0)
                {
                    this.gun.gameObject.AddComponent<CantEatBehavior>();
                    if(this.gun.sprite != null)
                    {
                        SpawnManager.SpawnVFX(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, this.gun.sprite.WorldTopLeft + Vector2.up / 2f, Quaternion.identity);
                    }
                }
            }
            if (!this.canEat && !this.Owner)
            {
                Vector3 vector = this.gun.sprite.WorldBottomLeft.ToVector3ZisY(0);
                Vector3 vector2 = this.gun.sprite.WorldTopRight.ToVector3ZisY(0);
                float num = (vector2.y - vector.y) * (vector2.x - vector.x);
                float num2 = 25f * num;
                int num3 = Mathf.CeilToInt(Mathf.Max(1f, num2 * BraveTime.DeltaTime));
                int num4 = num3;
                Vector3 minPosition = vector;
                Vector3 maxPosition = vector2;
                Vector3 direction = Vector3.up / 2f;
                float angleVariance = 120f;
                float magnitudeVariance = 0.2f;
                float? startLifetime = new float?(UnityEngine.Random.Range(0.8f, 1.25f));
                GlobalSparksDoer.DoRandomParticleBurst(num4, minPosition, maxPosition, direction, angleVariance, magnitudeVariance, null, startLifetime, null, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
            }
            if(this.Owner != null)
            {
                this.fiveSecondsTimer = 5f;
            }
            if(this.Player != null)
            {
                if (this.Player.PlayerHasActiveSynergy("#LITERALLY"))
                {
                    foreach(ProjectileModule mod in this.gun.Volley.projectiles)
                    {
                        mod.projectiles[0] = HotDogController.literallySynergyReplacementProjectile;
                    }
                }
                else
                {
                    foreach (ProjectileModule mod in this.gun.Volley.projectiles)
                    {
                        mod.projectiles[0] = HotDogController.hotDogBasicProjectile;
                    }
                }
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool bSOMETHING)
        {
            base.OnReloadPressed(player, gun, bSOMETHING);
            if((gun.ClipShotsRemaining >= gun.DefaultModule.GetModNumberOfShotsInClip(player) || gun.ammo <= gun.DefaultModule.GetModNumberOfShotsInClip(player)) && !player.PlayerHasActiveSynergy("#LITERALLY")
                && player.healthHaver.GetCurrentHealthPercentage() < 1f && this.canEat)
            {
                player.healthHaver.ApplyHealing(0.5f);
                AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", player.gameObject);
                gun.SetBaseMaxAmmo(this.gun.GetBaseMaxAmmo() - 60);
                gun.ammo -= 60;
                if(gun.ammo < 0)
                {
                    gun.ammo = 0;
                }
                if(gun.GetBaseMaxAmmo() <= 0)
                {
                    player.inventory.RemoveGunFromInventory(gun);
                }
            }
        }

        private float fiveSecondsTimer = 5f;
        private bool canEat
        {
            get
            {
                return this.gun.GetComponent<CantEatBehavior>() == null;
            }
        }

        public static Projectile literallySynergyReplacementProjectile;
        public static Projectile hotDogBasicProjectile;

        private class CantEatBehavior : MonoBehaviour
        {

        }

        private class FieryParticlesBehaviour : MonoBehaviour
        {
            private void Start()
            {
                this.m_proj = base.GetComponent<Projectile>();
                this.m_proj.OnPostUpdate += this.DoParticleBurst;
            }

            private void DoParticleBurst(Projectile proj)
            {
                Vector3 vector = proj.sprite.WorldBottomLeft.ToVector3ZisY(0);
                Vector3 vector2 = proj.sprite.WorldTopRight.ToVector3ZisY(0);
                float num = (vector2.y - vector.y) * (vector2.x - vector.x);
                float num2 = 25f * num;
                int num3 = Mathf.CeilToInt(Mathf.Max(1f, num2 * BraveTime.DeltaTime));
                int num4 = num3;
                Vector3 minPosition = vector;
                Vector3 maxPosition = vector2;
                Vector3 direction = Vector3.up / 2f;
                float angleVariance = 120f;
                float magnitudeVariance = 0.2f;
                float? startLifetime = new float?(UnityEngine.Random.Range(0.8f, 1.25f));
                GlobalSparksDoer.DoRandomParticleBurst(num4, minPosition, maxPosition, direction, angleVariance, magnitudeVariance, null, startLifetime, null, GlobalSparksDoer.SparksType.EMBERS_SWIRLING);
            }

            private Projectile m_proj;
        }
    }
}