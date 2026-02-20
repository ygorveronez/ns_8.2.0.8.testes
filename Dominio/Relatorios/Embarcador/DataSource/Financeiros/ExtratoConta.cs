using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ExtratoConta
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public DateTime Data_Base { get; set; }
        public string Observacao { get; set; }
        public TipoDocumentoMovimento TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string CentroResultado { get; set; }
        public string Plano { get; set; }
        public string PlanoDescricao { get; set; }
        public string PlanoContraPartida { get; set; }
        public string PlanoDescricaoContraPartida { get; set; }
        public decimal ValorDebito { get; set; }
        public decimal ValorCredito { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoAnterior { get; set; }
        public int CodigoPlanoConta { get; set; }
        public string Colaborador { get; set; }
        public string GrupoFavorecido { get; set; }
        public string PessoaFavorecido { get; set; }

        public string NumeroDocumentoTitulo { get; set; }
        public string DocumentoTitulo { get; set; }
        public decimal AcrescimoTitulo { get; set; }
        public decimal DescontoTitulo { get; set; }
        public decimal ValorPagoTitulo { get; set; }
        public string PessoaTitulo { get; set; }
        public string Motorista { get; set; }

        private MoedaCotacaoBancoCentral MoedaCotacaoBancoCentral { get; set; }
        public decimal ValorMoedaCotacao { get; set; }
        public decimal ValorDebitoMoedaEstrangeira { get; set; }
        public decimal ValorCreditoMoedaEstrangeira { get; set; }
        public decimal SaldoMoedaEstrangeira { get; set; }
        public decimal SaldoAnteriorMoedaEstrangeira { get; set; }
        public bool UtilizaMoedaEstrangeira { get; set; }
        private DateTime DataVencimentoTitulo { get; set; }

        #region Propriedades com Regras

        public string DataFormatada
        {
            get { return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataBaseFormatada
        {
            get { return Data_Base != DateTime.MinValue ? Data_Base.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataVencimentoTituloFormatada
        {
            get { return DataVencimentoTitulo != DateTime.MinValue ? DataVencimentoTitulo.ToString("dd/MM/yyyy") : string.Empty; }
        }


        public string DescricaoTipoDocumento
        {
            get { return TipoDocumentoMovimentoHelper.ObterDescricao(TipoDocumento); }
        }

        public string MoedaCotacaoBancoCentralFormatada
        {
            get { return MoedaCotacaoBancoCentral.ObterDescricao(); }
        }

        #endregion
    }
}
