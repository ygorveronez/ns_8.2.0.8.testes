namespace Monitoramento.Thread
{
    public class MonitoramentoCache
    {
        public int Codigo;
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> Entregas;
        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento CargaJanelaCarregamento;
        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> CargaJanelasDescarregamento;
    }

    public class ProcessarEventos : AbstractThreadProcessarEventos
    {

        #region Atributos privados

        private static ProcessarEventos Instante;
        private List<MonitoramentoCache> monitoramentosCache;
        private Dominio.Entidades.Embarcador.Logistica.Locais[] locaisPernoite;
        private Dominio.Entidades.Embarcador.Logistica.Locais[] locaisAreaDeRisco;
        private Dominio.Entidades.Embarcador.Logistica.Locais[] locaisPontoDeApoio;
        private List<Dominio.Entidades.Embarcador.Logistica.RaioProximidade> locaisRaioProximidade;
        private string diretorioFila;
        private string arquivoFilaPrefixo;
        private List<string> arquivosEmProcessamento;
        #endregion

        #region Métodos públicos

        // Singleton
        public static ProcessarEventos GetInstance(string stringConexao)
        {
            if (Instante == null) Instante = new ProcessarEventos(stringConexao);
            return Instante;
        }

        #endregion

        #region Construtor privado

        private ProcessarEventos(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = string.IsNullOrWhiteSpace(stringConexao) ? null : new Repositorio.UnitOfWork(stringConexao);
            base.dataAtual = DateTime.Now;
            try
            {
                base.tempoSleep = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().TempoSleepThread;
                base.enable = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().Ativo;
                base.limiteRegistros = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().LimiteRegistros;
                base.limiteDiasConsulta = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().LimiteDiasRetroativos;
                base.MinutosFiltroProcessar = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().FiltrarMinutos;
                base.arquivoNivelLog = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().ArquivoNivelLog;
                this.monitoramentosCache = new List<MonitoramentoCache>();
                this.diretorioFila = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().DiretorioFila;
                this.arquivoFilaPrefixo = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().ArquivoFilaPrefixo;
            }
            catch (Exception e)
            {
                Log(e.Message);
                throw;
            }
            finally
            {
                unitOfWork?.Dispose();
            }
        }
        #endregion

        #region Métodos abstratos


        public override void ProcessarEventosPendentes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {

            DateTime inicio;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            if (!configuracao.PossuiMonitoramento) return;

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> processarEventos = BuscarMonitoramentoProcessarEvento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> listaMonitoramentoEventoAtivo = repMonitoramentoEvento.BuscarTodosAtivos();

            int total = processarEventos.Count;
            Log(total + " eventos pendentes aptos a processar");
            if (total > 0)
            {
                // Identifica os eventos ativos e configurados para serem processados
                List<Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento> eventosAtivos = CarregarEventosAtivos(unitOfWork, locaisPernoite, locaisAreaDeRisco, cliente);
                if (eventosAtivos.Count > 0)
                {

                    // Extrai os veículos únicos envolvidos nas posições recebidas
                    List<int> codigosVeiculos = ObtemCodigosVeiculosDistintos(processarEventos);

                    // Busca uma lista de monitoramentos abertos durante as posições recebidas
                    List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentosAbertos = BuscarMonitoramentosAbertos(unitOfWork, codigosVeiculos, processarEventos);

                    // Carrega o último alerta alerta por veículos e evento
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas = BuscarAlertasPendentes(unitOfWork, codigosVeiculos, eventosAtivos);

                    // Busca o maior tempo de evento configurado para carregar o histórico de posições
                    int maiorTempoEventoMinutos = BusarMaiorTempoEvento(eventosAtivos);

                    DateTime dataFinal = processarEventos.Last().DataVeiculoPosicao.Value;
                    DateTime dataInicio = processarEventos.First().DataVeiculoPosicao.Value.AddMinutes(-maiorTempoEventoMinutos);

                    if (limiteDiasConsulta > 0)
                    {
                        DateTime hoje = DateTime.Now;
                        if (dataInicio <= hoje.AddDays(-limiteDiasConsulta)) //new DateTime(hoje.Year, hoje.Month - limiteMesesConsulta, hoje.Day))
                        {
                            dataInicio = hoje.AddDays(-limiteDiasConsulta);  //new DateTime(hoje.Year, hoje.Month - limiteMesesConsulta, hoje.Day);
                        }
                    }

                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaLocal> permanenciaLocais = CarregarPermanenciaLocais(unitOfWork, eventosAtivos);
                    IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculos = CarregarPosicoesVeiculos(unitOfWork, codigosVeiculos, dataInicio, dataFinal);

                    try
                    {
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);

                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> processarEventosVeiculo;
                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor;
                        Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento;
                        MonitoramentoCache monitoramentoCache;

                        DateTime inicioTotalVeiculos = DateTime.UtcNow;
                        int totalVeiculos = codigosVeiculos.Count;
                        for (int i = 0; i < totalVeiculos; i++)
                        {
                            inicio = DateTime.UtcNow;
                            processarEventosVeiculo = ObtemProcessarEventosDoVeiculo(codigosVeiculos[i], processarEventos, MinutosFiltroProcessar);
                            int totalEventosProcessar = processarEventosVeiculo.Count;
                            if (totalEventosProcessar > 0)
                            {
                                posicoesObjetoValor = BuscarPosicoesVeiculo(codigosVeiculos[i], posicoesVeiculos);
                                for (int j = 0; j < totalEventosProcessar; j++)
                                {

                                    // Alvos da posição
                                    List<double> codigosClientesAlvo = Servicos.Embarcador.Monitoramento.Util.ExtrairDouble(processarEventosVeiculo[j].CodigosClientesAlvoPosicao);

                                    // Busca algum monitoramento aberto
                                    monitoramento = BuscarMonitoramentoEmAberto(monitoramentosAbertos, processarEventosVeiculo[j].CodigoVeiculo.Value, processarEventosVeiculo[j].DataVeiculoPosicao.Value);

                                    // Busca as informações do monitoramento/carga do cache: entregas da carga, horário agendado do carregamento e horários agendados dos descarregamentos
                                    monitoramentoCache = BuscarMonitoramentoCache(monitoramento, repCargaEntrega, repCargaJanelaCarregamento, repCargaJanelaDescarregamento);

                                    try
                                    {
                                        // Processamento de cada um dos eventos ativos
                                        ProcessarEventosAtivos(listaMonitoramentoEventoAtivo, eventosAtivos, monitoramento, processarEventosVeiculo[j], posicoesObjetoValor, alertas, monitoramentoCache.Entregas, codigosClientesAlvo, monitoramentoCache.CargaJanelaCarregamento, monitoramentoCache.CargaJanelasDescarregamento, configuracao, unitOfWork, permanenciaLocais);
                                    }
                                    catch (Exception e)
                                    {
                                        Servicos.Log.TratarErro(e);
                                    }
                                }
                            }
                            Log($"{i.ToString().PadLeft(5, '0')} - Veiculo {codigosVeiculos[i]}", inicio, 2);
                        }
                        Log($"ProcessarEventoAtivos (TotalVeiculos: {totalVeiculos})", inicioTotalVeiculos, 1);

                        inicio = DateTime.UtcNow;

                        Log("CommitChanges BuscarPosicoesProcessarEventos", inicio, 1);

                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        throw;
                    }
                }

                // Exclui o registro de pendência de processamento da troca de alvo
                ExcluirArquivosDePendencias();

                Log($"{total} posicoes processadas com sucesso");
            }


        }

        private List<Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento> CarregarEventosAtivos(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Locais[] locaisPernoite, Dominio.Entidades.Embarcador.Logistica.Locais[] locaisAreaDeRisco, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            DateTime inicio = DateTime.UtcNow;
            List<Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento> eventos = new List<Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento>();

            this.locaisPernoite = null;
            this.locaisAreaDeRisco = null;

            // Inicialização da verificação dos eventos
            Servicos.Embarcador.Monitoramento.Eventos.VelocidadeExcedida VelocidadeExcedida = new Servicos.Embarcador.Monitoramento.Eventos.VelocidadeExcedida(unitOfWork);
            if (VelocidadeExcedida.EstaAtivo())
            {
                eventos.Add(VelocidadeExcedida);
            }

            Servicos.Embarcador.Monitoramento.Eventos.TemperaturaForaDaFaixa TemperaturaForaDaFaixa = new Servicos.Embarcador.Monitoramento.Eventos.TemperaturaForaDaFaixa(unitOfWork);
            if (TemperaturaForaDaFaixa.EstaAtivo())
            {
                eventos.Add(TemperaturaForaDaFaixa);
            }

            Servicos.Embarcador.Monitoramento.Eventos.SensorDeTemperaturaComProblema SensorDeTemperaturaComProblema = new Servicos.Embarcador.Monitoramento.Eventos.SensorDeTemperaturaComProblema(unitOfWork);
            if (SensorDeTemperaturaComProblema.EstaAtivo())
            {
                eventos.Add(SensorDeTemperaturaComProblema);
            }

            Servicos.Embarcador.Monitoramento.Eventos.Pernoite Pernoite = new Servicos.Embarcador.Monitoramento.Eventos.Pernoite(unitOfWork);
            if (Pernoite.EstaAtivo())
            {
                CarregarLocais(unitOfWork);
                Pernoite.LocaisPernoite = this.locaisPernoite;
                eventos.Add(Pernoite);
            }

            Servicos.Embarcador.Monitoramento.Eventos.ParadaNaoProgramada ParadaNaoProgramada = new Servicos.Embarcador.Monitoramento.Eventos.ParadaNaoProgramada(unitOfWork, cliente);
            if (ParadaNaoProgramada.EstaAtivo())
            {
                CarregarLocais(unitOfWork);
                ParadaNaoProgramada.LocaisPernoite = this.locaisPernoite;
                ParadaNaoProgramada.LocaisAreaDeRisco = this.locaisAreaDeRisco;
                eventos.Add(ParadaNaoProgramada);
            }

            Servicos.Embarcador.Monitoramento.Eventos.ParadaEmAreaDeRisco ParadaEmAreaDeRisco = new Servicos.Embarcador.Monitoramento.Eventos.ParadaEmAreaDeRisco(unitOfWork);
            if (ParadaEmAreaDeRisco.EstaAtivo())
            {
                CarregarLocais(unitOfWork);
                ParadaEmAreaDeRisco.LocaisAreaDeRisco = this.locaisAreaDeRisco;
                eventos.Add(ParadaEmAreaDeRisco);
            }

            Servicos.Embarcador.Monitoramento.Eventos.ParadaExcessiva ParadaExcessiva = new Servicos.Embarcador.Monitoramento.Eventos.ParadaExcessiva(unitOfWork);
            if (ParadaExcessiva.EstaAtivo())
            {
                CarregarLocais(unitOfWork);
                ParadaExcessiva.LocaisPernoite = this.locaisPernoite;
                ParadaExcessiva.LocaisAreaDeRisco = this.locaisAreaDeRisco;
                ParadaExcessiva.LocaisPontoDeApoio = this.locaisPontoDeApoio;
                eventos.Add(ParadaExcessiva);
            }

            Servicos.Embarcador.Monitoramento.Eventos.DesvioDeRota DesvioDeRota = new Servicos.Embarcador.Monitoramento.Eventos.DesvioDeRota(unitOfWork);
            if (DesvioDeRota.EstaAtivo())
            {
                eventos.Add(DesvioDeRota);
            }

            Servicos.Embarcador.Monitoramento.Eventos.AtrasoNaDescarga AtrasoNaDescarga = new Servicos.Embarcador.Monitoramento.Eventos.AtrasoNaDescarga(unitOfWork);
            if (AtrasoNaDescarga.EstaAtivo())
            {
                eventos.Add(AtrasoNaDescarga);
            }

            Servicos.Embarcador.Monitoramento.Eventos.AtrasoNaEntrega AtrasoNaEntrega = new Servicos.Embarcador.Monitoramento.Eventos.AtrasoNaEntrega(unitOfWork);
            if (AtrasoNaEntrega.EstaAtivo())
            {
                eventos.Add(AtrasoNaEntrega);
            }

            Servicos.Embarcador.Monitoramento.Eventos.AtrasoNaLiberacao AtrasoNaLiberacao = new Servicos.Embarcador.Monitoramento.Eventos.AtrasoNaLiberacao(unitOfWork);
            if (AtrasoNaLiberacao.EstaAtivo())
            {
                eventos.Add(AtrasoNaLiberacao);
            }

            Servicos.Embarcador.Monitoramento.Eventos.AtrasoNoCarregamento AtrasoNoCarregamento = new Servicos.Embarcador.Monitoramento.Eventos.AtrasoNoCarregamento(unitOfWork);
            if (AtrasoNoCarregamento.EstaAtivo())
            {
                eventos.Add(AtrasoNoCarregamento);
            }

            Servicos.Embarcador.Monitoramento.Eventos.DirecaoSemDescanso DirecaoSemDescanso = new Servicos.Embarcador.Monitoramento.Eventos.DirecaoSemDescanso(unitOfWork);
            if (DirecaoSemDescanso.EstaAtivo())
            {
                eventos.Add(DirecaoSemDescanso);
            }

            Servicos.Embarcador.Monitoramento.Eventos.DirecaoContuinuaExcessiva DirecaoContinuaExcessiva = new Servicos.Embarcador.Monitoramento.Eventos.DirecaoContuinuaExcessiva(unitOfWork);
            if (DirecaoContinuaExcessiva.EstaAtivo())
            {
                eventos.Add(DirecaoContinuaExcessiva);
            }

            Servicos.Embarcador.Monitoramento.Eventos.PermanenciaNoRaio PermanenciaNoRaio = new Servicos.Embarcador.Monitoramento.Eventos.PermanenciaNoRaio(unitOfWork);
            if (PermanenciaNoRaio.EstaAtivo())
            {
                eventos.Add(PermanenciaNoRaio);
            }

            Servicos.Embarcador.Monitoramento.Eventos.PermanenciaNoRaioEntrega PermanenciaNoRaioEntrega = new Servicos.Embarcador.Monitoramento.Eventos.PermanenciaNoRaioEntrega(unitOfWork);
            if (PermanenciaNoRaioEntrega.EstaAtivo())
            {
                eventos.Add(PermanenciaNoRaioEntrega);
            }

            Servicos.Embarcador.Monitoramento.Eventos.ForaDoPrazo ForaDoPrazo = new Servicos.Embarcador.Monitoramento.Eventos.ForaDoPrazo(unitOfWork);
            if (ForaDoPrazo.EstaAtivo())
            {
                eventos.Add(ForaDoPrazo);
            }

            Servicos.Embarcador.Monitoramento.Eventos.InicioViagemSemDocumentacao inicioViagemSemDocumentacao = new Servicos.Embarcador.Monitoramento.Eventos.InicioViagemSemDocumentacao(unitOfWork);
            if (inicioViagemSemDocumentacao.EstaAtivo())
            {
                eventos.Add(inicioViagemSemDocumentacao);
            }

            Servicos.Embarcador.Monitoramento.Eventos.SensorDesengate sensorDesengate = new Servicos.Embarcador.Monitoramento.Eventos.SensorDesengate(unitOfWork);
            if (sensorDesengate.EstaAtivo())
            {
                eventos.Add(sensorDesengate);
            }

            Servicos.Embarcador.Monitoramento.Eventos.PermanenciaNoPontoApoio permanenciaPontoAporio = new Servicos.Embarcador.Monitoramento.Eventos.PermanenciaNoPontoApoio(unitOfWork);
            if (permanenciaPontoAporio.EstaAtivo())
            {
                eventos.Add(permanenciaPontoAporio);
            }

            Servicos.Embarcador.Monitoramento.Eventos.AusenciaDeInicioDeViagem ausenciaDeInicioDeViagem = new Servicos.Embarcador.Monitoramento.Eventos.AusenciaDeInicioDeViagem(unitOfWork);
            if (ausenciaDeInicioDeViagem.EstaAtivo())
            {
                eventos.Add(ausenciaDeInicioDeViagem);
            }

            Servicos.Embarcador.Monitoramento.Eventos.PossivelAtrasoNaOrigem possivelAtrasoNaOrigem = new Servicos.Embarcador.Monitoramento.Eventos.PossivelAtrasoNaOrigem(unitOfWork);
            if (possivelAtrasoNaOrigem.EstaAtivo())
            {
                eventos.Add(possivelAtrasoNaOrigem);
            }

            Servicos.Embarcador.Monitoramento.Eventos.ConcentracaoDeVeiculosNoRaio concentracaoDeVeiculosNoRaio = new Servicos.Embarcador.Monitoramento.Eventos.ConcentracaoDeVeiculosNoRaio(unitOfWork);
            if (concentracaoDeVeiculosNoRaio.EstaAtivo())
            {
                CarregarLocais(unitOfWork);
                concentracaoDeVeiculosNoRaio.LocaisRaioProximidade = this.locaisRaioProximidade;
                eventos.Add(concentracaoDeVeiculosNoRaio);
            }

            Log(eventos.Count + " eventos a processar", inicio, 1);

            return eventos;
        }

        /**
         * Busca uma lista de monitoramentos abertos no período das posições recebidas
         */
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> BuscarMonitoramentosAbertos(Repositorio.UnitOfWork unitOfWork, List<int> codigosVeiculos, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> processarEventos)
        {
            List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentos = new List<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            if (processarEventos != null && processarEventos.Count > 0 && processarEventos.First().CodigoPosicao != null && processarEventos.Last().CodigoPosicao != null)
            {
                DateTime inicio = DateTime.UtcNow;

                // As posições estão ordenadas pela DataVeiculo, de forma crescente
                DateTime dataInicio = processarEventos.First().DataVeiculoPosicao.Value;

                if (limiteDiasConsulta > 0)
                {
                    DateTime hoje = DateTime.Now;
                    if (dataInicio <= hoje.AddDays(-limiteDiasConsulta))//new DateTime(hoje.Year, hoje.Month - limiteMesesConsulta, hoje.Day))
                    {
                        dataInicio = hoje.AddDays(-limiteDiasConsulta); // DateTime(hoje.Year, hoje.Month - limiteMesesConsulta, hoje.Day);
                    }
                }

                DateTime dataFim = processarEventos.Last().DataVeiculoPosicao.Value;

                Log($"BuscarMonitoramentosAbertos {dataInicio} a {dataFim}", 2);
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                monitoramentos = repMonitoramento.BuscarMonitoramentoEmAbertoNoPeriodo(codigosVeiculos, dataInicio, dataFim);
                Log($"BuscarMonitoramentosAbertos {monitoramentos.Count}", inicio, 1);

            }
            return monitoramentos;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarEntregas(Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega, int codigoCarga)
        {
            DateTime inicio = DateTime.UtcNow;
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(codigoCarga);
            Log($"BuscarEntregas {cargaEntregas.Count}", inicio, 5);
            return cargaEntregas;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarCargaJanelaDescarregamento(Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento, int codigoCarga)
        {
            DateTime inicio = DateTime.UtcNow;
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento = repCargaJanelaDescarregamento.BuscarTodasPorCarga(codigoCarga);
            Log($"BuscarCargaJanelaDescarregamento {cargaJanelasDescarregamento.Count}", inicio, 5);
            return cargaJanelasDescarregamento;
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarCargaJanelaCarregamento(Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento, int codigoCarga)
        {
            DateTime inicio = DateTime.UtcNow;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);
            Log("BuscarCargaJanelaCarregamento", inicio, 5);
            return cargaJanelaCarregamento;
        }

        private MonitoramentoCache BuscarMonitoramentoCache(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega, Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento, Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento)
        {
            if (monitoramento != null)
            {
                DateTime inicio = DateTime.UtcNow;
                // Localiza o cache do monitoramento
                int total = this.monitoramentosCache.Count;
                for (int i = 0; i < total; i++)
                {
                    if (this.monitoramentosCache[i].Codigo == monitoramento.Codigo)
                    {
                        return this.monitoramentosCache[i];
                    }
                }

                // ... não encontrou, cria e alimenta
                MonitoramentoCache monitoramentoCache = new MonitoramentoCache();
                monitoramentoCache.Codigo = monitoramento.Codigo;
                if (monitoramento.Carga != null)
                {

                    monitoramentoCache.Entregas = BuscarEntregas(repCargaEntrega, monitoramento.Carga.Codigo);
                    monitoramentoCache.CargaJanelaCarregamento = BuscarCargaJanelaCarregamento(repCargaJanelaCarregamento, monitoramento.Carga.Codigo);
                    monitoramentoCache.CargaJanelasDescarregamento = BuscarCargaJanelaDescarregamento(repCargaJanelaDescarregamento, monitoramento.Carga.Codigo);
                }

                // Adiciona na lista de cache
                this.monitoramentosCache.Add(monitoramentoCache);

                Log($"BuscarMonitoramentoCache", inicio, 4);

                return monitoramentoCache;
            }
            return new MonitoramentoCache();
        }

        /**
         * Entre uma lista de posições, extrai as posições de um determinado veículo
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> ObtemProcessarEventosDoVeiculo(int codigoVeiculo, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> processarEventos, double intervaloMinutosPosicao)
        {

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> processarEventosVeiculo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento>();

            processarEventosVeiculo = processarEventos.Where(x => x.CodigoPosicao != null && x.CodigoVeiculo != null && x.CodigoVeiculo == codigoVeiculo).ToList();

            return filtrarPosicoesProcessar(processarEventosVeiculo, intervaloMinutosPosicao);

            //int t = processarEventos.Count;


            //for (int i = 0; i < t; i++)
            //{


            //    if (processarEventos[i].CodigoPosicao != null && processarEventos[i].CodigoVeiculo != null && processarEventos[i].CodigoVeiculo == codigoVeiculo)
            //    {
            //        processarEventosVeiculo.Add(processarEventos[i]);
            //    }
            //}
            //Log($"ObtemProcessarEventosDoVeiculo {processarEventosVeiculo.Count}", inicio, 3);
            //return processarEventosVeiculo;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> filtrarPosicoesProcessar(List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> processarEventos, double intervaloMinutosPosicao)
        {
            DateTime inicio = DateTime.UtcNow;
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> listafiltrada = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento>();

            int t = processarEventos.Count;
            DateTime dataUltimaPosicaoVeiculo = DateTime.MinValue;
            for (int i = 0; i < t; i++)
            {
                if (processarEventos[i].DataVeiculoPosicao.HasValue && processarEventos[i].DataVeiculoPosicao.Value != null && (processarEventos[i].DataVeiculoPosicao.Value - dataUltimaPosicaoVeiculo).TotalMinutes >= intervaloMinutosPosicao)
                {
                    dataUltimaPosicaoVeiculo = processarEventos[i].DataVeiculoPosicao.Value;
                    listafiltrada.Add(processarEventos[i]);
                }


            }
            Log($"ObtemProcessarEventosDoVeiculo {listafiltrada.Count}", inicio, 3);
            return listafiltrada;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> BuscarMonitoramentoProcessarEvento(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> processarEventos = BuscarPendenciasEventos(unitOfWork, base.limiteRegistros);
            Log(processarEventos.Count + " eventos de posicao pendentes", 1);

            return processarEventos;
        }

        private void CarregarLocais(Repositorio.UnitOfWork unitOfWork)
        {
            if (this.locaisPernoite == null || this.locaisAreaDeRisco == null || this.locaisPontoDeApoio == null || this.locaisRaioProximidade == null)
            {
                Repositorio.Embarcador.Logistica.Locais repLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                Repositorio.Embarcador.Logistica.RaioProximidade repRaioProximidade = new Repositorio.Embarcador.Logistica.RaioProximidade(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal> tiposLocais = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal>();
                tiposLocais.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.Pernoite);
                tiposLocais.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.AreaDeRisco);
                tiposLocais.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.RaioProximidade);
                Dominio.Entidades.Embarcador.Logistica.Locais[] locais = repLocais.BuscarPorTiposDeLocais(tiposLocais);

                this.locaisPernoite = SelecionarLocais(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.Pernoite, locais);
                this.locaisAreaDeRisco = SelecionarLocais(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.AreaDeRisco, locais);
                this.locaisPontoDeApoio = SelecionarLocais(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.PontoDeApoio, locais);
                this.locaisRaioProximidade = repRaioProximidade.BuscarPorCodigosLocais(locais.Where(l => l.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.RaioProximidade).Select(l => l.Codigo).ToList());
            }
        }

        private Dominio.Entidades.Embarcador.Logistica.Locais[] SelecionarLocais(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal tipoLocal, Dominio.Entidades.Embarcador.Logistica.Locais[] locais)
        {
            List<Dominio.Entidades.Embarcador.Logistica.Locais> locaisDoTipo = new List<Dominio.Entidades.Embarcador.Logistica.Locais>();
            int total = locais?.Length ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (locais[i].Tipo == tipoLocal)
                {
                    locaisDoTipo.Add(locais[i]);
                }
            }
            return locaisDoTipo.ToArray();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> BuscarPendenciasEventos(Repositorio.UnitOfWork unitOfWork, int quantidadeRegistros)
        {
            DateTime inicio = DateTime.UtcNow;
            this.arquivosEmProcessamento = new List<string>();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento> processarEventos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento>();

            // Busca os arquivos com as pendências da fila
            string diretorioFila = this.diretorioFila;
#if DEBUG
            diretorioFila = "C:\\GerenciadorApp\\Producao\\Filas\\ProcessarEventos\\";
#endif
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendenciaArquivo> pendencias = base.BuscarPendenciasFila(diretorioFila, quantidadeRegistros);
            int totalArquivos = pendencias.Count;
            if (totalArquivos > 0)
            {
                Repositorio.Embarcador.Logistica.MonitoramentoProcessarEvento repMonitoramentoProcessarEvento = new Repositorio.Embarcador.Logistica.MonitoramentoProcessarEvento(unitOfWork);
                for (int i = 0; i < totalArquivos; i++)
                {
                    // Adiciona na lista de arquivos em processamento
                    this.arquivosEmProcessamento.Add(pendencias[i].CaminhoArquivo);

                    // Consulta os dados do monitoramento e das posições das pendências
                    int totalPendencias = pendencias[i].Pendencias?.Count ?? 0;
                    for (int j = 0; j < totalPendencias; j++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento pendencia = repMonitoramentoProcessarEvento.BuscarProcessarEventosPendentes(pendencias[i].Pendencias[j].PosicaoAtual ?? 0, pendencias[i].Pendencias[j].Monitoramento);
                        if (pendencia != null) processarEventos.Add(pendencia);
                    }

                }
                Log("BuscarPendenciasEventos", inicio, 6);
            }
            return processarEventos;
        }

        private void ExcluirArquivosDePendencias()
        {
            int total = this.arquivosEmProcessamento.Count;
            if (total > 0)
            {
                DateTime inicio = DateTime.UtcNow;
                Log($"Excluindo {total} arquivos de pendencias", 1);
                for (int i = 0; i < total; i++)
                {
                    Utilidades.IO.FileStorageService.Storage.Delete(this.arquivosEmProcessamento[i]);
                }
                Log($"Excluidos {total} arquivos de pendencias", inicio, 1);
            }
        }

        private int BusarMaiorTempoEvento(List<Servicos.Embarcador.Monitoramento.Eventos.AbstractEvento> eventos)
        {
            int maiorTempoEvento = 0, total = eventos.Count;
            for (int i = 0; i < total; i++)
            {
                int tempoEvento = Math.Max(eventos[i].GetTempoEvento(), eventos[i].GetTempoEvento2());
                if (tempoEvento > maiorTempoEvento)
                {
                    maiorTempoEvento = tempoEvento;
                }
            }
            return maiorTempoEvento + 30;
        }

        #endregion

    }
}
