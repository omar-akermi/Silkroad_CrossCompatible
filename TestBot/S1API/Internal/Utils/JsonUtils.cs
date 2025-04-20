
using Newtonsoft.Json;
using System;

public static class JsonBridge
{
    public static TNative DeserializeToNative<TWrapper, TNative>(string json, Func<TWrapper, TNative> mapper)
        where TWrapper : class
        where TNative : Il2CppSystem.Object
    {
        TWrapper? wrapper = JsonConvert.DeserializeObject<TWrapper>(json);
        if (wrapper == null)
            throw new Exception($"Failed to deserialize JSON into wrapper type {typeof(TWrapper).Name}");
        return mapper(wrapper);
    }

    public static string SerializeFromNative<TWrapper, TNative>(TNative native, Func<TNative, TWrapper> mapper)
        where TWrapper : class
        where TNative : Il2CppSystem.Object
    {
        TWrapper wrapper = mapper(native);
        return JsonConvert.SerializeObject(wrapper, Formatting.Indented);
    }
}
