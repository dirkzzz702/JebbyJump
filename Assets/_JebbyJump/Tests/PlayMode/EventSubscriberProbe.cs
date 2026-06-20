using System;
using System.Reflection;

namespace JebbyJump.Tests
{
    // P24 correction #6: concrete subscriber-leak measurement. A field-like C#
    // event compiles to a private backing delegate field of the same name; reading
    // its invocation-list length gives the current subscriber count (0 if null).
    // Used to assert static-event subscribers return to baseline after cycles.
    public static class EventSubscriberProbe
    {
        public static int Count(Type type, string eventName)
        {
            var field = type.GetField(eventName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (field == null)
                throw new ArgumentException(
                    $"No backing delegate field for event {type.Name}.{eventName} " +
                    "(custom add/remove events are not supported by this probe).");
            var del = field.GetValue(null) as Delegate;
            return del?.GetInvocationList().Length ?? 0;
        }
    }
}
