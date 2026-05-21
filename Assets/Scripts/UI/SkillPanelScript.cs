using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

public class SkillPanelScript : MonoBehaviour
{
    public static SkillPanelScript instance;
    public bool isFinishSelectSkill;
    RectTransform Rect;
    [SerializeField] Transform SkillList;
    [SerializeField] GameObject TextObject;
    [SerializeField] RectTransform SkillSelecter;
    [SerializeField] TextMeshProUGUI SkillName;
    [SerializeField] TextMeshProUGUI SkillInstruct;
    [SerializeField] TextMeshProUGUI SkillMP;
    [SerializeField] TextMeshProUGUI[] SkillOrbsText;
    [SerializeField] TextMeshProUGUI PageText;
    List<GameObject> Texts;

    private void Start()
    {
        instance = this;
        Texts = new List<GameObject>();
        isFinishSelectSkill = true;
    }

    public void StartSetText(int num)
    {
        StartCoroutine(SetTexts(num));
    }

    async public UniTask ActionActivate(OperateActions gameActions)
    {
        gameActions.SelectSkill.SelectH.started += OnSelectH;
        gameActions.SelectSkill.SelectH.performed += OnSelectH;
        gameActions.SelectSkill.SelectH.canceled += OnSelectH;
        gameActions.SelectSkill.SelectV.started += OnSelectV;
        gameActions.SelectSkill.SelectV.performed += OnSelectV;
        gameActions.SelectSkill.SelectV.canceled += OnSelectV;
        gameActions.SelectSkill.Act.started += OnDecide;
        gameActions.SelectSkill.Close.started += OnCancel;
    }

    public void SetExplainText(string Name, string Instruct, OrbClass orbs)
    {
        SkillName.text = Name;
        SkillInstruct.text = Instruct;
        SkillOrbsText[0].text = ($"{orbs.green}/{ForBattleData.instance.OrbPieces.green}");
        SkillOrbsText[1].text = ($"{orbs.red}/{ForBattleData.instance.OrbPieces.red}");
        SkillOrbsText[2].text = ($"{orbs.blue}/{ForBattleData.instance.OrbPieces.blue}");
        if (orbs.green <= ForBattleData.instance.OrbPieces.green) SkillOrbsText[0].color = new Color(1, 1, 1, 1);
        else SkillOrbsText[0].color = new Color(1, 0, 0, 0.3f);
        if (orbs.red <= ForBattleData.instance.OrbPieces.red) SkillOrbsText[1].color = new Color(1, 1, 1, 1);
        else SkillOrbsText[1].color = new Color(1, 0, 0, 0.3f);
        if (orbs.blue <= ForBattleData.instance.OrbPieces.blue) SkillOrbsText[2].color = new Color(1, 1, 1, 1);
        else SkillOrbsText[2].color = new Color(1, 0, 0, 0.3f);
    }

    bool _onDecideF = false;
    bool _onCancelF = false;
    float _verticalAxis = 0;
    float _horizontalAxis = 0;

    public void OnDecide(InputAction.CallbackContext context)
    {
        if(!isFinishSelectSkill)_onDecideF = true;
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!isFinishSelectSkill)
        {
            _onCancelF = true;
            //Debug.Log("cancelFchanged: " + _onCancelF);
        }
    }

    public void OnSelectV(InputAction.CallbackContext context)
    {
        if(!isFinishSelectSkill)_verticalAxis = context.ReadValue<float>();
    }

    public void OnSelectH(InputAction.CallbackContext context)
    {
        if(!isFinishSelectSkill)_horizontalAxis = context.ReadValue<float>();
    }


    public IEnumerator SetTexts(int SavedNumber)
    {
        isFinishSelectSkill = false;
        int Num = SavedNumber;
        int PageCount = 1;
        //0~3�Ԗڂ����炱��ł���
        while (Num > 3)
        {
            Num -= 4;
            PageCount++;
        }
        int MaxPageCount = (int)Mathf.Floor(((ForBattleData.instance.Skills.Count - 1) / 4) + 1);

        Rect = GetComponent<RectTransform>();
        Rect.localScale = new Vector3(0, 0, 1);
        Rect.DOScale(new Vector3(1, 1, 1), 0.3f).SetEase(Ease.OutCubic).SetUpdate(true);
        //yield return StartCoroutine(CreateText(Num,PageCount));
        CreateText(Num, PageCount);
        bool Finish = false;
        
        while (!Finish)
        {
            PageText.text = $"{PageCount}/{(ForBattleData.instance.Skills.Count - 1) / 4 + 1}";

            yield return null;
            _onCancelF = false;
            _onDecideF = false;
            yield return new WaitUntil(() => _verticalAxis==1 || _verticalAxis == -1 ||_onCancelF || _onDecideF || _horizontalAxis==1 || _horizontalAxis == -1);
            if (_onCancelF)
            {
                //閉じる
                Debug.Log($"Number{Num + (PageCount - 1) * 4}");
                Rect.DOComplete();
                Rect.DOScale(new Vector3(0, 0, 1), 0.25f).SetEase(Ease.OutCubic);
                PlayerController.instance.SkillSelectNumber = Num + (PageCount - 1) * 4;
                PlayerController.instance.Data = null;
                PlayerController.instance.ControlF = true;
                PlayerController.instance.PlayerAnim.SetBool("ThinkingF", false);
                Debug.Log("Closed");
                _onCancelF = false;
                Finish = true;
            }
            else if (_onDecideF)
            {
                if (CheckOrbAmount())
                {
                    Rect.DOComplete();
                    Debug.Log($"Number{Num + (PageCount - 1) * 4}");
                    Rect.DOScale(new Vector3(0, 0, 1), 0.25f).SetEase(Ease.OutCubic);
                    PlayerController.instance.SkillSelectNumber = Num + (PageCount - 1) * 4;
                    //スキルの位置を取得する。localでな
                    if (PlayerController.instance.AllAttackCollider.Find($"Skill{PlayerController.instance.Data.ID}")) PlayerController.instance.SpawnPos = PlayerController.instance.AllAttackCollider.Find($"Skill{PlayerController.instance.Data.ID}").transform;
                    else PlayerController.instance.SpawnPos = PlayerController.instance.AllAttackCollider.Find($"non").transform;
                    PlayerController.instance.Effect = PlayerController.instance.Data.EffectObject;
                    //PlayerController.instance.SpawnPos = PlayerController.instance.Data.SpawnPos;
                    PlayerController.instance.PlayerAnim.SetBool("SkillF", true);
                    PlayerController.instance.PlayerAnim.SetTrigger($"Skill{PlayerController.instance.Data.ID}");
                    PlayerController.instance.SelectedData = PlayerController.instance.Data;
                    //Orb
                    ForBattleData.instance.OrbPieces.green -= PlayerController.instance.Data.NeedOrbs.green;
                    ForBattleData.instance.OrbPieces.red -= PlayerController.instance.Data.NeedOrbs.red;
                    ForBattleData.instance.OrbPieces.blue -= PlayerController.instance.Data.NeedOrbs.blue;
                    SceneManagerScript.instance.SetOrbPieceAmount(0, ForBattleData.instance.OrbPieces.green);
                    SceneManagerScript.instance.SetOrbPieceAmount(1, ForBattleData.instance.OrbPieces.red);
                    SceneManagerScript.instance.SetOrbPieceAmount(2, ForBattleData.instance.OrbPieces.blue);
                    _onDecideF = true;
                    Finish = true;
                }
                else
                {
                    Rect.DOComplete();
                    Rect.DOShakePosition(0.4f, 25).SetUpdate(true);
                }
            }
            else if (_verticalAxis==1)
            {
                SkillSelecter.DOComplete();
                Num -= 1;
                if (Num == -1) Num = SkillList.childCount - 1;
                SkillSelecter.DOAnchorPos(new Vector3(-12, 44 - (Num * 40), 0), 0.1f).SetEase(Ease.OutCubic).SetUpdate(true);
                SetExplainText(ForBattleData.instance.Skills[Num + (PageCount - 1) * 4].Name, ForBattleData.instance.Skills[Num + (PageCount - 1) * 4].Introduct, ForBattleData.instance.Skills[Num + (PageCount - 1) * 4].NeedOrbs);
                PlayerController.instance.Data = ForBattleData.instance.Skills[Num + (PageCount - 1) * 4];
                yield return new WaitUntil(() => _verticalAxis == 0);

            }
            else if (_verticalAxis==-1)
            {
                SkillSelecter.DOComplete();
                Num += 1;
                if (Num == SkillList.childCount) Num = 0;
                SkillSelecter.DOAnchorPos(new Vector3(-12, 44 - (Num * 40), 0), 0.1f).SetEase(Ease.OutCubic).SetUpdate(true);
                SetExplainText(ForBattleData.instance.Skills[Num + (PageCount - 1) * 4].Name, ForBattleData.instance.Skills[Num + (PageCount - 1) * 4].Introduct, ForBattleData.instance.Skills[Num + (PageCount - 1) * 4].NeedOrbs);
                PlayerController.instance.Data = ForBattleData.instance.Skills[Num + (PageCount - 1) * 4];
                yield return new WaitUntil(() => _verticalAxis == 0);

            }
            else if (_horizontalAxis == -1)
            {
                Debug.Log("A Pushed");
                PageCount -= 1;
                if (PageCount == 0) PageCount = MaxPageCount;
                Debug.Log($"PageCount:{PageCount}");
                while (ForBattleData.instance.Skills.Count - 1 < Num + 4 * (PageCount - 1)) Num--;
                //yield return StartCoroutine(CreateText(Num, PageCount));
                CreateText(Num, PageCount);
                SetExplainText(ForBattleData.instance.Skills[Num + (PageCount - 1) * 4].Name, ForBattleData.instance.Skills[Num + (PageCount - 1) * 4].Introduct, ForBattleData.instance.Skills[Num + (PageCount - 1) * 4].NeedOrbs);
                PlayerController.instance.Data = ForBattleData.instance.Skills[Num + (PageCount - 1) * 4];
                yield return new WaitUntil(() => _horizontalAxis == 0);

            }
            else if (_horizontalAxis == 1)
            {
                PageCount += 1;
                if (PageCount == MaxPageCount + 1) PageCount = 1;
                while (ForBattleData.instance.Skills.Count - 1 < Num + 4 * (PageCount - 1)) Num--;
                //yield return StartCoroutine(CreateText(Num, PageCount));
                CreateText(Num, PageCount);
                Debug.Log($"Explain:{Num + (PageCount - 1) * 4}");
                SetExplainText(ForBattleData.instance.Skills[Num + (PageCount - 1) * 4].Name, ForBattleData.instance.Skills[Num + (PageCount - 1) * 4].Introduct, ForBattleData.instance.Skills[Num + (PageCount - 1) * 4].NeedOrbs);
                PlayerController.instance.Data = ForBattleData.instance.Skills[Num + (PageCount - 1) * 4];
                yield return new WaitUntil(() => _horizontalAxis == 0);
            }
        }

        isFinishSelectSkill = true;
    }

    public bool CheckOrbAmount()
    {
        ForBattleData userData = ForBattleData.instance;
        SkillData skill = PlayerController.instance.Data;
        if (userData.OrbPieces.green >= skill.NeedOrbs.green && userData.OrbPieces.red >= skill.NeedOrbs.red && userData.OrbPieces.blue >= skill.NeedOrbs.blue) return true;
        return false;
    }

    public void CreateText(int Number, int page)
    {
        Debug.Log($"Number:{Number}");
        if (Texts.Count != 0)
        {
            foreach (GameObject obj in Texts)
            {
                Destroy(obj);
            }
        }
        int Count = 0;
        while (Count < 4)
        {
            if (ForBattleData.instance.Skills.Count <= Count + 4 * (page - 1))
            {
                Debug.Log($"Finish Count:{Count}");
                break;
            }
            TextMeshProUGUI CreatedTMP = Instantiate(TextObject).GetComponent<TextMeshProUGUI>();
            CreatedTMP.rectTransform.localScale = new Vector3(1, 1, 1);
            CreatedTMP.text = ForBattleData.instance.Skills[(page - 1) * 4 + Count].Name;
            Debug.Log($"Generated {ForBattleData.instance.Skills[(page - 1) * 4 + Count].Name}");
            CreatedTMP.transform.SetParent(SkillList, false);
            Texts.Add(CreatedTMP.gameObject);
            Count++;
        }
        //yield return null;
        int num = Number;

        Debug.Log($"num:{num}");
        Debug.Log($"{SkillList.GetChild(num).GetComponent<TextMeshProUGUI>().text}");
        RectTransform TextRect = SkillList.GetChild(num).GetComponent<RectTransform>();
        SkillSelecter.anchoredPosition = new Vector3(-12, 44 - (num * 40), 0);
        SetExplainText(ForBattleData.instance.Skills[Number].Name, ForBattleData.instance.Skills[Number].Introduct, ForBattleData.instance.Skills[Number].NeedOrbs);
    }
}
