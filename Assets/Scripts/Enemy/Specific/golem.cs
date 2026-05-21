using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class golem : EnemyScript
{
    public GameObject targetObject;


    public GameObject[] summonEnemys = new GameObject[3];

    async public UniTask throwObject()
    {
        GameObject target = Instantiate(targetObject, transform.position - new Vector3(0, 3, 0), Quaternion.identity);
        target.TryGetComponent<AttackPointScript>(out var at);
        this.TryGetComponent<FinallyCalcuratedBattleData>(out var val);
        at.UserData = val;
        await target.transform.DOMoveY(transform.position.y + 1.5f, 2 / 3f).SetEase(Ease.OutQuad);
        Vector3 direc = target.transform.position - PlayerController.instance.transform.position;
        direc = new Vector3(direc.x, 0, direc.z);
        target.TryGetComponent<Rigidbody>(out var r);
        r.useGravity = true;
        r.AddForce(-direc.normalized * Vector3.Distance(target.transform.position, PlayerController.instance.transform.position), ForceMode.Impulse);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        Destroy(target.gameObject);
    }
    
    async public UniTask summonEnemy()
    {
        if(GameObject.FindGameObjectsWithTag("Middle").Length < 3)Instantiate(summonEnemys[UnityEngine.Random.Range(0, summonEnemys.Length)],transform.position + Vector3.up,Quaternion.identity);
    }
}
