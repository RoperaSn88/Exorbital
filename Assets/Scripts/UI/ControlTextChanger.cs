using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ControlTextChanger : MonoBehaviour,IChangerUI
{
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private ControllerTextData _textData;
    [Header("Legacy data: 0: キーボード, 1: XBox, 2: PlayStation")]
    [SerializeField, TextArea, Tooltip("0: キーボード, 1: XBox, 2: PlayStation")]
    [FormerlySerializedAs("_texts")]
    private string[] _legacyTexts = new string[3];

    //num: 0:キーボード,1:GamePad
    public void ChangeUI(int num)
    {
        if (_textData != null)
        {
            tmp.text = _textData.GetText(num);
            return;
        }

        if (_legacyTexts == null || num < 0 || num >= _legacyTexts.Length)
        {
            Debug.LogWarning($"Legacy controller text index out of range: {num}", this);
            tmp.text = string.Empty;
            return;
        }

        tmp.text = _legacyTexts[num];
    }

    public void VisibleUI()
    {
        tmp.gameObject.SetActive(true);
    }

    public void InvisibleUI()
    {
        tmp.gameObject.SetActive(false);
    }
}