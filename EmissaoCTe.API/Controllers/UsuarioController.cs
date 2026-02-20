using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using Dominio.Enumeradores;

namespace EmissaoCTe.API.Controllers
{
    public class UsuarioController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("usuarios.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult ConsultarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string nome = Request.Params["Nome"];
                string status = Request.Params["Status"];

                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaUsuario filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaUsuario()
                {
                    Nome = nome,
                    Status = status,
                    CodigoEmpresa = this.EmpresaUsuario.Codigo,
                    IgnorarSituacaoMotorista = true,
                    TipoAcesso = this.Usuario.TipoAcesso
                };

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                List<Dominio.Entidades.Usuario> listaUsuarios = repUsuario.Consultar(filtrosPesquisa, inicioRegistros, 50);
                int countUsuarios = repUsuario.ContarConsulta(filtrosPesquisa);

                var retorno = from obj in listaUsuarios select new { obj.Codigo, obj.Nome, CPFCNPJ = obj.CPF, RGIE = obj.RG, obj.Email };
                return Json(retorno, true, null, new string[] { "Codigo", "Nome|40", "CPF/CNPJ|15", "RG/IE|15", "E-mail|20" }, countUsuarios);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os usuários.");
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
                string login = Request.Params["Login"];

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                List<Dominio.Entidades.Usuario> listaUsuarios = repUsuario.ConsultaUsuarios(this.EmpresaUsuario.Codigo, nome, login, this.Usuario.TipoAcesso, inicioRegistros, 50);
                int countUsuarios = repUsuario.ContaConsultaUsuarios(this.EmpresaUsuario.Codigo, nome, login, this.Usuario.TipoAcesso);

                var retorno = from obj in listaUsuarios select new { obj.Codigo, obj.Nome, obj.Login, CPFCNPJ = obj.CPF, obj.Email };
                return Json(retorno, true, null, new string[] { "Codigo", "Nome|40", "Login|15", "CPF/CNPJ|15", "E-mail|20" }, countUsuarios);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os usuários.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarPorEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoEmpresa, inicioRegistros = 0;

                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                if (!int.TryParse(Servicos.Criptografia.Descriptografar(Request.Params["CodigoEmpresa"], "CT3##MULT1@#$S0FTW4R3"), out codigoEmpresa))
                    return Json<bool>(false, false, "Código da empresa inválido.");

                string nome = Request.Params["Nome"];
                string cpfCnpj = Request.Params["CPFCNPJ"];
                string login = Request.Params["Login"];

                Dominio.Enumeradores.TipoAcesso tipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                Enum.TryParse<Dominio.Enumeradores.TipoAcesso>(Request.Params["TipoAcesso"], out tipoAcesso);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                IList<Dominio.Entidades.Usuario> listaUsuarios = repUsuario.ConsultarUsuarios(codigoEmpresa, nome, tipoAcesso, inicioRegistros, 50, cpfCnpj, login);

                int countUsuarios = repUsuario.ContarConsultaUsuarios(codigoEmpresa, nome, tipoAcesso, cpfCnpj, login);

                var retorno = from obj in listaUsuarios select new { obj.Codigo, obj.Nome, CPFCNPJ = obj.CPF, RGIE = obj.RG, obj.Email };
                return Json(retorno, true, null, new string[] { "Codigo", "Nome|40", "CPF/CNPJ|15", "RG/IE|15", "E-mail|20" }, countUsuarios);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os usuários.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarAdministradores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin)
                {
                    int inicioRegistros = 0;
                    int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                    string nome = Request.Params["Nome"];
                    string cpfCnpj = Request.Params["CPFCNPJ"];

                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                    IList<Dominio.Entidades.Usuario> listaUsuarios = repUsuario.ConsultarUsuarios(this.EmpresaUsuario.Codigo, nome, Dominio.Enumeradores.TipoAcesso.Admin, inicioRegistros, 50, cpfCnpj);
                    int countUsuarios = repUsuario.ContarConsultaUsuarios(this.EmpresaUsuario.Codigo, nome, Dominio.Enumeradores.TipoAcesso.Admin, cpfCnpj);

                    var retorno = from obj in listaUsuarios select new { obj.Codigo, obj.Nome, CPFCNPJ = obj.CPF, RGIE = obj.RG, obj.Email };
                    return Json(retorno, true, null, new string[] { "Codigo", "Nome|40", "CPF/CNPJ|15", "RG/IE|15", "E-mail|20" }, countUsuarios);
                }
                else
                {
                    return Json<bool>(false, false, "Acesso negado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os usuários.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarAdministradoresAtivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin)
                {
                    int inicioRegistros = 0;
                    int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                    string nome = Request.Params["Nome"];
                    string cpfCnpj = Request.Params["CPFCNPJ"];

                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                    IList<Dominio.Entidades.Usuario> listaUsuarios = repUsuario.ConsultarUsuariosAtivos(this.EmpresaUsuario.Codigo, nome, Dominio.Enumeradores.TipoAcesso.Admin, inicioRegistros, 50, cpfCnpj);
                    int countUsuarios = repUsuario.ContarConsultaUsuariosAtivos(this.EmpresaUsuario.Codigo, nome, Dominio.Enumeradores.TipoAcesso.Admin, cpfCnpj);

                    var retorno = from obj in listaUsuarios select new { obj.Codigo, obj.Nome, CPFCNPJ = obj.CPF, RGIE = obj.RG, obj.Email };
                    return Json(retorno, true, null, new string[] { "Codigo", "Nome|40", "CPF/CNPJ|15", "RG/IE|15", "E-mail|20" }, countUsuarios);
                }
                else
                {
                    return Json<bool>(false, false, "Acesso negado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os usuários.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                unidadeDeTrabalho.Start();

                DateTime dataNascimento = new DateTime();
                DateTime dataAdmissao = new DateTime();
                decimal salario;
                int localidade, perfil, codigo = 0;
                Dominio.Enumeradores.TipoAcesso tipoAcesso;
                Enum.TryParse<Dominio.Enumeradores.TipoAcesso>(Request.Params["TipoAcesso"], out tipoAcesso);
                string status = Request.Params["Status"];
                string nome = Request.Params["Nome"];
                string cpfCnpj = Request.Params["CPFCNPJ"];
                string rgIE = Request.Params["RGIE"];
                string telefone = Request.Params["Telefone"];
                string endereco = Request.Params["Endereco"];
                string complemento = Request.Params["Complemento"];
                string email = Request.Params["Email"];
                string usuario = Request.Params["Usuario"];
                string senha = Request.Params["Senha"];
                string confirmacaoSenha = Request.Params["ConfirmacaoSenha"];
                string cnpjEmbarcador = Request.Params["CNPJEmbarcador"];

                bool alterarSenhaAcesso;
                bool.TryParse(Request.Params["AlterarSenhaAcesso"], out alterarSenhaAcesso);
                bool.TryParse(Request.Params["Callcenter"], out bool callCenter);

                int.TryParse(Request.Params["Codigo"], out codigo);

                if (string.IsNullOrWhiteSpace(nome))
                    return Json<bool>(false, false, "Nome inválido!");

                if (string.IsNullOrWhiteSpace(cpfCnpj) || (cpfCnpj.Length != 11 && cpfCnpj.Length != 14))
                    return Json<bool>(false, false, "CPF/CNPJ inválido!");

                if (!decimal.TryParse(Request.Params["Salario"], out salario))
                    return Json<bool>(false, false, "Salário inválido!");

                if (!int.TryParse(Request.Params["Localidade"], out localidade))
                    return Json<bool>(false, false, "Município inválido!");

                if (string.IsNullOrWhiteSpace(email))
                    return Json<bool>(false, false, "E-mail inválido!");

                if (string.IsNullOrWhiteSpace(usuario) || usuario.Length < 5)
                    return Json<bool>(false, false, "Usuário inválido! Deve conter no mínimo cinco caracteres.");

                if (string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(confirmacaoSenha) || senha.Length < 5 || (senha != confirmacaoSenha))
                    return Json<bool>(false, false, "Senha inválida! Deve conter no mínimo 5 caracteres e coincidir com a confirmação de senha.");

                int.TryParse(Request.Params["CodigoPerfil"], out perfil);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Usuario usuarioAux = repUsuario.BuscarPorLogin(usuario, tipoAcesso);

                if (usuarioAux != null)
                    if (usuarioAux.Codigo != codigo)
                        return Json<bool>(false, false, "Já existe um usuário registrado com este login.");

                usuarioAux = null;

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.Setor repSetor = new Repositorio.Setor(unidadeDeTrabalho);
                Repositorio.PerfilPermissao repPerfilPermissao = new Repositorio.PerfilPermissao(unidadeDeTrabalho);

                Dominio.Entidades.Usuario user = new Dominio.Entidades.Usuario();
                Dominio.Entidades.PerfilPermissao perfilPermissao = repPerfilPermissao.BuscarPorCodigo(perfil);
                if (perfilPermissao == null && configuracaoEmbarcador.ExigePerfilUsuario)
                    return Json<bool>(false, false, "É obrigatório informar um perfil de usuário.");

                bool inserir = false;

                user = repUsuario.BuscarPorPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigo);
                if (user == null)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de usuário negada!");
                    user = new Dominio.Entidades.Usuario();
                    user.Empresa = this.EmpresaUsuario;
                    user.Setor = repSetor.BuscarPorCodigo(1);
                    user.CPF = cpfCnpj;
                    user.Tipo = "U";
                    user.Status = "A";
                    user.TipoAcesso = tipoAcesso;
                    inserir = true;
                }
                else
                {
                    user.Initialize();
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de usuário negada!");

                    if (user.TipoAcesso != tipoAcesso)
                        return Json<bool>(false, false, "O tipo de acesso do usuário existente é diferente do solicitado.");
                }
                user.Complemento = complemento;

                if (!string.IsNullOrWhiteSpace(Request.Params["DataAdmissao"]))
                    if (!DateTime.TryParseExact(Request.Params["DataAdmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAdmissao))
                        return Json<bool>(false, false, "Data de admissão inválida!");
                    else
                        user.DataAdmissao = dataAdmissao;
                else
                    user.DataAdmissao = null;

                if (!string.IsNullOrWhiteSpace(Request.Params["DataNascimento"]))
                    if (!DateTime.TryParseExact(Request.Params["DataNascimento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataNascimento))
                        return Json<bool>(false, false, "Data de nascimento inválida!");
                    else
                        user.DataNascimento = dataNascimento;
                else
                    user.DataNascimento = null;

                user.Email = email;
                user.Endereco = endereco;
                user.Empresa = this.EmpresaUsuario;
                user.Localidade = repLocalidade.BuscarPorCodigo(localidade);

                if (user.Localidade == null)
                    return Json<bool>(false, false, "Município não encontrado!");

                user.Login = usuario;
                user.Nome = nome;
                user.RG = rgIE;
                user.Salario = salario;
                user.Senha = senha;
                user.Telefone = telefone;
                user.AlterarSenhaAcesso = alterarSenhaAcesso;
                user.PerfilPermissao = perfilPermissao;
                user.CNPJEmbarcador = cnpjEmbarcador;
                if (tipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin)
                    user.Callcenter = callCenter;

                if (this.Permissao() != null || this.Permissao().PermissaoDeDelecao == "A")
                    user.Status = status;

                this.AtualizarSeries(user, unidadeDeTrabalho);

                if (inserir)
                    repUsuario.Inserir(user, Auditado);
                else
                    repUsuario.Atualizar(user, Auditado);

                this.AtualizarPermissoes(user, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o usuário.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarPorEmpresa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                unidadeDeTrabalho.Start();

                if (this.Usuario.TipoAcesso != Dominio.Enumeradores.TipoAcesso.Admin && this.Usuario.TipoAcesso != Dominio.Enumeradores.TipoAcesso.AdminInterno)
                    return Json<bool>(false, false, "Permissão negada.");

                int codigoEmpresa, localidade, codigoUsuario = 0;
                int.TryParse(Request.Params["Codigo"], out codigoUsuario);

                decimal salario = 0m;

                DateTime dataNascimento = new DateTime();
                DateTime dataAdmissao = new DateTime();

                string status = Request.Params["Status"];
                string nome = Request.Params["Nome"];
                string cpfCnpj = Request.Params["CPFCNPJ"];
                string rgIE = Request.Params["RGIE"];
                string telefone = Request.Params["Telefone"];
                string endereco = Request.Params["Endereco"];
                string complemento = Request.Params["Complemento"];
                string email = Request.Params["Email"];
                string usuario = Request.Params["Usuario"];
                string senha = Request.Params["Senha"];
                string confirmacaoSenha = Request.Params["ConfirmacaoSenha"];

                bool alterarSenhaAcesso;
                bool.TryParse(Request.Params["AlterarSenhaAcesso"], out alterarSenhaAcesso);

                if (!int.TryParse(Servicos.Criptografia.Descriptografar(HttpUtility.UrlDecode(Request.Params["CodigoEmpresa"]), "CT3##MULT1@#$S0FTW4R3"), out codigoEmpresa))
                    return Json<bool>(false, false, "Código da empresa inválido.");

                if (string.IsNullOrWhiteSpace(nome))
                    return Json<bool>(false, false, "Nome inválido!");

                if (string.IsNullOrWhiteSpace(cpfCnpj) || (cpfCnpj.Length != 11 && cpfCnpj.Length != 14))
                    return Json<bool>(false, false, "CPF/CNPJ inválido!");

                if (!decimal.TryParse(Request.Params["Salario"], out salario))
                    return Json<bool>(false, false, "Salário inválido!");

                if (!int.TryParse(Request.Params["Localidade"], out localidade))
                    return Json<bool>(false, false, "Município inválido!");

                if (string.IsNullOrWhiteSpace(email))
                    return Json<bool>(false, false, "E-mail inválido!");

                if (string.IsNullOrWhiteSpace(usuario) || usuario.Length < 5)
                    return Json<bool>(false, false, "Usuário inválido! Deve conter no mínimo cinco caracteres.");

                if (string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(confirmacaoSenha) || senha.Length < 5 || (senha != confirmacaoSenha))
                    return Json<bool>(false, false, "Senha inválida! Deve conter no mínimo 5 caracteres e coincidir com a confirmação de senha.");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Dominio.Entidades.Usuario usuarioAux = repUsuario.BuscarPorLoginETipo(usuario, Dominio.Enumeradores.TipoAcesso.Emissao);

                if (usuarioAux != null)
                    if (usuarioAux.Codigo != codigoUsuario)
                        return Json<bool>(false, false, "Já existe um usuário registrado com este login.");

                usuarioAux = null;

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.Setor repSetor = new Repositorio.Setor(unidadeDeTrabalho);
                Dominio.Entidades.Usuario user = new Dominio.Entidades.Usuario();

                bool inserir = false;

                user = repUsuario.BuscarPorPorCodigoEEmpresa(codigoEmpresa, codigoUsuario);

                if (user == null)
                {
                    user = new Dominio.Entidades.Usuario();
                    user.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    user.Setor = repSetor.BuscarPorCodigo(1);
                    user.CPF = cpfCnpj;
                    user.Tipo = "U";
                    user.Status = "A";
                    inserir = true;
                }

                user.Complemento = complemento;

                if (!string.IsNullOrWhiteSpace(Request.Params["DataAdmissao"]))
                    if (!DateTime.TryParseExact(Request.Params["DataAdmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAdmissao))
                        return Json<bool>(false, false, "Data de admissão inválida!");
                    else
                        user.DataAdmissao = dataAdmissao;
                else
                    user.DataAdmissao = null;

                if (!string.IsNullOrWhiteSpace(Request.Params["DataNascimento"]))
                    if (!DateTime.TryParseExact(Request.Params["DataNascimento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataNascimento))
                        return Json<bool>(false, false, "Data de nascimento inválida!");
                    else
                        user.DataNascimento = dataNascimento;
                else
                    user.DataNascimento = null;

                user.Email = email;
                user.Endereco = endereco;
                user.Localidade = repLocalidade.BuscarPorCodigo(localidade);

                if (user.Localidade == null)
                    return Json<bool>(false, false, "Município não encontrado!");

                user.Login = usuario;
                user.Nome = nome;
                user.RG = rgIE;
                user.Salario = salario;
                user.Senha = senha;
                user.Telefone = telefone;
                user.Status = status;
                user.AlterarSenhaAcesso = alterarSenhaAcesso;

                if (inserir)
                {
                    if (user.Empresa.EmpresaAdministradora != null)
                        user.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Admin;
                    else if (user.Empresa.EmpresaPai != null)
                        user.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                }

                this.AtualizarSeries(user, unidadeDeTrabalho);

                if (inserir)
                    repUsuario.Inserir(user);
                else
                    repUsuario.Atualizar(user);

                this.AtualizarPermissoes(user, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o usuário.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoUsuario = 0;
                int.TryParse(Request.Params["CodigoUsuario"], out codigoUsuario);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigoUsuario);

                if (usuario != null)
                {
                    var retorno = new
                    {
                        usuario.Codigo,
                        usuario.Complemento,
                        CPFCNPJ = usuario.CPF,
                        DataAdmissao = usuario.DataAdmissao != null ? usuario.DataAdmissao.Value.ToString("dd/MM/yyyy") : "",
                        DataNascimento = usuario.DataNascimento != null ? usuario.DataNascimento.Value.ToString("dd/MM/yyyy") : "",
                        usuario.Email,
                        usuario.Endereco,
                        Localidade = usuario.Localidade.Codigo,
                        SiglaUF = usuario.Localidade.Estado.Sigla,
                        Usuario = usuario.Login,
                        usuario.Nome,
                        RGIE = usuario.RG,
                        Salario = string.Format("{0:n2}", usuario.Salario),
                        usuario.Senha,
                        usuario.Telefone,
                        usuario.Status,
                        AlterarSenhaAcesso = usuario.AlterarSenhaAcesso,
                        Permissoes = this.BuscarPermissoes(usuario, unitOfWork),
                        Series = (from obj in usuario.Series where obj.Status.Equals("A") select new { obj.Codigo, obj.Numero, obj.DescricaoTipo, obj.Tipo }).ToList(),
                        CodigoPerfil = usuario.PerfilPermissao != null ? usuario.PerfilPermissao.Codigo : 0,
                        DescricaoPerfil = usuario.PerfilPermissao != null ? usuario.PerfilPermissao.Descricao : string.Empty,
                        usuario.CNPJEmbarcador,
                        usuario.Callcenter
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Usuário não encontrado!");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do usuário.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesPorEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoEmpresa, codigoUsuario = 0;

                if (!int.TryParse(Servicos.Criptografia.Descriptografar(HttpUtility.UrlDecode(Request.Params["CodigoEmpresa"]), "CT3##MULT1@#$S0FTW4R3"), out codigoEmpresa))
                    return Json<bool>(false, false, "Código da empresa inválido.");

                int.TryParse(Request.Params["CodigoUsuario"], out codigoUsuario);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorPorCodigoEEmpresa(codigoEmpresa, codigoUsuario);

                if (usuario != null)
                {
                    var retorno = new
                    {
                        usuario.Codigo,
                        usuario.Complemento,
                        CPFCNPJ = usuario.CPF,
                        DataAdmissao = usuario.DataAdmissao != null ? usuario.DataAdmissao.Value.ToString("dd/MM/yyyy") : "",
                        DataNascimento = usuario.DataNascimento != null ? usuario.DataNascimento.Value.ToString("dd/MM/yyyy") : "",
                        usuario.Email,
                        usuario.Endereco,
                        Localidade = usuario.Localidade.Codigo,
                        SiglaUF = usuario.Localidade.Estado.Sigla,
                        Usuario = usuario.Login,
                        usuario.Nome,
                        RGIE = usuario.RG,
                        Salario = string.Format("{0:n2}", usuario.Salario),
                        usuario.Senha,
                        usuario.Telefone,
                        usuario.Status,
                        AlterarSenhaAcesso = usuario.AlterarSenhaAcesso,
                        Permissoes = this.BuscarPermissoes(usuario, unitOfWork),
                        Series = (from obj in usuario.Series where obj.Status.Equals("A") select new { obj.Codigo, obj.Numero, obj.Tipo, obj.DescricaoTipo }).ToList()
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Usuário não encontrado!");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do usuário.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterSeriesDoUsuario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Dominio.Enumeradores.TipoSerie tipo;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoSerie>(Request.Params["Tipo"], out tipo))
                    return Json<bool>(false, false, "Tipo de série inválido.");

                List<Dominio.Entidades.EmpresaSerie> series = new List<Dominio.Entidades.EmpresaSerie>();

                if (this.Usuario.Series != null)
                    series = this.Usuario.Series.Where(o => o.Tipo == tipo).ToList();

                if (series.Count() == 0)
                {
                    Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
                    series = repSerie.BuscarTodosPorEmpresa(this.EmpresaUsuario.Codigo, tipo, "A");
                }

                var retorno = (from obj in series select new { obj.Codigo, obj.Numero });

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as séries.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult AlterarSenha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string senha = Request.Params["Senha"];
                string confirmacaoSenha = Request.Params["ConfirmacaoSenha"];
                string senhaAtual = Request.Params["SenhaAtual"];

                if (string.IsNullOrWhiteSpace(senha) || senha.Trim().Length < 5 || senha != confirmacaoSenha)
                    return Json<bool>(false, false, "Não foi possível alterar a senha. Certifique-se que a nova senha possua 6 caracteres ou mais e seja igual à confirmação.");

                if (senhaAtual != this.Usuario.Senha)
                    return Json<bool>(false, false, "Senha atual inválida.");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                this.Usuario.Senha = senha;
                this.Usuario.AlterarSenhaAcesso = false;

                repUsuario.Atualizar(this.Usuario);

                return Json(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as séries.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadRelatorioPermissoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int perfil, tipoRelatorio, codigoUsuario = 0;
                int.TryParse(Request.Params["Codigo"], out codigoUsuario);
                int.TryParse(Request.Params["TipoRelatorio"], out tipoRelatorio);
                int.TryParse(Request.Params["Perfil"], out perfil);

                string tipoArquivo = Request.Params["TipoArquivo"];
                string status = Request.Params["Status"];
                string login = Request.Params["Login"];
                string nome = Request.Params["Nome"];

                Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.PerfilPermissao repPerfilPermissao = new Repositorio.PerfilPermissao(unitOfWork);

                Dominio.Entidades.Usuario usuario = new Dominio.Entidades.Usuario();
                Dominio.Entidades.PerfilPermissao perfilPermissao = null;

                if (codigoUsuario > 0)
                    usuario = repUsuario.BuscarPorCodigo(codigoUsuario);

                if (perfil > 0)
                    perfilPermissao = repPerfilPermissao.BuscarPorCodigo(perfil);


                List<Dominio.ObjetosDeValor.Relatorios.RelatorioPermissaoUsuario> listaPermissoes = repPaginaUsuario.RelatorioPermissaoUsuario(this.EmpresaUsuario.Codigo, codigoUsuario, status, nome, login, perfil);

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("Empresa", this.EmpresaUsuario.CNPJ_Formatado + " " + this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("Nome", string.IsNullOrWhiteSpace(nome) ? "Todos" : nome));
                parametros.Add(new ReportParameter("Login", string.IsNullOrWhiteSpace(login) ? "Todos" : login));
                parametros.Add(new ReportParameter("Status", string.IsNullOrWhiteSpace(status) ? "Todos" : status == "A" ? "Ativo" : "Inativo"));
                parametros.Add(new ReportParameter("Perfil", perfilPermissao == null ? "Todos" : perfilPermissao.Descricao));

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Permissoes", listaPermissoes));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                if (tipoRelatorio == 0)
                {
                    Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioPermissaoUsuario.rdlc", tipoArquivo, parametros, dataSources);

                    return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioPermissaoUsuario." + arquivo.FileNameExtension);
                }
                else
                {
                    Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelatorioPermissaoUsuarioPerfil.rdlc", tipoArquivo, parametros, dataSources);

                    return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelatorioPermissaoUsuario." + arquivo.FileNameExtension);
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterUsuarioLogado()
        {
            try
            {
                Dominio.Entidades.Usuario usuario = this.Usuario;
                var retorno = new
                {
                    usuario.Codigo,
                    CPFCNPJ = usuario.CPF,
                    usuario.Email,
                    usuario.Endereco,
                    usuario.Nome,
                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do usuário.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterConfiguracaoParaPesquisaNPS()
        {
            try
            {
                Dominio.Entidades.Usuario usuario = this.Usuario;
                string exibirPesquisaNPS = System.Configuration.ConfigurationManager.AppSettings["ExibirPesquisaNPS"] ?? "";

                if ((exibirPesquisaNPS != "SIM") ||
                    (IdUsuarioAdministrativo != 0) ||
                    (usuario.Empresa.TipoAmbiente != Dominio.Enumeradores.TipoAmbiente.Producao) ||
                    (usuario.Empresa.StatusEmissao != "S") || // S - Sistema Web
                    string.IsNullOrEmpty(usuario.Nome) || string.IsNullOrEmpty(usuario.Email))
                {
                    return Json<bool>(false, false, "Os requisitos necessários para pesquisa de NPS não foram preenchidos.");
                }

                DateTimeOffset dateTimeOffset = new((DateTime)usuario.Empresa.DataCadastro);

                var retorno = new
                {
                    usuarioNome = usuario.Nome,
                    usuarioEmail = usuario.Email,
                    empresaCodigo = usuario.Empresa.Codigo,
                    empresaCNPJ = usuario.Empresa.CNPJ_Formatado,
                    empresaDataCadastro = dateTimeOffset.ToUnixTimeMilliseconds()
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados para pesquisa de NPS.");
            }
        }
        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.PaginasUsuario> BuscarPermissoes(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);
            List<Dominio.Entidades.PaginaUsuario> paginasUsuario = repPaginaUsuario.BuscarPorUsuario(usuario.Codigo);
            List<Dominio.ObjetosDeValor.PaginasUsuario> permissoes = (from obj in paginasUsuario
                                                                      select new Dominio.ObjetosDeValor.PaginasUsuario
                                                                      {
                                                                          Codigo = obj.Pagina.Codigo,
                                                                          Acesso = obj.PermissaoDeAcesso == "A" ? true : false,
                                                                          Incluir = obj.PermissaoDeInclusao == "A" ? true : false,
                                                                          Alterar = obj.PermissaoDeAlteracao == "A" ? true : false,
                                                                          Excluir = obj.PermissaoDeDelecao == "A" ? true : false
                                                                      }).ToList();
            return permissoes;
        }

        private void AtualizarSeries(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Series"]))
            {
                if (usuario.Series == null)
                    usuario.Series = new List<Dominio.Entidades.EmpresaSerie>();

                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);

                List<Dominio.ObjetosDeValor.Serie> series = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Serie>>(Request.Params["Series"]);

                List<int> listaCodigoSeriesAdicionar = (from obj in series select obj.Codigo).ToList();
                List<Dominio.Entidades.EmpresaSerie> listaSeriesRemover = (from obj in usuario.Series where !listaCodigoSeriesAdicionar.Contains(obj.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.EmpresaSerie serieRemover in listaSeriesRemover)
                    usuario.Series.Remove(serieRemover);

                foreach (int codigoSerie in listaCodigoSeriesAdicionar)
                {
                    Dominio.Entidades.EmpresaSerie serie = repSerie.BuscarPorCodigo(codigoSerie);
                    usuario.Series.Add(serie);
                }
            }
            else
            {
                if (usuario.Series != null && usuario.Series.Count > 0)
                    usuario.Series.Clear();
            }
        }

        private void AtualizarPermissoes(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<Dominio.ObjetosDeValor.PaginasUsuario> listaPermissoesUsuario = new List<Dominio.ObjetosDeValor.PaginasUsuario>();

            if (!string.IsNullOrWhiteSpace(Request.Params["Permissoes"]))
            {
                listaPermissoesUsuario = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.PaginasUsuario>>(Request.Params["Permissoes"]);

                if (listaPermissoesUsuario.Count > 0)
                {
                    Repositorio.Pagina repPagina = new Repositorio.Pagina(unidadeDeTrabalho);
                    Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unidadeDeTrabalho);
                    foreach (Dominio.ObjetosDeValor.PaginasUsuario permissaoUsuario in listaPermissoesUsuario)
                    {
                        Dominio.Entidades.PaginaUsuario permissao = repPaginaUsuario.BuscarPorPaginaEUsuario(permissaoUsuario.Codigo, usuario.Codigo);
                        bool inserir = false;
                        if (permissao == null)
                        {
                            permissao = new Dominio.Entidades.PaginaUsuario();
                            permissao.Pagina = repPagina.BuscarPorCodigo(permissaoUsuario.Codigo);
                            permissao.Usuario = usuario;
                            inserir = true;
                        }

                        permissao.PermissaoDeAcesso = permissaoUsuario.Acesso ? "A" : "I";
                        permissao.PermissaoDeAlteracao = permissaoUsuario.Alterar ? "A" : "I";
                        permissao.PermissaoDeDelecao = permissaoUsuario.Excluir ? "A" : "I";
                        permissao.PermissaoDeInclusao = permissaoUsuario.Incluir ? "A" : "I";

                        if (inserir)
                            repPaginaUsuario.Inserir(permissao);
                        else
                            repPaginaUsuario.Atualizar(permissao);
                    }
                }
            }
        }

        #endregion
    }
}
