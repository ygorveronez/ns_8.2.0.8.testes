using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.TorreControle;
using Repositorio;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.TorreControle
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/TorreControle/ConsultaPorNotaFiscal")]
    public class ConsultaPorNotaFiscalController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal>
    {
		#region Construtores

		public ConsultaPorNotaFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Atributos

		private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R271_TorreControleConsultaPorNotaFiscal;
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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Consulta por Nota Fiscal - Torre de Controle", "TorreControle", "ConsultaPorNotaFiscal.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Numero", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.TorreControle.ConsultaPorNotaFiscal servicoRelatorioConsultaPorNotaFiscal = new Servicos.Embarcador.Relatorios.TorreControle.ConsultaPorNotaFiscal(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioConsultaPorNotaFiscal.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.TorreControle.RelatorioConsultaPorNotaFiscal> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal()
            {
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataCarregamentoInicial = Request.GetNullableDateTimeParam("DataCarregamentoInicial"),
                DataCarregamentoFinal = Request.GetNullableDateTimeParam("DataCarregamentoFinal"),
                DataPrevisaoEntregaInicial = Request.GetNullableDateTimeParam("DataPrevisaoEntregaInicial"),
                DataPrevisaoEntregaFinal = Request.GetNullableDateTimeParam("DataPrevisaoEntregaFinal"),
                DataAgendamentoInicial = Request.GetNullableDateTimeParam("DataAgendamentoInicial"),
                DataAgendamentoFinal = Request.GetNullableDateTimeParam("DataAgendamentoFinal"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroNota = Request.GetIntParam("NumeroNota"),
                SituacaoAgendamento = Request.GetNullableEnumParam<SituacaoAgendamentoEntregaPedido>("SituacaoAgendamento"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CnpjCpfCliente = Request.GetDoubleParam("Cliente"),
                Filial = Request.GetIntParam("Filial"),
                Expedidor = Request.GetDoubleParam("Expedidor"),
                Recebedor = Request.GetDoubleParam("Recebedor"),
                TipoTrecho = Request.GetIntParam("TipoTrecho")
            };
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Nota Fiscal", "Numero", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Nº Carga", "Carga", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Situação NF-e", "SituacaoNotaFiscal", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Situação Agendamento", "SituacaoAgendamentoDescricao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação Reagendamento", "ObservacaoReagendamento", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Cliente", "ClienteDescricao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Destino", "Destino", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "TransportadorDescricao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Sugestão Data de Entrega", "SugestaoDataEntregaFormatada", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data de Agendamento", "DataAgendamentoFormatada", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data de Reagendamento", "DataReagendamentoFormatada", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Término de Carregamento", "DataTerminoCarregamentoFormatada", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Início de Entrega", "DataInicioEntregaFormatada", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Fim de Entrega", "DataFimEntregaFormatada", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Previsão de Entrega", "DataPrevisaoEntregaFormatada", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Trecho", "Trecho", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("CNPJ Filial Emissora", "CNPJFilialEmissoraFormatado", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Nome fantasia do expedidor", "NomeFantasia", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Categoria do expedidor", "CategoriaExpedidor", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Nº Pedido Embarcador", "NumeroPedidoEmbarcador", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Cidade de Origem", "CidadeOrigemPedido", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Código Cliente", "CodigoCliente", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Nome Destinatário", "NomeCliente", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Cidade destinatário", "CidadeDestinatario", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Endereço destinatário", "ClienteEndereco", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Placa", "PlacaVeiculo", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Código integração do transportador", "TransportadorCodigoIntegracao", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("ETA", "DataEntregaPrevistaFormatada", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Ordem prevista", "OrdemPrevista", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Analista responsável pelo monitoramento", "AnalistaResponsavelMonitoramento", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação entrega (data previsão entrega)", "SituacaoEntregaPrevisaoDescricao", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Peso da Carga", "PesoCarga", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Sequencia da entrega", "SequenciaEntrega", _tamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);

            grid.AdicionarCabecalho("Situação da Viagem", "SituacaoViagem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Contato Cliente", "ContatoCliente", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Contato Transportador", "ContatoTransportador", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Entrega (data de agendamento)", "SituacaoEntregaDescricao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Entrega NF-e", "SituacaoEntregaNotaFiscalDescricao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Responsável Agenda", "ResponsavelAgenda", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Holding", "Holding", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Categoria Destinatário", "Categoria", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Motivo Reagendamento", "MotivoReagendamento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("ISIS Return", "ISISReturn", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Filial", "NomeFilial", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Filial", "CNPJFilialFormatado", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Responsável Reagendamento", "ResponsavelReagendamento", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo do Veículo", "ModeloVeicular", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Observação Motivo Reagendamento", "ObservacaoMotivoReagendamento", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Rota Frete", "RotaFreteDescricao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Codigo de integração da Rota Frete", "RotaFreteCodigoIntegracao", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);

            return grid;
        }

        protected override Task<FiltroPesquisaRelatorioConsultaPorNotaFiscal> ObterFiltrosPesquisaAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
