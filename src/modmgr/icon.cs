// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Decode the Fahrenheit airship mark (assets/fh_base_256.png) into an
 *   OpenGL texture once at startup (see main.cs), and hold the resulting
 *   ImTextureRef for ui_menu.cs to draw via ImGui.Image().
 */

namespace Fahrenheit.Tools.ModManager;

internal static unsafe class FhAppIcon {
    internal static ImTextureRef? Texture { get; private set; }
    internal static Vector2       Size    { get; private set; }

    // Manages its own GL texture outside of Dear ImGui's newer texture-data
    // system (see ImTextureData) rather than registering it there - passing a
    // null ImTextureData* and a plain GL texture name as the ImTextureID is
    // still the supported path for a texture the app uploads and owns itself,
    // which this one-off icon is simple enough to be. Failures are logged and
    // left as Texture == null rather than thrown, the same "don't take down
    // the whole app over a missing/bad asset" treatment main.cs already gives
    // the UI font.
    internal static void load(GL gl) {
        string icon_path = Path.Join(
            AppContext.BaseDirectory,
            "resources",
            "icons",
            "fh_base_256.png");

        if (!File.Exists(icon_path)) {
            Console.WriteLine($"Could not find the Fahrenheit icon: {icon_path}");
            return;
        }

        ImageResult image;

        try {
            using FileStream stream = File.OpenRead(icon_path);

            image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        }
        catch (Exception exception) {
            Console.WriteLine($"Could not decode the Fahrenheit icon: {exception.Message}");
            return;
        }

        uint texture_name = gl.GenTexture();

        gl.BindTexture(GLTextureTarget.Texture2D, texture_name);

        gl.TexParameteri(GLTextureTarget.Texture2D, GLTextureParameterName.MinFilter, (int)GLTextureMinFilter.Linear);
        gl.TexParameteri(GLTextureTarget.Texture2D, GLTextureParameterName.MagFilter, (int)GLTextureMagFilter.Linear);
        gl.TexParameteri(GLTextureTarget.Texture2D, GLTextureParameterName.WrapS,     (int)GLTextureWrapMode.ClampToEdge);
        gl.TexParameteri(GLTextureTarget.Texture2D, GLTextureParameterName.WrapT,     (int)GLTextureWrapMode.ClampToEdge);

        gl.TexImage2D<byte>(
            GLTextureTarget.Texture2D,
            0,
            GLInternalFormat.Rgba8,
            image.Width,
            image.Height,
            0,
            GLPixelFormat.Rgba,
            GLPixelType.UnsignedByte,
            image.Data);

        gl.BindTexture(GLTextureTarget.Texture2D, 0);

        Texture = new ImTextureRef(null, new ImTextureID((ulong)texture_name));
        Size    = new Vector2(image.Width, image.Height);
    }
}
