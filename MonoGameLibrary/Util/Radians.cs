using System;

namespace MonoGameLibrary.Util;

public struct Radians
{
	public readonly float Value;

    public Radians(float value)
    {
        Value = Normalize(value);
    }

    private static float Normalize(float value)
    {
        float twoPi = 2 * (float)Math.PI;
        value %= twoPi;

        if (value < 0)
        {
            value += twoPi;
        }

        return value;
    }

    public override bool Equals(object obj) => obj is Radians radians && radians.Value == Value;
    public override int GetHashCode() => HashCode.Combine(Value);
    public override string ToString() => $"{Value} radians";

    public Degrees ToDegrees() => new Degrees(Value * 180 / (float)Math.PI);
}
