global using System;                          // primitives
global using System.Collections.Generic;      // List<T>, Dictionary<T,U> and others
global using System.Diagnostics.CodeAnalysis; // [NotNullWhen()] and other nullability static analysis attributes
global using System.Globalization;            // CultureInfo.InvariantCulture et al.
global using System.IO;                       // Path, File, and similar
global using System.Numerics;                 // generic math - INumber<T>, IBinaryInteger<T>
global using System.Reflection;               // Assembly
global using System.Runtime.CompilerServices; // [InlineArray]
global using System.Runtime.InteropServices;  // [DllImport], [LibraryImport], et al.
global using System.Runtime.Loader;           // AssemblyLoadContext, AssemblyDependencyResolver
global using System.Text.Json;                // For JSON (de)serialization, we use STJ.
global using System.Text.Json.Serialization;
global using System.Threading;                // Lock