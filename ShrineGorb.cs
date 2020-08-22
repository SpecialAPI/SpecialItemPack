using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class ShrineGorb : AdvancedCustomShrineController
    {
        public ShrineGorb()
        {
            this.AcceptKey = "#SHRINE_GORB_ACCEPT";
            this.DeclineKey = "#SHRINE_GORB_DECLINE";
            this.SpentKey = "#SHRINE_SREAPER_SPENT";
            this.StoneTableKey = "#SHRINE_GORB_TABLE";
        }

        public override void Start()
        {
            base.Start();
            this.statue = UnityEngine.Object.Instantiate(SpecialItemModule.gorbShrine.GetComponent<ShrineGorb>().statue, GameManager.Instance.PrimaryPlayer.sprite.WorldTopCenter, Quaternion.identity, base.transform);
            this.statue.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(base.sprite.WorldTopCenter + new Vector2(-4f / 16f, -8f / 16f), tk2dBaseSprite.Anchor.LowerCenter);
        }

        protected override bool CheckCosts(PlayerController interactor)
        {
            return interactor.healthHaver != null && interactor.healthHaver.GetMaxHealth() > 1f;
        }

        protected override void DoShrineEffect(PlayerController player)
        {
            player.ownerlessStatModifiers.Add(Toolbox.SetupStatModifier(PlayerStats.StatType.Health, -1f, StatModifier.ModifyMethod.ADDITIVE, false));
            GameUIRoot.Instance.notificationController.DoCustomNotification(StringTableManager.GetString("#SHRINE_GORB_HEADER"), StringTableManager.GetString("#SHRINE_GORB_TEXT"), SpriteBuilder.itemCollection,
                spriteId, UINotificationController.NotificationColor.SILVER, false, false);
            player.gameObject.AddComponent<AscendAscendAscendWithGorb>();
            AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", base.gameObject);
        }

        public GameObject statue;
        public static int spriteId;
    }

    public class AscendAscendAscendWithGorb : BraveBehaviour
    {
        public void Start()
        {
            if(base.healthHaver != null)
            {
                base.healthHaver.OnDamaged += this.LaunchSpikesOnDamage;
            }
        }

        private void LaunchSpikesOnDamage(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            base.StartCoroutine(this.LaunchSpikes());
        }

        private void Update()
        {
            if (!this.m_isBursting)
            {
                if (this.cooldown <= 0f)
                {
                    base.StartCoroutine(this.LaunchSpikes());
                }
                else
                {
                    this.cooldown -= BraveTime.DeltaTime;
                }
            }
        }

        private IEnumerator LaunchSpikes()
        {
            this.m_isBursting = true;
            int num = 1;
            if(base.healthHaver != null)
            {
                if(base.healthHaver.GetCurrentHealthPercentage() <= 0.7f)
                {
                    num++;
                }
                if(base.healthHaver.GetCurrentHealthPercentage() <= 0.4f)
                {
                    num++;
                }
            }
            float burstCooldown;
            for(int i = 0; i < num; i++)
            {
                AkSoundEngine.PostEvent("Play_WPN_spellactionrevolver_shot_01", base.gameObject);
                float angleOffset = (i == 1 ? 22.5f : 0);
                for(int j = 0; j < 8; j++)
                {
                    GameObject obj = SpawnManager.SpawnProjectile(Toolbox.GetGunById(124).DefaultModule.projectiles[0].gameObject, base.sprite.WorldCenter, Quaternion.Euler(0, 0, ((float)j * 45) + angleOffset));
                    Projectile proj = obj.GetComponent<Projectile>();
                    if(proj != null)
                    {
                        proj.Owner = base.gameActor;
                        proj.Shooter = base.specRigidbody;
                        proj.DefaultTintColor = Color.yellow;
                        proj.HasDefaultTint = true;
                        proj.baseData.AccelerationCurve = Toolbox.GetGunById(760).DefaultModule.projectiles[0].baseData.AccelerationCurve;
                        proj.enemyImpactEventName = "";
                        proj.objectImpactEventName = "";
                        proj.onDestroyEventName = "";
                        proj.baseData.UsesCustomAccelerationCurve = true;
                        proj.baseData.damage = 6f;
                        PierceProjModifier pierceMod = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
                        pierceMod.penetratesBreakables = true;
                        pierceMod.penetration = 999;
                    }
                }
                burstCooldown = 0.5f;
                while(burstCooldown > 0f)
                {
                    burstCooldown -= BraveTime.DeltaTime;
                    yield return null;
                }
            }
            this.cooldown = 3.5f;
            this.m_isBursting = false;
            yield break;
        }

        private float cooldown = 5f;
        private bool m_isBursting = false;
    }
}
