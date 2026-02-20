using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class MotoristaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("motoristas.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorCPF()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string cpf = Utilidades.String.OnlyNumbers(Request.Params["CPF"]);

                if (string.IsNullOrWhiteSpace(cpf))
                    return Json<bool>(false, false, "CPF inválido.");

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarMotoristaPorCPF(this.EmpresaUsuario.Codigo, cpf);

                if (motorista != null)
                    return Json(new { motorista.Codigo, motorista.CPF, motorista.Nome }, true);

                return Json<bool>(false, false, "Motorista não encontrado.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados do motorista.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string nome = Request.Params["Nome"];
                string cpf = Utilidades.String.OnlyNumbers(Request.Params["CPF"]);
                string status = Request.Params["Status"];

                if (!string.IsNullOrWhiteSpace(status))
                    status = status.ToUpper();

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                bool consultarFiliais = this.EmpresaUsuario.Matriz.Count > 0 || this.EmpresaUsuario.Filiais.Count > 0;//  repEmpresa.PossuiMatrizFilial(this.EmpresaUsuario.Codigo);

                List<Dominio.Entidades.Usuario> listaMotoristas = repUsuario.ConsultarMotoristas(this.EmpresaUsuario.Codigo, nome, this.Usuario.TipoAcesso, status, inicioRegistros, 50, cpf, consultarFiliais);
                int countMotoristas = repUsuario.ContarConsultaMotoristas(this.EmpresaUsuario.Codigo, nome, this.Usuario.TipoAcesso, status, cpf, consultarFiliais);

                var retorno = (from obj in listaMotoristas select new { obj.Codigo, obj.PercentualComissao, Salario = obj.Salario.ToString("n2"), obj.Nome, CPFCNPJ = obj.CPF, RGIE = obj.RG, obj.Email }).ToList() ;
                return Json(retorno, true, null, new string[] { "Codigo", "Percentual Comissao", "Salario", "Nome|40", "CPF/CNPJ|15", "RG/IE|15", "E-mail|20" }, countMotoristas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os usuários.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                unidadeDeTrabalho.Start();

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
                string tipoSanguineo = Request.Params["TipoSanguineo"];
                string numeroHabilitacao = Request.Params["NumeroHabilitacao"];
                string categoriaHabilitacao = Request.Params["CategoriaHabilitacao"];
                string moop = Request.Params["MOOP"];
                string pis = Request.Params["PIS"];
                string numeroCartao = Request.Params["NumeroCartao"];
                string cep = Request.Params["CEP"];
                string bairro = Request.Params["Bairro"];
                string estadoRG = Request.Params["EstadoRG"];

                if (string.IsNullOrWhiteSpace(nome))
                    return Json<bool>(false, false, "Nome inválido!");

                if (string.IsNullOrWhiteSpace(cpfCnpj) || (cpfCnpj.Length != 11 && cpfCnpj.Length != 14))
                    return Json<bool>(false, false, "CPF/CNPJ inválido!");

                decimal salario, percentualComissao = 0m;
                decimal.TryParse(Request.Params["Salario"], out salario);
                decimal.TryParse(Request.Params["PercentualComissao"], out percentualComissao);

                int localidade, codigoUsuario = 0;
                int.TryParse(Request.Params["Codigo"], out codigoUsuario);
                int.TryParse(Request.Params["Localidade"], out localidade);

                Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG orgaoEmissorRG;
                Dominio.ObjetosDeValor.Enumerador.Sexo sexo;

                List<Dominio.ObjetosDeValor.PaginasUsuario> listaPermissoesUsuario = new List<Dominio.ObjetosDeValor.PaginasUsuario>();

                if (!string.IsNullOrWhiteSpace(Request.Params["Permissoes"]))
                    listaPermissoesUsuario = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.PaginasUsuario>>(Request.Params["Permissoes"]);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);
                Repositorio.Setor repSetor = new Repositorio.Setor(unidadeDeTrabalho);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unidadeDeTrabalho);

                Dominio.Entidades.Usuario user = new Dominio.Entidades.Usuario();

                bool inserir = false;

                user = repUsuario.BuscarMotoristaPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigoUsuario);

                if (user == null)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de motorista negada!");

                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaPorCPF(this.EmpresaUsuario.Codigo, cpfCnpj);
                    if (motorista != null)
                        return Json<bool>(false, false, "Já existe um cadastro com o CPF informado!");

                    user = new Dominio.Entidades.Usuario
                    {
                        Empresa = this.EmpresaUsuario,
                        Setor = repSetor.BuscarPorCodigo(1),
                        CPF = cpfCnpj,
                        Tipo = "M",
                        Status = "A"
                    };
                    inserir = true;
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de motorista negada!");

                    user.Initialize();
                }

                user.TipoSanguineo = tipoSanguineo;
                user.NumeroHabilitacao = numeroHabilitacao;
                user.Categoria = categoriaHabilitacao;
                user.Moop = moop;
                user.PercentualComissao = percentualComissao;
                user.Complemento = complemento;
                user.Bairro = bairro;
                user.CEP = cep;

                if (Enum.TryParse(Request.Params["Sexo"], out sexo))
                    user.Sexo = sexo;
                else
                    user.Sexo = null;

                if (Enum.TryParse(Request.Params["OrgaoEmissorRG"], out orgaoEmissorRG))
                    user.OrgaoEmissorRG = orgaoEmissorRG;
                else
                    user.OrgaoEmissorRG = null;

                DateTime dataNascimento, dataAdmissao, dataHabilitacao, dataVencimentoHabilitacao, dataValidadeSeguradora;

                if (!string.IsNullOrWhiteSpace(Request.Params["DataHabilitacao"]))
                    if (!DateTime.TryParseExact(Request.Params["DataHabilitacao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataHabilitacao))
                        return Json<bool>(false, false, "Data da habilitação inválida!");
                    else
                        user.DataHabilitacao = dataHabilitacao;
                else
                    user.DataHabilitacao = null;

                if (!string.IsNullOrWhiteSpace(Request.Params["DataVencimentoHabilitacao"]))
                    if (!DateTime.TryParseExact(Request.Params["DataVencimentoHabilitacao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimentoHabilitacao))
                        return Json<bool>(false, false, "Data de vencimento da habilitação inválida!");
                    else
                        user.DataVencimentoHabilitacao = dataVencimentoHabilitacao;
                else
                    user.DataVencimentoHabilitacao = null;

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


                if (!string.IsNullOrWhiteSpace(Request.Params["DataValidadeSeguradora"]))
                    if (!DateTime.TryParseExact(Request.Params["DataValidadeSeguradora"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataValidadeSeguradora))
                        return Json<bool>(false, false, "Data Validade Seguradaora inválida!");
                    else
                        user.DataValidadeLiberacaoSeguradora = dataValidadeSeguradora;
                else
                    user.DataValidadeLiberacaoSeguradora = null;                

                user.Email = email;
                user.Endereco = endereco;
                user.Empresa = this.EmpresaUsuario;
                user.Localidade = repLocalidade.BuscarPorCodigo(localidade);
                user.EstadoRG = repEstado.BuscarPorSigla(estadoRG);

                if (localidade == 0)
                    return Json<bool>(false, false, "Município é obrigatório!");

                if (user.Localidade == null)
                    return Json<bool>(false, false, "Município não encontrado!");

                user.Nome = nome;
                user.RG = rgIE;
                user.Salario = salario;
                user.Telefone = telefone;
                user.PIS = pis;
                user.NumeroCartao = numeroCartao;

                if (this.Permissao() != null || this.Permissao().PermissaoDeDelecao == "A")
                    user.Status = status;

                if (inserir)
                    repUsuario.Inserir(user, Auditado);
                else
                    repUsuario.Atualizar(user, Auditado);

                if (user.Status == "I") //Desvincula motorista de todos veiculos
                {
                    try
                    {
                        List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> listaVeiculoMotorista = repVeiculoMotorista.BuscarVeiculosMotoristaPorMotorista(user.Codigo);

                        foreach (var motoristaVeiculo in listaVeiculoMotorista)
                            repVeiculoMotorista.Deletar(motoristaVeiculo);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Falha ao remover motoristas dos veículos: "+ ex);
                    }
                }


                if (listaPermissoesUsuario.Count > 0 && !string.IsNullOrWhiteSpace(usuario) && !string.IsNullOrWhiteSpace(senha))
                {
                    user.Login = usuario;
                    user.Senha = senha;

                    Repositorio.Pagina repPagina = new Repositorio.Pagina(unidadeDeTrabalho);
                    Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unidadeDeTrabalho);
                    foreach (Dominio.ObjetosDeValor.PaginasUsuario permissaoUsuario in listaPermissoesUsuario)
                    {
                        Dominio.Entidades.PaginaUsuario permissao = repPaginaUsuario.BuscarPorPaginaEUsuario(permissaoUsuario.Codigo, user.Codigo);
                        if (permissao == null)
                        {
                            permissao = new Dominio.Entidades.PaginaUsuario
                            {
                                Pagina = repPagina.BuscarPorCodigo(permissaoUsuario.Codigo),
                                Usuario = user
                            };
                        }

                        permissao.PermissaoDeAcesso = permissaoUsuario.Acesso ? "A" : "I";
                        permissao.PermissaoDeAlteracao = permissaoUsuario.Alterar ? "A" : "I";
                        permissao.PermissaoDeDelecao = permissaoUsuario.Excluir ? "A" : "I";
                        permissao.PermissaoDeInclusao = permissaoUsuario.Incluir ? "A" : "I";

                        if (permissao.Codigo == 0)
                            repPaginaUsuario.Inserir(permissao);
                        else
                            repPaginaUsuario.Atualizar(permissao);
                    }
                }

                unidadeDeTrabalho.CommitChanges();

                //try
                //{
                //    Servicos.Usuario svcUsuario = new Servicos.Usuario(Conexao.StringConexao);
                //    svcUsuario.SalvarViculosMatrizFilial(user, unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);
                //}
                //catch (Exception ex)
                //{
                //    Servicos.Log.TratarErro(ex);
                //    return Json<bool>(false, false, "Ocorreu uma falha ao salvar motorista nas filiais.");
                //}

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, ex.InnerException.ToString().ToLower().Contains("violation of primary key constraint") ? "CPF já existente no sistema." : "Ocorreu uma falha ao salvar o usuário.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    return Json<bool>(false, false, "Permissão para alteração de usuário negada!");

                int codigoUsuario = 0;
                int.TryParse(Request.Params["CodigoUsuario"], out codigoUsuario);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarMotoristaPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigoUsuario);

                if (usuario != null)
                {
                    var retorno = new
                    {
                        usuario.Codigo,
                        usuario.Complemento,
                        CPFCNPJ = usuario.CPF,
                        DataAdmissao = usuario.DataAdmissao?.ToString("dd/MM/yyyy") ?? "",
                        DataNascimento = usuario.DataNascimento?.ToString("dd/MM/yyyy") ?? "",
                        usuario.Email,
                        usuario.Endereco,
                        Localidade = usuario.Localidade != null ? usuario.Localidade.Codigo : 0,
                        SiglaUF = usuario.Localidade != null ? usuario.Localidade.Estado.Sigla : string.Empty,
                        Usuario = usuario.Login,
                        usuario.Nome,
                        RGIE = usuario.RG,
                        Salario = string.Format("{0:n2}", usuario.Salario),
                        usuario.Senha,
                        usuario.Telefone,
                        usuario.Status,
                        Permissoes = this.BuscarPermissoes(usuario, unidadeDeTrabalho),
                        CategoriaHabilitacao = usuario.Categoria,
                        DataHabilitacao = usuario.DataHabilitacao?.ToString("dd/MM/yyyy") ?? "",
                        DataVencimentoHabilitacao = usuario.DataVencimentoHabilitacao?.ToString("dd/MM/yyyy") ?? "",
                        usuario.NumeroHabilitacao,
                        PercentualComissao = usuario.PercentualComissao.ToString("n2"),
                        usuario.TipoSanguineo,
                        MOOP = usuario.Moop,
                        DataValidadeSeguradora = usuario.DataValidadeLiberacaoSeguradora?.ToString("dd/MM/yyyy") ?? "",
                        PIS = usuario.PIS,
                        NumeroCartao = usuario.NumeroCartao,
                        EstadoRG = usuario.EstadoRG?.Sigla ?? string.Empty,
                        Sexo = usuario.Sexo?.ToString("d") ?? string.Empty,
                        OrgaoEmissorRG = usuario.OrgaoEmissorRG?.ToString("d") ?? string.Empty,
                        usuario.CEP,
                        usuario.Bairro
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
                unidadeDeTrabalho.Dispose();
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

        #endregion
    }
}
