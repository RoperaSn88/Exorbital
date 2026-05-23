using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionSceneController : MonoBehaviour
{
    SceneManagerScript Manager => SceneManagerScript.instance;
    SettingUIClass Settings => Manager.Settings;
    CinemachineThirdPersonFollow C3F => Manager.C3F;
    CinemachineRotationComposer CRC => Manager.CRC;

    IEnumerator Start()
    {
        if (Manager == null || PlayerController.instance == null)
        {
            yield return SceneManager.UnloadSceneAsync(gameObject.scene);
            yield break;
        }

        yield return Setting();
    }

    IEnumerator Setting()
    {
        Time.timeScale = 0f;
        Settings.Base.SetActive(true);
        Settings.BGMSlider.value = AudioManager.instance.BGMvolume;
        Settings.SESlider.value = AudioManager.instance.SEvolume;
        PlayerController.instance.StopController();
        Settings.Settingnum = 0;
        GameObject[] textureCanvases = GameObject.FindGameObjectsWithTag("TextureCanvas");
        optionAction optAct = new optionAction();
        optAct.Enable();

        try
        {
            while (true)
            {
                Settings.Texts[Settings.Settingnum].DOFade(1f, 0.2f).SetUpdate(UpdateType.Normal, true);
                yield return null;
                yield return new WaitUntil(() => optAct.normal.SelectV.ReadValue<float>() == 1 || optAct.normal.SelectV.ReadValue<float>() == -1 || optAct.normal.SelectH.ReadValue<float>() == 1 || optAct.normal.SelectH.ReadValue<float>() == -1 ||
                optAct.normal.decide.IsPressed() || optAct.normal.close.IsPressed() || optAct.normal.option.IsPressed());
                if (optAct.normal.SelectV.ReadValue<float>() == -1 && Settings.Settingnum < 3)
                {
                    Settings.Texts[Settings.Settingnum].DOFade(0.3f, 0.2f).SetUpdate(UpdateType.Normal, true);
                    Settings.Settingnum++;
                    if (Settings.Settingnum == 3) Settings.Settingnum = 0;
                    yield return new WaitUntil(() => optAct.normal.SelectV.ReadValue<float>() == 0);
                }
                else if (optAct.normal.SelectV.ReadValue<float>() == 1 && Settings.Settingnum > -1)
                {
                    Settings.Texts[Settings.Settingnum].DOFade(0.3f, 0.2f).SetUpdate(UpdateType.Normal, true);
                    Settings.Settingnum--;
                    if (Settings.Settingnum == -1) Settings.Settingnum = 2;
                    yield return new WaitUntil(() => optAct.normal.SelectV.ReadValue<float>() == 0);
                }

                if (Settings.Settingnum == 1 && optAct.normal.decide.IsPressed())
                {
                    yield return DetailSetting(textureCanvases, optAct);
                }

                if (Settings.Settingnum == 0 && optAct.normal.decide.IsPressed() || optAct.normal.option.IsPressed())
                {
                    Debug.Log("Close");
                    Settings.Texts[Settings.Settingnum].DOFade(0.3f, 0.1f);
                    yield return CloseSettingScene();
                    yield break;
                }

                if (Settings.Settingnum == 2 && optAct.normal.decide.IsPressed())
                {
                    Manager.FadePanelImage.gameObject.SetActive(true);
                    Manager.FadePanelImage.DOFade(1f, 1.5f).SetUpdate(UpdateType.Normal, true);
                    AudioManager.instance.StopBGM(1.5f);
                    yield return new WaitForSecondsRealtime(1.5f);
                    Time.timeScale = 1;
                    SceneManager.LoadScene("Title2");
                    yield break;
                }

                yield return null;
            }
        }
        finally
        {
            optAct.Dispose();
        }
    }

    IEnumerator DetailSetting(GameObject[] textureCanvases, optionAction optAct)
    {
        int detailPanelNum = 0;
        int detailNum = 0;
        Settings._pausePanel.SetActive(false);
        Settings._detailSettingPanels[detailPanelNum].SetActive(true);
        Settings._cameraHeightSlider.value = staticScript.CameraPos;
        Settings._immeLevelToggle.isOn = staticScript.ImmeLevel;
        yield return new WaitUntil(() => !optAct.normal.decide.IsPressed());
        while (true)
        {
            yield return null;

            Debug.Log($"Page:{detailPanelNum},num:{detailNum}");
            yield return new WaitUntil(() => optAct.normal.pageTab.ReadValue<float>() == 1 || optAct.normal.pageTab.ReadValue<float>() == -1
            || optAct.normal.SelectV.ReadValue<float>() == 1 || optAct.normal.SelectV.ReadValue<float>() == -1 || optAct.normal.SelectH.ReadValue<float>() == 1 || optAct.normal.SelectH.ReadValue<float>() == -1
            || optAct.normal.decide.IsPressed() || optAct.normal.close.IsPressed() || optAct.normal.option.IsPressed());
            if (optAct.normal.close.IsPressed() || optAct.normal.option.IsPressed())
            {
                Settings._pausePanel.SetActive(true);
                Settings._detailSettingPanels[detailPanelNum].SetActive(false);
                yield return new WaitUntil(() => !optAct.normal.close.IsPressed() && !optAct.normal.option.IsPressed());
                yield break;
            }

            if (optAct.normal.decide.IsPressed() || optAct.normal.SelectH.ReadValue<float>() == 1 || optAct.normal.SelectH.ReadValue<float>() == -1)
            {
                switch (detailPanelNum)
                {
                    case 0:
                        switch (detailNum)
                        {
                            case 0:
                                if (optAct.normal.SelectH.ReadValue<float>() == -1)
                                {
                                    AudioManager.instance.BGMvolume -= Time.unscaledDeltaTime;
                                    Settings.BGMSlider.value = AudioManager.instance.BGMvolume;
                                    AudioManager.instance.BGMsource.volume = AudioManager.instance.BGMvolume;
                                    staticScript._bgmVolume = AudioManager.instance.BGMvolume;
                                    continue;
                                }
                                if (optAct.normal.SelectH.ReadValue<float>() == 1)
                                {
                                    AudioManager.instance.BGMvolume += Time.unscaledDeltaTime;
                                    Settings.BGMSlider.value = AudioManager.instance.BGMvolume;
                                    AudioManager.instance.BGMsource.volume = AudioManager.instance.BGMvolume;
                                    staticScript._bgmVolume = AudioManager.instance.BGMvolume;
                                    continue;
                                }
                                break;
                            case 1:
                                if (optAct.normal.SelectH.ReadValue<float>() == -1)
                                {
                                    AudioManager.instance.SEvolume -= Time.unscaledDeltaTime;
                                    Settings.SESlider.value = AudioManager.instance.SEvolume;
                                    staticScript._seVolume = AudioManager.instance.SEvolume;
                                    continue;
                                }
                                if (optAct.normal.SelectH.ReadValue<float>() == 1)
                                {
                                    AudioManager.instance.SEvolume += Time.unscaledDeltaTime;
                                    Settings.SESlider.value = AudioManager.instance.SEvolume;
                                    staticScript._seVolume = AudioManager.instance.SEvolume;
                                    continue;
                                }
                                break;
                        }
                        break;

                    case 1:
                        switch (detailNum)
                        {
                            case 0:
                                if (optAct.normal.SelectH.ReadValue<float>() == -1)
                                {
                                    Settings._cameraHeightSlider.value -= Time.unscaledDeltaTime * 2.5f;
                                }
                                else if (optAct.normal.SelectH.ReadValue<float>() == 1)
                                {
                                    Settings._cameraHeightSlider.value += Time.unscaledDeltaTime * 2.5f;
                                }
                                C3F.ShoulderOffset = new Vector3(0, Settings._cameraHeightSlider.value, 0);
                                staticScript.CameraPos = Settings._cameraHeightSlider.value;
                                staticScript.CanvasRotate = staticScript.CalcurateCanvasRotate(staticScript.CameraPos);
                                foreach (var tex in textureCanvases)
                                {
                                    tex.transform.rotation = Quaternion.Euler(staticScript.CanvasRotate, tex.transform.rotation.eulerAngles.y, 0);
                                    float keepY = tex.GetComponent<TextureKeepPos>().BeginKeepPosY;
                                    tex.transform.localPosition = new Vector3(0, keepY - tex.transform.localScale.x * (1 - Mathf.Cos(Mathf.PI * staticScript.CanvasRotate / 180)) / 2, 0);
                                }
                                break;
                            case 1:
                                if (optAct.normal.decide.IsPressed())
                                {
                                    staticScript.ImmeLevel = !staticScript.ImmeLevel;
                                    Settings._immeLevelToggle.isOn = staticScript.ImmeLevel;
                                    yield return new WaitUntil(() => !optAct.normal.decide.IsPressed());
                                }
                                break;
                        }
                        break;
                }
                yield return null;
            }
            if (optAct.normal.pageTab.ReadValue<float>() == -1)
            {
                Settings.TextClass[detailPanelNum].Texts[detailNum].color = new Color(1, 1, 1, 0.3f);
                detailNum = 0;
                Settings._detailSettingPanels[detailPanelNum].SetActive(false);
                if (detailPanelNum == 0) detailPanelNum = Settings._detailSettingPanels.Length - 1;
                else detailPanelNum--;
                Settings._detailSettingPanels[detailPanelNum].SetActive(true);
                Settings.TextClass[detailPanelNum].Texts[detailNum].color = new Color(1, 1, 1, 1f);
                yield return new WaitUntil(() => optAct.normal.pageTab.ReadValue<float>() == 0);
            }
            if (optAct.normal.pageTab.ReadValue<float>() == 1)
            {
                Settings.TextClass[detailPanelNum].Texts[detailNum].color = new Color(1, 1, 1, 0.3f);
                detailNum = 0;
                Settings._detailSettingPanels[detailPanelNum].SetActive(false);
                if (detailPanelNum == Settings._detailSettingPanels.Length - 1) detailPanelNum = 0;
                else detailPanelNum++;
                Settings._detailSettingPanels[detailPanelNum].SetActive(true);
                Settings.TextClass[detailPanelNum].Texts[detailNum].color = new Color(1, 1, 1, 1f);
                yield return new WaitUntil(() => optAct.normal.pageTab.ReadValue<float>() == 0);
            }

            if (optAct.normal.SelectV.ReadValue<float>() == -1)
            {
                Settings.TextClass[detailPanelNum].Texts[detailNum].color = new Color(1, 1, 1, 0.3f);
                detailNum++;
                if (detailNum == 2) detailNum = 0;
                Settings.TextClass[detailPanelNum].Texts[detailNum].color = new Color(1, 1, 1, 1f);
                yield return new WaitUntil(() => optAct.normal.SelectV.ReadValue<float>() == 0);
            }
            else if (optAct.normal.SelectV.ReadValue<float>() == 1)
            {
                Settings.TextClass[detailPanelNum].Texts[detailNum].color = new Color(1, 1, 1, 0.3f);
                detailNum--;
                if (detailNum == -1) detailNum = 1;
                Settings.TextClass[detailPanelNum].Texts[detailNum].color = new Color(1, 1, 1, 1f);
                yield return new WaitUntil(() => optAct.normal.SelectV.ReadValue<float>() == 0);
            }
        }
    }

    IEnumerator CloseSettingScene()
    {
        Time.timeScale = 1;
        Settings.Base.SetActive(false);
        PlayerController.instance.StartController();
        Settings.Settingnum = 0;
        yield return null;
        yield return SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}

[System.Serializable]
public class SettingUIClass
{
    public int Settingnum;
    public GameObject Base;
    public TextMeshProUGUI[] Texts;
    public Slider BGMSlider;
    public Slider SESlider;
    public GameObject _pausePanel;
    public Slider _cameraHeightSlider;
    public GameObject[] _detailSettingPanels;
    public Toggle _immeLevelToggle;
    public SettingUITextClass[] TextClass;
}

[System.Serializable]
public class SettingUITextClass
{
    public List<TextMeshProUGUI> Texts;
}
