namespace JebbyJump.Analytics
{
    // Destination for analytics events. This phase ships only a local
    // DebugAnalyticsSink; a real provider (Firebase / GameAnalytics /
    // custom backend) would implement this interface later without any
    // gameplay change.
    public interface IAnalyticsSink
    {
        void Track(in AnalyticsEvent analyticsEvent);
    }
}
