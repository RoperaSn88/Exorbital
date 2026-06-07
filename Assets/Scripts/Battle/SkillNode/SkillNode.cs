using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Create Skill Node/Basic")]
public class SkillNode : ScriptableObject
{
    public enum typeKinds
    {
        Normal,Continution,Skill
    }
    public enum colorKinds
    {
        Green, Red, Blue
    }
    public enum Kinds
    {
        HP,
        MaxHP,
        Attack,
        RedPoint,
        GreenPoint,
        BluePoint,
        Defence,
        Speed,
        None,
    }
    public enum valueKinds
    {
        value,
        percent,
    }
    public typeKinds typeKind;
    public colorKinds colorKind;
    public Kinds Kind;
    public valueKinds valueKind;
    public float value;
    public bool isLock = false;
    [Header("Localization")]
    public string LevelTextID;
    [TextArea]
    public string LevelText;
    public string ExplainTextID;
    [TextArea]
    public string ExplainText;
    public string NoNextNodeTextID;
    [TextArea]
    public string NoNextNodeText = "なし";
    public OrbClass needOrbs;
    public SkillNode childNode;
    public virtual void SkillAction(ForBattleData baseData)
    {

    }

    public virtual void GetSkill()
    {
        
    }
}