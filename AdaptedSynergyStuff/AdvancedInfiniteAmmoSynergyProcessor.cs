using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialItemPack.AdaptedSynergyStuff
{
    public class AdvancedInfiniteAmmoSynergyProcessor : MonoBehaviour
    {
        public AdvancedInfiniteAmmoSynergyProcessor()
        {
            this.PreventsReload = true;
            this.m_cachedReloadTime = -1f;
        }

        public void Awake()
        {
            this.m_gun = base.GetComponent<Gun>();
        }

        public void Update()
        {
            bool flag = this.m_gun && this.m_gun.OwnerHasSynergy(this.RequiredSynergy);
            if (flag && !this.m_processed)
            {
                this.m_gun.GainAmmo(this.m_gun.AdjustedMaxAmmo);
                this.m_gun.InfiniteAmmo = true;
                this.m_processed = true;
                if (this.PreventsReload)
                {
                    this.m_cachedReloadTime = this.m_gun.reloadTime;
                    this.m_gun.reloadTime = 0f;
                }
            }
            else if (!flag && this.m_processed)
            {
                this.m_gun.InfiniteAmmo = false;
                this.m_processed = false;
                if (this.PreventsReload)
                {
                    this.m_gun.reloadTime = this.m_cachedReloadTime;
                }
            }
        }

        public string RequiredSynergy;
        public bool PreventsReload;
        private bool m_processed;
        private Gun m_gun;
        private float m_cachedReloadTime;
    }
}
