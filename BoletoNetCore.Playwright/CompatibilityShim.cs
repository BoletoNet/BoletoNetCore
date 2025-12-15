#if NETSTANDARD2_0

using System.Runtime.CompilerServices;

// Nullable reference type attributes are based on the "Nullable" library by Manuel Römer
// https://github.com/manuelroemer/Nullable
// Licensed under MIT License

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// This class should not be used by developers in source code.
    /// Enables init-only property setters in C# 9.0+.
    /// </summary>
    internal static class IsExternalInit
    {
    }

    /// <summary>
    /// Specifies that a type has required members or that a member is required.
    /// </summary>
    internal class RequiredMemberAttribute : Attribute
    {
    }

    /// <summary>
    /// Indicates that compiler support for a particular feature is required for the location where this attribute is applied.
    /// </summary>
    internal class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string name)
        {
        }
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Specifies that this constructor sets all required members for the current type,
    /// and callers do not need to set any required members themselves.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    internal sealed class SetsRequiredMembersAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies that null is allowed as an input even if the corresponding type disallows it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    internal sealed class AllowNullAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies that null is disallowed as an input even if the corresponding type allows it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    internal sealed class DisallowNullAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies that a method that will never return under any circumstance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class DoesNotReturnAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies that the method will not return if the associated Boolean parameter is passed the specified value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class DoesNotReturnIfAttribute : Attribute
    {
        public bool ParameterValue { get; }

        public DoesNotReturnIfAttribute(bool parameterValue)
        {
            this.ParameterValue = parameterValue;
        }
    }

    /// <summary>
    /// Specifies that an output may be null even if the corresponding type disallows it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    internal sealed class MaybeNullAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies that when a method returns ReturnValue, the parameter may be null even if the corresponding type disallows it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class MaybeNullWhenAttribute : Attribute
    {
        public bool ReturnValue { get; }

        public MaybeNullWhenAttribute(bool returnValue)
        {
            this.ReturnValue = returnValue;
        }
    }

    /// <summary>
    /// Specifies that the method or property will ensure that the listed field and property members have not-null values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    internal sealed class MemberNotNullAttribute : Attribute
    {
        public string[] Members { get; }

        public MemberNotNullAttribute(string member)
        {
            this.Members = [member];
        }

        public MemberNotNullAttribute(params string[] members)
        {
            this.Members = members;
        }
    }

    /// <summary>
    /// Specifies that the method or property will ensure that the listed field and property members have non-null values when returning with the specified return value condition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    internal sealed class MemberNotNullWhenAttribute : Attribute
    {
        public bool ReturnValue { get; }
        public string[] Members { get; }

        public MemberNotNullWhenAttribute(bool returnValue, string member)
        {
            this.ReturnValue = returnValue;
            this.Members = [member];
        }

        public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
        {
            this.ReturnValue = returnValue;
            this.Members = members;
        }
    }

    /// <summary>
    /// Specifies that an output is not null even if the corresponding type allows it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    internal sealed class NotNullAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies that the output will be non-null if the named parameter is non-null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true)]
    internal sealed class NotNullIfNotNullAttribute : Attribute
    {
        public string ParameterName { get; }

        public NotNullIfNotNullAttribute(string parameterName)
        {
            this.ParameterName = parameterName;
        }
    }

    /// <summary>
    /// Specifies that when a method returns ReturnValue, the parameter will not be null even if the corresponding type allows it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        public bool ReturnValue { get; }

        public NotNullWhenAttribute(bool returnValue)
        {
            this.ReturnValue = returnValue;
        }
    }
}

namespace System
{
    /// <summary>Represent a type can be used to index a collection either from the start or the end.</summary>
    internal readonly struct Index : IEquatable<Index>
    {
        private readonly int value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Index(int value, bool fromEnd = false)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");

            this.value = fromEnd ? ~value : value;
        }

        private Index(int value) => this.value = value;

        public static Index Start => new(0);
        public static Index End => new(~0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Index FromStart(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
            return new Index(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Index FromEnd(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "value must be non-negative");
            return new Index(~value);
        }

        public int Value => this.value < 0 ? ~this.value : this.value;
        public bool IsFromEnd => this.value < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetOffset(int length)
        {
            var offset = this.value;
            if (this.IsFromEnd)
                offset += length + 1;
            return offset;
        }

        public override bool Equals(object? val) => val is Index index && this.value == index.value;
        public bool Equals(Index other) => this.value == other.value;
        public override int GetHashCode() => this.value;
        public static implicit operator Index(int value) => FromStart(value);
        public override string ToString() => this.IsFromEnd ? "^" + ((uint)this.Value).ToString() : ((uint)this.Value).ToString();
    }

    /// <summary>Represent a range has start and end indexes.</summary>
    internal readonly struct Range : IEquatable<Range>
    {
        public Index Start { get; }
        public Index End { get; }

        public Range(Index start, Index end)
        {
            this.Start = start;
            this.End = end;
        }

        public override bool Equals(object value) =>
            value is Range r && r.Start.Equals(this.Start) && r.End.Equals(this.End);

        public bool Equals(Range other) => other.Start.Equals(this.Start) && other.End.Equals(this.End);
        public override int GetHashCode() => this.Start.GetHashCode() * 31 + this.End.GetHashCode();
        public override string ToString() => this.Start + ".." + this.End;

        public static Range StartAt(Index start) => new(start, Index.End);
        public static Range EndAt(Index end) => new(Index.Start, end);
        public static Range All => new(Index.Start, Index.End);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int Offset, int Length) GetOffsetAndLength(int length)
        {
            var start = this.Start.IsFromEnd ? length - this.Start.Value : this.Start.Value;
            var end = this.End.IsFromEnd ? length - this.End.Value : this.End.Value;

            if ((uint)end > (uint)length || (uint)start > (uint)end)
                throw new ArgumentOutOfRangeException(nameof(length));

            return (start, end - start);
        }
    }
}

#endif // NETSTANDARD2_0
