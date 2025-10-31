using Assets.Code.Tools;
using System;
using UnityEngine;

namespace Assets.Code.CharactersLogic
{
    [RequireComponent(typeof(Animator))]
    public class Health : MonoBehaviour
    {
        [SerializeField][Min(0.1f)] private float _regenerationDelay = 1;

        private readonly float _defaultResist = 0;

        private float _regenerationCurrentDelay;
        private float _invincibleTimer;
        private float _invincibilityDuration;
        private float _regeneration;
        private float _invincibilityTriggerValue;
        private float _resist = 0;
        private Animator _animator;

        public event Action<Health> Died;
        public event Action<float> ValueChanged;

        public float DefaultMaxValue { get; private set; }
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

            if (Value >= MaxValue || _regeneration == Constants.Zero)
            {
                return;
            }

            if (_regenerationCurrentDelay >= _regenerationDelay)
            {
                Heal();
                _regenerationCurrentDelay = Constants.Zero;
            }
            else
            {
                _regenerationCurrentDelay += Time.deltaTime;
            }
        }

        public void Initialize(float maxValue, float invincibilityDuration, float invincibilityTriggerPercent, float regeneration = 0)
        {
            DefaultMaxValue = maxValue.ThrowIfZeroOrLess();
            MaxValue = DefaultMaxValue;
            Value = MaxValue;

            _regeneration = regeneration.ThrowIfNegative();
            _invincibilityTriggerValue = MaxValue * Constants.PercentToMultiplier(invincibilityTriggerPercent.ThrowIfNegative());
            _invincibilityDuration = invincibilityDuration.ThrowIfNegative();

            ValueChanged?.Invoke(Value);
            _animator.SetBool(AnimationParameters.IsAlive, Value >= Constants.Zero);
        }

        public void SetMaxValue(float value)
        {
            MaxValue = value.ThrowIfZeroOrLess();
        }

        public HitResult TakeDamage(float damage)
        {
            if (IsInvincible || IsDead)
            {
                return new(Constants.Zero, false);
            }

            float resultDamage = damage.ThrowIfNegative() - _resist;

            if (resultDamage <= Constants.Zero)
            {
                return new(Constants.Zero, false);
            }

            float tempValue = Value - resultDamage;

            if (tempValue <= Constants.Zero)
            {
                Value = Constants.Zero;
                Died?.Invoke(this);

                return new(resultDamage, true);
            }
            else
            {
                Value = tempValue;
                _animator.SetTrigger(AnimationParameters.GetHit);

                if (damage >= _invincibilityTriggerValue)
                {
                    _invincibleTimer = _invincibilityDuration;
                }
            }

            ValueChanged?.Invoke(Value);
            _animator.SetBool(AnimationParameters.IsAlive, Value > Constants.Zero);

            return new(resultDamage, false);
        }

        public void SetRegeneration(int value)
        {
            _regeneration = value.ThrowIfNegative();
        }

        public void IncreaseResist(float value, float time = -1)
        {
            if (time > Constants.Zero)
            {
                TimerService.StartTimer(time, () => DecreaseResist(value));
            }

            _resist += value.ThrowIfNegative();
        }

        public void DecreaseResist(float value)
        {
            float result = _resist - value.ThrowIfNegative();
            _resist = result < _defaultResist ? _defaultResist : result;
        }

        private void Heal()
        {
            float tempValue = Value + _regeneration * Time.deltaTime;
            Value = tempValue > MaxValue ? MaxValue : tempValue;

            _animator.SetBool(AnimationParameters.IsAlive, Value >= Constants.Zero);
            ValueChanged?.Invoke(Value);
        }

        public void ResetValue()
        {
            MaxValue = DefaultMaxValue;
            Value = MaxValue;
            ValueChanged?.Invoke(Value);
            _animator.SetBool(AnimationParameters.IsAlive, Value >= Constants.Zero);
        }

        public void AddMaxValue(float value)
        {
            MaxValue += value.ThrowIfNegative();
            Value += value;
            ValueChanged?.Invoke(Value);
        }
    }
}
