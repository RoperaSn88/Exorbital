using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
public class MoveTutorialAction : ITutorialActivater
{
    bool b;
    public async UniTask<float> Activate()
    {
        await UniTask.WaitUntil(() => TutorialManager.Instance.inp.Player.Move.ReadValue<Vector2>().magnitude >= 0.8f);
        return Time.deltaTime / 5;
    }

    public async UniTask Finish()
    {
        
    }
}
