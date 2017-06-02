using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Qwiq
{
    /// <summary>
    ///     Helper class that converts objects (as they are returned from the TFS API) to usable types.
    /// </summary>
    public class TypeParser : ITypeParser
    {
        private TypeParser()
        {
        }

        public static ITypeParser Default => Nested.Instance;

        public object Parse(Type destinationType, object value, object defaultValue)
        {
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));
            return ParseImpl(destinationType, value, defaultValue);
        }

        public object Parse(Type destinationType, object input)
        {
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));
            return ParseImpl(destinationType, input);
        }

        public T Parse<T>(object value)
        {
            return Parse(value, default(T));
        }

        public T Parse<T>(object value, T defaultValue)
        {
            return (T)Parse(typeof(T), value, defaultValue);
        }

        private static object GetDefaultValueOfType(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        private static bool IsGenericNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>).GetGenericTypeDefinition();
        }

        [CanBeNull]
        private static object ParseImpl([NotNull] Type destinationType, [CanBeNull] object value)
        {
            // If the incoming value is null, return the default value
            if (ValueRepresentsNull(value)) return GetDefaultValueOfType(destinationType);

            var valueType = value.GetType();

            // Quit if no type conversion is actually required
            if (valueType == destinationType) return value;
            if (destinationType.IsInstanceOfType(value)) return value;

            switch (Type.GetTypeCode(destinationType))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                    var destNullable = IsGenericNullable(destinationType);
                    if (!destNullable && valueType == typeof(string))
                    {
                        var val = (string)value;
                        if (string.IsNullOrEmpty(val))
                        {
                            return GetDefaultValueOfType(destinationType);
                        }
                    }
                    break;
            }

            if (TryConvert(destinationType, value, out object result)) return result;

            var defaultValue = GetDefaultValueOfType(destinationType);
            if (IsGenericNullable(destinationType) && defaultValue == null) return null;
            if (TryConvert(destinationType, defaultValue, out result)) return result;

            return null;
        }

        [CanBeNull]
        private static object ParseImpl([NotNull] Type destinationType, [CanBeNull] object value, [CanBeNull] object defaultValue)
        {
            // If the incoming value is null, return the default value
            if (ValueRepresentsNull(value)) return defaultValue;

            var valueType = value.GetType();

            // Quit if no type conversion is actually required
            if (valueType == destinationType) return value;
            if (destinationType.IsInstanceOfType(value)) return value;

            switch (Type.GetTypeCode(destinationType))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                    var destNullable = IsGenericNullable(destinationType);
                    if (!destNullable && valueType == typeof(string))
                    {
                        var val = (string)value;
                        if (string.IsNullOrEmpty(val))
                        {
                            return defaultValue;
                        }
                    }
                    break;
            }

            if (TryConvert(destinationType, value, out object result)) return result;
            if (IsGenericNullable(destinationType) && defaultValue == null) return null;
            if (TryConvert(destinationType, defaultValue, out result)) return result;

            return null;
        }

        private static bool TryConvert(Type destinationType, object value, out object result)
        {
            if (IsGenericNullable(destinationType))
                try
                {
                    var converter = new NullableConverter(destinationType);
                    result = converter.ConvertTo(value, converter.UnderlyingType);
                    return true;
                }
                // ReSharper disable CatchAllClause
                catch
                // ReSharper restore CatchAllClause
                {
                }

            var valueType = value.GetType();
            var typeConverter = TypeDescriptor.GetConverter(valueType);
            if (typeConverter.CanConvertTo(destinationType))
                try
                {
                    result = typeConverter.ConvertTo(value, destinationType);
                    return true;
                }
                // ReSharper disable CatchAllClause
                catch
                // ReSharper restore CatchAllClause
                {
                }

            typeConverter = TypeDescriptor.GetConverter(destinationType);
            if (typeConverter.CanConvertFrom(valueType))
                try
                {
                    result = typeConverter.ConvertFrom(value);
                    return true;
                }
                // ReSharper disable CatchAllClause
                catch
                // ReSharper restore CatchAllClause
                {
                }

            if (value != null)
            {
                var val = value.ToString();
                if (!string.IsNullOrEmpty(val))
                    if (typeConverter.IsValid(val))
                        try
                        {
                            result = typeConverter.ConvertFromString(val);
                            return true;
                        }
                        // ReSharper disable EmptyGeneralCatchClause
                        catch
                        // ReSharper restore EmptyGeneralCatchClause
                        {
                        }
            }

            result = null;
            return false;
        }

        [ContractAnnotation("value:null => true")]
        private static bool ValueRepresentsNull([CanBeNull] object value)
        {
            return value == null || value == DBNull.Value;
        }

        // ReSharper disable ClassNeverInstantiated.Local
        private class Nested
        // ReSharper restore ClassNeverInstantiated.Local
        {
            // ReSharper disable MemberHidesStaticFromOuterClass
            internal static readonly ITypeParser Instance = new TypeParser();

            // ReSharper restore MemberHidesStaticFromOuterClass

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
            static Nested()
            {
            }
        }
    }
}