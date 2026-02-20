namespace Dominio.ObjetosDeValor.Embarcador.Configuracoes
{
    public class ConfiguracaoLog
    {
        public bool? UtilizaLogArquivo { get; set; }
        public bool UtilizaLogWeb { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProtocoloLogWeb ProtocoloLogWeb { get; set; }
        public string Url { get; set; }
        public int Porta { get; set; }
        public bool? GravarLogInfo { get; set; }
        public bool? GravarLogError { get; set; }
        public bool? GravarLogAdvertencia { get; set; }
        public bool? GravarLogDebug { get; set; }
        public bool? UtilizaGraylog { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProtocoloLogWeb ProtocoloLogGraylog { get; set; }
        public string UrlGraylog { get; set; }
        public int PortaGraylog { get; set; }
    }
}
