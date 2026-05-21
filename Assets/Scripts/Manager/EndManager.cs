using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    [SerializeField]
    private Image _panel;

    [SerializeField]
    private AudioSource _audio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async UniTask Start()
    {
        await _panel.DOFade(0,0.5f).ToUniTask();
        OperateActions _opt = new OperateActions();
        _opt.Enable();
        await UniTask.WaitUntil(() => _opt.Player.Jump.IsPressed());
        await UniTask.WhenAll(
            _panel.DOFade(1,0.5f).ToUniTask(),
            DOTween.To(() => _audio.volume, x =>
            {
                _audio.volume = x;
            },0,0.5f).ToUniTask()
        );
        SceneManager.LoadScene("Title2");

    }
}
