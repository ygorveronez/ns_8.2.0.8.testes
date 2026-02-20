using MediatR;

namespace SGT.WebAdmin.Notifications
{
    public class UsuarioImportadoNotification : INotification
    {
        public string NomeUsuario { get; set; }
        public string Login { get; set; }
        public string EmailUsuario { get; set; }
        public string SenhaTemporaria { get; set; }        
        public DateTime DataImportacao { get; set; }

        public UsuarioImportadoNotification(string nomeUsuario, string emailUsuario, string senhaTemporaria, string login)
        {
            NomeUsuario = nomeUsuario;
            EmailUsuario = emailUsuario;
            SenhaTemporaria = senhaTemporaria;
            Login = login;
            
            DataImportacao = DateTime.Now;
        }
    }
}