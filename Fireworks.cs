using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;

namespace SpecialItemPack
{
    class Fireworks : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Fireworks";
            string resourceName = "SpecialItemPack/Resources/Fireworks";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Fireworks>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Look At That!";
            string longDesc = "An unlimited supply of firework rockets. It needs some time to recharge, through.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 155);
            item.consumable = false;
            item.numberOfUses = 3;
            item.UsesNumberOfUsesBeforeCooldown = true;
            item.quality = ItemQuality.B;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(462);
            GameObject rocketObj = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/FireworkRocket");
            rocketObj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(rocketObj);
            UnityEngine.Object.DontDestroyOnLoad(rocketObj);
            Fireworks.fireworkObj = rocketObj;
        }

        protected override void DoEffect(PlayerController user)
        {
            GameObject obj = UnityEngine.Object.Instantiate(Fireworks.fireworkObj, user.sprite.WorldCenter, Quaternion.identity);
            AkSoundEngine.SetSwitch("WPN_Guns", Toolbox.GetGunById(39).gunSwitchGroup, base.gameObject);
            AkSoundEngine.PostEvent("Play_WPN_gun_shot_01", base.gameObject);
            obj.AddComponent<SpeedUpBehavior>().owner = user;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user);
        }

        public static GameObject fireworkObj;

        private class SpeedUpBehavior : MonoBehaviour
        {
            private void Start()
            {
                this.obj = base.gameObject;
                base.StartCoroutine(this.ExplosionCoroutine());
            }

            private IEnumerator ExplosionCoroutine()
            {
                yield return new WaitForSeconds(1.5f);
                if(this.owner != null && this.owner.CurrentRoom != null && this.owner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All) != null)
                {
                    foreach(AIActor aiactor in this.owner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All))
                    {
                        if(aiactor.behaviorSpeculator != null)
                        {
                            aiactor.behaviorSpeculator.Stun(2f);
                        }
                    }
                }
                GameManager.Instance.StartCoroutine(this.StealingCoroutine());
                AkSoundEngine.PostEvent("Play_WPN_grenade_blast_01", base.gameObject);
                Pixelator.Instance.FadeToColor(0.1f, new Color(1, 1, 1).WithAlpha(0.5f), true, 0.05f);
                UnityEngine.Object.Destroy(this.obj);
                yield break;
            }

            private IEnumerator StealingCoroutine()
            {
                if(this.owner != null)
                {
                    this.owner.SetCapableOfStealing(true, "Fireworks");
                }
                yield return new WaitForSeconds(2f);
                if (this.owner != null)
                {
                    this.owner.SetCapableOfStealing(false, "Fireworks");
                }
                yield break;
            }

            private void Update()
            {
                Vector3 pos = this.obj.transform.position;
                pos.y += this.currentSpeed;
                this.obj.transform.position = pos;
                this.currentSpeed *= 1.25f;
            }

            private GameObject obj;
            private float currentSpeed = 0.001f;
            public PlayerController owner;
        }
    }
}
