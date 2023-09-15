namespace Fahrenheit.CoreLib;

public static class FhXFontColor {
	public enum Foreground {
		White = 0x00, // Also 0x04, 0x0A
		LightGray = 0x01,
		Yellow = 0x02,
		Red = 0x03,
		Gray = 0x05, // Really dark
		Pink = 0x06,
		Cyan = 0x07,
		Black = 0x08, // Also 0x09
		ShadowTheHedgehog = 0x0B,
		Blue = 0x0C,
		Crimson = 0x0D,
		DarkBlue = 0x0E,
		LightBlue = 0x0F,
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
