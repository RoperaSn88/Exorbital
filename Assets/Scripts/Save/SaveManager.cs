using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; set; }

    public PlayerSaveData _saveData { get; set; }
    [SerializeField] string _filePath;
    [SerializeField] string _SkillPath;
    [SerializeField] string _fileName = "PlayerData";
    [SerializeField] string _SkillName = "SkillData.json";

    private void Awake()
    {
        Instance = this;
        staticScript._dataNumber = 1;
        _fileName = _fileName + staticScript._dataNumber.ToString() + ".json";
        _filePath = Application.dataPath + "/" + _fileName;
        _SkillPath = Application.dataPath + "/" + _SkillName;
    }


    void SetData(ForBattleData data)
    {
        if (_saveData == null) _saveData = new PlayerSaveData();
        //セーブデータは1として設定しておく
        _saveData._saveDataNumber = 1;

        _saveData._sceneID = SceneLoader.Instance.SceneID;
        _saveData._hp = data.HP;
        _saveData._maxHP = data.MaxHP;
        _saveData._attack = data.AttackPoint;
        _saveData._defend = data.DefencePoint;
        _saveData._greenOrbs = data.OrbPieces.green;
        _saveData._redOrbs = data.OrbPieces.red;
        _saveData._blueOrbs = data.OrbPieces.blue;
        _saveData._greenPoint = data.GreenPoint;
        _saveData._redPoint = data.RedPoint;
        _saveData._bluePoint = data.BluePoint;
        _saveData._exp = data.Exp;
        _saveData._maxExp = data.MaxExp;
        _saveData._level = data.Level;
        _saveData._skillCount = SceneManagerScript.instance.LevelCount;
        _saveData._greenNodeCount = SkillTreeManager.Instance.NodeCounts[0];
        _saveData._redNodeCount = SkillTreeManager.Instance.NodeCounts[1];
        _saveData._blueNodeCount = SkillTreeManager.Instance.NodeCounts[2];
        _saveData._skillData = new List<SaveSkillData>();
        _saveData._finishingTime = PlayerController.instance._finishingTimer;
        _saveData._levelCount = SceneManagerScript.instance.LevelCount;
        foreach (var d in data.Skills)
        {
            SaveSkillData saveSkillData = new SaveSkillData();
            saveSkillData._id = d.ID;
            saveSkillData._needGreenOrbs = d.NeedOrbs.green;
            saveSkillData._needRedOrbs = d.NeedOrbs.red;
            saveSkillData._needBlueOrbs = d.NeedOrbs.blue;
            saveSkillData._plusDamage = d.PlusDamage;
            saveSkillData._healPoint = d.HealPoint;
            saveSkillData._level = d.SkillLevel;
            saveSkillData._timer = d.Timer;
            _saveData._skillData.Add(saveSkillData);
        }
        //Debug.Log("set Data");
    }

    public void SaveData()
    {
        SetData(ForBattleData.instance);
        _filePath = Application.dataPath + "/" + _fileName;
        string json = JsonUtility.ToJson(_saveData);
        StreamWriter wr = new StreamWriter(_filePath, false);
        wr.WriteLine(json);
        wr.Close();
    }

    PlayerSaveData LoadData()
    {
        if (!File.Exists(_filePath)) return null;
        StreamReader rd = new StreamReader(_filePath);
        string json = rd.ReadToEnd();
        rd.Close();

        return JsonUtility.FromJson<PlayerSaveData>(json);
    }

    public void ReflexPlayerData()
    {
        PlayerSaveData saveData = LoadData();
        if (saveData == null)
        {
            Debug.Log("Non Saved Data");
            return;
        }

        //ForBattleData.instanceにPlayerのDataがまとまっている悪しきコードなので、それに合わせる
        ForBattleData.instance.HP = saveData._hp;
        ForBattleData.instance.MaxHP = saveData._maxHP;
        ForBattleData.instance.AttackPoint = saveData._attack;
        ForBattleData.instance.DefencePoint = saveData._defend;
        ForBattleData.instance.OrbPieces = new OrbClass();
        ForBattleData.instance.OrbPieces.green = saveData._greenOrbs;
        ForBattleData.instance.OrbPieces.red = saveData._redOrbs;
        ForBattleData.instance.OrbPieces.blue = saveData._blueOrbs;
        ForBattleData.instance.GreenPoint = saveData._greenPoint;
        ForBattleData.instance.RedPoint = saveData._redPoint;
        ForBattleData.instance.BluePoint = saveData._bluePoint;
        ForBattleData.instance.Exp = saveData._exp;
        ForBattleData.instance.MaxExp = saveData._maxExp;
        ForBattleData.instance.Level = saveData._level;

        //スキル
        ForBattleData.instance.Skills.Clear();
        foreach (var s in saveData._skillData)
        {
            SkillData skillData = null;
            foreach (var baseSkill in SceneManagerScript.instance.AllSkills)
            {
                if (baseSkill.ID == s._id)
                {
                    skillData = SceneManagerScript.instance.GenerateSkill(baseSkill);
                    skillData.PlusDamage = s._plusDamage;
                    skillData.HealPoint = s._healPoint;
                    skillData.NeedOrbs.green = s._needGreenOrbs;
                    skillData.NeedOrbs.red = s._needRedOrbs;
                    skillData.NeedOrbs.blue = s._needBlueOrbs;
                    skillData.SkillLevel = s._level;
                    skillData.Timer = s._timer;
                    ForBattleData.instance.Skills.Add(skillData);
                    break;
                }
            }

            if (skillData == null)
            {
                Debug.LogError($"存在しないスキルが保存されています ID:{s._id}");
                continue;
            }
        }

        for (int i = 0; i < saveData._greenNodeCount; i++)
        {
            SkillTreeManager.Instance.AddNode(SkillTreeManager.Instance.NowGreenNode,false);
            SkillTreeManager.Instance.NowGreenNode = SkillTreeManager.Instance.NowGreenNode.childNode;
            SkillTreeManager.Instance.NodeCounts[0]++;
        }

        for (int i = 0; i < saveData._redNodeCount; i++)
        {
            SkillTreeManager.Instance.AddNode(SkillTreeManager.Instance.NowRedNode,false);
            SkillTreeManager.Instance.NowRedNode = SkillTreeManager.Instance.NowRedNode.childNode;
            SkillTreeManager.Instance.NodeCounts[1]++;
        }

        for (int i = 0; i < saveData._blueNodeCount; i++)
        {
            SkillTreeManager.Instance.AddNode(SkillTreeManager.Instance.NowBlueNode,false);
            SkillTreeManager.Instance.NowBlueNode = SkillTreeManager.Instance.NowBlueNode.childNode;
            SkillTreeManager.Instance.NodeCounts[2]++;
        }

        ForBattleData.instance.ReturnCalcurateData();

        PlayerController.instance._finishingTimer = saveData._finishingTime;
        SceneManagerScript.instance.LevelCount = saveData._levelCount;
        if(saveData._levelCount > 0)SceneManagerScript.instance.ActiveLevelCounter();
    }
}

[System.Serializable]
public class SaveSkillData
{
    //保存してほしいものだけ保存をして、元のデータからそのまま設定できるものは対象外として実行する
    public int _id;
    public int _needGreenOrbs;
    public int _needRedOrbs;
    public int _needBlueOrbs;
    public int _plusDamage;
    public int _healPoint;
    public float _timer;
    public int _level;
    // public int _levelUpDamage;
    // public int _levelUpTimer;
}
