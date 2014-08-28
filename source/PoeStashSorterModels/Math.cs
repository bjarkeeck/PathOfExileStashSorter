using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Provides constants and static methods for trigonometric, logarithmic and other common mathematical functions.
/// </summary>
public class Mathf
{
    /// <summary>
    /// Degrees-to-radians conversion constant.
    /// </summary>
    public const float Deg2Rad = 0.0174533f;
    /// <summary>
    /// A tiny floating point value.
    /// </summary>
    public const float Epsilon = 1.4013e-045f;
    /// <summary>
    /// Exponential e.
    /// </summary>
    public const float ExponentialE = 2.71828f;
    /// <summary>
    /// The golden ratio. Oooooh!
    /// </summary>
    public const float GoldenRatio = 1.61803f;
    /// <summary>
    /// A representation of positive infinity.
    /// </summary>
    public const float Infinity = 1.0f / 0.0f;
    /// <summary>
    /// A representation of negative infinity.
    /// </summary>
    public const float NegativeInfinity = -1.0f / 0.0f;
    /// <summary>
    /// The infamous 3.14159265358979... value.
    /// </summary>
    public const float PI = 3.14159f;
    /// <summary>
    /// Radians-to-degrees conversion constant.
    /// </summary>
    public const float Rad2Deg = 57.2958f;
    /// <summary>
    /// The not-so-infamous TAU value.
    /// </summary>
    public const float TAU = PI * 2;

    /// <summary>
    /// Returns the absolute value of a.
    /// </summary>
    /// <param name="a">The value.</param>
    public static int Abs(int a) { return Math.Abs(a); }
    /// <summary>
    /// Returns the absolute value of a.
    /// </summary>
    /// <param name="a">The value.</param>
    public static float Abs(float a) { return (float)Math.Abs(a); }
    /// <summary>
    /// Returns the arc-cosine of a - the angle in radians whose cosine is a.
    /// </summary>
    /// <param name="a">The value.</param>
    public static float Acos(float a) { return (float)Math.Acos(a); }
    /// <summary>
    /// Returns the arc-sine of a - the angle in radians whose sine is a.
    /// </summary>
    /// <param name="a">The value.</param>
    public static float Asin(float a) { return (float)Math.Asin(a); }
    /// <summary>
    /// Returns the arc-tangent of a - the angle in radians whose tangent is a.
    /// </summary>
    /// <param name="a">The value.</param>
    public static float Atan(float a) { return (float)Math.Atan(a); }
    /// <summary>
    /// Returns the angle in radians whose Tan is y/x.
    /// </summary>
    /// <param name="y">The y value.</param>
    /// <param name="x">The x value.</param>
    public static float Atan2(float y, float x) { return (float)Math.Atan2(y, x); }
    /// <summary>
    /// Returns the smallest integer greater to or equal to a.
    /// </summary>
    /// <param name="a">The value.</param>
    public static float Ceil(float a) { return (float)Math.Ceiling(a); }
    /// <summary>
    /// Returns the smallest integer greater to or equal to a.
    /// </summary>
    /// <param name="a">The value.</param>
    public static int CeilToInt(float a) { return (int)Ceil(a); }
    /// <summary>
    /// Clamps a value between a minimum int and maximum int value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public static int Clamp(int value, int min, int max)
    {
        return (int)ClampFloat((float)value, (float)min, (float)max);
    }
    /// <summary>
    /// Clamps a value between a minimum float and maximum float value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public static float Clamp(float value, float min, float max)
    {
        if (max <= min)
            return min;
        return value < min ? min : value > max ? max : value;
    }
    /// <summary>
    /// Clamps a value between a minimum float and maximum float value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public static float ClampFloat(float value, float min, float max)
    {
        if (max <= min)
            return min;
        return value < min ? min : value > max ? max : value;
    }
    /// <summary>
    /// Clamps value between 0 and 1 and returns value.
    /// </summary>
    /// <param name="value">The value.</param>
    public static float Clamp01(float value) { return value < 0 ? 0 : value > 1 ? 1 : value; }
    /// <summary>
    /// Returns the cosine of angle f in radians.
    /// </summary>
    /// <param name="a">The value.</param>
    public static float Cos(float a) { return (float)Math.Cos(a); }
    /// <summary>
    /// Returns e raised to the specified power.
    /// </summary>
    /// <param name="power">The power.</param>
    public static float Exp(float power) { return (float)Math.Exp(power); }
    /// <summary>
    /// Returns the largest integer smaller to or equal to a.
    /// </summary>
    /// <param name="a">The value.</param>
    public static float Floor(float a) { return (float)Math.Floor(a); }
    /// <summary>
    /// Returns the largest integer smaller to or equal to a.
    /// </summary>
    /// <param name="a">The value.</param>
    public static int FloorToInt(float a) { return (int)Floor(a); }
    /// <summary>
    /// Interpolates between from and to by t. t is clamped between 0 and 1.
    /// </summary>
    /// <param name="from">The from value.</param>
    /// <param name="to">The to value.</param>
    /// <param name="t">The t value.</param>
    public static float Lerp(float from, float to, float t) { return t >= 1 ? to : t < 0 ? from : from + (to - from) * t; }
    /// <summary>
    /// Returns the natural (base e) logarithm of a specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    public static float Log(float value) { return (float)Math.Log(value); }
    /// <summary>
    /// Returns the base 10 logarithm of a specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    public static float Log10(float value) { return (float)Math.Log10(value); }
    /// <summary>
    /// Returns the largest of two integer values.
    /// </summary>
    /// <param name="a">One value.</param>
    /// <param name="b">The other value.</param>
    public static int Max(int a, int b) { return Math.Max(a, b); }
    /// <summary>
    /// Returns the largest of two float values.
    /// </summary>
    /// <param name="a">One value.</param>
    /// <param name="b">The other value.</param>
    public static float Max(float a, float b) { return Math.Max(a, b); }
    /// <summary>
    /// Returns the smaller of two integer values.
    /// </summary>
    /// <param name="a">One value.</param>
    /// <param name="b">The other value.</param>
    public static int Min(int a, int b) { return Math.Min(a, b); }
    /// <summary>
    /// Returns the smaller of two float values.
    /// </summary>
    /// <param name="a">One value.</param>
    /// <param name="b">The other value.</param>
    public static float Min(float a, float b) { return Math.Min(a, b); }
    /// <summary>
    /// Returns f raised to power p.
    /// </summary>
    /// <param name="f">The value to raise.</param>
    /// <param name="p">The power.</param>
    public static float Pow(float f, float p) { return (float)Math.Pow(f, p); }
    /// <summary>
    /// Returns f rounded to the nearest integer.
    /// </summary>
    /// <param name="f">The value.</param>
    public static float Round(float f) { return (float)Math.Round(f); }
    /// <summary>
    /// Rounds a floating-point value to a specified number of fractional digits. 
    /// </summary>
    /// <param name="f">The value.</param>
    /// <param name="decimals">The number of fractional digits to round to.</param>
    public static float Round(float f, int decimals) { return (float)Math.Round(f, decimals); }
    /// <summary>
    /// Rounds a floating-point value to a specified number of fractional digits. A parameter specifies how to round a value if it is midway between two other numbers.
    /// </summary>
    /// <param name="f">The value.</param>
    /// <param name="decimals">The number of fractional digits to round to.</param>
    /// <param name="mode">The rounding mode to use.</param>
    public static float Round(float f, int decimals, MidpointRounding mode) { return (float)Math.Round(f, decimals, mode); }
    /// <summary>
    /// Returns f rounded to the nearest integer.
    /// </summary>
    /// <param name="f">The value to round.</param>
    public static int RoundToInt(float f) { return (int)Round(f); }
    /// <summary>
    ///  Rounds a floating-point value to a specified number of fractional digits. Except not. This method makes no sense.
    /// </summary>
    /// <param name="f">The value.</param>
    /// <param name="decimals">The number of fractional digits to round to.</param>
    public static int RoundToInt(float f, int decimals) { return (int)Round(f, decimals); }
    /// <summary>
    /// Rounds a floating-point value to a specified number of fractional digits. A parameter specifies how to round a value if it is midway between two other numbers. Except not. This method makes no sense.
    /// </summary>
    /// <param name="f">The value.</param>
    /// <param name="decimals">The number of fractional digits to round to.</param>
    /// <param name="mode">The rounding mode to use.</param>
    public static int RoundToInt(float f, int decimals, MidpointRounding mode) { return (int)Round(f, decimals, mode); }
    /// <summary>
    /// Returns the sign of f.
    /// </summary>
    /// <param name="f">The value.</param>
    public static float Sign(float f) { return (float)Math.Sign(f); }
    /// <summary>
    /// Returns the sine of angle f in radians.
    /// </summary>
    /// <param name="f">The value.</param>
    public static float Sin(float f) { return (float)Math.Sin(f); }
    /// <summary>
    /// Returns square root of f.
    /// </summary>
    /// <param name="f">The value.</param>
    public static float Sqrt(float f) { return (float)Math.Sqrt(f); }
    /// <summary>
    /// Returns the tangent of angle f in radians.
    /// </summary>
    /// <param name="f">The value.</param>
    public static float Tan(float f) { return (float)Math.Tan(f); }
    /// <summary>
    /// Compares two floating point values if they are similar.
    /// </summary>
    /// <param name="a">One value.</param>
    /// <param name="b">The other value.</param>
    /// <param name="threshold">The threshold of similarity.</param>
    /// <returns>True if the values are similar, otherwise false.</returns>
    public static bool RoughlyEqual(float a, float b, float threshold = 0.01f) { return (Mathf.Abs(a - b) <= threshold); }
}
