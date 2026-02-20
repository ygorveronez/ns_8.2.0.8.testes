using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaRelatorioPneu
    {
        public int CodigoPneu { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoModeloPneu { get; set; }
        public int CodigoMarcaPneu { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoBandaRodagemPneu> TiposBandaRodagem { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPneu? StatusPneu { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao? Movimentacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.VidaPneu? VidaUtil { get; set; }
    }
}
