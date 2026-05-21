using Cysharp.Threading.Tasks;
using UnityEngine;
public class CameraTutorialAction : ITutorialActivater
{
    public async UniTask<float> Activate()
    {
        await UniTask.WaitUntil(() => Mathf.Abs(TutorialManager.Instance.inp.Player.CameraRotate.ReadValue<Vector2>().x) >= 0.3f);
        return Time.deltaTime / 3;
    }
    public async UniTask Finish()
    {
        
    }
}
