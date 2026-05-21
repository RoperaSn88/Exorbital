using Cysharp.Threading.Tasks;
using UnityEngine;
public class JumpTutorialAction : ITutorialActivater
{
    public async UniTask<float> Activate()
    {
        await UniTask.WaitUntil(() => TutorialManager.Instance.inp.Player.Jump.IsPressed());
        return 0.334f;

    }
    
    public async UniTask Finish()
    {
        await UniTask.WaitUntil(() => !TutorialManager.Instance.inp.Player.Jump.IsPressed());
    }
}
