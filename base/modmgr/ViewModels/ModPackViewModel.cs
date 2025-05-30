using System;

namespace Fahrenheit.Core.ModManager.ViewModels;

public class ModPackViewModel {
    public string name { get; } = "Mod Pack";
    public int mod_count { get; } = 3;
    public string last_played { get; } = DateTime.Now.ToShortDateString();
    public string playtime { get; } = new DateTime().AddHours(12).AddMinutes(34).AddSeconds(56).ToString("H'h 'm'm 's's'");
}
