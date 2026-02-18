//How to clean this up more / better utilize?

using Il2CppPhoton.Pun;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players;
using UnityEngine;
using static RumbleModdingAPI.RumbleModdingAPI;

namespace RumbleModdingAPI.RMAPI
{
    /// <summary>
    /// Contains Methods to get info about different stuff
    /// </summary>
    public class Calls
    {
        #region API Calls

        /// <summary>
        /// Returns true/false on if the API is Initialized
        /// </summary>
        public static bool IsInitialized() { return init; }

        /// <summary>
        /// Returns true/false on if the Map Objects have been stored
        /// </summary>
        public static bool IsMapInitialized() { return mapInit; }

        /// <summary>
        /// Be able to retrieve Matchmaking specific info
        /// </summary>
        public class Matchmaking
        {
            /// <summary>
            /// Returns a string readout of what Matchmaking Queue type is selected
            /// </summary>
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
                        returnText = "Friends Only";
                        break;
                }
                return returnText;
            }

            /// <summary>
            /// Returns an int readout of what Matchmaking Queue type is selected
            /// </summary>
            public static int getMatchmakingTypeAsInt()
            {
                if ((currentScene != "Map0") && (currentScene != "Map1"))
                {
                    return -1;
                }
                return matchmakingType;
            }
        }

        /// <summary>
        /// Be able to retrieve Mod specific info
        /// </summary>
        public class Mods
        {
            /// <summary>
            /// Returns the Mod list as a string
            /// </summary>
            public static string getMyModString() { return myModString; }

            /// <summary>
            /// (Use only in a Match) Returns the opponents Mod list as a string
            /// </summary>
            public static string getOpponentModString() { return opponentModString; }

            /// <summary>
            /// Returns the Mod list as a custom class
            /// </summary>
            public static List<ModInfo> getMyMods() { return myMods; }

            /// <summary>
            /// (Use only in a Match) Returns the opponents Mod list as a custom class
            /// </summary>
            public static List<ModInfo> getOpponentMods() { return opponentMods; }

            /// <summary>
            /// (Use only in a Match) Returns true/false on if the opponent's mod contains a mod matching the supplied info
            /// </summary>
            /// <param name="modName"></param>
            /// <param name="ModVersion"></param>
            /// <param name="matchVersion"></param>
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

            /// <summary>
            /// Returns true/false on if a your mods contains a mod matching the supplied info
            /// </summary>
            /// <param name="modName"></param>
            /// <param name="ModVersion"></param>
            /// <param name="matchVersion"></param>
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

        /// <summary>
        /// Be able to retrieve Controller Input Values
        /// </summary>
        public class ControllerMap
        {
            /// <summary>
            /// Contains Right Controller Values
            /// </summary>
            public class RightController
            {
                /// <summary>
                /// Returns a float of the Right Controller Trigger Value
                /// </summary>
                public static float GetTrigger()
                {
                    return rightTrigger.ReadValue<float>();
                }

                /// <summary>
                /// Returns a float of the Right Controller Grip Value
                /// </summary>
                public static float GetGrip()
                {
                    return rightGrip.ReadValue<float>();
                }

                /// <summary>
                /// Returns a float of the Right Controller Primary Button Value
                /// </summary>
                public static float GetPrimary()
                {
                    return rightPrimary.ReadValue<float>();
                }

                /// <summary>
                /// Returns a float of the Right Controller Secondary Button Value
                /// </summary>
                public static float GetSecondary()
                {
                    return rightSecondary.ReadValue<float>();
                }

                /// <summary>
                /// Returns a Vector2 of the Right Controller Joystick Value
                /// </summary>
                public static Vector2 GetJoystick()
                {
                    return rightJoystick.ReadValue<Vector2>();
                }

                /// <summary>
                /// Returns a float of the Right Controller Joystick Click Value
                /// </summary>
                public static float GetJoystickClick()
                {
                    return rightJoystickClick.ReadValue<float>();
                }
            }

            /// <summary>
            /// Contains Left Controller Values
            /// </summary>
            public class LeftController
            {
                /// <summary>
                /// Returns a float of the Left Controller Trigger Value
                /// </summary>
                public static float GetTrigger()
                {
                    return leftTrigger.ReadValue<float>();
                }

                /// <summary>
                /// Returns a float of the Left Controller Grip Value
                /// </summary>
                public static float GetGrip()
                {
                    return leftGrip.ReadValue<float>();
                }

                /// <summary>
                /// Returns a float of the Left Controller Primary Button Value
                /// </summary>
                public static float GetPrimary()
                {
                    return leftPrimary.ReadValue<float>();
                }

                /// <summary>
                /// Returns a float of the Left Controller Secondary Button Value
                /// </summary>
                public static float GetSecondary()
                {
                    return leftSecondary.ReadValue<float>();
                }

                /// <summary>
                /// Returns a Vector2 of the Left Controller Joystick Value
                /// </summary>
                public static Vector2 GetJoystick()
                {
                    return leftJoystick.ReadValue<Vector2>();
                }

                /// <summary>
                /// Returns a float of the Left Controller Joystick Click Value
                /// </summary>
                public static float GetJoystickClick()
                {
                    return leftJoystickClick.ReadValue<float>();
                }
            }
        }

        /// <summary>
        /// Be able to retrieve Scene specific info
        /// </summary>
        public class Scene
        {

            /// <summary>
            /// Returns current scene name
            /// </summary>
            public static string GetSceneName() { return currentScene; }

            /// <summary>
            /// Returns last scene name
            /// </summary>
            public static string GetLastSceneName() { return lastScene; }
        }

        /// <summary>
        /// Be able to retrieve Player specific info
        /// </summary>
        public class Players
        {
            /// <summary>
            /// Returns true/false if the player is Hist of the lobby (returns false if not in a Photon Lobby)
            /// </summary>
            public static bool IsHost() { return PhotonNetwork.IsMasterClient; }

            /// <summary>
            /// honestly just use PlayerManager.instance.AllPlayers
            /// </summary>
            public static Il2CppSystem.Collections.Generic.List<Player> GetAllPlayers() { return PlayerManager.instance.AllPlayers; }

            /// <summary>
            /// honestly just use PlayerManager.instance.localPlayer
            /// </summary>
            public static Player GetLocalPlayer() { return PlayerManager.instance.localPlayer; }

            /// <summary>
            /// honestly just use PlayerManager.instance.localPlayer.Controller
            /// </summary>
            public static PlayerController GetLocalPlayerController() { return PlayerManager.instance.localPlayer.Controller; }

            /// <summary>
            /// returns a list of only other people (in PlayerManager.instance.AllPlayers order)
            /// </summary>
            public static List<Player> GetEnemyPlayers()
            {
                List<Player> enemies = new List<Player>();
                for (int i = 1; i < PlayerManager.instance.AllPlayers.Count; i++) { enemies.Add(PlayerManager.instance.AllPlayers[i]); }
                return enemies;
            }

            /// <summary>
            /// Returns the Closest Player to a specified position (able to ignore local user)
            /// </summary>
            /// <param name="position"></param>
            /// <param name="ignoreLocalController"></param>
            public static Player GetClosestPlayer(Vector3 position, bool ignoreLocalController) { return PlayerManager.instance.GetClosestPlayer(position, ignoreLocalController); }

            /// <summary>
            /// Returns the Closest Player to a specified position while ignoring a spicific player
            /// </summary>
            /// <param name="position"></param>
            /// <param name="ignoreController"></param>
            public static Player GetClosestPlayer(Vector3 position, PlayerController ignoreController) { return PlayerManager.instance.GetClosestPlayer(position, ignoreController); }

            /// <summary>
            /// Returns the Player to a specified by a controller type. (just use PlayerManager.instance.GetPlayer(controllerType))
            /// </summary>
            /// <param name="controllerType"></param>
            public static Player GetPlayerByControllerType(ControllerType controllerType) { return PlayerManager.instance.GetPlayer(controllerType); }

            /// <summary>
            /// Returns the Player with a specified Actor Number
            /// </summary>
            /// <param name="actorNumber"></param>
            public static Player GetPlayerByActorNo(int actorNumber) { return PlayerManager.instance.GetPlayerByActorNo(actorNumber); }

            /// <summary>
            /// Returns a List of Players in order of Actor Number (oldest to youngest in the room)
            /// </summary>
            public static List<Player> GetPlayersInActorNoOrder()
            {
                bool spotFound = false;
                List<Player> players = new List<Player>();
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
