using Assets.Code.Tools;
using System;
using UnityEngine;

namespace Assets.Code.CharactersLogic
{
    [RequireComponent(typeof(Animator))]
    public class Health : MonoBehaviour
    {
        private Animator _animator;
        private Invincibility _invincibility;

        public event Action<Health> Died;
        public event Action<float> ValueChanged;

        public Regenerator Regenerator { get; private set; }
        public Resist Resist { get; private set; }
        public float DefaultMaxValue { get; private set; }
        public float Value { get; private set; }
        public float MaxValue { get; private set; }
        private bool IsDead => Value <= Constants.Zero;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            Value = MaxValue;
            ValueChanged?.Invoke(Value);
            Regenerator?.Run();
        }

        private void OnDisable()
        {
            Regenerator.Stop();
        }

        public void Initialize(
            float maxValue,
            Invincibility invincibility,
            Regenerator regenerator,
            Resist resist)
        {
            DefaultMaxValue = maxValue.ThrowIfZeroOrLess();
            MaxValue = DefaultMaxValue;
            Value = MaxValue;

            ValueChanged?.Invoke(Value);
            _animator.SetBool(AnimationParameters.IsAlive, Value >= Constants.Zero);

            _invincibility = invincibility.ThrowIfNull();

            Regenerator = regenerator.ThrowIfNull();
            Regenerator.Run();

            Resist = resist.ThrowIfNull();
        }

        public HitResult TakeDamage(float damage)
        {
            if (_invincibility.IsOn || IsDead)
            {
                return new(Constants.Zero, false);
            }

            float resultDamage = Resist.Affect(damage.ThrowIfNegative());

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
                _invincibility.HandleDamage(resultDamage);
            }

            ValueChanged?.Invoke(Value);
            _animator.SetBool(AnimationParameters.IsAlive, Value > Constants.Zero);

            return new(resultDamage, false);
        }

        public void Heal(float value)
        {
            Value = (Value + value).Clamp(Constants.Zero, MaxValue);

            _animator.SetBool(AnimationParameters.IsAlive, Value >= Constants.Zero);
            ValueChanged?.Invoke(Value);
        }

        public void ResetValues()
        {
            MaxValue = DefaultMaxValue;
            Value = MaxValue;

            ValueChanged?.Invoke(Value);
            _animator.SetBool(AnimationParameters.IsAlive, Value >= Constants.Zero);

            Regenerator.Reset();
            Resist.Reset();
        }

        public void SetMaxValue(float value)
        {
            value.ThrowIfNegative();

            Value += value - MaxValue;
            MaxValue = value;

            ValueChanged?.Invoke(Value);
        }
    }
}
