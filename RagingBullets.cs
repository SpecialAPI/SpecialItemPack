using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class RagingBullets : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Raging Bullets";
            string resourceName = "SpecialItemPack/Resources/RagingBullets";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<RagingBullets>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "You're Making Me ANGRY!";
            string longDesc = "Your bullets will go berserk for a short period of time.\n\nSomebody combined Angry Bullets and the Bloody 9mm. This was a big mistake.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 750);
            item.consumable = false;
            item.quality = ItemQuality.B;
            item.AddToTrorkShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(323);
        }

        protected override void OnPreDrop(PlayerController user)
        {
            if (this.m_isCurrentlyActive)
            {
                this.EndEffect(user);
            }
            base.OnPreDrop(user);
        }

        public override void Update()
        {
            base.Update();
            if(this.m_pickedUp && this.m_isCurrentlyActive && this.LastOwner != null)
            {
                this.LastOwner.baseFlatColorOverride = new Color(0.5f, 0f, 0f).WithAlpha(0.75f);
                particleCounter += BraveTime.DeltaTime * 40f;
                if (particleCounter > 1f)
                {
                    int num = Mathf.FloorToInt(particleCounter);
                    particleCounter %= 1f;
                    GlobalSparksDoer.DoRandomParticleBurst(num, this.LastOwner.sprite.WorldBottomLeft.ToVector3ZisY(0f), this.LastOwner.sprite.WorldTopRight.ToVector3ZisY(0f), Vector3.up, 90f, 0.5f, null, null, null, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
                }
            }
        }

        protected override void DoEffect(PlayerController user)
        {
            user.PostProcessProjectile += this.PostProcessRage;
            this.particleCounter = 0f;
            this.StartCoroutine(ItemBuilder.HandleDuration(this, user.PlayerHasActiveSynergy("#>=(") ? 25f : 15f, user, this.EndEffect));
            AkSoundEngine.PostEvent("Play_BOSS_bulletbros_anger_01", base.gameObject);
        }

        private void PostProcessRage(Projectile proj, float f)
        {
            BounceProjModifier component = proj.gameObject.GetOrAddComponent<BounceProjModifier>();
            PierceProjModifier component2 = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
            proj.baseData.speed += 6;
            proj.baseData.damage += 3;
            if (component)
            {
                component.numberOfBounces += 2;
            }
            if (component2)
            {
                component2.penetration += 3;
                component2.penetratesBreakables = true;
                component2.preventPenetrationOfActors = false;
                component2.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
            }
            HomingModifier homingModifier = proj.gameObject.AddComponent<HomingModifier>();
            homingModifier.HomingRadius = 8f;
            homingModifier.AngularVelocity = 360f;
            proj.AdjustPlayerProjectileTint(new Color(0.5f, 0f, 0f).WithAlpha(0.75f), 0, 0);
        }

        private void EndEffect(PlayerController user)
        {
            user.PostProcessProjectile -= this.PostProcessRage;
            user.baseFlatColorOverride = user.baseFlatColorOverride.WithAlpha(0);
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user);
        }

        private float particleCounter = 0f;
    }
}
