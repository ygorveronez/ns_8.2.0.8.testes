using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos.CamposObrigatorios
{
    [CustomAuthorize("Pedidos/TipoOperacaoCampo")]
    public class TipoOperacaoCampoController : BaseController
    {
		#region Construtores

		public TipoOperacaoCampoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.GetStringParam("Descricao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 80, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoCampo repTipoOperacaoCampo = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoCampo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoCampo> listaCampos = repTipoOperacaoCampo.Consultar(descricao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repTipoOperacaoCampo.ContarConsulta(descricao);

                var retorno = (from obj in listaCampos
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        #endregion
    }
}
