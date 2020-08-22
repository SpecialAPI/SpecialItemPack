using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Dungeonator;
using SpecialItemPack;

namespace GungeonAPI
{
    public class ShrineFactory
    {
        public string
            name,
            modID,
            spritePath,
            text, acceptText, declineText;
        public Action<PlayerController, GameObject>
            OnAccept,
            OnDecline;
        public Func<PlayerController, GameObject, bool> CanUse;
        public Vector3 talkPointOffset;
        public Vector3 offset = new Vector3(43.8f, 42.4f, 42.9f);
        public IntVector2 colliderOffset, colliderSize;
        public bool
            isToggle,
            usesCustomColliderOffsetAndSize;
        public Type interactableComponent = null;
        public bool isBreachShrine = false;
        public PrototypeDungeonRoom room;
        public Dictionary<string, int> roomStyles;

        public static Dictionary<string, GameObject> builtShrines = new Dictionary<string, GameObject>();
        private static bool m_initialized, m_builtShrines;
        public static Dictionary<string, GameObject> breachShrines = new Dictionary<string, GameObject>();

        public static void Init()
        {
            if (m_initialized) return;
            DungeonHooks.OnFoyerAwake += PlaceBreachShrines;
            DungeonHooks.OnPreDungeonGeneration += (generator, dungeon, flow, dungeonSeed) =>
            {
                if (flow.name != "Foyer Flow" && !GameManager.IsReturningToFoyerWithPlayer)
                {
                    foreach (var cshrine in GameObject.FindObjectsOfType<BreachShrineController>())
                    {
                        if (!cshrine.instantiated)
                            UnityEngine.Object.Destroy(cshrine.gameObject);
                    }
                    m_builtShrines = false;
                }
            };
            m_initialized = true;
        }


        ///maybe add some value proofing here (name != null, collider != IntVector2.Zero)
        public GameObject Build()
        {
            try
            {
                //Get texture and create sprite
                Texture2D tex = ResourceExtractor.GetTextureFromResource(spritePath);
                var shrine = SpecialItemPack.ItemAPI.SpriteBuilder.SpriteFromResource(spritePath, null, false);

                //Add (hopefully) unique ID to shrine for tracking
                string ID = $"{modID}:{name}".ToLower().Replace(" ", "_");
                shrine.name = ID;

                //Position sprite 
                var sprite = shrine.GetComponent<tk2dSprite>();
                sprite.IsPerpendicular = true;
                sprite.PlaceAtPositionByAnchor(offset, tk2dBaseSprite.Anchor.LowerCenter);

                //Add speech bubble origin
                var talkPoint = new GameObject("talkpoint").transform;
                talkPoint.position = shrine.transform.position + talkPointOffset;
                talkPoint.SetParent(shrine.transform);

                //Set up collider
                if (!usesCustomColliderOffsetAndSize)
                {
                    IntVector2 spriteDimensions = new IntVector2(tex.width, tex.height);
                    colliderOffset = new IntVector2(0, 0);
                    colliderSize = new IntVector2(spriteDimensions.x, spriteDimensions.y / 2);
                }
                var body = ItemAPI.SpriteBuilder.SetUpSpeculativeRigidbody(sprite, colliderOffset, colliderSize);

                var data = shrine.AddComponent<CustomShrineController>();
                data.ID = ID;
                data.roomStyles = roomStyles;
                data.isBreachShrine = true;
                data.offset = offset;
                data.pixelColliders = body.specRigidbody.PixelColliders;
                data.factory = this;
                data.OnAccept = OnAccept;
                data.OnDecline = OnDecline;
                data.CanUse = CanUse;

                IPlayerInteractable interactable;
                //Register as interactable
                if (interactableComponent != null)
                    interactable = shrine.AddComponent(interactableComponent) as IPlayerInteractable;
                else
                {
                    var simpInt = shrine.AddComponent<SimpleInteractable>();
                    simpInt.isToggle = this.isToggle;
                    simpInt.OnAccept = this.OnAccept;
                    simpInt.OnDecline = this.OnDecline;
                    simpInt.CanUse = CanUse;
                    simpInt.text = this.text;
                    simpInt.acceptText = this.acceptText;
                    simpInt.declineText = this.declineText;
                    simpInt.talkPoint = talkPoint;
                    interactable = simpInt as IPlayerInteractable;
                }


                var prefab = FakePrefab.Clone(shrine);
                prefab.GetComponent<CustomShrineController>().Copy(data);
                prefab.name = ID;
                if (isBreachShrine)
                {
                    if (!RoomHandler.unassignedInteractableObjects.Contains(interactable))
                        RoomHandler.unassignedInteractableObjects.Add(interactable);
                }
                else
                {
                    if (!room)
                        room = RoomFactory.CreateEmptyRoom();
                    RegisterShrineRoom(prefab, room, ID, offset);
                }


                builtShrines.Add(ID, prefab);
                Tools.Print("Added shrine: " + ID);
                return shrine;
            }
            catch (Exception e)
            {
                Tools.PrintException(e);
                return null;
            }
        }

        public static void RegisterShrineRoom(GameObject shrine, PrototypeDungeonRoom protoroom, string ID, Vector2 offset)
        {

            protoroom.category = PrototypeDungeonRoom.RoomCategory.NORMAL;

            DungeonPrerequisite[] emptyReqs = new DungeonPrerequisite[0];
            Vector2 position = new Vector2(protoroom.Width / 2 + offset.x, protoroom.Height / 2 + offset.y);
            protoroom.placedObjectPositions.Add(position);

            var placeableContents = ScriptableObject.CreateInstance<DungeonPlaceable>();
            placeableContents.width = 2;
            placeableContents.height = 2;
            placeableContents.respectsEncounterableDifferentiator = true;
            placeableContents.variantTiers = new List<DungeonPlaceableVariant>()
            {
                new DungeonPlaceableVariant()
                {
                    percentChance = 1,
                    nonDatabasePlaceable = shrine,
                    prerequisites = emptyReqs,
                    materialRequirements= new DungeonPlaceableRoomMaterialRequirement[0]
                }
            };

            protoroom.placedObjects.Add(new PrototypePlacedObjectData()
            {
                contentsBasePosition = position,
                fieldData = new List<PrototypePlacedObjectFieldData>(),
                instancePrerequisites = emptyReqs,
                linkedTriggerAreaIDs = new List<int>(),
                placeableContents = placeableContents
            });

            var data = new RoomFactory.RoomData()
            {
                room = protoroom,
                isSpecialRoom = true,
                category = "SPECIAL",
                specialSubCategory = "UNSPECIFIED_SPECIAL"
            };
            RoomFactory.rooms.Add(ID, data);
            DungeonHandler.Register(data);
        }

        private static void PlaceBreachShrines()
        {
            if (m_builtShrines) return;
            Tools.Print("Placing breach shrines: ");
            foreach (var prefab in breachShrines.Values)
            {
                try
                {
                    Tools.Print($"    {prefab.name}");
                    var shrine = UnityEngine.Object.Instantiate(prefab, prefab.GetComponent<BreachShrineController>().offset, Quaternion.identity).GetComponent<BreachShrineController>();
                    shrine.instantiated = true;
                    var interactable = shrine.GetComponent<IPlayerInteractable>();
                    if (!RoomHandler.unassignedInteractableObjects.Contains(interactable))
                        RoomHandler.unassignedInteractableObjects.Add(interactable);
                }
                catch (Exception e)
                {
                    Tools.PrintException(e);
                }
            }
            m_builtShrines = true;
        }

        public class CustomShrineController : DungeonPlaceableBehaviour
        {
            public string ID;
            public bool isBreachShrine;
            public Vector3 offset;
            public List<PixelCollider> pixelColliders;
            public Dictionary<string, int> roomStyles;
            public ShrineFactory factory;
            public Action<PlayerController, GameObject>
                OnAccept,
                OnDecline;
            public Func<PlayerController, GameObject, bool> CanUse;
            private RoomHandler m_parentRoom;
            private GameObject m_instanceMinimapIcon;
            public int numUses = 0;

            void Start()
            {
                string id = this.name.Replace("(Clone)", "");

                if (ShrineFactory.builtShrines.ContainsKey(id))
                    Copy(ShrineFactory.builtShrines[id].GetComponent<CustomShrineController>());
                else
                    Tools.PrintError($"Was this shrine registered correctly?: {id}");

                this.GetComponent<SimpleInteractable>().OnAccept = OnAccept;
                this.GetComponent<SimpleInteractable>().OnDecline = OnDecline;
                this.GetComponent<SimpleInteractable>().CanUse = CanUse;
            }

            public void Copy(CustomShrineController other)
            {
                this.ID = other.ID;
                this.roomStyles = other.roomStyles;
                this.isBreachShrine = other.isBreachShrine;
                this.offset = other.offset;
                this.pixelColliders = other.pixelColliders;
                this.factory = other.factory;
                this.OnAccept = other.OnAccept;
                this.OnDecline = other.OnDecline;
                this.CanUse = other.CanUse;
            }

            public void ConfigureOnPlacement(RoomHandler room)
            {
                this.m_parentRoom = room;
                this.RegisterMinimapIcon();
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
        }
    }
}

