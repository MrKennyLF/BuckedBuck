using System;
using System.Collections;
using UnityEngine;
using Project.Core;

namespace Project.Visuals
{
    public class VisualController : MonoBehaviour
    {
        // Recibimos la instrucción lógica y un Callback (Action) para avisar cuando terminemos
        public void AnimateShot(Target target, bool isLiveRound, Action onVisualsComplete)
        {
            StartCoroutine(ShotRoutine(target, isLiveRound, onVisualsComplete));
        }

        private IEnumerator ShotRoutine(Target target, bool isLiveRound, Action onVisualsComplete)
        {
            Debug.Log($"[Vista] Iniciando animación de disparo hacia {target}...");

            // Simulamos el tiempo que tardaría una animación, partículas y sonido
            yield return new WaitForSeconds(2.0f);

            Debug.Log($"[Vista] Efectos visuales terminados. El cartucho era {(isLiveRound ? "REAL" : "FOGUEO")}.");

            // Devolvemos el control a la Máquina de Estados (C# Puro)
            onVisualsComplete?.Invoke();
        }
        public void SimulateDealerThinking(Action onThinkingComplete)
        {
            StartCoroutine(DealerThinkingRoutine(onThinkingComplete));
        }

        private IEnumerator DealerThinkingRoutine(Action onThinkingComplete)
        {
            Debug.Log("[Vista] El Dealer está pensando sus probabilidades...");

            // Aquí irían las animaciones del Dealer mirando la escopeta, fumando, etc.
            yield return new WaitForSeconds(2.5f);

            Debug.Log("[Vista] El Dealer ha tomado una decisión.");

            // Devolvemos el control a la FSM
            onThinkingComplete?.Invoke();
        }
    }
}