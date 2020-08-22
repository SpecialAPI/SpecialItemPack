using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;
using Gungeon;

namespace SpecialItemPack
{
    class YourselfBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Gungeoneer Bullet";
            string resourceName = "SpecialItemPack/Resources/YourselfBullet";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<YourselfBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Be Yourself, Be the Bullet";
            string longDesc = "Shooting launches the owner instead.\n\nSometimes you must just relax and remember your true purpose - being the bullet.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.D;
            item.AddToTrorkShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(288);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.ReceivesTouchDamage = false;
            player.PostProcessProjectile += this.PostProcessProjectile;
        }

        public bool IsAnActualGunProjectile(Projectile proj)
        {
            bool result = false;
            if (proj.PossibleSourceGun != null)
            {
                Gun gun = proj.PossibleSourceGun;
                if (gun.Volley != null)
                {
                    foreach (ProjectileModule mod in gun.Volley.projectiles)
                    {
                        if (mod != null)
                        {
                            if (mod.shootStyle == ProjectileModule.ShootStyle.Charged)
                            {
                                foreach (ProjectileModule.ChargeProjectile chargeProj in mod.chargeProjectiles)
                                {
                                    if (chargeProj != null && chargeProj.Projectile != null && proj.name.Contains(chargeProj.Projectile.name))
                                    {
                                        result = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                foreach (Projectile proj2 in mod.projectiles)
                                {
                                    if (proj2 != null && proj.name.Contains(proj2.name))
                                    {
                                        result = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (result == true)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (gun.singleModule != null)
                    {
                        if (gun.singleModule.shootStyle == ProjectileModule.ShootStyle.Charged)
                        {
                            foreach (ProjectileModule.ChargeProjectile chargeProj in gun.singleModule.chargeProjectiles)
                            {
                                if (chargeProj != null && chargeProj.Projectile != null && proj.name.Contains(chargeProj.Projectile.name))
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (Projectile proj2 in gun.singleModule.projectiles)
                            {
                                if (proj2 != null && proj.name.Contains(proj2.name))
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public void PostProcessProjectile(Projectile proj, float f)
        {
            if(proj.Owner is PlayerController && (proj.Owner as PlayerController) == this.m_owner && this.IsAnActualGunProjectile(proj))
            {
                if (proj != null && !this.isLaunching && proj.GetComponent<NonLaunchBehaviour>() == null && proj.GetComponent<TigerProjectile>() == null)
                {
                    this.isLaunching = true;
                    if (proj.sprite != null)
                    {
                        if (proj.sprite.renderer != null)
                        {
                            proj.sprite.renderer.enabled = false;
                        }
                    }
                    this.m_owner.StartCoroutine(this.WaitAndStart(proj, this.m_owner, this.m_owner.CurrentGun.CurrentAngle));
                }
                else if (proj != null && this.isLaunching && proj.GetComponent<NonLaunchBehaviour>() == null && proj.GetComponent<TigerProjectile>() == null)
                {
                    this.damage += proj.baseData.damage / 2f;
                    Destroy(proj.gameObject);
                }
            }
        }

        public IEnumerator WaitAndStart(Projectile proj, PlayerController player, float deg)
        {
            yield return null;
            try
            {
                this.damage = proj.baseData.damage * 3;
                if (player.stats.GetStatValue(PlayerStats.StatType.DodgeRollDamage) > 0)
                {
                    this.damage *= player.stats.GetStatValue(PlayerStats.StatType.DodgeRollDamage);
                }
                this.speed = proj.baseData.speed;
                if (speed <= 0)
                {
                    this.speed = 1f;
                }
                this.damageTypes = proj.damageTypes;
                this.ignoreDamageCaps = proj.ignoreDamageCaps;
                this.AppliesBleed = proj.AppliesBleed;
                this.BleedApplyChance = proj.BleedApplyChance;
                this.bleedEffect = proj.bleedEffect;
                this.AppliesCharm = proj.AppliesCharm;
                this.CharmApplyChance = proj.CharmApplyChance;
                this.charmEffect = proj.charmEffect;
                this.AppliesCheese = proj.AppliesCheese;
                this.CheeseApplyChance = proj.CheeseApplyChance;
                this.cheeseEffect = proj.cheeseEffect;
                this.AppliesFire = proj.AppliesFire;
                this.FireApplyChance = proj.FireApplyChance;
                this.fireEffect = proj.fireEffect;
                this.AppliesFreeze = proj.AppliesFreeze;
                this.FreezeApplyChance = proj.FreezeApplyChance;
                this.freezeEffect = proj.freezeEffect;
                this.AppliesPoison = proj.AppliesPoison;
                this.PoisonApplyChance = proj.PoisonApplyChance;
                this.healthEffect = proj.healthEffect;
                this.AppliesSpeedModifier = proj.AppliesSpeedModifier;
                this.SpeedApplyChance = proj.SpeedApplyChance;
                this.speedEffect = proj.speedEffect;
                this.AppliesStun = proj.AppliesStun;
                this.StunApplyChance = proj.StunApplyChance;
                this.AppliedStunDuration = proj.AppliedStunDuration;
                this.statusEffectsToApply = proj.statusEffectsToApply;
                this.CanTransmogrify = proj.CanTransmogrify;
                this.ChanceToTransmogrify = proj.ChanceToTransmogrify;
                this.TransmogrifyTargetGuids = proj.TransmogrifyTargetGuids;
                this.isDoingLivingGunSynergy = false;
                if (player.passiveItems != null)
                {
                    bool found = false;
                    foreach (PassiveItem passive in player.passiveItems)
                    {
                        if (passive is ModifyProjectileOnEnemyImpact)
                        {
                            found = true;
                            this.bounceOffEnemies = true;
                        }
                    }
                    if (!found)
                    {
                        this.bounceOffEnemies = false;
                    }
                }
                try
                {
                    this.appliedEffects = proj.GetComponents<AppliedEffectBase>();
                }
                catch
                {
                    this.appliedEffects = new AppliedEffectBase[0];
                }
                try
                {
                    if (proj.GetComponent<PierceProjModifier>() != null)
                    {
                        this.penetration = proj.GetComponent<PierceProjModifier>().penetration;
                    }
                    else
                    {
                        this.penetration = 0;
                    }
                }
                catch
                {
                    this.penetration = 0;
                }
                try
                {
                    if (proj.GetComponent<BounceProjModifier>() != null)
                    {
                        this.bounces = proj.GetComponent<BounceProjModifier>().numberOfBounces;
                    }
                    else
                    {
                        this.bounces = 0;
                    }
                }
                catch
                {
                    this.bounces = 0;
                }
                if (this.bounces > 40)
                {
                    this.bounces = 40;
                }
                try
                {
                    if (proj.GetComponent<GoopModifier>() != null)
                    {
                        GoopModifier goopMod = proj.GetComponent<GoopModifier>();
                        this.goopDefinition = goopMod.goopDefinition;
                        this.SpawnGoopInFlight = goopMod.SpawnGoopInFlight;
                        this.InFlightSpawnFrequency = goopMod.InFlightSpawnFrequency;
                        this.InFlightSpawnRadius = goopMod.InFlightSpawnRadius;
                        this.SpawnGoopOnCollision = goopMod.SpawnGoopOnCollision;
                        this.CollisionSpawnRadius = goopMod.CollisionSpawnRadius;
                        this.spawnOffset = goopMod.spawnOffset;
                        this.UsesInitialDelay = goopMod.UsesInitialDelay;
                        this.InitialDelay = goopMod.InitialDelay;
                        this.m_totalElapsed = 0f;
                        this.elapsed = 0f;
                        this.m_manager = null;
                        this.doGoop = true;
                    }
                    else
                    {
                        this.doGoop = false;
                    }
                }
                catch
                {
                    this.doGoop = false;
                }
                Projectile result = null;
                if (proj.PossibleSourceGun != null)
                {
                    Gun gun = proj.PossibleSourceGun;
                    bool found = false;
                    if (gun.Volley != null)
                    {
                        foreach (ProjectileModule mod in gun.Volley.projectiles)
                        {
                            if (mod != null)
                            {
                                if (mod.shootStyle == ProjectileModule.ShootStyle.Charged)
                                {
                                    foreach (ProjectileModule.ChargeProjectile chargeProj in mod.chargeProjectiles)
                                    {
                                        if (chargeProj != null && chargeProj.Projectile != null && proj.name.Contains(chargeProj.Projectile.name))
                                        {
                                            result = chargeProj.Projectile;
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (Projectile proj2 in mod.projectiles)
                                    {
                                        if (proj2 != null && proj.name.Contains(proj2.name))
                                        {
                                            result = proj2;
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (found == true)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (gun.singleModule != null)
                        {
                            if (gun.singleModule.shootStyle == ProjectileModule.ShootStyle.Charged)
                            {
                                foreach (ProjectileModule.ChargeProjectile chargeProj in gun.singleModule.chargeProjectiles)
                                {
                                    if (chargeProj != null && chargeProj.Projectile != null && proj.name.Contains(chargeProj.Projectile.name))
                                    {
                                        result = chargeProj.Projectile;
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                foreach (Projectile proj2 in gun.singleModule.projectiles)
                                {
                                    if (proj2 != null && proj.name.Contains(proj2.name))
                                    {
                                        result = proj2;
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!found)
                    {
                        result = Toolbox.GetGunById(38).DefaultModule.projectiles[0];
                    }
                    else
                    {
                        if (livingGuns.Contains(gun.PickupObjectId))
                        {
                            this.isDoingLivingGunSynergy = true;
                        }
                    }
                }
                else
                {
                    result = Toolbox.GetGunById(38).DefaultModule.projectiles[0];
                }
                this.sourceProjectile = result;
                if (proj.sprite != null)
                {
                    if (proj.sprite.renderer != null)
                    {
                        proj.sprite.renderer.enabled = false;
                    }
                }
                proj.specRigidbody.CollideWithOthers = false;
                proj.specRigidbody.CollideWithTileMap = false;
                proj.baseData.damage = 0;
                proj.baseData.range = float.MaxValue;
                proj.baseData.speed = 0;
                proj.Owner = this.m_owner;
                proj.Shooter = this.m_owner.specRigidbody;
                this.currentProjectile = proj;
                player.StartCoroutine(this.HandleLaunch(player, deg));
            }
            catch
            {
                this.isLaunching = false;
            }
            yield break;
        }

        public IEnumerator HandleLaunch(PlayerController player, float deg)
        {
            if(this.ignoreList != null)
            {
                this.ignoreList.Clear();
            }
            else
            {
                this.ignoreList = new List<SpeculativeRigidbody>();
            }
            this.degrees = deg;
            player.SetInputOverride("YourselfBullet");
            player.SetIsFlying(true, "BulletsCanFly", false, false);
            player.OnReceivedDamage += this.BreakLaunchOnDamage;
            player.specRigidbody.OnTileCollision += new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision);
            player.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
            player.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
            float livingGunsCooldown = 0f;
            while (!this.forceBreak)
            {
                if(this.currentProjectile == null)
                {
                    break;
                }
                player.specRigidbody.Velocity = BraveMathCollege.DegreesToVector(this.degrees, this.speed);
                this.currentProjectile.transform.position = player.sprite.WorldCenter;
                this.currentProjectile.specRigidbody.Reinitialize();
                if (this.isDoingLivingGunSynergy)
                {
                    if(livingGunsCooldown > 0)
                    {
                        livingGunsCooldown -= BraveTime.DeltaTime;
                    }
                    else
                    {
                        for (int i=0; i<2; i++)
                        {
                            GameObject obj = SpawnManager.SpawnProjectile(this.sourceProjectile.gameObject, this.m_owner.sprite.WorldCenter, Quaternion.Euler(0, 0, this.degrees + (i < 1 ? -90 : 90)));
                            obj.AddComponent<NonLaunchBehaviour>();
                            Projectile proj = obj.GetComponent<Projectile>();
                            if (proj != null)
                            {
                                proj.Owner = this.m_owner;
                                proj.Shooter = this.m_owner.specRigidbody;
                                this.m_owner.DoPostProcessProjectile(proj);
                            }
                        }
                        livingGunsCooldown = 0.05f;
                    }
                }
                if (this.doGoop)
                {
                    if (this.m_manager == null)
                    {
                        this.m_manager = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopDefinition);
                    }
                    if (this.SpawnGoopInFlight)
                    {
                        this.elapsed += BraveTime.DeltaTime;
                        this.m_totalElapsed += BraveTime.DeltaTime;
                        if ((!this.UsesInitialDelay || this.m_totalElapsed > this.InitialDelay) && this.elapsed >= this.InFlightSpawnFrequency)
                        {
                            this.elapsed -= this.InFlightSpawnFrequency;
                            Vector2 b = player.sprite.WorldCenter + this.spawnOffset - player.transform.position.XY();
                            this.m_manager.AddGoopLine(player.sprite.WorldCenter + this.spawnOffset, (player.sprite.WorldCenter - player.specRigidbody.Velocity * BraveTime.DeltaTime) + b, this.InFlightSpawnRadius);
                            if (this.goopDefinition.CanBeFrozen && (this.damageTypes | CoreDamageTypes.Ice) == this.damageTypes)
                            {
                                this.Manager.FreezeGoopCircle(player.sprite.WorldCenter, this.InFlightSpawnRadius);
                            }
                        }
                    }
                }
                yield return null;
            }
            if (this.doGoop)
            {
                if (this.SpawnGoopOnCollision)
                {
                    this.Manager.TimedAddGoopCircle(player.sprite.WorldBottomCenter, this.CollisionSpawnRadius, 0.5f, false);
                    if (this.goopDefinition.CanBeFrozen && (this.damageTypes | CoreDamageTypes.Ice) == this.damageTypes)
                    {
                        this.Manager.FreezeGoopCircle(player.sprite.WorldBottomCenter, this.CollisionSpawnRadius);
                    }
                }
            }
            if(this.currentProjectile != null)
            {
                this.currentProjectile.DieInAir(true, false, false, true);
            }
            player.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
            player.specRigidbody.OnTileCollision -= new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision);
            player.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
            player.OnReceivedDamage -= this.BreakLaunchOnDamage;
            player.SetIsFlying(false, "BulletsCanFly", false, false);
            player.ClearInputOverride("YourselfBullet");
            this.isLaunching = false;
            this.forceBreak = false;
            yield break;
        }

        public IEnumerator HandleDestruction(PlayerController player)
        {
            GameObject obj = SpawnManager.SpawnProjectile(this.sourceProjectile.gameObject, player.sprite.WorldCenter, Quaternion.identity);
            obj.AddComponent<NonLaunchBehaviour>();
            Projectile proj = obj.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.sprite.renderer.enabled = false;
                proj.specRigidbody.CollideWithOthers = false;
                proj.specRigidbody.CollideWithTileMap = false;
                proj.baseData.damage = 0;
                proj.baseData.range = float.MaxValue;
                proj.baseData.speed = 0;
                proj.Owner = player;
                proj.Shooter = player.specRigidbody;
                proj.SuppressHitEffects = true;
                player.DoPostProcessProjectile(proj);
                yield return null;
                proj.DieInAir(true, false, false, true);
            }
            else
            {
                Destroy(obj);
            }
            yield break;
        }

        public IEnumerator HandleHitEnemy(PlayerController player, AIActor aiactor)
        {
            GameObject obj = SpawnManager.SpawnProjectile(this.sourceProjectile.gameObject, this.m_owner.sprite.WorldCenter, Quaternion.identity);
            obj.AddComponent<NonLaunchBehaviour>();
            Projectile proj = obj.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.sprite.renderer.enabled = false;
                proj.specRigidbody.CollideWithOthers = false;
                proj.specRigidbody.CollideWithTileMap = false;
                proj.baseData.damage = 0;
                proj.baseData.range = float.MaxValue;
                proj.baseData.speed = 0;
                proj.Owner = this.m_owner;
                proj.Shooter = this.m_owner.specRigidbody;
                proj.SuppressHitEffects = true;
                this.m_owner.DoPostProcessProjectile(proj);
                yield return null;
                proj.OnHitEnemy?.Invoke(proj, aiactor.specRigidbody, aiactor.healthHaver.IsAlive);
            }
            Destroy(obj);
            yield break;
        }

        public void OnTileCollision(CollisionData data)
        {
            if(this.bounces > 0)
            {
                Vector2 vector = data.Normal;
                Vector2 velocity = data.MyRigidbody.Velocity;
                float num2 = (-velocity).ToAngle();
                float num3 = vector.ToAngle();
                float num4 = BraveMathCollege.ClampAngle360(num2 + 2f * (num3 - num2));
                this.degrees = num4;
                this.bounces--;
            }
            else
            {
                //this.ForceBreak();
                /*CellData cellData2 = GameManager.Instance.Dungeon.data[data.TilePosition];
                cellData2.breakable = true;
                cellData2.occlusionData.overrideOcclusion = true;
                cellData2.occlusionData.cellOcclusionDirty = true;
                tk2dTileMap map = GameManager.Instance.Dungeon.DestroyWallAtPosition(data.TilePosition.x, data.TilePosition.y, true);
                this.m_owner.CurrentRoom.Cells.Add(cellData2.position);
                this.m_owner.CurrentRoom.CellsWithoutExits.Add(cellData2.position);
                this.m_owner.CurrentRoom.RawCells.Add(cellData2.position);
                if (map)
                {
                    GameManager.Instance.Dungeon.RebuildTilemap(map);
                }*/
                //base.StartCoroutine(this.HandleCombatRoomExpansion(this.Owner.CurrentRoom, this.Owner.CurrentRoom, null));
                this.ForceBreak();
            }
        }

        private IEnumerator HandleCombatRoomExpansion(RoomHandler sourceRoom, RoomHandler targetRoom, Chest sourceChest)
        {
            //yield return new WaitForSeconds(this.DelayPreExpansion);
            float duration = 5.5f;
            float elapsed = 0f;
            int numExpansionsDone = 0;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime * 9f;
                while (elapsed > (float)numExpansionsDone)
                {
                    numExpansionsDone++;
                    this.ExpandRoom(targetRoom);
                    AkSoundEngine.PostEvent("Play_OBJ_rock_break_01", GameManager.Instance.gameObject);
                }
                yield return null;
            }
            Dungeon d = GameManager.Instance.Dungeon;
            //Pathfinder.Instance.InitializeRegion(d.data, targetRoom.area.basePosition + new IntVector2(-5, -5), targetRoom.area.dimensions + new IntVector2(10, 10));
            yield break;
        }

        private void ExpandRoom(RoomHandler r)
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

        public void ForceBreak()
        {
            if (!forceBreak)
            {
                this.forceBreak = true;
            }
        }

        public void HandleEnemyDamage(SpeculativeRigidbody enemy, PixelCollider hitPixelCollider)
        {
            if(enemy.spriteAnimator != null && enemy.spriteAnimator.QueryInvulnerabilityFrame())
            {
                return;
            }
            bool flag = !enemy.healthHaver.IsDead;
            if (this.currentProjectile.OnWillKillEnemy != null && this.damage >= enemy.healthHaver.GetCurrentHealth())
            {
                this.currentProjectile.OnWillKillEnemy(this.currentProjectile, enemy);
            }
            enemy.aiActor.healthHaver.ApplyDamage(this.damage, enemy.Velocity, this.m_owner.ActorName, this.damageTypes, DamageCategory.Normal, false, hitPixelCollider, this.ignoreDamageCaps);
            if (this.m_owner.activeItems != null)
            {
                foreach (PlayerItem active in this.m_owner.activeItems)
                {
                    if (active is SprenThing)
                    {
                        active.CurrentDamageCooldown = Mathf.Max(0, active.CurrentDamageCooldown - this.damage);
                    }
                }
            }
            bool killedTarget = (flag && enemy.healthHaver.IsDead);
            this.currentProjectile.OnHitEnemy?.Invoke(this.currentProjectile, enemy, killedTarget);
            if (!killedTarget && enemy.gameActor != null)
            {
                if (this.AppliesPoison && UnityEngine.Random.value < this.PoisonApplyChance)
                {
                    enemy.gameActor.ApplyEffect(this.healthEffect, 1f, null);
                }
                if (this.AppliesSpeedModifier && UnityEngine.Random.value < this.SpeedApplyChance)
                {
                    enemy.gameActor.ApplyEffect(this.speedEffect, 1f, null);
                }
                if (this.AppliesCharm && UnityEngine.Random.value < this.CharmApplyChance)
                {
                    enemy.gameActor.ApplyEffect(this.charmEffect, 1f, null);
                }
                if (this.AppliesFreeze && UnityEngine.Random.value < this.FreezeApplyChance)
                {
                    enemy.gameActor.ApplyEffect(this.freezeEffect, 1f, null);
                }
                if (this.AppliesCheese && UnityEngine.Random.value < this.CheeseApplyChance)
                {
                    enemy.gameActor.ApplyEffect(this.cheeseEffect, 1f, null);
                }
                if (this.AppliesBleed && UnityEngine.Random.value < this.BleedApplyChance)
                {
                    enemy.gameActor.ApplyEffect(this.bleedEffect, -1f, null);
                }
                if (this.AppliesFire && UnityEngine.Random.value < this.FireApplyChance)
                {
                    enemy.gameActor.ApplyEffect(this.fireEffect, 1f, null);
                }
                if (this.AppliesStun && UnityEngine.Random.value < this.StunApplyChance && enemy.gameActor.behaviorSpeculator)
                {
                    enemy.gameActor.behaviorSpeculator.Stun(this.AppliedStunDuration, true);
                }
                for (int i = 0; i < this.statusEffectsToApply.Count; i++)
                {
                    enemy.gameActor.ApplyEffect(this.statusEffectsToApply[i], 1f, null);
                }
                foreach(AppliedEffectBase effect in this.appliedEffects)
                {
                    effect.AddSelfToTarget(enemy.gameObject);
                }
                if (this.CanTransmogrify && UnityEngine.Random.value < this.ChanceToTransmogrify && enemy.aiActor && !enemy.aiActor.IsMimicEnemy && enemy.aiActor.healthHaver && !enemy.aiActor.healthHaver.IsBoss && enemy.aiActor.healthHaver.IsVulnerable)
                {
                    enemy.aiActor.Transmogrify(EnemyDatabase.GetOrLoadByGuid(this.TransmogrifyTargetGuids[UnityEngine.Random.Range(0, this.TransmogrifyTargetGuids.Length)]), (GameObject)ResourceCache.Acquire("Global VFX/VFX_Item_Spawn_Poof"));
                }
            }
            this.ignoreList.Clear();
            this.ignoreList.Add(enemy);
        }

        public void OnPreRigidbodyCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
        {
            if (otherRigidbody.GetComponent<FlippableCover>() == null && !this.ignoreList.Contains(otherRigidbody))
            {
                if((otherRigidbody.aiActor != null || otherRigidbody.minorBreakable != null || otherRigidbody.majorBreakable != null || otherRigidbody.majorBreakable != null))
                {
                    if (this.bounceOffEnemies && otherRigidbody.aiActor != null)
                    {
                        if (otherRigidbody.aiActor != null)
                        {
                            if (otherRigidbody.aiActor.healthHaver != null)
                            {
                                this.HandleEnemyDamage(otherRigidbody, otherPixelCollider);
                            }
                        }
                        this.degrees = UnityEngine.Random.Range(0, 359);
                        PhysicsEngine.SkipCollision = true;
                    }
                    else if((!this.bounceOffEnemies || otherRigidbody.aiActor == null) && this.penetration > 0 && otherRigidbody.majorBreakable == null)
                    {
                        if (otherRigidbody.aiActor != null)
                        {
                            if (otherRigidbody.aiActor.healthHaver != null)
                            {
                                this.HandleEnemyDamage(otherRigidbody, otherPixelCollider);
                            }
                        }
                        else if (otherRigidbody.minorBreakable != null)
                        {
                            otherRigidbody.minorBreakable.Break();
                        }
                        this.penetration--;
                        this.bounces--;
                        PhysicsEngine.SkipCollision = true;
                    }
                }
            }
            else
            {
                PhysicsEngine.SkipCollision = true;
            }
        }

        public void OnRigidbodyCollision(CollisionData data)
        {
            if ((data.OtherRigidbody.aiActor != null || data.OtherRigidbody.minorBreakable != null || data.OtherRigidbody.majorBreakable != null || data.OtherRigidbody.majorBreakable != null))
            {
                if (this.penetration <= 0 || data.OtherRigidbody.majorBreakable != null)
                {
                    if (data.OtherRigidbody.aiActor != null)
                    {
                        if (data.OtherRigidbody.aiActor.healthHaver != null)
                        {
                            this.HandleEnemyDamage(data.OtherRigidbody, data.OtherPixelCollider);
                        }
                    }
                    else if (data.OtherRigidbody.minorBreakable != null)
                    {
                        data.OtherRigidbody.minorBreakable.Break();
                    }
                    else if(data.OtherRigidbody.majorBreakable != null)
                    {
                        MajorBreakable majorBreakable = data.OtherRigidbody.majorBreakable;
                        float num = 1f;
                        if (!majorBreakable.IsSecretDoor || !(this.m_owner.CurrentGun != null) || !this.m_owner.CurrentGun.InfiniteAmmo)
                        {
                            float num2 = this.damage;
                            if (num2 <= 0f && GameManager.Instance.InTutorial)
                            {
                                majorBreakable.ApplyDamage(1.5f, this.m_owner.specRigidbody.Velocity, false, false, false);
                            }
                            else
                            {
                                majorBreakable.ApplyDamage(num2 * num, this.m_owner.specRigidbody.Velocity, false, false, false);
                            }
                        }
                    }
                    if (data.OtherRigidbody.aiActor == null && this.bounces > 0)
                    {
                        Vector2 vector = data.Normal;
                        Vector2 velocity = data.MyRigidbody.Velocity;
                        float num2 = (-velocity).ToAngle();
                        float num3 = vector.ToAngle();
                        float num4 = BraveMathCollege.ClampAngle360(num2 + 2f * (num3 - num2));
                        this.degrees = num4;
                        this.bounces--;
                    }
                    else
                    {
                        this.ForceBreak();
                    }
                }
            }
            else
            {
                if (this.bounces > 0)
                {
                    Vector2 vector = data.Normal;
                    Vector2 velocity = data.MyRigidbody.Velocity;
                    float num2 = (-velocity).ToAngle();
                    float num3 = vector.ToAngle();
                    float num4 = BraveMathCollege.ClampAngle360(num2 + 2f * (num3 - num2));
                    this.degrees = num4;
                    this.bounces--;
                }
                else
                {
                    this.ForceBreak();
                }
            }
        }

        public void BreakLaunchOnDamage(PlayerController player)
        {
            this.ForceBreak();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProcessProjectile;
            player.ReceivesTouchDamage = true;
            return base.Drop(player);
        }

        public DeadlyDeadlyGoopManager Manager
        {
            get
            {
                if (this.m_manager == null)
                {
                    this.m_manager = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopDefinition);
                }
                return this.m_manager;
            }
        }

        private bool forceBreak = false;
        private float damage = 0;
        private float speed = 0;
        private float degrees = 0;
        private int penetration = 0;
        private bool ignoreDamageCaps = false;
        private int bounces = 0;
        private CoreDamageTypes damageTypes = CoreDamageTypes.None;
        private bool isLaunching = false;
        private Projectile sourceProjectile = null;
        private List<SpeculativeRigidbody> ignoreList = new List<SpeculativeRigidbody>();
        public bool AppliesPoison;
        public float PoisonApplyChance;
        public GameActorHealthEffect healthEffect;
        public bool AppliesSpeedModifier;
        public float SpeedApplyChance;
        public GameActorSpeedEffect speedEffect;
        public bool AppliesCharm;
        public float CharmApplyChance;
        public GameActorCharmEffect charmEffect;
        public bool AppliesFreeze;
        public float FreezeApplyChance;
        public GameActorFreezeEffect freezeEffect;
        public bool AppliesFire;
        public float FireApplyChance;
        public GameActorFireEffect fireEffect;
        public bool AppliesStun;
        public float StunApplyChance;
        public float AppliedStunDuration;
        public bool AppliesBleed;
        public GameActorBleedEffect bleedEffect;
        public bool AppliesCheese;
        public float CheeseApplyChance;
        public GameActorCheeseEffect cheeseEffect;
        public float BleedApplyChance;
        public List<GameActorEffect> statusEffectsToApply;
        public AppliedEffectBase[] appliedEffects;
        public GoopDefinition goopDefinition;
        public bool SpawnGoopInFlight;
        public float InFlightSpawnFrequency;
        public float InFlightSpawnRadius;
        public bool SpawnGoopOnCollision;
        public float CollisionSpawnRadius;
        public Vector2 spawnOffset;
        public bool UsesInitialDelay;
        public float InitialDelay;
        private float m_totalElapsed;
        private DeadlyDeadlyGoopManager m_manager;
        private float elapsed;
        public bool doGoop = false;
        public bool CanTransmogrify;
        public float ChanceToTransmogrify;
        public string[] TransmogrifyTargetGuids;
        public bool bounceOffEnemies = false;
        public Projectile currentProjectile = null;
        public bool isDoingLivingGunSynergy = false;
        public static List<int> livingGuns = new List<int>
        {
            599,
            338,
            598,
            566
        };
        private class NonLaunchBehaviour : MonoBehaviour
        {
        }
    }
}