using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public sealed class JanelaAgendamento
    {
        #region Propriedades

        public bool AgendaExtra { get; set; }
        public int Codigo { get; set; }
        public double CnpjFornecedor { get; set; }
        public string DataDescarregamento { get; set; }
        public string DataTentativa { get; set; }
        public string Filial { get; set; }
        public string Fornecedor { get; set; }
        public string HoraDescarregamento { get; set; }
        public string Modalidade { get; set; }
        public string ModeloVeicular { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroPedido { get; set; }
        public int QuantidadeCaixas { get; set; }
        public int QuantidadeItens { get; set; }
        public int QuantidadeCaixasPedido { get; set; }
        public string ResponsavelConfirmacao { get; set; }
        public string Senha { get; set; }
        public SituacaoAgendamentoColeta SituacaoAgendamento { get; set; }
        public SituacaoCarga SituacaoCarga { get; set; }
        public DateTime DataConfirmacaoAgenda { get; set; }
        public DateTime DataSolicitacaoAgenda { get; set; }
        public decimal ValorTotalPedido { get; set; }
        public decimal ValorMedioCaixa { get; set; }
        public decimal ValorAgendado { get; set; }
        public string OperadorAgendamento { get; set; }
        public string Observacao { get; set; }
        public string GrupoProduto { get; set; }
        public TipoAcaoParcial TipoAcaoParcial { get; set; }
        public int QuantidadeCaixasParcial { get; set; }
        public string CodigoIntegracaoProduto { get; set; }
        public string DescricaoProduto { get; set; }
        public decimal ValorProdutoAgendado { get; set; }
        public int QtdProdutoAgendado { get; set; }
        public SituacaoCargaJanelaDescarregamento SituacaoJanela {  get; set; }
        #endregion

        #region Propriedades com Regras

        public string CnpjFornecedorFormatado
        {
            get
            {
                return this.CnpjFornecedor > 0 ? this.CnpjFornecedor.ToString().ObterCnpjFormatado() : "";
            }
        }
        public string DescricaoSituacaoAgendamento
        {
            get
            {
                return SituacaoAgendamento.ObterDescricao();
            }
        }

        public string AgendaExtraDescricao
        {
            get
            {
                return this.AgendaExtra ? "Sim" : "Não";
            }
        }
        public string DataConfirmacaoAgendaFormatada
        {
            get
            {
                return this.DataConfirmacaoAgenda != DateTime.MinValue ? this.DataConfirmacaoAgenda.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }
        public string DataSolicitacaoAgendaFormatada
        {
            get
            {
                return this.DataSolicitacaoAgenda != DateTime.MinValue ? this.DataSolicitacaoAgenda.ToString("dd/MM/yyyy HH:mm") : "";
            }
        }

        public int QtdItensAgendado
        {
            get
            {
                if (QtdProdutoAgendado > 0)
                    return 1;
                return 0;
            }
        }

        public int QuantidadeCaixasDevolvidas
        {
            get
            {
                if (TipoAcaoParcial == TipoAcaoParcial.DevolvidaParcialmente)
                {
                    return QuantidadeCaixasParcial;
                }
                return 0;
            }
        }
        public int QuantidadeCaixasNaoEntregues
        {
            get
            {
                if (TipoAcaoParcial == TipoAcaoParcial.EntregueParcialmente)
                {
                    return QuantidadeCaixasParcial;
                }
                return 0;
            }
        }

        public string SituacaoJanelaDescricao
        {
            get
            {
                return this.SituacaoJanela.ObterDescricao();
            }
        }
        #endregion
    }
}
