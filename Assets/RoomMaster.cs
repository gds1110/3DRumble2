using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit.Forms;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomMaster : MonoBehaviourPunCallbacks
{

    public GameObject inRoomPanel;

    public Dictionary<Player, GameObject> playerUI = new Dictionary<Player, GameObject>();
    public GameObject playerUIOrigin;
    public List<Player> onRoomUis = new List<Player>();
    public Button GamePlayButton;
    public TextMeshProUGUI PlayButtonText;
    public override void OnJoinedRoom()
    {

        // GameObject p = CreatePlayerUI(PhotonNetwork.LocalPlayer);
        //// p.transform.SetParent(inRoomPanel.transform, false);
        // onRoomUis.Add(PhotonNetwork.LocalPlayer);
        UpdateButtonState();

        

    }
    public void AfterOnJoined()
    {

        for (int i = 0; i < onRoomUis.Count; i++)
        {
            if (playerUI.ContainsKey(onRoomUis[i]))
                continue;
            GameObject p = CreatePlayerUI(onRoomUis[i]);
            p.transform.SetParent(inRoomPanel.transform, false);
            playerUI.Add(onRoomUis[i], p);
        }
    }

    private void Awake()
    {
        
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    public override void OnLeftRoom()
    {
        //if(onRoomUis.Contains(PhotonNetwork.LocalPlayer))
        //{
        //    onRoomUis.Remove(PhotonNetwork.LocalPlayer);    
        //}
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // AfterOnJoined();
        //if (PhotonNetwork.LocalPlayer == newPlayer) return;

        //GameObject p = CreatePlayerUI(newPlayer);

        //onRoomUis.Add(newPlayer);
        UpdateButtonState();
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

    void UpdateButtonState()
    {
        if (PlayButtonText == null || GamePlayButton == null)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
                GamePlayButton.interactable = true;
                PlayButtonText.text = "PlayGame";
            }
            else
            {
                GamePlayButton.interactable = false;
                PlayButtonText.text = "Wait";
            }
        }
        else
        {
            GamePlayButton.interactable = false;
            PlayButtonText.text = "Wait";
        }
    }

    public void GameStart()
    {
        PhotonNetwork.LoadLevel("GameNetWork 1");
    }
}
