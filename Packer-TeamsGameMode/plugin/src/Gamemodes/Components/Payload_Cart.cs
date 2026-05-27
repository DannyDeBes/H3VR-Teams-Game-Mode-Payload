using System.Collections.Generic;
using System.Linq;
using TeamsGameMode;
using UnityEngine;
using UnityEngine.Events;
using static TeamsGameMode.TGM_Payload;

namespace TeamsGameMode;

[RequireComponent(typeof(Rigidbody))]
public class Payload_Cart : MonoBehaviour
{
    [Header("Cart Config")]
    [Tooltip("The volume sosigs or players have to be within to be considered on the cart")]
    public Transform captureVolume;
    [Tooltip("Comparison = cart is slowed by defenders\nDeadstop = cart comes to a deadstop if a defender is near it")]
    public Behavior cartBehavior = Behavior.Comparison;
    public enum Behavior
    {
        Comparison = 0,
        Deadstop = 1,
        /* TF2 = 2,
        Overwatch = 3,*/
    }

    [Tooltip("If enabled, defenders can push the cart backwards same as the attackers can forwards")]
    public bool defendersCanPushCartBack = false;
    [Tooltip("The base speed at which the cart moves when being pushed by one attacker")]
    public float cartForwardBaseSpeed = 1;
    [Tooltip("The percentage increase in speed per extra attacker on the cart")]
    public float attackerSpeedMultiplier = 0.10f;
    [Tooltip("The maximum ammount of attackers on the cart before the speed stops being affected")]
    public int maxAttackersPushing = 4;
    [Tooltip("The base speed at which the cart moves when being pushed back by one defender")]
    public float cartReverseBaseSpeed = 1;
    [Tooltip("The percentage decrease in speed per extra defender on the cart")]
    public float defenderSpeedMultiplier = 0.10f;
    [Tooltip("The maximum ammount of defenders on the cart before the speed stops being affected")]
    public int maxDefendersPushing = 4;
    [Tooltip("Time cart is still before it starts sliding back (change to -1 to disable this functionality)")]
    public float timeBeforeFallback = 10f;
    [Tooltip("Speed at which the cart slides back (or forward?) when left unattended for the timeBeforeFallback")]
    public float fallbackSpeed = -0.5f;
    [Tooltip("Whether an overtime mechanic exists or not (eg: if the time runs out but there are attackers on the cart the game doesnt end till theyre cleared off)")]
    public bool doesOvertime = true;
    [Tooltip("Whether an attacker has to be on the cart when the time runs out to start overtime or not")]
    public bool doesOvertimeRequireAttackers = true;
    [Tooltip("How long the cart can be left unattended in seconds whilst in overtime before the game is lost for the attackers (leave 0 to disable this mechanic)")]
    public float overtimeLength = 5;
    [Tooltip("Ammount of time the game starts with (payload generally demands seperate times from more generic gamemodes)")]
    public int timeLimitOverride = 120;

    /*[Header("Map Events")]
    [Tooltip("Unity events that are called when the cart reaches the end of the track and the game ends")]
    public UnityEvent attackerWinEvent;
    [Tooltip("Unity events that are called when the time runs out without the attackers making it to the end and the game ends")]
    public UnityEvent defenderWinEvent;
    [Tooltip("Unity events that are called locally when the game ends and the player is on the winning team.")]
    public UnityEvent playerWin;
    [Tooltip("Unity events that are called locally when the game ends and the player is on the losing team.")]
    public UnityEvent playerLose;*/

    [Header("Track Points")]
    [Tooltip("The points in sequential order (top to bottom) that the cart will follow along.")]
    public Payload_TrackPoint[] trackPoints = new Payload_TrackPoint[0];

    [Tooltip("The height from the ground points are moved to when \"Put points on ground\" is pressed in editor")]
    public float CartVerticalOffset = 0f;

    [Header("Editor Testing Options")]
    //[Tooltip("Enable to be able to test the carts behaviour and the track in editor (MUST BE DISABLED FOR CART TO WORK IN GAME!)")]
    //public bool isTestingCart = false;
    [Tooltip("Whether or not the cart controls its speed via the game logic or can have its speed set manually below in editor (MUST BE ENABLED FOR CART TO WORK IN GAME!)")]
    public bool isSpeedSetAutomatically = true;

    [Header("Values exposed for testing (only touch in play mode)")]
    private Rigidbody rb;
    public float currentSpeed = 0f;
    private int currentIndex = 0;
    private float cartPos = 0;
    private bool isSliding = false;
    private float fallbackTimer = 0f;
    private CartMovement MovementBehaviorVar = CartMovement.None;
    //public float timeMultiplier = 1;
    private int forwardcounter = 0;
    private int backwardcounter = 0;
    [HideInInspector] public bool isCartEnabled = false;
    [HideInInspector] public TGM_Payload plm;
    [HideInInspector] public int defendersOnCart = 0;
    [HideInInspector] public int attackersOnCart = 0;

    private enum CartMovement
    {
        None = 0,
        Forward = 100,
        Backward = 200,
        Sliding = 300
    }

    void OnValidate()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (trackPoints != null)
        {
            for (int i = 0; i < trackPoints.Length; i++)
            {
                if (trackPoints[i] == null)
                {
                    Debug.LogError("Missing track point at index: " + i);
                    return;
                }

                if (i < trackPoints.Length - 1 && trackPoints[i] != null && trackPoints[i + 1] != null)
                {
                    if (Vector3.Distance(trackPoints[i].transform.position, trackPoints[i + 1].transform.position) < 0.01f)
                    {
                        Debug.LogError("Track point at index: " + (i + 1) + " is too close to the track point at index: " + i);
                    }
                }

                /*
                if (trackPoints[i].isCheckpoint)
                    trackPoints[i].pointname = "Checkpoint " + i;
                else if (trackPoints[i].isInstantSlidePoint)
                    trackPoints[i].pointname = "Slide " + i;
                else
                    trackPoints[i].pointname = i.ToString();
                */
            }
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            //Debug.LogError(GetType().Name + ": \"" + gameObject.name + "\" failed to have a Rigidbody. If you're seeing this error... how?");
        }
    }

    void FixedUpdate()
    {
        if (!isCartEnabled)
        {
            return;
        }
        else
        {
            if (isSpeedSetAutomatically)
            {
                SetCartSpeed();
            }

            SetCartMovementBehavior();

            BlockPassingBackCheckpoint();

            MoveCart();

            CheckPointForward();

            //COME BACK TO THIS
            /*if (currentIndex >= trackPoints.Length - 1)
            {
                GameEnd(0);
                return;
            }*/

            CheckPointBackwards();

            float lerpPosition = InverseLerp(
                        trackPoints[currentIndex].transform.position,
                        trackPoints[currentIndex + 1].transform.position,
                        rb.position);

            cartPos = currentIndex + lerpPosition;
        }
    }

    public void CartSetup()
    {
        //isCartEnabled = false;
        rb.isKinematic = true;
        rb.position = trackPoints[0].transform.position;
        rb.rotation = trackPoints[0].transform.rotation;
        currentIndex = 0;
        cartPos = 0;
        defendersOnCart = 0;
        attackersOnCart = 0;
        fallbackTimer = 0;
        isSliding = false;
        isSpeedSetAutomatically = true;

        //Debug.Log("Game Started!");
    }

    void SetCartSpeed()
    {
        if (attackersOnCart < 0)
        {
            attackersOnCart = 0;

            foreach (PlSosig sos in plm.livesosigs)
            {
                if (sos.iff == 0)
                {
                    sos.isoncart = false;
                }
            }

            Debug.LogError("Negative ammount of attackers on cart!?");
        }
        if (defendersOnCart < 0)
        {
            defendersOnCart = 0;

            foreach (PlSosig sos in plm.livesosigs)
            {
                if (sos.iff == 1)
                {
                    sos.isoncart = false;
                }
            }

            Debug.LogError("Negative ammount of defenders on cart!?");
        }

        float movespeed = 0;

        if (attackersOnCart > 0)
        {
            isSliding = false;
            fallbackTimer = 0;
        }

        if (trackPoints[currentIndex].pointIsLocked)
        {
            currentSpeed = 0;
            return;
        }

        if (trackPoints[currentIndex].speedOverride != 0)
        {
            currentSpeed = trackPoints[currentIndex].speedOverride;

            return;
        }

        if (isSliding)
        {
            if (trackPoints[currentIndex].slideSpeedOverride != 0)
            {
                movespeed = trackPoints[currentIndex].slideSpeedOverride;
            }
            else
            {
                movespeed = fallbackSpeed;
            }

            currentSpeed = movespeed;

            return;
        }

        if (attackersOnCart == 1)
        {
            movespeed += cartForwardBaseSpeed;
        }
        else if (attackersOnCart > 1 && attackersOnCart <= maxAttackersPushing)
        {
            movespeed += cartForwardBaseSpeed * (1 + (attackerSpeedMultiplier * attackersOnCart));
        }
        else if (attackersOnCart > maxAttackersPushing)
        {
            movespeed += cartForwardBaseSpeed * (1 + (attackerSpeedMultiplier * maxAttackersPushing));
        }

        switch (cartBehavior)
        {
            case Behavior.Comparison:
                if (defendersOnCart == 1)
                {
                    movespeed -= cartReverseBaseSpeed;
                }
                else if (defendersOnCart > 1 && defendersOnCart <= maxDefendersPushing)
                {
                    movespeed -= cartReverseBaseSpeed * (1 + (defenderSpeedMultiplier * defendersOnCart));
                }
                else if (defendersOnCart > maxDefendersPushing)
                {
                    movespeed -= cartReverseBaseSpeed * (1 + (defenderSpeedMultiplier * maxDefendersPushing));
                }

                break;
            case Behavior.Deadstop:
                if (defendersOnCart > 0)
                {
                    movespeed = 0;
                }

                break;
        }

        if (!defendersCanPushCartBack && movespeed < 0)
        {
            movespeed = 0;
        }

        currentSpeed = movespeed;
    }

    void SetCartMovementBehavior()
    {
        if (!isSliding && currentSpeed == 0)
        {
            MovementBehaviorVar = CartMovement.None;
        }

        if (!isSliding && currentSpeed > 0)
        {
            MovementBehaviorVar = CartMovement.Forward;
        }

        if (!isSliding && currentSpeed < 0)
        {
            MovementBehaviorVar = CartMovement.Backward;
        }

        if (isSliding)
        {
            MovementBehaviorVar = CartMovement.Sliding;
        }
    }

    void MoveCart()
    {
        switch (MovementBehaviorVar)
        {
            case CartMovement.Forward:
                //Debug.Log("Im Going Forward!");

                rb.MoveRotation(
                    Quaternion.Lerp(
                        trackPoints[currentIndex].transform.rotation,
                        trackPoints[currentIndex + 1].transform.rotation,
                        InverseLerp(
                            trackPoints[currentIndex].transform.position,
                            trackPoints[currentIndex + 1].transform.position,
                            rb.position)));

                rb.MovePosition(
                    Vector3.MoveTowards(
                        rb.position,
                        trackPoints[currentIndex + 1].transform.position,
                        currentSpeed * Time.fixedDeltaTime));

                break;

            case CartMovement.Backward:

                //Debug.Log("Im Going Backwards!");

                rb.MoveRotation(
                    Quaternion.Lerp(
                    trackPoints[currentIndex].transform.rotation,
                    trackPoints[currentIndex + 1].transform.rotation,
                    InverseLerp(
                        trackPoints[currentIndex].transform.position,
                        trackPoints[currentIndex + 1].transform.position,
                        rb.position)));

                rb.MovePosition(
                    Vector3.MoveTowards(
                        rb.position,
                        trackPoints[currentIndex].transform.position,
                        -currentSpeed * Time.fixedDeltaTime));

                break;

            case CartMovement.Sliding:
                rb.MoveRotation(
                    Quaternion.Lerp(
                       trackPoints[currentIndex].transform.rotation,
                       trackPoints[currentIndex + 1].transform.rotation,
                       InverseLerp(
                           trackPoints[currentIndex].transform.position,
                           trackPoints[currentIndex + 1].transform.position,
                           rb.position)));

                if (currentSpeed < 0)
                {
                    rb.MovePosition(
                        Vector3.MoveTowards(
                            rb.position,
                            trackPoints[currentIndex].transform.position,
                            -currentSpeed * Time.fixedDeltaTime));
                }
                if (currentSpeed > 0)
                {
                    rb.MovePosition(
                        Vector3.MoveTowards(
                            rb.position,
                            trackPoints[currentIndex + 1].transform.position,
                            currentSpeed * Time.fixedDeltaTime));
                }

                //Debug.Log("Im Sliding!");

                break;
            case CartMovement.None:

                //Debug.Log("Im not doing anything!");
                if (attackersOnCart > 0 || trackPoints[currentIndex].pointIsLocked)
                {
                    break;
                }

                if (trackPoints[currentIndex].isInstantSlidePoint)
                {
                    isSliding = true;
                    break;
                }

                if (timeBeforeFallback > 0 && fallbackTimer < timeBeforeFallback)
                {
                    fallbackTimer += Time.deltaTime;
                }
                else if (timeBeforeFallback > 0 && fallbackTimer >= timeBeforeFallback)
                {
                    isSliding = true;
                }

                break;
        }
    }

    void BlockPassingBackCheckpoint()
    {
        if (InverseLerp(
            trackPoints[currentIndex].transform.position, trackPoints[currentIndex + 1].transform.position, rb.position) < 0.01f
            && currentSpeed < 0
            && trackPoints[currentIndex].isCheckpoint)
        {
            currentSpeed = 0;
        }
    }

    void CheckPointForward()
    {
        if (trackPoints[currentIndex].pointIsLocked)
        {
            return;
        }

        if (forwardcounter > 0)
        {
            forwardcounter--;

            return;
        }

        if (InverseLerp(
            trackPoints[currentIndex].transform.position, trackPoints[currentIndex + 1].transform.position, rb.position) > 0.99f
            && currentSpeed > 0
            && (MovementBehaviorVar == CartMovement.Forward || MovementBehaviorVar == CartMovement.Sliding)
            && currentIndex < trackPoints.Length)
        {
            PointForward();
        }
    }

    void PointForward()
    {

        currentIndex++;

        backwardcounter = 1;

        /*if (trackPoints[currentIndex].passForwardEvent != null)
        {
            if (!trackPoints[currentIndex].onlyPlayPassEventsOnce || !trackPoints[currentIndex].haspassedforward)
            {
                trackPoints[currentIndex].passForwardEvent.Invoke();

                trackPoints[currentIndex].haspassedforward = true;
            }
        }

        if (!trackPoints[currentIndex].timehasadded)
        {
            timeLeft += (trackPoints[currentIndex].timeAdded * timeMultiplier);
        }

        if (trackPoints[currentIndex].attackerSpawnsOverride != null && trackPoints[currentIndex].attackerSpawnsOverride.Count() > 0)
        {
            tempattackerSpawns.Clear();
            tempattackerSpawns.AddRange(trackPoints[currentIndex].attackerSpawnsOverride);
        }
        if (trackPoints[currentIndex].attackerSpawnWaveFrequencyOverride > -1)
        {
            if (attackerSpawnWaveFrequencyMult > 0)
            {
                tempattackerSpawnWaveFrequency = (trackPoints[currentIndex].attackerSpawnWaveFrequencyOverride / attackerSpawnWaveFrequencyMult);
            }
            else
            {
                tempattackerSpawnWaveFrequency = 0;
            }
        }

        if (trackPoints[currentIndex].defenderSpawnsOverride != null && trackPoints[currentIndex].defenderSpawnsOverride.Count() > 0)
        {
            tempdefenderSpawns.Clear();
            tempdefenderSpawns.AddRange(trackPoints[currentIndex].defenderSpawnsOverride);
        }
        if (trackPoints[currentIndex].defenderSpawnWaveFrequencyOverride > -1)
        {
            if (defenderSpawnWaveFrequencyMult > 0)
            {
                tempdefenderSpawnWaveFrequency = (trackPoints[currentIndex].defenderSpawnWaveFrequencyOverride / defenderSpawnWaveFrequencyMult);
            }
            else
            {
                tempdefenderSpawnWaveFrequency = 0;
            }
        }*/
    }

    void CheckPointBackwards()
    {
        if (trackPoints[currentIndex].pointIsLocked)
        {
            return;
        }

        if (backwardcounter > 0)
        {
            backwardcounter--;

            return;
        }

        if (InverseLerp(
            trackPoints[currentIndex].transform.position, trackPoints[currentIndex + 1].transform.position, rb.position) < 0.01f
            && currentSpeed < 0
            && !trackPoints[currentIndex].isCheckpoint
            && (MovementBehaviorVar == CartMovement.Backward || MovementBehaviorVar == CartMovement.Sliding)
            && currentIndex > 0)
        {
            PointBackwards();
        }
    }

    void PointBackwards()
    {
        //Debug.Log("Went back a point!");

        currentIndex--;

        forwardcounter = 1;

        if (trackPoints[currentIndex + 1].passBackwardsEvent != null)
        {
            if (!trackPoints[currentIndex + 1].onlyPlayPassEventsOnce || !trackPoints[currentIndex + 1].haspassedbackwards)
            {
                trackPoints[currentIndex + 1].passBackwardsEvent.Invoke();

                trackPoints[currentIndex + 1].haspassedbackwards = true;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (captureVolume != null)
        {
            Gizmos.color = new Color(0.72f, 0.23f, 0.31f, 0.6f);

            Matrix4x4 oldMatrix = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(
                captureVolume.position,
                captureVolume.rotation,
                captureVolume.lossyScale);

            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

            Gizmos.matrix = oldMatrix;
        }
    }

    float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
    }
}
