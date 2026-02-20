using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class PneuHistorico
    {
        public int Protocolo { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public string Servicos { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoPneuHistorico Tipo { get; set; }
        public Pneu Pneu { get; set; }
        public BandaRodagemPneu BandaRodagem { get; set; }
        public int KmAtualRodado { get; set; }
        public decimal CustoEstimado { get; set; }
        public DateTime? DataHoraMovimentacao { get; set; }
    }
}
