using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pagina")]
    public class PaginaController : BaseController
    {
		#region Construtores

		public PaginaController(Conexao conexao) : base(conexao) { }

		#endregion

        // GET: Pagina
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPaginas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Enumeradores.TipoAcesso? tipoAcesso = null;
                Dominio.Enumeradores.TipoAcesso tipoAcessoAux;

                if (Enum.TryParse<Dominio.Enumeradores.TipoAcesso>(Request.Params("TipoAcesso"), out tipoAcessoAux))
                    tipoAcesso = tipoAcessoAux;

                Repositorio.Pagina repPagina = new Repositorio.Pagina(unitOfWork);
                List<Dominio.Entidades.Pagina> paginas = repPagina.BuscarPorTipoAcessoETipoSistema(tipoAcesso.HasValue ? tipoAcesso.Value : Dominio.Enumeradores.TipoAcesso.Embarcador, this.Empresa?.TipoSistema ?? Dominio.Enumeradores.TipoSistema.MultiEmbarcador);

                List<string> menus = (from p in paginas orderby p.Menu select p.Menu).Distinct().ToList();

                List<dynamic> dynRetorno = new List<dynamic>();
                foreach (string menu in menus)
                {
                    List<Dominio.Entidades.Pagina> paginasMenu = (from p in paginas where p.Menu == menu orderby p.Descricao select p).ToList();
                    var blocoPaginas = new
                    {
                        menu = string.IsNullOrWhiteSpace(menu) ? "Geral" : menu,
                        paginas = (from p in paginasMenu
                                   select new
                                   {
                                       p.Codigo,
                                       p.Descricao,
                                       p.Formulario,
                                       p.MostraNoMenu,
                                       p.Status,
                                       p.TipoAcesso,
                                       p.TipoSistema
                                   }).ToList()
                    };
                    dynRetorno.Add(blocoPaginas);
                }

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarModulosMultiCTe()
        {
            try
            {
                Modulos controllerModulos = new Modulos(_conexao);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    return new JsonpResult(controllerModulos.RetornarListaModulosFormulariosPorTipo(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe));
                else
                    return new JsonpResult(controllerModulos.RetornarListaModulosFormulariosPorTipo(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarModulos()
        {
            try
            {
                Modulos controllerModulos = new Modulos(_conexao);
                return new JsonpResult(controllerModulos.RetornarListaModulosFormularios());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarModulosMobile()
        {
            try
            {
                Modulos controllerModulos = new Modulos(_conexao);
                return new JsonpResult(controllerModulos.RetornarListaModulosFormulariosPorTipo(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiMobile));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }
    }
}
