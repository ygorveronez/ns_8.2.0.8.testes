using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Servicos.Embarcador.Integracao.Gmail;


namespace SGT.WebAdmin.Controllers.Email
{
    [CustomAuthorize("Email/ConfigEmailDocTransporte")]
    public class ConfigEmailDocTransporteController : BaseController
    {
		#region Construtores

		public ConfigEmailDocTransporteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string email = Request.Params("Email");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("DT_RowColor", false);
                grid.AdicionarCabecalho(Localization.Resources.Email.ConfigEmailDocTransporte.Email, "Email", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("SMTP", "Smtp", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("POP 3", "Pop3", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Email.ConfigEmailDocTransporte.EmailAtivo, "EmailAtivo", 15, Models.Grid.Align.center, false);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                List<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> listaConfigEmailDocTransporte = repConfigEmailDocTransporte.Consultar(email, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, null, codigoEmpresa);
                grid.setarQuantidadeTotal(repConfigEmailDocTransporte.ContarConsulta(email, codigoEmpresa));
                var lista = from p in listaConfigEmailDocTransporte
                            select new
                            {
                                p.Codigo,
                                p.Email,
                                p.Smtp,
                                p.Pop3,
                                EmailAtivo = p.EmailAtivo ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                                DT_RowColor = p.EmailAtivo ? "#dff0d8" : ""
                            };
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmailDocTransporte = new Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte();

                var emailEmpresa = string.Empty;
                var razaoSocialEmpresa = string.Empty;
                bool emailAtivo = bool.Parse(Request.Params("EmailAtivo"));

                configEmailDocTransporte.Email = Request.Params("Email");
                configEmailDocTransporte.DisplayEmail = Request.Params("DisplayEmail");
                configEmailDocTransporte.Smtp = Request.Params("Smtp");
                configEmailDocTransporte.Senha = Request.Params("Senha");
                configEmailDocTransporte.MensagemRodape = Request.Params("MensagemRodape");

                configEmailDocTransporte.Pop3Ativo = bool.Parse(Request.Params("Pop3Ativo"));
                configEmailDocTransporte.SmtpAtivo = bool.Parse(Request.Params("SmtpAtivo"));
                configEmailDocTransporte.LerDocumentos = bool.Parse(Request.Params("LerDocumentos"));
                configEmailDocTransporte.EnviarDocumentos = bool.Parse(Request.Params("EnviarDocumentos"));

                configEmailDocTransporte.TipoConexaoEmail = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConexaoEmail>("TipoConexaoEmail");
                configEmailDocTransporte.ClientId = Request.GetStringParam("ClientId");
                configEmailDocTransporte.ClientSecret = Request.GetStringParam("ClientSecret");
                configEmailDocTransporte.TenantId = Request.GetStringParam("TenantId");
                configEmailDocTransporte.RedirectUri = Request.GetStringParam("RedirectUri");
                configEmailDocTransporte.ServidorEmail = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServidorEmail>("ServidorEmail");
                configEmailDocTransporte.UrlEnvio = Request.GetStringParam("UrlEnvio");
                configEmailDocTransporte.UrlAutenticacao = Request.GetStringParam("UrlAutenticacao");
                configEmailDocTransporte.ApiEnvioEmail = Request.GetBoolParam("ApiEnvioEmail");

                if (!string.IsNullOrEmpty(Request.Params("PortaSmtp")))
                {
                    configEmailDocTransporte.PortaSmtp = int.Parse(Request.Params("PortaSmtp"));
                }
                configEmailDocTransporte.RequerAutenticacaoSmtp = bool.Parse(Request.Params("RequerAutenticacaoSmtp"));
                configEmailDocTransporte.Pop3 = Request.Params("Pop3");

                if (!string.IsNullOrEmpty(Request.Params("PortaPop3")))
                {
                    configEmailDocTransporte.PortaPop3 = int.Parse(Request.Params("PortaPop3"));
                }

                configEmailDocTransporte.RequerAutenticacaoPop3 = bool.Parse(Request.Params("RequerAutenticacaoPop3"));

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    configEmailDocTransporte.Empresa = this.Usuario.Empresa;
                    bool emailDuplicado = repConfigEmailDocTransporte.ValidaConfiguracaoEmailDuplicado(this.Usuario.Empresa.Codigo);
                    if (emailDuplicado && emailAtivo)
                        return new JsonpResult(false, Localization.Resources.Email.ConfigEmailDocTransporte.JaExisteConfiguracaoEmailAtivaAdicionar);
                    emailEmpresa = this.Usuario.Empresa.Email;
                    razaoSocialEmpresa = this.Usuario.Empresa.RazaoSocial;
                    configEmailDocTransporte.EnviarDocumentos = true;
                }

                string retornoSMTP = "Sucesso";
                if (emailAtivo && configEmailDocTransporte.SmtpAtivo && configEmailDocTransporte.TipoConexaoEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConexaoEmail.Padrao) 
                {
                    Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                    retornoSMTP = serEmail.TestarConexaoSMTP(configEmailDocTransporte.Email, configEmailDocTransporte.Email, configEmailDocTransporte.Senha, configEmailDocTransporte.Smtp, emailEmpresa, TipoServicoMultisoftware, configEmailDocTransporte.RequerAutenticacaoSmtp, configEmailDocTransporte.PortaSmtp, razaoSocialEmpresa);
                }

                if (retornoSMTP == "Sucesso" && emailAtivo && configEmailDocTransporte.SmtpAtivo && configEmailDocTransporte.TipoConexaoEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConexaoEmail.Exchange)
                {// para Exchange atualmente temos apenas a leitura abilitada
                    Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                    serEmail.ReceberEmail(configEmailDocTransporte, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor, "", "", "", true, 0, unitOfWork, true);
                }

                if (retornoSMTP == "Sucesso")
                {
                    string retornoPOP3 = "Sucesso";
                    if (emailAtivo && configEmailDocTransporte.Pop3Ativo && configEmailDocTransporte.TipoConexaoEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConexaoEmail.Padrao)
                    {
                        Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                        retornoPOP3 = serEmail.textarConexaoPOP3(configEmailDocTransporte.Email, configEmailDocTransporte.Senha, configEmailDocTransporte.Pop3, configEmailDocTransporte.RequerAutenticacaoPop3, configEmailDocTransporte.PortaPop3);
                    }

                    if (retornoPOP3 == "Sucesso")
                    {
                        if (emailAtivo && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        {
                            if (configEmailDocTransporte.EnviarDocumentos)
                            {
                                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte emailAtual = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(codigoEmpresa);
                                if (emailAtual != null && emailAtual.Codigo != configEmailDocTransporte.Codigo)
                                {
                                    emailAtual.EmailAtivo = false;
                                    repConfigEmailDocTransporte.Atualizar(emailAtual);
                                }
                            }
                        }
                        configEmailDocTransporte.EmailAtivo = emailAtivo;
                        repConfigEmailDocTransporte.Inserir(configEmailDocTransporte, Auditado);
                        unitOfWork.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(retornoPOP3) && retornoPOP3.Length > 300)
                            retornoPOP3 = retornoPOP3.Substring(0, 300);

                        return new JsonpResult(false, true, retornoPOP3);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(retornoSMTP) && retornoSMTP.Length > 300)
                        retornoSMTP = retornoSMTP.Substring(0, 300);

                    return new JsonpResult(false, true, retornoSMTP);
                }
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
        }
        public async Task<IActionResult> EnviarArquivo(int codigo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmailDocTransporte = repConfigEmailDocTransporte.BuscarPorCodigo(codigo, true);

            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files == null || files.Count == 0)
                    return new JsonpResult(false, "Nenhum arquivo enviado.");

                var file = files[0];
                string base64 = TokenResponseFileConverter.EncodeFileToBase64(file);

                configEmailDocTransporte.caminhoTokenResposta = base64;
                repConfigEmailDocTransporte.Atualizar(configEmailDocTransporte, Auditado);

                return new JsonpResult(true, "Arquivo salvo com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o arquivo. " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmailDocTransporte = repConfigEmailDocTransporte.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                var emailEmpresa = string.Empty;
                var razaoSocialEmpresa = string.Empty;
                bool emailAtivo = bool.Parse(Request.Params("EmailAtivo"));

                configEmailDocTransporte.Email = Request.Params("Email");
                configEmailDocTransporte.Smtp = Request.Params("Smtp");
                configEmailDocTransporte.Senha = Request.Params("Senha");
                configEmailDocTransporte.MensagemRodape = Request.Params("MensagemRodape");
                configEmailDocTransporte.DisplayEmail = Request.Params("DisplayEmail");

                configEmailDocTransporte.Pop3Ativo = bool.Parse(Request.Params("Pop3Ativo"));
                configEmailDocTransporte.SmtpAtivo = bool.Parse(Request.Params("SmtpAtivo"));
                configEmailDocTransporte.LerDocumentos = bool.Parse(Request.Params("LerDocumentos"));
                configEmailDocTransporte.EnviarDocumentos = bool.Parse(Request.Params("EnviarDocumentos"));

                configEmailDocTransporte.TipoConexaoEmail = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConexaoEmail>("TipoConexaoEmail");
                configEmailDocTransporte.ClientId = Request.GetStringParam("ClientId");
                configEmailDocTransporte.ClientSecret = Request.GetStringParam("ClientSecret");
                configEmailDocTransporte.TenantId = Request.GetStringParam("TenantId");
                configEmailDocTransporte.RedirectUri = Request.GetStringParam("RedirectUri");
                configEmailDocTransporte.ServidorEmail = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServidorEmail>("ServidorEmail");
                configEmailDocTransporte.UrlEnvio = Request.GetStringParam("UrlEnvio");
                configEmailDocTransporte.UrlAutenticacao = Request.GetStringParam("UrlAutenticacao");
                configEmailDocTransporte.ApiEnvioEmail = Request.GetBoolParam("ApiEnvioEmail");
                configEmailDocTransporte.ClientId = Request.GetStringParam("ClientId");
                configEmailDocTransporte.ClientSecret = Request.GetStringParam("ClientSecret");

                if (!string.IsNullOrEmpty(Request.Params("PortaSmtp")))
                {
                    configEmailDocTransporte.PortaSmtp = int.Parse(Request.Params("PortaSmtp"));
                }
                configEmailDocTransporte.RequerAutenticacaoSmtp = bool.Parse(Request.Params("RequerAutenticacaoSmtp"));

                configEmailDocTransporte.Pop3 = Request.Params("Pop3");
                if (!string.IsNullOrEmpty(Request.Params("PortaPop3")))
                {
                    configEmailDocTransporte.PortaPop3 = int.Parse(Request.Params("PortaPop3"));
                }
                configEmailDocTransporte.RequerAutenticacaoPop3 = bool.Parse(Request.Params("RequerAutenticacaoPop3"));

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    bool emailDuplicado = repConfigEmailDocTransporte.ValidaConfiguracaoEmailDuplicado(this.Usuario.Empresa.Codigo, configEmailDocTransporte.Codigo);
                    if (emailDuplicado && emailAtivo)
                        return new JsonpResult(false, Localization.Resources.Email.ConfigEmailDocTransporte.JaExisteConfiguracaoEmailAtivaAtualizar);
                    emailEmpresa = this.Usuario.Empresa.Email;
                    razaoSocialEmpresa = this.Usuario.Empresa.RazaoSocial;
                    configEmailDocTransporte.EnviarDocumentos = true;
                }

                string retornoSMTP = "Sucesso";
                if (emailAtivo && configEmailDocTransporte.SmtpAtivo && configEmailDocTransporte.TipoConexaoEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConexaoEmail.Padrao)
                {
                    Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                    retornoSMTP = serEmail.TestarConexaoSMTP(configEmailDocTransporte.Email, configEmailDocTransporte.Email, configEmailDocTransporte.Senha, configEmailDocTransporte.Smtp, emailEmpresa, TipoServicoMultisoftware, configEmailDocTransporte.RequerAutenticacaoSmtp, configEmailDocTransporte.PortaSmtp, razaoSocialEmpresa);
                }

                if (retornoSMTP == "Sucesso" && emailAtivo && configEmailDocTransporte.SmtpAtivo && configEmailDocTransporte.TipoConexaoEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConexaoEmail.Exchange)
                {// para Exchange atualmente temos apenas a leitura abilitada
                    Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                    serEmail.ReceberEmail(configEmailDocTransporte, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor, "", "", "", true, 0, unitOfWork, true);
                }

                if (retornoSMTP == "Sucesso")
                {

                    string retornoPOP3 = "Sucesso";
                    if (emailAtivo && configEmailDocTransporte.Pop3Ativo && configEmailDocTransporte.TipoConexaoEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConexaoEmail.Padrao)
                    {
                        Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                        retornoPOP3 = serEmail.textarConexaoPOP3(configEmailDocTransporte.Email, configEmailDocTransporte.Senha, configEmailDocTransporte.Pop3, configEmailDocTransporte.RequerAutenticacaoPop3, configEmailDocTransporte.PortaPop3);
                    }

                    if (retornoPOP3 == "Sucesso")
                    {
                        if (emailAtivo && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        {
                            if (configEmailDocTransporte.EnviarDocumentos)
                            {
                                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte emailAtual = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(codigoEmpresa);
                                if (emailAtual != null && emailAtual.Codigo != configEmailDocTransporte.Codigo)
                                {
                                    emailAtual.EmailAtivo = false;
                                    repConfigEmailDocTransporte.Atualizar(emailAtual);
                                }
                            }
                        }
                        configEmailDocTransporte.EmailAtivo = emailAtivo;
                        repConfigEmailDocTransporte.Atualizar(configEmailDocTransporte, Auditado);
                        unitOfWork.CommitChanges();
                        return new JsonpResult(true);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(retornoPOP3) && retornoPOP3.Length > 300)
                            retornoPOP3 = retornoPOP3.Substring(0, 300);

                        return new JsonpResult(false, true, retornoPOP3);
                    }
                }
                else
                {
                    unitOfWork.Rollback();

                    if (!string.IsNullOrEmpty(retornoSMTP) && retornoSMTP.Length > 300)
                        retornoSMTP = retornoSMTP.Substring(0, 300);

                    return new JsonpResult(false, true, retornoSMTP);
                }
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmailDocTransporte = repConfigEmailDocTransporte.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte emailAtivo = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(codigoEmpresa);

                if (!configEmailDocTransporte.EnviarDocumentos)
                    emailAtivo = null;

                var dynConfigEmailDocTransporte = new
                {
                    configEmailDocTransporte.Codigo,
                    Senha = String.Empty,
                    configEmailDocTransporte.Email,
                    configEmailDocTransporte.DisplayEmail,
                    configEmailDocTransporte.Smtp,
                    configEmailDocTransporte.PortaSmtp,
                    configEmailDocTransporte.RequerAutenticacaoSmtp,
                    configEmailDocTransporte.Pop3,
                    configEmailDocTransporte.PortaPop3,
                    configEmailDocTransporte.RequerAutenticacaoPop3,
                    configEmailDocTransporte.EmailAtivo,
                    configEmailDocTransporte.MensagemRodape,
                    configEmailDocTransporte.Pop3Ativo,
                    configEmailDocTransporte.SmtpAtivo,
                    configEmailDocTransporte.LerDocumentos,
                    configEmailDocTransporte.EnviarDocumentos,
                    EmailJaAtivo = emailAtivo != null && configEmailDocTransporte.Codigo != emailAtivo.Codigo ? true : false,
                    configEmailDocTransporte.TipoConexaoEmail,
                    configEmailDocTransporte.ClientId,
                    configEmailDocTransporte.ClientSecret,
                    configEmailDocTransporte.TenantId,
                    configEmailDocTransporte.RedirectUri,
                    configEmailDocTransporte.ApiEnvioEmail,
                    configEmailDocTransporte.UrlAutenticacao,
                    configEmailDocTransporte.caminhoTokenResposta,
                    configEmailDocTransporte.UrlEnvio,
                    configEmailDocTransporte.ServidorEmail,               
                };
                return new JsonpResult(dynConfigEmailDocTransporte);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarSeExisteEmailAtivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte emailAtivo = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(codigoEmpresa);

                if (emailAtivo != null)
                {
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Email.ConfigEmailDocTransporte.OcorreuFalhaVerificarEmailPadrao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("codigo"));

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmailDocTransporte = repConfigEmailDocTransporte.BuscarPorCodigo(codigo);

                repConfigEmailDocTransporte.Deletar(configEmailDocTransporte, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Email.ConfigEmailDocTransporte.NaoFoiPossivelExcluirRegistro);
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
