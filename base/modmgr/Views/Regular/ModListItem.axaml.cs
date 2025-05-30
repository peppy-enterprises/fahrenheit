using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Fahrenheit.Core.ModManager.ViewModels;

namespace Fahrenheit.Core.ModManager.Views.Regular;

public partial class ModListItem : UserControl {
    public ModListItem() {
        InitializeComponent();
        DataContext = new ModViewModel();
    }
}

