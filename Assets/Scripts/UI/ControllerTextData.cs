using UnityEngine;

[CreateAssetMenu(menuName = "UI/Controller Text Data")]
public class ControllerTextData : ScriptableObject
{
    private const int ControllerTypeCount = 3;

    [TextArea, Tooltip("0: キーボード, 1: XBox, 2: PlayStation")]
    [SerializeField] private string[] _japaneseTexts = new string[ControllerTypeCount];
    [TextArea, Tooltip("0: キーボード, 1: XBox, 2: PlayStation")]
    [SerializeField] private string[] _englishTexts = new string[ControllerTypeCount];

    public string GetText(int index)
    {
        return GetText(index, Application.systemLanguage);
    }

    public string GetText(int index, SystemLanguage language)
    {
        if (index < 0 || index >= ControllerTypeCount)
        {
            Debug.LogWarning($"ControllerTextData index out of range: {index}", this);
            return string.Empty;
        }

        bool isEnglish = language == SystemLanguage.English;
        string text = GetTextFromArray(isEnglish ? _englishTexts : _japaneseTexts, index);
        if (!string.IsNullOrEmpty(text))
        {
            return text;
        }

        return GetTextFromArray(isEnglish ? _japaneseTexts : _englishTexts, index);
    }

    private static string GetTextFromArray(string[] texts, int index)
    {
        if (texts == null || index >= texts.Length) return string.Empty;
        return texts[index];
    }

    private void OnValidate()
    {
        if (_japaneseTexts == null || _japaneseTexts.Length != ControllerTypeCount)
        {
            System.Array.Resize(ref _japaneseTexts, ControllerTypeCount);
        }
        if (_englishTexts == null || _englishTexts.Length != ControllerTypeCount)
        {
            System.Array.Resize(ref _englishTexts, ControllerTypeCount);
        }
    }
}
