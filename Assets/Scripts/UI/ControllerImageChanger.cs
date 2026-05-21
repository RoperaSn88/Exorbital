using UnityEngine;
using UnityEngine.UI;

public class ControllerImageChanger: MonoBehaviour,IChangerUI
{
    [SerializeField] private Image img;
    [SerializeField] private Sprite[] _sprites = new Sprite[3];

    //num: 0:キーボード,1:Xbox, 2:ps4
    public void ChangeUI(int num)
    {
        img.sprite = _sprites[num];
    }

    public void VisibleUI()
    {
        img.gameObject.SetActive(true);
    }

    public void InvisibleUI()
    {
        img.gameObject.SetActive(false);
    }
    
}
