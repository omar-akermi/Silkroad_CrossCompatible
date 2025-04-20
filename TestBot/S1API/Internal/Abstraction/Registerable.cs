namespace S1API.Internal.Abstraction
{
    /// <summary>
    /// INTERNAL: A registerable base class for use internally.
    /// Not intended for modder use.
    /// </summary>
    public abstract class Registerable : IRegisterable
    {
        /// <summary>
        /// TODO
        /// </summary>
        void IRegisterable.CreateInternal() =>
            CreateInternal();
        
        /// <summary>
        /// TODO
        /// </summary>
        internal virtual void CreateInternal() =>
            OnCreated();
        
        /// <summary>
        /// TODO
        /// </summary>
        void IRegisterable.DestroyInternal() => 
            DestroyInternal();
        
        /// <summary>
        /// TODO
        /// </summary>
        internal virtual void DestroyInternal() =>
            OnDestroyed();
        
        /// <summary>
        /// TODO
        /// </summary>
        void IRegisterable.OnCreated() => 
            OnCreated();

        /// <summary>
        /// TODO
        /// </summary>
        protected virtual void OnCreated() { }
        
        /// <summary>
        /// TODO
        /// </summary>
        void IRegisterable.OnDestroyed() => 
            OnDestroyed();

        /// <summary>
        /// TODO
        /// </summary>
        protected virtual void OnDestroyed() { }
    }
}