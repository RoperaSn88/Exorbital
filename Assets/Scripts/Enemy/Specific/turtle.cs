using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class turtle : EnemyScript
{
    public float cooltime;

    public override void FixedUpdate()
    {
        if(cooltime>=0){
            cooltime-=Time.deltaTime;
        }
        if (Following)
        {
            if(Vector3.Distance(PlayerController.instance.transform.position, transform.position) < FollowingDistance){
                Nav.destination = PlayerController.instance.transform.position;
                transform.rotation=PlayerController.instance.transform.rotation;
            }
            if (!SceneManagerScript.instance.EnemyControllF) Nav.speed = 0;
            else Nav.speed = Speed;
            if (Vector3.Distance(PlayerController.instance.transform.position, transform.position) < AttackDistance && SceneManagerScript.instance.EnemyControllF && cooltime<=0)
            {
                EnemyAnim.SetTrigger("AttackT");
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
    public override void StartSpecificAttack()
    {
        Vector3 PlayerToEnemyVec=transform.position - PlayerController.instance.transform.position;
        transform.DOMove(transform.position-2*new Vector3(PlayerToEnemyVec.x,0,PlayerToEnemyVec.z),1.0f).SetEase(Ease.InQuad);
    }

    public override void EndAttack()
    {
        Attacking = false;
        cooltime=1f;
        EnemyAnim.ResetTrigger("AttackT");
    }

}
