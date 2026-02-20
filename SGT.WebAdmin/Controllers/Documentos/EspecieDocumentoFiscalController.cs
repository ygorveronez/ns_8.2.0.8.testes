using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/EspecieDocumentoFiscal")]
    public class EspecieDocumentoFiscalController : BaseController
    {
		#region Construtores

		public EspecieDocumentoFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                string sigla = Request.Params("Sigla");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Sigla", "Sigla", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.EspecieDocumentoFiscal repEspecieDocumentoFiscal = new Repositorio.EspecieDocumentoFiscal(unidadeDeTrabalho);

                List<Dominio.Entidades.EspecieDocumentoFiscal> listaEspecieDocumentoFiscal = repEspecieDocumentoFiscal.Consultar(descricao, sigla, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repEspecieDocumentoFiscal.ContarConsulta(descricao, sigla));

                var retorno = (from obj in listaEspecieDocumentoFiscal
                               select new
                               {
                                   obj.Codigo,
                                   obj.Sigla,
                                   obj.Descricao
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
