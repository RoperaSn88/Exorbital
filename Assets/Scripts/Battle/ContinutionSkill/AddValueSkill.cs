using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName ="Create Continution Skills/Value Skill")]
public class AddValueSkill : ContinuationSkillTrigger
{

    public Kinds kind;
    public int value;
    public int NeedCount;
    public override void OnAction(ForBattleData userData)
    {
        count++;
        if (count >= NeedCount)
        {
            switch (kind)
            {
                case Kinds.HP:
                    userData.healing(value);
                    break;
                case Kinds.AttackPoint:
                    userData.AttackPoint += value;
                    break;
                case Kinds.DefencePoint:
                    userData.DefencePoint += value;
                    break;
                case Kinds.RedPoint:
                    userData.RedPoint += value;
                    break;
                case Kinds.GreenPoint:
                    userData.GreenPoint += value;
                    break;
                case Kinds.BluePoint:
                    userData.BluePoint += value;
                    break;
            }

            Debug.Log("ACTIVEEEEEEEEEEEEEEE");
        }

        SkillTreeManager.Instance.CalculateAllDatas(userData,userData.GetCalcurateDate());
    }
}
