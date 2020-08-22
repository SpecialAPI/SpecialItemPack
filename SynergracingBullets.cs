using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class SynergracingBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Synergracing Bullets";
            string resourceName = "SpecialItemPack/Resources/SynergyBullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<SynergracingBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Synergies = Damage";
            string longDesc = "Increases damage of the owner depending of their synergies.\n\nThe favorite bullets of Synergrace. Theese bullets like love and good matches, and get stronger the more love is in your equipment";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.C;
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(822);
            item.SetupUnlockOnFlag(GungeonFlags.SYNERGRACE_UNLOCKED, true);
        }

        protected override void Update()
        {
            base.Update();
            if (this.m_pickedUp && this.m_owner != null)
            {
                int s = this.m_owner.ActiveExtraSynergies.Count;
                this.synergies = s;
                this.hasCompletion = this.m_owner.PlayerHasCompletionItem() || this.m_owner.PlayerHasCompletionGun();
                if(this.synergies != this.synergiesLast || this.hasCompletion != this.hasCompletionLast)
                {
                    float dmg = ((float)this.synergies) / 7;
                    if (this.hasCompletion)
                    {
                        dmg = 0.33f;
                    }
                    this.RemoveStat(PlayerStats.StatType.Damage);
                    this.AddStat(PlayerStats.StatType.Damage, dmg, StatModifier.ModifyMethod.ADDITIVE);
                    this.m_owner.stats.RecalculateStats(this.m_owner, true, false);
                    this.synergiesLast = this.synergies;
                    this.hasCompletionLast = this.hasCompletion;
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

        private int synergies = 0;
        private int synergiesLast = 0;
        private bool hasCompletion = false;
        private bool hasCompletionLast = false;
    }
}
