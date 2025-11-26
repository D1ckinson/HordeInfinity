using Assets.Code.Data;
using Assets.Code.Tools;

namespace Assets.Code.CharactersLogic
{
    public class Regenerator : ValueContainer
    {
        private readonly Health _health;

        public Regenerator(Health health, float value = 0) : base(value, value)
        {
            _health = health.ThrowIfNull();
        }

        public void Run()
        {
            UpdateService.RegisterUpdate(Regenerate);
        }

        public void Stop()
        {
            UpdateService.UnregisterUpdate(Regenerate);
        }

        private void Regenerate(float deltaTime)
        {
            _health.Heal(Value * deltaTime);
        }
    }
}
