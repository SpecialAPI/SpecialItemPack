using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;

namespace SpecialItemPack
{
    class JunkArmor : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Junk Armor";
            string resourceName = "SpecialItemPack/Resources/JunkArmor";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<JunkArmor>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Next Time Protect Yourself";
            string longDesc = "Just an armor shield made out of junk. Because of that it's pretty soft, but the more junk you attach to it, the harder it becomes for bullets to get through it.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.C;
            item.PlaceItemInAmmonomiconAfterItemById(65);
        }

        protected override void Update()
        {
            base.Update();
            if(this.m_owner != null && this.m_pickedUp)
            {
                if (this.m_owner.passiveItems != null)
                {
                    int num = 0;
                    foreach (PassiveItem passive in this.m_owner.passiveItems)
                    {
                        if (passive is BasicStatPickup)
                        {
                            if ((passive as BasicStatPickup).IsJunk)
                            {
                                num++;
                                if (passive.gameObject.GetComponent<NoArmorForJunkBehaviour>() == null)
                                {
                                    if (this.m_owner.healthHaver != null)
                                    {
                                        this.m_owner.healthHaver.Armor += 1;
                                        passive.gameObject.AddComponent<NoArmorForJunkBehaviour>();
                                    }
                                }
                            }
                        }
                    }
                    num = Mathf.Max(0, num);
                    bool hasSynergy = this.m_owner.PlayerHasActiveSynergy("#JW2");
                    if (num != this.JunkLast || hasSynergy != this.HasSynergyLast)
                    {
                        this.RemoveStat(PlayerStats.StatType.Damage);
                        if (hasSynergy)
                        {
                            this.AddStat(PlayerStats.StatType.Damage, (float)num / 12.5f, StatModifier.ModifyMethod.ADDITIVE);
                        }
                        this.JunkLast = num;
                        this.HasSynergyLast = hasSynergy;
                    }
                }
            }
        }

        private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier statModifier = new StatModifier();
            statModifier.amount = amount;
            statModifier.statToBoost = statType;
            statModifier.modifyType = method;
            foreach (StatModifier statModifier2 in this.passiveStatModifiers)
            {
                bool flag = statModifier2.statToBoost == statType;
                if (flag)
                {
                    return;
                }
            }
            bool flag2 = this.passiveStatModifiers == null;
            if (flag2)
            {
                this.passiveStatModifiers = new StatModifier[]
                {
                    statModifier
                };
                return;
            }
            this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[]
            {
                statModifier
            }).ToArray<StatModifier>();
        }

        private void RemoveStat(PlayerStats.StatType statType)
        {
            List<StatModifier> list = new List<StatModifier>();
            for (int i = 0; i < this.passiveStatModifiers.Length; i++)
            {
                bool flag = this.passiveStatModifiers[i].statToBoost != statType;
                if (flag)
                {
                    list.Add(this.passiveStatModifiers[i]);
                }
            }
            this.passiveStatModifiers = list.ToArray();
        }

        private float JunkLast = 0;
        private bool HasSynergyLast = false;

        private class NoArmorForJunkBehaviour : MonoBehaviour
        {
        }
    }
}
