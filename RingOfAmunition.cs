using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class RingOfAmmunition : CompanionItem
    {
        public static void Init()
        {
            string itemName = "Ring of Live Ammo";
            string resourceName = "SpecialItemPack/Resources/RingOfAmmunition";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<RingOfAmmunition>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Betrayer!";
            string longDesc = "Creates a little bullet kin that will follow the player.\n\nRing worn by a young bullet kin, that was inspired by the Bullet and betrayed his whole kin to be a hero. He is too young to explore dungeons by himself, " +
                "but is still glad to help gungeoneers as a sidekick.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.B;
            item.CompanionGuid = "c1757107b9a44f0c823a707adeb4ae7e";
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(495);
        }

        private void RecoverComapnionSelf()
        {
            if (this.ExtantCompanion != null)
            {
                AIActor aiactor = this.ExtantCompanion.GetComponent<AIActor>();
                CompanionController companionController = this.ExtantCompanion.GetComponent<CompanionController>();
                aiactor.HitByEnemyBullets = false;
                GameManager.Instance.StartCoroutine(this.DecreaseEnemyScale(aiactor));
                aiactor.aiShooter.PostProcessProjectile += delegate (Projectile proj)
                {
                    if (this.m_owner.PlayerHasActiveSynergy("#CHICKEN_FRIENDS"))
                    {
                        CuccoController chickenFriendController = EnemyDatabase.GetOrLoadByGuid((PickupObjectDatabase.GetById(572) as CompanionItem).CompanionGuid).GetComponent<CuccoController>();
                        Vector2 v = GameManager.Instance.MainCameraController.transform.position.XY() + UnityEngine.Random.insideUnitCircle.normalized * GameManager.Instance.MainCameraController.Camera.orthographicSize * 2f;
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(chickenFriendController.MicroCuccoPrefab, v, Quaternion.identity);
                        gameObject.GetComponent<MicroCuccoController>().Initialize(this.m_owner);
                    }
                    proj.baseData.damage = 10;
                };
            }
        }

        protected override void Update()
        {
            base.Update();
            if(this.ExtantCompanion != null && this.ExtantCompanion.GetComponent<AIActor>() != null && this.ExtantCompanion.GetComponent<AIActor>().healthHaver != null)
            {
                if (this.ExtantCompanion.GetComponent<AIActor>().healthHaver.IsVulnerable)
                {
                    this.ExtantCompanion.GetComponent<AIActor>().healthHaver.IsVulnerable = false;
                }
            }
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            if (this.ExtantCompanion != null)
            {
                AIActor aiactor = this.ExtantCompanion.GetComponent<AIActor>();
                CompanionController companionController = this.ExtantCompanion.GetComponent<CompanionController>();
                aiactor.HitByEnemyBullets = false;
                GameManager.Instance.StartCoroutine(this.DecreaseEnemyScale(aiactor));
                aiactor.aiShooter.PostProcessProjectile += delegate (Projectile proj)
                {
                    proj.baseData.damage = 10;
                };
            }
            GameManager.Instance.OnNewLevelFullyLoaded += this.RecoverComapnionSelf;
        }

        public IEnumerator DecreaseEnemyScale(AIActor enemy)
        {
            yield return new WaitForSeconds(0.1f);
            enemy.EnemyScale *= 0.75f;
            yield break;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            GameManager.Instance.OnNewLevelFullyLoaded -= this.RecoverComapnionSelf;
            return base.Drop(player);
        }
    }
}
