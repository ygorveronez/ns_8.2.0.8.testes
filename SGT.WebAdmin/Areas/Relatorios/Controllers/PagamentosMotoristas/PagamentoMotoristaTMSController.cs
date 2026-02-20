using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.PagamentosMotoristas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/PagamentosMotoristas/PagamentoMotoristaTMS")]
    public class PagamentoMotoristaTMSController : BaseController
    {
		#region Construtores

		public PagamentoMotoristaTMSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R225_PagamentoMotoristaTMS;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Pagamento ao Motorista", "PagamentosMotoristas", "PagamentoMotoristaTMS.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
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

                Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.PagamentosMotoristas.PagamentosMotoristas servicoRelatorioPagamentoMotorista = new Servicos.Embarcador.Relatorios.PagamentosMotoristas.PagamentosMotoristas(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioPagamentoMotorista.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista.PagamentoMotorista> listaPagamentos, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaPagamentos);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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

                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = svcRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = svcRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS
            {
                NumeroPagamento = Request.GetIntParam("NumeroPagamento"),
                NumeroCarga = Request.GetIntParam("NumeroCarga"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataEfetivacaoInicial = Request.GetNullableDateTimeParam("DataEfetivacaoInicial"),
                DataEfetivacaoFinal = Request.GetNullableDateTimeParam("DataEfetivacaoFinal"),
                Situacao = Request.GetEnumParam<SituacaoPagamentoMotorista>("Situacao"),
                Etapa = Request.GetEnumParam<EtapaPagamentoMotorista>("Etapa"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CodigosTipoPagamento = Request.GetListParam<int>("TipoPagamento"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                PagamentosSemAcertoViagem = Request.GetBoolParam("PagamentosSemAcertoViagem"),
                CpfCnpjFavorecido = Request.GetDoubleParam("Favorecido"),
                NumeroDocumento = Request.GetIntParam("NumeroDocumento"),
                Tomador = Request.GetDoubleParam("Tomador"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nº Pagamento", "NumeroPagamento", 6, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "SituacaoDescricao", 12, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Etapa", "EtapaDescricao", 12, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Carga", "Carga", 6, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.left, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Data de Lançamento", "DataLancamentoFormatada", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Pagamento", "DataPagamentoFormatada", 10, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Tipo Pagamento", "TipoPagamento", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Plano Saída", "PlanoSaida", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Plano Entrada", "PlanoEntrada", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Operador", "Operador", 15, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CPF Motorista", "CPFMotoristaFormatado", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Acerto", "Acerto", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Favorecido", "Favorecido", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Efetivação", "DataEfetivacaoFormatada", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Número do Documento", "NumeroDocumento", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Número do Documento Complementar", "NumeroDocumentoComplementar", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ do Tomador", "CNPJTomadorFormatado", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor IRRF", "ValorIRRF", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor INSS", "ValorINSS", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor SEST", "ValorSEST", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor SENAT", "ValorSENAT", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Líquido", "ValorLiquido", 10, Models.Grid.Align.center, true, false, false, false, false);

            if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
            {
                grid.AdicionarCabecalho("Moeda", "MoedaDescricao", 10, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Moeda", "ValorMoeda", 10, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Original Moeda", "ValorOriginalMoeda", 10, Models.Grid.Align.left, false, false, false, false, false);
            }

            return grid;
        }

        #endregion
    }
}
