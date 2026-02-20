namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class RetornoTipoOcorrencia
    {
        public bool success { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public RetornoTipoOcorrenciaOccurrenceCategory occurrenceCategory { get; set; }
    }

    public class RetornoTipoOcorrenciaOccurrenceCategory
    {
        public string _id { get; set; }
    }
}
