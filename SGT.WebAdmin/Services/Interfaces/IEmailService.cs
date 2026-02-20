using Repositorio;

namespace SGT.WebAdmin.Services.Interfaces
{
    /// <summary>
    /// Interface para serviços de envio de email
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envia email com nova senha para o usuário
        /// </summary>
        /// <param name="emailUsuario">Email do usuário</param>
        /// <param name="senha">Nova senha</param>
        /// <param name="usuario">Nome/Login do usuário</param>
        /// <param name="unitOfWork">Unit of Work para acessar o banco</param>
        /// <returns>True se enviado com sucesso, False caso contrário</returns>
        Task<bool> EnviarEmailNovaSenhaAsync(string emailUsuario, string senha, string usuario);

        /// <summary>
        /// Envia email de boas-vindas para usuário importado
        /// </summary>
        /// <param name="emailUsuario">Email do usuário</param>
        /// <param name="nomeUsuario">Nome do usuário</param>
        /// <param name="usuario">Login do usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <param name="unitOfWork">Unit of Work para acessar o banco</param>
        /// <returns>True se enviado com sucesso, False caso contrário</returns>
        Task<bool> EnviarEmailBoasVindasAsync(string emailUsuario, string nomeUsuario, string usuario, string senha);

        /// <summary>
        /// Envia email genérico
        /// </summary>
        /// <param name="emailUsuario">Email do destinatário</param>
        /// <param name="assunto">Assunto do email</param>
        /// <param name="mensagem">Corpo do email</param>
        /// <param name="unitOfWork">Unit of Work para acessar o banco</param>
        /// <returns>True se enviado com sucesso, False caso contrário</returns>
        Task<bool> EnviarEmailAsync(string emailUsuario, string assunto, string mensagem);
    }
}