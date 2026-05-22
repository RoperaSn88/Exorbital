using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.VFX;
using System.Threading.Tasks;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using Manager.SelectElement;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public bool ControlF;
    bool isAttackingF;
    [SerializeField] float WalkSpeed;
    [SerializeField] float DashSpeed;
    [SerializeField] float FirstGravity;
    [SerializeField] float AddGravityPower;
    [SerializeField] int JumpCount;
    [SerializeField] Rigidbody RB;
    [SerializeField] bool ShiftBool;
    public Animator PlayerAnim;
    public SpriteRenderer spriteRenderer;
    [SerializeField] PlayerFloorDetector floorDetector;
    [SerializeField] GameObject ColliderPos;
    
    bool JudgeNextAttack;
    bool CanEnterNextAttack;
    [NonSerialized] public bool isRight;
    [SerializeField]bool Grounded;
    bool staired;
    bool StaireToAir;
    [SerializeField] bool JumpingF;
    bool LPushed;
    [SerializeField] bool Air;
    [SerializeField] int JumpC;
    [SerializeField]float jumpTimer;
    
    int GroundLayer;
    int StairLayer;
    public int SkillSelectNumber;
    public GameObject Effect;
    public Transform SpawnPos;
    public SkillData Data;
    public bool SpeedF;
    public bool BuffF;
    public GameObject Light;
    public Vector3 LightPos;
    [NonSerialized]public SkillData SelectedData;
    [NonSerialized]public Transform AllAttackCollider;
    
    //宝箱の情報を管理
    [SerializeField] treasureScript treasure;
    [SerializeField]AudioClip thinkingSound;
    public bool DefendF;
    public bool rollingF;
    [SerializeField] GameObject FinishingEffect;
    [SerializeField] AudioClip nowFinishingSound;

    [SerializeField] List<FinallyCalcuratedBattleData> _canDefeatEnemys = new List<FinallyCalcuratedBattleData>();
    const float _specialTechMaxTime = 180f;
    public float _specialTechCoolTime = 0;
    [SerializeField] GameObject Hissatu1;
    //PayerControllerの_gameActionsインスタンス1つだけで操作の入力は制御させる
    private OperateActions _gameActions;
    private CancellationTokenSource _elementTimerCancellationTokenSource;

    private async void Start()
    {
        instance = this;
        await ActionActivate();
        isRight = true;
        GroundLayer = LayerMask.GetMask("Block");
        StairLayer = LayerMask.GetMask("Stair");
        LPushed = true;
        Air = true;
        SkillSelectNumber = 0;
        AllAttackCollider = transform.Find("AttackColliders");
        _elementTimerCancellationTokenSource = new CancellationTokenSource();
        StartElementTimer(_elementTimerCancellationTokenSource.Token).Forget();
    }

    async UniTask ActionActivate()
    {
        _gameActions = new OperateActions();
        _gameActions.Player.Move.started += OnMove;
        _gameActions.Player.Move.performed += OnMove;
        _gameActions.Player.Move.canceled += OnMove;
        _gameActions.Player.Act.started += OnAct;
        _gameActions.Player.CameraRotate.started += OnRotate;
        _gameActions.Player.CameraRotate.performed += OnRotate;
        _gameActions.Player.CameraRotate.canceled += OnRotate;
        _gameActions.Player.Defend.started += OnDefend;
        _gameActions.Player.Defend.canceled += OnDefendRelease;
        _gameActions.Player.ActLevelUp.started += OnLevelUp;
        _gameActions.Player.Skill.started += OnSkill;
        _gameActions.Player.Special.started += OnSpecialing;
        _gameActions.Player.Jump.started += OnJump;
        _gameActions.Player.Attack.started += OnAttack;
        _gameActions.Player.Finishing.started += OnFinishing;
        // _gameActions.Player.SelectElement.started += StartSelectElement; 属性はいったん無効にする
        _gameActions.Player.Option.started += OnOption;
        _gameActions.DistinguishOperator.KeyBoard.started += OnKeyboard;
        _gameActions.DistinguishOperator.XBox.started += OnGamePad;
        _gameActions.DistinguishOperator.PlayStation.started += OnPlayStation;
        await UniTask.WaitUntil(() => SkillPanelScript.instance);
        await SkillPanelScript.instance.ActionActivate(_gameActions);
        await SceneManagerScript.instance.ActionActivate(_gameActions);
        // await ElementPresenter.instance.ActionActivate(_gameActions);

        _gameActions.Enable();
    }

    float _rotateValue;
    bool _rotateF;
    bool rollCheck = false;
    Vector2 moveVec;

    async UniTask StartElementTimer(CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                switch (ForBattleData.instance.Element)
                {
                    case ElementKinds.Normal:
                        await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
                        break;
                    case ElementKinds.Fire:
                        if(ForBattleData.instance.OrbPieces.red > 0)
                        {
                            ForBattleData.instance.OrbPieces.red--;
                            SceneManagerScript.instance.SetOrbPieceAmount(1, ForBattleData.instance.OrbPieces.red);
                            await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: cancellationToken);
                        }
                        else
                        {
                            ForBattleData.instance.Element = ElementKinds.Normal;
                            SceneManagerScript.instance.SetElementImage(ForBattleData.instance.Element);
                            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
                        }
                        break;
                    case ElementKinds.Water:
                        if(ForBattleData.instance.OrbPieces.blue > 0)
                        {
                            ForBattleData.instance.OrbPieces.blue--;
                            SceneManagerScript.instance.SetOrbPieceAmount(2, ForBattleData.instance.OrbPieces.blue);
                            await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: cancellationToken);
                        }
                        else
                        {
                            ForBattleData.instance.Element = ElementKinds.Normal;
                            SceneManagerScript.instance.SetElementImage(ForBattleData.instance.Element);
                            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
                        }
                        break;
                    case ElementKinds.Leaf:
                        if(ForBattleData.instance.OrbPieces.green > 0)
                        {
                            ForBattleData.instance.OrbPieces.green--;
                            SceneManagerScript.instance.SetOrbPieceAmount(0, ForBattleData.instance.OrbPieces.green);
                            await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: cancellationToken);
                        }
                        else
                        {
                            ForBattleData.instance.Element = ElementKinds.Normal;
                            SceneManagerScript.instance.SetElementImage(ForBattleData.instance.Element);
                            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancellationToken);
                        }
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void OnDestroy()
    {
        _elementTimerCancellationTokenSource?.Cancel();
        _elementTimerCancellationTokenSource?.Dispose();
    }

    void OnKeyboard(InputAction.CallbackContext context)
    {
        SceneManagerScript.instance._changerController(0);
    }

    void OnGamePad(InputAction.CallbackContext context)
    {
        SceneManagerScript.instance._changerController(1);
    }
    void OnPlayStation(InputAction.CallbackContext context)
    {
        SceneManagerScript.instance._changerController(2);
    }
    void OnRotate(InputAction.CallbackContext context)
    {
        _rotateValue = context.ReadValue<Vector2>().x * 0.6f;
        _rotateF = _rotateValue != 0;
    }

    void OnMove(InputAction.CallbackContext context)
    {
        if (ControlF)
        {
            moveVec = context.ReadValue<Vector2>();
            
            var HorizontalVec = moveVec.x * transform.right;
            var VerticalVec = moveVec.y * transform.forward;
            if (DefendF && !rollCheck && SkillPanelScript.instance.isFinishSelectSkill)
            {
                //防御中に入力が起きたら回避を行う
                if (moveVec.magnitude >= 0.8f)
                {
                    if (moveVec.x > 0)
                    {
                        spriteRenderer.flipX = false;
                        ColliderPos.transform.rotation = Quaternion.Euler(0, 0 + transform.rotation.eulerAngles.y, 0);
                        isRight = true;
                    }
                    else if (moveVec.x < 0)
                    {
                        spriteRenderer.flipX = true;
                        ColliderPos.transform.rotation = Quaternion.Euler(0, 180 + transform.rotation.eulerAngles.y, 0);
                        isRight = false;
                    }
                    rollCheck = true;
                    PlayerAnim.SetBool("DefendF", false);
                    PlayerAnim.SetBool("RollF", true);
                    ControlF = false;
                    var MoveVec = new Vector3((HorizontalVec.x + VerticalVec.x), 0, (HorizontalVec.z + VerticalVec.z)).normalized;
                    RB.constraints = RigidbodyConstraints.FreezeRotation;
                    RB.linearVelocity = MoveVec * 10;
                    DOTween.To(() => 1f, t =>
                    {
                        RB.linearVelocity = MoveVec * 10 * t;
                    }, 0f, 0.8f);
                }
            }
        }
    }

    public void StartSelectElement(InputAction.CallbackContext context)
    {
        OnSelectElement();
    }

    public async UniTask OnSelectElement()
    {
        if (!ElementPresenter.instance.IsActivate)
        {
            StopController();
            Time.timeScale = 0;
            await SceneManagerScript.instance.StartElement();
            Time.timeScale = 1;
            StartController();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
    }
    
    public void AddSpeed(int value)
    {
        DashSpeed = 5 + value;
        WalkSpeed = 3 + (value / 2);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (ControlF)
        {
            if (JumpC <= JumpCount)
            {
                LPushed = true;
                JumpC++;
                if (JumpC > 1)
                {
                    float a = FirstGravity * 1.5f;
                    RB.linearVelocity = new Vector3(RB.linearVelocity.x, a / JumpC, RB.linearVelocity.z);
                }
                else
                {
                    RB.linearVelocity = new Vector3(RB.linearVelocity.x, FirstGravity / JumpC, RB.linearVelocity.z);
                    jumpTimer = 3;
                }
            }
        }
    }


    

    void velocityTOZero()
    {
        Debug.Log("Set zero");
        moveVec = Vector3.zero;
        RB.linearVelocity = new Vector3(0, RB.linearVelocity.y, 0);
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (isAttackingF && CanEnterNextAttack)
        {
            Debug.Log("MoveNextAttack");
            JudgeNextAttack = true;
            PlayerAnim.SetBool("DontMoveAttack1F", true);
            PlayerAnim.SetBool("DashF", false);
            PlayerAnim.SetBool("WalkF", false);
        }
        if (ControlF &&!(PlayerAnim.GetBool("JumpUpF") || PlayerAnim.GetBool("JumpDownF")||DefendF))
        {
            PlayerAnim.SetBool("AttackF", true);
            isAttackingF = true;
            ControlF = false;
        }
    }
    
    void OnSkill(InputAction.CallbackContext context)
    {
        if (ControlF && !DefendF && SkillPanelScript.instance.isFinishSelectSkill)
        {
            ControlF = false;
            StartCoroutine(SelectSpecificAttack());
        }
    }

    void OnOption(InputAction.CallbackContext context)
    {
        if (ControlF)
        {
            SceneManagerScript.instance.StartSetting();
            _gameActions.Disable();
        }
    }

    void OnDefend(InputAction.CallbackContext context)
    {
        if(!(PlayerAnim.GetBool("JumpUpF") || PlayerAnim.GetBool("JumpDownF")))
        {
            DefendF = true;
            velocityTOZero();
        }
        
    }

    void OnDefendRelease(InputAction.CallbackContext context)
    {
        DefendF = false;
    }

    void OnLevelUp(InputAction.CallbackContext context)
    {
        if (ControlF && SceneManagerScript.instance.CanLevelUp)
        {
            SceneManagerScript.instance.StartCoroutine("LevelUpCoroutine");
            ControlF = false;
        }
    }

    void OnSpecialing(InputAction.CallbackContext context)
    {
        if (ControlF && _specialTechCoolTime == _specialTechMaxTime)
        {
            StartCoroutine(SpecialCoroutine(1));
            _specialTechCoolTime = 0;
        }
    }
    void OnAct(InputAction.CallbackContext context)
    {
        if (treasure != null)
        {
            if (!treasure.isOpen && Input.GetButtonDown("CircleButton"))
            {
                treasure.OpenTreasure();
                treasure = null;
            }
        }
    }

    void OnFinishing(InputAction.CallbackContext context)
    {
        if (_finishingFlug) isFinishingPress = true;
    }

    private void Update()
    {
        if (isAttackingF && CanEnterNextAttack)
        {
            RB.linearVelocity = Vector3.zero;
        }

        Grounded = Physics.Linecast(transform.position, transform.position - new Vector3(0, 0.85f, 0), GroundLayer) || Physics.Linecast(transform.position, transform.position - new Vector3(0, 1f, 0), StairLayer);
        if (Grounded) PlayerAnim.SetBool("JumpDownF", false);
        //階段から空中にいるフラグを、階段設置時に立てる
        staired = Physics.Linecast(transform.position, transform.position - new Vector3(0, 1f, 0), StairLayer);
        if (staired && !StaireToAir) StaireToAir = true;
        if (!staired && StaireToAir)
        {
            RB.linearVelocity = new Vector3(RB.linearVelocity.x, 0, RB.linearVelocity.z);
            StaireToAir = false;
        }

        if (SceneManagerScript.instance.isGameOver)
        {
            ControlF = false;
            velocityTOZero();
        }

        if (_rotateF && (ControlF || !SkillPanelScript.instance.isFinishSelectSkill))
        {
            transform.eulerAngles += new Vector3(0, 180 * Time.unscaledDeltaTime, 0) * _rotateValue;
        }

        if (ControlF)
        {

            if (DefendF)
            {
                PlayerAnim.SetBool("DefendF", true);
            }
            else
            {
                PlayerAnim.SetBool("DefendF", false);
            }

            var HorizontalVec = moveVec.x * transform.right;
            var VerticalVec = moveVec.y * transform.forward;

            float MovePower = moveVec.magnitude;
            if (MovePower >= 0.9f) ShiftBool = true;
            else ShiftBool = false;
            if (MovePower == 0 && staired && (DefendF||!rollCheck)) RB.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            else if (MovePower == 0) RB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            else RB.constraints = RigidbodyConstraints.FreezeRotation;

            if (!Grounded) //�󒆎�
            {
                if (ShiftBool)
                {
                    RB.linearVelocity = new Vector3((HorizontalVec.x + VerticalVec.x) * DashSpeed, RB.linearVelocity.y, (HorizontalVec.z + VerticalVec.z) * DashSpeed);
                    if (SpeedF) RB.linearVelocity = new Vector3(RB.linearVelocity.x * 1.5f, RB.linearVelocity.y, RB.linearVelocity.z * 1.5f);
                }
                else
                {
                    RB.linearVelocity = new Vector3((HorizontalVec.x + VerticalVec.x) * WalkSpeed, RB.linearVelocity.y, (HorizontalVec.z + VerticalVec.z) * WalkSpeed);
                    if (SpeedF) RB.linearVelocity = new Vector3(RB.linearVelocity.x * 1.5f, RB.linearVelocity.y, RB.linearVelocity.z * 1.5f);
                }

                
            }
            else if (ShiftBool)
            {
                if (!DefendF)
                {
                    RB.linearVelocity = new Vector3((HorizontalVec.x + VerticalVec.x) * DashSpeed, RB.linearVelocity.y, (HorizontalVec.z + VerticalVec.z) * DashSpeed);
                    if (SpeedF) RB.linearVelocity = new Vector3(RB.linearVelocity.x * 1.5f, RB.linearVelocity.y, RB.linearVelocity.z * 1.5f);
                }
                
                if (MovePower == 0)
                {
                    PlayerAnim.SetBool("DashF", false);
                    PlayerAnim.SetBool("WalkF", false);

                }
                else if (MovePower < 0.9f)
                {
                    PlayerAnim.SetBool("DashF", false);
                    PlayerAnim.SetBool("WalkF", true);
                }
                else
                {
                    PlayerAnim.SetBool("DashF", true);
                    PlayerAnim.SetBool("WalkF", false);
                }
            }
            else
            {
                if (Air)//�n�ʂɂ�����
                {
                    Air = false;
                    LPushed = false;
                    JumpC = 0;
                    PlayerAnim.SetBool("JumpUpF", false);
                    PlayerAnim.SetBool("JumpDownF", false);
                }

                if (!DefendF)
                {
                    RB.linearVelocity = new Vector3((HorizontalVec.x + VerticalVec.x) * WalkSpeed, RB.linearVelocity.y, (HorizontalVec.z + VerticalVec.z) * WalkSpeed);
                    if (SpeedF) RB.linearVelocity = new Vector3(RB.linearVelocity.x * 1.5f, RB.linearVelocity.y, RB.linearVelocity.z * 1.5f);
                }
                
                if (MovePower == 0)
                {
                    PlayerAnim.SetBool("DashF", false);
                    PlayerAnim.SetBool("WalkF", false);
                }
                else if (MovePower < 0.9f)
                {
                    PlayerAnim.SetBool("DashF", false);
                    PlayerAnim.SetBool("WalkF", true);
                }
                else
                {
                    PlayerAnim.SetBool("DashF", true);
                    PlayerAnim.SetBool("WalkF", false);
                }
            }

            if (moveVec.x > 0)
            {
                spriteRenderer.flipX = false;
                ColliderPos.transform.rotation = Quaternion.Euler(0, 0 + transform.rotation.eulerAngles.y, 0);
                isRight = true;
            }
            else if (moveVec.x < 0)
            {
                spriteRenderer.flipX = true;
                ColliderPos.transform.rotation = Quaternion.Euler(0, 180 + transform.rotation.eulerAngles.y, 0);
                isRight = false;
            }
            
            if (!Grounded)
            {
                if (LPushed) Air = true;

                if (RB.linearVelocity.y >= 0 && JumpC != 0)
                {
                    PlayerAnim.SetBool("JumpUpF", true);
                    PlayerAnim.SetBool("JumpDownF", false);
                }
                else if (RB.linearVelocity.y < 0)
                {
                    PlayerAnim.SetBool("JumpUpF", false);
                    PlayerAnim.SetBool("JumpDownF", true);
                }
            }
            else if (Air)
            {
                Air = false;
                LPushed = false;
                JumpC = 0;
                PlayerAnim.SetBool("JumpUpF", false);
                PlayerAnim.SetBool("JumpDownF", false);
            }

            if (LPushed && jumpTimer > 0)
            {
                jumpTimer -= Time.deltaTime;
                if (jumpTimer <= 0)
                {
                    LPushed = false;
                    jumpTimer = 3;
                    Air=true;
                }
            }
        }


        //�o�t�n��
        if (SpeedF)
        {
            SpeedT -= Time.deltaTime;
            if (SpeedT < 0)
            {
                SpeedF = false;
                SpeedT = 0;
            }
        }

        if (BuffF)
        {
            BuffT -= Time.deltaTime;
            if (BuffT < 0)
            {
                BuffF = false;
                BuffT = 0;
            }
        }


        if (transform.position.y < -20)
        {
            transform.position = SceneManagerScript.instance.ReSpawnVec;
            //SceneManagerScript.instance.ResetMapPos();
        }

        if (_finishingFlug)
        {
            _finishingTimer += Time.unscaledDeltaTime;
        }

        if (_specialTechMaxTime > _specialTechCoolTime)
        {
            _specialTechCoolTime += Time.deltaTime;
            SceneManagerScript.instance.SetSpecialGauge(_specialTechCoolTime / _specialTechMaxTime, _specialTechCoolTime >= _specialTechMaxTime);
        }
        if (_specialTechMaxTime < _specialTechCoolTime)
        {
            _specialTechCoolTime = _specialTechMaxTime;
            SceneManagerScript.instance.SetSpecialGauge(_specialTechCoolTime / _specialTechMaxTime,_specialTechCoolTime >= _specialTechMaxTime);
        }



    }

    float SpeedT;
    public void SetRollF(int f)
    {
        rollingF = f == 0 ? false : true;
        if (f == 1) PlayerAnim.SetBool("RollingF", true);
    }
    public void FinishRolling()
    {
        Debug.Log("Finishroll");
        PlayerAnim.SetBool("RollF",false);
        PlayerAnim.SetBool("RollingF", false);
        rollCheck = false;
        if(!SceneManagerScript.instance.LevelSelecting)StartController();
    }
    public float _finishingTimer;
    [NonSerialized]public bool _finishingFlug;
    public void AddDefeatEnemy(FinallyCalcuratedBattleData enemy)
    {
        _canDefeatEnemys.Add(enemy);
        SceneManagerScript.instance.ShowFinishingButton();
        if (!_finishingFlug) StartCoroutine(FinishingCoroutine());
    }
    public void RemoveDefeatEnemy(FinallyCalcuratedBattleData enemy)
    {
        _canDefeatEnemys.Remove(enemy);
    }

    bool isFinishingPress = false;
    IEnumerator FinishingCoroutine()
    {

        _finishingFlug = true;
        Time.timeScale = 0.25f;
        yield return new WaitUntil(() => isFinishingPress || _finishingTimer >= 2.5f);
        if (isFinishingPress)
        {
            StopController();
            Vector3 vector = _canDefeatEnemys[0].gameObject.transform.position - transform.position;
            transform.position += vector + vector.normalized * 1.2f + Vector3.up;
            foreach (var e in _canDefeatEnemys)
            {
                int damage = Mathf.FloorToInt(1 + e.MaxHP / 100) * 100;
                e.Damage(damage);
                SceneManagerScript.instance.ShowFinishingDamageText(damage, e.gameObject.transform);
                StartCoroutine(EffectEmitVFX(FinishingEffect, e.transform.position));
            }
            PlayerAnim.SetBool("FinishingF", true);
            AudioManager.instance.PlaySE(nowFinishingSound);
            yield return new WaitForSecondsRealtime(0.8f);//
            PlayerAnim.SetBool("FinishingF", false);
            _canDefeatEnemys.RemoveAt(0);
            PlayerAnim.SetBool("AttackF", false);
            PlayerAnim.SetBool("DontMoveAttack1F", false);
            StartController();
        }
        _canDefeatEnemys.Clear();
        SceneManagerScript.instance.CloseFinishingButton();
        Time.timeScale = 1;
        _finishingTimer = 0;
        _finishingFlug = false;
        isFinishingPress = false;
        EndAttack();
    }

    public List<FinallyCalcuratedBattleData> GetFinishingEnemys()
    {
        return _canDefeatEnemys;
    }

    public void KillEffect(int i)
    {
        GameObject obj = GameObject.FindWithTag($"Skill{i}");
        if (obj)
        {
            Destroy(obj);
        }

    }
    
    public void StartSpeed()
    {
        SpeedF = true;
        var system=GameObject.FindWithTag($"Skill2").GetComponent<ParticleSystem>();
        var main=system.main;
        main.duration=Data.Timer;
        system.Play();
        foreach(SkillData daata in ForBattleData.instance.Skills)
        {
            if (daata.ID == 2)
            {
                SpeedT = daata.Timer;
                break;
            }
        }
    }


    float BuffT;
    public void StartBuff()
    {
        BuffF = true;
        var system=GameObject.FindWithTag($"Skill3").GetComponent<ParticleSystem>();
        var main=system.main;
        main.duration=Data.Timer;
        system.Play();
        foreach (SkillData daata in ForBattleData.instance.Skills)
        {
            if (daata.ID == 3)
            {
                BuffT = daata.Timer;
                break;
            }
        }
    }

    IEnumerator SpecialCoroutine(int i)
    {
        StopController();
        PlayerAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.5f);
        spriteRenderer.enabled = false;
        //今回は必殺技1つのみ実装する
        SceneManagerScript.instance.SetSpecialAnimTrigger("Hissatu1T");
        yield return new WaitForSecondsRealtime(1.08f);
        spriteRenderer.enabled = true;
        PlayerAnim.SetBool("Hissatu1F", true);
    }

    public async void EndHissatuAct()
    {
        PlayerAnim.SetBool("Hissatu1F", false);
        Time.timeScale = 1;
        //攻撃判定、エフェクトを表示
        PlayerAnim.updateMode = AnimatorUpdateMode.Normal;
        StartController();
        Hissatu1.SetActive(true);
        await Task.Delay(TimeSpan.FromSeconds(2f));
        Hissatu1.SetActive(false);
        Debug.Log("おわり");
    }

    public IEnumerator SelectSpecificAttack()
    {
        PlayerAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        Time.timeScale = 0;
        //Selecting = true;
        PlayerAnim.SetBool("ThinkingF", true);

        //SceneManagerScript.instance.C3F.CameraDistance = 2f;
        DOVirtual.Float(5f, 3f, 0.5f, value => SceneManagerScript.instance.C3F.CameraDistance = value).SetEase(Ease.OutCubic).SetUpdate(true);
        SceneManagerScript.instance.Icons.DOComplete();
        SceneManagerScript.instance.Icons.DOScale(new Vector3(0.5f, 0.5f, 0), 0.5f).SetEase(Ease.OutCubic).SetUpdate(true);
        SceneManagerScript.instance.Icons.DOAnchorPos(new Vector3(40, 42, 0), 0.5f).SetEase(Ease.OutCubic).SetUpdate(true);
        if (isRight)
        {
            SceneManagerScript.instance.C3F.ShoulderOffset = new Vector3(1.5f, 0.5f, 0);
            SceneManagerScript.instance.CRC.TargetOffset = new Vector3(1.5f, 0, 0);
        }
        else
        {
            SceneManagerScript.instance.C3F.ShoulderOffset = new Vector3(-1.5f, 0.5f, 0);
            SceneManagerScript.instance.CRC.TargetOffset = new Vector3(-1.5f, 0, 0);
        }

        Data = ForBattleData.instance.Skills[SkillSelectNumber];
        //スキル選択を開始する
        SkillPanelScript.instance.StartSetText(SkillSelectNumber);
        Effect = Data.EffectObject;
        SpawnPos = Data.SpawnPos;
        AudioManager.instance.PlaySE(thinkingSound);
        SkillPanelScript.instance.isFinishSelectSkill = false;
        yield return null;
        RB.linearVelocity = new Vector3(0, 0, 0);
        yield return new WaitUntil(() => SkillPanelScript.instance.isFinishSelectSkill == true);

        Debug.Log("FinishSelect");

        //PlayerAnim.SetBool("ThinkingF", false);
        DOVirtual.Float(3f, 5f, 0.5f, value => SceneManagerScript.instance.C3F.CameraDistance = value).SetEase(Ease.OutCubic).SetUpdate(true);
        SceneManagerScript.instance.C3F.ShoulderOffset = new Vector3(0, staticScript.CameraPos, 0);
        SceneManagerScript.instance.CRC.TargetOffset = new Vector3(0, 0, 0);
        SceneManagerScript.instance.Icons.DOComplete();
        SceneManagerScript.instance.Icons.DOScale(new Vector3(1f, 1f, 0), 0.5f).SetEase(Ease.OutCubic).SetUpdate(true);
        SceneManagerScript.instance.Icons.DOAnchorPos(new Vector3(80, 85, 0), 0.5f).SetEase(Ease.OutCubic).SetUpdate(true);
        Time.timeScale = 1f;
        PlayerAnim.updateMode = AnimatorUpdateMode.Normal;
        velocityTOZero();
        //Selecting = false;

    }

    public void healing()
    {
        ForBattleData.instance.healing(SelectedData.HealPoint);
        SelectedData = null;
        Data = null;
    }

    public void SpawnEffect()
    {
        Transform Pos = Instantiate(Effect, SpawnPos).transform;
        Pos.SetParent(null);
    }

    public void UseSkill()
    {
        SelectedData.UseSkill();
    }
    public void SpawnEffectParent()
    {
        GameObject Trans = Instantiate(Effect, SpawnPos);
        Trans.transform.SetParent(gameObject.transform);
        //Trans.GetComponent<ParticleSystem>().main.duration = Data.Timer;
    }
    public void StopController()
    {
        velocityTOZero();
        ControlF = false;
        PlayerAnim.SetBool("DashF",false);
        PlayerAnim.SetBool("WalkF",false);
        PlayerAnim.SetBool("JumpUpF",false);
        PlayerAnim.SetBool("JumpDownF",false);
    }

    public void StopGravity()
    {
        RB.useGravity = false;
    }
    public void StartGravity()
    {
        RB.useGravity = true;
    }

    public void StartController()
    {
        ControlF = true;
        _gameActions.Enable();
    }

    public void EntryNextAttack()  //���U����t�J�n�@�A�j���[�V�������ɂ�
    {
        CanEnterNextAttack = true;
    }
    public void EndAttack()  //�U���I����Ď��̍U���s�����I��邩�@�A�j����
    {
        if (!JudgeNextAttack)
        {
            PlayerAnim.SetBool("AttackF", false);
            isAttackingF = false;
            PlayerAnim.SetBool("DontMoveAttack1F", false);
            PlayerAnim.SetBool("DashF", false);
            PlayerAnim.SetBool("WalkF", false);
            moveVec = _gameActions.Player.Move.ReadValue<Vector2>();
            var HorizontalVec = moveVec.x * transform.right;
            var VerticalVec = moveVec.y * transform.forward;
            RB.linearVelocity = new Vector3((HorizontalVec.x + VerticalVec.x) * DashSpeed, RB.linearVelocity.y, (HorizontalVec.z + VerticalVec.z) * DashSpeed);
            if (!SceneManagerScript.instance.LevelSelecting) ControlF = true;
        }
        
        JudgeNextAttack = false;
        CanEnterNextAttack = false;
    }

    public void StartReviveCoroutine()
    {
        StartCoroutine("ReviveCoroutine");
    }

    IEnumerator ReviveCoroutine()
    {
        Debug.Log("REVIVE COROUTINE");
        PlayerAnim.SetBool("DeadF", false);
        PlayerAnim.SetBool("ReviveF",true);
        _specialTechCoolTime = 0;
        yield return new WaitForSeconds(1.4f);
        PlayerAnim.SetBool("ReviveF",false);
        StartController();
    }


    public void EndFinalAttack()
    {
        PlayerAnim.SetBool("AttackF", false);
        isAttackingF = false;
        if (!SceneManagerScript.instance.LevelSelecting) ControlF = true;
        Debug.Log("EndFinalAttack");
        PlayerAnim.SetBool("DontMoveAttack1F", false);
        moveVec = _gameActions.Player.Move.ReadValue<Vector2>();
        var HorizontalVec = moveVec.x * transform.right;
        var VerticalVec = moveVec.y * transform.forward;
        RB.linearVelocity = new Vector3((HorizontalVec.x + VerticalVec.x) * DashSpeed, RB.linearVelocity.y, (HorizontalVec.z + VerticalVec.z) * DashSpeed);
        JudgeNextAttack = false;
        CanEnterNextAttack = false;
        velocityTOZero();
    }

    public void EndSkillAttack()
    {
        PlayerAnim.SetBool("SkillF", false);
        PlayerAnim.SetBool("ThinkingF", false);
        PlayerAnim.SetBool("DashF", false);
        PlayerAnim.SetBool("WalkF", false);
        isAttackingF = false;
        moveVec = _gameActions.Player.Move.ReadValue<Vector2>();
        var HorizontalVec = moveVec.x * transform.right;
        var VerticalVec = moveVec.y * transform.forward;
        RB.linearVelocity = new Vector3((HorizontalVec.x + VerticalVec.x) * DashSpeed, RB.linearVelocity.y, (HorizontalVec.z + VerticalVec.z) * DashSpeed);
        if (!SceneManagerScript.instance.LevelSelecting) ControlF = true;
        JudgeNextAttack = false;
        CanEnterNextAttack = false;
        SelectedData = null;
        Data = null;
        velocityTOZero();
    }

    IEnumerator StageEndCoroutine(){
        ControlF=false;
        BossBool = false;
        Time.timeScale=0.5f;
        yield return new WaitForSeconds(0.25f);
        SceneManagerScript.instance.StartStageEndCoroutine();
        PlayerAnim.SetBool("DashF", false);
        PlayerAnim.SetBool("WalkF", false);
        PlayerAnim.SetBool("JumpUpF", false);
        PlayerAnim.SetBool("JumpDownF", false);
        Time.timeScale=1f;
        spriteRenderer.DOFade(0f,0.5f);
        CheckEnd=false;
    }

    public void MovedStage(){
        transform.rotation = Quaternion.Euler(0, 0,0);
        spriteRenderer.color=new Color(1,1,1,1);
    }

    public void SetDeadAnimBool(){
        PlayerAnim.StopPlayback();
    }



    void EffectEmit(GameObject Effect)
    {
        GameObject spawnEffect = Instantiate(Effect, transform);
        spawnEffect.transform.SetParent(null);
        Vector3 rotateVec = transform.rotation.eulerAngles;
        if (!isRight) rotateVec += new Vector3(0, 180, 0);
        spawnEffect.transform.rotation = Quaternion.Euler(rotateVec);
    }

    IEnumerator EffectEmitVFX(GameObject vfx,Vector3 enemyPos)
    {
        VisualEffect visualEffect = Instantiate(vfx, Vector3.zero,Quaternion.identity).GetComponent<VisualEffect>();
        visualEffect.SetVector3("BasePos", enemyPos);
        visualEffect.SendEvent("OnPlay");
        float a = visualEffect.GetFloat("lifeTime");
        yield return new WaitForSeconds(a);
        Destroy(visualEffect.gameObject);
    }


    bool CheckEnd=false;
    bool BossBool=false;
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("EndArea") && !CheckEnd)
        {
            CheckEnd = true;
            StartCoroutine(StageEndCoroutine());
        }

        if (col.CompareTag("MoveStage"))
        {
            SceneManagerScript.instance.StartSceneMoveCoroutine();
        }

        if (col.CompareTag("Finish"))
        {
            SceneManagerScript.instance.StartFinishSceneCoroutine();
        }

        if (col.CompareTag("Treasure"))
        {
            treasure = col.gameObject.GetComponent<treasureScript>();
        }
        if (col.CompareTag("bossArea") && !BossBool)
        {
            BossBool = true;
            SceneManagerScript.instance.StartBossPerform();
        }

        if (col.CompareTag("SkillNode"))
        {
            StopController();
            SceneManagerScript.instance.StartSkillNodeCoroutine();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.CompareTag("Treasure")){
            treasure=null;
        }
    }
}
