using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Cargas/ControleEntrega")]
    public class ControleEntregaController : BaseController
    {
		#region Construtores

		public ControleEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R283_ControleEntrega;
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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Controle de Coleta e Entrega", "Cargas", "ControleEntrega.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroCTe", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioControleEntrega filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.ControleEntrega.ControleEntrega svcRelatorioControleEntrega = new Servicos.Embarcador.Relatorios.ControleEntrega.ControleEntrega(unitOfWork, TipoServicoMultisoftware, Cliente);

                svcRelatorioControleEntrega.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.ControleEntrega> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioControleEntrega filtrosPesquisa = ObterFiltrosPesquisa();
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

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número CT-e", "NumeroCTe", TamanhoColunaPequena, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Número Carga", "NumeroCarga", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data Emissão CT-e", "DataEmissaoCTeFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Localidade do Remetente", "LocalidadeRemetente", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Localidade do Destinatário", "LocalidadeDestinatario", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Notas", "Notas", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Veículos", "Veiculos", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Motoristas", "Motoristas", TamanhoColunaGrande, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Pallets", "NumeroPallets", TamanhoColunaPequena, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Data de Agendamento", "DataAgendamentoFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoaCarga", TamanhoColunaMedia, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Observação", "Observacao", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Data da Ocorrência", "DataOcorrenciaFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Descrição Ocorrência", "DescricaoOcorrencia", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Centro Resultado", "DescricaoCentroResultado", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nro. Pedido Embarcador", "NumeroPedidoEmbarcador", TamanhoColunaPequena, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Data Previsão Coleta", "DataPrevisaoColeta", TamanhoColunaMedia, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Data Previsão Entrega", "DataPrevisaoEntrega", TamanhoColunaMedia, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Senha Agendamento", "SenhaAgendamento", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Observação Ocorrência", "ObservacaoOcorrencia", TamanhoColunaGrande, Models.Grid.Align.left, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioControleEntrega ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioControleEntrega()
            {
                DataOcorrenciaInicial = Request.GetDateTimeParam("DataOcorrenciaInicial"),
                DataOcorrenciaFinal = Request.GetDateTimeParam("DataOcorrenciaFinal"),
                CodigosTipoOcorrencia = Request.GetListParam<int>("TipoOcorrencia"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoas"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroNotaFiscal = Request.GetStringParam("NumeroNotaFiscal"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculo"),
                CodigosMotoristas = Request.GetListParam<int>("Motorista"),
                DataPrevisaoEntregaInicial = Request.GetDateTimeParam("DataPrevisaoEntregaPedidoInicial"),
                DataPrevisaoEntregaFinal = Request.GetDateTimeParam("DataPreviscaoEntregaPedidoFinal"),
                UFsDestino = Request.GetListParam<string>("UFDestino"),
                UFsOrigem = Request.GetListParam<string>("UFOrigem")
            };
        }

        #endregion
    }
}
