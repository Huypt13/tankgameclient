using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Server_Objects", menuName = "Scriptable Object/Server Objects", order = 3)]
public class ServerObject : ScriptableObject
{
    public List<ServerObjectData> Objects;
    public ServerObjectData GetObjectByName(string name)
    {
        return Objects.SingleOrDefault(x => x.Name == name);
    }
}

[Serializable]
public class ServerObjectData
{
    public string Name;
    public GameObject Prefab;
}