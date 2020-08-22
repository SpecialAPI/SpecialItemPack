using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SGUI;
using UnityEngine;
using System.Reflection;
using System.Diagnostics;

namespace SpecialItemPack.ItemAPI
{
    public static class ResourceExtractor
    {
        private static string spritesDirectory = Path.Combine(ETGMod.ResourcesDirectory, "sprites");
        private static Assembly baseAssembly;
        /// <summary>
        /// Converts all png's in a folder to a list of Texture2D objects
        /// </summary>
        public static List<Texture2D> GetTexturesFromFolder(string folder)
        {
            string collectionPath = Path.Combine(spritesDirectory, folder);
            if (!Directory.Exists(collectionPath))
                return null;


            List<Texture2D> textures = new List<Texture2D>();
            foreach (string filePath in Directory.GetFiles(collectionPath))
            {
                Texture2D texture = BytesToTexture(File.ReadAllBytes(filePath), Path.GetFileName(filePath).Replace(".png", ""));
                textures.Add(texture);
            }
            return textures;
        }

        /// <summary>
        /// Creates a Texture2D from a file in the sprites directory
        /// </summary>
        public static Texture2D GetTextureFromFile(string fileName)
        {
            fileName = fileName.Replace(".png", "");
            string filePath = Path.Combine(spritesDirectory, fileName + ".png");
            if (!File.Exists(filePath))
            {
                ETGModConsole.Log("<color=#FF0000FF>" + filePath + " not found. </color>");
                return null;
            }
            Texture2D texture = BytesToTexture(File.ReadAllBytes(filePath), fileName);
            return texture;
        }

        /// <summary>
        /// Retuns a list of sprite collections in the sprite folder
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCollectionFiles()
        {
            List<string> collectionNames = new List<string>();
            foreach (string filePath in Directory.GetFiles(spritesDirectory))
            {
                if (filePath.EndsWith(".png"))
                {
                    collectionNames.Add(Path.GetFileName(filePath).Replace(".png", ""));
                }
            }
            return collectionNames;
        }

        /// <summary>
        /// Converts a byte array into a Texture2D
        /// </summary>
        public static Texture2D BytesToTexture(byte[] bytes, string resourceName)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(texture, bytes);
            texture.filterMode = FilterMode.Point;
            texture.name = resourceName;
            return texture;
        }

        /// <summary>
        /// Returns a list of folders in the ETG resources directory
        /// </summary>
        public static List<String> GetResourceFolders()
        {
            List<String> dirs = new List<String>();
            string spritesDirectory = Path.Combine(ETGMod.ResourcesDirectory, "sprites");

            if (Directory.Exists(spritesDirectory))
            {
                foreach (String directory in Directory.GetDirectories(spritesDirectory))
                {
                    dirs.Add(Path.GetFileName(directory));
                }
            }
            return dirs;
        }

        /// <summary>
        /// Converts an embedded resource to a byte array
        /// </summary>
        public static byte[] ExtractEmbeddedResource(String filename)
        {
            if (baseAssembly == null)
            {
                throw new NullReferenceException("Assembly not set! Did you call ItemBuilder.Init() ?");
            }
            using (Stream resFilestream = baseAssembly.GetManifestResourceStream(filename))
            {
                if (resFilestream == null)
                {
                    return null;
                }
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }

        /// <summary>
        /// Converts an embedded resource to a Texture2D object
        /// </summary>
        public static Texture2D GetTextureFromResource(string resourceName)
        {
            string file = resourceName;
            file = file.Replace("/", ".");
            file = file.Replace("\\", ".");
            byte[] bytes = ExtractEmbeddedResource(file);
            if (bytes == null)
            {
                ETGModConsole.Log("No bytes found in " + file);
                return null;
            }
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(texture, bytes);
            texture.filterMode = FilterMode.Point;

            string name = file.Substring(0, file.LastIndexOf('.'));
            if (name.LastIndexOf('.') >= 0)
            {
                name = name.Substring(name.LastIndexOf('.') + 1);
            }
            texture.name = name;

            return texture;
        }


        /// <summary>
        /// Returns a list of the names of all embedded resources
        /// </summary>
        public static string[] GetResourceNames()
        {
            if (baseAssembly == null)
            {
                throw new NullReferenceException("Assembly not set! Did you call ItemBuilder.Init() ?");
            }
            string[] names = baseAssembly.GetManifestResourceNames();
            if (names == null)
            {
                ETGModConsole.Log("No resources found.");
                return null;
            }
            return names;
        }

        public static void SetAssembly(Assembly assembly)
        {
            baseAssembly = assembly;
        }

        public static void SetAssembly(Type t)
        {
            baseAssembly = t.Assembly;
        }
    }
}
