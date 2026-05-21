using System.Collections.Generic;
using Manager.SelectElement;
using UnityEngine;
public class FinallyCalcuratedBattleData : MonoBehaviour
{
    public bool isPlayer;
    public int HP;
    public int MaxHP;

    public OrbClass OrbPieces;
    public int AttackPoint;
    public int RedPoint;
    public int GreenPoint;
    public int BluePoint;
    public int DefencePoint;
    public int Exp;
    public int MaxExp;
    public int Level = 1;
    public bool isDeath;
    public List<SkillData> Skills;
    public int money;
    public List<ContinuationSkillTrigger> SkillTriggers;
    public  ElementKinds _element;

    public int TotalAttackPoint(FinallyCalcuratedBattleData target)
    {
        int ReturnInt = AttackPoint + Random.Range(-1, 2) -target.DefencePoint;

        return ReturnInt < 1? 1 : ReturnInt;
    }
    public void SkillTriggerCount()
    {
        foreach (var tri in SkillTriggers)
        {
            tri.OnAction(ForBattleData.instance);
        }
    }

    public float GetDamagePercentage()
    {
        return ((float)HP / MaxHP * 100);
    }

    public void Damage(int d)
    {
        HP -= d;
        if (HP < 0)
        {
            if(gameObject.tag == "Boss") SceneManagerScript.instance.StartBossKill(this);
            else SceneManagerScript.instance.ShowExpOrb(Exp, transform);
            EnemyScript EnemyS = GetComponent<EnemyScript>();
            EnemyS.BeginDeadAction();
        }
    }
    
    
}
