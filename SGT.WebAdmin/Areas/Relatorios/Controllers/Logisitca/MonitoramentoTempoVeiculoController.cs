using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Logisitca
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Logistica/MonitoramentoTempoVeiculo")]
    public class MonitoramentoTempoVeiculoController : BaseController
    {
		#region Construtores

		public MonitoramentoTempoVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R238_MonitoramentoTempoVeiculo;
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

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Tempos de Veículos", "Logistica", "MonitoramentoTempoVeiculo.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

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
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoTempoVeiculo filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.Monitoramento repositorio = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaRelatorioTempoVeiculo(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoTempoVeiculo> lista = totalRegistros > 0 ? repositorio.ConsultarRelatorioTempoVeiculo(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoTempoVeiculo>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

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
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoTempoVeiculo filtrosPesquisa = ObterFiltrosPesquisa( unitOfWork);
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
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

        private async Task GerarRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoTempoVeiculo filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Logistica.Monitoramento repositorio = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoTempoVeiculo> dataSource = repositorio.ConsultarRelatorioTempoVeiculo(filtrosPesquisa, propriedades, parametrosConsulta);
                List<Parametro> parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa, parametrosConsulta);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Logistica/MonitoramentoTempoVeiculo",parametros,relatorioControleGeracao, relatorioTemporario, dataSource, unitOfWork, null, null, true, TipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoTempoVeiculo ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            List<int> codigosFiliais = Request.GetListParam<int>("Filial");
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoTempoVeiculo()
            {
                DataInicioEntregaInicial = Request.GetDateTimeParam("DataInicioEntregaInicial"),
                DataInicioEntregaFinal = Request.GetDateTimeParam("DataInicioEntregaFinal"),
                DataEntregaInicial = Request.GetDateTimeParam("DataEntregaInicial"),
                DataEntregaFinal = Request.GetDateTimeParam("DataEntregaFinal"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                CodigosFilial = codigosFiliais.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork): codigosFiliais,
                Recebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                CodigosCarga = Request.GetListParam<int>("Carga"),
                CodigosClienteEntrega = Request.GetListParam<double>("ClienteEntrega"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoDestino = Request.GetIntParam("Destino")
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Número do Pedido Provisório", "NumeroPedidoProvisorio", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Carga", "NumeroCarga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Filial", "Filial", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Natureza", "Natureza", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Pedido", "Pedido", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número EXP", "NumeroEXP", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Cliente", "Cliente", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("UF Destino", "UFDestino", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("País Destino", "PaisDestino", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Coleta", "DataColeta", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tração", "Veiculo", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Reboque", "Reboque", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Reboque", "ModeloReboque", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeiculo", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Previsão Chegada", "DataPrevisaoChegadaOrigemFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Início Entrega Reprogramada", "DataInicioEntregaReprogramada", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Início Entrega", "DataInicioEntrega", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Entrega", "DataEntrega", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Status Viagem", "StatusViagem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Início Viagem", "DataInicioViagem", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Previsão Entrega", "DataPrevisaoEntrega", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Entrega Recalculada", "DataEntregaRecalculada", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Chegada Cliente", "DataEntradaRaio", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Saída Cliente", "DataSaidaRaio", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Entrega", "SituacaoEntregaFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("% Viagem", "PercentualViagem", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Distância", "Distancia", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("KM Restante", "KMRestante", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso Total", "PesoTotal", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Notas", "NumeroNotas", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Notas", "ValorNotas", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Frete a Pagar", "ValorFreteAPagar", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Deadline", "DataDeadLineNavioViagem", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Deadline da Carga", "DataDeadLCargaNavioViagem", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Taxa Ocupação Carregado", "TaxaOcupacaoCarregado", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tabela de Frete", "TabelaFrete", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Capacidade KG Veículo", "CapacidadeKGVeiculo", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);//.NumberFormat("n0");

            grid.AdicionarCabecalho("Tempo Deslocamento para Planta", "TempoDeslocamentoParaPlanta", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tempo Aguardando Horário Carregamento", "TempoAguardandoHorarioCarregamento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tempo Aguardando Carregamento", "TempoAguardandoCarregamento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tempo em Carregamento", "TempoEmCarregamento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tempo em Liberação", "TempoEmLiberacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tempo Trânsito", "TempoTransito", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tempo Aguardando Horário Descarga", "TempoAguardandoHorarioDescarga", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tempo Aguardando Descarga", "TempoAguardandoDescarga", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tempo Descarga", "TempoDescarga", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Data Criação Carga", "DataCriacaoCargaFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Salvamento Dados Transporte", "DataSalvamentoDadosTransporteFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Confirmação Envio Documentos", "DataConfirmacaoEnvioDocumentosFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Confirmação Valor Frete", "DataConfirmacaoValorFreteFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Início Emissão Documentos", "DataInicioEmissaoDocumentosFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Fim Emissão Documentos", "DataFimEmissaoDocumentosFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Envio CT-e Ocorrência", "DataEnvioCTeOcorrenciaFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            //grid.AdicionarCabecalho("Data Previsão Saída Ajustada", "DataPrevisaoSaidaPedidoAjustadaFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            //grid.AdicionarCabecalho("Data Previsão Entrega Pedido Ajustada", "DataPrevisaoEntregaPedidoAjustadaFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Previsão Entrega Pedido Recalculada", "DataPrevisaoEntregaPedidoRecalculadaFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Cidade Origem do Pedido", "CidadeOrigemPedido", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Cidade Destino do Pedido", "CidadeDestinoPedido", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Cliente Origem do Pedido", "ClienteOrigemPedido", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Cliente Destino do Pedido", "ClienteDestinoPedido", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo Pessoas (TOMADOR)", "GrupoPessoasTomador", _tamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);

            return grid;
        }

        private List<Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoTempoVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFilial.Count > 0 ? repFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFilial) : new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            List<Dominio.Entidades.Empresa> transportadores = filtrosPesquisa.CodigosTransportador.Count > 0 ? repEmpresa.BuscarPorCodigos(filtrosPesquisa.CodigosTransportador) : new List<Dominio.Entidades.Empresa>();
            List<Dominio.Entidades.Cliente> clientesEntrega = filtrosPesquisa.CodigosClienteEntrega.Count > 0 ? repCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CodigosClienteEntrega) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = filtrosPesquisa.CodigosCarga.Count > 0 ? repCarga.BuscarPorCodigos(filtrosPesquisa.CodigosCarga) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            Dominio.Entidades.Localidade origem = filtrosPesquisa.CodigoOrigem > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoOrigem) : null;
            Dominio.Entidades.Localidade destino = filtrosPesquisa.CodigoDestino > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoDestino) : null;

            parametros.Add(new Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar));
            parametros.Add(new Parametro("DataInicioEntrega", filtrosPesquisa.DataInicioEntregaInicial, filtrosPesquisa.DataInicioEntregaFinal));
            parametros.Add(new Parametro("DataEntrega", filtrosPesquisa.DataEntregaInicial, filtrosPesquisa.DataEntregaFinal));
            parametros.Add(new Parametro("DataEmissao", filtrosPesquisa.DataEmissaoInicial, filtrosPesquisa.DataEmissaoFinal));
            parametros.Add(new Parametro("Filial", filiais.Select(o => o.Descricao)));
            parametros.Add(new Parametro("Transportador", transportadores.Select(o => o.RazaoSocial)));
            parametros.Add(new Parametro("Origem", origem?.DescricaoCidadeEstado));
            parametros.Add(new Parametro("Destino", destino?.DescricaoCidadeEstado));
            parametros.Add(new Parametro("ClienteEntrega", clientesEntrega.Select(o => o.Nome)));
            parametros.Add(new Parametro("Carga", cargas.Select(o => o.CodigoCargaEmbarcador)));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Contains("Formatada"))
                return propriedadeOrdenar.Replace("Formatada", "");

            return propriedadeOrdenar;
        }

        #endregion
    }
}
