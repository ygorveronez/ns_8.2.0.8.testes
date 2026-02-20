using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Bind
{
    public class RequisicaoBind
    {
        [JsonProperty(PropertyName = "id_carga", Required = Required.Default)]
        public string NumeroCarga { get; set; }

        [JsonProperty(PropertyName = "etapa", Required = Required.Default)]
        public string Etapa { get; set; }

        [JsonProperty(PropertyName = "inicio", Required = Required.Default)]
        public DateTime? Inicio { get; set; }

        [JsonProperty(PropertyName = "fim", Required = Required.Default)]
        public DateTime? Fim { get; set; }

        [JsonProperty(PropertyName = "veiculo", Required = Required.Default)]
        public string Veiculo { get; set; }

        [JsonProperty(PropertyName = "prioridade", Required = Required.Default)]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeGrupoPessoas Prioridade { get; set; }
    }
}
