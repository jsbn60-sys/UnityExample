using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Example for player class. Not important for development. Only as reference.
/// </summary>
public class PlayerExample : NetworkBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Canvas playerCanvas;
    public float speed = 6.0f;
    private string name;

    /// <summary>
    /// Sets up localPlayer when started.
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        /* Activate Camera */
        Camera.main.GetComponent<TransformFollower>().Target = this.transform;
        
        /*
         Inform other clients about name. The cast is a one-time workaround,
         because singleton is of type NetworkManager.
         */
        CmdSetupPlayer(((GameServer) GameServer.singleton).LocalPlayerInfo.name);
    }

    /// <summary>
    /// Disables playerCanvas on load, if not localPlayer.
    /// </summary>
    private void Start()
    {
        if (!isLocalPlayer)
        {
            playerCanvas.enabled = false;   
        }
    }

    /// <summary>
    /// Only updates rotation if localPlayer.
    /// </summary>
    void Update()
    {
        if (isLocalPlayer)
        {
            float X = Input.GetAxis("Mouse X") * speed;
            float Y = Input.GetAxis("Mouse Y") * speed;
 
            transform.Rotate(0, X, 0);
            transform.Rotate(0, Y, 0);   
        }
    }
    
    
    [Command]
    public void CmdSetupPlayer(string text)
    {
        RpcSetupPlayer(text);
    }

    [ClientRpc]
    public void RpcSetupPlayer(string text)
    {
        nameText.text = text;
    }
}
