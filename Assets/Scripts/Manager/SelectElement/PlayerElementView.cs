using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Manager.SelectElement
{
    public class PlayerElementView: MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Animator _anim;


        public void SetAlphaColor(float value)
        {
            gameObject.SetActive(true);
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, value);
        }

        public void StartMove(float time)
        {
            _image.rectTransform.localPosition = new Vector3(-200,0,0);
            _image.rectTransform.DOLocalMoveX(0,time).SetUpdate(true).ToUniTask();
            _image.DOFade(1,time).SetUpdate(true).ToUniTask();
        }

        public async UniTask ToInvisible()
        {
            await _image.DOFade(0,0.5f).SetUpdate(true);
            gameObject.SetActive(false);
        }
    }
}