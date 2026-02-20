namespace Dominio.ObjetosDeValor.Embarcador.Notificacao
{
    public class ChatUsuario
    {
        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Login { get; set; }
        public virtual int TotalMensagensNaoLidas { get; set; }
        public virtual int TotalMensagens { get; set; }
    }
}