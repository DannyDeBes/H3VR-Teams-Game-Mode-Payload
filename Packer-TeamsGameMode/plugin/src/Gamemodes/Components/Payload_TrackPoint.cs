using UnityEngine;
using UnityEngine.Events;

namespace TeamsGameMode
{
    public class Payload_TrackPoint : MonoBehaviour
    {
        [HideInInspector]
        public string pointname;
        [Tooltip("Will stop the cart from moving back past this point")]
        public bool isCheckpoint;
        [Tooltip("Will cause the cart to immediately start sliding back towards this point when left unnatended")]
        public bool isInstantSlidePoint;
        [Tooltip("Will override the speed at which the cart slides back to this point")]
        public float slideSpeedOverride = 0;
        [Tooltip("Will override the speed of the cart between this point and the next causing it to move on its own (leave at 0 to use the overall value)")]
        public float speedOverride = 0;
        [Tooltip("Multiplies the speed of the cart when moving forward between this point and the next")]
        public float forwardSpeedMultiplier = 1;
        [Tooltip("Multiplies the speed of the cart when moving backwards between this point and the next")]
        public float backwardsSpeedMultiplier = 1;
        [Tooltip("The ammount of extra time given to attackers whenever the cart passes over this point, leave at 0 for no added time")]
        public float timeAdded = 0;
        [Tooltip("The event assigned will play when the cart passes over this point.")]
        public UnityEvent passForwardEvent;
        [Tooltip("The event assigned will play when the cart passes back over this point.")]
        public UnityEvent passBackwardsEvent;
        [Tooltip("Only allows the events above to play the first time they're triggered")]
        public bool onlyPlayPassEventsOnce = true;
        [Tooltip("The ammount of time the cart will become unable to move when passing over this point (Leave -1 to disable this feature)")]
        public float timeToHoldCart = -1;
        [Tooltip("When true, the cart cannot move when over this point (dont touch unless youre doing your own scripting)")]
        public bool pointIsLocked = false;

        [Header("Sosig Setup")]

        public Transform[] attackerSpawnsOverride;
        public float attackerSpawnWaveFrequencyOverride = -1;
        public Transform[] flankPointsOverride;
        public float flankerSpawnChanceOverride = -1;

        public Transform[] defenderSpawnsOverride;
        public float defenderSpawnWaveFrequencyOverride = -1;
        public Transform[] guardPointsOverride;
        public float guardSpawnChanceOverride = -1;

        [HideInInspector]
        public bool haspassedforward = false;
        [HideInInspector]
        public bool haspassedbackwards = false;
        [HideInInspector]
        public bool timehasadded = false;
    }
}
