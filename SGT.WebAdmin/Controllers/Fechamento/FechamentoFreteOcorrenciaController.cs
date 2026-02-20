using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fechamento
{
    [CustomAuthorize("Fechamento/FechamentoFrete")]
    public class FechamentoFreteOcorrenciaController : BaseController
    {
		#region Construtores

		public FechamentoFreteOcorrenciaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("NumeroOcorrencia").Nome("Número").Tamanho(7).Align(Models.Grid.Align.center);
            grid.Prop("TipoOcorrencia").Nome("Tipo da Ocorrência").Tamanho(30).Align(Models.Grid.Align.left);
            grid.Prop("TipoDocumentoCreditoDebito").Nome("Tipo").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("ValorOcorrencia").Nome("Valor").Tamanho(10).Align(Models.Grid.Align.right);

            return grid;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "TipoDocumentoCreditoDebito") propOrdenar = "ModeloDocumentoFiscal?.TipoDocumentoCreditoDebito";
        }


        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Fechamento.FechamentoFreteOcorrencia repFechamentoFreteOcorrencia = new Repositorio.Embarcador.Fechamento.FechamentoFreteOcorrencia(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("Fechamento"), out int fechamento);

            // Consulta
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaGrid = repFechamentoFreteOcorrencia.Consultar(fechamento, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repFechamentoFreteOcorrencia.ContarConsulta(fechamento);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            obj.NumeroOcorrencia,
                            TipoOcorrencia = obj.TipoOcorrencia.Descricao,
                            TipoDocumentoCreditoDebito = obj.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebitoDescricao ?? "Crédito",
                            obj.ValorOcorrencia,
                        };

            return lista.ToList();
        }
    }
}
