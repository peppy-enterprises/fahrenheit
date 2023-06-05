using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fahrenheit.CoreLib;

/// <summary>
///     The ultimate base class for any configuration struct in Fahrenheit. Primarily used for non-module
///     related configurations, or configurations which are not subject to enable/disable mechanisms.
///  <br></br>
///     _All_ configuration structs, without exception, have the <see cref="Type"/> field set to typeof(T).FullName
///     to permit their polymorphic deserialization. See <see cref="FhConfigParser{T}"/>.
/// </summary>
public abstract record FhConfigStruct
{
    protected FhConfigStruct(string configName,
                             uint   configVersion)
    {
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
///     3) ConfigEnabled, which instructs the <see cref="IFiModuleController"/> not to spawn the module.
/// </summary>
public abstract record FhModuleConfig(string ConfigName, uint ConfigVersion, bool ConfigEnabled) : FhConfigStruct(ConfigName, ConfigVersion)
{
    public abstract bool TrySpawnModule(IFhModuleController parent, [NotNullWhen(true)] out FhModule? fm);
}

public abstract record FhSerializerConfig;