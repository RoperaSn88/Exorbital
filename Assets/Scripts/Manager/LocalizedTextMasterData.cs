using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalizedTextData
{
    public string ID;
    [TextArea]
    public string Japanese;
    [TextArea]
    public string English;

    public string GetText()
    {
        if (staticScript.Language == LanguageKinds.English)
        {
            if (string.IsNullOrEmpty(English)) return Japanese;
            return English;
        }

        if (string.IsNullOrEmpty(Japanese)) return English;
        return Japanese;
    }
}

public class LocalizedTextMasterDataBase : ScriptableObject
{
    [SerializeField] LocalizedTextData[] _texts;
    Dictionary<string, LocalizedTextData> _cache;

    public string GetText(string id, string defaultText = "")
    {
        if (string.IsNullOrEmpty(id)) return defaultText;

        if (_cache == null)
        {
            _cache = new Dictionary<string, LocalizedTextData>();
            if (_texts != null)
            {
                foreach (var textData in _texts)
                {
                    if (textData == null || string.IsNullOrEmpty(textData.ID)) continue;
                    _cache[textData.ID] = textData;
                }
            }
        }

        if (_cache.TryGetValue(id, out var data)) return data.GetText();
        return defaultText;
    }
}

[CreateAssetMenu(fileName = "SkillTextMasterData", menuName = "Localization/SkillTextMasterData")]
public class SkillTextMasterData : LocalizedTextMasterDataBase
{
}

[CreateAssetMenu(fileName = "SkillNodeTextMasterData", menuName = "Localization/SkillNodeTextMasterData")]
public class SkillNodeTextMasterData : LocalizedTextMasterDataBase
{
    public const string GreenLevelTextID = "SKILL_NODE_GREEN_LEVEL";
    public const string RedLevelTextID = "SKILL_NODE_RED_LEVEL";
    public const string BlueLevelTextID = "SKILL_NODE_BLUE_LEVEL";
    public const string NoneTextID = "SKILL_NODE_NONE";
}

[CreateAssetMenu(fileName = "OperationInstructionTextMasterData", menuName = "Localization/OperationInstructionTextMasterData")]
public class OperationInstructionTextMasterData : LocalizedTextMasterDataBase
{
}

[CreateAssetMenu(fileName = "StageTextMasterData", menuName = "Localization/StageTextMasterData")]
public class StageTextMasterData : LocalizedTextMasterDataBase
{
}
