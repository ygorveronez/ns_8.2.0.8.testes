using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ContingenciaEstado")]
    public class ContingenciaEstadoController : BaseController
    {
		#region Construtores

		public ContingenciaEstadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string nome = Request.GetStringParam("Nome");
                string sigla = Request.GetStringParam("Sigla");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Nome", "Nome", 55, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Sigla", "Sigla", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Emissão", "DescricaoTipoEmissao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Codigo", false);

                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                List<Dominio.Entidades.Estado> estados = repEstado.ConsultarContingencia(nome, sigla, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repEstado.ContarConsultaContingencia(nome, sigla));

                var lista = (from p in estados
                             select new
                             {
                                 Codigo = p.Sigla,
                                 p.Sigla,
                                 p.Nome,
                                 p.DescricaoTipoEmissao
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
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

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                string sigla = Request.GetStringParam("Codigo");

                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(sigla);
                estado.Initialize();

                estado.TipoEmissao = Request.GetStringParam("TipoEmissao");

                repEstado.Atualizar(estado, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string sigla = Request.GetStringParam("Codigo");
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(sigla);

                var dynEstado = new
                {
                    Codigo = estado.Sigla,
                    estado.Nome,
                    estado.TipoEmissao
                };

                return new JsonpResult(dynEstado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        #endregion
    }
}
