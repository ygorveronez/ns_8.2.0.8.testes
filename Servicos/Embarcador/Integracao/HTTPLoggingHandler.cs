using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class HTTPLoggingHandler : DelegatingHandler
    {
        public HTTPLoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //Servicos.Log.TratarErro("Request: " + request.ToString(), "IntegracaoMundialRisk");

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            //Servicos.Log.TratarErro("Response: " + response.ToString(), "IntegracaoMundialRisk");

            return response;
        }
    }
}
