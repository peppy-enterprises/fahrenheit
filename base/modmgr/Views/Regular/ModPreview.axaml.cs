using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Fahrenheit.Core.ModManager.ViewModels;

namespace Fahrenheit.Core.ModManager.Views.Regular;

public partial class ModPreview : UserControl {
    public ModPreview() {
        InitializeComponent();
        DataContext = new ModViewModel();
    }
}

