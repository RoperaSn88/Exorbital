using UnityEngine;

[CreateAssetMenu(menuName = "UI/Controller Text Data")]
public class ControllerTextData : ScriptableObject
{
    private const int ControllerTypeCount = 3;

    [TextArea, Tooltip("0: キーボード, 1: XBox, 2: PlayStation")]
    [SerializeField] private string[] _texts = new string[ControllerTypeCount];

    public string GetText(int index)
    {
        if (_texts == null)
        {
            Debug.LogWarning("ControllerTextData texts are not set.", this);
            return string.Empty;
        }
        if (index < 0 || index >= _texts.Length)
        {
            Debug.LogWarning($"ControllerTextData index out of range: {index}", this);
            return string.Empty;
        }
        return _texts[index];
    }

    private void OnValidate()
    {
        if (_texts == null || _texts.Length != ControllerTypeCount)
        {
            System.Array.Resize(ref _texts, ControllerTypeCount);
        }
    }
}
