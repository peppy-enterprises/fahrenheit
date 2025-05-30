using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using Fahrenheit.Core.ModManager.ViewModels;

namespace Fahrenheit.Core.ModManager.Views.Regular;

public partial class InstallListItem : UserControl {
    public InstallListItem() {
        InitializeComponent();
        DataContext = new ModViewModel();
    }

    public void install_remove(object? sender, RoutedEventArgs e) {
        if (DataContext is not ModViewModel vm) return;

        //TODO: Probably spin up a thread here so the download goes smoothly async, icon path should only change after it's done
        //TODO: Icon should probably be a spinner while it's downloading, or we can do a fancy fill of the download arrow if we figure that out
        vm.install_remove();
        InstallRemoveIcon.Path = vm.install_remove_icon;
    }
}

