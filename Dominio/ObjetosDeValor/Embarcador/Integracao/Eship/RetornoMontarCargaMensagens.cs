using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Eship
{
    public class RetornoMontarCargaMensagens
    {
        [JsonProperty(PropertyName = "mensagem", Required = Required.Default)]
        public RetornoMontarCargaMensagensMensagem Mensagem { get; set; }

    }
}
