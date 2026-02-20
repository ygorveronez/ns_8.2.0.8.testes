using Microsoft.AspNetCore.Mvc;

namespace SGT.Riachuelo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PubSubMessageriaController : ControllerBase
    {
        private string ProjectId = "teste-pubsub-371620";
        private string TopicId = "Teste-PubSubMultiSoftware";

        [HttpGet(Name = "GetWeatherForecast")]
        async public Task<bool> PublicarMensagem(string messagem)
        {
            if (string.IsNullOrEmpty(messagem))
                return false;

            Service.PubSub servioPubSub = new Service.PubSub();
           var respotas =  await servioPubSub.PublicarMensagemAsyn(ProjectId, TopicId, new List<string>() { messagem });

            return true;
        }
    }
}
