using System;

namespace Project.Core
{
    public interface IItem
    {
        string Id { get; } // El identificador abstracto
        string Name { get; }

        void Use(GameContext context, Action onComplete);
    }
}