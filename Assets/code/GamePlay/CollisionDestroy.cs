using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDestroy : MonoBehaviour
{
    [SerializeField]
    private NetworkIdentity networkIdentity;

    [SerializeField]
    private WhoActiveMe whoActiveMe;



    private void OnCollisionEnter2D(Collision2D collision)
    {


        NetworkIdentity ni = collision?.gameObject?.GetComponent<NetworkIdentity>();

        // cham cay
        if (ni == null)
        {
            Destroy(gameObject);
            Netwoking.serverObject.Remove(networkIdentity.GetId());
            return;
        }

        // dan cham nhau

        if (ni.GetComponent<WhoActiveMe>() != null)
        {
            return;
        }

        // ai ban nhau hoac ban nguoi

        var niActive = Netwoking.serverObject[whoActiveMe.getActivation()];

        if (ni.GetComponent<AIManager>() == null || niActive.GetComponent<AIManager>() == null)
        {


            if (ni.GetId() != whoActiveMe.getActivation())
            {

                networkIdentity.GetSocket().Emit("collisionDestroy", new JSONObject(JsonUtility.ToJson(new IDData()
                {
                    id = networkIdentity.GetId(),
                    enemyId = ni.GetId()
                })));
                Destroy(gameObject);
                Netwoking.serverObject.Remove(networkIdentity.GetId());
            }
        }


    }

}
