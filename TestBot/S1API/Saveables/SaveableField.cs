using System;

namespace S1API.Saveables
{
    /// <summary>
    /// Marks a field to be saved alongside the class instance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SaveableField : Attribute
    {
        /// <summary>
        /// What the save data should be named.
        /// </summary>
        internal string SaveName { get; }

        public SaveableField(string saveName)
        {
            SaveName = saveName;
        }
    }
}