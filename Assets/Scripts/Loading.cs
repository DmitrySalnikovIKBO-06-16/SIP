using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LoadingScene
{
    Menu,
    Sc
}


public class Loading : Photon.MonoBehaviour
{

    private static LoadingScene _nextScene;
    /*
     void OnReceivedRoomListUpdate()
     {
         RoomInfo[] rooms = PhotonNetwork.GetRoomList();
         Debug.Log("test1");
         foreach (RoomInfo room in rooms)
         {
             Debug.Log("room.name" + room.ToStringFull());
         }
     }
     */
    void Start()
    {
        if (_nextScene == LoadingScene.Menu)
        {

            Debug.Log("Пришли");
            PhotonNetwork.networkingPeer.OpJoinLobby(TypedLobby.Default);
            /*
            if (PhotonNetwork.connectedAndReady)
            {
                Debug.Log("PhotonNetwork.connectedAndReady");
            }
            
            StartCoroutine(JointLobby());// запустим курротину на вход в лобби
            */
        }
    }
    /*
        private IEnumerator JointLobby()
        {
            Debug.Log("Прищли");

            Debug.Log("PhotonNetwork.networkingPeer.State:"+ PhotonNetwork.networkingPeer.State);
            Debug.Log("ClientState.ConnectingToMasterserver:" + ClientState.ConnectingToMasterserver);
            while (PhotonNetwork.networkingPeer.State != ClientState.ConnectingToMasterserver)// пока не подклюились к нашему мастер клиенту ждем
            {
                Debug.Log("PhotonNetwork.networkingPeer.State:" + PhotonNetwork.networkingPeer.State + 
                    "ClientState.ConnectingToMasterserver:" + ClientState.ConnectingToMasterserver);
                yield return new WaitForFixedUpdate();
            }
            Debug.Log("До PhotonNetwork.networkingPeer.OpJoinLobby(TypedLobby.Default);");

            PhotonNetwork.networkingPeer.OpJoinLobby(TypedLobby.Default);
        }
     */

    private void OnJoinedLobby()
    {
        Debug.Log("До PhotonNetwork.LoadLevel(Menu);");
        PhotonNetwork.LoadLevel("Menu");
        Debug.Log("после PhotonNetwork.LoadLevel(Menu);");
    }

    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Sc");
    }
    public static void Load(LoadingScene nextScene)
    {
        _nextScene = nextScene;
        PhotonNetwork.LoadLevel("Load");
    }
}