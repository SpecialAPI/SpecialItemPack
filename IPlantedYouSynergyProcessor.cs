using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialItemPack
{
    class IPlantedYouSynergyProcessor : AdvancedGunBehaviour
    {
        public override void PostProcessProjectile(Projectile projectile)
        {
            base.PostProcessProjectile(projectile);
            projectile.OnHitEnemy += this.RechargeFaster;
        }

        private void RechargeFaster(Projectile proj, SpeculativeRigidbody body, bool fatal)
        {
            if(proj.Owner is PlayerController)
            {
                PlayerController player = proj.Owner as PlayerController;
                if(player.activeItems != null)
                {
                    foreach(PlayerItem active in player.activeItems)
                    {
                        if(active is MahogunySapling)
                        {
                            active.CurrentDamageCooldown = Mathf.Max(0, active.CurrentDamageCooldown - 5f);
                        }
                    }
                }
            }
        }
    }
}
