using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;

namespace SpecialItemPack
{
    class RedirectRounds : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Redirect Rounds";
            string resourceName = "SpecialItemPack/Resources/RedirectBullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<RedirectRounds>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Can't Miss";
            string longDesc = "Bullets will be redirected to enemies when they hit a wall.\n\nThese bullets learned how to change their direction. With that ability, you don't even need to aim anymore.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.A;
            item.AddToBlacksmithShop();
            item.AddToTrorkShop();
            item.PlaceItemInAmmonomiconAfterItemById(172);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.ApplyModifier;
        }

        public void ApplyModifier(Projectile proj, float f)
        {
            proj.specRigidbody.OnPreTileCollision += new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnCollision);
        }

        public void OnCollision(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            if(myRigidbody.projectile != null)
            {
                Projectile proj = myRigidbody.projectile;
                RoomHandler room;
                Vector2 vector;
                if(proj.sprite != null)
                {
                    vector = proj.sprite.WorldCenter;
                }
                else if(proj.specRigidbody != null)
                {
                    vector = proj.specRigidbody.UnitCenter;
                }
                else
                {
                    vector = proj.transform.position.XY();
                }
                room = vector.GetAbsoluteRoom();
                if(room != null)
                {
                    float num = -1f;
                    AIActor actor = room.GetNearestEnemy(vector, out num, true, false);
                    if(actor != null)
                    {
                        Vector2 vector2 = actor.CenterPosition - vector;
                        proj.SendInDirection(vector2, false, true);
                        proj.specRigidbody.OnPreTileCollision -= new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnCollision);
                        PhysicsEngine.SkipCollision = true;
                    }
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.ApplyModifier;
            return base.Drop(player);
        }
    }
}
