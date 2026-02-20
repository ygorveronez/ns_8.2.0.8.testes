namespace Dominio.ObjetosDeValor.Embarcador.Configuracoes
{
    public sealed class FiltroPesquisaNotificacaoMotoristaSMS
    {
        public string Descricao { get; set; }
        public bool? Ativo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacaoMotoristaSMS? TipoNotificacaoSMS { get; set; }
        public bool? NotificacaoSuperApp { get; set; }
    }
}
