using System.Collections.Generic;
using H3MP.Scripts;
using UnityEngine;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;

namespace H3MP.Networking;

public class Networking
{
    static byte h3mp = 2; // 0 - No H3MP | 1 - H3MP | 2 - Not Checked 

    public static bool H3MP
    {
        get
        {
            if (h3mp == 2)
                h3mp = (byte)(Chainloader.PluginInfos.ContainsKey("VIP.TommySoucy.H3MP") ? 1 : 0);

            return h3mp == (byte)1 ? true : false;
        }
    }

    /// <summary>
    /// Returns true if a server is running
    /// </summary>
    /// <returns></returns>
    public static bool ServerRunning()
    {
        if (H3MP)
            return isServerRunning;

        return false;
    }

    static bool isServerRunning
    {
        get
        {
            if (Mod.managerObject == null)
                return false;

            return true;
        }
    }

    /// <summary>
    /// Returns true if a server is Running AND the local player is a client
    /// </summary>
    /// <returns></returns>
    public static bool IsClient()
    {
        if (H3MP)
            return isClient();

        return false;
    }

    static bool isClient()
    {
        if (Mod.managerObject == null)
            return false;

        if (ThreadManager.host == false)
            return true;
        return false;
    }

    /// <summary>
    /// Returns true if a server is Running AND the local player is the host
    /// </summary>
    /// <returns></returns>
    public static bool IsHost()
    {
        if (H3MP)
            return isHosting;

        return false;
    }

    //Soft Dependency
    static bool isHosting
    {
        get
        {
            if (Mod.managerObject == null)
                return false;

            if (ThreadManager.host == true)
                return true;
            return false;
        }
    }

    public static int GetPlayerCount()
    {
        if (H3MP)
            return GetNetworkPlayerCount();
        return 1;
    }

    static int GetNetworkPlayerCount()
    {
        return GameManager.players.Count;
    }


    /*
    /// <summary>
    /// Returns array of PlayerManagers of the current connected players (Not including the local player).
    /// </summary>
    /// <returns></returns>
    public static PlayerManager[] GetPlayers()
    {
        PlayerManager[] playerArray = new PlayerManager[GameManager.players.Count];

        int i = 0;
        foreach (KeyValuePair<int, PlayerManager> entry in GameManager.players)
        {
            playerArray[i] = entry.Value;
            i++;
        }
        return playerArray;
    }
    */

    /// <summary>
    /// Returns array of all players IDs (Does not include local player)
    /// </summary>
    /// <returns></returns>
    public static int[] GetPlayerIDs()
    {
        int[] playerArray = new int[GameManager.players.Count];

        int i = 0;
        foreach (KeyValuePair<int, PlayerManager> entry in GameManager.players)
        {
            playerArray[i] = entry.Key;
            i++;
        }

        return playerArray;
    }

    /// <summary>
    /// Returns the local player's id.
    /// </summary>
    /// <returns></returns>
    public static int GetLocalPlayerID()
    {
        return GameManager.ID;
    }

    /// <summary>
    /// Returns the Custom Packet ID
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public static int RegisterHostCustomPacket(string identifier)
    {
        int id;
        if (Mod.registeredCustomPacketIDs.ContainsKey(identifier))
            id = Mod.registeredCustomPacketIDs[identifier];
        else
            id = Server.RegisterCustomPacketType(identifier);

        return id;
    }

    /// <summary>
    /// Returns the Gamemanager player at index i, does not include the local player. 
    /// Use FistVR.GM.CurrentPlayerBody for local player.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static PlayerData GetPlayer(int i)
    {
        //Only get player if they exist
        if (i < GameManager.players.Count)
            return PlayerData.GetPlayer(i);
        else
            return null;    //Maybe return a blank player instead (vector3 zero)
    }
}


[System.Serializable]
public class NetworkData
{
    public object Value { get; private set; }
    /// <summary>
    /// The previous value set on this Network Data
    /// </summary>
    public object LastValue { get; private set; }

    public NetworkData(object value)
    {
        Value = value;
        LastValue = value;
    }
}

[System.Serializable]
public class PlayerData
{
    public Transform head;
    public string username;
    public Transform handLeft;
    public Transform handRight;
    public int ID;
    public float health;
    public int iff;

    /*
    public PlayerData(Transform playerHead, string playerName, Transform leftHand, Transform rightHand)
    {
        head = playerHead;
        username = playerName;
        handLeft = leftHand;
        handRight = rightHand;
    }
    */

    public static PlayerData GetPlayer(int i)
    {
        return new PlayerData
        {
            head = GameManager.players[i].head,
            username = GameManager.players[i].username,
            handLeft = GameManager.players[i].leftHand,
            handRight = GameManager.players[i].rightHand,
        };
    }
}