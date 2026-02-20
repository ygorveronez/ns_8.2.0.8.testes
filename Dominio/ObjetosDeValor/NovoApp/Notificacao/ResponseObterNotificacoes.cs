using AdminMultisoftware.Dominio.Enumeradores;

namespace Dominio.ObjetosDeValor.NovoApp.Notificacao
{
    public partial class ResponseObterNotificacoes
    {
        public long Codigo { get; set; }
        public string Headings { get; set; }
        public string Contents { get; set; }
        public string Data { get; set; }
        public long DataCriacao { get; set; }
        public MobileHubs Tipo { get; set; }
        public bool Lida { get; set; }
        public string notificationId { get; set; }
    }
   
}
