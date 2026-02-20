namespace Dominio.ObjetosDeValor.Embarcador.ProcessarCalculoDeFreteService
{
    public class ConfiguracaoThread
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativa { get; set; }
        public int NumeroTheadsParalelas { get; set; }
        public ConfiguracaoThread(int id, string nome, bool ativa, int numeroTheadsParalelas)
        {
            Id = id;
            Nome = nome;
            Ativa = ativa;
            NumeroTheadsParalelas = numeroTheadsParalelas;
        }
    }
}
