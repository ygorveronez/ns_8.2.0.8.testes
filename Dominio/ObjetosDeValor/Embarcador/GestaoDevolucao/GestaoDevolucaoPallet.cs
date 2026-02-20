using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao
{
    public class GestaoDevolucaoPallet
    {
        #region Atribútos Públicos
        public int CodigoNF { get; set; }
        public int CodigoCarga { get; set; }
        public long CodigoGestaoDevolucao { get; set; }
        public int NumeroNF { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string Chave { get; set; }
        public string Serie { get; set; }
        public string StringDataEmissao
        {
            get
            {
                return this.DataEmissao.ToString();
            }
        }
        public string Transportador { get; set; }
        public string CNPJTransportador { get; set; }
        public string Filial { get; set; }
        public string CNPJFilial { get; set; }
        public string Cliente { get; set; }
        public double CNPJCliente { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet ResponsavelMovimentacaoPallet { get; set; }
        public TipoNotaFiscalIntegrada TipoNF { get; set; }
        public string NumerosAtendimentos { get; set; }
        public string MotivoDevolucao { get; set; }
        public string DevolucaoTotal { get; set; }
        public string TipoTomadorDescricao { get; set; }
        public int NumeroNFDevolucao { get; set; }
        public decimal QuantidadePallets { get; set; }
        public string CodigoIntegracaoFilial { get; set; }
        public string DescricaoTipoNF
        {
            get
            {
                return this.TipoNF.ObterDescricao();
            }
        }
        public string DescricaoResponsavelPallet
        {
            get
            {
                return this.ResponsavelMovimentacaoPallet.ObterDescricao();
            }
        }
        public bool DevolucaoGerada
        {
            get
            {
                if (this.CodigoGestaoDevolucao > 0)
                    return true;

                return false;
            }
        }
        public string DescricaoDevolucaoGerada
        {
            get
            {
                if (!this.DevolucaoGerada)
                    return "NÃO";

                return "SIM";
            }
        }
        public string PrazoGeracaoDevolucao
        {
            get
            {
                if (!DevolucaoGerada && TipoNF == TipoNotaFiscalIntegrada.RemessaPallet)
                   return DataEmissao.AddDays(this.LimiteDiasDevolucao).ToString();

                return "";
            }
        }
        public string NomeCNPJTransportador
        {
            get
            {
                string descricao = string.Empty;

                if (!string.IsNullOrEmpty(this.Transportador))
                    descricao += this.Transportador;
                if (!string.IsNullOrEmpty(this.CNPJTransportadorFormatado))
                    descricao += " " + this.CNPJTransportadorFormatado;

                return descricao;
            }
        }
        public string NomeCNPJFilial
        {
            get
            {
                string descricao = string.Empty;

                if (!string.IsNullOrEmpty(this.Filial))
                    descricao += this.Filial;
                if (!string.IsNullOrEmpty(this.CNPJFilialFormatado))
                    descricao += " " + this.CNPJFilialFormatado;

                return descricao;
            }
        }
        public string NomeCNPJCliente
        {
            get
            {
                string descricao = string.Empty;

                if (!string.IsNullOrEmpty(this.Cliente))
                    descricao += this.Cliente;
                if (!string.IsNullOrEmpty(this.CNPJClienteFormatado))
                    descricao += " " + this.CNPJClienteFormatado;

                return descricao;
            }
        }
        #endregion

        #region Atribútos privados
        private string CNPJTransportadorFormatado
        {
            get
            {
                if (this.CNPJTransportador != null)
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJTransportador.ToString()));

                return string.Empty;
            }
        }
        private string CNPJFilialFormatado
        {
            get
            {
                return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJFilial.ToString()));
            }
        }
        private string CNPJClienteFormatado
        {
            get
            {
                return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJCliente.ToString()));
            }
        }
        private DateTime DataEmissao { get; set; }
        private int LimiteDiasDevolucao { get; set; }
        #endregion

    }
}

