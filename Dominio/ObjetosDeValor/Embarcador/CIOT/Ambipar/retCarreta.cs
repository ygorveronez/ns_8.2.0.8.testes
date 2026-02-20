namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public class retCarreta
    {
        public int? ID { get; set; }
        public int? TransportadorID { get; set; }
        public int? CarretaTipoID { get; set; }
        public string Placa { get; set; }
        public int? QuantidadeEixos { get; set; }
        public string AnoFabricacao { get; set; }
        public string CodigoRenavam { get; set; }
        public string NumeroChassis { get; set; }
        public string CorPredominante { get; set; }
        public string CidadeDaPlaca { get; set; }
        public string EstadoDaPlaca { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Tara { get; set; }
        public bool FrotaPropria { get; set; }
        public string DataInclusao { get; set; }
        public string DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}