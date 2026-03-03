using System.Collections.Generic;
using UnityEngine;

namespace Project.Core
{
    public class SetupRoundState : IGameState
    {
        private TurnStateMachine _stateMachine;
        private GameContext _context;
        private IItemFactory _itemFactory; // Nueva dependencia

        public SetupRoundState(TurnStateMachine stateMachine, GameContext context, IItemFactory itemFactory)
        {
            _stateMachine = stateMachine;
            _context = context;
            _itemFactory = itemFactory;
        }

        public void Enter()
        {
            Debug.Log("[Estado] Entrando a SetupRoundState");
            GenerateCartridges();
            DistributeItems(); // Nuevo paso

            _stateMachine.ChangeState(typeof(PlayerTurnState));
        }

        public void Execute() { }
        public void Exit() { }

        private void GenerateCartridges()
        {
            List<bool> newCartridges = new List<bool> { true, false, true }; // Hardcodeo temporal
            _context.LoadChamber(newCartridges);
        }

        private void DistributeItems()
        {
            // Regla de Buckshot: Se reparten de 1 a 4 objetos por ronda
            int itemsToDistribute = UnityEngine.Random.Range(1, 5);

            for (int i = 0; i < itemsToDistribute; i++)
            {
                _context.AddItem(TurnOwner.Player, _itemFactory.GetRandomItem());
                _context.AddItem(TurnOwner.Dealer, _itemFactory.GetRandomItem());
            }

            Debug.Log($"[Sistema] Repartidos {itemsToDistribute} objetos a cada participante.");
        }
    }
}