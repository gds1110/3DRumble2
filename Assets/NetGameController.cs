using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetGameController : MonoBehaviour
{
    public Transform PlayerPoint;
    public Transform EnemyPoint;
    public GameObject PlayerCamera;
    public GameObject EnemyCamera;

    public GameObject MainUIPrefab;
    public GameObject PlayerTower;
    public GameObject EnemyTower;

    private void Start()
    {

        Hashtable playerProperty = PhotonNetwork.LocalPlayer.CustomProperties;

        if(playerProperty.ContainsKey("Faction"))
        {
            Debug.Log($"this is MasterClient in Faction number : {playerProperty["Faction"]}");

            int faction = (int)playerProperty["Faction"];
            if(faction == 0 )
            {
                //player
                EnemyCamera.SetActive(false);
                PlayerCamera.SetActive(true);
                if (PhotonNetwork.IsConnectedAndReady)
                    PhotonNetwork.Instantiate("PlayerStatue", PlayerPoint.position, PlayerPoint.rotation);

                UI_MainCardManager CM = GameObject.Instantiate(MainUIPrefab).GetComponent<UI_MainCardManager>();
                CM.NetInit(Define.WorldObject.Player);
            }
            else
            {
                //enemy

                PlayerCamera.SetActive(false);
                EnemyCamera.SetActive(true);
                if (PhotonNetwork.IsConnectedAndReady)
                    PhotonNetwork.Instantiate("EnemyStatue", EnemyPoint.position, EnemyPoint.rotation);
                UI_MainCardManager CM = GameObject.Instantiate(MainUIPrefab).GetComponent<UI_MainCardManager>();
                CM.NetInit(Define.WorldObject.Monster);
            }
        }

    }
    public GameObject SpawnMonster(Define.WorldObject type, GameObject original, Vector3 Pos, Transform parent = null)
    {
        string spawnPath = "Prefabs/Unit/";
        if (!PhotonNetwork.IsMasterClient)
        {
            Managers.Instance.PV.RPC("SpawnRPC", RpcTarget.MasterClient, type, original, Pos, Quaternion.identity, parent);
        }
        else
        {
            GameObject go = PhotonNetwork.Instantiate(spawnPath + original.name, Pos, Quaternion.identity);
            SetUnitInfo(type, go);

            go.GetComponent<PhotonView>().RPC("SetUnitInfo", RpcTarget.AllBuffered, type, go);
        }

        return null;
    }
    [PunRPC]
    void SetUnitInfo(Define.WorldObject type, GameObject go)
    {
        go.GetComponent<Unit>()._owner = type;
        go.GetComponent<UnitController>()._owner = type;
        switch (type)
        {
            case Define.WorldObject.Unknown:
                break;
            case Define.WorldObject.Player:
                go.tag = "Player";
                go.GetComponent<UnitController>()._destPos =Managers.Game.GetNearWaypoint(go.GetComponent<Controller>());
                Managers.Game.SpawnCardEvent?.Invoke();
                Managers.Game.OffPlaceEvent?.Invoke();
                break;
            case Define.WorldObject.Monster:
                go.tag = "Enemy";
                go.GetComponent<UnitController>()._destPos = Managers.Game.GetNearWaypoint(go.GetComponent<Controller>());
                Managers.Game.SpawnCardEvent?.Invoke();
                Managers.Game.OffPlaceEvent?.Invoke();
                break;
            case Define.WorldObject.None:
                break;
        }
        go.GetComponent<UnitController>()._isPlaced = true;
        go.transform.LookAt(Managers.Game.GetNearWaypoint(go.GetComponent<Controller>()));
        go.GetComponent<UnitController>().OnPlace();
        Managers.Game.allUnits.Add(go.GetComponent<UnitController>());
    }
    [PunRPC]
    public GameObject SpawnRPC(Define.WorldObject type, GameObject original, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        string spawnPath = "Prefabs/Unit/";

        GameObject go = PhotonNetwork.Instantiate(spawnPath + original.name, pos, rot);
        SetUnitInfo(type, go);

        go.GetComponent<PhotonView>().RPC("SetUnitInfo", RpcTarget.AllBuffered, type, go);
        return go;
    }
}
