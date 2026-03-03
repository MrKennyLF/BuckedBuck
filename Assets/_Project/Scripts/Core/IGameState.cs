namespace Project.Core
{
    public interface IGameState
    {
        // Se ejecuta una sola vez al entrar al estado (ej. Cargar la escopeta)
        void Enter();

        // Se ejecuta cada frame o ciclo de decisión si es necesario
        void Execute();

        // Se ejecuta justo antes de cambiar a otro estado (ej. Limpiar variables)
        void Exit();
    }
}