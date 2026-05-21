using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;
using UnityEngine.UI;

public class TutorialParts : MonoBehaviour
{
    public enum Kinds
    {
        move,
        jump,
        attack,
        skill,
        camera,
        defence,
        kaihi,
        hissatu
    }
    public Kinds Kind;
    public TutorialParts NextParts;
    public ITutorialActivater tutorialActivater;
    public Image TutorialGage;
    public Animator DoorAnim;
    public bool isWait;
    public float waitTime;

    void Start()
    {
        TutorialGage.fillAmount = 0;
    }


    public async UniTask ActTutorial()
    {
        switch (Kind)
        {
            case Kinds.move:
                tutorialActivater = new MoveTutorialAction();
                break;
            case Kinds.jump:
                tutorialActivater = new JumpTutorialAction();
                break;
            case Kinds.attack:
                tutorialActivater = new AttackTutorialAction();
                break;
            case Kinds.skill:
                tutorialActivater = new SkillTutorialAction();
                break;
            case Kinds.camera:
                tutorialActivater = new CameraTutorialAction();
                break;
            case Kinds.defence:
                tutorialActivater = new DefenceTutorialAction();
                break;
            case Kinds.kaihi:
                tutorialActivater = new RollingTutorialAction();
                break;
            case Kinds.hissatu:
                tutorialActivater = new FinishingTutorialAction();
                break;
        }
        float waitGage = 0;
        
        while (waitGage < 1)
        {
            float waitValue = await tutorialActivater.Activate();
            waitGage += waitValue;
            if (isWait)
            {
                DOTween.To(() => TutorialGage.fillAmount, t => TutorialGage.fillAmount = t, waitGage, waitTime);
                await tutorialActivater.Finish();
            }
            else
            {
                TutorialGage.fillAmount = waitGage;
            }
        }
        DoorAnim.SetTrigger("OpenT");
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        if (NextParts != null) await NextParts.ActTutorial();
    }

}
