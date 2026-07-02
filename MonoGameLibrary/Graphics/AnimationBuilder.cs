namespace MonoGameLibrary.Graphics;

public static class AnimationBuilder
{
    public static Sprite GetSpriteHorizontally(SpriteSheet spriteSheet, int row, int frame) => spriteSheet.GetSprite(row, frame);
    public static Sprite GetSpriteVertically(SpriteSheet spriteSheet, int column, int frame) => spriteSheet.GetSprite(frame, column);
    public static int GetIndex(int elapsedFrames, int duration, int length) => elapsedFrames / (duration / length) % length;
}