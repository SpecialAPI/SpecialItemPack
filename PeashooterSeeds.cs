using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;
using System.Collections;

namespace SpecialItemPack
{
    class PeashooterSeeds : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Peashooter Seeds";
            string resourceName = "SpecialItemPack/Resources/PeashooterSeeds";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<PeashooterSeeds>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Plant 'Em!";
            string longDesc = "Plants a Pea Shooter.\n\nThese seeds are used to plant a Pea Shooter. They are just pea seeds, but with \"shooter\" added.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 150f);
            PeashooterSeeds.BuildPrefab();
            item.consumable = false;
            item.quality = ItemQuality.B;
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(201);

        }

        private static void BuildPrefab()
        {
            GameObject gameObject = new GameObject("Peashooter");
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            PeashooterBehaviour behaviour = gameObject.AddComponent<PeashooterBehaviour>();
            if (behaviour.spriteAnimator == null)
            {
                behaviour.spriteAnimator = behaviour.gameObject.AddComponent<tk2dSpriteAnimator>();
            }
            behaviour.spriteAnimator.Library = Toolbox.GetGunById(197).GetComponent<tk2dSpriteAnimator>().Library;
            behaviour.idleAnimation = Toolbox.GetGunById(197).idleAnimation;
            behaviour.shootAnimation = Toolbox.GetGunById(197).shootAnimation;
            behaviour.reloadAnimation = Toolbox.GetGunById(197).reloadAnimation;
            behaviour.GunSwitchGroup = Toolbox.GetGunById(197).gunSwitchGroup;
            behaviour.CooldownTime = Toolbox.GetGunById(197).DefaultModule.cooldownTime * 2;
            behaviour.AngleVariance = Toolbox.GetGunById(197).DefaultModule.angleVariance;
            behaviour.ProjectileToShoot = Toolbox.GetGunById(197).DefaultModule.projectiles[0];
            behaviour.ReloadTime = Toolbox.GetGunById(197).reloadTime;
            behaviour.NumberOfShotsInClip = Toolbox.GetGunById(197).DefaultModule.numberOfShotsInClip;
            behaviour.leafObj = Toolbox.GetGunById(339).shellCasing;
            tk2dSprite.SetSprite(Toolbox.GetGunById(197).sprite.Collection, Toolbox.GetGunById(197).sprite.spriteId);
            gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.GameObject.DontDestroyOnLoad(gameObject);
            PeashooterPrefab = gameObject;
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

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            GameObject obj = UnityEngine.Object.Instantiate(PeashooterPrefab, user.sprite.WorldCenter, Quaternion.identity);
            PeashooterBehaviour behaviour = obj.GetComponent<PeashooterBehaviour>();
            behaviour.Owner = user;
            behaviour.DelayedInitialization();
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user);
        }

        public static GameObject PeashooterPrefab;

        public class PeashooterBehaviour : BraveBehaviour
        {
            public void DelayedInitialization()
            {
                this.m_cooldownRemaining = 0;
                this.m_reloadRemaining = 0;
                this.m_clipShotsRemaining = this.NumberOfShotsInClip;
                AkSoundEngine.SetSwitch("WPN_Guns", this.GunSwitchGroup, base.gameObject);
                for(int i=0; i<10; i++)
                {
                    this.SpawnShellCasingAtPosition(this.sprite.WorldCenter);
                }
                base.StartCoroutine(this.HandleDuration());
            }

            private IEnumerator HandleDuration()
            {
                float elapsed = 0;
                while(elapsed < 10)
                {
                    elapsed += BraveTime.DeltaTime;
                    yield return null;
                }
                for (int i = 0; i < 10; i++)
                {
                    this.SpawnShellCasingAtPosition(this.sprite.WorldCenter);
                }
                UnityEngine.Object.Destroy(base.gameObject);
                yield break;
            }
            
            private void Update()
            {
                if (this.m_reloadRemaining > 0)
                {
                }
                else
                {
                    if (this.m_cooldownRemaining > 0)
                    {
                    }
                    else
                    {
                        if (this.m_clipShotsRemaining > 0)
                        {
                            if (this.GetNearestEnemy() != null)
                            {
                                GameObject obj = SpawnManager.SpawnProjectile(this.ProjectileToShoot.gameObject, this.sprite.WorldCenter,
                                    Quaternion.Euler(0, 0, BraveMathCollege.Atan2Degrees(this.GetNearestEnemy().sprite.WorldCenter - this.sprite.WorldCenter) + UnityEngine.Random.Range(-this.AngleVariance, this.AngleVariance)));
                                Projectile proj = obj.GetComponent<Projectile>();
                                if (proj != null)
                                {
                                    if (this.Owner != null)
                                    {
                                        proj.Owner = this.Owner;
                                        proj.Shooter = this.Owner.specRigidbody;
                                    }
                                }
                                if(this.Owner != null && this.Owner.PlayerHasActiveSynergy("#2_REPEAT_IT"))
                                {
                                    this.SpawnShadowBullet(proj);
                                }
                                AkSoundEngine.PostEvent("Play_WPN_gun_shot_01", base.gameObject);
                                this.spriteAnimator.Play(this.shootAnimation);
                                base.StartCoroutine(this.HandleCooldown());
                                this.m_clipShotsRemaining -= 1;
                            }
                        }
                        else
                        {
                            AkSoundEngine.PostEvent("Play_WPN_gun_reload_01", base.gameObject);
                            this.spriteAnimator.Play(this.reloadAnimation);
                            base.StartCoroutine(this.HandleReload());
                        }
                    }
                }
                if (this.GetNearestEnemy() != null)
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, BraveMathCollege.ClampAngle180(BraveMathCollege.Atan2Degrees(this.GetNearestEnemy().sprite.WorldCenter - this.sprite.WorldCenter)));
                    //ETGModConsole.Log(.ToString());
                    float num = 75f;
                    float num2 = 105f;
                    float gunAngle = BraveMathCollege.ClampAngle180(BraveMathCollege.Atan2Degrees(this.GetNearestEnemy().sprite.WorldCenter - this.sprite.WorldCenter));
                    if (gunAngle <= 155f && gunAngle >= 25f)
                    {
                        num = 75f;
                        num2 = 105f;
                    }
                    if (Mathf.Abs(gunAngle) > num2)
                    {
                        this.sprite.FlipY = true;
                    }
                    else if (Mathf.Abs(gunAngle) < num)
                    {
                        base.sprite.FlipY = false;
                    }
                    this.CurrentAngle = gunAngle;
                }
            }

            private void SpawnShellCasingAtPosition(Vector3 position)
            {
                if (this.leafObj != null && this.transform)
                {
                    GameObject gameObject = SpawnManager.SpawnDebris(this.leafObj, position.WithZ(this.transform.position.z), this.transform.rotation);
                    ShellCasing component = gameObject.GetComponent<ShellCasing>();
                    if (component != null)
                    {
                        component.Trigger();
                    }
                    DebrisObject component2 = gameObject.GetComponent<DebrisObject>();
                    if (component2 != null)
                    {
                        int num = (component2.transform.right.x <= 0f) ? -1 : 1;
                        Vector3 vector = Vector3.up * (UnityEngine.Random.value * 1.5f + 1f) + -1.5f * Vector3.right * (float)num * (UnityEngine.Random.value + 1.5f);
                        Vector3 startingForce = new Vector3(vector.x, 0f, vector.y);
                        if (this.transform.position.GetAbsoluteRoom() != null && this.transform.position.GetAbsoluteRoom().area.PrototypeRoomSpecialSubcategory == PrototypeDungeonRoom.RoomSpecialSubCategory.CATACOMBS_BRIDGE_ROOM)
                        {
                            startingForce = (vector.x * (float)num * -1f * (this.transform.position.XY() - this.sprite.WorldCenter).normalized).ToVector3ZUp(vector.y);
                        }
                        float y = this.transform.position.y;
                        float num2 = position.y - this.transform.position.y + 0.2f;
                        float num3 = component2.transform.position.y - y + UnityEngine.Random.value * 0.5f;
                        component2.additionalHeightBoost = num2 - num3;
                        if (this.CurrentAngle > 25f && this.CurrentAngle < 155f)
                        {
                            component2.additionalHeightBoost += -0.25f;
                        }
                        else
                        {
                            component2.additionalHeightBoost += 0.25f;
                        }
                        component2.Trigger(startingForce, num3, 1f);
                    }
                }
            }

            public AIActor GetNearestEnemy()
            {
                AIActor aiactor = null;
                float nearestDistance = float.MaxValue;
                List<AIActor> activeEnemies = this.transform.position.GetAbsoluteRoom().GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                AIActor result;
                if(activeEnemies == null)
                {
                    return null;
                }
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    AIActor aiactor2 = activeEnemies[i]; ;
                    bool flag3 = !aiactor2.healthHaver.IsDead;
                    if (flag3)
                    {
                        float num = Vector2.Distance(this.transform.position, aiactor2.CenterPosition);
                        bool flag5 = num < nearestDistance;
                        if (flag5)
                        {
                            nearestDistance = num;
                            aiactor = aiactor2;
                        }
                    }
                }
                result = aiactor;
                return result;
            }

            public void SpawnShadowBullet(Projectile obj)
            {
                float num = 0f;
                if (obj.sprite && obj.sprite.GetBounds().size.x > 0.5f)
                {
                    num += obj.sprite.GetBounds().size.x / 10f;
                }
                num = Mathf.Max(num, 0.1f);
                base.StartCoroutine(this.SpawnShadowBullet(obj, num));
            }

            protected IEnumerator SpawnShadowBullet(Projectile obj, float additionalDelay)
            {
                Vector3 cachedSpawnPosition = obj.transform.position;
                Quaternion cachedSpawnRotation = obj.transform.rotation;
                if (additionalDelay > 0f)
                {
                    float ela = 0f;
                    while (ela < additionalDelay)
                    {
                        ela += BraveTime.DeltaTime;
                        yield return null;
                    }
                }
                if (obj)
                {
                    GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(obj.gameObject, cachedSpawnPosition, cachedSpawnRotation);
                    gameObject2.transform.position += gameObject2.transform.right * -0.5f;
                    Projectile component2 = gameObject2.GetComponent<Projectile>();
                    component2.specRigidbody.Reinitialize();
                    component2.collidesWithPlayer = false;
                    component2.Owner = obj.Owner;
                    component2.Shooter = obj.Shooter;
                    component2.baseData.damage = obj.baseData.damage;
                    component2.baseData.range = obj.baseData.range;
                    component2.baseData.speed = obj.baseData.speed;
                    component2.baseData.force = obj.baseData.force;
                }
                yield break;
            }

            public IEnumerator HandleCooldown()
            {
                this.m_cooldownRemaining = this.CooldownTime;
                while (this.m_cooldownRemaining > 0)
                {
                    this.m_cooldownRemaining -= BraveTime.DeltaTime;
                    yield return null;
                }
                yield break;
            }

            public IEnumerator HandleReload()
            {
                this.m_reloadRemaining = this.ReloadTime;
                while (this.m_reloadRemaining > 0)
                {
                    this.m_reloadRemaining -= BraveTime.DeltaTime;
                    if(this.m_reloadRemaining <= 0)
                    {
                        this.m_clipShotsRemaining = this.NumberOfShotsInClip;
                    }
                    yield return null;
                }
                yield break;
            }

            public float CooldownTime;
            private float m_cooldownRemaining;
            public float AngleVariance;
            public Projectile ProjectileToShoot;
            public float ReloadTime;
            private float m_reloadRemaining;
            public int NumberOfShotsInClip;
            private int m_clipShotsRemaining;
            public int MaxDistance;
            public string GunSwitchGroup;
            public PlayerController Owner;
            public string shootAnimation;
            public string reloadAnimation;
            public string idleAnimation;
            public GameObject leafObj;
            public float CurrentAngle = 0;
        }
    }
}
