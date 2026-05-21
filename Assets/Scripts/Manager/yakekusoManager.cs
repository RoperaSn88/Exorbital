using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.IO;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class yakekusoManager : MonoBehaviour
{
    public Image Panel;
    public RectTransform[] Texts;
    [SerializeField] TextMeshProUGUI _topText;
    [SerializeField] TextMeshProUGUI _versionText;
    [SerializeField] TextMeshProUGUI _controllText;
    [TextArea, SerializeField] string[] controlTexts = new string[2];

    public RectTransform Icon;
    [SerializeField] GameObject _attentionPanel;
    [SerializeField] RectTransform _attentionWaku;

    public SceneObject[] Scenes;

    public AudioSource BGMsource;
    public AudioSource SEsource;
    int num = 0;
    
    
    bool a;
    [SerializeField]string _filePath;
    [SerializeField]string _fileName = "PlayerData";
    bool hasData = false;
    private OperateActions _gameActions;

    private void Awake()
    {
        staticScript._dataNumber = 1;
        _fileName = _fileName + staticScript._dataNumber.ToString() + ".json";
        _filePath = Application.dataPath + "/" + _fileName;
        if (!File.Exists(_filePath))
        {
            _topText.text = "Start Game";
            hasData = false;
            staticScript._hasData = false;
            staticScript.SetTutorial(true);
        }
        else
        {
            _topText.text = "Load Game";
            hasData = true;
            staticScript._hasData = true;
            staticScript.SetTutorial(false);
        }
        

        // セーブなし↓
        _topText.text = "Start Game";
        hasData = false;
        staticScript._hasData = false;
        staticScript.SetTutorial(true);

        _gameActions = new OperateActions();
        _gameActions.Title.Move.started += OnMove;
        _gameActions.Title.Move.performed += OnMove;
        _gameActions.Title.Move.canceled += OnMove;
        _gameActions.Title.Decide.started += OnDecidePush;
        _gameActions.Title.Decide.canceled += OnDecideRelease;
        _gameActions.Title.Cancel.started += OnCancelPush;
        _gameActions.Title.Cancel.canceled += OnCancelRelease;

        _gameActions.Enable();
    }
    // Start is called before the first frame update
    void Start()
    {
        _versionText.text = "ver" + Application.version;
        Panel.color = new Color(0, 0, 0, 1f);
        Panel.DOFade(0f, 1.5f);
        StartCoroutine(SelectingGame());
        //Texta.GetComponent<TextMeshProUGUI>().DOFade(0f, 3f);
    }

    void ChangeText(int num)
    {
        _controllText.text = controlTexts[num];
    }

    // Update is called once per frame
    void Update()
    {
        if (StoppingBGM)
        {
            if (StopTime < EndTime)
            {
                StopTime += Time.deltaTime;
                BGMsource.volume = firstvol - 0.2f * StopTime / EndTime;
            }
            else
            {
                StoppingBGM = false;
                EndTime = 0;
                StopTime = 0;
            }
        }
        if (_gameActions.DistinguishOperator.KeyBoard.IsPressed())
        {
            ChangeText(0);
        }

        if (_gameActions.DistinguishOperator.XBox.IsPressed())
        {
            ChangeText(1);
        }

        if (_gameActions.DistinguishOperator.PlayStation.IsPressed())
        {
            ChangeText(2);
        }
    }
    
    PlayerSaveData LoadData()
    {
        if (!File.Exists(_filePath)) return null;
        StreamReader rd = new StreamReader(_filePath);
        string json = rd.ReadToEnd();
        rd.Close();

        return JsonUtility.FromJson<PlayerSaveData>(json);
    }
    public void StopBGM(float time)
    {
        //DOTweenではうまくいかないのでUpdateで対処
        EndTime = time;
        StopTime = 0;
        StoppingBGM = true;
        firstvol = BGMsource.volume;
    }
    float firstvol=0;

    bool StoppingBGM=false;
    float StopTime=0;
    bool _resetCoroutine = false;
    float EndTime;
    float _selectValue = 0;
    bool _selected = false;
    bool _canceled = false;

    IEnumerator SelectingGame() {
        while (true)
        {
            yield return new WaitUntil(()=>_selectValue == 1 || _selectValue == -1 || _selected || _canceled);
            if (_selected || _canceled)
            {
                if (_canceled && num == 0 && hasData)
                {
                    //新しくゲームを始まる警告
                    _attentionPanel.SetActive(true);
                    _attentionWaku.localPosition = new Vector3(0, -120, 0);
                    int attentionNum = 1;

                    Tweener tw = null;
                    yield return new WaitUntil(()=>!_selected && !_canceled);
                    while (true)
                    {
                        yield return new WaitUntil(() => _selectValue == 1 || _selectValue == -1 || _selected || _canceled);
                        if (_selectValue == 1)
                        {
                            if (attentionNum == 1)
                            {
                                attentionNum = 0;
                                if (tw != null) if (tw.active) tw.Kill();
                                tw = _attentionWaku.DOAnchorPos(new Vector3(0, -90, 0), 0.5f).SetEase(Ease.OutQuad);
                            }
                        }
                        if (_selectValue == -1)
                        {
                            if (attentionNum == 0)
                            {
                                attentionNum = 1;
                                if (tw != null) if (tw.active) tw.Kill();
                                tw = _attentionWaku.DOAnchorPos(new Vector3(0, -120, 0), 0.5f).SetEase(Ease.OutQuad);
                            }
                        }

                        if (attentionNum == 0 && _selected)
                        {
                            //初期化
                            hasData = false;
                            _attentionPanel.SetActive(false);
                            staticScript._hasData = false;
                            staticScript.SetTutorial(true);
                            Panel.DOFade(1f, 2f);
                            SEsource.Play();
                            _gameActions.Dispose();
                            StopBGM(2f);
                            yield return new WaitForSeconds(2f);
                            SceneManager.LoadScene("LouglikeScene");
                            break;
                        }
                        else if ((attentionNum == 1 && _selected) || _canceled)
                        {
                            _attentionPanel.SetActive(false);
                            _resetCoroutine = true;
                            break;
                        }
                    }


                }

                if (_resetCoroutine)
                {
                    _resetCoroutine = false;
                    continue;
                }

                if (num == 1)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
                    Application.Quit();//ゲームプレイ終了
#endif
                }
                //breakでゲームスタート
                //numの値によってゲームスタート
                if (_selected)
                {
                    Panel.DOFade(1f, 2f);
                    SEsource.Play();
                    StopBGM(2f);
                    _gameActions.Dispose();
                    yield return new WaitForSeconds(2f);
                    if (num == 0)
                    {
                        if (!staticScript.checkTutorial()) SceneManager.LoadScene(Scenes[num]);
                        else SceneManager.LoadScene("LouglikeScene");
                    }
                    else if(num == 1)SceneManager.LoadScene(Scenes[num]);
                    
                    break;
                }

            }
            else if (_selectValue == 1)
            {
                //上にずらせ
                if (num != 0) num--;
            }
            else if (_selectValue == -1)
            {
                //下にずらせ
                if (num != Texts.Length - 1) num++;
            }
            Icon.DOKill();
            Icon.DOAnchorPos(Texts[num].localPosition + new Vector3(145, 0), 0.25f).SetEase(Ease.OutQuad);
            yield return null;
            if (_selectValue != 0) yield return new WaitUntil(() => _selectValue == 0);
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _selectValue = context.ReadValue<Vector2>().y;
    }

    private void OnDecidePush(InputAction.CallbackContext context)
    {
        _selected = true;
    }

    private void OnDecideRelease(InputAction.CallbackContext context)
    {
        _selected = false;
    }

    private void OnCancelPush(InputAction.CallbackContext context)
    {
        _canceled = true;
    }
    private void OnCancelRelease(InputAction.CallbackContext context)
    {
        _canceled = false;
    }

}
