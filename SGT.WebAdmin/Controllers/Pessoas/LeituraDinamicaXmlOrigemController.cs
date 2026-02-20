using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("LeituraDinamicaXmlOrigem")]
    public class LeituraDinamicaXmlOrigemController : BaseController
    {
        #region Construtores

        public LeituraDinamicaXmlOrigemController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento>("TipoDocumento");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 10, Models.Grid.Align.left, true, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 75, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Pessoas.LeituraDinamicaXmlOrigem repLeituraDinamicaXmlOrigem = new Repositorio.Embarcador.Pessoas.LeituraDinamicaXmlOrigem(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigem> listaLeituraDinamicaXmlOrigem = repLeituraDinamicaXmlOrigem.Consultar(tipoDocumento, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repLeituraDinamicaXmlOrigem.ContarConsulta(tipoDocumento);
                grid.setarQuantidadeTotal(totalRegistros);

                dynamic lista = (from p in listaLeituraDinamicaXmlOrigem select new { Codigo = p.Codigo, Descricao = p.Descricao }).ToList();

                grid.AdicionaRows(lista);

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
    }
}
