using UnityEngine;

[System.Serializable]
public class LocalizedTextData
{
    public string ID;
    [TextArea]
    public string Japanese;
    [TextArea]
    public string English;

    public string GetText(string defaultText = "")
    {
        string targetText;
        if (staticScript.Language == LanguageKinds.English)
        {
            targetText = English;
            if (string.IsNullOrEmpty(targetText)) targetText = Japanese;
        }
        else
        {
            targetText = Japanese;
            if (string.IsNullOrEmpty(targetText)) targetText = English;
        }

        if (string.IsNullOrEmpty(targetText)) return defaultText;
        return targetText;
    }
}
