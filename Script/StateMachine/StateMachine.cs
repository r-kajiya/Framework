using System.Collections.Generic;

namespace Framework
{
    public interface ITransitionParam { }

    public interface IStateMachine
    {
        bool Dispatch(int key, ITransitionParam transitionParam);
    }

	public abstract class StateMachine<TState> : IStateMachine
        where TState : State
    {
        TState _curState;
        TState _nextState;
        ITransitionParam _transitionParam;

        Dictionary<int, TState> _states = new Dictionary<int, TState>();

        public int CurStateKey { get; private set; }

        public bool Register(TState state, int key)
        {
            if (_states.ContainsKey(key))
            {
                return false;
            }

            _states.Add(key, state);

            return true;
        }

        public bool Dispatch(int key, ITransitionParam transitionParam = null)
        {
            TState next = null;

            if (_states.TryGetValue(key, out next))
            {
                _nextState = next;
                CurStateKey = key;
                _transitionParam = transitionParam;
                return true;
            }

            return false;
        }

        public void Update()
        {
            if (_nextState != null)
            {
                if (_curState != null)
                {
                    _curState.Exit();
                }
                
                _curState = _nextState;
                _curState.Enter(_transitionParam);
                _nextState = null;
                _transitionParam = null;
            }

            if (_curState != null)
            {
                _curState.Update();
            }
        }
	}

    public abstract class State
    {
        protected IStateMachine StateMachine { get; private set; }
        public State(IStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        public virtual void Enter(ITransitionParam transitionParam) { }
        public virtual void Update() { }
        public virtual void Exit() { }
        protected void Dispatch(int key, ITransitionParam transitionParam = null) { StateMachine.Dispatch(key, transitionParam); }
    }
}