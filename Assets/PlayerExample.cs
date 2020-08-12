using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Example for player class. Not important for development. Only as reference.
/// </summary>
public class PlayerExample : NetworkBehaviour
{
    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text playerInfoText;
    [SerializeField] private Text gameInfoText;
    
    public float speed = 6.0f;
    private string name;

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
            // EXAMPLE FOR ITERATING THROUGH JSONNODES
            foreach (KeyValuePair<string, JSONNode> kvp in GameServer.Instance.PlayerInfos)
            {
                playerInfoText.text += kvp.Key + " : " + kvp.Value + "\n";
            }

            foreach (KeyValuePair<string, JSONNode> kvp in GameServer.Instance.GameInfos)
            {
                gameInfoText.text += kvp.Key + " : " + kvp.Value + "\n";
            }
            
            // EXAMPLE FOR ACCESSING SINGLE VALUE
            playerNameText.text = GameServer.Instance.PlayerInfos["name"].Value;
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
}
