using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class MultaParcela
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Veiculo { get; set; }
        public string NumeroAtuacao { get; set; }
        public int NumeroMulta { get; set; }
        private DateTime DataMulta { get; set; }
        private ResponsavelPagamentoInfracao PagoPor { get; set; }
        public string LocalInfracao { get; set; }
        public string Cidade { get; set; }
        public string TipoInfracao { get; set; }
        public string Motorista { get; set; }
        public string Pessoa { get; set; }
        private DateTime Vencimento { get; set; }
        private DateTime Compensacao { get; set; }
        public decimal ValorAteVencimento { get; set; }
        public decimal ValorAposVencimento { get; set; }
        public int CodigoTitulo { get; set; }
        private StatusTitulo SituacaoTitulo { get; set; }
        public decimal SaldoTitulo { get; set; }
        private SituacaoInfracao StatusMulta { get; set; }
        public string NumeroMatriculaMotorista { get; set; }
        public int NumeroParcela { get; set; }
        public int TituloParcela { get; set; }

        private NivelInfracaoTransito Nivel { get; set; }
        public decimal ValorTipoInfracao { get; set; }
        public int PontosTipoInfracao { get; set; }
        public decimal ReducaoComissao { get; set; }

        private DateTime VencimentoPagar { get; set; }
        public string FornecedorPagar { get; set; }
        public string CodigoIntegracaoMotorista { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DescricaoNivel
        {
            get { return Nivel.ObterDescricao(); }
        }

        public string DescricaoStatusMulta
        {
            get { return StatusMulta.ObterDescricao(); }
        }
        public string DescricaoSituacaoTitulo
        {
            get { return SituacaoTitulo.ObterDescricao(); }
        }
        public string DescricaoCompensacao
        {
            get { return Compensacao > DateTime.MinValue ? Compensacao.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DescricaoVencimento
        {
            get { return Vencimento > DateTime.MinValue ? Vencimento.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DescricaoDataMulta
        {
            get { return DataMulta > DateTime.MinValue ? DataMulta.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoVencimentoPagar
        {
            get { return VencimentoPagar > DateTime.MinValue ? VencimentoPagar.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoPagoPor
        {
            get { return PagoPor.ObterDescricao(); }
        }

        #endregion
    }
}
