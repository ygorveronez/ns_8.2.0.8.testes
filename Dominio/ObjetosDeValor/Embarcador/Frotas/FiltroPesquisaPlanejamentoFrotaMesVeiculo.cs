namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public sealed class FiltroPesquisaPlanejamentoFrotaMesVeiculo
    {
        public int Ano { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public int CodigoPlanejamentoFrotaMes { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoTransportador { get; set; }

        public int Mes { get; set; }

        public Enumeradores.RespostaTransportadorPlanejamentoFrota? RespostaTransportador { get; set; }

        public Enumeradores.SituacaoPlanejamentoFrota? Situacao { get; set; }
    }
}
