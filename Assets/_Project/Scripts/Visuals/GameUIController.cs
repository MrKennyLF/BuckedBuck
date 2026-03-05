using UnityEngine;
using TMPro;
using UnityEngine.UI; // Necesario para el Botón
using UnityEngine.SceneManagement; // Necesario para reiniciar la escena

namespace Project.Visuals
{
    public class GameUIController : MonoBehaviour
    {
        [Header("Textos de Salud")]
        [SerializeField] private TextMeshProUGUI _playerHealthText;
        [SerializeField] private TextMeshProUGUI _dealerHealthText;

        [Header("Textos de Escopeta")]
        [SerializeField] private TextMeshProUGUI _chamberInfoText;

        [Header("Pantalla Final (Game Over)")]
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _winnerText;
        [SerializeField] private Button _restartButton;

        private void Start()
        {
            // Ocultamos el panel al iniciar por si lo dejaste encendido en el editor
            if (_gameOverPanel != null) _gameOverPanel.SetActive(false);

            // Le decimos al botón qué función ejecutar al hacerle clic
            if (_restartButton != null)
            {
                _restartButton.onClick.AddListener(RestartGame);
            }
        }

        public void HandleHealthChanged(int playerHp, int dealerHp)
        {
            _playerHealthText.text = $"VIDA JUGADOR: {playerHp} / 4";
            _dealerHealthText.text = $"VIDA DEALER: {dealerHp} / 4";
        }

        public void HandleChamberLoaded(int live, int blank)
        {
            _chamberInfoText.text = $"CARGADAS: {live} Vivas | {blank} Fogueo";
        }

        // Esta función se llama cuando el GameOverState dispara el evento
        public void HandleGameOver(string winnerMessage)
        {
            if (_gameOverPanel != null)
            {
                _gameOverPanel.SetActive(true); // Encendemos la pantalla negra
                _winnerText.text = winnerMessage; // Mostramos quién ganó
            }
        }

        private void RestartGame()
        {
            // 1. Buscamos el Bootstrapper inmortal y lo destruimos para que no se duplique
            var bootstrapper = Object.FindFirstObjectByType<Project.Core.Bootstrapper>();
            if (bootstrapper != null)
            {
                Destroy(bootstrapper.gameObject);
            }

            // 2. Recargamos la escena inicial (¡El ciclo de la vida recomienza!)
            SceneManager.LoadScene("Init");
        }
    }
}