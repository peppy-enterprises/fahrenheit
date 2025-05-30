using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Fahrenheit.Core.ModManager.ViewModels;

namespace Fahrenheit.Core.ModManager.Views.Regular;

public partial class ModPackListItem : UserControl {
    public ModPackListItem() {
        InitializeComponent();
        DataContext = new ModPackViewModel();
    }
}

