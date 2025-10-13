using Assets.Code.Tools;
using System;
using UnityEngine;

namespace Assets.Code.CharactersLogic
{
    [RequireComponent(typeof(Animator))]
    public class Health : MonoBehaviour
    {
        private float _invincibleTimer;
        private float _invincibilityDuration;
        private float _regeneration;
        private float _additionalValue;
        private float _resistMultiplier = 1;
        private Animator _animator;

        public event Action<Health> Died;
        public event Action<float> ValueChanged;

        public float Value { get; private set; }
        public float MaxValue { get; private set; }
        private bool IsInvincible => _invincibleTimer > Constants.Zero;
        private bool IsDead => Value <= Constants.Zero;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            Value = MaxValue;
            ValueChanged?.Invoke(Value);
        }

        private void Update()
        {
            if (IsInvincible)
            {
                _invincibleTimer -= Time.deltaTime;
            }

            if (Value < MaxValue && _regeneration != Constants.Zero)
            {
                Heal();
            }
        }

        public void Initialize(float maxValue, float invincibilityDuration)
        {
            MaxValue = maxValue.ThrowIfZeroOrLess();
            Value = MaxValue;

            _invincibilityDuration = invincibilityDuration.ThrowIfNegative();
            ValueChanged?.Invoke(Value);
            _animator.SetBool(AnimationParameters.IsAlive, Value >= Constants.Zero);
        }

        public void SetMaxValue(float value)
        {
            MaxValue = value.ThrowIfZeroOrLess() + _additionalValue;
        }

        public void TakeDamage(float damage)
        {
            if (IsInvincible || IsDead)
            {
                return;
            }

            float tempValue = Value - damage.ThrowIfNegative() * _resistMultiplier;

            if (tempValue <= Constants.Zero)
            {
                Value = Constants.Zero;
                Died?.Invoke(this);
            }
            else
            {
                Value = tempValue;
                _invincibleTimer = _invincibilityDuration;
                _animator.SetTrigger(AnimationParameters.GetHit);
            }

            ValueChanged?.Invoke(Value);
            _animator.SetBool(AnimationParameters.IsAlive, Value > Constants.Zero);
        }

        public void SetAdditionalValue(int value)
        {
            float tempValue = value.ThrowIfNegative() - _additionalValue;

            _additionalValue = value;
            MaxValue += tempValue;
            Value += tempValue;

            ValueChanged?.Invoke(Value);
            _animator.SetBool(AnimationParameters.IsAlive, Value >= Constants.Zero);
        }

        public void SetRegeneration(int value)
        {
            _regeneration = value.ThrowIfNegative();
        }

        public void SetResistPercent(int resistPercent)
        {
            _resistMultiplier = Constants.PercentToMultiplier(resistPercent.ThrowIfNegative());
        }

        private void Heal()
        {
            float tempValue = Value + _regeneration;
            Value = tempValue > MaxValue ? MaxValue : tempValue;
            _animator.SetBool(AnimationParameters.IsAlive, Value >= Constants.Zero);
        }

        public void ResetValue()
        {
            Value = MaxValue;
            ValueChanged?.Invoke(Value);
            _animator.SetBool(AnimationParameters.IsAlive, Value >= Constants.Zero);
        }
    }
}
