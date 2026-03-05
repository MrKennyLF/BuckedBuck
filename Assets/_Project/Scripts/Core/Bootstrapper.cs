using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Core
{
    public class Bootstrapper : MonoBehaviour
    {
        private GameContext _context;
        private TurnStateMachine _stateMachine;

        private void Start()
        {
            // 1. Hacemos que este objeto sobreviva al cambio de escena
            DontDestroyOnLoad(this.gameObject);

            // 2. Iniciamos el proceso de carga de la siguiente escena
            StartCoroutine(LoadGameSceneAndInject());
        }

        private IEnumerator LoadGameSceneAndInject()
        {
            // 3. Creamos la memoria pura (esto ocurre aún en la escena Init)
            _context = new GameContext();

            // 4. Cargamos la escena principal 
            // IMPORTANTE: Asegúrate de que tu escena se llame exactamente "MainGame"
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainGame");

            while (!asyncLoad.isDone)
            {
                yield return null; // Esperamos a que la escena cargue por completo
            }

            Debug.Log("[Bootstrapper] Escena MainGame cargada. Buscando dependencias...");

            // 5. Buscamos los componentes en la nueva escena dinámicamente
            var visualController = Object.FindFirstObjectByType<Project.Visuals.VisualController>();
            var uiController = Object.FindFirstObjectByType<Project.Visuals.GameUIController>();
            var inputReader = Object.FindFirstObjectByType<InputReader>();

            if (visualController == null || uiController == null || inputReader == null)
            {
                Debug.LogError("[Bootstrapper] Faltan controladores en la escena MainGame. Revisa que VisualController, GameUIController e InputReader estén en la jerarquía.");
                yield break;
            }

            // 6. Inyectamos y conectamos los cables
            InjectDependencies(visualController, uiController, inputReader);
        }

        private void InjectDependencies(Project.Visuals.VisualController visualController, Project.Visuals.GameUIController uiController, InputReader inputReader)
        {
            _stateMachine = new TurnStateMachine();

            // --- FÁBRICAS E IA ---
            IItemFactory itemFactory = new StandardItemFactory();
            ITurnRuleEvaluator ruleEvaluator = new BuckshotRules();
            IDealerAI dealerAI = new ProbabilisticDealerAI();

            // --- INSTANCIACIÓN DE ESTADOS ---
            var setupState = new SetupRoundState(_stateMachine, _context, itemFactory);
            var playerState = new PlayerTurnState(_stateMachine, _context, inputReader);
            var resolutionState = new ActionResolutionState(_stateMachine, _context, ruleEvaluator);
            var dealerState = new DealerTurnState(_stateMachine, _context, dealerAI);
            var gameOverState = new GameOverState(_stateMachine, _context);
            _stateMachine.AddState(setupState);
            _stateMachine.AddState(playerState);
            _stateMachine.AddState(resolutionState);
            _stateMachine.AddState(dealerState);
            _stateMachine.AddState(gameOverState);
            // --- ENLACE DE EVENTOS VISUALES ---
            _context.OnItemAdded += visualController.HandleItemAdded;
            _context.OnItemConsumed += visualController.HandleItemConsumed;
            _context.OnItemAnimationRequested += visualController.PlayItemAnimation;

            // --- ENLACE DE EVENTOS UI ---
            _context.OnHealthChanged += uiController.HandleHealthChanged;
            _context.OnChamberLoaded += uiController.HandleChamberLoaded;

            // EL NUEVO CABLE:
            _context.OnGameOver += uiController.HandleGameOver;

            // Forzamos la primera actualización de la UI al arrancar
            uiController.HandleHealthChanged(_context.PlayerHealth, _context.DealerHealth);

            // --- INICIO DE LA MÁQUINA ---
            _stateMachine.ChangeState(typeof(SetupRoundState));
        }
    }
}