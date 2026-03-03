using System;

namespace Project.Core
{
    public interface IItem
    {
        string Name { get; }

        // Recibe el contexto para leer datos, y un callback para avisar cuando terminó de usarse
        void Use(GameContext context, Action onComplete);
    }
}