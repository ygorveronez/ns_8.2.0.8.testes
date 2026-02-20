using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.Embarcador.Monitoramento;
using System.Data.Common;

namespace Monitoramento.Thread
{
    class ProcessarMonitoramentosStatusViagem : AbstractThreadProcessamento
    {
        #region Atributos privados

        private static ProcessarMonitoramentosStatusViagem Instante;
        private static System.Threading.Thread ProcessarMonitoramentosThread;

        private int tempoSleep = 5;
        private bool enable = true;
        private int limiteRegisrosStatusViagem = 100;
        private string arquivoNivelLog;
        private string codigoIntegracaoViaTansporteMaritima;
        private bool possuiColetaContainer = false;
        private int intervaloCalculoPrevisaoCargaEntregaMinutos;
        private bool regrasTransito = true;
        private bool enviarNotificacaoMSMQ = false;

        private DateTime dataAtual;

        #endregion

        #region Caches

        protected List<Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao> UltimasAtualizacoesPrevisaoEntregaCarga;

        #endregion

        #region Métodos públicos

        // Singleton
        public static ProcessarMonitoramentosStatusViagem GetInstance(string stringConexao)
        {
            if (Instante == null) Instante = new ProcessarMonitoramentosStatusViagem(stringConexao);
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
            BuscarProcessarMonitoramentosPendentesStatusViagem(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware);
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

        private ProcessarMonitoramentosStatusViagem(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = string.IsNullOrWhiteSpace(stringConexao) ? null : new Repositorio.UnitOfWork(stringConexao);
            try
            {
                tempoSleep = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().TempoSleepThread;
                enable = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().Ativo;
                limiteRegisrosStatusViagem = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().LimiteRegistros;
                arquivoNivelLog = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().ArquivoNivelLog;
                codigoIntegracaoViaTansporteMaritima = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().CodigoIntegracaoViaTansporteMaritima;
                intervaloCalculoPrevisaoCargaEntregaMinutos = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().IntervaloCalculoPrevisaoCargaEntregaMinutos;
                regrasTransito = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().RegrasTransito;
                possuiColetaContainer = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().PossuiColetaContainer;
                enviarNotificacaoMSMQ = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().EnviarNotificacao;
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

        private void BuscarProcessarMonitoramentosPendentesStatusViagem(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            this.dataAtual = DateTime.Now;

            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> monitoramentos = BuscarMonitoramentosPendentesStatusViagem(unitOfWork);
            Log(monitoramentos.Count + " monitoramentos pendentes aptos");

            if (monitoramentos.Count > 0)
            {
                // Atualiza as rotas realizadas dos monitoramentos e marcá-los como "Processado"
                try
                {
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repositorioConfguracoesMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repositorioConfguracoesMonitoramento.BuscarConfiguracaoPadrao();

                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                    Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

                    Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracoesControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracoesControleEntrega.ObterConfiguracaoPadrao();

                    //vamos processar de 100 em 100 ate finalizar para buscar novamente (garantindo que devemos processar todos)
                    int take = 200;
                    int start = 0;

                    List<Task> tasks = new List<Task>();
                    while (start < monitoramentos.Count)
                    {
                        DateTime inicio = DateTime.UtcNow;

                        Log("Processando " + start + " de " + monitoramentos.Count + " monitoramentos");

                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> temp = monitoramentos.Skip(start).Take(take).ToList();

                        Task task = new Task(() =>
                        {
                            Repositorio.UnitOfWork unitOfWorkAsync = new(unitOfWork.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.AtualizarAtual);
                            ProcessarMonitoramentosPendentesStatusViagem(unitOfWorkAsync, temp, tipoServicoMultisoftware, clienteMultisoftware, configuracaoControleEntrega, configuracaoIntegracao, configuracao, configuracaoMonitoramento);
                        });
                        tasks.Add(task);
                        task.Start();

                        start += take;

                        Log("Intervalo de  " + start + " monitoramentos processados com sucesso", inicio, 3);

                    }

                    Task.WaitAll(tasks.ToArray());

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
         * vai processar cada um dos monitoramentos apenas para Atualizar a Rota e calcular Previsoes
         */
        private void ProcessarMonitoramentosPendentesStatusViagem(Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> monitoramentos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.GerenciadorApp
            };

            // Carrega os dados básicos do monitoramento
            List<MonitoramentoPendente> monitoramentosParaAtualizarRota = CarregarDadosMonitoramentos(unitOfWork, monitoramentos);

            // Consulta as posições do veículo
            ConsultaPosicoesDoVeiculo(unitOfWork, monitoramentosParaAtualizarRota);

            // Identifica a última posição do monitoramento
            IdentificaUltimaPosicao(unitOfWork, monitoramentosParaAtualizarRota);

            //Carrega os dados complexos do monitoramento
            DadosComplexosMonitoramentos dadosComplexosMonitoramento = CarregarDadosComplexosDoMonitoramento(unitOfWork, monitoramentosParaAtualizarRota);

            // Processa e identicia os novos status dos monitoramentos
            IdentificarNovoStatusViagem(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoMonitoramento, tipoServicoMultisoftware, dadosComplexosMonitoramento);

            // Gera a rota esperada da posição atual até a origem (polilinha e distância) do monitoramento
            RoteirizarRotaAteOrigemMonitoramento(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoIntegracao, dadosComplexosMonitoramento);

            // Processa alterações na carga e dependencias de acordo com o novo status do monitoramento (por enquanto apena para containers)
            if (possuiColetaContainer)
                ProcessarAlteracoesCargaDependendoStatusViagem(unitOfWork, monitoramentosParaAtualizarRota, tipoServicoMultisoftware);

            // Início de viagem caso já esteja em trânsito mas sem haver iniciado a viagem
            VerificarInicioViagemPorTransito(unitOfWork, monitoramentosParaAtualizarRota, configuracao, tipoServicoMultisoftware, clienteMultisoftware);

            // Atualizar as datas de previsão de chegada na origem
            EstimarPrevisaoChegadaOrigem(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoIntegracao);

            // Atualiza os dados do monitoramento
            InformarRotaRealizada(unitOfWork, monitoramentosParaAtualizarRota, configuracao);

            // Finaliza monitoramentos concluídos por finalização de entregas e carga sem o devido registro do monitoramento
            FinalizarMonitoramentosConcluidos(unitOfWork, monitoramentosParaAtualizarRota, configuracao, auditoria);

            // Atualizar as datas reprogramadas e data prevista das entregas
            CalcularDefinirPrevisaoCargaEntrega(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoIntegracao, tipoServicoMultisoftware, auditoria, configuracaoControleEntrega, dadosComplexosMonitoramento);

            if (enviarNotificacaoMSMQ)
                EnviarAtualizacaoStatusMonitoramentos(unitOfWork, monitoramentosParaAtualizarRota, clienteMultisoftware);
        }

        public List<MonitoramentoPendente> CarregarDadosMonitoramentos(Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> monitoramentos)
        {
            DateTime inicio = DateTime.UtcNow;
            List<MonitoramentoPendente> monitoramentosParaAtualizarRota = new List<MonitoramentoPendente>();
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            int total = monitoramentos.Count;

            List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> listaMonitoramentos = repMonitoramento.BuscarPorCodigosLista(monitoramentos.Select(x => x.CodigoMonitoramento).ToList());

            for (int i = 0; i < total; i++)
            {
                if (!listaMonitoramentos.Exists(x => x.Codigo == monitoramentos[i].CodigoMonitoramento)) continue;
                MonitoramentoPendente monitoramentoPendente = new MonitoramentoPendente();
                monitoramentoPendente.Monitoramento = listaMonitoramentos.Find(x => x.Codigo == monitoramentos[i].CodigoMonitoramento);
                monitoramentoPendente.DataInicial = monitoramentos[i].DataInicioMonitoramento ?? monitoramentos[i].DataCriacaoMonitoramento;
                monitoramentoPendente.DataFim = monitoramentos[i].DataFimMonitoramento ?? this.dataAtual;
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

        public void ProcessarAlteracoesCargaDependendoStatusViagem(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
            Servicos.Embarcador.Pedido.ColetaContainer servColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);

            DateTime inicio = DateTime.UtcNow, inicio1;
            int total = monitoramentosParaAtualizarRota.Count;
            for (int i = 0; i < total; i++)
            {
                try
                {
                    if (monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null && monitoramentosParaAtualizarRota[i].Posicao != null && monitoramentosParaAtualizarRota[i].StatusViagem != null)
                    {
                        if (monitoramentosParaAtualizarRota[i].Monitoramento.StatusViagem == null || monitoramentosParaAtualizarRota[i].StatusViagem.Codigo != monitoramentosParaAtualizarRota[i].Monitoramento.StatusViagem.Codigo)
                        {
                            // ... há um novo status no monitoramento
                            if (monitoramentosParaAtualizarRota[i].StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmCarregamento
                                || monitoramentosParaAtualizarRota[i].StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoCarregamento
                                || monitoramentosParaAtualizarRota[i].StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.AguardandoHorarioCarregamento
                                || monitoramentosParaAtualizarRota[i].StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmLiberacao)
                            {
                                inicio1 = DateTime.UtcNow;

                                if (monitoramentosParaAtualizarRota[i].Monitoramento.Carga != null)
                                {
                                    Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repColetaContainer.BuscarPorCargaAtual(monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo);

                                    if (coletaContainer != null && coletaContainer.Container != null)
                                    {
                                        List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer> statusAnterioresAoEmCarregamento = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer>()
                                                                    {
                                                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer.AgColeta,
                                                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer.EmAreaEsperaVazio,
                                                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer.EmDeslocamentoCarregamento,
                                                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer.EmDeslocamentoCarregamento
                                                                    };

                                        if (statusAnterioresAoEmCarregamento.Contains(coletaContainer.Status))
                                        {

                                            Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametrosColetaContainer = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro();
                                            parametrosColetaContainer.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer.EmCarregamento;
                                            parametrosColetaContainer.DataAtualizacao = DateTime.Now;
                                            parametrosColetaContainer.coletaContainer = coletaContainer;
                                            parametrosColetaContainer.LocalAtual = monitoramentosParaAtualizarRota[i].ClienteOrigem;
                                            parametrosColetaContainer.OrigemMonimentacaoContainer = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMovimentacaoContainer.Tracking;
                                            parametrosColetaContainer.InformacaoOrigemMonimentacaoContainer = Dominio.ObjetosDeValor.Embarcador.Enumeradores.InformacaoOrigemMovimentacaoContainer.ProcessarMonitoramentoEmCarregamento;

                                            servColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(parametrosColetaContainer);
                                        }

                                        Log($"Registrou alterações", inicio1, 4);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    throw;
                }
            }
            Log("ProcessarAlteracoesCargaDependendoStatusViagem", inicio, 3);
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
                    monitoramentosParaAtualizarRota[i].Posicao = ultimasPosicos?.FirstOrDefault(x => x.Veiculo.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo.Codigo); //repPosicao.BuscarPorCodigo(monitoramentosParaAtualizarRota[i].PosicoesVeiculo.Last().ID);
                }
            }
            Log("BuscarUltimaPosicaoVeiculo", inicio, 3);
        }

        private void EnviarAtualizacaoStatusMonitoramentos(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Servicos.Embarcador.Monitoramento.Monitoramento servMonitoramento = new Servicos.Embarcador.Monitoramento.Monitoramento();
            DateTime inicio = DateTime.UtcNow;
            List<int> ListaCargas = new List<int>();

            for (int i = 0; i < monitoramentosParaAtualizarRota.Count; i++)
            {
                if ((monitoramentosParaAtualizarRota[i].RespostaRotaRealizada?.Distancia ?? 0) > 0)
                    ListaCargas.Add(monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo);
            }

            if (ListaCargas.Count > 0)
                servMonitoramento.InformarAtualizacaoListaMonitoramentosMSMQ(ListaCargas, clienteMultisoftware.Codigo, unitOfWork);

            Log($"EnviarAtualizacaoStatusMonitoramentos", inicio, 3);
        }

        private void IdentificarNovoStatusViagem(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, DadosComplexosMonitoramentos dadosComplexosMonitoramento)
        {
            // Consulta os status de viagem cadastrados
            Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> monitoramentoStatusViagens = repMonitoramentoStatusViagem.BuscarAtivos();

            // Subáreas dos clientes
            Repositorio.Embarcador.Logistica.SubareaCliente repSubareaCliente = new Repositorio.Embarcador.Logistica.SubareaCliente(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.SubareaCliente[] subareas = repSubareaCliente.BuscarAtivas();

            // Verifica há necessidade de consultar algumas informações de acordo com os status configurados
            bool precisaDoHistoricoStatusViagem = Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.VerificaStatusNecessitamDeHistoricoStatusViagem(monitoramentoStatusViagens);
            bool precisaDePontosDePassagem = Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.VerificaStatusNecessitamDePontosDePassagem(monitoramentoStatusViagens);
            bool precisaDeCargaEntregas = Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.VerificaStatusNecessitamDeCargaEntrega(monitoramentoStatusViagens);
            bool precisaDeViaTransporte = Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.VerificaStatusNecessitamDeViaTransporte(monitoramentoStatusViagens);
            bool precisaDeJanelaCarregamento = Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.VerificaStatusNecessitamDeJanelaCarregamento(monitoramentoStatusViagens);
            bool precisaDeJanelaDescarregamento = Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.VerificaStatusNecessitamDeJanelaDescarregamento(monitoramentoStatusViagens);

            DateTime inicio = DateTime.UtcNow, inicio1;
            int total = monitoramentosParaAtualizarRota.Count;

            for (int i = 0; i < total; i++)
            {
                if (monitoramentosParaAtualizarRota[i].Monitoramento != null && monitoramentosParaAtualizarRota[i].Monitoramento.StatusViagem != null && monitoramentosParaAtualizarRota[i].Monitoramento.StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida)
                {
                    monitoramentosParaAtualizarRota[i].StatusViagem = monitoramentoStatusViagens.Where(x => x.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida).FirstOrDefault();
                    return;
                }

                if (monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null && monitoramentosParaAtualizarRota[i].Posicao != null)
                {
                    inicio1 = DateTime.UtcNow;
                    if (precisaDoHistoricoStatusViagem && monitoramentosParaAtualizarRota[i].HistoricosStatusViagem == null) monitoramentosParaAtualizarRota[i].HistoricosStatusViagem = dadosComplexosMonitoramento._ListStatusViagemTodosMonitoramentos.Where(x => x.Monitoramento.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento?.Codigo).ToList(); // repMonitoramentoHistoricoStatusViagem.BuscarPorMonitoramento(monitoramentosParaAtualizarRota[i].Monitoramento);
                    Log("HistoricoStatusViagem", inicio1, 4);

                    inicio1 = DateTime.UtcNow;
                    if (precisaDePontosDePassagem && monitoramentosParaAtualizarRota[i].PontosDePassagens == null) monitoramentosParaAtualizarRota[i].PontosDePassagens = dadosComplexosMonitoramento._ListTodosCargaRotaFretePontosPassagem.Where(x => x.CargaRotaFrete.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento?.Carga?.Codigo).ToList(); // BuscarPontosDePassagem(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento);
                    Log("BuscarPontosDePassagem", inicio1, 4);

                    inicio1 = DateTime.UtcNow;
                    List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes = dadosComplexosMonitoramento._ListPermanenciasClientesTodos.Where(x => x.CargaEntrega.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento?.Carga?.Codigo).ToList(); // repPermanenciaCliente.BuscarPorCarga(monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo);
                    Log("BuscarPermanenciasCliente", inicio1, 4);

                    inicio1 = DateTime.UtcNow;
                    List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas = dadosComplexosMonitoramento._ListPermanenciasSubareasTodos.Where(x => x.CargaEntrega.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento?.Carga?.Codigo).ToList();  // repPermanenciaSubarea.BuscarPorCarga(monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo);
                    Log("BuscarPermanenciasSubarea", inicio1, 4);

                    inicio1 = DateTime.UtcNow;
                    if (precisaDeCargaEntregas && monitoramentosParaAtualizarRota[i].CargaEntregas == null) monitoramentosParaAtualizarRota[i].CargaEntregas = dadosComplexosMonitoramento._CargaEntregasTodos.Where(c => c.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo).ToList(); // BuscarCargaEntrega(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);
                    Log("BuscarControleEntrega", inicio1, 4);

                    inicio1 = DateTime.UtcNow;
                    Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte = null;
                    if (precisaDeViaTransporte) viaTransporte = BuscarViaTransporte(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);
                    Log("BuscarViaTransporte", inicio1, 4);

                    //TIRAR TODOS E TRAZER PARA LISTAS UNICAS.
                    inicio1 = DateTime.UtcNow;
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = null;
                    if (precisaDeJanelaCarregamento) cargaJanelaCarregamento = dadosComplexosMonitoramento._cargaJanelaCarregamentosTodos.FirstOrDefault(j => j.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo); // BuscarJanelaCarregamento(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);
                    Log("BuscarJanelaCarregamento", inicio1, 4);

                    //TIRAR TODOS E TRAZER PARA LISTAS UNICAS.
                    inicio1 = DateTime.UtcNow;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento = null;
                    if (precisaDeJanelaDescarregamento) cargaJanelasDescarregamento = dadosComplexosMonitoramento._cargaJanelasDescarregamentosTodos.Where(d => d.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo).ToList(); // BuscarJanelasDescarregamento(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);
                    Log("BuscarJanelasDescarregamento", inicio1, 4);

                    //TIRAR TODOS E TRAZER PARA LISTAS UNICAS.
                    inicio1 = DateTime.UtcNow;
                    if (monitoramentosParaAtualizarRota[i].PosicaoAlvos == null) monitoramentosParaAtualizarRota[i].PosicaoAlvos = dadosComplexosMonitoramento._posicoesAlvoTodos.Where(p => p.Posicao.Codigo == monitoramentosParaAtualizarRota[i].Posicao.Codigo).ToList(); // BuscarPosicaoAlvos(unitOfWork, monitoramentosParaAtualizarRota[i].Posicao.Codigo);
                    if (monitoramentosParaAtualizarRota[i].PosicaoAlvoSubareas == null) monitoramentosParaAtualizarRota[i].PosicaoAlvoSubareas = dadosComplexosMonitoramento._posicoesAlvoSubareaTodos.Where(p => p.PosicaoAlvo.Posicao.Codigo == monitoramentosParaAtualizarRota[i].Posicao.Codigo).ToList(); // BuscarPosicaoAlvosSubareas(unitOfWork, monitoramentosParaAtualizarRota[i].Posicao.Codigo);
                    Log("BuscarAlvos", inicio1, 4);

                    //TIRAR TODOS E TRAZER PARA LISTAS UNICAS.
                    inicio1 = DateTime.UtcNow;
                    if (monitoramentosParaAtualizarRota[i].ClienteOrigem == null) monitoramentosParaAtualizarRota[i].ClienteOrigem = dadosComplexosMonitoramento._clientesOrigemTodos.FirstOrDefault(p => p.Item1 == monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo)?.Item2; // Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);
                    Log("BuscarClienteOrigemDaCargaPeloPedido", inicio1, 4);

                    inicio1 = DateTime.UtcNow;

                    monitoramentosParaAtualizarRota[i].StatusViagem = Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.IdentificarStatusViagem(
                        monitoramentoStatusViagens,
                        monitoramentosParaAtualizarRota[i].Monitoramento,
                        monitoramentosParaAtualizarRota[i].Posicao,
                        monitoramentosParaAtualizarRota[i].PosicaoAlvos,
                        monitoramentosParaAtualizarRota[i].PosicaoAlvoSubareas,
                        permanenciasClientes,
                        permanenciasSubareas,
                        monitoramentosParaAtualizarRota[i].PontosDePassagens,
                        subareas,
                        monitoramentosParaAtualizarRota[i].CargaEntregas,
                        monitoramentosParaAtualizarRota[i].HistoricosStatusViagem,
                        monitoramentosParaAtualizarRota[i].PosicoesVeiculo,
                        configuracao,
                        configuracaoMonitoramento,
                        monitoramentosParaAtualizarRota[i].ClienteOrigem,
                        viaTransporte,
                        codigoIntegracaoViaTansporteMaritima,
                        cargaJanelaCarregamento,
                        cargaJanelasDescarregamento,
                        regrasTransito
                    );
                    Log($"IdentificarStatusViagem {monitoramentosParaAtualizarRota[i].Monitoramento.Codigo}", inicio1, 4);

                    inicio1 = DateTime.UtcNow;
                    //Desvincula o cavalo da carga quando o status da viagem estiver Em Parqueamento
                    //e a flag Exige que a placa da tração seja informada na carga estiver marcada.
                    Servicos.Embarcador.Monitoramento.Carga.DesvincularCavaloNaCargaComStatusEmParqueamento(monitoramentosParaAtualizarRota[i].Monitoramento, unitOfWork);
                    Log($"DesvincularCavaloDaCarga {monitoramentosParaAtualizarRota[i].Monitoramento.Codigo}", inicio1, 4);

                    inicio1 = DateTime.UtcNow;

                    monitoramentosParaAtualizarRota[i].DataInicialStatusViagem = Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.IdentificarDataInicialStatusViagem(
                        monitoramentosParaAtualizarRota[i].StatusViagem,
                        monitoramentosParaAtualizarRota[i].Posicao,
                        monitoramentosParaAtualizarRota[i].PosicaoAlvos,
                        permanenciasClientes,
                        permanenciasSubareas,
                        monitoramentosParaAtualizarRota[i].HistoricosStatusViagem
                    );

                    Log($"IdentificarDataInicialStatusViagem {monitoramentosParaAtualizarRota[i].Monitoramento.Codigo}", inicio1, 4);

                }
            }
            Log($"IdentificarNovoStatusViagem", inicio, 3);
        }

        private void VerificarInicioViagemPorTransito(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            DateTime inicio = DateTime.UtcNow, inicio1;
            int total = monitoramentosParaAtualizarRota.Count;
            for (int i = 0; i < total; i++)
            {
                bool iniciarViagemPeloStatus = monitoramentosParaAtualizarRota[i].Monitoramento.Carga.TipoOperacao?.IniciarViagemPeloStatusViagem ?? false;

                if (monitoramentosParaAtualizarRota[i].Posicao != null &&
                    monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null &&
                    monitoramentosParaAtualizarRota[i].Monitoramento.Carga != null &&
                    monitoramentosParaAtualizarRota[i].Monitoramento.Carga.DataInicioViagem == null &&
                    monitoramentosParaAtualizarRota[i].StatusViagem != null &&
                    monitoramentosParaAtualizarRota[i].StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Transito &&
                    configuracao.QuandoIniciarViagemViaMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarViagemViaMonitoramento.NoStatusViagemTransito &&
                    configuracao.PossuiMonitoramento
                )
                {
                    inicio1 = DateTime.UtcNow;
                    Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint()
                    {
                        Latitude = monitoramentosParaAtualizarRota[i].Posicao.Latitude,
                        Longitude = monitoramentosParaAtualizarRota[i].Posicao.Longitude
                    };
                    Servicos.Embarcador.Monitoramento.Carga.IniciarViagemEPrimeiraColetaAsync(
                        monitoramentosParaAtualizarRota[i].Monitoramento,
                        wayPoint,
                        monitoramentosParaAtualizarRota[i].Posicao.DataVeiculo,
                        monitoramentosParaAtualizarRota[i].ClienteOrigem,
                        configuracao,
                        tipoServicoMultisoftware,
                        clienteMultisoftware,
                        unitOfWork
                    );
                    Log($"IniciarViagem {monitoramentosParaAtualizarRota[i].Monitoramento.Codigo}", inicio1, 4);
                }
                else if (monitoramentosParaAtualizarRota[i].Posicao != null && // feito para DPA cargas Milk na qual nao recebe posicoes na planta e assim nao indica data inicio viagem.
                   monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null &&
                   monitoramentosParaAtualizarRota[i].Monitoramento.Carga != null &&
                   monitoramentosParaAtualizarRota[i].Monitoramento.Carga.DataInicioViagem == null &&
                   monitoramentosParaAtualizarRota[i].StatusViagem != null &&
                   monitoramentosParaAtualizarRota[i].StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmViagem &&
                   iniciarViagemPeloStatus && configuracao.PossuiMonitoramento
               )
                {
                    inicio1 = DateTime.UtcNow;
                    Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint()
                    {
                        Latitude = monitoramentosParaAtualizarRota[i].Posicao.Latitude,
                        Longitude = monitoramentosParaAtualizarRota[i].Posicao.Longitude
                    };
                    Servicos.Embarcador.Monitoramento.Carga.IniciarViagemEPrimeiraColetaAsync(
                        monitoramentosParaAtualizarRota[i].Monitoramento,
                        wayPoint,
                        monitoramentosParaAtualizarRota[i].Posicao.DataVeiculo,
                        monitoramentosParaAtualizarRota[i].ClienteOrigem,
                        configuracao,
                        tipoServicoMultisoftware,
                        clienteMultisoftware,
                        unitOfWork
                    );
                    Log($"IniciarViagem {monitoramentosParaAtualizarRota[i].Monitoramento.Codigo}", inicio1, 4);
                }
            }
            Log($"VerificarInicioViagemPorTransito", inicio, 3);
        }

        public void RoteirizarRotaAteOrigemMonitoramento(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, DadosComplexosMonitoramentos dadosComplexosMonitoramento)
        {
            DateTime inicio = DateTime.UtcNow, inicio1 = DateTime.UtcNow, inicio2;
            List<MonitoramentoPendente> monitoramentosParaRoteirizar = new List<MonitoramentoPendente>();

            int total = monitoramentosParaAtualizarRota.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentosParaAtualizarRota[i].Monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                    monitoramentosParaAtualizarRota[i].Posicao != null &&
                    monitoramentosParaAtualizarRota[i].Monitoramento.Carga != null &&
                    monitoramentosParaAtualizarRota[i].Monitoramento.Carga.DataInicioViagem == null &&
                    monitoramentosParaAtualizarRota[i].StatusViagem != null &&
                    (
                        monitoramentosParaAtualizarRota[i].StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmViagem ||
                        monitoramentosParaAtualizarRota[i].StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta ||
                        monitoramentosParaAtualizarRota[i].StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento ||
                        monitoramentosParaAtualizarRota[i].StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoComEquipamentoParaPlanta
                    )
                )
                {
                    inicio2 = DateTime.UtcNow;
                    if (monitoramentosParaAtualizarRota[i].ClienteOrigem == null) monitoramentosParaAtualizarRota[i].ClienteOrigem = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);
                    Log("RoteirizarRotaAteOrigemMonitoramento BuscarClienteOrigemDaCargaPeloPedido", inicio2, 4);

                    if (monitoramentosParaAtualizarRota[i].ClienteOrigem != null && !EstaNaOrigem(unitOfWork, monitoramentosParaAtualizarRota[i], dadosComplexosMonitoramento))
                    {
                        inicio2 = DateTime.UtcNow;
                        if (monitoramentosParaAtualizarRota[i].CargaEntregas == null) monitoramentosParaAtualizarRota[i].CargaEntregas = dadosComplexosMonitoramento._CargaEntregasTodos.Where(e => e.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo).ToList();
                        Log("RoteirizarRotaAteOrigemMonitoramento BuscarCargaEntrega", inicio2, 4);

                        monitoramentosParaRoteirizar.Add(monitoramentosParaAtualizarRota[i]);
                        continue;
                    }
                }
            }
            Log("RoteirizarRotaAteOrigemMonitoramento BuscarDados", inicio1, 3);

            List<Task> tasks = new List<Task>();
            inicio = DateTime.UtcNow;
            total = monitoramentosParaRoteirizar.Count;
            for (int i = 0; i < total; i++)
            {
                MonitoramentoPendente monitoramentoParaRoteirizar = monitoramentosParaRoteirizar[i];
                Task task = new Task(() =>
                {
                    // Roteirizar o caminho da posição atual até a origem
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsPassagem = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                    wayPointsPassagem.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(monitoramentoParaRoteirizar.Posicao.Latitude, monitoramentoParaRoteirizar.Posicao.Longitude));

                    // Passagem pelo porto
                    if (monitoramentoParaRoteirizar.StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaColetarEquipamento &&
                        monitoramentoParaRoteirizar.CargaEntregas != null &&
                        monitoramentoParaRoteirizar.CargaEntregas.Count > 0
                    )
                    {
                        wayPointsPassagem.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(monitoramentoParaRoteirizar.CargaEntregas[0].Cliente?.Latitude ?? "0", monitoramentoParaRoteirizar.CargaEntregas[0].Cliente?.Longitude ?? "0"));
                    }

                    wayPointsPassagem.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(monitoramentoParaRoteirizar.ClienteOrigem.Latitude, monitoramentoParaRoteirizar.ClienteOrigem.Longitude));

                    monitoramentoParaRoteirizar.RespostaRotaAteOrigem = Servicos.Embarcador.Logistica.Monitoramento.ControleDistancia.ObterRespostaRoteirizacao(wayPointsPassagem, configuracaoIntegracao.ServidorRouteOSM, false, false);

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

            Log($"RoteirizarRotaAteOrigemMonitoramento", inicio, 3);
        }

        private void EstimarPrevisaoChegadaOrigem(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega servicoPrevisaoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega(unitOfWork, configuracao);
            List<CargaPendente> cargasPendentes = new List<CargaPendente>();
            DateTime inicio = DateTime.UtcNow, inicio1;
            int total = monitoramentosParaAtualizarRota.Count;

            for (int i = 0; i < total; i++)
            {
                if ((monitoramentosParaAtualizarRota[i].RespostaRotaAteOrigem?.Distancia ?? 0) > 0)
                {
                    double velocidadeKmH = (configuracao.PrevisaoEntregaVelocidadeMediaVazio > 0) ? configuracao.PrevisaoEntregaVelocidadeMediaVazio : 60;
                    DateTime dataPrevisaoChegada = servicoPrevisaoControleEntrega.CalcularDataChegadaPrevista(monitoramentosParaAtualizarRota[i].Posicao.DataVeiculo, (double)(monitoramentosParaAtualizarRota[i].RespostaRotaAteOrigem.Distancia * 1000), velocidadeKmH);
                    cargasPendentes.Add(new CargaPendente
                    {
                        Carga = monitoramentosParaAtualizarRota[i].Monitoramento.Carga,
                        DataPrevisao = dataPrevisaoChegada
                    });
                }
            }
            Log("EstimarPrevisaoCargaEntrega CalcularDataChegadaPrevista", inicio, 4);

            inicio = DateTime.UtcNow;
            total = cargasPendentes.Count;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            for (int i = 0; i < total; i++)
            {
                try
                {
                    inicio1 = DateTime.UtcNow;
                    cargasPendentes[i].Carga.DataAtualizacaoCarga = DateTime.Now;
                    cargasPendentes[i].Carga.DataPrevisaoChegadaOrigem = cargasPendentes[i].DataPrevisao;
                    repCarga.Atualizar(cargasPendentes[i].Carga);
                    Log($"Atualizar PrevisaoChegadaOrigem {monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo}", inicio1, 4);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                }
            }
            Log("EstimarPrevisaoChegadaOrigem", inicio, 3);
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

                        // Confirma que entre o início do processamento e este ponto, não houve alteração do status ou do status de viagem via aplicação web
                        if (VerificaSeStatusMonitoramentoNaoAlterou(connection, transaction, repMonitoramento, monitoramentosParaAtualizarRota[i].Monitoramento))
                        {

                            // Inicia sem alteração no status da viagem
                            int codigoNovoStatusViagem = -1;

                            // Nenhum status de viagem identificado
                            if (monitoramentosParaAtualizarRota[i].StatusViagem == null)
                            {
                                // Com configuração desmarcada, deve limpar o status
                                if (!configuracao.MonitoramentoStatusViagemQuandoFicarSemStatusManterUltimo) codigoNovoStatusViagem = 0;
                            }
                            else if (monitoramentosParaAtualizarRota[i].Monitoramento.StatusViagem == null || monitoramentosParaAtualizarRota[i].StatusViagem.Codigo != monitoramentosParaAtualizarRota[i].Monitoramento.StatusViagem.Codigo)
                            {
                                // ... há um novo status

                                DateTime? dataFinal = null;
                                if (monitoramentosParaAtualizarRota[i].StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida)
                                {
                                    dataFinal = monitoramentosParaAtualizarRota[i].Posicao.DataVeiculo;
                                }

                                double codigoClienteEntrega = IdentificarClienteDaEntregaNoAlvo(monitoramentosParaAtualizarRota[i].Posicao, monitoramentosParaAtualizarRota[i].PosicaoAlvos, monitoramentosParaAtualizarRota[i].CargaEntregas);
                                int codigoSubareaClienteEntrega = IdentificarSubareaClienteDaEntregaNoAlvo(monitoramentosParaAtualizarRota[i].Posicao, monitoramentosParaAtualizarRota[i].PosicaoAlvoSubareas, monitoramentosParaAtualizarRota[i].CargaEntregas);

                                codigoNovoStatusViagem = monitoramentosParaAtualizarRota[i].StatusViagem.Codigo;
                                inicio1 = DateTime.UtcNow;

                                Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoStatusStatusViagem monitoramentoStatus = repMonitoramentoHistoricoStatusViagem.BuscarUltimoStatusViagemDoHistoricoMonitoramentoDBComponents(connection, transaction, monitoramentosParaAtualizarRota[i].Monitoramento.Codigo);

                                if (monitoramentoStatus?.CodigoStatusViagem != codigoNovoStatusViagem)// só adiciona se for diferente ou nao existir.
                                {
                                    repMonitoramentoHistoricoStatusViagem.InserirEncerrarAnterior(
                                        monitoramentosParaAtualizarRota[i].Monitoramento.Codigo,
                                        codigoNovoStatusViagem,
                                        monitoramentosParaAtualizarRota[i].Posicao.Latitude,
                                        monitoramentosParaAtualizarRota[i].Posicao.Longitude,
                                        monitoramentosParaAtualizarRota[i].Posicao.Veiculo.Codigo,
                                        monitoramentosParaAtualizarRota[i].DataInicialStatusViagem,
                                        codigoClienteEntrega,
                                        codigoSubareaClienteEntrega,
                                        dataFinal,
                                        connection,
                                        transaction
                                    );

                                    Log($"RegistraHistorico", inicio1, 4);

                                    if (monitoramentosParaAtualizarRota[i].Monitoramento.Carga != null)
                                        repCarga.AtualizarDataAtualizacaoCarga(monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo, connection, transaction);
                                }
                            }

                            inicio1 = DateTime.UtcNow;
                            repMonitoramento.AtualizarRotaRealizada(
                                monitoramentosParaAtualizarRota[i].Monitoramento.Codigo,
                                string.Empty,
                                0,
                                monitoramentosParaAtualizarRota[i].RespostaRotaAteOrigem?.Polilinha ?? string.Empty,
                                monitoramentosParaAtualizarRota[i].RespostaRotaAteOrigem?.Distancia ?? 0,
                                string.Empty,
                                0,
                                codigoNovoStatusViagem,
                                0,
                                false,
                                connection,
                                transaction
                            );
                            Log($"InformarRotaRealizada {monitoramentosParaAtualizarRota[i].Monitoramento.Codigo}", inicio1, 4);
                        }
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

        private void FinalizarMonitoramentosConcluidos(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria)
        {
            DateTime inicio = DateTime.UtcNow, inicio1;
            int total = monitoramentosParaAtualizarRota.Count;
            // unitOfWork.Start();
            try
            {
                for (int i = 0; i < total; i++)
                {
                    if (monitoramentosParaAtualizarRota[i].Posicao != null &&
                        monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null &&
                        monitoramentosParaAtualizarRota[i].Monitoramento.Carga != null &&
                        monitoramentosParaAtualizarRota[i].StatusViagem != null &&
                        monitoramentosParaAtualizarRota[i].Monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                        monitoramentosParaAtualizarRota[i].StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida
                    )
                    {
                        string msg = "Status de viagem concluída";
                        DateTime dataFim = monitoramentosParaAtualizarRota[i].Monitoramento.Carga.DataFimViagem ?? monitoramentosParaAtualizarRota[i].Posicao.DataVeiculo;

                        inicio1 = DateTime.UtcNow;
                        Servicos.Embarcador.Monitoramento.Monitoramento.FinalizarMonitoramento(monitoramentosParaAtualizarRota[i].Monitoramento, dataFim, configuracao, auditoria, msg, unitOfWork, MotivoFinalizacaoMonitoramento.StatusViagemConcluida);
                        Log($"FinalizarMonitoramento {monitoramentosParaAtualizarRota[i].Monitoramento.Codigo}", inicio1, 4);
                    }
                }
                //unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                //unitOfWork.Rollback();
                throw;
            }
            Log("FinalizarMonitoramentosConcluidos", inicio, 3);
        }

        private void CalcularDefinirPrevisaoCargaEntrega(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, DadosComplexosMonitoramentos dadosComplexosMonitoramento)
        {
            // Calcula as previsões de entrega das cargas
            Log("Calculando previsoes entregas", 3);
            DateTime inicio = DateTime.UtcNow, inicio1;
            int total = monitoramentosParaAtualizarRota.Count, totalCalculada = 0, totalNaoCalculada = 0;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);

            List<int> idCargaEntregas = monitoramentosParaAtualizarRota.Where(obj => obj.CargaEntregas != null).SelectMany(x => x.CargaEntregas.ToList()).Select(x => x.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidosTodos = repCargaEntregaPedido.BuscarPorCargaEntregas(idCargaEntregas);

            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete> cargasRotaFrete = repCargaRotaFrete.BuscarPorCargas(monitoramentosParaAtualizarRota.Select(x => x.Monitoramento.Carga.Codigo).ToList());

            for (int i = 0; i < total; i++)
            {
                if (monitoramentosParaAtualizarRota[i].Posicao != null &&
                    monitoramentosParaAtualizarRota[i].Monitoramento != null &&
                    monitoramentosParaAtualizarRota[i].Monitoramento.Carga != null &&
                    monitoramentosParaAtualizarRota[i].Monitoramento.Carga.DataFimViagem == null
                )
                {

                    // Tempo mínimo entre os cálculos das previsões de entrega das cargas
                    Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao ultimaAtualizacao = Servicos.Embarcador.Monitoramento.UltimaAtualizacao.ObterUltimaAtualizacao(monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo, ref this.UltimasAtualizacoesPrevisaoEntregaCarga);
                    if (UltimaAtualizacao.VerificaSeJaExpirouEAtualiza(ref ultimaAtualizacao, monitoramentosParaAtualizarRota[i].Posicao.DataVeiculo, this.intervaloCalculoPrevisaoCargaEntregaMinutos))
                    {
                        totalCalculada++;
                        if (
                            monitoramentosParaAtualizarRota[i].Monitoramento.Carga.DataInicioViagem != null ||
                            EstaNaOrigem(unitOfWork, monitoramentosParaAtualizarRota[i], dadosComplexosMonitoramento)
                        )
                        {

                            // Já iniciou a viagem
                            if (monitoramentosParaAtualizarRota[i].Monitoramento.Carga.DataInicioViagem != null)
                            {
                                inicio1 = DateTime.UtcNow;
                                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete? cargaRotaFrete = cargasRotaFrete.Where(x => x.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo).FirstOrDefault();

                                CalcularPrevisoesEntregas(unitOfWork, monitoramentosParaAtualizarRota[i], configuracao, configuracaoIntegracao, configuracaoControleEntrega, cargaRotaFrete, cargaEntregaPedidosTodos);
                                Log($"CalcularPrevisoesEntregas ViagemIniciada {monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo}", inicio1, 4);

                            }
                            // ... ainda não iniciou a viagem, está na área do cliente de origem
                            else
                            {
                                // Identifica a maior data entre a entrada na origem e a data de inicio de carregamento da janela
                                DateTime? dataBaseInicial = null;
                                DateTime? dataEntradaNaOrigem = BuscaDataEntradaClienteCargaEntrega(unitOfWork, monitoramentosParaAtualizarRota[i].ClienteOrigem, monitoramentosParaAtualizarRota[i].CargaEntregas, dadosComplexosMonitoramento);

                                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento? cargaJanelaCarregamento = dadosComplexosMonitoramento._cargaJanelaCarregamentosTodos.Find(j => j.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo);
                                DateTime? dataInicioCarregamentoJanela = cargaJanelaCarregamento?.InicioCarregamento; //BuscarInicioCarregamentoJanela(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);

                                if (dataEntradaNaOrigem.HasValue && dataInicioCarregamentoJanela.HasValue)
                                {
                                    dataBaseInicial = (dataEntradaNaOrigem > dataInicioCarregamentoJanela) ? dataEntradaNaOrigem : dataInicioCarregamentoJanela;
                                }
                                else if (dataEntradaNaOrigem.HasValue)
                                {
                                    dataBaseInicial = dataEntradaNaOrigem;
                                }
                                else if (dataInicioCarregamentoJanela.HasValue)
                                {
                                    dataBaseInicial = dataInicioCarregamentoJanela;
                                }

                                if (dataBaseInicial.HasValue)
                                {

                                    // Adiciona o tempo padrão de carregamento
                                    if (cargaJanelaCarregamento != null && cargaJanelaCarregamento.CentroCarregamento?.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento > 0)
                                        dataBaseInicial = dataBaseInicial.Value.AddHours(cargaJanelaCarregamento.CentroCarregamento.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento);
                                    else
                                        dataBaseInicial = dataBaseInicial.Value.AddHours(configuracao.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento);


                                    // Data da posição é maior que a tolerância
                                    if (monitoramentosParaAtualizarRota[i].Posicao.DataVeiculo > dataBaseInicial)
                                    {
                                        // ... o veículo ainda está na origem e já se passou toda a tolerância de carregamento padrão, passa a recalcular a partir da data da posição
                                        Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete? cargaRotaFrete = cargasRotaFrete.Find(x => x.Carga.Codigo == monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo);
                                        inicio1 = DateTime.UtcNow;

                                        CalcularPrevisoesEntregas(unitOfWork, monitoramentosParaAtualizarRota[i], configuracao, configuracaoIntegracao, configuracaoControleEntrega, cargaRotaFrete, cargaEntregaPedidosTodos);
                                        Log($"CalcularPrevisoesEntregas Data {monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo}", inicio1, 4);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        totalNaoCalculada++;
                    }
                }
            }
            Log($"CalcularPrevisoesEntregas {total} ({totalCalculada},{totalNaoCalculada})", inicio, 3);

            // Atualiza as datas reprogramadas das entregas calculadas
            Log("Salvando previsoes entregas", 3);
            //unitOfWork.Start();


            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorencia = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega? confOcorrencia = repConfiguracaoOcorencia.BuscarRegrasPorEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.RecalculoPrevisao)?.FirstOrDefault();

            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repConfiguracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configIntegracaoTrizy = repConfiguracaoTrizy.BuscarPrimeiroRegistro();

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntregasCalculoPrevisao = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(EventoColetaEntrega.CalculoPrevisao);

            for (int i = 0; i < total; i++)
            {
                try
                {
                    if (monitoramentosParaAtualizarRota[i].PrevisoesCargaEntrega != null)
                    {
                        inicio1 = DateTime.UtcNow;
                        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = cargaEntregaPedidosTodos.Where(x => monitoramentosParaAtualizarRota[i].CargaEntregas.Contains(x.CargaEntrega)).ToList(); // repCargaEntregaPedido.BuscarPorCargaEntregas((from o in monitoramentosParaAtualizarRota[i].CargaEntregas select o.Codigo).ToList());

                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarPrevisaoCargaEntrega(monitoramentosParaAtualizarRota[i].PrevisoesCargaEntrega, cargaEntregaPedidos, monitoramentosParaAtualizarRota[i].Monitoramento.Carga, monitoramentosParaAtualizarRota[i].CargaEntregas, false, true, configuracao, unitOfWork, tipoServicoMultisoftware, false, OrigemSituacaoEntrega.MonitoramentoAutomaticamente, configuracaoOcorrenciaEntregasCalculoPrevisao, configuracaoControleEntrega, configIntegracaoTrizy);

                        if (confOcorrencia != null)
                        {
                            for (int j = 0; j < monitoramentosParaAtualizarRota[i].CargaEntregas.Count; j++)
                            {
                                if (monitoramentosParaAtualizarRota[i].CargaEntregas[j] == null)
                                    continue;

                                if (ExisteOcorrenciaRecalculoEntregaEmTempo(unitOfWork, monitoramentosParaAtualizarRota[i].CargaEntregas[j], confOcorrencia.TipoDeOcorrencia, confOcorrencia.TempoRecalculo))
                                    continue;

                                Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configOcorrenciaCoordenada = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas();
                                configOcorrenciaCoordenada.Latitude = (decimal)monitoramentosParaAtualizarRota[i].Posicao.Latitude;
                                configOcorrenciaCoordenada.Longitude = (decimal)monitoramentosParaAtualizarRota[i].Posicao.Longitude;
                                configOcorrenciaCoordenada.DataExecucao = DateTime.Now;
                                configOcorrenciaCoordenada.DataPosicao = monitoramentosParaAtualizarRota[i].Posicao.DataVeiculo;
                                configOcorrenciaCoordenada.DistanciaAteDestino = monitoramentosParaAtualizarRota[i].PrevisoesCargaEntrega[j].DistanciaAteDestino;
                                configOcorrenciaCoordenada.DataPrevisaoRecalculada = monitoramentosParaAtualizarRota[i].CargaEntregas[j].DataReprogramada ?? null;
                                configOcorrenciaCoordenada.TempoPercurso = monitoramentosParaAtualizarRota[i].CargaEntregas[j].DataReprogramada.HasValue ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(DateTime.Now - monitoramentosParaAtualizarRota[i].CargaEntregas[j].DataReprogramada.Value) : "";

                                Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarOcorrenciaEntregaRecalculoPrevisao(monitoramentosParaAtualizarRota[i].CargaEntregas[j], DateTime.Now, confOcorrencia.TipoDeOcorrencia, configuracao, tipoServicoMultisoftware, null, configOcorrenciaCoordenada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.MonitoramentoAutomaticamente, configuracaoControleEntrega, unitOfWork, auditoria);
                            }
                        }

                        Log($"SalvarPrevisaoCargaEntrega {monitoramentosParaAtualizarRota[i].Monitoramento.Carga.Codigo}", inicio1, 4);
                    }
                }
                catch (Exception)
                {
                    //unitOfWork.Rollback();
                    throw;
                }
            }
            //unitOfWork.CommitChanges();
            Log("SalvarPrevisaoCargaEntrega", inicio, 3);

        }

        private IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> BuscarMonitoramentosPendentesStatusViagem(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> monitoramentos = repMonitoramento.BuscarProcessarStatusViagem();

            Log(monitoramentos.Count + " monitoramentos pendentes", 1);

            return monitoramentos;
        }

        /**
         * Consulta a via de transporte do pedido
         */
        private Dominio.Entidades.Embarcador.Cargas.ViaTransporte BuscarViaTransporte(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga != null)
            {
                Repositorio.Embarcador.Cargas.ViaTransporte repViaTransporte = new Repositorio.Embarcador.Cargas.ViaTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte = repViaTransporte.BuscarViaTransportePorCarga(carga.Codigo);
                return viaTransporte;
            }
            return null;
        }

        private bool EstaNaOrigem(Repositorio.UnitOfWork unitOfWork, MonitoramentoPendente monitoramentoParaAtualizarRota, DadosComplexosMonitoramentos dadosComplexosMonitoramento)
        {
            if (monitoramentoParaAtualizarRota != null && (monitoramentoParaAtualizarRota.Posicao?.EmAlvo ?? false) == true)
            {
                if (monitoramentoParaAtualizarRota.ClienteOrigem == null) monitoramentoParaAtualizarRota.ClienteOrigem = dadosComplexosMonitoramento._clientesOrigemTodos.FirstOrDefault(p => p.Item1 == monitoramentoParaAtualizarRota.Monitoramento.Carga.Codigo)?.Item2;// Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, monitoramentoParaAtualizarRota.Monitoramento.Carga);
                if (monitoramentoParaAtualizarRota.PosicaoAlvos == null) monitoramentoParaAtualizarRota.PosicaoAlvos = dadosComplexosMonitoramento._posicoesAlvoTodos.Where(p => p.Codigo == monitoramentoParaAtualizarRota.Posicao.Codigo).ToList(); // BuscarPosicaoAlvos(unitOfWork, monitoramentoParaAtualizarRota.Posicao.Codigo);
                if (monitoramentoParaAtualizarRota.ClienteOrigem != null && monitoramentoParaAtualizarRota.PosicaoAlvos != null)
                {
                    int total = monitoramentoParaAtualizarRota.PosicaoAlvos.Count;
                    for (int i = 0; i < total; i++)
                    {
                        if (monitoramentoParaAtualizarRota.PosicaoAlvos[i].Cliente.CPF_CNPJ == monitoramentoParaAtualizarRota.ClienteOrigem.CPF_CNPJ)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
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

        private bool VerificaSeStatusMonitoramentoNaoAlterou(DbConnection connection, DbTransaction transaction, Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoStatusStatusViagem monitoramentoStatus = repMonitoramento.BuscarStatusMonitoramento(connection, transaction, monitoramento.Codigo);
            if (monitoramentoStatus != null)
            {
                return monitoramento.Status == monitoramentoStatus.Status && (monitoramento.StatusViagem?.Codigo ?? 0) == monitoramentoStatus.CodigoStatusViagem;
            }
            return false;
        }

        private double IdentificarClienteDaEntregaNoAlvo(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas)
        {
            double codigoClienteEntrega = 0;
            if (posicao.EmAlvo ?? false)
            {
                int totalAlvos = posicaoAlvos?.Count ?? 0;
                int totalEntregas = cargaEntregas?.Count ?? 0;
                for (int i = 0; i < totalAlvos; i++)
                {
                    for (int j = 0; j < totalEntregas; j++)
                    {
                        if (posicaoAlvos[i].Cliente != null && cargaEntregas[j].Cliente != null && posicaoAlvos[i].Cliente.CPF_CNPJ == cargaEntregas[j].Cliente.CPF_CNPJ)
                        {
                            return cargaEntregas[j].Cliente.CPF_CNPJ;
                        }
                    }
                }
            }
            return codigoClienteEntrega;
        }

        private int IdentificarSubareaClienteDaEntregaNoAlvo(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvosSubareas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas)
        {
            int codigoSubareaClienteEntrega = 0;
            if (posicao.EmAlvo ?? false)
            {
                int totalAlvosSubareas = posicaoAlvosSubareas?.Count ?? 0;
                int totalEntregas = cargaEntregas?.Count ?? 0;
                for (int i = 0; i < totalAlvosSubareas; i++)
                {
                    for (int j = 0; j < totalEntregas; j++)
                    {
                        if (cargaEntregas[j].Cliente != null && posicaoAlvosSubareas[i].SubareaCliente.Cliente != null && posicaoAlvosSubareas[i].SubareaCliente.Cliente.CPF_CNPJ == cargaEntregas[j].Cliente.CPF_CNPJ)
                        {
                            return posicaoAlvosSubareas[i].SubareaCliente.Codigo;
                        }
                    }
                }
            }
            return codigoSubareaClienteEntrega;
        }

        private void CalcularPrevisoesEntregas(Repositorio.UnitOfWork unitOfWork, MonitoramentoPendente monitoramentoParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidosTodos)
        {
            if (cargaRotaFrete == null) { return; }

            Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega servicoPrevisaoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega(unitOfWork, configuracao);

            if (monitoramentoParaAtualizarRota.CargaEntregas == null)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                monitoramentoParaAtualizarRota.CargaEntregas = repCargaEntrega.BuscarPorCarga(monitoramentoParaAtualizarRota.Monitoramento.Carga.Codigo);
            }

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = cargaEntregaPedidosTodos.Where(x => monitoramentoParaAtualizarRota.CargaEntregas.Contains(x.CargaEntrega)).ToList();
            monitoramentoParaAtualizarRota.PrevisoesCargaEntrega = servicoPrevisaoControleEntrega.CalcularPrevisoesEntregasComPosicao(monitoramentoParaAtualizarRota.Monitoramento.Carga, cargaRotaFrete, monitoramentoParaAtualizarRota.CargaEntregas, cargaEntregaPedidos, monitoramentoParaAtualizarRota.Posicao, configuracaoIntegracao, configuracaoControleEntrega);

        }

        protected DateTime? BuscaDataEntradaClienteCargaEntrega(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Cliente? cliente, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, DadosComplexosMonitoramentos dadosComplexosMonitoramento)
        {
            int total = entregas?.Count ?? 0;
            if (cliente != null && total > 0)
            {
                for (int i = 0; i < total; i++)
                {
                    if (entregas[i].Cliente != null && entregas[i].Cliente.CPF_CNPJ == cliente.CPF_CNPJ)
                    {
                        //Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
                        Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente? permanenciaCliente = dadosComplexosMonitoramento._ListPermanenciasClientesTodos.Find(x => x.Cliente.Codigo == cliente.Codigo && x.CargaEntrega.Codigo == entregas[i].Codigo); // repPermanenciaCliente.BuscarAbertaPorClienteECargaEntrega(cliente.Codigo, entregas[i].Codigo);
                        if (permanenciaCliente != null) return permanenciaCliente.DataInicio;
                    }
                }
            }
            return null;
        }

        private bool ExisteOcorrenciaRecalculoEntregaEmTempo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, int tempo)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega eventosGerado = repOcorrenciaColetaEntrega.BuscarPorTipoOcorrenciaCargaEntrega(tipoDeOcorrencia.Codigo, cargaEntrega.Codigo);

            if (eventosGerado != null)
            {
                int diferencaEmMinutos = (int)(dataAtual - eventosGerado.DataOcorrencia).TotalMinutes;
                if (diferencaEmMinutos <= tempo) return true;
            }

            return false;
        }

        #endregion
    }
}
