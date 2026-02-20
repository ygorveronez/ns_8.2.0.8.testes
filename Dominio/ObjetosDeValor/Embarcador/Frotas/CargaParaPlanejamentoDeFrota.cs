namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public class CargaParaPlanejamentoDeFrota
    {
        public int CargaCodigo { get; set; }
        public int VeiculoCodigo { get; set; }
        public string VeiculoPlaca { get; set; }
        public int ModeloVeicularCodigo { get; set; }
        //public string ModeloVeicularDescricao { get; set; }
        public int TipoOperacaoCodigo { get; set; }
        //public string TipoOperacaoDescricao { get; set; }
        public int FilialCodigo { get; set; }
        //public string FilialDescricao { get; set; }
        public int TransportadorCodigo { get; set; }
        public string EmailTransportador { get; set; }
        // public string TransportadorDescricao { get; set; }
    }
}
