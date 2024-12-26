using UnityEngine;

namespace TeamsGameMode
{
    public class TGM_Compass : MonoBehaviour
    {
        public static TGM_Compass instance;


        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            //TODO proper checks
            if (TGM_Manager.instance == null)
                return;
        }
    }
}
