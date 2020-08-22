using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;

namespace SpecialItemPack
{
    class GungeonCompass : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Gungeon Compass";
            string resourceName = "SpecialItemPack/Resources/GungeonCompass";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<GungeonCompass>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Can Find Anything!";
            string longDesc = "Points to a room with the current search category. Using it will change the search category.\n\nSense of Direction modified with the knowledge of making compasses. In Gungeon, there is no magnet field. Instead the arrow of the " +
                "compass is made out of special lead, which will point towards a teleporter. A tricky device in the compass will tell the arrow what to search and the arrow will quickly change direction. The teleporter system, through acts kinda weird." +
                " The arrow will not point towards the nearest room, instead it will point to the first room using a specific order.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 0.5f);
            item.consumable = false;
            item.quality = ItemQuality.C;
            GungeonCompass.arrowObj = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GungeonCompassArrow", new GameObject("CompassArrow"), true);
            UnityEngine.Object.DontDestroyOnLoad(GungeonCompass.arrowObj);
            FakePrefab.MarkAsFakePrefab(GungeonCompass.arrowObj);
            GungeonCompass.arrowObj.SetActive(false);
            GungeonCompass.arrowObj.GetComponent<tk2dBaseSprite>().GetCurrentSpriteDef().ConstructOffsetsFromAnchor(tk2dBaseSprite.Anchor.MiddleCenter, GungeonCompass.arrowObj.GetComponent<tk2dBaseSprite>().GetCurrentSpriteDef().position3);
            item.PlaceItemInAmmonomiconAfterItemById(209);
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnNewFloorLoaded += this.MaybeRevealLevel;
        }

        private void MaybeRevealLevel(PlayerController user)
        {
            if(this.LastOwner != null)
            {
                if(this.LastOwner.PlayerHasActiveSynergy("#2_COMPASSES_ONE_EXIT"))
                {
                    Minimap.Instance.RevealAllRooms(true);
                }
            }
        }

        protected override void OnPreDrop(PlayerController user)
        {
            base.OnPreDrop(user);
            user.OnNewFloorLoaded -= this.MaybeRevealLevel;
            if (this.extantObj != null)
            {
                UnityEngine.Object.Destroy(this.extantObj);
                this.extantObj = null;
            }
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            this.currentIndex += 1;
            if(this.currentIndex > GungeonCompass.availableSearchObjects.Count)
            {
                this.currentIndex = 0;
            }
            this.currentSearchObject = GungeonCompass.availableSearchObjects[this.currentIndex];
            this.Notify("Currently Searching For:", this.currentSearchObject.ToString());
        }

        private void Notify(string header, string text)
        {
            tk2dBaseSprite notificationObjectSprite = this.sprite;
            GameUIRoot.Instance.notificationController.DoCustomNotification(header, text, notificationObjectSprite.Collection, notificationObjectSprite.spriteId, UINotificationController.NotificationColor.PURPLE, false, false);
        }

        private void LateUpdate()
        {
            if(this.m_pickedUp && this.LastOwner != null)
            {
                if(this.extantObj == null)
                {
                    this.extantObj = UnityEngine.Object.Instantiate<GameObject>(GungeonCompass.arrowObj, this.LastOwner.transform.position, Quaternion.identity);
                    LootEngine.DoDefaultItemPoof(this.LastOwner.sprite.WorldTopCenter + new Vector2(0f, 1f));
                }
                this.extantObj.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(this.LastOwner.sprite.WorldTopCenter + new Vector2(0f, 1f), tk2dBaseSprite.Anchor.MiddleCenter);
                Vector2 coords = new Vector2();
                bool found = false;
                foreach (RoomHandler room in GameManager.Instance.Dungeon.data.rooms)
                {
                    //ETGModConsole.Log("still searching!");
                    if ((room.area.PrototypeRoomCategory == this.currentSearchObject))
                    {
                        coords = room.GetCenterCell().ToVector2();
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    Vector2 offset = this.extantObj.GetComponent<tk2dBaseSprite>().WorldCenter - coords;
                    this.extantObj.transform.Rotate(this.LastOwner.sprite.WorldTopCenter + Vector2.up, BraveMathCollege.Atan2Degrees(offset), Space.World);
                    this.extantObj.transform.rotation = Quaternion.Euler(0, 0, BraveMathCollege.Atan2Degrees(offset));
                }
                else
                {
                    this.extantObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else
            {
                if (this.extantObj != null)
                {
                    UnityEngine.Object.Destroy(this.extantObj);
                    this.extantObj = null;
                }
            }
        }

        private GameObject extantObj;
        public static GameObject arrowObj;
        private PrototypeDungeonRoom.RoomCategory currentSearchObject = PrototypeDungeonRoom.RoomCategory.EXIT;
        private static readonly List<PrototypeDungeonRoom.RoomCategory> availableSearchObjects = new List<PrototypeDungeonRoom.RoomCategory>
        {
            PrototypeDungeonRoom.RoomCategory.EXIT,
            PrototypeDungeonRoom.RoomCategory.BOSS,
            PrototypeDungeonRoom.RoomCategory.ENTRANCE,
            PrototypeDungeonRoom.RoomCategory.HUB,
            PrototypeDungeonRoom.RoomCategory.REWARD,
            PrototypeDungeonRoom.RoomCategory.SECRET,
            PrototypeDungeonRoom.RoomCategory.SPECIAL,
        };
        private int currentIndex = 0;
    }
}
