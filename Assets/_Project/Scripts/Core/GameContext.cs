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

        // Variables de estado
        public int PlayerHealth { get; set; }
        public int DealerHealth { get; set; }
        public Target CurrentTarget { get; set; }
        public TurnOwner CurrentTurnOwner { get; set; }

        // Eventos Reactivos
        public event Action<TurnOwner, IItem> OnItemAdded;
        public event Action<TurnOwner, IItem> OnItemConsumed;
        public event Action<string, bool, Action> OnItemAnimationRequested;

        public GameContext()
        {
            PlayerHealth = 4;
            DealerHealth = 4;
            CurrentTarget = Target.None;
            CurrentTurnOwner = TurnOwner.None;
        }

        // --- MÉTODOS DE ESCOPETA ---
        public void LoadChamber(List<bool> cartridges)
        {
            _shotgunChamber = cartridges;
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

        // --- PUENTE ASÍNCRONO ---
        public void RequestItemAnimation(string itemId, bool extraData, Action onComplete)
        {
            OnItemAnimationRequested?.Invoke(itemId, extraData, onComplete);
        }
    }
}