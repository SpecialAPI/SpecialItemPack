using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;

namespace SpecialItemPack
{
    class PastkillersPlan : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Pastkiller's Plan";
            string resourceName = "SpecialItemPack/Resources/PastkillersPlan";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<PastkillersPlan>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "10 Step Pastkill";
            string longDesc = "Grants benefits when the owner collects components of the Bullet or the Bullet itself.\n\nAll the time you spent thinking of this plan cannot be in vain. You gotta collect it, no matter what!";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.C;
            Game.Items.Rename("spapi:pastkiller's_plan", "spapi:pastkillers_plan");
            item.PlaceItemInAmmonomiconAfterItemById(350);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.PostProcessProjectile += this.OnPostProcessProjectile;
        }

        private void OnPostProcessProjectile(Projectile proj, float f)
        {
            if (this.m_owner.HasPickupID(351))
            {
                ExplosiveModifier mod = proj.gameObject.AddComponent<ExplosiveModifier>();
                mod.explosionData = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData;
            }
            if (this.m_owner.HasPickupID(348))
            {
                proj.baseData.speed *= 2f;
                proj.baseData.damage *= 1.5f;
            }
            if (this.m_owner.HasPickupID(349))
            {
                proj.baseData.speed /= 2f;
                proj.baseData.damage *= 2f;
                proj.baseData.force *= 1.5f;
            }
            if (this.m_owner.HasPickupID(350))
            {
                PierceProjModifier pierceMod = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
                pierceMod.penetratesBreakables = true;
                pierceMod.preventPenetrationOfActors = false;
                pierceMod.penetration = 999;
            }
            if (this.m_owner.HasPickupID(303))
            {
                proj.BossDamageMultiplier *= 2f;
                proj.ignoreDamageCaps = true;
            }
            if (this.m_owner.HasPickupID(491) || this.m_owner.PlayerHasCompletionGun())
            {
                if (UnityEngine.Random.value <= 0.05f)
                {
                    Projectile proj2 = ((Gun)PickupObjectDatabase.GetById(16)).DefaultModule.projectiles[UnityEngine.Random.Range(0, ((Gun)PickupObjectDatabase.GetById(476)).DefaultModule.projectiles.Count)];
                    BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.m_owner.PlayerIDX);
                    bool flag2 = instanceForPlayer == null;
                    float z = 0;
                    bool notDo = false;
                    if (!flag2)
                    {
                        bool flag3 = instanceForPlayer.IsKeyboardAndMouse(false);
                        Vector2 a = Vector2.zero;
                        if (flag3)
                        {
                            a = this.m_owner.unadjustedAimPoint.XY() - base.sprite.WorldCenter;
                        }
                        else
                        {
                            bool flag4 = instanceForPlayer.ActiveActions == null;
                            if (flag4)
                            {
                                notDo = true;
                            }
                            else
                            {
                                a = instanceForPlayer.ActiveActions.Aim.Vector;
                            }
                        }
                        if (!notDo)
                        {
                            a.Normalize();
                            z = BraveMathCollege.Atan2Degrees(a);
                        }
                    }
                    if (!notDo)
                    {
                        GameObject obj = SpawnManager.SpawnProjectile(proj2.gameObject, this.m_owner.sprite.WorldCenter, Quaternion.Euler(0, 0, z));
                        Projectile component = obj.GetComponent<Projectile>();
                        if (component != null)
                        {
                            component.Owner = this.m_owner;
                            component.Shooter = this.m_owner.specRigidbody;
                        }
                    }
                }
            }
            if (this.m_owner.HasPickupID(492) || this.m_owner.PlayerHasCompletionGun())
            {
                if (UnityEngine.Random.value <= 0.1f)
                {
                    proj.AdjustPlayerProjectileTint(Color.red, 0);
                    proj.OnHitEnemy += delegate (Projectile proj3, SpeculativeRigidbody enemyRigidbody, bool fatal)
                    {
                        if(enemyRigidbody.aiActor != null)
                        {
                            AIActor aiactor = enemyRigidbody.aiActor;
                            AIActorDebuffEffect debuffEffect = null;
                            foreach (AttackBehaviorBase attackBehaviour in EnemyDatabase.GetOrLoadByGuid((PickupObjectDatabase.GetById(492) as CompanionItem).CompanionGuid).behaviorSpeculator.AttackBehaviors)
                            {
                                if (attackBehaviour is WolfCompanionAttackBehavior)
                                {
                                    debuffEffect = (attackBehaviour as WolfCompanionAttackBehavior).EnemyDebuff;
                                }
                            }
                            if (debuffEffect != null)
                            {
                                aiactor.ApplyEffect(debuffEffect, 1, null);
                            }
                        }
                    };
                }
            }
            if (this.m_owner.HasPickupID(493) || this.m_owner.PlayerHasCompletionGun())
            {
                if (UnityEngine.Random.value <= 0.1f)
                {
                    Projectile proj2 = ((Gun)PickupObjectDatabase.GetById(476)).DefaultModule.projectiles[UnityEngine.Random.Range(0, ((Gun)PickupObjectDatabase.GetById(476)).DefaultModule.projectiles.Count)];
                    BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.m_owner.PlayerIDX);
                    bool flag2 = instanceForPlayer == null;
                    float z = 0;
                    bool notDo = false;
                    if (!flag2)
                    {
                        bool flag3 = instanceForPlayer.IsKeyboardAndMouse(false);
                        Vector2 a = Vector2.zero;
                        if (flag3)
                        {
                            a = this.m_owner.unadjustedAimPoint.XY() - base.sprite.WorldCenter;
                        }
                        else
                        {
                            bool flag4 = instanceForPlayer.ActiveActions == null;
                            if (flag4)
                            {
                                notDo = true;
                            }
                            else
                            {
                                a = instanceForPlayer.ActiveActions.Aim.Vector;
                            }
                        }
                        if (!notDo)
                        {
                            a.Normalize();
                            z = BraveMathCollege.Atan2Degrees(a);
                        }
                    }
                    if (!notDo)
                    {
                        GameObject obj = SpawnManager.SpawnProjectile(proj2.gameObject, this.m_owner.sprite.WorldCenter, Quaternion.Euler(0, 0, z));
                        Projectile component = obj.GetComponent<Projectile>();
                        if (component != null)
                        {
                            component.Owner = this.m_owner;
                            component.Shooter = this.m_owner.specRigidbody;
                        }
                    }
                }
            }
            if (this.m_owner.HasPickupID(494) || this.m_owner.PlayerHasCompletionGun())
            {
                if (UnityEngine.Random.value <= 0.05f)
                {
                    proj.AdjustPlayerProjectileTint(Color.cyan, 0, 0);
                    proj.BossDamageMultiplier *= 2;
                    proj.ignoreDamageCaps = true;
                }
            }
            if (this.m_owner.HasPickupID(573) || this.m_owner.PlayerHasCompletionGun())
            {
                if (UnityEngine.Random.value <= 0.25f)
                {
                    proj.AdjustPlayerProjectileTint(Color.white, 0, 0);
                    proj.OnHitEnemy += delegate (Projectile proj2, SpeculativeRigidbody enemyRigidbody, bool fatal)
                    {
                        if (enemyRigidbody.aiActor != null && enemyRigidbody.aiActor.behaviorSpeculator != null) { enemyRigidbody.aiActor.behaviorSpeculator.Stun(2f, true); }
                    };
                }
            }
            if (this.m_owner.HasPickupID(572) || this.m_owner.PlayerHasCompletionGun())
            {
                if (this.m_owner.healthHaver != null && this.m_owner.healthHaver.GetCurrentHealthPercentage() >= 1f)
                {
                    proj.baseData.damage *= 1.5f;
                    proj.AdjustPlayerProjectileTint(Color.magenta, 0, 0);
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.OnPostProcessProjectile;
            return base.Drop(player);
        }
    }
}
