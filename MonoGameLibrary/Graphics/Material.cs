

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public sealed class Material
{
    public Effect Effect { get; private set; }
    private Dictionary<string, EffectParameter> _effectParametersDict;
    public Material(Effect effect)
    {
        Effect = effect;
        _effectParametersDict = effect.Parameters.ToDictionary(p => p.Name);
    }

    private bool TryGetParameter(string name, out EffectParameter effectParameter)
    {
        return _effectParametersDict.TryGetValue(name, out effectParameter);
    }
    
    public void SetParameter(string name, float value)
    {
        if (TryGetParameter(name, out var parameter))
        {
            parameter.SetValue(value);
        }
    }
    
    public void SetParameter(string name, Vector2 value)
    {
        if (TryGetParameter(name, out var parameter))
        {
            parameter.SetValue(value);
        }
    }

    public void SetParameter(string name, Texture2D value)
    {
        if (TryGetParameter(name, out var parameter))
        {
            parameter.SetValue(value);
        }
    }
}