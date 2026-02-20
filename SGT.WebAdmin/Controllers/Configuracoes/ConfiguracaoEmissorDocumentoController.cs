using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ConfiguracaoEmissorDocumento")]
    public class ConfiguracaoEmissorDocumentoController : BaseController
    {
        #region Construtores

        public ConfiguracaoEmissorDocumentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (!UsuarioPossuiPermissaoAlterarConfiguracoesSistema())
                    return new JsonpResult(true, false, "Seu usuário não tem permissão de alterar essas configurações.");

                TipoEmissorDocumento tipoEmissorDocumentoCTe = Request.GetEnumParam<TipoEmissorDocumento>("TipoEmissorDocumentoCTe");
                TipoEmissorDocumento tipoEmissorDocumentoMDFe = Request.GetEnumParam<TipoEmissorDocumento>("TipoEmissorDocumentoMDFe");
                string responsavelTecnicoCNPJ = Request.GetStringParam("ResponsavelTecnicoCNPJ");
                string responsavelTecnicoNomeContato = Request.GetStringParam("ResponsavelTecnicoNomeContato");
                string responsavelTecnicoEmail = Request.GetStringParam("ResponsavelTecnicoEmail");
                string responsavelTecnicoTelefone = Request.GetStringParam("ResponsavelTecnicoTelefone");
                string nstechUrlAPICte = Request.GetStringParam("NSTechUrlAPICte");
                string nstechUrlAPIMDFe = Request.GetStringParam("NSTechUrlAPIMDFe");
                string nstechUrlAPIWebHook = Request.GetStringParam("NSTechUrlAPIWebHook");
                string nstechUrlAPICertificado = Request.GetStringParam("NSTechUrlAPICertificado");
                string nstechUrlAPILogo = Request.GetStringParam("NSTechUrlAPILogo");
                string nstechTokenAPIKey = Request.GetStringParam("NSTechTokenAPIKey");
                string nstechUrlWebhook = Request.GetStringParam("NSTechUrlWebhook");

                if (tipoEmissorDocumentoCTe != TipoEmissorDocumento.Integrador || tipoEmissorDocumentoMDFe != TipoEmissorDocumento.Integrador)
                {
                    if (string.IsNullOrEmpty(responsavelTecnicoCNPJ))
                        throw new ControllerException("Processo abortado! CNPJ do responsável tecnico não informado.");

                    if (string.IsNullOrEmpty(responsavelTecnicoNomeContato))
                        throw new ControllerException("Processo abortado! Nome de contato do responsável tecnico não informado.");

                    if (string.IsNullOrEmpty(responsavelTecnicoEmail))
                        throw new ControllerException("Processo abortado! E-mail do responsável tecnico não informado.");

                    if (string.IsNullOrEmpty(responsavelTecnicoTelefone))
                        throw new ControllerException("Processo abortado! Telefone do responsável tecnico não informado.");
                }

                if (tipoEmissorDocumentoCTe == TipoEmissorDocumento.NSTech || tipoEmissorDocumentoMDFe == TipoEmissorDocumento.NSTech)
                {
                    if (string.IsNullOrEmpty(nstechUrlAPICte))
                        throw new ControllerException("Processo abortado! Url API CT-e não informada.");

                    if (string.IsNullOrEmpty(nstechUrlAPIMDFe))
                        throw new ControllerException("Processo abortado! Url API MDF-e não informada.");

                    if (string.IsNullOrEmpty(nstechUrlAPIWebHook))
                        throw new ControllerException("Processo abortado! Url API WebHook não informada.");

                    if (string.IsNullOrEmpty(nstechUrlAPICertificado))
                        throw new ControllerException("Processo abortado! Url API Certificado não informada.");

                    if (string.IsNullOrEmpty(nstechTokenAPIKey))
                        throw new ControllerException("Processo abortado! Token API Key não informada.");

                    if (string.IsNullOrEmpty(nstechUrlWebhook))
                        throw new ControllerException("Processo abortado! Url Webhook não informada.");
                }

                Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento repConfiguracaoIntegracaoEmissorDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento config = await repConfiguracaoIntegracaoEmissorDocumento.BuscarConfiguracaoPadraoAsync();

                bool removerInscricaoWebhookNStech = false;
                // Ao trocar o emissor, caso o emissor anterior seja NSTech é executado o comando para remover a inscrição do webhook
                //if (config.TipoEmissorDocumentoCTe == TipoEmissorDocumento.NSTech && config.TipoEmissorDocumentoCTe != tipoEmissorDocumentoCTe)
                //removerInscricaoWebhookNStech = true;

                config.Initialize();
                await unitOfWork.StartAsync(cancellationToken);

                config.TipoEmissorDocumentoCTe = tipoEmissorDocumentoCTe;
                config.TipoEmissorDocumentoMDFe = tipoEmissorDocumentoMDFe;
                config.ResponsavelTecnicoCNPJ = responsavelTecnicoCNPJ;
                config.ResponsavelTecnicoNomeContato = responsavelTecnicoNomeContato;
                config.ResponsavelTecnicoEmail = responsavelTecnicoEmail;
                config.ResponsavelTecnicoTelefone = responsavelTecnicoTelefone;
                config.NSTechUrlAPICte = nstechUrlAPICte;
                config.NSTechUrlAPIMDFe = nstechUrlAPIMDFe;
                config.NSTechUrlAPIWebHook = nstechUrlAPIWebHook;
                config.NSTechUrlAPICertificado = nstechUrlAPICertificado;
                config.NSTechUrlAPILogo = nstechUrlAPILogo;
                config.NSTechTokenAPIKey = nstechTokenAPIKey;
                config.NSTechUrlWebhook = nstechUrlWebhook;

                if (config.TipoEmissorDocumentoCTe == TipoEmissorDocumento.NSTech || config.TipoEmissorDocumentoMDFe == TipoEmissorDocumento.NSTech)
                {
                    if (string.IsNullOrEmpty(config.NSTechExternalId))
                    {
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            config.NSTechExternalId = $"K{this.Cliente.Codigo}-{this.ClienteAcesso.Codigo}";
                        else
                            config.NSTechExternalId = $"E{this.Cliente.Codigo}-{this.ClienteAcesso.Codigo}";
                    }

                    if (config.NSTechIntegradora == null)
                    {
                        Repositorio.WebService.Integradora repIntegradora = new Repositorio.WebService.Integradora(unitOfWork);
                        Dominio.Entidades.WebService.Integradora integradora = new Dominio.Entidades.WebService.Integradora();
                        integradora.Descricao = "NSTechEmissorCTe";
                        integradora.Token = Guid.NewGuid().ToString();
                        integradora.TipoAutenticacao = TipoAutenticacao.Token;
                        integradora.TodosWebServicesLiberados = true;
                        integradora.Ativo = true;
                        await repIntegradora.InserirAsync(integradora);
                        config.NSTechIntegradora = integradora;
                    }
                }

                await repConfiguracaoIntegracaoEmissorDocumento.AtualizarAsync(config, Auditado);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                // Atualizar classe de integração do emissor de documento
                Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.ConfigureApplicationEmissorDocumento(unitOfWork);

                if (tipoEmissorDocumentoCTe == TipoEmissorDocumento.NSTech || tipoEmissorDocumentoMDFe == TipoEmissorDocumento.NSTech)
                {
                    string mensagemErro = string.Empty;
                    string subscribeID = string.Empty;

                    Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech integracaoNStech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(config);
                    if (!integracaoNStech.IncluirAtualizarInscricaoWebhook(out mensagemErro, out subscribeID))
                        throw new ControllerException(mensagemErro);

                    if (!string.IsNullOrEmpty(subscribeID))
                    {
                        config.NSTechSubscribeId = subscribeID;
                        await repConfiguracaoIntegracaoEmissorDocumento.AtualizarAsync(config);
                    }
                }
                else if (removerInscricaoWebhookNStech)
                {
                    string mensagemErro = string.Empty;
                    Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech integracaoNStech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(config);

                    if (!integracaoNStech.RemoverInscricaoWebhook(out mensagemErro))
                        throw new ControllerException(mensagemErro);

                    config.NSTechSubscribeId = null;
                    await repConfiguracaoIntegracaoEmissorDocumento.AtualizarAsync(config);
                }

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a configuração.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento repConfiguracaoIntegracaoEmissorDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento config = await repConfiguracaoIntegracaoEmissorDocumento.BuscarConfiguracaoPadraoAsync();

                return new JsonpResult(new
                {
                    config.TipoEmissorDocumentoCTe,
                    TipoEmissorDocumentoMDFe = config.TipoEmissorDocumentoMDFe,
                    config.ResponsavelTecnicoCNPJ,
                    config.ResponsavelTecnicoNomeContato,
                    config.ResponsavelTecnicoEmail,
                    config.ResponsavelTecnicoTelefone,
                    config.NSTechUrlAPICte,
                    NSTechUrlAPIMDFe = config.NSTechUrlAPIMDFe,
                    config.NSTechUrlAPIWebHook,
                    config.NSTechUrlAPICertificado,
                    config.NSTechUrlAPILogo,
                    config.NSTechTokenAPIKey,
                    config.NSTechUrlWebhook,
                    NSTechTokenWebhook = config.NSTechIntegradora?.Token,
                    config.NSTechSubscribeId
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os as configurações.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarCertificadosDigitaisValido(CancellationToken cancellationToken)
        {
            Servicos.Log.TratarErro($"EnviarCertificadosDigitaisValido iniciado", "CertificadoNstech");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                List<Dominio.Entidades.Empresa> empresas = await repEmpresa.BuscarAtivasAsync();

                Servicos.Log.TratarErro($"EnviarCertificadosDigitaisValido executando para {empresas.Count} empresas", "CertificadoNstech");

                bool completadoComSucesso = true;
                foreach (Dominio.Entidades.Empresa empresaCert in empresas)
                {
                    DateTime dataAtual = DateTime.Today;
                    if (dataAtual >= empresaCert.DataInicialCertificado && dataAtual <= empresaCert.DataFinalCertificado)
                    {
                        string mensagemErro = string.Empty;
                        Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech integracaoNStech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
                        if (!integracaoNStech.IncluirAtualizarCertificado(out mensagemErro, empresaCert, unitOfWork))
                        {
                            Servicos.Log.TratarErro($"Ocorreu um erro ao enviar o certificado para o emissor NStech: {mensagemErro} {empresaCert.CNPJ}", "CertificadoNstech");
                            completadoComSucesso = false;
                        }
                        else
                        {
                            Servicos.Log.TratarErro($"O envio do certificado foi realizado com sucesso {empresaCert.CNPJ}", "CertificadoNstech");
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro($"Certificado vencido (informaçao validada na base de dados) {empresaCert.CNPJ}", "CertificadoNstech");
                    }
                }

                if (!completadoComSucesso)
                    return new JsonpResult(false, false, "O processo de importação apresentou falhas em uma ou mais empresas. Consulte os logs 'CertificadoNstech' no servidor para mais informações.");

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar os certificados digitais válidos.");
            }
        }
        #endregion
    }
}