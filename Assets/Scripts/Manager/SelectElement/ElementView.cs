using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Manager.SelectElement.Models;
using UnityEngine.ProBuilder;
using UnityEngine.Serialization;

namespace Manager.SelectElement
{
    public class ElementView : MonoBehaviour
    {
        /// <summary>
        /// 最後に実施したアニメーションの種類
        /// </summary>
        [FormerlySerializedAs("_kind")] [SerializeField]
        private ElementUIMoveKinds moveKind;
        public ElementUIMoveKinds MoveKind
        {
            get
            {
                return moveKind;
            }
            set
            {
                moveKind = value;
            }
        }
        /// <summary>
        /// 全体のRectTransform
        /// </summary>
        [SerializeField]
        private RectTransform _rect;

        /// <summary>
        /// アイコンフレーム
        /// </summary>
        [SerializeField]
        private Image _iconFrame;

        /// <summary>
        /// 属性アイコン
        /// </summary>
        [SerializeField]
        private Image _icon;

        /// <summary>
        /// アイコン用のアニメーター
        /// </summary>
        [SerializeField]
        private Animator _iconAnim;

        /// <summary>
        /// 説明フレーム
        /// </summary>
        [SerializeField]
        private Image _introductFrame;

        /// <summary>
        /// 属性の名前のテキスト
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _elementNameText;

        /// <summary>
        /// 属性の説明のテキスト
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _elementIntroductText;

        private Animator _animator;

        private ElementKinds _elementKind;
        public ElementKinds ElementKind => _elementKind;

        void Awake()
        {
            _rect = (RectTransform)transform;
            TryGetComponent<Animator>(out _animator);
        }

        public void InVisible()
        {
            _iconFrame.color = SetAlphaColor(_iconFrame.color,0);
            _icon.color = SetAlphaColor(_icon.color,0);
            _introductFrame.color = SetAlphaColor(_introductFrame.color,0);
            _elementNameText.color = SetAlphaColor(_elementNameText.color,0);
            _elementIntroductText.color = SetAlphaColor(_elementIntroductText.color,0);
        }

        Color SetAlphaColor(Color c, float a)
        {
            return new Color(c.r,c.g,c.b,a);
        }

        /// <summary>
        /// モデルの値を設定させる
        /// </summary>
        /// <param name="model">対象のモデル</param>
        public void SetElement(ElementModel model)
        {
            _animator.SetTrigger(model.ElementKind.ToString() + "T");
            _elementNameText.text = model.ElementName;
            _elementIntroductText.text = model.ElementDescription;
            _elementKind = model.ElementKind;
        }

        public async UniTask Move(ElementUIMoveKinds moveKind, float duration)
        {
            switch (moveKind)
            {
                case ElementUIMoveKinds.TopNullToTopMini:
                    // 初期化する
                    _iconFrame.color = SetAlphaColor(_iconFrame.color,0);
                    _icon.color = SetAlphaColor(_icon.color,0);
                    _introductFrame.color = SetAlphaColor(_introductFrame.color,0);
                    _elementNameText.color = SetAlphaColor(_elementNameText.color,0);
                    _elementIntroductText.color = SetAlphaColor(_elementIntroductText.color,0);
                    _rect.localScale = Vector3.one;

                    // 移動をする
                    // 半径150の円の120°から60°へ
                    await UniTask.WhenAll(
                        DOTween.To(()=>2*Mathf.PI/3,t =>
                        {
                            _rect.localPosition = new Vector3(150 * Mathf.Cos(t),150 * Mathf.Sin(t),0);
                        },Mathf.PI/3, duration).SetUpdate(true).ToUniTask(),
                        _iconFrame.DOFade(1f,duration).SetUpdate(true).ToUniTask(),
                        _icon.DOFade(1f,duration).SetUpdate(true).ToUniTask()
                        );
                    break;

                case ElementUIMoveKinds.TopMiniToMain:
                    _rect.localScale = Vector3.one;
                    _iconFrame.color = SetAlphaColor(_iconFrame.color,1);
                    _icon.color = SetAlphaColor(_icon.color,1);
                    _introductFrame.color = SetAlphaColor(_introductFrame.color,0);
                    _elementNameText.color = SetAlphaColor(_elementNameText.color,0);
                    _elementIntroductText.color = SetAlphaColor(_elementIntroductText.color,0);

                    // 移動をする
                    // 半径150の円の60°から0°へ
                    _introductFrame.rectTransform.localPosition = new Vector3(75f,_introductFrame.rectTransform.localPosition.y,0);
                    await UniTask.WhenAll(
                        // 位置を調節
                        DOTween.To(()=>Mathf.PI/3,t =>
                        {
                            _rect.localPosition = new Vector3(150 * Mathf.Cos(t),150 * Mathf.Sin(t),0);
                        },0, duration).SetUpdate(true).ToUniTask(),

                        // 説明フレームを表示
                        _introductFrame.DOFade(1f,duration).SetUpdate(true).ToUniTask(),
                        _elementNameText.DOFade(1f,duration).SetUpdate(true).ToUniTask(),
                        _elementIntroductText.DOFade(1f,duration).SetUpdate(true).ToUniTask(),

                        // 説明フレームを動かす
                        _introductFrame.rectTransform.DOLocalMoveX(100f,duration).SetUpdate(true).ToUniTask(),

                        // 大きさを2にする
                        _rect.DOScale(Vector3.one * 2, duration).SetUpdate(true).ToUniTask()
                        );
                    break;
                case ElementUIMoveKinds.MainToBottomMini:
                    _rect.localScale = Vector3.one * 2;
                    _iconFrame.color = SetAlphaColor(_iconFrame.color,1);
                    _icon.color = SetAlphaColor(_icon.color,1);
                    _introductFrame.color = SetAlphaColor(_introductFrame.color,1);
                    _elementNameText.color = SetAlphaColor(_elementNameText.color,1);
                    _elementIntroductText.color = SetAlphaColor(_elementIntroductText.color,1);

                    // 移動をする
                    // 半径150の円の0°から-60°へ
                    await UniTask.WhenAll(
                        // 位置を調節
                        DOTween.To(()=>0f,t =>
                        {
                            _rect.localPosition = new Vector3(150 * Mathf.Cos(t),150 * Mathf.Sin(t),0);
                        },-Mathf.PI/3, duration).SetUpdate(true).ToUniTask(),

                        // 説明フレームを非表示
                        _introductFrame.DOFade(0f,duration).SetUpdate(true).ToUniTask(),
                        _elementNameText.DOFade(0f,duration).SetUpdate(true).ToUniTask(),
                        _elementIntroductText.DOFade(0f,duration).SetUpdate(true).ToUniTask(),

                        // 説明フレームを動かす
                        _introductFrame.rectTransform.DOLocalMoveX(75f,duration).SetUpdate(true).ToUniTask(),

                        // 大きさを1にする
                        _rect.DOScale(Vector3.one, duration).SetUpdate(true).ToUniTask()
                        );
                    break;
                case ElementUIMoveKinds.BottomMiniToBottomNull:
                    _rect.localScale = Vector3.one;
                    _iconFrame.color = SetAlphaColor(_iconFrame.color,1);
                    _icon.color = SetAlphaColor(_icon.color,1);
                    _introductFrame.color = SetAlphaColor(_introductFrame.color,0);
                    _elementNameText.color = SetAlphaColor(_elementNameText.color,0);
                    _elementIntroductText.color = SetAlphaColor(_elementIntroductText.color,0);

                    // 移動をする
                    // 半径150の円の-60°から-120°へ
                    await UniTask.WhenAll(
                        // 位置を調節
                        DOTween.To(()=>-Mathf.PI/3,t =>
                        {
                            _rect.localPosition = new Vector3(150 * Mathf.Cos(t),150 * Mathf.Sin(t),0);
                        },-2*Mathf.PI/3, duration).SetUpdate(true).ToUniTask(),

                        // 説明フレームを非表示
                        _iconFrame.DOFade(0f,duration).SetUpdate(true).ToUniTask(),
                        _icon.DOFade(0f,duration).SetUpdate(true).ToUniTask()
                        );
                    this.moveKind = ElementUIMoveKinds.TopNull;
                    break;

                /////////////////////////////////////////////////////////////////////////////////////////
                /// 下入力の場合
                /////////////////////////////////////////////////////////////////////////////////////////
                
                case ElementUIMoveKinds.BottomNullToBottomMini:
                    // 初期化する
                    _iconFrame.color = SetAlphaColor(_iconFrame.color,0);
                    _icon.color = SetAlphaColor(_icon.color,0);
                    _introductFrame.color = SetAlphaColor(_introductFrame.color,0);
                    _elementNameText.color = SetAlphaColor(_elementNameText.color,0);
                    _elementIntroductText.color = SetAlphaColor(_elementIntroductText.color,0);
                    _rect.localScale = Vector3.one;

                    // 移動をする
                    // 半径150の円の-120°から-60°へ
                    await UniTask.WhenAll(
                        DOTween.To(()=>-2*Mathf.PI/3,t =>
                        {
                            _rect.localPosition = new Vector3(150 * Mathf.Cos(t),150 * Mathf.Sin(t),0);
                        },-Mathf.PI/3, duration).SetUpdate(true).ToUniTask(),
                        _iconFrame.DOFade(1f,duration).SetUpdate(true).ToUniTask(),
                        _icon.DOFade(1f,duration).SetUpdate(true).ToUniTask()
                        );
                    break;
                case ElementUIMoveKinds.BottomMiniToMain:
                    _rect.localScale = Vector3.one;
                    _iconFrame.color = SetAlphaColor(_iconFrame.color,1);
                    _icon.color = SetAlphaColor(_icon.color,1);
                    _introductFrame.color = SetAlphaColor(_introductFrame.color,0);
                    _elementNameText.color = SetAlphaColor(_elementNameText.color,0);
                    _elementIntroductText.color = SetAlphaColor(_elementIntroductText.color,0);

                    // 移動をする
                    // 半径150の円の-60°から0°へ
                    _introductFrame.rectTransform.localPosition = new Vector3(75f,_introductFrame.rectTransform.localPosition.y,0);
                    await UniTask.WhenAll(
                        // 位置を調節
                        DOTween.To(()=>-Mathf.PI/3,t =>
                        {
                            _rect.localPosition = new Vector3(150 * Mathf.Cos(t),150 * Mathf.Sin(t),0);
                        },0, duration).SetUpdate(true).ToUniTask(),

                        // 説明フレームを表示
                        _introductFrame.DOFade(1f,duration).SetUpdate(true).ToUniTask(),
                        _elementNameText.DOFade(1f,duration).SetUpdate(true).ToUniTask(),
                        _elementIntroductText.DOFade(1f,duration).SetUpdate(true).ToUniTask(),

                        // 説明フレームを動かす
                        _introductFrame.rectTransform.DOLocalMoveX(100f,duration).SetUpdate(true).ToUniTask(),

                        // 大きさを2にする
                        _rect.DOScale(Vector3.one * 2, duration).SetUpdate(true).ToUniTask()
                        );
                    break;
                case ElementUIMoveKinds.MainToTopMini:
                    _rect.localScale = Vector3.one * 2;
                    _iconFrame.color = SetAlphaColor(_iconFrame.color,1);
                    _icon.color = SetAlphaColor(_icon.color,1);
                    _introductFrame.color = SetAlphaColor(_introductFrame.color,1);
                    _elementNameText.color = SetAlphaColor(_elementNameText.color,1);
                    _elementIntroductText.color = SetAlphaColor(_elementIntroductText.color,1);

                    // 移動をする
                    // 半径150の円の0°から60°へ
                    await UniTask.WhenAll(
                        // 位置を調節
                        DOTween.To(()=>0f,t =>
                        {
                            _rect.localPosition = new Vector3(150 * Mathf.Cos(t),150 * Mathf.Sin(t),0);
                        },Mathf.PI/3, duration).SetUpdate(true).ToUniTask(),

                        // 説明フレームを非表示
                        _introductFrame.DOFade(0f,duration).SetUpdate(true).ToUniTask(),
                        _elementNameText.DOFade(0f,duration).SetUpdate(true).ToUniTask(),
                        _elementIntroductText.DOFade(0f,duration).SetUpdate(true).ToUniTask(),

                        // 説明フレームを動かす
                        _introductFrame.rectTransform.DOLocalMoveX(75f,duration).SetUpdate(true).ToUniTask(),

                        // 大きさを1にする
                        _rect.DOScale(Vector3.one, duration).SetUpdate(true).ToUniTask()
                        );
                    break;
                case ElementUIMoveKinds.TopMiniToTopNull:
                    _rect.localScale = Vector3.one;
                    _iconFrame.color = SetAlphaColor(_iconFrame.color,1);
                    _icon.color = SetAlphaColor(_icon.color,1);
                    _introductFrame.color = SetAlphaColor(_introductFrame.color,0);
                    _elementNameText.color = SetAlphaColor(_elementNameText.color,0);
                    _elementIntroductText.color = SetAlphaColor(_elementIntroductText.color,0);

                    // 移動をする
                    // 半径150の円の60°から120°へ
                    await UniTask.WhenAll(
                        // 位置を調節
                        DOTween.To(()=>Mathf.PI/3,t =>
                        {
                            _rect.localPosition = new Vector3(150 * Mathf.Cos(t),150 * Mathf.Sin(t),0);
                        },2*Mathf.PI/3, duration).SetUpdate(true).ToUniTask(),

                        // 説明フレームを非表示
                        _iconFrame.DOFade(0f,duration).SetUpdate(true).ToUniTask(),
                        _icon.DOFade(0f,duration).SetUpdate(true).ToUniTask()
                        );
                    this.moveKind = ElementUIMoveKinds.BottomNull;
                    break;

            }
        }

        public void ToInvisible(float time)
        {
            _iconFrame.DOFade(0,time);
            _icon.DOFade(0,time);
            _introductFrame.DOFade(0,time);
            _elementNameText.DOFade(0,time);
            _elementIntroductText.DOFade(0,time);
        }
    }
}