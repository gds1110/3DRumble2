using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        Screen.SetResolution(1080, 1920, false);
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

    }

    private void Update()
    {
    }

    //public override void OnConnectedToMaster() => PhotonNetwork.JoinOrCreateRoom("Room", new Photon.Realtime.RoomOptions { MaxPlayers = 3 }, null);

    public override void OnJoinedRoom()
    {
        GameObject PI = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","PhotonPlayer"),Vector3.zero,Quaternion.identity);
    }

}
