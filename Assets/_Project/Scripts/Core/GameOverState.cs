using UnityEngine;

namespace Project.Core
{
    public class GameOverState : IGameState
    {
        private readonly TurnStateMachine _stateMachine;
        private readonly GameContext _context;

        public GameOverState(TurnStateMachine stateMachine, GameContext context)
        {
            _stateMachine = stateMachine;
            _context = context;
        }

        public void Enter()
        {
            Debug.Log("<color=black>--- FIN DEL JUEGO ---</color>");
            Execute();
        }

        public void Execute()
        {
            string message = "";

            if (_context.PlayerHealth <= 0)
            {
                message = "EL DEALER GANA.\nHas muerto.";
            }
            else if (_context.DealerHealth <= 0)
            {
                message = "¡HAS GANADO!\nEl Dealer ha caído.";
            }

            // Disparamos el evento para que la UI muestre la pantalla
            _context.TriggerGameOver(message);
        }

        public void Exit() { }
    }
}