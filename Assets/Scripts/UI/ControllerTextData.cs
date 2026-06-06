using UnityEngine;

[CreateAssetMenu(menuName = "UI/Controller Text Data")]
public class ControllerTextData : ScriptableObject
{
    [TextArea, Tooltip("0: キーボード, 1: XBox, 2: PlayStation")]
    public string[] Texts = new string[3];

    public string GetText(int index)
    {
        if (Texts == null) return string.Empty;
        if (index < 0 || index >= Texts.Length) return string.Empty;
        return Texts[index];
    }
}
