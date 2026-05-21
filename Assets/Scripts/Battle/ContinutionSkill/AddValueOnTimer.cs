using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName ="Create Continution Skills/Value Skill On Timer")]
public class AddValueOnTimer : ContinuationSkillTrigger
{
    public Kinds kind;
    public int value;
    public int KillCount;
    public float MaxTimer;
    bool timerF;
    float timer;
    ForBattleData data;
    SkillTimer timerr;
    public override void OnAction(ForBattleData userData)
    {
        if (!data) data = userData;
        count++;
        if (count == KillCount)
        {
            count = 0;
            if (timerF)
            {
                timerr.timer = MaxTimer;
            }
            else
            {
                GameObject obj = new GameObject();
                timerr = obj.AddComponent<SkillTimer>();
                timerr.TargetSkill = this;
                timerr.StartTimer();
                switch (kind)
                {
                    case Kinds.HP:
                        userData.HP += value;
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
                timer = MaxTimer;
                timerF = true;
            }
        }

    }

    public void FinishSkill()
    {
        timerF = false;
        timer = 0;
        switch (kind)
        {
            case Kinds.HP:
                data.HP -= value;
                break;
            case Kinds.AttackPoint:
                data.AttackPoint -= value;
                break;
            case Kinds.DefencePoint:
                data.DefencePoint -= value;
                break;
            case Kinds.RedPoint:
                data.RedPoint -= value;
                break;
            case Kinds.GreenPoint:
                data.GreenPoint -= value;
                break;
            case Kinds.BluePoint:
                data.BluePoint -= value;
                break;
        }
    }

}
