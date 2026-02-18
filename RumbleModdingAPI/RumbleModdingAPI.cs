//To do:
//finish moving the Actions to the other file
//rework the way it grabs scene objects then store them

using HarmonyLib;
using Il2CppExitGames.Client.Photon;
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
using static MelonLoader.MelonLogger;

namespace RumbleModdingAPI
{
    public static class ModBuildInfo
    {
        public const string Version = "5.0.0";
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

    public class RumbleModdingAPI : MelonMod
    {
        #region Variables
        public static MelonMod instance;
        public byte myEventCode = 15;
        public static RaiseEventOptions eventOptions = new RaiseEventOptions() { Receivers = ReceiverGroup.Others, CachingOption = EventCaching.AddToRoomCache };
        public bool EventSent = false;
        public static List<ModInfo> myMods = new List<ModInfo>();
        public static string myModString;
        public static List<ModInfo> opponentMods = new List<ModInfo>();
        public static string opponentModString;
        private bool sceneChanged = false;
        private static DateTime whenSceneChanged = DateTime.Now;
        private static string currentScene = "";
        private static string lastScene = "";
        private static bool init = false;
        private static bool mapInit = false;
        public static System.Collections.Generic.List<GameObject> allBaseDDOLGameObjects = new System.Collections.Generic.List<GameObject>();
        public static System.Collections.Generic.List<GameObject> allBaseGymGameObjects = new System.Collections.Generic.List<GameObject>();
        public static System.Collections.Generic.List<GameObject> allBaseParkGameObjects = new System.Collections.Generic.List<GameObject>();
        public static System.Collections.Generic.List<GameObject> allBaseMap0GameObjects = new System.Collections.Generic.List<GameObject>();
        public static System.Collections.Generic.List<GameObject> allBaseMap1GameObjects = new System.Collections.Generic.List<GameObject>();
        public static event System.Action onModStringRecieved;
        public static event System.Action onMyModsGathered;
        public static event System.Action onMapInitialized;
        public static event System.Action<string> onAMapInitialized;
        public static event System.Action onPlayerSpawned;
        public static event System.Action<Il2CppRUMBLE.Players.Player> onAPlayerSpawned;
        public static event System.Action onMatchStarted;
        public static event System.Action onMatchEnded;
        public static event System.Action onRoundStarted;
        public static event System.Action onRoundEnded;
        public static event System.Action onLocalPlayerHealthChanged;
        public static event System.Action onRemotePlayerHealthChanged;
        public static event System.Action<Il2CppRUMBLE.Players.Player, int> onPlayerHealthChanged;
        public static GameObject parentAPIItems;

        private static InputActionMap map = new InputActionMap("InputMap");
        private static InputAction rightTrigger = map.AddAction("Right Trigger");
        private static InputAction rightPrimary = map.AddAction("Right Primary");
        private static InputAction rightSecondary = map.AddAction("Right Secondary");
        private static InputAction rightGrip = map.AddAction("Right Grip");
        private static InputAction rightJoystick = map.AddAction("Right Joystick");
        private static InputAction rightJoystickClick = map.AddAction("Right Joystick Click");
        private static InputAction leftTrigger = map.AddAction("Left Trigger");
        private static InputAction leftPrimary = map.AddAction("Left Primary");
        private static InputAction leftSecondary = map.AddAction("Left Secondary");
        private static InputAction leftGrip = map.AddAction("Left Grip");
        private static InputAction leftJoystick = map.AddAction("Left Joystick");
        private static InputAction leftJoystickClick = map.AddAction("Left Joystick Click");

        private int sceneCount = 0;
        private int[] healths;
        private int playerCount = 0;
        private bool waitForMatchStart = false;
        private GameObject matchSlab;
        private bool matchStarted = false;
        public static int matchmakingType = 0;
        private static Instance logger;
        public GameObject RMLManager;

        #endregion

        #region API Initialization

        public static void Log(string msg)
        {
            logger.Msg(msg);
        }

        public override void OnLateInitializeMelon()
        {
            instance = this;
            logger = LoggerInstance;
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

        private void CreateMyModString()
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
            sceneCount++;
            lastScene = currentScene;
            currentScene = sceneName;
            sceneChanged = true;
            whenSceneChanged = DateTime.Now;
            mapInit = false;
            EventSent = false;
        }

        public static IEnumerator AddModsToLocalProps() // adds the player's mods to their custom props
        {
            string sceneName = Scene.GetSceneName();
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

        public override void OnFixedUpdate()
        {
            if (sceneChanged)
            {
                try
                {
                    if (currentScene == "Loader")
                    {
                        if (!init)
                        {
                            allBaseDDOLGameObjects.Clear();
                            allBaseDDOLGameObjects.Add(GameObject.Find("/LanguageManager"));
                            allBaseDDOLGameObjects.Add(GameObject.Find("/PhotonMono"));
                            allBaseDDOLGameObjects.Add(GameObject.Find("/Game Instance"));
                            allBaseDDOLGameObjects.Add(GameObject.Find("/Timer Updater"));
                            allBaseDDOLGameObjects.Add(GameObject.Find("/LIV"));
                            allBaseDDOLGameObjects.Add(GameObject.Find("/PlayFabHttp"));
                            init = true;
                            Log("API Initialized");
                            Log("API By UlvakSkillz. Consider Donating to Their KoFi: https://ko-fi.com/ulvakskillz");
                            //Log("onMyModsGathered Running");
                            Delegate[] listeners = onMyModsGathered?.GetInvocationList();
                            if (listeners != null)
                            {
                                foreach (Delegate listener in listeners)
                                {
                                    try
                                    {
                                        // Invoke each listener individually
                                        listener.DynamicInvoke();
                                    }
                                    catch (Exception e)
                                    {
                                        logger.Msg("onMyModsGathered Error for Mod: " + listener.Target);
                                        logger.Error(e.InnerException);
                                        // The loop continues to the next listener even if one fails
                                    }
                                }
                            }
                        }
                    }
                    else if ((currentScene == "Gym") && (!mapInit))
                    {
                        allBaseGymGameObjects.Clear();
                        allBaseGymGameObjects.Add(GameObject.Find("------------TUTORIAL------------"));
                        allBaseGymGameObjects.Add(GameObject.Find("--------------SCENE--------------"));
                        allBaseGymGameObjects.Add(GameObject.Find("!ftraceLightmaps"));
                        allBaseGymGameObjects.Add(GameObject.Find("LanguageManager"));
                        allBaseGymGameObjects.Add(GameObject.Find("--------------LOGIC--------------"));
                        if (parentAPIItems == null)
                        {
                            PhotonNetwork.NetworkingClient.EventReceived += (Action<EventData>)OnEvent;
                            parentAPIItems = new GameObject();
                            parentAPIItems.name = "APIItems";
                            GameObject.DontDestroyOnLoad(parentAPIItems);
                        }
                        if (RMAPI.Create.newTextGameObject == null)
                        {
                            RMAPI.Create.newTextGameObject = GameObject.Instantiate(RMAPI.GameObjects.Gym.INTERACTABLES.Leaderboard.PlayerTags.HighscoreTag.Nr.GetGameObject());
                            TextMeshPro tmp = RMAPI.Create.newTextGameObject.GetComponent<TextMeshPro>();
                            RMAPI.Create.newTextGameObject.name = "NewTextGameObject";
                            tmp.text = "new Text";
                            tmp.color = Color.black;
                            RMAPI.Create.newTextGameObject.SetActive(false);
                            RMAPI.Create.newTextGameObject.transform.parent = parentAPIItems.transform;
                        }
                        if (RMAPI.Create.newButtonGameObject == null)
                        {
                            RMAPI.Create.newButtonGameObject = GameObject.Instantiate(RMAPI.GameObjects.Gym.TUTORIAL.Statictutorials.RUMBLEStarterGuide.NextPageButton.InteractionButton.GetGameObject());
                            RMAPI.Create.newButtonGameObject.name = "newButton";
                            RMAPI.Create.newButtonGameObject.SetActive(false);
                            RMAPI.Create.newButtonGameObject.transform.parent = parentAPIItems.transform;
                        }
                        mapInit = true;
                    }
                    else if ((currentScene == "Park") && (!mapInit))
                    {
                        allBaseParkGameObjects.Clear();
                        allBaseParkGameObjects.Add(GameObject.Find("________________LOGIC__________________ "));
                        allBaseParkGameObjects.Add(GameObject.Find("!ftraceLightmaps"));
                        allBaseParkGameObjects.Add(GameObject.Find("________________SCENE_________________"));
                        allBaseParkGameObjects.Add(GameObject.Find("Lighting and effects"));
                        mapInit = true;
                    }
                    else if ((currentScene == "Map0") && (!mapInit))
                    {
                        allBaseMap0GameObjects.Clear();
                        allBaseMap0GameObjects.Add(GameObject.Find("Logic"));
                        allBaseMap0GameObjects.Add(GameObject.Find("!ftraceLightmaps"));
                        allBaseMap0GameObjects.Add(GameObject.Find("Lighting & Effects"));
                        allBaseMap0GameObjects.Add(GameObject.Find("Map0_production"));
                        mapInit = true;
                    }
                    else if ((currentScene == "Map1") && (!mapInit))
                    {
                        allBaseMap1GameObjects.Clear();
                        allBaseMap1GameObjects.Add(GameObject.Find("!ftraceLightmaps"));
                        allBaseMap1GameObjects.Add(GameObject.Find("Lighting & Effects"));
                        allBaseMap1GameObjects.Add(GameObject.Find("Logic"));
                        allBaseMap1GameObjects.Add(GameObject.Find("Map1_production"));
                        mapInit = true;
                    }
                    if (mapInit)
                    {
                        MelonCoroutines.Start(GetHealth());
                    }
                }
                catch (Exception e) { logger.Error(e); return; }
                sceneChanged = false;
            }
        }

        private IEnumerator GetHealth()
        {
            bool gotHealth = false;
            GameObject localHealthbarGameObject = null;
            while (!gotHealth)
            {
                try
                {
                    localHealthbarGameObject = PlayerManager.instance.localPlayer.Controller.gameObject.transform.GetChild(6).GetChild(0).gameObject;
                    //Log("Got Local Healthbar");
                }
                catch { }
                if (localHealthbarGameObject == null)
                {
                    //Log("Local Health Null");
                    yield return new WaitForFixedUpdate();
                    continue;
                }
                if (localHealthbarGameObject != null)
                {
                    //Log("Local Health != null");
                    gotHealth = true;
                    yield return new WaitForFixedUpdate();
                    try
                    {
                        //Log("onMapInitialized Running: " + currentScene);
                        Delegate[] listeners = onMapInitialized?.GetInvocationList();
                        if (listeners != null)
                        {
                            foreach (Delegate listener in listeners)
                            {
                                try
                                {
                                    // Invoke each listener individually
                                    listener.DynamicInvoke();
                                }
                                catch (Exception e)
                                {
                                    logger.Msg("onMapInitialized Error for Mod: " + listener.Target);
                                    logger.Error(e.InnerException);
                                    // The loop continues to the next listener even if one fails
                                }
                            }
                        }
                        //Log("onAMapInitialized Running: " + currentScene);
                        Delegate[] listeners2 = onAMapInitialized?.GetInvocationList();
                        if (listeners2 != null)
                        {
                            foreach (Delegate listener in listeners2)
                            {
                                try
                                {
                                    // Invoke each listener individually
                                    listener.DynamicInvoke(currentScene);
                                }
                                catch (Exception e)
                                {
                                    logger.Msg("onAMapInitialized Error for Mod: " + listener.Target);
                                    logger.Error(e.InnerException);
                                    // The loop continues to the next listener even if one fails
                                }
                            }
                        }
                        //Log("onMapInitialized Complete");
                        GetMods();
                        if (PlayerManager.instance.AllPlayers.Count > 1)
                        {
                            //Log("More than 1 Player");
                            if (currentScene == "Map0")
                            {
                                //Log("Grabbing SlabOne MatchFormCanvas");
                                matchSlab = RMAPI.GameObjects.Map0.Logic.MatchSlabOne.MatchSlab.Slabbuddymatchvariant.MatchForm.MatchFormCanvas.GetGameObject();
                            }
                            else if (currentScene == "Map1")
                            {
                                //Log("Grabbing SlabTwo MatchFormCanvas");
                                matchSlab = RMAPI.GameObjects.Map1.Logic.MatchSlabTwo.MatchSlab.Slabbuddymatchvariant.MatchForm.MatchFormCanvas.GetGameObject();
                            }
                            matchStarted = true;
                            //Log("Get Health Current Scene: " + currentScene);
                            if ((currentScene == "Map0") || (currentScene == "Map1"))
                            {
                                //Log("onMatchStarted Running");
                                Delegate[] listeners3 = onMatchStarted?.GetInvocationList();
                                if (listeners3 != null)
                                {
                                    foreach (Delegate listener in listeners3)
                                    {
                                        try
                                        {
                                            // Invoke each listener individually
                                            listener.DynamicInvoke();
                                        }
                                        catch (Exception e)
                                        {
                                            logger.Msg("onMatchStarted Error for Mod: " + listener.Target);
                                            logger.Error(e.InnerException);
                                            // The loop continues to the next listener even if one fails
                                        }
                                    }
                                }
                                //Log("onRoundStarted Running");
                                Delegate[] listeners4 = onRoundStarted?.GetInvocationList();
                                if (listeners4 != null)
                                {
                                    foreach (Delegate listener in listeners4)
                                    {
                                        try
                                        {
                                            // Invoke each listener individually
                                            listener.DynamicInvoke();
                                        }
                                        catch (Exception e)
                                        {
                                            logger.Msg("onRoundStarted Error for Mod: " + listener.Target);
                                            logger.Error(e.InnerException);
                                            // The loop continues to the next listener even if one fails
                                        }
                                    }
                                }
                            }
                        }
                        MelonCoroutines.Start(HealthWatcher(sceneCount));
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }
                    yield break;
                }
                else
                {
                    yield return new WaitForFixedUpdate();
                }
            }
            yield break;
        }

        private void GetMods()
        {
            if (!EventSent && (!PhotonNetwork.IsMasterClient) && ((currentScene == "Map0") || (currentScene == "Map1")))
            {
                EventSent = true;
                PhotonNetwork.RaiseEvent(myEventCode, myModString, eventOptions, SendOptions.SendReliable);
            }
        }

        public void OnEvent(EventData eventData)
        {
            if (eventData.Code == myEventCode)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.RaiseEvent(myEventCode, myModString, eventOptions, SendOptions.SendReliable);
                }
                opponentMods.Clear();
                string recievedString = eventData.CustomData.ToString();
                opponentModString = recievedString;
                string[] processedString = recievedString.Split('|');
                for (int i = 0; i < processedString.Length; i += 2)
                {
                    ModInfo mod = new ModInfo(processedString[i], processedString[i + 1]);
                    opponentMods.Add(mod);
                }
                Log($"Player: {PlayerManager.instance.AllPlayers[1].Data.GeneralData.PublicUsername} / {PlayerManager.instance.AllPlayers[1].Data.GeneralData.PlayFabMasterId}");
                Log($"Mods: {recievedString}");
                //Log("onModStringRecieved Running");
                Delegate[] listeners = onModStringRecieved?.GetInvocationList();
                if (listeners != null)
                {
                    foreach (Delegate listener in listeners)
                    {
                        try
                        {
                            // Invoke each listener individually
                            listener.DynamicInvoke();
                        }
                        catch (Exception e)
                        {
                            logger.Msg("onModStringRecieved Error for Mod: " + listener.Target);
                            logger.Error(e.InnerException);
                            // The loop continues to the next listener even if one fails
                        }
                    }
                }
            }
        }

        #endregion

        #region Actions

        [HarmonyPatch(typeof(Il2CppRUMBLE.Environment.Matchmaking.MatchmakeConsole), "MatchmakeStatusUpdated", new Type[] { typeof(MatchmakingHandler.MatchmakeStatus) })]
        public static class MatchmakingType
        {
            private static void Prefix(GameObject __instance, MatchmakingHandler.MatchmakeStatus status)
            {
                if (status == MatchmakingHandler.MatchmakeStatus.Success)
                {
                    RumbleModdingAPI.matchmakingType = RMAPI.GameObjects.Gym.INTERACTABLES.MatchConsole.MatchmakingSettings.GetGameObject().transform.GetChild(0).GetChild(8).gameObject.GetComponent<InteractionSlider>().snappedStep;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerController), "Initialize", new Type[] { typeof(Il2CppRUMBLE.Players.Player) })]
        public static class playerspawn
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
                //Log("onAPlayerSpawned Running");
                Delegate[] listeners = onAPlayerSpawned?.GetInvocationList();
                if (listeners != null)
                {
                    foreach (Delegate listener in listeners)
                    {
                        try
                        {
                            // Invoke each listener individually
                            listener.DynamicInvoke(__instance.assignedPlayer);
                        }
                        catch (Exception e)
                        {
                            logger.Msg("onAPlayerSpawned Error for Mod: " + listener.Target);
                            logger.Error(e.InnerException);
                            // The loop continues to the next listener even if one fails
                        }
                    }
                }
                //Log("onPlayerSpawned Running");
                Delegate[] listeners2 = onPlayerSpawned?.GetInvocationList();
                if (listeners2 != null)
                {
                    foreach (Delegate listener in listeners2)
                    {
                        try
                        {
                            // Invoke each listener individually
                            listener.DynamicInvoke();
                        }
                        catch (Exception e)
                        {
                            logger.Msg("onPlayerSpawned Error for Mod: " + listener.Target);
                            logger.Error(e.InnerException);
                            // The loop continues to the next listener even if one fails
                        }
                    }
                }
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

        private IEnumerator HealthWatcher(int sceneNumber)
        {
            //Log("HealthWatcher Started");
            yield return new WaitForSeconds(3.2f);
            //Log("HealthWatcher Waited 3.2 Seconds");
            playerCount = PlayerManager.instance.AllPlayers.Count;
            healths = new int[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                healths[i] = PlayerManager.instance.AllPlayers[i].Data.HealthPoints;
            }
            waitForMatchStart = false;
            while (sceneNumber == sceneCount)
            {
                if (!waitForMatchStart)
                {
                    try
                    {
                        if (playerCount != PlayerManager.instance.AllPlayers.Count)
                        {
                            //Log("Player Count Changed: " + playerCount + " -> " + PlayerManager.instance.AllPlayers.Count);
                            playerCount = PlayerManager.instance.AllPlayers.Count;
                            if (matchStarted && (playerCount == 1) && ((currentScene == "Map0") || (currentScene == "Map1")))
                            {
                                //Log("Ending Match due to 1 Player");
                                //Log("onRoundEnded Running");
                                Delegate[] listeners = onRoundEnded?.GetInvocationList();
                                if (listeners != null)
                                {
                                    foreach (Delegate listener in listeners)
                                    {
                                        try
                                        {
                                            // Invoke each listener individually
                                            listener.DynamicInvoke();
                                        }
                                        catch (Exception e)
                                        {
                                            logger.Msg("onRoundEnded Error for Mod: " + listener.Target);
                                            logger.Error(e.InnerException);
                                            // The loop continues to the next listener even if one fails
                                        }
                                    }
                                }
                                //Log("onMatchEnded Running");
                                Delegate[] listeners2 = onMatchEnded?.GetInvocationList();
                                if (listeners2 != null)
                                {
                                    foreach (Delegate listener in listeners2)
                                    {
                                        try
                                        {
                                            // Invoke each listener individually
                                            listener.DynamicInvoke();
                                        }
                                        catch (Exception e)
                                        {
                                            logger.Msg("onMatchEnded Error for Mod: " + listener.Target);
                                            logger.Error(e.InnerException);
                                            // The loop continues to the next listener even if one fails
                                        }
                                    }
                                }
                                matchStarted = false;
                                break;
                            }
                            healths = new int[playerCount];
                            for (int i = 0; i < playerCount; i++)
                            {
                                healths[i] = PlayerManager.instance.AllPlayers[i].Data.HealthPoints;
                            }
                        }
                        for (int i = 0; i < playerCount; i++)
                        {
                            if (healths[i] != PlayerManager.instance.AllPlayers[i].Data.HealthPoints)
                            {
                                //Log("onPlayerHealthChanged Running (" + (PlayerManager.instance.AllPlayers[i].Data.HealthPoints - healths[i]) + ")");
                                Delegate[] listeners = onPlayerHealthChanged?.GetInvocationList();
                                if (listeners != null)
                                {
                                    foreach (Delegate listener in listeners)
                                    {
                                        try
                                        {
                                            // Invoke each listener individually
                                            listener.DynamicInvoke(PlayerManager.instance.AllPlayers[i], healths[i] - PlayerManager.instance.AllPlayers[i].Data.HealthPoints);
                                        }
                                        catch (Exception e)
                                        {
                                            logger.Msg("onLocalPlayerHealthChangedSpecific Error for Mod: " + listener.Target);
                                            logger.Error(e.InnerException);
                                            // The loop continues to the next listener even if one fails
                                        }
                                    }
                                }
                                if (i == 0)
                                {
                                    //Log("onLocalPlayerHealthChanged Running (" + (PlayerManager.instance.AllPlayers[i].Data.HealthPoints - healths[i]) + ")");
                                    Delegate[] listeners2 = onLocalPlayerHealthChanged?.GetInvocationList();
                                    if (listeners2 != null)
                                    {
                                        foreach (Delegate listener in listeners2)
                                        {
                                            try
                                            {
                                                // Invoke each listener individually
                                                listener.DynamicInvoke();
                                            }
                                            catch (Exception e)
                                            {
                                                logger.Msg("onLocalPlayerHealthChanged Error for Mod: " + listener.Target);
                                                logger.Error(e.InnerException);
                                                // The loop continues to the next listener even if one fails
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //Log("onRemotePlayerHealthChanged Running (Player Spot " + i + ") (" + (PlayerManager.instance.AllPlayers[i].Data.HealthPoints - healths[i]) + ")");
                                    Delegate[] listeners2 = onRemotePlayerHealthChanged?.GetInvocationList();
                                    if (listeners2 != null)
                                    {
                                        foreach (Delegate listener in listeners2)
                                        {
                                            try
                                            {
                                                // Invoke each listener individually
                                                listener.DynamicInvoke();
                                            }
                                            catch (Exception e)
                                            {
                                                logger.Msg("onRemotePlayerHealthChanged Error for Mod: " + listener.Target);
                                                logger.Error(e.InnerException);
                                                // The loop continues to the next listener even if one fails
                                            }
                                        }
                                    }
                                }
                                healths[i] = PlayerManager.instance.AllPlayers[i].Data.HealthPoints;
                                if (((currentScene == "Map0") || (currentScene == "Map1")) && (healths[i] <= 0) && !waitForMatchStart)
                                {
                                    if (PlayerManager.instance.AllPlayers.Count > 1)
                                    {
                                        //Log("onRoundEnded Running (HP hit 0)");
                                        Delegate[] listeners2 = onRoundEnded?.GetInvocationList();
                                        if (listeners2 != null)
                                        {
                                            foreach (Delegate listener in listeners2)
                                            {
                                                try
                                                {
                                                    // Invoke each listener individually
                                                    listener.DynamicInvoke();
                                                }
                                                catch (Exception e)
                                                {
                                                    logger.Msg("onRoundEnded Error for Mod: " + listener.Target);
                                                    logger.Error(e.InnerException);
                                                    // The loop continues to the next listener even if one fails
                                                }
                                            }
                                        }
                                    }
                                    MelonCoroutines.Start(WaitForRoundStart(i, sceneNumber));
                                }
                            }
                        }
                    }
                    catch (Exception e) { /*logger.Error(e);*/ }
                }
                yield return new WaitForFixedUpdate();
            }
            //Log("HealthWatcher Ending, Scene Changed");
            yield break;
        }

        private IEnumerator<WaitForSeconds> WaitForRoundStart(int playerNumber, int sceneNumber)
        {
            //Log("Waiting for Round to Start");
            yield return new WaitForSeconds(0.5f);
            //Log("WaitForSeconds(0.5f) Finished");
            waitForMatchStart = true;
            bool matchEnded = false;
            while (waitForMatchStart && (sceneCount == sceneNumber) && (playerNumber < PlayerManager.instance.AllPlayers.Count))
            {
                try
                {
                    if (PlayerManager.instance.AllPlayers[playerNumber].Data.HealthPoints == 20)
                    {
                        for (int i = 0; i < playerCount; i++)
                        {
                            healths[i] = PlayerManager.instance.AllPlayers[i].Data.HealthPoints;
                        }
                        waitForMatchStart = false;
                        if ((PlayerManager.instance.AllPlayers.Count > 1) && ((currentScene == "Map0") || (currentScene == "Map1")))
                        {
                            //Log("onRoundStarted Running (Normal Round Start Mid Match)");
                            Delegate[] listeners = onRoundStarted?.GetInvocationList();
                            if (listeners != null)
                            {
                                foreach (Delegate listener in listeners)
                                {
                                    try
                                    {
                                        // Invoke each listener individually
                                        listener.DynamicInvoke();
                                    }
                                    catch (Exception e)
                                    {
                                        logger.Msg("onRoundStarted Error for Mod: " + listener.Target);
                                        logger.Error(e.InnerException);
                                        // The loop continues to the next listener even if one fails
                                    }
                                }
                            }
                        }
                    }
                    if ((PlayerManager.instance.AllPlayers.Count > 1) && ((currentScene == "Map0") || (currentScene == "Map1")) && !matchEnded && (matchSlab.activeSelf))
                    {
                        matchEnded = true;
                        matchStarted = false;
                        //Log("onMatchEnded Running (Normal End)");
                        Delegate[] listeners = onMatchEnded?.GetInvocationList();
                        if (listeners != null)
                        {
                            foreach (Delegate listener in listeners)
                            {
                                try
                                {
                                    // Invoke each listener individually
                                    listener.DynamicInvoke();
                                }
                                catch (Exception e)
                                {
                                    logger.Msg("onMatchEnded Error for Mod: " + listener.Target);
                                    logger.Error(e.InnerException);
                                    // The loop continues to the next listener even if one fails
                                }
                            }
                        }
                    }
                }
                catch { }
                yield return new WaitForSeconds(0.02f);
            }
            //Log("WaitForRoundStart Finished");
            yield break;
        }

        #endregion



        #region API Calls

        public static bool IsInitialized() { return init; }

        public static bool IsMapInitialized() { return mapInit; }


        public class Matchmaking
        {
            public static string getMatchmakingTypeAsString()
            {
                if ((currentScene != "Map0") && (currentScene != "Map1"))
                {
                    return "NULL";
                }
                string returnText = "";
                switch (matchmakingType)
                {
                    case 0:
                        returnText = "Any Rank";
                        break;
                    case 1:
                        returnText = "Friends Only";
                        break;
                }
                return returnText;
            }

            public static int getMatchmakingTypeAsInt()
            {
                if ((currentScene != "Map0") && (currentScene != "Map1"))
                {
                    return -1;
                }
                return matchmakingType;
            }
        }

        public class Mods
        {
            public static string getMyModString() { return myModString; }

            public static string getOpponentModString() { return opponentModString; }

            public static List<ModInfo> getMyMods() { return myMods; }

            public static List<ModInfo> getOpponentMods() { return opponentMods; }

            public static bool doesOpponentHaveMod(string modName, string ModVersion, bool matchVersion = true)
            {
                for (int i = 0; i < opponentMods.Count; i++)
                {
                    if (modName == opponentMods[i].ModName)
                    {
                        if (matchVersion)
                        {
                            return ModVersion == opponentMods[i].ModVersion;
                        }
                        return true;
                    }
                }
                return false;
            }

            public static bool findOwnMod(string modName, string ModVersion, bool matchVersion = true)
            {
                for (int i = 0; i < myMods.Count; i++)
                {
                    if (modName == myMods[i].ModName)
                    {
                        if (matchVersion)
                        {
                            return ModVersion == myMods[i].ModVersion;
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        public class ControllerMap
        {
            public class RightController
            {
                public static float GetTrigger()
                {
                    return rightTrigger.ReadValue<float>();
                }

                public static float GetGrip()
                {
                    return rightGrip.ReadValue<float>();
                }

                public static float GetPrimary()
                {
                    return rightPrimary.ReadValue<float>();
                }

                public static float GetSecondary()
                {
                    return rightSecondary.ReadValue<float>();
                }

                public static Vector2 GetJoystick()
                {
                    return rightJoystick.ReadValue<Vector2>();
                }

                public static float GetJoystickClick()
                {
                    return rightJoystickClick.ReadValue<float>();
                }
            }

            public class LeftController
            {
                public static float GetTrigger()
                {
                    return leftTrigger.ReadValue<float>();
                }

                public static float GetGrip()
                {
                    return leftGrip.ReadValue<float>();
                }

                public static float GetPrimary()
                {
                    return leftPrimary.ReadValue<float>();
                }

                public static float GetSecondary()
                {
                    return leftSecondary.ReadValue<float>();
                }

                public static Vector2 GetJoystick()
                {
                    return leftJoystick.ReadValue<Vector2>();
                }

                public static float GetJoystickClick()
                {
                    return leftJoystickClick.ReadValue<float>();
                }
            }
        }


        public class Scene
        {

            public static DateTime WhenSceneChanged() { return whenSceneChanged; }

            public static string GetSceneName() { return currentScene; }

            public static string GetLastSceneName() { return lastScene; }
        }

        public class Pools
        {

            public class Structures
            {
                public static GameObject GetPoolDisc() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolDiscRUMBLEMoveSystemStructure.GetGameObject(); }

                public static GameObject GetPoolPillar() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolPillarRUMBLEMoveSystemStructure.GetGameObject(); }

                public static GameObject GetPoolBall() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolBallRUMBLEMoveSystemStructure.GetGameObject(); }

                public static GameObject GetPoolCube() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolRockCubeRUMBLEMoveSystemStructure.GetGameObject(); }

                public static GameObject GetPoolWall() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolWallRUMBLEMoveSystemStructure.GetGameObject(); }

                public static GameObject GetPoolSmallRock() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolSmallRockRUMBLEMoveSystemStructure.GetGameObject(); }

                public static GameObject GetPoolLargeRock() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolLargeRockRUMBLEMoveSystemStructure.GetGameObject(); }

                public static GameObject GetPoolBoulderBall() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolBoulderBallRUMBLEMoveSystemStructure.GetGameObject(); }
            }

            public class ShiftStones
            {
                public static GameObject GetPoolVolatileStone() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolVolatileStoneRUMBLECombatShiftStonesVolatileStone.GetGameObject(); }

                public static GameObject GetPoolChargeStone() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolChargeStoneRUMBLECombatShiftStonesChargeStone.GetGameObject(); }

                public static GameObject GetPoolSurgeStone() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolSurgeStoneRUMBLECombatShiftStonesCounterStone.GetGameObject(); }

                public static GameObject GetPoolFlowStone() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolFlowStoneRUMBLECombatShiftStonesFlowStone.GetGameObject(); }

                public static GameObject GetPoolGuardStone() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolGuardStoneRUMBLECombatShiftStonesGuardStone.GetGameObject(); }

                public static GameObject GetPoolStubbornStone() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolStubbornStoneRUMBLECombatShiftStonesStubbornStone.GetGameObject(); }

                public static GameObject GetPoolAdamantStone() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolAdamantStoneRUMBLECombatShiftStonesUnyieldingStone.GetGameObject(); }

                public static GameObject GetPoolVigorStone() { return RMAPI.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.PoolVigorStoneRUMBLECombatShiftStonesVigorStone.GetGameObject(); }
            }
        }

        public class Players
        {
            public static bool IsHost() { return PhotonNetwork.IsMasterClient; }

            public static Il2CppSystem.Collections.Generic.List<Il2CppRUMBLE.Players.Player> GetAllPlayers() { return PlayerManager.instance.AllPlayers; }

            public static Il2CppRUMBLE.Players.Player GetLocalPlayer() { return PlayerManager.instance.localPlayer; }
            
            public static PlayerController GetLocalPlayerController() { return PlayerManager.instance.localPlayer.Controller; }

            public static System.Collections.Generic.List<Il2CppRUMBLE.Players.Player> GetEnemyPlayers()
            {
                System.Collections.Generic.List<Il2CppRUMBLE.Players.Player> enemies = new System.Collections.Generic.List<Il2CppRUMBLE.Players.Player>();
                for (int i = 1; i < PlayerManager.instance.AllPlayers.Count; i++) { enemies.Add(PlayerManager.instance.AllPlayers[i]); }
                return enemies;
            }

            public static Il2CppRUMBLE.Players.Player GetClosestPlayer(UnityEngine.Vector3 pos, bool ignoreLocalController) { return PlayerManager.instance.GetClosestPlayer(pos, ignoreLocalController); }

            public static Il2CppRUMBLE.Players.Player GetClosestPlayer(UnityEngine.Vector3 pos, PlayerController ignoreController) { return PlayerManager.instance.GetClosestPlayer(pos, ignoreController); }

            public static Il2CppRUMBLE.Players.Player GetPlayerByControllerType(Il2CppRUMBLE.Players.ControllerType controllerType) { return PlayerManager.instance.GetPlayer(controllerType); }

            public static Il2CppRUMBLE.Players.Player GetPlayerByActorNo(int actorNumber) { return PlayerManager.instance.GetPlayerByActorNo(actorNumber); }

            public static System.Collections.Generic.List<Il2CppRUMBLE.Players.Player> GetPlayersInActorNoOrder()
            {
                bool spotFound = false;
                System.Collections.Generic.List<Il2CppRUMBLE.Players.Player> players = new System.Collections.Generic.List<Il2CppRUMBLE.Players.Player>();
                for (int i = 1; i < PlayerManager.instance.AllPlayers.Count; i++)
                {
                    if ((!spotFound) && (PlayerManager.instance.AllPlayers[0].Data.GeneralData.ActorNo < PlayerManager.instance.AllPlayers[i].Data.GeneralData.ActorNo))
                    {
                        players.Add(PlayerManager.instance.AllPlayers[0]);
                        spotFound = true;
                    }
                    players.Add(PlayerManager.instance.AllPlayers[i]);
                }
                return players;
            }
        }
        #endregion

    }
}
