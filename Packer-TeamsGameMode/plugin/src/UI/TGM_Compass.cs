using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TeamsGameMode
{
    public class TGM_Compass : MonoBehaviour
    {
        public static TGM_Compass instance;

        public float distance = .25f;

        [Header("Health")]
        public Image healthFill;
        public Color healthFull = Color.green;
        public Color healthEmpty = Color.red;

        [Header("Stats")]
        public Text killText;
        public Text deathText;
        public Text scoreText;

        [Header("Markers")]
        public Marker[] markers;

        public Sprite[] markerSprites;
        public bool colorBlind = false;

        [Header("Direction")]
        public Transform directionNeedle;
        public Text directionText;

        [System.Serializable]
        public class Marker
        {
            public Transform parent;    //Center of compass
            public Transform marker;    //Edge of compass
            public Image thumbnail;
            public Transform target;

            public void LookAtTarget()
            {
                if (target == null)
                    return;

                parent.LookAt(target);
                parent.rotation = Quaternion.Euler(0, marker.rotation.eulerAngles.z, 0);
                marker.rotation = Quaternion.Euler(marker.rotation.eulerAngles.x, 0, 0);
            }
        }

        public void SetMarkerTaget(Marker marker, Transform newTarget, MarkerEnum type)
        {
            marker.target = newTarget;
            //marker.thumbnail.color = markerColors[(int)type];
            marker.thumbnail.sprite = markerSprites[(int)type];

        }

        public enum MarkerEnum
        {
            Attack,
            Defend,
        }

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            //TODO proper checks
            if (TGM_Manager.instance == null)
                return;

            //Compass Position
            if (!TGM_MainMenu.handSide)
                transform.position = GM.CurrentPlayerBody.LeftHand.position - GM.CurrentPlayerBody.LeftHand.forward * distance;
            else
                transform.position = GM.CurrentPlayerBody.RightHand.position - GM.CurrentPlayerBody.RightHand.forward * distance;
            transform.rotation = Quaternion.identity;

            //Markers
            for (int i = 0; i < markers.Length; i++)
            {
                markers[i].LookAtTarget();
            }

            //Health
            healthFill.fillAmount = GM.GetPlayerHealth();
            healthFill.color = Color.Lerp(healthEmpty, healthFull, GM.GetPlayerHealth());

            //Direction
            directionNeedle.LookAt(GM.CurrentPlayerBody.Head.position);
            directionText.text = Mathf.FloorToInt(directionNeedle.eulerAngles.y).ToString();
            directionNeedle.rotation = Quaternion.Euler(0,0, directionNeedle.eulerAngles.z);

        }
    }
}
