using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public static class WindowManager
{
    public static int DefaultWidth { get; private set; }
    public static int DefaultHeight { get; private set; }

    private static GraphicsDevice _graphicsDevice;
    private static GraphicsDeviceManager _graphicsDeviceManager;

    private static bool _isFullscreened;
    private static int _previousWindowWidth;
    private static int _previousWindowHeight;

    public static void Initialize(int defaultWidth, int defaultHeight, GameWindow gameWindow)
    {
        DefaultWidth = defaultWidth;
        DefaultHeight = defaultHeight;

        gameWindow.AllowUserResizing = true;
        gameWindow.ClientSizeChanged += OnClientSizeChanged;
    }

    public static void Load(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphicsDeviceManager)
    {
        _graphicsDevice = graphicsDevice;
        _graphicsDeviceManager = graphicsDeviceManager;

        graphicsDeviceManager.PreferredBackBufferWidth = DefaultWidth;
        graphicsDeviceManager.PreferredBackBufferHeight = DefaultHeight;

        graphicsDeviceManager.ApplyChanges();
    }

    public static void OnClientSizeChanged(object sender, System.EventArgs e) {}

    public static void Fullscreen()
    {
        _isFullscreened = true;
        _previousWindowWidth = _graphicsDevice.Viewport.Width;
        _previousWindowHeight = _graphicsDevice.Viewport.Height;

        _graphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphicsDeviceManager.IsFullScreen = true;
        _graphicsDeviceManager.ApplyChanges();
    }

    public static void Unfullscreen()
    {
        _isFullscreened = false;

        _graphicsDeviceManager.PreferredBackBufferWidth = _previousWindowWidth;
        _graphicsDeviceManager.PreferredBackBufferHeight = _previousWindowHeight;
        _graphicsDeviceManager.IsFullScreen = false;
        _graphicsDeviceManager.ApplyChanges();
    }

    public static void ToggleFullscreen()
    {
        if (_isFullscreened) Unfullscreen(); else Fullscreen();
    }

    public static Point ScreenToGame(Point screenPosition)
    {
        int gameWidth = DefaultWidth;
        int gameHeight = DefaultHeight;

        int windowWidth = _graphicsDevice.Viewport.Width;
        int windowHeight = _graphicsDevice.Viewport.Height;

        float scale = MathF.Min((float)windowWidth / gameWidth, (float)windowHeight / gameHeight);

        float viewportWidth = gameWidth * scale;
        float viewportHeight = gameHeight * scale;

        float viewportX = (windowWidth - viewportWidth) * 0.5f;
        float viewportY = (windowHeight - viewportHeight) * 0.5f;

        float gameX = (screenPosition.X - viewportX) / scale;
        float gameY = (screenPosition.Y - viewportY) / scale;

        return new Point((int)gameX, (int)gameY);
}
}