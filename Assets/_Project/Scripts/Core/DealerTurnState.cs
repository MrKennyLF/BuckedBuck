using UnityEngine;

namespace Project.Core
{
    public class DealerTurnState : IGameState
    {
        private readonly TurnStateMachine _stateMachine;
        private readonly GameContext _context;
        private readonly IDealerAI _dealerAI;

        public DealerTurnState(TurnStateMachine stateMachine, GameContext context, IDealerAI dealerAI)
        {
            _stateMachine = stateMachine;
            _context = context;
            _dealerAI = dealerAI;
        }

        public void Enter()
        {
            Debug.Log("<color=red>--- TURNO DEL DEALER ---</color>");
            _context.CurrentTurnOwner = TurnOwner.Dealer;
            Execute();
        }

        public void Execute()
        {
            var visual = Object.FindFirstObjectByType<Project.Visuals.VisualController>();
            if (visual != null)
            {
                // El Dealer hace una pausa para pensar (o repensar) su jugada
                visual.SimulateDealerThinking(() => TryUseItemOrShoot());
            }
            else
            {
                TryUseItemOrShoot();
            }
        }

        private void TryUseItemOrShoot()
        {
            // 1. Le preguntamos a su cerebro si quiere usar algo
            IItem itemToUse = _dealerAI.DecideItemToUse(_context);

            if (itemToUse != null)
            {
                Debug.Log($"<color=orange>[Estado Dealer] El Dealer decide usar: {itemToUse.Name}</color>");

                // Lo borramos de su lado de la mesa
                _context.ConsumeItem(TurnOwner.Dealer, itemToUse);

                // El Dealer usa el objeto y evaluamos si pierde el turno
                itemToUse.Use(_context, () =>
                {
                    // NUEVA REGLA: Si no es la lupa, el Dealer pierde su turno
                    if (itemToUse.Id == "item_cigarette")
                    {
                        Debug.Log($"<color=orange>[Estado Dealer] Usar {itemToUse.Name} consumió su turno. Pasa al Jugador.</color>");
                        _stateMachine.ChangeState(typeof(PlayerTurnState));
                    }
                    else
                    {
                        Execute(); // Conserva el turno y vuelve a pensar
                    }
                });
            }
            else
            {
                // 2. Si ya no quiere usar objetos, decide a quién disparar
                MakeDecision();
            }
        }

        private void MakeDecision()
        {
            Target chosenTarget = _dealerAI.DecideTarget(_context);
            _context.CurrentTarget = chosenTarget;
            _stateMachine.ChangeState(typeof(ActionResolutionState));
        }

        public void Exit()
        {
            // Limpieza al salir de su turno
        }
    }
}