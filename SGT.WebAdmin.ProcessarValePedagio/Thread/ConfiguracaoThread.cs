namespace SGT.WebAdmin.ProcessarValePedagio.Thread
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
        public ConfiguracaoThread(int id, string nome, System.Collections.Specialized.NameValueCollection cfg)
        {
            Id = id;
            Nome = nome;
            Ativa = bool.Parse(cfg["Ativa"]);
            NumeroTheadsParalelas = int.Parse(cfg["NumeroTheadsParalelas"]);
        }
        


    }
}
