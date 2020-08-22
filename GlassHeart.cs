using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;

namespace SpecialItemPack
{
    class GlassHeart : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Glass Heart (SpecialItemPack)";
            string resourceName = "SpecialItemPack/Resources/GlassHeart";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<GlassHeart>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Strength of Glass!";
            string longDesc = "Increases all stats, but sets it's owner to their most vulnerable point. Can be dropped, but will be shattered in the procces\n\nThe item from the Glass Kingdom, the home of the Lady of Pane. Originally developed to increase strength of glass people, mainly soldiers. " +
                "However, that item was brought to the Gungeon by a mysterious traveler. Because the item was made for glass people and nobody thought someone from outside of the Glass Kingdom would use it, creators of this item did not prevent the item " +
                "from shattering non-glass people.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Accuracy, 0.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, -1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Coolness, 1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.KnockbackMultiplier, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MoneyMultiplierFromEnemies, 1.25f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.PlayerBulletScale, 1.3f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RateOfFire, 1.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ReloadSpeed, 0.5f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ShadowBulletChance, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ExtremeShadowBulletChance, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.GlobalPriceMultiplier, 0.85f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ProjectileSpeed, 1.2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ThrownGunDamage, 2f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            item.quality = ItemQuality.C;
            GlassHeart.HeartborkenSpriteId = SpriteBuilder.AddSpriteToCollection("SpecialItemPack/Resources/GlassHeartbroken", item.sprite.Collection);
            item.PlaceItemInAmmonomiconAfterItemById(425);
            Game.Items.Rename("spapi:glass_heart_(specialitempack)", "spapi:glass_heart");
            item.SetName("Glass Heart");
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.PostProjectile;
            player.PostProcessBeamTick += this.PostBeamTick;
            player.PostProcessBeam += this.PostBeam;
            player.healthHaver.OnDeath += this.OnReceivedDamage;
        }

        public void PostProjectile(Projectile proj, float f)
        {
            proj.ignoreDamageCaps = true;
        }

        public void PostBeamTick(BeamController beam, SpeculativeRigidbody rigidbody, float f)
        {
            beam.projectile.ignoreDamageCaps = true;
        }

        public void PostBeam(BeamController beam)
        {
            beam.projectile.ignoreDamageCaps = true;
        }
        
        public void OnReceivedDamage(Vector2 direction)
        {
            this.sprite.SetSprite(GlassHeart.HeartborkenSpriteId);
        }

        protected override void Update()
        {
            base.Update();
            if(this.m_pickedUp && this.m_owner != null)
            {
                if(this.m_owner.healthHaver != null && !this.m_owner.healthHaver.NextShotKills)
                {
                    this.m_owner.healthHaver.NextShotKills = true;
                }
            }
        }

        protected override void OnDestroy()
        {
            if ((PickupObjectDatabase.GetById(565) as PlayerOrbitalItem).BreakVFX && this.sprite != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(565) as PlayerOrbitalItem).BreakVFX, this.sprite.WorldCenter.ToVector3ZisY(0f), Quaternion.identity);
                }
                SpawnManager.SpawnVFX((PickupObjectDatabase.GetById(538) as SilverBulletsPassiveItem).SynergyPowerVFX, this.sprite.WorldCenter.ToVector3ZisY(0f), Quaternion.identity).GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(
                    this.sprite.WorldCenter.ToVector3ZisY(0f), tk2dBaseSprite.Anchor.MiddleCenter);
            }
            base.OnDestroy();
        }

        public void Break()
        {
            this.m_pickedUp = true;
            UnityEngine.Object.Destroy(base.gameObject, 1f);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProjectile;
            player.PostProcessBeamTick -= this.PostBeamTick;
            player.PostProcessBeam -= this.PostBeam;
            player.healthHaver.NextShotKills = false;
            AkSoundEngine.PostEvent("Play_OBJ_glass_shatter_01", player.gameObject);
            DebrisObject debrisObject = base.Drop(player);
            GlassHeart component = debrisObject.GetComponent<GlassHeart>();
            component.m_pickedUpThisRun = true;
            component.Break();
            return debrisObject;
        }

        private static int HeartborkenSpriteId;
    }
}
