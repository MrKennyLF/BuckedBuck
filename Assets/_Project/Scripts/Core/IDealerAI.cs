namespace Project.Core
{
    public interface IDealerAI
    {
        Target DecideTarget(GameContext context);

        // NUEVO: El cerebro debe saber elegir un objeto de su inventario
        IItem DecideItemToUse(GameContext context);
    }
}