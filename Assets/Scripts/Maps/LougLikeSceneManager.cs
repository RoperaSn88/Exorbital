using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;

public class LougLikeSceneManager : MonoBehaviour
{
    public static LougLikeSceneManager instance;
    Camera MainCam;
    public CinemachineCamera CVC;
    public CinemachineThirdPersonFollow C3F;
    public CinemachineRotationComposer CC;
    public GameObject Canvas;
    public GameObject CameraCanvas;
    RectTransform CanvasTrans;
    [SerializeField] GameObject DamageText;
    [SerializeField] GameObject DamageTextPlayer;
    [SerializeField] Slider HPSlider;
    [SerializeField] Slider MPSlider;
    [SerializeField] TextMeshProUGUI HPText;
    [SerializeField] TextMeshProUGUI MPText;
    [SerializeField] Slider ExpSlider;
    public RectTransform Icons;
    [SerializeField] Image IconChara;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] GameObject[] ExpOrb;
    public List<SkillData> AllSkills;
    [SerializeField] SkillTextMasterData _skillTexts;
    public SkillPanelScript SkillPanel;
    [SerializeField] Transform SkillGainParent;
    public SkillGainPanelScript SkillGainPanelS;
    [SerializeField] RectTransform SkillGainSelecter;
    ForBattleData PlayerData;
    [NonSerialized] public bool Selecting;
    [NonSerialized] public int LevelCount;
    [SerializeField] GameObject LevelUpEffect;
    public bool EnemyControllF;
    public int GenerateSize;
    [SerializeField] Image FadePanel;
    [SerializeField] GameObject FinishPanel;
    [SerializeField] TextMeshProUGUI TimeText;
    [SerializeField] TextMeshProUGUI LevelText;
    [SerializeField] TextMeshProUGUI KillText;
    bool timerF;
    float LowSTimer;
    int sTimer;
    int mtimer;
    [NonSerialized] public int kills;
    string filePath;
    public int maxEnemyCount;
    [NonSerialized]public bool Finishing;
    Volume StageVolume;
    [SerializeField] TextMeshProUGUI MoneyText;
    //Generator用
    MapGeneraterVer2 b;

    private void Awake()
    {
        FadePanel.gameObject.SetActive(true);
        FadePanel.color = new Color(0, 0, 0, 1f);
        FadePanel.DOFade(0,0.5f);
        instance = this;
        EnemyControllF = true;
        MainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        CanvasTrans=Canvas.GetComponent<RectTransform>();
        
    }
    public void StartStage(){
        FadePanel.DOFade(0f, 0.75f);
        PlayerData = PlayerController.instance.GetComponent<ForBattleData>();
        //CVC.enabled = true;
        timerF=true;
        SetCameraBackGround();
        PlayerController.instance.ControlF=true;
    }

    public void AddMoney(int amount){
        ForBattleData.instance.money+=amount;
        DOTween.To(()=>0,(m)=>{
            MoneyText.text=(ForBattleData.instance.money+m).ToString("D6");
        },amount,0.5f).SetEase(Ease.InQuad);
    }

    public void StartStageEndCoroutine(){
        StartCoroutine(StageEndCoroutine());
    }
    IEnumerator StageEndCoroutine(){
        //ステージ→休憩所
        TimeText.gameObject.SetActive(true);
        KillText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        TimeText.text = $"Time : {mtimer}:{sTimer}";
        TimeText.rectTransform.DOAnchorPosX(-210f,1f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.2f);
        KillText.text = $"Kills : {kills}/{maxEnemyCount}";
        KillText.rectTransform.DOAnchorPosX(-210f, 1f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1.5f);
        FadePanel.gameObject.SetActive(true);
        FadePanel.color = new Color(0, 0, 0, 0f);
        FadePanel.DOFade(1f, 0.5f);
        yield return null;
        //次のステージを取得しておく
        SceneLoader.Instance.targetScene=b.NextScene;
        //CVC.enabled = false;
        yield return new WaitForSeconds(0.5f);
        SceneLoader.Instance.SceneUnLoad(b.thisScene);
        SceneLoader.Instance.isBreaking=true;
        SceneLoader.Instance.SceneLoad();
        yield return null;
        PlayerController.instance.MovedStage();
        yield return new WaitForSeconds(0.5f);
        SetCameraBackGround();
        TimerReset();
        //CVC.enabled = true;
        FadePanel.DOFade(0f, 0.75f);
        PlayerController.instance.ControlF=true;
    }

    void SetCameraBackGround(){
        GameObject aaa=GameObject.FindWithTag("Generator");
        Color c;
        if(aaa!=null){
            b=aaa.GetComponent<MapGeneraterVer2>();
            c=b.color;
        }
        else{
            c=new Color(0.986f,1,0);
        }
        StageVolume=GameObject.FindWithTag("Volume").GetComponent<Volume>();
        StageVolume.profile.TryGet<Bloom>(out Bloom bl);
        bl.tint.value=c;
        MainCam.backgroundColor=c;
        RenderSettings.fogColor=c;
    }

    public void StartSceneMoveCoroutine(){
        StartCoroutine(SceneMoveCoroutine());
    }

    IEnumerator SceneMoveCoroutine(){
        //休憩所→ステージ
        Finishing=true;
        PlayerController.instance.ControlF=false;
        FadePanel.gameObject.SetActive(true);
        FadePanel.color = new Color(0, 0, 0, 0f);
        FadePanel.DOFade(1f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        //CVC.enabled = false;
        yield return null;
        //isBreakingなのでどの道結果は変わらん
        SceneLoader.Instance.SceneUnLoad(SceneLoader.Instance.targetScene);
        SceneLoader.Instance.isBreaking=false;
        SceneLoader.Instance.SceneLoad();
        TimerReset();
        PlayerController.instance.MovedStage();
    }

    [NonSerialized]public bool isGameOver;
    public void StartGameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            StartCoroutine(GameOver());
        }
        
    }
    public IEnumerator GameOver()
    {
        //���̃Q�[���I�[�o�[�̓��@���T�o�V�[���̂�A��قǏC���K�v
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        //���ԁA���x���Akill���\��
        FinishPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        TimeText.text = $"Time : {mtimer}:{sTimer}";
        TimeText.rectTransform.DOAnchorPosX(0, 1f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1.5f);
        LevelText.rectTransform.DOAnchorPosX(0, 1f).SetEase(Ease.OutBack);
        LevelText.text = $"Level : {ForBattleData.instance.Level}";
        yield return new WaitForSeconds(1.5f);
        KillText.text = $"Kills : {kills}";
        KillText.rectTransform.DOAnchorPosX(0, 1f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(5f);
        
        //�����L���O�\��,�ۑ�
        //�t�F�[�h�A�E�g
        FadePanel.DOFade(1f, 1.5f);
        yield return new WaitForSeconds(1.5f);

    }

    void TimerReset(){
        sTimer=0;
        LowSTimer=0;
        mtimer=0;
        TimeText.rectTransform.anchoredPosition =new Vector3(200,40,0);
        KillText.rectTransform.anchoredPosition =new Vector3(200,100,0);
    }

    private void Update()
    {
        if(timerF){
            if (!Selecting && !isGameOver) LowSTimer += Time.deltaTime;
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
    }

    public void SetSlider(int Value,int MaxValue,int Num)
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


    public IEnumerator LevelUpCoroutine()
    {
        Selecting = true;
        
        List<SkillData> SelectedSkills = new List<SkillData>();
        List<GameObject> PanelObject = new List<GameObject>();
        SkillGainParent.gameObject.SetActive(true);
        LevelCount=1;
        SkillGainSelecter.gameObject.SetActive(true);
        while (LevelCount > 0)
        {
            ForBattleData.instance.LevelUp();
            Debug.Log("LEVEL UP!!!");
            int count = 3;
            int SelectNumber = 1;
            List<SkillData> CopyAllSkills = new List<SkillData>();
            foreach (SkillData data in AllSkills) CopyAllSkills.Add(GenerateSkill(data));
            while (count > 0)
            {
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
                foreach(SkillData hasData in ForBattleData.instance.Skills)
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
                SkillGainPanel.SkillMP.text = $"MP:{data.NeedMP}";
            }
            PlayerController.instance.StopController();
            EnemyControllF = false;
            SkillGainSelecter.anchoredPosition = new Vector3(0, -5, 0);
            Instantiate(LevelUpEffect, Canvas.transform);
            yield return null;
            yield return new WaitForSeconds(0.3f);
            //�I�Ԓi�K
            
            while (true)
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.J)
                || Input.GetButtonDown("CircleButton") || Input.GetAxis("HorizontalButton") == -1 || Input.GetAxis("HorizontalButton") == 1
                || Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1);
                if ((Input.GetKeyDown(KeyCode.A)||Input.GetAxis("HorizontalButton")==-1 || Input.GetAxisRaw("Horizontal") == -1) && SelectNumber != 0)
                {
                    //�X�L���I��
                    SelectNumber--;
                    SkillGainSelecter.DOAnchorPosX(300 * (SelectNumber - 1), 0.25f).SetEase(Ease.OutCubic);
                    yield return new WaitUntil(() => Input.GetAxisRaw("HorizontalButton") == 0);
                    yield return new WaitUntil(() => Input.GetAxisRaw("Horizontal") == 0);
                }
                if ((Input.GetKeyDown(KeyCode.D) || Input.GetAxis("HorizontalButton") == 1 || Input.GetAxisRaw("Horizontal") == 1) && SelectNumber != 2)
                {
                    SelectNumber++;
                    SkillGainSelecter.DOAnchorPosX(300 * (SelectNumber - 1), 0.25f).SetEase(Ease.OutCubic);
                    yield return new WaitUntil(() => Input.GetAxisRaw("HorizontalButton") == 0);
                    yield return new WaitUntil(() => Input.GetAxisRaw("Horizontal") == 0);
                }
                if (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("CircleButton"))
                {
                    ForBattleData.instance.GainSkill(SelectedSkills[SelectNumber]);
                    if (ForBattleData.instance.Level % 2 == 0) GenerateSize++;
                    LevelCount--;
                    foreach (GameObject obj in PanelObject) Destroy(obj);
                    SelectedSkills.Clear();
                    break;
                }
            }
        }
        //ボス出現を出す

        SkillGainSelecter.gameObject.SetActive(false);
        SkillGainParent.gameObject.SetActive(false);
        PlayerController.instance.Data = null;
        PlayerController.instance.StartController();
        EnemyControllF = true;
        Selecting = false;
    }

    public void SetExpSlider(int value,int MaxValue)
    {
        ExpSlider.maxValue = MaxValue;
        ExpSlider.value = value;
    }

    public void AddExpSlider(int value)
    {
        ExpSlider.DOComplete();
        ExpSlider.DOValue(ExpSlider.value + value, 0.2f).SetEase(Ease.InCubic)
            .OnComplete(()=>PlayerData.StartAddPlayerExp(value));
    }

    public SkillData GenerateSkill(SkillData BaseSkill)
    {
        SkillData skill = new SkillData();

        skill.NameTextID = BaseSkill.NameTextID;
        skill.IntroductTextID = BaseSkill.IntroductTextID;
        skill.Name = _skillTexts ? _skillTexts.GetText(BaseSkill.NameTextID, BaseSkill.Name) : BaseSkill.Name;
        skill.Introduct = _skillTexts ? _skillTexts.GetText(BaseSkill.IntroductTextID, BaseSkill.Introduct) : BaseSkill.Introduct;
        skill.NeedMP = BaseSkill.NeedMP;
        skill.ID = BaseSkill.ID;
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
    public void ShowDamageText(int Damage,Transform Trans)
    {
        Vector3 TargetScreenPos = MainCam.WorldToScreenPoint(Trans.position);
        GameObject Txt = (GameObject)Instantiate(DamageText);
        Txt.transform.SetParent(Canvas.transform);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasTrans, TargetScreenPos,null,out Vector2 EndPos);
        Txt.transform.localPosition= EndPos + new Vector2(UnityEngine.Random.Range(-15f,15f), UnityEngine.Random.Range(-15f, 15f));
        DamageTextScript TText=Txt.GetComponent<DamageTextScript>();
        TText.text.text = ($"{Damage}");
    }
    public void ShowDamageTextForPlayer(int Damage, Transform Trans)
    {
        Vector3 TargetScreenPos = MainCam.WorldToScreenPoint(Trans.position);
        GameObject Txt = (GameObject)Instantiate(DamageTextPlayer);
        Txt.transform.SetParent(Canvas.transform);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasTrans, TargetScreenPos, null, out Vector2 EndPos);
        Txt.transform.localPosition = EndPos + new Vector2(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-15f, 15f));
        DamageTextScript TText = Txt.GetComponent<DamageTextScript>();
        TText.text.text = ($"{Damage}");

        //���łɃA�C�R���̏������s��
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
}
