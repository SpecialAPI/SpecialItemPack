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
    class EvoBullets : GunVolleyModificationItem
    {
        public static void Init()
        {
            string itemName = "Evo-Bullets";
            string resourceName = "SpecialItemPack/Resources/Evo1Bullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<EvoBullets>();
            EvoBullets.spriteIds = new int[EvoBullets.spritePaths.Length];
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Line of Revolverution";
            string longDesc = "Theese bullets adapt.";
            EvoBullets.spriteIds[0] = item.sprite.spriteId;
            EvoBullets.spriteIds[1] = SpriteBuilder.AddSpriteToCollection(EvoBullets.spritePaths[1], item.sprite.Collection);
            EvoBullets.spriteIds[2] = SpriteBuilder.AddSpriteToCollection(EvoBullets.spritePaths[2], item.sprite.Collection);
            EvoBullets.spriteIds[3] = SpriteBuilder.AddSpriteToCollection(EvoBullets.spritePaths[3], item.sprite.Collection);
            EvoBullets.spriteIds[4] = SpriteBuilder.AddSpriteToCollection(EvoBullets.spritePaths[4], item.sprite.Collection);
            EvoBullets.spriteIds[5] = SpriteBuilder.AddSpriteToCollection(EvoBullets.spritePaths[5], item.sprite.Collection);
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.C;
            item.AddToGooptonShop();
            item.AddsModule = false;
            item.DuplicatesOfEachModule = 0;
            item.DuplicatesOfBaseModule = 0;
            item.DuplicateAngleOffset = 0;
            item.DuplicateAngleBaseOffset = 0;
            item.EachModuleOffsetAngle = 0;
            item.EachSingleModuleMinAngleVariance = 0;
            item.AddsHelixModifier = false;
            item.AddsOrbitModifier = false;
            item.SetupUnlockOnFlag(GungeonFlags.TUTORIAL_COMPLETED, true);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnKilledEnemyContext += this.KilledEnemyContext;
            player.PostProcessProjectile += this.OnPostProcessProjectile;
        }

        private void OnPostProcessProjectile(Projectile proj, float f)
        {
            if (this.addsHoming)
            {
                HomingModifier homingMod = proj.GetComponent<HomingModifier>();
                if(homingMod == null)
                {
                    homingMod = proj.gameObject.AddComponent<HomingModifier>();
                    homingMod.HomingRadius = this.homingRadius;
                    homingMod.AngularVelocity = this.homingVelocity;
                }
                else
                {
                    homingMod.HomingRadius += this.homingRadius;
                    homingMod.AngularVelocity += this.homingVelocity;
                }
            }
            proj.baseData.damage *= this.damageMultiplier;
            proj.baseData.speed *= this.speedMultiplier;
            if (this.penetratesDamageCaps)
            {
                proj.ignoreDamageCaps = true;
            }
            proj.BossDamageMultiplier *= this.bossDamageMultiplier;
            if (this.modifiesSpeed)
            {
                proj.ModifyVelocity += this.ModifySpeed;
            }
        }

        private Vector2 ModifySpeed(Vector2 invel)
        {
            Vector2 vector = invel;
            vector *= 1.25f;
            return vector;
        }

        private void KilledEnemyContext(PlayerController sourcePlayer, HealthHaver killedEnemy)
        {
            if (killedEnemy)
            {
                AIActor component = killedEnemy.GetComponent<AIActor>();
                if (component)
                {
                    this.m_enemiesKilled.Add(component.EnemyGuid);
                    this.UpdateTier(sourcePlayer);
                }
            }
        }

        private void UpdateTier(PlayerController sourcePlayer)
        {
            int num = this.m_enemiesKilled.Count;
            int num2 = this.TypesPerForm;
            if (sourcePlayer && sourcePlayer.HasActiveBonusSynergy(CustomSynergyType.NATURAL_SELECTION, false))
            {
                num2 = Mathf.Max(1, this.TypesPerForm - 2);
            }
            if (sourcePlayer && sourcePlayer.HasActiveBonusSynergy(CustomSynergyType.POWERHOUSE_OF_THE_CELL, false))
            {
                num += num2;
            }
            int num3 = Mathf.FloorToInt((float)num / (float)num2);
            num3 = Mathf.Min(num3, 5);
            if (num3 != this.m_currentForm)
            {
                this.m_currentForm = num3;
                this.TransformToForm(this.m_currentForm);
            }
        }

        private void TransformToForm(int form)
        {
            if(form == 0)
            {
                this.addsHoming = false;
                this.homingRadius = 0f;
                this.homingVelocity = 0f;
                this.damageMultiplier = 1.15f;
                this.penetratesDamageCaps = false;
                this.bossDamageMultiplier = 1f;
                this.speedMultiplier = 1f;
                this.modifiesSpeed = false;
                this.AddsModule = false;
                this.DuplicatesOfEachModule = 0;
            }
            else if(form == 1)
            {
                this.addsHoming = false;
                this.homingRadius = 0f;
                this.homingVelocity = 0f;
                this.damageMultiplier = 1.2f;
                this.penetratesDamageCaps = false;
                this.bossDamageMultiplier = 1f;
                this.speedMultiplier = 1f;
                this.modifiesSpeed = false;
                this.AddsModule = false;
                this.DuplicatesOfEachModule = 0;
            }
            else if(form == 2)
            {
                this.addsHoming = false;
                this.homingRadius = 0f;
                this.homingVelocity = 0f;
                this.damageMultiplier = 1.25f;
                this.penetratesDamageCaps = false;
                this.bossDamageMultiplier = 1f;
                this.speedMultiplier = 1f;
                this.modifiesSpeed = false;
                this.AddsModule = false;
                this.DuplicatesOfEachModule = 0;
            }
            else if (form == 3)
            {
                this.addsHoming = false;
                this.homingRadius = 0f;
                this.homingVelocity = 0f;
                this.damageMultiplier = 0.45f;
                this.penetratesDamageCaps = false;
                this.bossDamageMultiplier = 1f;
                this.speedMultiplier = 1f;
                this.modifiesSpeed = false;
                this.AddsModule = false;
                this.DuplicatesOfEachModule = 2;
            }
            else if (form == 4)
            {
                this.addsHoming = true;
                this.homingRadius = 9999f;
                this.homingVelocity = 9999f;
                this.damageMultiplier = 0.45f;
                this.penetratesDamageCaps = false;
                this.bossDamageMultiplier = 1f;
                this.speedMultiplier = 1f;
                this.modifiesSpeed = false;
                this.AddsModule = false;
                this.DuplicatesOfEachModule = 0;
            }
            else if (form == 5)
            {
                this.addsHoming = false;
                this.homingRadius = 420f;
                this.homingVelocity = 10f;
                this.damageMultiplier = 2f;
                this.penetratesDamageCaps = false;
                this.bossDamageMultiplier = 1f;
                this.speedMultiplier = 0.1f;
                this.modifiesSpeed = true;
                this.AddsModule = false;
                this.DuplicatesOfEachModule = 0;
            }
            
        }

        protected override void Update()
        {
            base.Update();
            this.sprite.SetSprite(EvoBullets.spriteIds[this.m_currentForm]);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.OnPostProcessProjectile;
            player.OnKilledEnemyContext -= this.KilledEnemyContext;
            return base.Drop(player);
        }

        private HashSet<string> m_enemiesKilled = new HashSet<string>();
        private int TypesPerForm = 5;
        private int m_currentForm = 0;
        private static readonly string[] spritePaths = new string[]
        {
            "SpecialItemPack/Resources/Evo1Bullets",
            "SpecialItemPack/Resources/Evo2Bullets",
            "SpecialItemPack/Resources/Evo3Bullets",
            "SpecialItemPack/Resources/Evo4Bullets",
            "SpecialItemPack/Resources/Evo5Bullets",
            "SpecialItemPack/Resources/Evo6Bullets"
        };
        private static int[] spriteIds;
        private bool addsHoming = false;
        private float homingRadius = 0f;
        private float homingVelocity = 0f;
        private float damageMultiplier = 1.15f;
        private bool penetratesDamageCaps = false;
        private float bossDamageMultiplier = 1f;
        private float speedMultiplier = 1f;
        private bool modifiesSpeed = false;
    }
}
