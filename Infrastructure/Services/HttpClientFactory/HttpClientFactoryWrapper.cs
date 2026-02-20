using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Net.Http;

namespace Infrastructure.Services.HttpClientFactory
{
    public static class HttpClientFactoryWrapper
    {
        private static IHttpClientFactory _factory;
        private readonly static ServiceCollection _services;
        private readonly static ConcurrentDictionary<string, (HttpClient Client, DateTime Expiration)> _httpClientsWithCertificate = new ConcurrentDictionary<string, (HttpClient, DateTime)>();
        private const int _clientTtlInSecs = 60;

        static HttpClientFactoryWrapper()
        {
            // Build service collection.
            _services = new ServiceCollection();
            _services.AddHttpClient();

            // Create the dependency injection provider and request the creation of the factory.
            var serviceProvider = _services.BuildServiceProvider();
            _factory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        }

        public static void Setup(string identifier, Func<HttpClientHandler> configAction)
        {
            _services.AddHttpClient(identifier)
                    .ConfigurePrimaryHttpMessageHandler(configAction);

            _factory = _services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
        }

        public static HttpClient GetClient(string clientIdentifier)
        {
            return _factory.CreateClient(clientIdentifier);
        }

        public static HttpClient GetClientWithCertificate(string clientIdentifier, Func<System.Security.Cryptography.X509Certificates.X509Certificate2> certificateBuilder, Action<HttpClientHandler> configureHandler = null)
        {
            var client = _httpClientsWithCertificate.GetOrAdd(clientIdentifier, (id) =>
            {
                var certificado = certificateBuilder();
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(certificado);

                if (configureHandler != null)
                {
                    configureHandler(handler);
                }

                return (new HttpClient(handler), DateTime.Now.AddSeconds(_clientTtlInSecs));
            });

            if (DateTime.Now > client.Expiration)
            {
                _httpClientsWithCertificate.TryRemove(clientIdentifier, out _);
            }

            return client.Client;
        }
    }
}
