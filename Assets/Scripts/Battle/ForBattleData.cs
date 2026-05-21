using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.SelectElement;

public class ForBattleData : MonoBehaviour
{
    public static ForBattleData instance;
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
    public int Level;
    public bool isDeath;
    public List<SkillData> SyokiSkills;
    public List<SkillData> Skills;
    public int money;
    public List<ContinuationSkillTrigger> SkillTriggers;

    /// <summary>
    /// 今の属性
    /// </summary>
    [SerializeField]
    private ElementKinds _element;

    public ElementKinds Element {get => _element; set => _element = value;}
    FinallyCalcuratedBattleData CalcuratedData;

    private void Awake()
    {
        if (isPlayer)
        {
            instance = this;
        }
    }

    public void SkillTriggerCount()
    {
        foreach (var tri in SkillTriggers)
        {
            tri.Count();
        }
    }

    public void healing(int amount)
    {
        CalcuratedData.HP += amount;
        if(CalcuratedData.HP > CalcuratedData.MaxHP) CalcuratedData.HP = CalcuratedData.MaxHP;
        SkillTreeManager.Instance.ReflexBaseData(this, CalcuratedData);
        ReturnCalcurateData();
    }



    public void ReturnCalcurateData()
    {
        if(CalcuratedData==null)CalcuratedData = gameObject.GetComponent<FinallyCalcuratedBattleData>();
        SkillTreeManager.Instance.CalculateAllDatas(this, CalcuratedData);
        if (isPlayer) SetSlider();
    }

    public void ResetCalcurateData()
    {
        SkillTreeManager.Instance.ResetCalcurateData(this, CalcuratedData);
        SetSlider();
    }

    public FinallyCalcuratedBattleData GetCalcurateDate()
    {
        return CalcuratedData;
    }

    void SetSlider()
    {
        SceneManagerScript.instance.SetSlider(CalcuratedData.HP, CalcuratedData.MaxHP, 1);
        SceneManagerScript.instance.SetOrbPieceAmount(0, OrbPieces.green);
        SceneManagerScript.instance.SetOrbPieceAmount(1, OrbPieces.red);
        SceneManagerScript.instance.SetOrbPieceAmount(2, OrbPieces.blue);
        SceneManagerScript.instance.SetExpSlider(Exp, MaxExp);
    }

    private IEnumerator Start()
    {
        yield return null;
        isDeath = false;
        MaxHP = HP;
        ReturnCalcurateData();

        foreach (var sk in SyokiSkills)
        {
            Skills.Add(SceneManagerScript.instance.GenerateSkill(sk));
        }
    }
    public int TotalAttackPoint()
    {
        int ReturnInt = AttackPoint + Random.Range(-1, 2);

        return ReturnInt;
    }
    public void StartAddPlayerExp(int value)
    {
        StartCoroutine(AddPlayerExp(value));
    }
    public IEnumerator AddPlayerExp(int value)
    {
        Exp += value;
        while (Exp >= MaxExp)
        {
            LevelUp();
            if (SceneManagerScript.instance.LevelCount == 0) SceneManagerScript.instance.ActiveLevelCounter();
            SceneManagerScript.instance.LevelCount++;
            SceneManagerScript.instance.SetLevelCounterText();
            Exp -= MaxExp;
            MaxExp = Mathf.FloorToInt(10 + (4f * Mathf.Pow(1.5f, Level - 1)));
            yield return null;
            if(staticScript.ImmeLevel && !SceneManagerScript.instance.LevelSelecting) SceneManagerScript.instance.StartCoroutine("LevelUpCoroutine");
        }
        SceneManagerScript.instance.SetExpSlider(Exp, MaxExp);
        yield return null;
    }

    public void LevelUp()
    {
        Level++;
        if (Level < 16)
        {
            int HPadd = (int)Mathf.Floor(MaxHP * 0.06f);
            MaxHP += HPadd;
            HP += HPadd;
            CalcuratedData.HP += HPadd;
            CalcuratedData.MaxHP += HPadd;
            if (Level < 7) AttackPoint += 2;
            else AttackPoint += 1;
        }
        else if (Level < 35)
        {
            int HPadd = (int)Mathf.Floor(MaxHP * 0.07f);
            MaxHP += HPadd;
            HP += HPadd;
            CalcuratedData.HP += HPadd;
            CalcuratedData.MaxHP += HPadd;
            AttackPoint += 1;
        }
        else
        {
            int HPadd = (int)Mathf.Floor(MaxHP * 0.05f);
            MaxHP += HPadd;
            HP += HPadd;
            CalcuratedData.HP += HPadd;
            CalcuratedData.MaxHP += HPadd;
            AttackPoint += 1;
        }
        SkillTreeManager.Instance.ReflexBaseData(this, CalcuratedData);
        SkillTreeManager.Instance.CalculateAllDatas(this, CalcuratedData);
        if (SceneManagerScript.instance.isSurvive) SceneManagerScript.instance.GenerateSize++;
        SetSlider();
    }

    public void GainSkill(SkillData data)
    {
        if (Skills.Count == 0)
        {
            SkillData addData = SceneManagerScript.instance.GenerateSkill(data);
            addData.SkillLevel = 1;
            Skills.Add(addData);
        }
        else
        {
            bool check = false;
            foreach (SkillData BaseData in Skills)
            {
                if (BaseData.ID == data.ID)
                {
                    BaseData.SkillLevelUp();
                    check = true;

                    break;
                }
            }
            if (check == false)
            {
                SkillData addData = SceneManagerScript.instance.GenerateSkill(data);
                addData.SkillLevel = 1;
                Skills.Add(addData);
            }

        }
    }



    public void PlaySoundEffect(AudioClip clip)
    {
        AudioManager.instance.PlaySE(clip);
    }

    public void PlaySoundEffect3D(AudioClip clip)
    {
        AudioManager.instance.PlaySE3D(clip, transform);
    }
    
    public void PlayerReviveAction()
    {
        MaxHP = 300;
        HP = MaxHP;
        AttackPoint = 8;
        OrbPieces.green = 5;
        OrbPieces.red = 5;
        OrbPieces.blue = 5;
        DefencePoint = 0;
        Level = 1;
        Exp = 0;
        MaxExp = 10;
        Skills.Clear();
        foreach (var sk in SyokiSkills)
        {
            Skills.Add(SceneManagerScript.instance.GenerateSkill(sk));
        }
        SceneManagerScript.instance.SetExpSlider(Exp, MaxExp);
        SceneManagerScript.instance.SetLevelText(Level);
        ResetCalcurateData();
    }
}

[System.Serializable]
public class OrbClass{
    public int green=0;
    public int red=0;
    public int blue=0;
}