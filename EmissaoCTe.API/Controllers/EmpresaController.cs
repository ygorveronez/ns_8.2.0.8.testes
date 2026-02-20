using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class EmpresaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("empresasemissoras.aspx") select obj).FirstOrDefault();
        }
        private Dominio.ObjetosDeValor.PaginaUsuario PermissaoAcessoSistema()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("empresas.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult BuscarDadosParaAcesso()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                if (this.Usuario.TipoAcesso != Dominio.Enumeradores.TipoAcesso.Admin)
                    return Json<bool>(false, false, "Acesso negado.");

                int.TryParse(Request.Params["CodigoEmpresa"], out int codigoEmpresa);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPrimeiroPorEmpresa(codigoEmpresa, Dominio.Enumeradores.TipoAcesso.Emissao);

                var permissao = this.PermissaoAcessoSistema();
                if (permissao == null || permissao.PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão negada para acessar o sistema!");

                if (empresa == null)
                    return Json<bool>(false, false, "Empresa não encontrada.");

                if (empresa != null && empresa.StatusFinanceiro == "B")
                    return Json<bool>(false, false, "Empresa com pendências: <br /><br />" + empresa.Observacao);

                if (empresa == null)
                    return Json<bool>(false, false, "Empresa não encontrada.");

                if (usuario == null)
                    return Json<bool>(false, false, "Nenhum usuário encontrado para esta empresa.");

                string key = string.Concat("CT3##MULT1@#$S0FTW4R3", DateTime.Now.ToString("ddMMyyyyhh"));
                Dominio.ObjetosDeValor.ChaveAcesso chave = new Dominio.ObjetosDeValor.ChaveAcesso
                {
                    Login = HttpUtility.UrlEncode(Servicos.Criptografia.Criptografar(usuario.Login, key)),
                    Senha = HttpUtility.UrlEncode(Servicos.Criptografia.Criptografar(usuario.Senha, key)),
                    Usuario = HttpUtility.UrlEncode(Servicos.Criptografia.Criptografar(this.Usuario.Codigo.ToString(), key)),
                    UriAcesso = string.Concat(ConfigurationManager.AppSettings["PastaEmissaoCTe"], "/EncodedLogin.aspx")
                };

                return Json(chave, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados para acesso ao sistema.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarLista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                List<Dominio.Entidades.Empresa> listaEmpresas = repEmpresa.BuscarPorEmpresaPai(this.EmpresaUsuario.Codigo);
                var retorno = from obj in listaEmpresas select new { obj.Codigo, obj.RazaoSocial };
                return Json(retorno, true);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                List<Dominio.Entidades.Empresa> listaEmpresas = repEmpresa.BuscarTodos(this.EmpresaUsuario.Codigo, 50, inicioRegistros);
                int countEmpresas = repEmpresa.ContarTodos(this.EmpresaUsuario.Codigo);

                var retorno = from obj in listaEmpresas
                              select new
                              {
                                  obj.Codigo,
                                  obj.RazaoSocial,
                                  obj.NomeFantasia,
                                  CNPJ = string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(obj.CNPJ)),
                                  obj.InscricaoEstadual,
                                  obj.Telefone,
                                  obj.Contato,
                                  obj.TelefoneContato
                              };

                return Json(retorno, true, null, new string[] { "Código|5", "Razão Social|20", "Nome Fantasia|20", "CNPJ|12", "Inscrição Estadual|8", "Telefone|8", "Contato|10", "Tel. Contato|8" }, countEmpresas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados das empresas!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarUsuarios()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros, countUsuarios = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string nome = Request.Params["Nome"];
                string cpf = Request.Params["CPF"];
                string login = Request.Params["Login"];
                string tipo = Request.Params["Tipo"];
                string status = Request.Params["Status"];

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                List<Dominio.Entidades.Usuario> listaUsuarios = repUsuario.ConsultarUsuarios(this.EmpresaUsuario.Codigo, tipo, status, nome, cpf, login, inicioRegistros, 50);
                countUsuarios = repUsuario.ContarConsultarUsuarios(this.EmpresaUsuario.Codigo, tipo, status, nome, cpf, login);

                var retorno = from obj in listaUsuarios
                              select new
                              {
                                  obj.Codigo,
                                  obj.Nome,
                                  obj.CPF_Formatado,
                              };

                return Json(retorno, true, null, new string[] { "Código", "Nome|40", "CPF|40" }, countUsuarios);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os usuários.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string nome = Request.Params["Nome"];
                string cnpj = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);
                string placaVeiculo = Request.Params["PlacaVeiculo"];
                string status = Request.Params["Status"];

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                List<Dominio.Entidades.Empresa> listaEmpresas = new List<Dominio.Entidades.Empresa>();
                int countEmpresas = 0;

                if (this.EmpresaUsuario.CNPJ == "13969629000196" && Request.UrlReferrer != null && (Request.UrlReferrer.AbsoluteUri.Contains("Empresas.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("EmpresasEmissoras.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("Series.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("EncerramentoManualMDFe.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("EmpresaContrato.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("EmpresaEDI.aspx")))
                {
                    listaEmpresas = repEmpresa.ConsultarAdmin(0, nome, cnpj, placaVeiculo, status, inicioRegistros, 50);
                    countEmpresas = repEmpresa.ContarConsultaAdmin(0, nome, cnpj, placaVeiculo, status);
                }
                else
                {
                    if (status == "Z")
                        return Json<bool>(false, false, "Sem permissão para mudar empresa admin.");

                    listaEmpresas = repEmpresa.ConsultarAdmin(this.EmpresaUsuario.Codigo, nome, cnpj, placaVeiculo, status, inicioRegistros, 50);
                    countEmpresas = repEmpresa.ContarConsultaAdmin(this.EmpresaUsuario.Codigo, nome, cnpj, placaVeiculo, status);
                }

                var retorno = (from obj in listaEmpresas
                               select new
                               {
                                   CodigoCriptografado = HttpUtility.UrlEncode(Servicos.Criptografia.Criptografar(obj.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3")),
                                   StatusEmissao = this.Usuario.Callcenter ? obj.StatusEmissao : string.Empty,
                                   obj.Codigo,
                                   obj.RazaoSocial,
                                   obj.NomeFantasia,
                                   CNPJ = string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(obj.CNPJ)),
                                   obj.Telefone,
                                   Cidade = obj.Localidade.Descricao + " - " + obj.Localidade.Estado.Sigla
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "StatusEmissao", "Código|10", "Razão Social|20", "Nome Fantasia|15", "CNPJ|15", "Telefone|10", "Cidade|10" }, countEmpresas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados das empresas!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarEmpresasGerenciadoras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string nome = Request.Params["Nome"];
                string cnpj = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);
                string status = Request.Params["Status"];

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                List<Dominio.Entidades.Empresa> listaEmpresas = repEmpresa.ConsultarEmpresasPai(this.EmpresaUsuario.Codigo, nome, cnpj, status, inicioRegistros, 50);
                int countEmpresas = repEmpresa.ContarConsultaEmpresasPai(this.EmpresaUsuario.Codigo, nome, cnpj, status);

                var retorno = from obj in listaEmpresas
                              select new
                              {
                                  CodigoCriptografado = HttpUtility.UrlEncode(Servicos.Criptografia.Criptografar(obj.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3")),
                                  obj.Codigo,
                                  obj.RazaoSocial,
                                  obj.NomeFantasia,
                                  CNPJ = string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(obj.CNPJ)),
                                  obj.Telefone
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "Código|10", "Razão Social|25", "Nome Fantasia|25", "CNPJ|15", "Telefone|15" }, countEmpresas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados das empresas!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarEmails()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string emails = Request.Params["Emails"];
                string emailsAdministrativos = Request.Params["EmailsAdministrativos"];
                string emailsContador = Request.Params["EmailsContador"];
                string statusEmails = Request.Params["StatusEmails"];
                string statusEmailsAdministrativos = Request.Params["StatusEmailsAdministrativos"];
                string statusEmailsContador = Request.Params["StatusEmailsContador"];

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = this.EmpresaUsuario;

                empresa.Initialize();

                empresa.Email = emails;
                empresa.EmailAdministrativo = emailsAdministrativos;
                empresa.EmailContador = emailsContador;
                empresa.StatusEmail = statusEmails;
                empresa.StatusEmailAdministrativo = statusEmailsAdministrativos;
                empresa.StatusEmailContador = statusEmailsContador;
                repEmpresa.Atualizar(empresa, Auditado);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar os e-mails da empresa. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult GerarPercursos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;

                if (!int.TryParse(Request.Params["Codigo"], out codigo))
                    return Json<bool>(false, false, "Código da empresa inválido!");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigo);

                Servicos.Embarcador.Logistica.PassagemEntreEstados serPercursos = new Servicos.Embarcador.Logistica.PassagemEntreEstados(unitOfWork);

                serPercursos.AdicionarPassagensEntreTodosOsEstadosPadrao(empresa, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, "Gerou Percurso.", unitOfWork);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar os e-mails da empresa. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;

                if (!int.TryParse(Request.Params["Codigo"], out codigo))
                    return Json<bool>(false, false, "Código da empresa inválido!");

                if (this.EmpresaUsuario.Codigo == codigo || this.EmpresaUsuario.EmpresaPai == null)
                {
                    string rntrc = Request.Params["RNTRC"];
                    string emissao = Request.Params["Emissao"];
                    string statusEmissao = Request.Params["StatusEmissao"];
                    string status = Request.Params["Status"];
                    string statusFinanceiro = Request.Params["StatusFinanceiro"];
                    string razaoSocial = Request.Params["RazaoSocial"];
                    string nomeFantasia = Request.Params["NomeFantasia"];
                    string cnpj = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);
                    string ie = Utilidades.String.OnlyNumbers(Request.Params["InscricaoEstadual"]);
                    string im = Request.Params["InscricaoMunicipal"];
                    string fusoHorario = Request.Params["FusoHorario"];
                    string ieST = Request.Params["InscricaoEstadualSubstituicao"];
                    string cnae = Request.Params["CNAE"];
                    string suframa = Request.Params["SUFRAMA"];
                    string cep = Utilidades.String.OnlyNumbers(Request.Params["CEP"]);
                    string logradouro = Request.Params["Logradouro"];
                    string complemento = Request.Params["Complemento"];
                    string numero = Request.Params["Numero"];
                    string bairro = Request.Params["Bairro"];
                    string telefone1 = Request.Params["Telefone"];
                    string telefone2 = Request.Params["Telefone2"];
                    string contato = Request.Params["Contato"];
                    string telefoneContato = Request.Params["TelefoneContato"];
                    string serieCertificado = Request.Params["SerieCertificado"];
                    string senhaCertificado = Request.Params["SenhaCertificado"];
                    string nomeContador = Request.Params["NomeContador"];
                    string telefoneContador = Request.Params["TelefoneContador"];
                    string emails = Request.Params["Emails"];
                    string emailsAdministrativos = Request.Params["EmailsAdministrativos"];
                    string emailsContador = Request.Params["EmailsContador"];
                    string crcContador = Request.Params["CRCContador"];
                    string observacao = Request.Params["Observacao"];
                    string taf = Request.Params["TAF"];
                    string nroRegEstadual = Request.Params["NroRegEstadual"];
                    string cotm = Request.Params["COTM"];
                    string codigoIntegracao = Request.Params["CodigoIntegracao"];

                    double cpfCnpjContador;
                    double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Contador"]), out cpfCnpjContador);

                    if (string.IsNullOrWhiteSpace(rntrc))
                        return Json<bool>(false, false, "RNTRC inválido!");

                    if (string.IsNullOrWhiteSpace(emissao))
                        return Json<bool>(false, false, "Tipo de Emissão inválido!");

                    if (string.IsNullOrWhiteSpace(statusEmissao))
                        return Json<bool>(false, false, "Status de emissão inválido!");

                    if (string.IsNullOrWhiteSpace(status))
                        return Json<bool>(false, false, "Status inválido.");

                    if (string.IsNullOrWhiteSpace(razaoSocial))
                        return Json<bool>(false, false, "Razão social inválida!");

                    if (cnpj.Length != 14 || !Utilidades.Validate.ValidarCNPJ(cnpj))
                        return Json<bool>(false, false, "CNPJ inválido!");

                    if (string.IsNullOrWhiteSpace(ie))
                        ie = "0";

                    if (cep.Length != 8)
                        return Json<bool>(false, false, "CEP inválido!");

                    if (string.IsNullOrWhiteSpace(logradouro))
                        return Json<bool>(false, false, "Logradouro inválido!");

                    if (string.IsNullOrWhiteSpace(numero))
                        return Json<bool>(false, false, "Número inválido!");

                    if (bairro.Length <= 0)
                        return Json<bool>(false, false, "Bairro inválido!");

                    if (telefone1.Length <= 0)
                        return Json<bool>(false, false, "Telefone 1 inválido!");

                    if (!string.IsNullOrWhiteSpace(emails))
                    {
                        var arrEmails = emails.Split(';');
                        foreach (string email in arrEmails)
                            if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                                return Json<bool>(false, false, string.Concat("E-mail (", email, ") inválido."));
                    }

                    if (!string.IsNullOrWhiteSpace(emailsContador))
                    {
                        var arrEmails = emailsContador.Split(';');
                        foreach (string email in arrEmails)
                            if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                                return Json<bool>(false, false, string.Concat("E-mail do contador (", email, ") inválido."));
                    }

                    if (!string.IsNullOrWhiteSpace(emailsAdministrativos))
                    {
                        var arrEmails = emailsAdministrativos.Split(';');
                        foreach (string email in arrEmails)
                            if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                                return Json<bool>(false, false, string.Concat("E-mail administrativo (", email, ") inválido."));
                    }

                    int localidade, codigoPlanoEmissao, codigoEmpresaCobradora, diaVencimentoFatura, serieCTeDentro, serieCTeFora, serieMDFe = 0;

                    if (!int.TryParse(Request.Params["Localidade"], out localidade) || localidade <= 0)
                        return Json<bool>(false, false, "Localidade inválida!");

                    int.TryParse(Request.Params["CodigoEmpresaCobradora"], out codigoEmpresaCobradora);
                    int.TryParse(Request.Params["CodigoPlanoEmissao"], out codigoPlanoEmissao);
                    int.TryParse(Request.Params["DiaVencimento"], out diaVencimentoFatura);
                    int.TryParse(Request.Params["SerieCTeDentro"], out serieCTeDentro);
                    int.TryParse(Request.Params["SerieCTeFora"], out serieCTeFora);
                    int.TryParse(Request.Params["SerieMDFe"], out serieMDFe);
                    int codigoEmpresaAdmin = 0;
                    if (this.EmpresaUsuario.CNPJ == "13969629000196" && Request.UrlReferrer != null && Request.UrlReferrer.AbsoluteUri.Contains("EmpresasEmissoras.aspx"))
                        int.TryParse(Request.Params["CodigoEmpresaAdmin"], out codigoEmpresaAdmin);

                    DateTime dataInicialCertificado, dataFinalCertificado;

                    DateTime.TryParseExact(Request.Params["DataInicialCertificado"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicialCertificado);
                    DateTime.TryParseExact(Request.Params["DataFinalCertificado"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinalCertificado);

                    if (dataInicialCertificado == DateTime.MinValue)
                        dataInicialCertificado = DateTime.Today;

                    if (dataFinalCertificado == DateTime.MinValue)
                        dataFinalCertificado = DateTime.Today;

                    bool emailsStatus, simplesNacional, emailsAdministrativosStatus, emailsContadorStatus, certificadoA3, enviarEmailBloqueio = false;

                    bool.TryParse(Request.Params["EmailsStatus"], out emailsStatus);
                    bool.TryParse(Request.Params["SimplesNacional"], out simplesNacional);
                    bool.TryParse(Request.Params["EmailsAdministrativosStatus"], out emailsAdministrativosStatus);
                    bool.TryParse(Request.Params["EmailsContadorStatus"], out emailsContadorStatus);
                    bool.TryParse(Request.Params["CertificadoA3"], out certificadoA3);
                    bool.TryParse(Request.Params["PermiteEmissaoDocumentosDestinados"], out bool permiteEmissaoDocumentosDestinados);
                    bool.TryParse(Request.Params["EmitirTodosCTesComoSimples"], out bool emitirTodosCTesComoSimples);
                    bool.TryParse(Request.Params["CobrarDocumentosDestinados"], out bool cobrarDocumentosDestinados);

                    Dominio.Enumeradores.RegimeEspecialEmpresa? regime = null;
                    if (Enum.TryParse(Request.Params["RegimeEspecial"], out Dominio.Enumeradores.RegimeEspecialEmpresa regimeAux))
                        regime = regimeAux;

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioCTe? regimeCTe = null;
                    if (Enum.TryParse(Request.Params["RegimeTributarioCTe"], out Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioCTe regimeCTeAux))
                        regimeCTe = regimeCTeAux;

                    Dominio.Enumeradores.TipoTransportador tipoTransportador = Dominio.Enumeradores.TipoTransportador.CTC;
                    Enum.TryParse<Dominio.Enumeradores.TipoTransportador>(Request.Params["TipoTransportador"], out tipoTransportador);

                    decimal percentualCredito;
                    decimal.TryParse(Request.Params["PercentualCredito"], out percentualCredito);

                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                    Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                    Dominio.Entidades.Empresa empresa = null;

                    if (codigo > 0)
                    {
                        if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                            return Json<bool>(false, false, "Permissão para alteração de empresa negada!");

                        empresa = repEmpresa.BuscarPorCodigo(codigo);
                        empresa.Initialize();
                    }
                    else
                    {
                        if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                            return Json<bool>(false, false, "Permissão para inclusão de empresa negada!");

                        empresa = new Dominio.Entidades.Empresa
                        {
                            DataCadastro = DateTime.Now,
                            UsuarioCadastro = Auditado.Usuario,
                            CNPJ = cnpj
                        };
                    }

                    Dominio.Entidades.Empresa empresaAux = repEmpresa.BuscarPorCNPJ(cnpj);

                    if (empresaAux != null && empresaAux.Codigo != codigo)
                        return Json<bool>(false, false, "A empresa já se encontra cadastrada no sistema.");

                    empresa.Bairro = bairro;
                    empresa.CEP = cep;
                    empresa.CNAE = cnae;
                    empresa.Complemento = complemento;
                    empresa.Contato = contato;
                    empresa.DataAtualizacao = DateTime.Now;
                    empresa.UsuarioAtualizacao = Auditado.Usuario;
                    empresa.DataFinalCertificado = dataFinalCertificado;
                    empresa.DataInicialCertificado = dataInicialCertificado;
                    empresa.Email = emails;
                    empresa.EmailAdministrativo = emailsAdministrativos;
                    empresa.EmailContador = emailsContador;
                    empresa.Endereco = logradouro;
                    empresa.Fax = telefone2;
                    empresa.Inscricao_ST = ieST;
                    empresa.InscricaoEstadual = ie;
                    empresa.InscricaoMunicipal = im;
                    empresa.Localidade = repLocalidade.BuscarPorCodigo(localidade);
                    empresa.NomeFantasia = nomeFantasia;
                    empresa.Numero = numero;
                    empresa.RazaoSocial = razaoSocial;
                    if (string.IsNullOrEmpty(empresa.SenhaCertificado) && !string.IsNullOrEmpty(senhaCertificado))
                        empresa.SenhaCertificado = senhaCertificado;
                    empresa.SerieCertificado = serieCertificado;
                    empresa.StatusEmail = emailsStatus ? "A" : "I";
                    if (!string.IsNullOrWhiteSpace(Request.Params["PermiteEmissaoDocumentosDestinados"]))
                        empresa.PermiteEmissaoDocumentosDestinados = permiteEmissaoDocumentosDestinados;
                    if (!string.IsNullOrWhiteSpace(Request.Params["EmitirTodosCTesComoSimples"]))
                        empresa.EmitirTodosCTesComoSimples = emitirTodosCTesComoSimples;
                    if (!string.IsNullOrWhiteSpace(Request.Params["CobrarDocumentosDestinados"]))
                        empresa.CobrarDocumentosDestinados = cobrarDocumentosDestinados;
                    empresa.StatusEmailAdministrativo = emailsAdministrativosStatus ? "A" : "I";
                    empresa.StatusEmailContador = emailsContadorStatus ? "A" : "I";
                    empresa.Suframa = suframa;
                    empresa.Telefone = telefone1;
                    empresa.TelefoneContato = telefoneContato;
                    empresa.NomeContador = nomeContador;
                    empresa.TelefoneContador = telefoneContador;
                    empresa.CRCContador = crcContador;
                    empresa.RegistroANTT = rntrc;
                    empresa.OptanteSimplesNacional = simplesNacional;
                    empresa.FusoHorario = fusoHorario;
                    if (regime.HasValue)
                        empresa.RegimeEspecial = regime.Value;
                    if (regimeCTe.HasValue)
                        empresa.RegimeTributarioCTe = regimeCTe.Value;
                    empresa.TipoTransportador = tipoTransportador;
                    empresa.PercentualCredito = percentualCredito;
                    empresa.TAF = taf;
                    empresa.NroRegEstadual = nroRegEstadual;
                    empresa.CertificadoA3 = certificadoA3;
                    empresa.COTM = cotm;
                    empresa.CodigoIntegracao = codigoIntegracao;

                    if (cpfCnpjContador > 0f)
                        empresa.Contador = repCliente.BuscarPorCPFCNPJ(cpfCnpjContador);
                    else
                        empresa.Contador = null;

                    if (codigoEmpresaAdmin > 0 && empresa.EmpresaPai != null)
                    {
                        var empresaPai = repEmpresa.BuscarPorCodigo(codigoEmpresaAdmin);
                        if (empresaPai != null)
                            empresa.EmpresaPai = empresaPai;
                    }

                    if (this.EmpresaUsuario.EmpresaPai == null)
                    {
                        List<string> logStatus = new List<string>() {
                            empresa.LogStatus
                        };
                        if (empresa.Status != status)
                            logStatus.Add(DateTime.Now.ToString("dd/MM/yyyyy HH:mm") + " - O Usuário " + this.Usuario.Nome + " alterou o Status para " + (status == "A" ? "ATIVO" : "INATIVO") + ".");

                        empresa.Status = status;
                        empresa.TipoAmbiente = emissao == "P" ? Dominio.Enumeradores.TipoAmbiente.Producao : Dominio.Enumeradores.TipoAmbiente.Homologacao;
                        empresa.StatusEmissao = statusEmissao;
                        if (!string.IsNullOrWhiteSpace(statusFinanceiro))
                        {
                            if (empresa.StatusFinanceiro == "N" && statusFinanceiro == "B")
                                enviarEmailBloqueio = true;

                            if (empresa.StatusFinanceiro != statusFinanceiro)
                                logStatus.Add(DateTime.Now.ToString("dd/MM/yyyyy HH:mm") + " - O Usuário " + this.Usuario.Nome + " alterou o Status Financeiro para " + (statusFinanceiro == "N" ? "NORMAL" : "BLOQUEADO") + ".");
                            empresa.StatusFinanceiro = statusFinanceiro;
                        }
                        empresa.EmpresaCobradora = repEmpresa.BuscarPorCodigo(codigoEmpresaCobradora);
                        empresa.DiaVencimentoFatura = diaVencimentoFatura;

                        Repositorio.PlanoEmissaoCTe repPlanoEmissaoCTe = new Repositorio.PlanoEmissaoCTe(unitOfWork);
                        empresa.PlanoEmissaoCTe = repPlanoEmissaoCTe.BuscarPorCodigo(codigoPlanoEmissao);

                        empresa.LogStatus = String.Join(Environment.NewLine, logStatus);
                    }

                    unitOfWork.Start();

                    if (codigo > 0)
                    {
                        repEmpresa.Atualizar(empresa, Auditado);
                    }
                    else
                    {
                        if (this.EmpresaUsuario.Codigo != codigo)
                            empresa.EmpresaPai = this.EmpresaUsuario;

                        repEmpresa.Inserir(empresa, Auditado);

                        this.ReplicarDadosPadroes(empresa, unitOfWork);
                    }

                    this.SalvarPermissoes(empresa, unitOfWork);

                    if (repUsuario.ContarPorEmpresaETipo(empresa.Codigo, Dominio.Enumeradores.TipoAcesso.Emissao) <= 0)
                        this.SalvarUsuarioPadrao(empresa, unitOfWork);
                    if (empresa.StatusEmissao == "S" || empresa.StatusEmissao == "C")
                        this.SalvarUsuarioEmpresa(empresa, unitOfWork);

                    if (repEmpresaSerie.ContarPorEmpresa(empresa.Codigo) <= 0)
                    {
                        if (serieCTeDentro > 0)
                        {
                            empresa.SerieCTeDentro = serieCTeDentro;
                            if (repEmpresaSerie.BuscarPorSerie(empresa.Codigo, serieCTeDentro, Dominio.Enumeradores.TipoSerie.CTe) == null)
                            {
                                Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();
                                serie.Empresa = empresa;
                                serie.Numero = serieCTeDentro;
                                serie.Status = "A";
                                serie.Tipo = Dominio.Enumeradores.TipoSerie.CTe;
                                repEmpresaSerie.Inserir(serie);
                            }
                        }
                        if (serieCTeFora > 0)
                        {
                            empresa.SerieCTeFora = serieCTeFora;
                            if (repEmpresaSerie.BuscarPorSerie(empresa.Codigo, serieCTeFora, Dominio.Enumeradores.TipoSerie.CTe) == null)
                            {
                                Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();
                                serie.Empresa = empresa;
                                serie.Numero = serieCTeFora;
                                serie.Status = "A";
                                serie.Tipo = Dominio.Enumeradores.TipoSerie.CTe;
                                repEmpresaSerie.Inserir(serie);
                            }
                        }
                        if (serieMDFe > 0)
                        {
                            empresa.SerieMDFe = serieMDFe;
                            if (repEmpresaSerie.BuscarPorSerie(empresa.Codigo, serieMDFe, Dominio.Enumeradores.TipoSerie.MDFe) == null)
                            {
                                Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();
                                serie.Empresa = empresa;
                                serie.Numero = serieMDFe;
                                serie.Status = "A";
                                serie.Tipo = Dominio.Enumeradores.TipoSerie.MDFe;
                                repEmpresaSerie.Inserir(serie);
                            }
                        }
                        if (serieCTeDentro > 0 || serieCTeFora > 0 || serieMDFe > 0)
                            this.ReplicarConfig(empresa, unitOfWork);

                        //if (serieCTeDentro == 0 && serieCTeFora == 0)
                        // this.SalvarSeriePadrao(empresa, unidadeTrabalho);
                        //if (serieMDFe == 0)
                        //  this.SalvarSerieMDFePadrao(empresa, unidadeTrabalho);
                    }


                    if (this.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin)
                    {
                        empresa.Observacao = observacao;

                        List<Dominio.ObjetosDeValor.Filial> filiais = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Filial>>(Request.Params["Filiais"]);

                        if (empresa.Filiais.Count() > 0)
                            empresa.Filiais.Clear();

                        if (filiais != null && filiais.Count() > 0)
                            empresa.Filiais = (from obj in filiais where obj.Codigo != empresa.Codigo select new Dominio.Entidades.Empresa() { Codigo = obj.Codigo }).ToList();
                    }

                    unitOfWork.CommitChanges();

                    if (enviarEmailBloqueio)
                        EnviarEmailDeNotificacaoDeBloqueio(empresa.Codigo, unitOfWork);

                    Servicos.AtualizacaoEmpresa svcAtualizaEmpresa = new Servicos.AtualizacaoEmpresa(unitOfWork);

                    string atualizarEmpresa = svcAtualizaEmpresa.Atualizar(empresa, unitOfWork);

                    string mensagemErro = string.Empty;
                    Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech integracaoNStech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
                    if (!integracaoNStech.IncluirAtualizarCertificado(out mensagemErro, empresa, unitOfWork))
                        Servicos.Log.TratarErro($"Ocorreu um erro ao enviar o certificado para o emissor NStech: {mensagemErro}");

                    object retorno = new
                    {
                        empresa.Codigo,
                        statusOracle = atualizarEmpresa
                    };

                    if (empresa.Configuracao != null && empresa.Configuracao.NFSeIntegracaoENotas)
                    {
                        Servicos.NFSeENotas svcNFSeENotas = new Servicos.NFSeENotas(unitOfWork);
                        var retornoEmpresaNFse = svcNFSeENotas.SalvarEmpresa(empresa.Codigo, unitOfWork);
                        if (retornoEmpresaNFse != null && !string.IsNullOrWhiteSpace(retornoEmpresaNFse.Result))
                            return Json<bool>(false, false, retornoEmpresaNFse.Result);
                        //Enviar certificado não disponibilizado
                        //if (!string.IsNullOrWhiteSpace(empresa.NomeCertificado) && Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado))
                        //{
                        //    var retornoCertificadoNFse = svcNFSeENotas.EnviarCertificadoDigital(empresa.Codigo, unidadeTrabalho);
                        //    if (retornoCertificadoNFse != null && !string.IsNullOrWhiteSpace(retornoCertificadoNFse.Result))
                        //        return Json<bool>(false, false, retornoCertificadoNFse.Result);
                        //}
                    }

                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Permissão negada! Não é possível cadastrar uma empresa emissora.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a empresa! Atualize a página e tente novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarEmpresaGerenciadora()
        {
            if (this.Usuario.TipoAcesso != Dominio.Enumeradores.TipoAcesso.AdminInterno)
                return Json<bool>(false, false, "Permissão negada.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo;
                if (!int.TryParse(Request.Params["Codigo"], out codigo))
                    return Json<bool>(false, false, "Código da empresa inválido!");

                string rntrc = Request.Params["RNTRC"];
                if (string.IsNullOrWhiteSpace(rntrc))
                    return Json<bool>(false, false, "RNTRC inválido!");
                string status = Request.Params["Status"];
                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");
                string razaoSocial = Request.Params["RazaoSocial"];
                if (string.IsNullOrWhiteSpace(razaoSocial))
                    return Json<bool>(false, false, "Razão social inválida!");
                string nomeFantasia = Request.Params["NomeFantasia"];
                string cnpj = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);
                if (cnpj.Length != 14 || !Utilidades.Validate.ValidarCNPJ(cnpj))
                    return Json<bool>(false, false, "CNPJ inválido!");
                string ie = Utilidades.String.OnlyNumbers(Request.Params["InscricaoEstadual"]);
                if (string.IsNullOrWhiteSpace(ie))
                    ie = "0";
                string fusoHorario = Request.Params["FusoHorario"];
                string ieST = Request.Params["InscricaoEstadualSubstituicao"];
                string cnae = Request.Params["CNAE"];
                string suframa = Request.Params["SUFRAMA"];
                string cep = Utilidades.String.OnlyNumbers(Request.Params["CEP"]);
                if (cep.Length != 8)
                    return Json<bool>(false, false, "CEP inválido!");
                string logradouro = Request.Params["Logradouro"];
                if (string.IsNullOrWhiteSpace(logradouro))
                    return Json<bool>(false, false, "Logradouro inválido!");
                string complemento = Request.Params["Complemento"];
                string numero = Request.Params["Numero"];
                if (string.IsNullOrWhiteSpace(numero))
                    return Json<bool>(false, false, "Número inválido!");
                string bairro = Request.Params["Bairro"];
                if (bairro.Length <= 0)
                    return Json<bool>(false, false, "Bairro inválido!");
                string telefone1 = Request.Params["Telefone"];
                if (telefone1.Length <= 0)
                    return Json<bool>(false, false, "Telefone 1 inválido!");
                string telefone2 = Request.Params["Telefone2"];
                int localidade;
                if (!int.TryParse(Request.Params["Localidade"], out localidade) || localidade <= 0)
                    return Json<bool>(false, false, "Localidade inválida!");
                string contato = Request.Params["Contato"];
                string telefoneContato = Request.Params["TelefoneContato"];
                string nomeContador = Request.Params["NomeContador"];
                string telefoneContador = Request.Params["TelefoneContador"];
                bool simplesNacional;
                bool.TryParse(Request.Params["SimplesNacional"], out simplesNacional);
                string emails = Request.Params["Emails"];
                string emailsAdministrativos = Request.Params["EmailsAdministrativos"];
                string emailsContador = Request.Params["EmailsContador"];

                if (!string.IsNullOrWhiteSpace(emails))
                {
                    var arrEmails = emails.Split(';');
                    foreach (string email in arrEmails)
                        if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                            return Json<bool>(false, false, string.Concat("E-mail (", email, ") inválido."));
                }

                if (!string.IsNullOrWhiteSpace(emailsContador))
                {
                    var arrEmails = emailsContador.Split(';');
                    foreach (string email in arrEmails)
                        if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                            return Json<bool>(false, false, string.Concat("E-mail do contador (", email, ") inválido."));
                }

                if (!string.IsNullOrWhiteSpace(emailsAdministrativos))
                {
                    var arrEmails = emailsAdministrativos.Split(';');
                    foreach (string email in arrEmails)
                        if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                            return Json<bool>(false, false, string.Concat("E-mail administrativo (", email, ") inválido."));
                }

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Dominio.Entidades.Empresa empresa;
                if (codigo > 0)
                {
                    empresa = repEmpresa.BuscarPorCodigo(codigo);
                    empresa.Initialize();
                }
                else
                {
                    empresa = new Dominio.Entidades.Empresa();
                    if (repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpj)) != null)
                        return Json<bool>(false, false, "A empresa já se encontra cadastrada no sistema.");
                }

                empresa.Bairro = bairro;
                empresa.CEP = cep;
                empresa.CNAE = cnae;
                empresa.CNPJ = cnpj;
                empresa.Complemento = complemento;
                empresa.Contato = contato;
                empresa.DataAtualizacao = DateTime.Now;
                empresa.UsuarioAtualizacao = Auditado.Usuario;
                empresa.Email = emails;
                empresa.EmailAdministrativo = emailsAdministrativos;
                empresa.EmailContador = emailsContador;
                empresa.Endereco = logradouro;
                empresa.Fax = telefone2;
                empresa.Inscricao_ST = ieST;
                empresa.InscricaoEstadual = ie;
                empresa.Localidade = repLocalidade.BuscarPorCodigo(localidade);
                empresa.NomeFantasia = nomeFantasia;
                empresa.Numero = numero;
                empresa.RazaoSocial = razaoSocial;
                empresa.Suframa = suframa;
                empresa.Telefone = telefone1;
                empresa.TelefoneContato = telefoneContato;
                empresa.NomeContador = nomeContador;
                empresa.TelefoneContador = telefoneContador;
                empresa.RegistroANTT = rntrc;
                empresa.OptanteSimplesNacional = simplesNacional;
                empresa.TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Homologacao;
                empresa.StatusEmissao = "M";
                empresa.Status = status;
                empresa.FusoHorario = fusoHorario;
                empresa.CertificadoA3 = false;

                empresa.EmpresaAdministradora = this.EmpresaUsuario;

                unitOfWork.Start();

                if (codigo > 0)
                {
                    repEmpresa.Atualizar(empresa, Auditado);
                }
                else
                {
                    repEmpresa.Inserir(empresa, Auditado);
                }

                if (repUsuario.ContarPorEmpresa(empresa.Codigo) <= 0)
                    this.SalvarUsuarioPadrao(empresa, unitOfWork);

                unitOfWork.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a empresa! Atualize a página e tente novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarSerieEmpresaUsuario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(this.Usuario.Empresa.Codigo);

                if (empresa != null)
                {
                    var retorno = new
                    {
                        empresa.SerieCTeDentro,
                        empresa.SerieCTeFora,
                        empresa.SerieMDFe
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Empresa não encontrada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados da empresa. Atualize a página e tente novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                string cnpj = Utilidades.String.OnlyNumbers(Request.Params["CNPJ"]);

                if (codigo == 0 && string.IsNullOrWhiteSpace(cnpj))
                    return Json<bool>(false, false, "Código da empresa inválido!");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);

                Dominio.Entidades.Empresa empresa = null;

                if (codigo > 0)
                    empresa = repEmpresa.BuscarPorCodigo(codigo);
                else if (!string.IsNullOrWhiteSpace(cnpj))
                    empresa = repEmpresa.BuscarPorCNPJ(cnpj);

                if (empresa != null)
                {
                    if (this.EmpresaUsuario.CNPJ != "13969629000196" && Request.UrlReferrer != null && (Request.UrlReferrer.AbsoluteUri.Contains("Empresas.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("EmpresasEmissoras.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("Series.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("EncerramentoManualMDFe.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("EmpresaContrato.aspx") || Request.UrlReferrer.AbsoluteUri.Contains("EmpresaEDI.aspx")))
                    {
                        if (this.EmpresaUsuario.Codigo != empresa.EmpresaPai?.Codigo)
                            return Json<bool>(false, false, "Empresa não encontrada.");
                    }

                    var retorno = new
                    {
                        empresa.RazaoSocial,
                        empresa.NomeFantasia,
                        CNPJ = string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(empresa.CNPJ)),
                        empresa.InscricaoEstadual,
                        empresa.Codigo,
                        CodigoCriptografado = HttpUtility.UrlEncode(Servicos.Criptografia.Criptografar(empresa.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3")),
                        InscricaoEstadualSubstituicao = empresa.Inscricao_ST,
                        empresa.CNAE,
                        SUFRAMA = empresa.Suframa,
                        CEP = string.Format(@"{0:00\.000\-000}", int.Parse(empresa.CEP)),
                        Logradouro = empresa.Endereco,
                        empresa.Complemento,
                        empresa.Numero,
                        empresa.Bairro,
                        empresa.Telefone,
                        Telefone2 = empresa.Fax,
                        Localidade = empresa.Localidade.Codigo,
                        SiglaUF = empresa.Localidade.Estado.Sigla,
                        empresa.Contato,
                        empresa.TelefoneContato,
                        empresa.TelefoneContador,
                        empresa.NomeContador,
                        DataCadastro = empresa.DataCadastro.HasValue ? empresa.DataCadastro.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataInicialCertificado = empresa.DataInicialCertificado.HasValue ? empresa.DataInicialCertificado.Value.ToString("dd/MM/yyyy") : string.Empty,
                        DataFinalCertificado = empresa.DataFinalCertificado.HasValue ? empresa.DataFinalCertificado.Value.ToString("dd/MM/yyyy") : string.Empty,
                        empresa.SerieCertificado,
                        empresa.SenhaCertificado,
                        empresa.NomeCertificado,
                        Emails = empresa.Email,
                        RNTRC = empresa.RegistroANTT,
                        empresa.Status,
                        empresa.StatusEmissao,
                        empresa.StatusFinanceiro,
                        empresa.TipoTransportador,
                        Emissao = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? "P" : "H",
                        EmailsStatus = empresa.StatusEmail == "A" ? true : false,
                        empresa.PermiteEmissaoDocumentosDestinados,
                        empresa.CobrarDocumentosDestinados,
                        empresa.EmitirTodosCTesComoSimples,
                        EmailsAdministrativos = empresa.EmailAdministrativo,
                        EmailsAdministrativosStatus = empresa.StatusEmailAdministrativo == "A" ? true : false,
                        EmailsContador = empresa.EmailContador,
                        EmailsContadorStatus = empresa.StatusEmailContador == "A" ? true : false,
                        SimplesNacional = empresa.OptanteSimplesNacional,
                        PossuiLogoSistema = string.IsNullOrWhiteSpace(empresa.CaminhoLogoSistema) ? false : true,
                        PossuiCertificado = string.IsNullOrWhiteSpace(empresa.NomeCertificado) ? false : true,
                        PossuiLogoDacte = string.IsNullOrWhiteSpace(empresa.CaminhoLogoDacte) ? false : true,
                        empresa.FusoHorario,
                        CodigoPlanoEmissao = empresa.PlanoEmissaoCTe == null ? 0 : empresa.PlanoEmissaoCTe.Codigo,
                        DescricaoPlanoEmissao = empresa.PlanoEmissaoCTe == null ? string.Empty : empresa.PlanoEmissaoCTe.Descricao,
                        CodigoEmpresaCobradora = empresa.EmpresaCobradora == null ? 0 : empresa.EmpresaCobradora.Codigo,
                        DescricaoEmpresaCobradora = empresa.EmpresaCobradora == null ? string.Empty : string.Concat(empresa.EmpresaCobradora.CNPJ, " - ", empresa.EmpresaCobradora.NomeFantasia),
                        empresa.DiaVencimentoFatura,
                        DescricaoContador = empresa.Contador != null ? empresa.Contador.CPF_CNPJ_Formatado + " - " + empresa.Contador.Nome : string.Empty,
                        CPFCNPJContador = empresa.Contador != null ? empresa.Contador.CPF_CNPJ_Formatado : string.Empty,
                        empresa.CRCContador,
                        empresa.Observacao,
                        Filiais = (from obj in empresa.Filiais select new Dominio.ObjetosDeValor.Filial() { CNPJ = obj.CNPJ_Formatado, Codigo = obj.Codigo, NomeFantasia = obj.NomeFantasia, RazaoSocial = obj.RazaoSocial }).ToList(),
                        empresa.InscricaoMunicipal,
                        empresa.RegimeEspecial,
                        empresa.RegimeTributarioCTe,
                        PercentualCredito = empresa.PercentualCredito.ToString("n2"),
                        empresa.SerieCTeDentro,
                        empresa.SerieCTeFora,
                        empresa.SerieMDFe,
                        seriesCadastradas = repEmpresaSerie.ContarPorEmpresa(empresa.Codigo),
                        empresa.TAF,
                        empresa.NroRegEstadual,
                        empresa.CertificadoA3,
                        empresa.COTM,
                        empresa.CodigoIntegracao,
                        LogStatus = empresa.LogStatus ?? string.Empty,
                        CodigoEmpresaAdmin = empresa.EmpresaPai?.Codigo ?? 0,
                        DescricaoEmpresaAdmin = empresa.EmpresaPai != null ? empresa.EmpresaPai.CNPJ + " " + empresa.EmpresaPai.RazaoSocial : string.Empty
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Empresa não encontrada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados da empresa. Atualize a página e tente novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesDaEmpresaDoUsuario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento configuracaoIntegracaoEmissorDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento(unitOfWork).BuscarConfiguracaoPadrao();

                var retorno = new
                {
                    CodigoCriptografado = HttpUtility.UrlEncode(Servicos.Criptografia.Criptografar(this.EmpresaUsuario.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3")),
                    this.EmpresaUsuario.RazaoSocial,
                    this.EmpresaUsuario.NomeFantasia,
                    CNPJ = string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.EmpresaUsuario.CNPJ)),
                    this.EmpresaUsuario.InscricaoEstadual,
                    this.EmpresaUsuario.InscricaoMunicipal,
                    this.EmpresaUsuario.Codigo,
                    InscricaoEstadualSubstituicao = this.EmpresaUsuario.Inscricao_ST,
                    this.EmpresaUsuario.CNAE,
                    SUFRAMA = this.EmpresaUsuario.Suframa,
                    CEP = string.Format(@"{0:00\.000\-000}", int.Parse(this.EmpresaUsuario.CEP)),
                    Logradouro = this.EmpresaUsuario.Endereco,
                    this.EmpresaUsuario.Complemento,
                    this.EmpresaUsuario.Numero,
                    this.EmpresaUsuario.Bairro,
                    this.EmpresaUsuario.Telefone,
                    Telefone2 = this.EmpresaUsuario.Fax,
                    Localidade = this.EmpresaUsuario.Localidade.Codigo,
                    SiglaUF = this.EmpresaUsuario.Localidade.Estado.Sigla,
                    this.EmpresaUsuario.Contato,
                    this.EmpresaUsuario.TelefoneContato,
                    this.EmpresaUsuario.TelefoneContador,
                    this.EmpresaUsuario.NomeContador,
                    DataInicialCertificado = this.EmpresaUsuario.DataInicialCertificado.HasValue ? this.EmpresaUsuario.DataInicialCertificado.Value.ToString("dd/MM/yyyy") : string.Empty,
                    DataFinalCertificado = this.EmpresaUsuario.DataFinalCertificado.HasValue ? this.EmpresaUsuario.DataFinalCertificado.Value.ToString("dd/MM/yyyy") : string.Empty,
                    this.EmpresaUsuario.SerieCertificado,
                    this.EmpresaUsuario.SenhaCertificado,
                    this.EmpresaUsuario.NomeCertificado,
                    Emails = this.EmpresaUsuario.Email,
                    RNTRC = this.EmpresaUsuario.RegistroANTT,
                    this.EmpresaUsuario.Status,
                    Emissao = this.EmpresaUsuario.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? "P" : "H",
                    this.EmpresaUsuario.TipoTransportador,
                    this.EmpresaUsuario.StatusEmissao,
                    EmailsStatus = this.EmpresaUsuario.StatusEmail == "A" ? true : false,
                    EmailsAdministrativos = this.EmpresaUsuario.EmailAdministrativo,
                    EmailsAdministrativosStatus = this.EmpresaUsuario.StatusEmailAdministrativo == "A" ? true : false,
                    EmailsContador = this.EmpresaUsuario.EmailContador,
                    EmailsContadorStatus = this.EmpresaUsuario.StatusEmailContador == "A" ? true : false,
                    SimplesNacional = this.EmpresaUsuario.OptanteSimplesNacional,
                    PossuiLogoSistema = string.IsNullOrWhiteSpace(this.EmpresaUsuario.CaminhoLogoSistema) ? false : true,
                    PossuiCertificado = string.IsNullOrWhiteSpace(this.EmpresaUsuario.NomeCertificado) ? false : true,
                    PossuiLogoDacte = string.IsNullOrWhiteSpace(this.EmpresaUsuario.CaminhoLogoDacte) ? false : true,
                    this.EmpresaUsuario.FusoHorario,
                    DescricaoContador = this.EmpresaUsuario.Contador != null ? this.EmpresaUsuario.Contador.CPF_CNPJ_Formatado + " - " + this.EmpresaUsuario.Contador.Nome : string.Empty,
                    CPFCNPJContador = this.EmpresaUsuario.Contador != null ? this.EmpresaUsuario.Contador.CPF_CNPJ_Formatado : string.Empty,
                    this.EmpresaUsuario.CRCContador,
                    this.EmpresaUsuario.RegimeEspecial,
                    this.EmpresaUsuario.RegimeTributarioCTe,
                    PercentualCredito = this.EmpresaUsuario.PercentualCredito.ToString("n2"),
                    this.EmpresaUsuario.TAF,
                    this.EmpresaUsuario.NroRegEstadual,
                    this.EmpresaUsuario.COTM,
                    this.EmpresaUsuario.CertificadoA3,
                    ExibirCobrancaCancelamento = this.UsuarioAdministrativo != null ? this.EmpresaUsuario.EmpresaPai?.Configuracao != null ? this.EmpresaUsuario.EmpresaPai.Configuracao.ExibirCobrancaCancelamento : false : false,
                    UtilizaResumoEmissaoCTe = this.UsuarioAdministrativo != null && this.EmpresaUsuario.EmpresaPai != null && this.EmpresaUsuario.EmpresaPai.Configuracao != null ? this.EmpresaUsuario.EmpresaPai.Configuracao.UtilizaResumoEmissaoCTe : this.EmpresaUsuario.Configuracao != null ? this.EmpresaUsuario.Configuracao.UtilizaResumoEmissaoCTe : false,
                    NaoSmarCreditoICMSNoValorDaPrestacao = this.EmpresaUsuario.Configuracao?.NaoSmarCreditoICMSNoValorDaPrestacao ?? false,
                    EmissaoCTe = new
                    {
                        // As informacoes desse objeto serao armazenadas numa variavel global no JS.
                        VersaoMDFe = this.BuscaVersaoMDFe(),
                        VersaoCTe = this.BuscaVersaoCTe(),
                    },
                    configuracaoIntegracaoEmissorDocumento.TipoEmissorDocumentoMDFe,
                    DataVigenciaReformaTributaria = configuracaoIntegracaoEmissorDocumento.DataLiberacaoImpostos
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados da empresa. Atualize a página e tente novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.EmpresaUsuario.Configuracao != null && !this.EmpresaUsuario.Configuracao.ExibirHomeVencimentoCertificado)
                    return Json<bool>(false, false, "Aviso vencimento certificado sem configuração para exibição na pagina inicial.");
                else
                {
                    var retorno = new { DataVencimento = this.EmpresaUsuario.DataFinalCertificado.Value != null ? this.EmpresaUsuario.DataFinalCertificado.Value.ToString("dd/MM/yyyy") : "" };
                    return Json(retorno, true);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do certificado da empresa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult SalvarCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    return Json<bool>(false, false, "Permissão para alteração de empresa negada!");

                int codigoEmpresa = 0;
                if (!int.TryParse(Request.Params["Codigo"], out codigoEmpresa) || codigoEmpresa <= 0)
                    return Json<bool>(false, false, "Empresa inválida.");

                string senha = Request.Params["SenhaCertificado"];
                if (string.IsNullOrWhiteSpace(senha))
                    return Json<bool>(false, false, "Senha não informada.");

                if (Request.Files.Count > 0)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                    if (empresa != null)
                    {
                        System.Web.HttpPostedFileBase file = Request.Files[0];
                        if (System.IO.Path.GetExtension(file.FileName).ToLowerInvariant() == ".pfx")
                        {
                            var config = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo();

                            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(config.CaminhoArquivosEmpresas, Utilidades.String.OnlyNumbers(empresa.CNPJ), "Certificado");

                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, empresa.CNPJ + ".pfx");

                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                            Utilidades.IO.FileStorageService.Storage.SaveStream(caminho, file.InputStream);

                            file.SaveAs(caminho);

                            byte[] certificadoDigitalByteArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);

                            X509Certificate2 certificado = new X509Certificate2(certificadoDigitalByteArray, senha,
                                                           X509KeyStorageFlags.MachineKeySet
                                                         | X509KeyStorageFlags.PersistKeySet
                                                         | X509KeyStorageFlags.Exportable);

                            var cnpjCertificado = certificado.ObterCnpj();

                            if (!string.IsNullOrWhiteSpace(cnpjCertificado) && cnpjCertificado != empresa.CNPJ.Substring(0, 8))
                                return Json<bool>(false, false, "CNPJ do certificado (" + cnpjCertificado + ") não pertence ao CNPJ da empresa (" + empresa.CNPJ.Substring(0, 8) + ").");

                            empresa.NomeCertificado = caminho;
                            empresa.SenhaCertificado = senha;
                            empresa.SerieCertificado = certificado.SerialNumber;
                            empresa.DataInicialCertificado = certificado.NotBefore;
                            empresa.DataFinalCertificado = certificado.NotAfter;

                            repEmpresa.Atualizar(empresa);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, "Atualizou o certificado.", unitOfWork);

                            return Json<bool>(true, true);
                        }
                        else
                        {
                            throw new Exception("Extensão de arquivo inválida!");
                        }
                    }
                    else
                    {
                        throw new Exception("Empresa não encontrada!");
                    }
                }
                else
                {
                    throw new Exception("Número de arquivos inválido.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoEmpresa = 0;
                int.TryParse(Request.Params["Codigo"], out codigoEmpresa);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (empresa == null)
                    return Json<bool>(false, false, "Empresa não encontrada.");

                if (string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado))
                    return Json<bool>(false, false, "Não há certificado cadastrado para esta empresa.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(empresa.NomeCertificado), "application/x-pkcs12", empresa.CNPJ + ".pfx");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter o certificado digital.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult DeletarCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    return Json<bool>(false, false, "Permissão para alteração de empresa negada!");

                int codigoEmpresa = 0;
                if (!int.TryParse(Request.Params["Codigo"], out codigoEmpresa) || codigoEmpresa <= 0)
                    return Json<bool>(false, false, "Empresa inválida.");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                if (empresa != null)
                {
                    if (!string.IsNullOrWhiteSpace(empresa.NomeCertificado) && Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado))
                        Utilidades.IO.FileStorageService.Storage.Delete(empresa.NomeCertificado);

                    empresa.NomeCertificado = string.Empty;
                    repEmpresa.Atualizar(empresa);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, "Deletou o certificado.", unitOfWork);

                    return Json<bool>(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "Empresa não encontrada!");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao remover o certificado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult SalvarLogo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    return Json<bool>(false, false, "Permissão para alteração de empresa negada!");

                int codigoEmpresa = 0;
                if (!int.TryParse(Request.Params["Codigo"], out codigoEmpresa) || codigoEmpresa <= 0)
                    return Json<bool>(false, false, "Empresa inválida.");
                if (Request.Files.Count > 0)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    if (empresa != null)
                    {
                        System.Web.HttpPostedFileBase file = Request.Files[0];
                        if (System.IO.Path.GetExtension(file.FileName).ToLowerInvariant() == ".bmp")
                        {

                            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivosEmpresas"], Utilidades.String.OnlyNumbers(empresa.CNPJ));

                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "Logo", "DACTE");

                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, file.FileName);

                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                            Utilidades.IO.FileStorageService.Storage.SaveStream(caminho, file.InputStream);

                            empresa.CaminhoLogoDacte = caminho;
                            repEmpresa.Atualizar(empresa);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, "Salvou uma Logo.", unitOfWork);

                            return Json<bool>(true, true);
                        }
                        else
                        {
                            return Json<bool>(false, false, "Extensão de arquivo inválida!");
                        }
                    }
                    else
                    {
                        return Json<bool>(false, false, "Empresa não encontrada!");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Número de arquivos inválido.");
                }
            }
            catch
            {
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao salvar a logo da empresa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult DeletarLogo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    return Json<bool>(false, false, "Permissão para alteração de empresa negada!");

                int codigoEmpresa = 0;
                if (!int.TryParse(Request.Params["Codigo"], out codigoEmpresa) || codigoEmpresa <= 0)
                    return Json<bool>(false, false, "Empresa inválida.");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                if (empresa != null)
                {
                    if (!string.IsNullOrWhiteSpace(empresa.CaminhoLogoDacte) && Utilidades.IO.FileStorageService.Storage.Exists(empresa.CaminhoLogoDacte))
                        Utilidades.IO.FileStorageService.Storage.Delete(empresa.CaminhoLogoDacte);

                    empresa.CaminhoLogoDacte = string.Empty;
                    repEmpresa.Atualizar(empresa);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, "Deletou a Logo.", unitOfWork);

                    return Json<bool>(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "Empresa não encontrada!");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao remover a logo da DACTE.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult ObterLogoDaEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (!string.IsNullOrWhiteSpace(this.EmpresaUsuario.CaminhoLogoSistema))
                    return File(this.EmpresaUsuario.CaminhoLogoSistema, "image/png");
                if (this.EmpresaUsuario.EmpresaPai != null && !string.IsNullOrWhiteSpace(this.EmpresaUsuario.EmpresaPai.CaminhoLogoSistema))
                    return File(this.EmpresaUsuario.EmpresaPai.CaminhoLogoSistema, "image/png");
                if (Utilidades.IO.FileStorageService.Storage.Exists(Server.MapPath("Images/logomulti_small.png").Replace("\\Empresa", "")))
                    return File(Server.MapPath("Images/logomulti_small.png").Replace("\\Empresa", ""), "image/png");
                return null;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return null;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult SalvarLogoDoSistema()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    return Json<bool>(false, false, "Permissão para alteração de empresa negada!");

                int codigoEmpresa = 0;
                if (!int.TryParse(Request.Params["Codigo"], out codigoEmpresa) || codigoEmpresa <= 0)
                    return Json<bool>(false, false, "Empresa inválida.");
                if (Request.Files.Count > 0)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    if (empresa != null)
                    {
                        System.Web.HttpPostedFileBase file = Request.Files[0];
                        if (System.IO.Path.GetExtension(file.FileName).ToLowerInvariant() == ".png")
                        {

                            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivosEmpresas"], Utilidades.String.OnlyNumbers(empresa.CNPJ));

                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "Logo", "Sistema");

                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, file.FileName);

                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                            Utilidades.IO.FileStorageService.Storage.SaveStream(caminho, file.InputStream);

                            empresa.CaminhoLogoSistema = caminho;
                            repEmpresa.Atualizar(empresa);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, "Salvou uma Logo do Sistema.", unitOfWork);

                            return Json<bool>(true, true);
                        }
                        else
                        {
                            return Json<bool>(false, false, "Extensão de arquivo inválida!");
                        }
                    }
                    else
                    {
                        return Json<bool>(false, false, "Empresa não encontrada!");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Número de arquivos inválido.");
                }
            }
            catch
            {
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao salvar a logo da empresa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult DeletarLogoSistema()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    return Json<bool>(false, false, "Permissão para alteração de empresa negada!");

                int codigoEmpresa = 0;
                if (!int.TryParse(Request.Params["Codigo"], out codigoEmpresa) || codigoEmpresa <= 0)
                    return Json<bool>(false, false, "Empresa inválida.");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                if (empresa != null)
                {
                    if (!string.IsNullOrWhiteSpace(empresa.CaminhoLogoSistema) && Utilidades.IO.FileStorageService.Storage.Exists(empresa.CaminhoLogoSistema))
                        Utilidades.IO.FileStorageService.Storage.Delete(empresa.CaminhoLogoSistema);

                    empresa.CaminhoLogoSistema = string.Empty;
                    repEmpresa.Atualizar(empresa);

                    return Json<bool>(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "Empresa não encontrada!");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao remover a logo do sistema do Sistema.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ReenviarInformacoesDeLogin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoEmpresa = 0;
                if (!int.TryParse(Request.Params["Codigo"], out codigoEmpresa) || codigoEmpresa <= 0)
                    return Json<bool>(false, false, "Empresa inválida.");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                if (empresa != null)
                {
                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                    Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCPF(empresa.Codigo, "11111111111", "U");
                    if (usuario != null)
                    {
                        this.EnviarEmailDeNotificacaoDeUsuarioCadastrado(usuario, unitOfWork);
                        return Json<bool>(true, true);
                    }
                    else
                    {
                        usuario = repUsuario.BuscarPrimeiroPorEmpresa(empresa.Codigo, Dominio.Enumeradores.TipoAcesso.Emissao);
                        if (usuario != null)
                        {
                            this.EnviarEmailDeNotificacaoDeUsuarioCadastrado(usuario, unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, "Reenviou as informações de login para " + usuario.Descricao + ".", unitOfWork);
                            return Json<bool>(true, true);
                        }
                        else
                            return Json<bool>(false, false, "Nenhum usuário encontrado.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Empresa não encontrada.");
                }
            }
            catch
            {
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao salvar a logo da empresa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterEmpresasComCertificadoAVencer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                List<Dominio.Entidades.Empresa> empresas = repEmpresa.BuscarPorDataDeVencimentoDoCertificado(this.EmpresaUsuario.CNPJ == "13969629000196" ? 0 : this.EmpresaUsuario.Codigo, DateTime.Now.AddDays(11).Date);

                var retorno = (from obj in empresas
                               select new
                               {
                                   obj.CNPJ,
                                   obj.NomeFantasia,
                                   Data = obj.DataFinalCertificado.HasValue ? obj.DataFinalCertificado.Value.ToString("dd/MM/yyyy") : string.Empty,
                                   EmpresaAdmin = obj.EmpresaPai != null ? obj.EmpresaPai.NomeFantasia : string.Empty
                               }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as empresas com certificado vencido ou à vencer.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EnviarImagemCorpoEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string caminhoImagem = "";
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];

                    Servicos.Embarcador.Configuracoes.ConfigurationInstance configurationInstance = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
                    var configArquivo = configurationInstance.ObterConfiguracaoArquivo();

                    caminhoImagem = Utilidades.IO.FileStorageService.Storage.Combine(configArquivo.Anexos, "Imagem", "Envio");

                    if (Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(caminhoImagem, "ImagemTexto.jpg")))
                        Utilidades.IO.FileStorageService.Storage.Delete(Utilidades.IO.FileStorageService.Storage.Combine(caminhoImagem, "ImagemTexto.jpg"));

                    Utilidades.IO.FileStorageService.Storage.SaveStream(Utilidades.IO.FileStorageService.Storage.Combine(caminhoImagem, "ImagemTexto.jpg"), file.InputStream);
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao enviar arquivos " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EnviarEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string titulo = System.Uri.UnescapeDataString(Request.Params["Titulo"]);
                string mensagem = System.Uri.UnescapeDataString(Request.Params["Mensagem"]);
                string assinatura = System.Uri.UnescapeDataString(Request.Params["Assinatura"]);
                string temImagem = System.Uri.UnescapeDataString(Request.Params["TemImagem"]);
                string respostaPara = System.Uri.UnescapeDataString(Request.Params["RespostaPara"]);
                string emailCopia = System.Uri.UnescapeDataString(Request.Params["EmailCopia"]);
                string emailDestino = System.Uri.UnescapeDataString(Request.Params["EmailDestino"]);
                string caminhoImagem = "";
                if (temImagem == "SIM")
                {
                    caminhoImagem = Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["Anexos"], "Imagem", "Envio"); ;
                    caminhoImagem = Utilidades.IO.FileStorageService.Storage.Combine(caminhoImagem, "ImagemTexto.jpg");
                }

                if (string.IsNullOrWhiteSpace(titulo))
                    return Json<bool>(false, false, "Título não do e-mail não informada.");

                int.TryParse(Request.Params["RNTRCInvalida"], out int rntrcInvalida);

                //if (string.IsNullOrWhiteSpace(mensagem))
                //    return Json<bool>(false, false, "Mensagem não do e-mail não informada.");
                if (mensagem == "")
                    mensagem = " ";

                if (!string.IsNullOrWhiteSpace(emailDestino))
                {
                    EnviarEmailEmpresa(titulo, mensagem, emailDestino, assinatura, emailCopia, caminhoImagem, unitOfWork, respostaPara);
                    return Json<bool>(true, true, "Emails enviados com sucesso.");
                }

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                List<Dominio.Entidades.Empresa> empresas = null;
                if (rntrcInvalida == 1)
                    empresas = repEmpresa.BuscarEmpresasComANTTInvalida(0);
                else
                    empresas = repEmpresa.BuscarAtivasProducao(0);


                foreach (Dominio.Entidades.Empresa empresa in empresas)
                {
                    var tituloEmail = string.Concat(titulo, " (" + empresa.RazaoSocial + ")");
                    var emailCopiaEnvio = emailCopia;

                    if (!string.IsNullOrWhiteSpace(empresa.Email))
                    {
                        for (var i = 0; i < empresa.Email.Split(';').Count(); i++)
                        {
                            EnviarEmailEmpresa(tituloEmail, mensagem, empresa.Email.Split(';')[i], assinatura, emailCopiaEnvio, caminhoImagem, unitOfWork, respostaPara);
                            emailCopiaEnvio = "";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(empresa.EmailAdministrativo))
                    {
                        for (var i = 0; i < empresa.EmailAdministrativo.Split(';').Count(); i++)
                        {
                            EnviarEmailEmpresa(tituloEmail, mensagem, empresa.EmailAdministrativo.Split(';')[i], assinatura, emailCopiaEnvio, caminhoImagem, unitOfWork, respostaPara);
                            emailCopiaEnvio = "";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(empresa.EmailContador))
                    {
                        for (var i = 0; i < empresa.EmailContador.Split(';').Count(); i++)
                        {
                            EnviarEmailEmpresa(tituloEmail, mensagem, empresa.EmailContador.Split(';')[i], assinatura, emailCopiaEnvio, caminhoImagem, unitOfWork, respostaPara);
                            emailCopiaEnvio = "";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(emailCopiaEnvio))
                    {
                        EnviarEmailEmpresa("Transportadora " + empresa.CNPJ + " sem e-mail cadastrado", "Transportadora " + empresa.RazaoSocial + "  sem e-mail cadastrado", emailCopiaEnvio, assinatura, "", caminhoImagem, unitOfWork, respostaPara);
                    }
                }

                return Json<bool>(true, true, "Emails enviados com sucesso.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao enviar e-mails.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult AtualizarEmpresasOracle()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;

                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Servicos.AtualizacaoEmpresa svcAtualizaEmpresa = new Servicos.AtualizacaoEmpresa(unitOfWork);

                if (codigo > 1)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigo);

                    svcAtualizaEmpresa.Atualizar(empresa, unitOfWork);
                }
                else
                {
                    List<Dominio.Entidades.Empresa> listaEmpresas = repEmpresa.BuscarTodas("A");

                    //foreach (Dominio.Entidades.Empresa empresa in listaEmpresas)
                    for (var i = 0; i < listaEmpresas.Count(); i++)
                    {
                        var empresa = repEmpresa.BuscarPorCodigo(listaEmpresas[i].Codigo);

                        svcAtualizaEmpresa.Atualizar(empresa, unitOfWork);
                        //Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, "Atualizou Empresas Oracle.", unitOfWork);

                        string mensagemErro = string.Empty;
                        Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech integracaoNStech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
                        if (!integracaoNStech.IncluirAtualizarCertificado(out mensagemErro, empresa, unitOfWork))
                            Servicos.Log.TratarErro($"Ocorreu um erro ao enviar o certificado para o emissor NStech: {mensagemErro}");

                        unitOfWork.FlushAndClear();
                    }
                }

                //Utilizado para criar config para as empresas que não tinham
                //if (codigo == 1)
                //{
                //    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(Conexao.StringConexao);
                //    List<Dominio.Entidades.Empresa> listaEmpresas = repEmpresa.BuscarTodas("A");
                //    foreach (Dominio.Entidades.Empresa empresa in listaEmpresas)
                //    {
                //        if (empresa.Configuracao == null)
                //            this.ReplicarConfig(empresa, unitOfWork);
                //    }
                //}


                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar os e-mails da empresa. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterSolicitacoesEmissaoPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.SolicitacaoEmissao repSolicitacaoEmissao = new Repositorio.SolicitacaoEmissao(unitOfWork);

                string mensagemAviso = string.Empty;

                if (this.EmpresaUsuario.CNPJ == "13969629000196")
                {
                    int quantidade = repSolicitacaoEmissao.ContasSolicitacoesPendentes();
                    if (quantidade > 0)
                    {
                        if (quantidade == 1)
                            mensagemAviso = "Existe " + quantidade.ToString() + " solicitação pendente!";
                        else
                            mensagemAviso = "Existem " + quantidade.ToString() + " solicitações pendentes!";
                    }
                }

                var retorno = new
                {
                    MensagemAviso = mensagemAviso
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as solicitações pendentes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarPermissoesUsuario(Dominio.Entidades.Usuario usuario, string tipoUsuario, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.PaginaUsuario repPermissao = new Repositorio.PaginaUsuario(unidadeTrabalho);
            Repositorio.PermissaoEmpresa repPermissaoEmpresa = new Repositorio.PermissaoEmpresa(unidadeTrabalho);

            List<Dominio.Entidades.PaginaUsuario> listaPermissao = repPermissao.BuscarPorUsuario(usuario.Codigo);
            List<Dominio.Entidades.PermissaoEmpresa> listaPermissaoEmpresa = repPermissaoEmpresa.BuscarPorEmpresa(usuario.Empresa.Codigo);

            if (listaPermissao.Count == 0)
            {
                if (tipoUsuario == "S" || tipoUsuario == "") //Sistema WEB
                {
                    foreach (Dominio.Entidades.PermissaoEmpresa permissaoEmpresa in listaPermissaoEmpresa)
                    {
                        Dominio.Entidades.PaginaUsuario permissao = new Dominio.Entidades.PaginaUsuario();
                        permissao.Usuario = usuario;
                        permissao.Pagina = permissaoEmpresa.Pagina;
                        permissao.PermissaoDeAcesso = permissaoEmpresa.PermissaoDeAcesso;
                        permissao.PermissaoDeAlteracao = permissaoEmpresa.PermissaoDeAlteracao;
                        permissao.PermissaoDeDelecao = permissaoEmpresa.PermissaoDeDelecao;
                        permissao.PermissaoDeInclusao = permissaoEmpresa.PermissaoDeInclusao;
                        repPermissao.Inserir(permissao);
                    }
                }
                else
                if (tipoUsuario == "C") //Sistema Callcenter
                {
                    foreach (Dominio.Entidades.PermissaoEmpresa permissaoEmpresa in listaPermissaoEmpresa)
                    {
                        Dominio.Entidades.PaginaUsuario permissao = new Dominio.Entidades.PaginaUsuario();
                        permissao.Usuario = usuario;
                        permissao.Pagina = permissaoEmpresa.Pagina;
                        if (permissaoEmpresa.Pagina.Menu.Equals("Relatórios") && (
                             permissaoEmpresa.Pagina.Descricao.Equals("Ocorrências de CT-e") ||
                             permissaoEmpresa.Pagina.Descricao.Equals("CT-es Emitidos") ||
                             permissaoEmpresa.Pagina.Descricao.Equals("MDF-es Emitidos") ||
                             permissaoEmpresa.Pagina.Descricao.Equals("Solicitação Arquivos") ||
                             permissaoEmpresa.Pagina.Descricao.Equals("Veículos")))
                        {
                            permissao.PermissaoDeAcesso = permissaoEmpresa.PermissaoDeAcesso;
                            permissao.PermissaoDeAlteracao = "I";
                            permissao.PermissaoDeDelecao = "I";
                            permissao.PermissaoDeInclusao = "I";
                            repPermissao.Inserir(permissao);
                        }
                        else if (permissaoEmpresa.Pagina.Menu.Equals("Cadastros") && (
                                permissaoEmpresa.Pagina.Descricao.Equals("Clientes") ||
                                //permissaoEmpresa.Pagina.Descricao.Equals("Empresa Emissora") || (Solicitado pelo Cesar para tirar a permissão para alterar
                                permissaoEmpresa.Pagina.Descricao.Equals("Veículos") ||
                                permissaoEmpresa.Pagina.Descricao.Equals("Motoristas") ||
                                permissaoEmpresa.Pagina.Descricao.Equals("Observações") ||
                                permissaoEmpresa.Pagina.Descricao.Equals("Veículos Vinculados") ||
                                permissaoEmpresa.Pagina.Descricao.Equals("Regras de ICMS") ||
                                permissaoEmpresa.Pagina.Descricao.Equals("Termos De Uso") ||
                                permissaoEmpresa.Pagina.Descricao.Equals("Percursos Entre Estados") ||
                                permissaoEmpresa.Pagina.Descricao.Equals("Apólices de Seguro")))
                        {
                            permissao.PermissaoDeAcesso = permissaoEmpresa.PermissaoDeAcesso;
                            permissao.PermissaoDeAlteracao = permissaoEmpresa.PermissaoDeAcesso;
                            permissao.PermissaoDeDelecao = "I";
                            permissao.PermissaoDeInclusao = permissaoEmpresa.PermissaoDeAcesso;
                            repPermissao.Inserir(permissao);
                        }
                        else if (permissaoEmpresa.Pagina.Descricao.Equals("Veículos Vinculados"))
                        {
                            permissao.PermissaoDeAcesso = permissaoEmpresa.PermissaoDeAcesso;
                            permissao.PermissaoDeAlteracao = permissaoEmpresa.PermissaoDeAcesso;
                            permissao.PermissaoDeDelecao = permissaoEmpresa.PermissaoDeAcesso;
                            permissao.PermissaoDeInclusao = permissaoEmpresa.PermissaoDeAcesso;
                            repPermissao.Inserir(permissao);
                        }
                        else if (permissaoEmpresa.Pagina.Menu.Equals("Cadastros") && (
                                permissaoEmpresa.Pagina.Descricao.Equals("Empresa Emissora")))
                        {
                            permissao.PermissaoDeAcesso = permissaoEmpresa.PermissaoDeAcesso;
                            permissao.PermissaoDeAlteracao = "I";
                            permissao.PermissaoDeDelecao = "I";
                            permissao.PermissaoDeInclusao = "I";
                            repPermissao.Inserir(permissao);
                        }

                        else if (permissaoEmpresa.Pagina.Menu.Equals("Emissões") && (
                               permissaoEmpresa.Pagina.Descricao.Equals("Encerramento de MDF-e") ||
                               permissaoEmpresa.Pagina.Descricao.Equals("MDF-e")))
                        {
                            permissao.PermissaoDeAcesso = permissaoEmpresa.PermissaoDeAcesso;
                            permissao.PermissaoDeAlteracao = permissaoEmpresa.PermissaoDeAcesso;
                            permissao.PermissaoDeDelecao = "I";
                            permissao.PermissaoDeInclusao = permissaoEmpresa.PermissaoDeAcesso;
                            repPermissao.Inserir(permissao);
                        }
                        else if (permissaoEmpresa.Pagina.Descricao.Equals("Alterar a Senha"))
                        {
                            permissao.PermissaoDeAcesso = "A";
                            permissao.PermissaoDeAlteracao = "A";
                            permissao.PermissaoDeDelecao = "A";
                            permissao.PermissaoDeInclusao = "A";
                            repPermissao.Inserir(permissao);
                        }
                        else
                        {
                            permissao.PermissaoDeAcesso = "I";
                            permissao.PermissaoDeAlteracao = "I";
                            permissao.PermissaoDeDelecao = "I";
                            permissao.PermissaoDeInclusao = "I";
                            repPermissao.Inserir(permissao);
                        }
                    }
                }
            }
        }

        private void SalvarPermissoes(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.PaginasUsuario> listaPermissoesEmpresa = new List<Dominio.ObjetosDeValor.PaginasUsuario>();

            if (!string.IsNullOrWhiteSpace(Request.Params["Permissoes"]))
                listaPermissoesEmpresa = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.PaginasUsuario>>(Request.Params["Permissoes"]);

            if (listaPermissoesEmpresa.Count > 0)
            {
                Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);
                Repositorio.Pagina repPagina = new Repositorio.Pagina(unitOfWork);
                Repositorio.PermissaoEmpresa repPermissaoEmpresa = new Repositorio.PermissaoEmpresa(unitOfWork);
                foreach (Dominio.ObjetosDeValor.PaginasUsuario permissaoEmpresa in listaPermissoesEmpresa)
                {
                    Dominio.Entidades.PermissaoEmpresa permissao = repPermissaoEmpresa.BuscarPorPaginaEEmpresa(empresa.Codigo, permissaoEmpresa.Codigo);
                    bool inserir = false;
                    if (permissao == null)
                    {
                        permissao = new Dominio.Entidades.PermissaoEmpresa();
                        permissao.Pagina = repPagina.BuscarPorCodigo(permissaoEmpresa.Codigo);
                        permissao.Empresa = empresa;
                        inserir = true;
                    }

                    permissao.PermissaoDeAcesso = permissaoEmpresa.Acesso ? "A" : "I";
                    permissao.PermissaoDeAlteracao = permissaoEmpresa.Alterar ? "A" : "I";
                    permissao.PermissaoDeDelecao = permissaoEmpresa.Excluir ? "A" : "I";
                    permissao.PermissaoDeInclusao = permissaoEmpresa.Incluir ? "A" : "I";

                    if (inserir)
                        repPermissaoEmpresa.Inserir(permissao);
                    else
                        repPermissaoEmpresa.Atualizar(permissao);

                    if (permissao.PermissaoDeAcesso.Equals("I"))
                    {
                        List<Dominio.Entidades.PaginaUsuario> listaPaginasUsuario = repPaginaUsuario.BuscarPorEmpresaEPagina(empresa.Codigo, permissaoEmpresa.Codigo);
                        foreach (Dominio.Entidades.PaginaUsuario paginaUsuario in listaPaginasUsuario)
                        {
                            repPaginaUsuario.Deletar(paginaUsuario);
                        }
                    }
                }
            }
        }

        private void SalvarSeriePadrao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeTrabalho);
            Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();
            serie.Empresa = empresa;
            serie.Numero = 1;
            serie.Status = "A";
            serie.Tipo = Dominio.Enumeradores.TipoSerie.CTe;
            repEmpresaSerie.Inserir(serie);
        }

        private void SalvarSerieMDFePadrao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeTrabalho);
            Dominio.Entidades.EmpresaSerie serie = new Dominio.Entidades.EmpresaSerie();
            serie.Empresa = empresa;
            serie.Numero = 1;
            serie.Status = "A";
            serie.Tipo = Dominio.Enumeradores.TipoSerie.MDFe;
            repEmpresaSerie.Inserir(serie);
        }

        private void SalvarUsuarioPadrao(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);
            Dominio.Entidades.Usuario usuarioAux = repUsuario.BuscarPorLogin(empresa.CNPJ);
            if (usuarioAux == null)
            {
                Repositorio.Setor repSetor = new Repositorio.Setor(unidadeTrabalho);
                Dominio.Entidades.Usuario usuario = new Dominio.Entidades.Usuario();
                usuario.Setor = repSetor.BuscarPorCodigo(1);
                usuario.CPF = empresa.CNPJ;
                usuario.Email = "";
                usuario.Empresa = empresa;
                usuario.Endereco = empresa.Endereco;
                usuario.Localidade = empresa.Localidade;
                usuario.Login = empresa.CNPJ;
                usuario.Senha = empresa.CNPJ.Substring(0, 5);
                usuario.Status = "A";
                usuario.Tipo = "U";
                usuario.Telefone = empresa.Telefone;
                usuario.Nome = empresa.RazaoSocial;
                usuario.AlterarSenhaAcesso = false;
                usuario.PermiteAuditar = true;

                if (empresa.EmpresaPai != null)
                    usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                else if (empresa.EmpresaAdministradora != null)
                    usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Admin;

                repUsuario.Inserir(usuario);

                this.SalvarPermissoesUsuario(usuario, "", unidadeTrabalho);
            }
        }

        private void SalvarUsuarioEmpresa(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);
            Repositorio.Setor repSetor = new Repositorio.Setor(unidadeTrabalho);

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCPF(empresa.Codigo, "11111111111", "U");

            if (usuario == null)
            {
                usuario = new Dominio.Entidades.Usuario();
                usuario.Setor = repSetor.BuscarPorCodigo(1);
                usuario.CPF = "11111111111";
                usuario.Email = "";
                usuario.Empresa = empresa;
                usuario.Endereco = empresa.Endereco;
                usuario.Localidade = empresa.Localidade;
                usuario.Login = (empresa.CNPJ.Length > 5 ? empresa.CNPJ.Substring(0, 5) : empresa.CodigoIntegracao) + "-" + empresa.Localidade.Estado.Sigla;
                usuario.Senha = (empresa.CNPJ.Length > 5 ? empresa.CNPJ.Substring(0, 5) : empresa.CodigoIntegracao);
                usuario.Status = "A";
                usuario.Tipo = "U";
                usuario.Telefone = empresa.Telefone;
                usuario.Nome = empresa.RazaoSocial;
                usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                usuario.Email = empresa.Email;
                usuario.AlterarSenhaAcesso = true;

                repUsuario.Inserir(usuario);

                this.SalvarPermissoesUsuario(usuario, empresa.StatusEmissao, unidadeTrabalho);

                if (empresa.StatusEmail == "A" && empresa.Email != "")
                    this.EnviarEmailDeNotificacaoDeUsuarioCadastrado(usuario, unidadeTrabalho);
            }
        }

        private void ReplicarDadosPadroes(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            this.ReplicarPlanosDeContas(empresa, unidadeTrabalho);
            this.ReplicarEixosDeVeiculos(empresa, unidadeTrabalho);
            this.ReplicarAliquotasDeICMS(empresa, unidadeTrabalho);
            this.ReplicarStatusDePneus(empresa, unidadeTrabalho);
            this.ReplicarTiposDeVeiculos(empresa, unidadeTrabalho);
            this.RepilcicarImpostoContratoFrete(empresa, unidadeTrabalho);
        }

        private void ReplicarAliquotasDeICMS(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.AliquotaDeICMS repAliquota = new Repositorio.AliquotaDeICMS(unidadeTrabalho);
            if (repAliquota.ContarPorEmpresa(empresa.Codigo) <= 0)
            {
                List<Dominio.Entidades.AliquotaDeICMS> aliquotas = repAliquota.BuscarPorEmpresa(empresa.EmpresaPai.Codigo);
                foreach (Dominio.Entidades.AliquotaDeICMS aliquota in aliquotas)
                {
                    Dominio.Entidades.AliquotaDeICMS aliquotaNova = new Dominio.Entidades.AliquotaDeICMS();

                    aliquotaNova.Aliquota = aliquota.Aliquota;
                    aliquotaNova.Empresa = empresa;
                    aliquotaNova.Status = aliquota.Status;

                    repAliquota.Inserir(aliquotaNova);
                }
            }
        }

        private void ReplicarStatusDePneus(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.StatusPneu repStatusPneu = new Repositorio.StatusPneu(unidadeTrabalho);

            if (repStatusPneu.ContarPorEmpresa(empresa.Codigo) <= 0)
            {
                List<Dominio.Entidades.StatusPneu> statusDePneus = repStatusPneu.BuscarPorEmpresa(empresa.EmpresaPai.Codigo);

                foreach (Dominio.Entidades.StatusPneu statusDePneu in statusDePneus)
                {
                    Dominio.Entidades.StatusPneu statusNovo = new Dominio.Entidades.StatusPneu();

                    statusNovo.Data = DateTime.Now;
                    statusNovo.Descricao = statusDePneu.Descricao;
                    statusNovo.Empresa = empresa;
                    statusNovo.Status = statusDePneu.Status;
                    statusNovo.Tipo = statusDePneu.Tipo;

                    repStatusPneu.Inserir(statusNovo);
                }
            }
        }

        private void ReplicarPlanosDeContas(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.PlanoDeConta repPlanosDeContas = new Repositorio.PlanoDeConta(unidadeTrabalho);
            if (repPlanosDeContas.ContarPorEmpresa(empresa.Codigo) <= 0)
            {
                List<Dominio.Entidades.PlanoDeConta> planosDeContas = repPlanosDeContas.BuscarPorEmpresa(empresa.EmpresaPai.Codigo);
                foreach (Dominio.Entidades.PlanoDeConta plano in planosDeContas)
                {
                    Dominio.Entidades.PlanoDeConta planoNovo = new Dominio.Entidades.PlanoDeConta();
                    planoNovo.Conta = plano.Conta;
                    planoNovo.ContaContabil = plano.ContaContabil;
                    planoNovo.Descricao = plano.Descricao;
                    planoNovo.Empresa = empresa;
                    planoNovo.Status = plano.Status;
                    planoNovo.Tipo = plano.Tipo;
                    planoNovo.TipoDeConta = plano.TipoDeConta;
                    repPlanosDeContas.Inserir(planoNovo);
                }
            }
        }

        private void ReplicarTiposDeVeiculos(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.TipoVeiculo repTipoVeiculo = new Repositorio.TipoVeiculo(unidadeTrabalho);
            if (repTipoVeiculo.ContarPorEmpresa(empresa.Codigo) <= 0)
            {
                List<Dominio.Entidades.TipoVeiculo> listaTipoVeiculo = repTipoVeiculo.BuscarPorEmpresa(empresa.EmpresaPai.Codigo);
                foreach (Dominio.Entidades.TipoVeiculo tipo in listaTipoVeiculo)
                {
                    Dominio.Entidades.TipoVeiculo tipoVeiculo = new Dominio.Entidades.TipoVeiculo();
                    tipoVeiculo.Empresa = empresa;
                    tipoVeiculo.CodigoIntegracao = tipo.CodigoIntegracao;
                    tipoVeiculo.Descricao = tipo.Descricao;
                    //tipoVeiculo.EixosDoVeiculo = tipo.EixosDoVeiculo;
                    tipoVeiculo.NumeroEixos = tipo.NumeroEixos;
                    tipoVeiculo.PesoBruto = tipo.PesoBruto;
                    tipoVeiculo.PesoLiquido = tipo.PesoLiquido;
                    tipoVeiculo.Status = tipo.Status;
                    repTipoVeiculo.Inserir(tipoVeiculo);
                }
            }
        }

        private void ReplicarEixosDeVeiculos(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.EixoVeiculo repEixoDeVeiculo = new Repositorio.EixoVeiculo(unidadeTrabalho);
            if (repEixoDeVeiculo.ContarPorEmpresa(empresa.Codigo) <= 0)
            {
                List<Dominio.Entidades.EixoVeiculo> listaEixos = repEixoDeVeiculo.BuscarPorEmpresa(empresa.EmpresaPai.Codigo);
                foreach (Dominio.Entidades.EixoVeiculo eixo in listaEixos)
                {
                    Dominio.Entidades.EixoVeiculo eixoNovo = new Dominio.Entidades.EixoVeiculo();
                    eixoNovo.Descricao = eixo.Descricao;
                    eixoNovo.Dianteiro = eixo.Dianteiro;
                    eixoNovo.Empresa = empresa;
                    eixoNovo.Interno_Externo = eixo.Interno_Externo;
                    eixoNovo.OrdemEixo = eixo.OrdemEixo;
                    eixoNovo.Posicao = eixo.Posicao;
                    eixoNovo.Status = eixo.Status;
                    eixoNovo.Tipo = eixo.Tipo;
                    repEixoDeVeiculo.Inserir(eixoNovo);
                }
            }
        }

        private void ReplicarConfig(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeTrabalho);

            if (empresa.EmpresaPai != null)
            {
                Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarPorCodigo(empresa.EmpresaPai.Codigo);

                bool inserir = false;

                if (empresaPai != null && empresaPai.Configuracao != null)
                {
                    if (empresa.Configuracao == null)
                    {
                        inserir = true;
                        empresa.Configuracao = new Dominio.Entidades.ConfiguracaoEmpresa();
                    }

                    empresa.Configuracao.DiasParaEmissaoDeCTeAnulacao = empresa.EmpresaPai.Configuracao.DiasParaEmissaoDeCTeAnulacao;
                    empresa.Configuracao.DiasParaEmissaoDeCTeComplementar = empresa.EmpresaPai.Configuracao.DiasParaEmissaoDeCTeComplementar;
                    empresa.Configuracao.DiasParaEmissaoDeCTeSubstituicao = empresa.EmpresaPai.Configuracao.DiasParaEmissaoDeCTeSubstituicao;
                    empresa.Configuracao.DiasParaEntrega = empresa.EmpresaPai.Configuracao.DiasParaEmissaoDeCTeSubstituicao;
                    empresa.Configuracao.PrazoCancelamentoCTe = empresa.EmpresaPai.Configuracao.PrazoCancelamentoCTe;
                    empresa.Configuracao.PrazoCancelamentoMDFe = empresa.EmpresaPai.Configuracao.PrazoCancelamentoMDFe;
                    empresa.Configuracao.ProdutoPredominante = empresa.EmpresaPai.Configuracao.ProdutoPredominante;
                    empresa.Configuracao.OutrasCaracteristicas = empresa.EmpresaPai.Configuracao.OutrasCaracteristicas;
                    empresa.Configuracao.ResponsavelSeguro = empresa.EmpresaPai.Configuracao.ResponsavelSeguro;
                    empresa.Configuracao.TipoImpressao = empresa.EmpresaPai.Configuracao.TipoImpressao;
                    if (empresa.SerieCTeFora != null && empresa.SerieCTeFora > 0)
                        empresa.Configuracao.SerieInterestadual = repSerie.BuscarPorSerie(empresa.Codigo, empresa.SerieCTeFora, Dominio.Enumeradores.TipoSerie.CTe);
                    if (empresa.SerieCTeDentro != null && empresa.SerieCTeDentro > 0)
                        empresa.Configuracao.SerieIntraestadual = repSerie.BuscarPorSerie(empresa.Codigo, empresa.SerieCTeDentro, Dominio.Enumeradores.TipoSerie.CTe);
                    if (empresa.SerieMDFe != null && empresa.SerieMDFe > 0)
                        empresa.Configuracao.SerieMDFe = repSerie.BuscarPorSerie(empresa.Codigo, empresa.SerieMDFe, Dominio.Enumeradores.TipoSerie.MDFe);

                    empresa.Configuracao.ExibirHomeVencimentoCertificado = true;
                    empresa.Configuracao.ExibirHomePendenciasEntrega = true;
                    empresa.Configuracao.ExibirHomeGraficosEmissoes = true;
                    empresa.Configuracao.ExibirHomeServicosVeiculos = false;
                    empresa.Configuracao.ExibirHomeParcelaDuplicatas = false;
                    empresa.Configuracao.ExibirHomePagamentosMotoristas = false;
                    empresa.Configuracao.ExibirHomeAcertoViagem = false;
                    empresa.Configuracao.GeraDuplicatasAutomaticamente = empresa.EmpresaPai.Configuracao.GeraDuplicatasAutomaticamente;
                    empresa.Configuracao.BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca = empresa.EmpresaPai.Configuracao.BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca;

                    if (inserir)
                        repEmpresa.Inserir(empresa);
                    else
                        repEmpresa.Atualizar(empresa);
                }
            }
        }

        private void RepilcicarImpostoContratoFrete(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (empresa.EmpresaPai != null)
            {
                Repositorio.ImpostoContratoFrete repImpostoContratoFrete = new Repositorio.ImpostoContratoFrete(unidadeTrabalho);
                Repositorio.INSSImpostoContratoFrete repINSS = new Repositorio.INSSImpostoContratoFrete(unidadeTrabalho);
                Repositorio.IRImpostoContratoFrete repIR = new Repositorio.IRImpostoContratoFrete(unidadeTrabalho);

                Dominio.Entidades.ImpostoContratoFrete impostoContratoFretePai = repImpostoContratoFrete.BuscarPorEmpresa(empresa.EmpresaPai.Codigo);

                if (impostoContratoFretePai != null)
                {
                    Dominio.Entidades.ImpostoContratoFrete impostoContratoFrete = new Dominio.Entidades.ImpostoContratoFrete();
                    impostoContratoFrete.Empresa = empresa;
                    impostoContratoFrete.AliquotaSENAT = impostoContratoFretePai.AliquotaSENAT;
                    impostoContratoFrete.AliquotaSEST = impostoContratoFretePai.AliquotaSEST;
                    impostoContratoFrete.PercentualBCINSS = impostoContratoFretePai.PercentualBCINSS;
                    impostoContratoFrete.PercentualBCIR = impostoContratoFretePai.PercentualBCIR;
                    impostoContratoFrete.ValorTetoRetencaoINSS = impostoContratoFretePai.ValorTetoRetencaoINSS;
                    repImpostoContratoFrete.Inserir(impostoContratoFrete);

                    List<Dominio.Entidades.INSSImpostoContratoFrete> listaINSSPai = repINSS.BuscarPorImposto(impostoContratoFretePai.Codigo);
                    foreach (Dominio.Entidades.INSSImpostoContratoFrete inssPai in listaINSSPai)
                    {
                        Dominio.Entidades.INSSImpostoContratoFrete inss = new Dominio.Entidades.INSSImpostoContratoFrete();
                        inss.Imposto = impostoContratoFrete;
                        inss.PercentualAplicar = inssPai.PercentualAplicar;
                        inss.ValorFinal = inssPai.ValorFinal;
                        inss.ValorInicial = inss.ValorInicial;
                        repINSS.Inserir(inss);
                    }

                    List<Dominio.Entidades.IRImpostoContratoFrete> listaIRPai = repIR.BuscarPorImposto(impostoContratoFretePai.Codigo);
                    foreach (Dominio.Entidades.IRImpostoContratoFrete irPai in listaIRPai)
                    {
                        Dominio.Entidades.IRImpostoContratoFrete ir = new Dominio.Entidades.IRImpostoContratoFrete();
                        ir.Imposto = impostoContratoFrete;
                        ir.PercentualAplicar = irPai.PercentualAplicar;
                        ir.ValorFinal = irPai.ValorFinal;
                        ir.ValorInicial = irPai.ValorInicial;
                        ir.ValorDeduzir = irPai.ValorDeduzir;
                        repIR.Inserir(ir);
                    }
                }
            }
        }

        private void EnviarEmailDeNotificacaoDeBloqueio(int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (empresa != null)
            {
                Servicos.Email svcEmail = new Servicos.Email(unidadeTrabalho);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                sb.Append("<p>Prezado cliente ").Append(empresa.RazaoSocial).Append(" ").Append(empresa.CNPJ_Formatado).Append("<br /><br />");
                sb.Append("Sua empresa foi bloqueda por pendências, entre em contato para regularização.<br /><br /></p>");

                System.Text.StringBuilder ss = new System.Text.StringBuilder();
                ss.Append("Att.<br />");
                ss.Append("Fone: " + empresa.EmpresaPai.Telefone + " <br />");
                ss.Append("E-mail: " + empresa.EmpresaPai.Email);

#if DEBUG
#else
                svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, empresa.Email.Split(';')[0], "", "", "MultiCTe - Empresa " + empresa.RazaoSocial + " bloqueada.", sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 0, unidadeTrabalho);
#endif
            }
        }

        private void EnviarEmailDeNotificacaoDeUsuarioCadastrado(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Email svcEmail = new Servicos.Email(unitOfWork);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<p>Prezado cliente ").Append(usuario.Empresa.RazaoSocial).Append("<br /><br />");
            sb.Append("Obrigado por aderir o sistema MultiCTe, em seguida nossa equipe irá entrar em contato. Caso de duvidas pode entrar em contato pelos telefones da assinatura.<br /><br />");

            sb.Append("<p>Seus dados para acesso ao sistema MultiCTe são:<br /><br />");
            sb.Append("Usuário: ").Append(usuario.Login).Append("<br />");
            sb.Append("Senha: ").Append(usuario.Senha).Append("</p><br />");

            if (usuario.Empresa.EmpresaPai != null && !string.IsNullOrWhiteSpace(usuario.Empresa.EmpresaPai.URLSistema))
                sb.Append("Para utilizar o sistema MultiCTe acesse ").Append(usuario.Empresa.EmpresaPai.URLSistema).Append(".");
            else
                sb.Append("Para utilizar o sistema MultiCTe acesse http://www.multicte.com.br/ e utilize a opção de Login.");

            System.Text.StringBuilder ss = new System.Text.StringBuilder();
            ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");
            ss.Append("Fone/Fax: (49)3025-9500 <br />");
            ss.Append("Cel.: (49)9999-8880(TIM) <br />");
            ss.Append("E-mail: cte@multisoftware.com.br");

            svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, usuario.Empresa.Email.Split(';')[0], "", "", "MultiCTe - Dados para Acesso ao Sistema", sb.ToString(), string.Empty, null, ss.ToString(), true, "cte@multisoftware.com.br", 587, unitOfWork);
        }

        private void EnviarEmailEmpresa(string titulo, string mensagem, string email, string assinatura, string emailCopia, string caminhoImagem, Repositorio.UnitOfWork unitOfWork, string respostaPara = "cte@multisoftware.com.br")
        {
            Servicos.Email svcEmail = new Servicos.Email(unitOfWork);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append(mensagem);

            System.Text.StringBuilder ss = new System.Text.StringBuilder();
            ss.Append(assinatura);

            if (!string.IsNullOrWhiteSpace(caminhoImagem))
                svcEmail.EnviarEmailImagemTexto(string.Empty, string.Empty, string.Empty, email, "", emailCopia, titulo, sb.ToString(), string.Empty, caminhoImagem, ss.ToString(), false, respostaPara, 0, unitOfWork);
            else
                svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, email, "", emailCopia, titulo, sb.ToString(), string.Empty, null, ss.ToString(), false, respostaPara, 0, unitOfWork);
        }

        private string BuscaVersaoMDFe()
        {
            if (this.EmpresaUsuario.Configuracao != null && !string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.VersaoMDFe))
                return this.EmpresaUsuario.Configuracao.VersaoMDFe;
            else if (this.EmpresaUsuario.EmpresaPai != null && this.EmpresaUsuario.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(this.EmpresaUsuario.EmpresaPai.Configuracao.VersaoMDFe))
                return this.EmpresaUsuario.EmpresaPai.Configuracao.VersaoMDFe;
            else
                return "3.00";
        }
        private string BuscaVersaoCTe()
        {
            if (this.EmpresaUsuario.Configuracao != null && !string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.VersaoCTe))
                return this.EmpresaUsuario.Configuracao.VersaoCTe;
            else if (this.EmpresaUsuario.EmpresaPai != null && this.EmpresaUsuario.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(this.EmpresaUsuario.EmpresaPai.Configuracao.VersaoCTe))
                return this.EmpresaUsuario.EmpresaPai.Configuracao.VersaoCTe;
            else
                return "4.00";
        }
        #endregion
    }
}

