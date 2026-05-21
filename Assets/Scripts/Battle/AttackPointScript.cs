using System;
using System.Collections;
using System.Collections.Generic;
using Manager.SelectElement;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class AttackPointScript : MonoBehaviour
{
    public enum Kinds
    {
        ForPlayer,
        ForEnemy,
    }
    public Kinds Kind;
    public bool DistanceMode; //�����ɂ���Č�������
    public float MaxRadius;
    public FinallyCalcuratedBattleData UserData;
    [SerializeField] int PlusDamage;
    [SerializeField] bool isContinuousAttack;
    SceneManagerScript SC;
    public AudioClip DamageSound;

    public virtual void Start()
    {
        SC = GameObject.FindWithTag("SceneManager").GetComponent<SceneManagerScript>();
    }

    public ElementEffective JudgeElementEffective(ElementKinds user, ElementKinds target)
    {
        switch (user)
        {
            case ElementKinds.Fire:
                if(target == ElementKinds.Water) return ElementEffective.Weakness;
                if(target == ElementKinds.Leaf) return ElementEffective.Effective;
                break;
            case ElementKinds.Water:
                if(target == ElementKinds.Leaf) return ElementEffective.Weakness;
                if(target == ElementKinds.Fire) return ElementEffective.Effective;
                break;
            case ElementKinds.Leaf:
                if(target == ElementKinds.Fire) return ElementEffective.Weakness;
                if(target == ElementKinds.Water) return ElementEffective.Effective;
                break;
        }
        return ElementEffective.Normal;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isContinuousAttack)  //�A���U���̂��̂�������
        {

        }
        else
        {
            //敵に対しての攻撃
            if (Kind == Kinds.ForEnemy)
            {
                if (other.CompareTag("Enemy") || other.CompareTag("Middle") || other.CompareTag("Boss"))
                {
                    FinallyCalcuratedBattleData HittedData = other.GetComponent<FinallyCalcuratedBattleData>();
                    if (!HittedData.isDeath)
                    {
                        int AllPoint;
                        int KariPoint = UserData.TotalAttackPoint(HittedData) + PlusDamage;

                        // ダメージ計算
                        if (PlayerController.instance.Data != null) KariPoint += PlayerController.instance.Data.PlusDamage;
                        if (PlayerController.instance.BuffF)
                        {
                            AllPoint = (int)Mathf.Floor(KariPoint * 1.3f);
                        }
                        else AllPoint = KariPoint;

                        // 属性反映
                        switch (JudgeElementEffective(UserData._element, HittedData._element))
                        {
                            case ElementEffective.Normal:
                                break;
                            case ElementEffective.Effective:
                                AllPoint = (int)(AllPoint * 1.3f);
                                break;
                            case ElementEffective.Weakness:
                                AllPoint = (int)(AllPoint * 0.7f);
                                break;
                        }

                        HittedData.HP -= AllPoint;

                        //ボスバーの反映
                        if (other.CompareTag("Boss"))
                        {
                            SceneManagerScript.instance.SetBossBar(HittedData.MaxHP, HittedData.HP);
                        }

                        //とどめの処理
                        if (other.CompareTag("Middle") || other.CompareTag("Boss"))
                        {
                            if (HittedData.HP < 0  && !PlayerController.instance.GetFinishingEnemys().Contains(HittedData)) HittedData.HP = 1;
                            if (HittedData.GetDamagePercentage() < 10f)
                            {
                                if (!PlayerController.instance._finishingFlug && !PlayerController.instance.GetFinishingEnemys().Contains(HittedData))
                                {
                                    PlayerController.instance.AddDefeatEnemy(HittedData);
                                }
                            }
                        }
                        //死亡処理
                        if (HittedData.HP <= 0)
                        {
                            //Destroy(HittedData.gameObject);
                            HittedData.isDeath = true;
                            if (other.CompareTag("Boss"))
                            {
                                SceneManagerScript.instance.StartBossKill(HittedData);
                                EnemyScript EnemyS = other.GetComponent<EnemyScript>();
                                EnemyS.BeginDeadAction();
                            }
                            else
                            {
                                SceneManagerScript.instance.ShowExpOrb(HittedData.Exp, HittedData.transform);
                                EnemyScript EnemyS = other.GetComponent<EnemyScript>();
                                EnemyS.BeginDeadAction();
                            }

                            UserData.SkillTriggerCount();
                        }

                        HitAction(UserData, HittedData);
                        SC.ShowDamageText(AllPoint, HittedData.transform,JudgeElementEffective(UserData._element, HittedData._element));
                        /*
                        GameObject a= Instantiate(SC.DamageText,HittedData.transform);
                        a.GetComponent<DamageTextScript>().text.text = ($"{AllPoint}");
                        */

                    }
                    



                }
            }

            //プレイヤーに対しての攻撃
            if (Kind == Kinds.ForPlayer&& !SceneManagerScript.instance.isGameOver)
            {
                if (other.CompareTag("Player"))
                {
                    
                    if (!DistanceMode)
                    {
                        
                        FinallyCalcuratedBattleData HittedData = other.GetComponent<FinallyCalcuratedBattleData>();
                        int AllPoint = UserData.TotalAttackPoint(HittedData) + PlusDamage;
                        if (!SceneManagerScript.instance.LevelSelecting)
                        {
                            if (PlayerController.instance.rollingF) return;
                            if (PlayerController.instance.DefendF) AllPoint /= 2;
                            HittedData.HP -= AllPoint;
                            AudioManager.instance.PlaySE(DamageSound);
                            SC.ShowDamageTextForPlayer(AllPoint, HittedData.transform,JudgeElementEffective(UserData._element, HittedData._element));
                            SceneManagerScript.instance.SetSlider(HittedData.HP, HittedData.MaxHP, 1);
                        }
                        HitAction(UserData, HittedData);
                        if (HittedData.HP <= 0)
                        {
                            SceneManagerScript.instance.StartGameOver();
                        }
                    }
                    else
                    {
                        //持続ダメージ、未実装
                        FinallyCalcuratedBattleData HittedData = other.GetComponent<FinallyCalcuratedBattleData>();
                        int AllPoint = UserData.TotalAttackPoint(HittedData) + PlusDamage;
                        AllPoint = Mathf.FloorToInt(AllPoint * ((MaxRadius - Vector3.Distance(transform.position, other.transform.position)) / MaxRadius));

                        if (!SceneManagerScript.instance.LevelSelecting)
                        {
                            HittedData.HP -= AllPoint;

                            SC.ShowDamageTextForPlayer(AllPoint, HittedData.transform,JudgeElementEffective(UserData._element, HittedData._element));
                            SceneManagerScript.instance.SetSlider(HittedData.HP, HittedData.MaxHP, 1);
                        }
                        HitAction(UserData, HittedData);
                        if (HittedData.HP <= 0)
                        {
                            SceneManagerScript.instance.StartGameOver();
                        }
                    }
                    

                }
            }

        }

    }

    public virtual void HitAction(FinallyCalcuratedBattleData User, FinallyCalcuratedBattleData Target)
    {

    }
}
