namespace Fahrenheit.Core;

public abstract record FhConfigStruct {
    protected FhConfigStruct(string name) {
        Type = GetType().FullName ?? throw new Exception("FH_E_CONFSTRUCT_TYPE_UNIDENTIFIABLE");
        Name = name;
    }

    public string Type { get; }
    public string Name { get; }
}

public abstract record FhModuleConfig(string Name) : FhConfigStruct(Name) {
    public abstract FhModule SpawnModule();
}

public class FhConfigParser<T> : JsonConverter<T> where T : FhModuleConfig {
    public override bool CanConvert(Type objtype) {
        return typeof(FhModuleConfig).IsAssignableFrom(objtype);
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        Utf8JsonReader reader_clone = reader;
        reader_clone.enter_json_object();

        if (!reader_clone.resolve_descendant_of(typeToConvert, out Type? actualType)) {
            throw new Exception("FH_E_CONF_TYPE_RESOLUTION_FAILED");
        }

        if (JsonSerializer.Deserialize(ref reader, actualType, FhUtil.InternalJsonOpts) is not T t) {
            throw new JsonException("FH_E_CONF_TYPE_CAST_FAILED");
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
