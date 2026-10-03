namespace eShop.Identity.API.Extensions;

#pragma warning disable LOGGEN002 // Extension-method logs use event id 0; keep that id.
internal static partial class IdentityApiTrace
{
    [LoggerMessage(EventId = 0, EventName = "AliceCreated", Level = LogLevel.Debug, Message = "alice created")]
    public static partial void LogAliceCreated(ILogger logger);

    [LoggerMessage(EventId = 0, EventName = "AliceAlreadyExists", Level = LogLevel.Debug, Message = "alice already exists")]
    public static partial void LogAliceAlreadyExists(ILogger logger);

    [LoggerMessage(EventId = 0, EventName = "BobCreated", Level = LogLevel.Debug, Message = "bob created")]
    public static partial void LogBobCreated(ILogger logger);

    [LoggerMessage(EventId = 0, EventName = "BobAlreadyExists", Level = LogLevel.Debug, Message = "bob already exists")]
    public static partial void LogBobAlreadyExists(ILogger logger);

    [LoggerMessage(EventId = 0, EventName = "HomepageDisabled", Level = LogLevel.Information, Message = "Homepage is disabled in production. Returning 404.")]
    public static partial void LogHomepageDisabled(ILogger logger);

    // {@claims} must stay an object for the existing formatter. The generator stringifies enumerables.
    private static readonly Action<ILogger, IEnumerable<string>, Exception> LogExternalClaimsCallback =
        LoggerMessage.Define<IEnumerable<string>>(
            LogLevel.Debug,
            new EventId(0, "ExternalClaims"),
            "External claims: {@claims}");

    public static void LogExternalClaims(ILogger logger, IEnumerable<string> claims)
        => LogExternalClaimsCallback(logger, claims, null);

    // Placeholder name "0" is rejected by the source generator (SYSLIB1014).
    private static readonly Action<ILogger, string, Exception> LogNoConsentRequestMatchingRequestCallback =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(0, "NoConsentRequestMatchingRequest"),
            "No consent request matching request: {0}");

    public static void LogNoConsentRequestMatchingRequest(ILogger logger, string returnUrl)
        => LogNoConsentRequestMatchingRequestCallback(logger, returnUrl, null);
}
#pragma warning restore LOGGEN002
