using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class Netwoking : SocketIOComponent
{
    public Transform networkContainer;

    public const float SERVER_UPDATE_TIME = 20;
    public static Dictionary<string, NetworkIdentity> serverObject;
    public GameObject playerPerfab;
    public GameObject healthComponent;
    public static string ClientID { get; private set; }

    [SerializeField]
    private ServerObject serverSpawnables;


    public static Action<SocketIOEvent> onGameStateChange = (E) => { };

    public override void Start()
    {
        base.Start();
        SetUpEvent();
        serverObject = new Dictionary<string, NetworkIdentity>();

        //Dictionary<string, string> data = new Dictionary<string, string>();
        //data["room"] = "a";
        //data["name"] = "123";

        //Emit("joinRoom", new JSONObject(data));
        //  Emit("`x");
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    void SetUpEvent()
    {
        On("open", (e) =>
        {
            Debug.Log("Connect success");
        });
        // thong bao someone join
        On("register", (e) =>
        {
            ClientID = e.data["id"].ToString().Replace("\"", "");

        });
        // tao my nhan vat
        On("spawn", (e) =>
        {

            string id = e.data["id"].ToString().Replace("\"", "");
            Debug.Log(id);
            GameObject g = Instantiate(playerPerfab, networkContainer);
            g.name = $"{id}";
            NetworkIdentity ni = g.GetComponent<NetworkIdentity>();
            ni.SetControllerId(id);
            ni.SetSocketReference(this);
            serverObject.Add(id, ni);

            GameObject h = Instantiate(healthComponent, networkContainer);
            var healthBar = h.transform.GetComponentInChildren<HealthBar>();
            healthBar.SetHealth(100);
            healthBar.SetMaxHealth(100);
            healthBar.setIsMyHealth(true);
            healthBar.setMyGamTransform(g.transform);
            if (id == ClientID)
            {
                healthBar.fill.color = new Color(51, 196, 56);
            }
            h.name = $"Health : {id}";
            ni.setHealthBar(healthBar);

        });

        On("disconnected", (e) =>
        {
            string id = e.data["id"].ToString().Replace("\"", "");
            GameObject go = serverObject[id].gameObject;
            Destroy(go);
            serverObject[id].getHealthBar().DestroyHealthBar();
            serverObject.Remove(id);
        });

        On("updatePos", (e) =>
        {
            Debug.Log("lala");
            string id = e.data["id"].ToString().Replace("\"", "");
            float x = e.data["position"]["x"].f;
            float y = e.data["position"]["y"].f;
            NetworkIdentity ni = serverObject[id];
            ni.transform.position = new Vector3(x, y, 0);

        });

        On("updateRotation", (e) =>
        {

            string id = e.data["id"].ToString().Replace("\"", "");
            float tankRotation = e.data["tankRotation"].f;
            float barrelRotation = e.data["barrelRotation"].f;

            NetworkIdentity ni = serverObject[id];
            ni.transform.localEulerAngles = new Vector3(0, 0, tankRotation);
            ni.GetComponent<PlayerManager>().SetRotaion(barrelRotation);
        });

        On("updateAI", (E) =>
        {
            string id = E.data["id"].ToString().Replace("\"", "");
            float x = E.data["position"]["x"].f;
            float y = E.data["position"]["y"].f;
            float tankRotation = E.data["tankRotation"].f;
            float barrelRotation = E.data["barrelRotation"].f;

            NetworkIdentity ni = serverObject[id];
            //  ni.transform.position = new Vector3(x, y, 0);
            StartCoroutine(AIPositionSmoothing(ni.transform, new Vector3(x, y, 0)));
            if (ni.gameObject.activeInHierarchy)
            {
                ni.GetComponent<AIManager>().SetTankRotation(tankRotation);
                ni.GetComponent<AIManager>().SetBarrelRotation(barrelRotation);
            }

        });

        On("serverSpawn", (e) =>
        {

            string id = e.data["id"].ToString().Replace("\"", "");
            string name = e.data["name"].ToString().Replace("\"", "");
            float x = e.data["position"]["x"].f;
            float y = e.data["position"]["y"].f;
            Debug.Log("server want to spawn " + name);
            if (!serverObject.ContainsKey(id))
            {

                Debug.Log("dan " + x + " " + y);

                ServerObjectData sod = serverSpawnables.GetObjectByName(name);
                GameObject spawnedObject = Instantiate(sod.Prefab, networkContainer);
                spawnedObject.name = id;
                spawnedObject.transform.position = new Vector3(x, y, 0);
                NetworkIdentity ni = spawnedObject.GetComponent<NetworkIdentity>();


                ni.SetControllerId(id);
                ni.SetSocketReference(this);

                if (name == "Bullet")
                {

                    string activator = e.data["activator"].ToString().Replace("\"", "");
                    float directionX = e.data["direction"]["x"].f;
                    float directionY = e.data["direction"]["y"].f;
                    float speed = e.data["speed"].f;

                    var whoActiveMe = spawnedObject.GetComponent<WhoActiveMe>();
                    whoActiveMe.SetActivation(activator);

                    float rot = Mathf.Atan2(directionY, directionX) * Mathf.Rad2Deg;
                    Vector3 currentRotation = new Vector3(0, 0, rot - 90);
                    spawnedObject.transform.rotation = Quaternion.Euler(currentRotation);

                    ProjectTile projectTile = spawnedObject.GetComponent<ProjectTile>();
                    projectTile.Direction = new Vector2(directionX, directionY);
                    projectTile.Speed = speed;
                }
                if (name == "AI_Tank")
                {
                    GameObject h = Instantiate(healthComponent, networkContainer);
                    var healthBar = h.transform.GetComponentInChildren<HealthBar>();
                    healthBar.SetHealth(100);
                    healthBar.SetMaxHealth(100);
                    healthBar.setIsMyHealth(false);
                    healthBar.setMyGamTransform(spawnedObject.transform);
                    h.name = $"Health : {id}";
                    ni.setHealthBar(healthBar);
                }
                serverObject.Add(id, ni);
            }
        });



        On("playerDied", (e) =>
        {
            string id = e.data["id"].ToString().Replace("\"", "");
            var ni = serverObject[id];
            if (ni.GetComponent<AIManager>())
            {
                ni.GetComponent<AIManager>().StopCoroutines();
            }

            ni.getHealthBar().transform.parent.gameObject.SetActive(false);
            ni.gameObject.SetActive(false);
        });
        On("playerRespawn", (e) =>
        {
            string id = e.data["id"].ToString().Replace("\"", "");
            var ni = serverObject[id];
            float x = e.data["position"]["x"].f;
            float y = e.data["position"]["y"].f;
            ni.transform.position = new Vector3(x, y, 0);
            ni.gameObject.SetActive(true);
            ni.getHealthBar().SetHealth(100);
            ni.getHealthBar().transform.parent.gameObject.SetActive(true);
        });
        On("playerAttacked", (e) =>
        {
            string id = e.data["id"].ToString().Replace("\"", "");
            Debug.Log(id + " .");
            float health = e.data["health"].f;
            var ni = serverObject[id];
            var healthBar = ni.getHealthBar();
            healthBar.SetHealth(health);

        });
        On("loadGame", (e) =>
        {
            SceneManagerment.Instance.LoadLevel(SceneList.LEVEL, (levelName) =>
            {
                SceneManagerment.Instance.UnLoadLevel(SceneList.MAIN_MENU);
            });
        });

        On("lobbyUpdate", (e) =>
        {
            Debug.Log("Lobby update" + e.data["state"].str);
            onGameStateChange.Invoke(e);
        });
    }

    private IEnumerator AIPositionSmoothing(Transform aiTransform, Vector3 goalPosition)
    {
        float count = 0.05f; //In sync with server update
        float currentTime = 0.0f;
        Vector3 startPosition = aiTransform.position;

        while (currentTime < count)
        {
            currentTime += Time.deltaTime;

            if (currentTime < count)
            {
                aiTransform.position = Vector3.Lerp(startPosition, goalPosition, currentTime / count);
            }

            yield return new WaitForEndOfFrame();

            if (aiTransform == null)
            {
                currentTime = count;
                yield return null;
            }
        }

        yield return null;
    }

    public void AttempToJoinLobby()
    {
        Emit("joinGame");
    }
}


public class Player
{
    public string id;
    public float x;
    public float y;
    public float tankRotation;
    public float barrelRotation;
}

[Serializable]
public class BulletData
{
    public string id;
    public string activator;
    public Position position;
    public Position direction;
}

[Serializable]
public class Position
{
    public float x;
    public float y;
}

[Serializable]
public class IDData
{
    public string id;
    public string enemyId;
}