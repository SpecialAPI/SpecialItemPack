using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class SprenThing : PlayerItem
    {
        public static void Init()
        {
            string itemName = "The Sprun Bullet";
            string resourceName = "SpecialItemPack/Resources/SprunBullet";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<SprenThing>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Become the Bullet";
            string longDesc = "Grants invulnerability, flight and a serious weapon for a small period of time\n\nMade by a legendary blacksmith, who saw what sprun is able to do. He then thought, how much power it would grant, if something could turn anyone" +
                " into a similar thing... and made this.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 250);
            item.consumable = false;
            item.quality = ItemQuality.S;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(578);
            item.SetupUnlockOnStat(TrackedStats.RUNS_PLAYED_POST_FTA, DungeonPrerequisite.PrerequisiteOperation.GREATER_THAN, 9);
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            AkSoundEngine.PostEvent("Play_WPN_deck4rd_shot_01", base.gameObject);
            user.sprite.renderer.enabled = false;
            SpriteOutlineManager.ToggleOutlineRenderers(user.sprite, false);
            this.extentObj = UnityEngine.Object.Instantiate<GameObject>((PickupObjectDatabase.GetById(578) as PlayerOrbitalItem).OrbitalFollowerPrefab.gameObject, user.transform.position, Quaternion.identity);
            Gun limitGun = PickupObjectDatabase.GetById(546) as Gun;
            this.m_extantGun = user.inventory.AddGunToInventory(limitGun, true);
            this.m_extantGun.CanBeDropped = false;
            this.m_extantGun.CanBeSold = false;
            user.inventory.GunLocked.AddOverride("sprun bullet", null);
            user.SetIsFlying(true, "spren bullet", true, false);
            float duration = 20f;
            if (user.PlayerHasActiveSynergy("#DOUBLE_DOUBLE_SPRUN"))
            {
                duration = 40f;
            }
            base.StartCoroutine(ItemBuilder.HandleDuration(this, duration, user, this.EndEffect));
        }

        protected override void OnPreDrop(PlayerController user)
        {
            if (this.m_isCurrentlyActive)
            {
                this.EndEffect(user);
            }
            base.OnPreDrop(user);
        }

        private void EndEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_WPN_spellactionrevolver_reload_01", base.gameObject);
            user.sprite.renderer.enabled = true;
            SpriteOutlineManager.ToggleOutlineRenderers(user.sprite, true);
            UnityEngine.Object.Destroy(this.extentObj);
            this.extentObj = null;
            user.inventory.RemoveGunFromInventory(this.m_extantGun);
            user.inventory.GunLocked.RemoveOverride("sprun bullet");
            user.SetIsFlying(false, "spren bullet", true, false);
            user.healthHaver.IsVulnerable = true;
        }

        public void LateUpdate()
        {
            base.Update();
            if(this.extentObj != null)
            {
                this.extentObj.transform.position = this.LastOwner.sprite.transform.position.XY() + this.LastOwner.sprite.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter).Rotate(this.LastOwner.sprite.transform.eulerAngles.z);
                this.LastOwner.healthHaver.IsVulnerable = false;
                this.LastOwner.sprite.renderer.enabled = false;
                SpriteOutlineManager.ToggleOutlineRenderers(this.LastOwner.sprite, false);
                this.extentObj.transform.parent = SpawnManager.Instance.Projectiles;
            }
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user.CurrentRoom != null && !user.CurrentRoom.IsWinchesterArcadeRoom;
        }

        private Gun m_extantGun;

        public GameObject extentObj;
    }
}
