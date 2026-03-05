using System;
using System.Collections.Generic;

namespace Project.Core
{
    public class TurnStateMachine
    {
        // Diccionario para guardar todos los estados que el Bootstrapper nos inyecte
        private Dictionary<Type, IGameState> _states = new Dictionary<Type, IGameState>();
        private IGameState _currentState;

        public void AddState(IGameState state)
        {
            _states[state.GetType()] = state;
        }

        public void ChangeState(Type stateType)
        {
            // 1. Salimos del estado actual si existe
            if (_currentState != null)
            {
                _currentState.Exit();
            }

            // 2. Buscamos el nuevo estado y entramos
            if (_states.TryGetValue(stateType, out IGameState nextState))
            {
                _currentState = nextState;
                _currentState.Enter();
            }
            else
            {
                UnityEngine.Debug.LogError($"[StateMachine] Intento de cambiar a un estado no registrado: {stateType}");
            }
        }
    }
}