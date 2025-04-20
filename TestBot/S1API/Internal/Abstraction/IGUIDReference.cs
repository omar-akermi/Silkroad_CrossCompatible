namespace S1API.Internal.Abstraction
{
    /// <summary>
    /// INTERNAL: Represents a class that should serialize by GUID instead of values directly.
    /// This is important to utilize on instances such as dead drops, item definitions, etc.
    /// </summary>
    internal interface IGUIDReference
    {
        /// <summary>
        /// The GUID associated with the object.
        /// </summary>
        public string GUID { get; }
    }
}