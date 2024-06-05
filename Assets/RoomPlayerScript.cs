using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayerScript : MonoBehaviourPun, IOnEventCallback,IPunObservable
{
    PhotonView PV;
     public TextMeshProUGUI PlayerState;
     public TextMeshProUGUI PlayerName;
    public bool IsReady = false;
    public Button readBtn;
    public Image bg;
    public string BtnString;
    private void OnReadyToggleChanged(bool isOn)
    {
        Debug.Log("ToggleEvent");

        if (photonView.IsMine)
        {
            Debug.Log("IF Mine Do Toggle");

            ToggleState();
        }
    }

    public void ToggleState()
    {
        IsReady = !IsReady;

        if (IsReady)
        {

            BtnString = "Ready";
            PlayerState.text = BtnString;
            PlayerState.color = Color.red;
            this.photonView.RPC("SyncReadyTextColor", RpcTarget.All, "Ready");

        }
        else
        {
            BtnString = "NotReady";
            PlayerState.text = BtnString;
            PlayerState.color = Color.black;
            this.photonView.RPC("SyncReadyTextColor", RpcTarget.All, "NotReady");



        }
        // if (this.photonView.IsMine)
    }

    [PunRPC]
    public void SyncReadyTextColor(string intext)
    {
        BtnString = intext;
        PlayerState.text = BtnString;

        Debug.Log("SyncEvent");


    }



    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code==0)
        {
            object[] data = (object[])photonEvent.CustomData;
            for(int i=0;i<data.Length; i++) print(data[i]); 
        }
    }
    public void Init(string name)
    {
        PlayerName.text = name;
    }

    private void Start()
    {
        //transform.SetParent(GameObject.Find("RoomPanel").transform, false);
       // PlayerName.text = this.photonView.Owner.NickName;
        //PV = photonView;
        //if (IsReady)
        //{
        //    PlayerState.text = "Ready";
        //    PlayerState.color = Color.red;
        //}
        //else
        //{
        //    PlayerState.text = "NotReady";
        //    PlayerState.color = Color.black;


        //}
        //if (PV.IsMine)
        //{
        //    PV.RPC("TestRPC", RpcTarget.All, "A", "B");
        //     readyTogle.onValueChanged.AddListener(OnReadyToggleChanged);
        //}
    }

    [PunRPC]
    void TestRPC(string value1, string value2)
    {
        print("RPC ½ÇÇà");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
     
        if(stream.IsWriting)
        {
            stream.SendNext(BtnString);
        }
        else
        {
            BtnString = (string)stream.ReceiveNext();
        }
    }
  
}
