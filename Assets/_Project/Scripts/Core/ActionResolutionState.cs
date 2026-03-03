using System;
using UnityEngine;

namespace Project.Core
{
    public class ActionResolutionState : IGameState
    {
        private TurnStateMachine _stateMachine;
        private GameContext _context;
        private ITurnRuleEvaluator _ruleEvaluator;

        private bool _lastShotWasLive;

        // Evento que notifica a la capa visual qué ocurrió, pasando el objetivo y si la bala era real
        public event Action<Target, bool> OnShotFired;

        public ActionResolutionState(TurnStateMachine stateMachine, GameContext context, ITurnRuleEvaluator ruleEvaluator)
        {
            _stateMachine = stateMachine;
            _context = context;
            _ruleEvaluator = ruleEvaluator;
        }

        public void Enter()
        {
            ResolveShot();
        }

        public void Execute() { }

        public void Exit()
        {
            // Limpiamos la intención del turno saliente para evitar datos fantasma
            _context.CurrentTarget = Target.None;
            _context.CurrentTurnOwner = TurnOwner.None;
        }

        private void ResolveShot()
        {
            // Medida de seguridad: Si no hay balas, salimos
            if (_context.ShotgunChamber.Count == 0) return;

            // 1. Extraemos la bala usando el método seguro (Mutación controlada)
            _lastShotWasLive = _context.ExtractNextRound();

            // 2. Aplicamos daño matemático
            if (_lastShotWasLive)
            {
                if (_context.CurrentTarget == Target.Player) _context.PlayerHealth--;
                if (_context.CurrentTarget == Target.Dealer) _context.DealerHealth--;
            }

            Debug.Log($"[Resolución] Disparo a {_context.CurrentTarget}. Bala viva: {_lastShotWasLive}. Salud Jugador: {_context.PlayerHealth}, Salud Dealer: {_context.DealerHealth}");

            // 3. Disparamos el evento a la vista (VisualController) para que haga las animaciones
            OnShotFired?.Invoke(_context.CurrentTarget, _lastShotWasLive);

            // La ejecución lógica se detiene aquí. 
            // La máquina de estados queda en pausa esperando el Callback de Unity.
        }

        // Este método es el Callback. Será invocado por la capa visual cuando termine su espectáculo.
        public void CompleteVisualResolution()
        {
            // Delegamos la decisión algorítmica a nuestra Estrategia Inyectada (BuckshotRules)
            Type nextState = _ruleEvaluator.EvaluateNextTurn(_context, _lastShotWasLive);

            if (nextState != null)
            {
                _stateMachine.ChangeState(nextState);
            }
            else
            {
                Debug.LogWarning("[Sistema] Fin del juego o estado nulo evaluado.");
            }
        }
    }
}