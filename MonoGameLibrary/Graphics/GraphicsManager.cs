using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Content;

namespace MonoGameLibrary.Graphics;

public static class GraphicsManager
{
    // resources
    private static GraphicsDevice _graphicsDevice;
    private static SpriteBatch _spriteBatch;
    private static List<Pass> _passesList;
    private static Dictionary<string, RasterPassWrapper> _rasterPassWrappersDict;
    private static List<string> _rasterPassWrapperKeysList;
    private static Dictionary<string, RenderTarget2D> _passOutputsDict;

    // runtime
    public static Color BarsColor { get; set; }
    public static Color ClearColor { get; set; }
    public static BlendState DefaultBlendState { get; set; }
    public static SamplerState DefaultSamplerState { get; set; }

    public static void Initialize()
    {
        BarsColor = Color.Black;
        ClearColor = Color.CornflowerBlue;
        DefaultBlendState = BlendState.AlphaBlend;
        DefaultSamplerState = SamplerState.PointClamp;
    }

    public static void Load(GraphicsDevice graphicsDevice, string json)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _passesList = JsonSerializer.Deserialize<List<Pass>>(json);
        _passOutputsDict = new Dictionary<string, RenderTarget2D>();
        ConfigureRasterPassWrappers();
    }

    private static void ConfigureRasterPassWrappers()
    {
        _rasterPassWrappersDict = new Dictionary<string, RasterPassWrapper>();
        _rasterPassWrapperKeysList = new List<string>();

        List<RasterPass> rasterPassesList = _passesList.OfType<RasterPass>().ToList();
        
        foreach (RasterPass rasterPass in rasterPassesList)
        {
            Dictionary<string, Layer> layersDict = new Dictionary<string, Layer>();

            foreach (string layerKey in rasterPass.LayerKeysArray)
            {
                layersDict.Add(layerKey, new Layer());
            }

            RasterPassWrapper rasterPassWrapper = new RasterPassWrapper(layersDict, rasterPass.LayerKeysArray, rasterPass.Width, rasterPass.Height);

            _rasterPassWrappersDict.Add(rasterPass.OutputKey, rasterPassWrapper);
            _rasterPassWrapperKeysList.Add(rasterPass.OutputKey);
        }
    }

    public static void Render()
    {
        foreach (Pass pass in _passesList)
        {
            switch (pass)
            {
                case RasterPass rasterPass:
                    ExecuteRasterPass(rasterPass);
                    continue;
                case MaterialPass materialPass:
                    ExecuteMaterialPass(materialPass);
                    continue;
                case CompositePass compositePass:
                    ExecuteCompositePass(compositePass);
                    continue;
                case BlitPass blitPass:
                    ExecuteBlitPass(blitPass);
                    continue;
                case DuplicatePass duplicatePass:
                    ExecuteDuplicatePass(duplicatePass);
                    continue;
                case PresentPass presentPass:
                    ExecutePresentPass(presentPass);
                    continue;
            }
        }

        _passOutputsDict.Clear();
        Clear();
        RenderTargetManager.ReleaseUsed();
    }

    private static void ExecuteRasterPass(RasterPass rasterPass)
    {
        if (!_rasterPassWrappersDict.ContainsKey(rasterPass.OutputKey))
        {
            throw new Exception();
        }

        RasterPassWrapper rasterPassWrapper = _rasterPassWrappersDict[rasterPass.OutputKey];

        RenderTarget2D renderTarget = RenderTargetManager.Get(rasterPass.Width, rasterPass.Height);
        _graphicsDevice.SetRenderTarget(renderTarget);
        _graphicsDevice.Clear(Color.Transparent);
        
        rasterPassWrapper.Render();

        if (_passOutputsDict.ContainsKey(rasterPass.OutputKey))
        {
            throw new Exception();
        }

        _passOutputsDict.Add(rasterPass.OutputKey, renderTarget);
    }

    private static void ExecuteMaterialPass(MaterialPass materialPass)
    {
        if (!_passOutputsDict.ContainsKey(materialPass.InputKey))
        {
            throw new Exception();
        }

        RenderTarget2D sourceRenderTarget = _passOutputsDict[materialPass.InputKey];
        RenderTarget2D destinationRenderTarget = RenderTargetManager.Get(sourceRenderTarget.Width, sourceRenderTarget.Height);

        _graphicsDevice.SetRenderTarget(destinationRenderTarget);
        _graphicsDevice.Clear(Color.Transparent);

        foreach (string key in materialPass.MaterialKeysArray)
        {
            Material material = NamedMaterials.Get(key);
            
            if (material == null)
            {
                throw new Exception();
            }

            _graphicsDevice.SetRenderTarget(destinationRenderTarget);

            _spriteBatch.Begin(blendState: DefaultBlendState, samplerState: DefaultSamplerState, effect: material.Effect);
            _spriteBatch.Draw(sourceRenderTarget, destinationRenderTarget.Bounds, Color.White);
            _spriteBatch.End();

            (sourceRenderTarget, destinationRenderTarget) = (destinationRenderTarget, sourceRenderTarget);
        }

        if (_passOutputsDict.ContainsKey(materialPass.OutputKey))
        {
            throw new Exception();
        }

        _passOutputsDict.Add(materialPass.OutputKey, sourceRenderTarget);
    }

    private static void ExecuteCompositePass(CompositePass compositePass)
    {
        if (!_passOutputsDict.ContainsKey(compositePass.InputKeyBack) || !_passOutputsDict.ContainsKey(compositePass.InputKeyFront))
        {
            throw new Exception();
        }

        RenderTarget2D backRenderTarget = _passOutputsDict[compositePass.InputKeyBack];
        RenderTarget2D frontRenderTarget = _passOutputsDict[compositePass.InputKeyFront];

        if (backRenderTarget.Width != frontRenderTarget.Width || backRenderTarget.Height != frontRenderTarget.Height)
        {
            throw new Exception();
        }

        RenderTarget2D destinationRenderTarget = RenderTargetManager.Get(backRenderTarget.Width, backRenderTarget.Height);

        _graphicsDevice.SetRenderTarget(destinationRenderTarget);
        _graphicsDevice.Clear(Color.Transparent);

        _spriteBatch.Begin(blendState: DefaultBlendState, samplerState: DefaultSamplerState);
        _spriteBatch.Draw(backRenderTarget, destinationRenderTarget.Bounds, Color.White);
        _spriteBatch.Draw(frontRenderTarget, destinationRenderTarget.Bounds, Color.White);
        _spriteBatch.End();

        if (_passOutputsDict.ContainsKey(compositePass.OutputKey))
        {
            throw new Exception();
        }

        _passOutputsDict.Add(compositePass.OutputKey, destinationRenderTarget);
    }

    private static void ExecuteBlitPass(BlitPass blitPass)
    {
        if (!_passOutputsDict.ContainsKey(blitPass.InputKey))
        {
            throw new Exception();
        }

        Rectangle sourceRect = new Rectangle(blitPass.SourceX, blitPass.SourceY, blitPass.SourceWidth, blitPass.SourceHeight);
        Rectangle destRect = new Rectangle(blitPass.DestX, blitPass.DestY, blitPass.DestWidth, blitPass.DestHeight);

        RenderTarget2D sourceRenderTarget = _passOutputsDict[blitPass.InputKey];
        RenderTarget2D destinationRenderTarget = RenderTargetManager.Get(blitPass.TargetWidth, blitPass.TargetHeight);

        _graphicsDevice.SetRenderTarget(destinationRenderTarget);
        _graphicsDevice.Clear(Color.Transparent);

        _spriteBatch.Begin(blendState: DefaultBlendState, samplerState: DefaultSamplerState);
        _spriteBatch.Draw(sourceRenderTarget, destRect, sourceRect, Color.White);
        _spriteBatch.End();

        if (_passOutputsDict.ContainsKey(blitPass.OutputKey))
        {
            throw new Exception();
        }

        _passOutputsDict.Add(blitPass.OutputKey, destinationRenderTarget);
    }

    private static void ExecuteDuplicatePass(DuplicatePass duplicatePass)
    {
        if (!_passOutputsDict.ContainsKey(duplicatePass.InputKey))
        {
            throw new Exception();
        }

        RenderTarget2D sourceRenderTarget = _passOutputsDict[duplicatePass.InputKey];

        foreach (string key in duplicatePass.OutputKeys)
        {
            RenderTarget2D destinationRenderTarget = RenderTargetManager.Get(sourceRenderTarget.Width, sourceRenderTarget.Height);

            _graphicsDevice.SetRenderTarget(destinationRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(blendState: DefaultBlendState, samplerState: DefaultSamplerState);
            _spriteBatch.Draw(sourceRenderTarget, destinationRenderTarget.Bounds, Color.White);
            _spriteBatch.End();

            if (_passOutputsDict.ContainsKey(key))
            {
                throw new Exception();
            }

            _passOutputsDict.Add(key, destinationRenderTarget);
        }
    }

    private static void ExecutePresentPass(PresentPass presentPass)
    {
        if (!_passOutputsDict.ContainsKey(presentPass.InputKey))
        {
            throw new Exception();
        }

        RenderTarget2D renderTarget = _passOutputsDict[presentPass.InputKey];
        
        _graphicsDevice.SetRenderTarget(null);
        _graphicsDevice.Clear(BarsColor);

        float scale = MathF.Min(
            (float)_graphicsDevice.Viewport.Width / renderTarget.Width, 
            (float)_graphicsDevice.Viewport.Height / renderTarget.Height);

        Rectangle dest = new(
            (int)((_graphicsDevice.Viewport.Width - renderTarget.Width * scale) * 0.5f),
            (int)((_graphicsDevice.Viewport.Height - renderTarget.Height * scale) * 0.5f),
            (int)(renderTarget.Width * scale),
            (int)(renderTarget.Height * scale));
        
        Texture2D pixel = new Texture2D(_graphicsDevice, 1, 1);
        pixel.SetData([ClearColor]); 

        _spriteBatch.Begin(blendState: DefaultBlendState, samplerState: DefaultSamplerState);
        _spriteBatch.Draw(pixel, dest, Color.White);
        _spriteBatch.Draw(renderTarget, dest, Color.White);
        _spriteBatch.End();
    }

    private static void BatchThenFlush(List<RenderSprite> renderSpritesList, Matrix transform)
    {
        List<List<RenderSprite>> batchesList = Batch(renderSpritesList);
        Flush(batchesList, transform);
    }

    private static List<List<RenderSprite>> Batch(List<RenderSprite> renderSpritesList)
    {
        renderSpritesList.Sort((a, b) => b.Depth.CompareTo(a.Depth));

        if (renderSpritesList.Count == 0)
        {
            return new List<List<RenderSprite>>();
        }

        Material currentMaterial = renderSpritesList[0].Material;
        
        List<List<RenderSprite>> batchesList = new List<List<RenderSprite>>();
        List<RenderSprite> batch = new List<RenderSprite>();

        foreach (RenderSprite renderSprite in renderSpritesList)
        {
            Material material = renderSprite.Material;

            if (material != currentMaterial)
            {
                batchesList.Add(batch);
                batch = new List<RenderSprite>();

                currentMaterial = material;
            }

            batch.Add(renderSprite);
        }

        batchesList.Add(batch);

        return batchesList;
    }

    private static void Flush(List<List<RenderSprite>> batchesList, Matrix transform)
    {
        foreach (List<RenderSprite> batch in batchesList)
        {
            Material material = batch[0].Material;

            _spriteBatch.Begin(blendState: DefaultBlendState, samplerState: DefaultSamplerState, effect: material?.Effect, transformMatrix: transform);

            foreach (RenderSprite renderSprite in batch)
            {
                Rectangle destRect = renderSprite.GetDestRect();
                Rectangle sourceRect = renderSprite.Sprite.GetSourceRect();

                SpriteEffects spriteEffects = SpriteEffects.None;
                if (renderSprite.FlipX)
                {
                    spriteEffects |= SpriteEffects.FlipHorizontally;
                }
                if (renderSprite.FlipY)
                {
                    spriteEffects |= SpriteEffects.FlipVertically;
                }

                Vector2 origin = new Vector2(renderSprite.Origin.Item1, renderSprite.Origin.Item2);

                _spriteBatch.Draw(renderSprite.Sprite.Texture, destRect, sourceRect, renderSprite.Color, renderSprite.Rotation.Value, origin, spriteEffects, 0);
            }

            _spriteBatch.End();
        }
    }

    public class RasterPassWrapper
    {
        // resources
        private readonly Dictionary<string, Layer> _layersDict;
        private readonly string[] _layerKeysArray;
        public readonly int Width;
        public readonly int Height;

        // runtime
        public Camera Camera { get; set; }

        public RasterPassWrapper(Dictionary<string, Layer> layersDict, string[] layerKeysArray, int width, int height)
        {
            // resources
            _layersDict = layersDict;
            _layerKeysArray = layerKeysArray;
            Width = width;
            Height = height;

            // runtime
            Camera = new Camera(new Rectangle(0, 0, width, height));
        }

        public void Render()
        {
            foreach (string key in _layerKeysArray)
            {
                Layer layer = _layersDict[key];
                layer.Render(Camera.GetViewMatrix());
            }
        }

        public void Clear()
        {
            foreach (string key in _layerKeysArray)
            {
                Layer layer = _layersDict[key];
                layer.Clear();
            }
        }

        public Layer GetLayer(string key)
        {
            if (!_layersDict.ContainsKey(key))
            {
                return null;
            }

            return _layersDict[key];
        }
    }

    public class Layer
    {
        private List<RenderObject> _renderObjectsList;

        public Layer()
        {
            _renderObjectsList = new List<RenderObject>();
        }

        public void Render(Matrix transform)
        {
            BatchThenFlush(_renderObjectsList.OfType<RenderSprite>().ToList(), transform);
        }

        public void Clear()
        {
            _renderObjectsList.Clear();
        }

        public void Add(RenderObject renderObject)
        {
            _renderObjectsList.Add(renderObject);
        }
    }

    private static void Clear()
    {
        foreach (KeyValuePair<string, RasterPassWrapper> kvp in _rasterPassWrappersDict)
        {
            kvp.Value.Clear();
        }
    }

    public static void Draw(string rasterPassKey, string layerKey, RenderObject renderObject)
    {
        if (!_rasterPassWrappersDict.ContainsKey(rasterPassKey))
        {
            return;
        }

        RasterPassWrapper rasterPassWrapper = _rasterPassWrappersDict[rasterPassKey];
        Layer layer = rasterPassWrapper.GetLayer(layerKey);

        if (layer == null)
        {
            return;
        }

        layer.Add(renderObject);
    }

    public static Camera GetCamera(string key)
    {
        if (!_rasterPassWrappersDict.ContainsKey(key))
        {
            return null;
        }

        RasterPassWrapper rasterPassWrapper = _rasterPassWrappersDict[key];
        return rasterPassWrapper.Camera;
    }
}