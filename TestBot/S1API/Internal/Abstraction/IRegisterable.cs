namespace S1API.Internal.Abstraction
{
    /// <summary>
    /// INTERNAL: Provides rigidity for registerable instance wrappers.
    /// </summary>
    internal interface IRegisterable
    {
        /// <summary>
        /// INTERNAL: Called upon creation of the instance.
        /// </summary>
        void CreateInternal();
        
        /// <summary>
        /// INTERNAL: Called upon destruction of the instance.
        /// </summary>
        void DestroyInternal();

        /// <summary>
        /// Called upon creation of the instance.
        /// </summary>
        void OnCreated();
        
        /// <summary>
        /// Called upon destruction of the instance.
        /// </summary>
        void OnDestroyed();
    }
}