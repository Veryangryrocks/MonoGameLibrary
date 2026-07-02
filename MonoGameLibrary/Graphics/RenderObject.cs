namespace MonoGameLibrary.Graphics;

public abstract class RenderObject
{
    public readonly float Depth;
    protected RenderObject(float depth)
    {
        Depth = depth;
    }
}