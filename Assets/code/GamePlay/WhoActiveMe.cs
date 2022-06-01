using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhoActiveMe : MonoBehaviour
{
    [SerializeField]
    private string whoActiveMe;
    public void SetActivation(string Id)
    {
        whoActiveMe = Id;
    }
    public string getActivation()
    {
        return whoActiveMe;
    }
}
