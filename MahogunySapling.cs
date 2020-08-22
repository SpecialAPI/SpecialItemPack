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
    class MahogunySapling : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Mahoguny Sapling";
            string resourceName = "SpecialItemPack/Resources/MahogunySapling";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<MahogunySapling>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Illegal Logging";
            string longDesc = "Plants a Mahoguny.\n\nThis sapling of Mahoguny was planted in a barrel of Blunderbuss. It's way too weak to grow on it's own, but it's growth visibly increases at an intense battlefield.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 150f);
            MahogunySapling.BuildPrefab();
            item.consumable = false;
            item.quality = ItemQuality.A;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(201);

        }

        private static void BuildPrefab()
        {
            GameObject gameObject = new GameObject("Mahoguny");
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            MahogunyBehaviour behaviour = gameObject.AddComponent<MahogunyBehaviour>();
            if (behaviour.spriteAnimator == null)
            {
                behaviour.spriteAnimator = behaviour.gameObject.AddComponent<tk2dSpriteAnimator>();
            }
            behaviour.spriteAnimator.Library = Toolbox.GetGunById(339).GetComponent<tk2dSpriteAnimator>().Library;
            behaviour.idleAnimation = Toolbox.GetGunById(339).idleAnimation;
            behaviour.shootAnimation = Toolbox.GetGunById(339).shootAnimation;
            behaviour.reloadAnimation = Toolbox.GetGunById(339).reloadAnimation;
            behaviour.GunSwitchGroup = Toolbox.GetGunById(339).gunSwitchGroup;
            behaviour.CooldownTime = Toolbox.GetGunById(339).DefaultModule.cooldownTime * 2;
            behaviour.ReloadTime = Toolbox.GetGunById(339).reloadTime;
            behaviour.NumberOfShotsInClip = Toolbox.GetGunById(339).DefaultModule.numberOfShotsInClip;
            behaviour.leafObj = Toolbox.GetGunById(339).shellCasing;
            foreach (ProjectileModule mod in Toolbox.GetGunById(339).Volley.projectiles)
            {
                if (mod.GetCurrentProjectile() != null)
                {
                    behaviour.ProjectilesToShoot.Add(mod.GetCurrentProjectile());
                    behaviour.AngleVariances.Add(mod.angleVariance);
                }
            }
            tk2dSprite.SetSprite(Toolbox.GetGunById(339).sprite.Collection, Toolbox.GetGunById(339).sprite.spriteId);
            gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.GameObject.DontDestroyOnLoad(gameObject);
            MahogunyPrefab = gameObject;
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            GameObject obj = UnityEngine.Object.Instantiate(MahogunyPrefab, user.sprite.WorldCenter, Quaternion.identity);
            MahogunyBehaviour behaviour = obj.GetComponent<MahogunyBehaviour>();
            behaviour.Owner = user;
            behaviour.lifespan = user.PlayerHasActiveSynergy("#TWO_TREES_ARE_BETTER_THAN_ONE") ? 17.5f : 10f;
            behaviour.DelayedInitialization();
            base.StartCoroutine(ItemBuilder.HandleDuration(this, user.PlayerHasActiveSynergy("#TWO_TREES_ARE_BETTER_THAN_ONE") ? 17.5f : 10f, user, null));
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user);
        }

        public static GameObject MahogunyPrefab;

        public class MahogunyBehaviour : BraveBehaviour
        {
            public void DelayedInitialization()
            {
                this.m_cooldownRemaining = 0;
                this.m_reloadRemaining = 0;
                this.m_clipShotsRemaining = this.NumberOfShotsInClip;
                AkSoundEngine.SetSwitch("WPN_Guns", this.GunSwitchGroup, base.gameObject);
                for (int i = 0; i < 10; i++)
                {
                    this.SpawnShellCasingAtPosition(this.sprite.WorldCenter);
                }
                base.StartCoroutine(this.HandleDuration());
            }

            private IEnumerator HandleDuration()
            {
                float elapsed = 0;
                while (elapsed < this.lifespan)
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
                                if(this.ProjectilesToShoot != null)
                                {
                                    for (int i=0; i<this.ProjectilesToShoot.Count; i++)
                                    {
                                        Projectile projectile = this.ProjectilesToShoot[i];
                                        float angleVariance = this.AngleVariances[i];
                                        if (projectile != null)
                                        {
                                            GameObject obj = SpawnManager.SpawnProjectile(projectile.gameObject, this.sprite.WorldCenter,
                                                Quaternion.Euler(0, 0, BraveMathCollege.Atan2Degrees(this.GetNearestEnemy().sprite.WorldCenter - this.sprite.WorldCenter) + UnityEngine.Random.Range(-angleVariance, angleVariance)));
                                            Projectile proj = obj.GetComponent<Projectile>();
                                            if (proj != null)
                                            {
                                                if (this.Owner != null)
                                                {
                                                    proj.Owner = this.Owner;
                                                    proj.Shooter = this.Owner.specRigidbody;
                                                }
                                            }
                                        }
                                    }
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
                if (activeEnemies == null)
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
                    if (this.m_reloadRemaining <= 0)
                    {
                        this.m_clipShotsRemaining = this.NumberOfShotsInClip;
                    }
                    yield return null;
                }
                yield break;
            }

            public float CooldownTime;
            private float m_cooldownRemaining;
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
            public float lifespan = 10f;
            public List<Projectile> ProjectilesToShoot = new List<Projectile>();
            public List<float> AngleVariances = new List<float>();
        }
    }
}
