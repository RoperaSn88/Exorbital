using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public int _saveDataNumber;
    public int _sceneID;
    public int _hp;
    public int _maxHP;
    public int _greenOrbs;
    public int _redOrbs;
    public int _blueOrbs;
    public int _attack;
    public int _defend;
    public int _greenPoint;
    public int _redPoint;
    public int _bluePoint;
    public int _exp;
    public int _maxExp;
    public int _level;
    public int _skillCount;
    public List<int> _skillDataID;
    public List<SaveSkillData> _skillData;
    public int _greenNodeCount;
    public int _redNodeCount;
    public int _blueNodeCount;
    public int _levelCount;
    public float _finishingTime;
}
