using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using System.Collections;

namespace SpecialItemPack
{
    public class SpecialChamberGunProcessor : MonoBehaviour, ILevelLoadedListener
    {
        public SpecialChamberGunProcessor()
        {
            this.RefillsOnFloorChange = true;
        }

        private void Awake()
        {
            this.m_currentTileset = GlobalDungeonData.ValidTilesets.CASTLEGEON;
            this.m_gun = base.GetComponent<Gun>();
            Gun gun = this.m_gun;
            gun.OnReloadPressed = (Action<PlayerController, Gun, bool>)Delegate.Combine(gun.OnReloadPressed, new Action<PlayerController, Gun, bool>(this.HandleReloadPressed));
        }

        private GlobalDungeonData.ValidTilesets GetFloorTileset()
        {
            if (GameManager.Instance.IsLoadingLevel || !GameManager.Instance.Dungeon)
            {
                return GlobalDungeonData.ValidTilesets.CASTLEGEON;
            }
            return GameManager.Instance.Dungeon.tileIndices.tilesetId;
        }

        private bool IsValidTileset(GlobalDungeonData.ValidTilesets t)
        {
            if (t == this.GetFloorTileset())
            {
                return true;
            }
            PlayerController playerController = this.m_gun.CurrentOwner as PlayerController;
            if (playerController)
            {
                if (t == GlobalDungeonData.ValidTilesets.CASTLEGEON && playerController.HasPassiveItem(GlobalItemIds.MasteryToken_Castle))
                {
                    return true;
                }
                if (t == GlobalDungeonData.ValidTilesets.GUNGEON && playerController.HasPassiveItem(GlobalItemIds.MasteryToken_Gungeon))
                {
                    return true;
                }
                if (t == GlobalDungeonData.ValidTilesets.MINEGEON && playerController.HasPassiveItem(GlobalItemIds.MasteryToken_Mines))
                {
                    return true;
                }
                if (t == GlobalDungeonData.ValidTilesets.CATACOMBGEON && playerController.HasPassiveItem(GlobalItemIds.MasteryToken_Catacombs))
                {
                    return true;
                }
                if (t == GlobalDungeonData.ValidTilesets.FORGEGEON && playerController.HasPassiveItem(GlobalItemIds.MasteryToken_Forge))
                {
                    return true;
                }
                if(t == (GlobalDungeonData.ValidTilesets)CustomValidTilesets.CHAMBERGEON && playerController.HasPassiveItem(SpecialItemIds.MasterChamber))
                {
                    return true;
                }
                if(Toolbox.ModdedItemExists("Corrupted Master Round") && playerController.HasPassiveItem(Toolbox.GetModdedItemId("Corrupted Master Round")))
                {
                    return true;
                }
            }
            return false;
        }

        private void ChangeToTileset(GlobalDungeonData.ValidTilesets t)
        {
            if (t == GlobalDungeonData.ValidTilesets.CASTLEGEON)
            {
                this.ChangeForme(this.CastleGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.CASTLEGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.SEWERGEON)
            {
                this.ChangeForme(this.OublietteGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.SEWERGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.GUNGEON)
            {
                this.ChangeForme(this.GungeonGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.GUNGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.CATHEDRALGEON)
            {
                this.ChangeForme(this.AbbeyGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.CATHEDRALGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.MINEGEON)
            {
                this.ChangeForme(this.MinesGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.MINEGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.RATGEON)
            {
                this.ChangeForme(this.RatgeonGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.RATGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.CATACOMBGEON)
            {
                this.ChangeForme(this.HollowGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.CATACOMBGEON;
            }
            else if(t == GlobalDungeonData.ValidTilesets.OFFICEGEON)
            {
                this.ChangeForme(this.OfficeGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.OFFICEGEON;
            }
            else if(t == GlobalDungeonData.ValidTilesets.FORGEGEON)
            {
                this.ChangeForme(this.ForgeGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.FORGEGEON;
            }
            else if(t == GlobalDungeonData.ValidTilesets.HELLGEON)
            {
                this.ChangeForme(this.HellGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.HELLGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.JUNGLEGEON)
            {
                this.ChangeForme(this.JungleGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.JUNGLEGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.BELLYGEON)
            {
                this.ChangeForme(this.BellyGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.BELLYGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.PHOBOSGEON)
            {
                this.ChangeForme(this.PhobosGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.PHOBOSGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.WESTGEON)
            {
                this.ChangeForme(this.OldWestGunID);
                this.m_currentTileset = GlobalDungeonData.ValidTilesets.WESTGEON;
            }
            else if (t == (GlobalDungeonData.ValidTilesets)CustomValidTilesets.CHAMBERGEON)
            {
                this.ChangeForme(this.ChamberGunID);
                this.m_currentTileset = (GlobalDungeonData.ValidTilesets)CustomValidTilesets.CHAMBERGEON;
            }
            else
            {
                this.ChangeForme(this.CastleGunID);
                this.m_currentTileset = this.GetFloorTileset();
            }
        }

        private void ChangeForme(int targetID)
        {
            Gun targetGun = PickupObjectDatabase.GetById(targetID) as Gun;
            this.m_gun.TransformToTargetGun(targetGun);
        }

        private void Update()
        {
            if (Dungeon.IsGenerating || GameManager.Instance.IsLoadingLevel)
            {
                return;
            }
            if (this.m_gun && (!this.m_gun.CurrentOwner || !this.IsValidTileset(this.m_currentTileset)))
            {
                GlobalDungeonData.ValidTilesets validTilesets = this.GetFloorTileset();
                if (!this.m_gun.CurrentOwner)
                {
                    validTilesets = GlobalDungeonData.ValidTilesets.CASTLEGEON;
                }
                if (this.m_currentTileset != validTilesets)
                {
                    this.ChangeToTileset(validTilesets);
                }
            }
            this.JustActiveReloaded = false;
        }

        private GlobalDungeonData.ValidTilesets GetNextTileset(GlobalDungeonData.ValidTilesets t)
        {
            if (t == GlobalDungeonData.ValidTilesets.CASTLEGEON)
            {
                return GlobalDungeonData.ValidTilesets.SEWERGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.SEWERGEON)
            {
                return GlobalDungeonData.ValidTilesets.JUNGLEGEON;
            }
            else if(t == GlobalDungeonData.ValidTilesets.JUNGLEGEON)
            {
                return GlobalDungeonData.ValidTilesets.GUNGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.GUNGEON)
            {
                return GlobalDungeonData.ValidTilesets.CATHEDRALGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.CATHEDRALGEON)
            {
                return GlobalDungeonData.ValidTilesets.BELLYGEON;
            }
            else if(t == GlobalDungeonData.ValidTilesets.BELLYGEON)
            {
                return GlobalDungeonData.ValidTilesets.MINEGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.MINEGEON)
            {
                return GlobalDungeonData.ValidTilesets.RATGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.RATGEON)
            {
                return GlobalDungeonData.ValidTilesets.CATACOMBGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.CATACOMBGEON)
            {
                return GlobalDungeonData.ValidTilesets.WESTGEON;
            }
            else if(t == GlobalDungeonData.ValidTilesets.WESTGEON)
            {
                return GlobalDungeonData.ValidTilesets.OFFICEGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.OFFICEGEON)
            {
                return GlobalDungeonData.ValidTilesets.PHOBOSGEON;
            }
            else if(t == GlobalDungeonData.ValidTilesets.PHOBOSGEON)
            {
                return GlobalDungeonData.ValidTilesets.FORGEGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.FORGEGEON)
            {
                return GlobalDungeonData.ValidTilesets.HELLGEON;
            }
            else if (t == GlobalDungeonData.ValidTilesets.HELLGEON)
            {
                return (GlobalDungeonData.ValidTilesets)CustomValidTilesets.CHAMBERGEON;
            }
            else if (t == (GlobalDungeonData.ValidTilesets)CustomValidTilesets.CHAMBERGEON)
            {
                return GlobalDungeonData.ValidTilesets.CASTLEGEON;
            }
            else
            {
                return GlobalDungeonData.ValidTilesets.CASTLEGEON;
            }
        }

        private GlobalDungeonData.ValidTilesets GetNextValidTileset()
        {
            GlobalDungeonData.ValidTilesets nextTileset = this.GetNextTileset(this.m_currentTileset);
            while (!this.IsValidTileset(nextTileset))
            {
                nextTileset = this.GetNextTileset(nextTileset);
            }
            return nextTileset;
        }

        private void HandleReloadPressed(PlayerController ownerPlayer, Gun sourceGun, bool manual)
        {
            if (this.JustActiveReloaded)
            {
                return;
            }
            if (manual && !sourceGun.IsReloading)
            {
                GlobalDungeonData.ValidTilesets nextValidTileset = this.GetNextValidTileset();
                if (this.m_currentTileset != nextValidTileset)
                {
                    this.ChangeToTileset(nextValidTileset);
                }
            }
        }

        public void BraveOnLevelWasLoaded()
        {
            if (this.RefillsOnFloorChange && this.m_gun && this.m_gun.CurrentOwner)
            {
                this.m_gun.StartCoroutine(this.DelayedRegainAmmo());
            }
        }

        private IEnumerator DelayedRegainAmmo()
        {
            yield return null;
            while (Dungeon.IsGenerating)
            {
                yield return null;
            }
            if (this.RefillsOnFloorChange && this.m_gun && this.m_gun.CurrentOwner)
            {
                this.m_gun.GainAmmo(this.m_gun.AdjustedMaxAmmo);
            }
            yield break;
        }

        public int CastleGunID;
        public int GungeonGunID;
        public int MinesGunID;
        public int HollowGunID;
        public int ForgeGunID;
        public int HellGunID;
        public int OublietteGunID;
        public int AbbeyGunID;
        public int RatgeonGunID;
        public int OfficeGunID;
        public int JungleGunID;
        public int BellyGunID;
        public int PhobosGunID;
        public int OldWestGunID;
        public int ChamberGunID;
        public bool RefillsOnFloorChange;
        private GlobalDungeonData.ValidTilesets m_currentTileset;
        private Gun m_gun;
        public bool JustActiveReloaded;
    }
}
