using System;

using Fahrenheit.Core.ModManager.Views;

namespace Fahrenheit.Core.ModManager.ViewModels;

public partial class MainWindowViewModel : ViewModelBase {
    public bool in_compact_mode { get; set; } = false;
    public bool show_mod_details { get; set; } = true;
}
