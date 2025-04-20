#if (IL2CPP)
using S1Product = Il2CppScheduleOne.Product;
#elif (MONO)
using S1Product = ScheduleOne.Product;
#endif

using S1API.Internal.Utils;
using S1API.Items;

namespace S1API.Products
{
    /// <summary>
    /// Represents a product definition in the game.
    /// </summary>
    public class ProductDefinition : ItemDefinition
    {
        /// <summary>
        /// INTERNAL: Stored reference to the game product definition.
        /// </summary>
        internal S1Product.ProductDefinition S1ProductDefinition => 
            CrossType.As<S1Product.ProductDefinition>(S1ItemDefinition);
        
        /// <summary>
        /// INTERNAL: Creates a product definition from the in-game product definition.
        /// </summary>
        /// <param name="productDefinition"></param>
        internal ProductDefinition(S1Product.ProductDefinition productDefinition) : base(productDefinition) { }
        
        /// <summary>
        /// The price associated with this product.
        /// </summary>
        public float Price => 
            S1ProductDefinition.Price;
        
        /// <summary>
        /// Creates an instance of this product in-game.
        /// </summary>
        /// <param name="quantity">The quantity of product.</param>
        /// <returns>An instance of the product.</returns>
        public override ItemInstance CreateInstance(int quantity = 1) => 
            new ProductInstance(CrossType.As<S1Product.ProductItemInstance>(S1ProductDefinition.GetDefaultInstance(quantity)));
    }
}