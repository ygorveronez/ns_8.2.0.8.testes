namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public class RetornoImportacaoNotaFiscal
    {
        public bool Sucesso { get; set; }
        public int TotalPedidos { get; set; }
        public int TotalNotas { get; set; }
        public string Mensagem { get; set; }
        public bool TerminouProcessar { get; set; }
    }
}
