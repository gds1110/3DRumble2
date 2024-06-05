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
    //����
    private readonly string version = "1.0f";

    // ����� ���̵� �Է�
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
            // ���� ���� �����鿡�� �ڵ����� ���� �ε�
            PhotonNetwork.AutomaticallySyncScene = true;
            // ���� ������ �������� ���� ���
            PhotonNetwork.GameVersion = version;
            // �������̵� �Ҵ�
            PhotonNetwork.NickName = _userId;
            // ���� ������ ��� Ƚ�� ���� . �ʴ� 30ȸ
            Debug.Log(PhotonNetwork.SendRate);
            // ���� ����
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

    //���� ������ ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master~!");
        Debug.Log($"PhotonNetwork.InLobby={PhotonNetwork.InLobby}");
       // PhotonNetwork.JoinLobby();

    }
    // �κ� ���� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby={PhotonNetwork.InLobby}");
        myList.Clear();
        //PhotonNetwork.JoinRandomRoom(); // ���� ��ġ����ŷ ���
    }

    // ������ �� ������ �������� ��� ȣ��Ǵ� �ݹ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        //���� �Ӽ� ����
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2; // �ִ� ������ �� : 20��
        roomOptions.IsOpen = true;   // �� ���� ����
        roomOptions.IsVisible = true; // �κ񿡼� �� ��Ͽ� ���� ��ų�� ����

        //�� ����
        PhotonNetwork.CreateRoom($"{PhotonNetwork.NickName} Room", roomOptions);
    }

    // �� ������ �Ϸ�� �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name :{PhotonNetwork.CurrentRoom.Name}");
       

    }

    // �뿡 ������ �� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetWork.InRoom = { PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        // �̺�Ʈ ����
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
        //PhotonNetwork.PlayerList[0].SetCustomProperties(new Hashtable() { { "Ű1", "���ڿ�" }, { "Ű", 1 } });
        //Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["Ű1"]);
        Hashtable playerCp = PhotonNetwork.LocalPlayer.CustomProperties;

        //PhotonNetwork.PlayerList[0].SetCustomProperties(new Hashtable() { { "Ű1", "���ڿ�" }, { "Ű", 1 } });
        //Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["Ű1"]);

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
