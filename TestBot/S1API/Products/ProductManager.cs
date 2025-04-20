#if (IL2CPP)
using S1Product = Il2CppScheduleOne.Product;
using Il2CppSystem.Collections.Generic;
#elif (MONO)
using S1Product = ScheduleOne.Product;
#endif
using System.Linq;

namespace S1API.Products
{
    /// <summary>
    /// Provides management over all products in the game.
    /// </summary>
    public static class ProductManager
    {
        /// <summary>
        /// A list of product definitions discovered on this save.
        /// </summary>
        public static ProductDefinition[] DiscoveredProducts => S1Product.ProductManager.DiscoveredProducts.ToArray()
            .Select(productDefinition => new ProductDefinition(productDefinition)).ToArray();
    }
}