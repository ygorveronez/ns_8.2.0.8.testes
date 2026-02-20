namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao
{
    public sealed class NotificacaoDados
    {
        public string Assunto { get; set; }

        public Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        public string Mensagem { get; set; }

        public Enumeradores.TipoNotificacaoMobile Tipo { get; set; }

        public Entidades.Usuario Usuario { get; set; }
    }
}
