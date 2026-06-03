using System.Reflection;
using System.Text.RegularExpressions;
using JebbyJump.Analytics;
using NUnit.Framework;

namespace JebbyJump.Tests
{
    // Guards the central catalog: every event name / parameter key const
    // must be a non-empty snake_case wire string. Reflection keeps this in
    // sync automatically as the catalog grows.
    public class AnalyticsCatalogTests
    {
        private static readonly Regex SnakeCase =
            new Regex("^[a-z][a-z0-9_]*$");

        [Test]
        public void AnalyticsEvents_AreNonEmptySnakeCase()
        {
            AssertAllConstStringsSnakeCase(typeof(AnalyticsEvents));
        }

        [Test]
        public void AnalyticsParams_AreNonEmptySnakeCase()
        {
            AssertAllConstStringsSnakeCase(typeof(AnalyticsParams));
        }

        private static void AssertAllConstStringsSnakeCase(System.Type type)
        {
            var consts = type.GetFields(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            int checkedCount = 0;
            foreach (var f in consts)
            {
                if (!f.IsLiteral || f.IsInitOnly) continue;
                if (f.FieldType != typeof(string)) continue;
                var value = (string)f.GetRawConstantValue();
                Assert.IsFalse(string.IsNullOrEmpty(value),
                    type.Name + "." + f.Name + " is empty.");
                Assert.IsTrue(SnakeCase.IsMatch(value),
                    type.Name + "." + f.Name + " = '" + value
                    + "' is not snake_case.");
                checkedCount++;
            }
            Assert.Greater(checkedCount, 0,
                "Expected at least one const string in " + type.Name);
        }
    }
}
