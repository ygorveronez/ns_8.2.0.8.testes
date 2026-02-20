namespace SGT.WebAdmin.ProcessamentoDocumentoTransporte
{
    public class ConfiguracaoThread
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativa { get; set; }
        public int NumeroTheadsParalelas { get; set; }
        public int NumeroMaximoTentativas { get; set; }
        public int MinutosEsperaIntegracoesQueFalharam { get; set; }
        public ConfiguracaoThread(int id, string nome, bool ativa, int numeroTheadsParalelas, int numeroMaximoTentativas, int minutosEsperaIntegracoesQueFalharam)
        {
            Id = id;
            Nome = nome;
            Ativa = ativa;
            NumeroTheadsParalelas = numeroTheadsParalelas;
            NumeroMaximoTentativas = numeroMaximoTentativas;
            MinutosEsperaIntegracoesQueFalharam = minutosEsperaIntegracoesQueFalharam;
        }
    }
}
