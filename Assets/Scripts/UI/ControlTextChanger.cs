using System;
using TMPro;
using UnityEngine;

public class ControlTextChanger : MonoBehaviour,IChangerUI
{
    [SerializeField] private TextMeshProUGUI tmp;
    [Header("0: キーボード, 1: XBox, 2: PlayStation")]
    [SerializeField,TextArea,Tooltip("0: キーボード, 1: XBox, 2: PlayStation")]
    private string[] _texts = new string[3];

    //num: 0:キーボード,1:GamePad
    public void ChangeUI(int num)
    {
        tmp.text = _texts[num];
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