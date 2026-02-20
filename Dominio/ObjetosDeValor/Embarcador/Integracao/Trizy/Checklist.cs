namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class Checklist
    {
        public string flow { get; set; }
        public TextoInternacionalizado label { get; set; }
        public InformacaoExterna externalInfo { get; set; }
        public ChecklistStep[] steps { get; set; }
    }
}

