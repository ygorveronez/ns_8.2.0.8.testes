using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pedidos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Pedidos/PedidoOcorrencia")]
    public class PedidoOcorrenciaController : BaseController
    {
		#region Construtores

		public PedidoOcorrenciaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R282_PedidoOcorrencia;
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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Ocorrências dos Pedidos", "Pedidos", "PedidoOcorrencia.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroPedido", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaBuscarDadosRelatorio);
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

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoOcorrencia filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Pedido.PedidoOcorrencia svcPedidoOcorrencia = new Servicos.Embarcador.Relatorios.Pedido.PedidoOcorrencia(unitOfWork, TipoServicoMultisoftware, Cliente);

                svcPedidoOcorrencia.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoOcorrencia> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
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
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoOcorrencia filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerarRelatorio);
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
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.PedidoOcorrencia.NumeroPedido, "NumeroPedido", TamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.PedidoOcorrencia.TipoOcorrencia, "TipoOcorrencia", TamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.PedidoOcorrencia.DataOcorrencia, "DataOcorrenciaFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Observacao, "Observacao", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Remetente, "Remetente", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destinatario, "Destinatario", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.PedidoOcorrencia.NumeroCarga, "NumeroCarga", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.PedidoOcorrencia.NotasFiscais, "NotasFiscais", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.PedidoOcorrencia.DataPedido, "DataPedidoFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Filial, "Filial", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Transportador, "Transportador", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Motorista, "Motoristas", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Veiculo, "Veiculo", TamanhoColunaPequena, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.PedidoOcorrencia.SituacaoEntrega, "SituacaoEntregaDescricao", TamanhoColunaMedia, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.PedidoOcorrencia.NaturezaNotaFiscal, "NaturezaOP", TamanhoColunaMedia, Models.Grid.Align.left, true, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoOcorrencia ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoOcorrencia filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoOcorrencia()
            {
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                DataOcorrenciaInicial = Request.GetDateTimeParam("DataOcorrenciaInicial"),
                DataOcorrenciaFinal = Request.GetDateTimeParam("DataOcorrenciaFinal"),
                CodigoTipoOcorrencia = Request.GetIntParam("TipoOcorrencia"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                CodigosFiliais = Request.GetIntParam("Filial") <= 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { Request.GetIntParam("Filial") },
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoDestino = Request.GetIntParam("Destino"),
                SituacaoEntrega = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega>("SituacaoEntrega"),
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (!Empresa.Matriz.Any())
                {
                    Repositorio.Empresa empresa = new Repositorio.Empresa(unitOfWork);
                    List<int> codigosEmpresa = empresa.BuscarCodigoMatrizEFiliais(Usuario.Empresa?.CNPJ_SemFormato);
                    filtrosPesquisa.CodigosTransportadores = codigosEmpresa?.Count > 0 ? codigosEmpresa : null;
                }
                else
                    filtrosPesquisa.CodigosTransportadores = Empresa != null ? new List<int>() { Empresa.Codigo } : null;
            }

            return filtrosPesquisa;
        }

        #endregion
    }
}
