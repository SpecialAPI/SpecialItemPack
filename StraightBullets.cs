using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class StraightBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Straight Bullets";
            string resourceName = "SpecialItemPack/Resources/StraightBullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<StraightBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Straight To You!";
            string longDesc = "Bullets can become very straight. The straighter they are = the harder they are.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.B;
            item.AddToBlacksmithShop();
            item.AddToTrorkShop();
            item.PlaceItemInAmmonomiconAfterItemById(523);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.BecomeStraight;
        }

        public void BecomeStraight(Projectile proj, float f)
        {
            proj.gameObject.AddComponent<StraightBehaviour>();
        }

        protected override void OnDestroy()
        {
            if(this.m_owner != null)
            {
                this.m_owner.PostProcessProjectile -= this.BecomeStraight;
            }
            base.OnDestroy();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.BecomeStraight;
            return base.Drop(player);
        }

        private class StraightBehaviour : BraveBehaviour
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
                float x = base.sprite.scale.x;
                float num = x > 0 ? (x + Mathf.Abs(base.projectile.Speed * BraveTime.DeltaTime)) : (x - Mathf.Abs(base.projectile.Speed * BraveTime.DeltaTime));
                base.projectile.AdditionalScaleMultiplier = base.projectile.AdditionalScaleMultiplier > 0 ? (base.projectile.AdditionalScaleMultiplier + Mathf.Abs(base.projectile.Speed * BraveTime.DeltaTime)) :
                    (base.projectile.AdditionalScaleMultiplier - Mathf.Abs(base.projectile.Speed * BraveTime.DeltaTime));
                base.sprite.scale = new Vector3(num, base.sprite.scale.y, base.sprite.scale.z);
                proj.baseData.damage += (base.projectile.Speed / 2) * BraveTime.DeltaTime;
                if (base.specRigidbody != null)
                {
                    base.specRigidbody.UpdateCollidersOnScale = true;
                }
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
