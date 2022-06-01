using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

public class GameLobby : MonoBehaviour
{
    [SerializeField]
    private GameObject gameLobbyContainer;
    // Start is called before the first frame update
    void Start()
    {
        //

        Netwoking.onGameStateChange += OnGameStateChange;
        gameLobbyContainer.SetActive(false);
    }

    // Update is called once per frame
    private void OnGameStateChange(SocketIOEvent e)
    {
        string state = e.data["state"].str;
        Debug.Log("state " + state);
        switch (state)
        {
            case "Game": gameLobbyContainer.SetActive(false); break;
            case "Endgame": gameLobbyContainer.SetActive(false); break;
            case "Lobby": gameLobbyContainer.SetActive(true); break;
            default: gameLobbyContainer.SetActive(false); break;

        }
    }

}
