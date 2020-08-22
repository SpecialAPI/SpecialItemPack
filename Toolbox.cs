using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using Gungeon;
using SpecialItemPack.ItemAPI;
using Dungeonator;
using System.Collections.ObjectModel;
using System.Reflection;
using Brave.BulletScript;
using tk2dRuntime.TileMap;
using SpecialItemPack.AdaptedSynergyStuff;
using System.IO;

namespace SpecialItemPack
{
    public static class Toolbox
    {
        public static void Init()
        {
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            AssetBundle assetBundle2 = ResourceManager.LoadAssetBundle("shared_auto_002");
            shared_auto_001 = assetBundle;
            shared_auto_002 = assetBundle2;
            GoopDefinition goopDefinition;
            string text = "assets/data/goops/water goop.asset";
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
            Toolbox.DefaultWaterGoop = goopDefinition;
            GoopDefinition goopDefinition2;
            text = "assets/data/goops/poison goop.asset";
            try
            {
                GameObject gameObject2 = assetBundle.LoadAsset(text) as GameObject;
                goopDefinition2 = gameObject2.GetComponent<GoopDefinition>();
            }
            catch
            {
                goopDefinition2 = (assetBundle.LoadAsset(text) as GoopDefinition);
            }
            goopDefinition2.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
            Toolbox.DefaultPoisonGoop = goopDefinition2;
            GoopDefinition goopDefinition3;
            text = "assets/data/goops/napalmgoopquickignite.asset";
            try
            {
                GameObject gameObject2 = assetBundle.LoadAsset(text) as GameObject;
                goopDefinition3 = gameObject2.GetComponent<GoopDefinition>();
            }
            catch
            {
                goopDefinition3 = (assetBundle.LoadAsset(text) as GoopDefinition);
            }
            goopDefinition3.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
            Toolbox.DefaultFireGoop = goopDefinition3;
            PickupObject byId = PickupObjectDatabase.GetById(310);
            GoopDefinition item;
            if (byId == null)
            {
                item = null;
            }
            else
            {
                WingsItem component = byId.GetComponent<WingsItem>();
                item = ((component != null) ? component.RollGoop : null);
            }
            Toolbox.DefaultCharmGoop = item;
            Toolbox.DefaultCheeseGoop = (PickupObjectDatabase.GetById(626) as Gun).DefaultModule.projectiles[0].cheeseEffect.CheeseGoop;
            Toolbox.DefaultBlobulonGoop = EnemyDatabase.GetOrLoadByGuid("0239c0680f9f467dbe5c4aab7dd1eca6").GetComponent<GoopDoer>().goopDefinition;
            Toolbox.DefaultPoopulonGoop = EnemyDatabase.GetOrLoadByGuid("116d09c26e624bca8cca09fc69c714b3").GetComponent<GoopDoer>().goopDefinition;
            Toolbox.DefaultGreenFireGoop = Toolbox.GetGunById(698).DefaultModule.projectiles[0].GetComponent<GoopModifier>().goopDefinition;
            GameObject obj = new GameObject("SprenSpunVFX");
            obj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(obj);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            tk2dSpriteAnimator animator = obj.AddComponent<tk2dSpriteAnimator>();
            if (animator.Library == null)
            {
                animator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
                animator.Library.clips = new tk2dSpriteAnimationClip[0];
                animator.Library.enabled = true;
            }
            animator.gameObject.AddComponent<tk2dSprite>();
            SprenOrbitalItem sprenItem = (PickupObjectDatabase.GetById(578) as SprenOrbitalItem);
            tk2dSpriteAnimationClip[] clips = new tk2dSpriteAnimationClip[0];
            foreach (tk2dSpriteAnimationClip clip in sprenItem.OrbitalFollowerPrefab.GetComponentInChildren<tk2dSpriteAnimator>().Library.clips)
            {
                tk2dSpriteAnimationClip clip2 = new tk2dSpriteAnimationClip();
                clip2.name = clip.name;
                clip2.fps = clip.fps;
                clip2.loopStart = clip.loopStart;
                clip2.wrapMode = clip.wrapMode;
                clip2.minFidgetDuration = clip.minFidgetDuration;
                clip2.maxFidgetDuration = clip.maxFidgetDuration;
                tk2dSpriteAnimationFrame[] frames = new tk2dSpriteAnimationFrame[0];
                foreach(tk2dSpriteAnimationFrame frame in clip.frames)
                {
                    tk2dSpriteAnimationFrame frame2 = new tk2dSpriteAnimationFrame();
                    frame2.spriteCollection = frame.spriteCollection;
                    frame2.spriteId = frame.spriteId;
                    frame2.invulnerableFrame = frame.invulnerableFrame;
                    frame2.groundedFrame = frame.groundedFrame;
                    frame2.requiresOffscreenUpdate = frame.requiresOffscreenUpdate;
                    frame2.eventVfx = frame.eventVfx;
                    frame2.eventStopVfx = frame.eventStopVfx;
                    frame2.eventLerpEmissive = frame.eventLerpEmissive;
                    frame2.eventLerpEmissiveTime = frame.eventLerpEmissiveTime;
                    frame2.eventLerpEmissivePower = frame.eventLerpEmissivePower;
                    frame2.forceMaterialUpdate = frame.forceMaterialUpdate;
                    frame2.finishedSpawning = frame.finishedSpawning;
                    frame2.triggerEvent = frame.triggerEvent;
                    frame2.eventAudio = "";
                    frame2.eventInfo = frame.eventInfo;
                    frame2.eventInt = frame.eventInt;
                    frame2.eventFloat = frame.eventFloat;
                    frame2.eventOutline = frame.eventOutline;
                    frames = frames.Concat(new tk2dSpriteAnimationFrame[] { frame2 }).ToArray();
                }
                clip2.frames = frames;
                clips = clips.Concat(new tk2dSpriteAnimationClip[] { clip2 }).ToArray();
            }
            animator.Library.clips = clips;
            SprenSpunBehaviour behaviour = obj.AddComponent<SprenSpunBehaviour>();
            behaviour.GunChangeMoreAnimation = sprenItem.GunChangeMoreAnimation;
            behaviour.BackchangeAnimation = sprenItem.BackchangeAnimation;
            SprenSpunPrefab = obj;
        }

        public static AIBulletBank.Entry Clone(this AIBulletBank.Entry other, bool cloneProjectiles = true, bool cloneProjectileData = true, bool cloneMuzzleflash = true, bool cloneShells = true)
        {
            AIBulletBank.Entry result = new AIBulletBank.Entry();
            result.Name = other.Name;
            if (cloneProjectiles)
            {
                if (other.BulletObject != null)
                {
                    if (other.BulletObject.GetComponent<Projectile>() != null)
                    {
                        Projectile bullet = UnityEngine.Object.Instantiate(other.BulletObject.GetComponent<Projectile>());
                        bullet.gameObject.SetActive(false);
                        FakePrefab.MarkAsFakePrefab(bullet.gameObject);
                        UnityEngine.Object.DontDestroyOnLoad(bullet);
                        result.BulletObject = bullet.gameObject;
                    }
                    else
                    {
                        GameObject bullet = UnityEngine.Object.Instantiate(other.BulletObject);
                        bullet.SetActive(false);
                        FakePrefab.MarkAsFakePrefab(bullet);
                        UnityEngine.Object.DontDestroyOnLoad(bullet);
                        result.BulletObject = bullet;
                    }
                }
                else
                {
                    result.BulletObject = null;
                }
            }
            else
            {
                result.BulletObject = other.BulletObject;
            }
            result.OverrideProjectile = other.OverrideProjectile;
            if (cloneProjectileData)
            {
                if (other.ProjectileData != null)
                {
                    result.ProjectileData = new ProjectileData(other.ProjectileData);
                }
                else
                {
                    result.ProjectileData = null;
                }
            }
            else
            {
                result.ProjectileData = other.ProjectileData;
            }
            result.PlayAudio = other.PlayAudio;
            result.AudioSwitch = other.AudioSwitch;
            result.AudioEvent = other.AudioEvent;
            result.AudioLimitOncePerFrame = other.AudioLimitOncePerFrame;
            result.AudioLimitOncePerAttack = other.AudioLimitOncePerAttack;
            if (cloneMuzzleflash)
            {
                result.MuzzleFlashEffects = other.MuzzleFlashEffects.Clone();
            }
            else
            {
                result.MuzzleFlashEffects = other.MuzzleFlashEffects;
            }
            result.MuzzleLimitOncePerFrame = other.MuzzleLimitOncePerFrame;
            result.MuzzleInheritsTransformDirection = other.MuzzleInheritsTransformDirection;
            result.SpawnShells = other.SpawnShells;
            result.ShellTransform = other.ShellTransform;
            if (cloneShells)
            {
                if (other.ShellPrefab != null)
                {
                    GameObject shell = UnityEngine.Object.Instantiate(other.ShellPrefab);
                    shell.SetActive(false);
                    FakePrefab.MarkAsFakePrefab(shell);
                    UnityEngine.Object.DontDestroyOnLoad(shell);
                    result.ShellPrefab = shell;
                }
                else
                {
                    result.ShellPrefab = null;
                }
            }
            else
            {
                result.ShellPrefab = other.ShellPrefab;
            }
            result.ShellForce = other.ShellForce;
            result.ShellForceVariance = other.ShellForceVariance;
            result.DontRotateShell = other.DontRotateShell;
            result.ShellGroundOffset = other.ShellGroundOffset;
            result.ShellsLimitOncePerFrame = other.ShellsLimitOncePerFrame;
            result.rampBullets = other.rampBullets;
            result.rampStartHeight = other.rampStartHeight;
            result.rampTime = other.rampTime;
            result.conditionalMinDegFromNorth = other.conditionalMinDegFromNorth;
            result.forceCanHitEnemies = other.forceCanHitEnemies;
            result.suppressHitEffectsIfOffscreen = other.suppressHitEffectsIfOffscreen;
            result.preloadCount = other.preloadCount;
            return result;
        }

        public static VFXPool Clone(this VFXPool other)
        {
            if(other == null)
            {
                return null;
            }
            VFXPool result = new VFXPool();
            result.type = other.type;
            List<VFXComplex> effects = new List<VFXComplex>();
            foreach(VFXComplex effect in other.effects)
            {
                VFXComplex newEffect = new VFXComplex();
                List<VFXObject> effects2 = new List<VFXObject>();
                foreach(VFXObject effect2 in effect.effects)
                {
                    VFXObject newEffect2 = new VFXObject();
                    if(effect2.effect != null)
                    {
                        GameObject obj = UnityEngine.Object.Instantiate(effect2.effect);
                        obj.SetActive(false);
                        FakePrefab.MarkAsFakePrefab(obj);
                        UnityEngine.Object.DontDestroyOnLoad(obj);
                        newEffect2.effect = obj;
                    }
                    else
                    {
                        newEffect2.effect = null;
                    }
                    newEffect2.orphaned = effect2.orphaned;
                    newEffect2.attached = effect2.attached;
                    newEffect2.persistsOnDeath = effect2.persistsOnDeath;
                    newEffect2.usesZHeight = effect2.usesZHeight;
                    newEffect2.zHeight = effect2.zHeight;
                    newEffect2.alignment = effect2.alignment;
                    newEffect2.destructible = effect2.destructible;
                    effects2.Add(newEffect2);
                }
                newEffect.effects = effects2.ToArray();
                effects.Add(newEffect);
            }
            result.effects = effects.ToArray();
            return result;
        }

        public static string PathCombine(string a, string b, string c)
        {
            return Path.Combine(Path.Combine(a, b), c);
        }

        public static void SafeMove(string oldPath, string newPath, bool allowOverwritting = false)
        {
            if (File.Exists(oldPath) && (allowOverwritting || !File.Exists(newPath)))
            {
                string contents = SaveManager.ReadAllText(oldPath);
                try
                {
                    SaveManager.WriteAllText(newPath, contents);
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Failed to move {0} to {1}: {2}", new object[]
                    {
                    oldPath,
                    newPath,
                    ex
                    });
                    return;
                }
                try
                {
                    File.Delete(oldPath);
                }
                catch (Exception ex2)
                {
                    Debug.LogErrorFormat("Failed to delete old file {0}: {1}", new object[]
                    {
                    oldPath,
                    newPath,
                    ex2
                    });
                    return;
                }
                if (File.Exists(oldPath + ".bak"))
                {
                    File.Delete(oldPath + ".bak");
                }
            }
        }

        public static void RuntimeDuplicateChunk(IntVector2 basePosition, IntVector2 dimensions, int tilemapExpansion, RoomHandler sourceRoom = null, bool ignoreOtherRoomCells = false)
        {
            Dungeon d = GameManager.Instance.Dungeon;
            FieldInfo inf = typeof(Dungeon).GetField("assembler", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo inf2 = typeof(Dungeon).GetField("m_tilemap", BindingFlags.NonPublic | BindingFlags.Instance);
            int num = tilemapExpansion + 3;
            IntVector2 intVector = new IntVector2(d.data.Width + num, num);
            int newWidth = d.data.Width + num * 2 + dimensions.x;
            int newHeight = Mathf.Max(d.data.Height, dimensions.y + num * 2);
            CellData[][] array = BraveUtility.MultidimensionalArrayResize<CellData>(d.data.cellData, d.data.Width, d.data.Height, newWidth, newHeight);
            d.data.cellData = array;
            d.data.ClearCachedCellData();
            GameObject gameObject = GameObject.Find("_Rooms");
            Transform transform = new GameObject("Room_ChunkDuplicate").transform;
            transform.parent = gameObject.transform;
            for (int i = -num; i < dimensions.x + num; i++)
            {
                for (int j = -num; j < dimensions.y + num; j++)
                {
                    IntVector2 intVector2 = basePosition + new IntVector2(i, j);
                    IntVector2 p = new IntVector2(i, j) + intVector;
                    CellData cellData = (!d.data.CheckInBoundsAndValid(intVector2)) ? null : d.data[intVector2];
                    CellData cellData2 = new CellData(p, CellType.WALL);
                    if (cellData != null && sourceRoom != null && cellData.nearestRoom != sourceRoom)
                    {
                        cellData2.cellVisualData.roomVisualTypeIndex = sourceRoom.RoomVisualSubtype;
                        cellData = null;
                    }
                    if (cellData != null && cellData.isExitCell && ignoreOtherRoomCells)
                    {
                        cellData2.cellVisualData.roomVisualTypeIndex = sourceRoom.RoomVisualSubtype;
                        cellData = null;
                    }
                    cellData2.positionInTilemap = cellData2.positionInTilemap - intVector + new IntVector2(tilemapExpansion, tilemapExpansion);
                    cellData2.parentArea = sourceRoom.area;
                    cellData2.parentRoom = sourceRoom;
                    cellData2.nearestRoom = sourceRoom;
                    cellData2.occlusionData.overrideOcclusion = true;
                    array[p.x][p.y] = cellData2;
                    BraveUtility.DrawDebugSquare(p.ToVector2(), Color.yellow, 1000f);
                    CellType type = (cellData == null) ? CellType.WALL : cellData.type;
                    sourceRoom.RuntimeStampCellComplex(p.x, p.y, type, DiagonalWallType.NONE);
                    if (cellData != null)
                    {
                        cellData2.distanceFromNearestRoom = cellData.distanceFromNearestRoom;
                        cellData2.cellVisualData.CopyFrom(cellData.cellVisualData);
                        if (cellData.cellVisualData.containsLight)
                        {
                            d.data.ReplicateLighting(cellData, cellData2);
                        }
                    }
                }
            }
            GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(BraveResources.Load("RuntimeTileMap", ".prefab"));
            tk2dTileMap component = gameObject2.GetComponent<tk2dTileMap>();
            component.Editor__SpriteCollection = d.tileIndices.dungeonCollection;
            TK2DDungeonAssembler.RuntimeResizeTileMap(component, dimensions.x + tilemapExpansion * 2, dimensions.y + tilemapExpansion * 2, ((tk2dTileMap)inf2.GetValue(d)).partitionSizeX, dimensions.y + tilemapExpansion * 2);
            for (int k = -tilemapExpansion; k < dimensions.x + tilemapExpansion; k++)
            {
                for (int l = -tilemapExpansion; l < dimensions.y + tilemapExpansion; l++)
                {
                    IntVector2 intVector3 = basePosition + new IntVector2(k, l);
                    IntVector2 intVector4 = new IntVector2(k, l) + intVector;
                    bool flag = false;
                    CellData cellData3 = (!d.data.CheckInBoundsAndValid(intVector3)) ? null : d.data[intVector3];
                    if (ignoreOtherRoomCells && cellData3 != null)
                    {
                        bool flag2 = cellData3.isExitCell;
                        if (!flag2 && sourceRoom != null && cellData3.parentRoom != sourceRoom)
                        {
                            flag2 = true;
                        }
                        if (!flag2 && cellData3.IsAnyFaceWall() && d.data.CheckInBoundsAndValid(cellData3.position + new IntVector2(0, -2)) && d.data[cellData3.position + new IntVector2(0, -2)].isExitCell)
                        {
                            flag2 = true;
                        }
                        if (!flag2 && cellData3.type == CellType.WALL && d.data.CheckInBoundsAndValid(cellData3.position + new IntVector2(0, -3)) && d.data[cellData3.position + new IntVector2(0, -3)].isExitCell)
                        {
                            flag2 = true;
                        }
                        if (!flag2 && cellData3.type == CellType.FLOOR && d.data.CheckInBoundsAndValid(cellData3.position + new IntVector2(0, -1)) && (d.data[cellData3.position + new IntVector2(0, -1)].isExitCell || d.data[cellData3.position + 
                            new IntVector2(0, -1)].GetExitNeighbor() != null))
                        {
                            flag2 = true;
                        }
                        if (!flag2 && (cellData3.IsAnyFaceWall() || cellData3.type == CellType.WALL) && cellData3.GetExitNeighbor() != null)
                        {
                            flag2 = true;
                        }
                        if (flag2)
                        {
                            BraveUtility.DrawDebugSquare(intVector4.ToVector2() + new Vector2(0.3f, 0.3f), intVector4.ToVector2() + new Vector2(0.7f, 0.7f), Color.cyan, 1000f);
                            ((TK2DDungeonAssembler)inf.GetValue(d)).BuildTileIndicesForCell(d, component, intVector.x + k, intVector.y + l);
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        if (intVector3.x >= 0 && intVector3.y >= 0)
                        {
                            for (int m = 0; m < component.Layers.Length; m++)
                            {
                                int tile = d.MainTilemap.Layers[m].GetTile(intVector3.x, intVector3.y);
                                component.Layers[m].SetTile(k + tilemapExpansion, l + tilemapExpansion, tile);
                            }
                        }
                    }
                }
            }
            RenderMeshBuilder.CurrentCellXOffset = intVector.x - tilemapExpansion;
            RenderMeshBuilder.CurrentCellYOffset = intVector.y - tilemapExpansion;
            component.Build();
            RenderMeshBuilder.CurrentCellXOffset = 0;
            RenderMeshBuilder.CurrentCellYOffset = 0;
            component.renderData.transform.position = new Vector3((float)(intVector.x - tilemapExpansion), (float)(intVector.y - tilemapExpansion), (float)(intVector.y - tilemapExpansion));
            sourceRoom.OverrideTilemap = component;
            sourceRoom.PostGenerationCleanup();
            DeadlyDeadlyGoopManager.ReinitializeData();
        }

        public static T GetAnyComponent<T>(this GameObject obj)
        {
            T result = obj.GetComponent<T>();
            if(result == null)
            {
                result = obj.GetComponentInChildren<T>();
                if(result == null)
                {
                    result = obj.GetComponentInParent<T>();
                }
            }
            return result;
        }

        public static T GetAnyComponent<T>(this Component component)
        {
            T result = component.GetComponent<T>();
            if (result == null)
            {
                result = component.GetComponentInChildren<T>();
                if (result == null)
                {
                    result = component.GetComponentInParent<T>();
                }
            }
            return result;
        }

        public static tk2dSpriteCollectionData ConstructCollection(GameObject obj, string name)
        {
            tk2dSpriteCollectionData tk2dSpriteCollectionData = obj.AddComponent<tk2dSpriteCollectionData>();
            UnityEngine.Object.DontDestroyOnLoad(tk2dSpriteCollectionData);
            tk2dSpriteCollectionData.assetName = name;
            tk2dSpriteCollectionData.spriteCollectionGUID = name;
            tk2dSpriteCollectionData.spriteCollectionName = name;
            tk2dSpriteCollectionData.spriteDefinitions = new tk2dSpriteDefinition[0];
            return tk2dSpriteCollectionData;
        }

        public static List<tk2dSpriteAnimationClip> GetGunAnimationClips(this Gun gun)
        {
            List<tk2dSpriteAnimationClip> clips = new List<tk2dSpriteAnimationClip>();
            if (!string.IsNullOrEmpty(gun.shootAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation));
            }
            if (!string.IsNullOrEmpty(gun.reloadAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation));
            }
            if (!string.IsNullOrEmpty(gun.emptyReloadAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyReloadAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyReloadAnimation));
            }
            if (!string.IsNullOrEmpty(gun.idleAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.idleAnimation));
            }
            if (!string.IsNullOrEmpty(gun.chargeAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.chargeAnimation));
            }
            if (!string.IsNullOrEmpty(gun.dischargeAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.dischargeAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.dischargeAnimation));
            }
            if (!string.IsNullOrEmpty(gun.emptyAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyAnimation));
            }
            if (!string.IsNullOrEmpty(gun.introAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.introAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.introAnimation));
            }
            if (!string.IsNullOrEmpty(gun.finalShootAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.finalShootAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.finalShootAnimation));
            }
            if (!string.IsNullOrEmpty(gun.enemyPreFireAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.enemyPreFireAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.enemyPreFireAnimation));
            }
            if (!string.IsNullOrEmpty(gun.outOfAmmoAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.outOfAmmoAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.outOfAmmoAnimation));
            }
            if (!string.IsNullOrEmpty(gun.criticalFireAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.criticalFireAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.criticalFireAnimation));
            }
            if (!string.IsNullOrEmpty(gun.dodgeAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.dodgeAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.dodgeAnimation));
            }
            if (!string.IsNullOrEmpty(gun.alternateIdleAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.alternateIdleAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.alternateIdleAnimation));
            }
            if (!string.IsNullOrEmpty(gun.alternateReloadAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.alternateReloadAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.alternateReloadAnimation));
            }
            if (!string.IsNullOrEmpty(gun.alternateShootAnimation) && gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.alternateShootAnimation) != null)
            {
                clips.Add(gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.alternateShootAnimation));
            }
            return clips;
        }

        public static void RegisterBreachShrine(string id, GameObject shrine)
        {
            GungeonAPI.ShrineFactory.breachShrines.Add(id, shrine);
            var instantiatedshrine = UnityEngine.Object.Instantiate(shrine, shrine.GetComponent<BreachShrineController>().offset, Quaternion.identity).GetComponent<BreachShrineController>();
            instantiatedshrine.instantiated = true;
            var interactable = instantiatedshrine.GetComponent<IPlayerInteractable>();
            if (!RoomHandler.unassignedInteractableObjects.Contains(interactable))
                RoomHandler.unassignedInteractableObjects.Add(interactable);
        }

        public static void Add<T>(this T[] array, T toAdd)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = toAdd;
        }

        public static tk2dTileMap DestroyWallAtPosition(int ix, int iy, bool deferRebuild = true, RoomHandler overrideParentRoom = null)
        {
            Dungeon d = GameManager.Instance.Dungeon;
            if (d.data.cellData[ix][iy] == null)
            {
                return null;
            }
            if (d.data.cellData[ix][iy].type != CellType.WALL)
            {
                return null;
            }
            if (!d.data.cellData[ix][iy].breakable)
            {
                return null;
            }
            d.data.cellData[ix][iy].type = CellType.FLOOR;
            FieldInfo inf = typeof(Dungeon).GetField("assembler", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo inf2 = typeof(Dungeon).GetField("m_tilemap", BindingFlags.NonPublic | BindingFlags.Instance);
            RoomHandler parentRoom = d.data.cellData[ix][iy].parentRoom;
            tk2dTileMap tk2dTileMap = (parentRoom == null || !(parentRoom.OverrideTilemap != null)) ? ((overrideParentRoom == null || !(overrideParentRoom.OverrideTilemap != null)) ? (tk2dTileMap)inf2.GetValue(d) : overrideParentRoom.OverrideTilemap) : 
                parentRoom.OverrideTilemap;
            CellData cellData = d.data.cellData[ix][iy];
            if (cellData != null)
            {
                cellData.hasBeenGenerated = false;
                if (cellData.parentRoom != null)
                {
                    List<GameObject> list = new List<GameObject>();
                    for (int k = 0; k < cellData.parentRoom.hierarchyParent.childCount; k++)
                    {
                        Transform child = cellData.parentRoom.hierarchyParent.GetChild(k);
                        if (child.name.StartsWith("Chunk_"))
                        {
                            list.Add(child.gameObject);
                        }
                    }
                    for (int l = list.Count - 1; l >= 0; l--)
                    {
                        UnityEngine.Object.Destroy(list[l]);
                    }
                }
                try
                {
                    DoItsThing(d, tk2dTileMap, cellData.position.x, cellData.position.y);
                    ((TK2DDungeonAssembler)inf.GetValue(d)).BuildTileIndicesForCell(d, tk2dTileMap, cellData.position.x, cellData.position.y);
                }
                catch { }
                cellData.HasCachedPhysicsTile = false;
                cellData.CachedPhysicsTile = null;
            }
            if (!deferRebuild)
            {
                d.RebuildTilemap(tk2dTileMap);
            }
            return tk2dTileMap;
        }

        public static void DoItsThing(Dungeon d, tk2dTileMap map, int ix, int iy)
        {
            CellData cellData = null;
            int x = -1;
            int y = -1;
            try
            {
                cellData = (!d.data.CheckInBoundsAndValid(ix, iy)) ? null : d.data[ix, iy];
                x = (cellData == null) ? ix : cellData.positionInTilemap.x;
                y = (cellData == null) ? iy : cellData.positionInTilemap.y;
            }
            catch
            {
                return;
            }
            try
            {
                try
                {

                    for (int i = 0; i < map.Layers.Length; i++)
                    {
                        try
                        {
                            map.Layers[i].SetTile(x, y, -1);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
                try
                {
                    if (TK2DTilemapChunkAnimator.PositionToAnimatorMap.ContainsKey(cellData.positionInTilemap))
                    {
                        for (int j = 0; j < TK2DTilemapChunkAnimator.PositionToAnimatorMap[cellData.positionInTilemap].Count; j++)
                        {
                            TilemapAnimatorTileManager tilemapAnimatorTileManager = TK2DTilemapChunkAnimator.PositionToAnimatorMap[cellData.positionInTilemap][j];
                            if (tilemapAnimatorTileManager.animator)
                            {
                                TK2DTilemapChunkAnimator.PositionToAnimatorMap[cellData.positionInTilemap].RemoveAt(j);
                                j--;
                                UnityEngine.Object.Destroy(tilemapAnimatorTileManager.animator.gameObject);
                                tilemapAnimatorTileManager.animator = null;
                            }
                        }
                    }
                }
                catch
                {
                    ETGModConsole.Log("problem in if thingy");
                }
            }
            catch
            {
                ETGModConsole.Log("problem in ending thingy");
            }
        }

        public static GameObject ProcessGameObject(this GameObject obj)
        {
            obj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(obj);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            return obj;
        }

        public static T CopyFields<T>(Projectile sample2) where T : Projectile
        {
            T sample = sample2.gameObject.AddComponent<T>();
            sample.PossibleSourceGun = sample2.PossibleSourceGun;
            sample.SpawnedFromOtherPlayerProjectile = sample2.SpawnedFromOtherPlayerProjectile;
            sample.PlayerProjectileSourceGameTimeslice = sample2.PlayerProjectileSourceGameTimeslice;
            sample.BulletScriptSettings = sample2.BulletScriptSettings;
            sample.damageTypes = sample2.damageTypes;
            sample.allowSelfShooting = sample2.allowSelfShooting;
            sample.collidesWithPlayer = sample2.collidesWithPlayer;
            sample.collidesWithProjectiles = sample2.collidesWithProjectiles;
            sample.collidesOnlyWithPlayerProjectiles = sample2.collidesOnlyWithPlayerProjectiles;
            sample.projectileHitHealth = sample2.projectileHitHealth;
            sample.collidesWithEnemies = sample2.collidesWithEnemies;
            sample.shouldRotate = sample2.shouldRotate;
            sample.shouldFlipVertically = sample2.shouldFlipVertically;
            sample.shouldFlipHorizontally = sample2.shouldFlipHorizontally;
            sample.ignoreDamageCaps = sample2.ignoreDamageCaps;
            sample.baseData = sample2.baseData;
            sample.AppliesPoison = sample2.AppliesPoison;
            sample.PoisonApplyChance = sample2.PoisonApplyChance;
            sample.healthEffect = sample2.healthEffect;
            sample.AppliesSpeedModifier = sample2.AppliesSpeedModifier;
            sample.SpeedApplyChance = sample2.SpeedApplyChance;
            sample.speedEffect = sample2.speedEffect;
            sample.AppliesCharm = sample2.AppliesCharm;
            sample.CharmApplyChance = sample2.CharmApplyChance;
            sample.charmEffect = sample2.charmEffect;
            sample.AppliesFreeze = sample2.AppliesFreeze;
            sample.FreezeApplyChance = sample2.FreezeApplyChance;
            sample.freezeEffect = (sample2.freezeEffect);
            sample.AppliesFire = sample2.AppliesFire;
            sample.FireApplyChance = sample2.FireApplyChance;
            sample.fireEffect = (sample2.fireEffect);
            sample.AppliesStun = sample2.AppliesStun;
            sample.StunApplyChance = sample2.StunApplyChance;
            sample.AppliedStunDuration = sample2.AppliedStunDuration;
            sample.AppliesBleed = sample2.AppliesBleed;
            sample.bleedEffect = (sample2.bleedEffect);
            sample.AppliesCheese = sample2.AppliesCheese;
            sample.CheeseApplyChance = sample2.CheeseApplyChance;
            sample.cheeseEffect = (sample2.cheeseEffect);
            sample.BleedApplyChance = sample2.BleedApplyChance;
            sample.CanTransmogrify = sample2.CanTransmogrify;
            sample.ChanceToTransmogrify = sample2.ChanceToTransmogrify;
            sample.TransmogrifyTargetGuids = sample2.TransmogrifyTargetGuids;
            sample.BossDamageMultiplier = sample2.BossDamageMultiplier;
            sample.SpawnedFromNonChallengeItem = sample2.SpawnedFromNonChallengeItem;
            sample.TreatedAsNonProjectileForChallenge = sample2.TreatedAsNonProjectileForChallenge;
            sample.hitEffects = sample2.hitEffects;
            sample.CenterTilemapHitEffectsByProjectileVelocity = sample2.CenterTilemapHitEffectsByProjectileVelocity;
            sample.wallDecals = sample2.wallDecals;
            sample.persistTime = sample2.persistTime;
            sample.angularVelocity = sample2.angularVelocity;
            sample.angularVelocityVariance = sample2.angularVelocityVariance;
            sample.spawnEnemyGuidOnDeath = sample2.spawnEnemyGuidOnDeath;
            sample.HasFixedKnockbackDirection = sample2.HasFixedKnockbackDirection;
            sample.FixedKnockbackDirection = sample2.FixedKnockbackDirection;
            sample.pierceMinorBreakables = sample2.pierceMinorBreakables;
            sample.objectImpactEventName = sample2.objectImpactEventName;
            sample.enemyImpactEventName = sample2.enemyImpactEventName;
            sample.onDestroyEventName = sample2.onDestroyEventName;
            sample.additionalStartEventName = sample2.additionalStartEventName;
            sample.IsRadialBurstLimited = sample2.IsRadialBurstLimited;
            sample.MaxRadialBurstLimit = sample2.MaxRadialBurstLimit;
            sample.AdditionalBurstLimits = sample2.AdditionalBurstLimits;
            sample.AppliesKnockbackToPlayer = sample2.AppliesKnockbackToPlayer;
            sample.PlayerKnockbackForce = sample2.PlayerKnockbackForce;
            sample.HasDefaultTint = sample2.HasDefaultTint;
            sample.DefaultTintColor = sample2.DefaultTintColor;
            sample.IsCritical = sample2.IsCritical;
            sample.BlackPhantomDamageMultiplier = sample2.BlackPhantomDamageMultiplier;
            sample.PenetratesInternalWalls = sample2.PenetratesInternalWalls;
            sample.neverMaskThis = sample2.neverMaskThis;
            sample.isFakeBullet = sample2.isFakeBullet;
            sample.CanBecomeBlackBullet = sample2.CanBecomeBlackBullet;
            sample.TrailRenderer = sample2.TrailRenderer;
            sample.CustomTrailRenderer = sample2.CustomTrailRenderer;
            sample.ParticleTrail = sample2.ParticleTrail;
            sample.DelayedDamageToExploders = sample2.DelayedDamageToExploders;
            sample.OnHitEnemy = sample2.OnHitEnemy;
            sample.OnWillKillEnemy = sample2.OnWillKillEnemy;
            sample.OnBecameDebris = sample2.OnBecameDebris;
            sample.OnBecameDebrisGrounded = sample2.OnBecameDebrisGrounded;
            sample.IsBlackBullet = sample2.IsBlackBullet;
            sample.statusEffectsToApply = sample2.statusEffectsToApply;
            sample.AdditionalScaleMultiplier = sample2.AdditionalScaleMultiplier;
            sample.ModifyVelocity = sample2.ModifyVelocity;
            sample.CurseSparks = sample2.CurseSparks;
            sample.PreMoveModifiers = sample2.PreMoveModifiers;
            sample.OverrideMotionModule = sample2.OverrideMotionModule;
            sample.Shooter = sample2.Shooter;
            sample.Owner = sample2.Owner;
            sample.Speed = sample2.Speed;
            sample.Direction = sample2.Direction;
            sample.DestroyMode = sample2.DestroyMode;
            sample.Inverted = sample2.Inverted;
            sample.LastVelocity = sample2.LastVelocity;
            sample.ManualControl = sample2.ManualControl;
            sample.ForceBlackBullet = sample2.ForceBlackBullet;
            sample.IsBulletScript = sample2.IsBulletScript;
            sample.OverrideTrailPoint = sample2.OverrideTrailPoint;
            sample.SkipDistanceElapsedCheck = sample2.SkipDistanceElapsedCheck;
            sample.ImmuneToBlanks = sample2.ImmuneToBlanks;
            sample.ImmuneToSustainedBlanks = sample2.ImmuneToSustainedBlanks;
            sample.ForcePlayerBlankable = sample2.ForcePlayerBlankable;
            sample.IsReflectedBySword = sample2.IsReflectedBySword;
            sample.LastReflectedSlashId = sample2.LastReflectedSlashId;
            sample.TrailRendererController = sample2.TrailRendererController;
            sample.braveBulletScript = sample2.braveBulletScript;
            sample.TrapOwner = sample2.TrapOwner;
            sample.SuppressHitEffects = sample2.SuppressHitEffects;
            UnityEngine.Object.Destroy(sample2);
            return sample;
        }

        public static void AddDualWieldSynergyProcessor(int id1, int id2, string synergyName)
        {
            AdvancedDualWieldSynergyProcessor dualWieldController = PickupObjectDatabase.GetById(id1).gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            dualWieldController.SynergyNameToCheck = synergyName;
            dualWieldController.PartnerGunID = id2;
            AdvancedDualWieldSynergyProcessor dualWieldController2 = PickupObjectDatabase.GetById(id2).gameObject.AddComponent<AdvancedDualWieldSynergyProcessor>();
            dualWieldController2.SynergyNameToCheck = synergyName;
            dualWieldController2.PartnerGunID = id1;
        }

        public static void NullOrNotNull<T>(T obj)
        {
            if(obj != null)
            {
                ETGModConsole.Log("Not Null");
            }
            else if(obj == null)
            {
                ETGModConsole.Log("Null");
            }
        }

        public static void RemoveOffset(this tk2dSpriteDefinition def)
        {
            def.MakeOffset(-def.position0);
        }

        public static VFXPool CreateMuzzleflash(string name, List<string> spriteNames, int fps, List<IntVector2> spriteSizes, List<tk2dBaseSprite.Anchor> anchors, List<Vector2> manualOffsets, bool orphaned, bool attached, bool persistsOnDeath,
            bool usesZHeight, float zHeight, VFXAlignment alignment, bool destructible, List<float> emissivePowers, List<Color> emissiveColors)
        {
            VFXPool pool = new VFXPool();
            pool.type = VFXPoolType.All;
            VFXComplex complex = new VFXComplex();
            VFXObject vfObj = new VFXObject();
            GameObject obj = new GameObject(name);
            obj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(obj);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            tk2dSprite sprite = obj.AddComponent<tk2dSprite>();
            tk2dSpriteAnimator animator = obj.AddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
            clip.fps = fps;
            clip.frames = new tk2dSpriteAnimationFrame[0];
            for (int i = 0; i < spriteNames.Count; i++)
            {
                string spriteName = spriteNames[i];
                IntVector2 spriteSize = spriteSizes[i];
                tk2dBaseSprite.Anchor anchor = anchors[i];
                Vector2 manualOffset = manualOffsets[i];
                float emissivePower = emissivePowers[i];
                Color emissiveColor = emissiveColors[i];
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteId = Toolbox.VFXCollection.GetSpriteIdByName(spriteName);
                tk2dSpriteDefinition def = Toolbox.SetupDefinitionForShellSprite(spriteName, frame.spriteId, spriteSize.x, spriteSize.y);
                def.ConstructOffsetsFromAnchor(anchor, def.position3);
                def.MakeOffset(manualOffset);
                def.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                def.material.SetFloat("_EmissiveColorPower", emissivePower);
                def.material.SetColor("_EmissiveColor", emissiveColor);
                def.materialInst.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
                def.materialInst.SetFloat("_EmissiveColorPower", emissivePower);
                def.materialInst.SetColor("_EmissiveColor", emissiveColor);
                frame.spriteCollection = Toolbox.VFXCollection;
                clip.frames = clip.frames.Concat(new tk2dSpriteAnimationFrame[] { frame }).ToArray();
            }
            sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitTk2dCustomFalloffTintableTiltedCutoutEmissive");
            sprite.renderer.material.SetFloat("_EmissiveColorPower", emissivePowers[0]);
            sprite.renderer.material.SetColor("_EmissiveColor", emissiveColors[0]);
            clip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            clip.name = "start";
            animator.spriteAnimator.Library = animator.gameObject.AddComponent<tk2dSpriteAnimation>();
            animator.spriteAnimator.Library.clips = new tk2dSpriteAnimationClip[] { clip };
            animator.spriteAnimator.Library.enabled = true;
            SpriteAnimatorKiller kill = animator.gameObject.AddComponent<SpriteAnimatorKiller>();
            kill.fadeTime = -1f;
            kill.animator = animator;
            kill.delayDestructionTime = -1f;
            vfObj.orphaned = orphaned;
            vfObj.attached = attached;
            vfObj.persistsOnDeath = persistsOnDeath;
            vfObj.usesZHeight = usesZHeight;
            vfObj.zHeight = zHeight;
            vfObj.alignment = alignment;
            vfObj.destructible = destructible;
            vfObj.effect = obj;
            complex.effects = new VFXObject[] { vfObj };
            pool.effects = new VFXComplex[] { complex };
            animator.playAutomatically = true;
            animator.DefaultClipId = animator.GetClipIdByName("start");
            return pool;
        }

        public static DungeonFlowNode GenerateFlowNode(DungeonFlow flow, PrototypeDungeonRoom.RoomCategory category, PrototypeDungeonRoom overrideRoom = null, GenericRoomTable overrideRoomTable = null, bool loopTargetIsOneWay = false, bool isWarpWing = false, 
            bool handlesOwnWarping = true, float weight = 1f, DungeonFlowNode.NodePriority priority = DungeonFlowNode.NodePriority.MANDATORY, string guid = "")
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = Guid.NewGuid().ToString();
            }
            DungeonFlowNode node = new DungeonFlowNode(flow)
            {
                isSubchainStandin = false,
                nodeType = DungeonFlowNode.ControlNodeType.ROOM,
                roomCategory = category,
                percentChance = weight,
                priority = priority,
                overrideExactRoom = overrideRoom,
                overrideRoomTable = overrideRoomTable,
                capSubchain = false,
                subchainIdentifier = string.Empty,
                limitedCopiesOfSubchain = false,
                maxCopiesOfSubchain = 1,
                subchainIdentifiers = new List<string>(0),
                receivesCaps = false,
                isWarpWingEntrance = isWarpWing,
                handlesOwnWarping = handlesOwnWarping,
                forcedDoorType = DungeonFlowNode.ForcedDoorType.NONE,
                loopForcedDoorType = DungeonFlowNode.ForcedDoorType.NONE,
                nodeExpands = false,
                initialChainPrototype = "n",
                chainRules = new List<ChainRule>(0),
                minChainLength = 3,
                maxChainLength = 8,
                minChildrenToBuild = 1,
                maxChildrenToBuild = 1,
                canBuildDuplicateChildren = false,
                guidAsString = guid,
                parentNodeGuid = string.Empty,
                childNodeGuids = new List<string>(0),
                loopTargetNodeGuid = string.Empty,
                loopTargetIsOneWay = loopTargetIsOneWay,
                flow = flow
            };
            return node;
        }

        public static Vector2 GetPlayerAimPosition(this PlayerController player)
        {
            BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(player.PlayerIDX);
            bool flag2 = instanceForPlayer == null;
            if (!flag2)
            {
                bool flag3 = instanceForPlayer.IsKeyboardAndMouse(false);
                Vector2 a;
                if (flag3)
                {
                    a = player.unadjustedAimPoint.XY();
                }
                else
                {
                    bool flag4 = instanceForPlayer.ActiveActions == null;
                    if (flag4)
                    {
                        return Vector2.zero;
                    }
                    a = instanceForPlayer.ActiveActions.Aim.Vector;
                }
                return a;
            }
            return Vector2.zero;
        }

        public static GameObject CreateSprenSpun(Vector2 position)
        {
            return UnityEngine.Object.Instantiate(SprenSpunPrefab, position.ToVector3ZUp(0), Quaternion.identity);
        }

        public static void RemovePassiveItem(this PlayerController self, PassiveItem passive)
        {
            int num = self.passiveItems.IndexOf(passive);
            if (num >= 0)
            {
                TakeableEnemy.RemovePassiveItemAt(self, num);
            }
        }

        public static PassiveItem AcquirePassiveItemPrefabDirectlyForFakePrefabs(this PlayerController player, PassiveItem item)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(item.gameObject, Vector2.zero, Quaternion.identity);
            PassiveItem component = gameObject.GetComponent<PassiveItem>();
            EncounterTrackable component2 = component.GetComponent<EncounterTrackable>();
            if (component2 != null)
            {
                component2.DoNotificationOnEncounter = false;
            }
            component.suppressPickupVFX = true;
            component.Pickup(player);
            return component;
        }

        public static void NormalizeAngle(this float self)
        {
            while(self < 0)
            {
                self += 360;
            }
            while(self > 359)
            {
                self -= 360;
            }
        }

        public static void RemovePassiveItemAt(this PlayerController self, int index)
        {
            PassiveItem passiveItem = self.passiveItems[index];
            self.passiveItems.RemoveAt(index);
            GameUIRoot.Instance.RemovePassiveItemFromDock(passiveItem);
            UnityEngine.Object.Destroy(passiveItem);
            self.stats.RecalculateStats(self, false, false);
        }

        public static void LogComponents(GameObject obj, bool logNormalComponents = true, bool logComponentsInChildren = true, bool logComponentsInParent = true)
        {
            if (logNormalComponents)
            {
                ETGModConsole.Log("---------------------COMPONENTS:-------------------");
                foreach (Component component in obj.GetComponents<Component>())
                {
                    ETGModConsole.Log(component.GetType().ToString());
                }
            }
            if (logComponentsInChildren)
            {
                ETGModConsole.Log("---------------------IN CHILDREN:-------------------");
                foreach (Component component in obj.GetComponentsInChildren<Component>())
                {
                    ETGModConsole.Log(component.GetType().ToString());
                }
            }
            if (logComponentsInParent)
            {
                ETGModConsole.Log("---------------------IN PARENT:-------------------");
                foreach (Component component in obj.GetComponentsInParent<Component>())
                {
                    ETGModConsole.Log(component.GetType().ToString());
                }
            }
        }

        public static void LogComponents(Component obj, bool logNormalComponents = true, bool logComponentsInChildren = true, bool logComponentsInParent = true)
        {
            if (logNormalComponents)
            {
                ETGModConsole.Log("---------------------COMPONENTS:-------------------");
                foreach (Component component in obj.GetComponents<Component>())
                {
                    ETGModConsole.Log(component.GetType().ToString());
                }
            }
            if (logComponentsInChildren)
            {
                ETGModConsole.Log("---------------------IN CHILDREN:-------------------");
                foreach (Component component in obj.GetComponentsInChildren<Component>())
                {
                    ETGModConsole.Log(component.GetType().ToString());
                }
            }
            if (logComponentsInParent)
            {
                ETGModConsole.Log("---------------------IN PARENT:-------------------");
                foreach (Component component in obj.GetComponentsInParent<Component>())
                {
                    ETGModConsole.Log(component.GetType().ToString());
                }
            }
        }

        public static void ShaderProjModifierProperties(ShaderProjModifier mod)
        {
            ETGModConsole.Log("ProcessProperty: " + mod.ProcessProperty.ToString());
            ETGModConsole.Log("ShaderProperty: " + mod.ShaderProperty);
            ETGModConsole.Log("StartValue: " + mod.StartValue.ToString());
            ETGModConsole.Log("EndValue: " + mod.EndValue.ToString());
            ETGModConsole.Log("LerpTime: " + mod.LerpTime.ToString());
            ETGModConsole.Log("ColorAttribute: " + mod.ColorAttribute.ToString());
            if (mod.StartColor != null)
            {
                ETGModConsole.Log("StartColor: " + mod.StartColor.ToString());
            }
            else
            {
                ETGModConsole.Log("StartColor is null.");
            }
            if (mod.EndColor != null)
            {
                ETGModConsole.Log("EndColor: " + mod.EndColor.ToString());
            }
            else
            {
                ETGModConsole.Log("EndColor is null.");
            }
            ETGModConsole.Log("OnDeath: " + mod.OnDeath.ToString());
            ETGModConsole.Log("PreventCorpse: " + mod.PreventCorpse.ToString());
            ETGModConsole.Log("DisablesOutlines: " + mod.DisablesOutlines.ToString());
            ETGModConsole.Log("EnableEmission: " + mod.EnableEmission.ToString());
            ETGModConsole.Log("GlobalSparks: " + mod.GlobalSparks.ToString());
            if (mod.GlobalSparksColor != null)
            {
                ETGModConsole.Log("GlobalSparksColor: " + mod.GlobalSparksColor.ToString());
            }
            else
            {
                ETGModConsole.Log("GlobalSparksColor is null.");
            }
            ETGModConsole.Log("GlobalSparksForce: " + mod.GlobalSparksForce.ToString());
            ETGModConsole.Log("GlobalSparksOverrideLifespan: " + mod.GlobalSparksOverrideLifespan.ToString());
            ETGModConsole.Log("AddMaterialPass: " + mod.AddMaterialPass.ToString());
            if (mod.AddPass != null)
            {
                ETGModConsole.Log("AddPass: " + mod.AddPass.name);
            }
            else
            {
                ETGModConsole.Log("AddPass is null.");
            }
            ETGModConsole.Log("IsGlitter: " + mod.IsGlitter.ToString());
            ETGModConsole.Log("ShouldAffectBosses: " + mod.ShouldAffectBosses.ToString());
            ETGModConsole.Log("AddsEncircler: " + mod.AddsEncircler.ToString());
            ETGModConsole.Log("AppliesLocalSlowdown: " + mod.AppliesLocalSlowdown.ToString());
            ETGModConsole.Log("LocalTimescaleMultiplier: " + mod.LocalTimescaleMultiplier.ToString());
            ETGModConsole.Log("AppliesParticleSystem: " + mod.AppliesParticleSystem.ToString());
            if (mod.ParticleSystemToSpawn != null)
            {
                ETGModConsole.Log("ParticleSystemToSpawn: " + mod.ParticleSystemToSpawn.name);
            }
            else
            {
                ETGModConsole.Log("ParticleSystemToSpawn is null.");
            }
            ETGModConsole.Log("GlobalSparkType: " + mod.GlobalSparkType.ToString());
            ETGModConsole.Log("GlobalSparksRepeat: " + mod.GlobalSparksRepeat.ToString());
        }

        public static void LogValues(object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                ETGModConsole.Log(propertyInfo.Name + ": " + propertyInfo.GetValue(obj, null).ToString());
            }
            FieldInfo[] fields = obj.GetType().GetFields();
            foreach (FieldInfo fieldInfo in fields)
            {
                ETGModConsole.Log(fieldInfo.Name + ": " + fieldInfo.GetValue(obj).ToString());
            }
        }

        public static void DefaultChargeProjectileProperties(int id)
        {
            ProjectileModule.ChargeProjectile chargeProj = Toolbox.GetGunById(id).DefaultModule.chargeProjectiles[0];
            ETGModConsole.Log("chargeTime: " + chargeProj.ChargeTime.ToString());
            if (chargeProj.Projectile != null)
            {
                ETGModConsole.Log("Projectile.name: " + chargeProj.Projectile.name);
            }
            else
            {
                ETGModConsole.Log("Projectile is null");
            }
            ETGModConsole.Log("UsedProperties: " + chargeProj.UsedProperties.ToString());
            ETGModConsole.Log("AmmoCost: " + chargeProj.AmmoCost.ToString());
            if (chargeProj.VfxPool != null)
            {
                ETGModConsole.Log("VfxPool.type: " + chargeProj.VfxPool.type.ToString());
            }
            else
            {
                ETGModConsole.Log("VfxPool is null");
            }
            ETGModConsole.Log("LightIntensity: " + chargeProj.LightIntensity.ToString());
            if (chargeProj.ScreenShake != null)
            {
                ETGModConsole.Log("ScreenShake.time: " + chargeProj.ScreenShake.time.ToString());
            }
            else
            {
                ETGModConsole.Log("ScreenShake is null");
            }
            ETGModConsole.Log("OverrideShootAnimation: " + chargeProj.OverrideShootAnimation);
            if (chargeProj.OverrideMuzzleFlashVfxPool != null)
            {
                ETGModConsole.Log("OverrideMuzzleFlashVfxPool.type: " + chargeProj.OverrideMuzzleFlashVfxPool.type.ToString());
            }
            else
            {
                ETGModConsole.Log("OverrideMuzzleFlashVfxPool is null");
            }
            ETGModConsole.Log("MegaReflection: " + chargeProj.MegaReflection.ToString());
            ETGModConsole.Log("AdditionalWwizeEvent: " + chargeProj.AdditionalWwiseEvent.ToString());
        }

        public static void Beams()
        {
            foreach (string text in Game.Items.IDs)
            {
                PickupObject obj = Game.Items[text];
                if (obj != null && obj is Gun)
                {
                    Gun gun = obj as Gun;
                    if (gun.Volley != null)
                    {
                        int i = 0;
                        foreach (ProjectileModule mod in gun.Volley.projectiles)
                        {
                            if (mod.shootStyle == ProjectileModule.ShootStyle.Beam)
                            {
                                foreach (Projectile projectile in mod.projectiles)
                                {
                                    if (projectile != null && projectile.GetComponents<BeamController>() != null)
                                    {
                                        foreach (BeamController controller in projectile.GetComponents<BeamController>())
                                        {
                                            if (controller is BasicBeamController && (controller as BasicBeamController).boneType == BasicBeamController.BeamBoneType.Straight)
                                            {
                                                ETGModConsole.Log(gun.DisplayName + "(volley's projectilemodule #" + i + " projectile " + projectile.name + ") " + controller.GetType().ToString());
                                                controller.AdjustPlayerBeamTint(Color.red, 0, 0);
                                            }
                                        }
                                    }
                                }
                            }
                            i++;
                        }
                    }
                    else
                    {
                        ProjectileModule mod = gun.singleModule;
                        foreach (Projectile projectile in mod.projectiles)
                        {
                            if (projectile != null && projectile.GetComponents<BeamController>() != null)
                            {
                                foreach (BeamController controller in projectile.GetComponents<BeamController>())
                                {
                                    if (controller is BasicBeamController && (controller as BasicBeamController).boneType == BasicBeamController.BeamBoneType.Straight)
                                    {
                                        ETGModConsole.Log(gun.DisplayName + " ( single module projecilte" + projectile.name + ") " + controller.GetType().ToString());
                                    }
                                }
                            }
                        }
                    }
                    if (gun.alternateVolley != null)
                    {
                        int i = 0;
                        foreach (ProjectileModule mod in gun.alternateVolley.projectiles)
                        {
                            if (mod.shootStyle == ProjectileModule.ShootStyle.Beam)
                            {
                                foreach (Projectile projectile in mod.projectiles)
                                {
                                    if (projectile != null && projectile.GetComponents<BeamController>() != null)
                                    {
                                        foreach (BeamController controller in projectile.GetComponents<BeamController>())
                                        {
                                            if (controller is BasicBeamController && (controller as BasicBeamController).boneType == BasicBeamController.BeamBoneType.Straight)
                                            {
                                                ETGModConsole.Log(gun.DisplayName + " (alt volley's projectilemodule #" + i + " projectile " + projectile.name + ") " + controller.GetType().ToString());
                                            }
                                        }
                                    }
                                }
                            }
                            i++;
                        }
                    }
                }
            }
        }

        public static GameActorEffect CopyEffectFrom(this GameActorEffect self, GameActorEffect other)
        {
            if (self == null || other == null)
            {
                return null;
            }
            self.AffectsPlayers = other.AffectsPlayers;
            self.AffectsEnemies = other.AffectsEnemies;
            self.effectIdentifier = other.effectIdentifier;
            self.resistanceType = other.resistanceType;
            self.stackMode = other.stackMode;
            self.duration = other.duration;
            self.maxStackedDuration = other.maxStackedDuration;
            self.AppliesTint = other.AppliesTint;
            self.TintColor = new Color
            {
                r = other.TintColor.r,
                g = other.TintColor.g,
                b = other.TintColor.b
            };
            self.AppliesDeathTint = other.AppliesDeathTint;
            self.DeathTintColor = new Color
            {
                r = other.DeathTintColor.r,
                g = other.DeathTintColor.g,
                b = other.DeathTintColor.b
            };
            self.AppliesOutlineTint = other.AppliesOutlineTint;
            self.OutlineTintColor = new Color
            {
                r = other.OutlineTintColor.r,
                g = other.OutlineTintColor.g,
                b = other.OutlineTintColor.b
            };
            self.OverheadVFX = other.OverheadVFX;
            self.PlaysVFXOnActor = other.PlaysVFXOnActor;
            return self;
        }

        public static bool PlayerHasCompletionGun(this PlayerController player)
        {
            bool result = false;
            if(player != null && player.inventory != null && player.inventory.AllGuns != null)
            {
                foreach(Gun gun in player.inventory.AllGuns)
                {
                    if (gun.GetComponent<LichsFavoriteController>() != null)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        public static bool PlayerHasCompletionItem(this PlayerController player)
        {
            bool result = false;
            if(player != null && player.passiveItems != null)
            {
                foreach(PassiveItem passive in player.passiveItems)
                {
                    if(passive is SynergyCompletionItem)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        public static void SetFlagForPlayer(this PlayerController player, Type toSet)
        {
            if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
            {
                PassiveItem.ActiveFlagItems.Add(player, new Dictionary<Type, int>());
            }
            if (!PassiveItem.ActiveFlagItems[player].ContainsKey(toSet))
            {
                PassiveItem.ActiveFlagItems[player].Add(toSet, 1);
            }
            else
            {
                PassiveItem.ActiveFlagItems[player][toSet] = PassiveItem.ActiveFlagItems[player][toSet] + 1;
            }
        }

        public static void Remove<T>(ref T[] array, T toRemove)
        {
            List<T> list = array.ToList();
            list.Remove(toRemove);
            array = list.ToArray<T>();
        }

        public static void Add<T>(ref T[] array, T toAdd)
        {
            List<T> list = array.ToList();
            list.Add(toAdd);
            array = list.ToArray<T>();
        }

        public static bool Contains<T>(this T[] array, T item)
        {
            return array.ToList().Contains(item);
        }

        public static void Clear<T>(ref T[] array)
        {
            List<T> list = array.ToList();
            list.Clear();
            array = list.ToArray();
        }

        public static float Distance(float a, float b)
        {
            return (a - b);
        }

        public static float Normalize(this float self)
        {
            float magnitude = Mathf.Abs(self);
            if (magnitude > 1E-05f)
            {
                self /= magnitude;
            }
            else
            {
                self = 0f;
            }
            return self;
        }

        public static void UnsetFlagForPlayer(this PlayerController player, Type toUnset)
        {
            if (PassiveItem.ActiveFlagItems[player].ContainsKey(toUnset))
            {
                PassiveItem.ActiveFlagItems[player][toUnset] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][toUnset] - 1);
                if (PassiveItem.ActiveFlagItems[player][toUnset] == 0)
                {
                    PassiveItem.ActiveFlagItems[player].Remove(toUnset);
                }
            }
        }

        public static bool PlayerHasActiveSynergy(this PlayerController player, string synergyNameToCheck)
        {
            foreach(int index in player.ActiveExtraSynergies)
            {
                AdvancedSynergyEntry synergy = GameManager.Instance.SynergyManager.synergies[index];
                if(synergy.NameKey == synergyNameToCheck)
                {
                    return true;
                }
            }
            return false;
        }

        public static tk2dSpriteDefinition CopyDefinitionFrom(this tk2dSpriteDefinition other)
        {
            tk2dSpriteDefinition result = new tk2dSpriteDefinition
            {
                boundsDataCenter = new Vector3
                {
                    x = other.boundsDataCenter.x,
                    y = other.boundsDataCenter.y,
                    z = other.boundsDataCenter.z
                },
                boundsDataExtents = new Vector3
                {
                    x = other.boundsDataExtents.x,
                    y = other.boundsDataExtents.y,
                    z = other.boundsDataExtents.z
                },
                colliderConvex = other.colliderConvex,
                colliderSmoothSphereCollisions = other.colliderSmoothSphereCollisions,
                colliderType = other.colliderType,
                colliderVertices = other.colliderVertices,
                collisionLayer = other.collisionLayer,
                complexGeometry = other.complexGeometry,
                extractRegion = other.extractRegion,
                flipped = other.flipped,
                indices = other.indices,
                material = new Material(other.material),
                materialId = other.materialId,
                materialInst = new Material(other.materialInst),
                metadata = other.metadata,
                name = other.name,
                normals = other.normals,
                physicsEngine = other.physicsEngine,
                position0 = new Vector3
                {
                    x = other.position0.x,
                    y = other.position0.y,
                    z = other.position0.z
                },
                position1 = new Vector3
                {
                    x = other.position1.x,
                    y = other.position1.y,
                    z = other.position1.z
                },
                position2 = new Vector3
                {
                    x = other.position2.x,
                    y = other.position2.y,
                    z = other.position2.z
                },
                position3 = new Vector3
                {
                    x = other.position3.x,
                    y = other.position3.y,
                    z = other.position3.z
                },
                regionH = other.regionH,
                regionW = other.regionW,
                regionX = other.regionX,
                regionY = other.regionY,
                tangents = other.tangents,
                texelSize = new Vector2
                {
                    x = other.texelSize.x,
                    y = other.texelSize.y
                },
                untrimmedBoundsDataCenter = new Vector3
                {
                    x = other.untrimmedBoundsDataCenter.x,
                    y = other.untrimmedBoundsDataCenter.y,
                    z = other.untrimmedBoundsDataCenter.z
                },
                untrimmedBoundsDataExtents = new Vector3
                {
                    x = other.untrimmedBoundsDataExtents.x,
                    y = other.untrimmedBoundsDataExtents.y,
                    z = other.untrimmedBoundsDataExtents.z
                }
            };
            List<Vector2> uvs = new List<Vector2>();
            foreach(Vector2 vector in other.uvs)
            {
                uvs.Add(new Vector2 
                {
                    x = vector.x,
                    y = vector.y
                });
            }
            result.uvs = uvs.ToArray();
            List<Vector3> colliderVertices = new List<Vector3>();
            foreach (Vector3 vector in other.colliderVertices)
            {
                colliderVertices.Add(new Vector3
                {
                    x = vector.x,
                    y = vector.y,
                    z = vector.z
                });
            }
            result.colliderVertices = colliderVertices.ToArray();
            return result;
        }

        public static void RemoveComponent<T>(this GameObject self) where T : Component
        {
            if (self.GetComponent<T>() != null)
            {
                UnityEngine.Object.Destroy(self.GetComponent<T>());
            }
        }

        public static void RemoveComponent<T>(this Component self) where T : Component
        {
            if(self.GetComponent<T>() != null)
            {
                UnityEngine.Object.Destroy(self.GetComponent<T>());
            }
        }

        public static tk2dSpriteDefinition FrameToDefinition(this tk2dSpriteAnimationFrame frame)
        {
            return frame.spriteCollection.spriteDefinitions[frame.spriteId];
        }

        public static BagelColliderData GetBagelColliders(int spriteId, tk2dSpriteCollectionData collection)
        {
            int num = collection.SpriteIDsWithBagelColliders.IndexOf(spriteId);
            if (num >= 0)
            {
                return collection.SpriteDefinedBagelColliders[num];
            }
            return null;
        }

        public static bool ModdedItemExists(string name)
        {
            return ETGMod.Databases.Items[name] != null;
        }

        public static int GetModdedItemId(string name)
        {
            if(ETGMod.Databases.Items[name] == null)
            {
                return -1;
            }
            return ETGMod.Databases.Items[name].PickupObjectId;
        }

        public static bool OwnerHasSynergy(this Gun gun, string synergyName)
        {
            return gun.CurrentOwner is PlayerController && (gun.CurrentOwner as PlayerController).PlayerHasActiveSynergy(synergyName);
        }

        public static void SetProjectileSpriteRight(this Projectile proj, string name, int pixelWidth, int pixelHeight, bool lightened = true, tk2dBaseSprite.Anchor anchor = tk2dBaseSprite.Anchor.LowerLeft, bool anchorChangesCollider = true,
            bool fixesScale = false, int? overrideColliderPixelWidth = null, int? overrideColliderPixelHeight = null, int? overrideColliderOffsetX = null, int? overrideColliderOffsetY = null, Projectile overrideProjectileToCopyFrom = null)
        {
            try
            {
                proj.GetAnySprite().spriteId = ETGMod.Databases.Items.ProjectileCollection.inst.GetSpriteIdByName(name);
                tk2dSpriteDefinition def = SetupDefinitionForProjectileSprite(name, proj.GetAnySprite().spriteId, pixelWidth, pixelHeight, lightened, overrideColliderPixelWidth, overrideColliderPixelHeight, overrideColliderOffsetX,
                    overrideColliderOffsetY, overrideProjectileToCopyFrom);
                def.ConstructOffsetsFromAnchor(anchor, def.position3, fixesScale, anchorChangesCollider);
                proj.GetAnySprite().scale = new Vector3(1f, 1f, 1f);
                proj.transform.localScale = new Vector3(1f, 1f, 1f);
                proj.GetAnySprite().transform.localScale = new Vector3(1f, 1f, 1f);
                proj.AdditionalScaleMultiplier = 1f;
            }
            catch (Exception ex)
            {
                ETGModConsole.Log("Ooops! Seems like something got very, Very, VERY wrong. Here's the exception:");
                ETGModConsole.Log(ex.ToString());
            }
        }

        public static void AnimateProjectile(this Projectile proj, List<string> names, int fps, bool loops, List<IntVector2> pixelSizes, List<bool> lighteneds, List<tk2dBaseSprite.Anchor> anchors, List<bool> anchorsChangeColliders,
            List<bool> fixesScales, List<Vector3?> manualOffsets, List<IntVector2?> overrideColliderPixelSizes, List<IntVector2?> overrideColliderOffsets, List<Projectile> overrideProjectilesToCopyFrom)
        {
            tk2dSpriteAnimationClip clip = new tk2dSpriteAnimationClip();
            clip.name = "idle";
            clip.fps = fps;
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();
            for (int i = 0; i < names.Count; i++)
            {
                string name = names[i];
                IntVector2 pixelSize = pixelSizes[i];
                IntVector2? overrideColliderPixelSize = overrideColliderPixelSizes[i];
                IntVector2? overrideColliderOffset = overrideColliderOffsets[i];
                Vector3? manualOffset = manualOffsets[i];
                bool anchorChangesCollider = anchorsChangeColliders[i];
                bool fixesScale = fixesScales[i];
                if (!manualOffset.HasValue)
                {
                    manualOffset = new Vector2?(Vector2.zero);
                }
                tk2dBaseSprite.Anchor anchor = anchors[i];
                bool lightened = lighteneds[i];
                Projectile overrideProjectileToCopyFrom = overrideProjectilesToCopyFrom[i];
                tk2dSpriteAnimationFrame frame = new tk2dSpriteAnimationFrame();
                frame.spriteId = ETGMod.Databases.Items.ProjectileCollection.inst.GetSpriteIdByName(name);
                frame.spriteCollection = ETGMod.Databases.Items.ProjectileCollection;
                frames.Add(frame);
                int? overrideColliderPixelWidth = null;
                int? overrideColliderPixelHeight = null;
                if (overrideColliderPixelSize.HasValue)
                {
                    overrideColliderPixelWidth = overrideColliderPixelSize.Value.x;
                    overrideColliderPixelHeight = overrideColliderPixelSize.Value.y;
                }
                int? overrideColliderOffsetX = null;
                int? overrideColliderOffsetY = null;
                if (overrideColliderOffset.HasValue)
                {
                    overrideColliderOffsetX = overrideColliderOffset.Value.x;
                    overrideColliderOffsetY = overrideColliderOffset.Value.y;
                }
                tk2dSpriteDefinition def = SetupDefinitionForProjectileSprite(name, frame.spriteId, pixelSize.x, pixelSize.y, lightened, overrideColliderPixelWidth, overrideColliderPixelHeight, overrideColliderOffsetX, overrideColliderOffsetY,
                    overrideProjectileToCopyFrom);
                def.ConstructOffsetsFromAnchor(anchor, def.position3, fixesScale, anchorChangesCollider);
                def.position0 += manualOffset.Value;
                def.position1 += manualOffset.Value;
                def.position2 += manualOffset.Value;
                def.position3 += manualOffset.Value;
                if (i == 0)
                {
                    proj.GetAnySprite().SetSprite(frame.spriteCollection, frame.spriteId);
                }
            }
            clip.wrapMode = loops ? tk2dSpriteAnimationClip.WrapMode.Loop : tk2dSpriteAnimationClip.WrapMode.Once;
            clip.frames = frames.ToArray();
            if (proj.sprite.spriteAnimator == null)
            {
                proj.sprite.spriteAnimator = proj.sprite.gameObject.AddComponent<tk2dSpriteAnimator>();
            }
            proj.sprite.spriteAnimator.playAutomatically = true;
            bool flag = proj.sprite.spriteAnimator.Library == null;
            if (flag)
            {
                proj.sprite.spriteAnimator.Library = proj.sprite.spriteAnimator.gameObject.AddComponent<tk2dSpriteAnimation>();
                proj.sprite.spriteAnimator.Library.clips = new tk2dSpriteAnimationClip[0];
                proj.sprite.spriteAnimator.Library.enabled = true;
            }
            proj.sprite.spriteAnimator.Library.clips = proj.sprite.spriteAnimator.Library.clips.Concat(new tk2dSpriteAnimationClip[] { clip }).ToArray();
            proj.sprite.spriteAnimator.DefaultClipId = proj.sprite.spriteAnimator.Library.GetClipIdByName("idle");
            proj.sprite.spriteAnimator.deferNextStartClip = false;
        }

        public static void MakeOffset(this tk2dSpriteDefinition def, Vector2 offset, bool changesCollider = false)
        {
            float xOffset = offset.x;
            float yOffset = offset.y;
            def.position0 += new Vector3(xOffset, yOffset, 0);
            def.position1 += new Vector3(xOffset, yOffset, 0);
            def.position2 += new Vector3(xOffset, yOffset, 0);
            def.position3 += new Vector3(xOffset, yOffset, 0);
            def.boundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.boundsDataExtents += new Vector3(xOffset, yOffset, 0);
            def.untrimmedBoundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.untrimmedBoundsDataExtents += new Vector3(xOffset, yOffset, 0);
            if(def.colliderVertices != null && def.colliderVertices.Length > 0 && changesCollider)
            {
                def.colliderVertices[0] += new Vector3(xOffset, yOffset, 0);
            }
        }

        public static void ConstructOffsetsFromAnchor(this tk2dSpriteDefinition def, tk2dBaseSprite.Anchor anchor, Vector2? scale = null, bool fixesScale = false, bool changesCollider = true)
        {
            if (!scale.HasValue)
            {
                scale = new Vector2?(def.position3);
            }
            if (fixesScale)
            {
                Vector2 fixedScale = scale.Value - def.position0.XY();
                scale = new Vector2?(fixedScale);
            }
            float xOffset = 0;
            if (anchor == tk2dBaseSprite.Anchor.LowerCenter || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.UpperCenter)
            {
                xOffset = -(scale.Value.x / 2f);
            }
            else if (anchor == tk2dBaseSprite.Anchor.LowerRight || anchor == tk2dBaseSprite.Anchor.MiddleRight || anchor == tk2dBaseSprite.Anchor.UpperRight)
            {
                xOffset = -scale.Value.x;
            }
            float yOffset = 0;
            if (anchor == tk2dBaseSprite.Anchor.MiddleLeft || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.MiddleLeft)
            {
                yOffset = -(scale.Value.y / 2f);
            }
            else if (anchor == tk2dBaseSprite.Anchor.UpperLeft || anchor == tk2dBaseSprite.Anchor.UpperCenter || anchor == tk2dBaseSprite.Anchor.UpperRight)
            {
                yOffset = -scale.Value.y;
            }
            def.MakeOffset(new Vector2(xOffset, yOffset), changesCollider);
        }

        public static tk2dSpriteDefinition SetupDefinitionForProjectileSprite(string name, int id, int pixelWidth, int pixelHeight, bool lightened = true, int? overrideColliderPixelWidth = null, int? overrideColliderPixelHeight = null,
            int? overrideColliderOffsetX = null, int? overrideColliderOffsetY = null, Projectile overrideProjectileToCopyFrom = null)
        {
            if (overrideColliderPixelWidth == null)
            {
                overrideColliderPixelWidth = pixelWidth;
            }
            if (overrideColliderPixelHeight == null)
            {
                overrideColliderPixelHeight = pixelHeight;
            }
            if(overrideColliderOffsetX == null)
            {
                overrideColliderOffsetX = 0;
            }
            if(overrideColliderOffsetY == null)
            {
                overrideColliderOffsetY = 0;
            }
            float thing = 16;
            float thing2 = 16;
            float trueWidth = (float)pixelWidth / thing;
            float trueHeight = (float)pixelHeight / thing;
            float colliderWidth = (float)overrideColliderPixelWidth.Value / thing2;
            float colliderHeight = (float)overrideColliderPixelHeight.Value / thing2;
            float colliderOffsetX = (float)overrideColliderOffsetX.Value / thing2;
            float colliderOffsetY = (float)overrideColliderOffsetY.Value / thing2;
            tk2dSpriteDefinition def = ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[(overrideProjectileToCopyFrom ??
                    (PickupObjectDatabase.GetById(lightened ? 47 : 12) as Gun).DefaultModule.projectiles[0]).GetAnySprite().spriteId].CopyDefinitionFrom();
            def.boundsDataCenter = new Vector3(trueWidth / 2f, trueHeight / 2f, 0f);
            def.boundsDataExtents = new Vector3(trueWidth, trueHeight, 0f); 
            def.untrimmedBoundsDataCenter = new Vector3(trueWidth / 2f, trueHeight / 2f, 0f);
            def.untrimmedBoundsDataExtents = new Vector3(trueWidth, trueHeight, 0f);
            def.texelSize = new Vector2(1 / 16f, 1 / 16f);
            def.position0 = new Vector3(0f, 0f, 0f);
            def.position1 = new Vector3(0f + trueWidth, 0f, 0f);
            def.position2 = new Vector3(0f, 0f + trueHeight, 0f);
            def.position3 = new Vector3(0f + trueWidth, 0f + trueHeight, 0f);
            def.colliderVertices[0].x = colliderOffsetX;
            def.colliderVertices[0].y = colliderOffsetY;
            def.colliderVertices[1].x = colliderWidth;
            def.colliderVertices[1].y = colliderHeight;
            def.name = name;
            ETGMod.Databases.Items.ProjectileCollection.inst.spriteDefinitions[id] = def;
            return def;
        }

        public static Vector2[] GenerateUVs(int x, int y, int width, int height)
		{
			return new Vector2[]
			{
				new Vector2((float)x / (float)width, (float)y / (float)height),
				new Vector2((float)(x + width) / (float)width, (float)y / (float)height),
				new Vector2((float)x / (float)width, (float)(y + height) / (float)height),
				new Vector2((float)(x + width) / (float)width, (float)(y + height) / (float)height)
			};
		}

        public static GameObject CreateCustomClip(string spriteName, int pixelWidth, int pixelHeight)
        {
            GameObject clip = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(95) as Gun).clipObject);
            clip.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(clip.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(clip);
            clip.GetComponent<tk2dBaseSprite>().spriteId = Toolbox.VFXCollection.inst.GetSpriteIdByName(spriteName);
            Toolbox.SetupDefinitionForClipSprite(spriteName, clip.GetComponent<tk2dBaseSprite>().spriteId, pixelWidth, pixelHeight);
            return clip;
        }

        public static void SetupDefinitionForClipSprite(string name, int id, int pixelWidth, int pixelHeight)
        {
            float thing = 14;
            float trueWidth = (float)pixelWidth / thing;
            float trueHeight = (float)pixelHeight / thing;
            tk2dSpriteDefinition def = Toolbox.VFXCollection.inst.spriteDefinitions[(PickupObjectDatabase.GetById(47) as Gun).clipObject.GetComponent<tk2dBaseSprite>().spriteId].CopyDefinitionFrom();
            def.boundsDataCenter = new Vector3(trueWidth / 2f, trueHeight / 2f, 0f);
            def.boundsDataExtents = new Vector3(trueWidth, trueHeight, 0f);
            def.untrimmedBoundsDataCenter = new Vector3(trueWidth / 2f, trueHeight / 2f, 0f);
            def.untrimmedBoundsDataExtents = new Vector3(trueWidth, trueHeight, 0f);
            def.position0 = new Vector3(0f, 0f, 0f);
            def.position1 = new Vector3(0f + trueWidth, 0f, 0f);
            def.position2 = new Vector3(0f, 0f + trueHeight, 0f);
            def.position3 = new Vector3(0f + trueWidth, 0f + trueHeight, 0f);
            def.colliderVertices[1].x = trueWidth;
            def.colliderVertices[1].y = trueHeight;
            def.name = name;
            Toolbox.VFXCollection.spriteDefinitions[id] = def;
        }

        public static GameObject CreateCustomShellCasing(string spriteName, int pixelWidth, int pixelHeight)
        {
            GameObject casing = UnityEngine.Object.Instantiate((PickupObjectDatabase.GetById(202) as Gun).shellCasing);
            casing.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(casing.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(casing);
            casing.GetComponent<tk2dBaseSprite>().spriteId = Toolbox.VFXCollection.inst.GetSpriteIdByName(spriteName);
            Toolbox.SetupDefinitionForShellSprite(spriteName, casing.GetComponent<tk2dBaseSprite>().spriteId, pixelWidth, pixelHeight);
            return casing;
        }

        public static tk2dSpriteDefinition SetupDefinitionForShellSprite(string name, int id, int pixelWidth, int pixelHeight, tk2dSpriteDefinition overrideToCopyFrom = null)
        {
            float thing = 14;
            float trueWidth = (float)pixelWidth / thing;
            float trueHeight = (float)pixelHeight / thing;
            tk2dSpriteDefinition def = overrideToCopyFrom ?? Toolbox.VFXCollection.inst.spriteDefinitions[(PickupObjectDatabase.GetById(202) as Gun).shellCasing.GetComponent<tk2dBaseSprite>().spriteId].CopyDefinitionFrom();
            def.boundsDataCenter = new Vector3(trueWidth / 2f, trueHeight / 2f, 0f);
            def.boundsDataExtents = new Vector3(trueWidth, trueHeight, 0f);
            def.untrimmedBoundsDataCenter = new Vector3(trueWidth / 2f, trueHeight / 2f, 0f);
            def.untrimmedBoundsDataExtents = new Vector3(trueWidth, trueHeight, 0f);
            def.position0 = new Vector3(0f, 0f, 0f);
            def.position1 = new Vector3(0f + trueWidth, 0f, 0f);
            def.position2 = new Vector3(0f, 0f + trueHeight, 0f);
            def.position3 = new Vector3(0f + trueWidth, 0f + trueHeight, 0f);
            def.name = name;
            Toolbox.VFXCollection.spriteDefinitions[id] = def;
            return def;
        }

        public static bool AnyoneHasActiveSynergy(string synergy, out int count)
        {
            count = 0;
            for (int i = 0; i < GameManager.Instance.AllPlayers.Length; i++)
            {
                if (!GameManager.Instance.AllPlayers[i].IsGhost)
                {
                    count += GameManager.Instance.AllPlayers[i].CountActiveBonusSynergies(synergy);
                }
            }
            return count > 0;
        }

        public static void AddItemToSynergy(this PickupObject obj, CustomSynergyType type)
        {
            Toolbox.AddItemToSynergy(type, obj.PickupObjectId);
        }

        public static void AddItemToSynergy(this PickupObject obj, string nameKey)
        {
            Toolbox.AddItemToSynergy(nameKey, obj.PickupObjectId);
        }

        public static void AddItemToSynergy(CustomSynergyType type, int id)
        {
            foreach(AdvancedSynergyEntry entry in GameManager.Instance.SynergyManager.synergies)
            {
                if (entry.bonusSynergies.Contains(type))
                {
                    if(PickupObjectDatabase.GetById(id) != null)
                    {
                        PickupObject obj = PickupObjectDatabase.GetById(id);
                        if(obj is Gun)
                        {
                            if (entry.OptionalGunIDs != null)
                            {
                                entry.OptionalGunIDs.Add(id);
                            }
                            else
                            {
                                entry.OptionalGunIDs = new List<int> { id };
                            }
                        }
                        else
                        {
                            if (entry.OptionalItemIDs != null)
                            {
                                entry.OptionalItemIDs.Add(id);
                            }
                            else
                            {
                                entry.OptionalItemIDs = new List<int> { id };
                            }
                        }
                    }
                }
            }
        }

        public static void AddItemToSynergy(string nameKey, int id)
        {
            foreach (AdvancedSynergyEntry entry in GameManager.Instance.SynergyManager.synergies)
            {
                if (entry.NameKey == nameKey)
                {
                    if (PickupObjectDatabase.GetById(id) != null)
                    {
                        PickupObject obj = PickupObjectDatabase.GetById(id);
                        if (obj is Gun)
                        {
                            if (entry.OptionalGunIDs != null)
                            {
                                entry.OptionalGunIDs.Add(id);
                            }
                            else
                            {
                                entry.OptionalGunIDs = new List<int> { id };
                            }
                        }
                        else
                        {
                            if (entry.OptionalItemIDs != null)
                            {
                                entry.OptionalItemIDs.Add(id);
                            }
                            else
                            {
                                entry.OptionalItemIDs = new List<int> { id };
                            }
                        }
                    }
                }
            }
        }

        public static int CountActiveBonusSynergies(this PlayerController player, string synergy)
        {
            if(player == null)
            {
                return 0;
            }
            int num = 0;
            foreach (int index in player.ActiveExtraSynergies)
            {
                AdvancedSynergyEntry entry = GameManager.Instance.SynergyManager.synergies[index];
                if(entry.NameKey == synergy)
                {
                    num++;
                }
            }
            return num;
        }

        public static List<T> ConstructListOfSameValues<T>(T value, int length)
        {
            List<T> list = new List<T>();
            for(int i = 0; i<length; i++)
            {
                list.Add(value);
            }
            return list;
        }

        public static StatModifier SetupStatModifier(PlayerStats.StatType statType, float modificationAmount, StatModifier.ModifyMethod modifyMethod = StatModifier.ModifyMethod.ADDITIVE, bool breaksOnDamage = false)
        {
            return new StatModifier()
            {
                statToBoost = statType,
                amount = modificationAmount,
                modifyType = modifyMethod,
                isMeatBunBuff = breaksOnDamage
            };
        }

        public static Gun GetGunById(this PickupObjectDatabase database, int id)
        {
            return (PickupObjectDatabase.GetById(id) as Gun);
        }

        public static Gun GetGunById(int id)
        {
            return Toolbox.GetGunById(null, id);
        }

        public static ExplosionData CopyExplosionData(this ExplosionData other)
        {
            return new ExplosionData
            {
                useDefaultExplosion = other.useDefaultExplosion,
                doDamage = other.doDamage,
                forceUseThisRadius = other.forceUseThisRadius,
                damageRadius = other.damageRadius,
                damageToPlayer = other.damageToPlayer,
                damage = other.damage,
                breakSecretWalls = other.breakSecretWalls,
                secretWallsRadius = other.secretWallsRadius,
                forcePreventSecretWallDamage = other.forcePreventSecretWallDamage,
                doDestroyProjectiles = other.doDestroyProjectiles,
                doForce = other.doForce,
                pushRadius = other.pushRadius,
                force = other.force,
                debrisForce = other.debrisForce,
                preventPlayerForce = other.preventPlayerForce,
                explosionDelay = other.explosionDelay,
                usesComprehensiveDelay = other.usesComprehensiveDelay,
                comprehensiveDelay = other.comprehensiveDelay,
                effect = other.effect,
                doScreenShake = other.doScreenShake,
                ss = new ScreenShakeSettings
                {
                    magnitude = other.ss.magnitude,
                    speed = other.ss.speed,
                    time = other.ss.time,
                    falloff = other.ss.falloff,
                    direction = new Vector2
                    {
                        x = other.ss.direction.x,
                        y = other.ss.direction.y
                    },
                    vibrationType = other.ss.vibrationType,
                    simpleVibrationTime = other.ss.simpleVibrationTime,
                    simpleVibrationStrength = other.ss.simpleVibrationStrength
                },
                doStickyFriction = other.doStickyFriction,
                doExplosionRing = other.doExplosionRing,
                isFreezeExplosion = other.isFreezeExplosion,
                freezeRadius = other.freezeRadius,
                freezeEffect = other.freezeEffect,
                playDefaultSFX = other.playDefaultSFX,
                IsChandelierExplosion = other.IsChandelierExplosion,
                rotateEffectToNormal = other.rotateEffectToNormal,
                ignoreList = other.ignoreList,
                overrideRangeIndicatorEffect = other.overrideRangeIndicatorEffect
            };
        }

        public static GameActorCharmEffect CopyCharmFrom(this GameActorCharmEffect self, GameActorCharmEffect other)
        {
            if (self == null)
            {
                self = new GameActorCharmEffect();
            }
            return (GameActorCharmEffect)self.CopyEffectFrom(other);
        }

        public static GameActorFireEffect CopyFireFrom(this GameActorFireEffect self, GameActorFireEffect other)
        {
            if (self == null)
            {
                self = new GameActorFireEffect();
            }
            if (other == null)
            {
                return self;
            }
            self = (GameActorFireEffect)CopyEffectFrom(self, other);
            self.FlameVfx = new List<GameObject>();
            foreach (GameObject flame in other.FlameVfx)
            {
                self.FlameVfx.Add(flame);
            }
            self.flameNumPerSquareUnit = other.flameNumPerSquareUnit;
            self.flameBuffer = new Vector2
            {
                x = other.flameBuffer.x,
                y = other.flameBuffer.y
            };
            self.flameFpsVariation = other.flameFpsVariation;
            self.flameMoveChance = other.flameMoveChance;
            self.IsGreenFire = other.IsGreenFire;
            return self;
        }

        public static GameActorHealthEffect CopyPoisonFrom(this GameActorHealthEffect self, GameActorHealthEffect other)
        {
            if (self == null)
            {
                self = new GameActorHealthEffect();
            }
            if (other == null)
            {
                return null;
            }
            self.CopyEffectFrom(other);
            self.DamagePerSecondToEnemies = other.DamagePerSecondToEnemies;
            self.ignitesGoops = other.ignitesGoops;
            return self;
        }

        public static GameActorSpeedEffect CopySpeedFrom(this GameActorSpeedEffect self, GameActorSpeedEffect other)
        {
            if (self == null)
            {
                self = new GameActorSpeedEffect();
            }
            if (other == null)
            {
                return null;
            }
            self.CopyEffectFrom(other);
            self.SpeedMultiplier = other.SpeedMultiplier;
            self.CooldownMultiplier = other.CooldownMultiplier;
            self.OnlyAffectPlayerWhenGrounded = other.OnlyAffectPlayerWhenGrounded;
            return self;
        }

        public static GameActorFreezeEffect CopyFreezeFrom(this GameActorFreezeEffect self, GameActorFreezeEffect other)
        {
            if (self == null)
            {
                self = new GameActorFreezeEffect();
            }
            if (other == null)
            {
                return null;
            }
            self.CopyEffectFrom(other);
            self.FreezeAmount = other.FreezeAmount;
            self.UnfreezeDamagePercent = other.UnfreezeDamagePercent;
            self.FreezeCrystals = new List<GameObject>();
            foreach (GameObject crystal in other.FreezeCrystals)
            {
                self.FreezeCrystals.Add(crystal);
            }
            self.crystalNum = other.crystalNum;
            self.crystalRot = other.crystalRot;
            self.crystalVariation = new Vector2
            {
                x = other.crystalVariation.x,
                y = other.crystalVariation.y
            };
            self.debrisMinForce = other.debrisMinForce;
            self.debrisMaxForce = other.debrisMaxForce;
            self.debrisAngleVariance = other.debrisAngleVariance;
            self.vfxExplosion = other.vfxExplosion;
            return self;
        }

        public static GameActorBleedEffect CopyBleedFrom(this GameActorBleedEffect self, GameActorBleedEffect other)
        {
            if (self == null)
            {
                self = new GameActorBleedEffect();
            }
            if (other == null)
            {
                return null;
            }
            self.CopyEffectFrom(other);
            self.ChargeAmount = other.ChargeAmount;
            self.ChargeDispelFactor = other.ChargeDispelFactor;
            self.vfxChargingReticle = other.vfxChargingReticle;
            self.vfxExplosion = other.vfxExplosion;
            return self;
        }

        public static GameActorCheeseEffect CopyCheeseFrom(this GameActorCheeseEffect self, GameActorCheeseEffect other)
        {
            if (self == null)
            {
                self = new GameActorCheeseEffect();
            }
            if (other == null)
            {
                return null;
            }
            self.CopyEffectFrom(other);
            self.CheeseAmount = other.CheeseAmount;
            self.CheeseGoop = other.CheeseGoop;
            self.CheeseGoopRadius = other.CheeseGoopRadius;
            self.CheeseCrystals = new List<GameObject>();
            foreach (GameObject crystal in other.CheeseCrystals)
            {
                self.CheeseCrystals.Add(crystal);
            }
            self.crystalNum = other.crystalNum;
            self.crystalRot = other.crystalRot;
            self.crystalVariation = new Vector2
            {
                x = other.crystalVariation.x,
                y = other.crystalVariation.y
            };
            self.debrisMinForce = other.debrisMinForce;
            self.debrisMaxForce = other.debrisMaxForce;
            self.debrisAngleVariance = other.debrisAngleVariance;
            self.vfxExplosion = other.vfxExplosion;
            return self;
        }

        public static tk2dSpriteCollectionData VFXCollection
        {
            get
            {
                return (PickupObjectDatabase.GetById(95) as Gun).clipObject.GetComponent<tk2dBaseSprite>().Collection;
            }
        }

        public static void SetupUnlockOnFlag(this PickupObject self, GungeonFlags flag, bool requiredFlagValue)
        {
            EncounterTrackable encounterTrackable = self.encounterTrackable;
            if (encounterTrackable.prerequisites == null)
            {
                encounterTrackable.prerequisites = new DungeonPrerequisite[1];
                encounterTrackable.prerequisites[0] = new DungeonPrerequisite
                {
                    prerequisiteType = DungeonPrerequisite.PrerequisiteType.FLAG,
                    requireFlag = requiredFlagValue,
                    saveFlagToCheck = flag
                };
            }
            else
            {
                encounterTrackable.prerequisites = encounterTrackable.prerequisites.Concat(new DungeonPrerequisite[] { new DungeonPrerequisite
                {
                    prerequisiteType = DungeonPrerequisite.PrerequisiteType.FLAG,
                    requireFlag = requiredFlagValue,
                    saveFlagToCheck = flag
                }}).ToArray();
            }
            EncounterDatabaseEntry databaseEntry = EncounterDatabase.GetEntry(encounterTrackable.EncounterGuid);
            if (!string.IsNullOrEmpty(databaseEntry.ProxyEncounterGuid))
            {
                databaseEntry.ProxyEncounterGuid = "";
            }
            if (databaseEntry.prerequisites == null)
            {
                databaseEntry.prerequisites = new DungeonPrerequisite[1];
                databaseEntry.prerequisites[0] = new DungeonPrerequisite
                {
                    prerequisiteType = DungeonPrerequisite.PrerequisiteType.FLAG,
                    requireFlag = requiredFlagValue,
                    saveFlagToCheck = flag
                };
            }
            else
            {
                databaseEntry.prerequisites = databaseEntry.prerequisites.Concat(new DungeonPrerequisite[] { new DungeonPrerequisite
                {
                    prerequisiteType = DungeonPrerequisite.PrerequisiteType.FLAG,
                    requireFlag = requiredFlagValue,
                    saveFlagToCheck = flag
                }}).ToArray();
            }
        }

        public static void SetupUnlockOnCustomFlag(this PickupObject self, CustomDungeonFlags flag, bool requiredFlagValue)
        {
            EncounterTrackable encounterTrackable = self.encounterTrackable;
            if (encounterTrackable.prerequisites == null)
            {
                encounterTrackable.prerequisites = new DungeonPrerequisite[1];
                encounterTrackable.prerequisites[0] = new AdvancedDungeonPrerequisite
                {
                    advancedPrerequisiteType = AdvancedDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG,
                    requireCustomFlag = requiredFlagValue,
                    customFlagToCheck = flag
                };
            }
            else
            {
                encounterTrackable.prerequisites = encounterTrackable.prerequisites.Concat(new DungeonPrerequisite[] { new AdvancedDungeonPrerequisite
                {
                    advancedPrerequisiteType = AdvancedDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG,
                    requireCustomFlag = requiredFlagValue,
                    customFlagToCheck = flag
                }}).ToArray();
            }
            EncounterDatabaseEntry databaseEntry = EncounterDatabase.GetEntry(encounterTrackable.EncounterGuid);
            if (!string.IsNullOrEmpty(databaseEntry.ProxyEncounterGuid))
            {
                databaseEntry.ProxyEncounterGuid = "";
            }
            if (databaseEntry.prerequisites == null)
            {
                databaseEntry.prerequisites = new DungeonPrerequisite[1];
                databaseEntry.prerequisites[0] = new AdvancedDungeonPrerequisite
                {
                    advancedPrerequisiteType = AdvancedDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG,
                    requireCustomFlag = requiredFlagValue,
                    customFlagToCheck = flag
                };
            }
            else
            {
                databaseEntry.prerequisites = databaseEntry.prerequisites.Concat(new DungeonPrerequisite[] { new AdvancedDungeonPrerequisite
                {
                    advancedPrerequisiteType = AdvancedDungeonPrerequisite.AdvancedPrerequisiteType.CUSTOM_FLAG,
                    requireCustomFlag = requiredFlagValue,
                    customFlagToCheck = flag
                }}).ToArray();
            }
        }

        public static void CreateAmmoType(string resourcePath, string silhouetteResourcePath)
        {
            dfAtlas.ItemInfo newItem = new dfAtlas.ItemInfo();
            dfAtlas.ItemInfo newItem2 = new dfAtlas.ItemInfo();
            newItem.name = "testeru tex";
            newItem2.name = "testeru tex2";
            newItem.texture = ResourceExtractor.GetTextureFromResource("SpecialItemPack/Resources/BlackGuonStone.png");
            newItem.sizeInPixels = new Vector2(newItem.texture.width, newItem.texture.height);
            newItem2.sizeInPixels = newItem.sizeInPixels;
            Texture2D atlas = GameUIRoot.Instance.heartControllers[0].armorSpritePrefab.Atlas.Texture;
            Rect region = GameUIRoot.Instance.heartControllers[0].extantHearts[0].Atlas.Items[49].region;

            for (int x = 0; x < newItem.texture.width; x++)
            {
                for (int y = 0; y < newItem.texture.height; y++)
                {
                    atlas.SetPixel(x + (int)(region.xMin * 2048), y + (int)(region.yMin * 2048), newItem.texture.GetPixel(x, y));
                }
            }

            for (int x = 0; x < newItem.texture.width; x++)
            {
                for (int y = 0; y < newItem.texture.height; y++)
                {
                    atlas.SetPixel(x + (int)(region.xMin * 2048) + 1 + newItem.texture.width, y + (int)(region.yMin * 2048) + 1 + newItem.texture.height, Color.gray);
                }
            }
            atlas.Apply(false, false);
            newItem.region = new Rect(region.xMin, region.yMin, (float)newItem.texture.width / 2048f, (float)newItem.texture.height / 2048f);
            newItem2.region = new Rect(region.xMin + (float)(1 + newItem.texture.width) / 2048f, region.yMin + (float)(1 + newItem.texture.height) / 2048f, (float)newItem.texture.width / 2048f, (float)newItem.texture.height / 2048f);
            GameUIRoot.Instance.heartControllers[0].extantHearts[0].Atlas.AddItem(newItem);
            GameUIRoot.Instance.heartControllers[0].extantHearts[0].Atlas.AddItem(newItem2);
            Array.Resize(ref GameUIRoot.Instance.ammoControllers[0].ammoTypes, GameUIRoot.Instance.ammoControllers[0].ammoTypes.Length + 1);
            int last_memeber = GameUIRoot.Instance.ammoControllers[0].ammoTypes.Length - 1;
            GameUIAmmoType type = new GameUIAmmoType 
            {
                ammoType = GameUIAmmoType.AmmoType.CUSTOM,
                customAmmoType = "ExampleUIClip"
            };
            GameUIRoot.Instance.ammoControllers[0].ammoTypes[last_memeber] = type;
            GameObject ExampleBG = new GameObject("ExampleBG");
            GameObject ExampleFG = new GameObject("ExampleFG");
            ExampleBG.AddComponent<dfTiledSprite>();
            ExampleFG.AddComponent<dfTiledSprite>();
            type.ammoBarBG = ExampleBG.GetComponent<dfTiledSprite>();
            type.ammoBarFG = ExampleFG.GetComponent<dfTiledSprite>();
            type.ammoBarBG.SpriteName = "testeru tex2";
            type.ammoBarFG.SpriteName = "testeru tex";
        }

        public static Rect AddFaceCardToAtlas(Texture2D tex, Texture2D atlas, int index, Rect bounds)
        {
            int num = (int)(bounds.width / 34f);
            int num2 = (int)(bounds.height / 34f);
            int num3 = index % num;
            int num4 = index / num;
            bool flag = num3 >= num || num4 >= num2;
            Rect result;
            if (flag)
            {
                result = Rect.zero;
            }
            else
            {
                int num5 = (int)bounds.x + num3 * 34;
                int num6 = (int)bounds.y + num4 * 34;
                for (int i = 0; i < tex.width; i++)
                {
                    for (int j = 0; j < tex.height; j++)
                    {
                        atlas.SetPixel(i + num5, j + num6, tex.GetPixel(i, j));
                    }
                }
                atlas.Apply(false, false);
                result = new Rect((float)num5 / (float)atlas.width, (float)num6 / (float)atlas.height, 34f / (float)atlas.width, 34f / (float)atlas.height);
            }
            return result;
        }

        public static void SetupUnlockOnStat(this PickupObject self, TrackedStats stat, DungeonPrerequisite.PrerequisiteOperation operation, int comparisonValue)
        {
            EncounterTrackable encounterTrackable = self.encounterTrackable;
            if (encounterTrackable.prerequisites == null)
            {
                encounterTrackable.prerequisites = new DungeonPrerequisite[1];
                encounterTrackable.prerequisites[0] = new DungeonPrerequisite
                {
                    prerequisiteType = DungeonPrerequisite.PrerequisiteType.COMPARISON,
                    prerequisiteOperation = operation,
                    statToCheck = stat,
                    comparisonValue = comparisonValue
                };
            }
            else
            {
                encounterTrackable.prerequisites = encounterTrackable.prerequisites.Concat(new DungeonPrerequisite[] { new DungeonPrerequisite
                {
                    prerequisiteType = DungeonPrerequisite.PrerequisiteType.COMPARISON,
                    prerequisiteOperation = operation,
                    statToCheck = stat,
                    comparisonValue = comparisonValue
                }}).ToArray();
            }
            EncounterDatabaseEntry databaseEntry = EncounterDatabase.GetEntry(encounterTrackable.EncounterGuid);
            if (!string.IsNullOrEmpty(databaseEntry.ProxyEncounterGuid))
            {
                databaseEntry.ProxyEncounterGuid = "";
            }
            if (databaseEntry.prerequisites == null)
            {
                databaseEntry.prerequisites = new DungeonPrerequisite[1];
                databaseEntry.prerequisites[0] = new DungeonPrerequisite
                {
                    prerequisiteType = DungeonPrerequisite.PrerequisiteType.COMPARISON,
                    prerequisiteOperation = operation,
                    statToCheck = stat,
                    comparisonValue = comparisonValue
                };
            }
            else
            {
                databaseEntry.prerequisites = databaseEntry.prerequisites.Concat(new DungeonPrerequisite[] { new DungeonPrerequisite
                {
                    prerequisiteType = DungeonPrerequisite.PrerequisiteType.COMPARISON,
                    prerequisiteOperation = operation,
                    statToCheck = stat,
                    comparisonValue = comparisonValue
                }}).ToArray();
            }
        }

        public static void SetupUnlockOnEncounter(this PickupObject self, string guid, DungeonPrerequisite.PrerequisiteOperation operation, int comparisonValue)
        {
            EncounterTrackable encounterTrackable = self.encounterTrackable;
            if (encounterTrackable.prerequisites == null)
            {
                encounterTrackable.prerequisites = new DungeonPrerequisite[1];
                encounterTrackable.prerequisites[0] = new DungeonPrerequisite
                {
                    prerequisiteType = DungeonPrerequisite.PrerequisiteType.ENCOUNTER,
                    prerequisiteOperation = operation,
                    encounteredObjectGuid = guid,
                    requiredNumberOfEncounters = comparisonValue
                };
            }
            else
            {
                encounterTrackable.prerequisites = encounterTrackable.prerequisites.Concat(new DungeonPrerequisite[] { new DungeonPrerequisite
                {
                    prerequisiteType = DungeonPrerequisite.PrerequisiteType.ENCOUNTER,
                    prerequisiteOperation = operation,
                    encounteredObjectGuid = guid,
                    requiredNumberOfEncounters = comparisonValue
                }}).ToArray();
            }
            EncounterDatabaseEntry databaseEntry = EncounterDatabase.GetEntry(encounterTrackable.EncounterGuid);
            if (!string.IsNullOrEmpty(databaseEntry.ProxyEncounterGuid))
            {
                databaseEntry.ProxyEncounterGuid = "";
            }
            if (databaseEntry.prerequisites == null)
            {
                databaseEntry.prerequisites = new DungeonPrerequisite[1];
                databaseEntry.prerequisites[0] = new DungeonPrerequisite
                {
                    prerequisiteType = DungeonPrerequisite.PrerequisiteType.ENCOUNTER,
                    prerequisiteOperation = operation,
                    encounteredObjectGuid = guid,
                    requiredNumberOfEncounters = comparisonValue
                };
            }
            else
            {
                databaseEntry.prerequisites = databaseEntry.prerequisites.Concat(new DungeonPrerequisite[] { new DungeonPrerequisite
                {
                    prerequisiteType = DungeonPrerequisite.PrerequisiteType.ENCOUNTER,
                    prerequisiteOperation = operation,
                    encounteredObjectGuid = guid,
                    requiredNumberOfEncounters = comparisonValue
                }}).ToArray();
            }
        }

        public static Toolbox.ChestType GetChestType(this Chest chest)
        {
            if(chest.ChestIdentifier == Chest.SpecialChestIdentifier.SECRET_RAINBOW)
            {
                return ChestType.SECRET_RAINBOW;
            }
            if(chest.ChestIdentifier == Chest.SpecialChestIdentifier.RAT)
            {
                return ChestType.RAT_REWARD;
            }
            if (chest.IsRainbowChest)
            {
                return ChestType.RAINBOW;
            }
            if (chest.IsGlitched)
            {
                return ChestType.GLITCHED;
            }
            if (chest.breakAnimName.Contains("wood"))
            {
                return ChestType.BROWN;
            }
            if (chest.breakAnimName.Contains("silver"))
            {
                return ChestType.BLUE;
            }
            if (chest.breakAnimName.Contains("green"))
            {
                return ChestType.GREEN;
            }
            if (chest.breakAnimName.Contains("redgold"))
            {
                return ChestType.RED;
            }
            if (chest.breakAnimName.Contains("black"))
            {
                return ChestType.BLACK;
            }
            if (chest.breakAnimName.Contains("synergy"))
            {
                return ChestType.SYNERGY;
            }
            return Toolbox.ChestType.UNIDENTIFIED;
        }

        public static void PlaceItemInAmmonomiconAfterItemById(this PickupObject item, int id)
        {
            item.ForcedPositionInAmmonomicon = PickupObjectDatabase.GetById(id).ForcedPositionInAmmonomicon;
        }

        public static T GetNextAvailableEnum<T>() where T : Enum
        {
            long num;
            bool flag = Toolbox.EnumsGiven.TryGetValue(typeof(T), out num);
            T result;
            if (flag)
            {
                num += 1L;
                Toolbox.EnumsGiven[typeof(T)] = num;
                result = (T)((object)Enum.ToObject(typeof(T), num));
            }
            else
            {
                int num2 = -1;
                int[] array = (int[])Enum.GetValues(typeof(T));
                for (int i = 0; i < array.Length; i++)
                {
                    bool flag2 = array[i] > 0;
                    if (flag2)
                    {
                        num2 = array[i];
                    }
                }
                num2++;
                Toolbox.EnumsGiven[typeof(T)] = (long)num2;
                result = (T)((object)Enum.ToObject(typeof(T), num2));
            }
            return result;
        }

        public static Dictionary<Type, long> EnumsGiven = new Dictionary<Type, long>();

        public enum ChestType
        {
            BROWN = 1,
            BLUE = 2,
            GREEN = 3,
            RED = 4,
            BLACK = 5,
            SYNERGY = -1,
            RAINBOW = -2,
            SECRET_RAINBOW = -3,
            GLITCHED = -4,
            RAT_REWARD = -5,
            UNIDENTIFIED = -50
        }

        public static GoopDefinition DefaultWaterGoop;
        public static GoopDefinition DefaultFireGoop;
        public static GoopDefinition DefaultPoisonGoop;
        public static GoopDefinition DefaultCharmGoop;
        public static GoopDefinition DefaultBlobulonGoop;
        public static GoopDefinition DefaultPoopulonGoop;
        public static GoopDefinition DefaultCheeseGoop;
        public static GoopDefinition DefaultGreenFireGoop;
        public static GameObject SprenSpunPrefab;
        public static AssetBundle shared_auto_002;
        public static AssetBundle shared_auto_001;
        public static AssetBundle specialeverything;
        public class CustomBulletScriptSelector : BulletScriptSelector
        {
            public Type bulletType;

            public CustomBulletScriptSelector(Type _bulletType)
            {
                bulletType = _bulletType;
                this.scriptTypeName = bulletType.AssemblyQualifiedName;
            }

            public new Bullet CreateInstance()
            {
                if (bulletType == null)
                {
                    ETGModConsole.Log("Unknown type! " + this.scriptTypeName);
                    return null;
                }
                return (Bullet)Activator.CreateInstance(bulletType);
            }

            public new bool IsNull
            {
                get
                {
                    return string.IsNullOrEmpty(this.scriptTypeName) || this.scriptTypeName == "null";
                }
            }

            public new BulletScriptSelector Clone()
            {
                return new CustomBulletScriptSelector(bulletType);
            }
        }

        public class SprenSpunBehaviour : BraveBehaviour
        {
            private void Update()
            {
                string anim = this.GunChangeMoreAnimation;
                if (this.isBackwards)
                {
                    anim = this.BackchangeAnimation;
                }
                if(this.spriteAnimator != null && !this.spriteAnimator.IsPlaying(anim))
                {
                    this.spriteAnimator.Play(anim);
                }
            }

            public void ChangeDirection(SprenSpunRotateType direction)
            {
                this.isBackwards = direction == SprenSpunRotateType.BACKWARDS;
            }

            public string GunChangeMoreAnimation;
            public string BackchangeAnimation;
            public bool isBackwards = false;
            public enum SprenSpunRotateType
            {
                NORMAL,
                BACKWARDS
            }
        }
    }
}
