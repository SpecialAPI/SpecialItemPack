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
    class Underflow : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Underflow";
            string resourceName = "SpecialItemPack/Resources/Underflow";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Underflow>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "No Underflows!";
            string longDesc = "When \"worse than normal\" is \"too bad\".";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.B;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.AddToOldRedShop();
            item.PlaceItemInAmmonomiconAfterItemById(344);
        }

        protected override void Update()
        {
            base.Update();
            if (this.m_pickedUp && this.m_owner != null)
            {
                this.ProcessStat(PlayerStats.StatType.MovementSpeed, this.m_owner, 7, ref this.movementSpeedIncrease, false);
                this.ProcessStat(PlayerStats.StatType.RateOfFire, this.m_owner, 1, ref this.rateOfFireIncrease, false);
                this.ProcessStat(PlayerStats.StatType.Health, this.m_owner, 3, ref this.healthIncrease, false);
                this.ProcessStat(PlayerStats.StatType.Coolness, this.m_owner, 0, ref this.coolnessIncrease, false);
                this.ProcessStat(PlayerStats.StatType.Damage, this.m_owner, 1, ref this.damageIncrease, false);
                this.ProcessStat(PlayerStats.StatType.ProjectileSpeed, this.m_owner, 1, ref this.projectileSpeedIncrease, false);
                this.ProcessStat(PlayerStats.StatType.AmmoCapacityMultiplier, this.m_owner, 1, ref this.ammoCapacityMultiplierIncrease, false);
                this.ProcessStat(PlayerStats.StatType.ReloadSpeed, this.m_owner, 1, ref this.reloadSpeedDecrease, true);
                this.ProcessStat(PlayerStats.StatType.AdditionalShotPiercing, this.m_owner, 0, ref this.additionalShotPiercingIncrease, false);
                this.ProcessStat(PlayerStats.StatType.GlobalPriceMultiplier, this.m_owner, 1, ref this.globalPriceMultiplierIncrease, true);
                this.ProcessStat(PlayerStats.StatType.KnockbackMultiplier, this.m_owner, 1, ref this.knockbackMultiplierIncrease, false);
                this.ProcessStat(PlayerStats.StatType.PlayerBulletScale, this.m_owner, 1, ref this.playerBulletScaleIncrease, false);
                this.ProcessStat(PlayerStats.StatType.AdditionalClipCapacityMultiplier, this.m_owner, 1, ref this.additionalClipCapacityMultiplierIncrease, false);
                this.ProcessStat(PlayerStats.StatType.AdditionalShotBounces, this.m_owner, 0, ref this.additionalShotBouncesIncrease, false);
                this.ProcessStat(PlayerStats.StatType.AdditionalBlanksPerFloor, this.m_owner, 2, ref this.additionalBlanksPerFloorIncrease, false);
                this.ProcessStat(PlayerStats.StatType.ShadowBulletChance, this.m_owner, 0, ref this.shadowBulletsChanceIncrease, false);
                this.ProcessStat(PlayerStats.StatType.ThrownGunDamage, this.m_owner, 1, ref this.thrownGunDamageIncrease, false);
                this.ProcessStat(PlayerStats.StatType.DodgeRollDamage, this.m_owner, 1, ref this.dodgeRollDamageIncrease, false);
                this.ProcessStat(PlayerStats.StatType.DamageToBosses, this.m_owner, 1, ref this.damageToBossesIncrease, false);
                this.ProcessStat(PlayerStats.StatType.EnemyProjectileSpeedMultiplier, this.m_owner, 1, ref this.enemyProjectileSpeedMultiplierDecrease, true);
                this.ProcessStat(PlayerStats.StatType.ExtremeShadowBulletChance, this.m_owner, 0, ref this.extremeShadowBulletChanceIncrease, false);
                this.ProcessStat(PlayerStats.StatType.ChargeAmountMultiplier, this.m_owner, 1, ref this.chargeAmountMultiplierIncrease, false);
                this.ProcessStat(PlayerStats.StatType.RangeMultiplier, this.m_owner, 1, ref this.rangeMultiplierIncrease, false);
                this.ProcessStat(PlayerStats.StatType.DodgeRollDistanceMultiplier, this.m_owner, 1, ref this.dodgeRollDistanceMultiplierIncrease, false);
                this.ProcessStat(PlayerStats.StatType.DodgeRollSpeedMultiplier, this.m_owner, 1, ref this.dodgeRollSpeedMultiplierIncrease, false);
                this.ProcessStat(PlayerStats.StatType.MoneyMultiplierFromEnemies, this.m_owner, 1, ref this.moneyMultiplierFromEnemiesIncrease, false);
                this.m_owner.stats.RecalculateStats(this.m_owner, true, false);
            }
        }

        private void ProcessStat(PlayerStats.StatType type, PlayerController player, float defaultValue, ref float increaseValue, bool decrease = false)
        {
            this.RemoveStat(type);
            if(type == PlayerStats.StatType.Damage)
            {
                ETGModConsole.Log("da: " + (player.stats.GetStatValue(type) - increaseValue < defaultValue));
            }
            if((player.stats.GetStatValue(type) - increaseValue < defaultValue && !decrease) || (player.stats.GetStatValue(type) + increaseValue > defaultValue && decrease))
            {
                increaseValue = defaultValue - player.stats.GetStatValue(type);
                this.AddStat(type, increaseValue, StatModifier.ModifyMethod.ADDITIVE);
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

        private float movementSpeedIncrease = 0;
        private float rateOfFireIncrease = 0;
        private float healthIncrease = 0;
        private float coolnessIncrease = 0;
        private float damageIncrease = 0;
        private float projectileSpeedIncrease = 0;
        private float ammoCapacityMultiplierIncrease = 0;
        private float reloadSpeedDecrease = 0;
        private float additionalShotPiercingIncrease = 0;
        private float knockbackMultiplierIncrease = 0;
        private float globalPriceMultiplierIncrease = 0;
        private float playerBulletScaleIncrease = 0;
        private float additionalClipCapacityMultiplierIncrease = 0;
        private float additionalShotBouncesIncrease = 0;
        private float additionalBlanksPerFloorIncrease = 0;
        private float shadowBulletsChanceIncrease = 0;
        private float thrownGunDamageIncrease = 0;
        private float dodgeRollDamageIncrease = 0;
        private float damageToBossesIncrease = 0;
        private float enemyProjectileSpeedMultiplierDecrease = 0;
        private float extremeShadowBulletChanceIncrease = 0;
        private float chargeAmountMultiplierIncrease = 0;
        private float rangeMultiplierIncrease;
        private float dodgeRollDistanceMultiplierIncrease = 0;
        private float dodgeRollSpeedMultiplierIncrease = 0;
        private float moneyMultiplierFromEnemiesIncrease = 0;
    }
}
