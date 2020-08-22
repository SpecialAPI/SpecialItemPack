using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;
using System.Collections;
using Dungeonator;

namespace SpecialItemPack
{
    class CarBattery : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Car Battery";
            string resourceName = "SpecialItemPack/Resources/CarBattery";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<CarBattery>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Perfect Charge!";
            string longDesc = "Decreases active item cooldowns.\n\nA partially charged car battery. It can charge active items, however partially. Strangelly enough it's charge doesn't run out. Secrets that will never be revealed.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.S;
            item.AddToTrorkShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(407);
        }

        private void LateUpdate()
        {
            if(this.m_pickedUp && this.m_owner != null)
            {
                foreach(PlayerItem active in this.m_owner.activeItems)
                {
                    if(!this.affectedItems.Contains(active))
                    {
                        if (active.GetComponent<ReduceChargetimeBehaviour>() != null)
                        {
                            active.GetComponent<ReduceChargetimeBehaviour>().stacks += 1;
                            active.GetComponent<ReduceChargetimeBehaviour>().parentItems.Add(this);
                            this.affectedItems.Add(active);
                        }
                        else
                        {
                            active.gameObject.AddComponent<ReduceChargetimeBehaviour>().parentItems.Add(this);
                            this.affectedItems.Add(active);
                        }
                    }
                }
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnUsedPlayerItem += this.MaybeShootEnemies;
        }

        private void MaybeShootEnemies(PlayerController player, PlayerItem item)
        {
            if(player.CurrentRoom != null && player.PlayerHasActiveSynergy("#PERFECTER_CHARGE"))
            {
                if(player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                {
                    foreach(AIActor aiactor in player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                    {
                        GameObject obj = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(153) as Gun).DefaultModule.projectiles[0].gameObject, player.sprite.WorldCenter, Quaternion.Euler(0, 0,
                           BraveMathCollege.Atan2Degrees(aiactor.sprite.WorldCenter - player.sprite.WorldCenter)));
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

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnUsedPlayerItem -= this.MaybeShootEnemies;
            foreach (PlayerItem active in this.affectedItems)
            {
                if(active != null && active.GetComponent<ReduceChargetimeBehaviour>() != null)
                {
                    if(active.GetComponent<ReduceChargetimeBehaviour>().stacks > 1)
                    {
                        active.GetComponent<ReduceChargetimeBehaviour>().stacks -= 1;
                        active.GetComponent<ReduceChargetimeBehaviour>().parentItems.Remove(this);
                    }
                    else
                    {
                        active.GetComponent<ReduceChargetimeBehaviour>().Destroy();
                    }
                }
            }
            this.affectedItems.Clear();
            return base.Drop(player);
        }

        private List<PlayerItem> affectedItems = new List<PlayerItem>();

        public class ReduceChargetimeBehaviour : MonoBehaviour
        {
            public float GetStackedMultiplier()
            {
                float value = 1f;
                for(int i = 0; i < this.stacks; i++)
                {
                    value *= 0.75f;
                }
                return value;
            }

            private void Start()
            {
                this.m_item = base.GetComponent<PlayerItem>();
                this.m_primalDamageCooldown = this.m_item.damageCooldown;
                this.m_primalTimeCooldown = this.m_item.timeCooldown;
                this.m_primalRoomCooldown = this.m_item.roomCooldown;
                if (this.m_item.damageCooldown > 0)
                {
                    this.m_item.damageCooldown *= this.GetStackedMultiplier();
                }
                if (this.m_item.timeCooldown > 0)
                {
                    this.m_item.timeCooldown *= this.GetStackedMultiplier();
                }
                if (this.m_item.roomCooldown > 0)
                {
                    float r = (float)this.m_item.roomCooldown;
                    this.m_item.roomCooldown = Mathf.RoundToInt(r * this.GetStackedMultiplier());
                }
                this.m_roomCooldown = this.m_item.roomCooldown;
                this.m_timeCooldown = this.m_item.timeCooldown;
                this.m_damageCooldown = this.m_item.damageCooldown;
                this.m_item.OnPreDropEvent += this.OnPreActiveItemDrop;
            }

            private void OnPreActiveItemDrop()
            {
                foreach(CarBattery battery in this.parentItems)
                {
                    battery.affectedItems.Remove(this.m_item);
                }
                this.Destroy();
            }

            private void LateUpdate()
            {
                if (this.m_item.damageCooldown != this.m_damageCooldown && this.m_item.damageCooldown > 0)
                {
                    this.m_primalDamageCooldown = this.m_item.damageCooldown;
                    this.m_item.damageCooldown *= this.GetStackedMultiplier();
                }
                else if(this.stacks != this.stacksLast && this.m_item.damageCooldown > 0)
                {
                    this.m_item.damageCooldown = this.m_primalDamageCooldown * this.GetStackedMultiplier();
                }
                if (this.m_item.timeCooldown != this.m_timeCooldown && this.m_item.timeCooldown > 0)
                {
                    this.m_primalTimeCooldown = this.m_item.timeCooldown;
                    this.m_item.timeCooldown *= this.GetStackedMultiplier();
                }
                else if (this.stacks != this.stacksLast && this.m_item.timeCooldown > 0)
                {
                    this.m_item.timeCooldown = this.m_primalTimeCooldown * this.GetStackedMultiplier();
                }
                if (this.m_item.roomCooldown != this.m_roomCooldown && this.m_item.roomCooldown > 0)
                {
                    this.m_primalRoomCooldown = this.m_item.roomCooldown;
                    float r = (float)this.m_item.roomCooldown;
                    this.m_item.roomCooldown = Mathf.RoundToInt(r * this.GetStackedMultiplier());
                }
                else if (this.stacks != this.stacksLast && this.m_item.roomCooldown > 0)
                {
                    float r = (float)this.m_primalRoomCooldown;
                    this.m_item.roomCooldown = Mathf.RoundToInt(r * this.GetStackedMultiplier());
                }
                this.m_damageCooldown = this.m_item.damageCooldown;
                this.m_timeCooldown = this.m_item.timeCooldown;
                this.m_roomCooldown = this.m_item.roomCooldown;
                this.stacksLast = this.stacks;
            }

            public void Destroy()
            {
                this.m_item.damageCooldown = this.m_primalDamageCooldown;
                this.m_item.timeCooldown = this.m_primalTimeCooldown;
                this.m_item.roomCooldown = this.m_primalRoomCooldown;
                UnityEngine.GameObject.Destroy(this);
            }

            private float m_damageCooldown = -1;
            private float m_timeCooldown = -1;
            private int m_roomCooldown = -1;
            private float m_primalDamageCooldown = -1;
            private float m_primalTimeCooldown = -1;
            private int m_primalRoomCooldown = -1;
            private PlayerItem m_item;
            public List<CarBattery> parentItems = new List<CarBattery>();
            public int stacks = 1;
            public int stacksLast = 1;
        }
    }
}
