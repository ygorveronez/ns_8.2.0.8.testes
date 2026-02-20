using System;
using System.Text.Json.Serialization;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento
{
    public class IntegracaoPagamento
    {
        [JsonIgnore]
        public int CodigoIntPagamento { get; set; }
        [JsonIgnore]
        public int CodigoIntDocumentoPagamento { get; set; }
        [JsonIgnore]
        public TipoIntegracao TipoInt { get; set; }
        public string Tipo
        {
            get
            {
                return TipoInt.ObterDescricao();
            }
        }
        [JsonIgnore]
        public SituacaoIntegracao SituacaoIntegracaoInt { get; set; }
        public string SituacaoIntegracao
        {
            get
            {
                return SituacaoIntegracaoInt.ObterDescricao();
            }
        }
        public string MensagemRetorno { get; set; }
        public DateTime DataEnvioIntegracao { get; set; }
    }
}
