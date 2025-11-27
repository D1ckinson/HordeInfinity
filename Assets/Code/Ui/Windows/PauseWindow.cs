using Assets.Code.Tools.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.Windows
{
    public class PauseWindow : BaseWindow
    {
        [field: SerializeField] public Button ContinueButton { get; private set; }
        [field: SerializeField] public Button ExitButton { get; private set; }

        private void Awake()
        {
            ContinueButton.Subscribe(Disable);
            ExitButton.Subscribe(Disable);
        }

        private void OnDestroy()
        {
            ContinueButton.Unsubscribe(Disable);
            ExitButton.Unsubscribe(Disable);
        }
    }
}