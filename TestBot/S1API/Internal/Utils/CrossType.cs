#if (MONO)
using System;
# elif (IL2CPP)
using Il2CppSystem;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
#endif

namespace S1API.Internal.Utils
{
    /// <summary>
    /// INTERNAL: Provide cross compatibility methods to assist between
    /// Mono and Il2Cpp. Meant for use only inside this API.
    /// </summary>
    internal static class CrossType
    {
        /// <summary>
        /// Gets the proper type of class.
        /// </summary>
        /// <typeparam name="T">A Il2Cpp or Mono class.</typeparam>
        /// <returns>The type of the class.</returns>
        internal static Type Of<T>()
        {
#if (MONO)
            return typeof(T);
# elif (IL2CPP)
            return Il2CppType.Of<T>();
#endif
        }

        /// <summary>
        /// Checks for if an object is of a type.
        /// </summary>
        /// <param name="obj">Object to perform check on.</param>
        /// <param name="result">A resulting cast for obj to type T.</param>
        /// <typeparam name="T">The class we're checking against.</typeparam>
        /// <returns>Whether obj is of class T or not.</returns>
        internal static bool Is<T>(object obj, out T result)
#if (IL2CPP) 
            where T : Il2CppObjectBase
#elif (MONO)
            where T : class
#endif
        {
#if (IL2CPP)
            if (obj is Object il2CppObj)
            {
                Type il2CppType = Il2CppType.Of<T>();
                if (il2CppType.IsAssignableFrom(il2CppObj.GetIl2CppType()))
                {
                    result = il2CppObj.TryCast<T>()!;
                    return true;
                }
            }
#elif (MONO)
            if (obj is T t)
            {
                result = t;
                return true;
            }
#endif

            result = null!;
            return false;
        }

        /// <summary>
        /// Casts an object to a type.
        /// </summary>
        /// <param name="obj">The object to cast.</param>
        /// <typeparam name="T">The type to cast to.</typeparam>
        /// <returns>The object cast to the specified type.</returns>
        internal static T As<T>(object obj)
#if (IL2CPP)
            where T : Il2CppObjectBase
#elif (MONO)
            where T : class
#endif
            =>
#if (IL2CPP)
                obj is Object il2CppObj ? il2CppObj.Cast<T>() : null!;
#elif (MONO)
                (T)obj;
#endif
    }
}