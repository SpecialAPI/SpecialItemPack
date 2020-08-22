using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class FlatBullets : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Flat Bullets";
            string resourceName = "SpecialItemPack/Resources/flatbullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<FlatBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Flat Damage";
            string longDesc = "Bullets can become very flat. The flatter they are = the harder they are.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.B;
            item.AddToBlacksmithShop();
            item.AddToTrorkShop();
            item.PlaceItemInAmmonomiconAfterItemById(523);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.BecomeFlat;
        }

        protected override void OnDestroy()
        {
            if (this.m_owner != null)
            {
                this.m_owner.PostProcessProjectile -= this.BecomeFlat;
            }
            base.OnDestroy();
        }

        protected override void Update()
        {
            base.Update();
            if(this.m_pickedUp && this.m_owner != null)
            {
                if(this.m_owner.PlayerHasActiveSynergy("#GIANTS_PLAYTOYLLETS"))
                {
                    int id = -1;
                    foreach (PassiveItem passive in this.m_owner.passiveItems)
                    {
                        if (passive is StraightBullets)
                        {
                            id = passive.PickupObjectId;
                            break;
                        }
                    }
                    if(id > 0)
                    {
                        this.m_owner.RemovePassiveItem(id);
                    }
                    LootEngine.GivePrefabToPlayer(PickupObjectDatabase.GetById(SpecialItemIds.GiantsPlaytoyllets).gameObject, this.m_owner);
                    this.m_owner.RemovePassiveItem(this.PickupObjectId);
                }
            }
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
                float num = y > 0 ? (y + Mathf.Abs(base.projectile.Speed * BraveTime.DeltaTime)) : (y - Mathf.Abs(base.projectile.Speed * BraveTime.DeltaTime));
                base.projectile.AdditionalScaleMultiplier = base.projectile.AdditionalScaleMultiplier > 0 ? (base.projectile.AdditionalScaleMultiplier + Mathf.Abs(base.projectile.Speed * BraveTime.DeltaTime)) : 
                    (base.projectile.AdditionalScaleMultiplier - Mathf.Abs(base.projectile.Speed * BraveTime.DeltaTime));
                base.sprite.scale = new Vector3(base.sprite.scale.x, num, base.sprite.scale.z);
                if (base.specRigidbody != null)
                {
                    base.specRigidbody.UpdateCollidersOnScale = true;
                }
                proj.baseData.damage += (base.projectile.Speed / 2) * BraveTime.DeltaTime;
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
