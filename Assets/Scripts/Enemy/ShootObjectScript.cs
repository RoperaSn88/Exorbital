using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootObjectScript : AttackPointScript
{
    public float timer = 6;

    Rigidbody rb;
    [SerializeField] GameObject Effect;
    public override void HitAction(FinallyCalcuratedBattleData User, FinallyCalcuratedBattleData Target)
    {
        GameObject o = Instantiate(Effect, transform);
        o.transform.SetParent(null);
        Destroy(gameObject);
    }

    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        Vector3 direction = PlayerController.instance.transform.position - gameObject.transform.position;
        direction=direction.normalized;
        rb.linearVelocity = direction*7;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0) Destroy(gameObject);
    }
}
