using Infrastructure.Interfaces.Logging;

namespace Infrastructure.Services.Logging
{
    public static class Logger
    {
        private static IAppLogger? _current;

        public static void Configure(IAppLogger logger)
        {
            _current = logger;
        }

        public static IAppLogger Current => _current ?? DefaultAppLogger.Instance;
    }
}
