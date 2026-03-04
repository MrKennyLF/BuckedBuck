using System;
using System.Linq;
using UnityEngine;

namespace Project.Core
{
    public class PlayerTurnState : IGameState
    {
        private TurnStateMachine _stateMachine;
        private GameContext _context;
        private InputReader _inputReader;

        public PlayerTurnState(TurnStateMachine stateMachine, GameContext context, InputReader inputReader)
        {
            _stateMachine = stateMachine;
            _context = context;
            _inputReader = inputReader;
        }

        public void Enter()
        {
            Debug.Log("[Estado] Turno del Jugador.");
            _context.CurrentTurnOwner = TurnOwner.Player;
            EnableInput();
        }

        public void Execute() { }

        public void Exit()
        {
            DisableInput();
        }

        // Centralizamos la suscripción
        private void EnableInput()
        {
            _inputReader.OnShootDealer += HandleShootDealer;
            _inputReader.OnShootSelf += HandleShootSelf;
            _inputReader.OnUseItem += HandleUseItem; // Nuevo evento hipotético
        }

        // Centralizamos la desuscripción
        private void DisableInput()
        {
            _inputReader.OnShootDealer -= HandleShootDealer;
            _inputReader.OnShootSelf -= HandleShootSelf;
            _inputReader.OnUseItem -= HandleUseItem;
        }

        private void HandleShootDealer()
        {
            DisableInput(); // Apagamos controles inmediatamente
            _context.CurrentTarget = Target.Dealer;
            _stateMachine.ChangeState(typeof(ActionResolutionState));
        }

        private void HandleShootSelf()
        {
            DisableInput(); // Apagamos controles inmediatamente
            _context.CurrentTarget = Target.Player;
            _stateMachine.ChangeState(typeof(ActionResolutionState));
        }

        private void HandleUseItem()
        {
            DisableInput(); // El jugador ya no puede disparar ni usar otro objeto

            // Lógica temporal para probar la Lupa
            IItem magnifier = new MagnifyingGlassItem();

            Debug.Log($"[Acción] Usando {magnifier.Name}...");

            // Pasamos el contexto y el Callback que reactivará los controles
            magnifier.Use(_context, EnableInput);
        }
        private void HandleItemUseRequested(IItem itemInstance)
        {
            // Validamos por seguridad que el jugador realmente posea esta instancia exacta
            // (Contains compara por referencia en memoria, es exacto y O(N) seguro para listas de 8 items)
            if (!_context.PlayerInventory.Contains(itemInstance)) return;

            DisableInput();

            // Aquí el IItem se encarga de aplicar su efecto y llamar al callback
            itemInstance.Use(_context, EnableInput);
        }
    }
}