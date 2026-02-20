using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class ListaBiddingOfertaAceitamento
    {
        public int CodigoTransportador { get; set; }
        public int CodigoRota { get; set; }
        public TipoLanceBidding TipoOferta { get; set; }
        public decimal CustoEstimado { get; set; }
        public bool NaoOfertar { get; set; }
        public decimal Quilometragem { get; set; }
        public decimal ValorFixo { get; set; }
        public decimal ValorFranquia { get; set; }
        public decimal ValorFixoEquipamento { get; set; }
        public decimal ValorFixoMensal { get; set; }
        public decimal ValorKmRodado { get; set; }
        public decimal Porcentagem { get; set; }
        public decimal ValorViagem { get; set; }
        public decimal ValorEntrega { get; set; }
        public decimal FreteTonelada { get; set; }
        public decimal PedagioParaEixo { get; set; }
        public decimal ICMSPorcentagem { get; set; }
        public decimal AdicionalPorEntrega { get; set; }
        public decimal Ajudante { get; set; }
    }
}