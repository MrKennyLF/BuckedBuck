using System;
using UnityEngine;

namespace Project.Core
{
    public class DealerTurnState : IGameState
    {
        private TurnStateMachine _stateMachine;
        private GameContext _context;
        private IDealerAI _aiEngine;

        // El delegado Action<Action> permite enviar una función como parámetro
        public event Action<Action> OnDealerThinking;

        public DealerTurnState(TurnStateMachine stateMachine, GameContext context, IDealerAI aiEngine)
        {
            _stateMachine = stateMachine;
            _context = context;
            _aiEngine = aiEngine;
        }

        public void Enter()
        {
            Debug.Log("[Estado] Turno del Dealer. Iniciando espera visual...");
            _context.CurrentTurnOwner = TurnOwner.Dealer;

            // Delegamos el control temporalmente a la vista. 
            // Cuando la vista termine, ejecutará ExecuteDealerAction.
            OnDealerThinking?.Invoke(ExecuteDealerAction);
        }

        public void Execute() { }

        public void Exit() { }

        // Este método es privado. Solo debe ser llamado a través del callback de la vista.
        private void ExecuteDealerAction()
        {
            Target chosenTarget = _aiEngine.DecideTarget(_context);
            _context.CurrentTarget = chosenTarget;

            _stateMachine.ChangeState(typeof(ActionResolutionState));
        }
    }
}