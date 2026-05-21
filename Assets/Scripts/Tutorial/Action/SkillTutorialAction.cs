using Cysharp.Threading.Tasks;
using UnityEngine;
public class SkillTutorialAction : ITutorialActivater
{
    public async UniTask<float> Activate()
    {
        await UniTask.WaitUntil(() => TutorialManager.Instance.inp.Player.Skill.IsPressed());
        await UniTask.WaitUntil(() => TutorialManager.Instance.inp.Player.Attack.IsPressed());
        return 0.5f;
    }

    public async UniTask Finish()
    {
        
    }
}
