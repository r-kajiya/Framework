using System.Collections.Generic;

namespace Framework
{
    public abstract class State
    {
        protected IStateMachine<IStateContext> StateMachine { get; }
        protected ITransitionParameter TransitionParameter { get; set; }

        public State(IStateMachine<IStateContext> stateMachine)
        {
            StateMachine = stateMachine;
        }
        
        public virtual void Enter(ITransitionParameter transitionParameter) { }
        public virtual void Update() { }
        public virtual void TransitionIfNeeded() { }
        public virtual void Exit() { }

        protected void Dispatch(int key, ITransitionParameter transitionParameter)
        {
            StateMachine.Dispatch(key, transitionParameter);
        }
    }
}