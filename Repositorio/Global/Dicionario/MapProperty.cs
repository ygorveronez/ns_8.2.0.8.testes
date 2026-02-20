namespace Repositorio.Global.Dicionario
{
    public class MapProperty
    {
        public string DBName { get; set; }
        public string PropoertyName { get; set; }
        public int Index { get; set; }
        public MapProperty IsClass { get; set; }
        public bool IsKey { get; internal set; }
        public bool ScopeIdentity { get; internal set; }
        public int MaxLen { get; set; }
        public int MinLen { get; set; }
        public string MsgError { get; set; }
        public string Value { get; set; }
        public bool Requirido { get; set; }
        public bool NotNull { get; set; }

    }
}
