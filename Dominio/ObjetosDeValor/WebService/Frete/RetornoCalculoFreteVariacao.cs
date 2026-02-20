using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Frete
{
    public class RetornoCalculoFreteVariacao
    {
        public string CodigoIntegracaoModeloVeicular { get; set; }
        public decimal ValorFrete { get; set; }
        public string CNPJTransportador { get; set; }
        public string RazaoSocialTransportador { get; set; }
        public string CodigoIntegracaoTransportador { get; set; }
        public string CodigoIntegracaoTipoCarga { get; set; }
        public string CodigoIntegracaoTipoOperacao { get; set; }
    }
}
