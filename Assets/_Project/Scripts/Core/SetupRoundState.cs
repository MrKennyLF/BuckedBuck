using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core
{
    public class SetupRoundState : IGameState
    {
        private readonly TurnStateMachine _stateMachine;
        private readonly GameContext _context;
        private readonly IItemFactory _itemFactory;

        public SetupRoundState(TurnStateMachine stateMachine, GameContext context, IItemFactory itemFactory)
        {
            _stateMachine = stateMachine;
            _context = context;
            _itemFactory = itemFactory;
        }

        public void Enter()
        {
            Debug.Log("<color=magenta>--- PREPARANDO NUEVA RONDA ---</color>");
            Execute();
        }

        public void Execute()
        {
            // 1. Generamos una carga de balas totalmente aleatoria
            List<bool> newChamber = GenerateRandomCartridges();
            _context.LoadChamber(newChamber);

            Debug.Log($"[Setup] Escopeta cargada con {newChamber.Count} balas.");

            // 2. Repartimos solo 2 objetos por ronda a cada quien
            // (Tu GameContext ya tiene la lógica de que el máximo es 8, así que no se desbordará)
            _context.AddItem(TurnOwner.Player, _itemFactory.GetRandomItem());
            _context.AddItem(TurnOwner.Player, _itemFactory.GetRandomItem());

            _context.AddItem(TurnOwner.Dealer, _itemFactory.GetRandomItem());
            _context.AddItem(TurnOwner.Dealer, _itemFactory.GetRandomItem());

            // 3. El jugador siempre empieza cuando se recarga la escopeta
            _stateMachine.ChangeState(typeof(PlayerTurnState));
        }

        // Algoritmo para generar y mezclar las balas
        private List<bool> GenerateRandomCartridges()
        {
            List<bool> cartridges = new List<bool>();
            System.Random rng = new System.Random();

            // Generamos un total aleatorio de balas entre 2 y 6
            int totalBullets = rng.Next(2, 7);

            // Aseguramos que al menos haya 1 viva y 1 fogueo para que sea divertido
            cartridges.Add(true);
            cartridges.Add(false);

            // Rellenamos el resto al azar (true = viva, false = fogueo)
            for (int i = 2; i < totalBullets; i++)
            {
                cartridges.Add(rng.Next(2) == 0);
            }

            // Mezclamos la lista como si fuera una baraja (Algoritmo Fisher-Yates)
            int n = cartridges.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                bool value = cartridges[k];
                cartridges[k] = cartridges[n];
                cartridges[n] = value;
            }

            return cartridges;
        }

        public void Exit() { }
    }
}