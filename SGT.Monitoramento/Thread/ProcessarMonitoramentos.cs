using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Configuration;
using System.Collections.Specialized;
using System.Data.Common;
using Servicos.Embarcador.Monitoramento;

namespace SGT.Monitoramento.Thread
{

    public class MonitoramentoPendente
    {
        public DateTime DataInicial;
        public DateTime DataFim;
        public Dominio.Entidades.Embarcador.Logistica.Monitoramento Monitoramento;
        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> MonitoramentoVeiculos;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao RotaRealizadaCompilada;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao RespostaRotaRealizada;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao RespostaRotaAteOrigem;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao RespostaRotaAteDestino;
        public Dominio.Entidades.Embarcador.Logistica.Posicao Posicao;
        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> PosicaoAlvos;
        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> PosicaoAlvoSubareas;
        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> PosicoesVeiculo;
        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem StatusViagem;
        public DateTime DataInicialStatusViagem;
        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> PontosDePassagens;
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> CargaEntregas;
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> PrevisoesCargaEntrega;
        public Dominio.Entidades.Cliente ClienteOrigem;
        public decimal Percentual;
        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> HistoricosStatusViagem;
    }

    public class CargaPendente
    {
        public Dominio.Entidades.Embarcador.Cargas.Carga Carga;
        public DateTime DataPrevisao;
    }

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
        //private DateTime dataUltimoAvisoFilaArquivos;
        //private int limiteArquivosProcessarAlerta;

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

        public System.Threading.Thread Iniciar(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (enable)
                ProcessarMonitoramentosThread = base.IniciarThread(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware, arquivoNivelLog, tempoSleep);

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
            // MonitorarAlertaQuantidadeArquivosTrocaAlvoProcessar(stringConexao, unitOfWork);
            BuscarProcessarMonitoramentosPendentes(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware);

        }

        override protected void Parar()
        {
            if (ProcessarMonitoramentosThread != null)
            {
                ProcessarMonitoramentosThread.Abort();
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
                throw e;
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

            // Buscar monitoramentos pendentes de atualização da rota realizada
            //List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentos = new List<Dominio.Entidades.Embarcador.Logistica.Monitoramento>();
            //Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            //List<int> codigos = new List<int>();
            //codigos.Add(228738);

            //List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentoteste = repMonitoramento.BuscarPorCodigos(codigos);
            //monitoramentos.AddRange(monitoramentoteste);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> monitoramentos = BuscarMonitoramentosPendentes(unitOfWork);

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
                    throw ex;
                }

                // Atualiza as rotas realizadas dos monitoramentos e marcá-los como "Processado"
                try
                {
                    ProcessarMonitoramentosPendentes(unitOfWork, monitoramentos, tipoServicoMultisoftware, clienteMultisoftware);
                    Log(monitoramentos.Count + " monitoramentos processados com sucesso");
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    throw ex;
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

            // Identifica a última posição do monitoramento
            IdentificaUltimaPosicao(unitOfWork, monitoramentosParaAtualizarRota);

            // Gera a rota realizada (polilinha e distância) do monitoramento com as posições do veículo atual
            RoteirizarRotaRealizadaMonitoramento(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoIntegracao);

            // Gera a rota esperada da posição atual até o destino (polilinha e distância) do monitoramento
            RoteirizarRotaAteDestinoMonitoramento(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoIntegracao);

            // Cálclo do percentual de viagem, garantindo que não retrocededa
            CalcularPercentualViagem(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoIntegracao);

            // Compila as rotas realizadas de todos pos veículos do monitoramento
            CompilarRotaRealizadaDoMonitoramentoVeiculos(unitOfWork, monitoramentosParaAtualizarRota);

            // Salva a rota realizada do veículo atual do monitoramento
            SalvarRotaRealizadaMonitoramentoVeiculo(unitOfWork, monitoramentosParaAtualizarRota);

            // Atualiza os dados do monitoramento
            InformarRotaRealizada(unitOfWork, monitoramentosParaAtualizarRota, configuracao);

            // Atualizar distancia das entregas pela roteirizacao
            CalcularDistanciaAteDestinoCargaEntrega(unitOfWork, monitoramentosParaAtualizarRota, configuracao, configuracaoIntegracao);


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
                MonitoramentoPendente monitoramentoPendente = new MonitoramentoPendente();
                monitoramentoPendente.Monitoramento = listaMonitoramentos.Where(x => x.Codigo == monitoramentos[i].CodigoMonitoramento).FirstOrDefault();
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
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            DateTime inicio = DateTime.UtcNow, inicio1;
            int total = monitoramentosParaAtualizarRota.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo != null)
                {
                    inicio1 = DateTime.UtcNow;
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = repPosicao.BuscarWaypointsPorMonitoramentoVeiculo(monitoramentosParaAtualizarRota[i].Monitoramento.Codigo, monitoramentosParaAtualizarRota[i].Monitoramento.Veiculo.Codigo, monitoramentosParaAtualizarRota[i].DataInicial);
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
                    throw e;
                }
            }
            Log("ProcessarAlteracoesCargaDependendoStatusViagem", inicio, 3);
        }

        public void RoteirizarRotaAteDestinoMonitoramento(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
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
                    if (monitoramentosParaAtualizarRota[i].CargaEntregas == null) monitoramentosParaAtualizarRota[i].CargaEntregas = BuscarCargaEntrega(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);
                    Log("RoteirizarRotaAteDestinoMonitoramento BuscarCargaEntrega", inicio2, 4);

                    inicio2 = DateTime.UtcNow;
                    if (monitoramentosParaAtualizarRota[i].PontosDePassagens == null) monitoramentosParaAtualizarRota[i].PontosDePassagens = BuscarPontosDePassagem(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento);
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

            List<long> UltimasPosicoes = monitoramentosParaAtualizarRota.Select(x => x.PosicoesVeiculo.Last().ID).ToList();
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

        private void CalcularPercentualViagem(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
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
                        monitoramentosParaAtualizarRota[i].HistoricosStatusViagem = repMonitoramentoHistoricoStatusViagem.BuscarPorMonitoramento(monitoramentosParaAtualizarRota[i].Monitoramento);
                    }

                    if (monitoramentosParaAtualizarRota[i].CargaEntregas == null)
                        monitoramentosParaAtualizarRota[i].CargaEntregas = BuscarCargaEntrega(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);

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
                try
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
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                }
            }
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
                        repMonitoramento.AtualizarProcessarRota(monitoramentosParaAtualizarRota[i].Monitoramento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado, connection, transaction);
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

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> BuscarMonitoramentosPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> monitoramentos = repMonitoramento.BuscarProcessarComLimite(limiteRegistros);

            Log(monitoramentos.Count + " monitoramentos pendentes", 1);

            return monitoramentos;
        }


        private void CalcularDistanciaAteDestinoCargaEntrega(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendente> monitoramentosParaAtualizarRota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
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
                    if (monitoramentosParaAtualizarRota[i].CargaEntregas == null) monitoramentosParaAtualizarRota[i].CargaEntregas = BuscarCargaEntrega(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento.Carga);
                    Log("CalcularDistanciaAteDestinoCargaEntrega BuscarCargaEntrega", inicio1, 4);

                    inicio1 = DateTime.UtcNow;
                    if (monitoramentosParaAtualizarRota[i].PontosDePassagens == null) monitoramentosParaAtualizarRota[i].PontosDePassagens = BuscarPontosDePassagem(unitOfWork, monitoramentosParaAtualizarRota[i].Monitoramento);
                    Log("CalcularDistanciaAteDestinoCargaEntrega BuscarPontosDePassagem", inicio1, 4);

                    inicio1 = DateTime.UtcNow;
                    int totalEntregas = monitoramentosParaAtualizarRota[i].CargaEntregas?.Count ?? 0;
                    for (int j = 0; j < totalEntregas; j++)
                    {
                        monitoramentosParaAtualizarRota[i].CargaEntregas[j].DistanciaAteDestino = Servicos.Embarcador.Monitoramento.Rota.CalcularDistanciaAteEntrega(monitoramentosParaAtualizarRota[i].CargaEntregas[j], monitoramentosParaAtualizarRota[i].PontosDePassagens, pontosNaRotaAteDestino);
                        repCargaEntrega.Atualizar(monitoramentosParaAtualizarRota[i].CargaEntregas[j]);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(monitoramentosParaAtualizarRota[i].CargaEntregas[j], repCargaEntrega, unitOfWork, configControleEntrega);
                    }
                    Log($"CalcularDistanciaAteDestinoCargaEntrega CalcularDistanciaAteEntrega {totalEntregas}", inicio1, 4);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                }
            }
        }


        private void AlterarMonitoramentosProcessar(Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessar> monitoramentos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao processar)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            int total = monitoramentos.Count;
            for (int i = 0; i < total; i++)
            {
                DbConnection connection = unitOfWork.GetConnection();
                DbTransaction transaction = connection.BeginTransaction();
                try
                {
                    repMonitoramento.AtualizarProcessarRota(monitoramentos[i].CodigoMonitoramento, processar, connection, transaction);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Servicos.Log.TratarErro(e);
                }
                finally { transaction.Dispose(); }
            }
        }

        /**
         * Busca os pontos de passagem e carregar para o cache
         */
        private List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> BuscarPontosDePassagem(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem = new List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            if (monitoramento != null && monitoramento.Carga != null)
            {
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(monitoramento.Carga.Codigo);
                if (cargaRotaFrete != null)
                {
                    Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem reCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> todosPontosDePassagem = reCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);
                    int total = todosPontosDePassagem.Count;
                    for (int i = 0; i < total; i++)
                    {
                        if (todosPontosDePassagem[i].Cliente != null && todosPontosDePassagem[i].Cliente.PossuiGeolocalizacao())
                        {
                            pontosDePassagem.Add(todosPontosDePassagem[i]);
                        }
                    }
                }
            }
            return pontosDePassagem;
        }

        /**
         * Consulta todos os horários de carregamento de todos os pontos de passagems das cargas dos monitoramentos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.HorarioCarregamento> BuscarHorariosCarregamento(Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.HorarioCarregamento> horariosCarregamento = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.HorarioCarregamento>();

            int total = pontosDePassagem.Count;
            if (total > 0)
            {

                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoCarregamento repPeriodosCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);

                // Percorre todas as cargas e seus pontos de passagem do cache
                for (int i = 0; i < total; i++)
                {
                    // Como trata-se de carregamnte, apenas a coleta interessa
                    if (pontosDePassagem[i].TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta)
                    {
                        // Consulta os horários de carregamento
                        string cnpjFilial = pontosDePassagem[i].Cliente.Codigo.ToString().PadLeft(14, '0');
                        Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCNPJ(cnpjFilial);
                        if (filial != null)
                        {
                            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repCentroCarregamento.BuscarPorFilial(filial.Codigo);
                            if (centroCarregamento != null)
                            {
                                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = repPeriodosCarregamento.BuscarPorCentroCarregamento(centroCarregamento.Codigo);
                                int contk = periodosCarregamento.Count;
                                for (int k = 0; k < contk; k++)
                                {
                                    horariosCarregamento.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.HorarioCarregamento
                                    {
                                        Cliente = pontosDePassagem[i].Cliente,
                                        Dia = periodosCarregamento[k].Dia,
                                        HoraInicio = periodosCarregamento[k].HoraInicio,
                                        HoraTermino = periodosCarregamento[k].HoraTermino
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return horariosCarregamento;
        }

        /**
         * Consulta todos os horários de descarregamento de todos os pontos de passagems das cargas dos monitoramentos
         */
        private List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarCargaEntrega(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga != null)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(carga.Codigo);
                return cargaEntregas;
            }
            return null;
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

        /**
         * Consulta a janela de carregamento da carga
         */
        private Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarJanelaCarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga != null)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);
                return cargaJanelaCarregamento;
            }
            return null;
        }

        /**
         * Consulta a janela de carregamento da carga
         */
        private DateTime? BuscarInicioCarregamentoJanela(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = BuscarJanelaCarregamento(unitOfWork, carga);
            if (cargaJanelaCarregamento != null)
            {
                return cargaJanelaCarregamento.InicioCarregamento;
            }
            return null;
        }

        /**
         * Consulta as janela de decarregamento da carga
         */
        private List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarJanelasDescarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga != null)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento = repCargaJanelaDescarregamento.BuscarTodasPorCarga(carga.Codigo);
                return cargaJanelasDescarregamento;
            }
            return null;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> BuscarPosicaoAlvos(Repositorio.UnitOfWork unitOfWork, long codigoPosicao)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos = new List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo>();
            if (codigoPosicao > 0)
            {
                Repositorio.Embarcador.Logistica.PosicaoAlvo repPosicaoAlvo = new Repositorio.Embarcador.Logistica.PosicaoAlvo(unitOfWork);
                posicaoAlvos = repPosicaoAlvo.BuscarPorPosicao(codigoPosicao);
            }
            return posicaoAlvos;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> BuscarPosicaoAlvosSubareas(Repositorio.UnitOfWork unitOfWork, long codigoPosicao)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> posicaoAlvosSubareas = new List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea>();
            if (codigoPosicao > 0)
            {
                Repositorio.Embarcador.Logistica.PosicaoAlvoSubarea repPosicaoAlvoSubarea = new Repositorio.Embarcador.Logistica.PosicaoAlvoSubarea(unitOfWork);
                posicaoAlvosSubareas = repPosicaoAlvoSubarea.BuscarPorPosicao(codigoPosicao);
            }
            return posicaoAlvosSubareas;
        }

        private bool EstaNaOrigem(Repositorio.UnitOfWork unitOfWork, MonitoramentoPendente monitoramentoParaAtualizarRota)
        {
            if (monitoramentoParaAtualizarRota != null && (monitoramentoParaAtualizarRota.Posicao?.EmAlvo ?? false) == true)
            {
                if (monitoramentoParaAtualizarRota.ClienteOrigem == null) monitoramentoParaAtualizarRota.ClienteOrigem = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, monitoramentoParaAtualizarRota.Monitoramento.Carga);
                if (monitoramentoParaAtualizarRota.PosicaoAlvos == null) monitoramentoParaAtualizarRota.PosicaoAlvos = BuscarPosicaoAlvos(unitOfWork, monitoramentoParaAtualizarRota.Posicao.Codigo);
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
