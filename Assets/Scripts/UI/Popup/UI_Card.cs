using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Card : UI_Popup
{
    enum GameObjects
    {
        NameText,
        AttackTypeImg,
        ElementalTypeImg,
        CharacterImg,
        CostText,
        BackNameText,
        BElementalText,
        BAttackTypeText,
        BMovementTypeText,
        BTargetTypeText,
        BTargetMovemnetTypeText,
        BLifeText,
        BDamageText,
        BAttackSpdText,
        BSpdText,
        BackBG,
        FrontBG
    }
  

    public TMP_Text _name;
    public TMP_Text _cost;

    public Image _attackType;
    public Image _elementalType;
    public Image _characterImg;


    public TMP_Text _bName;
    public TMP_Text _bElemental;
    public TMP_Text _bMovemnet;
    public TMP_Text _bAttackType;
    public TMP_Text _bTargetType;
    public TMP_Text _bTargetMovementType;
    public TMP_Text _bLife;
    public TMP_Text _bDamage;
    public TMP_Text _bAttackSpd;
    public TMP_Text _bSpd;

    [SerializeField]
   public Color _selectColor;
    [SerializeField]
    public Color _unableColor;
    [SerializeField]
    public Color _ableColor;

    [SerializeField]
    List<Image> _targetImages = new List<Image>();
    [SerializeField]
    List<RawImage> _targetRawImages = new List<RawImage>();

    [SerializeField]
    UnitData _unitData;
    [SerializeField]
    CardImages _CardImageSO;

    GameObject _fg, _bg;
    bool _isFront;
    public bool _isFlipping;

    public bool _isSelected = false;
    public bool _isActive = false;
    public Define.CardState _cardSate = CardState.Active;

    public UnitData UnitData
    { 
        get
        {
            return _unitData; 
        }

        set
        { 
         _unitData = value;
         SetCardInfo();
        }
    }
    public void FlipCard()
    {
        if(_isFront)
        {
            _fg.SetActive(false);
            _bg.SetActive(true);
        }
        else
        {
            _fg.SetActive(true);
            _bg.SetActive(false);
        }
        _isFront=!_isFront;
    }

    private void Awake()
    {
        Bind<GameObject>(typeof(GameObjects));

        _name = Get<GameObject>((int)GameObjects.NameText).GetComponent<TMP_Text>();
        _cost = Get<GameObject>((int)GameObjects.CostText).GetComponent<TMP_Text>();
        _attackType = Get<GameObject>((int)GameObjects.AttackTypeImg).GetComponent<Image>();
        _elementalType = Get<GameObject>((int)GameObjects.ElementalTypeImg).GetComponent<Image>();
        _characterImg = Get<GameObject>((int)GameObjects.CharacterImg).GetComponent<Image>();
        _bName = Get<GameObject>((int)GameObjects.BackNameText).GetComponent<TMP_Text>();
        _bElemental = Get<GameObject>((int)GameObjects.BElementalText).GetComponent<TMP_Text>();
        _bMovemnet = Get<GameObject>((int)GameObjects.BMovementTypeText).GetComponent<TMP_Text>();
        _bAttackType = Get<GameObject>((int)GameObjects.BAttackTypeText).GetComponent<TMP_Text>(); ;
        _bTargetType = Get<GameObject>((int)GameObjects.BTargetTypeText).GetComponent<TMP_Text>();
        _bTargetMovementType = Get<GameObject>((int)GameObjects.BTargetMovemnetTypeText).GetComponent<TMP_Text>();
        _bLife = Get<GameObject>((int)GameObjects.BLifeText).GetComponent<TMP_Text>();
        _bDamage = Get<GameObject>((int)GameObjects.BDamageText).GetComponent<TMP_Text>();
        _bAttackSpd = Get<GameObject>((int)GameObjects.BAttackSpdText).GetComponent<TMP_Text>();
        _bSpd = Get<GameObject>((int)GameObjects.BSpdText).GetComponent<TMP_Text>();
        _fg = Get<GameObject>((int)GameObjects.FrontBG);
        _bg = Get<GameObject>((int)GameObjects.BackBG);
        SetCardInfo();
        _isFront = true;
        _isFlipping = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        

    }
    public void SetCardInfo()
    {
        if (_unitData != null)
        {
            _name.text = _unitData._displayName;
            _cost.text = _unitData.cost.ToString();
            _characterImg.sprite = _unitData.UnitPortrait;
            if (_CardImageSO != null)
            {
                string belemental= "";
                switch (_unitData._elementalType)
                {
                    case Define.UnitElementalType.Fire:
                        _elementalType.sprite = _CardImageSO.Fire;
                        belemental = "ȭ��";
                        break;
                    case Define.UnitElementalType.Ice:
                        _elementalType.sprite = _CardImageSO.Ice;
                        belemental = "����";

                        break;
                    case Define.UnitElementalType.Earth:
                        _elementalType.sprite = _CardImageSO.Earth; 
                        belemental = "����";

                        break;
                    case Define.UnitElementalType.Wind:
                        _elementalType.sprite = _CardImageSO.Wind;
                        belemental = "�ٶ�";

                        break;
                    case Define.UnitElementalType.Dark:
                        _elementalType.sprite = _CardImageSO.Dark;
                        belemental = "���";

                        break;
                    case Define.UnitElementalType.Light:
                        _elementalType.sprite = _CardImageSO.Light;
                        belemental = "��";

                        break;
                    case Define.UnitElementalType.Normal:
                        _elementalType.sprite = _CardImageSO.Normal;
                        belemental = "�븻";

                        break;
                }
                
                string battacktype = "";
                switch (_unitData._attackType)
                {
                    case Define.AttackType.Meele:
                        _attackType.sprite = _CardImageSO.Meele;
                        battacktype = "����";
                        break;
                    case Define.AttackType.Arange:
                        _attackType.sprite = _CardImageSO.Arange;
                        battacktype = "���Ÿ�";
                        break;
                    case Define.AttackType.Both:
                        _attackType.sprite = _CardImageSO.Both;
                        battacktype = "��Ƽ";

                        break;
                }
                

                string bmovemnt = "";
                string btargettype = "";
                string btargetmovement = "";
                switch (_unitData._movementType)
                {
                    case Define.MovemnetType.Ground:
                        bmovemnt = "����";
                        break;
                    case Define.MovemnetType.Aerial:
                        bmovemnt = "����";

                        break;
                    case Define.MovemnetType.Both:
                        bmovemnt = "��Ƽ";

                        break;
                }
                switch (_unitData._targetType)
                {
                    case Define.TargetType.Unit:
                        btargettype = "����";
                        break;
                    case Define.TargetType.Building:
                        btargettype = "�ǹ�";

                        break;
                    case Define.TargetType.Both:
                        btargettype = "��Ƽ";

                        break;
                }
                switch (_unitData._targetMovementType)
                {
                    case Define.MovemnetType.Ground:
                        btargetmovement = "����";
                        break;
                    case Define.MovemnetType.Aerial:
                        btargetmovement = "����";

                        break;
                    case Define.MovemnetType.Both:
                        btargetmovement = "��Ƽ";

                        break;
                }

                _attackType.enabled = true;
                _elementalType.enabled = true;

                _bName.text = _unitData._displayName;
                _bElemental.text = "�Ӽ� : "+ belemental;
               
                _bMovemnet.text ="�̵�Ÿ�� : "+bmovemnt;
                _bAttackType.text ="����Ÿ�� : "+ battacktype;
                _bTargetType.text ="Ÿ��Ÿ�� : "+btargettype;
                _bTargetMovementType.text ="Ÿ���̵�Ÿ�� : "+btargetmovement;
                _bLife.text = "ü�� : " + _unitData._life.ToString();
                _bDamage.text = "���ݷ� : " + _unitData._damage.ToString();
                _bAttackSpd.text ="���ݼӵ� : " + _unitData._attackRatio.ToString();
                _bSpd.text ="�̵��ӵ� : " + _unitData._speed.ToString();

            }



        }
        else
        {
            _name.text = "�� ����";
            _bName.text = "�� ����";
            _cost.text = "0";
            _characterImg.sprite = _CardImageSO.DefaultImg;
            _attackType.sprite = null;
            _elementalType.sprite = null;
            _attackType.enabled = false;
            _elementalType.enabled = false;
            _bElemental.text = "�Ӽ� : �� ����";

            _bMovemnet.text = "�̵�Ÿ�� : �� ����";
            _bAttackType.text = "����Ÿ�� : �� ����";
            _bTargetType.text = "Ÿ��Ÿ�� : �� ����";
            _bTargetMovementType.text = "Ÿ���̵�Ÿ�� : �� ����";
            _bLife.text = "ü�� : 0";
            _bDamage.text = "���ݷ� : 0";
            _bAttackSpd.text = "���ݼӵ� : 0";
            _bSpd.text = "�̵��ӵ� : 0";
        }
    }

    public void Refresh(int cost)
    {
        if (_unitData == null) return;
        if (_cardSate == CardState.Selected) return;

        if(_unitData.cost<=cost)
        {
            _cardSate = CardState.Active;
            
        }
        else if(_unitData.cost>cost)
        {
            _cardSate = CardState.UnActive;
        }
        SetColor();
    }
    public void IsSelect()
    {
        _cardSate = CardState.Selected;
        SetColor();
    }
    public void UnSelect()
    {
        _cardSate = CardState.None;
    }
    public void SetColor()
    {
       
        Color color = Color.white;

        switch (_cardSate)
        {
            case CardState.Active:
                color = _ableColor;
                break;
            case CardState.UnActive:
                color = _unableColor;
                break;
            case CardState.Selected:
                color = _selectColor;
                break;
        }
       
        for (int i = 0; i < _targetImages.Count; i++)
        {
           if(color != _targetImages[i].color)
            _targetImages[i].color = color;
        }
        for (int i = 0; i < _targetRawImages.Count; i++)
        {
            if (color != _targetRawImages[i].color)
                _targetRawImages[i].color = color;
        }



    }


}
