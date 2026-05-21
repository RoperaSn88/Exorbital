using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set;}
    public TutorialParts BeginPart;
    public String SceneName = "チュートリアル";
    public AudioClip music = null;
    public SceneObject thisScene;
    public SceneObject NextScene;
    public Color BackGroundColor;
    public OperateActions inp;

    public List<GameObject> _changers;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async Task Start()
    {
        Instance = this;
        inp = new OperateActions();
        inp.Enable();
        SceneManagerScript.instance.StartStage();
        foreach(var v in _changers)
        {
            v.TryGetComponent<IChangerUI>(out var p);
            SceneManagerScript.instance.AddChangeWithController(p);
        }
        await BeginPart.ActTutorial();
        foreach(var v in _changers)
        {
            v.TryGetComponent<IChangerUI>(out var p);
            SceneManagerScript.instance.RemoveChangeWithController(p);
        }
    }


}
