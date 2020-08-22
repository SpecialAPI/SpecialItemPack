using System;
using System.Collections;
using System.Collections.Generic;
using Dungeonator;
using UnityEngine;

namespace SpecialItemPack
{
    class AdvancedCustomShrineController : DungeonPlaceableBehaviour, IPlayerInteractable, IPlaceConfigurable
    {

        public virtual void ConfigureOnPlacement(RoomHandler room)
        {
            this.m_parentRoom = room;
            room.OptionalDoorTopDecorable = (ResourceCache.Acquire("Global Prefabs/Shrine_Lantern") as GameObject);
            if (!room.IsOnCriticalPath && room.connectedRooms.Count == 1)
            {
                room.ShouldAttemptProceduralLock = true;
                room.AttemptProceduralLockChance = Mathf.Max(room.AttemptProceduralLockChance, UnityEngine.Random.Range(0.3f, 0.5f));
            }
            this.RegisterMinimapIcon();
        }

        public virtual void Start()
        {
            if (base.specRigidbody)
            {
                base.specRigidbody.PreventPiercing = true;
            }
        }

        public void RegisterMinimapIcon()
        {
            this.m_instanceMinimapIcon = Minimap.Instance.RegisterRoomIcon(this.m_parentRoom, (GameObject)BraveResources.Load("Global Prefabs/Minimap_Shrine_Icon", ".prefab"), false);
        }

        public void GetRidOfMinimapIcon()
        {
            if (this.m_instanceMinimapIcon != null)
            {
                Minimap.Instance.DeregisterRoomIcon(this.m_parentRoom, this.m_instanceMinimapIcon);
                this.m_instanceMinimapIcon = null;
            }
        }

        protected virtual bool CheckCosts(PlayerController interactor)
        {
            return true;
        }

        protected virtual bool CheckAndApplyCosts(PlayerController interactor)
        {
            bool flag = this.CheckCosts(interactor);
            if (flag)
            {
                this.ApplyCosts(interactor);
                return true;
            }
            return false;
        }

        protected virtual void ApplyCosts(PlayerController interactor)
        {
        }

        private void ResetForReuse()
        {
            this.m_useCount--;
        }

        protected virtual void DoShrineEffect(PlayerController player)
        {
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (base.sprite == null)
            {
                return 100f;
            }
            Vector3 v = BraveMathCollege.ClosestPointOnRectangle(point, base.specRigidbody.UnitBottomLeft, base.specRigidbody.UnitDimensions);
            return Vector2.Distance(point, v) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return -1f;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white);
        }

        public void OnExitRange(PlayerController interactor)
        {
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
        }

        private IEnumerator HandleShrineConversation(PlayerController interactor)
        {
            string targetDisplayKey = this.StoneTableKey;
            TextBoxManager.ShowStoneTablet(this.talkPoint.position, this.talkPoint, -1f, StringTableManager.GetLongString(targetDisplayKey), true, false);
            int selectedResponse = -1;
            interactor.SetInputOverride("shrineConversation");
            yield return null;
            bool canUse = this.CheckCosts(interactor);
            if (canUse)
            {
                GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, StringTableManager.GetString(AcceptKey), StringTableManager.GetString(DeclineKey));
            }
            else
            {
                GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, StringTableManager.GetString(DeclineKey), string.Empty);
            }
            while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
            {
                yield return null;
            }
            interactor.ClearInputOverride("shrineConversation");
            TextBoxManager.ClearTextBox(this.talkPoint);
            if (canUse && selectedResponse == 0)
            {
                this.DoShrineEffect(interactor);
                this.m_totalUseCount++;
                if (this.CanBeReused)
                {
                    this.ResetForReuse();
                }
            }
            else
            {
                this.ResetForReuse();
            }
            yield break;
        }

        private IEnumerator HandleSpentText(PlayerController interactor)
        {
            TextBoxManager.ShowStoneTablet(this.talkPoint.position, this.talkPoint, -1f, StringTableManager.GetLongString(SpentKey), true, false);
            int selectedResponse = -1;
            interactor.SetInputOverride("shrineConversation");
            GameUIRoot.Instance.DisplayPlayerConversationOptions(interactor, null, StringTableManager.GetString(DeclineKey), string.Empty);
            while (!GameUIRoot.Instance.GetPlayerConversationResponse(out selectedResponse))
            {
                yield return null;
            }
            interactor.ClearInputOverride("shrineConversation");
            TextBoxManager.ClearTextBox(this.talkPoint);
            yield break;
        }

        public void Interact(PlayerController interactor)
        {
            if (TextBoxManager.HasTextBox(this.talkPoint))
            {
                return;
            }
            if (this.m_useCount > 0 || AlwaysSpent)
            {
                if (!string.IsNullOrEmpty(this.SpentKey))
                {
                    base.StartCoroutine(this.HandleSpentText(interactor));
                }
                return;
            }
            this.m_useCount++;
            base.StartCoroutine(this.HandleShrineConversation(interactor));
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;
            return string.Empty;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private RoomHandler m_parentRoom;
        private GameObject m_instanceMinimapIcon;
        private int m_useCount;
        public Transform talkPoint;
        private int m_totalUseCount;
        public bool CanBeReused;
        public bool AlwaysSpent;
        public string DeclineKey;
        public string AcceptKey;
        public string StoneTableKey;
        public string SpentKey;
    }
}
