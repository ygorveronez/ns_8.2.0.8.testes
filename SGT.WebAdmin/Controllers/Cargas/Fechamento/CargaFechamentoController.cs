using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Fechamento
{
    [CustomAuthorize("Cargas/CargaFechamento")]
    public class CargaFechamentoController : BaseController
    {
		#region Construtores

		public CargaFechamentoController(Conexao conexao) : base(conexao) { }

		#endregion


        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string codigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador");
                string codigoPedidoEmbarcador = Request.GetStringParam("CodigoPedidoEmbarcador");
                SituacaoCargaFechamento? Situacao = Request.GetNullableEnumParam<SituacaoCargaFechamento>("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCarga", false);

                grid.AdicionarCabecalho("Carga", "Carga", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Empresa", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor Recalculo", "ValorRecalculo", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Diferença Valor", "DiferencaValor", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motivo Rejeição Recalculo", "MotivoRejeicao", 12, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Carga")
                    propOrdenar = "Carga.CodigoCargaEmbarcador";

                Repositorio.Embarcador.Cargas.CargaFechamento repCargaFechamento = new Repositorio.Embarcador.Cargas.CargaFechamento(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaFechamento> cargasFechamento = repCargaFechamento.Consultar(codigoCargaEmbarcador, codigoPedidoEmbarcador, Situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCargaFechamento.ContarConsulta(codigoCargaEmbarcador, codigoPedidoEmbarcador, Situacao));

                var retorno = (from obj in cargasFechamento
                               select new
                               {
                                   obj.Codigo,
                                   CodigoCarga = obj.Carga.Codigo,
                                   Carga = obj.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                                   Empresa = obj.Carga.Empresa?.Descricao ?? string.Empty,
                                   ValorFrete = obj.Carga.ValorFrete.ToString("n2"),
                                   ValorRecalculo = obj.ValorRecalculado.ToString("n2"),
                                   DiferencaValor = obj.SituacaoFechamento == SituacaoCargaFechamento.Finalizado ? (obj.ValorRecalculado - obj.Carga.ValorFrete).ToString("n2") : "0",
                                   Situacao = obj.SituacaoFechamento.ObterDescricao(),
                                   MotivoRejeicao = obj.MotivoRejeicaoCalculoFrete
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
                unidadeTrabalho.Dispose();
            }
        }

    }
}
