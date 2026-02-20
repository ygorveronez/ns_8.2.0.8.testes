using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize(new string[] { "WebService/PermissaoWebService", "Transportadores/SolicitacaoToken" })]
    public class PermissaoWebServiceController : BaseController
    {
		#region Construtores

		public PermissaoWebServiceController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.WebService.PermissaoWebService.FiltroPesquisaPermissaoWebService filtrosPesquisa = ObterFiltrosPesquisa();

                Grid grid = ObterGridPesquisa();

                Repositorio.WebService.PermissaoWebservice repPermissao = new Repositorio.WebService.PermissaoWebservice(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                List<Dominio.Entidades.WebService.PermissaoWebservice> listaPermissoes = new List<Dominio.Entidades.WebService.PermissaoWebservice>();
                int totalRegistros = 0;

                totalRegistros = repPermissao.ContarConsulta(filtrosPesquisa);

                if (totalRegistros > 0)
                    listaPermissoes = repPermissao.Consultar(filtrosPesquisa, parametrosConsulta);

                var listaPermissoesRetornar = (
                    from permissao in listaPermissoes
                    select new
                    {
                        permissao.Codigo,
                        permissao.NomeMetodo,
                        permissao.RequisicoesMinuto,
                        QuantidadeRequisicoes = permissao.QtdRequisicoes
                    }
                ).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaPermissoesRetornar);

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

        public async Task<IActionResult> ObterRegistros()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.WebService.PermissaoWebService.FiltroPesquisaPermissaoWebService filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.WebService.PermissaoWebservice repPermissao = new Repositorio.WebService.PermissaoWebservice(unitOfWork);
                List<Dominio.Entidades.WebService.PermissaoWebservice> listaPermissoes = new List<Dominio.Entidades.WebService.PermissaoWebservice>();

                listaPermissoes = repPermissao.Consultar(filtrosPesquisa, null);

                var listaPermissoesRetornar = (
                    from permissao in listaPermissoes
                    select new
                    {
                        Codigo = 0,
                        CodigoMetodo = permissao.Codigo,
                        Metodo = permissao.NomeMetodo,
                        permissao.RequisicoesMinuto,
                        QuantidadeRequisicoes = permissao.QtdRequisicoes
                    }
                ).ToList();

                return new JsonpResult(listaPermissoesRetornar);
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

        #endregion

        #region Métodos Privados

        private Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nome do Método", "NomeMetodo", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Requisições por minuto", "RequisicoesMinuto", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Quantidade Requisições", "QuantidadeRequisicoes", 10, Models.Grid.Align.center, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.WebService.PermissaoWebService.FiltroPesquisaPermissaoWebService ObterFiltrosPesquisa()
        {

            Dominio.ObjetosDeValor.WebService.PermissaoWebService.FiltroPesquisaPermissaoWebService filtrosPesquisa = new Dominio.ObjetosDeValor.WebService.PermissaoWebService.FiltroPesquisaPermissaoWebService()
            {
                NomeMetodo = Request.GetStringParam("NomeMetodo"),
                DistinguirPorNomeMetodo = Request.GetBoolParam("DistinguirPorNomeMetodo")
            };

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        #endregion
    }
}
