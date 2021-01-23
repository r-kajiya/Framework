using System.Collections.Generic;

namespace Framework
{
    public interface IStateMachine<out TContext>
        where TContext : IStateContext
    {
        bool Dispatch(int key, ITransitionParameter transitionParameter);
        TContext Context { get; }
    }

	public abstract class StateMachine<TState, TContext> : IStateMachine<TContext>
        where TState : State
        where TContext : IStateContext
    {
        TState _currentState;
        TState _nextState;
        ITransitionParameter _transitionParameter;

        readonly Dictionary<int, TState> _states = new Dictionary<int, TState>();

        public int CurrentStateKey { get; private set; }

        public TContext Context { get; private set; }

        public StateMachine(TContext context)
        {
            Context = context;
        }

        public bool Register(TState state, int key)
        {
            if (_states.ContainsKey(key))
            {
                return false;
            }

            _states.Add(key, state);

            return true;
        }

        public bool Dispatch(int key, ITransitionParameter transitionParameter = null)
        {
            if (_states.TryGetValue(key, out var next))
            {
                _nextState = next;
                CurrentStateKey = key;
                _transitionParameter = transitionParameter;
                return true;
            }

            return false;
        }

        public void Update()
        {
            if (_nextState != null)
            {
                _currentState?.Exit();
                _currentState = _nextState;
                _currentState.Enter(_transitionParameter);
                _nextState = null;
                _transitionParameter = null;
            }

            if (_currentState != null)
            {
                _currentState.Update();
                _currentState.TransitionIfNeeded();
            }
        }
	}
}