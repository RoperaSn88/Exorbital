using UnityEngine;

public class ContinuationSkillTrigger : ScriptableObject
{
    public int ID;
    public int count = 0;
    public enum Kinds
    {
        HP, RedPoint, GreenPoint, BluePoint, AttackPoint, DefencePoint
    }
    public virtual void OnAction(ForBattleData data)
    {

    }

    public virtual void Count()
    {
        count++;
    }
}
