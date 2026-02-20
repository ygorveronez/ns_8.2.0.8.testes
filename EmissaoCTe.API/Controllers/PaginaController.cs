using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class PaginaController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Pagina repPagina = new Repositorio.Pagina(unitOfWork);
                List<Dominio.Entidades.Pagina> listaPaginas = repPagina.BuscarTodos();

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

        [AcceptVerbs("POST")]
        public ActionResult BuscarPaginasDaEmpresa()
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

        [AcceptVerbs("POST")]
        public ActionResult BuscarPaginasPorEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoEmpresa = 0;
                if (!int.TryParse(Servicos.Criptografia.Descriptografar(HttpUtility.UrlDecode(Request.Params["CodigoEmpresa"]), "CT3##MULT1@#$S0FTW4R3"), out codigoEmpresa))
                    return Json<bool>(false, false, "Código da empresa inválido.");
                Repositorio.PermissaoEmpresa repPermissaoEmpresa = new Repositorio.PermissaoEmpresa(unitOfWork);
                List<Dominio.Entidades.PermissaoEmpresa> listaPermissoes = repPermissaoEmpresa.BuscarPorEmpresa(codigoEmpresa);
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

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorTipoAcesso()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Dominio.Enumeradores.TipoAcesso tipoAcesso;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoAcesso>(Request.Params["TipoAcesso"], out tipoAcesso))
                    return Json<bool>(false, false, "Tipo de página inválido.");

                Repositorio.Pagina repPagina = new Repositorio.Pagina(unitOfWork);
                List<Dominio.Entidades.Pagina> listaPaginas = repPagina.BuscarPorTipoAcesso(tipoAcesso);

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
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as páginas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarMenus()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.Menu repMenu = new Repositorio.Menu(unidadeDeTrabalho);

                List<Dominio.Entidades.Menu> listaMenus = repMenu.ConsultarMenus(inicioRegistros, 50);
                int countMenus = repMenu.ContarConsultaMenus();

                var retorno = (from obj in listaMenus
                               orderby obj.Descricao ascending
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   MenuPai = obj.MenuPai != null ? obj.MenuPai.Descricao : string.Empty
                               }).ToList();
                
                return Json(retorno, true, null, new string[] { "Codigo", "Descrição|40", "Menu Pai|30" }, countMenus);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os menus.");
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

                bool ambienteAdmin, ambienteEmissao = false;
                bool.TryParse(Request.Params["AmbienteAdmin"], out ambienteAdmin);
                bool.TryParse(Request.Params["AmbienteEmissao"], out ambienteEmissao);

                string descricao = Request.Params["Descricao"];
                string menu = Request.Params["Menu"];

                Repositorio.Pagina repPagina = new Repositorio.Pagina(unidadeDeTrabalho);

                List<Dominio.Entidades.Pagina> listaPaginas = repPagina.Consultar(Dominio.Enumeradores.TipoSistema.MultiCTe, ambienteAdmin, ambienteEmissao, descricao, menu, inicioRegistros, 50);
                int countPagina = repPagina.ContarConsulta(Dominio.Enumeradores.TipoSistema.MultiCTe, ambienteAdmin, ambienteEmissao, descricao, menu);

                var retorno = (from obj in listaPaginas
                               orderby obj.Descricao ascending
                               select new
                               {
                                   obj.Codigo,
                                   obj.Status,
                                   obj.TipoAcesso,
                                   obj.MostraNoMenu,
                                   obj.Formulario,
                                   obj.Descricao,
                                   Menu = !string.IsNullOrWhiteSpace(obj.Menu) ? obj.Menu : string.Empty,
                                   TipoAcessoDescricao = obj.TipoAcesso.ToString("G"),
                                   StatusDescricao = obj.Status.Equals("A") ? "Ativo" : obj.Status.Equals("I") ? "Inativo" : string.Empty,
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Status", "TipoAcesso", "MostraNoMenu", "Formulário|20", "Descricao|25", "Menu|15", "Tipo do Acesso|15", "Status|10" }, countPagina);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os formulários.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarPagina()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                bool mostraNoMenu = false;
                bool.TryParse(Request.Params["MostraNoMenu"], out mostraNoMenu);

                string formulario = Request.Params["Formulario"];
                string descricao = Request.Params["Descricao"];
                string menu = Request.Params["Menu"];
                string status = Request.Params["Status"];

                Dominio.Enumeradores.TipoAcesso tipoAcesso;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoAcesso>(Request.Params["TipoAcesso"], out tipoAcesso))
                    return Json<bool>(false, false, "Tipo de página inválido.");

                Repositorio.Pagina repPagina = new Repositorio.Pagina(unidadeDeTrabalho);
                Repositorio.Menu repMenu = new Repositorio.Menu(unidadeDeTrabalho);
                Dominio.Entidades.Pagina pagina;
                Dominio.Entidades.Menu menuApp = repMenu.BuscarPorDescricao(menu); ;

                // Instancia ou busca a entidade
                if (codigo > 0)
                    pagina = repPagina.BuscarPorCodigo(codigo);
                else
                {
                    pagina = new Dominio.Entidades.Pagina();
                    pagina.TipoSistema = Dominio.Enumeradores.TipoSistema.MultiCTe;
                    pagina.Icone = null;
                }

                // Seta as informacoes
                pagina.Descricao = descricao;
                pagina.Formulario = formulario;
                pagina.Menu = menuApp != null ? menuApp.Descricao : menu;
                pagina.MenuApp = menuApp;
                pagina.Status = status.Equals("A") ? "A" : "I";
                pagina.TipoAcesso = tipoAcesso;
                pagina.MostraNoMenu = mostraNoMenu;

                if (codigo > 0)
                    repPagina.Atualizar(pagina);
                else
                    repPagina.Inserir(pagina);

                return Json("Formulário atualizado com sucesso.", true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao atualizar o formulário.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
