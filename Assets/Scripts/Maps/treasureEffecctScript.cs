using System.Collections;
using UnityEngine;
using DG.Tweening;

public class treasureEffecctScript : MonoBehaviour
{
    public enum Kinds{
        little,
        middle,
        match
    }
    public Kinds kind;
    public void Effect(int amount){
        StartCoroutine(EffectCoroutine(amount));
    }

    IEnumerator EffectCoroutine(int amount){
        transform.DOMove(new Vector3(transform.position.x + Random.Range(-0.5f,0.5f),transform.position.y + Random.Range(0.2f,1.0f),transform.position.z + Random.Range(-0.5f,0.5f)),
            0.5f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.4f);
        Vector3 BeginPos=new Vector3(transform.position.x,transform.position.y,transform.position.z);
        DOTween.To(()=>0f,t=>{
            transform.position=Vector3.Lerp(BeginPos,PlayerController.instance.transform.position,t);
        },1f,Random.Range(0.4f,0.75f)).SetEase(Ease.InQuad).OnComplete(()=>{
            SceneManagerScript.instance.AddMoney(amount);
            Destroy(gameObject);
        });
    }
}
