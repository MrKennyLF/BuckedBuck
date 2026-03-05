using System;

namespace Project.Core
{
    public class StandardItemFactory : IItemFactory
    {
        private readonly Func<IItem>[] _availableItems = new Func<IItem>[]
        {
            () => new MagnifyingGlassItem(),
            () => new CigaretteItem(),
            () => new BeerItem(),     // <--- NUEVO
            () => new HandSawItem()   // <--- NUEVO
        };
        // ¡Aquí está el nombre correcto!
        public IItem GetRandomItem()
        {
            Random random = new Random();
            int index = random.Next(_availableItems.Length);
            return _availableItems[index]();
        }
    }
}