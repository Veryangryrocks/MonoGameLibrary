using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Graphics;

public static class CursorManager
{
    private static Dictionary<string, Texture2D> _imagesDict;
    private static Dictionary<string, (int, int)> _originsDict;

    public static void Initialize()
    {
        _imagesDict = new Dictionary<string, Texture2D>();
        _originsDict = new Dictionary<string, (int, int)>();

        string fullPath = Directory.GetCurrentDirectory();
        Console.WriteLine(fullPath);
    }

    public static void Add(string name, Texture2D image, (int, int)? origin = null)
    {
        if (_imagesDict.ContainsKey(name) || _originsDict.ContainsKey(name))
        {
            Console.WriteLine($"[CursorManager] cursor with name {name} already exists");
        }

        _imagesDict.Add(name, image);
        _originsDict.Add(name, origin ?? (0, 0));
    }

    private static Texture2D GetImage(string cursorName)
    {
        if (!_imagesDict.ContainsKey(cursorName))
        {
            Console.WriteLine($"[CursorManager] no image found for cursor name {cursorName}");
            return null;
        }

        return _imagesDict[cursorName];
    }

    private static (int, int) GetOrigin(string cursorName)
    {
        if (!_originsDict.ContainsKey(cursorName))
        {
            Console.WriteLine($"[CursorManager] no origin found for cursor name {cursorName}");
            return (0, 0);
        }

        return _originsDict[cursorName];
    }

    public static void Set(string cursorName)
    {
        Texture2D image = GetImage(cursorName);
        (int, int) origin = GetOrigin(cursorName);

        if (image == null)
        {
            return;
        }

        MouseCursor mouseCursor = MouseCursor.FromTexture2D(image, origin.Item1, origin.Item2);
        Mouse.SetCursor(mouseCursor);
    }
}
