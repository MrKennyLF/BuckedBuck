namespace Project.Core
{
    public interface IDealerAI
    {
        // El motor evalúa el contexto y devuelve un objetivo
        Target DecideTarget(GameContext context);
    }
}