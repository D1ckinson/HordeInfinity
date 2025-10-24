using Assets.Code.Tools;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.Windows
{
    public class FadeWindow : BaseWindow
    {
        private const float VisibleThreshold = 0.99f;
        private const float HiddenThreshold = 0.01f;

        [SerializeField] private Image _image;
        [SerializeField][Min(0)] private float _fadeDuration1 = 0.5f;

        private Sequence _showSequence;
        private Sequence _hideSequence;

        private Action _onShow;
        private Action _onHide;

        private bool IsVisible => _image.color.a >= VisibleThreshold;
        private bool IsHidden => _image.color.a <= HiddenThreshold;

        private void Awake()
        {
            _showSequence = DOTween.Sequence()
           .SetAutoKill(false)
           .Append(_image.DOFade(Constants.One, _fadeDuration1))
           .OnComplete(() => _onShow?.Invoke())
           .Pause();

            _hideSequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(_image.DOFade(Constants.Zero, _fadeDuration1))
                .OnComplete(() => _onHide?.Invoke())
                .Pause();
        }

        private void OnDestroy()
        {
            _showSequence?.Kill();
            _hideSequence?.Kill();
        }


        public void Show(Action onComplete = null)
        {
            if (IsVisible)
            {
                onComplete?.Invoke();
                return;
            }

            if (_showSequence.IsPlaying())
            {
                _onShow += onComplete;
                return;
            }

            _onShow = onComplete;
            _hideSequence.Pause();
            _showSequence.Restart();
        }

        public void Hide(Action onComplete = null)
        {
            if (IsHidden)
            {
                onComplete?.Invoke();
                return;
            }

            if (_hideSequence.IsPlaying())
            {
                _onHide += onComplete;
                return;
            }

            _onHide = onComplete;
            _showSequence.Pause();
            _hideSequence.Restart();
        }
    }
}