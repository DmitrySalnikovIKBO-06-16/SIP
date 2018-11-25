using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public delegate void Fptr();

public class Menu1 : MonoBehaviour
{
    RoomInfo[] rooms;
    public Texture2D BackWall;
    public MenuState state;
    private List<buttonMenu> list;
    int Summ;
    string MapName = "map1";
    int PlayerCount = 10;
    //параметры кнопок
    static readonly int BtnH = 30; //высота
    static readonly int BtnW = 200;//ширина
    static readonly int BtnO = 10; //отступ



    public enum MenuState
    {
        Home,
        Create,
        Add_Room,
        Connect,
        //Exit
        Options
    }

    public struct buttonMenu
    {
        public buttonMenu(string name, Fptr ptr, string lvl)
        {
            buttonName = name;
            pointer = ptr;
            LobbyLvl = lvl;

        }
        public string buttonName;
        public Fptr pointer;
        public string LobbyLvl;
    }


    void Start()
    {
        UnityEngine.Cursor.visible = true;
        state = MenuState.Home;
        list = new List<buttonMenu>();
        Fptr ptr;
        ptr = Exit;
        list.Add(new buttonMenu("Exit", ptr, "Home"));
        ptr = Back;
        list.Add(new buttonMenu("Back", ptr, "Add_Room"));
        ptr = Back;
        list.Add(new buttonMenu("Back", ptr, "Connect"));
        ptr = Options;
        list.Add(new buttonMenu("Options", ptr, "Home"));
        ptr = Connect;
        list.Add(new buttonMenu("Connect", ptr, "Home"));
        ptr = Add_Room;
        list.Add(new buttonMenu("Add_Room", ptr, "Home"));
        ptr = Create;
        list.Add(new buttonMenu("Create", ptr, "Add_Room"));



    }
    void Update()
    {
        UnityEngine.Cursor.visible = true;
    }
    void OnReceivedRoomListUpdate()
    {
        rooms = PhotonNetwork.GetRoomList();
        Debug.Log("room.name");
        foreach (RoomInfo room in rooms)
        {
            Debug.Log("room.name" + room.ToStringFull());
        }
    }

    void Create()
    {
        if (PhotonNetwork.insideLobby)
        {
            foreach (RoomInfo room in rooms)
            {
                if (room.Name == MapName)
                {
                    Debug.Log("Такая комната уже есть");
                    return;
                }
            }
            PhotonNetwork.JoinOrCreateRoom(MapName, new RoomOptions(), TypedLobby.Default);
        }

        //PhotonNetwork.JoinRoom(MapName);

        /*
        PhotonNetwork.CreateRoom(MapName, new RoomOptions()
        {
            isVisible = true,
            maxPlayers = (byte)PlayerCount,
            CustomRoomProperties = { { "map", MapName }, }
        }, null, null);
        */

    }
    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Sc");
    }
    void Connect()
    {
        state = MenuState.Connect;
    }
    void Back()
    {
        state = MenuState.Home;
    }
    void Add_Room()
    {
        state = MenuState.Add_Room;
    }
    void Exit()
    {
        Application.Quit();
    }
    void Options()
    {
        // Должно быть state = MenuState.Options; но пока это не сделано, так что как есть.
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BackWall);

        switch (state)
        {
            case MenuState.Home:
                DrawMenu();
                break;
            case MenuState.Add_Room:
                DrawAdd_Room();
                break;
            case MenuState.Connect:
                DrawConnect();
                break;
        }
    }


    void DrawMenu()
    {
        GUI.Box(new Rect((Screen.width) / 9, (Screen.height) / 6, (Screen.width) / 4, (Screen.height) * 3 / 4), "");
        Summ = list.Count * (BtnH + BtnO) - BtnO;
        int k = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].LobbyLvl == "Home")
            {
                k++;
                if (GUI.Button(new Rect((Screen.width - BtnW) / 6, (Screen.height / 2 + Summ / 2 - BtnH - k * (BtnO + BtnH)), BtnW, BtnH), list[i].buttonName))
                {
                    list[i].pointer();
                }
            }

        }
    }
    void DrawAdd_Room()
    {
        GUI.Box(new Rect((Screen.width) / 9, (Screen.height) / 6, (Screen.width) / 4, (Screen.height) * 3 / 4), "");
        GUI.Box(new Rect(((Screen.width) / 9) + ((Screen.width) / 4), (Screen.height) * 1 / 6, (Screen.width) / 10, (Screen.height) / 10), "Map name");
        GUI.Box(new Rect(((Screen.width) / 9) + ((Screen.width) / 4), (Screen.height) * 2 / 6, (Screen.width) / 10, (Screen.height) / 10), "Players");
        //TAB так и задуман
        MapName = GUI.TextField(new Rect(((Screen.width) / 9) + ((Screen.width) / 4) + ((Screen.width) / 10), (Screen.height) * 1 / 6, (Screen.width) / 10, (Screen.height) / 10), MapName);
        PlayerCount = Int32.Parse(GUI.TextField(new Rect(((Screen.width) / 9) + ((Screen.width) / 4) + ((Screen.width) / 10), (Screen.height) * 2 / 6, (Screen.width) / 10, (Screen.height) / 10), PlayerCount.ToString()));


        Summ = list.Count * (BtnH + BtnO) - BtnO;
        int k = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].LobbyLvl == "Add_Room")
            {
                k++;
                if (GUI.Button(new Rect((Screen.width - BtnW) / 6, (Screen.height / 2 + Summ / 2 - BtnH - k * (BtnO + BtnH)), BtnW, BtnH), list[i].buttonName))
                {
                    list[i].pointer();
                }
            }

        }
    }
    void DrawConnect()
    {
        GUI.Box(new Rect((Screen.width) / 9, (Screen.height) / 6, (Screen.width) / 4, (Screen.height) * 3 / 4), "");

        GUI.Box(new Rect((Screen.width) / 9 + (Screen.width) / 4, (Screen.height) / 6, (Screen.width) / 8, (Screen.height) / 10), "");
        GUI.Label(new Rect((Screen.width) / 9 + (Screen.width) / 4, (Screen.height) / 6, (Screen.width) / 4, (Screen.height) / 10), "Name");
        GUI.Box(new Rect((Screen.width) / 9 + (Screen.width) / 4 + (Screen.width) / 8, (Screen.height) / 6, (Screen.width) / 8, (Screen.height) / 10), "");
        GUI.Label(new Rect((Screen.width) / 9 + (Screen.width) / 4 + (Screen.width) / 8, (Screen.height) / 6, (Screen.width) / 4, (Screen.height) / 10), "Players");

        Summ = list.Count * (BtnH + BtnO) - BtnO;
        int k = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].LobbyLvl == "Connect")
            {
                k++;
                if (GUI.Button(new Rect((Screen.width - BtnW) / 6, (Screen.height / 2 + Summ / 2 - BtnH - k * (BtnO + BtnH)), BtnW, BtnH), list[i].buttonName))
                {
                    list[i].pointer();
                }
            }
        }
        for (int i = 0; i < rooms.Length; i++)
        {
            GUI.Box(new Rect((Screen.width) / 9 + (Screen.width) / 4, (Screen.height) / 6 + (i + 1) * ((Screen.height) / 10), (Screen.width) / 8, (Screen.height) / 10), "");
            GUI.Label(new Rect((Screen.width) / 9 + (Screen.width) / 4, (Screen.height) / 6 + (i + 1) * ((Screen.height) / 10), (Screen.width) / 8, (Screen.height) / 10), rooms[i].Name);

            GUI.Box(new Rect((Screen.width) / 9 + (Screen.width) / 4 + (Screen.width) / 8, (Screen.height) / 6 + (i + 1) * ((Screen.height) / 10), (Screen.width) / 8, (Screen.height) / 10), "");
            GUI.Label(new Rect((Screen.width) / 9 + (Screen.width) / 4 + (Screen.width) / 8, (Screen.height) / 6 + (i + 1) * ((Screen.height) / 10), (Screen.width) / 8, (Screen.height) / 10), (rooms[i].PlayerCount).ToString());

            if (GUI.Button(new Rect((Screen.width) / 9 + (Screen.width) / 2, (Screen.height) / 6 + (i + 1) * ((Screen.height) / 10), Screen.width / 10, Screen.height / 10), "Join"))
            {
                PhotonNetwork.JoinRoom(rooms[i].Name);
            }
        }
    }
}