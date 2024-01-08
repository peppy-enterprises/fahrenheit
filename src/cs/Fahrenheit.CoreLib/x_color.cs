namespace Fahrenheit.CoreLib;

public static class FhXFontColor {
	public enum Foreground {
		White = 0x0, // Also 0x04, 0x0A
		LightGray = 0x1,
		Yellow = 0x2,
		Red = 0x3,
		Gray = 0x5, // Really dark
		Pink = 0x6,
		Cyan = 0x7,
		Black = 0x8, // Also 0x09
		ShadowTheHedgehog = 0xB,
		Blue = 0xC,
		Crimson = 0xD,
		DarkBlue = 0xE,
		LightBlue = 0xF,
	}

	public enum Background {
		Black = 0x00, // Also 0x10, 0x80, 0x90
		White = 0x20, // Also 0xA0
		ShadowTheHedgehog = 0x30, // Also 0xB0
		Blue = 0x40, // Also 0xC0
		Crimson = 0x50, // Also 0xD0
		DarkBlue = 0x60, // Also 0xE0
		LightBlue = 0x70, // Also 0xF0
	}

	public static byte create(Foreground fg, Background bg) => (byte)((byte)bg + (byte)fg);
}
