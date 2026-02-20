using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Canhoto
{
    public class RetornoOcr
    {
        public List<ResultadoProcessado> ParsedResults { get; set; }
        public int OCRExitCode { get; set; }
        public bool IsErroredOnProcessing { get; set; }
        public string ProcessingTimeInMilliseconds { get; set; }
        public string SearchablePDFURL { get; set; }
        public string RetornoParse { get; set; }
    }
}
