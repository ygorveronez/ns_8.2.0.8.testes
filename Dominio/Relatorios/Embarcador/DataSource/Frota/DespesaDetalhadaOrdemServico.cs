using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class DespesaDetalhadaOrdemServico
    {
        public int Codigo { get; set; }
        public int Ano { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public string LocalManutencao { get; set; }
        public DateTime Data { get; set; }
        public string MesAnoOS { get; set; }
        public int Numero { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorServicos { get; set; }
        public decimal ValorTotal { get; set; }
        public int MesOS { get; set; }
        public int AnoOS { get; set; }
        public string Servico { get; set; }
        public string Produto { get; set; }
        public decimal ValorProdutosFechamento { get; set; }
        public decimal ValorServicosFechamento { get; set; }
        public decimal ValorTotalFechamento { get; set; }
        public string Tipo { get; set; }
        public SituacaoOrdemServicoFrota Situacao { get; set; }

        public string Observacao { get; set; }
        public string ModeloVeicular { get; set; }
        public int KMExecucao { get; set; }
        public int HorimetroExecucao { get; set; }
        public decimal KMUltimoAbastecimento { get; set; }
        public string CodigoProduto { get; set; }
        public decimal QuantidadeProduto { get; set; }
        public string NotaFiscal { get; set; }
        public string CentroResultado { get; set; }
        public string Operador { get; set; }
        public decimal ValorProduto { get; set; }
        public decimal ValorServico { get; set; }
        public decimal ValorOrcadoEmProdutos { get; set; }
        public decimal ValorOrcadoEmServicos { get; set; }
        public string DataFormatada
        {
            get { return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public string Equipamento { get; set; }
        public string LocalArmazenamento { get; set; }

        public string Responsavel { get; set; }
        public string ObservacaoServicos { get; set; }
        public string GrupoProduto { get; set; }
        public string Mecanicos { get; set; }
        public int TempoPrevisto { get; set; }
        public int TempoExecutado { get; set; }
    }
}
