using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Michelin
{
    public class RespostaConteudo
    {
        [JsonProperty(PropertyName = "fileName", Required = Required.Default)]
        public string NomeArquivo { get; set; }

        [JsonProperty(PropertyName = "created", Required = Required.Default)]
        public DateTime? DataCriacao { get; set; }

        [JsonProperty(PropertyName = "header", Required = Required.Default)]
        public RespostaConteudoHeader Header { get; set; }

        [JsonProperty(PropertyName = "details", Required = Required.Default)]
        public List<RespostaConteudoDetalhe> Detalhes { get; set; }
    }
}
