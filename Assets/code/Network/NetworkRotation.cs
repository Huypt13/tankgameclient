using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRotation : MonoBehaviour
{
    private float oldTankRotation;
    private float oldBarrelRotaion;
    [SerializeField]
    private PlayerManager playerManager;
    private NetworkIdentity networkIdentity;
    private Player player;
    private float stillCounter = 0;
    private void Start()
    {
        networkIdentity = GetComponent<NetworkIdentity>();
        player = new Player();
        player.tankRotation = 0;
        player.barrelRotation = 0;
        if (!networkIdentity.IsControlling())
        {
            enabled = false;
        }
    }
    private void Update()
    {
        if (networkIdentity.IsControlling())
        {
            if (oldTankRotation != transform.localEulerAngles.z || oldBarrelRotaion != playerManager.GetLastRotation())
            {
                oldTankRotation = transform.localEulerAngles.z;
                oldBarrelRotaion = playerManager.GetLastRotation();
                SendData();
            }
            else
            {
                stillCounter += Time.deltaTime;
                if (stillCounter >= 1)
                {
                    stillCounter = 0;
                    SendData();
                }
            }
        }
    }
    private void SendData()
    {
        player.tankRotation = Mathf.Round(transform.localEulerAngles.z * 1000.0f) / 1000.0f;
        player.barrelRotation = Mathf.Round(playerManager.GetLastRotation() * 1000.0f) / 1000.0f;
        networkIdentity.GetSocket().Emit("updateRotation", new JSONObject(JsonUtility.ToJson(player)));
    }

}
