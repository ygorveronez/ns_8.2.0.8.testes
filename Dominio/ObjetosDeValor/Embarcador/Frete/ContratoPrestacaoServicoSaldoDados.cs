using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class ContratoPrestacaoServicoSaldoDados
    {
        public int CodigoContratoPrestacaoServico { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoTransportador { get; set; }

        public string Descricao { get; set; }

        public TipoLancamento TipoLancamento { get; set; }

        public TipoMovimentacaoContratoPrestacaoServico TipoMovimentacao { get; set; }

        public decimal Valor { get; set; }
    }
}
