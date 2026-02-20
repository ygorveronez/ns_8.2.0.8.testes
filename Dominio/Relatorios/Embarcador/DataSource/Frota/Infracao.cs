using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class Infracao
    {
        #region Propriedades

        public string Veiculo { get; set; }
        public string NumeroAtuacao { get; set; }
        public int NumeroMulta { get; set; }
        public DateTime DataMulta { get; set; }
        public ResponsavelPagamentoInfracao PagoPor { get; set; }
        public string LocalInfracao { get; set; }
        public string Cidade { get; set; }
        public string TipoInfracao { get; set; }
        public TipoInfracaoTransito TipoTipoInfracao { get; set; }
        public string Motorista { get; set; }
        public string Pessoa { get; set; }
        public DateTime Vencimento { get; set; }
        public DateTime Compensacao { get; set; }
        public TipoHistoricoInfracao UltimoHistorico { get; set; }
        public decimal ValorAteVencimento { get; set; }
        public decimal ValorAposVencimento { get; set; }
        public int CodigoTitulo { get; set; }
        public StatusTitulo SituacaoTitulo { get; set; }
        public decimal SaldoTitulo { get; set; }
        public SituacaoInfracao StatusMulta { get; set; }
        public string NumeroMatriculaMotorista { get; set; }
        public string NumerosParcelasMulta { get; set; }
        public string TitulosParcelasMulta { get; set; }

        public NivelInfracaoTransito Nivel { get; set; }
        public decimal ValorTipoInfracao { get; set; }
        public int PontosTipoInfracao { get; set; }
        public decimal ReducaoComissao { get; set; }
        public string NumeroFrota { get; set; }

        public DateTime VencimentoPagar { get; set; }
        public string FornecedorPagar { get; set; }
        public string Observacao { get; set; }
        private DateTime DataInfracao { get; set; }
        private string CPFMotorista { get; set; }
        public decimal ValorEtapa4 { get; set; }

        public string RGMotorista { get; set; }

        public string CodigoIntegracaoMotorista { get; set; }
        public decimal ValorNotaFiscal { get; set; }
        public decimal ValorEstimadoPrejuizo { get; set; }
        public DateTime DataLancamento { get; set; }
        public string OrgaoEmissor { get; set; }
        public TipoMotorista TipoMotorista { get; set; }
        public string UF { get; set; }

        public string AcertoViagem { get; set; }
        private DateTime DataEmissaoInfracao { get; set; }

        public DateTime DataAssinaturaMulta { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DescricaoNivel
        {
            get { return Nivel.ObterDescricao(); }
        }

        public string TipoTipoInfracaoDescricao
        {
            get { return TipoTipoInfracao.ObterDescricao(); }
        }

        public string UltimoHistoricoDescricao
        {
            get { return UltimoHistorico.ObterDescricao(); }
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

        public string DataInfracaoFormatada
        {
            get { return DataInfracao > DateTime.MinValue ? DataInfracao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string CPFMotoristaFormatada
        {
            get { return !string.IsNullOrWhiteSpace(CPFMotorista) ? CPFMotorista.ObterCpfOuCnpjFormatado() : string.Empty; }
        }

        public string DataLancamentoFormatada
        {
            get { return DataLancamento > DateTime.MinValue ? DataLancamento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoTipoMotorista
        {
            get { return TipoMotorista.ObterDescricao(); }
        }
        public string DataEmissaoInfracaoFormatada
        {
            get { return DataEmissaoInfracao > DateTime.MinValue ? DataEmissaoInfracao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        #endregion
    }
}
