using Il2CppBhaptics.SDK2;
using MelonLoader;
using Il2CppPhoton.Pun;
using Il2CppRUMBLE.Combat;
using Il2CppRUMBLE.Combat.ShiftStones.UI;
using Il2CppRUMBLE.Environment;
using Il2CppRUMBLE.Environment.Howard;
using Il2CppRUMBLE.Environment.Matchmaking;
using Il2CppRUMBLE.Interactions.InteractionBase;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.MoveSystem;
using Il2CppRUMBLE.Players;
using Il2CppRUMBLE.Economy;
using Il2CppRUMBLE.Social;
using Il2CppRUMBLE.Social.Phone;
using Il2CppRUMBLE.Tutorial.MoveLearning;
using Il2CppRUMBLE.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Il2CppPhoton.Realtime;
using Il2CppExitGames.Client.Photon;
using HarmonyLib;
using Il2CppRUMBLE.Slabs.Forms;
using Il2CppRUMBLE.Players.Subsystems;
using static MelonLoader.MelonLogger;

namespace RumbleModdingAPI
{
    public static class ModBuildInfo
    {
        public const string Version = "3.3.7";
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

    public class Calls : MelonMod
    {
        #region Variables
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
        private static System.Collections.Generic.List<GameObject> allBaseDDOLGameObjects = new System.Collections.Generic.List<GameObject>();
        private static System.Collections.Generic.List<GameObject> allBaseGymGameObjects = new System.Collections.Generic.List<GameObject>();
        private static System.Collections.Generic.List<GameObject> allBaseParkGameObjects = new System.Collections.Generic.List<GameObject>();
        private static System.Collections.Generic.List<GameObject> allBaseMap0GameObjects = new System.Collections.Generic.List<GameObject>();
        private static System.Collections.Generic.List<GameObject> allBaseMap1GameObjects = new System.Collections.Generic.List<GameObject>();
        private static Game gameManager;
        private static NetworkManager networkManager;
        private static PlayerManager playerManager;
        private static Il2CppRUMBLE.Input.InputManager inputManager;
        private static SceneManager sceneManager;
        private static NotificationManager notificationManager;
        private static StackManager stackManager;
        private static QualityManager qualityManager;
        private static SocialHandler socialHandler;
        private static SlabManager slabManager;
        private static RecordingCamera recordingCamera;
        private static CombatManager combatManager;
        private static PoolManager poolManager;
        private static BhapticsSDK2 bHapticsManager;
        private static PhotonHandler photonHandler;
        private static AudioManager audioManager;
        private static GameObject uIGameObject;
        private static MailTube mailTube;
        private static MatchmakeConsole matchConsole;
        private static GameObject regionSelectorGameObject;
        private static BeltRack beltRack;
        private static PhoneHandler gymFriendBoard;
        private static ParkBoard parkBoardBasicGymVariant;
        private static ParkBoardGymVariant parkBoardGymVariant;
        private static Howard howard;
        private static MoveLearnHandler poseGhostHandler;
        private static Leaderboard dailyLeaderboard;
        private static GameObject rankStatusSlabGameObject;
        private static GameObject communitySlabGameObject;
        private static ShiftstoneQuickswapper gymShiftstoneQuickswapper;
        private static ShiftstoneCabinet shiftstoneCabinet;
        private static GameObject gymGondolaGameObject;
        private static GameObject ranksGameObject;
        private static Il2CppRUMBLE.Players.SpawnPointHandler gymSpawnPointHandler;
        private static MatchmakingHandler matchmakingHandler;
        private static Il2CppRUMBLE.Players.SpawnPointHandler parkSpawnPointHandler;
        private static PhoneHandler parkFriendBoard;
        private static ParkBoard parkBoardBasicParkVariant;
        private static ParkBoardParkVariant parkBoardParkVariant;
        private static ShiftstoneQuickswapper parkShiftstoneQuickswapper;
        private static ParkInstance parkInstance;
        private static GameObject localHealthbarGameObject;
        public static event System.Action onModStringRecieved;
        public static event System.Action onMyModsGathered;
        public static event System.Action onMapInitialized;
        public static event System.Action onPlayerSpawned;
        public static event System.Action onMatchStarted;
        public static event System.Action onMatchEnded;
        public static event System.Action onRoundStarted;
        public static event System.Action onRoundEnded;
        public static event System.Action onLocalPlayerHealthChanged;
        public static event System.Action onRemotePlayerHealthChanged;
        private static GameObject newTextGameObject;
        private static GameObject newButtonGameObject;
        private static GameObject parentAPIItems;

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

        private static GameObject pokeBalls, catEars;

        private int sceneCount = 0;
        private int[] healths;
        private int playerCount = 0;
        private bool waitForMatchStart = false;
        private GameObject matchSlab;
        private bool matchStarted = false;
        public static int matchmakingType = 0;

        #endregion

        #region API Initialization

        public static void Log(string msg)
        {
            MelonLogger.Msg(msg);
        }

        public override void OnLateInitializeMelon()
        {
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
            pokeBalls = LoadAssetBundle("RumbleModdingAPI.pokeballs", "Pokeball");
            catEars = LoadAssetBundle("RumbleModdingAPI.catears", "Ears");
            GameObject.DontDestroyOnLoad(pokeBalls);
            GameObject.DontDestroyOnLoad(catEars);
            pokeBalls.SetActive(false);
            catEars.SetActive(false);
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
                            allBaseDDOLGameObjects.Add(GameObject.Find("[bHaptics]"));
                            allBaseDDOLGameObjects.Add(GameObject.Find("LanguageManager"));
                            allBaseDDOLGameObjects.Add(GameObject.Find("PhotonMono"));
                            allBaseDDOLGameObjects.Add(GameObject.Find("Game Instance"));
                            allBaseDDOLGameObjects.Add(GameObject.Find("Timer Updater"));
                            bHapticsManager = BhapticsSDK2.instance;
                            photonHandler = PhotonHandler.instance;
                            gameManager = Game.instance;
                            audioManager = AudioManager.instance;
                            poolManager = PoolManager.instance;
                            networkManager = NetworkManager.instance;
                            playerManager = PlayerManager.instance;
                            inputManager = Il2CppRUMBLE.Input.InputManager.instance;
                            sceneManager = SceneManager.instance;
                            notificationManager = NotificationManager.instance;
                            stackManager = StackManager.instance;
                            qualityManager = QualityManager.instance;
                            socialHandler = SocialHandler.instance;
                            slabManager = SlabManager.instance;
                            recordingCamera = RecordingCamera.instance;
                            combatManager = CombatManager.instance;
                            uIGameObject = GameObjects.DDOL.GameInstance.UI.GetGameObject();
                            init = true;
                            Log("API Initialized");
                            Log("API By UlvakSkillz. Consider Donating to Their KoFi: https://ko-fi.com/ulvakskillz");
                            onMyModsGathered?.Invoke();
                        }
                    }
                    else if ((currentScene == "Gym") && (!mapInit))
                    {
                        allBaseGymGameObjects.Clear();
                        allBaseGymGameObjects.Add(GameObject.Find("!ftraceLightmaps"));
                        allBaseGymGameObjects.Add(GameObject.Find("------------TUTORIAL------------"));
                        allBaseGymGameObjects.Add(GameObject.Find("--------------SCENE--------------"));
                        allBaseGymGameObjects.Add(GameObject.Find("--------------LOGIC--------------"));
                        matchConsole = GameObjects.Gym.Logic.HeinhouserProducts.MatchConsole.GetGameObject().GetComponent<MatchmakeConsole>();
                        regionSelectorGameObject = GameObjects.Gym.Logic.HeinhouserProducts.RegionSelector.GetGameObject();
                        beltRack = GameObjects.Gym.Logic.HeinhouserProducts.BeltRack.GetGameObject().GetComponent<BeltRack>();
                        gymFriendBoard = GameObjects.Gym.Logic.HeinhouserProducts.Telephone.GetGameObject().GetComponent<PhoneHandler>();
                        parkBoardBasicGymVariant = GameObjects.Gym.Logic.HeinhouserProducts.Parkboard.GetGameObject().GetComponent<ParkBoard>();
                        parkBoardGymVariant = GameObjects.Gym.Logic.HeinhouserProducts.Parkboard.GetGameObject().GetComponent<ParkBoardGymVariant>();
                        howard = GameObjects.Gym.Logic.HeinhouserProducts.HowardRoot.GetGameObject().GetComponent<Howard>();
                        poseGhostHandler = MoveLearnHandler.instance;
                        dailyLeaderboard = GameObjects.Gym.Logic.HeinhouserProducts.Leaderboard.GetGameObject().GetComponent<Leaderboard>();
                        rankStatusSlabGameObject = GameObjects.Gym.Logic.HeinhouserProducts.RankStatusSlab.GetGameObject();
                        communitySlabGameObject = GameObjects.Gym.Logic.HeinhouserProducts.CommunitySlab.GetGameObject();
                        gymShiftstoneQuickswapper = GameObjects.Gym.Logic.HeinhouserProducts.ShiftstoneQuickswapper.GetGameObject().GetComponent<ShiftstoneQuickswapper>();
                        shiftstoneCabinet = GameObjects.Gym.Logic.HeinhouserProducts.ShiftstoneCabinet.GetGameObject().GetComponent<ShiftstoneCabinet>();
                        gymGondolaGameObject = GameObjects.Gym.Logic.HeinhouserProducts.Gondola.GetGameObject();
                        ranksGameObject = GameObjects.Gym.Logic.HeinhouserProducts.RankBoard.GetGameObject();
                        gymSpawnPointHandler = GameObjects.Gym.Logic.Handlers.SpawnPointHandler.GetGameObject().GetComponent<Il2CppRUMBLE.Players.SpawnPointHandler>();
                        matchmakingHandler = MatchmakingHandler.instance;
                        if (parentAPIItems == null)
                        {
                            PhotonNetwork.NetworkingClient.EventReceived += (Action<EventData>)OnEvent;
                            parentAPIItems = new GameObject();
                            parentAPIItems.name = "APIItems";
                            GameObject.DontDestroyOnLoad(parentAPIItems);
                            pokeBalls.transform.parent = parentAPIItems.transform;
                            catEars.transform.parent = parentAPIItems.transform;
                        }
                        if (newTextGameObject == null)
                        {
                            newTextGameObject = GameObject.Instantiate(GameObjects.Gym.Logic.HeinhouserProducts.Leaderboard.PlayerTags.HighscoreTag0.Nr.GetGameObject());
                            TextMeshPro tmp = newTextGameObject.GetComponent<TextMeshPro>();
                            newTextGameObject.name = "NewTextGameObject";
                            tmp.text = "new Text";
                            tmp.color = Color.black;
                            newTextGameObject.SetActive(false);
                            newTextGameObject.transform.parent = parentAPIItems.transform;
                        }
                        if (newButtonGameObject == null)
                        {
                            newButtonGameObject = GameObject.Instantiate(GameObjects.Gym.Tutorial.StaticTutorials.RUMBLEStarterGuide.NextPageButton.InteractionButton.GetGameObject());
                            newButtonGameObject.name = "newButton";
                            newButtonGameObject.SetActive(false);
                            newButtonGameObject.transform.parent = parentAPIItems.transform;
                        }
                        mapInit = true;
                    }
                    else if ((currentScene == "Park") && (!mapInit))
                    {
                        allBaseParkGameObjects.Clear();
                        allBaseParkGameObjects.Add(GameObject.Find("________________LOGIC__________________ "));
                        allBaseParkGameObjects.Add(GameObject.Find("!ftraceLightmaps"));
                        allBaseParkGameObjects.Add(GameObject.Find("________________SCENE_________________"));
                        allBaseParkGameObjects.Add(GameObject.Find("ParkBoardP1ScoreVFX"));
                        allBaseParkGameObjects.Add(GameObject.Find("Lighting and effects"));
                        allBaseParkGameObjects.Add(GameObject.Find("VoiceLogger"));
                        parkSpawnPointHandler = GameObjects.Park.Logic.SpawnPointHandler.GetGameObject().GetComponent<Il2CppRUMBLE.Players.SpawnPointHandler>();
                        parkInstance = GameObjects.Park.Logic.ParkInstance.GetGameObject().GetComponent<ParkInstance>();
                        parkFriendBoard = GameObjects.Park.Logic.HeinhouserProducts.Telephone.GetGameObject().GetComponent<PhoneHandler>();
                        parkBoardBasicParkVariant = GameObjects.Park.Logic.HeinhouserProducts.Parkboard.GetGameObject().GetComponent<ParkBoard>();
                        parkBoardParkVariant = GameObjects.Park.Logic.HeinhouserProducts.Parkboard.GetGameObject().GetComponent<ParkBoardParkVariant>();
                        parkShiftstoneQuickswapper = GameObjects.Park.Logic.ShiftstoneQuickswapper.GetGameObject().GetComponent<ShiftstoneQuickswapper>();
                        mapInit = true;
                    }
                    else if ((currentScene == "Map0") && (!mapInit))
                    {
                        allBaseMap0GameObjects.Clear();
                        allBaseMap0GameObjects.Add(GameObject.Find("SceneProcessor"));
                        allBaseMap0GameObjects.Add(GameObject.Find("Logic"));
                        allBaseMap0GameObjects.Add(GameObject.Find("!ftraceLightmaps"));
                        allBaseMap0GameObjects.Add(GameObject.Find("Lighting & Effects"));
                        allBaseMap0GameObjects.Add(GameObject.Find("Map0_production"));
                        allBaseMap0GameObjects.Add(GameObject.Find("VoiceLogger"));
                        mapInit = true;
                    }
                    else if ((currentScene == "Map1") && (!mapInit))
                    {
                        allBaseMap1GameObjects.Clear();
                        allBaseMap1GameObjects.Add(GameObject.Find("!ftraceLightmaps"));
                        allBaseMap1GameObjects.Add(GameObject.Find("Lighting & Effects"));
                        allBaseMap1GameObjects.Add(GameObject.Find("Logic"));
                        allBaseMap1GameObjects.Add(GameObject.Find("Map1_production"));
                        allBaseMap1GameObjects.Add(GameObject.Find("VoiceLogger"));
                        mapInit = true;
                    }
                    if (mapInit)
                    {
                        MelonCoroutines.Start(GetHealth());
                    }
                }
                catch (Exception e) { MelonLogger.Error(e); return; }
                sceneChanged = false;
            }
        }

        private IEnumerator GetHealth()
        {
            bool gotHealth = false;
            while (!gotHealth)
            {
                localHealthbarGameObject = GameObject.Find("/Health");
                if (localHealthbarGameObject != null)
                {
                    gotHealth = true;
                    yield return new WaitForFixedUpdate();
                    try
                    {
                        onMapInitialized?.Invoke();
                        GetMods();
                        if (PlayerManager.instance.AllPlayers.Count > 1)
                        {
                            if (currentScene == "Map0")
                            {
                                matchSlab = Calls.GameObjects.Map0.Logic.MatchSlabOne.MatchSlab.SlabBuddyMatchVariant.MatchForm.MatchFormCanvas.GetGameObject();
                            }
                            else if (currentScene == "Map1")
                            {
                                matchSlab = Calls.GameObjects.Map1.Logic.MatchSlabOne.MatchSlab.SlabBuddyMatchVariant.MatchForm.MatchFormCanvas.GetGameObject();
                            }
                            matchStarted = true;
                            if ((currentScene == "Map0") || (currentScene == "Map1"))
                            {
                                onMatchStarted?.Invoke();
                                onRoundStarted?.Invoke();
                            }
                        }
                        MelonCoroutines.Start(HealthWatcher(sceneCount));
                    }
                    catch (Exception e)
                    {
                        MelonLogger.Error(e);
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
                onModStringRecieved?.Invoke();
            }
        }

        #endregion

        #region Actions

        [HarmonyPatch(typeof(Il2CppRUMBLE.Environment.Matchmaking.MatchmakeConsole), "MatchmakeStatusUpdated", new Type[] { typeof(MatchmakingHandler.MatchmakeStatus), typeof(bool) })]
        public static class MatchmakingType
        {
            private static void Prefix(GameObject __instance, MatchmakingHandler.MatchmakeStatus status, bool instantLeverStep)
            {
                if (status == MatchmakingHandler.MatchmakeStatus.Success)
                {
                    Calls.matchmakingType = GameObjects.Gym.Logic.HeinhouserProducts.MatchConsole.RankRelaxControls.GetGameObject().transform.GetChild(8).gameObject.GetComponent<InteractionSlider>().snappedStep;
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
                    try
                    {
                        if (player.Controller.controllerType != ControllerType.Local)
                        {
                            GameObject cat = GameObject.Instantiate(catEars);
                            cat.transform.parent = __instance.gameObject.transform.FindChild("Visuals/Skelington/Bone_Pelvis/Bone_Spine_A/Bone_Chest/Bone_Neck/Bone_Head");
                            cat.transform.localPosition = new Vector3(0, 0.15f, 0);
                            cat.transform.localRotation = Quaternion.Euler(270, 0, 0);
                            cat.transform.localScale = new Vector3(50, 50, 50);
                            cat.SetActive(true);
                        }
                        else
                        {
                            MelonCoroutines.Start(SetDressingRoomObjects());
                        }
                    }
                    catch
                    {
                        GameObject cat = GameObject.Instantiate(catEars);
                        cat.transform.parent = __instance.gameObject.transform.FindChild("Visuals/Skelington/Bone_Pelvis/Bone_Spine_A/Bone_Chest/Bone_Neck/Bone_Head");
                        cat.transform.localPosition = new Vector3(0, 0.15f, 0);
                        cat.transform.localRotation = Quaternion.Euler(270, 0, 0);
                        cat.transform.localScale = new Vector3(50, 50, 50);
                        cat.SetActive(true);
                    }
                    GameObject poke = GameObject.Instantiate(pokeBalls);
                    poke.transform.parent = __instance.gameObject.transform.FindChild("Visuals/Skelington/Bone_Pelvis/Bone_Spine_A");
                    poke.transform.localPosition = new Vector3(-0.01f, 0, 0);
                    poke.transform.localRotation = Quaternion.Euler(0.4877f, 359.2524f, 8.7574f);
                    poke.transform.localScale = new Vector3(0.9128f, 0.9128f, 0.9128f);
                    poke.SetActive(true);
                    if (PlayerManager.instance.localPlayer != player)
                    {
                        MelonCoroutines.Start(WaitForTitleLoad(player));
                    }
                }
                onPlayerSpawned?.Invoke();
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

        private static bool dressingRoomObjectsCreated = false;
        private static IEnumerator SetDressingRoomObjects()
        {
            yield return new WaitForSeconds(1);
            if (currentScene != "Gym") { yield break; }
            if (dressingRoomObjectsCreated)
            {
                yield break;
            }
            try
            {
                GameObject dressingRoomCat = GameObject.Instantiate(catEars);
                dressingRoomCat.transform.parent = Calls.GameObjects.Gym.Scene.GymProduction.DressingRoom.PreviewPlayerController.GetGameObject().transform.FindChild("Visuals/Skelington/Bone_Pelvis/Bone_Spine_A/Bone_Chest/Bone_Neck/Bone_Head");
                dressingRoomCat.transform.localPosition = new Vector3(0, 0.15f, 0);
                dressingRoomCat.transform.localRotation = Quaternion.Euler(270, 0, 0);
                dressingRoomCat.transform.localScale = new Vector3(50, 50, 50);
                dressingRoomCat.SetActive(true);
                GameObject dressingRoomPoke = GameObject.Instantiate(pokeBalls);
                dressingRoomPoke.transform.parent = Calls.GameObjects.Gym.Scene.GymProduction.DressingRoom.PreviewPlayerController.GetGameObject().transform.FindChild("Visuals/Skelington/Bone_Pelvis/Bone_Spine_A");
                dressingRoomPoke.transform.localPosition = new Vector3(-0.01f, 0, 0);
                dressingRoomPoke.transform.localRotation = Quaternion.Euler(0.4877f, 359.2524f, 8.7574f);
                dressingRoomPoke.transform.localScale = new Vector3(0.9128f, 0.9128f, 0.9128f);
                dressingRoomPoke.SetActive(true);
                dressingRoomObjectsCreated = true;
            }
            catch (Exception e) { MelonLogger.Error(e); }
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
            yield return new WaitForSeconds(3f);
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
                            playerCount = PlayerManager.instance.AllPlayers.Count;
                            if (matchStarted && (playerCount == 1) && ((currentScene == "Map0") || (currentScene == "Map1")))
                            {
                                onRoundEnded?.Invoke();
                                onMatchEnded?.Invoke();
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
                                if (i == 0)
                                {
                                    onLocalPlayerHealthChanged?.Invoke();
                                }
                                else
                                {
                                    onRemotePlayerHealthChanged?.Invoke();
                                }
                                healths[i] = PlayerManager.instance.AllPlayers[i].Data.HealthPoints;
                                if (((currentScene == "Map0") || (currentScene == "Map1")) && (healths[i] <= 0) && !waitForMatchStart)
                                {
                                    if (PlayerManager.instance.AllPlayers.Count > 1)
                                    {
                                        onRoundEnded?.Invoke();
                                    }
                                    MelonCoroutines.Start(WaitForRoundStart(i, sceneNumber));
                                }
                            }
                        }
                    }
                    catch { }
                }
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        private IEnumerator WaitForRoundStart(int playerNumber, int sceneNumber)
        {
            yield return new WaitForSeconds(0.5f);
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
                            onRoundStarted?.Invoke();
                        }
                    }
                    if ((PlayerManager.instance.AllPlayers.Count > 1) && ((currentScene == "Map0") || (currentScene == "Map1")) && !matchEnded && (matchSlab.active))
                    {
                        matchEnded = true;
                        matchStarted = false;
                        onMatchEnded?.Invoke();
                    }
                }
                catch { }
                yield return new WaitForFixedUpdate();
            }
            yield break;
        }

        #endregion

        #region Asset Bundle

        public GameObject LoadAssetBundle(string bundleName, string objectName)
        {
            using (System.IO.Stream bundleStream = MelonAssembly.Assembly.GetManifestResourceStream(bundleName))
            {
                byte[] bundleBytes = new byte[bundleStream.Length];
                bundleStream.Read(bundleBytes, 0, bundleBytes.Length);
                Il2CppAssetBundle bundle = Il2CppAssetBundleManager.LoadFromMemory(bundleBytes);
                return UnityEngine.Object.Instantiate(bundle.LoadAsset<GameObject>(objectName));
            }
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
                        returnText = "Same Rank";
                        break;
                    case 2:
                        returnText = "1 Rank Difference";
                        break;
                    case 3:
                        returnText = "2 Ranks Difference";
                        break;
                    case 4:
                        returnText = "3 Ranks Difference";
                        break;
                    case 5:
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

        public class Create
        {
            public static GameObject NewText()
            {
                GameObject newTextGO = GameObject.Instantiate(newTextGameObject);
                newTextGO.SetActive(true);
                newTextGO.GetComponent<TextMeshPro>().autoSizeTextContainer = true;
                return newTextGO;
            }

            public static GameObject NewText(string text, float textSize, Color textColor, UnityEngine.Vector3 textPosition, Quaternion textRotation)
            {
                GameObject newTextGO = GameObject.Instantiate(newTextGameObject);
                newTextGO.SetActive(true);
                TextMeshPro tmp = newTextGO.GetComponent<TextMeshPro>();
                tmp.text = text;
                tmp.fontSize = textSize;
                tmp.color = textColor;
                tmp.autoSizeTextContainer = true;
                newTextGameObject.transform.position = textPosition;
                newTextGameObject.transform.rotation = textRotation;
                return newTextGO;
            }

            public static GameObject NewButton()
            {
                GameObject newButtonGO = GameObject.Instantiate(newButtonGameObject);
                newButtonGO.SetActive(true);
                return newButtonGO;
            }

            public static GameObject NewButton(UnityEngine.Vector3 buttonPosition, Quaternion buttonRotation)
            {
                GameObject newButtonGO = GameObject.Instantiate(newButtonGameObject);
                newButtonGO.transform.position = buttonPosition;
                newButtonGO.transform.rotation = buttonRotation;
                newButtonGO.SetActive(true);
                return newButtonGO;
            }

            public static GameObject NewButton(Action action)
            {
                GameObject newButtonGO = GameObject.Instantiate(newButtonGameObject);
                newButtonGO.SetActive(true);
                newButtonGO.transform.GetChild(0).gameObject.GetComponent<InteractionButton>().onPressed.AddListener(action);
                return newButtonGO;
            }

            public static GameObject NewButton(UnityEngine.Vector3 buttonPosition, Quaternion buttonRotation, Action action)
            {
                GameObject newButtonGO = GameObject.Instantiate(newButtonGameObject);
                newButtonGO.transform.position = buttonPosition;
                newButtonGO.transform.rotation = buttonRotation;
                newButtonGO.SetActive(true);
                newButtonGO.transform.GetChild(0).gameObject.GetComponent<InteractionButton>().onPressed.AddListener(action);
                return newButtonGO;
            }
        }

        public class GameObjects
        {
            public class DDOL
            {
                public static System.Collections.Generic.List<GameObject> GetBaseDDOLGameObjects() { return allBaseDDOLGameObjects; }

                public class BHaptics
                {
                    public static GameObject GetGameObject() { return allBaseDDOLGameObjects[0]; }
                }

                public class LanguageManager
                {
                    public static GameObject GetGameObject() { return allBaseDDOLGameObjects[1]; }
                }

                public class PhotonMono
                {
                    public static GameObject GetGameObject() { return allBaseDDOLGameObjects[2]; }
                }

                public class GameInstance
                {
                    public static GameObject GetGameObject() { return allBaseDDOLGameObjects[3]; }

                    public class PreInitializable
                    {
                        public static GameObject GetGameObject() { return GameInstance.GetGameObject().transform.GetChild(0).gameObject; }

                        public class AudioManager
                        {
                            public static GameObject GetGameObject() { return PreInitializable.GetGameObject().transform.GetChild(0).gameObject; }

                            public class SteamAudioManagerSettings
                            {
                                public static GameObject GetGameObject() { return AudioManager.GetGameObject().transform.GetChild(0).gameObject; }
                            }
                        }

                        public class PoolManager
                        {
                            public static GameObject GetGameObject() { return PreInitializable.GetGameObject().transform.GetChild(1).gameObject; }

                            public class VigorstoneBurstVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class OnPlayerVisualsChangedVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class PlayerFistBumpBonusVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class BoulderBall
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class GuardstoneTutorial
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class VolatileStone
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(5).gameObject; }
                            }

                            public class ChargeStone
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(6).gameObject; }
                            }

                            public class SurgeStone
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(7).gameObject; }
                            }

                            public class FlowStone
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(8).gameObject; }
                            }

                            public class GuardStone
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(9).gameObject; }
                            }

                            public class StubbornStone
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(10).gameObject; }
                            }

                            public class AdamantStone
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(11).gameObject; }
                            }

                            public class VigorStone
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(12).gameObject; }
                            }

                            public class SurgestoneVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(13).gameObject; }
                            }

                            public class GuardstoneVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(14).gameObject; }
                            }

                            public class StubbornstoneVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(15).gameObject; }
                            }

                            public class AdamantstoneVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(16).gameObject; }
                            }

                            public class VigorstoneVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(17).gameObject; }
                            }

                            public class SmallRock
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(18).gameObject; }
                            }

                            public class LargeRock
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(19).gameObject; }
                            }

                            public class DustBreakDISCVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(20).gameObject; }
                            }

                            public class FlickVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(21).gameObject; }
                            }

                            public class PlayerBoxInteractionVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(22).gameObject; }
                            }

                            public class ExplodeActivationVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(23).gameObject; }
                            }

                            public class ExplodeFinaleVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(24).gameObject; }
                            }

                            public class ExplodeStatusVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(25).gameObject; }
                            }

                            public class DashVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(26).gameObject; }
                            }

                            public class DustPlayerKnockbackVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(27).gameObject; }
                            }

                            public class Hitmarker
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(28).gameObject; }
                            }

                            public class GroundVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(29).gameObject; }
                            }

                            public class StructureCollisionVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(30).gameObject; }
                            }

                            public class UngroundVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(31).gameObject; }
                            }

                            public class JumpVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(32).gameObject; }
                            }

                            public class PosePerformedVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(33).gameObject; }
                            }

                            public class MovePerformedVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(34).gameObject; }
                            }

                            public class RicochetVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(35).gameObject; }
                            }

                            public class ParryVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(36).gameObject; }
                            }

                            public class HoldVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(37).gameObject; }
                            }

                            public class UppercutVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(38).gameObject; }
                            }

                            public class StraightVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(39).gameObject; }
                            }

                            public class KickVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(40).gameObject; }
                            }

                            public class Stomp_VFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(41).gameObject; }
                            }

                            public class DustFootstep
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(42).gameObject; }
                            }

                            public class Pillar
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(43).gameObject; }
                            }

                            public class Disc
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(44).gameObject; }
                            }

                            public class GroundedStateVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(45).gameObject; }
                            }

                            public class DustSpawnVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(46).gameObject; }
                            }

                            public class DustBreakVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(47).gameObject; }
                            }

                            public class DustImpactVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(48).gameObject; }
                            }

                            public class JointControl
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(49).gameObject; }
                            }

                            public class Wall
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(50).gameObject; }
                            }

                            public class RockCube
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(51).gameObject; }
                            }

                            public class Ball
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(52).gameObject; }
                            }

                            public class FloatingDamageText
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(53).gameObject; }
                            }

                            public class PooledAudioSource
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(54).gameObject; }
                            }

                            public class DustGroundedFrictionVFX
                            {
                                public static GameObject GetGameObject() { return PoolManager.GetGameObject().transform.GetChild(55).gameObject; }
                            }
                        }

                        public class PlayFabHandler
                        {
                            public static GameObject GetGameObject() { return PreInitializable.GetGameObject().transform.GetChild(2).gameObject; }
                        }
                    }

                    public class Initializable
                    {
                        public static GameObject GetGameObject() { return GameInstance.GetGameObject().transform.GetChild(1).gameObject; }

                        public class NetworkManager
                        {
                            public static GameObject GetGameObject() { return Initializable.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class PlayerManager
                        {
                            public static GameObject GetGameObject() { return Initializable.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class InputManager
                        {
                            public static GameObject GetGameObject() { return Initializable.GetGameObject().transform.GetChild(2).gameObject; }
                        }

                        public class SceneManager
                        {
                            public static GameObject GetGameObject() { return Initializable.GetGameObject().transform.GetChild(3).gameObject; }
                        }

                        public class NotificationManager
                        {
                            public static GameObject GetGameObject() { return Initializable.GetGameObject().transform.GetChild(4).gameObject; }
                        }

                        public class StackManager
                        {
                            public static GameObject GetGameObject() { return Initializable.GetGameObject().transform.GetChild(5).gameObject; }
                        }

                        public class QualityManager
                        {
                            public static GameObject GetGameObject() { return Initializable.GetGameObject().transform.GetChild(6).gameObject; }
                        }

                        public class SocialHandler
                        {
                            public static GameObject GetGameObject() { return Initializable.GetGameObject().transform.GetChild(7).gameObject; }
                        }

                        public class SlabManager
                        {
                            public static GameObject GetGameObject() { return Initializable.GetGameObject().transform.GetChild(8).gameObject; }
                        }

                        public class RecordingCamera
                        {
                            public static GameObject GetGameObject() { return Initializable.GetGameObject().transform.GetChild(9).gameObject; }

                            public class FadeScreenRenderer
                            {
                                public static GameObject GetGameObject() { return RecordingCamera.GetGameObject().transform.GetChild(0).gameObject; }
                            }
                        }

                        public class CatalogHandler
                        {
                            public static GameObject GetGameObject() { return Initializable.GetGameObject().transform.GetChild(10).gameObject; }
                        }

                        public class EconomyHandler
                        {
                            public static GameObject GetGameObject() { return Initializable.GetGameObject().transform.GetChild(11).gameObject; }
                        }
                    }

                    public class External
                    {
                        public static GameObject GetGameObject() { return GameInstance.GetGameObject().transform.GetChild(2).gameObject; }

                        public class LanguageManager
                        {
                            public static GameObject GetGameObject() { return External.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class PUNVoiceClient
                        {
                            public static GameObject GetGameObject() { return External.GetGameObject().transform.GetChild(1).gameObject; }
                        }
                    }

                    public class Other
                    {
                        public static GameObject GetGameObject() { return GameInstance.GetGameObject().transform.GetChild(3).gameObject; }

                        public class RaiseEventHandler
                        {
                            public static GameObject GetGameObject() { return Other.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class Layermasks
                        {
                            public static GameObject GetGameObject() { return Other.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class CombatManager
                        {
                            public static GameObject GetGameObject() { return Other.GetGameObject().transform.GetChild(2).gameObject; }
                        }
                    }

                    public class UI
                    {
                        public static GameObject GetGameObject() { return GameInstance.GetGameObject().transform.GetChild(4).gameObject; }

                        public class LegacyRecordingCameraUI
                        {
                            public static GameObject GetGameObject() { return UI.GetGameObject().transform.GetChild(0).gameObject; }

                            public class Panel
                            {
                                public static GameObject GetGameObject() { return UI.GetGameObject().transform.GetChild(0).gameObject; }

                                public class FieldOfView
                                {
                                    public static GameObject GetGameObject() { return Panel.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Label
                                    {
                                        public static GameObject GetGameObject() { return FieldOfView.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Minlabel
                                    {
                                        public static GameObject GetGameObject() { return FieldOfView.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class MaxLabel
                                    {
                                        public static GameObject GetGameObject() { return FieldOfView.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class SliderOutline
                                    {
                                        public static GameObject GetGameObject() { return FieldOfView.GetGameObject().transform.GetChild(3).gameObject; }

                                        public class Background
                                        {
                                            public static GameObject GetGameObject() { return SliderOutline.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class FillArea
                                        {
                                            public static GameObject GetGameObject() { return SliderOutline.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Fill
                                            {
                                                public static GameObject GetGameObject() { return FillArea.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }

                                        public class HandleSlideArea
                                        {
                                            public static GameObject GetGameObject() { return SliderOutline.GetGameObject().transform.GetChild(2).gameObject; }

                                            public class Handle
                                            {
                                                public static GameObject GetGameObject() { return HandleSlideArea.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class Current
                                                {
                                                    public static GameObject GetGameObject() { return Handle.GetGameObject().transform.GetChild(0).gameObject; }
                                                }
                                            }
                                        }
                                    }
                                }

                                public class PositionSmoothing
                                {
                                    public static GameObject GetGameObject() { return Panel.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class Label
                                    {
                                        public static GameObject GetGameObject() { return PositionSmoothing.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Minlabel
                                    {
                                        public static GameObject GetGameObject() { return PositionSmoothing.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class MaxLabel
                                    {
                                        public static GameObject GetGameObject() { return PositionSmoothing.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class SliderOutline
                                    {
                                        public static GameObject GetGameObject() { return PositionSmoothing.GetGameObject().transform.GetChild(3).gameObject; }

                                        public class Background
                                        {
                                            public static GameObject GetGameObject() { return SliderOutline.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class FillArea
                                        {
                                            public static GameObject GetGameObject() { return SliderOutline.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Fill
                                            {
                                                public static GameObject GetGameObject() { return FillArea.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }

                                        public class HandleSlideArea
                                        {
                                            public static GameObject GetGameObject() { return SliderOutline.GetGameObject().transform.GetChild(2).gameObject; }

                                            public class Handle
                                            {
                                                public static GameObject GetGameObject() { return HandleSlideArea.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class Current
                                                {
                                                    public static GameObject GetGameObject() { return Handle.GetGameObject().transform.GetChild(0).gameObject; }
                                                }
                                            }
                                        }
                                    }
                                }

                                public class RotationSmoothing
                                {
                                    public static GameObject GetGameObject() { return Panel.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class Label
                                    {
                                        public static GameObject GetGameObject() { return RotationSmoothing.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Minlabel
                                    {
                                        public static GameObject GetGameObject() { return RotationSmoothing.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class maxLabel
                                    {
                                        public static GameObject GetGameObject() { return RotationSmoothing.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class SliderOutline
                                    {
                                        public static GameObject GetGameObject() { return RotationSmoothing.GetGameObject().transform.GetChild(3).gameObject; }

                                        public class Background
                                        {
                                            public static GameObject GetGameObject() { return SliderOutline.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class FillArea
                                        {
                                            public static GameObject GetGameObject() { return SliderOutline.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Fill
                                            {
                                                public static GameObject GetGameObject() { return FillArea.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }

                                        public class HandleSlideArea
                                        {
                                            public static GameObject GetGameObject() { return SliderOutline.GetGameObject().transform.GetChild(2).gameObject; }

                                            public class Handle
                                            {
                                                public static GameObject GetGameObject() { return HandleSlideArea.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class Current
                                                {
                                                    public static GameObject GetGameObject() { return Handle.GetGameObject().transform.GetChild(0).gameObject; }
                                                }
                                            }
                                        }
                                    }
                                }

                                public class Enabled
                                {
                                    public static GameObject GetGameObject() { return Panel.GetGameObject().transform.GetChild(3).gameObject; }

                                    public class Label
                                    {
                                        public static GameObject GetGameObject() { return Enabled.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class ToggleStandard
                                    {
                                        public static GameObject GetGameObject() { return Enabled.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class Check
                                        {
                                            public static GameObject GetGameObject() { return ToggleStandard.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class Checkmark
                                            {
                                                public static GameObject GetGameObject() { return Check.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class Filled
                                            {
                                                public static GameObject GetGameObject() { return Check.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class LabelOn
                                        {
                                            public static GameObject GetGameObject() { return ToggleStandard.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }

                                public class ToggleLabel
                                {
                                    public static GameObject GetGameObject() { return Panel.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class SwitchToModernButton
                                {
                                    public static GameObject GetGameObject() { return Panel.GetGameObject().transform.GetChild(5).gameObject; }

                                    public class Text
                                    {
                                        public static GameObject GetGameObject() { return SwitchToModernButton.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Ripple
                                    {
                                        public static GameObject GetGameObject() { return SwitchToModernButton.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }
                            }

                            public class CameraDisableWarning
                            {
                                public static GameObject GetGameObject() { return UI.GetGameObject().transform.GetChild(1).gameObject; }

                                public class ToolTipRect
                                {
                                    public static GameObject GetGameObject() { return CameraDisableWarning.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Content
                                    {
                                        public static GameObject GetGameObject() { return ToolTipRect.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class Shadow
                                        {
                                            public static GameObject GetGameObject() { return Content.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Background
                                        {
                                            public static GameObject GetGameObject() { return Content.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class Description
                                        {
                                            public static GameObject GetGameObject() { return Content.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }
                            }
                        }

                        public class ModernRecordingCameraUI
                        {
                            public static GameObject GetGameObject() { return UI.GetGameObject().transform.GetChild(1).gameObject; }

                            public class Panel
                            {
                                public static GameObject GetGameObject() { return ModernRecordingCameraUI.GetGameObject().transform.GetChild(0).gameObject; }

                                public class Enabled
                                {
                                    public static GameObject GetGameObject() { return Panel.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Label
                                    {
                                        public static GameObject GetGameObject() { return Enabled.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class ToggleStandard
                                    {
                                        public static GameObject GetGameObject() { return Enabled.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class Check
                                        {
                                            public static GameObject GetGameObject() { return ToggleStandard.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class Checkmark
                                            {
                                                public static GameObject GetGameObject() { return Check.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class Filled
                                            {
                                                public static GameObject GetGameObject() { return Check.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class LabelOn
                                        {
                                            public static GameObject GetGameObject() { return ToggleStandard.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }

                                public class TurnOffRUMBLEAvatar
                                {
                                    public static GameObject GetGameObject() { return Panel.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class Check
                                    {
                                        public static GameObject GetGameObject() { return TurnOffRUMBLEAvatar.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class Checkmark
                                        {
                                            public static GameObject GetGameObject() { return Check.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Filled
                                        {
                                            public static GameObject GetGameObject() { return Check.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class LabelOn
                                    {
                                        public static GameObject GetGameObject() { return TurnOffRUMBLEAvatar.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }

                                public class LIVSetUpManualButton
                                {
                                    public static GameObject GetGameObject() { return Panel.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class Text
                                    {
                                        public static GameObject GetGameObject() { return LIVSetUpManualButton.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Ripple
                                    {
                                        public static GameObject GetGameObject() { return LIVSetUpManualButton.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }

                                public class SwitchToLegacyButton
                                {
                                    public static GameObject GetGameObject() { return Panel.GetGameObject().transform.GetChild(3).gameObject; }

                                    public class Text
                                    {
                                        public static GameObject GetGameObject() { return SwitchToLegacyButton.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Ripple
                                    {
                                        public static GameObject GetGameObject() { return SwitchToLegacyButton.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }

                                public class ToggleLabel
                                {
                                    public static GameObject GetGameObject() { return Panel.GetGameObject().transform.GetChild(4).gameObject; }
                                }
                            }

                            public class CameraDisableWarning
                            {
                                public static GameObject GetGameObject() { return ModernRecordingCameraUI.GetGameObject().transform.GetChild(1).gameObject; }

                                public class ToolTipRect
                                {
                                    public static GameObject GetGameObject() { return CameraDisableWarning.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Content
                                    {
                                        public static GameObject GetGameObject() { return ToolTipRect.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class Shadow
                                        {
                                            public static GameObject GetGameObject() { return Content.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Background
                                        {
                                            public static GameObject GetGameObject() { return Content.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class Description
                                        {
                                            public static GameObject GetGameObject() { return Content.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }
                            }
                        }

                        public class EventSystem
                        {
                            public static GameObject GetGameObject() { return UI.GetGameObject().transform.GetChild(2).gameObject; }
                        }
                    }
                }

                public class TimerUpdater
                {
                    public static GameObject GetGameObject() { return allBaseDDOLGameObjects[4]; }
                }
            }

            public class Gym
            {
                public static System.Collections.Generic.List<GameObject> GetBaseDDOLGameObjects() { return allBaseGymGameObjects; }

                public class FTraceLightmaps
                {
                    public static GameObject GetGameObject() { return allBaseGymGameObjects[0]; }
                }

                public class Tutorial
                {
                    public static GameObject GetGameObject() { return allBaseGymGameObjects[1]; }

                    public class WorldTutorials
                    {
                        public static GameObject GetGameObject() { return Tutorial.GetGameObject().transform.GetChild(0).gameObject; }

                        public class ToolTips
                        {
                            public static GameObject GetGameObject() { return WorldTutorials.GetGameObject().transform.GetChild(0).gameObject; }

                            public class MailTube
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(0).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }

                                    public class ToolTipHand
                                    {
                                        public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject; }

                                        public class Hand
                                        {
                                            public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject; }

                                            public class GhostHand
                                            {
                                                public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject; }

                                                public class BoneLowerArmRollL
                                                {
                                                    public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject; }

                                                    public class BoneHandL
                                                    {
                                                        public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject; }

                                                        public class BoneArmTwistboneL
                                                        {
                                                            public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject; }

                                                            public class BoneThumbAL
                                                            {
                                                                public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject; }

                                                                public class BoneThumbBL
                                                                {
                                                                    public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject; }
                                                                }
                                                            }
                                                        }

                                                        public class BoneMiddlefingerAL
                                                        {
                                                            public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject; }

                                                            public class BoneMiddlefingerBL
                                                            {
                                                                public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject; }

                                                                public class BoneMiddlefingerCL
                                                                {
                                                                    public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject; }
                                                                }
                                                            }
                                                        }

                                                        public class BonePinkieAL
                                                        {
                                                            public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject; }

                                                            public class BonePinkieBL
                                                            {
                                                                public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).gameObject; }

                                                                public class BonePinkieCL
                                                                {
                                                                    public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).gameObject; }
                                                                }
                                                            }
                                                        }

                                                        public class BonePointerAL
                                                        {
                                                            public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(3).gameObject; }

                                                            public class BonePointerBL
                                                            {
                                                                public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetChild(0).gameObject; }

                                                                public class BonePointerCL
                                                                {
                                                                    public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(3).GetChild(0).GetChild(0).gameObject; }
                                                                }
                                                            }
                                                        }

                                                        public class BoneRingFingerAL
                                                        {
                                                            public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(4).gameObject; }

                                                            public class BoneRingFingerBL
                                                            {
                                                                public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(4).GetChild(0).gameObject; }

                                                                public class BoneRingFingerCL
                                                                {
                                                                    public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(4).GetChild(0).GetChild(0).gameObject; }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            public class HandLPoseghost
                                            {
                                                public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(1).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class TotemReader
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(1).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return TotemReader.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class MoveSelector
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(2).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return MoveSelector.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class ParkBoard
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(3).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return ParkBoard.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class Gondola
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(4).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class Telephone
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(5).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class Matchmaker
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(6).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return Matchmaker.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }

                                    public class ToolTipHand
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class Hand
                                        {
                                            public static GameObject GetGameObject() { return ToolTipHand.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class GhostHand
                                            {
                                                public static GameObject GetGameObject() { return Hand.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class BoneLowerArmRollL
                                                {
                                                    public static GameObject GetGameObject() { return GhostHand.GetGameObject().transform.GetChild(0).gameObject; }

                                                    public class BoneHandL
                                                    {
                                                        public static GameObject GetGameObject() { return BoneLowerArmRollL.GetGameObject().transform.GetChild(0).gameObject; }

                                                        public class BoneArmTwistBoneL
                                                        {
                                                            public static GameObject GetGameObject() { return BoneHandL.GetGameObject().transform.GetChild(0).gameObject; }

                                                            public class BoneThumbAL
                                                            {
                                                                public static GameObject GetGameObject() { return BoneArmTwistBoneL.GetGameObject().transform.GetChild(0).gameObject; }

                                                                public class BoneThumbBL
                                                                {
                                                                    public static GameObject GetGameObject() { return BoneThumbAL.GetGameObject().transform.GetChild(0).gameObject; }
                                                                }
                                                            }
                                                        }

                                                        public class BoneMiddlefingerAL
                                                        {
                                                            public static GameObject GetGameObject() { return BoneHandL.GetGameObject().transform.GetChild(1).gameObject; }

                                                            public class BoneMiddlefingerBL
                                                            {
                                                                public static GameObject GetGameObject() { return BoneMiddlefingerAL.GetGameObject().transform.GetChild(0).gameObject; }

                                                                public class BoneMiddlefingerCL
                                                                {
                                                                    public static GameObject GetGameObject() { return BoneMiddlefingerBL.GetGameObject().transform.GetChild(0).gameObject; }
                                                                }
                                                            }
                                                        }

                                                        public class BonePinkieAL
                                                        {
                                                            public static GameObject GetGameObject() { return BoneHandL.GetGameObject().transform.GetChild(2).gameObject; }

                                                            public class BonePinkieBL
                                                            {
                                                                public static GameObject GetGameObject() { return BonePinkieAL.GetGameObject().transform.GetChild(0).gameObject; }

                                                                public class BonePinkieCL
                                                                {
                                                                    public static GameObject GetGameObject() { return BonePinkieBL.GetGameObject().transform.GetChild(0).gameObject; }
                                                                }
                                                            }
                                                        }

                                                        public class BonePointerAL
                                                        {
                                                            public static GameObject GetGameObject() { return BoneHandL.GetGameObject().transform.GetChild(3).gameObject; }

                                                            public class BonePointerBL
                                                            {
                                                                public static GameObject GetGameObject() { return BonePointerAL.GetGameObject().transform.GetChild(0).gameObject; }

                                                                public class BonePointerCL
                                                                {
                                                                    public static GameObject GetGameObject() { return BonePointerBL.GetGameObject().transform.GetChild(0).gameObject; }
                                                                }
                                                            }
                                                        }

                                                        public class BoneRingFingerAL
                                                        {
                                                            public static GameObject GetGameObject() { return BoneHandL.GetGameObject().transform.GetChild(4).gameObject; }

                                                            public class BoneRingFingerBL
                                                            {
                                                                public static GameObject GetGameObject() { return BoneRingFingerAL.GetGameObject().transform.GetChild(0).gameObject; }

                                                                public class BoneRingFingerCL
                                                                {
                                                                    public static GameObject GetGameObject() { return BoneRingFingerBL.GetGameObject().transform.GetChild(0).gameObject; }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            public class HandLPoseghost
                                            {
                                                public static GameObject GetGameObject() { return allBaseGymGameObjects[2].transform.GetChild(0).GetChild(0).GetChild(6).GetChild(0).GetChild(1).GetChild(0).GetChild(1).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class RegionBoard
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(7).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return RegionBoard.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class HowardControlPanel
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(8).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return HowardControlPanel.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class OptionsSlab
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(9).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return OptionsSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class Rank
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(10).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return Rank.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class CCStructures
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(11).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return CCStructures.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class CCModifiers
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(12).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return CCModifiers.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class CCHits
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(13).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return CCHits.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class CCCollisions
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(14).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return CCCollisions.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class CCRicochets
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(15).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return CCRicochets.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class CCStates
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(16).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return CCStates.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class CCLaunches
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(17).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return CCLaunches.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class CCExplosions
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(18).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return CCExplosions.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }

                            public class Shiftstones
                            {
                                public static GameObject GetGameObject() { return ToolTips.GetGameObject().transform.GetChild(19).gameObject; }

                                public class InfoContent
                                {
                                    public static GameObject GetGameObject() { return Shiftstones.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoContent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipSpriteCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class ToolTipTextBackground
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToolTipArrowIcon
                                            {
                                                public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class ToolTipTextCanvas
                                        {
                                            public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class ToolTipTextComponent
                                            {
                                                public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        public class CombatCarvings
                        {
                            public static GameObject GetGameObject() { return WorldTutorials.GetGameObject().transform.GetChild(1).gameObject; }

                            public class Structures
                            {
                                public static GameObject GetGameObject() { return CombatCarvings.GetGameObject().transform.GetChild(0).gameObject; }

                                public class CarvingFoot
                                {
                                    public static GameObject GetGameObject() { return Structures.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class WoodCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class MetalCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class RockCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class CarvingHeadParent
                                {
                                    public static GameObject GetGameObject() { return Structures.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class StructureCarvingHead
                                    {
                                        public static GameObject GetGameObject() { return CarvingHeadParent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class StructureCube
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class StructureWall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class StructureBall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }

                                public class CarvingName
                                {
                                    public static GameObject GetGameObject() { return Structures.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class StructureWall
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class NameCanvas
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }
                            }

                            public class Modifiers
                            {
                                public static GameObject GetGameObject() { return CombatCarvings.GetGameObject().transform.GetChild(1).gameObject; }

                                public class CarvingFoot
                                {
                                    public static GameObject GetGameObject() { return Modifiers.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class WoodCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class MetalCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class RockCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class CarvingHeadParent
                                {
                                    public static GameObject GetGameObject() { return Modifiers.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class StructureCarvingHead
                                    {
                                        public static GameObject GetGameObject() { return CarvingHeadParent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class StructureCube
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class StructureWall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class StructureBall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }

                                public class CarvingName
                                {
                                    public static GameObject GetGameObject() { return Modifiers.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class StructureWall
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class NameCanvas
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }
                            }

                            public class Hits
                            {
                                public static GameObject GetGameObject() { return CombatCarvings.GetGameObject().transform.GetChild(2).gameObject; }

                                public class CarvingFoot
                                {
                                    public static GameObject GetGameObject() { return Hits.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class WoodCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class MetalCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class RockCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class CarvingHeadParent
                                {
                                    public static GameObject GetGameObject() { return Hits.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class StructureCarvingHead
                                    {
                                        public static GameObject GetGameObject() { return CarvingHeadParent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class StructureCube
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class StructureWall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class StructureBall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }

                                public class CarvingName
                                {
                                    public static GameObject GetGameObject() { return Hits.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class StructureWall
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class NameCanvas
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }
                            }

                            public class Collisions
                            {
                                public static GameObject GetGameObject() { return CombatCarvings.GetGameObject().transform.GetChild(3).gameObject; }

                                public class CarvingFoot
                                {
                                    public static GameObject GetGameObject() { return Collisions.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class WoodCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class MetalCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class RockCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class CarvingHeadParent
                                {
                                    public static GameObject GetGameObject() { return Collisions.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class StructureCarvingHead
                                    {
                                        public static GameObject GetGameObject() { return CarvingHeadParent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class StructureCube
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class StructureWall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class StructureBall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }

                                public class CarvingName
                                {
                                    public static GameObject GetGameObject() { return Collisions.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class StructureWall
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class NameCanvas
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }
                            }

                            public class Ricochets
                            {
                                public static GameObject GetGameObject() { return CombatCarvings.GetGameObject().transform.GetChild(4).gameObject; }

                                public class CarvingFoot
                                {
                                    public static GameObject GetGameObject() { return Ricochets.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class WoodCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class MetalCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class RockCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class CarvingHeadParent
                                {
                                    public static GameObject GetGameObject() { return Ricochets.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class StructureCarvingHead
                                    {
                                        public static GameObject GetGameObject() { return CarvingHeadParent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class StructureCube
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class StructureWall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class StructureBall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }

                                public class CarvingName
                                {
                                    public static GameObject GetGameObject() { return Ricochets.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class StructureWall
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class NameCanvas
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }
                            }

                            public class States
                            {
                                public static GameObject GetGameObject() { return CombatCarvings.GetGameObject().transform.GetChild(5).gameObject; }

                                public class CarvingFoot
                                {
                                    public static GameObject GetGameObject() { return States.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class WoodCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class MetalCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class RockCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class CarvingHeadParent
                                {
                                    public static GameObject GetGameObject() { return States.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class StructureCarvingHead
                                    {
                                        public static GameObject GetGameObject() { return CarvingHeadParent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class StructureCube
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class StructureWall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class StructureBall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }

                                public class CarvingName
                                {
                                    public static GameObject GetGameObject() { return States.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class StructureWall
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class NameCanvas
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }
                            }

                            public class Launches
                            {
                                public static GameObject GetGameObject() { return CombatCarvings.GetGameObject().transform.GetChild(6).gameObject; }

                                public class CarvingFoot
                                {
                                    public static GameObject GetGameObject() { return Launches.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class WoodCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class MetalCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class RockCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class CarvingHeadParent
                                {
                                    public static GameObject GetGameObject() { return Launches.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class StructureCarvingHead
                                    {
                                        public static GameObject GetGameObject() { return CarvingHeadParent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class StructureCube
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class StructureWall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class StructureBall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }

                                public class CarvingName
                                {
                                    public static GameObject GetGameObject() { return Launches.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class StructureWall
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class NameCanvas
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }
                            }

                            public class Explosions
                            {
                                public static GameObject GetGameObject() { return CombatCarvings.GetGameObject().transform.GetChild(7).gameObject; }

                                public class CarvingFoot
                                {
                                    public static GameObject GetGameObject() { return Explosions.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class WoodCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class MetalCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class RockCollider
                                    {
                                        public static GameObject GetGameObject() { return CarvingFoot.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class CarvingHeadParent
                                {
                                    public static GameObject GetGameObject() { return Explosions.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class StructureCarvingHead
                                    {
                                        public static GameObject GetGameObject() { return CarvingHeadParent.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class StructureCube
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class StructureWall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class StructureBall
                                        {
                                            public static GameObject GetGameObject() { return StructureCarvingHead.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }

                                public class CarvingName
                                {
                                    public static GameObject GetGameObject() { return Explosions.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class StructureWall
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class NameCanvas
                                    {
                                        public static GameObject GetGameObject() { return CarvingName.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }
                            }
                        }

                        public class Sprinting
                        {
                            public static GameObject GetGameObject() { return WorldTutorials.GetGameObject().transform.GetChild(2).gameObject; }

                            public class SprintHint
                            {
                                public static GameObject GetGameObject() { return Sprinting.GetGameObject().transform.GetChild(0).gameObject; }

                                public class HintSlab1
                                {
                                    public static GameObject GetGameObject() { return SprintHint.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class InfoFormCanvas
                                    {
                                        public static GameObject GetGameObject() { return HintSlab1.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class InfoText
                                        {
                                            public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Quad
                                        {
                                            public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }

                                public class HintSlab2
                                {
                                    public static GameObject GetGameObject() { return SprintHint.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class InfoFormCanvas
                                    {
                                        public static GameObject GetGameObject() { return HintSlab2.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class InfoText
                                        {
                                            public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Quad
                                        {
                                            public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }
                            }

                            public class ThreadMill
                            {
                                public static GameObject GetGameObject() { return Sprinting.GetGameObject().transform.GetChild(1).gameObject; }

                                public class Ghost
                                {
                                    public static GameObject GetGameObject() { return ThreadMill.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Visuals
                                    {
                                        public static GameObject GetGameObject() { return Ghost.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class HandLPoseghost
                                        {
                                            public static GameObject GetGameObject() { return Visuals.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class HandRPoseghost
                                        {
                                            public static GameObject GetGameObject() { return Visuals.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class RIG
                                        {
                                            public static GameObject GetGameObject() { return Visuals.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }

                                public class StandIndicator
                                {
                                    public static GameObject GetGameObject() { return ThreadMill.GetGameObject().transform.GetChild(1).gameObject; }
                                }
                            }
                        }
                    }

                    public class StaticTutorials
                    {
                        public static GameObject GetGameObject() { return Tutorial.GetGameObject().transform.GetChild(1).gameObject; }

                        public class Measure
                        {
                            public static GameObject GetGameObject() { return StaticTutorials.GetGameObject().transform.GetChild(0).gameObject; }

                            public class StoneRumbleMan
                            {
                                public static GameObject GetGameObject() { return Measure.GetGameObject().transform.GetChild(0).gameObject; }

                                public class Text
                                {
                                    public static GameObject GetGameObject() { return StoneRumbleMan.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class MeasureTextCanvas
                                    {
                                        public static GameObject GetGameObject() { return Text.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class MeasureText
                                        {
                                            public static GameObject GetGameObject() { return MeasureTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                        }
                                    }
                                }
                            }
                        }

                        public class RUMBLEStarterGuide
                        {
                            public static GameObject GetGameObject() { return StaticTutorials.GetGameObject().transform.GetChild(1).gameObject; }

                            public class PreviousPageButton
                            {
                                public static GameObject GetGameObject() { return RUMBLEStarterGuide.GetGameObject().transform.GetChild(0).gameObject; }

                                public class ButtonRock
                                {
                                    public static GameObject GetGameObject() { return PreviousPageButton.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class PreviousText
                                {
                                    public static GameObject GetGameObject() { return PreviousPageButton.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class InteractionButton
                                {
                                    public static GameObject GetGameObject() { return PreviousPageButton.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class Spring
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }
                            }

                            public class NextPageButton
                            {
                                public static GameObject GetGameObject() { return RUMBLEStarterGuide.GetGameObject().transform.GetChild(1).gameObject; }

                                public class ButtonRock
                                {
                                    public static GameObject GetGameObject() { return NextPageButton.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class PreviousText
                                {
                                    public static GameObject GetGameObject() { return NextPageButton.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class InteractionButton
                                {
                                    public static GameObject GetGameObject() { return NextPageButton.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class Spring
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }
                            }

                            public class BaseSlab
                            {
                                public static GameObject GetGameObject() { return RUMBLEStarterGuide.GetGameObject().transform.GetChild(2).gameObject; }

                                public class SlabRock
                                {
                                    public static GameObject GetGameObject() { return BaseSlab.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class TopicUnderline
                                {
                                    public static GameObject GetGameObject() { return BaseSlab.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class Topics
                                {
                                    public static GameObject GetGameObject() { return BaseSlab.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class Introduction
                                    {
                                        public static GameObject GetGameObject() { return Topics.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class TextField
                                        {
                                            public static GameObject GetGameObject() { return Introduction.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class TitleField
                                        {
                                            public static GameObject GetGameObject() { return Introduction.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class PageField
                                        {
                                            public static GameObject GetGameObject() { return Introduction.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }

                                    public class Practice
                                    {
                                        public static GameObject GetGameObject() { return Topics.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class TextField
                                        {
                                            public static GameObject GetGameObject() { return Practice.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class TitleField
                                        {
                                            public static GameObject GetGameObject() { return Practice.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class PageField
                                        {
                                            public static GameObject GetGameObject() { return Practice.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }

                                    public class Posture
                                    {
                                        public static GameObject GetGameObject() { return Topics.GetGameObject().transform.GetChild(2).gameObject; }

                                        public class TextField
                                        {
                                            public static GameObject GetGameObject() { return Posture.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class TitleField
                                        {
                                            public static GameObject GetGameObject() { return Posture.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class PageField
                                        {
                                            public static GameObject GetGameObject() { return Posture.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }

                                    public class Gaze
                                    {
                                        public static GameObject GetGameObject() { return Topics.GetGameObject().transform.GetChild(3).gameObject; }

                                        public class TextField
                                        {
                                            public static GameObject GetGameObject() { return Gaze.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class TitleField
                                        {
                                            public static GameObject GetGameObject() { return Gaze.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class PageField
                                        {
                                            public static GameObject GetGameObject() { return Gaze.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }

                                    public class Poses
                                    {
                                        public static GameObject GetGameObject() { return Topics.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class TextField
                                        {
                                            public static GameObject GetGameObject() { return Poses.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class TitleField
                                        {
                                            public static GameObject GetGameObject() { return Poses.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class PageField
                                        {
                                            public static GameObject GetGameObject() { return Poses.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }

                                    public class Hands
                                    {
                                        public static GameObject GetGameObject() { return Topics.GetGameObject().transform.GetChild(5).gameObject; }

                                        public class TextField
                                        {
                                            public static GameObject GetGameObject() { return Hands.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class TitleField
                                        {
                                            public static GameObject GetGameObject() { return Hands.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class PageField
                                        {
                                            public static GameObject GetGameObject() { return Hands.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                public class Scene
                {
                    public static GameObject GetGameObject() { return allBaseGymGameObjects[2]; }

                    public class GymProduction
                    {
                        public static GameObject GetGameObject() { return Scene.GetGameObject().transform.GetChild(0).gameObject; }

                        private class CollissionGroup
                        {
                            public static GameObject GetGameObject() { return GymProduction.GetGameObject().transform.GetChild(0).gameObject; }

                            public class CombatFloor
                            {
                                public static GameObject GetGameObject() { return CollissionGroup.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class EnvironmentCollission
                            {
                                public static GameObject GetGameObject() { return CollissionGroup.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class Floor
                            {
                                public static GameObject GetGameObject() { return CollissionGroup.GetGameObject().transform.GetChild(2).gameObject; }
                            }
                        }

                        public class MainStaticGroup
                        {
                            public static GameObject GetGameObject() { return GymProduction.GetGameObject().transform.GetChild(1).gameObject; }

                            public class Bridge
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(0).gameObject; }

                                public class BridgeMesh
                                {
                                    public static GameObject GetGameObject() { return Bridge.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class FloorMeshParent
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(1).gameObject; }

                                public class FloorMesh
                                {
                                    public static GameObject GetGameObject() { return FloorMeshParent.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class Foliage
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(2).gameObject; }

                                public class RootLeaves000
                                {
                                    public static GameObject GetGameObject() { return Foliage.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class RootLeaves001
                                {
                                    public static GameObject GetGameObject() { return Foliage.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class RootLeaves002
                                {
                                    public static GameObject GetGameObject() { return Foliage.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class RootLeaves003
                                {
                                    public static GameObject GetGameObject() { return Foliage.GetGameObject().transform.GetChild(3).gameObject; }
                                }
                            }

                            public class GymArena
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(3).gameObject; }

                                public class Arena
                                {
                                    public static GameObject GetGameObject() { return GymArena.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class ArenaMetalRim
                                {
                                    public static GameObject GetGameObject() { return GymArena.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class Cylinder
                                {
                                    public static GameObject GetGameObject() { return GymArena.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class LeafSphere23
                                {
                                    public static GameObject GetGameObject() { return GymArena.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class LeafSphere24
                                {
                                    public static GameObject GetGameObject() { return GymArena.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class Rocks
                                {
                                    public static GameObject GetGameObject() { return GymArena.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class Stair
                                {
                                    public static GameObject GetGameObject() { return GymArena.GetGameObject().transform.GetChild(6).gameObject; }
                                }
                            }

                            public class Pedistals
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(4).gameObject; }

                                public class LargePedistal
                                {
                                    public static GameObject GetGameObject() { return Pedistals.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class SpawnArea
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(5).gameObject; }

                                public class PathToSchool
                                {
                                    public static GameObject GetGameObject() { return SpawnArea.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class SpawnRingFloor
                                {
                                    public static GameObject GetGameObject() { return SpawnArea.GetGameObject().transform.GetChild(1).gameObject; }
                                }
                            }

                            public class Woodset1
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(6).gameObject; }
                            }

                            public class WoodStuff
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(7).gameObject; }

                                public class Woodset2
                                {
                                    public static GameObject GetGameObject() { return WoodStuff.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }
                        }

                        public class SubStaticGroup
                        {
                            public static GameObject GetGameObject() { return GymProduction.GetGameObject().transform.GetChild(2).gameObject; }

                            public class Rocks
                            {
                                public static GameObject GetGameObject() { return SubStaticGroup.GetGameObject().transform.GetChild(0).gameObject; }

                                public class FarGym
                                {
                                    public static GameObject GetGameObject() { return Rocks.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class FarRocks0
                                    {
                                        public static GameObject GetGameObject() { return FarGym.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class FarRocks1
                                    {
                                        public static GameObject GetGameObject() { return FarGym.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }

                                public class Hill
                                {
                                    public static GameObject GetGameObject() { return Rocks.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class RocksGym
                                {
                                    public static GameObject GetGameObject() { return Rocks.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class LargeBoulder
                                    {
                                        public static GameObject GetGameObject() { return RocksGym.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class MergedRocks000
                                    {
                                        public static GameObject GetGameObject() { return RocksGym.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class MergedRocks001
                                    {
                                        public static GameObject GetGameObject() { return RocksGym.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class MergedRocks002
                                    {
                                        public static GameObject GetGameObject() { return RocksGym.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class MergedRocks003
                                    {
                                        public static GameObject GetGameObject() { return RocksGym.GetGameObject().transform.GetChild(4).gameObject; }
                                    }
                                }
                            }

                            public class SceneRoots
                            {
                                public static GameObject GetGameObject() { return SubStaticGroup.GetGameObject().transform.GetChild(1).gameObject; }

                                public class TestRoot1Middetail
                                {
                                    public static GameObject GetGameObject() { return SceneRoots.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Cylinder_014__6_
                                    {
                                        public static GameObject GetGameObject() { return TestRoot1Middetail.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Cylinder_015__1_
                                    {
                                        public static GameObject GetGameObject() { return TestRoot1Middetail.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Cylinder_015__4__1
                                    {
                                        public static GameObject GetGameObject() { return TestRoot1Middetail.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class Cylinder_018__2_
                                    {
                                        public static GameObject GetGameObject() { return TestRoot1Middetail.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class GymCompRoot
                                    {
                                        public static GameObject GetGameObject() { return TestRoot1Middetail.GetGameObject().transform.GetChild(4).gameObject; }
                                    }

                                    public class Rumble_root_branch__4_
                                    {
                                        public static GameObject GetGameObject() { return TestRoot1Middetail.GetGameObject().transform.GetChild(5).gameObject; }
                                    }

                                    public class Rumble_root_branch__5_
                                    {
                                        public static GameObject GetGameObject() { return TestRoot1Middetail.GetGameObject().transform.GetChild(6).gameObject; }
                                    }

                                    public class Rumble_root_branch__6_
                                    {
                                        public static GameObject GetGameObject() { return TestRoot1Middetail.GetGameObject().transform.GetChild(7).gameObject; }
                                    }

                                    public class Rumble_root_branch__7_
                                    {
                                        public static GameObject GetGameObject() { return TestRoot1Middetail.GetGameObject().transform.GetChild(8).gameObject; }
                                    }

                                    public class Rumble_root_branch__8_
                                    {
                                        public static GameObject GetGameObject() { return TestRoot1Middetail.GetGameObject().transform.GetChild(9).gameObject; }
                                    }

                                    public class Rumble_root_branch__9_
                                    {
                                        public static GameObject GetGameObject() { return TestRoot1Middetail.GetGameObject().transform.GetChild(10).gameObject; }
                                    }
                                }
                            }
                        }

                        public class SubStaticGroupBuildings
                        {
                            public static GameObject GetGameObject() { return GymProduction.GetGameObject().transform.GetChild(3).gameObject; }

                            public class DressingRoom
                            {
                                public static GameObject GetGameObject() { return SubStaticGroupBuildings.GetGameObject().transform.GetChild(0).gameObject; }

                                public class DressingRoomText
                                {
                                    public static GameObject GetGameObject() { return DressingRoom.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class IndustrialStuff
                                {
                                    public static GameObject GetGameObject() { return DressingRoom.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class Planks
                                {
                                    public static GameObject GetGameObject() { return DressingRoom.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class Rock
                                {
                                    public static GameObject GetGameObject() { return DressingRoom.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class Topwood
                                {
                                    public static GameObject GetGameObject() { return DressingRoom.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class Drapes
                                {
                                    public static GameObject GetGameObject() { return DressingRoom.GetGameObject().transform.GetChild(5).gameObject; }

                                    public class DrapeMesh
                                    {
                                        public static GameObject GetGameObject() { return Drapes.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Drapes_
                                    {
                                        public static GameObject GetGameObject() { return Drapes.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class Bone
                                        {
                                            public static GameObject GetGameObject() { return Drapes_.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Bone1
                                        {
                                            public static GameObject GetGameObject() { return Drapes_.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class Bone2
                                        {
                                            public static GameObject GetGameObject() { return Drapes_.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                        public class Bone3
                                        {
                                            public static GameObject GetGameObject() { return Drapes_.GetGameObject().transform.GetChild(3).gameObject; }
                                        }

                                        public class Bone4
                                        {
                                            public static GameObject GetGameObject() { return Drapes_.GetGameObject().transform.GetChild(4).gameObject; }
                                        }

                                        public class Bone5
                                        {
                                            public static GameObject GetGameObject() { return Drapes_.GetGameObject().transform.GetChild(5).gameObject; }
                                        }

                                        public class Bone6
                                        {
                                            public static GameObject GetGameObject() { return Drapes_.GetGameObject().transform.GetChild(6).gameObject; }
                                        }

                                        public class Bone7
                                        {
                                            public static GameObject GetGameObject() { return Drapes_.GetGameObject().transform.GetChild(7).gameObject; }
                                        }

                                        public class Bone8
                                        {
                                            public static GameObject GetGameObject() { return Drapes_.GetGameObject().transform.GetChild(8).gameObject; }
                                        }
                                    }

                                    public class DrapeTrigger
                                    {
                                        public static GameObject GetGameObject() { return Drapes.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                            }
                            
                            public class HowardsPlace
                            {
                                public static GameObject GetGameObject() { return SubStaticGroupBuildings.GetGameObject().transform.GetChild(1).gameObject; }

                                public class HowardTrackStop
                                {
                                    public static GameObject GetGameObject() { return HowardsPlace.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class StationFloor
                                {
                                    public static GameObject GetGameObject() { return HowardsPlace.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class StationSidePlint000
                                {
                                    public static GameObject GetGameObject() { return HowardsPlace.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class StationSidePlint001
                                {
                                    public static GameObject GetGameObject() { return HowardsPlace.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class StationStuds
                                {
                                    public static GameObject GetGameObject() { return HowardsPlace.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class StationWall
                                {
                                    public static GameObject GetGameObject() { return HowardsPlace.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class StudsAndStrength
                                {
                                    public static GameObject GetGameObject() { return HowardsPlace.GetGameObject().transform.GetChild(6).gameObject; }
                                }

                                public class Track
                                {
                                    public static GameObject GetGameObject() { return HowardsPlace.GetGameObject().transform.GetChild(7).gameObject; }
                                }

                                public class TrackPillars
                                {
                                    public static GameObject GetGameObject() { return HowardsPlace.GetGameObject().transform.GetChild(8).gameObject; }
                                }

                                public class TrackPlint
                                {
                                    public static GameObject GetGameObject() { return HowardsPlace.GetGameObject().transform.GetChild(9).gameObject; }
                                }

                                public class TrackRoof
                                {
                                    public static GameObject GetGameObject() { return HowardsPlace.GetGameObject().transform.GetChild(10).gameObject; }
                                }

                                public class TrackWall
                                {
                                    public static GameObject GetGameObject() { return HowardsPlace.GetGameObject().transform.GetChild(11).gameObject; }
                                }
                            }

                            public class RumbleStation
                            {
                                public static GameObject GetGameObject() { return SubStaticGroupBuildings.GetGameObject().transform.GetChild(2).gameObject; }

                                public class FightResFloor
                                {
                                    public static GameObject GetGameObject() { return RumbleStation.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Industrial
                                {
                                    public static GameObject GetGameObject() { return RumbleStation.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class MeasureArea
                                {
                                    public static GameObject GetGameObject() { return RumbleStation.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class Planks
                                {
                                    public static GameObject GetGameObject() { return RumbleStation.GetGameObject().transform.GetChild(3).gameObject; }

                                    public class Plank53
                                    {
                                        public static GameObject GetGameObject() { return Planks.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Plank54
                                    {
                                        public static GameObject GetGameObject() { return Planks.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }

                                public class RankSigns
                                {
                                    public static GameObject GetGameObject() { return RumbleStation.GetGameObject().transform.GetChild(4).gameObject; }

                                    public class RankSign
                                    {
                                        public static GameObject GetGameObject() { return RankSigns.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class Text
                                        {
                                            public static GameObject GetGameObject() { return RankSign.GetGameObject().transform.GetChild(0).gameObject; }
                                        }
                                    }
                                }

                                public class RankSign
                                {
                                    public static GameObject GetGameObject() { return RumbleStation.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class Roof
                                {
                                    public static GameObject GetGameObject() { return RumbleStation.GetGameObject().transform.GetChild(6).gameObject; }
                                }

                                public class Root
                                {
                                    public static GameObject GetGameObject() { return RumbleStation.GetGameObject().transform.GetChild(7).gameObject; }
                                }

                                public class Storage
                                {
                                    public static GameObject GetGameObject() { return RumbleStation.GetGameObject().transform.GetChild(8).gameObject; }
                                }

                                public class Wood
                                {
                                    public static GameObject GetGameObject() { return RumbleStation.GetGameObject().transform.GetChild(9).gameObject; }

                                    public class Bench
                                    {
                                        public static GameObject GetGameObject() { return Wood.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class WoodsetLarge
                                        {
                                            public static GameObject GetGameObject() { return Bench.GetGameObject().transform.GetChild(0).gameObject; }
                                        }
                                    }

                                    public class WoodsetLarge
                                    {
                                        public static GameObject GetGameObject() { return Wood.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }
                            }

                            public class School
                            {
                                public static GameObject GetGameObject() { return SubStaticGroupBuildings.GetGameObject().transform.GetChild(3).gameObject; }

                                public class Cylinder003
                                {
                                    public static GameObject GetGameObject() { return School.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Cylinder011
                                {
                                    public static GameObject GetGameObject() { return School.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class Industrial
                                {
                                    public static GameObject GetGameObject() { return School.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class Rocks
                                {
                                    public static GameObject GetGameObject() { return School.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class Roof
                                {
                                    public static GameObject GetGameObject() { return School.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class WoodsetPlank
                                {
                                    public static GameObject GetGameObject() { return School.GetGameObject().transform.GetChild(5).gameObject; }
                                }
                            }

                            public class GearMarket
                            {
                                public static GameObject GetGameObject() { return SubStaticGroupBuildings.GetGameObject().transform.GetChild(4).gameObject; }

                                public class ItemHighlightWindow
                                {
                                    public static GameObject GetGameObject() { return GearMarket.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class MessageScreen
                                {
                                    public static GameObject GetGameObject() { return GearMarket.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class StallFrame
                                {
                                    public static GameObject GetGameObject() { return GearMarket.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class ItemPanels
                                {
                                    public static GameObject GetGameObject() { return GearMarket.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class Tags
                                {
                                    public static GameObject GetGameObject() { return GearMarket.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class ControlPanel
                                {
                                    public static GameObject GetGameObject() { return GearMarket.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class MailTube
                                {
                                    public static GameObject GetGameObject() { return GearMarket.GetGameObject().transform.GetChild(6).gameObject; }
                                }

                                public class AlertLightVFX
                                {
                                    public static GameObject GetGameObject() { return GearMarket.GetGameObject().transform.GetChild(7).gameObject; }
                                }

                                public class Bell
                                {
                                    public static GameObject GetGameObject() { return GearMarket.GetGameObject().transform.GetChild(8).gameObject; }
                                }
                            }

                            public class ProgressTracker
                            {
                                public static GameObject GetGameObject() { return SubStaticGroupBuildings.GetGameObject().transform.GetChild(5).gameObject; }
                            }
                        }

                        public class Vista
                        {
                            public static GameObject GetGameObject() { return GymProduction.GetGameObject().transform.GetChild(4).gameObject; }

                            public class Mountains
                            {
                                public static GameObject GetGameObject() { return Vista.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class Water
                            {
                                public static GameObject GetGameObject() { return Vista.GetGameObject().transform.GetChild(1).gameObject; }
                            }
                        }

                        public class DressingRoom
                        {
                            public static GameObject GetGameObject() { return GymProduction.GetGameObject().transform.GetChild(5).gameObject; }

                            public class DyeSink
                            {
                                public static GameObject GetGameObject() { return DressingRoom.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class GearCabinet
                            {
                                public static GameObject GetGameObject() { return DressingRoom.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class ControlPanel
                            {
                                public static GameObject GetGameObject() { return DressingRoom.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class PreviewPlayerController
                            {
                                public static GameObject GetGameObject() { return DressingRoom.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class FirstTimeSexSelect
                            {
                                public static GameObject GetGameObject() { return DressingRoom.GetGameObject().transform.GetChild(4).gameObject; }
                            }
                        }
                    }

                    private class FloorCollision
                    {
                        public static GameObject GetGameObject() { return Scene.GetGameObject().transform.GetChild(1).gameObject; }

                        public class PathCollision
                        {
                            public static GameObject GetGameObject() { return FloorCollision.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class FightHouse
                        {
                            public static GameObject GetGameObject() { return FloorCollision.GetGameObject().transform.GetChild(1).gameObject; }

                            public class Floor
                            {
                                public static GameObject GetGameObject() { return FightHouse.GetGameObject().transform.GetChild(0).gameObject; }
                            }
                        }

                        public class CombatFloors
                        {
                            public static GameObject GetGameObject() { return FloorCollision.GetGameObject().transform.GetChild(2).gameObject; }

                            public class FloorCollission
                            {
                                public static GameObject GetGameObject() { return CombatFloors.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class PathwayMeshColliders
                            {
                                public static GameObject GetGameObject() { return CombatFloors.GetGameObject().transform.GetChild(1).gameObject; }

                                public class SpawnMoveFloorCollider
                                {
                                    public static GameObject GetGameObject() { return PathwayMeshColliders.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class SpawnPathsMeshCollider
                                {
                                    public static GameObject GetGameObject() { return PathwayMeshColliders.GetGameObject().transform.GetChild(1).gameObject; }
                                }
                            }
                        }

                        public class FloorCollisionPreventers
                        {
                            public static GameObject GetGameObject() { return FloorCollision.GetGameObject().transform.GetChild(3).gameObject; }

                            public class Gondola
                            {
                                public static GameObject GetGameObject() { return FloorCollisionPreventers.GetGameObject().transform.GetChild(0).gameObject; }
                            }
                        }
                    }

                    public class Grass
                    {
                        public static GameObject GetGameObject() { return Scene.GetGameObject().transform.GetChild(2).gameObject; }

                        public System.Collections.Generic.List<GameObject> GetAllGrassGameObjects()
                        {
                            System.Collections.Generic.List<GameObject> grass = new System.Collections.Generic.List<GameObject>();
                            for (int i = 0; i < Grass.GetGameObject().transform.childCount; i++) { grass.Add(Grass.GetGameObject().transform.GetChild(i).gameObject); }
                            return grass;
                        }
                    }

                    public class Dynamic
                    {
                        public static GameObject GetGameObject() { return Scene.GetGameObject().transform.GetChild(3).gameObject; }

                        public class Signs
                        {
                            public static GameObject GetGameObject() { return Dynamic.GetGameObject().transform.GetChild(0).gameObject; }

                            public class AreaSigns0
                            {
                                public static GameObject GetGameObject() { return Signs.GetGameObject().transform.GetChild(0).gameObject; }

                                public class CombatStudy
                                {
                                    public static GameObject GetGameObject() { return AreaSigns0.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class HowardsYard
                                {
                                    public static GameObject GetGameObject() { return AreaSigns0.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class RumbleStation
                                {
                                    public static GameObject GetGameObject() { return AreaSigns0.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class Sign
                                {
                                    public static GameObject GetGameObject() { return AreaSigns0.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class Cylinder0
                                {
                                    public static GameObject GetGameObject() { return AreaSigns0.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class Cylinder1
                                {
                                    public static GameObject GetGameObject() { return AreaSigns0.GetGameObject().transform.GetChild(5).gameObject; }
                                }
                            }

                            public class AreaSigns1
                            {
                                public static GameObject GetGameObject() { return Signs.GetGameObject().transform.GetChild(1).gameObject; }

                                public class CombatStudy
                                {
                                    public static GameObject GetGameObject() { return AreaSigns1.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class HowardsYard
                                {
                                    public static GameObject GetGameObject() { return AreaSigns1.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class RumbleStation
                                {
                                    public static GameObject GetGameObject() { return AreaSigns1.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class Sign
                                {
                                    public static GameObject GetGameObject() { return AreaSigns1.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class Cylinder0
                                {
                                    public static GameObject GetGameObject() { return AreaSigns1.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class Cylinder1
                                {
                                    public static GameObject GetGameObject() { return AreaSigns1.GetGameObject().transform.GetChild(5).gameObject; }
                                }
                            }

                            public class AreaSigns2
                            {
                                public static GameObject GetGameObject() { return Signs.GetGameObject().transform.GetChild(2).gameObject; }

                                public class CombatStudy
                                {
                                    public static GameObject GetGameObject() { return AreaSigns2.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class HowardsYard
                                {
                                    public static GameObject GetGameObject() { return AreaSigns2.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class RumbleStation
                                {
                                    public static GameObject GetGameObject() { return AreaSigns2.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class Sign
                                {
                                    public static GameObject GetGameObject() { return AreaSigns2.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class Cylinder0
                                {
                                    public static GameObject GetGameObject() { return AreaSigns2.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class Cylinder1
                                {
                                    public static GameObject GetGameObject() { return AreaSigns2.GetGameObject().transform.GetChild(5).gameObject; }
                                }
                            }
                        }

                        public class ExtraArm
                        {
                            public static GameObject GetGameObject() { return Dynamic.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class Crate
                        {
                            public static GameObject GetGameObject() { return Dynamic.GetGameObject().transform.GetChild(2).gameObject; }
                        }
                    }

                    public class LightingAndEffects
                    {
                        public static GameObject GetGameObject() { return Scene.GetGameObject().transform.GetChild(4).gameObject; }

                        public class Probes
                        {
                            public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(0).gameObject; }

                            public class LightProbes
                            {
                                public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(0).gameObject; }

                                public class MainLightProbeGroup
                                {
                                    public static GameObject GetGameObject() { return LightProbes.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class ReflectionProbe2
                            {
                                public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class ReflectionProbe0
                            {
                                public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class ReflectionProbe1
                            {
                                public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class ReflectionProbe3
                            {
                                public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class ReflectionProbe4
                            {
                                public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(5).gameObject; }
                            }

                            public class ReflectionProbe5
                            {
                                public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(6).gameObject; }
                            }

                            public class ReflectionProbe6
                            {
                                public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(7).gameObject; }
                            }

                            public class ReflectionProbe7
                            {
                                public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(8).gameObject; }
                            }

                            public class ReflectionProbe8
                            {
                                public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(9).gameObject; }
                            }

                            public class ReflectionProbe9
                            {
                                public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(10).gameObject; }
                            }

                            public class ReflectionProbe10
                            {
                                public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(11).gameObject; }
                            }
                        }

                        public class DirectionalLight
                        {
                            public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class Skylight
                        {
                            public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(2).gameObject; }
                        }

                        public class VisualEffects
                        {
                            public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(3).gameObject; }

                            public class WindVelocitySource
                            {
                                public static GameObject GetGameObject() { return VisualEffects.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class WindStreakVFX
                            {
                                public static GameObject GetGameObject() { return VisualEffects.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class FallingLeafVFXs
                            {
                                public static GameObject GetGameObject() { return VisualEffects.GetGameObject().transform.GetChild(2).gameObject; }

                                public class FallingLeaves0
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class FallingLeaves1
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class FallingLeaves2
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class FallingLeaves3
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class FallingLeaves4
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class FallingLeaves5
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class FallingLeaves6
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(6).gameObject; }
                                }

                                public class FallingLeaves7
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(7).gameObject; }
                                }

                                public class FallingLeaves8
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(8).gameObject; }
                                }

                                public class FallingLeaves9
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(9).gameObject; }
                                }

                                public class FallingLeaves10
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(10).gameObject; }
                                }

                                public class FallingLeaves11
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(11).gameObject; }
                                }

                                public class FallingLeaves12
                                {
                                    public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(12).gameObject; }
                                }
                            }
                        }

                        public class ExtraLights
                        {
                            public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(4).gameObject; }

                            public class Omni0
                            {
                                public static GameObject GetGameObject() { return ExtraLights.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class Omni1
                            {
                                public static GameObject GetGameObject() { return ExtraLights.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class Omni2
                            {
                                public static GameObject GetGameObject() { return ExtraLights.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class Omni3
                            {
                                public static GameObject GetGameObject() { return ExtraLights.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class Spot
                            {
                                public static GameObject GetGameObject() { return ExtraLights.GetGameObject().transform.GetChild(4).gameObject; }
                            }
                        }
                    }
                }

                public class Logic
                {
                    public static GameObject GetGameObject() { return allBaseGymGameObjects[3]; }

                    public class SlabbuddyMenuVariant
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(0).gameObject; }

                        public class MenuForm
                        {
                            public static GameObject GetGameObject() { return SlabbuddyMenuVariant.GetGameObject().transform.GetChild(0).gameObject; }

                            public class Base
                            {
                                public static GameObject GetGameObject() { return MenuForm.GetGameObject().transform.GetChild(0).gameObject; }

                                public class BaseMesh
                                {
                                    public static GameObject GetGameObject() { return Base.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class ControlsSlab
                                {
                                    public static GameObject GetGameObject() { return Base.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class VisualsSlab
                                {
                                    public static GameObject GetGameObject() { return Base.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class AudioSlab
                                {
                                    public static GameObject GetGameObject() { return Base.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class VoiceSlab
                                {
                                    public static GameObject GetGameObject() { return Base.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class SocialSlab
                                {
                                    public static GameObject GetGameObject() { return Base.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class PageSlab
                                {
                                    public static GameObject GetGameObject() { return Base.GetGameObject().transform.GetChild(6).gameObject; }
                                }
                            }

                            public class SwapPageSFX
                            {
                                public static GameObject GetGameObject() { return MenuForm.GetGameObject().transform.GetChild(1).gameObject; }
                            }
                        }
                    }

                    public class School
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(1).gameObject; }

                        public class LogoSlab
                        {
                            public static GameObject GetGameObject() { return School.GetGameObject().transform.GetChild(0).gameObject; }

                            public class NotificationSlab
                            {
                                public static GameObject GetGameObject() { return LogoSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                public class SlabbuddyInfoVariant
                                {
                                    public static GameObject GetGameObject() { return NotificationSlab.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class SlabNotification
                            {
                                public static GameObject GetGameObject() { return LogoSlab.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class SlabAutoState
                            {
                                public static GameObject GetGameObject() { return LogoSlab.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class SpawnTransform
                            {
                                public static GameObject GetGameObject() { return LogoSlab.GetGameObject().transform.GetChild(3).gameObject; }
                            }
                        }

                        public class ShowLogoButton
                        {
                            public static GameObject GetGameObject() { return School.GetGameObject().transform.GetChild(1).gameObject; }

                            public class SlabRockFloat
                            {
                                public static GameObject GetGameObject() { return ShowLogoButton.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class InteractionButton
                            {
                                public static GameObject GetGameObject() { return ShowLogoButton.GetGameObject().transform.GetChild(1).gameObject; }

                                public class Button
                                {
                                    public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class LeftHandle
                                    {
                                        public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class RightHandle
                                    {
                                        public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Spring
                                    {
                                        public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }
                            }

                            public class ShowLogoText
                            {
                                public static GameObject GetGameObject() { return ShowLogoButton.GetGameObject().transform.GetChild(2).gameObject; }
                            }
                        }

                        public class OptOutButton
                        {
                            public static GameObject GetGameObject() { return School.GetGameObject().transform.GetChild(2).gameObject; }

                            public class SlabRockFloat
                            {
                                public static GameObject GetGameObject() { return OptOutButton.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class ShowLogoText
                            {
                                public static GameObject GetGameObject() { return OptOutButton.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class InteractionButtonLongPressVariant
                            {
                                public static GameObject GetGameObject() { return OptOutButton.GetGameObject().transform.GetChild(2).gameObject; }

                                public class Button
                                {
                                    public static GameObject GetGameObject() { return InteractionButtonLongPressVariant.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class LeftHandle
                                    {
                                        public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class RightHandle
                                    {
                                        public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Spring
                                    {
                                        public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }
                            }
                        }
                    }

                    public class FightHouse
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(2).gameObject; }

                        public class InsideAudioTrigger
                        {
                            public static GameObject GetGameObject() { return FightHouse.GetGameObject().transform.GetChild(0).gameObject; }
                        }
                    }

                    public class HeinhouserProducts
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(3).gameObject; }

                        public class MatchConsole
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(0).gameObject; }

                            public class ConsoleBody
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class ConsoleDoor
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class ConsoleFan
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class Radio
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class Lights
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.gameObject; }
                            }

                            public class ConsoleCables
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(5).gameObject; }
                            }

                            public class ConsoleDangerSign
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(6).gameObject; }
                            }

                            public class ConsoleInteractionLever
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(7).gameObject; }
                            }

                            public class ConsoleTape
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(8).gameObject; }
                            }

                            public class Bell
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(9).gameObject; }
                            }

                            public class MatchConsoleColiders
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(10).gameObject; }
                            }

                            public class Screen
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(11).gameObject; }
                            }

                            public class TutorialChecklist
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(12).gameObject; }
                            }

                            public class RankRelaxControls
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(13).gameObject; }
                            }

                            public class MatchmakeConsoleTrigger
                            {
                                public static GameObject GetGameObject() { return MatchConsole.GetGameObject().transform.GetChild(14).gameObject; }
                            }
                        }

                        public class RegionSelector
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(1).gameObject; }

                            public class Model
                            {
                                public static GameObject GetGameObject() { return RegionSelector.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class HowardConsole
                            {
                                public static GameObject GetGameObject() { return RegionSelector.GetGameObject().transform.GetChild(1).gameObject; }
                            }
                        }

                        public class BeltRack
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(2).gameObject; }

                            public class Belt0
                            {
                                public static GameObject GetGameObject() { return BeltRack.GetGameObject().transform.GetChild(0).gameObject; }

                                public class Mesh
                                {
                                    public static GameObject GetGameObject() { return Belt0.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class Belt1
                            {
                                public static GameObject GetGameObject() { return BeltRack.GetGameObject().transform.GetChild(1).gameObject; }

                                public class Mesh
                                {
                                    public static GameObject GetGameObject() { return Belt1.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class Belt2
                            {
                                public static GameObject GetGameObject() { return BeltRack.GetGameObject().transform.GetChild(2).gameObject; }

                                public class Mesh
                                {
                                    public static GameObject GetGameObject() { return Belt2.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class Belt3
                            {
                                public static GameObject GetGameObject() { return BeltRack.GetGameObject().transform.GetChild(3).gameObject; }

                                public class Mesh
                                {
                                    public static GameObject GetGameObject() { return Belt3.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class Belt4
                            {
                                public static GameObject GetGameObject() { return BeltRack.GetGameObject().transform.GetChild(4).gameObject; }

                                public class Mesh
                                {
                                    public static GameObject GetGameObject() { return Belt4.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class Belt5
                            {
                                public static GameObject GetGameObject() { return BeltRack.GetGameObject().transform.GetChild(5).gameObject; }

                                public class Mesh
                                {
                                    public static GameObject GetGameObject() { return Belt5.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class NewBeltVFX
                            {
                                public static GameObject GetGameObject() { return BeltRack.GetGameObject().transform.GetChild(6).gameObject; }
                            }

                            public class BeltUnlockVFX
                            {
                                public static GameObject GetGameObject() { return BeltRack.GetGameObject().transform.GetChild(7).gameObject; }
                            }
                        }

                        public class Telephone
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(3).gameObject; }

                            public class FriendScreen
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class Frame
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class SettingsScreen
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class PlayerFinderScreen
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class RecentScreen
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class NotificationScreen
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(5).gameObject; }
                            }

                            public class InteractionSlider1
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(6).gameObject; }
                            }

                            public class InteractionSlider0
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(7).gameObject; }
                            }

                            public class PhoneProp
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(8).gameObject; }
                            }

                            public class IDButton
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(9).gameObject; }
                            }

                            public class OfflineModeSlab
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(10).gameObject; }
                            }

                            public class TelephoneLightingPatch
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(11).gameObject; }
                            }
                        }

                        public class Parkboard
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(4).gameObject; }

                            public class Model
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class RotatingScreen
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class PrimaryDisplay
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class PlayerRelocationTrigger
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class OfflineModeSlab
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class TutorialChecklist
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(5).gameObject; }
                            }

                            public class ParkSearchUnavailableSlab
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(6).gameObject; }
                            }
                        }

                        public class HowardRoot
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(5).gameObject; }

                            public class RingOrigin
                            {
                                public static GameObject GetGameObject() { return HowardRoot.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class Props
                            {
                                public static GameObject GetGameObject() { return HowardRoot.GetGameObject().transform.GetChild(1).gameObject; }

                                public class Storage
                                {
                                    public static GameObject GetGameObject() { return Props.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Crate4
                                {
                                    public static GameObject GetGameObject() { return Props.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class Crate5
                                {
                                    public static GameObject GetGameObject() { return Props.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class Crate6
                                {
                                    public static GameObject GetGameObject() { return Props.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class Crate7
                                {
                                    public static GameObject GetGameObject() { return Props.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class Crate8
                                {
                                    public static GameObject GetGameObject() { return Props.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class ExtraArm
                                {
                                    public static GameObject GetGameObject() { return Props.GetGameObject().transform.GetChild(6).gameObject; }
                                }
                            }

                            public class PlayerDetection
                            {
                                public static GameObject GetGameObject() { return HowardRoot.GetGameObject().transform.GetChild(2).gameObject; }

                                public class Origin0
                                {
                                    public static GameObject GetGameObject() { return PlayerDetection.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Origin1
                                {
                                    public static GameObject GetGameObject() { return PlayerDetection.GetGameObject().transform.GetChild(1).gameObject; }
                                }
                            }

                            public class DummyRoot
                            {
                                public static GameObject GetGameObject() { return HowardRoot.GetGameObject().transform.GetChild(3).gameObject; }

                                public class Howard
                                {
                                    public static GameObject GetGameObject() { return DummyRoot.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class StructureCleanupOrigin
                                {
                                    public static GameObject GetGameObject() { return DummyRoot.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class HowardMovementSparks
                                {
                                    public static GameObject GetGameObject() { return DummyRoot.GetGameObject().transform.GetChild(2).gameObject; }
                                }
                            }

                            public class HowardsConsole
                            {
                                public static GameObject GetGameObject() { return HowardRoot.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class TutorialChecklist
                            {
                                public static GameObject GetGameObject() { return HowardRoot.GetGameObject().transform.GetChild(5).gameObject; }
                            }
                        }

                        public class MoveLearning
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(6).gameObject; }

                            public class GhostPosition
                            {
                                public static GameObject GetGameObject() { return MoveLearning.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class Ghost
                            {
                                public static GameObject GetGameObject() { return MoveLearning.GetGameObject().transform.GetChild(1).gameObject; }

                                public class Visuals
                                {
                                    public static GameObject GetGameObject() { return Ghost.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class HandLPoseGhost
                                    {
                                        public static GameObject GetGameObject() { return Visuals.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class HandRPoseGhost
                                    {
                                        public static GameObject GetGameObject() { return Visuals.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class RIG
                                    {
                                        public static GameObject GetGameObject() { return Visuals.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }
                            }

                            public class Timer
                            {
                                public static GameObject GetGameObject() { return MoveLearning.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class MoveLearnSelector
                            {
                                public static GameObject GetGameObject() { return MoveLearning.GetGameObject().transform.GetChild(3).gameObject; }

                                public class PosesChangedNotification
                                {
                                    public static GameObject GetGameObject() { return MoveLearnSelector.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Model
                                {
                                    public static GameObject GetGameObject() { return MoveLearnSelector.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class PoseDisplaySelector
                                {
                                    public static GameObject GetGameObject() { return MoveLearnSelector.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class PageSelector
                                {
                                    public static GameObject GetGameObject() { return MoveLearnSelector.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class TotemPedistalCompact
                                {
                                    public static GameObject GetGameObject() { return MoveLearnSelector.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class MovesIntroducedNotification
                                {
                                    public static GameObject GetGameObject() { return MoveLearnSelector.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class HowardConsole
                                {
                                    public static GameObject GetGameObject() { return MoveLearnSelector.GetGameObject().transform.GetChild(6).gameObject; }
                                }
                            }

                            public class TutorialChecklist
                            {
                                public static GameObject GetGameObject() { return MoveLearning.GetGameObject().transform.GetChild(4).gameObject; }

                                public class Canvas
                                {
                                    public static GameObject GetGameObject() { return TutorialChecklist.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class TutorialChecklist_
                                {
                                    public static GameObject GetGameObject() { return TutorialChecklist.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class DisposableCollider
                                {
                                    public static GameObject GetGameObject() { return TutorialChecklist.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class DisposeVFX
                                {
                                    public static GameObject GetGameObject() { return TutorialChecklist.GetGameObject().transform.GetChild(3).gameObject; }
                                }
                            }
                        }

                        public class Leaderboard
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(7).gameObject; }

                            public class PlayerTags
                            {
                                public static GameObject GetGameObject() { return Leaderboard.GetGameObject().transform.GetChild(0).gameObject; }

                                public class HighscoreTag0
                                {
                                    public static GameObject GetGameObject() { return PlayerTags.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class PlayerTag
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag0.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Nr
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag0.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Highscore
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag0.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class HighscoreTag1
                                {
                                    public static GameObject GetGameObject() { return PlayerTags.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class PlayerTag
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag1.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Nr
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag1.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Highscore
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag1.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class HighscoreTag2
                                {
                                    public static GameObject GetGameObject() { return PlayerTags.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class PlayerTag
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag2.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Nr
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag2.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Highscore
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag2.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class HighscoreTag3
                                {
                                    public static GameObject GetGameObject() { return PlayerTags.GetGameObject().transform.GetChild(3).gameObject; }

                                    public class PlayerTag
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag3.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Nr
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag3.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Highscore
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag3.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class HighscoreTag4
                                {
                                    public static GameObject GetGameObject() { return PlayerTags.GetGameObject().transform.GetChild(4).gameObject; }

                                    public class PlayerTag
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag4.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Nr
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag4.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Highscore
                                    {
                                        public static GameObject GetGameObject() { return HighscoreTag4.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class PersonalHighscoreTag
                                {
                                    public static GameObject GetGameObject() { return PlayerTags.GetGameObject().transform.GetChild(5).gameObject; }

                                    public class PlayerTag
                                    {
                                        public static GameObject GetGameObject() { return PersonalHighscoreTag.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Nr
                                    {
                                        public static GameObject GetGameObject() { return PersonalHighscoreTag.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Highscore
                                    {
                                        public static GameObject GetGameObject() { return PersonalHighscoreTag.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }
                            }

                            public class OfflineModeSlab
                            {
                                public static GameObject GetGameObject() { return Leaderboard.GetGameObject().transform.GetChild(1).gameObject; }

                                public class InfoFormCanvas
                                {
                                    public static GameObject GetGameObject() { return OfflineModeSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class InfoText
                                    {
                                        public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                    }
                                }
                            }

                            public class Frame
                            {
                                public static GameObject GetGameObject() { return Leaderboard.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class TextObjects
                            {
                                public static GameObject GetGameObject() { return Leaderboard.GetGameObject().transform.GetChild(3).gameObject; }

                                public class TitlePlate
                                {
                                    public static GameObject GetGameObject() { return TextObjects.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class LeaderboardTitlePlate
                                    {
                                        public static GameObject GetGameObject() { return TitlePlate.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Leaderboard
                                    {
                                        public static GameObject GetGameObject() { return TitlePlate.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }

                                public class PlayerPlate
                                {
                                    public static GameObject GetGameObject() { return TextObjects.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class LeaderboardTitlePlate
                                    {
                                        public static GameObject GetGameObject() { return PlayerPlate.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Leaderboard
                                    {
                                        public static GameObject GetGameObject() { return PlayerPlate.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }

                                public class PositionPlate
                                {
                                    public static GameObject GetGameObject() { return TextObjects.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class LeaderboardTitlePlate
                                    {
                                        public static GameObject GetGameObject() { return allBaseGymGameObjects[7].transform.GetChild(3).GetChild(8).GetChild(3).GetChild(2).GetChild(0).gameObject; }
                                    }

                                    public class Leaderboard
                                    {
                                        public static GameObject GetGameObject() { return allBaseGymGameObjects[7].transform.GetChild(3).GetChild(8).GetChild(3).GetChild(2).GetChild(1).gameObject; }
                                    }
                                }

                                public class BPPlate
                                {
                                    public static GameObject GetGameObject() { return TextObjects.GetGameObject().transform.GetChild(3).gameObject; }

                                    public class LeaderboardTitlePlate
                                    {
                                        public static GameObject GetGameObject() { return BPPlate.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Leaderboard
                                    {
                                        public static GameObject GetGameObject() { return BPPlate.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }

                                public class Divider
                                {
                                    public static GameObject GetGameObject() { return TextObjects.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class NoRecords
                                {
                                    public static GameObject GetGameObject() { return TextObjects.GetGameObject().transform.GetChild(5).gameObject; }
                                }
                            }

                            public class Colliders
                            {
                                public static GameObject GetGameObject() { return Leaderboard.GetGameObject().transform.GetChild(4).gameObject; }

                                public class LeftFoot
                                {
                                    public static GameObject GetGameObject() { return Colliders.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class RightFoot
                                {
                                    public static GameObject GetGameObject() { return Colliders.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class Roof
                                {
                                    public static GameObject GetGameObject() { return Colliders.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class Canvas
                                {
                                    public static GameObject GetGameObject() { return Colliders.GetGameObject().transform.GetChild(3).gameObject; }
                                }
                            }
                        }

                        public class RankStatusSlab
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(8).gameObject; }

                            public class StatusForm
                            {
                                public static GameObject GetGameObject() { return RankStatusSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                public class GraphicsSlab
                                {
                                    public static GameObject GetGameObject() { return StatusForm.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Mesh
                                    {
                                        public static GameObject GetGameObject() { return GraphicsSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class FloatRock0
                                        {
                                            public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class MeshGraphicsSlab
                                        {
                                            public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class FloatRock1
                                        {
                                            public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }

                                public class RankStatusFormCanvas
                                {
                                    public static GameObject GetGameObject() { return StatusForm.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class RankStatusText
                                    {
                                        public static GameObject GetGameObject() { return RankStatusFormCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class MountainRank
                                    {
                                        public static GameObject GetGameObject() { return RankStatusFormCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class RankNames
                                    {
                                        public static GameObject GetGameObject() { return RankStatusFormCanvas.GetGameObject().transform.GetChild(2).gameObject; }

                                        public class CurrRank
                                        {
                                            public static GameObject GetGameObject() { return RankNames.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ToRank
                                        {
                                            public static GameObject GetGameObject() { return RankNames.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class RankUpText
                                    {
                                        public static GameObject GetGameObject() { return RankStatusFormCanvas.GetGameObject().transform.GetChild(3).gameObject; }

                                        public class TextRock0
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class TextRock1
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class TextRock2
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                        public class TextRock3
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(3).gameObject; }
                                        }

                                        public class TextRock4
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(4).gameObject; }
                                        }

                                        public class TextRock5
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(5).gameObject; }
                                        }

                                        public class TextRock6
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(6).gameObject; }
                                        }

                                        public class TextRock7
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(7).gameObject; }
                                        }

                                        public class TextRock8
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(8).gameObject; }
                                        }

                                        public class TextRock9
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(9).gameObject; }
                                        }

                                        public class TextRock10
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(10).gameObject; }
                                        }

                                        public class TextRock11
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(11).gameObject; }
                                        }

                                        public class TextRock12
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(12).gameObject; }
                                        }

                                        public class TextRock13
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(13).gameObject; }
                                        }

                                        public class TextRock14
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(14).gameObject; }
                                        }

                                        public class TextRock15
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(15).gameObject; }
                                        }

                                        public class TextRock16
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(16).gameObject; }
                                        }

                                        public class TextRock17
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(17).gameObject; }
                                        }

                                        public class TextRock18
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(18).gameObject; }
                                        }

                                        public class TextRock19
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(19).gameObject; }
                                        }

                                        public class TextRock20
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(20).gameObject; }
                                        }

                                        public class TextRock21
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(21).gameObject; }
                                        }
                                    }

                                    public class RankUpBar
                                    {
                                        public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class XPGainVFX
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class LevelUpVFX
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class Bar
                                        {
                                            public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }
                            }

                            public class SlabAutoState
                            {
                                public static GameObject GetGameObject() { return RankStatusSlab.GetGameObject().transform.GetChild(1).gameObject; }
                            }
                        }

                        public class CommunitySlab
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(9).gameObject; }

                            public class DiscordButton
                            {
                                public static GameObject GetGameObject() { return CommunitySlab.GetGameObject().transform.GetChild(0).gameObject; }

                                public class ButtonRock
                                {
                                    public static GameObject GetGameObject() { return DiscordButton.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class DiscordText
                                {
                                    public static GameObject GetGameObject() { return DiscordButton.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class InteractionButton
                                {
                                    public static GameObject GetGameObject() { return DiscordButton.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class Spring
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }
                            }

                            public class FeedbackButton
                            {
                                public static GameObject GetGameObject() { return CommunitySlab.GetGameObject().transform.GetChild(1).gameObject; }

                                public class ButtonRock
                                {
                                    public static GameObject GetGameObject() { return FeedbackButton.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class DiscordText
                                {
                                    public static GameObject GetGameObject() { return FeedbackButton.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class InteractionButton
                                {
                                    public static GameObject GetGameObject() { return FeedbackButton.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class Spring
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }
                            }

                            public class BaseSlab
                            {
                                public static GameObject GetGameObject() { return CommunitySlab.GetGameObject().transform.GetChild(2).gameObject; }

                                public class SlabRock
                                {
                                    public static GameObject GetGameObject() { return BaseSlab.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class TextUnderline
                                {
                                    public static GameObject GetGameObject() { return BaseSlab.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class TextField
                                {
                                    public static GameObject GetGameObject() { return BaseSlab.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class TitleField
                                {
                                    public static GameObject GetGameObject() { return BaseSlab.GetGameObject().transform.GetChild(3).gameObject; }
                                }
                            }
                        }

                        public class ShiftstoneQuickswapper
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(10).gameObject; }

                            public class FloatingButton
                            {
                                public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(0).gameObject; }

                                public class Rock
                                {
                                    public static GameObject GetGameObject() { return FloatingButton.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Text
                                {
                                    public static GameObject GetGameObject() { return FloatingButton.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class InteractionButtonToggleVariant
                                {
                                    public static GameObject GetGameObject() { return FloatingButton.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return InteractionButtonToggleVariant.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class Spring
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }
                            }

                            public class LeftHandSlab
                            {
                                public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(1).gameObject; }

                                public class InfoFormCanvas
                                {
                                    public static GameObject GetGameObject() { return LeftHandSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Title
                                    {
                                        public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Icon
                                    {
                                        public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Description
                                    {
                                        public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class TitleUnderline
                                    {
                                        public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class LeftHandText
                                    {
                                        public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(4).gameObject; }
                                    }
                                }
                            }

                            public class RightHandSlab
                            {
                                public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(2).gameObject; }

                                public class InfoFormCanvas
                                {
                                    public static GameObject GetGameObject() { return RightHandSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Title
                                    {
                                        public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Icon
                                    {
                                        public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Description
                                    {
                                        public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class TitleUnderline
                                    {
                                        public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class LeftHandText
                                    {
                                        public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(4).gameObject; }
                                    }
                                }
                            }

                            public class LeftHandToolTip
                            {
                                public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(3).gameObject; }

                                public class ToolTipCanvas
                                {
                                    public static GameObject GetGameObject() { return LeftHandToolTip.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipSpriteCanvas
                                    {
                                        public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipTextBackground
                                        {
                                            public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ToolTipArrowIconPositive
                                        {
                                            public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class ToolTipArrowIconNegative
                                        {
                                            public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }

                                    public class ToolTipTextCanvas
                                    {
                                        public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class ToolTipTextComponent
                                        {
                                            public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                        }
                                    }
                                }
                            }

                            public class RightHandToolTip
                            {
                                public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(4).gameObject; }

                                public class ToolTipCanvas
                                {
                                    public static GameObject GetGameObject() { return RightHandToolTip.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipSpriteCanvas
                                    {
                                        public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class ToolTipTextBackground
                                        {
                                            public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ToolTipArrowIconPositive
                                        {
                                            public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class ToolTipArrowIconNegative
                                        {
                                            public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }

                                    public class ToolTipTextCanvas
                                    {
                                        public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class ToolTipTextComponent
                                        {
                                            public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                        }
                                    }
                                }
                            }

                            public class ShiftstoneButtonVFX
                            {
                                public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(5).gameObject; }
                            }
                        }

                        public class ShiftstoneCabinet
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(11).gameObject; }

                            public class NoSwapsAllowed
                            {
                                public static GameObject GetGameObject() { return ShiftstoneCabinet.GetGameObject().transform.GetChild(0).gameObject; }

                                public class InfoForm
                                {
                                    public static GameObject GetGameObject() { return NoSwapsAllowed.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class InfoSlab
                                    {
                                        public static GameObject GetGameObject() { return InfoForm.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class InfoSlab_
                                        {
                                            public static GameObject GetGameObject() { return InfoSlab.GetGameObject().transform.GetChild(0).gameObject; }
                                        }
                                    }

                                    public class InfoFormCanvas
                                    {
                                        public static GameObject GetGameObject() { return InfoForm.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class EnterText
                                        {
                                            public static GameObject GetGameObject() { return InfoForm.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ExitText
                                        {
                                            public static GameObject GetGameObject() { return InfoForm.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }
                            }

                            public class Cabinet
                            {
                                public static GameObject GetGameObject() { return ShiftstoneCabinet.GetGameObject().transform.GetChild(1).gameObject; }

                                public class ShiftstoneBox00
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class AdamantStone
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return AdamantStone.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return AdamantStone.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class VigorStone_
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return VigorStone_.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return VigorStone_.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                }

                                public class ShiftstoneBox01
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox01.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox01.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox01.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class ChargeStone
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox01.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return ChargeStone.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return ChargeStone.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class GuardStone_
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox01.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return GuardStone_.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return GuardStone_.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }

                                public class ShiftstoneBox02
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox02.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox02.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox02.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class FlowStone
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox02.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return FlowStone.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return FlowStone.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }

                                public class ShiftstoneBox03
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(3).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox03.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox03.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox03.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class GuardStone
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox03.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return GuardStone.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return GuardStone.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class StubbornStone_
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox03.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return StubbornStone_.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return StubbornStone_.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }

                                public class ShiftstoneBox04
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(4).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox04.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox04.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox04.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class StubbornStone
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox04.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return StubbornStone.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return StubbornStone.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class ChargeStone_
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox04.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return ChargeStone_.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return ChargeStone_.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }

                                public class ShiftstoneBox05
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(5).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox05.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox05.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox05.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class SurgeStone
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox05.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return SurgeStone.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return SurgeStone.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }

                                public class ShiftstoneBox06
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(6).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox06.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox06.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox06.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class VigorStone
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox06.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return VigorStone.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return VigorStone.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class SurgeStone
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox06.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return SurgeStone.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return SurgeStone.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }

                                public class ShiftstoneBox07
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(7).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox07.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox07.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox07.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class VolatileStone
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox07.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return VolatileStone.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return VolatileStone.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class AdamantStone
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox07.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return AdamantStone.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class ShiftstoneTriggerVFX
                                        {
                                            public static GameObject GetGameObject() { return AdamantStone.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }

                                public class ShiftstoneBox08
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(8).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox08.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox08.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox08.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                    
                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox09
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(9).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox09.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox09.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox09.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox10
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(10).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox10.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox10.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox10.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox11
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(11).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox11.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox11.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox11.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox12
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(12).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox12.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox12.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox12.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox13
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(13).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox13.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox13.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox13.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox14
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(14).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox14.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox14.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox14.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox15
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(15).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox15.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox15.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox15.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox16
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(16).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox16.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox16.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox16.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox17
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(17).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox17.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox17.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox17.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox18
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(18).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox18.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox18.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox18.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox19
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(19).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox19.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox19.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox19.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox20
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(20).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox20.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox20.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox20.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox21
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(21).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox21.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox21.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox21.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox22
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(22).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox22.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox22.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox22.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox23
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(23).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox23.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox23.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox23.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox24
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(24).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox24.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox24.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox24.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox25
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(25).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox25.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox25.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox25.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox26
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(26).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox26.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox26.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox26.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox27
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(27).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox27.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox27.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox27.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox28
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(28).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox28.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox28.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox28.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class ShiftstoneBox29
                                {
                                    public static GameObject GetGameObject() { return Cabinet.GetGameObject().transform.GetChild(29).gameObject; }

                                    public class ShiftstoneNameText
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox29.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox29.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class EquippedIcon
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox29.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class AttentionPoint
                                    {
                                        public static GameObject GetGameObject() { return ShiftstoneBox00.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }
                            }

                            public class Cases
                            {
                                public static GameObject GetGameObject() { return ShiftstoneCabinet.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class Glass
                            {
                                public static GameObject GetGameObject() { return ShiftstoneCabinet.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class Glassplane
                            {
                                public static GameObject GetGameObject() { return ShiftstoneCabinet.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class LeftShiftstoneTutorialLocation
                            {
                                public static GameObject GetGameObject() { return ShiftstoneCabinet.GetGameObject().transform.GetChild(5).gameObject; }
                            }

                            public class RightShiftstoneTutorialLocation
                            {
                                public static GameObject GetGameObject() { return ShiftstoneCabinet.GetGameObject().transform.GetChild(6).gameObject; }
                            }
                        }

                        public class Gondola
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(12).gameObject; }

                            public class Cabin
                            {
                                public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class Step
                            {
                                public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class Wheel
                            {
                                public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class Bell
                            {
                                public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class WalkableFloor0
                            {
                                public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class Cylinder
                            {
                                public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(5).gameObject; }
                            }

                            public class WalkableFloor1
                            {
                                public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(6).gameObject; }
                            }

                            public class ReflectionProbe0
                            {
                                public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(7).gameObject; }
                            }

                            public class ReflectionProbe1
                            {
                                public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(8).gameObject; }
                            }
                        }

                        public class RankBoard
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(13).gameObject; }

                            public class Gravel
                            {
                                public static GameObject GetGameObject() { return RankBoard.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class Pebble
                            {
                                public static GameObject GetGameObject() { return RankBoard.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class Boulder
                            {
                                public static GameObject GetGameObject() { return RankBoard.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class Tor
                            {
                                public static GameObject GetGameObject() { return RankBoard.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class Monolith
                            {
                                public static GameObject GetGameObject() { return RankBoard.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class Mountain
                            {
                                public static GameObject GetGameObject() { return RankBoard.GetGameObject().transform.GetChild(5).gameObject; }
                            }
                        }
                    }

                    public class Toys
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(4).gameObject; }

                        public class Bag
                        {
                            public static GameObject GetGameObject() { return Toys.GetGameObject().transform.GetChild(0).gameObject; }

                            public class Bag_
                            {
                                public static GameObject GetGameObject() { return Bag.GetGameObject().transform.GetChild(0).gameObject; }

                                public class Bone000
                                {
                                    public static GameObject GetGameObject() { return Bag_.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Bone001
                                {
                                    public static GameObject GetGameObject() { return Bag_.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class Bone002
                                {
                                    public static GameObject GetGameObject() { return Bag_.GetGameObject().transform.GetChild(2).gameObject; }
                                }
                            }

                            public class Mesh
                            {
                                public static GameObject GetGameObject() { return Bag.GetGameObject().transform.GetChild(1).gameObject; }
                            }
                        }

                        public class Tetherball0
                        {
                            public static GameObject GetGameObject() { return Toys.GetGameObject().transform.GetChild(1).gameObject; }

                            public class Ball
                            {
                                public static GameObject GetGameObject() { return Tetherball0.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class InteractionSlider
                            {
                                public static GameObject GetGameObject() { return Tetherball0.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class Line
                            {
                                public static GameObject GetGameObject() { return Tetherball0.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class LineRig
                            {
                                public static GameObject GetGameObject() { return Tetherball0.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class Collision
                            {
                                public static GameObject GetGameObject() { return Tetherball0.GetGameObject().transform.GetChild(4).gameObject; }
                            }
                        }

                        public class Tetherball1
                        {
                            public static GameObject GetGameObject() { return Toys.GetGameObject().transform.GetChild(2).gameObject; }

                            public class Ball
                            {
                                public static GameObject GetGameObject() { return Tetherball1.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class InteractionSlider
                            {
                                public static GameObject GetGameObject() { return Tetherball1.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class Line
                            {
                                public static GameObject GetGameObject() { return Tetherball1.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class LineRig
                            {
                                public static GameObject GetGameObject() { return allBaseGymGameObjects[7].transform.GetChild(4).GetChild(2).GetChild(3).gameObject; }
                            }

                            public class Collision
                            {
                                public static GameObject GetGameObject() { return Tetherball1.GetGameObject().transform.GetChild(4).gameObject; }
                            }
                        }

                        public class SmallRockSpawns
                        {
                            public static GameObject GetGameObject() { return Toys.GetGameObject().transform.GetChild(3).gameObject; }
                        }

                        public class LargeRockSpawns
                        {
                            public static GameObject GetGameObject() { return Toys.GetGameObject().transform.GetChild(4).gameObject; }
                        }

                        public class Targets
                        {
                            public static GameObject GetGameObject() { return Toys.GetGameObject().transform.GetChild(5).gameObject; }
                        }
                    }

                    public class Handlers
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(5).gameObject; }

                        public class SpawnPointHandler
                        {
                            public static GameObject GetGameObject() { return Handlers.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class MatchmakingHandler
                        {
                            public static GameObject GetGameObject() { return Handlers.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class RewardHandler
                        {
                            public static GameObject GetGameObject() { return Handlers.GetGameObject().transform.GetChild(2).gameObject; }
                        }
                    }

                    public class Notifications
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(6).gameObject; }

                        public class NotificationSlabGondola
                        {
                            public static GameObject GetGameObject() { return Notifications.GetGameObject().transform.GetChild(0).gameObject; }

                            public class NotificationSlab
                            {
                                public static GameObject GetGameObject() { return NotificationSlabGondola.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class SlabNotification
                            {
                                public static GameObject GetGameObject() { return NotificationSlabGondola.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class SlabAutoState
                            {
                                public static GameObject GetGameObject() { return NotificationSlabGondola.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class SpawnTransform
                            {
                                public static GameObject GetGameObject() { return NotificationSlabGondola.GetGameObject().transform.GetChild(3).gameObject; }
                            }
                        }

                        public class NotificationSlabOther
                        {
                            public static GameObject GetGameObject() { return Notifications.GetGameObject().transform.GetChild(1).gameObject; }

                            public class NotificationSlab
                            {
                                public static GameObject GetGameObject() { return NotificationSlabOther.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class SlabNotification
                            {
                                public static GameObject GetGameObject() { return NotificationSlabOther.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class SlabAutoState
                            {
                                public static GameObject GetGameObject() { return NotificationSlabOther.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class SpawnTransform
                            {
                                public static GameObject GetGameObject() { return NotificationSlabOther.GetGameObject().transform.GetChild(3).gameObject; }
                            }
                        }

                        public class NotificationSlabRank
                        {
                            public static GameObject GetGameObject() { return Notifications.GetGameObject().transform.GetChild(2).gameObject; }

                            public class SlabNotification
                            {
                                public static GameObject GetGameObject() { return NotificationSlabRank.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class NotificationSlab
                            {
                                public static GameObject GetGameObject() { return NotificationSlabRank.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class SlabAutoState
                            {
                                public static GameObject GetGameObject() { return NotificationSlabRank.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class SpawnTransform
                            {
                                public static GameObject GetGameObject() { return NotificationSlabRank.GetGameObject().transform.GetChild(3).gameObject; }
                            }
                        }
                    }

                    public class Locations
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(7).gameObject; }

                        public class Gym
                        {
                            public static GameObject GetGameObject() { return Locations.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class Gondola
                        {
                            public static GameObject GetGameObject() { return Locations.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class MatchmakeConsole
                        {
                            public static GameObject GetGameObject() { return Locations.GetGameObject().transform.GetChild(2).gameObject; }
                        }

                        public class RankLocation
                        {
                            public static GameObject GetGameObject() { return Locations.GetGameObject().transform.GetChild(3).gameObject; }
                        }
                    }

                    public class PoseGhostArea
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(8).gameObject; }

                        public class ForbiddenArea
                        {
                            public static GameObject GetGameObject() { return PoseGhostArea.GetGameObject().transform.GetChild(0).gameObject; }
                        }
                    }

                    public class Bounds
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(9).gameObject; }

                        public class SceneBoundaryPlayer
                        {
                            public static GameObject GetGameObject() { return Bounds.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class SceneBoundaryStructures
                        {
                            public static GameObject GetGameObject() { return Bounds.GetGameObject().transform.GetChild(1).gameObject; }
                        }
                    }

                    public class AutoShowNotification
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(10).gameObject; }
                    }

                    public class AutoFade
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(11).gameObject; }
                    }

                    public class BGM
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(12).gameObject; }
                    }

                    public class Ambience
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(13).gameObject; }
                    }

                    public class Analytics
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(14).gameObject; }
                    }

                    public class FriendHandler
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(15).gameObject; }
                    }

                    public class GymPostEffects
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(16).gameObject; }
                    }

                    public class VoiceLogger
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(17).gameObject; }
                    }
                }
            }

            public class Park
            {
                public static System.Collections.Generic.List<GameObject> GetBaseDDOLGameObjects() { return allBaseParkGameObjects; }

                public class Logic
                {
                    public static GameObject GetGameObject() { return allBaseParkGameObjects[0]; }

                    public class Ambience
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(0).gameObject; }
                    }

                    public class SpawnPointHandler
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(1).gameObject; }
                    }

                    public class ParkInstance
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(2).gameObject; }
                    }

                    public class Boundaries
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(3).gameObject; }

                        public class PlayerBoundary
                        {
                            public static GameObject GetGameObject() { return Boundaries.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class StructureBoundary
                        {
                            public static GameObject GetGameObject() { return Boundaries.GetGameObject().transform.GetChild(1).gameObject; }
                        }
                    }

                    public class FriendHandler
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(4).gameObject; }
                    }

                    public class Tracks
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(5).gameObject; }

                        public class Track1Slider
                        {
                            public static GameObject GetGameObject() { return Tracks.GetGameObject().transform.GetChild(0).gameObject; }

                            public class RRTrack01Gate01
                            {
                                public static GameObject GetGameObject() { return Track1Slider.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class RRTrack01Gate02
                            {
                                public static GameObject GetGameObject() { return Track1Slider.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class RRTrack01Gate03
                            {
                                public static GameObject GetGameObject() { return Track1Slider.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class RRTrack01Gate04
                            {
                                public static GameObject GetGameObject() { return Track1Slider.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class RRTrack01Gate05
                            {
                                public static GameObject GetGameObject() { return Track1Slider.GetGameObject().transform.GetChild(4).gameObject; }
                            }
                        }

                        public class Track2Roundabout
                        {
                            public static GameObject GetGameObject() { return Tracks.GetGameObject().transform.GetChild(1).gameObject; }

                            public class RRTrack02Gate01
                            {
                                public static GameObject GetGameObject() { return Track2Roundabout.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class RRTrack02Gate02
                            {
                                public static GameObject GetGameObject() { return Track2Roundabout.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class RRTrack02Gate03
                            {
                                public static GameObject GetGameObject() { return Track2Roundabout.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class RRTrack02Gate04
                            {
                                public static GameObject GetGameObject() { return Track2Roundabout.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class RRTrack02Gate05
                            {
                                public static GameObject GetGameObject() { return Track2Roundabout.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class RRTrack02Gate06
                            {
                                public static GameObject GetGameObject() { return Track2Roundabout.GetGameObject().transform.GetChild(5).gameObject; }
                            }
                        }

                        public class Track3Station
                        {
                            public static GameObject GetGameObject() { return Tracks.GetGameObject().transform.GetChild(2).gameObject; }

                            public class RRTrack03Gate01
                            {
                                public static GameObject GetGameObject() { return Track3Station.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class RRTrack03Gate02
                            {
                                public static GameObject GetGameObject() { return Track3Station.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class RRTrack03Gate03
                            {
                                public static GameObject GetGameObject() { return Track3Station.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class RRTrack03Gate04
                            {
                                public static GameObject GetGameObject() { return Track3Station.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class RRTrack03Gate05
                            {
                                public static GameObject GetGameObject() { return Track3Station.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class RRTrack03Gate06
                            {
                                public static GameObject GetGameObject() { return Track3Station.GetGameObject().transform.GetChild(5).gameObject; }
                            }
                        }

                        public class Track4Mountain
                        {
                            public static GameObject GetGameObject() { return Tracks.GetGameObject().transform.GetChild(3).gameObject; }

                            public class RRTrack04Gate01
                            {
                                public static GameObject GetGameObject() { return Track4Mountain.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class RRTrack04Gate02
                            {
                                public static GameObject GetGameObject() { return Track4Mountain.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class RRTrack04Gate03
                            {
                                public static GameObject GetGameObject() { return Track4Mountain.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class RRTrack04Gate04
                            {
                                public static GameObject GetGameObject() { return Track4Mountain.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class RRTrack04Gate05
                            {
                                public static GameObject GetGameObject() { return Track4Mountain.GetGameObject().transform.GetChild(4).gameObject; }
                            }
                        }

                        public class Track5Helix
                        {
                            public static GameObject GetGameObject() { return Tracks.GetGameObject().transform.GetChild(4).gameObject; }

                            public class RRTrack05Gate01
                            {
                                public static GameObject GetGameObject() { return Track5Helix.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class RRTrack05Gate02
                            {
                                public static GameObject GetGameObject() { return Track5Helix.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class RRTrack05Gate03
                            {
                                public static GameObject GetGameObject() { return Track5Helix.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class RRTrack05Gate04
                            {
                                public static GameObject GetGameObject() { return Track5Helix.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class RRTrack05Gate05
                            {
                                public static GameObject GetGameObject() { return Track5Helix.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class RRTrack05Gate06
                            {
                                public static GameObject GetGameObject() { return Track5Helix.GetGameObject().transform.GetChild(5).gameObject; }
                            }

                            public class RRTrack05Gate07
                            {
                                public static GameObject GetGameObject() { return Track5Helix.GetGameObject().transform.GetChild(6).gameObject; }
                            }

                            public class RRTrack05Gate08
                            {
                                public static GameObject GetGameObject() { return Track5Helix.GetGameObject().transform.GetChild(7).gameObject; }
                            }
                        }
                    }

                    public class LargeRockSpawners
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(6).gameObject; }

                        public class LargeRockSpawnerInstant0
                        {
                            public static GameObject GetGameObject() { return LargeRockSpawners.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class LargeRockSpawnerInstant1
                        {
                            public static GameObject GetGameObject() { return LargeRockSpawners.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class LargeRockSpawnerInstant2
                        {
                            public static GameObject GetGameObject() { return LargeRockSpawners.GetGameObject().transform.GetChild(2).gameObject; }
                        }

                        public class LargeRockSpawnerInstant3
                        {
                            public static GameObject GetGameObject() { return LargeRockSpawners.GetGameObject().transform.GetChild(3).gameObject; }
                        }

                        public class LargeRockSpawnerInstant4
                        {
                            public static GameObject GetGameObject() { return LargeRockSpawners.GetGameObject().transform.GetChild(4).gameObject; }
                        }

                        public class LargeRockSpawnerInstant5
                        {
                            public static GameObject GetGameObject() { return LargeRockSpawners.GetGameObject().transform.GetChild(5).gameObject; }
                        }

                        public class LargeRockSpawnerInstant6
                        {
                            public static GameObject GetGameObject() { return LargeRockSpawners.GetGameObject().transform.GetChild(6).gameObject; }
                        }

                        public class LargeRockSpawnerInstant7
                        {
                            public static GameObject GetGameObject() { return LargeRockSpawners.GetGameObject().transform.GetChild(7).gameObject; }
                        }

                        public class LargeRockSpawnerInstant8
                        {
                            public static GameObject GetGameObject() { return LargeRockSpawners.GetGameObject().transform.GetChild(8).gameObject; }
                        }

                        public class LargeRockSpawnerInstant9
                        {
                            public static GameObject GetGameObject() { return LargeRockSpawners.GetGameObject().transform.GetChild(9).gameObject; }
                        }

                        public class LargeRockSpawnerInstant10
                        {
                            public static GameObject GetGameObject() { return LargeRockSpawners.GetGameObject().transform.GetChild(10).gameObject; }
                        }

                        public class LargeRockSpawnerInstant11
                        {
                            public static GameObject GetGameObject() { return LargeRockSpawners.GetGameObject().transform.GetChild(11).gameObject; }
                        }
                    }

                    public class BoulderballSpawners
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(7).gameObject; }

                        public class BoulderballSpawnersInstant0
                        {
                            public static GameObject GetGameObject() { return BoulderballSpawners.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class BoulderballSpawnersInstant1
                        {
                            public static GameObject GetGameObject() { return BoulderballSpawners.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class BoulderballSpawnersInstant2
                        {
                            public static GameObject GetGameObject() { return BoulderballSpawners.GetGameObject().transform.GetChild(2).gameObject; }
                        }

                        public class BoulderballSpawnersInstant3
                        {
                            public static GameObject GetGameObject() { return BoulderballSpawners.GetGameObject().transform.GetChild(3).gameObject; }
                        }
                    }

                    public class ParkToys
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(8).gameObject; }

                        public class BoulderBallHoop0
                        {
                            public static GameObject GetGameObject() { return ParkToys.GetGameObject().transform.GetChild(0).gameObject; }

                            public class HoopTrigger0
                            {
                                public static GameObject GetGameObject() { return BoulderBallHoop0.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class HoopTrigger1
                            {
                                public static GameObject GetGameObject() { return BoulderBallHoop0.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class MinigameVictor
                            {
                                public static GameObject GetGameObject() { return BoulderBallHoop0.GetGameObject().transform.GetChild(2).gameObject; }
                            }
                        }

                        public class BoulderBallHoop1
                        {
                            public static GameObject GetGameObject() { return ParkToys.GetGameObject().transform.GetChild(1).gameObject; }

                            public class HoopTrigger0
                            {
                                public static GameObject GetGameObject() { return BoulderBallHoop1.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class HoopTrigger1
                            {
                                public static GameObject GetGameObject() { return BoulderBallHoop1.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class MinigameVictor
                            {
                                public static GameObject GetGameObject() { return BoulderBallHoop1.GetGameObject().transform.GetChild(2).gameObject; }
                            }
                        }

                        public class Tetherball0
                        {
                            public static GameObject GetGameObject() { return ParkToys.GetGameObject().transform.GetChild(2).gameObject; }

                            public class Ball
                            {
                                public static GameObject GetGameObject() { return Tetherball0.GetGameObject().transform.GetChild(0).gameObject; }

                                public class Ball_
                                {
                                    public static GameObject GetGameObject() { return Ball.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class BallCage
                                {
                                    public static GameObject GetGameObject() { return Ball.GetGameObject().transform.GetChild(1).gameObject; }
                                }
                            }

                            public class InteractionSlider
                            {
                                public static GameObject GetGameObject() { return Tetherball0.GetGameObject().transform.GetChild(1).gameObject; }

                                public class OneTwoThree
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class InteractionSlider_
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class InteractionSliderCap
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class InteractionSliderSegment0
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class OnOff
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class StartPoint
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class EndPoint
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(6).gameObject; }
                                }

                                public class SliderHandle
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(7).gameObject; }

                                    public class HandleRotationParent
                                    {
                                        public static GameObject GetGameObject() { return SliderHandle.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class HandleParent
                                        {
                                            public static GameObject GetGameObject() { return HandleRotationParent.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class RightHandle
                                            {
                                                public static GameObject GetGameObject() { return HandleParent.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LeftHandle
                                            {
                                                public static GameObject GetGameObject() { return HandleParent.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }
                                    }
                                }

                                public class InteractionSliderSegment1
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(8).gameObject; }
                                }

                                public class InteractionSliderSegment2
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(9).gameObject; }
                                }
                            }

                            public class Line
                            {
                                public static GameObject GetGameObject() { return Tetherball0.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class LineRig
                            {
                                public static GameObject GetGameObject() { return Tetherball0.GetGameObject().transform.GetChild(3).gameObject; }

                                public class Bone000
                                {
                                    public static GameObject GetGameObject() { return LineRig.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Bone001
                                {
                                    public static GameObject GetGameObject() { return LineRig.GetGameObject().transform.GetChild(1).gameObject; }
                                }
                            }

                            public class Collision
                            {
                                public static GameObject GetGameObject() { return Tetherball0.GetGameObject().transform.GetChild(4).gameObject; }

                                public class WoodCollider0
                                {
                                    public static GameObject GetGameObject() { return Collision.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class WoodCollider1
                                {
                                    public static GameObject GetGameObject() { return Collision.GetGameObject().transform.GetChild(1).gameObject; }
                                }
                            }
                        }

                        public class Tetherball1
                        {
                            public static GameObject GetGameObject() { return ParkToys.GetGameObject().transform.GetChild(3).gameObject; }

                            public class Ball
                            {
                                public static GameObject GetGameObject() { return Tetherball1.GetGameObject().transform.GetChild(0).gameObject; }

                                public class Ball_
                                {
                                    public static GameObject GetGameObject() { return Ball.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class BallCage
                                {
                                    public static GameObject GetGameObject() { return Ball.GetGameObject().transform.GetChild(1).gameObject; }
                                }
                            }

                            public class InteractionSlider
                            {
                                public static GameObject GetGameObject() { return Tetherball1.GetGameObject().transform.GetChild(1).gameObject; }

                                public class OneTwoThree
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class InteractionSlider_
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class InteractionSliderCap
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class InteractionSliderSegment0
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class OnOff
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class StartPoint
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class EndPoint
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(6).gameObject; }
                                }

                                public class SliderHandle
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(7).gameObject; }

                                    public class HandleRotationParent
                                    {
                                        public static GameObject GetGameObject() { return SliderHandle.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class HandleParent
                                        {
                                            public static GameObject GetGameObject() { return HandleRotationParent.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class RightHandle
                                            {
                                                public static GameObject GetGameObject() { return HandleParent.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LeftHandle
                                            {
                                                public static GameObject GetGameObject() { return HandleParent.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }
                                    }
                                }

                                public class InteractionSliderSegment1
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(8).gameObject; }
                                }

                                public class InteractionSliderSegment2
                                {
                                    public static GameObject GetGameObject() { return InteractionSlider.GetGameObject().transform.GetChild(9).gameObject; }
                                }
                            }

                            public class Line
                            {
                                public static GameObject GetGameObject() { return Tetherball1.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class LineRig
                            {
                                public static GameObject GetGameObject() { return Tetherball1.GetGameObject().transform.GetChild(3).gameObject; }

                                public class Bone000
                                {
                                    public static GameObject GetGameObject() { return LineRig.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Bone001
                                {
                                    public static GameObject GetGameObject() { return LineRig.GetGameObject().transform.GetChild(1).gameObject; }
                                }
                            }

                            public class Collision
                            {
                                public static GameObject GetGameObject() { return Tetherball1.GetGameObject().transform.GetChild(4).gameObject; }

                                public class WoodCollider0
                                {
                                    public static GameObject GetGameObject() { return Collision.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class WoodCollider1
                                {
                                    public static GameObject GetGameObject() { return Collision.GetGameObject().transform.GetChild(1).gameObject; }
                                }
                            }
                        }

                        public class Bags
                        {
                            public static GameObject GetGameObject() { return ParkToys.GetGameObject().transform.GetChild(4).gameObject; }

                            public class Bag0
                            {
                                public static GameObject GetGameObject() { return Bags.GetGameObject().transform.GetChild(0).gameObject; }

                                public class Bag
                                {
                                    public static GameObject GetGameObject() { return Bag0.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Bone000
                                    {
                                        public static GameObject GetGameObject() { return Bag.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Bone001
                                    {
                                        public static GameObject GetGameObject() { return Bag.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Bone002
                                    {
                                        public static GameObject GetGameObject() { return Bag.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class Mesh
                                {
                                    public static GameObject GetGameObject() { return Bag0.GetGameObject().transform.GetChild(1).gameObject; }
                                }
                            }

                            public class Bag1
                            {
                                public static GameObject GetGameObject() { return Bags.GetGameObject().transform.GetChild(1).gameObject; }

                                public class Bag
                                {
                                    public static GameObject GetGameObject() { return Bag1.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Bone000
                                    {
                                        public static GameObject GetGameObject() { return Bag.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Bone001
                                    {
                                        public static GameObject GetGameObject() { return Bag.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Bone002
                                    {
                                        public static GameObject GetGameObject() { return Bag.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class Mesh
                                {
                                    public static GameObject GetGameObject() { return Bag1.GetGameObject().transform.GetChild(1).gameObject; }
                                }
                            }

                            public class Plank0
                            {
                                public static GameObject GetGameObject() { return Bags.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class Plank1
                            {
                                public static GameObject GetGameObject() { return Bags.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class Plank2
                            {
                                public static GameObject GetGameObject() { return Bags.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class Plank3
                            {
                                public static GameObject GetGameObject() { return Bags.GetGameObject().transform.GetChild(5).gameObject; }
                            }
                        }

                        public class MatchCounter
                        {
                            public static GameObject GetGameObject() { return ParkToys.GetGameObject().transform.GetChild(5).gameObject; }

                            public class Scoreboard
                            {
                                public static GameObject GetGameObject() { return MatchCounter.GetGameObject().transform.GetChild(0).gameObject; }

                                public class IndustrialSetGirdir1
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class IndustrialSetSkewedLSectionHalfLength1
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class IndustrialSetLSection1
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class CubeIndustrialSetStraightDubbleWith1
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class IndustrialSetGirdir2
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class IndustrialSetLSection2
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class IndustrialSetSkewedLSectionHalfLength2
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(6).gameObject; }
                                }

                                public class CubeIndustrialSetStraightDubbleWith2
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(7).gameObject; }
                                }

                                public class CubeIndustrialSetStraightDubbleWith3
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(8).gameObject; }
                                }

                                public class CubeIndustrialSetStraightDubbleWith4
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(9).gameObject; }
                                }

                                public class FirstPlayerNameBoard
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(10).gameObject; }
                                }

                                public class FirstPlayerNameUI
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(11).gameObject; }

                                    public class Text
                                    {
                                        public static GameObject GetGameObject() { return FirstPlayerNameUI.GetGameObject().transform.GetChild(0).gameObject; }
                                    }
                                }

                                public class SecondPlayerNameBoard
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(12).gameObject; }
                                }

                                public class SecondPlayerNameUI
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(13).gameObject; }

                                    public class Text
                                    {
                                        public static GameObject GetGameObject() { return SecondPlayerNameUI.GetGameObject().transform.GetChild(0).gameObject; }
                                    }
                                }

                                public class FirstPlayerScoreBoard
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(14).gameObject; }
                                }

                                public class FirstPlayerScoreUI
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(15).gameObject; }

                                    public class Text
                                    {
                                        public static GameObject GetGameObject() { return FirstPlayerScoreUI.GetGameObject().transform.GetChild(0).gameObject; }
                                    }
                                }

                                public class SecondPlayerScoreBoard
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(16).gameObject; }
                                }

                                public class SecondPlayerScoreUI
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(17).gameObject; }

                                    public class Text
                                    {
                                        public static GameObject GetGameObject() { return SecondPlayerScoreUI.GetGameObject().transform.GetChild(0).gameObject; }
                                    }
                                }

                                public class ResetButton
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(18).gameObject; }

                                    public class Button
                                    {
                                        public static GameObject GetGameObject() { return ResetButton.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class LeftHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RightHandle
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class Spring
                                        {
                                            public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                        }
                                    }
                                }

                                public class ResetScoreBoard
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(19).gameObject; }
                                }

                                public class RestScoreUI
                                {
                                    public static GameObject GetGameObject() { return Scoreboard.GetGameObject().transform.GetChild(20).gameObject; }

                                    public class Text
                                    {
                                        public static GameObject GetGameObject() { return RestScoreUI.GetGameObject().transform.GetChild(0).gameObject; }
                                    }
                                }
                            }
                        }
                    }

                    public class Interactables
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(9).gameObject; }

                        public class Bell
                        {
                            public static GameObject GetGameObject() { return Interactables.GetGameObject().transform.GetChild(0).gameObject; }

                            public class BellCollider
                            {
                                public static GameObject GetGameObject() { return Bell.GetGameObject().transform.GetChild(0).gameObject; }
                            }
                        }
                    }

                    public class HeinhouserProducts
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(10).gameObject; }

                        public class Telephone
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(0).gameObject; }

                            public class FriendScreen
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class Frame
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class SettingsScreen
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class PlayerFinderScreen
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class RecentScreen
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class NotificationScreen
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(5).gameObject; }
                            }

                            public class InteractionSlider1
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(6).gameObject; }
                            }

                            public class InteractionSlider0
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(7).gameObject; }
                            }

                            public class PhoneProp
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(8).gameObject; }
                            }

                            public class IDButton
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(9).gameObject; }
                            }

                            public class OfflineModeSlab
                            {
                                public static GameObject GetGameObject() { return Telephone.GetGameObject().transform.GetChild(10).gameObject; }
                            }
                        }

                        public class Parkboard
                        {
                            public static GameObject GetGameObject() { return HeinhouserProducts.GetGameObject().transform.GetChild(1).gameObject; }

                            public class Model
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class RotatingScreen
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class PrimaryDisplay
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class PlayerRelocationTrigger
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class OfflineModeSlab
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class MiniGameSharedObjects
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(5).gameObject; }
                            }

                            public class MinigameUniqueObjects
                            {
                                public static GameObject GetGameObject() { return Parkboard.GetGameObject().transform.GetChild(6).gameObject; }
                            }
                        }
                    }

                    public class ShiftstoneQuickswapper
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(11).gameObject; }

                        public class FloatingButton
                        {
                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(0).gameObject; }

                            public class Rock
                            {
                                public static GameObject GetGameObject() { return FloatingButton.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class Text
                            {
                                public static GameObject GetGameObject() { return FloatingButton.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class InteractionButtonToggleVariant
                            {
                                public static GameObject GetGameObject() { return FloatingButton.GetGameObject().transform.GetChild(2).gameObject; }

                                public class Button
                                {
                                    public static GameObject GetGameObject() { return InteractionButtonToggleVariant.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class LeftHandle
                                    {
                                        public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class RightHandle
                                    {
                                        public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Spring
                                    {
                                        public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }
                            }
                        }

                        public class LeftHandSlab
                        {
                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(1).gameObject; }

                            public class InfoFormCanvas
                            {
                                public static GameObject GetGameObject() { return LeftHandSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                public class Title
                                {
                                    public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Icon
                                {
                                    public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class Description
                                {
                                    public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class TitleUnderline
                                {
                                    public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class LeftHandText
                                {
                                    public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(4).gameObject; }
                                }
                            }
                        }

                        public class RightHandSlab
                        {
                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(2).gameObject; }

                            public class InfoFormCanvas
                            {
                                public static GameObject GetGameObject() { return RightHandSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                public class Title
                                {
                                    public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Icon
                                {
                                    public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class Description
                                {
                                    public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class TitleUnderline
                                {
                                    public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class LeftHandText
                                {
                                    public static GameObject GetGameObject() { return InfoFormCanvas.GetGameObject().transform.GetChild(4).gameObject; }
                                }
                            }
                        }

                        public class LeftHandToolTip
                        {
                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(3).gameObject; }

                            public class ToolTipCanvas
                            {
                                public static GameObject GetGameObject() { return LeftHandToolTip.GetGameObject().transform.GetChild(0).gameObject; }

                                public class ToolTipSpriteCanvas
                                {
                                    public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipTextBackground
                                    {
                                        public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class ToolTipArrowIconPositive
                                    {
                                        public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class ToolTipArrowIconNegative
                                    {
                                        public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class ToolTipTextCanvas
                                {
                                    public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class ToolTipTextComponent
                                    {
                                        public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                    }
                                }
                            }
                        }

                        public class RightHandToolTip
                        {
                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(4).gameObject; }

                            public class ToolTipCanvas
                            {
                                public static GameObject GetGameObject() { return RightHandToolTip.GetGameObject().transform.GetChild(0).gameObject; }

                                public class ToolTipSpriteCanvas
                                {
                                    public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class ToolTipTextBackground
                                    {
                                        public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class ToolTipArrowIconPositive
                                    {
                                        public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class ToolTipArrowIconNegative
                                    {
                                        public static GameObject GetGameObject() { return ToolTipSpriteCanvas.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class ToolTipTextCanvas
                                {
                                    public static GameObject GetGameObject() { return ToolTipCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class ToolTipTextComponent
                                    {
                                        public static GameObject GetGameObject() { return ToolTipTextCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                    }
                                }
                            }
                        }

                        public class ShiftstoneButtonVFX
                        {
                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(5).gameObject; }
                        }
                    }
                }

                public class FTraceLightMaps
                {
                    public static GameObject GetGameObject() { return allBaseParkGameObjects[1]; }
                }

                public class Scene
                {
                    public static GameObject GetGameObject() { return allBaseParkGameObjects[2]; }

                    public class Park
                    {
                        public static GameObject GetGameObject() { return Scene.GetGameObject().transform.GetChild(0).gameObject; }

                        private class CollisionGroup
                        {
                            public static GameObject GetGameObject() { return Park.GetGameObject().transform.GetChild(0).gameObject; }

                            public class CombatFloor
                            {
                                public static GameObject GetGameObject() { return CollisionGroup.GetGameObject().transform.GetChild(0).gameObject; }

                                public class Col_combatfloor0
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Col_combatfloor1
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class Col_combatfloor2
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class Col_combatfloor3
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class Col_combatfloor4
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class Col_combatfloor5
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class Col_combatfloor6
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(6).gameObject; }
                                }

                                public class Col_combatfloor7
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(7).gameObject; }
                                }

                                public class Col_combatfloor8
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(8).gameObject; }
                                }

                                public class Col_combatfloor9
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(9).gameObject; }
                                }

                                public class Col_combatfloor10
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(10).gameObject; }
                                }

                                public class Col_combatfloor11
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(11).gameObject; }
                                }

                                public class Col_combatfloor12
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(12).gameObject; }
                                }

                                public class Col_combatfloor13
                                {
                                    public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(13).gameObject; }
                                }
                            }

                            public class EnviromentalCollission
                            {
                                public static GameObject GetGameObject() { return CollisionGroup.GetGameObject().transform.GetChild(1).gameObject; }

                                public class Col_Environmental0
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Col_Environmental1
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class Col_Environmental2
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class Col_Environmental3
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class Col_Environmental4
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class Col_Environmental5
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class Col_Environmental6
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(6).gameObject; }
                                }

                                public class Col_Environmental6_001
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(7).gameObject; }
                                }

                                public class Col_Environmental6_002
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(8).gameObject; }
                                }

                                public class Col_Environmental7
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(9).gameObject; }
                                }

                                public class Col_Environmental8
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(10).gameObject; }
                                }

                                public class Col_Environmental9
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(11).gameObject; }
                                }

                                public class Col_Environmental10
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(12).gameObject; }
                                }

                                public class Col_Environmental12
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(13).gameObject; }
                                }

                                public class Col_Environmental13
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(14).gameObject; }
                                }

                                public class Col_Environmental14
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(15).gameObject; }
                                }

                                public class Col_Environmental15
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(16).gameObject; }
                                }

                                public class Col_Environmental16
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(17).gameObject; }
                                }

                                public class Col_Environmental17
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(18).gameObject; }
                                }

                                public class Col_Environmental18
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(19).gameObject; }
                                }

                                public class Col_Environmental19
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(20).gameObject; }
                                }

                                public class Col_Environmental20
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(21).gameObject; }
                                }

                                public class Col_Environmental21
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(22).gameObject; }
                                }

                                public class Col_Environmental22
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(23).gameObject; }
                                }

                                public class Col_Environmental23
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(24).gameObject; }
                                }

                                public class Col_Environmental24
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(25).gameObject; }
                                }

                                public class Col_Environmental25
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(26).gameObject; }
                                }

                                public class Col_Environmental26
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(27).gameObject; }
                                }

                                public class Col_gondola
                                {
                                    public static GameObject GetGameObject() { return EnviromentalCollission.GetGameObject().transform.GetChild(28).gameObject; }

                                    public class GondolaFloor
                                    {
                                        public static GameObject GetGameObject() { return Col_gondola.GetGameObject().transform.GetChild(0).gameObject; }
                                    }
                                }
                            }

                            public class Floor
                            {
                                public static GameObject GetGameObject() { return CollisionGroup.GetGameObject().transform.GetChild(2).gameObject; }

                                public class Col_floor0
                                {
                                    public static GameObject GetGameObject() { return Floor.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Col_floor1
                                {
                                    public static GameObject GetGameObject() { return Floor.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class Stair_collider_1
                                    {
                                        public static GameObject GetGameObject() { return Col_floor1.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Stair_collider_2
                                    {
                                        public static GameObject GetGameObject() { return Col_floor1.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Stair_collider_3
                                    {
                                        public static GameObject GetGameObject() { return Col_floor1.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class Stair_collider_4
                                    {
                                        public static GameObject GetGameObject() { return Col_floor1.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class Stair_collider_5
                                    {
                                        public static GameObject GetGameObject() { return Col_floor1.GetGameObject().transform.GetChild(4).gameObject; }
                                    }
                                }
                            }
                        }

                        public class MainStaticGroup
                        {
                            public static GameObject GetGameObject() { return Park.GetGameObject().transform.GetChild(1).gameObject; }

                            public class Arenas
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(0).gameObject; }

                                public class GymArena0
                                {
                                    public static GameObject GetGameObject() { return Arenas.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Arena
                                    {
                                        public static GameObject GetGameObject() { return GymArena0.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class ArenaMetalRim
                                    {
                                        public static GameObject GetGameObject() { return GymArena0.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class RingClamp
                                    {
                                        public static GameObject GetGameObject() { return GymArena0.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class Rocks
                                    {
                                        public static GameObject GetGameObject() { return GymArena0.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class GymArena1
                                {
                                    public static GameObject GetGameObject() { return Arenas.GetGameObject().transform.GetChild(1).gameObject; }

                                    public class Arena
                                    {
                                        public static GameObject GetGameObject() { return GymArena1.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Rocks
                                    {
                                        public static GameObject GetGameObject() { return GymArena1.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    private class RocksCollider
                                    {
                                        public static GameObject GetGameObject() { return GymArena1.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class Stairs
                                    {
                                        public static GameObject GetGameObject() { return GymArena1.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }

                                public class GymArena2
                                {
                                    public static GameObject GetGameObject() { return Arenas.GetGameObject().transform.GetChild(2).gameObject; }

                                    public class Arena
                                    {
                                        public static GameObject GetGameObject() { return GymArena2.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Rocks
                                    {
                                        public static GameObject GetGameObject() { return GymArena2.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Stairs
                                    {
                                        public static GameObject GetGameObject() { return GymArena2.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class GymArena3
                                {
                                    public static GameObject GetGameObject() { return Arenas.GetGameObject().transform.GetChild(3).gameObject; }

                                    public class Arena
                                    {
                                        public static GameObject GetGameObject() { return GymArena3.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Rocks
                                    {
                                        public static GameObject GetGameObject() { return GymArena3.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Stairs
                                    {
                                        public static GameObject GetGameObject() { return GymArena3.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class GymArena4
                                {
                                    public static GameObject GetGameObject() { return Arenas.GetGameObject().transform.GetChild(4).gameObject; }

                                    public class Arena
                                    {
                                        public static GameObject GetGameObject() { return GymArena4.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Rocks
                                    {
                                        public static GameObject GetGameObject() { return GymArena4.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Stairs
                                    {
                                        public static GameObject GetGameObject() { return GymArena4.GetGameObject().transform.GetChild(2).gameObject; }
                                    }
                                }

                                public class GymArena5
                                {
                                    public static GameObject GetGameObject() { return Arenas.GetGameObject().transform.GetChild(5).gameObject; }

                                    public class Arena
                                    {
                                        public static GameObject GetGameObject() { return GymArena5.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class ArenaMetalRim
                                    {
                                        public static GameObject GetGameObject() { return GymArena5.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Rocks
                                    {
                                        public static GameObject GetGameObject() { return GymArena5.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class Stairs
                                    {
                                        public static GameObject GetGameObject() { return GymArena5.GetGameObject().transform.GetChild(3).gameObject; }
                                    }
                                }
                            }

                            public class HubFloor
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(1).gameObject; }

                                public class HubFloorHIghRes
                                {
                                    public static GameObject GetGameObject() { return HubFloor.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class Leaves
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(2).gameObject; }

                                public class LeavesSpherePark000
                                {
                                    public static GameObject GetGameObject() { return Leaves.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class LeavesSpherePark001
                                {
                                    public static GameObject GetGameObject() { return Leaves.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class LeavesSpherePark002
                                {
                                    public static GameObject GetGameObject() { return Leaves.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class LeavesSpherePark003
                                {
                                    public static GameObject GetGameObject() { return Leaves.GetGameObject().transform.GetChild(3).gameObject; }
                                }
                            }

                            public class Root
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(3).gameObject; }

                                public class RootBaseMesh005_2
                                {
                                    public static GameObject GetGameObject() { return Root.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class RootBaseMesh006_3
                                {
                                    public static GameObject GetGameObject() { return Root.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class RootBaseMesh008
                                {
                                    public static GameObject GetGameObject() { return Root.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class RootBaseMesh009
                                {
                                    public static GameObject GetGameObject() { return Root.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class RootBaseMesh016_3
                                {
                                    public static GameObject GetGameObject() { return Root.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class RootBaseMesh021
                                {
                                    public static GameObject GetGameObject() { return Root.GetGameObject().transform.GetChild(5).gameObject; }
                                }
                            }

                            public class Wood
                            {
                                public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(4).gameObject; }

                                public class Plank000
                                {
                                    public static GameObject GetGameObject() { return Wood.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class Plank001
                                {
                                    public static GameObject GetGameObject() { return Wood.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class Plank002
                                {
                                    public static GameObject GetGameObject() { return Wood.GetGameObject().transform.GetChild(2).gameObject; }
                                }
                            }
                        }

                        public class SubStaticGroup
                        {
                            public static GameObject GetGameObject() { return Park.GetGameObject().transform.GetChild(2).gameObject; }

                            public class Pedistals
                            {
                                public static GameObject GetGameObject() { return SubStaticGroup.GetGameObject().transform.GetChild(0).gameObject; }

                                public class LargePedistal2
                                {
                                    public static GameObject GetGameObject() { return Pedistals.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class LargePedistal10
                                {
                                    public static GameObject GetGameObject() { return Pedistals.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class LargePedistal11
                                {
                                    public static GameObject GetGameObject() { return Pedistals.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class LargePedistal14
                                {
                                    public static GameObject GetGameObject() { return Pedistals.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class LargePedistal14_002
                                {
                                    public static GameObject GetGameObject() { return Pedistals.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class LargePedistal5
                                {
                                    public static GameObject GetGameObject() { return Pedistals.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class LargePedistal9
                                {
                                    public static GameObject GetGameObject() { return Pedistals.GetGameObject().transform.GetChild(6).gameObject; }
                                }

                                public class LargePedistal12
                                {
                                    public static GameObject GetGameObject() { return Pedistals.GetGameObject().transform.GetChild(7).gameObject; }
                                }
                            }

                            public class Rocks
                            {
                                public static GameObject GetGameObject() { return SubStaticGroup.GetGameObject().transform.GetChild(1).gameObject; }

                                public class SetA
                                {
                                    public static GameObject GetGameObject() { return Rocks.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class SetB
                                {
                                    public static GameObject GetGameObject() { return Rocks.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class SetC
                                {
                                    public static GameObject GetGameObject() { return Rocks.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class SetD
                                {
                                    public static GameObject GetGameObject() { return Rocks.GetGameObject().transform.GetChild(3).gameObject; }
                                }

                                public class SetE
                                {
                                    public static GameObject GetGameObject() { return Rocks.GetGameObject().transform.GetChild(4).gameObject; }
                                }

                                public class SetF
                                {
                                    public static GameObject GetGameObject() { return Rocks.GetGameObject().transform.GetChild(5).gameObject; }
                                }

                                public class SetG
                                {
                                    public static GameObject GetGameObject() { return Rocks.GetGameObject().transform.GetChild(6).gameObject; }
                                }
                            }

                            public class Slide
                            {
                                public static GameObject GetGameObject() { return SubStaticGroup.GetGameObject().transform.GetChild(2).gameObject; }

                                public class Slide_
                                {
                                    public static GameObject GetGameObject() { return Slide.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class Station
                            {
                                public static GameObject GetGameObject() { return SubStaticGroup.GetGameObject().transform.GetChild(3).gameObject; }

                                public class Gondola
                                {
                                    public static GameObject GetGameObject() { return Station.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class Bell0
                                    {
                                        public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(0).gameObject; }
                                    }

                                    public class Bell1
                                    {
                                        public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(1).gameObject; }
                                    }

                                    public class Cabin
                                    {
                                        public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class Cylinder
                                    {
                                        public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(3).gameObject; }
                                    }

                                    public class Step
                                    {
                                        public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class Cube
                                        {
                                            public static GameObject GetGameObject() { return Step.GetGameObject().transform.GetChild(0).gameObject; }
                                        }
                                    }

                                    public class WalkableFloor
                                    {
                                        public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(5).gameObject; }
                                    }

                                    public class Wheel
                                    {
                                        public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(6).gameObject; }
                                    }

                                    private class PlayerNoEntryColliders
                                    {
                                        public static GameObject GetGameObject() { return Gondola.GetGameObject().transform.GetChild(7).gameObject; }

                                        public class Collider0
                                        {
                                            public static GameObject GetGameObject() { return PlayerNoEntryColliders.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Collider1
                                        {
                                            public static GameObject GetGameObject() { return PlayerNoEntryColliders.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }
                                }

                                public class Platform
                                {
                                    public static GameObject GetGameObject() { return Station.GetGameObject().transform.GetChild(1).gameObject; }

                                    private class NoStructuresZone
                                    {
                                        public static GameObject GetGameObject() { return Platform.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class KillStructureZone0
                                        {
                                            public static GameObject GetGameObject() { return NoStructuresZone.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class KillStructureZone1
                                        {
                                            public static GameObject GetGameObject() { return NoStructuresZone.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class Platform_
                                    {
                                        public static GameObject GetGameObject() { return Platform.GetGameObject().transform.GetChild(1).gameObject; }
                                    }
                                }
                            }
                        }
                    }

                    public class Mountains
                    {
                        public static GameObject GetGameObject() { return Scene.GetGameObject().transform.GetChild(1).gameObject; }

                        public class Mountain0
                        {
                            public static GameObject GetGameObject() { return Mountains.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class Mountain1
                        {
                            public static GameObject GetGameObject() { return Mountains.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class Mountain3
                        {
                            public static GameObject GetGameObject() { return Mountains.GetGameObject().transform.GetChild(2).gameObject; }
                        }

                        public class Mountain5
                        {
                            public static GameObject GetGameObject() { return Mountains.GetGameObject().transform.GetChild(3).gameObject; }
                        }

                        public class Mountain6
                        {
                            public static GameObject GetGameObject() { return Mountains.GetGameObject().transform.GetChild(4).gameObject; }
                        }

                        public class Mountain7
                        {
                            public static GameObject GetGameObject() { return Mountains.GetGameObject().transform.GetChild(5).gameObject; }
                        }

                        public class Mountain8
                        {
                            public static GameObject GetGameObject() { return Mountains.GetGameObject().transform.GetChild(6).gameObject; }
                        }

                        public class Mountain9
                        {
                            public static GameObject GetGameObject() { return Mountains.GetGameObject().transform.GetChild(7).gameObject; }
                        }

                        public class MountainSegments
                        {
                            public static GameObject GetGameObject() { return Mountains.GetGameObject().transform.GetChild(8).gameObject; }

                            public class Segment1
                            {
                                public static GameObject GetGameObject() { return MountainSegments.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class Segment2
                            {
                                public static GameObject GetGameObject() { return MountainSegments.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class Segment3
                            {
                                public static GameObject GetGameObject() { return MountainSegments.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class Segment4
                            {
                                public static GameObject GetGameObject() { return MountainSegments.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class Segment5
                            {
                                public static GameObject GetGameObject() { return MountainSegments.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class Segment6
                            {
                                public static GameObject GetGameObject() { return MountainSegments.GetGameObject().transform.GetChild(5).gameObject; }
                            }
                        }

                        public class Mountain2
                        {
                            public static GameObject GetGameObject() { return Mountains.GetGameObject().transform.GetChild(9).gameObject; }
                        }

                        public class Plane
                        {
                            public static GameObject GetGameObject() { return Mountains.GetGameObject().transform.GetChild(10).gameObject; }
                        }
                    }
                }

                public class ParkBoardP1ScoreVFX
                {
                    public static GameObject GetGameObject() { return allBaseParkGameObjects[3]; }
                }

                public class LightingAndEffects
                {
                    public static GameObject GetGameObject() { return allBaseParkGameObjects[4]; }

                    public class Probes
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(0).gameObject; }

                        public class MegaProbe0
                        {
                            public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class MegaProbe1
                        {
                            public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(2).gameObject; }
                        }

                        public class MegaProbe2
                        {
                            public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(3).gameObject; }
                        }

                        public class MegaProbe3
                        {
                            public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(4).gameObject; }
                        }

                        public class MegaProbe4
                        {
                            public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(5).gameObject; }
                        }

                        public class MegaProbe5
                        {
                            public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(6).gameObject; }
                        }

                        public class MegaProbe6
                        {
                            public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(7).gameObject; }
                        }

                        public class LightProbes
                        {
                            public static GameObject GetGameObject() { return Probes.GetGameObject().transform.GetChild(0).gameObject; }
                        }
                    }

                    public class DirectionalLight
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(1).gameObject; }
                    }

                    public class Skylight
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(2).gameObject; }
                    }

                    public class VisualEffects
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(3).gameObject; }

                        public class WindVelocitySource
                        {
                            public static GameObject GetGameObject() { return VisualEffects.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class WindStreakVFX
                        {
                            public static GameObject GetGameObject() { return VisualEffects.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class FallingLeafVFXs
                        {
                            public static GameObject GetGameObject() { return VisualEffects.GetGameObject().transform.GetChild(2).gameObject; }

                            public class FallingLeaves0
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class FallingLeaves1
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class FallingLeaves2
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class FallingLeaves3
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class FallingLeaves4
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class FallingLeaves5
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(5).gameObject; }
                            }

                            public class FallingLeaves6
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(6).gameObject; }
                            }

                            public class FallingLeaves7
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(7).gameObject; }
                            }

                            public class FallingLeaves8
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(8).gameObject; }
                            }

                            public class FallingLeaves9
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(9).gameObject; }
                            }

                            public class FallingLeaves10
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(10).gameObject; }
                            }

                            public class FallingLeaves11
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(11).gameObject; }
                            }

                            public class FallingLeaves12
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(12).gameObject; }
                            }
                        }
                    }

                    public class DustColorOverride
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(4).gameObject; }
                    }
                }

                public class VoiceLogger
                {
                    public static GameObject GetGameObject() { return allBaseParkGameObjects[5]; }
                }
            }

            public class Map0
            {
                public static System.Collections.Generic.List<GameObject> GetBaseDDOLGameObjects() { return allBaseMap0GameObjects; }

                public class SceneProcessor
                {
                    public static GameObject GetGameObject() { return allBaseMap0GameObjects[0]; }
                }

                public class Logic
                {
                    public static GameObject GetGameObject() { return allBaseMap0GameObjects[1]; }

                    public class SpawnPointHandler
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(0).gameObject; }
                    }

                    public class Pedestals
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(1).gameObject; }

                        public class MatchPedestalP2
                        {
                            public static GameObject GetGameObject() { return Pedestals.GetGameObject().transform.GetChild(0).gameObject; }

                            public class MoveLimiter
                            {
                                public static GameObject GetGameObject() { return MatchPedestalP2.GetGameObject().transform.GetChild(0).gameObject; }

                                public class PlayerGrounderIKCollision
                                {
                                    public static GameObject GetGameObject() { return MoveLimiter.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class VFX
                            {
                                public static GameObject GetGameObject() { return MatchPedestalP2.GetGameObject().transform.GetChild(1).gameObject; }

                                public class DustPedestalLift
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class DustPedestalRelocate
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class DustPedestalSink
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class DustPedestalSpawn
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(3).gameObject; }
                                }
                            }
                        }

                        public class MatchPedestalP1
                        {
                            public static GameObject GetGameObject() { return Pedestals.GetGameObject().transform.GetChild(1).gameObject; }

                            public class MoveLimiter
                            {
                                public static GameObject GetGameObject() { return MatchPedestalP1.GetGameObject().transform.GetChild(0).gameObject; }

                                public class PlayerGrounderIKCollision
                                {
                                    public static GameObject GetGameObject() { return MoveLimiter.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class VFX
                            {
                                public static GameObject GetGameObject() { return MatchPedestalP1.GetGameObject().transform.GetChild(1).gameObject; }

                                public class DustPedestalLift
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class DustPedestalRelocate
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class DustPedestalSink
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class DustPedestalSpawn
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(3).gameObject; }
                                }
                            }
                        }
                    }

                    public class SpawnTransformSlabOne
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(2).gameObject; }
                    }

                    public class SpawnTransformSlabTwo
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(3).gameObject; }
                    }

                    public class MatchHandler
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(4).gameObject; }
                    }

                    public class Ambience
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(5).gameObject; }
                    }

                    public class ArenaLocation
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(6).gameObject; }

                        public class PlayerOneLocation
                        {
                            public static GameObject GetGameObject() { return ArenaLocation.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class PlayerTwoLocation
                        {
                            public static GameObject GetGameObject() { return ArenaLocation.GetGameObject().transform.GetChild(1).gameObject; }
                        }
                    }

                    public class MatchSlabOne
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(7).gameObject; }

                        public class SlabNotification
                        {
                            public static GameObject GetGameObject() { return MatchSlabOne.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class SlabAutoState
                        {
                            public static GameObject GetGameObject() { return MatchSlabOne.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class MatchSlab
                        {
                            public static GameObject GetGameObject() { return MatchSlabOne.GetGameObject().transform.GetChild(2).gameObject; }

                            public class SlabBuddyMatchVariant
                            {
                                public static GameObject GetGameObject() { return MatchSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                public class MatchForm
                                {
                                    public static GameObject GetGameObject() { return SlabBuddyMatchVariant.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class GraphicsSlab
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return GraphicsSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class FloarRock0
                                            {
                                                public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class MeshGraphicsSlab
                                            {
                                                public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class FloatRock1
                                            {
                                                public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(2).gameObject; }
                                            }
                                        }
                                    }

                                    public class MatchFormCanvas
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class MatchText
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RankNames
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class CurrRank
                                            {
                                                public static GameObject GetGameObject() { return RankNames.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToRank
                                            {
                                                public static GameObject GetGameObject() { return RankNames.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class RankUpText
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(2).gameObject; }

                                            public class TextRock0
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class TextRock1
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class TextRock2
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(2).gameObject; }
                                            }

                                            public class TextRock3
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(3).gameObject; }
                                            }

                                            public class TextRock4
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(4).gameObject; }
                                            }

                                            public class TextRock5
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(5).gameObject; }
                                            }

                                            public class TextRock6
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(6).gameObject; }
                                            }

                                            public class TextRock7
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(7).gameObject; }
                                            }

                                            public class TextRock8
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(8).gameObject; }
                                            }

                                            public class TextRock9
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(9).gameObject; }
                                            }

                                            public class TextRock10
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(10).gameObject; }
                                            }

                                            public class TextRock11
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class TextRock12
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(12).gameObject; }
                                            }

                                            public class TextRock13
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(13).gameObject; }
                                            }

                                            public class TextRock14
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(14).gameObject; }
                                            }

                                            public class TextRock15
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(15).gameObject; }
                                            }

                                            public class TextRock16
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(16).gameObject; }
                                            }

                                            public class TextRock17
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(17).gameObject; }
                                            }

                                            public class TextRock18
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(18).gameObject; }
                                            }

                                            public class TextRock19
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(19).gameObject; }
                                            }

                                            public class TextRock20
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(20).gameObject; }
                                            }

                                            public class TextRock21
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(21).gameObject; }
                                            }
                                        }

                                        public class RankUpBar
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(3).gameObject; }

                                            public class XPGainVFX
                                            {
                                                public static GameObject GetGameObject() { return RankUpBar.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LevelUpVFX
                                            {
                                                public static GameObject GetGameObject() { return RankUpBar.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class Bar
                                            {
                                                public static GameObject GetGameObject() { return RankUpBar.GetGameObject().transform.GetChild(2).gameObject; }
                                            }
                                        }
                                    }

                                    private class DisposableCollider
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class RePlayButton
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(3).gameObject; }

                                        public class SlabRockFloat
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class InteractionButton
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Button
                                            {
                                                public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class LeftHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                                }

                                                public class RightHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                                }

                                                public class Spring
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                                }
                                            }
                                        }

                                        public class RePlayText
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                        public class InteractionLightNetworked
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(3).gameObject; }

                                            public class Light
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightNetworked.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LightPlug
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightNetworked.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class InteractionLightLocal
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(4).gameObject; }

                                            public class Light
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightLocal.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LightPlug
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightLocal.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }
                                    }

                                    public class ReQueueButton
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class SlabRockFloat
                                        {
                                            public static GameObject GetGameObject() { return ReQueueButton.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class InteractionButton
                                        {
                                            public static GameObject GetGameObject() { return ReQueueButton.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Button
                                            {
                                                public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class LeftHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                                }

                                                public class RightHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                                }

                                                public class Spring
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                                }
                                            }
                                        }

                                        public class ReQueueText
                                        {
                                            public static GameObject GetGameObject() { return ReQueueButton.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                    }

                                    public class ExitMatchButton
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(5).gameObject; }

                                        public class SlabRockFloat
                                        {
                                            public static GameObject GetGameObject() { return ExitMatchButton.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class InteractionButton
                                        {
                                            public static GameObject GetGameObject() { return ExitMatchButton.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Button
                                            {
                                                public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class LeftHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                                }

                                                public class RightHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                                }

                                                public class Spring
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                                }
                                            }
                                        }

                                        public class RePlayText
                                        {
                                            public static GameObject GetGameObject() { return ExitMatchButton.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                    }

                                    public class ShiftstoneQuickswapper
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(6).gameObject; }

                                        public class FloatingButton
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class LeftHandSlab
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class RightHandSlab
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                        public class LeftHandTooltip
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(3).gameObject; }
                                        }

                                        public class RIghtHandTooltip
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(4).gameObject; }
                                        }

                                        public class ShiftstoneButtonVFX
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(5).gameObject; }
                                        }
                                    }

                                    public class Wallet
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(7).gameObject; }

                                        public class CoinText
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Wallet_
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class Cheque
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(8).gameObject; }

                                        public class CoinText
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Check
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class CoinVFX
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(9).gameObject; }
                                    }

                                    public class WalletWooshVFX
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(10).gameObject; }
                                    }
                                }
                            }
                        }

                        public class ProgressTracker
                        {
                            public static GameObject GetGameObject() { return MatchSlabOne.GetGameObject().transform.GetChild(3).gameObject;}
                        }
                    }

                    public class MatchSlabTwo
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(8).gameObject; }

                        public class SlabNotification
                        {
                            public static GameObject GetGameObject() { return MatchSlabTwo.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class SlabAutoState
                        {
                            public static GameObject GetGameObject() { return MatchSlabTwo.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class MatchSlab
                        {
                            public static GameObject GetGameObject() { return MatchSlabTwo.GetGameObject().transform.GetChild(2).gameObject; }

                            public class SlabBuddyMatchVariant
                            {
                                public static GameObject GetGameObject() { return MatchSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                public class MatchForm
                                {
                                    public static GameObject GetGameObject() { return SlabBuddyMatchVariant.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class GraphicsSlab
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return GraphicsSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class FloarRock0
                                            {
                                                public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class MeshGraphicsSlab
                                            {
                                                public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class FloatRock1
                                            {
                                                public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(2).gameObject; }
                                            }
                                        }
                                    }

                                    public class MatchFormCanvas
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class MatchText
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RankNames
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class CurrRank
                                            {
                                                public static GameObject GetGameObject() { return RankNames.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToRank
                                            {
                                                public static GameObject GetGameObject() { return RankNames.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class RankUpText
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(2).gameObject; }

                                            public class TextRock0
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class TextRock1
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class TextRock2
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(2).gameObject; }
                                            }

                                            public class TextRock3
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(3).gameObject; }
                                            }

                                            public class TextRock4
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(4).gameObject; }
                                            }

                                            public class TextRock5
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(5).gameObject; }
                                            }

                                            public class TextRock6
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(6).gameObject; }
                                            }

                                            public class TextRock7
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(7).gameObject; }
                                            }

                                            public class TextRock8
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(8).gameObject; }
                                            }

                                            public class TextRock9
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(9).gameObject; }
                                            }

                                            public class TextRock10
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(10).gameObject; }
                                            }

                                            public class TextRock11
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class TextRock12
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(12).gameObject; }
                                            }

                                            public class TextRock13
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(13).gameObject; }
                                            }

                                            public class TextRock14
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(14).gameObject; }
                                            }

                                            public class TextRock15
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(15).gameObject; }
                                            }

                                            public class TextRock16
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(16).gameObject; }
                                            }

                                            public class TextRock17
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(17).gameObject; }
                                            }

                                            public class TextRock18
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(18).gameObject; }
                                            }

                                            public class TextRock19
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(19).gameObject; }
                                            }

                                            public class TextRock20
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(20).gameObject; }
                                            }

                                            public class TextRock21
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(21).gameObject; }
                                            }
                                        }

                                        public class RankUpBar
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(3).gameObject; }

                                            public class XPGainVFX
                                            {
                                                public static GameObject GetGameObject() { return RankUpBar.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LevelUpVFX
                                            {
                                                public static GameObject GetGameObject() { return RankUpBar.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class Bar
                                            {
                                                public static GameObject GetGameObject() { return RankUpBar.GetGameObject().transform.GetChild(2).gameObject; }
                                            }
                                        }
                                    }

                                    private class DisposableCollider
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class RePlayButton
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(3).gameObject; }

                                        public class SlabRockFloat
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class InteractionButton
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Button
                                            {
                                                public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class LeftHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                                }

                                                public class RightHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                                }

                                                public class Spring
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                                }
                                            }
                                        }

                                        public class RePlayText
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                        public class InteractionLightNetworked
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(3).gameObject; }

                                            public class Light
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightNetworked.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LightPlug
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightNetworked.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class InteractionLightLocal
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(4).gameObject; }

                                            public class Light
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightLocal.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LightPlug
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightLocal.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }
                                    }

                                    public class ReQueueButton
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class SlabRockFloat
                                        {
                                            public static GameObject GetGameObject() { return ReQueueButton.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class InteractionButton
                                        {
                                            public static GameObject GetGameObject() { return ReQueueButton.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Button
                                            {
                                                public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class LeftHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                                }

                                                public class RightHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                                }

                                                public class Spring
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                                }
                                            }
                                        }

                                        public class ReQueueText
                                        {
                                            public static GameObject GetGameObject() { return ReQueueButton.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                    }

                                    public class ExitMatchButton
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(5).gameObject; }

                                        public class SlabRockFloat
                                        {
                                            public static GameObject GetGameObject() { return ExitMatchButton.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class InteractionButton
                                        {
                                            public static GameObject GetGameObject() { return ExitMatchButton.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Button
                                            {
                                                public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class LeftHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                                }

                                                public class RightHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                                }

                                                public class Spring
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                                }
                                            }
                                        }

                                        public class RePlayText
                                        {
                                            public static GameObject GetGameObject() { return ExitMatchButton.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                    }

                                    public class ShiftstoneQuickswapper
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(6).gameObject; }

                                        public class FloatingButton
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class LeftHandSlab
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class RightHandSlab
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                        public class LeftHandTooltip
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(3).gameObject; }
                                        }

                                        public class RIghtHandTooltip
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(4).gameObject; }
                                        }

                                        public class ShiftstoneButtonVFX
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(5).gameObject; }
                                        }
                                    }

                                    public class Wallet
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(7).gameObject; }

                                        public class CoinText
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Wallet_
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class Cheque
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(8).gameObject; }

                                        public class CoinText
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Check
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class CoinVFX
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(9).gameObject; }
                                    }

                                    public class WalletWooshVFX
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(10).gameObject; }
                                    }
                                }
                            }
                        }

                        public class ProgressTracker
                        {
                            public static GameObject GetGameObject() { return MatchSlabTwo.GetGameObject().transform.GetChild(3).gameObject; }
                        }
                    }

                    public class SceneBoundaryPlayers
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(9).gameObject; }
                    }

                    public class SceneBoundaryStructures
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(10).gameObject; }
                    }

                    public class Analytics
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(11).gameObject; }
                    }

                    public class ClearNotificationSlabs
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(12).gameObject; }
                    }

                    public class CombatMusic
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(13).gameObject; }
                    }
                }

                public class FTraceLightMaps
                {
                    public static GameObject GetGameObject() { return allBaseMap0GameObjects[2]; }
                }

                public class LightingAndEffects
                {
                    public static GameObject GetGameObject() { return allBaseMap0GameObjects[3]; }

                    public class DirectionalLight
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(0).gameObject; }
                    }

                    public class Skylight
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(1).gameObject; }
                    }

                    public class LightProbeGroup
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(2).gameObject; }
                    }

                    public class ReflectionProbe
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(3).gameObject; }
                    }

                    public class VisualEffects
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(4).gameObject; }

                        public class WindVelocitySource
                        {
                            public static GameObject GetGameObject() { return VisualEffects.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class WindStreakVFX
                        {
                            public static GameObject GetGameObject() { return VisualEffects.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class FallingLeafVFXs
                        {
                            public static GameObject GetGameObject() { return VisualEffects.GetGameObject().transform.GetChild(2).gameObject; }

                            public class FallingLeaves0
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class FallingLeaves1
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class FallingLeaves2
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class FallingLeaves3
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class FallingLeaves4
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class FallingLeaves5
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(5).gameObject; }
                            }
                        }
                    }

                    public class DustColorOverride
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(5).gameObject; }
                    }
                }

                public class Map0Production
                {
                    public static GameObject GetGameObject() { return allBaseMap0GameObjects[4]; }

                    private class CollisionGroup
                    {
                        public static GameObject GetGameObject() { return Map0Production.GetGameObject().transform.GetChild(0).gameObject; }

                        public class CombatFloor
                        {
                            public static GameObject GetGameObject() { return CollisionGroup.GetGameObject().transform.GetChild(0).gameObject; }

                            public class Col_combat0
                            {
                                public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(0).gameObject; }
                            }
                        }

                        public class Environment
                        {
                            public static GameObject GetGameObject() { return CollisionGroup.GetGameObject().transform.GetChild(1).gameObject; }

                            public class Col_Environment0
                            {
                                public static GameObject GetGameObject() { return Environment.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class Col_Environment1
                            {
                                public static GameObject GetGameObject() { return Environment.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class Col_Environment2
                            {
                                public static GameObject GetGameObject() { return Environment.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class Col_Environment4
                            {
                                public static GameObject GetGameObject() { return Environment.GetGameObject().transform.GetChild(3).gameObject; }
                            }
                        }

                        public class Floor
                        {
                            public static GameObject GetGameObject() { return CollisionGroup.GetGameObject().transform.GetChild(2).gameObject; }

                            public class Col_floor0
                            {
                                public static GameObject GetGameObject() { return Floor.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class Col_floor1
                            {
                                public static GameObject GetGameObject() { return Floor.GetGameObject().transform.GetChild(1).gameObject; }
                            }
                        }
                    }

                    public class MainStaticGroup
                    {
                        public static GameObject GetGameObject() { return Map0Production.GetGameObject().transform.GetChild(1).gameObject; }

                        public class BackgroundPlane
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class BackgroundRocks
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class CombatFloor
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(2).gameObject; }
                        }

                        public class Gutter
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(3).gameObject; }
                        }

                        public class Leaves
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(4).gameObject; }
                        }

                        public class RingBoarder
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(5).gameObject; }
                        }

                        public class Root
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(6).gameObject; }
                        }
                    }

                    public class Mountain
                    {
                        public static GameObject GetGameObject() { return Map0Production.GetGameObject().transform.GetChild(2).gameObject; }
                    }
                }

                public class VoiceLogger
                {
                    public static GameObject GetGameObject() { return allBaseMap0GameObjects[5]; }
                }
            }

            public class Map1
            {
                public static System.Collections.Generic.List<GameObject> GetBaseDDOLGameObjects() { return allBaseMap1GameObjects; }

                public class FTraceLightMaps
                {
                    public static GameObject GetGameObject() { return allBaseMap1GameObjects[0]; }
                }

                public class LightingAndEffects
                {
                    public static GameObject GetGameObject() { return allBaseMap1GameObjects[1]; }

                    public class DirectionalLight
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(0).gameObject; }
                    }

                    public class Skylight
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(1).gameObject; }
                    }

                    public class LightProbeGroup
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(2).gameObject; }
                    }

                    public class ReflectionProbe
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(3).gameObject; }
                    }

                    public class VisualEffects
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(4).gameObject; }

                        public class WindVelocitySource
                        {
                            public static GameObject GetGameObject() { return VisualEffects.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class WindStreakVFX
                        {
                            public static GameObject GetGameObject() { return VisualEffects.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class FallingLeafVFXs
                        {
                            public static GameObject GetGameObject() { return VisualEffects.GetGameObject().transform.GetChild(2).gameObject; }

                            public class FallingLeaves0
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(0).gameObject; }
                            }

                            public class FallingLeaves1
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(1).gameObject; }
                            }

                            public class FallingLeaves2
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(2).gameObject; }
                            }

                            public class FallingLeaves3
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(3).gameObject; }
                            }

                            public class FallingLeaves4
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(4).gameObject; }
                            }

                            public class FallingLeaves5
                            {
                                public static GameObject GetGameObject() { return FallingLeafVFXs.GetGameObject().transform.GetChild(5).gameObject; }
                            }
                        }
                    }

                    public class DustColorOverride
                    {
                        public static GameObject GetGameObject() { return LightingAndEffects.GetGameObject().transform.GetChild(5).gameObject; }
                    }
                }

                public class Logic
                {
                    public static GameObject GetGameObject() { return allBaseMap1GameObjects[2]; }

                    public class SpawnPointHandler
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(0).gameObject; }
                    }

                    public class Pedestals
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(1).gameObject; }

                        public class MatchPedestalP2
                        {
                            public static GameObject GetGameObject() { return Pedestals.GetGameObject().transform.GetChild(0).gameObject; }

                            public class MoveLimiter
                            {
                                public static GameObject GetGameObject() { return MatchPedestalP2.GetGameObject().transform.GetChild(0).gameObject; }

                                public class PlayerGrounderIKCollision
                                {
                                    public static GameObject GetGameObject() { return MoveLimiter.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class VFX
                            {
                                public static GameObject GetGameObject() { return MatchPedestalP2.GetGameObject().transform.GetChild(1).gameObject; }

                                public class DustPedestalLift
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class DustPedestalRelocate
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class DustPedestalSink
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class DustPedestalSpawn
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(3).gameObject; }
                                }
                            }
                        }

                        public class MatchPedestalP1
                        {
                            public static GameObject GetGameObject() { return Pedestals.GetGameObject().transform.GetChild(1).gameObject; }

                            public class MoveLimiter
                            {
                                public static GameObject GetGameObject() { return MatchPedestalP1.GetGameObject().transform.GetChild(0).gameObject; }

                                public class PlayerGrounderIKCollision
                                {
                                    public static GameObject GetGameObject() { return MoveLimiter.GetGameObject().transform.GetChild(0).gameObject; }
                                }
                            }

                            public class VFX
                            {
                                public static GameObject GetGameObject() { return MatchPedestalP1.GetGameObject().transform.GetChild(1).gameObject; }

                                public class DustPedestalLift
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(0).gameObject; }
                                }

                                public class DustPedestalRelocate
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(1).gameObject; }
                                }

                                public class DustPedestalSink
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(2).gameObject; }
                                }

                                public class DustPedestalSpawn
                                {
                                    public static GameObject GetGameObject() { return VFX.GetGameObject().transform.GetChild(3).gameObject; }
                                }
                            }
                        }
                    }

                    public class SpawnTransformSlabOne
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(2).gameObject; }
                    }

                    public class SpawnTransformSlabTwo
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(3).gameObject; }
                    }

                    public class MatchHandler
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(4).gameObject; }
                    }

                    public class Ambience
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(5).gameObject; }
                    }

                    public class ArenaLocation
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(6).gameObject; }

                        public class PlayerOneLocation
                        {
                            public static GameObject GetGameObject() { return ArenaLocation.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class PlayerTwoLocation
                        {
                            public static GameObject GetGameObject() { return ArenaLocation.GetGameObject().transform.GetChild(1).gameObject; }
                        }
                    }

                    public class MatchSlabOne
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(7).gameObject; }

                        public class SlabNotification
                        {
                            public static GameObject GetGameObject() { return MatchSlabOne.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class SlabAutoState
                        {
                            public static GameObject GetGameObject() { return MatchSlabOne.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class MatchSlab
                        {
                            public static GameObject GetGameObject() { return MatchSlabOne.GetGameObject().transform.GetChild(2).gameObject; }

                            public class SlabBuddyMatchVariant
                            {
                                public static GameObject GetGameObject() { return MatchSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                public class MatchForm
                                {
                                    public static GameObject GetGameObject() { return SlabBuddyMatchVariant.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class GraphicsSlab
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return GraphicsSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class FloarRock0
                                            {
                                                public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class MeshGraphicsSlab
                                            {
                                                public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class FloatRock1
                                            {
                                                public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(2).gameObject; }
                                            }
                                        }
                                    }

                                    public class MatchFormCanvas
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class MatchText
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RankNames
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class CurrRank
                                            {
                                                public static GameObject GetGameObject() { return RankNames.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToRank
                                            {
                                                public static GameObject GetGameObject() { return RankNames.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class RankUpText
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(2).gameObject; }

                                            public class TextRock0
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class TextRock1
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class TextRock2
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(2).gameObject; }
                                            }

                                            public class TextRock3
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(3).gameObject; }
                                            }

                                            public class TextRock4
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(4).gameObject; }
                                            }

                                            public class TextRock5
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(5).gameObject; }
                                            }

                                            public class TextRock6
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(6).gameObject; }
                                            }

                                            public class TextRock7
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(7).gameObject; }
                                            }

                                            public class TextRock8
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(8).gameObject; }
                                            }

                                            public class TextRock9
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(9).gameObject; }
                                            }

                                            public class TextRock10
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(10).gameObject; }
                                            }

                                            public class TextRock11
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class TextRock12
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(12).gameObject; }
                                            }

                                            public class TextRock13
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(13).gameObject; }
                                            }

                                            public class TextRock14
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(14).gameObject; }
                                            }

                                            public class TextRock15
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(15).gameObject; }
                                            }

                                            public class TextRock16
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(16).gameObject; }
                                            }

                                            public class TextRock17
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(17).gameObject; }
                                            }

                                            public class TextRock18
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(18).gameObject; }
                                            }

                                            public class TextRock19
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(19).gameObject; }
                                            }

                                            public class TextRock20
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(20).gameObject; }
                                            }

                                            public class TextRock21
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(21).gameObject; }
                                            }
                                        }

                                        public class RankUpBar
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(3).gameObject; }

                                            public class XPGainVFX
                                            {
                                                public static GameObject GetGameObject() { return RankUpBar.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LevelUpVFX
                                            {
                                                public static GameObject GetGameObject() { return RankUpBar.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class Bar
                                            {
                                                public static GameObject GetGameObject() { return RankUpBar.GetGameObject().transform.GetChild(2).gameObject; }
                                            }
                                        }
                                    }

                                    private class DisposableCollider
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class RePlayButton
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(3).gameObject; }

                                        public class SlabRockFloat
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class InteractionButton
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Button
                                            {
                                                public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class LeftHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                                }

                                                public class RightHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                                }

                                                public class Spring
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                                }
                                            }
                                        }

                                        public class RePlayText
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                        public class InteractionLightNetworked
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(3).gameObject; }

                                            public class Light
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightNetworked.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LightPlug
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightNetworked.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class InteractionLightLocal
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(4).gameObject; }

                                            public class Light
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightLocal.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LightPlug
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightLocal.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }
                                    }

                                    public class ReQueueButton
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class SlabRockFloat
                                        {
                                            public static GameObject GetGameObject() { return ReQueueButton.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class InteractionButton
                                        {
                                            public static GameObject GetGameObject() { return ReQueueButton.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Button
                                            {
                                                public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class LeftHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                                }

                                                public class RightHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                                }

                                                public class Spring
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                                }
                                            }
                                        }

                                        public class ReQueueText
                                        {
                                            public static GameObject GetGameObject() { return ReQueueButton.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                    }

                                    public class ExitMatchButton
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(5).gameObject; }

                                        public class SlabRockFloat
                                        {
                                            public static GameObject GetGameObject() { return ExitMatchButton.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class InteractionButton
                                        {
                                            public static GameObject GetGameObject() { return ExitMatchButton.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Button
                                            {
                                                public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class LeftHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                                }

                                                public class RightHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                                }

                                                public class Spring
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                                }
                                            }
                                        }

                                        public class RePlayText
                                        {
                                            public static GameObject GetGameObject() { return ExitMatchButton.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                    }

                                    public class ShiftstoneQuickswapper
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(6).gameObject; }

                                        public class FloatingButton
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class LeftHandSlab
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class RightHandSlab
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                        public class LeftHandTooltip
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(3).gameObject; }
                                        }

                                        public class RIghtHandTooltip
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(4).gameObject; }
                                        }

                                        public class ShiftstoneButtonVFX
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(5).gameObject; }
                                        }
                                    }

                                    public class Wallet
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(7).gameObject; }

                                        public class CoinText
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Wallet_
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class Cheque
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(8).gameObject; }

                                        public class CoinText
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Check
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class CoinVFX
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(9).gameObject; }
                                    }

                                    public class WalletWooshVFX
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(10).gameObject; }
                                    }
                                }
                            }
                        }

                        public class ProgressTracker
                        {
                            public static GameObject GetGameObject() { return MatchSlabOne.GetGameObject().transform.GetChild(3).gameObject; }
                        }
                    }

                    public class MatchSlabTwo
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(8).gameObject; }

                        public class SlabNotification
                        {
                            public static GameObject GetGameObject() { return MatchSlabTwo.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class SlabAutoState
                        {
                            public static GameObject GetGameObject() { return MatchSlabTwo.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class MatchSlab
                        {
                            public static GameObject GetGameObject() { return MatchSlabTwo.GetGameObject().transform.GetChild(2).gameObject; }

                            public class SlabBuddyMatchVariant
                            {
                                public static GameObject GetGameObject() { return MatchSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                public class MatchForm
                                {
                                    public static GameObject GetGameObject() { return SlabBuddyMatchVariant.GetGameObject().transform.GetChild(0).gameObject; }

                                    public class GraphicsSlab
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(0).gameObject; }

                                        public class Mesh
                                        {
                                            public static GameObject GetGameObject() { return GraphicsSlab.GetGameObject().transform.GetChild(0).gameObject; }

                                            public class FloarRock0
                                            {
                                                public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class MeshGraphicsSlab
                                            {
                                                public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class FloatRock1
                                            {
                                                public static GameObject GetGameObject() { return Mesh.GetGameObject().transform.GetChild(2).gameObject; }
                                            }
                                        }
                                    }

                                    public class MatchFormCanvas
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(1).gameObject; }

                                        public class MatchText
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class RankNames
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class CurrRank
                                            {
                                                public static GameObject GetGameObject() { return RankNames.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class ToRank
                                            {
                                                public static GameObject GetGameObject() { return RankNames.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class RankUpText
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(2).gameObject; }

                                            public class TextRock0
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class TextRock1
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class TextRock2
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(2).gameObject; }
                                            }

                                            public class TextRock3
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(3).gameObject; }
                                            }

                                            public class TextRock4
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(4).gameObject; }
                                            }

                                            public class TextRock5
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(5).gameObject; }
                                            }

                                            public class TextRock6
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(6).gameObject; }
                                            }

                                            public class TextRock7
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(7).gameObject; }
                                            }

                                            public class TextRock8
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(8).gameObject; }
                                            }

                                            public class TextRock9
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(9).gameObject; }
                                            }

                                            public class TextRock10
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(10).gameObject; }
                                            }

                                            public class TextRock11
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class TextRock12
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(12).gameObject; }
                                            }

                                            public class TextRock13
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(13).gameObject; }
                                            }

                                            public class TextRock14
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(14).gameObject; }
                                            }

                                            public class TextRock15
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(15).gameObject; }
                                            }

                                            public class TextRock16
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(16).gameObject; }
                                            }

                                            public class TextRock17
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(17).gameObject; }
                                            }

                                            public class TextRock18
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(18).gameObject; }
                                            }

                                            public class TextRock19
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(19).gameObject; }
                                            }

                                            public class TextRock20
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(20).gameObject; }
                                            }

                                            public class TextRock21
                                            {
                                                public static GameObject GetGameObject() { return RankUpText.GetGameObject().transform.GetChild(21).gameObject; }
                                            }
                                        }

                                        public class RankUpBar
                                        {
                                            public static GameObject GetGameObject() { return MatchFormCanvas.GetGameObject().transform.GetChild(3).gameObject; }

                                            public class XPGainVFX
                                            {
                                                public static GameObject GetGameObject() { return RankUpBar.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LevelUpVFX
                                            {
                                                public static GameObject GetGameObject() { return RankUpBar.GetGameObject().transform.GetChild(1).gameObject; }
                                            }

                                            public class Bar
                                            {
                                                public static GameObject GetGameObject() { return RankUpBar.GetGameObject().transform.GetChild(2).gameObject; }
                                            }
                                        }
                                    }

                                    private class DisposableCollider
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(2).gameObject; }
                                    }

                                    public class RePlayButton
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(3).gameObject; }

                                        public class SlabRockFloat
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class InteractionButton
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Button
                                            {
                                                public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class LeftHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                                }

                                                public class RightHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                                }

                                                public class Spring
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                                }
                                            }
                                        }

                                        public class RePlayText
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                        public class InteractionLightNetworked
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(3).gameObject; }

                                            public class Light
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightNetworked.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LightPlug
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightNetworked.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }

                                        public class InteractionLightLocal
                                        {
                                            public static GameObject GetGameObject() { return RePlayButton.GetGameObject().transform.GetChild(4).gameObject; }

                                            public class Light
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightLocal.GetGameObject().transform.GetChild(0).gameObject; }
                                            }

                                            public class LightPlug
                                            {
                                                public static GameObject GetGameObject() { return InteractionLightLocal.GetGameObject().transform.GetChild(1).gameObject; }
                                            }
                                        }
                                    }

                                    public class ReQueueButton
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(4).gameObject; }

                                        public class SlabRockFloat
                                        {
                                            public static GameObject GetGameObject() { return ReQueueButton.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class InteractionButton
                                        {
                                            public static GameObject GetGameObject() { return ReQueueButton.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Button
                                            {
                                                public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class LeftHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                                }

                                                public class RightHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                                }

                                                public class Spring
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                                }
                                            }
                                        }

                                        public class ReQueueText
                                        {
                                            public static GameObject GetGameObject() { return ReQueueButton.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                    }

                                    public class ExitMatchButton
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(5).gameObject; }

                                        public class SlabRockFloat
                                        {
                                            public static GameObject GetGameObject() { return ExitMatchButton.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class InteractionButton
                                        {
                                            public static GameObject GetGameObject() { return ExitMatchButton.GetGameObject().transform.GetChild(1).gameObject; }

                                            public class Button
                                            {
                                                public static GameObject GetGameObject() { return InteractionButton.GetGameObject().transform.GetChild(0).gameObject; }

                                                public class LeftHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(0).gameObject; }
                                                }

                                                public class RightHandle
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(1).gameObject; }
                                                }

                                                public class Spring
                                                {
                                                    public static GameObject GetGameObject() { return Button.GetGameObject().transform.GetChild(2).gameObject; }
                                                }
                                            }
                                        }

                                        public class RePlayText
                                        {
                                            public static GameObject GetGameObject() { return ExitMatchButton.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                    }

                                    public class ShiftstoneQuickswapper
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(6).gameObject; }

                                        public class FloatingButton
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class LeftHandSlab
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(1).gameObject; }
                                        }

                                        public class RightHandSlab
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(2).gameObject; }
                                        }

                                        public class LeftHandTooltip
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(3).gameObject; }
                                        }

                                        public class RIghtHandTooltip
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(4).gameObject; }
                                        }

                                        public class ShiftstoneButtonVFX
                                        {
                                            public static GameObject GetGameObject() { return ShiftstoneQuickswapper.GetGameObject().transform.GetChild(5).gameObject; }
                                        }
                                    }

                                    public class Wallet
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(7).gameObject; }

                                        public class CoinText
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Wallet_
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class Cheque
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(8).gameObject; }

                                        public class CoinText
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(0).gameObject; }
                                        }

                                        public class Check
                                        {
                                            public static GameObject GetGameObject() { return Wallet.GetGameObject().transform.GetChild(1).gameObject; }
                                        }
                                    }

                                    public class CoinVFX
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(9).gameObject; }
                                    }

                                    public class WalletWooshVFX
                                    {
                                        public static GameObject GetGameObject() { return MatchForm.GetGameObject().transform.GetChild(10).gameObject; }
                                    }
                                }
                            }
                        }

                        public class ProgressTracker
                        {
                            public static GameObject GetGameObject() { return MatchSlabTwo.GetGameObject().transform.GetChild(3).gameObject; }
                        }
                    }
                    
                    public class SceneBoundaryPlayers
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(9).gameObject; }
                    }

                    public class SceneBoundaryStructures
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(10).gameObject; }
                    }

                    public class Analytics
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(11).gameObject; }
                    }

                    public class ClearNotificationSlabs
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(12).gameObject; }
                    }

                    public class CombatMusic
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(13).gameObject; }
                    }

                    public class SceneProcessors
                    {
                        public static GameObject GetGameObject() { return Logic.GetGameObject().transform.GetChild(14).gameObject; }

                        public class LargeRockSpawnerWithEffects0
                        {
                            public static GameObject GetGameObject() { return SceneProcessors.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class LargeRockSpawnerWithEffects1
                        {
                            public static GameObject GetGameObject() { return SceneProcessors.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class LargeRockSpawnerWithEffects2
                        {
                            public static GameObject GetGameObject() { return SceneProcessors.GetGameObject().transform.GetChild(2).gameObject; }
                        }

                        public class LargeRockSpawnerWithEffects3
                        {
                            public static GameObject GetGameObject() { return SceneProcessors.GetGameObject().transform.GetChild(3).gameObject; }
                        }

                        public class LargeRockSpawnerWithEffects4
                        {
                            public static GameObject GetGameObject() { return SceneProcessors.GetGameObject().transform.GetChild(4).gameObject; }
                        }

                        public class LargeRockSpawnerWithEffects5
                        {
                            public static GameObject GetGameObject() { return SceneProcessors.GetGameObject().transform.GetChild(5).gameObject; }
                        }
                    }
                }

                public class Map1Production
                {
                    public static GameObject GetGameObject() { return allBaseMap1GameObjects[3]; }

                    private class CollisionGroup
                    {
                        public static GameObject GetGameObject() { return Map1Production.GetGameObject().transform.GetChild(0).gameObject; }

                        public class CombatFloor
                        {
                            public static GameObject GetGameObject() { return CollisionGroup.GetGameObject().transform.GetChild(0).gameObject; }

                            public class Col_combatfloor
                            {
                                public static GameObject GetGameObject() { return CombatFloor.GetGameObject().transform.GetChild(0).gameObject; }
                            }
                        }

                        public class Environment
                        {
                            public static GameObject GetGameObject() { return CollisionGroup.GetGameObject().transform.GetChild(1).gameObject; }

                            public class Col_Cliff
                            {
                                public static GameObject GetGameObject() { return Environment.GetGameObject().transform.GetChild(0).gameObject; }
                            }
                        }

                        public class Floor
                        {
                            public static GameObject GetGameObject() { return CollisionGroup.GetGameObject().transform.GetChild(2).gameObject; }
                        }
                    }

                    public class MainStaticGroup
                    {
                        public static GameObject GetGameObject() { return Map1Production.GetGameObject().transform.GetChild(1).gameObject; }

                        public class Cliff
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(0).gameObject; }
                        }

                        public class CombatFloor
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(1).gameObject; }
                        }

                        public class DeathDirt
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(2).gameObject; }
                        }

                        public class Leaves
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(3).gameObject; }
                        }

                        public class OuterBoundry
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(4).gameObject; }
                        }

                        public class RingClamp
                        {
                            public static GameObject GetGameObject() { return MainStaticGroup.GetGameObject().transform.GetChild(5).gameObject; }
                        }
                    }
                }

                public class VoiceLogger
                {
                    public static GameObject GetGameObject() { return allBaseMap1GameObjects[4]; }
                }
            }
        }

        public class Scene
        {

            public static DateTime WhenSceneChanged() { return whenSceneChanged; }

            public static string GetSceneName() { return currentScene; }

            public static string GetLastSceneName() { return lastScene; }
        }

        public class Managers
        {
            public static Game GetGameManager() { return gameManager; }

            public static NetworkManager GetNetworkManager() { return networkManager; }

            public static PlayerManager GetPlayerManager() { return playerManager; }

            public static Il2CppRUMBLE.Input.InputManager GetInputManager() { return inputManager; }

            public static SceneManager GetSceneManager() { return sceneManager; }

            public static NotificationManager GetNotificationManager() { return notificationManager; }

            public static StackManager GetStackManager() { return stackManager; }

            public static QualityManager GetQualityManager() { return qualityManager; }

            public static SocialHandler GetSocialHandler() { return socialHandler; }

            public static SlabManager GetSlabManager() { return slabManager; }

            public static RecordingCamera GetRecordingCamera() { return recordingCamera; }

            public static CombatManager GetCombatManager() { return combatManager; }

            public static BhapticsSDK2 GetBHapticsManager() { return bHapticsManager; }

            public static PhotonHandler GetPhotonHandler() { return photonHandler; }

            public static AudioManager GetAudioManager() { return audioManager; }

            public static GameObject GetUIGameObject() { return uIGameObject; }

            public static PoolManager GetPoolManager() { return poolManager; }
        }

        public class Gym
        {
            public static MailTube GetGymMailTube() { return mailTube; }

            public static MatchmakeConsole GetGymMatchConsole() { return matchConsole; }

            public static GameObject GetGymRegionSelectorGameObject() { return regionSelectorGameObject; }

            public static BeltRack GetGymBeltRack() { return beltRack; }

            public static PhoneHandler GetGymFriendBoard() { return gymFriendBoard; }

            public static ParkBoard GetParkBoardBasicGymVariant() { return parkBoardBasicGymVariant; }

            public static ParkBoardGymVariant GetGymParkBoardGymVariant() { return parkBoardGymVariant; }

            public static Howard GetGymHoward() { return howard; }

            public static MoveLearnHandler GetGymPoseGhostHandler() { return poseGhostHandler; }

            public static Leaderboard GetGymDailyLeaderboard() { return dailyLeaderboard; }

            public static GameObject GetGymRankStatusSlabGameObject() { return rankStatusSlabGameObject; }

            public static GameObject GetGymCommunitySlabGameObject() { return communitySlabGameObject; }

            public static ShiftstoneQuickswapper GetGymShiftstoneQuickswapper() { return gymShiftstoneQuickswapper; }

            public static ShiftstoneCabinet GetGymShiftstoneCabinet() { return shiftstoneCabinet; }

            public static GameObject GetGymGondolaGameObject() { return gymGondolaGameObject; }

            public static GameObject GetGymRanksGameObject() { return ranksGameObject; }

            public static Il2CppRUMBLE.Players.SpawnPointHandler GetGymSpawnPointHandler() { return gymSpawnPointHandler; }

            public static MatchmakingHandler GetGymMatchmakingHandler() { return matchmakingHandler; }
        }

        public class Park
        {
            public static Il2CppRUMBLE.Players.SpawnPointHandler GetParkSpawnPointHandler() { return parkSpawnPointHandler; }

            public static ParkInstance GetParkInstance() { return parkInstance; }

            public static PhoneHandler GetParkFriendBoard() { return parkFriendBoard; }

            public static ParkBoard GetParkBoardBasicParkVariant() { return parkBoardBasicParkVariant; }

            public static ParkBoardParkVariant GetParkBoardParkVariant() { return parkBoardParkVariant; }

            public static ShiftstoneQuickswapper GetParkShiftstoneQuickswapper() { return parkShiftstoneQuickswapper; }
        }

        public class Pools
        {

            public class Structures
            {
                public static GameObject GetPoolDisc() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.Disc.GetGameObject(); }

                public static GameObject GetPoolPillar() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.Pillar.GetGameObject(); }

                public static GameObject GetPoolBall() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.Ball.GetGameObject(); }

                public static GameObject GetPoolCube() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.RockCube.GetGameObject(); }

                public static GameObject GetPoolWall() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.Wall.GetGameObject(); }

                public static GameObject GetPoolSmallRock() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.SmallRock.GetGameObject(); }

                public static GameObject GetPoolLargeRock() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.LargeRock.GetGameObject(); }

                public static GameObject GetPoolBoulderBall() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.BoulderBall.GetGameObject(); }
            }

            public class ShiftStones
            {
                public static GameObject GetPoolVolatileStone() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.VolatileStone.GetGameObject(); }

                public static GameObject GetPoolChargeStone() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.ChargeStone.GetGameObject(); }

                public static GameObject GetPoolSurgeStone() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.SurgeStone.GetGameObject(); }

                public static GameObject GetPoolFlowStone() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.FlowStone.GetGameObject(); }

                public static GameObject GetPoolGuardStone() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.GuardStone.GetGameObject(); }

                public static GameObject GetPoolStubbornStone() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.StubbornStone.GetGameObject(); }

                public static GameObject GetPoolAdamantStone() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.AdamantStone.GetGameObject(); }

                public static GameObject GetPoolVigorStone() { return Calls.GameObjects.DDOL.GameInstance.PreInitializable.PoolManager.VigorStone.GetGameObject(); }
            }
        }

        public class Players
        {
            public static bool IsHost() { return PhotonNetwork.IsMasterClient; }

            public static Il2CppSystem.Collections.Generic.List<Il2CppRUMBLE.Players.Player> GetAllPlayers() { return playerManager.AllPlayers; }

            public static Il2CppRUMBLE.Players.Player GetLocalPlayer() { return playerManager.localPlayer; }

            public static PlayerController GetPlayerController() { return playerManager.localPlayer.Controller; }

            public static System.Collections.Generic.List<Il2CppRUMBLE.Players.Player> GetEnemyPlayers()
            {
                System.Collections.Generic.List<Il2CppRUMBLE.Players.Player> enemies = new System.Collections.Generic.List<Il2CppRUMBLE.Players.Player>();
                for (int i = 1; i < playerManager.AllPlayers.Count; i++) { enemies.Add(playerManager.AllPlayers[i]); }
                return enemies;
            }

            public static Il2CppRUMBLE.Players.Player GetClosestPlayer(UnityEngine.Vector3 pos, bool ignoreLocalController) { return playerManager.GetClosestPlayer(pos, ignoreLocalController); }

            public static Il2CppRUMBLE.Players.Player GetClosestPlayer(UnityEngine.Vector3 pos, PlayerController ignoreController) { return playerManager.GetClosestPlayer(pos, ignoreController); }

            public static Il2CppRUMBLE.Players.Player GetPlayerByControllerType(Il2CppRUMBLE.Players.ControllerType controllerType) { return playerManager.GetPlayer(controllerType); }

            public static Il2CppRUMBLE.Players.Player GetPlayerByActorNo(int actorNumber) { return playerManager.GetPlayerByActorNo(actorNumber); }

            public static System.Collections.Generic.List<Il2CppRUMBLE.Players.Player> GetPlayersInActorNoOrder()
            {
                bool spotFound = false;
                System.Collections.Generic.List<Il2CppRUMBLE.Players.Player> players = new System.Collections.Generic.List<Il2CppRUMBLE.Players.Player>();
                for (int i = 1; i < playerManager.AllPlayers.Count; i++)
                {
                    if ((!spotFound) && (playerManager.AllPlayers[0].Data.GeneralData.ActorNo < playerManager.AllPlayers[i].Data.GeneralData.ActorNo))
                    {
                        players.Add(playerManager.AllPlayers[0]);
                        spotFound = true;
                    }
                    players.Add(playerManager.AllPlayers[i]);
                }
                return players;
            }

            public static GameObject GetLocalHealthbarGameObject() { return localHealthbarGameObject; }
        }
        #endregion
    }
}
