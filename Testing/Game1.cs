using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Content;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Util;

namespace Testing;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphicsDeviceManager;
    public Game1()
    {
        Content.RootDirectory = "Content";
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        //Window.ClientSizeChanged += WindowManager.OnClientSizeChanged;
    }

    protected override void Initialize()
    {
        CursorManager.Initialize();
        InputManager.Initialize();
        WindowManager.Initialize(640, 360, Window);
        AssetManager.Initialize(Content);
        NamedMaterials.Initialize();
        RenderTargetManager.Initialize();
        GraphicsManager.Initialize();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        WindowManager.Load(GraphicsDevice, _graphicsDeviceManager);
        AssetManager.Load();
        RenderTargetManager.Load(GraphicsDevice);
        GameManager.NameEffects();
        
        string json = File.ReadAllText(Path.Combine(PathManager.ProjectDataDir, "render_graph.json"));
        GraphicsManager.Load(GraphicsDevice, json);

        base.LoadContent();
        GameManager.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.Update();

        if (InputManager.Get("exit"))
            Exit();
        if (InputManager.Get("fullscreen"))
            WindowManager.ToggleFullscreen();
        
        GameManager.Update();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GameManager.Draw();
        GraphicsManager.Render();
        base.Draw(gameTime);
    }
}
