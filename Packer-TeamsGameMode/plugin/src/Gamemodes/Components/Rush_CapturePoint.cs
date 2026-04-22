using UnityEngine;
using UnityEngine.UI;
using FistVR;
using H3MP.Networking;
using System.Collections.Generic;

namespace TeamsGameMode;
public class Rush_CapturePoint : MonoBehaviour
{
    public int detectIFF = 0;   //Detect RED by default
    [HideInInspector]
    public bool canCapture = false;

    public float captureTotalTime = 5;
    private float captureTime = 0;
    public Image captureCircle;
    public GameObject capturedPrefab;
    private GameObject spawnedPrefab;

    private int handCount = 0;
    private List<Sosig> sosigCount = new List<Sosig>();


    void OnEnabled()
    {
        //Defaults on Enabled
        captureTime = 0;
        captureCircle.gameObject.SetActive(false);
        handCount = 0;
        sosigCount.Clear();

        if (spawnedPrefab != null)
            Destroy(spawnedPrefab);
    }

    void Update()
    {
        if (!canCapture)
            return;

        if (captureTime != 0)
        {
            captureCircle.transform.parent.LookAt(GM.CurrentPlayerBody.Head.position);
            captureCircle.fillAmount = Mathf.InverseLerp(0, captureTotalTime, captureTime);
        }

        if (!IsCapturing())
        {
            if (captureTime != 0)
            {
                captureTime = Mathf.Clamp(captureTime - (Time.deltaTime * 2), 0, captureTotalTime);
                captureCircle.gameObject.SetActive(false);
                return;
            }
        }
        else
        {
            //Capturing
            captureCircle.gameObject.SetActive(true);
            captureTime = Mathf.Clamp(captureTime + Time.deltaTime, 0, captureTotalTime);
            if (captureTime >= captureTotalTime)
            {
                SendCapture();
                CapturePoint();
            }
        }
    }

    public void CapturePoint()
    {
        //Called on all clients
        spawnedPrefab = Instantiate(capturedPrefab, transform.position, transform.rotation);
        spawnedPrefab.SetActive(true);

        //Disable capturing
        canCapture = false;

        //Disable
        gameObject.SetActive(false);

        TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.Point);
    }

    void SendCapture()
    {
        Debug.Log("SENDING CAPTURE NOTIFICATION");
        //Host
        if (!Networking.IsClient())
        {
            TGM_Manager.instance.gamemode.AdjustTeamScore(detectIFF, 1);
            return;
        }
        else
        {
            //Tell Server we captured it!
        }
    }

    bool IsCapturing()
    {
        for (int i = sosigCount.Count - 1; i >= 0; i--)
        {
            if (sosigCount[i] != null)
                return true;
            else
                sosigCount.RemoveAt(i); //Missing Sosig, remove it
        }

        return handCount > 0 && GM.CurrentPlayerBody.Health > 0;
    }

    void OnTriggerEnter(Collider other)
    {
        //If Player enters Trigger
        if (GM.CurrentPlayerBody.GetPlayerIFF() == detectIFF 
            && other.name.Contains("Controller ("))
        {
            handCount++;
            return;
        }

        SosigLink sosig = other.gameObject.GetComponent<SosigLink>();

        if (sosig != null 
            && sosig.S.GetIFF() == detectIFF)
        {
            sosigCount.Add(sosig.S);
        }
    }

    void OnTriggerExit(Collider other)
    {
        //If Player enters Trigger
        if (GM.CurrentPlayerBody.GetPlayerIFF() == detectIFF
            && other.name.Contains("Controller ("))
        {
            handCount--;
            return;
        }

        SosigLink sosig = other.gameObject.GetComponent<SosigLink>();

        if (sosig != null 
            && sosig.S.GetIFF() == detectIFF)
        {
            sosigCount.Remove(sosig.S);
        }
    }

    void OnValidate()
    {
        //gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}
