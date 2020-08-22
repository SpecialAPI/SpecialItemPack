using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;

namespace SpecialItemPack
{
    public class PurpleChamberPickup : PickupObject, IPlayerInteractable
    {
	    public bool pickedUp
	    {
		    get
		    {
			    return this.m_pickedUp;
		    }
	    }

	    private void Start()
	    {
		    if (this.minimapIcon != null && !this.m_pickedUp)
		    {
			    this.m_minimapIconRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(base.transform.position.IntXY(VectorConversions.Floor));
			    this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_minimapIconRoom, this.minimapIcon, false);
		    }
		    SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
	    }

	    private void GetRidOfMinimapIcon()
	    {
		    if (this.m_instanceMinimapIcon != null)
		    {
			    Minimap.Instance.DeregisterRoomIcon(this.m_minimapIconRoom, this.m_instanceMinimapIcon);
			    this.m_instanceMinimapIcon = null;
		    }
	    }

	    protected override void OnDestroy()
	    {
		    base.OnDestroy();
		    if (Minimap.HasInstance)
		    {
			    this.GetRidOfMinimapIcon();
		    }
	    }

	    public override void Pickup(PlayerController player)
	    {

	    }

	    public float GetDistanceToPoint(Vector2 point)
	    {
		    if (!base.sprite)
		    {
			    return 1000f;
		    }
		    Bounds bounds = base.sprite.GetBounds();
		    bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
		    float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
		    float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
		    return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 1.5f;
	    }

	    public float GetOverrideMaxDistance()
	    {
		    return -1f;
	    }

	    public void OnEnteredRange(PlayerController interactor)
	    {
		    if (!this)
		    {
			    return;
		    }
		    if (!interactor.CurrentRoom.IsRegistered(this) && !RoomHandler.unassignedInteractableObjects.Contains(this))
		    {
			    return;
		    }
		    SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
		    SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
		    base.sprite.UpdateZDepth();
	    }

	    public void OnExitRange(PlayerController interactor)
	    {
		    if (!this)
		    {
			    return;
		    }
		    SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
		    SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
		    base.sprite.UpdateZDepth();
	    }

	    public void Interact(PlayerController interactor)
	    {
		    if (!this)
		    {
			    return;
		    }
		    if (interactor.CurrentGun == null || interactor.CurrentGun.ammo == interactor.CurrentGun.AdjustedMaxAmmo || interactor.CurrentGun.InfiniteAmmo || interactor.CurrentGun.RequiresFundsToShoot)
		    {
			    if (interactor.CurrentGun != null)
			    {
				    GameUIRoot.Instance.InformNeedsReload(interactor, new Vector3(interactor.specRigidbody.UnitCenter.x - interactor.transform.position.x, 1.25f, 0f), 1f, "#RELOAD_FULL");
			    }
			    return;
		    }
		    if (RoomHandler.unassignedInteractableObjects.Contains(this))
		    {
			    RoomHandler.unassignedInteractableObjects.Remove(this);
		    }
		    SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
		    this.Pickup(interactor);
	    }

	    public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
	    {
		    shouldBeFlipped = false;
		    return string.Empty;
	    }

	    public AmmoPickup.AmmoPickupMode mode;
	    public GameObject pickupVFX;
	    public GameObject minimapIcon;
	    public float SpreadAmmoCurrentGunPercent;
	    public float SpreadAmmoOtherGunsPercent;
	    public bool AppliesCustomAmmunition;
	    public float CustomAmmunitionDamageModifier;
	    public float CustomAmmunitionSpeedModifier;
	    public float CustomAmmunitionRangeModifier;
	    private bool m_pickedUp;
	    private RoomHandler m_minimapIconRoom;
	    private GameObject m_instanceMinimapIcon;
    }
}
