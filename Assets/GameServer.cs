using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.Websocket;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This class represents a game server.
/// The server starts automatically and loads the player prefab into the online scene.
/// </summary>
public class GameServer : NetworkManager
{
    /// <summary>
    /// Location of the file that holds playerInfo of
    /// the player that started the game.
    /// NOTE: This is not important for development.
    /// </summary>
    private const string FILE_NAME = "PlayerInfo.txt";

    /// <summary>
    /// Defines how long the game server will try to connect the client before quitting the application.
    /// </summary>
    [SerializeField] private float disconnectWaitTime;

    /// <summary>
    /// Stores the time until disconnect.
    /// </summary>
    private float disconnectTimer;

    /// <summary>
    /// Stores information about the player.
    /// </summary>
    private JSONNode playerInfos;
    
    /// <summary>
    /// Stores information about the game.
    /// </summary>
    private JSONNode gameInfos;

    /// <summary>
    /// Stores if the local player is the host.
    /// </summary>
    private bool isHost;

    public JSONNode PlayerInfos => playerInfos;

    public JSONNode GameInfos => gameInfos;

    public static GameServer Instance => (GameServer) singleton;

    /// <summary>
    /// Start is called before the first frame update.
    /// The server loads the player info of the local player, if:
    /// - the player is the host -> start server as host
    /// - the player is the client -> join server as client
    /// NOTE: This is not important for development.
    /// </summary>
    void Start()
    {
        base.Start();
        isHost = false;
        
        //LoadPlayerInfoMockup();     // <- FOR DEVELOPMENT

        LoadPlayerInfo();             // <- FOR RELEASE

        if (isHost)
        {
            StartHost();
        }
        else
        {
            disconnectTimer = disconnectWaitTime;
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// Checks if the local player is a client and not connected.
    /// If so the client tries to connect and the disconnect timer is updated.
    /// </summary>
    private void Update()
    {
        if (!isHost && !NetworkClient.isConnected)
        {
            disconnectTimer -= Time.deltaTime;

            if (disconnectTimer <= 0f)
            {
                Application.Quit();
            }

            StartClient();
        }
    }

    /// <summary>
    /// Loads player info from file.
    /// NOTE: This is not important for development.
    /// </summary>
    private void LoadPlayerInfo()
    {
        // Read file
        StreamReader file = new StreamReader(Application.dataPath + @"\..\..\..\..\Framework\" + FILE_NAME);
        JSONNode jsonFile = JSON.Parse(file.ReadLine());

        // Load data
        isHost = jsonFile["playerInfo"]["isHost"].AsBool;
        playerInfos = jsonFile["playerInfo"];
        gameInfos = jsonFile["gameInfo"];

        // Close file
        file.Close();
        File.Delete(FILE_NAME);
    }

    /// <summary>
    /// Sets mockup player info.
    /// The actual info is irrelevant. It does NOT matter if the info says the player is host.
    /// This will become relevant when using loadPlayerInfo.
    /// NOTE: This is important for development.
    /// </summary>
    private void LoadPlayerInfoMockup()
    {
        isHost = true;
        JSONNode tmp = new JSONObject();
        tmp.Add("name", "Mustermann");
        playerInfos.Add(tmp);
    }
}