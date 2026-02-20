using Repositorio;
using SGT.WebAdmin.Controllers;
using SGT.WebAdmin.Services.Interfaces;

namespace SGT.WebAdmin.Services
{
    /// <summary>
    /// Serviço para envio de emails do sistema
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly Conexao _conexao;

        public EmailService(ILogger<EmailService> logger, Conexao conexao)
        {
            _logger = logger;
            _conexao = conexao;
        }

        /// <summary>
        /// Envia email com nova senha para o usuário
        /// </summary>
        public async Task<bool> EnviarEmailNovaSenhaAsync(string emailUsuario, string senha, string usuario)
        {
            try
            {
                var configEmail = await ObterConfiguracaoEmailAsync();
                if (configEmail == null)
                {
                    _logger.LogError("Não há configuração de email ativa para envio");
                    return false;
                }

                string assunto = Localization.Resources.Login.Login.NovaSenhaParaAcesso;

                var embarcador = await ObterNomeEmpresa();

                string mensagemEmail = string.Format(Localization.Resources.Login.Login.MensagemEmail, embarcador, usuario, senha);

                return await EnviarEmailInternoAsync(emailUsuario, assunto, mensagemEmail, configEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email de nova senha para {Usuario}", usuario);
                return false;
            }
        }

        private async Task<string> ObterNomeEmpresa()
        {
            UnitOfWork unitOfWork = null;
            try
            {
                unitOfWork = new UnitOfWork(_conexao.StringConexao);
                var repConfiguracaoCliente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoClienteMultisoftware(unitOfWork);
                var configuracaoCliente = repConfiguracaoCliente.BuscarConfiguracaoPadrao();

                if (configuracaoCliente != null && !string.IsNullOrWhiteSpace(configuracaoCliente.RazaoSocial))
                    return configuracaoCliente.RazaoSocial;

                return "MultiSoftware";
            }
            catch (Exception)
            {
                unitOfWork?.Dispose();
                return "MultiSoftware";
            }
        }

        /// <summary>
        /// Envia email de boas-vindas para usuário importado
        /// </summary>
        public async Task<bool> EnviarEmailBoasVindasAsync(string emailUsuario, string nomeUsuario, string usuario, string senha)
        {
            try
            {
                var configEmail = await ObterConfiguracaoEmailAsync();
                if (configEmail == null)
                {
                    _logger.LogError("Não há configuração de email ativa para envio");
                    return false;
                }

                //TODOS: o melhor é estar em um cache
                var embarcador = await ObterNomeEmpresa();

                string assunto = $"Bem-vindo {nomeUsuario} - Dados de Acesso";

                var mensagemEmail = $@"
                    <h2>Bem-vindo ao {embarcador}!</h2>
                    <p>Olá <strong>{nomeUsuario}</strong>,</p>
                    <p>Sua conta foi criada com sucesso. Seus dados para acesso ao sistema são:</p>
                    <ul>
                        <li><strong>Usuário:</strong> {usuario}</li>
                        <li><strong>Senha:</strong> {senha}</li>
                    </ul>
                    <p>Recomendamos que altere sua senha no primeiro acesso.</p>
                    <br>
                    <p>Atenciosamente,<br>Equipe</p>
                ";

                return await EnviarEmailInternoAsync(emailUsuario, assunto, mensagemEmail, configEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email de boas-vindas para {Usuario}", nomeUsuario);
                return false;
            }
        }

        /// <summary>
        /// Envia email genérico
        /// </summary>
        public async Task<bool> EnviarEmailAsync(string emailUsuario, string assunto, string mensagem)
        {
            try
            {
                var configEmail = await ObterConfiguracaoEmailAsync();
                if (configEmail == null)
                {
                    _logger.LogError("Não há configuração de email ativa para envio");
                    return false;
                }

                return await EnviarEmailInternoAsync(emailUsuario, assunto, mensagem, configEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email para {Email}", emailUsuario);
                return false;
            }
        }

        #region Métodos Privados

        /// <summary>
        /// Obtém a configuração de email ativa do sistema
        /// </summary>
        private async Task<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> ObterConfiguracaoEmailAsync()
        {
            UnitOfWork unitOfWork = null;
            try
            {
                unitOfWork = new UnitOfWork(_conexao.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);
                var repositorio = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                return await Task.FromResult(repositorio.BuscarEmailEnviaDocumentoAtivo(0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar configuração de email");
                return null;
            }
            finally
            {
                unitOfWork?.Dispose();
            }
        }

        /// <summary>
        /// Obtém a razão social do cliente
        /// </summary>
        //private string ObterRazaoSocialCliente(UnitOfWork unitOfWork)
        //{
        //    try
        //    {
        //        // Aqui você pode implementar a lógica para buscar a razão social
        //        // Por enquanto, retorna um valor padrão
        //        return "MultiSoftware"; // TODO: Buscar do contexto/configuração
        //    }
        //    catch
        //    {
        //        return "Sistema";
        //    }
        //}

        /// <summary>
        /// Método interno para envio de email usando a infraestrutura existente
        /// </summary>
        private async Task<bool> EnviarEmailInternoAsync(string emailUsuario, string assunto, string mensagem,
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmail)
        {
            UnitOfWork unitOfWork = null;
            try
            {
                unitOfWork = new UnitOfWork(_conexao.StringConexao);
                string mensagemErro = string.Empty;

                var emails = ProcessarEmails(emailUsuario);
                if (emails.Count == 0)
                {
                    _logger.LogWarning("Nenhum email válido fornecido para envio");
                    return false;
                }

                _logger.LogInformation("Enviando email para {Count} destinatários. Assunto: {Assunto}",
                    emails.Count, assunto);

                bool sucesso = await Task.FromResult(
                    Servicos.Email.EnviarEmail(
                        configEmail.Email,
                        configEmail.Usuario,
                        configEmail.Senha,
                        null,
                        emails.ToArray(),
                        null,
                        assunto,
                        mensagem,
                        configEmail.Smtp,
                        out mensagemErro,
                        configEmail.DisplayEmail,
                        null,
                        "",
                        configEmail.RequerAutenticacaoSmtp,
                        "",
                        configEmail.PortaSmtp,
                        unitOfWork
                    )
                );

                if (!sucesso)
                {
                    _logger.LogError("Falha ao enviar email: {Erro}", mensagemErro);
                    return false;
                }

                _logger.LogInformation("Email enviado com sucesso para {Emails}", string.Join(", ", emails));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno no envio de email");
                return false;
            }
            finally
            {
                unitOfWork?.Dispose();
            }
        }

        /// <summary>
        /// Processa e valida lista de emails
        /// </summary>
        private List<string> ProcessarEmails(string emailUsuario)
        {
            var emails = new List<string>();

            if (!string.IsNullOrWhiteSpace(emailUsuario))
            {
                emails.AddRange(emailUsuario.Split(';', ',').Select(e => e.Trim()).Where(e => !string.IsNullOrEmpty(e)));
            }

            return emails.Distinct().ToList();
        }

        #endregion
    }
}