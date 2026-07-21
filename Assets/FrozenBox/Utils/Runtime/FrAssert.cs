using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

#nullable disable warnings
namespace FrozenBox.Utils
{
    [DebuggerStepThrough]
    public static class FrAssert
    {
        private const string UNITY_ASSERTIONS = "UNITY_ASSERTIONS";
        private const string ExpectedString = "Expected:";
        private const string AssertionFailedString = "Assertion failure.";

        private static string Format(string fmt, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture.NumberFormat, fmt, args);
        }

        private static string GetMessage(string failureMessage)
        {
            return Format("{0} {1}", AssertionFailedString, failureMessage);
        }

        private static string GetMessage(string failureMessage, string expected)
        {
            return GetMessage(Format("{0}{1}{2} {3}", failureMessage, Environment.NewLine, ExpectedString, expected));
        }
        
        private static void Fail(string message, string? userMessage)
        {
            throw new AssertionException(message, userMessage);
        }

        [Conditional(UNITY_ASSERTIONS)]
        public static void IsInRange(int value, int min, int max, string? message = null)
        {
            if (value >= min && value <= max)
                return;

            Fail(GetMessage("Value is out of range", Format("value {{0}} is in range [{1};{2}]", value, min, max)),
                message);
        }

        [Conditional(UNITY_ASSERTIONS)]
        public static void IsInRange(float value, FloatRange range, string? message = null)
        {
            if (range.Contains(value))
                return;

            Fail(
                GetMessage("Value is out of range",
                    Format("value {{0}} is in range [{1};{2}]", value, range.Min, range.Max)), message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T? value) where T : class 
            => Assert.IsNull(value);

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T? value, string message) where T : class 
            => Assert.IsNull(value, message);

        /// <summary>
        ///     <para>Assert the value is null.</para>
        /// </summary>
        /// <param name="value">The Object or type being checked for.</param>
        /// <param name="message">The string used to describe the Assert.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull(Object? value, string message) 
            => Assert.IsNull(value, message);

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>([NotNull] T? value) where T : class 
            => Assert.IsNotNull(value, null);

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>([NotNull] T? value, string message) where T : class 
            => Assert.IsNotNull(value, message);

        /// <summary>
        ///     <para>Assert that the value is not null.</para>
        /// </summary>
        /// <param name="value">The Object or type being checked for.</param>
        /// <param name="message">The string used to describe the Assert.</param>
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull([NotNull] Object? value, string message) 
            => Assert.IsNotNull(value, message);

        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>([NotNull] T? value) where T : unmanaged
            => Assert.IsTrue(value.HasValue, null);
        
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>(T? value) where T : unmanaged
            => Assert.IsFalse(value.HasValue, null);
    }
}