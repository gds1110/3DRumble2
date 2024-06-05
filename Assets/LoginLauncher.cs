using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginLauncher : MonoBehaviourPunCallbacks
{


    public TMP_InputField playerNickName;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();       
    }

   public void StartTheGame()
    {
        PhotonNetwork.NickName = playerNickName.text;
        //PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        // �κ� ���������� �������� �� �� ��ȯ
        Debug.Log($"{PhotonNetwork.NickName} Joined Lobby");
        Managers.Scene.LoadScene(Define.Scene.Lobby);

    }
}
