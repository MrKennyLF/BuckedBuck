using System;
using UnityEngine;

namespace Project.Core
{
    public class InputReader : MonoBehaviour
    {
        private GameControls _controls;

        public Action OnUseItem { get; internal set; }

        // Eventos nativos de C# a los que la FSM se suscribirá
        public event Action OnShootDealer;
        public event Action OnShootSelf;

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new GameControls();
                _controls.Player.ShootDealer.performed += ctx => OnShootDealer?.Invoke();
                _controls.Player.ShootSelf.performed += ctx => OnShootSelf?.Invoke();
            }
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }
    }
}