using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;

namespace SpecialItemPack
{
    class MagicBeans : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Magic Beans";
            string resourceName = "SpecialItemPack/Resources/Magic_Beans";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<MagicBeans>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Magic!";
            string longDesc = "A transformative spell of incredible power.\n\nThe wizard Alben Smallbore theorized that the more power was put into a spell, the less could be known about its outcome. This spell is immensely powerful.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 500f);
            item.consumable = false;
            item.quality = ItemQuality.EXCLUDED;
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            GameManager.Instance.StartCoroutine(this.BecomeLarge(user, this));
        }

        private IEnumerator BecomeLarge(PlayerController user, MagicBeans parent)
        {
            if(parent != null)
            {
                parent.m_isCurrentlyActive = true;
            }
            float scaleToBecome = user.transform.localScale.x * 3f;
            StatModifier statmod = null;
            float scalemod = 1;
            StatModifier statmod2 = null;
            float dodgemod = 1;
            StatModifier statmod3 = null;
            float damagemod = 1;
            while (user.transform.localScale.x < scaleToBecome)
            {
                user.transform.localScale = new Vector3(user.transform.localScale.x + 0.05f, user.transform.localScale.y + 0.05f, user.transform.localScale.z);
                if (user.specRigidbody)
                {
                    user.specRigidbody.UpdateCollidersOnScale = true;
                    user.specRigidbody.RegenerateColliders = true;
                }
                if(statmod != null)
                {
                    user.ownerlessStatModifiers.Remove(statmod);
                }
                scalemod += 0.05f;
                statmod = Toolbox.SetupStatModifier(PlayerStats.StatType.PlayerBulletScale, scalemod, StatModifier.ModifyMethod.MULTIPLICATIVE, false);
                user.ownerlessStatModifiers.Add(statmod);
                if (statmod2 != null)
                {
                    user.ownerlessStatModifiers.Remove(statmod2);
                }
                dodgemod += 2.5f;
                statmod2 = Toolbox.SetupStatModifier(PlayerStats.StatType.DodgeRollDamage, dodgemod, StatModifier.ModifyMethod.MULTIPLICATIVE, false);
                user.ownerlessStatModifiers.Add(statmod2);
                if (statmod3 != null)
                {
                    user.ownerlessStatModifiers.Remove(statmod3);
                }
                damagemod += 0.025f;
                statmod3 = Toolbox.SetupStatModifier(PlayerStats.StatType.Damage, damagemod, StatModifier.ModifyMethod.MULTIPLICATIVE, false);
                user.ownerlessStatModifiers.Add(statmod3);
                user.stats.RecalculateStats(user, true, false);
                yield return null;
            }
            float elapsed = 0;
            while(elapsed < 15)
            {
                elapsed += BraveTime.DeltaTime;
                user.healthHaver.IsVulnerable = false;
                if (!user.IsInCombat)
                {
                    break;
                }
                yield return null;
            }
            scaleToBecome = user.transform.localScale.x / 3f;
            while (user.transform.localScale.x > scaleToBecome)
            {
                user.transform.localScale = new Vector3(user.transform.localScale.x - 0.05f, user.transform.localScale.y - 0.05f, user.transform.localScale.z);
                if (user.specRigidbody)
                {
                    user.specRigidbody.UpdateCollidersOnScale = true;
                    user.specRigidbody.RegenerateColliders = true;
                }
                if (statmod != null)
                {
                    user.ownerlessStatModifiers.Remove(statmod);
                }
                scalemod -= 0.05f;
                statmod = Toolbox.SetupStatModifier(PlayerStats.StatType.PlayerBulletScale, scalemod, StatModifier.ModifyMethod.MULTIPLICATIVE, false);
                user.ownerlessStatModifiers.Add(statmod);
                if (statmod2 != null)
                {
                    user.ownerlessStatModifiers.Remove(statmod2);
                }
                dodgemod -= 2.5f;
                statmod2 = Toolbox.SetupStatModifier(PlayerStats.StatType.DodgeRollDamage, dodgemod, StatModifier.ModifyMethod.MULTIPLICATIVE, false);
                user.ownerlessStatModifiers.Add(statmod2);
                if (statmod3 != null)
                {
                    user.ownerlessStatModifiers.Remove(statmod3);
                }
                damagemod -= 0.025f;
                statmod3 = Toolbox.SetupStatModifier(PlayerStats.StatType.Damage, damagemod, StatModifier.ModifyMethod.MULTIPLICATIVE, false);
                user.ownerlessStatModifiers.Add(statmod3);
                user.stats.RecalculateStats(user, true, false);
                yield return null;
            }
            if (statmod != null)
            {
                user.ownerlessStatModifiers.Remove(statmod);
            }
            if (statmod2 != null)
            {
                user.ownerlessStatModifiers.Remove(statmod2);
            }
            if (statmod3 != null)
            {
                user.ownerlessStatModifiers.Remove(statmod3);
            }
            user.healthHaver.IsVulnerable = true;
            if (parent != null)
            {
                parent.m_isCurrentlyActive = false;
            }
            yield break;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return user.IsInCombat && base.CanBeUsed(user);
        }
    }
}
