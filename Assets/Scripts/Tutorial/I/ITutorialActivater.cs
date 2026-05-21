using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ITutorialActivater
{
    public UniTask<float> Activate();
    public UniTask Finish();
} 
