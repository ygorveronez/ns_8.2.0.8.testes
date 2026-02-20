namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class RetornoChecklist
    {
        public bool success { get; set; }
        public int code { get; set; }
        public int requisicao_id { get; set; }
        public string message { get; set; }
        public string error { get; set; }
        public string details { get; set; }
        public ChecklistFlow checklistflow { get; set; }
        
    }
}
