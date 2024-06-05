using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;
using UnityEngine.UI;


using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    //버전
    private readonly string version = "1.0f";

    // 사용자 아이디 입력
    private string _userId = "Sonny";

    public static PhotonManager instance;
    public TextMeshProUGUI userCount;

    public event Action ListRenewalHandle;
    public event Action JoinRoomHandle;

    public List<RoomInfo> myList = new List<RoomInfo>();
    Dictionary<Player, GameObject> RoomMembers = new Dictionary<Player, GameObject>();
    int currentPage = 1, maxPage, multiple;

    public GameObject playerUIOrigin;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // 같은 룸의 유저들에게 자동은로 씬을 로딩
            PhotonNetwork.AutomaticallySyncScene = true;
            // 같은 버전의 유저끼리 접속 허용
            PhotonNetwork.GameVersion = version;
            // 유저아이디 할당
            PhotonNetwork.NickName = _userId;
            // 포톤 서버와 통신 횟수 설정 . 초당 30회
            Debug.Log(PhotonNetwork.SendRate);
            // 서버 접속
            PhotonNetwork.ConnectUsingSettings();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    private void Update()
    {
        if (userCount)
            userCount.text = PhotonNetwork.CountOfPlayers.ToString();
    }

    //포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master~!");
        Debug.Log($"PhotonNetwork.InLobby={PhotonNetwork.InLobby}");
       // PhotonNetwork.JoinLobby();

    }
    // 로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby={PhotonNetwork.InLobby}");
        myList.Clear();
        //PhotonNetwork.JoinRandomRoom(); // 랜덤 매치메이킹 기능
    }

    // 랜덤한 룸 입장이 실패했을 경우 호출되는 콜백
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        //룸의 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2; // 최대 접속자 수 : 20명
        roomOptions.IsOpen = true;   // 룸 오픈 여부
        roomOptions.IsVisible = true; // 로비에서 룸 목록에 노출 시킬지 여부

        //룸 생성
        PhotonNetwork.CreateRoom($"{PhotonNetwork.NickName} Room", roomOptions);
    }

    // 룸 생성이 완료된 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name :{PhotonNetwork.CurrentRoom.Name}");
       

    }

    // 룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetWork.InRoom = { PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        // 이벤트 실행
        JoinRoomHandle?.Invoke();
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}.{player.Value.ActorNumber}");

            if (!RoomMembers.ContainsKey(player.Value))
                RoomMembers.Add(player.Value, CreatePlayerUI(player.Value));

        }
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Faction", 0 } });

        }
        else
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Faction", -1 } });
        }
        //PhotonNetwork.PlayerList[0].SetCustomProperties(new Hashtable() { { "키1", "문자열" }, { "키", 1 } });
        //Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["키1"]);
        Hashtable playerCp = PhotonNetwork.LocalPlayer.CustomProperties;

        //PhotonNetwork.PlayerList[0].SetCustomProperties(new Hashtable() { { "키1", "문자열" }, { "키", 1 } });
        //Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["키1"]);

        //foreach (var player in PhotonNetwork.CurrentRoom.Players)
        //{

        //}


    }
    private void PlayerJoinedRoom(Player player)
    {
        if(player==null)
        {
            return;
        }
        PlayerLeftRoom(player);

        GameObject playerListingObj = Instantiate(playerUIOrigin);
       

    }
    

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for(int i=0;i<roomCount;i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i])!=-1) myList.RemoveAt(myList.IndexOf(roomList[i])); 
        }
    }

    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    private void MyListRenewal()
    {

        ListRenewalHandle?.Invoke();
    }

    public void CheckInteratable(Button prev,Button next, List<Button> btnList)
    {

        int roomNum = btnList.Count;

        maxPage = (myList.Count % roomNum == 0) ? myList.Count / roomNum : myList.Count / roomNum + 1;


        prev.interactable = (currentPage <= 1) ? false : true;
        next.interactable = (currentPage >= maxPage) ? false : true;

        multiple = (currentPage - 1) * roomNum;


        for (int i=0;i<btnList.Count;i++)
        {
            btnList[i].interactable = (multiple + i < myList.Count) ? true : false;
            btnList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            btnList[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/ "+myList[multiple+i].MaxPlayers :"";
        }

    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        JoinRoomHandle?.Invoke();
        if (newPlayer != PhotonNetwork.LocalPlayer)
        {
            RoomMembers.Add(newPlayer, CreatePlayerUI(newPlayer));
        }

    }
    GameObject CreatePlayerUI(Player inplayer)
    {
        if (!GameObject.Find(inplayer.NickName))
        {
            Debug.Log("CreatePlayerUi");
            GameObject uigo = PhotonNetwork.Instantiate("PlayerUI", Vector3.zero, Quaternion.identity);
            uigo.name = inplayer.NickName;

            if (uigo.GetComponent<RoomPlayerScript>())
            {
                uigo.transform.SetParent(GameObject.Find("RoomPanel").transform, false);
                uigo.GetComponent<RoomPlayerScript>().PlayerName.text = inplayer.NickName;
                uigo.GetComponent<RoomPlayerScript>().ToggleState();
                uigo.GetComponent<RoomPlayerScript>().readBtn.interactable = (inplayer == PhotonNetwork.LocalPlayer) ? true : false;
                uigo.GetComponent<RoomPlayerScript>().Init(inplayer.NickName);

            }

            return uigo;
        }
        else
        {
            return null;
        }
    }
  
}
