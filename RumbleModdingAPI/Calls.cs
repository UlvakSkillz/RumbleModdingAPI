using Bhaptics.SDK2;
using Il2CppSystem.Collections.Generic;
using Photon.Pun;
using RUMBLE.Combat;
using RUMBLE.Combat.ShiftStones.UI;
using RUMBLE.Environment;
using RUMBLE.Environment.Howard;
using RUMBLE.Environment.Matchmaking;
using RUMBLE.Input;
using RUMBLE.Managers;
using RUMBLE.MoveSystem;
using RUMBLE.Players;
using RUMBLE.Rewards;
using RUMBLE.Social;
using RUMBLE.Social.Phone;
using RUMBLE.Tutorial.MoveLearning;
using RUMBLE.Utilities;
using SteamAudio;
using System.Collections.Generic;
using UnityEngine;

namespace RumbleModdingAPI
{
    public class Calls
    {
        private bool sceneChanged = false;
        private bool sceneChangedPassed = false;
        private string currentScene = "";
        private bool init = false;
        private Game gameManager;
        private NetworkManager networkManager;
        private PlayerManager playerManager;
        private InputManager inputManager;
        private SceneManager sceneManager;
        private NotificationManager notificationManager;
        private StackManager stackManager;
        private QualityManager qualityManager;
        private SocialHandler socialHandler;
        private SlabManager slabManager;
        private RecordingCamera recordingCamera;
        private CombatManager combatManager;
        private PoolManager poolManager;
        private BhapticsSDK2 bHapticsManager;
        private PhotonHandler photonHandler;
        private AudioManager audioManager;
        private SteamAudioManager steamAudioManager;
        private GameObject uIGameObject;
        private GameObject gymHeinhouserProductsGameObject;
        private MailTube mailTube;
        private MatchmakeConsole matchConsole;
        private GameObject regionSelectorGameObject;
        private BeltRack beltRack;
        private PhoneHandler gymFriendBoard;
        private ParkBoard parkBoardBasicGymVariant;
        private ParkBoardGymVariant parkBoardGymVariant;
        private Howard howard;
        private MoveLearnHandler poseGhostHandler;
        private Leaderboard dailyLeaderboard;
        private GameObject rankStatusSlabGameObject;
        private GameObject communitySlabGameObject;
        private ShiftstoneQuickswapper gymShiftstoneQuickswapper;
        private ShiftstoneCabinet shiftstoneCabinet;
        private GameObject gymGondolaGameObject;
        private GameObject ranksGameObject;
        private SpawnPointHandler gymSpawnPointHandler;
        private MatchmakingHandler matchmakingHandler;
        private RewardHandler rewardHandler;
        private SpawnPointHandler parkSpawnPointHandler;
        private PhoneHandler parkFriendBoard;
        private ParkBoard parkBoardBasicParkVariant;
        private ParkBoardParkVariant parkBoardParkVariant;
        private ShiftstoneQuickswapper parkShiftstoneQuickswapper;
        private ParkInstance parkInstance;
        private GameObject localHealthbarGameObject;

        public void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
            sceneChanged = true;
            sceneChangedPassed = true;
        }

        public void OnFixedUpdate()
        {
            if (sceneChanged)
            {
                try
                {
                    if (currentScene == "Loader")
                    {
                        if (!init)
                        {
                            bHapticsManager = GameObject.Find("[bHaptics]").GetComponent<BhapticsSDK2>();
                            photonHandler = GameObject.Find("PhotonMono").GetComponent<PhotonHandler>();
                            gameManager = GameObject.Find("Game Instance").GetComponent<Game>();
                            audioManager = GameObject.Find("Game Instance/Pre-Initializable/AudioManager").GetComponent<AudioManager>();
                            steamAudioManager = GameObject.Find("Game Instance/Pre-Initializable/AudioManager/Steam Audio Manager Settings").GetComponent<SteamAudioManager>();
                            poolManager = GameObject.Find("Game Instance/Pre-Initializable/PoolManager").GetComponent<PoolManager>();
                            networkManager = GameObject.Find("Game Instance/Initializable/NetworkManager").GetComponent<NetworkManager>();
                            playerManager = GameObject.Find("Game Instance/Initializable/PlayerManager").GetComponent<PlayerManager>();
                            inputManager = GameObject.Find("Game Instance/Initializable/InputManager").GetComponent<InputManager>();
                            sceneManager = GameObject.Find("Game Instance/Initializable/SceneManager").GetComponent<SceneManager>();
                            notificationManager = GameObject.Find("Game Instance/Initializable/NotificationManager").GetComponent<NotificationManager>();
                            stackManager = GameObject.Find("Game Instance/Initializable/StackManager").GetComponent<StackManager>();
                            qualityManager = GameObject.Find("Game Instance/Initializable/QualityManager").GetComponent<QualityManager>();
                            socialHandler = GameObject.Find("Game Instance/Initializable/SocialHandler").GetComponent<SocialHandler>();
                            slabManager = GameObject.Find("Game Instance/Initializable/SlabManager").GetComponent<SlabManager>();
                            recordingCamera = GameObject.Find("Game Instance/Initializable/RecordingCamera").GetComponent<RecordingCamera>();
                            combatManager = GameObject.Find("Game Instance/Other/CombatManager").GetComponent<CombatManager>();
                            uIGameObject = GameObject.Find("Game Instance/UI");
                            init = true;
                        }
                    }
                    else if (currentScene == "Gym")
                    {
                        gymHeinhouserProductsGameObject = GameObject.Find("--------------LOGIC--------------/Heinhouser products");
                        mailTube = gymHeinhouserProductsGameObject.transform.GetChild(0).GetComponent<MailTube>();
                        matchConsole = gymHeinhouserProductsGameObject.transform.GetChild(1).GetComponent<MatchmakeConsole>();
                        regionSelectorGameObject = gymHeinhouserProductsGameObject.transform.GetChild(2).gameObject;
                        beltRack = gymHeinhouserProductsGameObject.transform.GetChild(3).GetComponent<BeltRack>();
                        gymFriendBoard = gymHeinhouserProductsGameObject.transform.GetChild(4).GetComponent<PhoneHandler>();
                        parkBoardBasicGymVariant = gymHeinhouserProductsGameObject.transform.GetChild(5).GetComponent<ParkBoard>();
                        parkBoardGymVariant = gymHeinhouserProductsGameObject.transform.GetChild(5).GetComponent<ParkBoardGymVariant>();
                        howard = gymHeinhouserProductsGameObject.transform.GetChild(6).GetComponent<Howard>();
                        poseGhostHandler = gymHeinhouserProductsGameObject.transform.GetChild(7).GetComponent<MoveLearnHandler>();
                        dailyLeaderboard = gymHeinhouserProductsGameObject.transform.GetChild(8).GetComponent<Leaderboard>();
                        rankStatusSlabGameObject = gymHeinhouserProductsGameObject.transform.GetChild(9).gameObject;
                        communitySlabGameObject = gymHeinhouserProductsGameObject.transform.GetChild(10).gameObject;
                        gymShiftstoneQuickswapper = gymHeinhouserProductsGameObject.transform.GetChild(11).GetComponent<ShiftstoneQuickswapper>();
                        shiftstoneCabinet = gymHeinhouserProductsGameObject.transform.GetChild(12).GetComponent<ShiftstoneCabinet>();
                        gymGondolaGameObject = gymHeinhouserProductsGameObject.transform.GetChild(13).gameObject;
                        ranksGameObject = gymHeinhouserProductsGameObject.transform.GetChild(14).gameObject;
                        gymSpawnPointHandler = GameObject.Find("--------------LOGIC--------------/Handelers/SpawnPointHandler").GetComponent<SpawnPointHandler>();
                        matchmakingHandler = GameObject.Find("--------------LOGIC--------------/Handelers/Matchmaking handler").GetComponent<MatchmakingHandler>();
                        rewardHandler = GameObject.Find("--------------LOGIC--------------/Handelers/RewardHandler").GetComponent<RewardHandler>();
                    }
                    else if (currentScene == "Park")
                    {
                        parkSpawnPointHandler = GameObject.Find("________________LOGIC__________________ /SpawnPointHandler").GetComponent<SpawnPointHandler>();
                        parkInstance = GameObject.Find("________________LOGIC__________________ /Park Instance").GetComponent<ParkInstance>();
                        parkFriendBoard = GameObject.Find("________________LOGIC__________________ /Heinhouwser products/Telephone 2.0 REDUX special edition").GetComponent<PhoneHandler>();
                        parkBoardBasicParkVariant = GameObject.Find("________________LOGIC__________________ /Heinhouwser products/Parkboard (Park)").GetComponent<ParkBoard>();
                        parkBoardParkVariant = GameObject.Find("________________LOGIC__________________ /Heinhouwser products/Parkboard (Park)").GetComponent<ParkBoardParkVariant>();
                        parkShiftstoneQuickswapper = GameObject.Find("________________LOGIC__________________ /ShiftstoneQuickswapper").GetComponent<ShiftstoneQuickswapper>();
                    }
                    if (currentScene != "Loader")
                    {
                        localHealthbarGameObject = GameObject.Find("Health");
                    }
                }
                catch
                {
                    return;
                }
                sceneChanged = false;
            }
        }

        public bool IsInitialized()
        {
            return init;
        }

        public bool SceneChangeStarted()
        {
            return sceneChangedPassed;
        }

        public void SceneChangeDone()
        {
            sceneChangedPassed = false;
        }

        public string GetSceneName()
        {
            return currentScene;
        }

        public Game GetGameManager()
        {
            return gameManager;
        }

        public NetworkManager GetNetworkManager()
        {
            return networkManager;
        }

        public PlayerManager GetPlayerManager()
        {
            return playerManager;
        }

        public InputManager GetInputManager()
        {
            return inputManager;
        }

        public SceneManager GetSceneManager()
        {
            return sceneManager;
        }

        public NotificationManager GetNotificationManager()
        {
            return notificationManager;
        }

        public StackManager GetStackManager()
        {
            return stackManager;
        }

        public QualityManager GetQualityManager()
        {
            return qualityManager;
        }

        public SocialHandler GetSocialHandler()
        {
            return socialHandler;
        }

        public SlabManager GetSlabManager()
        {
            return slabManager;
        }

        public RecordingCamera GetRecordingCamera()
        {
            return recordingCamera;
        }

        public CombatManager GetCombatManager()
        {
            return combatManager;
        }

        public BhapticsSDK2 GetBHapticsManager()
        {
            return bHapticsManager;
        }

        public PhotonHandler GetPhotonHandler()
        {
            return photonHandler;
        }

        public AudioManager GetAudioManager()
        {
            return audioManager;
        }

        public SteamAudioManager GetSteamAudioManager()
        {
            return steamAudioManager;
        }

        public PoolManager GetPoolManager()
        {
            return poolManager;
        }

        public GameObject GetPoolDisc()
        {
            return poolManager.transform.GetChild(43).gameObject;
        }

        public GameObject GetPoolPillar()
        {
            return poolManager.transform.GetChild(42).gameObject;
        }

        public GameObject GetPoolBall()
        {
            return poolManager.transform.GetChild(51).gameObject;
        }

        public GameObject GetPoolCube()
        {
            return poolManager.transform.GetChild(50).gameObject;
        }

        public GameObject GetPoolWall()
        {
            return poolManager.transform.GetChild(49).gameObject;
        }

        public GameObject GetPoolSmallRock()
        {
            return poolManager.transform.GetChild(16).gameObject;
        }

        public GameObject GetPoolLargeRock()
        {
            return poolManager.transform.GetChild(17).gameObject;
        }

        public GameObject GetPoolBoulderBall()
        {
            return poolManager.transform.GetChild(1).gameObject;
        }

        public GameObject GetPoolVolatileStone()
        {
            return poolManager.transform.GetChild(3).gameObject;
        }

        public GameObject GetPoolChargeStone()
        {
            return poolManager.transform.GetChild(4).gameObject;
        }

        public GameObject GetPoolSurgeStone()
        {
            return poolManager.transform.GetChild(5).gameObject;
        }

        public GameObject GetPoolFlowStone()
        {
            return poolManager.transform.GetChild(6).gameObject;
        }

        public GameObject GetPoolGuardStone()
        {
            return poolManager.transform.GetChild(7).gameObject;
        }

        public GameObject GetPoolStubbornStone()
        {
            return poolManager.transform.GetChild(8).gameObject;
        }

        public GameObject GetPoolAdamantStone()
        {
            return poolManager.transform.GetChild(9).gameObject;
        }

        public GameObject GetPoolVigorStone()
        {
            return poolManager.transform.GetChild(10).gameObject;
        }

        public GameObject GetUIGameObject()
        {
            return uIGameObject;
        }

        public Il2CppSystem.Collections.Generic.List<Player> GetAllPlayers()
        {
            return playerManager.AllPlayers;
        }

        public Player GetLocalPlayer()
        {
            return playerManager.localPlayer;
        }

        public PlayerController GetPlayerController()
        {
            return playerManager.localPlayer.Controller;
        }

        public System.Collections.Generic.List<Player> GetEnemyPlayers()
        {
            System.Collections.Generic.List<Player> enemies = new System.Collections.Generic.List<Player>();
            for(int i = 1; i < playerManager.AllPlayers.Capacity; i++)
            {
                enemies.Add(playerManager.AllPlayers[i]);
            }
            return enemies;
        }

        public Player GetClosestPlayer(UnityEngine.Vector3 pos, bool ignoreLocalController)
        {
            return playerManager.GetClosestPlayer(pos, ignoreLocalController);
        }

        public Player GetClosestPlayer(UnityEngine.Vector3 pos, PlayerController ignoreController)
        {
            return playerManager.GetClosestPlayer(pos, ignoreController);
        }

        public Player GetPlayerByControllerType(RUMBLE.Players.ControllerType controllerType)
        {
            return playerManager.GetPlayer(controllerType);
        }

        public Player GetPlayerByActorNo(int actorNumber)
        {
            return playerManager.GetPlayerByActorNo(actorNumber);
        }

        public MailTube GetGymMailTube()
        {
            return mailTube;
        }

        public MatchmakeConsole GetGymMatchConsole()
        {
            return matchConsole;
        }

        public GameObject GetGymRegionSelectorGameObject()
        {
            return regionSelectorGameObject;
        }

        public BeltRack GetGymBeltRack()
        {
            return beltRack;
        }
        
        public PhoneHandler GetGymFriendBoard()
        {
            return gymFriendBoard;
        }

        public ParkBoard GetParkBoardBasicGymVariant()
        {
            return parkBoardBasicGymVariant;
        }

        public ParkBoardGymVariant GetGymParkBoardGymVariant()
        {
            return parkBoardGymVariant;
        }

        public Howard GetGymHoward()
        {
            return howard;
        }

        public MoveLearnHandler GetGymPoseGhostHandler()
        {
            return poseGhostHandler;
        }

        public Leaderboard GetGymDailyLeaderboard()
        {
            return dailyLeaderboard;
        }

        public GameObject GetGymRankStatusSlabGameObject()
        {
            return rankStatusSlabGameObject;
        }

        public GameObject GetGymCommunitySlabGameObject()
        {
            return communitySlabGameObject;
        }

        public ShiftstoneQuickswapper GetGymShiftstoneQuickswapper()
        {
            return gymShiftstoneQuickswapper;
        }

        public ShiftstoneCabinet GetGymShiftstoneCabinet()
        {
            return shiftstoneCabinet;
        }

        public GameObject GetGymGondolaGameObject()
        {
            return gymGondolaGameObject;
        }

        public GameObject GetGymRanksGameObject()
        {
            return ranksGameObject;
        }

        public SpawnPointHandler GetGymSpawnPointHandler()
        {
            return gymSpawnPointHandler;
        }

        public MatchmakingHandler GetGymMatchmakingHandler()
        {
            return matchmakingHandler;
        }

        public RewardHandler GetGymRewardHandler()
        {
            return rewardHandler;
        }

        public SpawnPointHandler GetParkSpawnPointHandler()
        {
            return parkSpawnPointHandler;
        }

        public ParkInstance GetParkInstance()
        {
            return parkInstance;
        }

        public PhoneHandler GetParkFriendBoard()
        {
            return parkFriendBoard;
        }

        public ParkBoard GetParkBoardBasicParkVariant()
        {
            return parkBoardBasicParkVariant;
        }

        public ParkBoardParkVariant GetParkBoardParkVariant()
        {
            return parkBoardParkVariant;
        }

        public ShiftstoneQuickswapper GetParkShiftstoneQuickswapper()
        {
            return parkShiftstoneQuickswapper;
        }

        public GameObject GetLocalHealthbarGameObject()
        {
            return localHealthbarGameObject;
        }
    }
}
