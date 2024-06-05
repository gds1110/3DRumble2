using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviourPunCallbacks
{
    GameObject _player;
   // Dictionary<int, GameObject> _monsters = new Dictionary<int, GameObject>();
   public HashSet<GameObject> _monsters= new HashSet<GameObject>();

    public Define.WorldObject _playerType = Define.WorldObject.Player;
    public Action<int> OnSpawnEvent;
    public Action GameOverTimeEvent;
    public int _score=0;
    public bool _isBattle = false;
    public float _playTime = 0;

    //Place On Off
    public Action OnPlaceEvent;
    public Action OffPlaceEvent;
    
    // _ghost setactive false , cardunselected , costminus
    public Action SpawnCardEvent;

    public GameObject playerAlter;
    public GameObject EnemyAlter;

    public HashSet<WayPoint> wayPoints = new HashSet<WayPoint>();
    public HashSet<Controller> allUnits = new HashSet<Controller>();
    public HashSet<PlaceableZone> allPlaceZone = new HashSet<PlaceableZone>();


    public bool GameWin = false;

    // 0: VS Cpu , 1 : Hot Seat
    private int _gameMode = 0;
    public int GameMode =>_gameMode;

    public void SetGameMode(int gameMode)
    {
        if (gameMode != 0 && gameMode != 1) gameMode = 0;
        _gameMode = gameMode;
    }


    public void SetGameEnding(bool isWin)
    {
        GameWin = isWin;
        Managers.Scene.LoadScene(Define.Scene.EndingScene);
    }

    public void Init()
    {
       
    }

    public GameObject Spawn(Define.WorldObject type,string path,Transform parent = null)
    {
        GameObject go =  Managers.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Monster:
                _monsters.Add(go);
                if(OnSpawnEvent!=null)
                {
                    OnSpawnEvent.Invoke(1);
                }
                break;
            case Define.WorldObject.Player:
                _player = go;
                break;


        }

        return go;
    }

    public GameObject SpawnEnemy(Transform parent = null,string name = null)
    {
       
        GameObject go = Managers.Resource.Instantiate($"Enemys/{name}");

        go.transform.SetParent(parent);
        _monsters.Add(go);
        if (OnSpawnEvent != null)
            OnSpawnEvent.Invoke(1);

        return go;

    }

    [PunRPC]
    public GameObject SpawnRPC(Define.WorldObject type, GameObject original, Vector3 pos,Quaternion rot, Transform parent = null)
    {
        string spawnPath = "Prefabs/Unit/";

        GameObject go = PhotonNetwork.Instantiate(spawnPath + original.name, pos, rot);
        SetUnitInfo(type, go);

       go.GetComponent<PhotonView>().RPC("SetUnitInfo", RpcTarget.AllBuffered, type, go);
        return go;
    }

    public GameObject CardSpawn(Define.WorldObject type,GameObject original,Transform transform,Transform parent = null)
    {
        //{
        //    //GameObject go = GameObject.Instantiate(original, transform.position,transform.rotation,parent);
        //    string spawnPath = "Prefabs/Unit/";
        //    if (!PhotonNetwork.IsMasterClient)
        //    {
        //      Managers.Instance.PV.RPC("SpawnRPC", RpcTarget.MasterClient, type, original, transform.position,transform.rotation, parent);
        //    }
        //    else
        //    {
        //        GameObject go = PhotonNetwork.Instantiate(spawnPath + original.name, transform.position, transform.rotation);
        //        SetUnitInfo(type, go);

        //        go.GetComponent<PhotonView>().RPC("SetUnitInfo", RpcTarget.AllBuffered, type, go);
        //    }
        string spawnPath = "Prefabs/Unit/";
        GameObject go = PhotonNetwork.Instantiate(spawnPath + original.name, transform.position, transform.rotation);
        go.GetComponent<UnitController>().SpawnEvent(type);
        return null;
    }
    public GameObject CardSpawn(Define.WorldObject type,GameObject original,Vector3 Pos, Transform parent = null)
    {
        //GameObject go = GameObject.Instantiate(original,Pos,Quaternion.identity,parent);
        //string spawnPath = "Prefabs/Unit/";
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    Managers.Instance.PV.RPC("SpawnRPC", RpcTarget.MasterClient, type, original, Pos,Quaternion.identity, parent);
        //}
        //else
        //{
        //    GameObject go = PhotonNetwork.Instantiate(spawnPath + original.name, Pos, Quaternion.identity);
        //    SetUnitInfo(type, go);

        //    go.GetComponent<PhotonView>().RPC("SetUnitInfo", RpcTarget.AllBuffered, type, go);
        //}

        //return null;
        //NetGameController netGame = GameObject.Find("GameMaster").GetComponent<NetGameController>();
        //GameObject go = netGame.SpawnMonster(type, original, Pos, parent);
        string spawnPath = "Prefabs/Unit/";
        GameObject go = PhotonNetwork.Instantiate(spawnPath + original.name, Pos, Quaternion.identity);
        go.GetComponent<UnitController>().SpawnEvent(type);
        return go;

    }
    [PunRPC]
    void SetUnitInfo(Define.WorldObject type,GameObject go)
    {
        go.GetComponent<Unit>()._owner = type;
        go.GetComponent<UnitController>()._owner = type;
        switch (type)
        {
            case Define.WorldObject.Unknown:
                break;
            case Define.WorldObject.Player:
                go.tag = "Player";
                go.GetComponent<UnitController>()._destPos = GetNearWaypoint(go.GetComponent<Controller>());
                SpawnCardEvent?.Invoke();
                OffPlaceEvent?.Invoke();
                break;
            case Define.WorldObject.Monster:
                go.tag = "Enemy";
                go.GetComponent<UnitController>()._destPos = GetNearWaypoint(go.GetComponent<Controller>());
                SpawnCardEvent?.Invoke();
                OffPlaceEvent?.Invoke();
                break;
            case Define.WorldObject.None:
                break;
        }
        go.GetComponent<UnitController>()._isPlaced = true;
        go.transform.LookAt(GetNearWaypoint(go.GetComponent<Controller>()));
        go.GetComponent<UnitController>().OnPlace();
        allUnits.Add(go.GetComponent<UnitController>());
    }

    public GameObject GetPlayer()
    {
        return _player;
    }
    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        return Define.WorldObject.Unknown;
    }
    public void Despawn(GameObject go)
    {

        if(go.GetComponent<Controller>() != null)
        {
            if(allUnits.Contains(go.GetComponent<Controller>()))
            {
                allUnits.Remove(go.GetComponent<Controller>());
            }
        }
        if(go.GetComponent<WayPoint>()!=null)
        {
            if(wayPoints.Contains(go.GetComponent<WayPoint>())) { 
                 wayPoints.Remove(go.GetComponent<WayPoint>());
            }
        }
        Managers.Resource.Destroy(go);
    }

    public void AddScore(int score)
    {
        _score += score;
    }

    public void GameOver()
    {
        _isBattle = false;
        GameOverTimeEvent?.Invoke();
        float maxPlayTime = PlayerPrefs.GetFloat("PlayTime",0);
        if(_playTime > maxPlayTime)
        {
            PlayerPrefs.SetFloat("PlayTime", _playTime);
        }
        int maxScore = PlayerPrefs.GetInt("MaxScore",0);
        if (_score > maxScore)
        {
            PlayerPrefs.SetInt("MaxScore", _score);
        }

        Managers.Scene.LoadScene(Define.Scene.EndingScene);
    }

    public Transform GetNearWaypoint(Controller go)
    {
        Transform closeTransform = null;
        float closDistance = 9999999999999f;
        foreach (WayPoint obj in wayPoints)
        {
            if (obj._owner == go._owner) continue;

            float distance = Vector3.Distance(go.transform.position, obj.transform.position);
            if (distance < closDistance)
            {
                closDistance = distance;
                closeTransform = obj.transform;
            }
        }

        return closeTransform;
    }
    public TowerController MustGetTower(Controller go)
    {
        TowerController to = null;
        float closDistance = 9999999999999f;
        foreach (Controller co in allUnits)
        {
            if (!co.GetComponent<TowerController>()) { continue; }
            if (!co.GetComponent<Tower>()._isUnbreakable) { continue; }
            if (co.GetComponent<TowerController>()._owner == go._owner) { continue; }
            float distance = Vector3.Distance(go.transform.position, co.transform.position);
            if (distance < closDistance)
            {
                closDistance = distance;
                to = co.GetComponent<TowerController>();
            }

        }


        return to;

    }
}
