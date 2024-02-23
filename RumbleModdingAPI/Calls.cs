using Bhaptics.SDK2;
using MelonLoader;
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
using UnityEngine;

namespace RumbleModdingAPI
{
    public class Calls : MelonMod
    {
        #region Variables
        private bool sceneChanged = false;
        private static bool sceneChangedPassed = false;
        private static string currentScene = "";
        private static bool init = false;
        private static Game gameManager;
        private static NetworkManager networkManager;
        private static PlayerManager playerManager;
        private static InputManager inputManager;
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
        private static SteamAudioManager steamAudioManager;
        private static GameObject uIGameObject;
        private static GameObject gymHeinhouserProductsGameObject;
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
        private static SpawnPointHandler gymSpawnPointHandler;
        private static MatchmakingHandler matchmakingHandler;
        private static RewardHandler rewardHandler;
        private static SpawnPointHandler parkSpawnPointHandler;
        private static PhoneHandler parkFriendBoard;
        private static ParkBoard parkBoardBasicParkVariant;
        private static ParkBoardParkVariant parkBoardParkVariant;
        private static ShiftstoneQuickswapper parkShiftstoneQuickswapper;
        private static ParkInstance parkInstance;
        private static GameObject localHealthbarGameObject;
        #endregion

        #region API Initialization

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
            sceneChanged = true;
            sceneChangedPassed = true;
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
                            MelonLogger.Msg("API Initialized");
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
        #endregion
        
        #region API Calls

        public static bool IsInitialized()
        {
            return init;
        }

        public class Scene
        {
            public static bool SceneChangeStarted()
            {
                return sceneChangedPassed;
            }

            public static void SceneChangeDone()
            {
                sceneChangedPassed = false;
            }

            public static string GetSceneName()
            {
                return currentScene;
            }
        }
        
        public class Managers
        {
            public static Game GetGameManager()
            {
                return gameManager;
            }

            public static NetworkManager GetNetworkManager()
            {
                return networkManager;
            }

            public static PlayerManager GetPlayerManager()
            {
                return playerManager;
            }

            public static InputManager GetInputManager()
            {
                return inputManager;
            }

            public static SceneManager GetSceneManager()
            {
                return sceneManager;
            }

            public static NotificationManager GetNotificationManager()
            {
                return notificationManager;
            }

            public static StackManager GetStackManager()
            {
                return stackManager;
            }

            public static QualityManager GetQualityManager()
            {
                return qualityManager;
            }

            public static SocialHandler GetSocialHandler()
            {
                return socialHandler;
            }

            public static SlabManager GetSlabManager()
            {
                return slabManager;
            }

            public static RecordingCamera GetRecordingCamera()
            {
                return recordingCamera;
            }

            public static CombatManager GetCombatManager()
            {
                return combatManager;
            }

            public static BhapticsSDK2 GetBHapticsManager()
            {
                return bHapticsManager;
            }

            public static PhotonHandler GetPhotonHandler()
            {
                return photonHandler;
            }

            public static AudioManager GetAudioManager()
            {
                return audioManager;
            }

            public static SteamAudioManager GetSteamAudioManager()
            {
                return steamAudioManager;
            }

            public static GameObject GetUIGameObject()
            {
                return uIGameObject;
            }

            public static PoolManager GetPoolManager()
            {
                return poolManager;
            }
        }

        public class Gym
        {
            public static MailTube GetGymMailTube()
            {
                return mailTube;
            }

            public static MatchmakeConsole GetGymMatchConsole()
            {
                return matchConsole;
            }

            public static GameObject GetGymRegionSelectorGameObject()
            {
                return regionSelectorGameObject;
            }

            public static BeltRack GetGymBeltRack()
            {
                return beltRack;
            }

            public static PhoneHandler GetGymFriendBoard()
            {
                return gymFriendBoard;
            }

            public static ParkBoard GetParkBoardBasicGymVariant()
            {
                return parkBoardBasicGymVariant;
            }

            public static ParkBoardGymVariant GetGymParkBoardGymVariant()
            {
                return parkBoardGymVariant;
            }

            public static Howard GetGymHoward()
            {
                return howard;
            }

            public static MoveLearnHandler GetGymPoseGhostHandler()
            {
                return poseGhostHandler;
            }

            public static Leaderboard GetGymDailyLeaderboard()
            {
                return dailyLeaderboard;
            }

            public static GameObject GetGymRankStatusSlabGameObject()
            {
                return rankStatusSlabGameObject;
            }

            public static GameObject GetGymCommunitySlabGameObject()
            {
                return communitySlabGameObject;
            }

            public static ShiftstoneQuickswapper GetGymShiftstoneQuickswapper()
            {
                return gymShiftstoneQuickswapper;
            }

            public static ShiftstoneCabinet GetGymShiftstoneCabinet()
            {
                return shiftstoneCabinet;
            }

            public static GameObject GetGymGondolaGameObject()
            {
                return gymGondolaGameObject;
            }

            public static GameObject GetGymRanksGameObject()
            {
                return ranksGameObject;
            }

            public static SpawnPointHandler GetGymSpawnPointHandler()
            {
                return gymSpawnPointHandler;
            }

            public static MatchmakingHandler GetGymMatchmakingHandler()
            {
                return matchmakingHandler;
            }

            public static RewardHandler GetGymRewardHandler()
            {
                return rewardHandler;
            }
        }

        public class Park
        {
            public static SpawnPointHandler GetParkSpawnPointHandler()
            {
                return parkSpawnPointHandler;
            }

            public static ParkInstance GetParkInstance()
            {
                return parkInstance;
            }

            public static PhoneHandler GetParkFriendBoard()
            {
                return parkFriendBoard;
            }

            public static ParkBoard GetParkBoardBasicParkVariant()
            {
                return parkBoardBasicParkVariant;
            }

            public static ParkBoardParkVariant GetParkBoardParkVariant()
            {
                return parkBoardParkVariant;
            }

            public static ShiftstoneQuickswapper GetParkShiftstoneQuickswapper()
            {
                return parkShiftstoneQuickswapper;
            }
        }

        public class Pools
        {

            public class Structures
            {
                public static GameObject GetPoolDisc()
                {
                    return poolManager.transform.GetChild(43).gameObject;
                }

                public static GameObject GetPoolPillar()
                {
                    return poolManager.transform.GetChild(42).gameObject;
                }

                public static GameObject GetPoolBall()
                {
                    return poolManager.transform.GetChild(51).gameObject;
                }

                public static GameObject GetPoolCube()
                {
                    return poolManager.transform.GetChild(50).gameObject;
                }

                public static GameObject GetPoolWall()
                {
                    return poolManager.transform.GetChild(49).gameObject;
                }

                public static GameObject GetPoolSmallRock()
                {
                    return poolManager.transform.GetChild(16).gameObject;
                }

                public static GameObject GetPoolLargeRock()
                {
                    return poolManager.transform.GetChild(17).gameObject;
                }

                public static GameObject GetPoolBoulderBall()
                {
                    return poolManager.transform.GetChild(1).gameObject;
                }
            }

            public class ShiftStones
            {
                public static GameObject GetPoolVolatileStone()
                {
                    return poolManager.transform.GetChild(3).gameObject;
                }

                public static GameObject GetPoolChargeStone()
                {
                    return poolManager.transform.GetChild(4).gameObject;
                }

                public static GameObject GetPoolSurgeStone()
                {
                    return poolManager.transform.GetChild(5).gameObject;
                }

                public static GameObject GetPoolFlowStone()
                {
                    return poolManager.transform.GetChild(6).gameObject;
                }

                public static GameObject GetPoolGuardStone()
                {
                    return poolManager.transform.GetChild(7).gameObject;
                }

                public static GameObject GetPoolStubbornStone()
                {
                    return poolManager.transform.GetChild(8).gameObject;
                }

                public static GameObject GetPoolAdamantStone()
                {
                    return poolManager.transform.GetChild(9).gameObject;
                }

                public static GameObject GetPoolVigorStone()
                {
                    return poolManager.transform.GetChild(10).gameObject;
                }
            }
        }

        public class Players
        {
            public static bool IsHost()
            {
                if (playerManager.AllPlayers.Count < 2)
                {
                    return true;
                }
                else
                {
                    return (playerManager.AllPlayers[0].Data.GeneralData.ActorNo < playerManager.AllPlayers[1].Data.GeneralData.ActorNo);
                }
            }

            public static Il2CppSystem.Collections.Generic.List<Player> GetAllPlayers()
            {
                return playerManager.AllPlayers;
            }

            public static Player GetLocalPlayer()
            {
                return playerManager.localPlayer;
            }

            public static PlayerController GetPlayerController()
            {
                return playerManager.localPlayer.Controller;
            }

            public static System.Collections.Generic.List<Player> GetEnemyPlayers()
            {
                System.Collections.Generic.List<Player> enemies = new System.Collections.Generic.List<Player>();
                for (int i = 1; i < playerManager.AllPlayers.Capacity; i++)
                {
                    enemies.Add(playerManager.AllPlayers[i]);
                }
                return enemies;
            }

            public static Player GetClosestPlayer(UnityEngine.Vector3 pos, bool ignoreLocalController)
            {
                return playerManager.GetClosestPlayer(pos, ignoreLocalController);
            }

            public static Player GetClosestPlayer(UnityEngine.Vector3 pos, PlayerController ignoreController)
            {
                return playerManager.GetClosestPlayer(pos, ignoreController);
            }

            public static Player GetPlayerByControllerType(RUMBLE.Players.ControllerType controllerType)
            {
                return playerManager.GetPlayer(controllerType);
            }

            public static Player GetPlayerByActorNo(int actorNumber)
            {
                return playerManager.GetPlayerByActorNo(actorNumber);
            }

            public static System.Collections.Generic.List<Player> GetPlayersInActorNoOrder()
            {
                bool spotFound = false;
                System.Collections.Generic.List<Player> players = new System.Collections.Generic.List<Player>();
                for (int i = 1; i < playerManager.AllPlayers.Capacity; i++)
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

            public static GameObject GetLocalHealthbarGameObject()
            {
                return localHealthbarGameObject;
            }
        }
        #endregion
    }
}
