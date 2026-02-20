using AdminMultisoftware.Dominio.Entidades.Pessoas;
using Microsoft.Extensions.DependencyInjection;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public abstract class LongRunningProcessBase<TProcess> : ILongRunningProcessBase
        where TProcess : LongRunningProcessBase<TProcess>, new()
    {
        private static IServiceProvider _serviceProvider;
        private static readonly Lazy<TProcess> _instance = new Lazy<TProcess>(() => new TProcess());
        private readonly object _proccessLock = new object();

        private Task _executionTask;
        private CancellationTokenSource _cancellationTokenSource;

        protected string _nomeProcesso;
        protected bool _processoIniciado;
        protected int _codigoEmpresa;
        protected string _stringConexao;
        protected string _stringConexaoAdmin;
        protected int _tempoAguardarProximaExecucao;
        protected bool _ambienteHomologacao;
        protected bool _utilizarLockNasThreads;
        protected bool _iniciarProcessoAtivo;
        protected AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        protected AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteUrlAcesso;
        protected Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        protected AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        protected string _webServiceConsultaCTe;
        protected string _webServiceOracle;
        protected string _urlAcesso;
        protected bool _urlHomologacao;
        protected string _tenantIdForMetrics;

        #region Construtores

        public static TProcess Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Construtores

        public void SetParameters(int codigoEmpresa, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, string stringConexao, string stringConexaoAdmin,
                                  int tempoAguardarProximaExecucao, bool? iniciarProcessoAtivo, bool utilizarLockNasThreads)
        {
            _nomeProcesso = typeof(TProcess).Name;
            _codigoEmpresa = codigoEmpresa;
            _stringConexao = stringConexao;
            _stringConexaoAdmin = stringConexaoAdmin;
            _tempoAguardarProximaExecucao = tempoAguardarProximaExecucao;
            _ambienteHomologacao = (clienteURLAcesso.URLHomologacao && clienteURLAcesso.Cliente.ClienteConfiguracaoHomologacao != null);
            _utilizarLockNasThreads = utilizarLockNasThreads;
            _tipoServicoMultisoftware = clienteURLAcesso.TipoServicoMultisoftware;
            _iniciarProcessoAtivo = iniciarProcessoAtivo ?? CanRun();
            _clienteUrlAcesso = clienteURLAcesso;
            _clienteMultisoftware = clienteURLAcesso.Cliente;
            _webServiceConsultaCTe = clienteURLAcesso.WebServiceConsultaCTe;
            _webServiceOracle = clienteURLAcesso.WebServiceOracle;
            _urlAcesso = clienteURLAcesso.URLAcesso;
            _urlHomologacao = clienteURLAcesso.URLHomologacao;
            _tenantIdForMetrics = GetTenantIdForMetrics(clienteURLAcesso);

            _auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema
            };

        }

        private string GetTenantIdForMetrics(ClienteURLAcesso clienteURLAcesso)
        {
            try
            {
                return JobMetricsHelper.SanitizeTenantId(_clienteUrlAcesso?.Cliente?.NomeFantasia);
            }
            catch
            {
                return "unknown";
            }
        }

        public Task StartAsync()
        {
            lock (_proccessLock)
            {
                if (_executionTask != null && !_executionTask.IsCompleted)
                    return Task.CompletedTask;

                _cancellationTokenSource = new CancellationTokenSource();
                _executionTask = ExecuteAsync(_cancellationTokenSource.Token);
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            lock (_proccessLock)
            {
                if (_executionTask == null)
                    return;

                Servicos.Log.TratarErro($"Thread {_nomeProcesso} StopAsync.", "ControleThread");

                _cancellationTokenSource?.Cancel();
            }

            try
            {
                await _executionTask;
            }
            catch (OperationCanceledException ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] OperationCanceledException durante execu��o de processo {_nomeProcesso} long running: {ex.ToString()}", "CatchNoAction");
            }
            finally
            {
                lock (_proccessLock)
                {
                    _executionTask = null;
                    _cancellationTokenSource?.Dispose();
                    _cancellationTokenSource = null;
                }
            }
        }


        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Servicos.Log.TratarErro($"[START] Thread {_nomeProcesso} iniciada.", "ControleThread");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider?.CreateScope())
                    {
                        bool lockAdquirido = false;
                        IDistributedLock distributedLock = null;
                        string environment = _ambienteHomologacao ? "homolog" : "prod";
                        string key = $"{_codigoEmpresa}-{_nomeProcesso}-{environment}";

                        if (_utilizarLockNasThreads)
                        {
                            distributedLock = scope.ServiceProvider.GetRequiredService<IDistributedLock>();
                            lockAdquirido = await distributedLock.LockAsync(key, TimeSpan.FromMinutes(2), stoppingToken);
                        }
                        else
                        {
                            lockAdquirido = true;
                        }

                        if (lockAdquirido)
                        {
                            // --- INICIO DA MEDICAO ---
                            var stopwatch = new Stopwatch();
                            var process = Process.GetCurrentProcess();
                            var initialCpuTime = process.TotalProcessorTime;
                            bool succedeed = false;
                            bool skipped = true;

                            try
                            {
                                using (var adminSqlConnection = new SqlConnection(_stringConexaoAdmin))
                                using (var defaultSqlConnection = new SqlConnection(_stringConexao))
                                using (var unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_stringConexaoAdmin, adminSqlConnection))
                                using (var unitOfWork = new Repositorio.UnitOfWork(_stringConexao, defaultSqlConnection))
                                {
                                    var controleThread = ObterControleThread(unitOfWork);

                                    if (!controleThread.Ativo && !_processoIniciado)
                                        break;

                                    _tempoAguardarProximaExecucao = controleThread.Ativo
                                        ? controleThread.Tempo * 1000
                                        : 300000;

                                    _processoIniciado = true;

                                    if (controleThread.Ativo)
                                    {
                                        skipped = false;
                                        stopwatch.Start();
                                        await ExecuteInternalAsync(unitOfWork, unitOfWorkAdmin, stoppingToken);
                                        succedeed = true;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.GravarDebug($"Erro ao executar fila {_nomeProcesso}: {ex}", "ControleThread");
                            }
                            finally
                            {
                                if (_utilizarLockNasThreads && distributedLock != null)
                                {
                                    await distributedLock.UnlockAsync(key, stoppingToken);
                                }

                                // Log metrics only if job was not skipped.
                                if (!skipped)
                                {
                                    stopwatch.Stop();
                                    var finalCpuTime = process.TotalProcessorTime;
                                    var finalMemoryMb = process.WorkingSet64 / 1024 / 1024;
                                    var cpuTimeMs = (finalCpuTime - initialCpuTime).TotalMilliseconds;

                                    // Chama nosso helper estatico para enviar todas as metricas
                                    JobMetricsHelper.RecordJobExecutionMetrics(
                                        _nomeProcesso,
                                        _tenantIdForMetrics,
                                        stopwatch.Elapsed,
                                        cpuTimeMs,
                                        finalMemoryMb,
                                        succedeed
                                    );
                                }
                            }

                            await Task.Delay(_tempoAguardarProximaExecucao, stoppingToken);
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    Servicos.Log.TratarErro($"Thread {_nomeProcesso} cancelada.", "ControleThread");
                    break;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"Erro ao executar processo {_nomeProcesso}: {ex}", "ControleThread");
                    await Task.Delay(_tempoAguardarProximaExecucao, stoppingToken);
                }
            }

            Servicos.Log.TratarErro($"[END] Thread {_nomeProcesso} finalizada.", "ControleThread");
        }

        protected abstract Task ExecuteInternalAsync(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin,
            CancellationToken cancellationToken);

        protected virtual Dominio.Entidades.ControleThread ObterControleThread(Repositorio.UnitOfWork unitOfWork)
        {
            var repositorioControleThread = new Repositorio.ControleThread(unitOfWork);
            var controleThread = repositorioControleThread.BuscarPorThread(_nomeProcesso);

            if (controleThread == null)
            {
                controleThread = new Dominio.Entidades.ControleThread
                {
                    Ativo = _iniciarProcessoAtivo,
                    DataCadastro = DateTime.Now,
                    Tempo = _tempoAguardarProximaExecucao / 1000,
                    Thread = _nomeProcesso
                };
                repositorioControleThread.Inserir(controleThread);
            }

            return controleThread;
        }

        public void ExecuteHangfire()
        {
            Servicos.Log.TratarErro($"_stringConexao: {_stringConexao} | _stringConexaoAdmin: {_stringConexaoAdmin}", "ControleThread");

            using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_stringConexaoAdmin))
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Dominio.Entidades.ControleThread controleThread = ObterControleThread(unitOfWork);

                if (!controleThread.Ativo)
                    return;

                try
                {
                    ExecuteAsync(default).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"Erro ao executar processo hangfire {_nomeProcesso}: {ex}", "ControleThread");
                }
            }
        }

        public string GetProccessName()
        {
            return _nomeProcesso;
        }

        public bool IsActive()
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Dominio.Entidades.ControleThread controleThread = ObterControleThread(unitOfWork);
                    return controleThread.Ativo;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro ao consultar a thread {_nomeProcesso}: {ex}", "ControleThread");
                return false;
            }
        }

        public virtual bool CanRun()
        {
            return true;
        }
    }
}

