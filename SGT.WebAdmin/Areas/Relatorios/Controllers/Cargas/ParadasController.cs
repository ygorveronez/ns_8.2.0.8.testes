using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Cargas/Paradas")]
    public class ParadasController : BaseController
    {
        #region Construtores

        public ParadasController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R232_Paradas;
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
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Paradas", "Cargas", "Paradas.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroParadas", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

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
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Carga.Paradas servicoRelatorioParadas = new Servicos.Embarcador.Relatorios.Carga.Paradas(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioParadas.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.Paradas> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas()
            {
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                NumeroCargas = ObterParametroFiltroLista(Request.GetStringParam("NumeroCarga")),
                CodigosTransportadores = Request.GetListParam<int>("Transportador"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculo"),
                CodigosMotoristas = Request.GetListParam<int>("Motorista"),
                CpfsCnpjsRemetentes = Request.GetListParam<double>("Remetente"),
                CpfsCnpjsDestinatarios = Request.GetListParam<double>("Destinatario"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoDestino = Request.GetIntParam("Destino"),
                TipoParada = Request.GetNullableBoolParam("TipoParada"),
                EscritorioVendas = Request.GetStringParam("EscritorioVendas"),
                ProtocoloIntegracaoSM = Request.GetStringParam("ProtocoloIntegracaoSM"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                DataEntregaPlanejadaInicio = Request.GetNullableDateTimeParam("DataEntregaPlanejadaInicio"),
                DataEntregaPlanejadaFinal = Request.GetNullableDateTimeParam("DataEntregaPlanejadaFinal"),
                MonitoramentoStatus = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus>("MonitoramentoStatus"),
                ExibirCargasAgrupadas = Request.GetBoolParam("ExibirCargasAgrupadas"),
                Transbordo = Request.GetNullableBoolParam("Transbordo")
            };

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosTipoOperacao = Request.GetListParam<int>("TipoOperacao");
            List<int> codigosTipoCargas = Request.GetListParam<int>("TipoCarga");

            filtrosPesquisa.CodigosFiliais = codigosFilial.Count > 0 ? codigosFilial : ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoOperacoes = codigosTipoOperacao.Count > 0 ? codigosTipoOperacao : ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoCargas = codigosTipoCargas.Count > 0 ? codigosTipoCargas : ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosGrupoPessoas = ObterListaCodigoGrupoPessoasPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);

            return filtrosPesquisa;
        }

        private List<string> ObterParametroFiltroLista(string parametro)
        {
            string separadosPorPontoVirgula = string.Join(";", parametro.Split(' '));
            List<string> parametros = new List<string>();

            foreach (string param in separadosPorPontoVirgula.Split(';'))
                if (!string.IsNullOrWhiteSpace(param))
                    parametros.Add(param);

            return parametros;
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork, HttpRequest request = null)
        {
            Models.Grid.Grid grid;

            grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>(),
                listTabs = new List<Models.Grid.Tab>(),
            };

            grid.AdicionarTab("Data", out Models.Grid.Tab tabData);
            grid.AdicionarTab("Monitoramento", out Models.Grid.Tab tabMonitoramento);
            grid.AdicionarTab("Carga", out Models.Grid.Tab tabCarga);

            // Aba "Data" - Todas as colunas relacionadas a datas
            grid.AdicionarCabecalho("Data Criação Carga", "DataCriacaoCargaFormatada", 5, Models.Grid.Align.center, true, false, tabData).Visibilidade(false);
            grid.AdicionarCabecalho("Data Chegada no Cliente", "DataChegadaClienteFormatada", 5, Models.Grid.Align.center, true, false, tabData);
            grid.AdicionarCabecalho("Data Saída do Cliente", "DataSaidaClienteFormatada", 5, Models.Grid.Align.center, true, false, tabData);
            grid.AdicionarCabecalho("Data da Entrega", "DataEntregaFormatada", 5, Models.Grid.Align.center, true, false, tabData);
            grid.AdicionarCabecalho("Data/Hora Avaliação", "DataHoraAvaliacaoFormatada", 5, Models.Grid.Align.center, false, false, tabData);
            grid.AdicionarCabecalho("Data de Carregamento", "DataCarregamentoFormatada", 5, Models.Grid.Align.center, false, false, tabData);
            grid.AdicionarCabecalho("Data de Início de Viagem", "DataInicioViagemFormatada", 5, Models.Grid.Align.center, false, false, tabData);
            grid.AdicionarCabecalho("Data de Fim de Viagem", "DataFimViagemFormatada", 5, Models.Grid.Align.center, false, false, tabData);
            grid.AdicionarCabecalho("Data da Confirmação de Chegada", "DataConfirmacaoChegadaFormatada", 5, Models.Grid.Align.center, false, false, tabData);
            grid.AdicionarCabecalho("Data de Início de Descarregamento", "DataInicioDescargaFormatada", 5, Models.Grid.Align.center, false, false, tabData);
            grid.AdicionarCabecalho("Data de Término de Descarregamento", "DataTerminoDescargaFormatada", 5, Models.Grid.Align.center, false, false, tabData);
            grid.AdicionarCabecalho("Data de início do monitoramento", "DataInicioMonitoramentoFormatada", 5, Models.Grid.Align.left, true, false, tabData);
            grid.AdicionarCabecalho("Data de fim do monitoramento", "DataFimMonitoramentoFormatada", 5, Models.Grid.Align.left, true, false, tabData);
            grid.AdicionarCabecalho("Data de início de carregamento", "DataInicioCarregamentoFormatada", 5, Models.Grid.Align.left, true, false, tabData);
            grid.AdicionarCabecalho("Data de término de carregamento", "DataTerminoCarregamentoFormatada", 5, Models.Grid.Align.left, true, false, tabData);
            grid.AdicionarCabecalho("Data do primeiro espelhamento", "DataPrimeiroEspelhamentoFormatada", 5, Models.Grid.Align.left, true, false, tabData);
            grid.AdicionarCabecalho("Data do último espelhamento", "DataUltimoEspelhamentoFormatada", 5, Models.Grid.Align.left, true, false, tabData);
            grid.AdicionarCabecalho("Data de Confirmação da entrega pelo Usuário", "DataConfirmacaoEntregaFormatada", 5, Models.Grid.Align.center, true, false, tabData);
            grid.AdicionarCabecalho("Data da Coleta", "DataColetaFormatada", 5, Models.Grid.Align.center, true, false, tabData);

            // Aba "Monitoramento" - Colunas específicas de monitoramento
            grid.AdicionarCabecalho("Ordem Prevista", "OrdemPrevista", 5, Models.Grid.Align.right, true, true, tabMonitoramento);
            grid.AdicionarCabecalho("Ordem Executada", "OrdemExecutada", 5, Models.Grid.Align.right, true, false, tabMonitoramento);
            grid.AdicionarCabecalho("Aderência", "Aderencia", 5, Models.Grid.Align.right, true, false, tabMonitoramento);
            grid.AdicionarCabecalho("Tempo Permanência", "TempoPermanencia", 5, Models.Grid.Align.left, false, false, tabMonitoramento);
            grid.AdicionarCabecalho("KM Planejado", "KMPlanejado", 5, Models.Grid.Align.right, false, false, tabMonitoramento);
            grid.AdicionarCabecalho("KM Realizado", "KMRealizado", 5, Models.Grid.Align.right, false, false, tabMonitoramento);
            grid.AdicionarCabecalho("Confirmação via App", "ConfirmacaoViaApp", 5, Models.Grid.Align.center, false, false, tabMonitoramento);
            grid.AdicionarCabecalho("Encerramento Manual da Viagem", "EncerramentoManualViagemFormatado", 5, Models.Grid.Align.center, false, false, tabMonitoramento);

            // Aba "Carga" - Todas as outras colunas
            grid.AdicionarCabecalho("Filial", "Filial", 8, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho("Carga", "Carga", 6, Models.Grid.Align.left, true, true, tabCarga);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Transportador", "Transportador", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Motoristas", "Motoristas", 6, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Placas", "Placas", 6, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Situação da Entrega", "SituacaoEntregaDescricao", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("CPF/CNPJ Destinatário", "CPFCNPJClienteFormatado", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Cliente Destinatário", "Cliente", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Endereço Destinatário", "Endereco", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Bairro Destinatário", "Bairro", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Cidade Destinatário", "Cidade", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Estado Destinatário", "Estado", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("CEP Destinatário", "CEP", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", 6, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Pedidos", "Pedidos", 6, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Peso Bruto", "PesoBruto", 5, Models.Grid.Align.right, true, false, tabCarga).Sumarizar(TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Resultado Avaliação", "ResultadoAvaliacao", 5, Models.Grid.Align.center, false, false, tabCarga);
            grid.AdicionarCabecalho("Motivo Avaliação", "MotivoAvaliacao", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Observação Avaliação", "ObservacaoAvaliacao", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Entrega Fora do Raio", "EntregaForaDoRaioFormatada", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Raio Médio Viagem (KM)", "RaioMedioViagem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Quantidade de Animais", "QuantidadeAnimais", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Quantidade de Mortalidade", "QuantidadeMortalidade", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Cliente Remetente", "ClienteRemetente", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Endereço Remetente", "EnderecoRemetente", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Bairro Remetente", "BairroRemetente", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Cidade Remetente", "CidadeRemetente", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Estado Remetente", "EstadoRemetente", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Transbordo?", "Transbordo", 6, Models.Grid.Align.left, true, true, tabCarga);

            grid.AdicionarCabecalho("Percentual Viagem", "PercentualViagemMonitoramentoDescricao", 5, Models.Grid.Align.center, true, false, tabCarga);
            grid.AdicionarCabecalho("Longitude Última Posição", "LongitudeUltimaPosicaoDescricao", 5, Models.Grid.Align.right, false, false, tabCarga);
            grid.AdicionarCabecalho("Latitude Última Posição", "LatitudeUltimaPosicaoDescricao", 5, Models.Grid.Align.right, false, false, tabCarga);
            grid.AdicionarCabecalho("Rota", "Rota", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Origem Fim Monitoramento", "OrigemMonitoramentoDescricao", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Motivo Fim Monitoramento", "MotivoFimMonitoramentoDescricao", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Próxima Carga Programada", "ProximaCargaProgramada", 5, Models.Grid.Align.left, false, false, tabCarga);

            grid.AdicionarCabecalho("Tipo Parada", "TipoParadaFormatada", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Valor Total das Notas", "ValorTotalNotas", 5, Models.Grid.Align.left, false, false, tabCarga);

            grid.AdicionarCabecalho("Código Integração Cliente", "CodigoIntegracaoCliente", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Prazo Cliente", "PrazoEntregaCliente", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Prazo Transportador", "PrazoEntregaTransportador", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Previsão Entrega Cliente", "PrevisaoEntregaCliente", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Agendamento Entrega Cliente", "AgendamentoEntregaClienteFormatada", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Previsão Entrega Transportador", "PrevisaoEntregaTransportador", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 5, Models.Grid.Align.left, false, false, tabCarga);
            grid.AdicionarCabecalho("CPF/CNPJ Remetente", "CPFCNPJRemetenteFormatado", 6, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Finalizador", "OrigemSituacaoEntregaDescricao", 5, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Perfil Finalizador", "UsuarioFinalizador", 5, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Número do pedido no cliente", "NumeroPedidoCliente", 5, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Situação do monitoramento", "DescricaoMonitoramentoStatus", 5, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Realizada no Prazo?", "RealizadaNoPrazoFormatada", 5, Models.Grid.Align.left, true, false, tabCarga);
            grid.AdicionarCabecalho("Motivo da Retificação", "MotivoRetificacao", 5, Models.Grid.Align.left, true, false, tabCarga);

            Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "/Relatorios/Paradas/Pesquisa", "gridPreviewRelatorio");
            grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            //grid.Prop("Quantidades").Nome("Quantidades").Tamanho(_tamanhoColunaPequena).Align(Models.Grid.Align.right).OrdAgr(true, false).Sumarizar(TipoSumarizacao.sum);
            //grid.Prop("Volumes").Nome("Volumes").Tamanho(_tamanhoColunaPequena).Align(Models.Grid.Align.right).OrdAgr(true, false).Sumarizar(TipoSumarizacao.sum);

            return grid;
        }

        #endregion
    }
}
