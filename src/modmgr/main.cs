// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - Bootstrap SDL3 + OpenGL3 + Dear ImGui and own the main window/event loop.
 * - Load the UI font, upload the menu bar's icon texture (see icon.cs), and
 *   apply initial DPI/style scaling.
 * - Drive the per-frame render loop, handing off actual UI content to
 *   FhModManagerUI.UI() (see ui.cs) each frame.
 */

namespace Fahrenheit.Tools.ModManager;

/* [fkelava 15/8/25 17:41]
 * This is the generic tool program stub. Do not edit this unless you encounter a bug.
 * To edit the tool's UI, please open `ui_*.cs`.
 */

/* [fkelava 19/07/26 23:30]
 * Previously, we used the same Win32+D3D11 support as we used inside the game process.
 * Now, in the interests of our tools working across platforms, we do OGL3+SDL3 for tools.
 *
 * For reasons which are not known, the 'backend' package and the OGL3/SDL3 wrappers from Hexa.NET
 * _do not_ use the same types. This necessitates a lot of ugly casting and/or wrapper types.
 * Legibility is thus rather poor. You have been warned.
 */

internal sealed unsafe class Program {

    // SDL3
    private static SDLWindow*   _sdl_window;
    private static uint         _sdl_window_id;
    private static SDLGLContext _sdl_context;

    // OGL3
    private static GL?          _gl_context;

    /* [fkelava 19/07/26 23:56]
     * https://github.com/ocornut/imgui/blob/81c008f90d488d18370dbe6741115e126d67f539/examples/example_sdl3_opengl3/main.cpp#L28
     * This is a straight rewrite. Comments appear as in the original, where they are in the original; with some extras.
     */

    private static void Main(string[] args) {

        // Setup SDL.
        SDLInitFlags init_flags = (
            SDLInitFlags.Video
          | SDLInitFlags.Events);

        if (!SDL.Init(init_flags)) {
            Console.WriteLine($"Fault in SDL_Init() - {SDL.GetErrorS()}");
            return;
        }

        /* [fkelava 20/07/26 00:12]
         * The example goes on to select a GL version. Since we don't have access to
         * the IMGUI_IMPL_OPENGL_* defines, we can't do that. Apparently we just let
         * the backend do its thing by passing nothing to ImGui_ImplOpenGL3_Init.
         */

        // Create window with graphics content
        SDL.GLSetAttribute(SDLGLAttr.Doublebuffer, 1);
        SDL.GLSetAttribute(SDLGLAttr.DepthSize,    24);
        SDL.GLSetAttribute(SDLGLAttr.StencilSize,  8);

        uint  main_display_id    = SDL.GetPrimaryDisplay();
        float main_display_scale = SDL.GetDisplayContentScale(main_display_id);

        /* 
         * A flat 1280x800 (even after DPI-scaling it via main_display_scale) looks
         * tiny on a 4K/8K display running at 100% OS scaling, since content_scale
         * only compensates for pixel density, not for how much actual screen real
         * estate is available. Size the window as a fraction of the display's own
         * usable area (screen minus taskbar/dock) instead, clamped so it doesn't
         * shrink below a usable size on a small display or balloon absurdly on an
         * ultrawide/8K one.
         */
        SDLRect usable_bounds = default;
        SDL.GetDisplayUsableBounds(main_display_id, ref usable_bounds);

        // The clamp bounds are scaled by main_display_scale too, same as the
        // 0.65/0.70 fractions are implicitly scaled by using usable_bounds
        // directly: usable_bounds and main_display_scale already have to be in
        // the same unit space for the fraction above to mean anything, so a
        // flat, unscaled clamp would silently stop meaning "1024-2200 logical
        // pixels" and start meaning "1024-2200 of whatever unit usable_bounds
        // happens to be in" - a much smaller effective window on a high-DPI
        // display than on a 100%-scale one.
        int window_width  = int.CreateChecked(Math.Clamp(usable_bounds.W * 0.65F, 1024F * main_display_scale, 2200F * main_display_scale));
        int window_height = int.CreateChecked(Math.Clamp(usable_bounds.H * 0.70F, 700F  * main_display_scale, 1400F * main_display_scale));

        SDLWindowFlags window_flags =
            SDLWindowFlags.Opengl
          | SDLWindowFlags.Resizable
          | SDLWindowFlags.Hidden
          | SDLWindowFlags.HighPixelDensity
          | SDLWindowFlags.Borderless;

        _sdl_window = SDL.CreateWindow(
            "Fahrenheit Mod Manager",
            window_width,
            window_height,
            window_flags);

        if (_sdl_window == null) {
            Console.WriteLine($"Fault in SDL_CreateWindow() - {SDL.GetErrorS()}");
            return;
        }

        /* 
         * Borderless means no OS title bar/min/max/close - FhModManagerUI draws its own
         * into the menu bar instead (see chrome.cs), so this restores the drag-to-
         * move and drag-edge-to-resize behavior the OS chrome would otherwise have
         * provided.
         */
        FhWindowChrome.install(_sdl_window);

        _sdl_window_id = SDL.GetWindowID    (_sdl_window);
        _sdl_context   = SDL.GLCreateContext(_sdl_window);

        if (_sdl_context == SDLGLContext.Null) {
            Console.WriteLine($"Fault in SDL_GL_CreateContext() - {SDL.GetErrorS()}");
            return;
        }

        /* [fkelava 20/07/26 00:31]
         * For some reason we do not get SDL_WINDOWPOS_CENTERED defined. We improvise.
         */

        SDL.GLMakeCurrent    (_sdl_window, _sdl_context);
        SDL.GLSetSwapInterval(1); // Enable VSync
        SDL.SetWindowPosition(_sdl_window,
            (int)SDL.SDL_WINDOWPOS_CENTERED_MASK,
            (int)SDL.SDL_WINDOWPOS_CENTERED_MASK);
        SDL.ShowWindow       (_sdl_window);

        // Setup Dear ImGui context
        ImGuiContextPtr ctx   = ImGui.CreateContext();
        ImGuiIOPtr      io    = ImGui.GetIO();
        ImGuiStylePtr   style = ImGui.GetStyle();

        // The main window's pos/size are pinned to the viewport every frame and
        // both modals recenter themselves on open (see ui.cs), so there's nothing
        // for a saved imgui.ini layout to restore. Unlike src/runtime/imgui.cs,
        // which points IniFilename at a real path for its debug overlay, this
        // tool has no use for one.
        io.IniFilename = null;

        // Load the Fahrenheit UI font. This is a variable font, so we can set the weight and width in the future if we want to.
        string font_path = Path.Join(
            AppContext.BaseDirectory,
            "resources",
            "fonts",
            "NotoSans-VariableFont_wdth,wght.ttf"
        );

        if (File.Exists(font_path)) {
            io.Fonts.AddFontFromFileTTF(
                font_path,
                20F,
                null,
                io.Fonts.GetGlyphRangesDefault()
            );
        }
        else {
            Console.WriteLine(
                $"Could not find the Fahrenheit UI font: {font_path}"
            );
        }

        io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard; // Enable Keyboard Controls
        io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;  // Enable Gamepad Controls
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;     // Enable Docking
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;   // Enable Multi-Viewport / Platform Windows

        // Setup Dear ImGui style
        //
        // FhTheme.UiScale has to be set before the first apply() call - unlike a
        // one-time style.ScaleAllSizes() call (the stock example this loop is
        // otherwise a port of uses one), apply() itself multiplies every layout
        // number it writes by UiScale, so it stays correct no matter how many
        // more times apply() runs later (every saved-settings load, every
        // Settings > Theme color change - see FhTheme's own comment on UiScale).
        FhTheme.UiScale = main_display_scale;

        ImGui.StyleColorsDark();
        FhTheme.apply();

        // Setup scaling
        style.FontScaleDpi = main_display_scale; // Set initial font scale.
        io.ConfigDpiScaleFonts     = true;       // [Experimental] Automatically overwrite style.FontScaleDpi in Begin() when Monitor DPI changes. This will scale fonts but _NOT_ scale sizes/padding for now.
        io.ConfigDpiScaleViewports = true;       // [Experimental] Scale Dear ImGui and Platform Windows when Monitor DPI changes.

        // The stock example this loop otherwise ports zeroes WindowRounding here
        // (so platform windows spawned by ViewportsEnable, which can't easily
        // round their own OS-level corners, look consistent with the main one) -
        // deliberately not done here, since FhTheme.apply() already gave this
        // window an intentional WindowRounding, and FhWindowChrome.install()
        // (see chrome.cs) opts the main window out of the OS's own competing
        // corner rounding instead of flattening ours to match it. Forcing full
        // opacity is still worth keeping regardless, since the Settings modal's
        // theme picker lets a user set an alpha on the background color.
        style.Colors[(int)ImGuiCol.WindowBg].W = 1.0F;

        // Setup Platform/Renderer backends
        ImGuiImplSDL3.SetCurrentContext(ctx);
        ImGuiImplSDL3.InitForOpenGL    (new SDLWindowPtrB((SDLWindowB*)_sdl_window), (void*)_sdl_context.Handle);

        ImGuiImplOpenGL3.SetCurrentContext(ctx);
        ImGuiImplOpenGL3.Init             ((byte*)null);

        /* [fkelava 20/07/26 00:17]
         * See comment beneath main loop body.
         */

        _gl_context = new (new FhGlContext(_sdl_window, _sdl_context));

        // Uploads the menu bar's icon (see icon.cs) as a GL texture - needs a
        // live GL context, so this can't happen any earlier than here.
        FhAppIcon.load(_gl_context);

        // Main loop
        bool     quit = false;
        SDLEvent msg  = default;

        while (!quit) {
            // Poll and handle events (inputs, window resize, etc.)
            while (SDL.PollEvent(ref msg)) {
                ImGuiImplSDL3.ProcessEvent((SDLEventB*)&msg);

                quit = (SDLEventType)msg.Type switch {
                    SDLEventType.Quit                                                            => true,
                    SDLEventType.WindowCloseRequested when msg.Window.WindowID == _sdl_window_id => true,
                    _                                                                            => false
                };
            }

            // There's no OS close button to raise WindowCloseRequested anymore -
            // the menu bar's own close button (see chrome.cs) sets this instead.
            if (FhWindowChrome.QuitRequested) {
                quit = true;
            }

            SDLWindowFlags current_window_flags = SDL.GetWindowFlags(_sdl_window);

            if (current_window_flags.HasFlag(SDLWindowFlags.Minimized)) {
                SDL.Delay(10);
                continue;
            }

            // Start the Dear ImGui frame
            ImGuiImplOpenGL3.NewFrame();
            ImGuiImplSDL3   .NewFrame();
            ImGui           .NewFrame();

            // Draw the mod manager UI
            FhModManagerUI.UI();

            // Rendering
            ImGui.Render();

            // The main window is drawn with rounded corners (see FhTheme.apply's
            // WindowRounding), but the SDL window behind it is a plain rectangle -
            // ImGui's rounded fill simply doesn't paint over the four corner
            // triangles outside that curve, so whatever this clears to is what
            // shows through them. The stock ImGui example this loop is otherwise a
            // straight port of (see the comment above Main()) clears to a fixed
            // teal-gray there, which read as a stray colored pixel in our own
            // (much darker, user-recolorable) theme; clearing to the theme's own
            // background keeps those corners visually blank instead.
            Vector4 clear_color = FhTheme.COLOR_BACKGROUND;

            _gl_context.Viewport  (0, 0, int.CreateChecked( io.DisplaySize.X ), int.CreateChecked( io.DisplaySize.Y ));
            _gl_context.ClearColor(clear_color.X, clear_color.Y, clear_color.Z, 1F);
            _gl_context.Clear     (GLClearBufferMask.ColorBufferBit);

            ImGuiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());

            // Update and Render additional Platform Windows
            // (Platform functions may change the current OpenGL context, so we save/restore it to make it easier to paste this code elsewhere.)
            if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable)) {
                SDLWindow*   current_window  = SDL.GLGetCurrentWindow();
                SDLGLContext current_context = SDL.GLGetCurrentContext();

                ImGui.UpdatePlatformWindows();
                ImGui.RenderPlatformWindowsDefault();

                SDL.GLMakeCurrent(current_window, current_context);
            }

            SDL.GLSwapWindow(_sdl_window);
        }

        // Cleanup
        ImGuiImplOpenGL3.Shutdown();
        ImGuiImplSDL3   .Shutdown();

        ImGui.DestroyContext();

        SDL.GLDestroyContext(_sdl_context);
        SDL.DestroyWindow   (_sdl_window);
        SDL.Quit();
    }
}

/* [fkelava 20/07/26 00:22]
 * For whatever reason we don't get to directly use GL functions. We have to go
 * through this intermediary Hexa type. The relevant parts were straight copy-pasted from
 * https://github.com/HexaEngine/Hexa.NET.ImGui/blob/0ec233064d0dcd565b9ce9b40d9762d94ef30a1b/Examples/ExampleSDL3OpenGL3/Program.cs
 */

internal sealed unsafe class FhGlContext(SDLWindow* window, SDLGLContext context) : IGLContext {
    public nint Handle    => (nint)window;
    public bool IsCurrent => SDL.GLGetCurrentContext() == context;

    public void Dispose() { }

    public nint GetProcAddress      (string procName)      => (nint)SDL.GLGetProcAddress    (procName);
    public bool IsExtensionSupported(string extensionName) =>       SDL.GLExtensionSupported(extensionName);

    public void MakeCurrent ()             => SDL.GLMakeCurrent(window, context);
    public void SwapBuffers ()             => SDL.GLSwapWindow (window);
    public void SwapInterval(int interval) => SDL.GLSetSwapInterval(interval);

    public bool TryGetProcAddress(string procName, out nint procAddress) {
        return (procAddress = (nint)SDL.GLGetProcAddress(procName)) != 0;
    }
}
