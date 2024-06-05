using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleAttack : Attack
{

    public void Start()
    {
        if (_bullet == null)
        {
            string projectileName = "MeeleBullet";
            _bullet = Managers.Resource.Instantiate("Projectile/" + projectileName,this.gameObject.transform);
        }
        if (_hitEffect == null)
        {
            string hitEffectName = _owner._unit._name + "HE";
            _hitEffect = Managers.Resource.Load<GameObject>("Prefabs/HitEffect/" + hitEffectName);
        }
        if(_hitEffect==null)
        {
         
            _hitEffect = Managers.Resource.Load<GameObject>("Prefabs/HitEffect/DefaultHE");
          
        }
        
    }

    public override void DoAttack(GameObject target)
    {
        MeeleBullet projectile = _bullet.GetComponent<MeeleBullet>();
        if(projectile)
        {
            projectile.SetProjectileInfo(target.transform, _damage, _owner, this);
        }


    }
 

}
