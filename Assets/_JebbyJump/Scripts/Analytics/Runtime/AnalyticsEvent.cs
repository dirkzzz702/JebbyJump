using System.Collections.Generic;

namespace JebbyJump.Analytics
{
    // An immutable analytics event: a snake_case name plus optional simple
    // parameters. Construction normalizes the name (trim); empty names are
    // rejected by AnalyticsService before a sink ever sees them.
    public readonly struct AnalyticsEvent
    {
        public string Name { get; }
        public IReadOnlyList<AnalyticsParam> Parameters { get; }

        public AnalyticsEvent(string name, IReadOnlyList<AnalyticsParam> parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }
}
