using Cysharp.Threading.Tasks;
using UnityEngine;
public class DefenceTutorialAction : ITutorialActivater
{
    public async UniTask<float> Activate()
    {
        await UniTask.WaitUntil(() => TutorialManager.Instance.inp.Player.Defend.IsPressed());
        return Time.deltaTime / 3;
    }
    
    public async UniTask Finish()
    {
        await UniTask.WaitUntil(() => !TutorialManager.Instance.inp.Player.Defend.IsPressed());
    }
}
