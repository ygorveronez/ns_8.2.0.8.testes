using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class PerfilPermissaoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("perfilpermissao.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string descricao = Request.Params["Descricao"];

                Repositorio.PerfilPermissao repPerfilPermissao = new Repositorio.PerfilPermissao(unitOfWork);

                List<Dominio.Entidades.PerfilPermissao> listaPerfilPermissao = repPerfilPermissao.Consulta(this.EmpresaUsuario.Codigo, descricao, inicioRegistros, 50);
                int countUsuarios = repPerfilPermissao.ContarConsultr(this.EmpresaUsuario.Codigo, descricao);

                var retorno = from obj in listaPerfilPermissao select new { obj.Codigo, obj.Descricao, Ativo = obj.Ativo ? "Ativo" : "Inativo" };
                return Json(retorno, true, null, new string[] { "Codigo", "Descrição|60", "Status|20" }, countUsuarios);
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
        public ActionResult ConsultarAtivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string descricao = Request.Params["Descricao"];

                Repositorio.PerfilPermissao repPerfilPermissao = new Repositorio.PerfilPermissao(unitOfWork);

                List<Dominio.Entidades.PerfilPermissao> listaPerfilPermissao = repPerfilPermissao.Consulta(this.EmpresaUsuario.Codigo, descricao, inicioRegistros, 50, true);
                int countUsuarios = repPerfilPermissao.ContarConsultr(this.EmpresaUsuario.Codigo, descricao, true);

                var retorno = from obj in listaPerfilPermissao select new { obj.Codigo, obj.Descricao, Ativo = obj.Ativo ? "Ativo" : "Inativo" };
                return Json(retorno, true, null, new string[] { "Codigo", "Descrição|60", "Status|20" }, countUsuarios);
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

                string descricao = Request.Params["Descricao"];

                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                bool status = true;
                bool.TryParse(Request.Params["Status"], out status);

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descricao inválida!");

                if (!status && codigo > 0)
                {
                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                    List<Dominio.Entidades.Usuario> listaUsuario = repUsuario.BuscarPorPerfil(this.EmpresaUsuario.Codigo, codigo);
                    if (listaUsuario.Count() > 0)
                    {
                        string usuarios = "";

                        for (var i = 0; i < listaUsuario.Count; i++)
                        {
                            if (string.IsNullOrWhiteSpace(usuarios))
                                usuarios = listaUsuario[i].Nome;
                            else
                                usuarios = string.Concat(usuarios, ", ", listaUsuario[i].Nome);
                        }

                        return Json<bool>(false, false, "Não é possível inativar, Perfil está sendo utilizado nos usuários: "+ usuarios);
                    }
                }

                Repositorio.PerfilPermissao repPerfilPermissao = new Repositorio.PerfilPermissao(unidadeDeTrabalho);

                Dominio.Entidades.PerfilPermissao perfilPermissao = new Dominio.Entidades.PerfilPermissao();

                bool inserir = false;

                perfilPermissao = repPerfilPermissao.BuscarPorCodigo(codigo);

                if (perfilPermissao == null)
                {
                    perfilPermissao = new Dominio.Entidades.PerfilPermissao();
                    inserir = true;
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de perfil negada!");
                }

                perfilPermissao.Descricao = descricao;
                perfilPermissao.Empresa = this.EmpresaUsuario;
                perfilPermissao.Ativo = status;

                if (inserir)
                    repPerfilPermissao.Inserir(perfilPermissao);
                else
                    repPerfilPermissao.Atualizar(perfilPermissao);

                this.AtualizarPermissoes(perfilPermissao, unidadeDeTrabalho);
                this.AtualizarPermissoesUsuarios(perfilPermissao, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                if (unidadeDeTrabalho != null)
                    unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar perfil.");
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
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.PerfilPermissao repPerfilPermissao = new Repositorio.PerfilPermissao(unitOfWork);
                Dominio.Entidades.PerfilPermissao perfilPermissao = repPerfilPermissao.BuscarPorCodigo(codigo);

                if (perfilPermissao != null)
                {
                    var retorno = new
                    {
                        perfilPermissao.Codigo,
                        perfilPermissao.Descricao,
                        Ativo = perfilPermissao.Ativo.ToString(),
                        Permissoes = this.BuscarPermissoes(perfilPermissao, unitOfWork)
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
        public ActionResult BuscarPaginas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.PermissaoEmpresa repPermissaoEmpresa = new Repositorio.PermissaoEmpresa(unitOfWork);
                List<Dominio.Entidades.PermissaoEmpresa> listaPermissoes = repPermissaoEmpresa.BuscarPorEmpresa(this.EmpresaUsuario.Codigo);
                List<Dominio.Entidades.Pagina> listaPaginas = (from obj in listaPermissoes where obj.PermissaoDeAcesso.Equals("A") select obj.Pagina).ToList();
                List<object> listaPaginasPorGrupo = new List<object>();
                List<string> grupos = (from obj in listaPaginas select obj.Menu).Distinct().ToList();
                foreach (string grupo in grupos)
                {
                    listaPaginasPorGrupo.Add(new { Grupo = string.IsNullOrWhiteSpace(grupo) ? "Geral" : grupo, Paginas = (from obj in listaPaginas where obj.Menu == grupo select new { obj.Codigo, obj.Descricao }).ToList() });
                }
                return Json(listaPaginasPorGrupo, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as páginas para permissão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.PaginasUsuario> BuscarPermissoes(Dominio.Entidades.PerfilPermissao perfil, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.PerfilPermissaoPagina repPerfilPermissaoPagina = new Repositorio.PerfilPermissaoPagina(unitOfWork);
            List<Dominio.Entidades.PerfilPermissaoPagina> perfilPermissaoPaginas = repPerfilPermissaoPagina.BuscarPorCodigoPerfil(perfil.Codigo);
            List<Dominio.ObjetosDeValor.PaginasUsuario> permissoes = (from obj in perfilPermissaoPaginas
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

        private void AtualizarPermissoes(Dominio.Entidades.PerfilPermissao perfil, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<Dominio.ObjetosDeValor.PaginasUsuario> listaPermissoesUsuario = new List<Dominio.ObjetosDeValor.PaginasUsuario>();

            if (!string.IsNullOrWhiteSpace(Request.Params["Permissoes"]))
            {
                listaPermissoesUsuario = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.PaginasUsuario>>(Request.Params["Permissoes"]);

                if (listaPermissoesUsuario.Count > 0)
                {
                    Repositorio.Pagina repPagina = new Repositorio.Pagina(unidadeDeTrabalho);
                    Repositorio.PerfilPermissaoPagina repPerfilPermissaoPaginao = new Repositorio.PerfilPermissaoPagina(unidadeDeTrabalho);
                    foreach (Dominio.ObjetosDeValor.PaginasUsuario permissaoUsuario in listaPermissoesUsuario)
                    {
                        Dominio.Entidades.PerfilPermissaoPagina permissao = repPerfilPermissaoPaginao.BuscarPorPerfilEPagina(perfil.Codigo, permissaoUsuario.Codigo);
                        bool inserir = false;
                        if (permissao == null)
                        {
                            permissao = new Dominio.Entidades.PerfilPermissaoPagina();
                            permissao.Pagina = repPagina.BuscarPorCodigo(permissaoUsuario.Codigo);
                            permissao.PerfilPermissao = perfil;
                            inserir = true;
                        }

                        permissao.PermissaoDeAcesso = permissaoUsuario.Acesso ? "A" : "I";
                        permissao.PermissaoDeAlteracao = permissaoUsuario.Alterar ? "A" : "I";
                        permissao.PermissaoDeDelecao = permissaoUsuario.Excluir ? "A" : "I";
                        permissao.PermissaoDeInclusao = permissaoUsuario.Incluir ? "A" : "I";

                        if (inserir)
                            repPerfilPermissaoPaginao.Inserir(permissao);
                        else
                            repPerfilPermissaoPaginao.Atualizar(permissao);
                    }
                }
            }
        }

        private void AtualizarPermissoesUsuarios(Dominio.Entidades.PerfilPermissao perfil, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<Dominio.ObjetosDeValor.PaginasUsuario> listaPermissoesUsuario = new List<Dominio.ObjetosDeValor.PaginasUsuario>();

            if (!string.IsNullOrWhiteSpace(Request.Params["Permissoes"]))
            {
                listaPermissoesUsuario = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.PaginasUsuario>>(Request.Params["Permissoes"]);

                if (listaPermissoesUsuario.Count > 0)
                {
                    Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                    List<Dominio.Entidades.Usuario> listaUsuario = repUsuario.BuscarPorPerfil(this.EmpresaUsuario.Codigo, perfil.Codigo);

                    foreach (Dominio.Entidades.Usuario usuario in listaUsuario)
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
        }

        #endregion
    }
}