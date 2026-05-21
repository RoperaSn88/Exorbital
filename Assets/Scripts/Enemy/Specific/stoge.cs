using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stoge :EnemyScript
{
    public GameObject ExplosionEffect;
    public override void StartSpecificAttack()
    {
        GameObject o=Instantiate(ExplosionEffect,transform);
        //o.transform.SetParent(null);
    }

    public override void EndAttack()
    {
        base.EndAttack();
        Destroy(gameObject);
    }
}
