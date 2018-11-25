using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectScript : Photon.MonoBehaviour {

    private double timerEnemySpawn;
    public  double timerEnemySpawnOffset = 4;
    public  double EnemySpawnOffsetPerPlayer = 15;
    public  float R = 60.0f;
    public  float r = 12.0f;
    public Dictionary<string, int> EnemyTypeArr;
    string key;
    int summ;

    public string PlName { get; set; }


    // Use this for initialization
    void Start () {
        EnemyTypeArr = new Dictionary<string, int>();
        EnemyTypeArr.Add("EnemyCyl",10); //Название префаба и вероятность (интом, чем больше - тем вероятнее)
        EnemyTypeArr.Add("EnemyCylHard", 3); //Название префаба и вероятность (интом, чем больше - тем вероятнее)
        key = "EnemyCyl";
        //PhotonNetwork.Connect("app.exitgamescloud.com", 5055, "здесь мог быть ваш AppID", "v 0.1");
        /*PhotonNetwork.ConnectUsingSettings("0.1");
        Debug.Log("OnJoinedLobby");
        if (PhotonNetwork.connectedAndReady)
        {
            Debug.Log("PhotonNetwork.connectedAndReady");
        }*/
        Spawn();

    }
	
	// Update is called once per frame
	void Update () {
        if (PhotonNetwork.isMasterClient)
        {
            if (timerEnemySpawn > 0) timerEnemySpawn -= Time.deltaTime;
            else
            {
                summ = 0;
                foreach (KeyValuePair<string, int> pair in EnemyTypeArr)
                {
                    summ += pair.Value;
                }
                int Randx = Random.Range(1, summ);
                foreach (KeyValuePair<string, int> pair in EnemyTypeArr)
                {
                    Randx = Randx - pair.Value;
                    if (Randx <= 0)
                    {
                        key = pair.Key;
                        break;
                    }
                }
                Randx = Random.Range(1, 4);
                timerEnemySpawn = timerEnemySpawnOffset;
                int length = GameObject.FindGameObjectsWithTag("Player").Length;
                if (length > 0 && GameObject.FindGameObjectsWithTag("Enemy").Length < EnemySpawnOffsetPerPlayer * length)
                    for (int i = 0; i < Random.Range(1, length); i++)
                    {
                             if (Randx == 1) Spawn(key, new Vector3(Random.Range(r, R), 1, Random.Range(r, R)), Quaternion.identity);
                        else if (Randx == 2) Spawn(key, new Vector3(Random.Range(r, R), 1, -Random.Range(r, R)), Quaternion.identity);
                        else if (Randx == 3) Spawn(key, new Vector3(-Random.Range(r, R), 1, Random.Range(r, R)), Quaternion.identity);
                        else if (Randx == 4) Spawn(key, new Vector3(-Random.Range(r, R), 1, -Random.Range(r, R)), Quaternion.identity);
                    }


            }
        }
		
	}
    public void SetName() //измените эту функцию
    {
        if (PhotonNetwork.playerList.Length == 1)
        {
            PlName = "Main Player";
            PhotonNetwork.playerName = PlName;
        }
        else if (PhotonNetwork.playerList.Length > 1)
        {
            PlName = "Player " + PhotonNetwork.playerList.Length;
            PhotonNetwork.playerName = PlName;

        }
        else
        {
            Debug.Log("No players?!");
        }
    }
    public void SetName(string Name) //измените эту функцию
    {
        foreach (PhotonPlayer pl in PhotonNetwork.playerList)
        {
            if (pl.NickName==Name)
            {
                //Debug.LogWarning("Cannot set name \""+Name+"\", name exists already!");
                return;
            }
        }
        //Debug.Log("For player "+PlName+" has been set name to: "+Name);
        PlName = Name;
        PhotonNetwork.playerName = PlName;
    }

    void OnJoinedLobby()
    {
        Debug.Log("RoomListLength: " + PhotonNetwork.GetRoomList().Length);
        if (PhotonNetwork.insideLobby)
        {
            Debug.Log("Inside a Lobby");
            var rooms = PhotonNetwork.GetRoomList();
            foreach (var room in rooms)
                Debug.Log("Found room: " + room.ToString());
        }
        PhotonNetwork.JoinOrCreateRoom("My_Room", new RoomOptions(), TypedLobby.Default);
        Debug.Log("RoomListLength: "+PhotonNetwork.GetRoomList().Length);
    }
    void OnReceivedRoomListUpdate()
    {
        var rooms = PhotonNetwork.GetRoomList();
        foreach (var room in rooms)
            Debug.Log("Found room: " + room.ToString());
    }

    void OnJoinedRoom()
    {
        Spawn();
    }
    public void Spawn()
    {
        SetName();
        //PhotonNetwork.playerName=PlName;
        PhotonNetwork.Instantiate("Player", Vector3.up * 5f, Quaternion.identity, 0);
    }
    public void Spawn(string obj, Vector3 where, Quaternion how)
    {
        PhotonNetwork.InstantiateSceneObject(obj, where, how, 0,null);
    }
    public void ReSpawn(GameObject gm)
    {
        //Debug.Log("Respawn gm: " + gm.name);
        PhotonNetwork.Destroy(gm);
        PhotonNetwork.Instantiate("Player", Vector3.up * 5f, Quaternion.identity, 0);
    }
    public void DeSpawn(GameObject gm)
    {
        //Debug.Log("Destroy gm: "+gm.name);
        PhotonNetwork.Destroy(gm);
    }
}
