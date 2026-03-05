using System;
using UnityEngine;
using Object = UnityEngine.Object; // Necesario para buscar la vista rápido

namespace Project.Core
{
    public class ActionResolutionState : IGameState
    {
        private readonly TurnStateMachine _stateMachine;
        private readonly GameContext _context;
        private readonly ITurnRuleEvaluator _ruleEvaluator;

        public ActionResolutionState(TurnStateMachine stateMachine, GameContext context, ITurnRuleEvaluator ruleEvaluator)
        {
            _stateMachine = stateMachine;
            _context = context;
            _ruleEvaluator = ruleEvaluator;
        }

        public void Enter()
        {
            Debug.Log("<color=yellow>--- RESOLVIENDO DISPARO ---</color>");
            Execute();
        }

        public void Execute()
        {
            // 1. Extraemos la siguiente bala de la escopeta
            bool isLive = _context.ExtractNextRound();
            Target target = _context.CurrentTarget;

            string tipoBala = isLive ? "VIVA (Roja)" : "FOGUEO (Azul)";
            Debug.Log($"[Resolución] ¡PUM! Disparo al {target} con bala de {tipoBala}.");

            // 2. Si es bala viva, aplicamos el daño
            if (isLive)
            {
                int damage = _context.CurrentDamageMultiplier;
                if (target == Target.Player) _context.PlayerHealth -= damage;
                else if (target == Target.Dealer) _context.DealerHealth -= damage;
            }

            // Reseteamos siempre el cañón de la escopeta a la normalidad después de apretar el gatillo
            _context.CurrentDamageMultiplier = 1;

            // 3. Consultamos el reglamento para saber de quién es el siguiente turno
            Type nextState = _ruleEvaluator.EvaluateNextTurn(_context, isLive);

            // 4. Disparamos la animación visual
            var visual = Object.FindFirstObjectByType<Project.Visuals.VisualController>();
            if (visual != null)
            {
                // La vista hace la pausa dramática y luego nos dice "ya puedes continuar"
                visual.AnimateShot(target, isLive, () =>
                {
                    if (nextState != null) _stateMachine.ChangeState(nextState);
                });
            }
            else
            {
                // Fallback por si acaso
                if (nextState != null) _stateMachine.ChangeState(nextState);
            }
        }

        public void Exit()
        {
            // Limpiamos la mira para la próxima jugada
            _context.CurrentTarget = Target.None;
        }
    }
}