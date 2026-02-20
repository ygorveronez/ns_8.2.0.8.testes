namespace Dominio.ObjetosDeValor.Embarcador.Notificacao
{
    public class NotificacaoUsuario
    {
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        public virtual Dominio.Entidades.Usuario UsuarioGerouNotificacao { get; set; }

        public virtual int CodigoObjeto { get; set; }

        public virtual string URLPagina { get; set; }

        public virtual string Nota { get; set; }

        public virtual Enumeradores.IconesNotificacao Icone { get; set; }

        public virtual Enumeradores.TipoNotificacao TipoNotificacao { get; set; }
    }
}
