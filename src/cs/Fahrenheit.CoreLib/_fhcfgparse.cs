using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fahrenheit.CoreLib;

/* [fkelava 27/2/23 00:02]
 * The core must be able to resolve descendants of FhModuleConfig in assemblies loaded at runtime.
 * 
 * If all descendants of FhModuleConfig contain a [JsonConstructor] or are otherwise capable of being
 * constructed from a simple JsonSerializer.Deserialize call, then we can search loaded assemblies
 * at runtime to resolve the actual derived type and obtain an instance of it with no special wrangling.
 *
 * See StrictResolveDescendantOf<T> for the actual type-matching mechanism.
 */
public class FhConfigParser<T> : JsonConverter<T> where T : FhModuleConfig
{
    public override bool CanConvert(Type objtype)
    {
        return typeof(FhModuleConfig).IsAssignableFrom(objtype);
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Utf8JsonReader readerClone = reader;

        readerClone.EnterObject();
        readerClone.StrictResolveDescendantOf<T>(typeToConvert, out Type actualType);

        if (JsonSerializer.Deserialize(ref reader, actualType, FhUtil.InternalJsonOpts) is not T t)
        {
            throw new JsonException("E_CONFIG_DESERIALIZE_TO_DERIVED_TYPE_FAILED");
        }

        return t;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize<object>(writer, value, options);
    }
}