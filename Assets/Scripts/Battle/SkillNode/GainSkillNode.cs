using UnityEngine;
[CreateAssetMenu(menuName = "Create Skill Node/Skill")]
public class GainSkillNode : SkillNode
{
    public SkillData skill;

    public override void GetSkill()
    {
        ForBattleData.instance.GainSkill(skill);
    }
}
