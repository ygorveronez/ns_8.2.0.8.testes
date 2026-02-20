using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public sealed class RetornoObterAgendamentos
    {
        public string DataDescarregamento { get; set; }
        public string HoraDescarregamento { get; set; }
        public string DataTentativa { get; set; }
        public string Senha { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Fornecedor { get; set; }
        public string NumeroPedidoEmbarcador { get; set; } 
        public string Modalidade { get; set; }
        public Embarcador.Filial.Filial Filial { get; set; }
        public int QuantidadeCaixas { get; set; } 
        public int QuantidadeItens { get; set; } 
        public string ModeloVeicular { get; set; } 
        public SituacaoCargaJanelaDescarregamento SituacaoAgendamento { get; set; }
        public bool AgendaExtra { get; set; }
        public string ResponsavelConfirmacao { get; set; }
        public string NumeroCarga { get; set; }
        public string DataConfirmacao { get; set; }
        public string DataSolicitacao { get; set; }
        public decimal ValorTotalPedido { get; set; }
        public int QuantidadeCaixasPedido { get; set; }
        public decimal ValorMedioCaixa { get; set; }
        public decimal ValorAgendado { get; set; }
        public string OperadorAgendamento { get; set; }
        public string GrupoProduto { get; set; } 
        public string TipoCarga { get; set; } 
        public string Observacao { get; set; }
    }
}
