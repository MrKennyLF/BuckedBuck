using System;
using UnityEngine;

namespace Project.Core
{
    public class PlayerTurnState : IGameState
    {
        private readonly TurnStateMachine _stateMachine;
        private readonly GameContext _context;
        private readonly InputReader _inputReader;

        // Bloqueo temporal para no hacer spam de clics mientras ocurre una animación
        private bool _isAnimating = false;

        public PlayerTurnState(TurnStateMachine stateMachine, GameContext context, InputReader inputReader)
        {
            _stateMachine = stateMachine;
            _context = context;
            _inputReader = inputReader;
        }

        public void Enter()
        {
            Debug.Log("<color=cyan>--- TURNO DEL JUGADOR ---</color>");
            _context.CurrentTurnOwner = TurnOwner.Player;
            _isAnimating = false;

            // Nos suscribimos a disparos Y a clics de objetos
            _inputReader.OnShootRequested += HandleShoot;
            _inputReader.OnItemUseRequested += HandleItemUse;
        }

        public void Execute()
        {
            // Vacio porque esperamos los eventos de InputReader
        }

        public void Exit()
        {
            _inputReader.OnShootRequested -= HandleShoot;
            _inputReader.OnItemUseRequested -= HandleItemUse;
        }

        private void HandleShoot(Target target)
        {
            if (_isAnimating) return; // Si estás en medio de una animación, no puedes disparar

            Debug.Log($"[Estado Jugador] Decisión tomada: Disparar a {target}");
            _context.CurrentTarget = target;
            _stateMachine.ChangeState(typeof(ActionResolutionState));
        }

        private void HandleItemUse(IItem item)
        {
            if (_isAnimating) return;

            // Validamos que el objeto realmente esté en nuestro lado de la mesa
            bool isMyItem = false;
            foreach (var i in _context.PlayerInventory)
            {
                if (i == item) isMyItem = true;
            }

            if (!isMyItem)
            {
                Debug.LogWarning("[Estado Jugador] No puedes usar un objeto que no es tuyo.");
                return;
            }

            Debug.Log($"[Estado Jugador] Usando: {item.Name}");
            _isAnimating = true;

            // 1. Lo borramos del inventario
            _context.ConsumeItem(TurnOwner.Player, item);

            // 2. Ejecutamos el objeto y evaluamos si perdemos el turno
            item.Use(_context, () =>
            {
                _isAnimating = false;

                // REGLA: Solo el cigarro (curación) te quita el turno para evitar la inmortalidad
                if (item.Id == "item_cigarette")
                {
                    Debug.Log($"<color=orange>[Estado Jugador] Usar {item.Name} consumió tu turno. Pasa al Dealer.</color>");
                    _stateMachine.ChangeState(typeof(DealerTurnState));
                }
                else
                {
                    Debug.Log($"[Estado Jugador] Acción rápida. Terminaste de usar {item.Name}. Aún es tu turno.");
                }
            });
        }
    }
}