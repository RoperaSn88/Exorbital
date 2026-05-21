using Cysharp.Threading.Tasks;
using UnityEngine;
public class FinishingTutorialAction : ITutorialActivater
{
    public async UniTask<float> Activate()
    {
        await UniTask.WaitUntil(() => PlayerController.instance._specialTechCoolTime >= 180);
        await UniTask.WaitUntil(() => TutorialManager.Instance.inp.Player.Special.IsPressed());
        return 1f;
    }

    public async UniTask Finish()
    {
        
    }
}
