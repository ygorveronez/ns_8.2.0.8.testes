using MediatR;
using SGT.WebAdmin.Controllers;
using SGT.WebAdmin.Notifications;
using SGT.WebAdmin.Services.Interfaces;

namespace SGT.WebAdmin.Handlers
{
    public class UsuarioImportadoNotificationHandler : INotificationHandler<UsuarioImportadoNotification>
    {
        private readonly ILogger<UsuarioImportadoNotificationHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly Conexao _conexao;

        public UsuarioImportadoNotificationHandler(
            ILogger<UsuarioImportadoNotificationHandler> logger,
            IEmailService emailService,
            Conexao conexao)
        {
            _logger = logger;
            _emailService = emailService;
            _conexao = conexao;
        }

        public async Task Handle(UsuarioImportadoNotification notification, CancellationToken cancellationToken)
        {
            using var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO repositorioConfiguracaoSSO = new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO(unitOfWork);

            try
            {
                //Logs de informação
                _logger.LogInformation($"Processando notificação de usuário importado: {notification.NomeUsuario}");
                _logger.LogInformation("Usuário {NomeUsuario} importado com sucesso. Quantidade: {Quantidade}, Data: {Data}",
                    notification.NomeUsuario,
                    notification.DataImportacao);

                // Enviar email de boas-vindas para usuário importado
                if (!string.IsNullOrWhiteSpace(notification.EmailUsuario) &&
                    !string.IsNullOrWhiteSpace(notification.SenhaTemporaria))
                {

                    //await _emailService.EnviarEmailNovaSenhaAsync(usuario.Email, novaSenha, usuario.Login);

                    await _emailService.EnviarEmailBoasVindasAsync(
                        notification.EmailUsuario,
                        notification.NomeUsuario,
                        notification.Login,
                        notification.SenhaTemporaria);

                    _logger.LogInformation("Email de boas-vindas enviado para {NomeUsuario} ({Email})",
                        notification.NomeUsuario, notification.EmailUsuario);
                }

                _logger.LogInformation("Notificação de importação processada com sucesso para {NomeUsuario}", notification.NomeUsuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar notificação de importação para {NomeUsuario}", notification.NomeUsuario);
                // Note: não relançamos a exceção para não afetar o processo principal
            }
        }
    }
}