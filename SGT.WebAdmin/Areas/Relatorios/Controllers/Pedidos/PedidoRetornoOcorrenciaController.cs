using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pedidos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Pedidos/PedidoRetornoOcorrencia")]
    public class PedidoRetornoOcorrenciaController : BaseController
    {
		#region Construtores

		public PedidoRetornoOcorrenciaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R312_PedidoRetornoOcorrencia;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Retorno de Ocorrências dos Pedidos", "Pedidos", "PedidoRetornoOcorrencia.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroPedido", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoRetornoOcorrencia filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Pedido.PedidoRetornoOcorrencia svcPedidoRetornoOcorrencia = new Servicos.Embarcador.Relatorios.Pedido.PedidoRetornoOcorrencia(unitOfWork, TipoServicoMultisoftware, Cliente);

                svcPedidoRetornoOcorrencia.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoRetornoOcorrencia> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
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
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoRetornoOcorrencia filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o reltároio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Filial", "Filial", TamanhoColunaGrande, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Cidade Destino", "CidadeDestino", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("UF Destino", "UFDestino", TamanhoColunaMedia, Models.Grid.Align.center, false, true);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", TamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Número Pedido", "NumeroPedido", TamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("NF", "NotaFiscal", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Tipo Ocorrência", "TipoOcorrencia", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Grupo Ocorrência", "GrupoOcorrencia", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data Ocorrência", "DataOcorrenciaFormatada", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Valor NF", "ValorNF", TamanhoColunaMedia, Models.Grid.Align.left, false, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoRetornoOcorrencia ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoRetornoOcorrencia filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoRetornoOcorrencia()
            {
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
