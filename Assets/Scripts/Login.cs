using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.ConnectUsingSettings("0.1");
    }
	
	// Update is called once per frame
	void OnConnectedToMaster ()
    {
        Debug.Log("Connected");
        Loading.Load(LoadingScene.Menu);
    }
}
