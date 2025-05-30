namespace Fahrenheit.Core.ModManager.ViewModels;

public class ModViewModel {
    public string name { get; } = "Mod Name";
    public string authors { get; } = "Mod Authors";
    public string description { get; } = "This is my super cool mod description!!";

    //TODO: Load readme from github? I guess?
    // If missing (or otherwise we decide not to), should default to $"# {name}\n##### by {authors}\n{description}"
    public string readme { get; } = "# My Super Cool Mod\n__Supports__ ***markdown!***";
    public string version { get; } = "1.0.0";
    public bool enabled { get; set; } = true;
    public bool installed { get; set; } = false;

    //TODO: Refactor these to a smarter system than a path
    public string icon { get; } = "../../Assets/icons/no_icon.svg";
    public string banner { get; } = "../../Assets/no_banner.png";

    //TODO: Refactor this to a smarter system than a path
    //TODO: Move this somewhere more logical, since it's not mod-specific apart from the `installed` bool
    public string install_remove_icon => $"../../Assets/icons/{(installed ? "remove" : "download")}.svg";

    public void install_remove() {
        if (installed) remove();
        else install();
    }

    public void install() {
        installed = true;
    }

    public void remove() {
        installed = false;
    }
}
