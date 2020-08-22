using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;

namespace SpecialItemPack
{
    class AmmoFlower : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Ammo Flower";
            string resourceName = "SpecialItemPack/Resources/AmmoFlower";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<AmmoFlower>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Grown in an Ammo Box";
            string longDesc = "Makes your guns reload automatically, and makes them full auto.\n\nTheese flowers grow in Kaliber's private garden. They grow in pure shells and can be only be watered with oil. They might have surprising effects on weapons." +
                "\n\nThis variety of flowers are called \"Relode-Lotus\". Plenty of lazy gungeoneers use it on their weapons to save their fingers from crazily pressing the trigger. Because it also grants \"Auto-Reload\" the gungeoneers can completely relax " +
                "while firing and for example drink tea.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.C;
            item.PlaceItemInAmmonomiconAfterItemById(258);
        }

        protected void LateUpdate()
        {
            base.Update();
            if(this.m_pickedUp && this.m_owner != null)
            {
                foreach(Gun gun in this.m_owner.inventory.AllGuns)
                {
                    if(gun.GetComponent<AutoGunBehaviour>() == null)
                    {
                        gun.gameObject.AddComponent<AutoGunBehaviour>();
                        this.affectedGuns.Add(gun);
                    }
                }
                if(this.m_owner.CurrentGun != null)
                {
                    if(this.m_owner.CurrentGun.ClipShotsRemaining <= 0 && !this.m_owner.CurrentGun.IsReloading)
                    {
                        this.m_owner.CurrentGun.Reload();
                    }
                }
                if(this.m_owner.CurrentRoom != null && this.m_owner.CurrentItem is RelodestoneItem && this.m_owner.CurrentItem.IsCurrentlyActive)
                {
                    if (this.m_owner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All) != null)
                    {
                        Vector2 centerPosition = this.m_owner.CenterPosition;
                        int num = 0;
                        foreach (AIActor aiactor in this.m_owner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All))
                        {
                            if (aiactor && aiactor.specRigidbody && aiactor.healthHaver && aiactor.healthHaver.GetMaxHealth() <= 5f)
                            {
                                int num2 = Mathf.RoundToInt(aiactor.healthHaver.GetCurrentHealth());
                                bool flag = this.AdjustRigidbodyVelocity(aiactor.specRigidbody, centerPosition);
                                if (flag)
                                {
                                    num += num2;
                                }
                            }
                        }
                        if (num > 0 && this.m_owner.CurrentGun && this.m_owner.CurrentGun.CanGainAmmo)
                        {
                            this.m_owner.CurrentGun.GainAmmo(num);
                        }
                    }
                }
            }
        }

        private Vector2 GetFrameAccelerationForRigidbody(Vector2 unitCenter, Vector2 myCenter, float currentDistance, float g)
        {
            Vector2 zero = Vector2.zero;
            float num = Mathf.Clamp01(1f - currentDistance / 10f);
            float d = g * num * num;
            Vector2 normalized = (myCenter - unitCenter).normalized;
            return normalized * d;
        }

        private bool AdjustRigidbodyVelocity(SpeculativeRigidbody other, Vector2 myCenter)
        {
            bool result = false;
            Vector2 a = other.UnitCenter - myCenter;
            float effectRadius = 10f;
            float num = Vector2.SqrMagnitude(a);
            if (num >=  effectRadius*effectRadius)
            {
                return result;
            }
            float gravitationalForce = 500f;
            Vector2 vector = other.Velocity;
            AIActor aiactor = other.aiActor;
            if (aiactor == null)
            {
                return false;
            }
            aiactor.CollisionDamage = 0;
            if (other.GetComponent<BlackHoleDoer>() != null)
            {
                return false;
            }
            if (vector == Vector2.zero)
            {
                Vector2 lhs = myCenter - other.UnitCenter;
                if (lhs == Vector2.zero)
                {
                    lhs = new Vector2(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f);
                }
                vector = lhs.normalized * 3f;
            }
            if (num < 4f)
            {
                aiactor.EraseFromExistence(true);
                result = true;
            }
            Vector2 frameAccelerationForRigidbody = this.GetFrameAccelerationForRigidbody(other.UnitCenter, myCenter, Mathf.Sqrt(num), gravitationalForce);
            float d = Mathf.Clamp(BraveTime.DeltaTime, 0f, 0.02f);
            Vector2 b = frameAccelerationForRigidbody * d;
            Vector2 vector2 = vector + b;
            if (BraveTime.DeltaTime > 0.02f)
            {
                vector2 *= 0.02f / BraveTime.DeltaTime;
            }
            aiactor.knockbackDoer.ApplyKnockback(vector2, 2 * (aiactor.knockbackDoer.weight / 10f), true);
            if(aiactor.behaviorSpeculator != null)
            {
                aiactor.behaviorSpeculator.Stun(0.1f, false);
            }
            if (aiactor != null)
            {
                aiactor.CollisionDamage = 0;
            }
            return result;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnReloadedGun += this.HandleInvisibility;
            player.OnReceivedDamage += this.TakeRevenge;
        }

        private void TakeRevenge(PlayerController player)
        {
            if (player.CurrentRoom != null && player.PlayerHasActiveSynergy("#AMMO_CLOAK"))
            {
                if (player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                {
                    foreach (AIActor aiactor in player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                    {
                        GameObject obj1 = (PickupObjectDatabase.GetById(52) as Gun).DefaultModule.GetCurrentProjectile().gameObject;
                        if(UnityEngine.Random.value < 0.25f)
                        {
                            obj1 = (PickupObjectDatabase.GetById(52) as Gun).DefaultModule.chargeProjectiles[(PickupObjectDatabase.GetById(52) as Gun).DefaultModule.chargeProjectiles.Count - 1].Projectile.gameObject;
                        }
                        GameObject obj = SpawnManager.SpawnProjectile(obj1, player.sprite.WorldCenter, Quaternion.Euler(0, 0, BraveMathCollege.Atan2Degrees(aiactor.sprite.WorldCenter - player.sprite.WorldCenter)));
                        Projectile proj = obj.GetComponent<Projectile>();
                        if (proj != null)
                        {
                            if (player != null)
                            {
                                proj.Owner = player;
                                proj.Shooter = player.specRigidbody;
                            }
                        }
                    }
                }
            }
        }

        private void HandleInvisibility(PlayerController player, Gun gun)
        {
            if(gun.ClipShotsRemaining <= 0 && player.PlayerHasActiveSynergy("#ARCANE_GUNFLOWER"))
            {
                GameManager.Instance.StartCoroutine(this.Invisibility(player));
            }
        }

        private IEnumerator Invisibility(PlayerController player)
        {
            float elapsed = 0f;
            player.ChangeSpecialShaderFlag(1, 1f);
            player.SetIsStealthed(true, "arcane gunflower");
            player.SetCapableOfStealing(true, "ArcaneGunflower", null);
            player.OnDidUnstealthyAction += this.BreakStealth;
            player.OnItemStolen += this.BreakStealthOnSteal;
            float duration = 0.5f * player.stats.GetStatValue(PlayerStats.StatType.ReloadSpeed);
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                if (!player.IsStealthed)
                {
                    break;
                }
                yield return null;
            }
            if (player.IsStealthed)
            {
                this.BreakStealth(player);
            }
            yield break;
        }

        private void BreakStealthOnSteal(PlayerController arg1, ShopItemController arg2)
        {
            this.BreakStealth(arg1);
        }

        private void BreakStealth(PlayerController obj)
        {
            obj.OnDidUnstealthyAction -= this.BreakStealth;
            obj.OnItemStolen -= this.BreakStealthOnSteal;
            obj.ChangeSpecialShaderFlag(1, 0f);
            obj.SetIsStealthed(false, "arcane gunflower");
            obj.SetCapableOfStealing(false, "ArcaneGunflower", null);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnReceivedDamage -= this.TakeRevenge;
            player.OnReloadedGun -= this.HandleInvisibility;
            foreach (Gun gun in this.affectedGuns)
            {
                if(gun != null && gun.GetComponent<AutoGunBehaviour>() != null)
                {
                    gun.GetComponent<AutoGunBehaviour>().Destroy();
                }
            }
            return base.Drop(player);
        }

        private List<Gun> affectedGuns = new List<Gun>();

        private class AutoGunBehaviour : MonoBehaviour
        {
            private void Start()
            {
                this.gun = base.GetComponent<Gun>();
            }

            private void Update()
            {
                if (this.gun.Volley != null)
                {
                    foreach (ProjectileModule mod in gun.Volley.projectiles)
                    {
                        if (mod.shootStyle != ProjectileModule.ShootStyle.Automatic && mod.shootStyle != ProjectileModule.ShootStyle.Charged && mod.shootStyle != ProjectileModule.ShootStyle.Beam)
                        {
                            if (this.shootStyles.ContainsKey(mod))
                            {
                                if (this.shootStyles[mod] != mod.shootStyle)
                                {
                                    this.shootStyles[mod] = mod.shootStyle;
                                }
                            }
                            else
                            {
                                this.shootStyles.Add(mod, mod.shootStyle);
                            }
                            mod.shootStyle = ProjectileModule.ShootStyle.Automatic;
                        }
                        else if (mod.shootStyle == ProjectileModule.ShootStyle.Charged)
                        {
                            if(gun.RuntimeModuleData[mod].chargeTime >= mod.LongestChargeTime)
                            {
                                gun.CeaseAttack(true, null);
                            }
                        }
                    }
                }
                else
                {
                    if (this.gun.singleModule.shootStyle != ProjectileModule.ShootStyle.Automatic && this.gun.singleModule.shootStyle != ProjectileModule.ShootStyle.Charged && this.gun.singleModule.shootStyle != ProjectileModule.ShootStyle.Beam)
                    {
                        if (this.shootStyles.ContainsKey(this.gun.singleModule))
                        {
                            if (this.shootStyles[this.gun.singleModule] != this.gun.singleModule.shootStyle)
                            {
                                this.shootStyles[this.gun.singleModule] = this.gun.singleModule.shootStyle;
                            }
                        }
                        else
                        {
                            this.shootStyles.Add(this.gun.singleModule, this.gun.singleModule.shootStyle);
                        }
                        this.gun.singleModule.shootStyle = ProjectileModule.ShootStyle.Automatic;
                    }
                    else if (this.gun.singleModule.shootStyle == ProjectileModule.ShootStyle.Charged)
                    {
                        if (gun.RuntimeModuleData[this.gun.singleModule].chargeTime >= this.gun.singleModule.LongestChargeTime)
                        {
                            gun.CeaseAttack(true, null);
                        }
                    }
                }
                
            }

            private void ForwardToPast()
            {
                foreach(KeyValuePair<ProjectileModule, ProjectileModule.ShootStyle> valuePairs in this.shootStyles)
                {
                    valuePairs.Key.shootStyle = valuePairs.Value;
                }
                this.shootStyles.Clear();
            }

            public void Destroy()
            {
                this.ForwardToPast();
                UnityEngine.Object.Destroy(this);
            }

            private Gun gun;
            private Dictionary<ProjectileModule, ProjectileModule.ShootStyle> shootStyles = new Dictionary<ProjectileModule, ProjectileModule.ShootStyle>();
        }
    }
}
