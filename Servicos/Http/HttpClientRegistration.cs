using Infrastructure.Services.HttpClientFactory;

namespace Servicos.Http
{
    public static class HttpClientRegistration
    {
        public static void RegisterClients()
        {
            HttpClientFactoryWrapper.Setup(nameof(Servicos.Embarcador.Integracao.Cebrace.IntegracaoCebrace), () => new System.Net.Http.HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            });

            HttpClientFactoryWrapper.Setup(nameof(Servicos.Embarcador.Integracao.Camil.IntegracaoCamil), () => new System.Net.Http.HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            });

            HttpClientFactoryWrapper.Setup(nameof(Servicos.Embarcador.Integracao.Cobasi.IntegracaoCobasi), () => new System.Net.Http.HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            });
            
            HttpClientFactoryWrapper.Setup(nameof(Servicos.Embarcador.Integracao.FS.IntegracaoFS), () => new System.Net.Http.HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            });

            HttpClientFactoryWrapper.Setup(nameof(Servicos.Embarcador.Integracao.Vedacit.IntegracaoJDEFaturas), () => new System.Net.Http.HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            });

            HttpClientFactoryWrapper.Setup(nameof(Servicos.Embarcador.Integracao.Lactalis.IntegracaoLactalis), () => new System.Net.Http.HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            });

            HttpClientFactoryWrapper.Setup(nameof(Servicos.Embarcador.Integracao.Olfar.IntegracaoOlfar), () => new System.Net.Http.HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            });

            HttpClientFactoryWrapper.Setup(nameof(Servicos.Embarcador.Integracao.Riachuelo.IntegracaoRiachuelo), () => new System.Net.Http.HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            });
        }
    }
}
