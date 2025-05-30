using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Fahrenheit.Core.ModManager.ViewModels;

namespace Fahrenheit.Core.ModManager.Views.Regular;

public partial class ModPack : UserControl {
    public ModPack() {
        InitializeComponent();
        DataContext = new ModPackViewModel();
    }
}

