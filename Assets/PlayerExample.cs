using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Example for player class. Not important for development. Only as reference.
/// </summary>
public class PlayerExample : NetworkBehaviour
{
    /// <summary>
    /// Canvas of the player gui.
    /// </summary>
    [SerializeField] private Canvas playerCanvas;
    
    /// <summary>
    /// Text to display the countdown.
    /// </summary>
    [SerializeField] private Text countdownText;
    
    /// <summary>
    /// Text to display the name of the player.
    /// </summary>
    [SerializeField] private Text playerNameText;
    
    /// <summary>
    /// Text to display  the player infos from the game server.
    /// </summary>
    [SerializeField] private Text playerInfoText;
    
    /// <summary>
    /// Text to display  the game infos from the game server.
    /// </summary>
    [SerializeField] private Text gameInfoText;

    /// <summary>
    /// Camera speed.
    /// </summary>
    public float speed = 6.0f;
    
    /// <summary>
    /// Name of the player.
    /// </summary>
    private string name;
    
    /// <summary>
    /// Duration of the countdown.
    /// </summary>
    private float countdownDuration = 30f;
    
    /// <summary>
    /// Stores the rest time of the countdown.
    /// </summary>
    private float countdownTimer;
    
    /// <summary>
    /// Signals if the countdown is finished.
    /// </summary>
    private bool finished;

    /// <summary>
    /// Sets up localPlayer when started.
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        /* Activate Camera */
        Camera.main.GetComponent<TransformFollower>().Target = this.transform;
    }

    /// <summary>
    /// Disables playerCanvas on load, if not localPlayer.
    /// If localPlayer loads all player and game info.
    /// </summary>
    private void Start()
    {
        if (!isLocalPlayer)
        {
            playerCanvas.enabled = false;
        }
        else
        {
            countdownTimer = countdownDuration;
            finished = false;

            // ---------- EXAMPLE FOR ITERATING THROUGH JSONNODES ----------
            foreach (KeyValuePair<string, JSONNode> kvp in GameServer.Instance.PlayerInfos)
            {
                playerInfoText.text += kvp.Key + " : " + kvp.Value + "\n";
            }
            foreach (KeyValuePair<string, JSONNode> kvp in GameServer.Instance.GameInfos)
            {
                gameInfoText.text += kvp.Key + " : " + kvp.Value + "\n";
            }
            
            // ---------- EXAMPLE FOR ACCESSING SINGLE VALUE ----------
            playerNameText.text = GameServer.Instance.PlayerInfos["name"].Value;
        }
    }
    

    /// <summary>
    /// Only updates rotation and countdown if localPlayer.
    /// </summary>
    void Update()
    {
        if (isLocalPlayer)
        {
            float X = Input.GetAxis("Mouse X") * speed;
            float Y = Input.GetAxis("Mouse Y") * speed;

            transform.Rotate(0, X, 0);
            transform.Rotate(0, Y, 0);


            countdownTimer -= Time.deltaTime;
            countdownText.text = ((int) countdownTimer).ToString();

            if (countdownTimer <= 0f && !finished)
            {
                finished = true;
                FinishGame();
            }
        }
    }

    /// <summary>
    /// Placeholder function for the end of the game.
    /// It always sets the host as the winner and the other players as losers.
    /// </summary>
    private void FinishGame()
    {
        if (GameServer.Instance.PlayerInfos["isHost"].AsBool)
        {
            GameServer.Instance.HandleGameResults(1, GameServer.Instance.PlayerInfos["name"].Value);
        }
        else
        {
            CmdGetHostName(netId);
        }
    }

    /// <summary>
    /// Command used to get the name of the host for calling HandleGameResults, because it needs the name of the winner.
    /// </summary>
    /// <param name="connId">NetId of the player that called this command</param>
    [Command]
    public void CmdGetHostName(uint connId)
    {
        NetworkConnection receiverConnection =
            NetworkIdentity.spawned[connId].connectionToClient;
        TargetReceiveHostName(receiverConnection, GameServer.Instance.PlayerInfos["name"].Value);
    }

    /// <summary>
    /// TargetRPC that sends the hosts name back to the local player and calls HandleGameResults.
    /// </summary>
    /// <param name="connection">Connection for the TargetRPC</param>
    /// <param name="hostName">Name of the host</param>
    [TargetRpc]
    public void TargetReceiveHostName(NetworkConnection connection, string hostName)
    {
        GameServer.Instance.HandleGameResults(2,hostName);
    }
}