using System;

namespace MonoGameLibrary.Util;

public struct Degrees
{
    public enum RotationalDirectionValues
    {
        CW,
        CCW
    }
    public readonly float Value;

    public Degrees(float value)
    {
        Value = Normalize(value);
    }

    public static Degrees Zero => new Degrees(0);

    private static float Normalize(float value)
    {
        value %= 360;

        if (value < 0)
        {
            value += 360;
        }

        return value;
    }

    public Degrees GetDistanceTo(Degrees b, RotationalDirectionValues dir)
    {
        if (dir == RotationalDirectionValues.CW)
        {
            if (this > b)
            {
                return new Degrees(Math.Abs(360 - Value + b.Value));
            }
            else
            {
                return new Degrees(Math.Abs(b.Value - Value));
            }
        }
        else
        {
            if (this > b)
            {
                return new Degrees(Math.Abs(Value - b.Value));
            }
            else
            {
                return new Degrees(Math.Abs(360 - b.Value + Value));
            }
        }
    }

    public override bool Equals(object obj) => obj is Degrees degrees && degrees.Value == Value;
    public override int GetHashCode() => HashCode.Combine(Value);
    public override string ToString() => $"{Value} degrees";
    
    public static Degrees operator +(Degrees a, Degrees b) => new Degrees(a.Value + b.Value);
    public static Degrees operator -(Degrees a, Degrees b) => new Degrees(a.Value - b.Value);
    public static Degrees operator *(Degrees a, float b) => new Degrees(a.Value * b);
    public static Degrees operator *(Degrees a, Degrees b) => new Degrees(a.Value * b.Value);
    public static Degrees operator /(Degrees a, float b) => new Degrees(a.Value / b);
    public static Degrees operator /(Degrees a, Degrees b) => new Degrees(a.Value / b.Value);

    public static bool operator ==(Degrees a, Degrees b) => a.Value == b.Value;
    public static bool operator !=(Degrees a, Degrees b) => a.Value != b.Value;

    public static bool operator >(Degrees a, Degrees b) => a.Value > b.Value;
    public static bool operator >=(Degrees a, Degrees b) => a.Value >= b.Value;
    public static bool operator <(Degrees a, Degrees b) => a.Value < b.Value;
    public static bool operator <=(Degrees a, Degrees b) => a.Value <= b.Value;

    public Radians ToRadians() => new Radians(Value * (float)Math.PI / 180);
}