//RML Merge condensed to 1 file (4 calls to it in Mod File)

using HarmonyLib;
using Il2CppExitGames.Client.Photon;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppPhoton.Pun;
using Il2CppPhoton.Realtime;
using MelonLoader;
using System.Reflection;
using UnityEngine;
using System.Collections;
using RC = RumbleModdingAPI.RumbleModdingAPI.ControllerMap.RightController;
using LC = RumbleModdingAPI.RumbleModdingAPI.ControllerMap.LeftController;

namespace RumbleModdingAPI.RMAPI
{
    #region RML merge
    public class Utilities
    {
        #region RumbleMod (MelonMod with more, meant to be inherited)
        /// <summary>
        /// this is a MelonMod but with a built in RaiseEvent manager
        /// </summary>
        public class RumbleMod : MelonMod
        {
            #region events
            public bool EventsRegisted;
            public void RegisterEvents()
            {
                if (!EventsRegisted)
                {
                    RaiseEventManager.instance.RegisterMod(this);
                    EventsRegisted = true;
                }
            }
            public void RaiseEvent(List<Il2CppSystem.Object> data, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
            {
                data.Insert(0, $"{this.Info.Name}|{this.Info.Author}");
                var il2cppArray = new Il2CppReferenceArray<Il2CppSystem.Object>(data.Count);
                for (int i = 0; i < data.Count; i++)
                {
                    il2cppArray[i] = data[i];
                }
                Il2CppSystem.Object boxedData = il2cppArray.Cast<Il2CppSystem.Object>();
                PhotonNetwork.RaiseEvent(18, boxedData, raiseEventOptions, sendOptions);
            }

            public virtual void OnEvent(List<Il2CppSystem.Object> data)
            {

            }
            #endregion

            public UnityEngine.Il2CppAssetBundle LoadAssetBundle(string path)
            {
                using (System.IO.Stream bundleStream = MelonAssembly.Assembly.GetManifestResourceStream(path))
                {
                    byte[] bundleBytes = new byte[bundleStream.Length];
                    bundleStream.Read(bundleBytes, 0, bundleBytes.Length);
                    return Il2CppAssetBundleManager.LoadFromMemory(bundleBytes);
                }
            }
        }
        #endregion

        #region RaiseEventManager (internal)
        [RegisterTypeInIl2Cpp]
        internal class RaiseEventManager : MonoBehaviour
        {
            public static Il2CppSystem.Object BoxListToObject(List<Il2CppSystem.Object> data)
            {
                var il2cppArray = new Il2CppReferenceArray<Il2CppSystem.Object>(data.Count);
                for (int i = 0; i < data.Count; i++)
                {
                    il2cppArray[i] = data[i];
                }
                Il2CppSystem.Object boxedData = il2cppArray.Cast<Il2CppSystem.Object>();
                return boxedData;
            }

            internal static RaiseEventManager instance;

            internal Dictionary<string, RumbleMod> RegisteredMods = new Dictionary<string, RumbleMod>();

            public void Start()
            {
                MelonCoroutines.Start(Initialize());
            }

            public IEnumerator Initialize()
            {
                yield return new WaitForFixedUpdate();
                try
                {
                    instance = this;
                    PhotonNetwork.NetworkingClient.EventReceived += (Action<EventData>)OnEvent;
                    MelonLogger.Msg("RaiseEventManager initialized");
                }
                catch
                {
                    Start();
                }
            }

            public void RegisterMod(RumbleMod mod)
            {
                RegisteredMods.Add($"{mod.Info.Name}|{mod.Info.Author}", mod);
            }

            public void OnEvent(EventData eventData)
            {
                if (eventData.Code == 18)
                {
                    var boxedData = eventData.CustomData;
                    var unboxedData = boxedData.Cast<Il2CppReferenceArray<Il2CppSystem.Object>>();
                    MelonLogger.Msg($"unboxing data with length of {unboxedData.Length} - RaiseEventManager"); // TO  BE REMOVED
                    List<Il2CppSystem.Object> unboxedList = new();
                    for (int i = 1; i < unboxedData.Count; i++)
                    {
                        unboxedList.Add(unboxedData[i]);
                    }
                    string sender = unboxedData[0].ToString();

                    MelonLogger.Msg($"RaiseEvent received and is being sent to '{sender}' - RaiseEventManager");
                    RegisteredMods[sender].OnEvent(unboxedList);
                }
            }
        }
        #endregion

        #region Controller Input Poller, basically more advanced input manager
        [RegisterTypeInIl2Cpp]
        internal class ControllerInputManager : MonoBehaviour // manages the inputs on the public class,
        {
            #region right controller values
            private float rPrimaryFloat; // right primary
            private bool rPrimaryBool;
            private bool rPrimaryPrevBool;

            private float rSecondaryFloat; // right secondary
            private bool rSecondaryBool;
            private bool rSecondaryPrevBool;

            private float rStickFloat; // right stick click
            private bool rStickBool;
            private bool rStickPrevBool;

            private float rTriggerFloat; // right trigger
            private bool rTriggerBool;
            private bool rTriggerPrevBool;

            private float rGripFloat; // right grip
            private bool rGripBool;
            private bool rGripPrevBool;
            #endregion
            #region left controller values
            private float lPrimaryFloat; // left primary
            private bool lPrimaryBool;
            private bool lPrimaryPrevBool;

            private float lSecondaryFloat; // left secondary
            private bool lSecondaryBool;
            private bool lSecondaryPrevBool;

            private float lStickFloat; // left stick click
            private bool lStickBool;
            private bool lStickPrevBool;

            private float lTriggerFloat; // left trigger
            private bool lTriggerBool;
            private bool lTriggerPrevBool;

            private float lGripFloat; // left grip
            private bool lGripBool;
            private bool lGripPrevBool;
            #endregion

            void Update()
            {
                GetFloats(); // gets values from RMAPI
                UpdateFloats(); // updates floats in ControllerInputPoller
                GetAndUpdateBooleans(); // gets and updates booleans
                UpdateJoysticks(); // updates the joystick position values in ControllerInputPoller
                UpdatePressedOrReleasedThisFrame(); // updates the WasPressedThisFrame and WasReleasedThisFrame variables in ControllerInputPoller
            }

            private void GetFloats()
            {
                rPrimaryFloat = RC.GetPrimary(); // right controller float values
                rSecondaryFloat = RC.GetSecondary();
                rStickFloat = RC.GetJoystickClick();
                rTriggerFloat = RC.GetTrigger();
                rGripFloat = RC.GetGrip();

                lPrimaryFloat = LC.GetPrimary(); // left controller float values
                lSecondaryFloat = LC.GetSecondary();
                lStickFloat = LC.GetJoystickClick();
                lTriggerFloat = LC.GetTrigger();
                lGripFloat = LC.GetGrip();
            }
            private void GetAndUpdateBooleans()
            {
                rPrimaryPrevBool = rPrimaryBool; // right controller starts here
                rPrimaryBool = rPrimaryFloat > 0.25f;
                ControllerInputPoller.RightController.primaryButton.pressed = rPrimaryBool;

                rSecondaryPrevBool = rSecondaryBool;
                rSecondaryBool = rSecondaryFloat > 0.25f;
                ControllerInputPoller.RightController.secondaryButton.pressed = rSecondaryBool;

                rStickPrevBool = rStickBool;
                rStickBool = rStickFloat > 0.25f;
                ControllerInputPoller.RightController.joystickClick.pressed = rStickBool;

                rTriggerPrevBool = rTriggerBool;
                rTriggerBool = rTriggerFloat > 0.25f;
                ControllerInputPoller.RightController.trigger.pressed = rTriggerBool;

                rGripPrevBool = rGripBool;
                rGripBool = rGripFloat > 0.25f;
                ControllerInputPoller.RightController.grip.pressed = rGripBool;


                lPrimaryPrevBool = lPrimaryBool; // left controller starts here
                lPrimaryBool = lPrimaryFloat > 0.25f;
                ControllerInputPoller.LeftController.primaryButton.pressed = lPrimaryBool;

                lSecondaryPrevBool = lSecondaryBool;
                lSecondaryBool = lSecondaryFloat > 0.25f;
                ControllerInputPoller.LeftController.secondaryButton.pressed = lSecondaryBool;

                lStickPrevBool = lStickBool;
                lStickBool = lStickFloat > 0.25f;
                ControllerInputPoller.LeftController.joystickClick.pressed = lStickBool;

                lTriggerPrevBool = lTriggerBool;
                lTriggerBool = lTriggerFloat > 0.25f;
                ControllerInputPoller.LeftController.trigger.pressed = lTriggerBool;

                lGripPrevBool = lGripBool;
                lGripBool = lGripFloat > 0.25f;
                ControllerInputPoller.LeftController.grip.pressed = lGripBool;
            }
            private void UpdateFloats()
            {
                ControllerInputPoller.RightController.primaryButton.value = rPrimaryFloat;
                ControllerInputPoller.RightController.secondaryButton.value = rSecondaryFloat;
                ControllerInputPoller.RightController.joystickClick.value = rStickFloat;
                ControllerInputPoller.RightController.trigger.value = rTriggerFloat;
                ControllerInputPoller.RightController.grip.value = rGripFloat;

                ControllerInputPoller.LeftController.primaryButton.value = lPrimaryFloat;
                ControllerInputPoller.LeftController.secondaryButton.value = lSecondaryFloat;
                ControllerInputPoller.LeftController.joystickClick.value = lStickFloat;
                ControllerInputPoller.LeftController.trigger.value = lTriggerFloat;
                ControllerInputPoller.LeftController.grip.value = lGripFloat;
            }
            private void UpdateJoysticks()
            {
                ControllerInputPoller.RightController.joystickPosition = RC.GetJoystick();
                ControllerInputPoller.LeftController.joystickPosition = LC.GetJoystick();
            }
            private void UpdatePressedOrReleasedThisFrame() // holy shit that's a long name
            {
                ControllerInputPoller.RightController.primaryButton.wasPressedThisFrame = (rPrimaryBool && !rPrimaryPrevBool); // right controller starts here
                ControllerInputPoller.RightController.primaryButton.wasReleasedThisFrame = (!rPrimaryBool && rPrimaryPrevBool);

                ControllerInputPoller.RightController.secondaryButton.wasPressedThisFrame = (rSecondaryBool && !rSecondaryPrevBool);
                ControllerInputPoller.RightController.secondaryButton.wasReleasedThisFrame = (!rSecondaryBool && rSecondaryPrevBool);

                ControllerInputPoller.RightController.joystickClick.wasPressedThisFrame = (rStickBool && !rStickPrevBool);
                ControllerInputPoller.RightController.joystickClick.wasReleasedThisFrame = (!rStickBool && rStickPrevBool);

                ControllerInputPoller.RightController.trigger.wasPressedThisFrame = (rTriggerBool && !rTriggerPrevBool);
                ControllerInputPoller.RightController.trigger.wasReleasedThisFrame = (!rTriggerBool && rTriggerPrevBool);

                ControllerInputPoller.RightController.grip.wasPressedThisFrame = (rGripBool && !rGripPrevBool);
                ControllerInputPoller.RightController.grip.wasReleasedThisFrame = (!rGripBool && rGripPrevBool);


                ControllerInputPoller.LeftController.primaryButton.wasPressedThisFrame = (lPrimaryBool && !lPrimaryPrevBool); // left controller starts here
                ControllerInputPoller.LeftController.primaryButton.wasReleasedThisFrame = (!lPrimaryBool && lPrimaryPrevBool);

                ControllerInputPoller.LeftController.secondaryButton.wasPressedThisFrame = (lSecondaryBool && !lSecondaryPrevBool);
                ControllerInputPoller.LeftController.secondaryButton.wasReleasedThisFrame = (!lSecondaryBool && lSecondaryPrevBool);

                ControllerInputPoller.LeftController.joystickClick.wasPressedThisFrame = (lStickBool && !lStickPrevBool);
                ControllerInputPoller.LeftController.joystickClick.wasReleasedThisFrame = (!lStickBool && lStickPrevBool);

                ControllerInputPoller.LeftController.trigger.wasPressedThisFrame = (lTriggerBool && !lTriggerPrevBool);
                ControllerInputPoller.LeftController.trigger.wasReleasedThisFrame = (!lTriggerBool && lTriggerPrevBool);

                ControllerInputPoller.LeftController.grip.wasPressedThisFrame = (lGripBool && !lGripPrevBool);
                ControllerInputPoller.LeftController.grip.wasReleasedThisFrame = (!lGripBool && lGripPrevBool);
            }
        }
        /// <summary>
        /// an extention to RMAPI's input manager that does a lot of the work for you
        /// </summary>
        public static class ControllerInputPoller // named after gorilla tag's Controller Input Poller (i'm horrible with naming things)
        {
            public class Button // button class to keep all the buttons neat
            {
                public bool wasPressedThisFrame;
                public bool wasReleasedThisFrame;
                public bool pressed;
                public float value;
            }
            public class Controller // controller class to save lines and make stuff look nice
            {
                public Button primaryButton = new();
                public Button secondaryButton = new();
                public Button joystickClick = new();
                public Button trigger = new();
                public Button grip = new();
                public Vector2 joystickPosition = new();
            }

            public static Controller RightController = new Controller();
            public static Controller LeftController = new Controller();
        }
        #endregion
        #region DoNotDisable (component)
        [RegisterTypeInIl2Cpp]
        public class DoNotDisable : MonoBehaviour
        {
            public void OnDisable()
            {
                gameObject.SetActive(true);
            }
        }
        #endregion
    }
    
    public class PhotonRPCs
    {
        /// <summary>
        /// add this attribute to a method inside of a MonoBehaviourPun component to use Photon's RPCs
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        public class PunRPC : Attribute { } // attribute for RPCs

        #region RPC injector
        public static class PhotonRpcInjector
        {
            public static Dictionary<string, List<System.Reflection.MethodInfo>> methodsInType = new();

            public static void Initialize() // initializes the rpc manager
            {
                MelonLogger.Msg("initializing RPC Manager");
                var loadedMods = MelonMod.RegisteredMelons;
                foreach (MelonMod mod in loadedMods)
                {
                    //MelonLogger.Msg($"searching for RPCs in {mod.Info.Name}"); // spams logs
                    var assembly = mod.MelonAssembly.Assembly;

                    Type[] types = assembly.GetTypes();

                    foreach (Type type in types)
                    {
                        //MelonLogger.Msg($"searching for methods in {type.FullName}"); // also spams logs
                        foreach (var method in type.GetMethods())
                        {
                            //MelonLogger.Msg($"looking for attribute at {method.Name}"); // 54k lines later...
                            var rpcAttribute = method.GetCustomAttribute<PunRPC>();

                            if (rpcAttribute != null)
                            {
                                MelonLogger.Msg($"found RPC attribute at {method.Name}");
                                PhotonRpcInjector.RegisterMethod(type, method);
                            }
                        }
                    }
                }
            }

            public static void RegisterMethod(Type type, System.Reflection.MethodInfo method) // registers a method to the RPC manager and adds it to the methods with an RPC Attribute list
            {
                string typeName = type.FullName;

                if (!methodsInType.ContainsKey(typeName))
                {
                    methodsInType[typeName] = new List<System.Reflection.MethodInfo>(); // adds a new key to the dictionary if there isnt one already
                }

                methodsInType[typeName].Add(method);
                RegisterRpc(method.Name);

                MelonLogger.Msg($"Successfully registered: {typeName}.{method.Name} as an RPC");
            }

            public static void RegisterRpc(string methodName) // adds the method name to the RPC Shortcuts and rpc list
            {

                Il2CppSystem.Collections.Generic.Dictionary<string, int> rpcShortcuts = PhotonNetwork.rpcShortcuts;
                Il2CppSystem.Collections.Generic.List<string> rpcList = PhotonNetwork.PhotonServerSettings.RpcList;

                if (!rpcShortcuts.ContainsKey(methodName) && !rpcList.Contains(methodName))
                {
                    int newId = rpcShortcuts.Count;
                    rpcShortcuts.Add(methodName, newId);
                    rpcList.Add(methodName);
                    MelonLogger.Msg($"Registered RPC '{methodName}' with shortcut ID {newId}");
                }
                else
                {
                    MelonLogger.Msg($"RPC '{methodName}' already registered.");
                }
            }
        }

        [HarmonyPatch(typeof(SupportClass), "GetMethods")]
        public static class GetMethodsPatch // patches the supportclass's GetMethods function to also return the custom RPCs
        {
            public static void Postfix(Il2CppSystem.Type type, ref Il2CppSystem.Collections.Generic.List<Il2CppSystem.Reflection.MethodInfo> __result)
            {
                string typeName = type.FullName;

                if (PhotonRpcInjector.methodsInType.ContainsKey(typeName))
                {
                    var rpcMethods = PhotonRpcInjector.methodsInType[typeName];
                    var il2cppMethods = type.GetMethods();

                    foreach (var il2cppMethod in il2cppMethods)
                    {
                        foreach (var rpcMethod in rpcMethods)
                        {
                            if (il2cppMethod.Name == rpcMethod.Name)
                            {
                                __result.Add(il2cppMethod);
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }

    #endregion
}
