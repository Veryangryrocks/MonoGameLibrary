using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;

namespace MonoGameLibrary.Content;

public static class NamedMaterials
{
    private static Dictionary<string, Material> _materialsDict;
    
    public static void Initialize()
    {
        _materialsDict = new Dictionary<string, Material>();
    }
    public static void Add(string key, Material material)
    {
        if (_materialsDict.ContainsKey(key))
        {
            return;
        }
        _materialsDict.Add(key, material);
    }

    public static Material Get(string key)
    {
        if (!_materialsDict.ContainsKey(key))
        {
            return null;
        }
        return _materialsDict[key];
    }
}