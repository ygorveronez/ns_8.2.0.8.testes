using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe.CTes
{
    public sealed class CustoRentabilidadeCteCrt
    {
        #region Propriedades

        public int CodigoCargaOriginal { get; set; }

        public string NotasFiscais { get; set; }
        public string Tomador { get; set; }
        public string CargaVenda { get; set; }
        public string NumeroDocumentoCTe { get; set; }
        public string CodigoIntegracaoFilial { get; set; }

        public DateTime DataEmissaoOriginal { get; set; }
        public decimal ValorBrutoOriginal { get; set; }
        public decimal ValorPisOriginal { get; set; }
        public decimal ValorCofinsOriginal { get; set; }
        public decimal ValorIssNfOriginal { get; set; }
        public decimal ValorIcmsOriginal { get; set; }
        public decimal ValorLiquidoOriginal { get; set; }
        public string TransportadoraOriginal { get; set; }
        private string CnpjTransportadoraOriginal { get; set; }

        public string NumeroProvisao { get; set; }
        public DateTime DataEmissaoProvisao { get; set; }
        public decimal ValorBrutoProvisao { get; set; }
        public decimal ValorImpostoProvisao { get; set; }
        public decimal ValorPisProvisao { get; set; }
        public decimal ValorCofinsProvisao { get; set; }
        public decimal ValorIcmsProvisao { get; set; }
        public decimal ValorLiquidoProvisao { get; set; }

        public DateTime DataEmissaoCTeEspelho { get; set; }
        public decimal ValorBrutoCTeEspelho { get; set; }
        public decimal ValorPisCTeEspelho { get; set; }
        public decimal ValorCofinsCTeEspelho { get; set; }
        public decimal ValorIssNfEspelho { get; set; }
        public decimal ValorIcmsCTeEspelho { get; set; }
        public decimal ValorLiquidoCTeEspelho { get; set; }
        public string TransportadoraEspelho { get; set; }
        private string CnpjTransportadoraEspelho { get; set; }

        public decimal ReceitaDolar { get; set; }
        public decimal ProvisaoDolar { get; set; }
        public decimal DespesaDolar { get; set; }
        public decimal TaxaConversaoMoeda { get; set; }

        #endregion

        #region Propriedades com Regras

        public decimal Rentabilidade
        {
            get
            {
                decimal valorSubtrair = ValorLiquidoCTeEspelho > 0 ? ValorLiquidoCTeEspelho : ValorLiquidoProvisao;
                return ValorLiquidoOriginal - valorSubtrair;
            }
        }

        public decimal RentabilidadeDolar
        {
            get
            {
                decimal valorSubtrair = DespesaDolar > 0 ? DespesaDolar : ProvisaoDolar;
                return ReceitaDolar - valorSubtrair;
            }
        }

        public string CnpjTransportadoraOriginalFormatado
        {
            get
            {
                if (string.IsNullOrEmpty(CnpjTransportadoraOriginal))
                    return CnpjTransportadoraOriginal;

                return CnpjTransportadoraOriginal.ObterCpfOuCnpjFormatado();
            }
        }

        public string CnpjTransportadoraEspelhoFormatado
        {
            get
            {
                if (string.IsNullOrEmpty(CnpjTransportadoraEspelho))
                    return CnpjTransportadoraEspelho;

                return CnpjTransportadoraEspelho.ObterCpfOuCnpjFormatado();
            }
        }

        #endregion
    }
}
