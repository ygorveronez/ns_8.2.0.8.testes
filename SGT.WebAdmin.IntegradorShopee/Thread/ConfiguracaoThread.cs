namespace SGT.WebAdmin.IntegradorShopee.Thread
{
    public class ConfiguracaoThread
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativa { get; set; }
        public int NucleosEmParalelo { get; set; }
        public bool LockDeletes { get; set; }
        public int SleepFilaVazia { get; set; }
        public int SleepPasso2 { get; set; }
        public ConfiguracaoThread(int id, string nome, bool ativa, int _NucleosEmParalelo, bool lockDeletes, int sleepFilaVazia, int sleepPasso2)
        {
            Id = id;
            Nome = nome;
            Ativa = ativa;
            NucleosEmParalelo = _NucleosEmParalelo;
            LockDeletes = lockDeletes;
            SleepFilaVazia = sleepFilaVazia;
            SleepPasso2 = sleepPasso2;
        }
    }
}
