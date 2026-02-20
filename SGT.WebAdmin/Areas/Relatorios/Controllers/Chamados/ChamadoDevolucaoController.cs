using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Chamados
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Chamados/ChamadoDevolucao")]
    public class ChamadoDevolucaoController : BaseController
    {
        #region Construtores

        public ChamadoDevolucaoController(Conexao conexao) : base(conexao) { }

        #endregion

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R324_ChamadoDevolucao;

        private const decimal TamanhoColunaPequena = 7m;
        private const decimal TamanhoColunaMedia = 10m;
        private const decimal TamanhoColunaGrande = 15m;
        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Chamados de Devolução", "Chamados", "ChamadoDevolucao.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", Codigo, unitOfWork, false, false);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
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

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamadoDevolucao filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork, configuracaoTMS);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.Chamado.ChamadoDevolucao servicoRelatorioChamadoDevolucao = new Servicos.Embarcador.Relatorios.Chamado.ChamadoDevolucao(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioChamadoDevolucao.ExecutarPesquisa(
                    out List<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoDevolucao.ChamadoDevolucao> listaChamadoDevolucao,
                    out int countRegistros,
                    filtrosPesquisa,
                    agrupamentos,
                    parametrosConsulta);

                grid.AdicionaRows(listaChamadoDevolucao);
                grid.setarQuantidadeTotal(countRegistros);

                return new JsonpResult(grid);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamadoDevolucao filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork, configuracaoTMS);

                await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
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

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("DevolucaoParcial", false);
            grid.AdicionarCabecalho("Número CT-e", "NumeroCte", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Nota Fiscal Devolução", "NfDevolucao", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Nota Fiscal Origem", "NfOrigem", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Motivo Devolução", "MotivoDevolucao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo de Devolução", "TipoDevolucao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Origens", "Origens", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destinos", "Destinos", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Veículos", "Veiculos", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Quantidade Devolução", "QuantidadeDevolucao", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor Devolução", "ValorDevolucao", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor Total Mercadoria", "ValorTotalMercadorias", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Número do Atendimento", "NumeroAtendimento", 7, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Data Abertura Atendimento", "DataAberturaAtendimentoFormatado", 7, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Número da Carga", "NumeroCarga", 7, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Grupo Tomador da Carga", "GrupoTomadorCarga", 7, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Responsável pela Devolução", "ResponsavelAtendimento", 7, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Quantidade Devolvida", "QuantidadeDevolvida", 7, Models.Grid.Align.left, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamadoDevolucao ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configGeral)
        {
            int codigoFilial = Request.GetIntParam("Filial");

            return new Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamadoDevolucao()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoResponsavel = Request.GetIntParam("Responsavel"),
                CodigoFilial = codigoFilial,
                CodigosMotivo = Request.GetListParam<int>("Motivo"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoRepresentante = Request.GetIntParam("Representante"),
                Nota = Request.GetIntParam("NF"),
                NumeroCTe = Request.GetIntParam("CTE"),
                CpfCnpjCliente = Request.GetDoubleParam("Cliente"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CodigoGrupoPessoasCliente = Request.GetIntParam("GrupoPessoasCliente"),
                CodigoGrupoPessoasTomador = Request.GetIntParam("GrupoPessoasTomador"),
                CodigoGrupoPessoasDestinatario = Request.GetIntParam("GrupoPessoasDestinatario"),
                Placa = Request.GetStringParam("Placa"),
                Carga = Request.GetStringParam("Carga"),
                GerouOcorrencia = Request.GetBoolParam("GerouOcorrencia"),
                Situacao = Request.GetEnumParam<SituacaoChamado>("SituacaoChamado"),
                DataCriacaoInicio = Request.GetDateTimeParam("DataCriacaoInicio"),
                DataCriacaoFim = Request.GetDateTimeParam("DataCriacaoFim"),
                DataFinalizacaoInicio = Request.GetDateTimeParam("DataFinalizacaoInicio"),
                DataFinalizacaoFim = Request.GetDateTimeParam("DataFinalizacaoFim"),
                CodigoFilialVenda = Request.GetIntParam("FilialVenda"),
                DataInicialChegadaDiaria = Request.GetDateTimeParam("DataInicialChegadaDiaria"),
                DataFinalChegadaDiaria = Request.GetDateTimeParam("DataFinalChegadaDiaria"),
                DataInicialSaidaDiaria = Request.GetDateTimeParam("DataInicialSaidaDiaria"),
                DataFinalSaidaDiaria = Request.GetDateTimeParam("DataFinalSaidaDiaria"),
                CpfCnpjClienteResponsavel = Request.GetListParam<double>("ClienteResponsavel"),
                CodigosGrupoPessoasResponsavel = Request.GetListParam<int>("GrupoPessoasResponsavel"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                SomenteAtendimentoEstornado = Request.GetBoolParam("SomenteAtendimentoEstornado"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                PossuiAnexoNFSe = Request.GetNullableBoolParam("PossuiAnexoNFSe"),
                Filiais = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial },
                Recebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                FiltrarCargasPorParteDoNumero = configGeral?.FiltrarCargasPorParteDoNumero ?? false
            };
        }

        #endregion
    }
}
