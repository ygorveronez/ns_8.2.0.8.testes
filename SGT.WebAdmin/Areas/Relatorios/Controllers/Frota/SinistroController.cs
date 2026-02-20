using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frota
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frota/Sinistro")]
    public class SinistroController : BaseController
    {
		#region Construtores

		public SinistroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R305_Sinistro;

        private readonly decimal TamanhoColunaPequena = 1.75m;
        private readonly decimal TamanhoColunaGrande = 5.50m;
        private readonly decimal TamanhoColunaMedia = 3m;


        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Sinistros", "Frota", "RelatorioSinistros.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Frota.Sinistro servicoRelatorioSinistro = new Servicos.Embarcador.Relatorios.Frota.Sinistro(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioSinistro.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Frota.Sinistro> listaSinistro, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaSinistro);

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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
            grid.AdicionarCabecalho("Número do Sinistro", "NumeroSinistro", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Causador do Sinistro", "CausadorSinistroDescricao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo do Sinistro", "TipoSinistro", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Número do B.O", "NumeroBoletimOcorrencia", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoDados", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data e Hora do Sinistro", "DataHoraSinistro", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Endereço", "Endereco", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Local", "Local", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Cidade", "Cidade", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Placa Cavalo", "PlacaCavalo", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Placa Carreta/Reboque", "PlacaReboque", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Tipo", "TipoDescricao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nome", "Nome", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF", "CPF", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Telefone Contato 1", "TelefoneContato1", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Telefone Contato 2", "TelefoneContato2", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Observação", "Observacao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Número da Ordem de Serviço", "NumeroOrdemServico", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Local de Manutenção", "LocalManutencao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação da OS", "SituacaoOSDescricao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Indicação do Pagador", "IndicacaoPagadorDescricao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo de Movimento", "Movimento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data de Emissão", "DataEmissaoTitulo", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data de Vencimento", "DataVencimentoTitulo", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Número do Documento", "NumeroDocumento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Original", "ValorOriginal", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Observação Título", "ObservacaoTitulo", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Documento de Entrada", "DocumentoEntrada", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Situação do Sinistro", "SituacaoSinistroDescricao", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro()
            {
                NumeroSinistro = Request.GetIntParam("NumeroSinistro"),
                CausadorSinistro = Request.GetEnumParam<CausadorSinistro>("CausadorSinistro"),
                CodigoTipoSinistro = Request.GetIntParam("TipoSinistro"),
                NumeroBoletimOcorrencia = Request.GetStringParam("NumeroBoletimOcorrencia"),
                DataSinistroInicial = Request.GetDateTimeParam("DataInicial"),
                DataSinistroFinal = Request.GetDateTimeParam("DataFinal"),
                CodigoCidade = Request.GetIntParam("Cidade"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoVeiculoReboque = Request.GetIntParam("VeiculoReboque"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                NumeroOrdemServico = Request.GetIntParam("NumeroOrdemServico"),
                IndicacaoPagador = Request.GetNullableEnumParam<IndicadorPagadorSinistro>("IndicacaoPagador"),
                SituacaoSinistro = Request.GetNullableEnumParam<TipoHistoricoInfracao>("SituacaoSinistro"),
            };
        }

        #endregion
    }
}
