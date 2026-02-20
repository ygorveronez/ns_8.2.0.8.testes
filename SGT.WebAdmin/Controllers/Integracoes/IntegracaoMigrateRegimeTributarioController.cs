using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/IntegracaoMigrateRegimeTributario")]
    public class IntegracaoMigrateRegimeTributario : BaseController
    {
		#region Construtores

		public IntegracaoMigrateRegimeTributario(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                if (!ExecutarPesquisa(out string mensagemErro, out dynamic lista, out int count, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, false, unitOfWork))
                    return new JsonpResult(false, true, mensagemErro);

                grid.setarQuantidadeTotal(count);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os regimes tributarios.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);

            return grid;
        }

        private bool ExecutarPesquisa(out string mensagemErro, out dynamic lista, out int count, string propOrdenar, string dirOrdenar, int inicio, int limite, bool exportacao, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;
            lista = null;
            count = 0;

            string descricao = Request.Params("Descricao");

            Repositorio.Embarcador.Integracao.IntegracaoMigrateRegimeTributario repDocumentoTransporte = new Repositorio.Embarcador.Integracao.IntegracaoMigrateRegimeTributario(unitOfWork);

            count = repDocumentoTransporte.ContarConsulta(descricao);

            List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMigrateRegimeTributario> documentosTransporte = repDocumentoTransporte.Consultar(descricao, propOrdenar, dirOrdenar, inicio, limite);

            lista = documentosTransporte.Select(obj => new
            {
                obj.Codigo,
                obj.Numero,
                Descricao = obj.Descricao ?? ""

            }).ToList();

            return true;
        }
        
        #endregion
    }
}
