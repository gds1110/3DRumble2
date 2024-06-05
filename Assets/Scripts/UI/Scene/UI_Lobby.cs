using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Services.Analytics.Internal.Platform;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : UI_Scene
{

    enum Btns
    {
        PlayBtn, DeckBtn, ExitBtn, RoomBtn,
    }
    enum txt
    {
        PlayerNameTxt
    }
    enum GameObjects
    {
        LobbyRoomPanel
    }
    public TextMeshProUGUI userCount;
    public GameObject LobbyPanel;
    public GameObject RoomPanel;
    public Button prevBtn;
    public Button nxtBtn;
    public List<Button> roomBtns;

    public TextMeshProUGUI RoomInfotext;
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Btns));
        Bind<TextMeshProUGUI>(typeof(txt));
        //Bind<GameObject>(typeof(GameObjects));
        Get<Button>((int)Btns.DeckBtn).onClick.AddListener(GoDeckBuildScene);
        Get<Button>((int)Btns.ExitBtn).onClick.AddListener(ExitBtn);
        Get<Button>((int)Btns.PlayBtn).onClick.AddListener(GoPlayScene);
        Get<Button>((int)Btns.RoomBtn).onClick.AddListener(OpenRoom);
    }
    public void GoPlayScene(/*0: vs Cpu , 1: hotseatMode*/)
    {
        Managers.Game.SetGameMode(0);

        //PlayerDeck deck = Resources.Load("DeckSo", typeof(ScriptableObject)) as PlayerDeck;
        bool isData = true;
        //if (deck != null)
        //{
        //    foreach (var item in deck.unitDatas)
        //    {
        //        if (item == null)
        //        {
        //            isData = false;
        //        }

        //    }
        //}
        //if (!deck) isData = false;

        string jsonFilePath = Application.persistentDataPath + $"/playerDeck/Deck_data/{PhotonNetwork.NickName}.txt";
        if (!File.Exists(jsonFilePath))
        {
            isData = false;
            var t = Managers.UI.ShowPopupUI<UI_MsgPopup>("MsgPopup");
            t.Init();
            t.SetText("Need DeckBuilding");
        }


        //if (isData) { Managers.Scene.LoadScene(Define.Scene.GameNetWork); }
        //else
        //{
        //    var t = Managers.UI.ShowPopupUI<UI_MsgPopup>("MsgPopup");
        //    t.Init();
        //    t.SetText("Need DeckBuilding");
        //}
    }
    public void HotseatPlay()
    {
        Managers.Game.SetGameMode(1);

        PlayerDeck deck = Resources.Load("DeckSo", typeof(ScriptableObject)) as PlayerDeck;
        bool isData = true;
        if (deck != null)
        {
            foreach (var item in deck.unitDatas)
            {
                if (item == null)
                {
                    isData = false;
                }

            }
        }
        if (!deck) isData = false;

        if (isData) { Managers.Scene.LoadScene(Define.Scene.GameNetWork); }
        else
        {
            var t = Managers.UI.ShowPopupUI<UI_MsgPopup>("MsgPopup");
            t.Init();
            t.SetText("Need DeckBuilding");
        }
    }

    public void OpenRoom()
    {
        if (LobbyPanel != null)
        {
            LobbyPanel.SetActive(true);
            PhotonManager.instance.CheckInteratable(prevBtn, nxtBtn, roomBtns);
        }

        //Get<GameObject>((int)GameObjects.LobbyRoomPanel).SetActive(true);
    }
    public void GoDeckBuildScene()
    {
        Managers.Scene.LoadScene(Define.Scene.Deck);
    }
    public void ExitBtn()
    {
        Application.Quit();
    }
    public void ExitRoomMenu()
    {
        LobbyPanel.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();

        Get<TextMeshProUGUI>((int)txt.PlayerNameTxt).SetText($"Name : {PhotonNetwork.NickName}");
        PhotonManager.instance.ListRenewalHandle += RenwalList;
        PhotonManager.instance.JoinRoomHandle += OnJoinRoom;

    }



    // Update is called once per frame
    void Update()
    {

        if (userCount)
            userCount.text = PhotonNetwork.CountOfPlayers.ToString();
    }

    public void ListClick(int num)
    {
        PhotonManager.instance.MyListClick(num);
    }

    public void RenwalList()
    {
        PhotonManager.instance.CheckInteratable(prevBtn, nxtBtn, roomBtns);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName, new RoomOptions { MaxPlayers = 2 });
      //  PhotonNetwork.JoinRoom(PhotonNetwork.NickName);
    }




    void OnJoinRoom()
    {
        if(RoomPanel)
            RoomPanel.SetActive(true);
        if(RoomInfotext)
            RoomInfotext.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "∏Ì / " + PhotonNetwork.CurrentRoom.MaxPlayers + "√÷¥Î";

    }

   
}
