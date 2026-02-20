namespace SGT.WebAdmin.Controllers.Logistica
{

    public class GridMonitoramento
    {

        #region Métodos públicos

        public Models.Grid.Grid ObterGrid(HttpRequest request, int codigoUsuario, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(request)
            {
                header = new List<Models.Grid.Head>()
            };

            var isMultiTMS = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;
            var isFornecedor = tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor;

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataDaCarga, "DataCriacaoCarga", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Carga", false);
            grid.AdicionarCabecalho("DataInicioMonitoramento", false);
            grid.AdicionarCabecalho("DataFimMonitoramento", false);
            grid.AdicionarCabecalho("Veiculo", false);
            grid.AdicionarCabecalho("IDEquipamento", false);
            grid.AdicionarCabecalho("CodigoProximaEntrega", false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataDoMonitoramento, "Data", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.SM, "Codigo", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CodigoFilial, "CodigoFilial", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Filial, "Filial", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Carga, "CargaEmbarcador", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PesoTotal, "PesoTotalCarga", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.ValorTotalNotaFiscal, "ValorTotalNFe", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.ClienteOrigem, "ClienteOrigem", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CidadeDestino, "CidadeDestino", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Destinos, "Destinos", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Recebedor, "Recebedor", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Expedidor, "Expedidor", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Pedidos, "Pedidos", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Ordens, "Ordens", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Carregamento, "DataCarregamentoFormatada", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PrevisaoChegadaPlanta, "DataPrevisaoChegadaPlanta", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataEntregaPedido, "DataPrevisaoEntregaPedido", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataInicioViagem, "DataInicioViagem", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataDescargaPrevista, "DataPrevisaoDescargaJanela", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.ProximoDestino, "ProximoDestino", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CodigoIntegracaoDestino, "CodigoIntegracaoDestino", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PrevisaoEntregaPlanejada, "DataEntregaPlanejadaProximaEntrega", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PrevisaoEntregaReprogramada, "DataEntregaReprogramadaProximaEntrega", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataSaidaDaOrigem, "DataSaidaOrigem", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataChegadaNoDestino, "DataChegadaDestino", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Coletas, "Coletas", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.QuantidadesEntregas, "QuantidadeEntregas", 5, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DistanciaRota, "DistanciaRotaPrevistaRealizada", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PorcentagemAderenciaSequencia, "AderenciaSequencia", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PorcentagemAderenciaRaio, "AderenciaRaio", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Tracao, "Tracao", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Reboque, "Reboques", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.VeiculoDedicado, "PossuiContratoFrete", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.RazaoSocialTransportador, "RazaoSocialTransportador", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NomeFantasiaTransportador, "NomeFantasiaTransportador", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Motorista, "Motoristas", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CPFMotorista, "CPFMotoristas", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Latitude, "Latitude", 5, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Longitude, "Longitude", 5, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Posicao, "Posicao", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Ignicao, "StatusIgnicao", 4, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NivelGPS, "NivelGPS", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Velocidade, "Velocidade", 4, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Temperatura, "Temperatura", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.FaixaDeTemperatura, "DescricaoFaixaTemperatura", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("TemperaturaFaixaInicial", false);
            grid.AdicionarCabecalho("TemperaturaFaixaFinal", false);
            grid.AdicionarCabecalho("ControleDeTemperatura", false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PorcentagemTemperaturaNaFaixa, "TemperaturaDentroFaixa", 7, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PorcentagemViagem, "PercentualViagem", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("N° Frota", "NumeroFrota", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.KmAteDestino, "DistanciaAteDestino", 7, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.KmRodado, "DistanciaPercorrida", 7, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.KmTotal, "DistanciaTotal", 7, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Rastreador, "RastreadorOnlineOffline", 4, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataDaPosicao, "DataPosicaoAtual", 5, Models.Grid.Align.left, true, false);

            if (isFornecedor)
            {
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TotalDeAlertas, "TotalAlertas", 4, Models.Grid.Align.right, true, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Alertas, "Alertas", 4, Models.Grid.Align.left, true, false);
            }

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataProgramadaDaColeta, "DataProgramadaColeta", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Semaforo, "CorSemaforo", 5, Models.Grid.Align.left, false);

            if (!configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento || !configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.EtapaMonitoramento, "StatusViagem", 5, Models.Grid.Align.left, true);

            if (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TipoDeOperacao, "GrupoTipoOperacao", 5, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Alvo, "CategoriasAlvos", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.SituacaoMonitoramento, "StatusDescricao", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NumeroEXP, "NumeroEXP", 4, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Critico, "Critico", 5, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NumeroRastreador, "NumeroRastreador", 7, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TendenciaProximaParada, "TendenciaProximaParadaDescricao", 10, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TendenciaColeta, "TendenciaColetaDescricao", 10, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TendenciaEntrega, "TendenciaEntregaDescricao", 10, Models.Grid.Align.left, false, true);

            if (isMultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.ResponsavelVeiculo, "ResponsavelVeiculo", 10, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CentrosDeResultados, "CentroResultado", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Fronteiras, "FronteiraRotaFrete", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.CidadeOrigem, "CidadeOrigem", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NomeRastreador, "NomeRastreador", 5, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Gerenciador, "Gerenciador", 5, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("TipoIntegracaoTecnologiaRastreador", false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NotasFiscais, "NotasFiscais", 7, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NumeroAE, "NumeroProtocoloIntegracaoCarga", 7, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.VersaoAppMotorista, "VersaoAppMotorista", 7, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NumeroPedidoNoEmbarcador, "NumeroPedidoEmbarcadorSumarizado", 7, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Anotacoes, "Observacao", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TempoStatus, "TempoStatusDescricao", 5, Models.Grid.Align.left, false, false);

            if (isMultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataReagendamento, "DataReagendamentoFormatada", 7, Models.Grid.Align.left, false, false);

            if (filtrosPesquisa.VeiculosEmLocaisTracking)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.LocaisTracking, "LocalTracking", 7, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho("Previsão Fim Viagem", "PrevisaoFimViagemFormatada", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TipoOperacao, "TipoOperacao", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NumeroContainer, "NumeroContainer", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TipoTrecho, "TipoTrecho", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Previsao Termino Viagem", "PrevisaoTerminoViagemFormatada", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Previsão StopTracking", "PrevisaoStopTrankingFormatada", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Previsão Saída Destino", "PrevisaoSaidaDestinoFormatada", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.TipoOcorrencia.ModalTransporte, "ModalTransporte", 5, Models.Grid.Align.left, false, false);

            if (isMultiTMS)
            {
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.UltimaOcorrencia, "UltimaOcorrencia", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Subcontratado, "Subcontratado", 10, Models.Grid.Align.left, false, false);
            }

            Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "Monitoramento/Pesquisa", "grid-monitoramento");
            grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(codigoUsuario, grid.modelo));

            return grid;

        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> ObterRegistrosPesquisa(HttpRequest request, int codigoUsuario, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int totalRegistros = 0)
        {
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> listaConsulta = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();

            Models.Grid.Grid grid = ObterGrid(request, codigoUsuario, filtrosPesquisa, configuracao, unitOfWork, tipoServicoMultisoftware);
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(null);

            if (totalRegistros <= 0)
                totalRegistros = ObterTotalRegistrosPesquisa(filtrosPesquisa, configuracao, configuracaoMonitoramento, unitOfWork);

            grid.setarQuantidadeTotal(totalRegistros);
            if (totalRegistros > 0)
            {
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                listaConsulta = repMonitoramento.ConsultarMonitoramentoRefatorado(filtrosPesquisa, parametrosConsulta, configuracao, configuracaoMonitoramento);
            }
            return listaConsulta;
        }

        public int ObterTotalRegistrosPesquisa(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repositorio = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            int totalRegistros = repositorio.ContarConsultaRefatorada(filtrosPesquisa, configuracao, configuracaoMonitoramento);
            return totalRegistros;
        }

        public Models.Grid.Grid ObterGridComPesquisa(HttpRequest request, int codigoUsuario, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Models.Grid.Grid grid = ObterGrid(request, codigoUsuario, filtrosPesquisa, configuracao, unitOfWork, tipoServicoMultisoftware);

            int totalRegistros = ObterTotalRegistrosPesquisa(filtrosPesquisa, configuracao, configuracaoMonitoramento, unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> listaConsulta = (totalRegistros > 0) ? ObterRegistrosPesquisa(request, codigoUsuario, filtrosPesquisa, configuracao, configuracaoMonitoramento, unitOfWork, tipoServicoMultisoftware, totalRegistros).ToList() : new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();
            // TODO: ToList cast
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> ListaAdicionada = AdicionarRegistrosAoGrid(listaConsulta, grid, configuracao, filtrosPesquisa);

            if (filtrosPesquisa.VeiculosEmLocaisTracking)
                totalRegistros = ListaAdicionada.Count;

            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> AdicionarRegistrosAoGrid(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> lista, Models.Grid.Grid grid, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa)
        {
            if (filtrosPesquisa.VeiculosEmLocaisTracking && filtrosPesquisa.locais != null && filtrosPesquisa.locais.Count > 0)
            {
                //vai filtrar apenas os veiculos/monitoramentos que estao nos locais selecionados;
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> Novalista = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento>();

                foreach (var monitoramento in lista)
                {
                    Dominio.Entidades.Embarcador.Logistica.Locais localAtual = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarLocalEmArea(filtrosPesquisa.locais.ToArray(), monitoramento.Latitude, monitoramento.Longitude);
                    if (localAtual != null)
                    {
                        monitoramento.LocalTracking = localAtual.Descricao;
                        Novalista.Add(monitoramento);
                    }

                }

                lista = Novalista;
            }

            var listaRetornar = lista;

            // Ordenação de colunas que não são do banco de dados
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(null);
            string direcaoOrdenar = parametrosConsulta.DirecaoOrdenar?.ToLower();
            string colunaOrdenacaoManual = (parametrosConsulta.PropriedadeOrdenar != null) ? parametrosConsulta.PropriedadeOrdenar : String.Empty;
            switch (colunaOrdenacaoManual)
            {
                case "Status":
                    if (direcaoOrdenar == "asc") listaRetornar.Sort((x, y) => String.Compare(x.StatusDescricao, y.StatusDescricao)); else listaRetornar.Sort((x, y) => String.Compare(y.StatusDescricao, x.StatusDescricao));
                    break;
                case "Rastreador":
                    if (direcaoOrdenar == "asc") listaRetornar.Sort((x, y) => String.Compare(x.Rastreador, y.Rastreador)); else listaRetornar.Sort((x, y) => String.Compare(y.Rastreador, x.Rastreador));
                    break;
                case "Coletas":
                    if (direcaoOrdenar == "asc") listaRetornar.Sort((x, y) => String.Compare(x.Coletas, y.Coletas)); else listaRetornar.Sort((x, y) => String.Compare(y.Coletas, x.Coletas));
                    break;
                case "EntregasDescricao":
                    if (direcaoOrdenar == "asc") listaRetornar.Sort((x, y) => String.Compare(x.EntregasDescricao, y.EntregasDescricao)); else listaRetornar.Sort((x, y) => String.Compare(y.EntregasDescricao, x.EntregasDescricao));
                    break;
            }

            grid.AdicionaRows(listaRetornar);

            return lista;
        }

        #endregion

        #region Métodos privados
        #endregion

    }
}