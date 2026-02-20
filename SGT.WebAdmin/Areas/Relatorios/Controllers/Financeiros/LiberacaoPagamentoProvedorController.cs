using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/LiberacaoPagamentoProvedor")]
    public class LiberacaoPagamentoProvedorController : BaseController
    {
		#region Construtores

		public LiberacaoPagamentoProvedorController(Conexao conexao) : base(conexao) { }

		#endregion

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R338_LiberacaoPagamentoProvedor;

        private readonly decimal _TamanhoColunaExtraPequena = 1m;
        private readonly decimal _TamanhoColunaPequena = 1.75m;
        private readonly decimal _TamanhoColunaGrande = 5.50m;
        private readonly decimal _TamanhoColunaMedia = 3m;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Liberação Pagamento Provedor", "Financeiros", "LiberacaoPagamentoProvedor.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "DataCargaInicial", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.LiberacaoPagamentoProvedor servicoLiberacaoPagamentoProvedor = new Servicos.Embarcador.Relatorios.Financeiros.LiberacaoPagamentoProvedor(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoLiberacaoPagamentoProvedor.ExecutarPesquisa(out List< Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioLiberacaoPagamentoProvedor> listaLoadLiberacaoPagamentoProvedor, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaLoadLiberacaoPagamentoProvedor);

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor filtrosPesquisa = ObterFiltrosPesquisa();
                await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerar);
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
            grid.AdicionarCabecalho("Número da Carga", "NumeroCarga", _TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Número da OS", "NumeroOS", _TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data de criação da carga", "DataCriacaoCargaFormatada", _TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo de operação", "TipoOperacao", _TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Cidade Origem da carga", "CidadeOrigemCarga", _TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Estado Origem da carga", "EstadoOrigemCarga", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Cidade Destino da carga", "CidadeDestinoCarga", _TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Estado Destino da carga", "EstadoDestinoCarga", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nome Filial Emissora da carga", "NomeFilialEmissoraCarga", _TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Filial Emissora da carga", "CNPJFilialEmissoraCargaFormatado", _TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nome Provedor", "NomeProvedor", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Provedor", "CNPJProvedorFormatado", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nome Tomador da carga", "NomeTomadorCarga", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Tomador da carga", "CNPJTomadorCargaFormatado", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Total Estimado Pagamento", "ValorTotalEstimadoPagamento", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Etapa Liberação Pagamento Provedor", "EtapaLiberacaoPagamentoProvedorDescricao", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Situação Liberação Pagamento Provedor", "SituacaoLiberacaoPagamentoProvedorDescricao", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Número do documento do provedor", "NumeroDocumentoProvedor", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo do documento do provedor", "TipoDocumentoProvedorDescricao", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data de emissão do documento do provedor", "DataEmissaoDocumentoProvedorFormatada", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Números dos Múltiplos Documentos do Provedor", "NumerosDosMultiplosDocumentosProvedor", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Número do Documento de Cobrança Contra o Cliente", "NumeroDocumentoCobrancaContraCliente", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Documento de Cobrança Contra o Cliente", "TipoDocumentoCobrancaContraClienteDescricao", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Total do Documento de Cobrança contra o cliente", "ValorTotalDocumentoCobrancaContraClienteFormatado", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data de emissão do documento de cobrança contra o cliente", "DataEmissaoDocumentoCobrancaContraClienteFormatada", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Total Real Pagamento CTe", "ValorTotalRealPagamentoCTe", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Total Real Pagamento NFSe", "ValorTotalRealPagamentoNFSe", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Indicação Liberação OK", "IndicacaoLiberacaoOKDescricao", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Justificativa Aprovação/Rejeição", "JustificativaAprovacao", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota ICMS Provedor", "AliquotaCTeProvedor", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor ICMS Provedor ", "ValorICMSProvedor", _TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor()
            {
                DataCargaInicial = Request.GetDateTimeParam("DataCargaInicial"),
                DataCargaFinal = Request.GetDateTimeParam("DataCargaFinal"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoProvedor = Request.GetDoubleParam("Provedor"),
                CodigoFilialEmissora = Request.GetIntParam("FilialEmissora"),
                CodigoTomador = Request.GetDoubleParam("Tomador"),
                SituacaoLiberacaoPagamentoProvedor = Request.GetEnumParam<SituacaoLiberacaoPagamentoProvedor>("SituacaoLiberacaoPagamentoProvedor"),
                TipoDocumentoProvedor = Request.GetEnumParam<TipoDocumentoProvedor>("TipoDocumentoProvedor"),
                IndicacaoLiberacaoOK = Request.GetEnumParam<OpcaoSimNaoPesquisa>("IndicacaoLiberacaoOK"),
                EtapaLiberacaoPagamentoProvedor = Request.GetEnumParam<EtapaLiberacaoPagamentoProvedor>("EtapaLiberacaoPagamentoProvedor")
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
