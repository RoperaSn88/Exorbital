using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Manager.SelectElement.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Manager.SelectElement
{
    public class ElementPresenter : MonoBehaviour
    {
        public static ElementPresenter instance;

        [SerializeField]
        private ElementMasterData _elementMasterData;
        [SerializeField]
        private ElementView[] _elementViews = new ElementView[4];

        [SerializeField]
        private float _duration;

        private bool _movingF = false;

        /// <summary>
        /// 選択されているかどうか
        /// </summary>
        [SerializeField]
        private bool _activateF= false;
        public bool IsActivate => _activateF;

        private OperateActions _gameActions;
        private int maximum;

        [SerializeField]
        private PlayerElementView _playerElemntView;
        [SerializeField]
        private AudioClip _startSound;

        [SerializeField]
        private AudioClip _selectSound;
        [SerializeField]
        private AudioClip _decideSound;

        private void Awake()
        {
            instance = this;
            maximum = System.Enum.GetValues(typeof(ElementKinds)).Length;
            foreach(var v in _elementViews)
            {
                v.InVisible();
            }
            _playerElemntView.SetAlphaColor(0);
        }

        public async UniTask ActionActivate(OperateActions gameActions)
        {
            _gameActions = gameActions;
        }

        /// <summary>
        /// 属性選択を開始させる
        /// </summary>
        /// <param name="number">現在のElementKind</param>
        /// <returns></returns>
        public async UniTask<ElementKinds> StartElement(ElementKinds element)
        {
            // 0: 下 1: 隠し 2: 上 3: 中央 
            _activateF = true;
            _playerElemntView.SetAlphaColor(0);
            StartElement();
            AudioManager.instance.PlaySE(_startSound);
            await SetElementView((int)element);
            await SetElementMoveKind();
            await MoveUp((int)element);
            ElementKinds result = await SelectElement((int)element);
            _activateF = false;
            return result;
        }

        void StartElement()
        {
            _playerElemntView.StartMove(0.5f);
        }

        async UniTask SetElementMoveKind()
        {
            _elementViews[0].MoveKind = ElementUIMoveKinds.TopNull;
            _elementViews[1].MoveKind = ElementUIMoveKinds.TopNullToTopMini;
            _elementViews[2].MoveKind = ElementUIMoveKinds.TopMiniToMain;
            _elementViews[3].MoveKind = ElementUIMoveKinds.MainToBottomMini;
        }

        async UniTask SetElementView(int number)
        {
            _elementViews[0].SetElement(_elementMasterData.Elements[(number - 1) % maximum < 0 ? (number - 1 + maximum) % maximum : (number - 1) % maximum]);
            _elementViews[1].SetElement(_elementMasterData.Elements[(number) % maximum < 0 ? (number + maximum) % maximum : (number) % maximum]);
            _elementViews[2].SetElement(_elementMasterData.Elements[(number + 1) % maximum < 0 ? (number + 1 + maximum) % maximum : (number + 1) % maximum]);
            _elementViews[3].SetElement(_elementMasterData.Elements[(number + 2) % maximum < 0 ? (number + 2 + maximum) % maximum : (number + 2) % maximum]);
        }

        async UniTask<ElementKinds> SelectElement(int number)
        {
            int baseNumber = number;
            while (true)
            {
                await UniTask.WaitUntil(()=>_gameActions.Element.Move.ReadValue<float>() == 1f || _gameActions.Element.Move.ReadValue<float>() == -1f || _gameActions.Element.Decide.IsPressed() || _gameActions.Element.Cancel.IsPressed());
                if (_gameActions.Element.Move.ReadValue<float>() == 1f)
                {
                    number++;
                    if(number > maximum - 1) number = 0;
                    AudioManager.instance.PlaySE(_selectSound);
                    await MoveDown(number);
                } 
                else if (_gameActions.Element.Move.ReadValue<float>() == -1f)
                {
                    number--;
                    if(number < 0) number = maximum - 1;
                    AudioManager.instance.PlaySE(_selectSound);
                    await MoveUp(number);
                }
                else if (_gameActions.Element.Decide.IsPressed())
                {
                    ElementKinds selectElement = ElementKinds.Normal;
                    bool isSelected = false;
                    foreach(var v in _elementViews)
                    {
                        RectTransform r = (RectTransform)v.transform;
                        if(v.MoveKind == ElementUIMoveKinds.TopMiniToMain || v.MoveKind == ElementUIMoveKinds.BottomMiniToMain)
                        {
                            selectElement = v.ElementKind;
                            isSelected = true;
                            SceneManagerScript.instance.SetElementImage(selectElement);
                            v.ToInvisible(0.5f);
                            r.DOScale(Vector3.one * 3, 0.5f);
                            continue;
                        }
                        v.ToInvisible(0.5f);
                        r.DOScale(Vector3.zero,0.5f);
                    }
                    if(!isSelected) throw new Exception("Elementが選べていません");
                    _playerElemntView.ToInvisible();
                    AudioManager.instance.PlaySE(_decideSound);
                    Time.timeScale = 1f;
                    return selectElement;
                }
            }

            throw new Exception("変なエラー");
        }

        void OnDecide()
        {
            
        }
        
        

        async UniTask MoveUp(int number)
        {
            _movingF = true;

            _elementViews[0].MoveKind = ChangeMoveKinds(true,_elementViews[0],(number + 1) % maximum);
            _elementViews[1].MoveKind = ChangeMoveKinds(true,_elementViews[1],number);
            _elementViews[2].MoveKind = ChangeMoveKinds(true,_elementViews[2],(number - 1) % maximum);
            _elementViews[3].MoveKind = ChangeMoveKinds(true,_elementViews[3],(number - 2) % maximum);

            await UniTask.WhenAll(
                // 画像のアニメーションがリセットされてしまうので、オブジェクトごとに動かすようにする
                _elementViews[0].Move(_elementViews[0].MoveKind,_duration),
                _elementViews[1].Move(_elementViews[1].MoveKind,_duration),
                _elementViews[2].Move(_elementViews[2].MoveKind,_duration),
                _elementViews[3].Move(_elementViews[3].MoveKind,_duration)
            );
            _movingF = false;
        }

        /// <summary>
        /// たぶん、2が中心に来る
        /// </summary>
        /// <returns></returns>
        async UniTask MoveDown(int number)
        {
            _movingF = true;

            _elementViews[0].MoveKind = ChangeMoveKinds(false,_elementViews[0],(number - 3) % maximum);
            _elementViews[1].MoveKind = ChangeMoveKinds(false,_elementViews[1],(number - 2) % maximum);
            _elementViews[2].MoveKind = ChangeMoveKinds(false,_elementViews[2],(number - 1) % maximum);
            _elementViews[3].MoveKind = ChangeMoveKinds(false,_elementViews[3],number);

            await UniTask.WhenAll(
                _elementViews[0].Move(_elementViews[0].MoveKind,_duration),
                _elementViews[1].Move(_elementViews[1].MoveKind,_duration),
                _elementViews[2].Move(_elementViews[2].MoveKind,_duration),
                _elementViews[3].Move(_elementViews[3].MoveKind,_duration)
            );
            _movingF = false;
        }

        ElementUIMoveKinds ChangeMoveKinds(bool isUp,ElementView moveKind,int number)
        {
            Debug.Log(moveKind.gameObject.name + ", " + number + ", " + ((number < 0 || number > maximum - 1) ? number % maximum : number));
            if (isUp)
            {
                if ((int)moveKind.MoveKind < 5)
                {
                    moveKind.MoveKind++;
                    return moveKind.MoveKind;
                }
                else
                {
                    if(number > maximum - 1 || number < 0) number = number % maximum;
                    switch (moveKind.MoveKind)
                    {
                        // 上無へ移動後、下無へ
                        case ElementUIMoveKinds.BottomNull:
                            moveKind.MoveKind = ElementUIMoveKinds.TopNullToTopMini;
                            // moveKind.SetElement(_elementMasterData.Elements[number > maximum - 1 ? number -(maximum - 1) : number]);
                            return moveKind.MoveKind;
                        case ElementUIMoveKinds.BottomNullToBottomMini:
                            moveKind.MoveKind = ElementUIMoveKinds.BottomMiniToBottomNull;
                            return moveKind.MoveKind;
                        case ElementUIMoveKinds.BottomMiniToMain:
                            moveKind.MoveKind = ElementUIMoveKinds.MainToBottomMini;
                            return moveKind.MoveKind;
                        case ElementUIMoveKinds.MainToTopMini:
                            moveKind.MoveKind = ElementUIMoveKinds.TopMiniToMain;
                            return moveKind.MoveKind;
                    }
                }
            }
            else
            {
                if ((int)moveKind.MoveKind >= 5 && (int)moveKind.MoveKind <= (int)ElementUIMoveKinds.TopMiniToTopNull)
                {
                    moveKind.MoveKind++;
                    return moveKind.MoveKind;
                }
                else
                {
                    if(number < 0) number = number % maximum;
                    switch (moveKind.MoveKind)
                    {
                        case ElementUIMoveKinds.TopNull:
                            moveKind.MoveKind = ElementUIMoveKinds.BottomNullToBottomMini;
                            // moveKind.SetElement(_elementMasterData.Elements[number < 0 ? (maximum - 1) + number : number]);
                            return moveKind.MoveKind;
                        case ElementUIMoveKinds.TopNullToTopMini:
                            moveKind.MoveKind = ElementUIMoveKinds.TopMiniToTopNull;
                            return moveKind.MoveKind;
                        case ElementUIMoveKinds.TopMiniToMain:
                            moveKind.MoveKind = ElementUIMoveKinds.MainToTopMini;
                            return moveKind.MoveKind;
                        case ElementUIMoveKinds.MainToBottomMini:
                            moveKind.MoveKind = ElementUIMoveKinds.BottomMiniToMain;
                            return moveKind.MoveKind;
                    }
                }
            }

            throw new System.IndexOutOfRangeException("属性がうまくできてないよ");
        }
    }
}