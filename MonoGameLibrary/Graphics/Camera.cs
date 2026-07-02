using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Util;

namespace MonoGameLibrary.Graphics;

public sealed class Camera
{
    public Vector2 Position;
    public Degrees Rotation;
    private float _zoom;
    public float Zoom 
    { 
        get
        {
            return _zoom;
        } 
        set
        {
            if (value < 0)
            {
                _zoom = 0;
            }
            else
            {
                _zoom = value;
            }
        }
    }
    public Vector2 Origin { get; set; }
    public Camera(Rectangle rect)
    {
        Position = new Vector2(rect.X, rect.Y);
        Rotation = new Degrees(0);
        Zoom = 1.0f;
        Origin = new Vector2(rect.Width / 2, rect.Height / 2);
    }

    public Matrix GetViewMatrix()
    {
        return
            Matrix.CreateTranslation(-Position.X, -Position.Y, 0)
            * Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0)
            * Matrix.CreateScale(Zoom)
            * Matrix.CreateRotationZ(-Rotation.ToRadians().Value)
            * Matrix.CreateTranslation(Origin.X, Origin.Y, 0);
    }
}