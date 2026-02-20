using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Chamados
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Chamados/ChamadoOcorrencia")]
    public class ChamadoOcorrenciaController : BaseController
    {
        #region Construtores

        public ChamadoOcorrenciaController(Conexao conexao) : base(conexao) { }

        #endregion

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R119_ChamadosOcorrencia;

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

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Chamados", "Chamados", "ChamadoOcorrencia.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, false, false);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(configuracaoTMS), relatorio);

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

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigGeral = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configGeral = await repConfigGeral.BuscarConfiguracaoPadraoAsync();

                Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamado filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork, configGeral);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);


                Servicos.Embarcador.Relatorios.Chamado.ChamadoOcorrencia servicoRelatorioChamadoOcorrencia = new Servicos.Embarcador.Relatorios.Chamado.ChamadoOcorrencia(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioChamadoOcorrencia.ExecutarPesquisa(
                    out List<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.ChamadoOcorrencia> listaChamadosOcorrencia,
                    out int countRegistros,
                    filtrosPesquisa,
                    agrupamentos,
                    parametrosConsulta);

                grid.AdicionaRows(listaChamadosOcorrencia);
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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigGeral = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configGeral = await repConfigGeral.BuscarConfiguracaoPadraoAsync();
                Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamado filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork, configGeral);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", TamanhoColunaPequena, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Carga", "Carga", TamanhoColunaPequena, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Motivo Chamado", "MotivoChamado", TamanhoColunaGrande, Models.Grid.Align.left, true);
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                grid.AdicionarCabecalho("Transportador", "Transportador", TamanhoColunaGrande, Models.Grid.Align.left, true);
            }
            grid.AdicionarCabecalho("Cliente", "Cliente", TamanhoColunaGrande, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tomador", "Tomador", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Responsável da Ocorrência", "DescricaoResponsavelOcorrencia", TamanhoColunaMedia, Models.Grid.Align.left, true, false);

            if (configuracaoTMS.TipoChamado == TipoChamado.PadraoEmbarcador)
            {
                grid.AdicionarCabecalho("Responsável do Chamado", "DescricaoResponsavelChamado", TamanhoColunaMedia, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Pallets", "NumeroPallet", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Veículo Carregado", "DescricaoVeiculoCarregado", TamanhoColunaPequena, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Representante", false);
            }
            else
            {
                grid.AdicionarCabecalho("Representante", "Representante", TamanhoColunaMedia, Models.Grid.Align.left, true, false);
                grid.AdicionarCabecalho("DescricaoResponsavelChamado", false);
                grid.AdicionarCabecalho("NumeroPallet", false);
                grid.AdicionarCabecalho("DescricaoVeiculoCarregado", false);
            }
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunaMedia, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunaMedia, Models.Grid.Align.left, true, false);

            grid.AdicionarCabecalho("Aos Cuidados Do", "DescricaoAosCuidadosDo", TamanhoColunaMedia, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Observacao", "Observacao", TamanhoColunaGrande, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Tempo Total", "TempoTotal", TamanhoColunaMedia, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Data Criação", "DataCriacaoFormatado", TamanhoColunaMedia, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data Retorno", "DataRetornoFormatado", TamanhoColunaMedia, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", TamanhoColunaPequena, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho("Placa", "Placa", TamanhoColunaGrande, Models.Grid.Align.left, true, false);

            grid.AdicionarCabecalho("Núm. Ocorrência", "NumeroOcorrencia", TamanhoColunaPequena, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Motivo da Ocorrência", "MotivoOcorrencia", TamanhoColunaGrande, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Data Finalização", "DataFinalizacaoFormatado", TamanhoColunaMedia, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data Carregamento", "DataCarregamentoFormatado", TamanhoColunaMedia, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Data Entrega", "DataEntregaFormatado", TamanhoColunaMedia, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Atraso Carga", "StatusAtrasoCarga", TamanhoColunaPequena, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Notas", "Notas", TamanhoColunaGrande, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("CT-es", "CTes", TamanhoColunaGrande, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", TamanhoColunaGrande, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Valor Chamado", "ValorChamado", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF/CNPJ Destinatário", "CNPJDestinatario", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Endereço Cliente", "EnderecoCliente", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Bairro Cliente", "BairroCliente", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Cidade Cliente", "CidadeCliente", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Estado Cliente", "EstadoCliente", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CEP Cliente", "CEPCliente", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Latitude Cliente", "LatitudeCliente", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Longitude Cliente", "LongitudeCliente", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Data Criação Carga", "DataCriacaoCargaFormatado", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Operação Carga", "TipoOperacaoCarga", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Filial Carga", "FilialCarga", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Filial de Venda", "FilialVenda", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            }
            grid.AdicionarCabecalho("Responsável", "Responsavel", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tempo Atendimento", "TempoAtendimentoFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Primeira Assumição Atendimento", "DataAssumicaoPrimeiroAtendimentoFormatado", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Usuário/Operador", "Operador", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Desconto", "ValorDesconto", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Total de Horas", "TotalDeHoras", TamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Observação Análise", "ObservacaoAnalise", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Chega Diária", "DataChegadaDiariaFormatada", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Saída Diária", "DataSaidaDiariaFormatada", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);

            // Diária automática
            grid.AdicionarCabecalho("Valor Automático", "ValorDiariaAutomatica", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Total de Horas Automático", "TotalDeHorasDiariaAutomatica", 4, Models.Grid.Align.center, true, false, false, false, false);

            grid.AdicionarCabecalho("Grupo do Cliente", "GrupoPessoasCliente", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Grupo do Tomador", "GrupoPessoasTomador", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Grupo do Destinatário", "GrupoPessoasDestinatario", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Tipo Motivo Chamado", "DescricaoTipoMotivoChamado", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Integração Cliente", "CodigoIntegracaoCliente", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Integração Tomador", "CodigoIntegracaoTomador", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Integração Destinatário", "CodigoIntegracaoDestinatario", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veicular Carga", "ModeloVeicularCarga", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Chegada Motorista", "DataChegadaMotoristaFormatada", TamanhoColunaGrande, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Tratativa Atendimento", "DescricaoTratativaAtendimento", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Código Integração Cliente Responsável", "CodigoIntegracaoClienteResponsavel", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !configuracaoTMS.ExigirClienteResponsavelPeloAtendimento);
            grid.AdicionarCabecalho("CPF/CNPJ Cliente Responsável", "CNPJClienteResponsavelFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !configuracaoTMS.ExigirClienteResponsavelPeloAtendimento);
            grid.AdicionarCabecalho("Cliente Responsável", "ClienteResponsavel", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !configuracaoTMS.ExigirClienteResponsavelPeloAtendimento);

            grid.AdicionarCabecalho("Grupo Pessoas Responsável", "GrupoPessoasResponsavel", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho("Data Retenção Início", "DataRetencaoInicioFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho("Data Retenção Fim", "DataRetencaoFimFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho("Data Reentrega", "DataReentregaFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho("Peso", "PesoCarga", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Período Janela Descarga", "PeriodoJanelaDescarga", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false).Ocultar(!configuracaoTMS.VisualizarDatasRaioNoAtendimento);
            grid.AdicionarCabecalho("Tipo de devolução", "TipoDevolucao", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Rota", "DescricaoRota", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Numero pedido embarcador", "NumeroPedidoEmbarcador", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Cliente", "CNPJClienteFormatado", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Justificativa Ocorrência", "JustificativaOcorrencia", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Prevista de Entrega/Retorno", "DataPrevistaEntregaRetorno", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Anexo NFS-e", "PossuiAnexoNFSe", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Gênero", "GeneroMotivoChamado", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Área Envolvida", "AreaEnvolvidaMotivoChamado", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Motivo Custo", "MotivoProcesso", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantidade", "QuantidadeDivergencia", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veicular Entrega", "ModeloVeicularCargaEntrega", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo Tipo de Ocorrência", "GrupoTipoOcorrencia", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Anexos", "Anexos", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Motivo Devolução", "MotivoDevolucao", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Documento Complementar", "DocumentoComplementar", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Já Incluso", "ValorJaIncluso", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("NF Atendimento", "NFAtendimento", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("CPF Motorista", "CPFMotorista", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Motorista", "TipoMotorista", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código SIF", "CodigoSIF", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Descrição SIF", "DescricaoSIF", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Dev. Item", "ValorDevItem", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Dev. Nota", "ValorDevNota", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Dev. Total", "ValorDevTotal", TamanhoColunaMedia, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor da Ocorrência", "ValorOcorrencia", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Codigo do Produto", "CodigoProduto", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Descrição do Produto", "DescricaoProduto", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Quantidade Devolução", "QuantidadeDevolucao", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Valor Devolução", "ValorDevolucao", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Numero NFD", "NumeroNFD", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Serie NFD", "SerieNFD", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Chave NFD", "ChaveNFD", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Data Emissão NFD", "DataEmissaoNFD", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Valor Total Produto NFD", "ValorTotalProdutoNFD", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Valor Total NF NFD", "ValorTotalNFNFD", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Peso Devolvido NFD", "PesoDevolvidoNFD", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("NFe Origem", "NFeOrigem", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Quantidade Devolvida", "QuantidadeTotalDevolvidaNoChamado", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Quantidade Vale Pallet", "QuantidadeValePallet", TamanhoColunaGrande, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Estadia", "EstadiaFormatado", TamanhoColunaPequena, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Senha", "SenhaDevolucao", TamanhoColunaPequena, Models.Grid.Align.center, false, false);

            return grid;
        }

        // TODO (ct-reports): Repassar CT
        private Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamado ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configGeral)
        {
            int codigoFilial = Request.GetIntParam("Filial");

            return new Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamado()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                CodigoTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Usuario.Empresa.Codigo : Request.GetIntParam("Transportador"),
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
                FiltrarCargasPorParteDoNumero = configGeral?.FiltrarCargasPorParteDoNumero ?? false,
            };
        }

        #endregion
    }
}
