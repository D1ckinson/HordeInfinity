using Assets.Code.Tools;

namespace Assets.Scripts.State_Machine
{
    public abstract class State : IState
    {
        private readonly StateMachine _stateMachine;

        public State(StateMachine stateMachine)
        {
            _stateMachine = stateMachine.ThrowIfNull();
        }

        public abstract void Enter();

        public abstract void Exit();

        protected void SetState<T>() where T : IState
        {
            _stateMachine.SetState<T>();
        }
    }
}
