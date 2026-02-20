using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class RelatorioOrdemServico
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int Numero { get; set; }
        private DateTime Data { get; set; }
        public string Veiculo { get; set; }
        public string NumeroFrota { get; set; }
        public string Motorista { get; set; }
        public string LocalManutencao { get; set; }
        public string Operador { get; set; }
        public TipoManutencaoOrdemServicoFrota TipoManutencao { get; set; }
        public string Tipo { get; set; }
        public string Observacao { get; set; }
        public SituacaoOrdemServicoFrota Situacao { get; set; }
        public string Equipamento { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorServicos { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorProdutosFechamento { get; set; }
        public decimal ValorServicosFechamento { get; set; }
        public decimal ValorTotalFechamento { get; set; }
        public string Servicos { get; set; }
        public string CidadeLocalManutencao { get; set; }
        public string UFLocalManutencao { get; set; }
        public int QuilometragemVeiculo { get; set; }
        public int Horimetro { get; set; }
        public string GrupoServico { get; set; }
        public string CentroResultado { get; set; }
        public string Segmento { get; set; }
        public string CidadePessoa { get; set; }
        public string UFPessoa { get; set; }
        public string CPFCNPJPessoa { get; set; }
        public string DocumentoEntrada { get; set; }
        public decimal DiferencaTotais { get; set; }
        public string OperadorFechamento { get; set; }
        private DateTime DataHoraInclusao { get; set; }
        public string Mecanicos { get; set; }
        public int TempoPrevisto { get; set; }
        public int TempoExecutado { get; set; }
        public string CondicaoPagamento { get; set; }
        public DateTime DataLimiteExecucao { get; set; }
        public PrioridadeOrdemServico Prioridade { get; set; }
        private DateTime DataLiberacao { get; set; }
        private DateTime DataFechamento { get; set; }
        private DateTime DataReabertura { get; set; }
        public string Produtos { get; set; }
        public string GrupoProdutos { get; set; }
        public decimal QuantidadeProduto { get; set; }
        public decimal ValorUnitarioProduto { get; set; }
        public string NomeFornecedorDocumentoEntrada { get; set; }
        public decimal ValorNF { get; set; } 
        public int NumeroOrdemCompra { get; set; }
        public string CodigoProduto { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataFormatada
        {
            get { return Data.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string DescricaoTipoManutencao
        {
            get { return TipoManutencao.ObterDescricao(); }
        }

        public string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public string DataHoraInclusaoFormatada
        {
            get { return DataHoraInclusao != DateTime.MinValue ? DataHoraInclusao.ToString("dd/MM/yyyy : HH:mm") : string.Empty; }
        }

        public decimal DiferencaValorOrcadoRealizado
        {
            get { return ValorTotal - ValorTotalFechamento; }
        }

        public string DataLimiteExecucaoFormatada
        {
            get { return DataLimiteExecucao != DateTime.MinValue ? DataLimiteExecucao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string PrioridadeDescricao
        {
            get { return Prioridade.ObterDescricao(); }
        }

        public string DataLiberacaoFormatada
        {
            get { return DataLiberacao != DateTime.MinValue ? DataLiberacao.ToString("dd/MM/yyyy : HH:mm") : string.Empty; }
        }

        public string DataFechamentoFormatada
        {
            get { return DataFechamento != DateTime.MinValue ? DataFechamento.ToString("dd/MM/yyyy : HH:mm") : string.Empty; }
        }

        public string DataReaberturaFormatada
        {
            get { return DataReabertura != DateTime.MinValue ? DataReabertura.ToString("dd/MM/yyyy : HH:mm") : string.Empty; }
        }

        public decimal ValorTotalProduto
        {
            get { return QuantidadeProduto > 0 && ValorUnitarioProduto > 0 ? QuantidadeProduto * ValorUnitarioProduto : 0; }
        }

        #endregion
    }
}
