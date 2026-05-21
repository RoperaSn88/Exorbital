using Cysharp.Threading.Tasks;
using UnityEngine;

public class AttackTutorialAction : ITutorialActivater
{
    public async UniTask<float> Activate()
    {
        await UniTask.WaitUntil(() => TutorialManager.Instance.inp.Player.Attack.IsPressed());
        return 0.112f;
    }

    public async UniTask Finish()
    {
        await UniTask.WaitUntil(() => !TutorialManager.Instance.inp.Player.Attack.IsPressed());
    }
}
