using Assets.Code.Tools.Base;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.Windows
{
    public class AdBonusButton : MonoBehaviour
    {
        private const float FadeTime = 0.3f;

        [SerializeField] private Image _progress;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField][Min(1f)] private float _lifeTime = 10f;

        [field: SerializeField] public Button Button { get; private set; }

        public void OnEnable()
        {
            Button.Subscribe(Disable);
            _progress.fillAmount = Constants.One;
            TimerService.StartTimerWithUpdate(_lifeTime, Disable, FillProgress, this);
        }

        private void OnDisable()
        {
            TimerService.StopTimer(this, Disable);
            Button.Unsubscribe(Disable);
        }

        private void FillProgress(float progress)
        {
            _progress.fillAmount = Constants.One - progress.ThrowIfNegative();
        }

        private void Disable()
        {
            _canvasGroup.DOFade(Constants.Zero, FadeTime)
                    .OnComplete(() => this.SetActive(false));
        }
    }
}
