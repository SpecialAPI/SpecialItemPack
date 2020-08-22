using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using System.Collections;

namespace SpecialItemPack
{
    class InvisibleBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Invisibullets";
            string resourceName = "SpecialItemPack/Resources/InvisibleBullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<InvisibleBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Hidden Danger";
            string longDesc = "Increases damage, but makes your bullets invisible.\n\nOriginally developed for sneaky snipers, they were accidentally made harder for aiming.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Damage, 0.33f, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = ItemQuality.B;
            item.AddToBlacksmithShop();
            item.AddToTrorkShop();
            item.PlaceItemInAmmonomiconAfterItemById(533);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.Invisible;
        }

        public void Invisible(Projectile proj, float f)
        {
            if(proj != null && proj.sprite != null)
            {
                proj.sprite.renderer.enabled = false;
            }
            if(proj.GetComponentInChildren<TrailController>() != null)
            {
                Destroy(proj.GetComponentInChildren<TrailController>());
            }
            proj.StartCoroutine(this.DelayedDestroyParticles(proj));
            if(proj.ParticleTrail != null)
            {
                BraveUtility.EnableEmission(proj.ParticleTrail, false);
            }
            if(proj.particleSystem != null)
            {
                Destroy(proj.particleSystem);
            }
            if (proj.CustomTrailRenderer != null)
            {
                proj.CustomTrailRenderer.enabled = false;
            }
            if (proj.TrailRenderer != null)
            {
                proj.TrailRenderer.enabled = false;
            }
            if (proj.TrailRendererController != null)
            {
                proj.TrailRendererController.enabled = false;
            }
        }

        public IEnumerator DelayedDestroyParticles(Projectile proj)
        {
            yield return null;

            yield break;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.Invisible;
            return base.Drop(player);
        }
    }
}
