using UnityEngine;
using DG.Tweening;
using System.Collections;

public class treasureScript : MonoBehaviour
{

    public GameObject OpenEffect;
    public GameObject OpenUI;
    public Animator Anim;
    bool check=false;
    public bool isOpen;

    public void OpenTreasure(){
        StartCoroutine(OpenTreasureCoroutine());
    }

    IEnumerator OpenTreasureCoroutine(){
        OrbClass orbs=new OrbClass(){
            green=Random.Range(3,6),
            red=Random.Range(3,6),
            blue=Random.Range(3,6)
        };
        check=false;
        isOpen=true;
        OpenUI.SetActive(false);
        Anim.SetTrigger("OpenT");
        if(orbs.green>0){
            OrbEffectScript sc=Instantiate(OpenEffect,transform).GetComponent<OrbEffectScript>();
            sc.Effect(0,orbs.green);
        }
        if(orbs.red>0){
            OrbEffectScript sc=Instantiate(OpenEffect,transform).GetComponent<OrbEffectScript>();
            sc.Effect(1,orbs.red);
        }
        if(orbs.blue>0){
            OrbEffectScript sc=Instantiate(OpenEffect,transform).GetComponent<OrbEffectScript>();
            sc.Effect(2,orbs.blue);
        }
        
        yield return null;
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player")){
            if(!isOpen) check=true;
        }
    }

    void Update()
    {
        if(check)SceneManagerScript.instance.TreasureOpenUI(transform,this);
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player")){
            OpenUI.SetActive(false);
            if(!isOpen) check=false;
        }
    }

}
