using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Project.Visuals;

namespace Project.Core
{
    public class Bootstrapper : MonoBehaviour
    {
        private TurnStateMachine _stateMachine;

        private IEnumerator Start()
        {
            DontDestroyOnLoad(gameObject);

            // 1. Carga asíncrona de la escena principal
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainGame");
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // 2. Ensamblaje de arquitectura
            InjectDependencies();
        }

        private void InjectDependencies()
        {
            // Dependencias de Unity
            var inputReader = GetComponent<InputReader>();
            var visualController = FindAnyObjectByType<VisualController>();

            if (visualController == null)
            {
                Debug.LogError("[Error Arquitectónico] No se encontró un VisualController en la escena MainGame.");
                return;
            }

            // Memoria y Cerebro (C# Puro)
            GameContext context = new GameContext();
            _stateMachine = new TurnStateMachine();

            // Estrategias (Reglas e IA)
            ITurnRuleEvaluator ruleEvaluator = new BuckshotRules();
            IDealerAI dealerAI = new ProbabilisticDealerAI();

            IItemFactory itemFactory = new StandardItemFactory();
            // 1. Instanciamos los estados con sus dependencias
            var setupState = new SetupRoundState(_stateMachine, context, itemFactory);

            var playerState = new PlayerTurnState(_stateMachine, context, inputReader);
            var resolutionState = new ActionResolutionState(_stateMachine, context, ruleEvaluator);
            var dealerState = new DealerTurnState(_stateMachine, context, dealerAI);

            // 2. Enlazamos los eventos lógicos a las corrutinas visuales mediante Callbacks
            resolutionState.OnShotFired += (target, isLive) =>
                visualController.AnimateShot(target, isLive, resolutionState.CompleteVisualResolution);

            dealerState.OnDealerThinking += visualController.SimulateDealerThinking;

            // 3. Registramos los estados en la Máquina de Estados
            _stateMachine.AddState(setupState);
            _stateMachine.AddState(playerState);
            _stateMachine.AddState(resolutionState);
            _stateMachine.AddState(dealerState);

            // 4. Arrancamos el flujo
            _stateMachine.ChangeState(typeof(SetupRoundState));
            Debug.Log("[Sistema] Arquitectura ensamblada y enlazada correctamente.");
        }

        private void Update()
        {
            // Bombear vida a la FSM
            _stateMachine?.Update();
        }
    }
}