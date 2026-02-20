using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class Pneu
    {
        public DateTime DataEntrada { get; set; }
        public string DescricaoNota { get; set; }
        public string DTO { get; set; }
        public int KmAtualRodado { get; set; }
        public int KmAnteriorRodado { get; set; }
        public int KmRodadoEntreSulcos { get; set; }
        public string NumeroFogo { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoPneu Situacao { get; set; }
        public decimal Sulco { get; set; }
        public decimal SulcoAnterior { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoAquisicaoPneu TipoAquisicao { get; set; }
        public decimal ValorAquisicao { get; set; }
        public decimal ValorCustoAtualizado { get; set; }
        public decimal ValorCustoKmAtualizado { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.VidaPneu VidaAtual { get; set; }
        public Almoxarifado Almoxarifado { get; set; }
        public DateTime? DataMovimentacaoEstoque { get; set; }
        public DateTime? DataMovimentacaoReforma { get; set; }
        public ModeloPneu Modelo { get; set; }
        public BandaRodagemPneu BandaRodagem { get; set; }

    }
}
