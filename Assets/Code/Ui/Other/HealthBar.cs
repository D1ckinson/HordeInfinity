using Assets.Code.CharactersLogic.GeneralLogic;
using Assets.Code.Tools.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.Other
{
    internal class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _healthBar;
        [SerializeField] private Image _healthBarFilling;
        [SerializeField] private Health _health;
        [SerializeField] private bool _isFullHealthHide = true;

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            _health.ValueChanged += SetSliderValue;
            SetSliderValue(_health.Value);
        }

        private void LateUpdate()
        {
            Vector3 lookPoint = new(transform.position.x, _camera.transform.position.y, _camera.transform.position.z);
            transform.LookAt(lookPoint);
        }

        private void OnDestroy()
        {
            if (_health.IsNull() == false)
            {
                _health.ValueChanged -= SetSliderValue;
            }
        }

        private void SetSliderValue(float value)
        {
            value.ThrowIfNegative();

            if (_isFullHealthHide && Mathf.Approximately(value, _health.MaxValue))
            {
                _healthBar.SetActive(false);
            }
            else if (_healthBar.IsActive() == false)
            {
                _healthBar.SetActive(true);
            }

            float percentValue = value / _health.MaxValue;
            _healthBarFilling.fillAmount = percentValue;
        }
    }
}
