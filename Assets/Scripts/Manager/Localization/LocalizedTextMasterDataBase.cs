using System.Collections.Generic;
using UnityEngine;

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

        if (_cache.TryGetValue(id, out var data)) return data.GetText(defaultText);
        return defaultText;
    }
}
