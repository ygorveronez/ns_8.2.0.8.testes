namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class PlanejamentoCarga
    {

        public int Codigo { get; set; }
        public int CodigoFrota { get; set; }
        public int CodigoCarga { get; set; }
        public string PlacaTracao { get; set; }
        public string PlacaReboque1 { get; set; }
        public string PlacaReboque2 { get; set; }
        public string Motorista { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprometimentoFrota? SituacaoComprometimentoFrota { get; set; }

        public virtual string PlacaVeiculo
        {
            get
            {
                string placa = PlacaTracao;
                if (!string.IsNullOrEmpty(PlacaReboque1))
                    placa += " / " + PlacaReboque1;

                if (!string.IsNullOrEmpty(PlacaReboque2))
                    placa += " / " + PlacaReboque2;

                return placa;
            }
        }

    }
}
