using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class DamageInfo
{
    Define.UnitElementalType elementalType;

}

public interface IConquerAble
{
  
    void StartConquer(Controller fromUnit);
    void Conquering(Controller fromUnit);  

    void StopConquer(Controller fromUnit);

    void EndConquer(Controller fromUnit);
}

public class Unit : BaseCombat, IDamageAble
{

    public UnitData unitData;

    public Define.WorldObject _owner;

    public UnitController _controller;

    public TakeDamagedDelegate damagedDelegate
    {
        get { return _damageDelegate; }
    }

    public void SetData()
    {
        _name = unitData._name;
        _attackType = unitData._attackType;
        _targetType = unitData._targetType;
        _attackDelay = unitData._attackRatio;
        _attackRange = unitData._attackRange;
        _attackRange = Mathf.Clamp(_attackRange, 5f, 45f);
        _damage = unitData._damage;
        _speed = unitData._speed;
        _life = unitData._life;
        _defense = unitData._defense;  
        _scanRange = unitData._scanRange;
        _scanRange=Mathf.Clamp(_scanRange, 10f,45f);
        _unitElementalType = unitData._elementalType;
        _movementType = unitData._movementType;
        _targetMovementType = unitData._targetMovementType;
        _unityType = unitData._UnitType;
        _isSplitAttackAnim = unitData._isSplitAttackAnim;
        _attackTiming = unitData._attackTiming;
        _currentLife = unitData._life;
        _controller = GetComponent<UnitController>();
    }

    public void TakeDamage(int damage, Controller fromUnit)
    {
        if(fromUnit == null) { return; }
        if (damage < 0)
        {
            damage = 1;
        }
        if (_controller._target == null&&_controller.State!=Define.State.Attack)
        {
            if (_controller.CheckCanAttackType(fromUnit._unit))
            {
                if (fromUnit.gameObject)
                    _controller._target = fromUnit.gameObject;
            }
        }
        float eleDamage = ElementalCalculate.GetMultiplier(fromUnit._unit._unitElementalType, _unitElementalType);
        
        int trueDamage = Mathf.RoundToInt(damage * eleDamage);
        if (trueDamage < 0)
        {
            trueDamage = 1;
        }

        _currentLife -= trueDamage;
        if (hpBar)
        {
            float ratio = _currentLife / _life;
            hpBar.SetHpRatio(ratio);
        }

        if (_currentLife <= 0)
        {
            if(fromUnit)
            {
                if(fromUnit._target==_controller)
                {
                    fromUnit._target = null;
                }
            }
            DeadAction?.Invoke();
            Invoke("OnDeath",0.5f);
        }
    }

    private void Start()
    {
        if (GetComponent<Collider>())
        {
            hpBar = Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform, "UI_WorldBar");
            hpBar.Init();
            TeamMark mark = hpBar.GetComponent<TeamMark>();
            if(mark != null) { mark.SetMarkColor(_owner); }
        }
        _damageDelegate += TakeDamage;


    }
    private void Awake()
    {
        SetData();
        
    }

   

}
