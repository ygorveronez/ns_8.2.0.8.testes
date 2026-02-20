
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Eship
{
    public class ControlePortaria
    {
        [JsonProperty(PropertyName = "auth", Required = Required.Default)]
        public ControlePortariaAutenticacao Autenticacao { get; set; }

        [JsonProperty(PropertyName = "parameters", Required = Required.Default)]
        public ControlePortariaParametros Parametros { get; set; }
    }
}
