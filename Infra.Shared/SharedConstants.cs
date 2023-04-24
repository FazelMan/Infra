namespace Infra.Shared
{
    public static class SharedConstants
    {
        public static class CoreEnvironmentName
        {
            public const string CI = nameof(CI);
            public const string Testing = nameof(Testing);
            public const string Local = nameof(Local);
            public const string Qa = nameof(Qa);
        }

        public class Elastic
        {
            public const string DEFAULT_INDEX = "default";
        }
        
        public static class HttpHeadersKey
        {
            public const string CoreContext = "Core-Context";
            public const string UserId = "User-Id";
        }


        public static class HttpContextItems
        {
            public const string CoreContext = HttpHeadersKey.CoreContext;
        }

    }
}
