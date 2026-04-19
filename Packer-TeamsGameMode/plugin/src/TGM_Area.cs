using FistVR;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TeamsGameMode;

public class TGM_Area : MonoBehaviour
{

    [Header("Gameplay")]
    [HideInInspector, Tooltip("Owner\nNeutral: -1\nRed: 0\nBlue: 1")]
    public int iff = -1; //Which team currently owns this area

    [Tooltip("The area which players can capture this point in if capturable")]
    public Transform capturePoint;
    [Tooltip("The location the objective will spawn, e.g. Flag for CTF")]
    public Transform objective;

    [Tooltip("The map defined objective time for this area, e.g. capture point time")]
    public float objectiveTime = 14f;

    public SpawnPoints[] spawnPoints = new SpawnPoints[2];

    [Header("Game Objects")]
    [Tooltip("Friendly Objects that enable when owned by a friendly team")]
    public GameObject[] friendlyObjects;
    [Tooltip("Enemy Objects that are enabled when owned by an enemy team, e.g. Nav Blockers to stop enemies getting into spawn area")]
    public GameObject[] enemyObjects;
    [Tooltip("Neutral Objects that are enabled when owned by no team, e.g. Door on unused spawn room")]
    public GameObject[] neutralObjects;

    [System.Serializable]
    public class SpawnPoints()
    {
        public string name;

        [Header("Player")]
        [Tooltip("Area where players can spawn, can be scaled")]
        public Transform[] playerSpawnPoints;

        [Header("Sosigs")]
        [Tooltip("Area where sosigs can spawn, can be scaled")]
        public Transform[] sosigSpawnPoints;
        [Tooltip("Areas sosigs will navigate to when owned by enemies to find enemies")]
        public Transform[] sosigAttackAreas;
        [Tooltip("Defined areas sosigs will defend this Area")]
        public Transform[] sosigDefendAreas;
    }

    public void UpdateArea()
    {
        int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();

        //Ally/Friendly
        if (iff == playerIFF)
        {
            for (int i = 0; i < friendlyObjects.Length; i++)
            {
                if(friendlyObjects[i] != null)
                    friendlyObjects[i].SetActive(true);
            }
            for (int i = 0; i < enemyObjects.Length; i++)
            {
                if(enemyObjects[i] != null)
                    enemyObjects[i].SetActive(false);
            }
            for (int i = 0; i < neutralObjects.Length; i++)
            {
                if (neutralObjects[i] != null)
                    neutralObjects[i].SetActive(false);
            }
        }
        else if (iff == Global.GetEnemyIFF(playerIFF)) //Enemy
        {
            for (int i = 0; i < friendlyObjects.Length; i++)
            {
                if (friendlyObjects[i] != null)
                    friendlyObjects[i].SetActive(false);
            }
            for (int i = 0; i < enemyObjects.Length; i++)
            {
                if (enemyObjects[i] != null)
                    enemyObjects[i].SetActive(true);
            }
            for (int i = 0; i < neutralObjects.Length; i++)
            {
                if (neutralObjects[i] != null)
                    neutralObjects[i].SetActive(false);
            }
        }
        else //Neutral
        {
            for (int i = 0; i < friendlyObjects.Length; i++)
            {
                if (friendlyObjects[i] != null)
                    friendlyObjects[i].SetActive(false);
            }
            for (int i = 0; i < enemyObjects.Length; i++)
            {
                if (enemyObjects[i] != null)
                    enemyObjects[i].SetActive(false);
            }
            for (int i = 0; i < neutralObjects.Length; i++)
            {
                if (neutralObjects[i] != null)
                    neutralObjects[i].SetActive(true);
            }
        }
    }

    public void OpenArea()
    {
        for (int i = 0; i < friendlyObjects.Length; i++)
        {
            friendlyObjects[i].SetActive(true);
        }
        for (int i = 0; i < enemyObjects.Length; i++)
        {
            enemyObjects[i].SetActive(false);
        }
        for (int i = 0; i < neutralObjects.Length; i++)
        {
            neutralObjects[i].SetActive(false);
        }

    }

    /// <summary>
    /// Returns 2 patrol points and 3rd index as rotation 
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetRandomAttackArea()
    {
        Transform area = spawnPoints[iff].sosigAttackAreas[Random.Range(0, spawnPoints[iff].sosigAttackAreas.Length)];
        //Transform area = spawnPoints[].sosigAttackAreas[Random.Range(0, sosigAttackAreas.Length)];
        return GetRandomAreaPositions(area);
    }

    //TODO THIS NEEDS TO BE RELATIVE TO WHAT TEAM IS REQUESTING THIS DATA
    public List<Vector3> GetRandomDefendArea()
    {
        Transform area = spawnPoints[iff].sosigDefendAreas[Random.Range(0, spawnPoints[iff].sosigAttackAreas.Length)];
        return GetRandomAreaPositions(area);
    }
    public List<Vector3> GetObjectiveArea()
    {
        Transform area = objective;
        return GetRandomAreaPositions(area);
    }

    private List<Vector3> GetRandomAreaPositions(Transform area)
    {
        List<Vector3> locations = new List<Vector3>();

        Vector3 areaScale = area.localScale / 2;

        for (int i = 0; i < 2; i++)
        {
            Vector3 pos = area.position + new Vector3(
                Random.Range(-areaScale.x, areaScale.x),
                Random.Range(-areaScale.y, areaScale.y),
                Random.Range(-areaScale.z, areaScale.z));

            locations.Add(pos);
        }

        //Look Direction
        locations.Add(area.rotation.eulerAngles);

        return locations;
    }

    public static Vector3[] attackIcon = {Vector3.up, (Vector3.up * 0.5f) + (Vector3.left * 0.5f), (Vector3.up * 0.5f) + (Vector3.right * 0.5f) };

    public void PlaceAllMarkersOnGround()
    {

    }


    void OnDrawGizmos()
    {
        if (objective != null)
        {
            Gizmos.color = new Color(0.99f, 0.75f, 0);
            Vector3 upPos = objective.position + (Vector3.up * 2);
            Vector3 forPos = upPos + (Vector3.down * 0.25f) + (objective.forward / 2);

            //Draw Flag
            Gizmos.DrawLine(objective.position, upPos);
            Gizmos.DrawLine(upPos, forPos);
            Gizmos.DrawLine(forPos, objective.position + (Vector3.up * 1.5f));
        }


        for (int s = 0; s < spawnPoints.Length; s++)
        {
            if (spawnPoints[s] == null)
                continue;

            if (spawnPoints[s].playerSpawnPoints != null)
            {
                Color newGreen = Color.green;
                newGreen.a = 0.2f;
                Gizmos.color = newGreen;
                for (int i = 0; i < spawnPoints[s].playerSpawnPoints.Length; i++)
                {
                    if (spawnPoints[i] == null)
                        continue;

                    Gizmos.DrawCube(spawnPoints[s].playerSpawnPoints[i].position, spawnPoints[s].playerSpawnPoints[i].localScale / 2);
                    Vector3 wireBottom = (spawnPoints[s].playerSpawnPoints[i].localScale / 2);
                    wireBottom.y = 0.001f;
                    Vector3 cubeBottom = spawnPoints[s].playerSpawnPoints[i].position + (Vector3.down * (spawnPoints[s].playerSpawnPoints[i].localScale.y * 0.25f));
                    Gizmos.DrawWireCube(cubeBottom, wireBottom);
                    Gizmos.DrawLine(spawnPoints[s].playerSpawnPoints[i].position, spawnPoints[s].playerSpawnPoints[i].position + spawnPoints[s].playerSpawnPoints[i].forward);
                }
            }

            if (spawnPoints[s].sosigSpawnPoints != null)
            {
                Color yellow = new Color(0.999f, 0.5f, 0);
                yellow.a = 0.2f;
                Gizmos.color = yellow;
                for (int i = 0; i < spawnPoints[s].sosigSpawnPoints.Length; i++)
                {
                    if (spawnPoints[s].sosigSpawnPoints[i] == null)
                        continue;

                    Gizmos.DrawCube(spawnPoints[s].sosigSpawnPoints[i].position, spawnPoints[s].sosigSpawnPoints[i].localScale / 2);
                    Vector3 wireBottom = (spawnPoints[s].sosigSpawnPoints[i].localScale / 2);
                    wireBottom.y = 0.001f;
                    Vector3 cubeBottom = spawnPoints[s].sosigSpawnPoints[i].position + (Vector3.down * (spawnPoints[s].sosigSpawnPoints[i].localScale.y * 0.25f));
                    Gizmos.DrawWireCube(cubeBottom, wireBottom);
                    Gizmos.DrawLine(spawnPoints[s].sosigSpawnPoints[i].position, spawnPoints[s].sosigSpawnPoints[i].position + spawnPoints[s].sosigSpawnPoints[i].forward);
                }
            }

            Color newColor = s == 0 ? Color.red : Color.blue;
            newColor.a = 0.25f;
            Gizmos.color = newColor;

            if (spawnPoints[s].sosigAttackAreas != null)
            {
                for (int i = 0; i < spawnPoints[s].sosigAttackAreas.Length; i++)
                {
                    Gizmos.DrawCube(spawnPoints[s].sosigAttackAreas[i].position, spawnPoints[s].sosigAttackAreas[i].localScale / 2);
                    for (int x = 0; x < attackIcon.Length; x++)
                    {
                        Gizmos.DrawLine(spawnPoints[s].sosigAttackAreas[i].position, spawnPoints[s].sosigAttackAreas[i].position + attackIcon[x]);
                    }
                    Gizmos.DrawWireCube(spawnPoints[s].sosigAttackAreas[i].position, spawnPoints[s].sosigAttackAreas[i].localScale / 2);
                }
            }

            if (spawnPoints[s].sosigDefendAreas != null)
            {
                Vector3 BL = (Vector3.up * 0.5f) + (Vector3.left * 0.5f);
                Vector3 TL = (Vector3.up * 0.5f) + (Vector3.left * 0.5f) + (Vector3.up * 0.5f);
                Vector3 BR = (Vector3.up * 0.5f) + (Vector3.right * 0.5f);
                Vector3 TR = (Vector3.up * 0.5f) + (Vector3.right * 0.5f) + (Vector3.up * 0.5f);

                for (int i = 0; i < spawnPoints[s].sosigDefendAreas.Length; i++)
                {
                    Gizmos.DrawCube(spawnPoints[s].sosigDefendAreas[i].position, spawnPoints[s].sosigDefendAreas[i].localScale / 2);
                    Gizmos.DrawWireCube(spawnPoints[s].sosigDefendAreas[i].position, spawnPoints[s].sosigDefendAreas[i].localScale / 2);

                    Gizmos.DrawLine(spawnPoints[s].sosigDefendAreas[i].position, spawnPoints[s].sosigDefendAreas[i].position + BL);
                    Gizmos.DrawLine(spawnPoints[s].sosigDefendAreas[i].position, spawnPoints[s].sosigDefendAreas[i].position + BR);
                    Gizmos.DrawLine(spawnPoints[s].sosigDefendAreas[i].position + BL, spawnPoints[s].sosigDefendAreas[i].position + TL);
                    Gizmos.DrawLine(spawnPoints[s].sosigDefendAreas[i].position + BR, spawnPoints[s].sosigDefendAreas[i].position + TR);
                    Gizmos.DrawLine(spawnPoints[s].sosigDefendAreas[i].position + TL, spawnPoints[s].sosigDefendAreas[i].position + TR);
                }
            }
        }

        //MATRIX

        if (capturePoint != null && capturePoint.gameObject.activeSelf == true)
        {
            Gizmos.matrix = Matrix4x4.TRS(capturePoint.position, capturePoint.rotation, capturePoint.lossyScale / 2);
            Gizmos.color = new Color(0.1f, 1, 1f, 0.25f);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }

    void OnValidate()
    {
        if (spawnPoints != null && spawnPoints.Length >= 2)
        {
            spawnPoints[0].name = "Red";
            spawnPoints[1].name = "Blue";
        }
    }
}
