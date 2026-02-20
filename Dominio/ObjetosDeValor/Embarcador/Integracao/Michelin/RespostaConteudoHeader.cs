using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin
{
    public class RespostaConteudoHeader
    {
        [JsonProperty(PropertyName = "branchFactoryDocument", Required = Required.Default)]
        public string CNPJRemetente { get; set; }
    }
}
