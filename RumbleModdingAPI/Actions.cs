using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players;
using MelonLoader;
using System.Collections;
using UnityEngine;

namespace RumbleModdingAPI.RMAPI
{
    /// <summary>
    /// Contains all the Actions to be able to Have listeners happen under certain conditions
    /// </summary>
    public class Actions
    {
        /// <summary>
        /// Runs when Mod String is recieved from another player. Spelling Error, will remove later. use onModStringRecieved
        /// </summary>
        [Obsolete("Spelling Error, will remove later. use onModStringRecieved")]
        public static event Action onModStringRecieved;
        /// <summary>
        /// Runs when Mod String is received from another player
        /// </summary>
        public static event Action onModStringReceived;
        /// <summary>
        /// Runs when clients Mods are stored
        /// </summary>
        public static event Action onMyModsGathered;
        /// <summary>
        /// Runs when Map Changed and Map GameObjects are Stored
        /// </summary>
        public static event Action<string> onMapInitialized;
        /// <summary>
        /// Runs when a player spawns (supplies player that spawned)
        /// </summary>
        public static event Action<Player> onPlayerSpawned;
        /// <summary>
        /// Runs when a Matchmaking Match begins
        /// </summary>
        public static event Action onMatchStarted;
        /// <summary>
        /// Runs when a Matchmaking Match ends
        /// </summary>
        public static event Action onMatchEnded;
        /// <summary>
        /// Runs when a Matchmaking Round begins
        /// </summary>
        public static event Action onRoundStarted;
        /// <summary>
        /// Runs when a Matchmaking Round ends
        /// </summary>
        public static event Action onRoundEnded;
        /// <summary>
        /// Runs when a players health changes (supplies the player and amount)
        /// </summary>
        public static event Action<Player, int> onPlayerHealthChanged;

        private static void TriggerAction(Action action)
        {
            Delegate[] listeners = action?.GetInvocationList();
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
                        RumbleModdingAPI.Log("Error for Mod: " + listener.Target);
                        RumbleModdingAPI.Error(e);
                        // The loop continues to the next listener even if one fails
                    }
                }
            }
        }

        private static void TriggerAction(Action<string> action, string msg)
        {
            Delegate[] listeners = action?.GetInvocationList();
            if (listeners != null)
            {
                foreach (Delegate listener in listeners)
                {
                    try
                    {
                        // Invoke each listener individually
                        listener.DynamicInvoke(msg);
                    }
                    catch (Exception e)
                    {
                        RumbleModdingAPI.Log("Error for Mod: " + listener.Target);
                        RumbleModdingAPI.Error(e);
                        // The loop continues to the next listener even if one fails
                    }
                }
            }
        }

        private static void TriggerAction(Action<Player> action, Player player)
        {
            Delegate[] listeners = action?.GetInvocationList();
            if (listeners != null)
            {
                foreach (Delegate listener in listeners)
                {
                    try
                    {
                        // Invoke each listener individually
                        listener.DynamicInvoke(player);
                    }
                    catch (Exception e)
                    {
                        RumbleModdingAPI.Log("Error for Mod: " + listener.Target);
                        RumbleModdingAPI.Error(e);
                        // The loop continues to the next listener even if one fails
                    }
                }
            }
        }

        private static void TriggerAction(Action<Player, int> action, Player player, int amount)
        {
            Delegate[] listeners = action?.GetInvocationList();
            if (listeners != null)
            {
                foreach (Delegate listener in listeners)
                {
                    try
                    {
                        // Invoke each listener individually
                        listener.DynamicInvoke(player, amount);
                    }
                    catch (Exception e)
                    {
                        RumbleModdingAPI.Log("Error for Mod: " + listener.Target);
                        RumbleModdingAPI.Error(e);
                        // The loop continues to the next listener even if one fails
                    }
                }
            }
        }

        internal static void TriggerOnModStringReceived()
        {
            TriggerAction(onModStringRecieved);
            TriggerAction(onModStringReceived);
        }

        internal static void TriggerOnMyModsGathered()
        {
            TriggerAction(onMyModsGathered);
        }

        internal static void TriggerOnMapInitialized()
        {
            TriggerAction(onMapInitialized, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        internal static void TriggerOnPlayerSpawned(Player player)
        {
            TriggerAction(onPlayerSpawned, player);
        }

        internal static void TriggerOnMatchStarted()
        {
            TriggerAction(onMatchStarted);
        }

        internal static void TriggerOnMatchEnded()
        {
            TriggerAction(onMatchEnded);
        }

        internal static void TriggerOnRoundStarted()
        {
            TriggerAction(onRoundStarted);
        }

        internal static void TriggerOnRoundEnded()
        {
            TriggerAction(onRoundEnded);
        }

        internal static void TriggerOnPlayerHealthChanged(Player player, int amount)
        {
            TriggerAction(onPlayerHealthChanged, player, amount);
        }

        private static int[] healths;
        private static int playerCount = 0;
        private static bool waitForMatchStart = false;
        internal static int sceneCount = 0;
        internal static bool matchStarted = false;
        internal static bool modsSentThisScene = false;
        internal static bool modsReceivedThisScene = false;

        internal static IEnumerator StartActionWatcher()
        {
            yield return new WaitForFixedUpdate();
            bool gotPlayer = false;
            GameObject localPlayerGameObject = null;
            while (!gotPlayer)
            {
                try
                {
                    localPlayerGameObject = PlayerManager.instance.localPlayer.Controller.gameObject;
                    RumbleModdingAPI.Log("grabbed Local Player", true);
                }
                catch { }
                if (localPlayerGameObject == null)
                {
                    RumbleModdingAPI.Log("Local Player Null", true);
                    yield return new WaitForFixedUpdate();
                    continue;
                }
                if (localPlayerGameObject != null)
                {
                    RumbleModdingAPI.Log("Local Player != null", true);
                    gotPlayer = true;
                    try
                    {
                        RumbleModdingAPI.Log("onMapInitialized Running: " + RumbleModdingAPI.currentScene, true);
                        TriggerOnMapInitialized();
                        RumbleModdingAPI.Log("onMapInitialized Complete", true);
                        //from here
                        if (PlayerManager.instance.AllPlayers.Count > 1)
                        {
                            RumbleModdingAPI.Log("More than 1 Player", true);
                            RumbleModdingAPI.GetMods();//to here
                            if (RumbleModdingAPI.currentScene == "Map0")
                            {
                                RumbleModdingAPI.Log("Grabbing SlabOne MatchFormCanvas", true);
                                matchSlab = GameObjects.Map0.Logic.MatchSlabOne.MatchSlab.Slabbuddymatchvariant.MatchForm.MatchFormCanvas.GetGameObject();
                            }
                            else if (RumbleModdingAPI.currentScene == "Map1")
                            {
                                RumbleModdingAPI.Log("Grabbing SlabTwo MatchFormCanvas", true);
                                matchSlab = GameObjects.Map1.Logic.MatchSlabTwo.MatchSlab.Slabbuddymatchvariant.MatchForm.MatchFormCanvas.GetGameObject();
                            }
                            matchStarted = true;
                            RumbleModdingAPI.Log("Get Health Current Scene: " + RumbleModdingAPI.currentScene, true);
                            if ((RumbleModdingAPI.currentScene == "Map0") || (RumbleModdingAPI.currentScene == "Map1"))
                            {
                                RumbleModdingAPI.Log("onMatchStarted Running", true);
                                TriggerOnMatchStarted();
                                RumbleModdingAPI.Log("onRoundStarted Running", true);
                                TriggerOnRoundStarted();
                            }
                        }
                        MelonCoroutines.Start(HealthWatcher(sceneCount));
                    }
                    catch (Exception e)
                    {
                        RumbleModdingAPI.Error(e);
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

        internal static IEnumerator HealthWatcher(int sceneNumber)
        {
            RumbleModdingAPI.Log("HealthWatcher Started", true);
            yield return new WaitForSeconds(3.2f); //3.2f to allow for initial healing to full health
            RumbleModdingAPI.Log("HealthWatcher Waited 3.2 Seconds", true);
            playerCount = PlayerManager.instance.AllPlayers.Count;
            //Store Initial Healths
            healths = new int[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                healths[i] = PlayerManager.instance.AllPlayers[i].Data.HealthPoints;
            }
            waitForMatchStart = false;
            //while scene hasn't changed
            while (sceneNumber == sceneCount)
            {
                if (!waitForMatchStart)
                {
                    try
                    {
                        //if player count has changed
                        if (playerCount != PlayerManager.instance.AllPlayers.Count)
                        {
                            //Log("Player Count Changed: " + playerCount + " -> " + PlayerManager.instance.AllPlayers.Count);
                            playerCount = PlayerManager.instance.AllPlayers.Count;
                            //if in a match and only 1 player, end match
                            if (matchStarted && (playerCount == 1) && ((RumbleModdingAPI.currentScene == "Map0") || (RumbleModdingAPI.currentScene == "Map1")))
                            {
                                RumbleModdingAPI.Log("Ending Match due to 1 Player", true);
                                RumbleModdingAPI.Log("onRoundEnded Running", true);
                                TriggerOnRoundEnded();
                                RumbleModdingAPI.Log("onMatchEnded Running", true);
                                TriggerOnMatchEnded();
                                matchStarted = false;
                                break;
                            }
                            //store healths since gained/lost player
                            healths = new int[playerCount];
                            for (int i = 0; i < playerCount; i++)
                            {
                                healths[i] = PlayerManager.instance.AllPlayers[i].Data.HealthPoints;
                            }
                        }
                        //for each player
                        for (int i = 0; i < playerCount; i++)
                        {
                            //check stored healt hvs live health and check if changed
                            if (healths[i] != PlayerManager.instance.AllPlayers[i].Data.HealthPoints)
                            {
                                RumbleModdingAPI.Log("onPlayerHealthChanged Running (" + PlayerManager.instance.AllPlayers[i].Data.GeneralData.PublicUsername + ", " + (PlayerManager.instance.AllPlayers[i].Data.HealthPoints - healths[i]) + ")", true);
                                TriggerOnPlayerHealthChanged(PlayerManager.instance.AllPlayers[i], healths[i] - PlayerManager.instance.AllPlayers[i].Data.HealthPoints);
                                healths[i] = PlayerManager.instance.AllPlayers[i].Data.HealthPoints;
                                //if health just hit 0, do round end/wait stuff
                                if (((RumbleModdingAPI.currentScene == "Map0") || (RumbleModdingAPI.currentScene == "Map1")) && (healths[i] <= 0) && !waitForMatchStart)
                                {
                                    if (PlayerManager.instance.AllPlayers.Count > 1)
                                    {
                                        RumbleModdingAPI.Log("onRoundEnded Running (HP hit 0)", true);
                                        TriggerOnRoundEnded();
                                    }
                                    MelonCoroutines.Start(WaitForRoundStart(i, sceneNumber));
                                }
                            }
                        }
                    }
                    catch (Exception e) { RumbleModdingAPI.Error(e); }
                }
                yield return new WaitForFixedUpdate();
            }
            RumbleModdingAPI.Log("HealthWatcher Ending, Scene Changed", true);
            yield break;
        }

        internal static GameObject matchSlab;
        private static IEnumerator<WaitForSeconds> WaitForRoundStart(int playerNumber, int sceneNumber)
        {
            RumbleModdingAPI.Log("Waiting for Round to Start", true);
            yield return new WaitForSeconds(0.5f); //wait for health regen start
            RumbleModdingAPI.Log("WaitForSeconds(0.5f) Finished", true);
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
                        if ((PlayerManager.instance.AllPlayers.Count > 1) && ((RumbleModdingAPI.currentScene == "Map0") || (RumbleModdingAPI.currentScene == "Map1")))
                        {
                            RumbleModdingAPI.Log("onRoundStarted Running (Normal Round Start Mid Match)", true);
                            TriggerOnRoundStarted();
                        }
                    }
                    if ((PlayerManager.instance.AllPlayers.Count > 1) && ((RumbleModdingAPI.currentScene == "Map0") || (RumbleModdingAPI.currentScene == "Map1")) && !matchEnded && (matchSlab.activeSelf))
                    {
                        matchEnded = true;
                        matchStarted = false;
                        RumbleModdingAPI.Log("onMatchEnded Running (Normal End)", true);
                        TriggerOnMatchEnded();
                    }
                }
                catch { }
                yield return new WaitForSeconds(0.02f);
            }
            RumbleModdingAPI.Log("WaitForRoundStart Finished", true);
            yield break;
        }
    }
}
