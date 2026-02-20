namespace Dominio.ObjetosDeValor.EDI.GTA
{
    public class ParadaAves
    {
        public string Tipo { get; set; }
        public string Situacao { get; set; }
        public string Numero { get; set; }
        public int SequenciaPlanejada { get; set; }
        public int SequenciaRealizada { get; set; }
        public DadosGTA GTA { get; set; }
        public string HorarioChegadaProdutor { get; set; }
        public string HorarioInicioColeta { get; set; }
        public string HorarioTerminoColeta { get; set; }
        public string HorarioSaidaProdutor { get; set; }
        public string HorarioChegadaUnidade { get; set; }
        public Pesquisa Pesquisa { get; set; }
        public int QuantidadeAvesPlanejada { get; set; }
        public int QuantidadeAvesPorCaixa { get; set; }

        public int QuantidadeCaixasVaziasPlanejada { get; set; }
        public int QuantidadeCaixasVazias { get; set; }
        public int QuantidadeAvesPorCaixaRealizada { get; set; }
        public int QuantidadeAvesCarregadas { get; set; }
        public string ChaveNfProdutor { get; set; }
        public string Observacao { get; set; }
    }
}
