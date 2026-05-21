using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class EnemyScript : MonoBehaviour
{
    public bool CanFollow=true;
    [SerializeField] PlayerSearcherScript Searcher;
    public SpriteRenderer texture;
    public int Size;
    public float Speed;
    public float AttackDistance;
    public bool Following = false;
    public NavMeshAgent Nav;
    public GameObject Colliders;
    public Animator EnemyAnim;
    public bool Attacking;
    public bool isRight;
    public OrbClass DropOrbs;
    public OrbEffectScript OrbEffects;
    bool isSurvive;
    public float FollowingDistance = 20f;
    [Header("percentが合計100になるように")]
    public WazaPattern[] Wazas;
    public WazaPattern SelectWaza;
    int wazaNum;

    //NavMeshAgent�g���Ȃ����玩��Œǔ��ł���V�X�e������邼�I�I�I�I
    //���@�g���܂���
    private void Start()
    {
        if(CanFollow)Following = true;
        
        Nav.speed = Speed;
        if(SceneManagerScript.instance.isSurvive) {
            FollowingDistance=50;
            isSurvive=true;
        }
    }
    public void StartEnemy()
    {
        //Searcher.isHitting = false;
        Following=true;
    }


    public virtual void FixedUpdate()
    {
        if (Following)
        {
            if(Vector3.Distance(PlayerController.instance.transform.position, transform.position) < FollowingDistance){
                Nav.destination = PlayerController.instance.transform.position;
                transform.rotation=PlayerController.instance.transform.rotation;
            }
            if (!SceneManagerScript.instance.EnemyControllF) Nav.speed = 0;
            else Nav.speed = Speed;
            if (CheckCanAttack() && !Attacking)
            {
                Attack();
            }
            if (PlayerController.instance == null) Following = false;
        }

        if (!Attacking)
        {
            Vector3 PlayerToEnemyVec=PlayerController.instance.transform.InverseTransformPoint(transform.position);
            //Debug.Log($"PlayerVec:{PlayerController.instance.transform.position},ToEnemy:{PlayerToEnemyVec}");
            if (PlayerToEnemyVec.x > 0) //�v���C���[���E���Ȃ�
            {
                texture.flipX = true;
                isRight = false;
                Colliders.transform.rotation = Quaternion.Euler(0, 180+transform.rotation.eulerAngles.y, 0);
            }
            else if (PlayerToEnemyVec.x < 0)
            {
                texture.flipX = false;
                isRight = true;
                Colliders.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            }
        }
    }

    public virtual bool CheckCanAttack(){
        int ran = Random.Range(0, 100);

        WazaPattern testWaza = Wazas[Random.Range(0, Wazas.Length)];
        // Debug.Log($"Selected:{testWaza.wazaName}");
        var dis = Vector3.Distance(PlayerController.instance.transform.position, gameObject.transform.position);
        if (testWaza.minDistance < dis && dis < testWaza.maxDistance && ran < testWaza.percent)
        {
            SelectWaza = testWaza;
            return true;
        }
        return false;
    }

    public virtual void Attack(){
        EnemyAnim.SetTrigger($"{SelectWaza.wazaName}T");
    }


    public void BeginDeadAction()
    {
        PlayerController.instance._specialTechCoolTime += 5;
        Following = false;
        Attacking = false;
        EnemyAnim.SetTrigger("DeadT");
        //お金もここで出す
        if(DropOrbs.red!=0){
            int orb=Random.Range(DropOrbs.red-1,DropOrbs.red+2);
            if(orb<0) orb=0;
            OrbEffectScript eff=Instantiate(OrbEffects,transform);
            eff.transform.SetParent(null);
            eff.Effect(1,orb);
        }
        if(DropOrbs.green!=0){
            int orb=Random.Range(DropOrbs.green-3,DropOrbs.green+2);
            if(orb<0) orb=0;
            OrbEffectScript eff=Instantiate(OrbEffects,transform);
            eff.transform.SetParent(null);
            eff.Effect(0,orb);
        }
        if(DropOrbs.blue!=0){
            int orb=Random.Range(DropOrbs.blue-3,DropOrbs.blue+2);
            if(orb<0) orb=0;
            OrbEffectScript eff=Instantiate(OrbEffects,transform);
            eff.transform.SetParent(null);
            eff.Effect(2,orb);
        }
    }
    public virtual void DeadAction()
    {
        PlayerController.instance.RemoveDefeatEnemy(GetComponent<FinallyCalcuratedBattleData>());
        SceneManagerScript.instance.kills++;
        //SceneManagerScript.instance.killEnemyCount++;
        Destroy(gameObject);
    }

    public virtual void StartAttack()
    {
        Attacking = true;
    }

    public virtual void EndAttack()
    {
        Attacking = false;
        var param = EnemyAnim.parameters;
        foreach (var p in param)
        {
            if (p.type == AnimatorControllerParameterType.Trigger)
            {
                EnemyAnim.ResetTrigger(p.name);
            }
        }
    }

    public virtual void StartSpecificAttack()
    {

    }
}

[System.Serializable]
public class WazaPattern{
    public string wazaName;
    public int percent;
    public float minDistance;
    public float maxDistance;
}