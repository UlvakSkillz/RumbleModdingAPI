//do to:
//rework away from onFixedUpdate If Checks
//create methods to trigger these since they can't be triggered outside the class

namespace RumbleModdingAPI.RMAPI
{
    public class Actions
    {
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
    }
}
