using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Piracanjuba
{
    public sealed class DocumentoCarga
    {
        [JsonIgnore]
        public bool AmbienteProducao { get; set; }

        [JsonIgnore]
        public string StringAmbiente { get; set; }

        [JsonProperty(PropertyName = "instance", Order = 1, Required = Required.Default)]
        public string AmbienteProducaoDescricao => !string.IsNullOrWhiteSpace(StringAmbiente) ? StringAmbiente : AmbienteProducao ? "prd" : "hml";

        [JsonProperty(PropertyName = "tor_id", Order = 2, Required = Required.Default)]
        public string NumeroCarga { get; set; }

        [JsonIgnore]
        public TipoDocumento? TipoDocumento { get; set; }

        [JsonProperty(PropertyName = "tipoInf", Order = 3, Required = Required.Default)]
        public string TipoDocumentoDescricao => TipoDocumento?.ObterValorIntegracao() ?? string.Empty;

        [JsonProperty(PropertyName = "valor", Order = 4, Required = Required.Default)]
        public string ValorDocumento { get; set; }

        [JsonProperty(PropertyName = "nDoc", Order = 5, Required = Required.Default)]
        public string NumeroDocumento { get; set; }

        [JsonProperty(PropertyName = "chave", Order = 6, Required = Required.Default)]
        public string Chave { get; set; }

        [JsonProperty(PropertyName = "chave_nf", Order = 7, Required = Required.Default)]
        public string ChaveNotaFiscal { get; set; }

        [JsonIgnore]
        public TipoArquivo? TipoArquivo { get; set; }

        [JsonProperty(PropertyName = "tipoConteudo", Order = 8, Required = Required.Default)]
        public string TipoArquivoDescricao => TipoArquivo?.ObterDescricao() ?? string.Empty;

        [JsonProperty(PropertyName = "conteudo", Order = 9, Required = Required.Default)]
        public string ConteudoArquivo { get; set; }

        [JsonProperty(PropertyName = "Componentes", Order = 9, Required = Required.Default)]
        public List<Componente> Componentes { get; set; }
    }
}
