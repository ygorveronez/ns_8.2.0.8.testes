using Servicos.Embarcador.Monitoramento;
using System.Data.Common;

namespace Monitoramento.Thread
{

    public class ProcessarMonitoramentos : AbstractThreadProcessamento
    {

        #region Atributos privados

        private static ProcessarMonitoramentos Instante;
        private static System.Threading.Thread ProcessarMonitoramentosThread;

        private int tempoSleep = 5;
        private bool enable = true;
        private int limiteRegistros = 100;
        private string arquivoNivelLog;
        private string codigoIntegracaoViaTansporteMaritima;
        private bool processarTrocaDeAlvo = false;
        private string processarTrocaDeAlvoDiretorioFila;
        private bool enviarNotificacaoMSMQ = false;
        private bool regrasTransito = true;

        private DateTime dataAtual;

        #endregion

        #region Caches

        protected List<Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao> UltimasAtualizacoesPrevisaoEntregaCarga;

        #endregion

        #region Métodos públicos

        // Singleton
        public static ProcessarMonitoramentos GetInstance(string stringConexao)
        {
            if (Instante == null) Instante = new ProcessarMonitoramentos(stringConexao);
            return Instante;
        }

        public System.Threading.Thread Iniciar(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancellationToken)
        {
            if (enable)
                ProcessarMonitoramentosThread = base.IniciarThread(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware, arquivoNivelLog, tempoSleep, cancellationToken);

            return ProcessarMonitoramentosThread;
        }

        public void Finalizar()
        {
            if (enable)
                Parar();
        }

        #endregion

        #region Implementação dos métodos abstratos

        override protected void Executar(Repositorio.UnitOfWork unitOfWork, string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            BuscarProcessarMonitoramentosPendentes(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware);
        }

        override protected void Parar()
        {
            if (ProcessarMonitoramentosThread != null)
            {
                ProcessarMonitoramentosThread.Interrupt();
                ProcessarMonitoramentosThread = null;
            }
        }

        #endregion

        #region Construtor privado

        private ProcessarMonitoramentos(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = string.IsNullOrWhiteSpace(stringConexao) ? null : new Repositorio.UnitOfWork(stringConexao);
            try
            {
                tempoSleep = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().TempoSleepThread;
                enable = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().Ativo;
                limiteRegistros = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().LimiteRegistros;
                arquivoNivelLog = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().ArquivoNivelLog;
                codigoIntegracaoViaTansporteMaritima = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().CodigoIntegracaoViaTansporteMaritima;
                enviarNotificacaoMSMQ = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().EnviarNotificacao;
                regrasTransito = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().RegrasTransito;

                processarTrocaDeAlvo = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().Ativo;
                processarTrocaDeAlvoDiretorioFila = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().DiretorioFila;

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

        #region Métodos privados

        private void BuscarProcessarMonitoramentosPendentes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            this.dataAtual = DateTime.Now;

            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> monitoramentos = BuscarMonitoramentosPendentes(unitOfWork);

            Log(monitoramentos.Count + " monitoramentos pendentes aptos");

            if (monitoramentos.Count > 0)
            {
                // Marcá-los como "Processando"
                try
                {
                    AlterarMonitoramentosProcessar(unitOfWork, monitoramentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processando);
                    Log(monitoramentos.Count + " monitoramentos em processamento");
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    throw;
                }

                //try
                //{
                //    //Vamos processar de 100 em 100 ate finalizar para buscar novamente (garantindo que devemos processar todos)
                //    int take = 200;
                //    int start = 0;

                //    List<Task> tasks = new List<Task>();
                //    while (start < monitoramentos.Count)
                //    {
                //        DateTime inicio = DateTime.UtcNow;
                //        Log("Processando " + start + " de " + monitoramentos.Count + " monitoramentos");
                //        List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> temp = monitoramentos.Skip(start).Take(take).ToList();

                //        // Atualiza as rotas realizadas dos monitoramentos e marcá-los como "Processado"
                //        Task task = new Task(() =>
                //        {
                //            Repositorio.UnitOfWork unitOfWorkAsync = new(unitOfWork.StringConexao, true);
                //            ProcessarMonitoramentosPendentes(unitOfWorkAsync, temp, tipoServicoMultisoftware, clienteMultisoftware);
                //        });

                //        tasks.Add(task);
                //        task.Start();

                //        start += take;
                //    }

                //    Task.WaitAll(tasks.ToArray());
                //    Log(monitoramentos.Count + " monitoramentos processados com sucesso");
                //}
                //catch (Exception ex)
                //{
                //    Servicos.Log.TratarErro(ex);
                //    throw ex;
                //}

                try
                {
                    //vamos processar de 100 em 100 ate finalizar para buscar novamente (garantindo que devemos processar todos)
                    int take = 100;
                    int start = 0;
                    while (start < monitoramentos.Count)
                    {
                        DateTime inicio = DateTime.UtcNow;
                        Log("Processando " + start + " de " + monitoramentos.Count + " monitoramentos");

                        // Atualiza as rotas realizadas dos monitoramentos e marcá-los como "Processado"
                        try
                        {
                            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> temp = monitoramentos.Skip(start).Take(take).ToList();
                            ProcessarMonitoramentosPendentes(unitOfWork, temp, tipoServicoMultisoftware, clienteMultisoftware);
                            Log(temp.Count + " monitoramentos processados com sucesso");
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }

                        start += take;
                        Log("Intervalo de  " + start + " monitoramentos processados com sucesso", inicio, 3);
                    }
                    Log(monitoramentos.Count + " monitoramentos processados com sucesso");
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    throw;
                }

            }

        }

        /**
         * Processa cada um dos monitoramentos para atualizar a rota realizada
         */
        private void ProcessarMonitoramentosPendentes(Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> monitoramentos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.GerenciadorApp
            };

            // Carrega os dados básicos do monitoramento
            List<MonitoramentoPendente> monitoramentosParaAtualizarRota = CarregarDadosMonitoramentos(unitOfWork, monitoramentos);

            // Consulta as posições do veículo
            ConsultaPosicoesDoVeiculo(unitOfWork, monitoramentosParaAtualizarRota);

            //Carrega os dados complexos do monitoramento
            DadosComplexosMonitoramentos dadosComplexosMonitoramento = CarregarDadosComplexosDoMonitoramento(unitOfWork, monitoramentosParaAtualizarRota);

            // Identifica a última posição do monitoramento
            IdentificaUltimaPosicao(unitOfWork, monitoramentosParaAtualizarRota);

            // Gera a rota realizada (polilinha e distância) do monitoramento com as posições do veículo atual
            RoteirizarRotaRealizadaMonitoramento(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoIntegracao);

            // Gera a rota esperada da posição atual até o destino (polilinha e distância) do monitoramento
            RoteirizarRotaAteDestinoMonitoramento(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoIntegracao, dadosComplexosMonitoramento);

            // Cálclo do percentual de viagem, garantindo que não retrocededa
            CalcularPercentualViagem(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoIntegracao, dadosComplexosMonitoramento);

            // Compila as rotas realizadas de todos pos veículos do monitoramento
            CompilarRotaRealizadaDoMonitoramentoVeiculos(unitOfWork, monitoramentosParaAtualizarRota);

            // Salva a rota realizada do veículo atual do monitoramento
            SalvarRotaRealizadaMonitoramentoVeiculo(unitOfWork, monitoramentosParaAtualizarRota);

            // Atualiza os dados do monitoramento
            InformarRotaRealizada(unitOfWork, monitoramentosParaAtualizarRota, configuracao);

            // Atualizar distancia das entregas pela roteirizacao
            CalcularDistanciaAteDestinoCargaEntrega(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoIntegracao, dadosComplexosMonitoramento);

            //Envia atualizacoes para a pagina de acompanhamento carga
            if (enviarNotificacaoMSMQ)
                EnviarAtualizacaoPosicaoMonitoramentos(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoIntegracao, clienteMultisoftware);

        }

        public List<MonitoramentoPendente> CarregarDadosMonitoramentos(Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> monitoramentos)
        {
            DateTime inicio = DateTime.UtcNow;
            List<MonitoramentoPendente> monitoramentosParaAtualizarRota = new List<MonitoramentoPendente>();
            Repositorio.Embarcador.Logistica.MonitoramentoVeiculo repMonitoramentoVeiculo = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculo(unitOfWork);
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            int total = monitoramentos.Count;

            List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> listaMonitoramentos = repMonitoramento.BuscarPorCodigosLista(monitoramentos.Select(x => x.CodigoMonitoramento).ToList());
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> listaMonitoramentosVeiculo = repMonitoramentoVeiculo.BuscarPorCodigosMonitoramentoLista(monitoramentos.Select(x => x.CodigoMonitoramento).ToList());

            for (int i = 0; i < total; i++)
            {
                if (!listaMonitoramentos.Exists(x => x.Codigo == monitoramentos[i].CodigoMonitoramento)) continue;
                MonitoramentoPendente monitoramentoPendente = new MonitoramentoPendente();
                monitoramentoPendente.Monitoramento = listaMonitoramentos.Find(x => x.Codigo == monitoramentos[i].CodigoMonitoramento);
                monitoramentoPendente.DataInicial = monitoramentos[i].DataInicioMonitoramento ?? monitoramentos[i].DataCriacaoMonitoramento;
                monitoramentoPendente.DataFim = monitoramentos[i].DataFimMonitoramento ?? this.dataAtual;
                monitoramentoPendente.MonitoramentoVeiculos = listaMonitoramentosVeiculo.Where(x => x.Monitoramento.Codigo == monitoramentos[i].CodigoMonitoramento).ToList(); //repMonitoramentoVeiculo.BuscarTodosPorMonitoramento(monitoramentos[i].CodigoMonitoramento);
                monitoramentosParaAtualizarRota.Add(monitoramentoPendente);
            }
            Log("CarregarDadosMonitoramentos", inicio, 3);
            return monitoramentosParaAtualizarRota;
        }

        public void ConsultaPosicoesDoVeiculo(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota)
        {
            DateTime inicio = DateTime.UtcNow, inicio1;
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);

            List<Tuple<int, int, DateTime>> monitoramentos = monitoramentosParaAtualizarRota.Select(m => new Tuple<int, int, DateTime>(m.Monitoramento.Codigo, m.Monitoramento.Veiculo.Codigo, m.Monitoramento.DataInicio ?? m.Monitoramento.DataCriacao ?? dataAtual)).ToList();
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculos = repPosicao.BuscarWaypointsPorMonitoramentoVeiculoTuplas(monitoramentos);

            int total = monitoramentosParaAtualizarRota.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null)
                {
                    inicio1 = DateTime.UtcNow;
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = posicoesVeiculos
                        .Where(p => p.CodigoMonitoramento == monitoramentosParaAtualizarRota[i].Monitoramento.Codigo &&
                                    p.CodigoVeiculo == monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo.Codigo &&
                                    p.DataVeiculo > (monitoramentosParaAtualizarRota[i].Monitoramento.DataInicio ?? monitoramentosParaAtualizarRota[i].Monitoramento.DataCriacao ?? dataAtual))
                        .ToList();

                    monitoramentosParaAtualizarRota[i].PosicoesVeiculo = ValidarPosicoes(posicoes);
                    Log($"BuscarWaypointsPorMonitoramentoVeiculo {monitoramentosParaAtualizarRota[i].Monitoramento.Codigo} {monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo.Codigo}: {monitoramentosParaAtualizarRota[i].PosicoesVeiculo.Count}", inicio1, 4);
                }
            }
            Log("ConsultaPosicoesDoVeiculo", inicio, 3);
        }

        public void RoteirizarRotaRealizadaMonitoramento(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            List<Task> tasks = new List<Task>();
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            DateTime inicio = DateTime.UtcNow;
            int total = monitoramentosParaAtualizarRota.Count;

            for (int i = 0; i < total; i++)
            {
                MonitoramentoPendente monitoramentoParaAtualizarRota = monitoramentosParaAtualizarRota[i];
                if (monitoramentoParaAtualizarRota.Monitoramento.Veiculo != null && monitoramentoParaAtualizarRota.PosicoesVeiculo.Count > 0)
                {
                    Task task = new Task(() =>
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = RestringirPosicoesDoPeriodo(monitoramentoParaAtualizarRota.Monitoramento, monitoramentoParaAtualizarRota.PosicoesVeiculo, configuracao);
                        monitoramentoParaAtualizarRota.RespostaRotaRealizada = Servicos.Embarcador.Logistica.Monitoramento.ControleDistancia.ObterRespostaRoteirizacao(posicoes, configuracaoIntegracao.ServidorRouteOSM, true, true);
                    });

                    tasks.Add(task);
                    task.Start();
                }
            }
            int totalTasks = tasks.Count;
            Log($"{totalTasks} tasks criadas", inicio, 4);

            // Aguarda todas as tasks finalizarem
            inicio = DateTime.UtcNow;
            Task.WaitAll(tasks.ToArray());
            Log($"{totalTasks} tasks finalizadas", inicio, 4);

            Log($"RoteirizarRotaRealizadaMonitoramento", inicio, 3);
        }

        public void RoteirizarRotaAteDestinoMonitoramento(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, DadosComplexosMonitoramentos dadosComplexosMonitoramento)
        {
            DateTime inicio = DateTime.UtcNow, inicio1 = DateTime.UtcNow, inicio2;
            List<MonitoramentoPendente> monitoramentosParaRoteirizar = new List<MonitoramentoPendente>();

            int total = monitoramentosParaAtualizarRota.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentosParaAtualizarRota[i].Monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                    monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null &&
                    monitoramentosParaAtualizarRota[i].Posicao != null
                )
                {
                    inicio2 = DateTime.UtcNow;
                    if (monitoramentosParaAtualizarRota[i].CargaEntregas == null) monitoramentosParaAtualizarRota[i].CargaEntregas = dadosComplexosMonitoramento._CargaEntregasTodos.Where(c => c.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo).ToList(); // BuscarCargaEntrega(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);
                    Log("RoteirizarRotaAteDestinoMonitoramento BuscarCargaEntrega", inicio2, 4);

                    inicio2 = DateTime.UtcNow;
                    if (monitoramentosParaAtualizarRota[i].PontosDePassagens == null) monitoramentosParaAtualizarRota[i].PontosDePassagens = dadosComplexosMonitoramento._ListTodosCargaRotaFretePontosPassagem.Where(x => x.CargaRotaFrete.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento?.Carga?.Codigo).ToList(); // BuscarPontosDePassagem(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento);
                    Log("RoteirizarRotaAteDestinoMonitoramento BuscarPontosDePassagem", inicio2, 4);

                    monitoramentosParaRoteirizar.Add(monitoramentosParaAtualizarRota[i]);
                }
            }
            Log("RoteirizarRotaAteDestinoMonitoramento BuscarDados", inicio1, 3);

            List<Task> tasks = new List<Task>();
            inicio = DateTime.UtcNow;
            total = monitoramentosParaRoteirizar.Count;
            for (int i = 0; i < total; i++)
            {
                MonitoramentoPendente monitoramentoParaRoteirizar = monitoramentosParaRoteirizar[i];
                Task task = new Task(() =>
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointAtual = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(monitoramentoParaRoteirizar.Posicao.Latitude, monitoramentoParaRoteirizar.Posicao.Longitude);
                    monitoramentoParaRoteirizar.RespostaRotaAteDestino = Servicos.Embarcador.Monitoramento.Rota.RotaRestanteAteDestino(monitoramentoParaRoteirizar.CargaEntregas, monitoramentoParaRoteirizar.PontosDePassagens, wayPointAtual, configuracaoIntegracao.ServidorRouteOSM);
                });
                tasks.Add(task);
                task.Start();
            }
            int totalTasks = tasks.Count;
            Log($"{totalTasks} tasks criadas", inicio, 4);

            // Aguarda todas as tasks finalizarem
            inicio = DateTime.UtcNow;
            Task.WaitAll(tasks.ToArray());
            Log($"{totalTasks} tasks finalizadas", inicio, 4);

            Log($"RoteirizarRotaAteDestinoMonitoramento", inicio, 3);
        }

        public void IdentificaUltimaPosicao(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota)
        {
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            DateTime inicio = DateTime.UtcNow;

            List<long> UltimasPosicoes = monitoramentosParaAtualizarRota.Where(x => x.PosicoesVeiculo.Count > 0).Select(x => x.PosicoesVeiculo.Last().ID).ToList();
            List<Dominio.Entidades.Embarcador.Logistica.Posicao> ultimasPosicos = repPosicao.BuscarPorCodigosLista(UltimasPosicoes);

            int total = monitoramentosParaAtualizarRota.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null && monitoramentosParaAtualizarRota[i].PosicoesVeiculo.Count > 0)
                {
                    monitoramentosParaAtualizarRota[i].Posicao = ultimasPosicos.FirstOrDefault(x => x.Veiculo.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo.Codigo); //repPosicao.BuscarPorCodigo(monitoramentosParaAtualizarRota[i].PosicoesVeiculo.Last().ID);
                }
            }
            Log("BuscarUltimaPosicaoVeiculo", inicio, 3);
        }

        private void CalcularPercentualViagem(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, DadosComplexosMonitoramentos dadosComplexosMonitoramento)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);

            DateTime inicio = DateTime.UtcNow, inicio1;
            int total = monitoramentosParaAtualizarRota.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null && monitoramentosParaAtualizarRota[i].Posicao != null)
                {
                    if (configuracao.MonitoramentoStatusViagemTipoRegraParaCalcularPercentualViagem != null && monitoramentosParaAtualizarRota[i].HistoricosStatusViagem == null)
                    {
                        monitoramentosParaAtualizarRota[i].HistoricosStatusViagem = dadosComplexosMonitoramento._ListStatusViagemTodosMonitoramentos.Where(s => s.Monitoramento.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Codigo).ToList();// repMonitoramentoHistoricoStatusViagem.BuscarPorMonitoramento(monitoramentosParaAtualizarRota[i].Monitoramento);
                    }

                    if (monitoramentosParaAtualizarRota[i].CargaEntregas == null) monitoramentosParaAtualizarRota[i].CargaEntregas = dadosComplexosMonitoramento._CargaEntregasTodos.Where(c => c.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo).ToList(); // BuscarCargaEntrega(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);

                    inicio1 = DateTime.UtcNow;
                    monitoramentosParaAtualizarRota[i].Percentual = Servicos.Embarcador.Monitoramento.PercentualViagem.CalcularPercentualViagem(monitoramentosParaAtualizarRota[i].Monitoramento, monitoramentosParaAtualizarRota[i].StatusViagem, monitoramentosParaAtualizarRota[i].HistoricosStatusViagem, monitoramentosParaAtualizarRota[i].CargaEntregas, configuracao);
                    Log($"CalcularPercentualViagem {monitoramentosParaAtualizarRota[i].Monitoramento.Codigo}", inicio1, 4);
                }
            }
            Log("CalcularPercentualViagem", inicio, 3);
        }

        private void CompilarRotaRealizadaDoMonitoramentoVeiculos(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota)
        {
            DateTime inicio = DateTime.UtcNow;
            int total = monitoramentosParaAtualizarRota.Count, totalVeiculos;

            for (int i = 0; i < total; i++)
            {
                try
                {
                    if (monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null && monitoramentosParaAtualizarRota[i].MonitoramentoVeiculos != null)
                    {
                        totalVeiculos = monitoramentosParaAtualizarRota[i].MonitoramentoVeiculos.Count;
                        if (totalVeiculos > 0)
                        {
                            // Apenas um veículo, a rota realizada calculada para o veículo é a única para o monitoramento
                            if (totalVeiculos == 1)
                            {
                                monitoramentosParaAtualizarRota[i].RotaRealizadaCompilada = monitoramentosParaAtualizarRota[i].RespostaRotaRealizada;
                            }
                            // ... mais de um veículo utilizado no monitoramento/carga, deve compilar (merge) de todas as rotas realizadas de todos os veículos
                            else
                            {
                                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPoints = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                                decimal distancia = 0;
                                for (int j = 0; j < totalVeiculos; j++)
                                {
                                    if (!string.IsNullOrWhiteSpace(monitoramentosParaAtualizarRota[i].MonitoramentoVeiculos[j].Polilinha))
                                    {
                                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsPolilinha = Servicos.Embarcador.Logistica.Polilinha.Decodificar(monitoramentosParaAtualizarRota[i].MonitoramentoVeiculos[j].Polilinha);
                                        wayPoints = wayPoints.Concat(wayPointsPolilinha).ToList();
                                        distancia += monitoramentosParaAtualizarRota[i].MonitoramentoVeiculos[j].Distancia ?? 0;
                                    }
                                }
                                monitoramentosParaAtualizarRota[i].RotaRealizadaCompilada = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao
                                {
                                    Polilinha = Servicos.Embarcador.Logistica.Polilinha.Codificar(wayPoints),
                                    Distancia = distancia
                                };
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                }
            }


            Log("CompilarRotaRealizadaDoMonitoramentoVeiculos", inicio, 3);
        }

        private void SalvarRotaRealizadaMonitoramentoVeiculo(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota)
        {
            DateTime inicio = DateTime.UtcNow;
            int total = monitoramentosParaAtualizarRota.Count, totalVeiculos;

            Repositorio.Embarcador.Logistica.MonitoramentoVeiculo repMonitoramentoVeiculo = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculo(unitOfWork);
            for (int i = 0; i < total; i++)
            {
                if (monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null && monitoramentosParaAtualizarRota[i].RespostaRotaRealizada != null && monitoramentosParaAtualizarRota[i].MonitoramentoVeiculos != null)
                {
                    totalVeiculos = monitoramentosParaAtualizarRota[i].MonitoramentoVeiculos.Count;
                    for (int j = totalVeiculos - 1; j >= 0; j--)
                    {
                        if (monitoramentosParaAtualizarRota[i].MonitoramentoVeiculos[j].Veiculo.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo.Codigo)
                        {
                            monitoramentosParaAtualizarRota[i].MonitoramentoVeiculos[j].Polilinha = monitoramentosParaAtualizarRota[i].RespostaRotaRealizada.Polilinha;
                            monitoramentosParaAtualizarRota[i].MonitoramentoVeiculos[j].Distancia = monitoramentosParaAtualizarRota[i].RespostaRotaRealizada.Distancia;
                            repMonitoramentoVeiculo.Atualizar(monitoramentosParaAtualizarRota[i].MonitoramentoVeiculos[j]);
                            break;
                        }
                    }
                }
            }

            // TENTATIVA DE ATUALIZAR EM MASSA
            //try
            //{
            //    repMonitoramentoVeiculo.Atualizar(monitoramentosParaAtualizarRota
            //        .SelectMany(monitoramentoParaAtualizarRota => monitoramentoParaAtualizarRota.MonitoramentoVeiculos)
            //        .Where(monitoramentoParaAtualizarRotaVeiculo => monitoramentoParaAtualizarRotaVeiculo.Polilinha != null && monitoramentoParaAtualizarRotaVeiculo.Distancia != null)
            //        .ToList(),
            //        "T_MONITORAMENTO_VEICULO",
            //        new List<string>() { "Polilinha", "Distancia" },
            //        50);
            //}
            //catch (Exception e)
            //{
            //    Servicos.Log.TratarErro(e);
            //}
            Log("SalvarRotaRealizadaMonitoramentoVeiculo", inicio, 3);
        }

        private void InformarRotaRealizada(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

            DateTime inicio = DateTime.UtcNow, inicio1;
            int total = monitoramentosParaAtualizarRota.Count;
            for (int i = 0; i < total; i++)
            {
                DbConnection connection = unitOfWork.GetConnection();
                DbTransaction transaction = connection.BeginTransaction();
                try
                {
                    if (monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null && monitoramentosParaAtualizarRota[i].Posicao != null)
                    {

                        inicio1 = DateTime.UtcNow;
                        repMonitoramento.AtualizarRotaRealizada(
                            monitoramentosParaAtualizarRota[i].Monitoramento.Codigo,
                            monitoramentosParaAtualizarRota[i].RotaRealizadaCompilada?.Polilinha ?? string.Empty,
                            monitoramentosParaAtualizarRota[i].RotaRealizadaCompilada?.Distancia ?? 0,
                            string.Empty,
                            0,
                            monitoramentosParaAtualizarRota[i].RespostaRotaAteDestino?.Polilinha ?? string.Empty,
                            monitoramentosParaAtualizarRota[i].RespostaRotaAteDestino?.Distancia ?? 0,
                            0,
                            monitoramentosParaAtualizarRota[i].Percentual,
                            true,
                            connection,
                            transaction
                        );
                        Log($"InformarRotaRealizada {monitoramentosParaAtualizarRota[i].Monitoramento.Codigo}", inicio1, 4);

                    }
                    else
                    {
                        inicio1 = DateTime.UtcNow;
                        repMonitoramento.AtualizarProcessarRota(new List<int>() { monitoramentosParaAtualizarRota[i].Monitoramento.Codigo }, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado, connection, transaction);
                        Log($"AtualizarProcessarRota {monitoramentosParaAtualizarRota[i].Monitoramento.Codigo}", inicio1, 4);
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Servicos.Log.TratarErro(e);
                    throw;
                }
                finally { transaction.Dispose(); }
            }
            Log("InformarRotaRealizada", inicio, 3);
        }

        private void EnviarAtualizacaoPosicaoMonitoramentos(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
            Servicos.Embarcador.Monitoramento.Monitoramento servMonitoramento = new Servicos.Embarcador.Monitoramento.Monitoramento();
            DateTime inicio = DateTime.UtcNow;
            List<int> ListaCargas = new List<int>();

            for (int i = 0; i < monitoramentosParaAtualizarRota.Count; i++)
            {
                if ((monitoramentosParaAtualizarRota[i].RespostaRotaRealizada?.Distancia ?? 0) > 0)
                    ListaCargas.Add(monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo);
            }

            if (ListaCargas.Count > 0)
            {
                servAlertaAcompanhamentoCarga.informarAtualizacaoListaCargasAcompanamentoMSMQ(ListaCargas, clienteMultisoftware.Codigo);
                servMonitoramento.InformarAtualizacaoListaMonitoramentosMSMQ(ListaCargas, clienteMultisoftware.Codigo, unitOfWork);
            }

            Log($"EnviarAtualizacaoPosicaoMonitoramentos", inicio, 3);
        }

        private IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> BuscarMonitoramentosPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> monitoramentos = repMonitoramento.BuscarProcessarComLimite(limiteRegistros);

            Log(monitoramentos.Count + " monitoramentos pendentes", 1);

            return monitoramentos;
        }

        private void AlterarMonitoramentosProcessar(Repositorio.UnitOfWork unitOfWork, IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> monitoramentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao processar)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            int total = monitoramentos.Count;
            int take = 1000;
            int start = 0;
            while (start < total)
            {
                DbConnection connection = unitOfWork.GetConnection();
                DbTransaction transaction = connection.BeginTransaction();
                try
                {
                    List<int> codigos = monitoramentos.Skip(start).Take(take).Select(m => m.CodigoMonitoramento).ToList();
                    repMonitoramento.AtualizarProcessarRota(codigos, processar, connection, transaction);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Servicos.Log.TratarErro(e);
                }
                finally { transaction.Dispose(); }
                start += take;
            }
        }

        private void CalcularDistanciaAteDestinoCargaEntrega(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, DadosComplexosMonitoramentos dadosComplexosMonitoramento)
        {
            Log("Calculando distancias ate destinos das entregas", 3);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

            DateTime inicio = DateTime.UtcNow, inicio1;
            int total = monitoramentosParaAtualizarRota.Count;
            for (int i = 0; i < total; i++)
            {
                try
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> pontosNaRotaAteDestino = null;
                    if (monitoramentosParaAtualizarRota[i].RespostaRotaAteDestino != null && !string.IsNullOrWhiteSpace(monitoramentosParaAtualizarRota[i].RespostaRotaAteDestino.PontoDaRota))
                    {
                        pontosNaRotaAteDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>>(monitoramentosParaAtualizarRota[i].RespostaRotaAteDestino.PontoDaRota);
                    }

                    inicio1 = DateTime.UtcNow;
                    if (monitoramentosParaAtualizarRota[i].CargaEntregas == null) monitoramentosParaAtualizarRota[i].CargaEntregas = dadosComplexosMonitoramento._CargaEntregasTodos.Where(c => c.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo).ToList(); // BuscarCargaEntrega(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);
                    Log("CalcularDistanciaAteDestinoCargaEntrega BuscarCargaEntrega", inicio1, 4);

                    inicio1 = DateTime.UtcNow;
                    if (monitoramentosParaAtualizarRota[i].PontosDePassagens == null) monitoramentosParaAtualizarRota[i].PontosDePassagens = dadosComplexosMonitoramento._ListTodosCargaRotaFretePontosPassagem.Where(x => x.CargaRotaFrete.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento?.Carga?.Codigo).ToList(); // BuscarPontosDePassagem(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento);
                    Log("CalcularDistanciaAteDestinoCargaEntrega BuscarPontosDePassagem", inicio1, 4);

                    inicio1 = DateTime.UtcNow;
                    int totalEntregas = monitoramentosParaAtualizarRota[i].CargaEntregas?.Count ?? 0;
                    for (int j = 0; j < totalEntregas; j++)
                    {
                        monitoramentosParaAtualizarRota[i].CargaEntregas[j].DistanciaAteDestino = Servicos.Embarcador.Monitoramento.Rota.CalcularDistanciaAteEntrega(monitoramentosParaAtualizarRota[i].CargaEntregas[j], monitoramentosParaAtualizarRota[i].PontosDePassagens, pontosNaRotaAteDestino);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(monitoramentosParaAtualizarRota[i].CargaEntregas[j], repCargaEntrega, unitOfWork, configControleEntrega);
                    }

                    repCargaEntrega.Atualizar(monitoramentosParaAtualizarRota[i].CargaEntregas, new List<string>() { "DistanciaAteDestino" }, "T_CARGA_ENTREGA");

                    Log($"CalcularDistanciaAteDestinoCargaEntrega CalcularDistanciaAteEntrega {totalEntregas}", inicio1, 4);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                }
            }

            //try
            //{
            //    repCargaEntrega.Atualizar(monitoramentosParaAtualizarRota
            //        .SelectMany(monitoramentoParaAtualizarRota => monitoramentoParaAtualizarRota.CargaEntregas)
            //        .Where(monitoramentoParaAtualizarRotaEntrega => monitoramentoParaAtualizarRotaEntrega.DistanciaAteDestino != null)
            //        .ToList(),
            //        "T_CARGA_ENTREGA",
            //        new List<string>() { "DistanciaAteDestino" },
            //        50);
            //}
            //catch (Exception e)
            //{
            //    Servicos.Log.TratarErro(e);
            //}
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ValidarPosicoes(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesValidas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            int total = posicoes.Count;
            for (int i = 0; i < total; i++)
            {
                if (Servicos.Embarcador.Logistica.WayPointUtil.ValidarCoordenadas(posicoes[i].Latitude, posicoes[i].Longitude))
                {
                    posicoesValidas.Add(posicoes[i]);
                }
            }
            return posicoesValidas;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> RestringirPosicoesDoPeriodo(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesValidas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            DateTime? dataInicio;

            // Identifica a data início do histórico das posições para considerar como rota realizada
            dataInicio = null;
            if (configuracao.AtualizarRotaRealizadaDoMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoCriarMonitoramento)
            {
                dataInicio = monitoramento.DataInicio ?? monitoramento.DataCriacao;
            }
            else if (configuracao.AtualizarRotaRealizadaDoMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarMonitoramento && monitoramento.DataInicio != null)
            {
                dataInicio = monitoramento.DataInicio.Value;
            }
            else if (configuracao.AtualizarRotaRealizadaDoMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarViagem && monitoramento.Carga != null && monitoramento.Carga.DataInicioViagem != null)
            {
                dataInicio = monitoramento.Carga.DataInicioViagem.Value;
            }

            //#32445 verificar no tipoOperacao se configurado;
            if (monitoramento.Carga != null && monitoramento.Carga.TipoOperacao != null && monitoramento.Carga.TipoOperacao.AtualizarRotaRealizadaDoMonitoramento)
            {

                if (monitoramento.Carga.TipoOperacao.QuandoProcessarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoCriarMonitoramento)
                {
                    dataInicio = monitoramento.DataInicio ?? monitoramento.DataCriacao;
                }
                else if (monitoramento.Carga.TipoOperacao.QuandoProcessarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarMonitoramento && monitoramento.DataInicio != null)
                {
                    dataInicio = monitoramento.DataInicio.Value;
                }
                else if (monitoramento.Carga.TipoOperacao.QuandoProcessarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarViagem && monitoramento.Carga.DataInicioViagem != null)
                {
                    dataInicio = monitoramento.Carga.DataInicioViagem.Value;
                }
            }


            if (dataInicio != null)
            {
                int total = posicoes.Count;
                for (int i = 0; i < total; i++)
                {
                    if (posicoes[i].DataVeiculo >= dataInicio)
                    {
                        posicoesValidas.Add(posicoes[i]);
                    }
                }
            }

            return posicoesValidas;
        }

        #endregion
    }
}
