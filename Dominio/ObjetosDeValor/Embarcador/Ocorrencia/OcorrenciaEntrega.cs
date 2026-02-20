namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class OcorrenciaEntrega
    {
        public string DataOcorrencia { get; set; }
        public TipoOcorrencia TipoOcorrencia { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Ocorrencia.PacoteOcorrencia Pacote { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal NotaFiscal { get; set; }
        public int Volumes { get; set; }
        public string Observacao { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
    }
}
