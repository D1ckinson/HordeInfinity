using Assets.Code.Tools.Base;
using TMPro;
using UnityEngine;

namespace Assets.Code.Ui.Windows
{
    public class FPSWindow : BaseWindow
    {
        [SerializeField] private TMP_Text _quantityText;

        private readonly FPSCalculator _fpsView = new();

        private void Update()
        {
            _quantityText.SetText(_fpsView.Calculate(Time.unscaledDeltaTime));
        }
    }
}
