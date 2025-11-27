using Assets.Code.Tools.Base;
using DG.Tweening;
using UnityEngine;

namespace Assets.Code.Ui.Windows
{
    public class BaseWindow : MonoBehaviour
    {
        [SerializeField][Min(0.1f)] private float _fadeTime = 0.3f;
        [SerializeField] private CanvasGroup _canvasGroup;

        private void OnEnable()
        {
            if (_canvasGroup.IsNotNull())
            {
                _canvasGroup.DOFade(Constants.One, _fadeTime);
            }

            OnEnableMethod();
        }

        protected void Disable()
        {
            if (_canvasGroup.IsNotNull())
            {
                _canvasGroup.DOFade(Constants.Zero, _fadeTime)
                    .OnComplete(() => this.SetActive(false));
            }
            else
            {
                Disable();
            }
        }

        protected virtual void OnEnableMethod() { }
    }
}
