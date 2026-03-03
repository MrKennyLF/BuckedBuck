using System;
using System.Collections.Generic;

namespace Project.Core
{
    public class TurnStateMachine
    {
        private IGameState _currentState;
        private Dictionary<Type, IGameState> _states = new Dictionary<Type, IGameState>();

        // Registramos los estados ya construidos con sus dependencias
        public void AddState(IGameState state)
        {
            _states[state.GetType()] = state;
        }

        public void ChangeState<T>() where T : IGameState
        {
            if (!_states.ContainsKey(typeof(T))) return;

            if (_currentState != null)
            {
                _currentState.Exit();
            }

            _currentState = _states[typeof(T)];
            _currentState.Enter();
        }
        public void ChangeState(Type stateType)
        {
            if (stateType == null || !_states.ContainsKey(stateType)) return;

            _currentState?.Exit();
            _currentState = _states[stateType];
            _currentState.Enter();
        }
        public void Update()
        {
            _currentState?.Execute();
        }
    }
}
