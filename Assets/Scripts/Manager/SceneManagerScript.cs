using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using System.Linq;
using Manager.SelectElement;
using System.Threading.Tasks;

public class SceneManagerScript : MonoBehaviour
{
    public bool isSurvive;
    public static SceneManagerScript instance;
    Camera MainCam;
    public CinemachineCamera CVC;
    public CinemachineThirdPersonFollow C3F;
    public CinemachineRotationComposer CRC;

    public GameObject Canvas;
    public GameObject CameraCanvas;
    RectTransform CanvasTrans;
    [SerializeField] GameObject DamageText;
    [SerializeField] GameObject DamageTextPlayer;
    [SerializeField] GameObject FinishingDamageText;
    [SerializeField] Slider HPSlider;
    [SerializeField] Slider MPSlider;
    [SerializeField] TextMeshProUGUI HPText;
    [SerializeField] TextMeshProUGUI MPText;
    [SerializeField] List<TextMeshProUGUI> OrbTexts;
    [SerializeField] Slider ExpSlider;
    public RectTransform Icons;
    [SerializeField] Image IconChara;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Animator levelEffectAnim;
    [SerializeField] GameObject[] ExpOrb;
    public List<SkillData> AllSkills;
    public SkillPanelScript SkillPanel;
    [SerializeField] Transform SkillGainParent;
    public SkillGainPanelScript SkillGainPanelS;
    [SerializeField] RectTransform SkillGainSelecter;
    ForBattleData PlayerData;
    [NonSerialized] public bool CanLevelUp = false;
    [NonSerialized] public bool LevelSelecting;
    [NonSerialized] public int LevelCount;
    [SerializeField] RectTransform LevelCounter;
    [SerializeField] Animator LevelAnimator;
    [SerializeField] TextMeshProUGUI LevelCounterText;
    [SerializeField] GameObject LevelOperateImage;
    [SerializeField] GameObject LevelUpOperateText;
    [SerializeField] GameObject LevelUpEffect;
    [SerializeField] AudioClip LevelUpSound;
    public bool EnemyControllF;
    public int GenerateSize;
    [SerializeField] Image FadePanel;
    [SerializeField] GameObject FinishPanel;
    [SerializeField] TextMeshProUGUI finishTimeText;
    [SerializeField] TextMeshProUGUI finishLevelText;
    [SerializeField] TextMeshProUGUI KillText;
    [SerializeField] TextMeshProUGUI StageText;
    [SerializeField] TextMeshProUGUI SerihuText;
    bool timerF;
    float LowSTimer;
    int sTimer;
    int mtimer;
    [NonSerialized] public int kills;
    string filePath;
    public int maxEnemyCount;
    [NonSerialized] public bool Finishing;
    Volume StageVolume;
    [SerializeField] TextMeshProUGUI MoneyText;
    public BossClass BossInfo;
    public SettingUIClass Settings;
    bool _isSettingSceneActive;
    [SerializeField] GameObject UpDownPanel;
    public GameObject MapCanvas;
    public GameObject MapPieceObject;

    //Generator用
    MapGeneraterVer2 generater;
    [SerializeField] AudioClip BGMclip;
    public List<LevelingClass> RogueLikeLevelUps;
    public Transform BossPos;
    float RealMapSize;
    bool MapSet = false;
    const float UnvisitedMiniMapAlpha = 0.5f;
    HashSet<MapBaseScriptVer2> _revealedMiniMaps = new HashSet<MapBaseScriptVer2>();
    public SkillNodeClass SkillNodeInfos;
    public Image _finishingPanel;
    public AudioClip _finishingSound;
    [SerializeField] Image _specialGauge;
    [SerializeField] GameObject _specialOperateImage;
    [SerializeField] Animator _specialPanelAnim;
    [SerializeField] AudioClip _specialAudio;
    public Image FadePanelImage => FadePanel;

    public delegate void ChangeWithController(int num);
    [NonSerialized] public ChangeWithController _changerController;
    [SerializeField] public List<GameObject> changers;

    private void Awake()
    {
        instance = this;

        if (!isSurvive)
        {
            CVC.enabled = false;
            SerihuText.text = "";
        }

        FadePanel.gameObject.SetActive(true);
        FadePanel.color = new Color(0, 0, 0, 1f);
        if (isSurvive)
        {
            timerF = true;
            FadePanel.DOFade(0, 0.5f);
            Debug.Log("asasasas");
        }
        EnemyControllF = true;
        MainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        CanvasTrans = Canvas.GetComponent<RectTransform>();

        foreach(var v in changers)
        {
            v.TryGetComponent<IChangerUI>(out var p);
            AddChangeWithController(p);
        }
    }
    public Vector3 ReSpawnVec;

    //SurviveModeのみ有効
    void Start()
    {
        if (isSurvive)
        {
            PlayerData = PlayerController.instance.GetComponent<ForBattleData>();
            AudioManager.instance.PlayBGM(BGMclip);
            // SetElementImage(ForBattleData.instance.Element);
        }
    }

    [SerializeField] RectTransform PlayerIconPos;
    [SerializeField] RectTransform MapCellsPos;
    float MapSize;

    Vector3 SavedPlayerPos = new Vector3(0, 0, 0);
    public void UpdatePlayerMapPos(Transform PlayerTrans, bool isRight)
    {
        if (!MapSet || PlayerTrans == null || PlayerIconPos == null || MapCellsPos == null || RealMapSize <= 0f) return;

        Vector3 UseVec = PlayerTrans.position - SavedPlayerPos;
        if (UseVec.x > 0 && PlayerIconPos.anchoredPosition.x > 40) MapCellsPos.anchoredPosition -= new Vector2(UseVec.x, 0) * MapSize / RealMapSize;
        else if (UseVec.x < 0 && PlayerIconPos.anchoredPosition.x < -40) MapCellsPos.anchoredPosition -= new Vector2(UseVec.x, 0) * MapSize / RealMapSize;
        else PlayerIconPos.anchoredPosition += new Vector2(UseVec.x, 0) * MapSize / RealMapSize;

        if (UseVec.z > 0 && PlayerIconPos.anchoredPosition.y > 20) MapCellsPos.anchoredPosition -= new Vector2(0, UseVec.z) * MapSize / RealMapSize;
        else if (UseVec.z < 0 && PlayerIconPos.anchoredPosition.y < -20) MapCellsPos.anchoredPosition -= new Vector2(0, UseVec.z) * MapSize / RealMapSize;
        else PlayerIconPos.anchoredPosition += new Vector2(0, UseVec.z) * MapSize / RealMapSize;

        float setRotationValue = PlayerTrans.rotation.eulerAngles.y - 90;
        //if (!isRight) setRotationValue += 180;
        PlayerIconPos.rotation = Quaternion.Euler(0, 0, -setRotationValue);

        SavedPlayerPos = PlayerTrans.position;
    }
    public void ResetMapPos()
    {
        SavedPlayerPos = Vector3.zero;
        MapCellsPos.anchoredPosition = Vector2.zero;
        PlayerIconPos.anchoredPosition = Vector2.zero;
    }
    public void StartStage(MapGeneraterVer2 gen=null)
    {
        TutorialManager Tutorial = null;
        if (gen == null)
        {
            Tutorial = GameObject.FindWithTag("Tutorial").GetComponent<TutorialManager>();
        }
        ReSpawnVec = gen? gen.ReSpawnVec:Vector3.zero;

        FadePanel.DOFade(0f, 3f);
        PlayerData = PlayerController.instance.GetComponent<ForBattleData>();
        CVC.enabled = true;
        timerF = true;
        StageText.DOFade(1, 0.01f);
        StageText.rectTransform.anchoredPosition = new Vector3(200, 60, 0);
        StageText.text = gen? gen.StageName:Tutorial.SceneName;
        StageText.rectTransform.DOAnchorPosX(-200, 0.75f).SetEase(Ease.OutQuad);
        StageText.DOFade(1f, 1.5f).OnComplete(() => StageText.DOFade(0, 1.0f));
        //背景設定　チュートリアルステージならばtrue
        if (gen == null) SetCameraBackGround(true);
        else SetCameraBackGround();
        if (gen != null) SetupMiniMapTileMaps(gen);
        PlayerController.instance.ControlF = true;
        SavedPlayerPos = PlayerController.instance.transform.position;
        //ForBattleData.instance.ReturnCalcurateData();
        if (!staticScript._hasData) SaveManager.Instance.SaveData();

        AudioManager.instance.PlayBGM(gen?gen.BackGroundMusic:Tutorial.music);

        //チュートリアルではない　→　続きからなのでデータを復元させる
        if (staticScript._hasData)
        {
            SaveManager.Instance.ReflexPlayerData();
            staticScript._hasData = false;
        }
    }

    void SetupMiniMapTileMaps(MapGeneraterVer2 gen)
    {
        _revealedMiniMaps.Clear();
        if (gen == null || gen.MapParent == null)
        {
            MapSet = false;
            return;
        }

        MapBaseScriptVer2[] maps = gen.MapParent.GetComponentsInChildren<MapBaseScriptVer2>(true);
        foreach (MapBaseScriptVer2 map in maps)
        {
            if (map == null) continue;
            map.SetMiniMapAlpha(UnvisitedMiniMapAlpha);
        }

        MapSet = maps.Length > 0;
    }

    public void RevealVisitedMiniMap(MapBaseScriptVer2 map)
    {
        if (map == null) return;
        if (_revealedMiniMaps.Contains(map)) return;
        map.SetMiniMapAlpha(1f);
        _revealedMiniMaps.Add(map);
    }

    public void InvisibleUIs()
    {
        MapCanvas.gameObject.SetActive(false);
        Icons.gameObject.SetActive(false);
        ExpSlider.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);
        BossInfo.HPBar.gameObject.SetActive(false);
        BossInfo.BossNameText.gameObject.SetActive(false);
        BossInfo.BossSecondNameText.gameObject.SetActive(false);
        BossInfo.BossHPNameText.gameObject.SetActive(false);
        BossInfo.BossHPSecondNameText.gameObject.SetActive(false);
        LevelCounter.gameObject.SetActive(false);
        LevelOperateImage.SetActive(false);
    }

    public void VisibleUIs()
    {
        MapCanvas.gameObject.SetActive(true);
        Icons.gameObject.SetActive(true);
        ExpSlider.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
        LevelCounter.gameObject.SetActive(true);
        if(LevelCount > 0)LevelOperateImage.SetActive(true);
    }

    public void AddChangeWithController(IChangerUI a)
    {
        _changerController += a.ChangeUI;
    }

    public void RemoveChangeWithController(IChangerUI a)
    {
        _changerController -= a.ChangeUI;
    }

    public void StartBGM()
    {
        AudioManager.instance.PlayBGM(BGMclip);
    }

    public void StartTutorialBGM(AudioClip audio)
    {
        AudioManager.instance.PlayBGM(audio);
    }

    public void AddMoney(int amount)
    {
        ForBattleData.instance.money += amount;
        DOTween.To(() => 0, (m) =>
        {
            MoneyText.text = (ForBattleData.instance.money + m).ToString("D6");
        }, amount, 0.5f).SetEase(Ease.InQuad);
    }
    private DG.Tweening.Sequence greenSeq;
    private DG.Tweening.Sequence redSeq;
    private DG.Tweening.Sequence yellowSeq;
    public void AddOrbPiece(int num, int amount)
    {
        //緑:0、赤:1、黄:2
        switch (num)
        {
            case 0:
                greenSeq.Kill(true);
                greenSeq = DOTween.Sequence().Append(DOTween.To(() => 0, (m) =>
                {
                    OrbTexts[num].text = (ForBattleData.instance.OrbPieces.green + m).ToString("D4");
                }, amount, 0.5f).SetEase(Ease.InQuad)).OnComplete(() => ForBattleData.instance.OrbPieces.green += amount);
                break;
            case 1:
                redSeq.Kill(true);
                redSeq = DOTween.Sequence().Append(DOTween.To(() => 0, (m) =>
                {
                    OrbTexts[num].text = (ForBattleData.instance.OrbPieces.red + m).ToString("D4");
                }, amount, 0.5f).SetEase(Ease.InQuad)).OnComplete(() => ForBattleData.instance.OrbPieces.red += amount);
                break;
            case 2:
                yellowSeq.Kill(true);
                yellowSeq = DOTween.Sequence().Append(DOTween.To(() => 0, (m) =>
                {
                    OrbTexts[num].text = (ForBattleData.instance.OrbPieces.blue + m).ToString("D4");
                }, amount, 0.5f).SetEase(Ease.InQuad)).OnComplete(() => ForBattleData.instance.OrbPieces.blue += amount);
                break;
        }
    }

    public async UniTask StartElement()
    {
        ForBattleData.instance.Element = await ElementPresenter.instance.StartElement(ForBattleData.instance.Element);
        ForBattleData.instance.ReturnCalcurateData();
    }

    public void StartStageEndCoroutine()
    {
        StartCoroutine(StageEndCoroutine());
    }
    IEnumerator StageEndCoroutine()
    {
        //ステージ→休憩所
        Debug.Log("ステージ→休憩所");
        finishTimeText.gameObject.SetActive(true);
        KillText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        finishTimeText.text = $"Time : {mtimer}:{sTimer}";
        finishTimeText.rectTransform.DOAnchorPosX(-210f, 1f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.2f);
        KillText.text = $"Kills : {kills}/{maxEnemyCount}";
        KillText.rectTransform.DOAnchorPosX(-210f, 1f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1.0f);
        AudioManager.instance.StopBGM(1.5f);
        FadePanel.gameObject.SetActive(true);
        FadePanel.color = new Color(0, 0, 0, 0f);
        FadePanel.DOFade(1f, 0.5f);
        yield return null;
        //次のステージを取得しておく
        SceneLoader.Instance.targetScene = generater?generater.NextScene:TutorialManager.Instance.NextScene;
        SceneLoader.Instance.SetnextSceneID(SceneLoader.Instance.targetScene);
        CVC.enabled = false;
        yield return new WaitForSeconds(1.5f);
        SceneLoader.Instance.SceneUnLoad(generater?generater.thisScene:TutorialManager.Instance.thisScene);
        SceneLoader.Instance.isBreaking = true;
        SceneLoader.Instance.SceneLoad();
        yield return null;
        AudioManager.instance.PlayBGM(BGMclip);
        PlayerController.instance.MovedStage();
        ForBattleData.instance.ReturnCalcurateData();
        yield return new WaitForSeconds(0.5f);
        //休憩所のBGMを流す
        SetCameraBackGround();
        TimerReset();
        CVC.enabled = true;
        FadePanel.DOFade(0f, 0.75f);
        PlayerController.instance.ControlF = true;
        SaveManager.Instance.SaveData();
    }
    public void SetLevelText(int l)
    {
        levelText.text = $"Lv.{l}";
    }
    void SetCameraBackGround(bool tutorial = false)
    {
        GameObject aaa = GameObject.FindWithTag("Generator");
        Color c;
        if (aaa != null)
        {
            generater = aaa.GetComponent<MapGeneraterVer2>();
            c = generater.color;
        }
        else
        {
            c = new Color(0.986f, 1, 0);
            if (tutorial) c = TutorialManager.Instance.BackGroundColor;
        }
        StageVolume = GameObject.FindWithTag("Volume").GetComponent<Volume>();
        StageVolume.profile.TryGet<Bloom>(out Bloom bl);
        bl.tint.value = c;
        MainCam.backgroundColor = c;
        RenderSettings.fogColor = c;
    }

    public void StartSceneMoveCoroutine()
    {
        StartCoroutine(SceneMoveCoroutine());
    }

    IEnumerator SceneMoveCoroutine()
    {
        //休憩所→ステージ
        Debug.Log("休憩所→ステージ");
        Finishing = true;
        PlayerController.instance.ControlF = false;
        FadePanel.gameObject.SetActive(true);
        FadePanel.color = new Color(0, 0, 0, 0f);
        FadePanel.DOFade(1f, 0.5f);
        AudioManager.instance.StopBGM(1.0f);
        yield return new WaitForSeconds(1.0f);
        CVC.enabled = false;
        yield return null;
        //isBreakingなのでどの道結果は変わらん
        SceneLoader.Instance.SceneUnLoad(SceneLoader.Instance.targetScene);
        SceneLoader.Instance.isBreaking = false;
        SceneLoader.Instance.SceneLoad();
        TimerReset();
        PlayerController.instance.MovedStage();
    }

    public void StartFinishSceneCoroutine()
    {
        StartCoroutine(FinishSceneCoroutine());
    }

    IEnumerator FinishSceneCoroutine()
    {
        Finishing = true;
        PlayerController.instance.ControlF = false;
        FadePanel.gameObject.SetActive(true);
        FadePanel.color = new Color(0, 0, 0, 0f);
        FadePanel.DOFade(1f, 0.5f);
        AudioManager.instance.StopBGM(1.0f);
        yield return new WaitForSeconds(1.0f);
        CVC.enabled = false;
        yield return null;
        SceneManager.LoadScene("End");
    }
    [NonSerialized] public bool isGameOver;
    public void StartGameOver()
    {
        PlayerController.instance.StopController();
        PlayerController.instance.PlayerAnim.SetBool("DeadF", true);
        if (!isGameOver)
        {
            isGameOver = true;
            if (isSurvive) StartCoroutine(GameOverSurvive());
            else StartCoroutine(GameOverNormal());

        }

    }

    public IEnumerator GameOverNormal()
    {
        Time.timeScale = 0.25f;
        //ここで敵にぶっとばされておきたい

        DOTween.To(() => RenderSettings.fogStartDistance, v =>
        {
            RenderSettings.fogStartDistance = v;
        }, 0, 0.5f);
        DOTween.To(() => RenderSettings.fogEndDistance, v =>
        {
            RenderSettings.fogEndDistance = v;
        }, 0, 0.5f);
        DOTween.To(() => RenderSettings.fogColor, v =>
        {
            RenderSettings.fogColor = v;
        }, new Color(0, 0, 0, 1), 0.5f);

        DOTween.To(() => MainCam.backgroundColor, v =>
        {
            MainCam.backgroundColor = v;
        }, new Color(0, 0, 0, 1), 0.5f);
        yield return new WaitForSeconds(0.375f);
        Time.timeScale = 1f;
        yield return new WaitForSeconds(2f);
        FadePanel.gameObject.SetActive(true);
        FadePanel.DOColor(new Color(0, 0, 0, 1), 1.0f);
        AudioManager.instance.StopBGM(1.0f);
        yield return new WaitForSeconds(1.0f);
        //SceneManager.LoadScene("Title2");

        SceneLoader.Instance.SceneUnLoad(generater.thisScene);
        foreach(var v in GameObject.FindGameObjectsWithTag("Middle"))
        {
            Destroy(v.gameObject);
        }
        SceneLoader.Instance.isBreaking = false;
        SceneLoader.Instance.SceneLoadNumber(5); //洞窟のシーンナンバー
        TimerReset();
        PlayerController.instance.MovedStage();
        InvisibleUIs();
        yield return new WaitForSeconds(1.0f);
        RenderSettings.fogStartDistance = 10;
        RenderSettings.fogEndDistance = 30;
        SceneLoader.Instance.SceneID = 5; //洞窟のシーンナンバー
        PlayerController.instance.StartReviveCoroutine();
        PlayerController.instance.GetComponent<ForBattleData>().PlayerReviveAction();
        LevelCount = 0;
        ResetLevelCounter();
        SaveManager.Instance.SaveData();
        levelText.text = "Lv.1";
        yield return new WaitForSeconds(1.4f);
        VisibleUIs();
        isGameOver = false;
    }


    public IEnumerator GameOverSurvive()
    {
        //���̃Q�[���I�[�o�[�̓��@���T�o�V�[���̂�A��قǏC���K�v
        Time.timeScale = 0.25f;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
        //���ԁA���x���Akill���\��
        FinishPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        finishTimeText.text = $"Time : {mtimer}:{sTimer}";
        finishTimeText.rectTransform.DOAnchorPosX(0, 1f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1.5f);
        finishLevelText.rectTransform.DOAnchorPosX(0, 1f).SetEase(Ease.OutBack);
        finishLevelText.text = $"Level : {ForBattleData.instance.Level}";
        yield return new WaitForSeconds(1.5f);
        KillText.text = $"Kills : {kills}";
        KillText.rectTransform.DOAnchorPosX(0, 1f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(5f);

        //�����L���O�\��,�ۑ�
        //�t�F�[�h�A�E�g
        FadePanel.DOFade(1f, 1.5f);
        AudioManager.instance.StopBGM(1.5f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Title2");

    }
    public void SetElementImage(ElementKinds element)
    {
        IconChara.TryGetComponent<Animator>(out var v);
        v.SetTrigger(element.ToString() + "T");
    }
    public void StartBossPerform()
    {
        StartCoroutine(BossPerform());
    }
    public IEnumerator BossPerform()
    {
        PlayerController.instance.StopController();
        yield return new WaitForSeconds(1.0f);

        GameObject BossEnemy = GameObject.FindWithTag("Boss");
        Vector3 SaveDamp = CRC.Damping;
        CRC.Damping = new Vector2(0, 0);

        DOTween.To(() => new Vector3(0, 0, 0), p =>
        {
            CRC.TargetOffset = p;
            C3F.ShoulderOffset = new Vector3(p.x, 1.5f, p.z);
        }, new Vector3(5, 0, 0), 0.75f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.75f);
        Instantiate(UpDownPanel, CanvasTrans);
        CVC.Follow = BossEnemy.transform;
        DOTween.To(() => new Vector3(-6, 0, 0), p =>
        {
            CRC.TargetOffset = p;
            C3F.ShoulderOffset = new Vector3(p.x, 1.5f, p.z);
        }, new Vector3(-1, 0, 0), 0.75f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(1f);
        BossEnemyInfo EnemyInfo = BossEnemy.GetComponent<BossEnemyInfo>();
        BossInfo.BossNameText.text = EnemyInfo.Name;
        BossInfo.BossSecondNameText.text = EnemyInfo.secondName;
        BossInfo.BossNameText.gameObject.SetActive(true);
        BossInfo.BossSecondNameText.gameObject.SetActive(true);
        BossInfo.BossNameText.color = new Color(1, 1, 1, 0);
        BossInfo.BossSecondNameText.color = new Color(1, 1, 1, 0);
        BossInfo.BossNameText.rectTransform.localPosition += new Vector3(-200, 0, 0);
        BossInfo.BossSecondNameText.rectTransform.localPosition += new Vector3(-200, 0, 0);
        BossInfo.BossNameText.DOFade(1f, 1.5f);
        BossInfo.BossSecondNameText.DOFade(1f, 1.5f);
        BossInfo.BossNameText.rectTransform.DOLocalMoveX(0, 1.5f).SetEase(Ease.OutQuad);
        BossInfo.BossSecondNameText.rectTransform.DOLocalMoveX(0, 1.5f).SetEase(Ease.OutQuad);
        AudioManager.instance.PlayBGM(EnemyInfo.BGM);
        yield return new WaitForSeconds(2f);
        BossEnemy.GetComponent<Animator>().SetTrigger("AttackT");
        yield return new WaitForSeconds(1.2f);
        BossInfo.BossNameText.DOFade(0f, 1.5f);
        BossInfo.BossSecondNameText.DOFade(0f, 1.5f);
        DOTween.To(() => new Vector3(-1, 0, 0), p =>
        {
            CRC.TargetOffset = p;
            C3F.ShoulderOffset = new Vector3(p.x, 1.5f, p.z);
        }, new Vector3(-6, 0, 0), 0.75f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.75f);
        //戦闘開始
        BossInfo.BossHPNameText.text = EnemyInfo.Name;
        BossInfo.BossHPSecondNameText.text = EnemyInfo.secondName;
        BossInfo.BossHPNameText.gameObject.SetActive(true);
        BossInfo.BossHPSecondNameText.gameObject.SetActive(true);
        BossInfo.BossHPNameText.color = new Color(1, 1, 1, 0);
        BossInfo.BossHPSecondNameText.color = new Color(1, 1, 1, 0);
        BossInfo.BossHPNameText.DOFade(1f, 0.75f);
        BossInfo.BossHPSecondNameText.DOFade(1f, 0.75f);
        BossInfo.HPBar.gameObject.SetActive(true);
        BossInfo.BackGround.color = new Color(1, 1, 1, 0);
        BossInfo.Fill.color = new Color(1, 1, 1, 0);
        BossInfo.BackGround.DOFade(1f, 0.75f);
        BossInfo.Fill.DOFade(1f, 0.75f);
        BossInfo.HPBar.maxValue = EnemyInfo.GetComponent<FinallyCalcuratedBattleData>().MaxHP;
        BossInfo.HPBar.value = EnemyInfo.GetComponent<FinallyCalcuratedBattleData>().HP;

        CVC.Follow = PlayerController.instance.transform;
        DOTween.To(() => new Vector3(5, 0, 0), p =>
        {
            CRC.TargetOffset = p;
            C3F.ShoulderOffset = new Vector3(p.x, 1.5f, p.z);
        }, new Vector3(0, 0, 0), 0.75f).SetEase(Ease.OutQuad);
        CRC.Damping = SaveDamp;
        PlayerController.instance.StartController();
    }

    public void StartBossKill(FinallyCalcuratedBattleData data)
    {
        if (isSurvive)
        {
            ShowExpOrb(data.Exp, data.transform);
            return;
        }
        StartCoroutine(BossKill(data));
    }

    IEnumerator BossKill(FinallyCalcuratedBattleData Enemy)
    {
        PlayerController.instance.StopController();
        Time.timeScale = 0.25f;
        CVC.Follow = Enemy.gameObject.transform;
        yield return new WaitForSeconds(0.375f);
        GameObject.FindWithTag("BossBarrier").SetActive(false);
        AudioManager.instance.StopBGM(3f);
        ShowExpOrb(Enemy.Exp, Enemy.transform);
        Time.timeScale = 1f;
        CVC.Follow = PlayerController.instance.gameObject.transform;
        PlayerController.instance.StartController();
        BossInfo.BossHPNameText.DOFade(0f, 0.75f);
        BossInfo.BossHPSecondNameText.DOFade(0f, 0.75f);
        BossInfo.BackGround.DOFade(0f, 0.75f);
        BossInfo.Fill.DOFade(0f, 0.75f);
        yield return new WaitForSeconds(0.75f);
        BossInfo.BossHPNameText.gameObject.SetActive(false);
        BossInfo.BossHPSecondNameText.gameObject.SetActive(false);
        BossInfo.HPBar.gameObject.SetActive(false);

    }
    public void SetBossBar(int max, int val)
    {
        if (isSurvive) return;
        BossInfo.HPBar.maxValue = max;
        BossInfo.HPBar.value = val;
    }

    void TimerReset()
    {
        sTimer = 0;
        LowSTimer = 0;
        mtimer = 0;
        finishTimeText.rectTransform.anchoredPosition = new Vector3(200, 40, 0);
        KillText.rectTransform.anchoredPosition = new Vector3(200, 100, 0);
    }

    private void Update()
    {
        if (timerF)
        {
            if (!LevelSelecting && !isGameOver) LowSTimer += Time.deltaTime;
            if (LowSTimer >= 1f)
            {
                sTimer++;
                LowSTimer = 0f;
            }
            if (sTimer >= 60)
            {
                mtimer++;
                sTimer = 0;
            }
            string Sstring = sTimer.ToString("D2");
            string Mstring = mtimer.ToString("D2");
            timerText.text = $"{Mstring}:{Sstring}";
            levelText.text = $"Lv.{ForBattleData.instance.Level}";
        }

        // if (!isSurvive && MapSet)
        // {
        //     UpdatePlayerMapPos(PlayerController.instance.transform, PlayerController.instance.isRight);
        // }

        if (Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.P) && !restartF)
        {
            restartF = true;
            Time.timeScale = 1;
            SceneManager.LoadScene("Title2");
        }
    }
    Tweener LevelTween = null;

    public void ActiveLevelCounter()
    {
        if (LevelTween != null) if (LevelTween.active) LevelTween.Kill();
        LevelTween = LevelCounter.DOScale(new Vector3(1.5f, 1.5f, 1), 0.5f).SetEase(Ease.OutCubic);
        LevelAnimator.SetBool("ActiveF", true);
        CanLevelUp = true;
        LevelOperateImage.SetActive(true);
        SetLevelCounterText();
    }
    public void SetLevelCounterText()
    {
        LevelCounterText.text = $"{LevelCount}";
        levelEffectAnim.SetTrigger("levelT");
    }

    public void ResetLevelCounter()
    {
        LevelCounterText.text = "0";
        if (LevelTween != null) if (LevelTween.active) LevelTween.Kill();
        LevelTween = LevelCounter.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutCubic);
        LevelAnimator.SetBool("ActiveF", false);
        LevelOperateImage.SetActive(false);
        CanLevelUp = false;
    }

    bool restartF = false;

    public void SetSlider(int Value, int MaxValue, int Num)
    {
        switch (Num)
        {
            case 1:
                HPSlider.maxValue = MaxValue;
                HPSlider.value = Value;
                HPText.text = $"HP:{Value}/{MaxValue}";
                break;
            case 2:
                MPSlider.maxValue = MaxValue;
                MPSlider.value = Value;
                MPText.text = $"MP:{Value}/{MaxValue}";
                break;
            default:
                Debug.Log("ERROR");
                break;
        }
    }
    float _horizontalValue = 0;
    bool _decideF = false;
    public async UniTask ActionActivate(OperateActions _gameAction)
    {
        _gameAction.LevelUp.Act.started += OnDecide;
        _gameAction.LevelUp.SelectH.started += OnSelect;
        _gameAction.LevelUp.SelectH.performed += OnSelect;
        _gameAction.LevelUp.SelectH.canceled += OnSelect;
    }
    public void OnSelect(InputAction.CallbackContext context)
    {
        _horizontalValue = context.ReadValue<float>();
    }
    public void OnDecide(InputAction.CallbackContext context)
    {
        _decideF = true;
    }

    public void SetOrbPieceAmount(int num, int amount)
    {
        OrbTexts[num].text = amount.ToString("D4");
    }


    public IEnumerator LevelUpCoroutine()
    {
        yield return new WaitUntil(() => SkillPanelScript.instance.isFinishSelectSkill);
        LevelSelecting = true;

        List<SkillData> SelectedSkills = new List<SkillData>();
        List<GameObject> PanelObject = new List<GameObject>();
        SkillGainParent.gameObject.SetActive(true);
        LevelUpOperateText.SetActive(true);
        SkillGainSelecter.gameObject.SetActive(true);
        bool bossFlug = false;
        GameObject BossEnemy = null;

        while (LevelCount > 0)
        {
            if (isSurvive)
            {
                GenerateSize++;
                //一番初めのレベルとプレイヤーのレベルが一致したら生成数を上げる
                if (ForBattleData.instance.Level == RogueLikeLevelUps[0].levelNum)
                {

                    if (RogueLikeLevelUps[0].isBoss)
                    {
                        bossFlug = true;
                        BossEnemy = RogueLikeLevelUps[0].BossEnemy;
                    }
                    RogueLikeLevelUps.RemoveAt(0);
                }
            }
            AudioManager.instance.PlaySE(SceneManagerScript.instance.LevelUpSound);
            SetLevelCounterText();
            int count = 3;
            int SelectNumber = 1;
            List<SkillData> CopyAllSkills = new List<SkillData>();
            foreach (SkillData data in AllSkills) CopyAllSkills.Add(GenerateSkill(data));
            while (count > 0)
            {
                //ランダムにスキルを選ぶ
                SkillData data = CopyAllSkills[UnityEngine.Random.Range(0, CopyAllSkills.Count)];
                if (ForBattleData.instance.Level >= data.LimitLevel)
                {
                    SelectedSkills.Add(data);
                    CopyAllSkills.Remove(data);
                    count--;
                }
            }

            foreach (SkillData data in SelectedSkills)
            {
                foreach (SkillData hasData in ForBattleData.instance.Skills)
                {
                    if (data.ID == hasData.ID)
                    {
                        //�\�L����f�[�^�����Ⴆ��΂���
                        //�~�����̂�SkillLevel����s
                        data.SkillLevel = hasData.SkillLevel;
                        Debug.Log($"data:{data.SkillLevel}");
                    }
                }
                SkillGainPanelScript SkillGainPanel = Instantiate(SkillGainPanelS, SkillGainParent);
                PanelObject.Add(SkillGainPanel.gameObject);
                SkillGainPanel.SkillName.text = data.Name;
                SkillGainPanel.SkillExplain.text = data.Introduct;
                SkillGainPanel.SkillLevel.text = $"SkillLevel:{data.SkillLevel}";
                //いったん、MP表示のまま
                //SkillGainPanel.SkillMP.text = $"MP:{data.NeedMP}";
                SkillGainPanel.SkillOrbsText[0].text = $":{data.NeedOrbs.green}/{ForBattleData.instance.OrbPieces.green}";
                SkillGainPanel.SkillOrbsText[1].text = $":{data.NeedOrbs.red}/{ForBattleData.instance.OrbPieces.red}";
                SkillGainPanel.SkillOrbsText[2].text = $":{data.NeedOrbs.blue}/{ForBattleData.instance.OrbPieces.blue}";
            }
            PlayerController.instance.StopController();
            EnemyControllF = false;
            SkillGainSelecter.anchoredPosition = new Vector3(0, -5, 0);
            Instantiate(LevelUpEffect, Canvas.transform);
            yield return null;
            yield return new WaitForSeconds(0.3f);


            while (true)
            {
                _decideF = false;
                yield return new WaitUntil(() =>_horizontalValue == 1 || _horizontalValue == -1 || _decideF);
                if (_horizontalValue == -1  && SelectNumber != 0)
                {
                    //�X�L���I��
                    SelectNumber--;
                    SkillGainSelecter.DOAnchorPosX(300 * (SelectNumber - 1), 0.25f).SetEase(Ease.OutCubic);
                    yield return new WaitUntil(() => _horizontalValue == 0);
                }
                if (_horizontalValue == 1 && SelectNumber != 2)
                {
                    SelectNumber++;
                    SkillGainSelecter.DOAnchorPosX(300 * (SelectNumber - 1), 0.25f).SetEase(Ease.OutCubic);
                    yield return new WaitUntil(() => _horizontalValue == 0);
                }
                if (_decideF)
                {
                    ForBattleData.instance.GainSkill(SelectedSkills[SelectNumber]);

                    LevelCount--;
                    foreach (GameObject obj in PanelObject) Destroy(obj);
                    SelectedSkills.Clear();
                    _decideF = false;
                    break;
                }
            }
        }

        if (bossFlug)
        {
            StageText.DOFade(1, 0.01f);
            StageText.rectTransform.anchoredPosition = new Vector3(200, 60, 0);
            StageText.rectTransform.DOAnchorPosX(-200, 0.75f).SetEase(Ease.OutQuad);
            StageText.DOFade(1f, 1.5f).OnComplete(() => StageText.DOFade(0, 1.0f));
            Instantiate(BossEnemy, BossPos);
            bossFlug = false;
        }

        SkillGainSelecter.gameObject.SetActive(false);
        SkillGainParent.gameObject.SetActive(false);
        LevelUpOperateText.SetActive(false);
        PlayerController.instance.Data = null;
        PlayerController.instance.StartController();
        EnemyControllF = true;
        LevelSelecting = false;
        CanLevelUp = false;
        ResetLevelCounter();
    }

    public void SetExpSlider(int value, int MaxValue)
    {
        ExpSlider.maxValue = MaxValue;
        ExpSlider.value = value;
    }

    public void AddExpSlider(int value)
    {
        ExpSlider.DOComplete();
        ExpSlider.DOValue(ExpSlider.value + value, 0.2f).SetEase(Ease.InCubic)
            .OnComplete(() => PlayerData.StartAddPlayerExp(value));
    }

    public SkillData GenerateSkill(SkillData BaseSkill)
    {
        //SkillDataを種類分けしておきたいのだが、生成をどうするか...
        SkillData skill = ScriptableObject.CreateInstance<SkillData>();
        skill.Kind = BaseSkill.Kind;
        skill.Name = BaseSkill.Name;
        skill.Introduct = BaseSkill.Introduct;
        skill.NeedMP = BaseSkill.NeedMP;
        skill.ID = BaseSkill.ID;
        skill.NeedOrbs = BaseSkill.NeedOrbs;
        skill.PlusDamage = BaseSkill.PlusDamage;
        skill.HealPoint = BaseSkill.HealPoint;
        skill.Timer = BaseSkill.Timer;
        skill.SkillLevel = BaseSkill.SkillLevel;
        skill.LimitLevel = BaseSkill.LimitLevel;
        skill.EffectObject = BaseSkill.EffectObject;
        skill.SpawnPos = BaseSkill.SpawnPos;
        skill.Light = BaseSkill.Light;
        skill.LightPos = BaseSkill.LightPos;
        skill.LevelUpPlusDamage = BaseSkill.LevelUpPlusDamage;
        skill.LevelUpTimer = BaseSkill.LevelUpTimer;

        return skill;
    }
    public void ShowDamageText(int Damage, Transform Trans,ElementEffective effective)
    {
        Vector3 TargetScreenPos = MainCam.WorldToScreenPoint(Trans.position);
        GameObject Txt = (GameObject)Instantiate(DamageText);
        Txt.transform.SetParent(Canvas.transform);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasTrans, TargetScreenPos, null, out Vector2 EndPos);
        Txt.transform.localPosition = EndPos + new Vector2(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-15f, 15f));
        DamageTextScript TText = Txt.GetComponent<DamageTextScript>();
        switch(effective){
            case ElementEffective.Normal:
                TText.text.text = ($"{Damage}");
                break;
            case ElementEffective.Effective:
                TText.text.text = ($"{Damage}!!");
                break;
            case ElementEffective.Weakness:
                TText.text.text = ($"{Damage}...");
                break;
        }
    }

    public void ShowFinishingDamageText(int Damage, Transform Trans)
    {
        Vector3 TargetScreenPos = MainCam.WorldToScreenPoint(Trans.position);
        GameObject Txt = (GameObject)Instantiate(FinishingDamageText);
        Txt.transform.SetParent(Canvas.transform);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasTrans, TargetScreenPos, null, out Vector2 EndPos);
        Txt.transform.localPosition = EndPos + new Vector2(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-15f, 15f));
        DamageTextScript TText = Txt.GetComponent<DamageTextScript>();
        TText.text.text = ($"{Damage}");
    }
    public void ShowDamageTextForPlayer(int Damage, Transform Trans,ElementEffective effective)
    {
        Vector3 TargetScreenPos = MainCam.WorldToScreenPoint(Trans.position);
        GameObject Txt = (GameObject)Instantiate(DamageTextPlayer);
        Txt.transform.SetParent(Canvas.transform);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasTrans, TargetScreenPos, null, out Vector2 EndPos);
        Txt.transform.localPosition = EndPos + new Vector2(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-15f, 15f));
        DamageTextScript TText = Txt.GetComponent<DamageTextScript>();
        switch(effective){
            case ElementEffective.Normal:
                TText.text.text = ($"{Damage}");
                break;
            case ElementEffective.Effective:
                TText.text.text = ($"{Damage}!!");
                break;
            case ElementEffective.Weakness:
                TText.text.text = ($"{Damage}...");
                break;
        }

        
        Icons.DOComplete();
        IconChara.rectTransform.DOComplete();
        IconChara.DOComplete();
        Icons.DOShakeAnchorPos(0.3f, 10, 10, 100);
        IconChara.rectTransform.DOShakeAnchorPos(0.3f, 10, 10, 100);
        IconChara.color = new Color(1, 0, 0, 1);
        IconChara.DOColor(new Color(1, 1, 1, 1), 0.3f).SetEase(Ease.InCubic);
    }
    public void ShowExpOrb(int Exp, Transform Trans)
    {
        Vector3 TargetScreenPos = MainCam.WorldToScreenPoint(Trans.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasTrans, TargetScreenPos, null, out Vector2 EndPos);
        int All = Exp;
        while (All >= 30)
        {
            All -= 30;
            GameObject Orb = (GameObject)Instantiate(ExpOrb[2]);
            Orb.transform.SetParent(Canvas.transform);
            Orb.transform.localPosition = EndPos + new Vector2(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-15f, 15f));
        }
        while (All >= 5)
        {
            All -= 5;
            GameObject Orb = (GameObject)Instantiate(ExpOrb[1]);
            Orb.transform.SetParent(Canvas.transform);
            Orb.transform.localPosition = EndPos + new Vector2(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-15f, 15f));
        }
        while (All > 0)
        {
            All--;
            GameObject Orb = (GameObject)Instantiate(ExpOrb[0]);
            Orb.transform.SetParent(Canvas.transform);
            Orb.transform.localPosition = EndPos + new Vector2(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-15f, 15f));
        }
    }

    public void ScreenToWorldInCameraCanvas(Transform Trans, RectTransform Target)
    {
        Vector3 TargetPos = MainCam.ScreenToWorldPoint(Trans.position);
        Target.localPosition = new Vector3(TargetPos.x, TargetPos.y, 1f);
    }


    public void TreasureOpenUI(Transform Trans, treasureScript treasure)
    {
        Vector3 TargetScreenPos = MainCam.WorldToScreenPoint(Trans.position);
        treasure.OpenUI.SetActive(true);
        GameObject ui = treasure.OpenUI;
        ui.transform.SetParent(Canvas.transform);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasTrans, TargetScreenPos, null, out Vector2 EndPos);
        ui.transform.localPosition = EndPos + new Vector2(0, 50);
    }

    //セッティング
    public void StartSetting()
    {
        if (_isSettingSceneActive || SceneManager.GetSceneByName("SettingScene").isLoaded) return;
        StartCoroutine(LoadSettingSceneCoroutine());
    }

    IEnumerator LoadSettingSceneCoroutine()
    {
        Time.timeScale = 0f;
        PlayerController.instance.StopController();
        _isSettingSceneActive = true;
        var sceneName = "SettingScene";
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => !SceneManager.GetSceneByName(sceneName).isLoaded);
        _isSettingSceneActive = false;
    }

    public void StartSkillNodeCoroutine()
    {
        StartCoroutine(SkillNodeCoroutine());
    }

    IEnumerator SkillNodeCoroutine()
    {
        PlayerController.instance.StopGravity();
        PlayerController.instance.transform.DOMove(GameObject.FindWithTag("SkillNode").transform.position, 1.5f).SetEase(Ease.OutQuad).OnComplete(() => PlayerController.instance.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, 0));
        var tweennn1 = DOTween.To(() => C3F.ShoulderOffset, p =>
        {
            C3F.ShoulderOffset = new Vector3(p.x, p.y, C3F.ShoulderOffset.z);
        }, new Vector3(-1f, 0.3f, 0), 0.75f).SetEase(Ease.OutQuad);
        var tweennn2 = DOTween.To(() => CRC.TargetOffset, p =>
        {
            CRC.TargetOffset = new Vector3(p.x, p.y, CRC.TargetOffset.z);
        }, new Vector3(-1f, 0.3f, 0), 0.75f).SetEase(Ease.OutQuad);
        var tween3 = DOTween.To(() => C3F.CameraDistance, p =>
        {
            C3F.CameraDistance = p;
        }, 1.5f, 0.75f).SetEase(Ease.OutQuad);
        InvisibleUIs();
        int selectNum = 0;
        SkillNodeInfos.root.SetActive(true);
        SkillNodeInfos.skillNodePanelRect.anchoredPosition = new Vector3(600, -200, 0);
        SkillNodeInfos.skillNodePanelRect.localScale = Vector3.zero;
        SkillNodeInfos.skillNodePanelRect.DOAnchorPos(new Vector3(300, -200, 0), 0.5f).SetEase(Ease.OutQuad);
        SkillNodeInfos.skillNodePanelRect.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutQuad);
        SkillNodeInfos.NeedOrbsRect.anchoredPosition = new Vector3(600, 300, 0);
        SkillNodeInfos.NeedOrbsRect.localScale = Vector3.zero;
        SkillNodeInfos.NeedOrbsRect.DOAnchorPos(new Vector3(400, 100, 0), 0.5f).SetEase(Ease.OutQuad);
        SkillNodeInfos.NeedOrbsRect.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutQuad);
        SkillNodeInfos.selectOrbsPanelRect.anchoredPosition = new Vector3(600, 300, 0);
        SkillNodeInfos.selectOrbsPanelRect.localScale = Vector3.zero;
        SkillNodeInfos.selectOrbsPanelRect.DOAnchorPos(new Vector3(650, 100, 0), 0.5f).SetEase(Ease.OutQuad);
        SkillNodeInfos.selectOrbsPanelRect.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutQuad);
        SkillNodeInfos.OperateText.SetActive(true);
        PlayerController.instance.PlayerAnim.SetBool("SkillNodeF", true);
        SkillNode targetSkillNode = null;
        SkillNode nextTargetNode = null;
        Tweener tw = null;
        OperateActions actions = new OperateActions();

        while (true)
        {
            ForBattleData.instance.ReturnCalcurateData();
            SkillNodeInfos.selectOrbs[selectNum].DOScale(new Vector3(1.5f, 1.5f, 1), 0.5f).SetEase(Ease.OutQuad);
            yield return null;
            foreach (var i in SkillNodeInfos.selectOrbImage) i.sprite = SkillNodeInfos.OrbSprites[selectNum];
            switch (selectNum)
            {
                case 0:
                    //0は緑
                    SkillNodeInfos.selectOrbText.text = "緑石レベル";
                    targetSkillNode = SkillTreeManager.Instance.NowGreenNode;
                    break;
                case 1:
                    //1は赤
                    SkillNodeInfos.selectOrbText.text = "赤石レベル";
                    targetSkillNode = SkillTreeManager.Instance.NowRedNode;
                    break;
                case 2:
                    //2は青
                    SkillNodeInfos.selectOrbText.text = "青石レベル";
                    targetSkillNode = SkillTreeManager.Instance.NowBlueNode;
                    break;
            }
            if (targetSkillNode.childNode)
            {
                nextTargetNode = targetSkillNode.childNode;
                SkillNodeInfos.nextNodeExplainText.text = nextTargetNode.ExplainText;
                SkillNodeInfos.nowLevelText.text = $"{SkillTreeManager.Instance.NodeCounts[selectNum]}";
                SkillNodeInfos.nextLevelText.text = $"{SkillTreeManager.Instance.NodeCounts[selectNum] + 1}";
                SkillNodeInfos.NeedOrbsText[0].text = $"{nextTargetNode.needOrbs.green}/{ForBattleData.instance.OrbPieces.green}";
                SkillNodeInfos.NeedOrbsText[1].text = $"{nextTargetNode.needOrbs.red}/{ForBattleData.instance.OrbPieces.red}";
                SkillNodeInfos.NeedOrbsText[2].text = $"{nextTargetNode.needOrbs.blue}/{ForBattleData.instance.OrbPieces.blue}";
                if (nextTargetNode.needOrbs.green <= ForBattleData.instance.OrbPieces.green) SkillNodeInfos.NeedOrbsText[0].color = Color.white;
                else SkillNodeInfos.NeedOrbsText[0].color = new Color(1, 0, 0, 0.3f);
                if (nextTargetNode.needOrbs.red <= ForBattleData.instance.OrbPieces.red) SkillNodeInfos.NeedOrbsText[1].color = Color.white;
                else SkillNodeInfos.NeedOrbsText[1].color = new Color(1, 0, 0, 0.3f);
                if (nextTargetNode.needOrbs.blue <= ForBattleData.instance.OrbPieces.blue) SkillNodeInfos.NeedOrbsText[2].color = Color.white;
                else SkillNodeInfos.NeedOrbsText[2].color = new Color(1, 0, 0, 0.3f);
            }
            else
            {
                nextTargetNode = null;
                SkillNodeInfos.nextNodeExplainText.text = "なし";
                SkillNodeInfos.nowLevelText.text = $"{SkillTreeManager.Instance.NodeCounts[selectNum]}";
                SkillNodeInfos.nextLevelText.text = $"{SkillTreeManager.Instance.NodeCounts[selectNum] + 1}";
                SkillNodeInfos.NeedOrbsText[0].text = $"0/{ForBattleData.instance.OrbPieces.green}";
                SkillNodeInfos.NeedOrbsText[1].text = $"0/{ForBattleData.instance.OrbPieces.red}";
                SkillNodeInfos.NeedOrbsText[2].text = $"0/{ForBattleData.instance.OrbPieces.blue}";
                SkillNodeInfos.NeedOrbsText[0].color = Color.white;
                SkillNodeInfos.NeedOrbsText[1].color = Color.white;
                SkillNodeInfos.NeedOrbsText[2].color = Color.white;
            }

            //設定画面用の操作を受け付ける
            
            actions.Enable();

            yield return new WaitUntil(() => actions.Player.Move.ReadValue<Vector2>().y == 1 || actions.Player.Move.ReadValue<Vector2>().y == -1
            || actions.Player.Jump.IsPressed() || actions.Player.Act.IsPressed());

            //決定したとき
            if (actions.Player.Jump.IsPressed())
            {
                if (nextTargetNode != null)
                {
                    if (checkOrb(nextTargetNode.needOrbs, ForBattleData.instance.OrbPieces))
                    {
                        SkillNodeInfos.ImageAnim.SetTrigger("EmitT");
                        SkillNodeInfos.TextAnim.SetTrigger("EmitT");
                        SkillTreeManager.Instance.AddNode(nextTargetNode);
                        switch (selectNum)
                        {
                            case 0:
                                //0は緑
                                ForBattleData.instance.OrbPieces.green -= nextTargetNode.needOrbs.green;
                                ForBattleData.instance.OrbPieces.red -= nextTargetNode.needOrbs.red;
                                ForBattleData.instance.OrbPieces.blue -= nextTargetNode.needOrbs.blue;
                                SkillTreeManager.Instance.NowGreenNode = nextTargetNode;
                                SkillNodeInfos.LevelUpImage.color = new Color(0.465f, 0.910f, 0.410f, 0.75f);
                                break;
                            case 1:
                                //1は赤
                                ForBattleData.instance.OrbPieces.green -= nextTargetNode.needOrbs.green;
                                ForBattleData.instance.OrbPieces.red -= nextTargetNode.needOrbs.red;
                                ForBattleData.instance.OrbPieces.blue -= nextTargetNode.needOrbs.blue;
                                SkillTreeManager.Instance.NowRedNode = nextTargetNode;
                                SkillNodeInfos.LevelUpImage.color = new Color(0.890f, 0.410f, 0.410f, 0.75f);
                                break;
                            case 2:
                                //2は青
                                ForBattleData.instance.OrbPieces.green -= nextTargetNode.needOrbs.green;
                                ForBattleData.instance.OrbPieces.red -= nextTargetNode.needOrbs.red;
                                ForBattleData.instance.OrbPieces.blue -= nextTargetNode.needOrbs.blue;
                                SkillTreeManager.Instance.NowBlueNode = nextTargetNode;
                                SkillNodeInfos.LevelUpImage.color = new Color(0.05f, 0.890f, 1f, 0.75f);
                                break;
                        }
                        SkillTreeManager.Instance.NodeCounts[selectNum]++;
                        yield return new WaitForSeconds(1.0f);
                    }
                    else
                    {
                        if (tw != null) if (tw.active) tw.Complete();
                        var rect = SkillNodeInfos.root.GetComponent<RectTransform>();
                        tw = rect.DOShakeAnchorPos(0.5f);
                    }
                }
                else
                {
                    var rect = SkillNodeInfos.root.GetComponent<RectTransform>();
                    rect.DOShakeAnchorPos(0.5f);
                }

                yield return new WaitUntil(() => !actions.Player.Jump.IsPressed());
            }
            if (actions.Player.Move.ReadValue<Vector2>().y == -1 && selectNum < 2)
            {
                SkillNodeInfos.selectOrbs[selectNum].DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutQuad);
                selectNum++;
                yield return new WaitUntil(() => actions.Player.Move.ReadValue<Vector2>().y == 0);
            }

            if (actions.Player.Move.ReadValue<Vector2>().y == 1 && selectNum > 0)
            {
                SkillNodeInfos.selectOrbs[selectNum].DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutQuad);
                selectNum--;
                yield return new WaitUntil(() => actions.Player.Move.ReadValue<Vector2>().y == 0);
            }

            if (actions.Player.Act.IsPressed())
            {
                break;
            }
        }

        //キャンセル時
        if (tweennn1.active) tweennn1.Kill();
        if (tweennn2.active) tweennn2.Kill();
        if (tween3.active) tween3.Kill();
        tweennn1 = DOTween.To(() => C3F.ShoulderOffset, p =>
        {
            C3F.ShoulderOffset = new Vector3(p.x, p.y, C3F.ShoulderOffset.z);
        }, new Vector3(0, 1.5f, 0), 0.75f).SetEase(Ease.OutQuad);
        tweennn2 = DOTween.To(() => CRC.TargetOffset, p =>
        {
            CRC.TargetOffset = new Vector3(p.x, p.y, CRC.TargetOffset.z);
        }, new Vector3(0, 0, 0), 0.75f).SetEase(Ease.OutQuad);
        tween3 = DOTween.To(() => C3F.CameraDistance, p =>
        {
            C3F.CameraDistance = p;
        }, 5, 0.75f).SetEase(Ease.OutQuad);
        actions.Dispose();
        SkillNodeInfos.OperateText.SetActive(false);
        SkillNodeInfos.skillNodePanelRect.DOAnchorPos(new Vector3(600, -200, 0), 0.1f).SetEase(Ease.OutQuad);
        SkillNodeInfos.skillNodePanelRect.DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutQuad);
        SkillNodeInfos.NeedOrbsRect.DOAnchorPos(new Vector3(600, 300, 0), 0.1f).SetEase(Ease.OutQuad);
        SkillNodeInfos.NeedOrbsRect.DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutQuad);
        SkillNodeInfos.selectOrbsPanelRect.DOAnchorPos(new Vector3(600, 300, 0), 0.1f).SetEase(Ease.OutQuad);
        SkillNodeInfos.selectOrbsPanelRect.DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutQuad);
        PlayerController.instance.PlayerAnim.SetBool("SkillNodeF", false);
        VisibleUIs();
        PlayerController.instance.StartGravity();
        PlayerController.instance.StartController();
    }

    bool checkOrb(OrbClass left, OrbClass right)
    {
        if (left.green <= right.green && left.red <= right.red && left.blue <= right.blue) return true;
        return false;
    }

    public void ShowFinishingButton()
    {
        _finishingPanel.gameObject.SetActive(true);
        _finishingPanel.color = new Color(1, 1, 1, 0);
        _finishingPanel.DOFade(0.1f, 0.25f);
        AudioManager.instance.PlaySE(_finishingSound);
    }

    public void CloseFinishingButton()
    {
        _finishingPanel.gameObject.SetActive(false);
        _finishingPanel.DOFade(0f, 0.25f);
    }

    public void SetSpecialGauge(float value, bool isMax)
    {
        _specialGauge.fillAmount = value;
        _specialOperateImage.SetActive(isMax);
    }

    public void SetSpecialAnimTrigger(string st2)
    {
        AudioManager.instance.PlaySE(_specialAudio);
        _specialPanelAnim.SetTrigger(st2);
    }
}

[System.Serializable]
public class BossClass{
    
    public TextMeshProUGUI BossNameText;
    public TextMeshProUGUI BossSecondNameText;
    public TextMeshProUGUI BossHPNameText;
    public TextMeshProUGUI BossHPSecondNameText;
    public Slider HPBar;
    public Image BackGround;
    public Image Fill;
}

[System.Serializable]
public class LevelingClass
{
    public int levelNum;
    public bool isBoss;
    public GameObject BossEnemy;
}

public class SkillNodeClass
{
    public GameObject root;
    public Sprite[] OrbSprites = new Sprite[3];
    public RectTransform skillNodePanelRect;
    public Image[] selectOrbImage = new Image[2]; //選択とデカイやつ
    public TextMeshProUGUI selectOrbText;
    public TextMeshProUGUI nextNodeExplainText;
    public TextMeshProUGUI nowLevelText;
    public TextMeshProUGUI nextLevelText;
    public RectTransform NeedOrbsRect;
    public TextMeshProUGUI[] NeedOrbsText = new TextMeshProUGUI[3];
    public RectTransform selectOrbsPanelRect;
    public RectTransform[] selectOrbs = new RectTransform[3];
    public Image LevelUpImage;
    public Animator ImageAnim;
    public Animator TextAnim;
    public GameObject OperateText;
}