using Avalonia.Controls;
using Avalonia.Interactivity;

using Fahrenheit.Core.ModManager.ViewModels;

namespace Fahrenheit.Core.ModManager.Views;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    public void toggle_compact_mode(object? sender, RoutedEventArgs e) {
        if (DataContext is not MainWindowViewModel vm) return;

        vm.in_compact_mode = !vm.in_compact_mode;
        CompactModeCheckBox.IsChecked = vm.in_compact_mode; // Surely there's a better way?
    }

    public void toggle_mod_details(object? sender, RoutedEventArgs e) {
        if (DataContext is not MainWindowViewModel vm) return;

        vm.show_mod_details = !vm.show_mod_details;
        ModDetailsCheckBox.IsChecked = vm.show_mod_details; // Surely there's a better way?
    }
}
