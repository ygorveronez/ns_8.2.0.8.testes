using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta
{
    public class ConsultaCustoPedagioRota
    {
        [JsonProperty(PropertyName = "CNPJAplicacao", Required = Required.Default)]
        public string CNPJAplicacao { get; set; }

        [JsonProperty(PropertyName = "Token", Required = Required.Default)]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "CNPJEmpresa", Required = Required.Default)]
        public string CNPJEmpresa { get; set; }

        [JsonProperty(PropertyName = "TipoVeiculo", Required = Required.Default)]
        public int TipoVeiculo { get; set; }

        [JsonProperty(PropertyName = "QtdEixos", Required = Required.Default)]
        public int QtdEixos { get; set; }

        [JsonProperty(PropertyName = "ExibirDetalhes", Required = Required.Default)]
        public int ExibirDetalhes { get; set; }

        [JsonProperty(PropertyName = "Localizacoes", Required = Required.Default)]
        public List<ConsultaCustoPedagioRotaLocalizacao> Localizacoes { get; set; }
    }
}
