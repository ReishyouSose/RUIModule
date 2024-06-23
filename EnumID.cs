using System;

namespace TerraLibra
{
    /// <summary>
    /// 请在使用时明确使用的Enum是继承自int
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct EnumID<T> where T : Enum
    {
        public T EnumValue { get; set; }
        public int IntValue
        {
            readonly get => (int)(object)EnumValue;
            set => EnumValue = (T)(object)value;
        }
        public EnumID(T value) => EnumValue = value;
        public EnumID(int value) => IntValue = value;

        #region operator
        public static implicit operator int(EnumID<T> id) => id.IntValue;
        public static implicit operator EnumID<T>(int value) => new(value);
        public static implicit operator EnumID<T>(T value) => new(value);
        public static EnumID<T> operator +(EnumID<T> left, int right) => new(left.IntValue + right);
        public static EnumID<T> operator +(int left, EnumID<T> right) => new(left + right.IntValue);
        public static EnumID<T> operator +(EnumID<T> left, T right) => new(left.IntValue + Convert.ToInt32(right));
        public static EnumID<T> operator +(T left, EnumID<T> right) => new(Convert.ToInt32(left) + right.IntValue);
        public static EnumID<T> operator -(EnumID<T> left, int right) => new(left.IntValue - right);
        public static EnumID<T> operator -(int left, EnumID<T> right) => new(left - right.IntValue);
        public static EnumID<T> operator -(EnumID<T> left, T right) => new(left.IntValue - Convert.ToInt32(right));
        public static EnumID<T> operator -(T left, EnumID<T> right) => new(Convert.ToInt32(left) - right.IntValue);
        public static bool operator ==(EnumID<T> left, int right) => left.IntValue == right;
        public static bool operator ==(int left, EnumID<T> right) => left == right.IntValue;
        public static bool operator ==(EnumID<T> left, T right) => left.EnumValue.Equals(right);
        public static bool operator ==(T left, EnumID<T> right) => right == left;
        public static bool operator !=(EnumID<T> left, int right) => !(left == right);
        public static bool operator !=(int left, EnumID<T> right) => !(left == right);
        public static bool operator !=(EnumID<T> left, T right) => !(left == right);
        public static bool operator !=(T left, EnumID<T> right) => !(right == left);
        #endregion

        public readonly override bool Equals(object obj) => obj is EnumID<T> other && EnumValue.Equals(other.EnumValue);
        public readonly override int GetHashCode() => EnumValue.GetHashCode();
        public override readonly string ToString() => EnumValue.ToString();
    }
}