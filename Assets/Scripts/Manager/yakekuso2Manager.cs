using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using System.Collections.Generic;

public class yakekuso2Manager : MonoBehaviour
{
    public Image Panel;
    public VideoPlayer videoPlayer;
    public RectTransform moveImage;
    public Image logoImage;
    public TextMeshProUGUI thanks;
    public AudioSource source;
    public AudioSource bgmSource;
    public TextMeshProUGUI returnText;
    public List<TextMeshProUGUI> texts;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async UniTask Start()
    {

        Panel.color = new Color(0, 0, 0, 1);
        Panel.DOFade(0f, 1.5f);
        await UniTask.Delay(TimeSpan.FromSeconds(3f));
        await moveImage.DOAnchorPosX(300f, 3f).SetEase(Ease.InOutQuad);
        logoImage.DOFade(1f, 1.5f);
        await logoImage.rectTransform.DOAnchorPosY(-120, 1.5f).SetEase(Ease.OutQuad);
        thanks.rectTransform.DOAnchorPosY(-141, 3f).SetEase(Ease.OutQuad);
        source.volume = staticScript._seVolume;
        source.Play();
        await thanks.DOFade(1f, 3f);
        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));
        foreach (var v in texts)
        {
            v.rectTransform.DOLocalMoveY(v.rectTransform.anchoredPosition.y - 20f, 0.5f).SetEase(Ease.OutQuad);
            v.DOFade(1f, 0.5f);
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        }
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        bgmSource.volume = staticScript._bgmVolume * 0.6f;
        bgmSource.Play();
        OperateActions _opt = new OperateActions();
        _opt.Enable();
        returnText.DOFade(1f, 1.5f);
        await UniTask.WaitUntil(() => _opt.Player.Jump.IsPressed());
        _opt.Dispose();
        Panel.DOKill();
        DOTween.To(() => staticScript._bgmVolume * 0.6f, t => bgmSource.volume = t, 0f, 1.5f);
        await Panel.DOFade(1, 1.5f);
        SceneManager.LoadScene("Title2");
    }

    // Update is called once per frame


}
