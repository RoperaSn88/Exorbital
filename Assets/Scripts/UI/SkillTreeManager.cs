using System.Collections.Generic;
using Unity.VisualScripting;//
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance { get; set; }
    [SerializeField] ForBattleData CalculateData;
    [SerializeField] List<SkillNode> EnableSkillNodes;
    [SerializeField] List<GainSkillNode> EnableGainSkillNodes;
    [SerializeField] List<ContinutionSkillNode> EnableContinutionNodes;
    
    public SkillNode NowRedNode;
    public SkillNode NowGreenNode;
    public SkillNode NowBlueNode;
    public int[] NodeCounts = new int[3]{0,0,0};

    List<GainSkillNode> GainSkillNodes;
    List<ContinutionSkillNode> ContinutionSkillNodes;
    [SerializeField]private AudioClip audio;

    void Awake()
    {
        Instance = this;
    }
    public void AddNode(SkillNode node,bool playSE = true)
    {
        if(playSE)AudioManager.instance.PlaySE(audio);
        if (node.typeKind == SkillNode.typeKinds.Normal)
        {
            EnableSkillNodes.Add(node);
        }
        else if (node.typeKind == SkillNode.typeKinds.Skill)
        {
            EnableGainSkillNodes.Add(node as GainSkillNode);
        }
        else if (node.typeKind == SkillNode.typeKinds.Continution)
        {
            EnableContinutionNodes.Add(node as ContinutionSkillNode);
        }
    }

    public void ReverseBaseData(ForBattleData baseData, FinallyCalcuratedBattleData targetData)
    {
        if (targetData.MaxHP != 0)
        {
            float degree = (float)(targetData.MaxHP - targetData.HP) / targetData.MaxHP;
            baseData.HP = Mathf.FloorToInt((1 - degree) * baseData.MaxHP);
        }
    }

    public void ReflexBaseData(ForBattleData baseData, FinallyCalcuratedBattleData targetData)
    {
        if (targetData.MaxHP != 0)
        {
            float degree = (float)(targetData.MaxHP - targetData.HP) / targetData.MaxHP;
            baseData.HP = Mathf.FloorToInt((1 - degree) * baseData.MaxHP);
        }
    }

    public void CalculateAllDatas(ForBattleData baseData, FinallyCalcuratedBattleData targetData)
    {
        //targetData8のHPhの割合をbaseDataに反映させる。
        int addSpeed = 0;

        int[] Values = new int[10]{
            baseData.HP,baseData.MaxHP,baseData.AttackPoint,baseData.RedPoint,baseData.GreenPoint,baseData.BluePoint,baseData.DefencePoint,addSpeed,0,0
        };
        //まずは数値は計算
        if (baseData.isPlayer && !SceneManagerScript.instance.isSurvive)
        {

            foreach (var node in EnableSkillNodes)
            {
                //HPのみHP.MaxHPの強化。それ以外は普通に強化。
                if (node.Kind == SkillNode.Kinds.HP && node.valueKind == SkillNode.valueKinds.value)
                {
                    Values[0] += ((int)node.valueKind == 0) ? (int)node.value : 0;
                    Values[1] += ((int)node.valueKind == 0) ? (int)node.value : 0;
                }
                else Values[(int)node.Kind] += ((int)node.valueKind == 0) ? (int)node.value : 0;

            }
            PlayerController.instance.AddSpeed(Values[(int)SkillNode.Kinds.Speed + 1]);

            foreach (var node in EnableSkillNodes)
            {
                if (node.Kind == SkillNode.Kinds.HP && node.valueKind == SkillNode.valueKinds.percent)
                {
                    Values[0] = ((int)node.valueKind == 1) ? (int)(Values[0] * (1 + node.value)) : Values[0];
                    Values[1] = ((int)node.valueKind == 1) ? (int)(Values[1] * (1 + node.value)) : Values[1];
                }
                else Values[(int)node.Kind] = ((int)node.valueKind == 1) ? (int)(Values[(int)node.Kind] * (1 + node.value)) : Values[(int)node.Kind];
            }
        }
        targetData.SkillTriggers = baseData.SkillTriggers;
        foreach (var nd in EnableContinutionNodes)
        {
            bool check = false;
            //重複しているか確認
            foreach (var trig in targetData.SkillTriggers)
            {
                if (nd.targetTrigger.ID == trig.ID) check = true;
            }
            if (!check) targetData.SkillTriggers.Add(nd.targetTrigger);
        }


        targetData.Skills = baseData.Skills;

        foreach (var sk in EnableGainSkillNodes)
        {
            bool check = false;
            foreach (var s in targetData.Skills)
            {
                if (s.ID == sk.skill.ID)
                {
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                targetData.Skills.Add(sk.skill);
            }
        }


        targetData.HP = Values[0];
        targetData.MaxHP = Values[1];
        targetData.AttackPoint = Values[2];
        targetData.RedPoint = Values[3];
        targetData.GreenPoint = Values[4];
        targetData.BluePoint = Values[5];
        targetData.DefencePoint = Values[6];


        targetData.Exp = baseData.Exp;
        targetData.MaxExp = baseData.MaxExp;
        targetData._element = baseData.Element;
    }
    
    public void ResetCalcurateData(ForBattleData baseData, FinallyCalcuratedBattleData targetData)
    {
        int[] Values = new int[10]{
            baseData.HP,baseData.MaxHP,baseData.AttackPoint,baseData.RedPoint,baseData.GreenPoint,baseData.BluePoint,baseData.DefencePoint,0,0,0
        };
        //まずは数値は計算
        if (baseData.isPlayer && !SceneManagerScript.instance.isSurvive)
        {

            foreach (var node in EnableSkillNodes)
            {
                if (node.Kind == SkillNode.Kinds.HP && node.valueKind == SkillNode.valueKinds.value)
                {
                    Values[0] += ((int)node.valueKind == 0) ? (int)node.value : 0;
                    Values[1] += ((int)node.valueKind == 0) ? (int)node.value : 0;
                }
                else Values[(int)node.Kind] += ((int)node.valueKind == 0) ? (int)node.value : 0;
            }

            foreach (var node in EnableSkillNodes)
            {
                if (node.Kind == SkillNode.Kinds.HP && node.valueKind == SkillNode.valueKinds.percent)
                {
                    Values[0] = ((int)node.valueKind == 1) ? (int)(Values[0] * (1 + node.value)) : Values[0];
                    Values[1] = ((int)node.valueKind == 1) ? (int)(Values[1] * (1 + node.value)) : Values[1];

                }
                else Values[(int)node.Kind] *= ((int)node.valueKind == 1) ? (int)(1 + node.value) : 1;
            }
        }
        targetData.SkillTriggers = baseData.SkillTriggers;
        foreach (var nd in EnableContinutionNodes)
        {
            bool check = false;
            foreach (var trig in targetData.SkillTriggers)
            {
                if (nd.targetTrigger.ID == trig.ID) check = true;
            }
            if (!check) targetData.SkillTriggers.Add(nd.targetTrigger);
        }


        targetData.Skills = baseData.Skills;

        foreach (var sk in EnableGainSkillNodes)
        {
            bool check = false;
            foreach (var s in targetData.Skills)
            {
                if (s.ID == sk.skill.ID)
                {
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                targetData.Skills.Add(sk.skill);
            }
        }


        targetData.HP = Values[0];
        targetData.MaxHP = Values[1];
        targetData.AttackPoint = Values[2];
        targetData.RedPoint = Values[3];
        targetData.GreenPoint = Values[4];
        targetData.BluePoint = Values[5];
        targetData.DefencePoint = Values[6];


        targetData.Exp = baseData.Exp;
        targetData.MaxExp = baseData.MaxExp;
    }

    public int CalculateAnyData(ForBattleData baseData, SkillNode.Kinds kind)
    {
        int value = 0;
        switch (kind)
        {
            case SkillNode.Kinds.HP:
                value = baseData.HP;
                break;
            case SkillNode.Kinds.MaxHP:
                value = baseData.MaxHP;
                break;
            case SkillNode.Kinds.Attack:
                value = baseData.AttackPoint;
                break;
            case SkillNode.Kinds.RedPoint:
                value = baseData.RedPoint;
                break;
            case SkillNode.Kinds.GreenPoint:
                value = baseData.GreenPoint;
                break;
            case SkillNode.Kinds.BluePoint:
                value = baseData.BluePoint;
                break;
            case SkillNode.Kinds.Defence:
                value = baseData.DefencePoint;
                break;
        }
        List<SkillNode> enableNode = EnableSkillNodes.FindAll(x => x.Kind == kind && x.valueKind == SkillNode.valueKinds.value);

        foreach (var v in enableNode)
        {
            value += (int)v.value;
        }

        enableNode = EnableSkillNodes.FindAll(x => x.Kind == kind && x.valueKind == SkillNode.valueKinds.percent);

        foreach (var v in enableNode)
        {
            value = (int)(value * (1 + v.value));
        }

        return value;
    }
}

