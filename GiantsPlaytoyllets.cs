using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;

namespace SpecialItemPack
{
    class GiantsPlaytoyllets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Giant's Playtoyllets";
            string resourceName = "SpecialItemPack/Resources/GiantsPlaytoyllets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<GiantsPlaytoyllets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Big, Very Big";
            string longDesc = "Bullets will become giant. The bigger they are = the harder they are.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.SPECIAL;
            item.PlaceItemInAmmonomiconAfterItemById(523);
            SpecialItemIds.GiantsPlaytoyllets = item.PickupObjectId;
            Game.Items.Rename("spapi:giant's_playtoyllets", "spapi:giants_playtoyllets");
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.BecomeFlat;
        }

        public void BecomeFlat(Projectile proj, float f)
        {
            proj.gameObject.AddComponent<FlatBehaviour>();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.BecomeFlat;
            return base.Drop(player);
        }

        private class FlatBehaviour : BraveBehaviour
        {
            private void Start()
            {
                base.projectile.OnPostUpdate += this.OnPostUpdate;
            }

            private void OnPostUpdate(Projectile proj)
            {
                if (!base.sprite)
                {
                    return;
                }
                float y = base.sprite.scale.y;
                float num = y > 0 ? (y + Mathf.Abs((base.projectile.Speed * 1.15f) * BraveTime.DeltaTime)) : (y - Mathf.Abs((base.projectile.Speed * 1.15f) * BraveTime.DeltaTime));
                base.projectile.AdditionalScaleMultiplier = base.projectile.AdditionalScaleMultiplier > 0 ? (base.projectile.AdditionalScaleMultiplier + Mathf.Abs((base.projectile.Speed * 1.15f) * BraveTime.DeltaTime)) :
                    (base.projectile.AdditionalScaleMultiplier - Mathf.Abs((base.projectile.Speed * 1.15f) * BraveTime.DeltaTime));
                float x = base.sprite.scale.y;
                float num2 = x > 0 ? (x + Mathf.Abs((base.projectile.Speed * 1.15f) * BraveTime.DeltaTime)) : (x - Mathf.Abs((base.projectile.Speed * 1.15f) * BraveTime.DeltaTime));
                base.projectile.AdditionalScaleMultiplier = base.projectile.AdditionalScaleMultiplier > 0 ? (base.projectile.AdditionalScaleMultiplier + Mathf.Abs((base.projectile.Speed * 1.15f) * BraveTime.DeltaTime)) :
                    (base.projectile.AdditionalScaleMultiplier - Mathf.Abs((base.projectile.Speed * 1.15f) * BraveTime.DeltaTime));
                base.sprite.scale = new Vector3(num2, num, base.sprite.scale.z);
                if (base.specRigidbody != null)
                {
                    base.specRigidbody.UpdateCollidersOnScale = true;
                }
                proj.baseData.damage += ((base.projectile.Speed) * 1.25f) * BraveTime.DeltaTime;
                if (num > 1.5f)
                {
                    Vector3 size = base.sprite.GetBounds().size;
                    if (size.x > 4f || size.y > 4f)
                    {
                        base.sprite.HeightOffGround = UnityEngine.Random.Range(0f, -3f);
                    }
                }
            }
        }
    }
}
