using UnityEngine;
using UnityEngine.Experimental.Video;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Collections;
using Cysharp.Threading.Tasks;

public class ProlougeManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Image skipButton;
    [SerializeField] private Image fadePanel;
    private bool videoStarted = false;
    private OperateActions _act;

    IEnumerator Start()
    {
        _act = new OperateActions();
        _act.Enable();
        videoPlayer.SetDirectAudioVolume(0, 0.5f);
        videoPlayer.Play();
        yield return new WaitUntil(() => videoPlayer.isPlaying);
        videoStarted = true;
    }

    void Update()
    {
        if (videoStarted)
        {
            if (!videoPlayer.isPlaying)
            {
                GoTutorialScene();
                videoStarted = false;
            }

            if (_act.Player.Jump.IsPressed())
            {
                skipButton.fillAmount += Time.deltaTime;
                if (skipButton.fillAmount >= 1f)
                {
                    ClosePanel();
                    videoStarted = false;
                }
            }
            else
            {
                if (skipButton.fillAmount > 0) skipButton.fillAmount -= Time.deltaTime * 0.5f;
                if(skipButton.fillAmount < 0) skipButton.fillAmount = 0;
            }
        }
    }

    async void ClosePanel()
    {
        await UniTask.WhenAll(fadePanel.DOFade(1, 1f).ToUniTask(),
        DOTween.To(() => videoPlayer.GetDirectAudioVolume(0), x => videoPlayer.SetDirectAudioVolume(0, x), 0, 1f).ToUniTask());
        GoTutorialScene();
    }
    
    async void GoTutorialScene()
    {
        _act.Dispose();
        await Task.Delay(TimeSpan.FromSeconds(0.5f));
        SceneManager.LoadScene("PlayerSceneVer3");
    }
}