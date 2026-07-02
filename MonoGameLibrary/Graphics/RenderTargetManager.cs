using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public static class RenderTargetManager
{
    private static GraphicsDevice _graphicsDevice;
    private static Dictionary<(int, int), Stack<RenderTarget2D>> _renderTargetPool;
    private static List<RenderTarget2D> _usedRenderTargetsList;

    public static void Initialize()
    {
        _renderTargetPool = new Dictionary<(int, int), Stack<RenderTarget2D>>();
        _usedRenderTargetsList = new List<RenderTarget2D>();
    }

    public static void Load(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }


    public static RenderTarget2D Get(int width, int height)
    {
        var key = (width, height);

        if (!_renderTargetPool.ContainsKey(key))
        {
            _renderTargetPool[key] = new Stack<RenderTarget2D>();
        }

        RenderTarget2D renderTarget;
        if (_renderTargetPool[key].Count > 0)
        {
            renderTarget = _renderTargetPool[key].Pop();
        }
        else
        {
            renderTarget = new RenderTarget2D(_graphicsDevice, width, height);
        }

        _usedRenderTargetsList.Add(renderTarget);
        return renderTarget;
    }

    private static void Release(RenderTarget2D renderTarget)
    {
        var key = (renderTarget.Width, renderTarget.Height);

        if (!_renderTargetPool.ContainsKey(key))
        {
            _renderTargetPool[key] = new Stack<RenderTarget2D>();
        }

        _renderTargetPool[key].Push(renderTarget);
    }

    public static void ReleaseUsed()
    {
        foreach (RenderTarget2D renderTarget in _usedRenderTargetsList)
        {
            Release(renderTarget);
        }
        _usedRenderTargetsList.Clear();
    }
}