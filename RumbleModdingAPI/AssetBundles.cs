//I had to comment out 13-22 due to error on MelonAssembly.Assembly (had no references to it)

using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine;

namespace RumbleModdingAPI.RMAPI
{

    /// <summary>
    /// Unsed to Load AssetBundles
    /// </summary>
    public class AssetBundles
    {
        #region Asset Bundle
        /*public GameObject LoadAssetBundle(string bundleName, string objectName)
        {
            using (System.IO.Stream bundleStream = MelonAssembly.Assembly.GetManifestResourceStream(bundleName))
            {
                byte[] bundleBytes = new byte[bundleStream.Length];
                bundleStream.Read(bundleBytes, 0, bundleBytes.Length);
                Il2CppAssetBundle bundle = Il2CppAssetBundleManager.LoadFromMemory(bundleBytes);
                return UnityEngine.Object.Instantiate(bundle.LoadAsset<GameObject>(objectName));
            }
        }*/

        private static Il2CppSystem.IO.Stream ConvertToIl2CppStream(Stream stream)
        {
            Il2CppSystem.IO.MemoryStream Il2CppStream = new Il2CppSystem.IO.MemoryStream();

            const int bufferSize = 4096;
            byte[] managedBuffer = new byte[bufferSize];
            Il2CppStructArray<byte> Il2CppBuffer = new(managedBuffer);

            int bytesRead;
            while ((bytesRead = stream.Read(managedBuffer, 0, managedBuffer.Length)) > 0)
            {
                Il2CppBuffer = managedBuffer;
                Il2CppStream.Write(Il2CppBuffer, 0, bytesRead);
            }
            Il2CppStream.Flush();
            return Il2CppStream;
        }

        private static MemoryStream StreamFromFile(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);
            return new MemoryStream(fileBytes);
        }

        /// <summary>
        /// Returns a GameObject that is loaded from an AssetBundle File
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="assetName"></param>
        public static GameObject LoadAssetBundleGameObjectFromFile(string filePath, string assetName)
        {
            Stream stream = StreamFromFile(filePath);
            Il2CppSystem.IO.Stream il2CppStream = ConvertToIl2CppStream(stream);
            AssetBundle bundle = AssetBundle.LoadFromStream(il2CppStream);
            GameObject bundleObject = UnityEngine.Object.Instantiate(bundle.LoadAsset<GameObject>(assetName));
            stream.Close();
            il2CppStream.Close();
            bundle.Unload(false);
            return bundleObject;
        }

        /// <summary>
        /// Returns The AssetBundle that is loaded from an AssetBundle File (please remember to .Unload() the bundle)
        /// </summary>
        /// <param name="filePath"></param>
        public static AssetBundle LoadAssetBundleFromFile(string filePath)
        {
            Stream stream = StreamFromFile(filePath);
            Il2CppSystem.IO.Stream il2CppStream = ConvertToIl2CppStream(stream);
            AssetBundle bundle = AssetBundle.LoadFromStream(il2CppStream);
            stream.Close();
            il2CppStream.Close();
            return bundle;
        }

        /// <summary>
        /// Returns an Asset of a specified type that is loaded from an AssetBundle File
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="assetName"></param>
        public static T LoadAssetFromFile<T>(string filePath, string assetName) where T : UnityEngine.Object
        {
            Stream managedStream = StreamFromFile(filePath);
            Il2CppSystem.IO.Stream Il2CppStream = ConvertToIl2CppStream(managedStream);
            AssetBundle bundle = AssetBundle.LoadFromStream(Il2CppStream);
            T asset = bundle.LoadAsset<T>(assetName);
            managedStream.Close();
            Il2CppStream.Close();
            bundle.Unload(false);
            return asset;
        }

        //didnt work preupdate
        /// <summary>
        /// Returns all assets from a specified AssetBundle File
        /// </summary>
        /// <param name="filePath"></param>
        public static List<T> LoadAllOfTypeFromFile<T>(string filePath) where T : UnityEngine.Object
        {
            Stream managedStream = StreamFromFile(filePath);
            Il2CppSystem.IO.Stream Il2CppStream = ConvertToIl2CppStream(managedStream);
            AssetBundle bundle = AssetBundle.LoadFromStream(Il2CppStream);
            List<T> assetsList = new List<T>();
            foreach (T anAsset in bundle.LoadAll<T>())
            {
                assetsList.Add(anAsset);
            }
            managedStream.Close();
            Il2CppStream.Close();
            bundle.Unload(false);
            return assetsList;
        }

        /// <summary>
        /// Returns an AssetBundle from a stream (Embedded Resource) (please remember to .Unload() the bundle)
        /// </summary>
        /// <param name="modName"></param>
        /// <param name="modAuthor"></param>
        /// <param name="assetPath"></param>
        public static AssetBundle LoadAssetBundleFromStream(string modName, string modAuthor, string assetPath)
        {
            using (Stream bundleStream = MelonMod.FindMelon(modName, modAuthor).MelonAssembly.Assembly.GetManifestResourceStream(assetPath))
            {
                Il2CppSystem.IO.Stream Il2CppStream = ConvertToIl2CppStream(bundleStream);
                AssetBundle bundle = AssetBundle.LoadFromStream(Il2CppStream);
                Il2CppStream.Close();
                return bundle;
            }
        }

        /// <summary>
        /// Returns an AssetBundle from a stream (Embedded Resource) (please remember to .Unload() the bundle)
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="assetPath"></param>
        public static AssetBundle LoadAssetBundleFromStream(MelonMod instance, string assetPath)
        {
            using (Stream bundleStream = instance.MelonAssembly.Assembly.GetManifestResourceStream(assetPath))
            {
                Il2CppSystem.IO.Stream Il2CppStream = ConvertToIl2CppStream(bundleStream);
                AssetBundle bundle = AssetBundle.LoadFromStream(Il2CppStream);
                Il2CppStream.Close();
                return bundle;
            }
        }

        /// <summary>
        /// Returns as Asset from a stream (embedded resource)
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="path"></param>
        /// <param name="assetName"></param>
        public static T LoadAssetFromStream<T>(MelonMod instance, string path, string assetName) where T : UnityEngine.Object
        {
            using (Stream bundleStream = instance.MelonAssembly.Assembly.GetManifestResourceStream(path))
            {
                Il2CppSystem.IO.Stream Il2CppStream = ConvertToIl2CppStream(bundleStream);
                AssetBundle bundle = AssetBundle.LoadFromStream(Il2CppStream);
                Il2CppStream.Close();
                T asset = bundle.LoadAsset<T>(assetName);
                bundle.Unload(false);
                return asset;
            }
        }

        //didnt work preupdate
        /// <summary>
        /// Returns a list of Assets of a specified type from a stream (Enbedded Resource)
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="path"></param>
        public static List<T> LoadAllOfTypeFromStream<T>(MelonMod instance, string path) where T : UnityEngine.Object
        {
            using (Stream bundleStream = instance.MelonAssembly.Assembly.GetManifestResourceStream(path))
            {
                Il2CppSystem.IO.Stream Il2CppStream = ConvertToIl2CppStream(bundleStream);
                AssetBundle bundle = AssetBundle.LoadFromStream(Il2CppStream);
                List<T> assetsList = new List<T>();
                Il2CppArrayBase<T> assets = bundle.LoadAll<T>();
                foreach (T anAsset in assets)
                {
                    assetsList.Add(anAsset);
                }
                Il2CppStream.Close();
                bundle.Unload(false);
                return assetsList;
            }
        }

        /// <summary>
        /// Returns an Asset a stream (Enbedded Resource)
        /// </summary>
        /// <param name="modName"></param>
        /// <param name="modAuthor"></param>
        /// <param name="path"></param>
        /// <param name="assetName"></param>
        public static T LoadAssetFromStream<T>(string modName, string modAuthor, string path, string assetName) where T : UnityEngine.Object
        {
            using (Stream bundleStream = MelonMod.FindMelon(modName, modAuthor).MelonAssembly.Assembly.GetManifestResourceStream(path))
            {
                Il2CppSystem.IO.Stream Il2CppStream = ConvertToIl2CppStream(bundleStream);
                AssetBundle bundle = AssetBundle.LoadFromStream(Il2CppStream);
                Il2CppStream.Close();
                T asset = bundle.LoadAsset<T>(assetName);
                bundle.Unload(false);
                return asset;
            }
        }
        #endregion
    }
}
