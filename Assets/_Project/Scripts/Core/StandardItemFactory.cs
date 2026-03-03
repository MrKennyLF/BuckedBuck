using System;

namespace Project.Core
{
    public class StandardItemFactory : IItemFactory
    {
        // Registro de los constructores de todos los items disponibles en el juego
        private readonly Func<IItem>[] _availableItems = new Func<IItem>[]
        {
            () => new MagnifyingGlassItem(),
            // () => new CigaretteItem(),
            // () => new HandcuffsItem()
        };

        public IItem GetRandomItem()
        {
            if (_availableItems.Length == 0) return null;

            // UnityEngine.Random está bien aquí porque esta fábrica es el puente de generación
            int index = UnityEngine.Random.Range(0, _availableItems.Length);
            return _availableItems[index]();
        }
    }
}