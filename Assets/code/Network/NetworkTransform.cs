using System;
using SocketIO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// cotroller my character
public class NetworkTransform : MonoBehaviour
{
    [SerializeField]
    [GreyOut]
    private Vector3 oldPos;
    private NetworkIdentity networkIdentity;
    private Player player;
    private float stillCounter = 0;
    private void Start()
    {
        networkIdentity = GetComponent<NetworkIdentity>();
        player = new Player();
        player.id = networkIdentity.GetId();

        player.x = 0;
        player.y = 0;
        if (!networkIdentity.IsControlling())
        {
            enabled = false;
        }
    }
    private void Update()
    {
        if (networkIdentity.IsControlling())
        {
            if (oldPos != transform.position)
            {
                oldPos = transform.position;
                stillCounter = 0;
                sendData();
            }
            else
            {
                stillCounter += Time.deltaTime;
                if (stillCounter >= 1)
                {
                    stillCounter = 0;
                    sendData();
                }
            }
        }
    }
    private void sendData()
    {
        // update player information
        player.x = Mathf.Round(transform.position.x * 1000.0f) / 1000.0f;
        player.y = Mathf.Round(transform.position.y * 1000.0f) / 1000.0f;
        networkIdentity.GetSocket().Emit("updatePos", new JSONObject(JsonUtility.ToJson(player)));
    }
}
