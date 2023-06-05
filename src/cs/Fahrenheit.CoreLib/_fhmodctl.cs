using System;
using System.Collections.Generic;

namespace Fahrenheit.CoreLib;

/// <summary>
///     Declares that the implementing class is capable of managing <see cref="FhModule"/>s.
/// </summary>
public interface IFhModuleController
{
    public IEnumerable<FhModule> FindAll();

    public TModule? Find<TModule>() where TModule : FhModule;

    public TModule? Find<TModule>(Predicate<TModule> match) where TModule : FhModule;

    public IEnumerable<TModule> FindAll<TModule>() where TModule : FhModule;

    public IEnumerable<TModule> FindAll<TModule>(Predicate<TModule> match) where TModule : FhModule;

    public IEnumerable<bool> StartAll();

    public bool Start(FhModule fm);

    public IEnumerable<bool> Start(IEnumerable<FhModule> fms);

    public IEnumerable<bool> StopAll();

    public bool Stop(FhModule fm);

    public IEnumerable<bool> Stop(IEnumerable<FhModule> fms);

    public bool RegisterModuleDependency(FhModule caller, FhModule target);

    public bool UnregisterModuleDependency(FhModule caller, FhModule target);

    public void ModuleStateChangeHandler(object? sender, FiModuleStateChangeEventArgs e);

    public bool SaveFileToRunDir(string filePath);
}
