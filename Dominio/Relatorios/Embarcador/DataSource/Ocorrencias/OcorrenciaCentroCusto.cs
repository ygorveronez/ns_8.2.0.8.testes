using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Ocorrencias
{
    public sealed class OcorrenciaCentroCusto
    {
        #region Propriedades

        public string CnpjTransportador { get; set; }

        public string CodigoIntegracaoCentroCusto { get; set; }

        public double CpfCnpjCentroCusto { get; set; }

        public string DescricaoCentroCusto { get; set; }

        public string NumeroCarga { get; set; }

        public int NumeroCte { get; set; }

        public int NumeroOcorrencia { get; set; }

        public string RazaoSocialTransportador { get; set; }

        public string TipoCentroCusto { get; set; }

        public decimal ValorReceberCte { get; set; }

        #endregion

        #region Propriedades com Regras

        public string CnpjTransportadorFormatado
        {
            get { return CnpjTransportador.ObterCnpjFormatado(); }
        }

        public string CpfCnpjCentroCustoFormatado
        {
            get { return CpfCnpjCentroCusto > 0d ? CpfCnpjCentroCusto.ToString().ObterCpfOuCnpjFormatado(TipoCentroCusto) : ""; }
        }

        #endregion
    }
}
