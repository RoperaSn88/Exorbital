using Cysharp.Threading.Tasks;
using UnityEngine;
public class RollingTutorialAction : ITutorialActivater
{
    public async UniTask<float> Activate()
    {
        await UniTask.WaitUntil(() => TutorialManager.Instance.inp.Player.Defend.IsPressed()
        && TutorialManager.Instance.inp.Player.Move.ReadValue<Vector2>().magnitude >= 0.8f);
        return 0.334f;
    }

    public async UniTask Finish()
    {
        
    }
}
