using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fahrenheit.CoreLib;

public abstract record FhConfigStruct {
    protected FhConfigStruct(string configName) {
        Type       = GetType().FullName ?? throw new Exception("E_CONFSTRUCT_TYPE_UNIDENTIFIABLE");
        ConfigName = configName;
    }

    public string Type       { get; }
    public string ConfigName { get; }
}

public abstract record FhModuleConfig(string ConfigName, bool ConfigEnabled) : FhConfigStruct(ConfigName) {
    public abstract FhModule SpawnModule();
}

public class FhConfigParser<T> : JsonConverter<T> where T : FhModuleConfig {
    public override bool CanConvert(Type objtype) {
        return typeof(FhModuleConfig).IsAssignableFrom(objtype);
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        Utf8JsonReader readerClone = reader;

        readerClone.enter_json_object();
        readerClone.strict_resolve_descendant_of<T>(typeToConvert, out Type actualType);

        if (JsonSerializer.Deserialize(ref reader, actualType, FhUtil.InternalJsonOpts) is not T t) {
            throw new JsonException("E_CONFIG_DESERIALIZE_TO_DERIVED_TYPE_FAILED");
        }

        return t;
    }

    /* [fkelava 9/10/2024 19:52]
     * We serialize to <object> to ensure all fields are written out, not just derived class fields.
     */
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
        JsonSerializer.Serialize<object>(writer, value, options);
    }
}
