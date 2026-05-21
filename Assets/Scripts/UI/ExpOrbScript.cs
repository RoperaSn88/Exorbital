using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExpOrbScript : MonoBehaviour
{
    public enum Kinds
    {
        One,
        Five,
        Thirty,
    }
    public Kinds Kind;
    public Animator ExpAnim;
    public GameObject Trail;
    TrailScript InstantiateTrail;
    RectTransform Rect;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        //�A�j���[�V�����ݒ�
        int a=(int)Kind;
        ExpAnim.SetTrigger($"{a}");
        InstantiateTrail = Instantiate(Trail).GetComponent<TrailScript>();
        InstantiateTrail.GetStart();
        InstantiateTrail.gameObject.transform.SetParent(SceneManagerScript.instance.CameraCanvas.transform);
        SceneManagerScript.instance.ScreenToWorldInCameraCanvas(transform,InstantiateTrail.Rect);
        
        //
        Rect = GetComponent<RectTransform>();
        float FirstTime = Random.Range(0.300f, 0.400f);
        float SecondTime = Random.Range(0.600f, 0.800f);
        float FirstPosX = Random.Range(-150, 150);
        InstantiateTrail.StartMoving(FirstTime,FirstPosX,SecondTime);
        Rect.DOLocalMoveX(FirstPosX, FirstTime).SetEase(Ease.OutCubic).SetLink(gameObject);
        Rect.DOLocalMoveY(-270, FirstTime + SecondTime).SetEase(Ease.InCubic).SetLink(gameObject);
        yield return new WaitForSeconds(FirstTime);
        Rect.DOLocalMoveX(0, SecondTime).SetEase(Ease.InCubic).SetLink(gameObject);
        yield return new WaitForSeconds(SecondTime);
        switch (Kind)
        {
            case Kinds.One:
                SceneManagerScript.instance.AddExpSlider(1);
                break;
            case Kinds.Five:
                SceneManagerScript.instance.AddExpSlider(5);
                break;
            case Kinds.Thirty:
                SceneManagerScript.instance.AddExpSlider(30);
                break;
        }
        Destroy(gameObject);
    }

    
    
    // Update is called once per frame
    void Update()
    {

    }
}
