using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/StatusAssinaturaContrato")]
    public class StatusAssinaturaContratoController : BaseController
    {
		#region Construtores

		public StatusAssinaturaContratoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Metódos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string codigoIntegracaogo = Request.GetStringParam("CodigoIntegracao");
                string descricao = Request.GetStringParam("Descricao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Código de Integração", "CodigoIntegracao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.center, true);

                Repositorio.Embarcador.Frete.StatusAssinaturaContrato repStatusAssinaturaContrato  = new Repositorio.Embarcador.Frete.StatusAssinaturaContrato(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repStatusAssinaturaContrato.ContarConsulta(codigoIntegracaogo, descricao);

                List<Dominio.Entidades.Embarcador.Frete.StatusAssinaturaContrato> listaStatusAssinaturaContrato = totalRegistros > 0 ? repStatusAssinaturaContrato.Consultar(codigoIntegracaogo, descricao, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.StatusAssinaturaContrato>();

                var lista = (from p in listaStatusAssinaturaContrato
                             select new
                             {
                                 p.Codigo,
                                 p.CodigoIntegracao,
                                 p.Descricao
                             }).ToList();

                grid.AdicionaRows(lista);
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
        #endregion
    }
}
