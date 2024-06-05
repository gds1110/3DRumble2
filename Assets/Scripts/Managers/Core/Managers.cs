using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance { get { Init(); return s_instance; } }
    #region Contents
    GameManager _game = new GameManager();
    EffectManager _effect = new EffectManager();
    public static GameManager Game { get { return Instance._game; } }
    public static EffectManager Effect { get {  return Instance._effect; } }

    public PhotonView PV;
    #endregion
    #region Core
    DataManager _dataManager = new DataManager();
    InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();
    UIManager _uiManager = new UIManager();
    SceneManagerEx _sceneManager = new SceneManagerEx();
    SoundManager _sound = new SoundManager();   
    PoolManager _pool = new PoolManager();
 
    public static DataManager Data { get { return Instance._dataManager; } }    
    public static InputManager Input { get { return Instance._input; } }
    public static ResourceManager Resource { get { return Instance._resource; } }

    public static SceneManagerEx Scene { get { return Instance._sceneManager; } }

    public static UIManager UI { get { return Instance._uiManager; } }
    public static SoundManager Sound { get { return Instance._sound; } }

    public static PoolManager Pool { get { return Instance._pool; } }

    [SerializeField]
    public HashSet<Controller> allUnits = new HashSet<Controller>();

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
         PV=  gameObject.AddComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        _input.OnUpdate();
       
    }

    static void Init()
    {
        if(s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if(go==null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._pool.Init();
            s_instance._sound.Init();
            s_instance._dataManager.Init();
            s_instance._game.Init();
        }
        
    }

    public static void Clear()
    {
        Sound.Clear();
        Input.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }
}
