using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

///<summary>Collection of Utilities</summary>
namespace Toolbox.Utilities
{
    /// <summary>General Math Functions</summary>
    public static class Maths
    {
        #region Signs
        /// <summary>Generates a random sign for a number</summary>
        public static float RandomizeSign(float inputNumber) => inputNumber * (UnityEngine.Random.Range(0, 2) * 2 - 1);

        /// <summary>Make a number the same sign as the latter</summary>
        public static float SetEqualSign(float originalNumber, float toEqualize)
        {
            toEqualize = Mathf.Abs(toEqualize);
            if (originalNumber < 0)
                toEqualize *= -1;

            return toEqualize;
        }
        #endregion
    }

    /// <summary>Collection of Operations like Vectors, Directions, Quaternions</summary>
    public static class Vectors
    {
        /// <summary>Get normalized direction from 1 vector3 pointing to another</summary>
        public static Vector3 ToDirection(Vector3 from, Vector3 to) => (to - from).normalized;

        /// <summary>Get the angle of a direction (for Direction use ToDirection(from, to)</summary>
        public static float DirectionAngle(Vector3 direction)
        {
            direction = direction.normalized;
            float n = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (n < 0)
                n += 360;

            return n;

        }

        /// <summary>Rotate a Vector by X degrees</summary>
        public static Vector2 Rotate(this Vector2 v, float degrees) => Quaternion.Euler(0, 0, degrees) * v;

        /// <summary>Rotate a Vector by X degrees</summary>
        public static Vector3 Rotate(this Vector3 v, float degrees) => Quaternion.Euler(0, degrees, 0) * v;

        /// <summary>Get the closest transform to the origin</summary>
        public static Transform ClosestTransform(Vector3 origin, List<Transform> targets)
        {
            float closestDistance = float.MaxValue;
            Transform closestTransform = null;
            foreach (Transform endpoint in targets)
            {
                float distance = Vector3.Distance(origin, endpoint.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTransform = endpoint;
                }
            }
            return closestTransform;
        }

        /// <summary>Checks if Distance is smaller than distance</summary>
        public static bool IsDistanceSmaller(Vector3 from, Vector3 to, float distance) => ((from - to).sqrMagnitude < distance * distance);
    }
}

namespace Toolbox.Time
{
    /// <summary>Collection Of Operations Regarding Time</summary>
    public static class Ops
    {
        // Formats to String like "30d 17h 3m 56s"
        /// <summary>Formats DateTime Span to Readable Short Format</summary>
        public static string SpanToString(DateTime start, DateTime present)
        {
            List<int> timeSections = new();
            string[] units = new string[] { "d", "h", "m", "s" };

            TimeSpan timeSpan = present - start;

            timeSections.Add(timeSpan.Days); // DD
            timeSections.Add(timeSpan.Hours); // HH
            timeSections.Add(timeSpan.Minutes); // MM
            timeSections.Add(timeSpan.Seconds);// SS
            //"DD/HH/MM/SS"

            string shortString = "";

            for (int i = 0; i < 4; i++)
            {
                int timeSection = timeSections[i];
                if (timeSection == 0)
                    continue;

                shortString += " " + timeSection + units[i];

                if (i <= 1)
                {
                    if (timeSections[i] > 1)
                        break; // 2d | 1d 23h | 1h 58m
                }
                else
                {
                    if (timeSections[i] > 4)
                        break;
                }

            }

            return shortString;
        }
    }
}

namespace Toolbox.Loot
{
    [System.Serializable]
    public struct Incremental
    {
        public double Value;
        public int Exp;

        #region Constructors
        public Incremental(double d)
        {
            Value = 0;
            Exp = 0;
            ConvertFromDouble(d);
        }

        public Incremental(byte value) : this((double)value) { }

        public Incremental(bool value)
        {
            this.Exp = 0;
            this.Value = value ? 1 : 0;
        }

        public Incremental(char value) : this((double)value) { }

        public Incremental(decimal value) : this((double)value) { }

        public Incremental(float value) : this((double)value) { }

        public Incremental(short value) : this((double)value) { }

        public Incremental(int value) : this((double)value) { }

        public Incremental(long value) : this((double)value) { }

        public Incremental(sbyte value) : this((double)value) { }

        public Incremental(ushort value) : this((double)value) { }

        public Incremental(uint value) : this((double)value) { }

        public Incremental(ulong value) : this((double)value) { }

        public Incremental(double value, int exp)
        {
            Value = value;
            Exp = exp;
        }
        #endregion

        public double RealValue
        {
            get
            {
                if (this.Exp < 308)
                {
                    double expandedPower = System.Math.Pow(10.0, (ulong)this.Exp);
                    double realValue = this.Value * expandedPower;
                    return realValue;
                }
                else
                {
                    return double.MaxValue;
                }
            }
        }

        void ConvertFromDouble(double d)
        {
            int newExp = 0;
            double newValue = d;
            while (newValue >= 1.0)
            {
                newValue /= 10.0;
                newExp++;
            }

            Value = newValue;
            Exp = newExp;
        }

        void Simplify()
        {

            if (Value <= 0)
            {
                Value = 0;
                Exp = 0;
            }
            else
            {
                if (Value >= 1.0)
                {
                    int newExp = Exp;
                    double newValue = Value;

                    while (newValue >= 1.0)
                    {
                        newValue /= 10.0;
                        newExp++;
                    }

                    Value = newValue;
                    Exp = newExp;
                }
                else if (Value < 0.1 && Value >= 0.0)
                {
                    int newExp = Exp;
                    double newValue = Value;

                    while (newValue < 0.1)
                    {
                        newValue *= 10.0;
                        newExp--;
                    }

                    Value = newValue;
                    Exp = newExp;
                }
            }
        }

        #region Calculation
        public static Incremental operator +(Incremental a, Incremental b)
        {
            if (a.Exp == b.Exp)
            {
                Incremental m;
                m.Exp = a.Exp;
                m.Value = a.Value + b.Value;
                m.Simplify();
                return m;
            }
            else if (a.Exp > b.Exp)
            {
                // a is bigger
                int deltaExp = a.Exp - b.Exp;
                if (deltaExp <= 16)
                {
                    double bX = b.Value / Math.Pow(10, (double)deltaExp);
                    Incremental m;
                    m.Exp = a.Exp;
                    m.Value = a.Value + bX;
                    m.Simplify();
                    return m;
                }
                else
                {
                    return a;
                }
            }
            else
            {
                // b is bigger
                int deltaExp = b.Exp - a.Exp;
                if (deltaExp <= 16)
                {
                    double aX = a.Value / System.Math.Pow(10, (double)deltaExp);
                    Incremental m;
                    m.Exp = b.Exp;
                    m.Value = b.Value + aX;
                    m.Simplify();
                    return m;
                }
                else
                {
                    return b;
                }
            }
        }

        public static Incremental operator +(Incremental a, float b) => a + new Incremental(b);
        public static Incremental operator +(float a, Incremental b) => new Incremental(a) + b;

        public static Incremental operator -(Incremental a, float b) => a - new Incremental(b);

        public static Incremental operator -(float a, Incremental b) => new Incremental(a) - b;

        public static Incremental operator *(Incremental a, float b) => a * new Incremental(b);

        public static Incremental operator *(float a, Incremental b) => new Incremental(a) * b;

        public static Incremental operator /(Incremental a, float b) => a / new Incremental(b);

        public static Incremental operator /(float a, Incremental b) => new Incremental(a) / b;

        public static Incremental operator +(Incremental a, int b) => a + new Incremental(b);

        public static Incremental operator +(int a, Incremental b) => new Incremental(a) + b;

        public static Incremental operator -(Incremental a, int b) => a - new Incremental(b);

        public static Incremental operator -(int a, Incremental b) => new Incremental(a) - b;

        public static Incremental operator *(Incremental a, int b) => a * new Incremental(b);

        public static Incremental operator *(int a, Incremental b) => new Incremental(a) * b;

        public static Incremental operator /(Incremental a, int b) => a / new Incremental(b);

        public static Incremental operator /(int a, Incremental b) => new Incremental(a) / b;
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Negate() { Value = -Value; }

        public static int Compare(Incremental left, object right)
        {
            if (right is Incremental amount)
                return Compare(left, amount);

            if (right is bool boolean)
                return Compare(left, new(boolean));

            if (right is byte @byte)
                return Compare(left, new(@byte));

            if (right is char @char)
                return Compare(left, new Incremental(@char));

            if (right is decimal @decimal)
                return Compare(left, new Incremental(@decimal));

            if (right is double @double)
                return Compare(left, new Incremental(@double));

            if (right is short @int)
                return Compare(left, new Incremental(@int));

            if (right is int int1)
                return Compare(left, new Incremental(int1));

            if (right is long int2)
                return Compare(left, new Incremental(int2));

            if (right is sbyte byte1)
                return Compare(left, new Incremental(byte1));

            if (right is float single)
                return Compare(left, new Incremental(single));

            if (right is ushort int3)
                return Compare(left, new Incremental(int3));

            if (right is uint int4)
                return Compare(left, new Incremental(int4));

            if (right is ulong int5)
                return Compare(left, new Incremental(int5));

            throw new ArgumentException();
        }

        public int Sign
        {
            get
            {
                if (Value == 0)
                    return 0;

                return Value > 0.0 ? 1 : -1;
            }
        }

        public static int Compare(Incremental left, Incremental right)
        {
            left.Format();
            right.Format();
            int leftSign = left.Sign;
            int rightSign = right.Sign;

            // Compare signs
            if (leftSign == 0 && rightSign == 0)
                return 0;

            if (leftSign >= 0 && rightSign < 0)
                return 1;

            if (leftSign < 0 && rightSign >= 0)
                return -1;

            // Compare exponents
            if (left.Exp > right.Exp)
                return 1;

            if (left.Exp < right.Exp)
                return -1;

            return left.Value.CompareTo(right.Value);
        }

        public override int GetHashCode() => this.Value.GetHashCode() ^ this.Exp.GetHashCode();

        public override bool Equals(object obj) => base.Equals(obj);

        public int CompareTo(Incremental value) => Compare(this, value);

        public Incremental ToAbs() => Abs(this);

        public static Incremental Abs(Incremental value)
        {
            if (value.Sign < 0)
                return -value;

            return value;
        }

        public static Incremental Add(Incremental left, Incremental right) => left + right;

        public static Incremental Subtract(Incremental left, Incremental right) => left - right;

        public static Incremental Divide(Incremental dividend, Incremental divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException();

            Incremental bd = new(
                dividend.Value / divisor.Value,
                dividend.Exp - divisor.Exp);
            bd.Simplify();
            return bd;
        }

        public static Incremental Multiply(Incremental left, Incremental right)
        {
            Incremental bd = new(
                left.Value * right.Value,
                left.Exp + right.Exp);
            bd.Simplify();
            return bd;
        }

        #region Operations
        public static implicit operator Incremental(bool value) => new(value);

        public static implicit operator Incremental(byte value) => (value);

        public static implicit operator Incremental(char value) => new(value);

        public static explicit operator Incremental(decimal value) => new Incremental(value);

        public static explicit operator Incremental(double value)
        {
            return new Incremental(value);
        }

        public static implicit operator Incremental(short value)
        {
            return new Incremental(value);
        }

        public static implicit operator Incremental(int value)
        {
            return new Incremental(value);
        }

        public static implicit operator Incremental(long value)
        {
            return new Incremental(value);
        }

        public static implicit operator Incremental(sbyte value)
        {
            return new Incremental(value);
        }

        public static explicit operator Incremental(float value)
        {
            return new Incremental(value);
        }

        public static implicit operator Incremental(ushort value)
        {
            return new Incremental(value);
        }

        public static implicit operator Incremental(uint value)
        {
            return new Incremental(value);
        }

        public static implicit operator Incremental(ulong value)
        {
            return new Incremental(value);
        }

        public static explicit operator bool(Incremental value)
        {
            return value.Sign != 0;
        }

        public static explicit operator byte(Incremental value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value.RealValue < byte.MinValue) || (value.RealValue > byte.MaxValue))
            {
                throw new OverflowException();
            }

            return (byte)value.RealValue;
        }

        public static explicit operator char(Incremental value)
        {
            if (value.Sign == 0)
            {
                return (char)0;
            }

            if ((value.RealValue < char.MinValue) || (value.RealValue > char.MaxValue))
            {
                throw new OverflowException();
            }

            return (char)(ushort)value.RealValue;
        }

        public static explicit operator double(Incremental value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if (value.Exp > 307)
            {
                throw new OverflowException();
            }

            return value.RealValue;
        }

        public static explicit operator float(Incremental value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if (value.Exp > 37)
            {
                throw new OverflowException();
            }

            return (float)value.RealValue;
        }

        public static explicit operator short(Incremental value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value.RealValue < short.MinValue) || (value.RealValue > short.MaxValue))
            {
                throw new OverflowException();
            }

            return (short)value.RealValue;
        }

        public static explicit operator int(Incremental value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value.RealValue < int.MinValue) || (value.RealValue > int.MaxValue))
            {
                throw new OverflowException();
            }

            return ((int)value.RealValue);
        }

        public static explicit operator long(Incremental value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value.RealValue < long.MinValue) || (value.RealValue > long.MaxValue))
            {
                throw new OverflowException();
            }

            return (long)value.RealValue;
        }

        public static explicit operator uint(Incremental value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value.RealValue < uint.MinValue) || (value.RealValue > uint.MaxValue))
            {
                throw new OverflowException();
            }

            return (uint)value.RealValue;
        }

        public static explicit operator ushort(Incremental value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value.RealValue < ushort.MinValue) || (value.RealValue > ushort.MaxValue))
            {
                throw new OverflowException();
            }

            return (ushort)value.RealValue;
        }

        public static explicit operator ulong(Incremental value)
        {
            if ((value.RealValue < ulong.MinValue) || (value.RealValue > ulong.MaxValue))
            {
                throw new OverflowException();
            }

            return (ulong)value.RealValue;
        }

        public static bool operator >(Incremental left, Incremental right)
        {
            return Compare(left, right) > 0;
        }

        public static bool operator <(Incremental left, Incremental right)
        {
            return Compare(left, right) < 0;
        }

        public static bool operator >=(Incremental left, Incremental right)
        {
            return Compare(left, right) >= 0;
        }

        public static bool operator <=(Incremental left, Incremental right)
        {
            return Compare(left, right) <= 0;
        }

        public static bool operator !=(Incremental left, Incremental right)
        {
            return Compare(left, right) != 0;
        }

        public static bool operator ==(Incremental left, Incremental right)
        {
            return Compare(left, right) == 0;
        }

        public static Incremental operator +(Incremental value)
        {
            return value;
        }

        public static Incremental operator -(Incremental value)
        {
            value.Negate();
            return value;
        }

        public static Incremental operator -(Incremental left, Incremental right)
        {
            return left + -right;
        }

        public static Incremental operator /(Incremental dividend, Incremental divisor)
        {
            return Divide(dividend, divisor);
        }

        public static Incremental operator *(Incremental left, Incremental right)
        {
            return Multiply(left, right);
        }

        public static Incremental operator ++(Incremental value)
        {
            value.Value++;
            return value;
        }

        public static Incremental operator --(Incremental value)
        {
            value.Value--;
            return value;
        }
        #endregion

        #region Format
        public void Format()
        {
            Incremental r = new(Value, Exp);
            r = Format(r);
            Value = r.Value;
            Exp = r.Exp;
        }

        public static Incremental Format(Incremental a)
        {
            Incremental r = a;
            int p = r.Exp;
            double rV = r.Value;

            if (rV < 1 && p <= 3)
            {
                rV *= Math.Pow(10, (double)p);
                p = 0;
            }

            while (rV < 100 && p > 3)
            {
                rV *= 10;
                p--;
            }

            if (p < 3)
            {
                rV *= Math.Pow(10, (double)p);
                p = 0;
            }
            else
            {
                if (p % 3 != 0)
                {
                    rV *= Math.Pow(10, (double)(p % 3));
                    p -= (p % 3);
                }

                if (rV >= 1000)
                {
                    rV /= 1000;
                    p += 3;
                }
            }

            r.Value = rV;
            r.Exp = p;

            return r;
        }

        public string ToShortString(string format = "#.##")
        {
            string[] PremitiveAlphapets = { "K", "M", "B", "T" };
            string[] FutureAlphapets = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

            Format();

            if (Exp == 0 && Value < 0.001)
                return "0";
            else
            {
                string result = Value.ToString(format);
                //}
                if (Exp < 3)
                {
                }
                else if (Exp >= 3 && Exp < 15)
                {
                    int index = (int)(Exp / 3);
                    result += PremitiveAlphapets[index - 1];
                }
                else
                {
                    int tExp = (int)(Exp - 15);
                    string tS = "";

                    //0-77
                    if (tExp < 78)
                    {
                        tS += FutureAlphapets[0] + FutureAlphapets[tExp / 3];
                    }
                    //78-155
                    else if (tExp < 78 * 2)
                    {
                        tExp -= 78;
                        tS += FutureAlphapets[1] + FutureAlphapets[tExp / 3];

                    }
                    //156-233
                    else if (tExp < 78 * 3)
                    {
                        tExp -= 78 * 2;
                        tS += FutureAlphapets[2] + FutureAlphapets[tExp / 3];

                    }
                    //234-311
                    else if (tExp < 78 * 4)
                    {
                        tExp -= 78 * 3;
                        tS += FutureAlphapets[3] + FutureAlphapets[tExp / 3];

                    }
                    //312-390
                    else if (tExp < 78 * 5)
                    {
                        tExp -= 78 * 4;
                        tS += FutureAlphapets[4] + FutureAlphapets[tExp / 3];

                    }
                    //391-467
                    else if (tExp < 78 * 6)
                    {
                        tExp -= 78 * 5;
                        tS += FutureAlphapets[5] + FutureAlphapets[tExp / 3];

                    }
                    else
                    {
                        tS = "+e" + (tExp + 15).ToString();
                    }
                    result += tS;
                }
                return result;
            }
        }

        public string ToLongString()
        {
            string[] PremitiveSuffixes = { "Thousand", "Million", "Billion", "Trillion" };
            string[] FutureSuffixes = { "Quadrillion","Quintillion", "Sextillion", "Septillion", "Octillion", "Nonillion",
                "Decillion", "Undecillion", "Duodecillion", "Tredecillion", "Quattuordecillion", "Quindecillion", "Sexdecillion", "Septendecillion", "Octodecillion", "Novemdecillion",
                "Vigintillion", "Unvigintillion", "Duovigintillion", "Trevigintillion","Quattuorvigintillion", "Quinvigintillion", "Sexvigintillion", "Septenvigintillion", "Octovigintillion", "Novemvigintillion",
                "Trigintillion","Untrigintillion","Duotrigintillion","Tretrigintillion","Quattuortrigintillion","Quintrigintillion","Sextrigintillion","Septentrigintillion","Octotrigintillion","Novemtrigintillion",
                "Quadragintillion","Unquadragintillion","Duoquadragintillion","Trequadragintillion","Quattuorquadragintillion","Quinquadragintillion","Sexquadragintillion","Septenquadragintillion","Octaquadragintillion","Novemquadragintillion",
                "Quinragintillion","Unquinquagintillion","Duoquinquagintillion","Trequinquagintillion","Quattuorquinquagintillion","Quinquinquagintillion","Sexquinquagintillion","Septenquinquagintillion","Octoquinquagintillion","Novemquinquagintillion",
                "Sexagintillion","Unsexagintillion","Duosexagintillion","Tresexagintillion","Quattuorsexagintillion","Quinsexagintillion","Sexsexagintillion","Septsexagintillion","Octosexagintillion","Novemsexagintillion",
                "Septuagintillion","Unseptuagintillion","Duoseptuagintillion","Treseptuagintillion","Quattuorseptuagintillion","Quinseptuagintillion","Sexseptuagintillion","Septseptuagintillion","Octoseptuagintillion","Novemseptuagintillion"};

            Format();

            if (Exp == 0 && Value < 0.001)
            {
                return "0";
            }
            else
            {

                string result = Value.ToString("#.##");
                //}
                if (Exp < 3) { }

                else if (Exp >= 3 && Exp < 15)
                {
                    int index = (int)(Exp / 3);
                    result += " " + PremitiveSuffixes[index - 1];
                }
                else
                {
                    int tExp = (int)(Exp - 15);
                    string tS = "";

                    //0-77
                    if (tExp < 226)
                    {
                        tS += " " + FutureSuffixes[tExp / 3];
                    }
                    else
                    {
                        tS = "+e" + (tExp + 15).ToString();
                    }
                    result += tS;
                }
                return result;
            }
        }

        public string ToExponentForm()
        {
            //string[] PremitiveSuffixes = { "Thousand", "Million", "Billion", "Trillion" };

            Format();

            //if (Exp < 3)
            //{

            if (Exp == 0 && Value < 0.001)
            {
                return "0";
            }
            else
            {

                string result = Value.ToString("#.##");
                string tS = "";
                //}
                if (Exp >= 3)
                {
                    tS = "+e" + (Exp).ToString();
                }
                result += tS;
                return result;
            }
        }

        public string GetStringForSave()
        {
            string s;
            s = Value.ToString() + "," + Exp.ToString();
            return s;
        }

        public void SetValueFromString(string s)
        {
            string[] split = s.Split(',');
            Value = double.Parse(split[0]);
            if (split.Length > 1)
                Exp = int.Parse(split[1]);
            else
                Exp = 0;
        }
        #endregion
    }
}
