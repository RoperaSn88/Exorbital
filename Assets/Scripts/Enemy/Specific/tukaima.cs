using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tukaima : EnemyScript
{
    public float coolTime=0;
    public GameObject AttackEffect;
    public GameObject ShootObject;
    public override void StartSpecificAttack()
    {
        if(coolTime<=0){
            Instantiate(AttackEffect, transform);
            GameObject o = Instantiate(ShootObject, transform);
            o.transform.SetParent(null);
            o.GetComponent<AttackPointScript>().UserData = gameObject.GetComponent<FinallyCalcuratedBattleData>();
            coolTime=5;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if(coolTime>0){
            coolTime-=Time.deltaTime;
        }
    }
}
