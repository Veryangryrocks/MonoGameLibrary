using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace MonoGameLibrary.Content;

public static class AssetManager
{
    private static ContentManager _contentManager;
    private static Dictionary<string, object> _assetCache;
    private static List<string> _assetPathsList;
    public static List<string> AssetPaths { get => _assetPathsList ; }

    public static void Initialize(ContentManager contentManager, string mgcbPath = "../../../Content/Content.mgcb")
    {
        _assetCache = new Dictionary<string, object>();
        _contentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
        _assetPathsList = GetPaths(mgcbPath);
    }

    public static T Get<T>(string path)
    {
        if (_contentManager == null)
        {
            return default;
        }

        if (!_assetCache.ContainsKey(path))
        {
            Console.WriteLine($"[AssetManager] failed to get {path}");
            return default;
        }

        if (_assetCache[path] is T asset && asset != null)
        {
            return asset;
        }

        var loaded = _contentManager.Load<T>(path);
        _assetCache[path] = loaded;
        return loaded;
    }

    public static void Load()
    {
        if (_contentManager == null)
        {
            throw new ArgumentNullException();
        }

        foreach (var assetPath in _assetPathsList)
        {
            try
            {
                _assetCache[assetPath] = null;
            }
            catch
            {
                Console.WriteLine($"[AssetManager] failed to load {assetPath}");
            }
        }

        Console.WriteLine($"[AssetManager] loaded {_assetCache.Count} asset(s) from MGCB");
    }

    private static List<string> GetPaths(string mgcbPath)
    {
        if (!File.Exists(mgcbPath))
        {
            Console.WriteLine($"[AssetManager] failed to find MGCB file at {mgcbPath}");
            return new List<string>();
        }

        var lines = File.ReadAllLines(mgcbPath)
            .Where(line => line.StartsWith("/build:", StringComparison.OrdinalIgnoreCase))
            .Select(line => line.Substring(7).Trim())
            .Select(path => Path.ChangeExtension(path, null).Replace('\\', '/'))
            .Distinct()
            .ToList();

        Console.WriteLine($"[AssetManager] found {lines.Count} asset(s) in MGCB");
        return lines;
    }
}
