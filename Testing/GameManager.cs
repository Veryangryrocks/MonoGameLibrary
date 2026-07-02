using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Content;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Util;

namespace Testing;

public static class GameManager
{
    public static int ElapsedFrames { get; private set; }
    private static SpriteSheet _spriteSheet;
    public static void Initialize()
    {
        InputManager.Add("exit", new InputManager.Tap(new InputManager.OrInputBind([new InputManager.Key(Keys.Escape)])));
        InputManager.Add("fullscreen", new InputManager.Tap(new InputManager.OrInputBind([new InputManager.Key(Keys.F11)])));

        CursorManager.Add("pointer", AssetManager.Get<Texture2D>("texture/cursor/pointer"));
        CursorManager.Set("pointer");

        _spriteSheet = new SpriteSheet(new Sprite("texture/atlas_test"), 4, 4);
    }

    public static void NameEffects()
    {
        NamedMaterials.Add("grayscale", new Material(AssetManager.Get<Effect>("effect/grayscale")));
    }

    public static void Update()
    {
        ElapsedFrames++;
        Console.WriteLine(ElapsedFrames);
    }

    public static void Draw()
    {
        Sprite sprite = AnimationBuilder.GetSpriteHorizontally(_spriteSheet, 0, AnimationBuilder.GetIndex(ElapsedFrames, 40, 4));
        GraphicsManager.Draw("game", "main", RenderSprite.FromPositions(sprite, 0, 0));
        GraphicsManager.Draw("game", "main", RenderSprite.FromPositions(new Sprite("texture/test"), 100, 100, originValue: RenderSprite.PositionValue.CENTER));
    }
}