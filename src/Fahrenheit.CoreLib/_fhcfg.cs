using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fahrenheit.CoreLib;

/// <summary>
///     The ultimate base class for any configuration struct in Fahrenheit. Primarily used for non-module
///     related configurations, or configurations which are not subject to enable/disable mechanisms.
///  <br></br>
///     _All_ configuration structs, without exception, have the <see cref="Type"/> field set to typeof(T).FullName
///     to permit their polymorphic deserialization. See <see cref="FhConfigParser{T}"/>.
/// </summary>
public abstract record FhConfigStruct {
    protected FhConfigStruct(string configName,
                             uint   configVersion) {
        Type          = GetType().FullName ?? throw new Exception("E_CONFSTRUCT_TYPE_UNIDENTIFIABLE");
        ConfigName    = configName;
        ConfigVersion = configVersion;
    }

    public string Type          { get; }
    public string ConfigName    { get; }
    public uint   ConfigVersion { get; }
}

public sealed record FhModuleConfigCollection(List<FhModuleConfig> ModuleConfigs);

/// <summary>
///     The base class for a <see cref="FhModule"/>'s configuration. In IoT Core, <see cref="FhModuleConfig"/>s spawn <see cref="FhModule"/>s,
///     and each <see cref="FhModule"/> _must_ have a matching <see cref="FhModuleConfig"/>. Its fields are arbitrary with the exception
///     of three mandatory fields:
/// <para></para>
///     1) ConfigName, which uniquely identifies both configuration _and_ module!<br></br>
///     2) ConfigVersion, which is available to you for versioning scenarios.<br></br>
///     3) ConfigEnabled, which instructs the <see cref="IFhModuleController"/> not to spawn the module.
/// </summary>
public abstract record FhModuleConfig(string ConfigName, uint ConfigVersion, bool ConfigEnabled) : FhConfigStruct(ConfigName, ConfigVersion) {
    public abstract bool TrySpawnModule([NotNullWhen(true)] out FhModule? fm);
}

/* [fkelava 27/2/23 00:02]
 * The core must be able to resolve descendants of FhModuleConfig in assemblies loaded at runtime.
 *
 * If all descendants of FhModuleConfig contain a [JsonConstructor] or are otherwise capable of being
 * constructed from a simple JsonSerializer.Deserialize call, then we can search loaded assemblies
 * at runtime to resolve the actual derived type and obtain an instance of it with no special wrangling.
 *
 * See StrictResolveDescendantOf<T> for the actual type-matching mechanism.
 */
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

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
        JsonSerializer.Serialize<object>(writer, value, options);
    }
}