using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace SpecialItemPack.ItemAPI
{
    public static class SpriteBuilder
    {
        public static tk2dSpriteCollectionData itemCollection = PickupObjectDatabase.GetByEncounterName("singularity").sprite.Collection;
        public static tk2dSpriteCollectionData ammonomiconCollection = AmmonomiconController.ForceInstance.EncounterIconCollection;
        public static tk2dSprite baseSprite = PickupObjectDatabase.GetByEncounterName("singularity").GetComponent<tk2dSprite>();

        /// <summary>
        /// Returns an object with a tk2dSprite component with the 
        /// texture of a file in the sprites folder
        /// </summary>
        public static GameObject SpriteFromFile(string spriteName, GameObject obj = null, bool copyFromExisting = true)
        {
            string filename = spriteName.Replace(".png", "");

            var texture = ResourceExtractor.GetTextureFromFile(filename);
            if (texture == null) return null;

            return SpriteFromTexture(texture, spriteName, obj, copyFromExisting);
        }

        public static SpeculativeRigidbody SetUpSpeculativeRigidbody(this tk2dSprite sprite, IntVector2 offset, IntVector2 dimensions)
        {
            SpeculativeRigidbody orAddComponent = sprite.gameObject.GetOrAddComponent<SpeculativeRigidbody>();
            PixelCollider pixelCollider = new PixelCollider();
            pixelCollider.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual;
            pixelCollider.CollisionLayer = CollisionLayer.EnemyCollider;
            pixelCollider.ManualWidth = dimensions.x;
            pixelCollider.ManualHeight = dimensions.y;
            pixelCollider.ManualOffsetX = offset.x;
            pixelCollider.ManualOffsetY = offset.y;
            orAddComponent.PixelColliders = new List<PixelCollider>
            {
                pixelCollider
            };
            return orAddComponent;
        }

        /// <summary>
        /// Returns an object with a tk2dSprite component with the 
        /// texture of an embedded resource
        /// </summary>
        public static GameObject SpriteFromResource(string spriteName, GameObject obj = null, bool copyFromExisting = true)
        {
            string extension = !spriteName.EndsWith(".png") ? ".png" : "";
            string resourcePath = spriteName + extension;

            var texture = ResourceExtractor.GetTextureFromResource(resourcePath);
            if (texture == null) return null;

            return SpriteFromTexture(texture, resourcePath, obj, copyFromExisting);
        }

        /// <summary>
        /// Returns an object with a tk2dSprite component with the texture provided
        /// </summary>
        public static GameObject SpriteFromTexture(Texture2D texture, string spriteName, GameObject obj = null, bool copyFromExisting = true)
        {
            if (obj == null)
            {
                obj = new GameObject();
            }
            tk2dSprite sprite;
            if (copyFromExisting)
                sprite = obj.AddComponent<tk2dSprite>(baseSprite);
            else
                sprite = obj.AddComponent<tk2dSprite>();

            int id = AddSpriteToCollection(spriteName, itemCollection);
            sprite.SetSprite(itemCollection, id);
            sprite.SortingOrder = 0;

            obj.GetComponent<BraveBehaviour>().sprite = sprite;
            FakePrefab.MarkAsFakePrefab(obj);
            obj.SetActive(false);

            return obj;
        }

        /// <summary>
        /// Adds a sprite (from a resource) to a collection
        /// </summary>
        /// <returns>The spriteID of the defintion in the collection</returns>
        public static int AddSpriteToCollection(string resourcePath, tk2dSpriteCollectionData collection)
        {
            string extension = !resourcePath.EndsWith(".png") ? ".png" : "";
            resourcePath += extension;
            var texture = ResourceExtractor.GetTextureFromResource(resourcePath); //Get Texture

            var definition = ConstructDefinition(texture); //Generate definition
            definition.name = texture.name; //naming the definition is actually extremely important 

            return AddSpriteToCollection(definition, collection);
        }

        /// <summary>
        /// Adds a sprite from a definition to a collection
        /// </summary>
        /// <returns>The spriteID of the defintion in the collection</returns>
        public static int AddSpriteToCollection(tk2dSpriteDefinition spriteDefinition, tk2dSpriteCollectionData collection)
        {
            //Add definition to collection
            var defs = collection.spriteDefinitions;
            var newDefs = defs.Concat(new tk2dSpriteDefinition[] { spriteDefinition }).ToArray();
            collection.spriteDefinitions = newDefs;

            //Reset lookup dictionary
            FieldInfo f = typeof(tk2dSpriteCollectionData).GetField("spriteNameLookupDict", BindingFlags.Instance | BindingFlags.NonPublic);
            f.SetValue(collection, null);  //Set dictionary to null
            collection.InitDictionary(); //InitDictionary only runs if the dictionary is null
            return newDefs.Length - 1;
        }

        /// <summary>
        /// Adds a sprite definition to the Ammonomicon sprite collection
        /// </summary>
        /// <returns>The spriteID of the defintion in the ammonomicon collection</returns>
        public static int AddToAmmonomicon(tk2dSpriteDefinition spriteDefinition)
        {
            return AddSpriteToCollection(spriteDefinition, ammonomiconCollection);
        }

        /// <summary>
        /// Constructs a new tk2dSpriteDefinition with the given texture
        /// </summary>
        /// <returns>A new sprite definition with the given texture</returns>
        public static tk2dSpriteDefinition ConstructDefinition(Texture2D texture)
        {
            RuntimeAtlasSegment ras = ETGMod.Assets.Packer.Pack(texture); //pack your resources beforehand or the outlines will turn out weird

            Material material = new Material(ShaderCache.Acquire(PlayerController.DefaultShaderName));
            material.mainTexture = ras.texture;
            //material.mainTexture = texture;

            var width = texture.width;
            var height = texture.height;

            var x = 0f;
            var y = 0f;

            var w = width / 16f;
            var h = height / 16f;

            var def = new tk2dSpriteDefinition
            {
                normals = new Vector3[] {
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
            },
                tangents = new Vector4[] {
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            },
                texelSize = new Vector2(1 / 16f, 1 / 16f),
                extractRegion = false,
                regionX = 0,
                regionY = 0,
                regionW = 0,
                regionH = 0,
                flipped = tk2dSpriteDefinition.FlipMode.None,
                complexGeometry = false,
                physicsEngine = tk2dSpriteDefinition.PhysicsEngine.Physics3D,
                colliderType = tk2dSpriteDefinition.ColliderType.None,
                collisionLayer = CollisionLayer.HighObstacle,
                position0 = new Vector3(x, y, 0f),
                position1 = new Vector3(x + w, y, 0f),
                position2 = new Vector3(x, y + h, 0f),
                position3 = new Vector3(x + w, y + h, 0f),
                material = material,
                materialInst = material,
                materialId = 0,
                //uvs = ETGMod.Assets.GenerateUVs(texture, 0, 0, width, height), //uv machine broke
                uvs = ras.uvs,
                boundsDataCenter = new Vector3(w / 2f, h / 2f, 0f),
                boundsDataExtents = new Vector3(w, h, 0f),
                untrimmedBoundsDataCenter = new Vector3(w / 2f, h / 2f, 0f),
                untrimmedBoundsDataExtents = new Vector3(w, h, 0f),
            };

            def.name = texture.name;
            return def;
        }

        public static tk2dSpriteCollectionData ConstructCollection(string name)
        {
            var collection = new tk2dSpriteCollectionData();
            UnityEngine.Object.DontDestroyOnLoad(collection);

            collection.assetName = name;
            collection.allowMultipleAtlases = false;
            collection.buildKey = 0x0ade;
            collection.dataGuid = "what even is this for";
            collection.spriteCollectionGUID = name;
            collection.spriteCollectionName = name;
            collection.spriteDefinitions = new tk2dSpriteDefinition[0];
            /*
            var material_arr = new Material[Textures.Length];

            for (int i = 0; i < Textures.Length; i++)
            {
                material_arr[i] = new Material(DefaultSpriteShader);
                material_arr[i].mainTexture = Textures[i];
            }

            collection.textures = Textures;
            collection.textureInsts = Textures;

            collection.materials = material_arr;
            collection.materialInsts = material_arr;

            collection.needMaterialInstance = false;
            collection.spriteDefinitions = ConstructDefinitions(material_arr);
            */

            return collection;
        }


        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            PropertyInfo[] pinfos = type.GetProperties();
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
                else
                {
                }
            }
            FieldInfo[] finfos = type.GetFields();
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }

        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd) as T;
        }

        /*
        public static GameObject SpriteObjectFromTexture(Texture2D texture)
        {
            Rect region = new Rect(0, 0, texture.width, texture.height);

            var obj = tk2dSprite.CreateFromTexture(texture, tk2dSpriteCollectionSize.PixelsPerMeter(16), region, Vector2.zero);

            var collection = obj.GetComponent<tk2dSprite>().Collection;
            UnityEngine.Object.DontDestroyOnLoad(collection);

            var def = collection.spriteDefinitions[0];
            def.ReplaceTexture(texture);
            def.name = texture.name;

            Serializer.Serialize(def, texture.name + "_latedef");
            return obj;
        }
        */

        /*
       /// <summary>
       /// Adds a new sprite definition to the ammonomicon's collection
       /// </summary>
       /// <returns>The sprite ID of the newly added definition</returns>
       public static int AddSpriteToAmmonomicon(tk2dSpriteDefinition definition)
       {
           //Add sprite to definitions
           var iconCollection = AmmonomiconController.ForceInstance.EncounterIconCollection;
           var defs = iconCollection.spriteDefinitions;
           var newDefs = defs.Concat(new tk2dSpriteDefinition[] { definition }).ToArray();
           iconCollection.spriteDefinitions = newDefs;

           //Reset lookup dictionary
           FieldInfo f = typeof(tk2dSpriteCollectionData).GetField("spriteNameLookupDict", BindingFlags.Instance | BindingFlags.NonPublic);
           f.SetValue(iconCollection, null);  //Set dictionary to null
           iconCollection.InitDictionary(); //InitDictionary only runs if the dictionary is null

           return newDefs.Length - 1;
       }
       */
    }
}
