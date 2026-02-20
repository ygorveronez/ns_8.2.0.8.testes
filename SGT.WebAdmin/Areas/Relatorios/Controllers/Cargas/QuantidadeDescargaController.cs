using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Cargas/QuantidadeDescarga")]
    public class QuantidadeDescargaController : BaseController
    {
        #region Construtores

        public QuantidadeDescargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R254_QuantidadeCargaDescarga;
        private readonly decimal _tamanhoColumaExtraPequena = 1m;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório Quantidades Descarga", "Cargas", "QuantidadeDescarga.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "NumeroCarga", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
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
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidadeDescarga filtrosPesquisa = await ObterFiltrosPesquisa(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Carga.QuantidadeDescarga servicoRelatorioCarga = new Servicos.Embarcador.Relatorios.Carga.QuantidadeDescarga(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCarga.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.QuantidadeDescarga.QuantidadeDescarga> lista, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

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
                await unitOfWork.DisposeAsync();

            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                int codigoEmpresa = Empresa?.Codigo ?? 0;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Carga.QuantidadeDescarga svrRelatorioDescarga = new Servicos.Embarcador.Relatorios.Carga.QuantidadeDescarga(unitOfWork, TipoServicoMultisoftware, Cliente);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = svcRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidadeDescarga filtrosPesquisa = await ObterFiltrosPesquisa(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = svcRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
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

        private async Task<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidadeDescarga> ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidadeDescarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidadeDescarga()
            {
                CodigosCentroDescarregamento = Request.GetListParam<int>("CentroDescarregamento"),
                CodigoModeloVeiculo = Request.GetIntParam("ModeloVeiculo"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CodigoRota = Request.GetIntParam("Rota"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                Situacao = Request.GetListEnumParam<SituacaoCargaJanelaDescarregamento>("Situacao")
            };

            int codigoFilial = Request.GetIntParam("Filial");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");

            filtrosPesquisa.CodigosFilial = codigoFilial == 0 ? await ObterListaCodigoFilialPermitidasOperadorLogisticaAsync(unitOfWork, cancellationToken) : new List<int>() { codigoFilial };
            filtrosPesquisa.CodigosTipoCarga = codigoTipoCarga == 0 ? await ObterListaCodigoTipoCargaPermitidosOperadorLogisticaAsync(unitOfWork, cancellationToken) : new List<int>() { codigoTipoCarga };
            filtrosPesquisa.CodigosTipoOperacao = await ObterListaCodigoTipoOperacaoPermitidosOperadorLogisticaAsync(unitOfWork, cancellationToken);

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            }; ;

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Operador", "Operador", _tamanhoColunaGrande, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Número da Carga", "NumeroCarga", _tamanhoColumaExtraPequena, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Dias em Atraso", "DiasAtrazo", _tamanhoColumaExtraPequena, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Descarregamento", "DataDescarregamento", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Descarregamento Programada", "DataDescarregamentoProgramada", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Início Descarregamento", "InicioDescarregamento", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Centro Descarregamento", "CentroDescarregamentoDescricao", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Nº de Entregas", "NumeroEntregas", _tamanhoColumaExtraPequena, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Expedidor", "Expedidor", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", _tamanhoColunaGrande, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Destino", "Destino", _tamanhoColunaGrande, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Cidade do Remetente", "CidadeRemetente", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Cidade do Expedidor", "CidadeExpedidor", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Rota", "Rota", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Modelo do Veículo", "ModeloVeiculo", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo da Carga", "TipoCarga", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", _tamanhoColunaGrande, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Veículos", "Veiculos", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Número do Pedido", "NumeroPedidoEmbarcador", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número do Pedido Provisório", "NumeroPedidoProvisorio", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número do Booking", "NumeroBooking", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número EXP", "NumeroEXP", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Descarregamento", "SituacaoCargaJanelaDescarregamentoDescricao", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Tempo Descarregamento", "TempoDescarregamento", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Porto de Origem", "PortoViagemOrigem", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Porto de Destino", "PortoViagemDestino", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Cliente Final", "ClienteAdicional", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Navio da Viagem", "NavioViagem", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Via Transporte", "ViaTransporte", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Despachante", "Despachante", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Possui Genset", "PossuiGenset", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Deadline", "DataDeadLineNavioViagem", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Deadline da Carga", "DataDeadLCargaNavioViagem", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Status GR", "StatusGr", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Previsão de Chegada na Planta", "DataPrevisaoChegadaOrigem", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false);
            grid.AdicionarCabecalho("CNPJ Transportador", "CnpjTransportador", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso da Carga", "PesoCarga", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Licença", "LicencaDescricao", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Ordem de Embarque", "OrdemEmbarque", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação da Ordem de Embarque", "SituacaoOrdemEmbarqueDescricao", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Rastreador", "Rastreador", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Filial", "Filial", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Filial", "CNPJFilial", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Fora do Período", "ForaPeriodoDescricao", _tamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data agendamento embarque", "DataCarregamentoProgramadaFormatada", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Data entrega pedido", "DataPrevisaoEntregaPedidoFormatada", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Previsão entrega planejada", "DataEntregaPlanejadaProximaEntregaFormatada", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Status da viagem", "StatusViagem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chegada para descarga", "DataEntradaRaioFormatada", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("KM Restante", "KmAteDestino", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Última Posição", "DataUltimaPosicaoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("SetPoint Transp.", "SetPointTransp", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Range Temp. Transp.", "RangeTempTransp", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Carga Taura", "TipoCargaTaura", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chegada Planejada", "ChegadaPlanejadaFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        #endregion
    }
}
