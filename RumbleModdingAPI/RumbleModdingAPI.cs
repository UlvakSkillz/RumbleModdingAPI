using HarmonyLib;
using Il2CppExitGames.Client.Photon;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppPhoton.Pun;
using Il2CppPhoton.Realtime;
using Il2CppRUMBLE.Interactions.InteractionBase;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players;
using Il2CppRUMBLE.Players.Subsystems;
using Il2CppTMPro;
using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RumbleModdingAPI
{
    public static class ModBuildInfo
    {
        public const string Version = "5.2.1";
    }

    public class ModInfo
    {
        public string ModName;
        public string ModVersion;
        
        public ModInfo(string name, string version)
        {
            ModName = name;
            ModVersion = version;
        }
    }

    /// <summary>
    /// MelonMod Class
    /// </summary>
    public class RumbleModdingAPI : MelonMod
    {
        #region Variables
        readonly static bool debug = false;

        public static MelonMod instance;
        internal static byte myEventCode = 15;
        internal static RaiseEventOptions eventOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCache };
        internal static bool EventSent = false;
        internal static List<ModInfo> myMods = new List<ModInfo>();
        internal static string myModString;
        internal static List<ModInfo> opponentMods = new List<ModInfo>();
        internal static string opponentModString;
        internal static string currentScene = "";
        internal static string lastScene = "";
        internal static bool init = false;
        internal static bool mapInit = false;
        internal static GameObject[] allBaseDDOLGameObjects = new GameObject[4];
        internal static GameObject[] allBaseGymGameObjects = new GameObject[8];
        internal static GameObject[] allBaseParkGameObjects = new GameObject[7];
        internal static GameObject[] allBaseMap0GameObjects = new GameObject[3];
        internal static GameObject[] allBaseMap1GameObjects = new GameObject[4];
        internal static GameObject parentAPIItems;

        readonly static InputActionMap map = new InputActionMap("InputMap");
        internal static InputAction rightTrigger = map.AddAction("Right Trigger");
        internal static InputAction rightPrimary = map.AddAction("Right Primary");
        internal static InputAction rightSecondary = map.AddAction("Right Secondary");
        internal static InputAction rightGrip = map.AddAction("Right Grip");
        internal static InputAction rightJoystick = map.AddAction("Right Joystick");
        internal static InputAction rightJoystickClick = map.AddAction("Right Joystick Click");
        internal static InputAction leftTrigger = map.AddAction("Left Trigger");
        internal static InputAction leftPrimary = map.AddAction("Left Primary");
        internal static InputAction leftSecondary = map.AddAction("Left Secondary");
        internal static InputAction leftGrip = map.AddAction("Left Grip");
        internal static InputAction leftJoystick = map.AddAction("Left Joystick");
        internal static InputAction leftJoystickClick = map.AddAction("Left Joystick Click");

        internal static int matchmakingType = 0;
        internal GameObject RMLManager;

        #endregion

        #region API Initialization
        internal static void Log(string msg, bool useDebug = false)
        {
            if ((useDebug) && (debug == false)) { return; }
            Melon<RumbleModdingAPI>.Logger.Msg(msg);
        }

        internal static void Error(Exception e)
        {
            Melon<RumbleModdingAPI>.Logger.Error("Error: " + e);
        }

        public override void OnLateInitializeMelon()
        {
            instance = this;
            rightTrigger.AddBinding("<XRController>{RightHand}/trigger");
            rightPrimary.AddBinding("<XRController>{RightHand}/primaryButton");
            rightSecondary.AddBinding("<XRController>{RightHand}/secondaryButton");
            rightGrip.AddBinding("<XRController>{RightHand}/Grip");
            rightJoystick.AddBinding("<XRController>{RightHand}/primary2DAxis");
            rightJoystickClick.AddBinding("<XRController>{RightHand}/joystickClicked");
            leftTrigger.AddBinding("<XRController>{LeftHand}/trigger");
            leftPrimary.AddBinding("<XRController>{LeftHand}/primaryButton");
            leftSecondary.AddBinding("<XRController>{LeftHand}/secondaryButton");
            leftGrip.AddBinding("<XRController>{LeftHand}/Grip");
            leftJoystick.AddBinding("<XRController>{LeftHand}/primary2DAxis");
            leftJoystickClick.AddBinding("<XRController>{LeftHand}/joystickClicked");
            map.Enable();
            CreateMyModString();

            // RML merge things here
            RMAPI.PhotonRPCs.PhotonRpcInjector.Initialize();

            RMLManager = new GameObject("Manager component holder (Rumble Modding Library)");
            RMLManager.AddComponent<RMAPI.Utilities.RaiseEventManager>();
            RMLManager.AddComponent<RMAPI.Utilities.ControllerInputManager>();
            RMLManager.AddComponent<RMAPI.Utilities.DoNotDisable>();
            GameObject.DontDestroyOnLoad(RMLManager);
        }

        private static void CreateMyModString()
        {
            myModString = "";
            for (int i = 0; i < MelonBase.RegisteredMelons.Count; i++)
            {
                ModInfo mod = new ModInfo(MelonBase.RegisteredMelons[i].Info.Name, MelonBase.RegisteredMelons[i].Info.Version);
                myMods.Add(mod);
                if (i > 0)
                {
                    myModString += "|";
                }
                myModString += mod.ModName + "|" + mod.ModVersion;
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            lastScene = currentScene;
            currentScene = sceneName;
            mapInit = false;
            EventSent = false;
            RMAPI.Actions.modsSentThisScene = false;
            RMAPI.Actions.modsReceivedThisScene = false;
            RMAPI.Actions.sceneCount++;
            try
            {
                if (currentScene == "Loader")
                {
                    if (!init)
                    {
                        GrabBaseDDOLObjects();
                        init = true;
                        Log("API Initialized");
                        Log("API By UlvakSkillz. Consider Donating to Their KoFi: https://ko-fi.com/ulvakskillz");
                        //Log("onMyModsGathered Running");
                        MelonCoroutines.Start(WaitATickThemTriggerOnMyModsGathered());
                    }
                }
                else if ((currentScene == "Gym") && (!mapInit))
                {
                    GrabBaseGymObjects();
                    PhotonNetwork.NetworkingClient.EventReceived += (Action<EventData>)OnEvent;
                    RMAPI.Create.SetupAPIItems();
                    mapInit = true;
                }
                else if ((currentScene == "Park") && (!mapInit))
                {
                    GrabBaseParkObjects();
                    mapInit = true;
                }
                else if ((currentScene == "Map0") && (!mapInit))
                {
                    GrabBaseMap0Objects();
                    mapInit = true;
                }
                else if ((currentScene == "Map1") && (!mapInit))
                {
                    GrabBaseMap1Objects();
                    mapInit = true;
                }
                if (mapInit)
                {
                    MelonCoroutines.Start(RMAPI.Actions.StartActionWatcher());
                }
            }
            catch (Exception e) { Error(e); return; }
        }

        private IEnumerator WaitATickThemTriggerOnMyModsGathered()
        {
            //need to wait to give time for Mods to Hook into this in OnLateInitializeMelon/OnLevelWasLoaded
            yield return new WaitForFixedUpdate();
            RMAPI.Actions.TriggerOnMyModsGathered();
        }

        private static IEnumerator AddModsToLocalProps() // adds the player's mods to their custom props
        {
            string sceneName = RMAPI.Calls.Scene.GetSceneName();
            if (sceneName == "Gym" || sceneName == "Loader")
            {
                yield break; // photon player does not exist in the loader or gym, since they're not networked
            }
            Il2CppPhoton.Realtime.Player local = null;
            while(local == null)
            {
                try
                {
                    local = PlayerManager.instance.localPlayer.Controller.gameObject.GetComponent<PhotonView>().Owner;
                }
                catch { }
                if (local == null)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
            Il2CppExitGames.Client.Photon.Hashtable table = new();
            foreach (MelonMod mod in RegisteredMelons)
            {
                table[mod.Info.Name] = mod.Info.Version;
            }
            local.SetCustomProperties(table); // this takes a bit to update tho, not sure how long
        }

        private static void GrabBaseDDOLObjects()
        {
            List<string> ddolNamesToGrab = new List<string> { "LanguageManager", "PhotonMono", "Game Instance", "Timer Updater" };
            Il2CppReferenceArray<GameObject> ddolRootObjects = PlayerManager.instance.gameObject.scene.GetRootGameObjects();
            allBaseDDOLGameObjects = new GameObject[] { null, null, null, null };
            foreach (GameObject gameObject in ddolRootObjects)
            {
                int spot = ddolNamesToGrab.IndexOf(gameObject.name);
                if (spot >= 0)
                {
                    allBaseDDOLGameObjects[spot] = gameObject;
                }
            }
            foreach (GameObject gameObject in allBaseDDOLGameObjects)
            {
                if (gameObject == null) { Log("null", true); continue; }
                Log("Stored: " + gameObject.name, true);
            }
        }

        private static void GrabBaseGymObjects()
        {
            List<string> gymNamesToGrab = new List<string> { "!ftraceLightmaps", "ProbeVolumePerSceneData", "SCENE VFX/SFX", "SCENE", "INTERACTABLES", "TUTORIAL", "LIGHTING", "LOGIC" };
            Il2CppReferenceArray<GameObject> gymRootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            allBaseGymGameObjects = new GameObject[] { null, null, null, null, null, null, null, null };
            foreach (GameObject gameObject in gymRootObjects)
            {
                if (gymNamesToGrab.Contains(gameObject.name))
                {
                    int spot = gymNamesToGrab.IndexOf(gameObject.name);
                    if (spot >= 0)
                    {
                        allBaseGymGameObjects[spot] = gameObject;
                    }
                }
            }
            foreach (GameObject gameObject in allBaseGymGameObjects)
            {
                if (gameObject == null) { Log("Stored: null", true); continue; }
                Log("Stored: " + gameObject.name, true);
            }
        }

        private static void GrabBaseParkObjects()
        {
            List<string> parkNamesToGrab = new List<string> { "LOGIC", "!ftraceLightmaps", "ProbeVolumePerSceneData", "INTERACTABLES", "LIGHTING", "SCENE VFX/SFX", "SCENE" };
            Il2CppReferenceArray<GameObject> parkRootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            allBaseParkGameObjects = new GameObject[] { null, null, null, null, null, null, null };
            foreach (GameObject gameObject in parkRootObjects)
            {
                if (parkNamesToGrab.Contains(gameObject.name))
                {
                    int spot = parkNamesToGrab.IndexOf(gameObject.name);
                    if (spot >= 0)
                    {
                        allBaseParkGameObjects[spot] = gameObject;
                    }
                }
            }
            foreach (GameObject gameObject in allBaseParkGameObjects)
            {
                if (gameObject == null) { Log("Stored: null", true); continue; }
                Log("Stored: " + gameObject.name, true);
            }
        }

        private static void GrabBaseMap0Objects()
        {
            List<string> map0NamesToGrab = new List<string> { "Logic", "Lighting & Effects", "Scene" };
            Il2CppReferenceArray<GameObject> map0RootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            allBaseMap0GameObjects = new GameObject[] { null, null, null };
            foreach (GameObject gameObject in map0RootObjects)
            {
                if (map0NamesToGrab.Contains(gameObject.name))
                {
                    int spot = map0NamesToGrab.IndexOf(gameObject.name);
                    if (spot >= 0)
                    {
                        allBaseMap0GameObjects[spot] = gameObject;
                    }
                }
            }
            foreach (GameObject gameObject in allBaseMap0GameObjects)
            {
                if (gameObject == null) { Log("Stored: null", true); continue; }
                Log("Stored: " + gameObject.name, true);
            }
        }

        private static void GrabBaseMap1Objects()
        {
            List<string> map1NamesToGrab = new List<string> { "Lighting & Effects", "Logic", "Scene" };
            Il2CppReferenceArray<GameObject> map1RootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            allBaseMap1GameObjects = new GameObject[] { null, null, null };
            foreach (GameObject gameObject in map1RootObjects)
            {
                if (map1NamesToGrab.Contains(gameObject.name))
                {
                    int spot = map1NamesToGrab.IndexOf(gameObject.name);
                    if (spot >= 0)
                    {
                        allBaseMap1GameObjects[spot] = gameObject;
                    }
                }
            }
            foreach (GameObject gameObject in allBaseMap1GameObjects)
            {
                if (gameObject == null) { Log("Stored: null", true); continue; }
                Log("Stored: " + gameObject.name, true);
            }
        }

        internal static void GetMods()
        {
            if (!RMAPI.Actions.modsSentThisScene && !EventSent && (!PhotonNetwork.IsMasterClient) && ((currentScene == "Map0") || (currentScene == "Map1")))
            {
                EventSent = true;
                RMAPI.Actions.modsSentThisScene = true;
                PhotonNetwork.RaiseEvent(myEventCode, myModString, eventOptions, SendOptions.SendReliable);
            }
            else if (RMAPI.Actions.modsSentThisScene)
            {
                RumbleModdingAPI.Log("Prevented Multiple Mods Sent", true);
                return;
            }
        }

        public void OnEvent(EventData eventData)
        {
            if (eventData.Code == myEventCode)
            {
                if (!RMAPI.Actions.modsReceivedThisScene && PhotonNetwork.IsMasterClient)
                {
                    RMAPI.Actions.modsReceivedThisScene = true;
                    PhotonNetwork.RaiseEvent(myEventCode, myModString, eventOptions, SendOptions.SendReliable);
                }
                else if (RMAPI.Actions.modsReceivedThisScene)
                {
                    RumbleModdingAPI.Log("Prevented Multiple Mods Received", true);
                    return;
                }
                opponentMods.Clear();
                string receivedString = eventData.CustomData.ToString();
                opponentModString = receivedString;
                string[] processedString = receivedString.Split('|');
                for (int i = 0; i < processedString.Length; i += 2)
                {
                    ModInfo mod = new ModInfo(processedString[i], processedString[i + 1]);
                    opponentMods.Add(mod);
                }
                Log($"Player: {PlayerManager.instance.AllPlayers[1].Data.GeneralData.PublicUsername} / {PlayerManager.instance.AllPlayers[1].Data.GeneralData.PlayFabMasterId}");
                Log($"Mods: {receivedString}");
                RMAPI.Actions.TriggerOnModStringReceived();
            }
        }

        #endregion

        #region Patches
        [HarmonyPatch(typeof(Il2CppRUMBLE.Environment.Matchmaking.MatchmakeConsole), "MatchmakeStatusUpdated", new Type[] { typeof(MatchmakingHandler.MatchmakeStatus) })]
        public static class MatchmakingType
        {
            private static void Prefix(GameObject __instance, MatchmakingHandler.MatchmakeStatus status)
            {
                if (status == MatchmakingHandler.MatchmakeStatus.Success)
                {
                    RumbleModdingAPI.matchmakingType = RMAPI.GameObjects.Gym.INTERACTABLES.MatchConsole.MatchmakingSettings.InteractionSliderHorizontalGrip.Sliderhandle.GetGameObject().GetComponent<InteractionSlider>().snappedStep;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerController), "Initialize", new Type[] { typeof(Il2CppRUMBLE.Players.Player) })]
        public static class PlayerSpawn
        {
            private static void Postfix(ref PlayerController __instance, ref Il2CppRUMBLE.Players.Player player)
            {
                if (player.Data.GeneralData.PlayFabMasterId == "5832566FD2375E31")
                {
                    if (PlayerManager.instance.localPlayer != player)
                    {
                        MelonCoroutines.Start(WaitForTitleLoad(player));
                    }
                }
                if (__instance.controllerType == ControllerType.Local)
                {
                    MelonCoroutines.Start(AddModsToLocalProps());
                }
                //Log("onPlayerSpawned Running");
                RMAPI.Actions.TriggerOnPlayerSpawned(player);
            }
        }

        [HarmonyPatch(typeof(PlayerNameTag), "FadePlayerNameTag", new Type[] { typeof(bool) })]
        public static class UpdatePlayerTitleText
        {
            private static void Postfix(ref PlayerNameTag __instance, ref bool on)
            {
                if (!on) { return; }
                MelonCoroutines.Start(UpdateTitle(__instance));
            }
        }

        [HarmonyPatch(typeof(PlayerNameTag), nameof(PlayerNameTag.UpdatePlayerTitleText), new Type[] { })]
        public static class UpdatePlayerBPText
        {
            private static void Postfix(ref PlayerNameTag __instance)
            {
                MelonCoroutines.Start(UpdateTitle(__instance));
            }
        }

        private static IEnumerator UpdateTitle(PlayerNameTag __instance)
        {
            yield return new WaitForSeconds(0.25f);
            try
            {
                if (__instance.transform.parent.GetComponent<PlayerController>().AssignedPlayer.Data.GeneralData.PlayFabMasterId == "5832566FD2375E31")
                {
                    TMP_Text titleTextMeshPro = __instance.playerTitleText;
                    titleTextMeshPro.text = "GrassBender";
                }
            }
            catch { }
            yield break;
        }

        private static IEnumerator WaitForTitleLoad(Il2CppRUMBLE.Players.Player player)
        {
            PlayerNameTag playerNameTag = player.Controller.transform.GetChild(9).GetComponent<PlayerNameTag>();
            bool worked = true;
            TMP_Text titleTextMeshPro = null;
            try
            {
                titleTextMeshPro = playerNameTag.playerTitleText;
            }
            catch { worked = false; }
            if (worked)
            {
                while (titleTextMeshPro.text == "Player Title")
                {
                    yield return new WaitForFixedUpdate();
                }
                titleTextMeshPro.text = "GrassBender";
            }
            yield break;
        }

        #endregion
    }
}
