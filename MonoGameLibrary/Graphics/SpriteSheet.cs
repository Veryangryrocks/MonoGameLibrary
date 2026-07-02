using System;

namespace MonoGameLibrary.Graphics;

public sealed class SpriteSheet
{
    public readonly Sprite Sprite;
    public readonly int Rows;
    public readonly int Columns;
    public int Width => Sprite.Width;
    public int Height => Sprite.Height;
    public int CellWidth => Width / Columns;
    public int CellHeight => Height / Rows;

    public SpriteSheet(Sprite sprite, int? rows = null, int? columns = null)
    {
        Sprite = sprite;

        Rows = rows ?? 1;
        Columns = columns ?? 1;

        if (Sprite.Width % Columns != 0)
        {
            throw new ArgumentException($"Sprite width {Sprite.Width} is not divisible by column(s) {Columns}");
        }
        if (Sprite.Height % Rows != 0)
        {
            throw new ArgumentException($"Sprite height {Sprite.Height} is not divisible by row(s) {Rows}");
        }
    }

    public Sprite GetSprite(int row, int column)
    {
        if (row < 0 || row >= Rows)
        {
            return null;
        }
        if (column < 0 || column >= Columns)
        {
            return null;
        }

        int x = Sprite.X + column * CellWidth;
        int y = Sprite.Y + row * CellHeight;

        return new Sprite(Sprite.Texture, Sprite.Path, x, y, CellWidth, CellHeight);
    }

    public override string ToString() => $"SpriteSheet [{Sprite}, {Rows}x{Columns}]";
    public override int GetHashCode() => base.GetHashCode();
    public override bool Equals(object obj) => obj is SpriteSheet spriteSheet && spriteSheet.Sprite.Equals(Sprite) && spriteSheet.Rows == Rows && spriteSheet.Columns == Columns;
}