using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OrbEffectScript : MonoBehaviour
{
    Light objectLight;
    Tween tw;
    [SerializeField]AudioClip audio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Effect(int num,int amount){
        objectLight = GetComponent<Light>();
        TrailRenderer renderer=GetComponent<TrailRenderer>();
        if(num==0){//green
            renderer.startColor=new Color(0, 1, 0,0.8f);
            renderer.endColor=new Color(0, 1, 0,0.8f);
            objectLight.color=new Color(0, 1, 0,0.8f);
        }
        else if(num==1){//red
            renderer.startColor=new Color(1, 0, 0,0.8f);
            renderer.endColor=new Color(1, 0, 0,0.8f);
            objectLight.color=new Color(1, 0, 0,0.8f);
        }
        if(num==2){//blue
            renderer.startColor=new Color(0, 0.8f, 0.8f,0.8f);
            renderer.endColor=new Color(0, 0.8f, 0.8f,0.8f);
            objectLight.color=new Color(0, 0.8f, 0.8f,0.8f);
        }
        
        StartCoroutine(EffectCoroutine(num,amount));
    }

    IEnumerator EffectCoroutine(int num,int amount){
        float randomT1 = Random.Range(0.3f, 0.6f);
        tw=transform.DOMove(new Vector3(transform.position.x + Random.Range(-0.5f,0.5f),transform.position.y + Random.Range(0.2f,1.0f),transform.position.z + Random.Range(-0.5f,0.5f)),
            randomT1).SetEase(Ease.OutQuad).SetLink(gameObject);
        yield return new WaitForSeconds(randomT1 - 0.1f);
        float randomT2 = Random.Range(0.9f, 1.3f);
        Vector3 BeginPos=new Vector3(transform.position.x,transform.position.y,transform.position.z);
        DOTween.To(()=>0f,t=>{
            transform.position=Vector3.Lerp(BeginPos,PlayerController.instance.transform.position,t);
            objectLight.intensity=1f-t;
        },randomT2,Random.Range(0.4f,0.75f)).SetEase(Ease.InQuad).SetLink(gameObject).OnComplete(()=>{
            AudioManager.instance.PlaySE(audio,0.6f);
            SceneManagerScript.instance.AddOrbPiece(num,amount);
            Destroy(this.gameObject);
        }).SetLink(gameObject);
    }


}
