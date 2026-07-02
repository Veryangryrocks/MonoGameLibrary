namespace MonoGameLibrary.Graphics;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Content;

public sealed class Sprite
{
    public readonly string Path;
    public readonly Texture2D Texture;
    public readonly int X;
    public readonly int Y;
    public readonly int Width;
    public readonly int Height;

    public Sprite(Texture2D texture, string path, int? x = null, int? y = null, int? width = null, int? height = null)
    {
        if (texture == null)
        {
            throw new ArgumentNullException();
        }
        Texture = texture;
        Path = path;
        X = x ?? texture.Bounds.X;
        Y = y ?? texture.Bounds.Y;
        Width = width ?? texture.Bounds.Width;
        Height = height ?? texture.Bounds.Height;
    }

    public SpriteSheet Splice(int? rows = null, int? columns = null) => new SpriteSheet(this, rows, columns);
    public Rectangle GetSourceRect() => new Rectangle(X, Y, Width, Height);
    public Sprite(string path, int? x = null, int? y = null, int? width = null, int? height = null) : this(AssetManager.Get<Texture2D>(path), path, x, y, width, height) {}
    public Sprite(Texture2D texture, int? x = null, int? y = null, int? width = null, int? height = null) : this(texture, null, x, y, width, height) {}
    public override string ToString() => $"Sprite [{Path}, {X}, {Y}, {Width}, {Height}]";
    public override int GetHashCode() => base.GetHashCode();
    public override bool Equals(object obj) => obj is Sprite sprite && sprite.Path == Path && sprite.X == X && sprite.Y == Y && sprite.Width == Width && sprite.Height == Height;
}