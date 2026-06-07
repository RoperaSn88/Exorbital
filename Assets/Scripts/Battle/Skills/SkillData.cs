using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Craete New Skill/Basic")]
public class SkillData : ScriptableObject
{
    //�ǉ����鎞��SceneManagerScript��GenerateSkill���m�F�I�I
    public enum Kinds
    {
        Attack,
        Magic,
    }
    public Kinds Kind;
    [Header("Localization")]
    public string NameTextID;
    public string IntroductTextID;
    public string Name;
    public string Introduct;
    public int NeedMP;
    public OrbClass NeedOrbs;
    public int ID;
    public int PlusDamage;
    public int HealPoint;
    public float Timer;
    public int SkillLevel = 1;
    public int LimitLevel;
    public GameObject EffectObject;
    public Transform SpawnPos;
    public GameObject Light;
    public Vector3 LightPos;

    public int LevelUpPlusDamage = 2;
    public int LevelUpTimer = 4;


    public virtual void SkillLevelUp()
    {
        SkillLevel++;
        if (Kind == Kinds.Attack) PlusDamage += LevelUpPlusDamage;
        else HealPoint += LevelUpPlusDamage;

        Timer += LevelUpTimer;
    }

    public virtual void UseSkill()
    {
        
    }

    
}
