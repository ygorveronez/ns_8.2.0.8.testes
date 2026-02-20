using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class FreteTerceirizadoValePedagio
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int ContratoFrete { get; set; }
        public string NumeroCTes { get; set; }

        public string PISPASEPTerceiro { get; set; }
        private double CPFCNPJTerceiro { get; set; }
        private string TipoTerceiro { get; set; }
        public string Terceiro { get; set; }
        private DateTime DataNascimentoTerceiro { get; set; }

        public decimal ValorPago { get; set; }
        public decimal ValorINSS { get; set; }
        public decimal ValorIRRF { get; set; }
        public decimal ValorSEST { get; set; }
        public decimal ValorSENAT { get; set; }
        private DateTime DataEmissao { get; set; }

        public string NumeroValePedagio { get; set; }
        private TipoIntegracao TipoIntegracao { get; set; }
        public decimal ValorValePedagio { get; set; }
        public string TransportadorTerceiro { get; set; }
        public string NomeTransportador { get; set; }

        #endregion

        #region Propriedades com Regras

        public string CPFCNPJTerceiroFormatado
        {
            get
            {
                if (TipoTerceiro == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoTerceiro == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJTerceiro) : string.Format(@"{0:000\.000\.000\-00}", CPFCNPJTerceiro);
            }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataNascimentoTerceiroFormatada
        {
            get { return DataNascimentoTerceiro != DateTime.MinValue ? DataNascimentoTerceiro.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string TipoIntegracaoFormatada
        {
            get { return TipoIntegracao.ObterDescricao(); }
        }

        #endregion
    }
}
