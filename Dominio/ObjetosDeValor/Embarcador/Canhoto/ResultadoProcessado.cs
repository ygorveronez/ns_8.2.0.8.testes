namespace Dominio.ObjetosDeValor.Embarcador.Canhoto
{
    public class ResultadoProcessado
    {
        public SobreposicaoTexto TextOverlay { get; set; }
        public string TextOrientation { get; set; }
        public int FileParseExitCode { get; set; }
        public string ParsedText { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
    }
}
