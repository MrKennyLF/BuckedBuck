using System;
using System.Collections.Generic;

namespace Project.Core
{
    public enum Target { None, Player, Dealer }
    public enum TurnOwner { None, Player, Dealer }

    public class GameContext
    {
        // Escopeta
        private List<bool> _shotgunChamber = new List<bool>();
        public IReadOnlyList<bool> ShotgunChamber => _shotgunChamber;

        // Inventarios
        private List<IItem> _playerInventory = new List<IItem>();
        private List<IItem> _dealerInventory = new List<IItem>();

        public IReadOnlyList<IItem> PlayerInventory => _playerInventory;
        public IReadOnlyList<IItem> DealerInventory => _dealerInventory;

        // Variables de estado generales
        public Target CurrentTarget { get; set; }
        public TurnOwner CurrentTurnOwner { get; set; }

        // Multiplicador de daño (Para la Sierra)
        public int CurrentDamageMultiplier { get; set; } = 1;

        // --- EVENTOS REACTIVOS ---
        public event Action<TurnOwner, IItem> OnItemAdded;
        public event Action<TurnOwner, IItem> OnItemConsumed;
        public event Action<string, bool, Action> OnItemAnimationRequested;

        public event Action<int, int> OnHealthChanged;
        public event Action<int, int> OnChamberLoaded;

        // NUEVO EVENTO: Para avisar a la UI del final del juego
        public event Action<string> OnGameOver;

        // --- PROPIEDADES REACTIVAS DE SALUD ---
        private int _playerHealth = 4;
        public int PlayerHealth
        {
            get => _playerHealth;
            set
            {
                _playerHealth = value;
                OnHealthChanged?.Invoke(_playerHealth, _dealerHealth);
            }
        }

        private int _dealerHealth = 4;
        public int DealerHealth
        {
            get => _dealerHealth;
            set
            {
                _dealerHealth = value;
                OnHealthChanged?.Invoke(_playerHealth, _dealerHealth);
            }
        }

        public GameContext()
        {
            CurrentTarget = Target.None;
            CurrentTurnOwner = TurnOwner.None;
            CurrentDamageMultiplier = 1;
        }

        // --- MÉTODOS DE ESCOPETA ---
        public void LoadChamber(List<bool> cartridges)
        {
            _shotgunChamber = cartridges;
            int vivas = cartridges.FindAll(b => b == true).Count;
            int fogueo = cartridges.Count - vivas;
            OnChamberLoaded?.Invoke(vivas, fogueo);
        }

        public bool ExtractNextRound()
        {
            if (_shotgunChamber.Count == 0) return false;
            bool round = _shotgunChamber[0];
            _shotgunChamber.RemoveAt(0);
            return round;
        }

        public bool PeekNextRound()
        {
            if (_shotgunChamber.Count == 0) return false;
            return _shotgunChamber[0];
        }

        // --- MÉTODOS DE INVENTARIO ---
        public void AddItem(TurnOwner owner, IItem item)
        {
            int maxInventorySize = 8;
            if (owner == TurnOwner.Player && _playerInventory.Count < maxInventorySize)
            {
                _playerInventory.Add(item);
                OnItemAdded?.Invoke(owner, item);
            }
            else if (owner == TurnOwner.Dealer && _dealerInventory.Count < maxInventorySize)
            {
                _dealerInventory.Add(item);
                OnItemAdded?.Invoke(owner, item);
            }
        }

        public void ConsumeItem(TurnOwner owner, IItem item)
        {
            if (owner == TurnOwner.Player && _playerInventory.Remove(item))
            {
                OnItemConsumed?.Invoke(owner, item);
            }
            else if (owner == TurnOwner.Dealer && _dealerInventory.Remove(item))
            {
                OnItemConsumed?.Invoke(owner, item);
            }
        }

        // --- PUENTES ASÍNCRONOS Y DE UI ---
        public void RequestItemAnimation(string itemId, bool extraData, Action onComplete)
        {
            OnItemAnimationRequested?.Invoke(itemId, extraData, onComplete);
        }

        public void TriggerGameOver(string winnerMessage)
        {
            OnGameOver?.Invoke(winnerMessage);
        }
    }
}