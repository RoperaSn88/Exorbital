using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrailScript : MonoBehaviour
{
    public RectTransform Rect;
    // Start is called before the first frame update
    public void GetStart()
    {
        Rect=GetComponent<RectTransform>();
    }
    public void StartMoving(float FirstT, float FirstX, float SecondT)
    {
        StartCoroutine(Moving(FirstT, FirstX, SecondT));
    }
    public IEnumerator Moving(float FirstT, float FirstX, float SecondT)
    {
        Rect.DOLocalMoveX(FirstX, FirstT).SetEase(Ease.OutCubic).SetLink(gameObject);
        Rect.DOLocalMoveY(-270, FirstT + SecondT).SetEase(Ease.InCubic).SetLink(gameObject);
        yield return new WaitForSeconds(FirstT);
        Rect.DOLocalMoveX(0, SecondT).SetEase(Ease.InCubic).SetLink(gameObject);
        yield return new WaitForSeconds(SecondT);
        Rect.DOLocalMoveY(-1000, 0.1f).SetLink(gameObject).OnComplete(() => Destroy(gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
