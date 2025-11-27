using Assets.Code.Tools.Base;

namespace Assets.Code.StateMachineLogic.Base
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
