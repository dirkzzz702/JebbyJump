namespace JebbyJump.Analytics
{
    // A single key/value pair on an analytics event. Values are kept to
    // simple types (string / int / float / bool) by convention; no Unity
    // objects, no PII.
    public readonly struct AnalyticsParam
    {
        public string Key { get; }
        public object Value { get; }

        public AnalyticsParam(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public static AnalyticsParam Of(string key, string value) =>
            new AnalyticsParam(key, value);
        public static AnalyticsParam Of(string key, int value) =>
            new AnalyticsParam(key, value);
        public static AnalyticsParam Of(string key, float value) =>
            new AnalyticsParam(key, value);
        public static AnalyticsParam Of(string key, bool value) =>
            new AnalyticsParam(key, value);
    }
}
