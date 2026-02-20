namespace SGT.WebAdmin.ProcessarDocumentosService.Thread
{
    public class ConfiguracaoThread
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativa { get; set; }
        public int NumeroTheadsParalelas { get; set; }
        public bool UtilizarInsertEmMassa { get; set; }
        public ConfiguracaoThread(int id, string nome, bool ativa, int numeroTheadsParalelas, bool utilizarInsertEmMassa)
        {
            Id = id;
            Nome = nome;
            Ativa = ativa;
            NumeroTheadsParalelas = numeroTheadsParalelas;
            UtilizarInsertEmMassa = utilizarInsertEmMassa;
        }
    }
}
