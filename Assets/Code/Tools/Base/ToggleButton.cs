using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Tools.Base
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Sprite _turnOnSprite;
        [SerializeField] private Sprite _turnOffSprite;
        [SerializeField] private Button _button;

        public bool IsOn { get; private set; } = true;

        public event Action<bool> Clicked;

        private void Awake()
        {
            _button.Subscribe(OnClick);
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.Unsubscribe(OnClick);
            }
        }

        public void SetState(bool isOn)
        {
            IsOn = isOn;
            _icon.sprite = IsOn ? _turnOnSprite : _turnOffSprite;
        }

        private void OnClick()
        {
            IsOn = !IsOn;
            _icon.sprite = IsOn ? _turnOnSprite : _turnOffSprite;
            Clicked?.Invoke(IsOn);
        }
    }
}
