using Assets.Code.Tools.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.Windows
{
    public class AdBonusButton : BaseWindow
    {
        [SerializeField] private Image _progress;
        [SerializeField][Min(1f)] private float _lifeTime = 10f;

        [field: SerializeField] public Button Button { get; private set; }

        private void Awake()
        {
            Button.Subscribe(Disable);
        }

        private void OnDisable()
        {
            TimerService.StopTimer(this, Disable);
        }

        private void OnDestroy()
        {
            Button.Unsubscribe(Disable);
        }

        protected override void OnEnableMethod()
        {
            _progress.fillAmount = Constants.One;
            TimerService.StartTimerWithUpdate(_lifeTime, Disable, FillProgress, this);
        }

        private void FillProgress(float progress)
        {
            _progress.fillAmount = Constants.One - progress.ThrowIfNegative();
        }
    }
}
