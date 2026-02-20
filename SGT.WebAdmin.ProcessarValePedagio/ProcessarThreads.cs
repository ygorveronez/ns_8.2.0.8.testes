using SGT.WebAdmin.ProcessarValePedagio.Thread;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceProcess;

namespace SGT.WebAdmin.ProcessarValePedagio
{
    public partial class ProcessarThreads : ServiceBase
    {
        private List<int> _IDsConsultarValoresPedagioPendente = new List<int>();
        private List<int> _IDsGerarIntegracoesValePedagio = new List<int>();
        private List<int> _IDsGerarIntegracoesCancelamentoValePedagio = new List<int>();
        private List<int> _IDsVerificarRetornosValePedagio = new List<int>();
        private List<int> _IDsProcessarExtratoValePedagioPendentes = new List<int>();

        private List<System.Threading.Thread> _threadsConsultarValoresPedagioPendente = new List<System.Threading.Thread>();
        private List<System.Threading.Thread> _threadsGerarIntegracoesValePedagio = new List<System.Threading.Thread>();
        private List<System.Threading.Thread> _threadsGerarIntegracoesCancelamentoValePedagio = new List<System.Threading.Thread>();
        private List<System.Threading.Thread> _threadsVerificarRetornosValePedagio = new List<System.Threading.Thread>();
        private List<System.Threading.Thread> _threadsProcessarExtratoValePedagioPendentes = new List<System.Threading.Thread>();
        private List<System.Threading.Thread> _jobs;
        private static string _arquivoLog = "Logs";
        private List<ConfiguracaoThread> _configuracoes;

        delegate List<int> ObterPendentes();
        delegate void Execute(int ID);

        public ProcessarThreads()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Servicos.Log.TratarErro("Serviço ValePedagio iniciado.");
            try
            {
                string defaultCulture = ConfigurationManager.AppSettings["DefaultCulture"];

                if (string.IsNullOrWhiteSpace(defaultCulture))
                    defaultCulture = "pt-BR";

                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(defaultCulture);

                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;

                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

                Servicos.IO.FileStorage.ConfigureApplicationFileStorage(Program.StringConexaoAdmin, Program.Host);
                ConfigurarThreads();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        public void ConfigurarThreads()
        {
            _configuracoes = new List<ConfiguracaoThread>();

            #region ConfiguraTreadCargasPendentes
            try
            {
                //var cfg = new ConfiguracaoThread(05, "ProcessarExtratoValePedagioPendentes", ConfigurationManager.GetSection("ProcessarExtratoValePedagioPendentes") as NameValueCollection);
                if (_jobs == null)
                {
                    _jobs = new List<System.Threading.Thread>();

                    // metodos 
                    #region ConsultarValoresPedagioPendente
                    try
                    {
                        var cfg = new ConfiguracaoThread(1, "ConsultarValoresPedagioPendente", ConfigurationManager.GetSection("ConsultarValoresPedagioPendente") as NameValueCollection);
                        if (cfg.Ativa)
                            _jobs.Add(new System.Threading.Thread(() => loop(cfg, _threadsConsultarValoresPedagioPendente, ObterConsultarValoresPedagioPendente, execute_ConsultarValoresPedagioPendente)));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro("Erro ao iniciar JOB ConsultarValoresPedagioPendente", _arquivoLog);
                        Servicos.Log.TratarErro(e, _arquivoLog);
                    }
                    #endregion
                    #region GerarIntegracoesValePedagio
                    try
                    {
                        var cfg = new ConfiguracaoThread(2, "GerarIntegracoesValePedagio", ConfigurationManager.GetSection("GerarIntegracoesValePedagio") as NameValueCollection);
                        if (cfg.Ativa)
                            _jobs.Add(new System.Threading.Thread(() => loop(cfg, _threadsGerarIntegracoesValePedagio, ObterPendentesGerarIntegracoesValePedagio, execute_GerarIntegracoesValePedagio)));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro("Erro ao iniciar JOB GerarIntegracoesValePedagio", _arquivoLog);
                        Servicos.Log.TratarErro(e, _arquivoLog);
                    }
                    #endregion
                    #region GerarIntegracoesCancelamentoValePedagio
                    try
                    {
                        var cfg = new ConfiguracaoThread(3, "GerarIntegracoesCancelamentoValePedagio", ConfigurationManager.GetSection("GerarIntegracoesCancelamentoValePedagio") as NameValueCollection);
                        if (cfg.Ativa)
                            _jobs.Add(new System.Threading.Thread(() => loop(cfg, _threadsGerarIntegracoesCancelamentoValePedagio, ObterPendentesGerarIntegracoesCancelamentoValePedagio, execute_GerarIntegracoesCancelamentoValePedagio)));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro("Erro ao iniciar JOB GerarIntegracoesCancelamentoValePedagio", _arquivoLog);
                        Servicos.Log.TratarErro(e, _arquivoLog);
                    }
                    #endregion
                    #region VerificarRetornosValePedagio
                    try
                    {
                        var cfg = new ConfiguracaoThread(4, "VerificarRetornosValePedagio", ConfigurationManager.GetSection("VerificarRetornosValePedagio") as NameValueCollection);
                        if (cfg.Ativa)
                            _jobs.Add(new System.Threading.Thread(() => loop(cfg, _threadsVerificarRetornosValePedagio, ObterVerificarRetornosValePedagio, execute_VerificarRetornosValePedagio)));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro("Erro ao iniciar JOB VerificarRetornosValePedagio", _arquivoLog);
                        Servicos.Log.TratarErro(e, _arquivoLog);
                    }
                    #endregion
                    #region ProcessarExtratoValePedagioPendentes
                    try
                    {
                        var cfg = new ConfiguracaoThread(1, "ProcessarExtratoValePedagioPendentes", ConfigurationManager.GetSection("ProcessarExtratoValePedagioPendentes") as NameValueCollection);
                        if (cfg.Ativa)
                            _jobs.Add(new System.Threading.Thread(() => loop(cfg, _threadsProcessarExtratoValePedagioPendentes, ObterProcessarExtratoValePedagioPendentes, execute_ProcessarExtratoValePedagioPendentes)));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro("Erro ao iniciar JOB ProcessarExtratoValePedagioPendentes", _arquivoLog);
                        Servicos.Log.TratarErro(e, _arquivoLog);
                    }
                    #endregion

                    foreach (var job in _jobs)
                        job.Start();

                }

            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, _arquivoLog);
                throw;
            }
            #endregion
        }

        private void loop(ConfiguracaoThread configuracao, List<System.Threading.Thread> threads, ObterPendentes obterPendentes, Execute execute)
        {
            while (true)
            {
                GC.Collect();
                try
                {
                    if (!(configuracao?.Ativa ?? false))
                    {
                        Servicos.Log.TratarErro($"Serviço {configuracao.Nome} desabilitado, aguardando 10 minutos para nova execução.", _arquivoLog);
                        System.Threading.Thread.Sleep(600000);
                        continue;
                    }

                    int quantidadeThreadsDisponiveis = 0;
                    int quantidadeThreadsAtivas = threads.Count;

                    for (int i = quantidadeThreadsAtivas - 1; i >= 0; i--)
                    {
                        if (threads[i].IsAlive)
                            continue;
                        threads.RemoveAt(i);
                    }
                    quantidadeThreadsDisponiveis = configuracao.NumeroTheadsParalelas - threads.Count;
                    if (quantidadeThreadsDisponiveis <= 0)
                    {
                        Servicos.Log.TratarErro($"Já existem {threads.Count} threads ativas. Aguardando 10 segundos para nova verificação.", _arquivoLog);
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }
                    List<int> IDs = obterPendentes();
                    if (IDs.Count <= 0)
                    {
                        Servicos.Log.TratarErro($"Não existem pendencias para o metodo {configuracao.Nome}. Aguardando 10 segundos para nova verificação. {threads.Count} de {configuracao.NumeroTheadsParalelas} threads em execução.", _arquivoLog);
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }

                    for (int i = 0; i < quantidadeThreadsDisponiveis && i < IDs.Count; i++)
                    {
                        int id = IDs[i];
                        System.Threading.Thread thread = new System.Threading.Thread(() => execute(id));
                        thread.Start();
                        threads.Add(thread);
                    }
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, _arquivoLog);
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }


        private void execute_ConsultarValoresPedagioPendente(int id)
        {
            try
            {
                lock (_IDsConsultarValoresPedagioPendente)
                {
                    if (!_IDsConsultarValoresPedagioPendente.Any(o => o == id))
                        _IDsConsultarValoresPedagioPendente.Add(id);
                    else
                        return;
                    Servicos.Log.TratarErro($"Iniciou thread para geração de ConsultarValoresPedagioPendente {id}.", _arquivoLog);
                }

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);

                    Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unitOfWork);
                    servicoValePedagio.ConsultarValoresPedagioPendenteAsync(Program.TipoServicoMultisoftware, new List<int> { id }).GetAwaiter().GetResult();
                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, _arquivoLog);
            }
            finally
            {
                lock (_IDsConsultarValoresPedagioPendente)
                {
                    if (_IDsConsultarValoresPedagioPendente.Any(o => o == id))
                        _IDsConsultarValoresPedagioPendente.Remove(id);

                    Servicos.Log.TratarErro($"Finalizou thread para geração de ConsultarValoresPedagioPendente {id}.", _arquivoLog);
                }
            }
        }
        private void execute_GerarIntegracoesValePedagio(int id)
        {
            try
            {
                lock (_IDsGerarIntegracoesValePedagio)
                {
                    if (!_IDsGerarIntegracoesValePedagio.Any(o => o == id))
                        _IDsGerarIntegracoesValePedagio.Add(id);
                    else
                        return;
                    Servicos.Log.TratarErro($"Iniciou thread para geração de GerarIntegracoesValePedagio {id}.", _arquivoLog);
                }

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unitOfWork);
                    servicoValePedagio.GerarIntegracoesValePedagioAsync(Program.TipoServicoMultisoftware, new List<int> { id }).GetAwaiter().GetResult();
                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, _arquivoLog);
            }
            finally
            {
                lock (_IDsGerarIntegracoesValePedagio)
                {
                    if (_IDsGerarIntegracoesValePedagio.Any(o => o == id))
                        _IDsGerarIntegracoesValePedagio.Remove(id);

                    Servicos.Log.TratarErro($"Finalizou thread para geração de GerarIntegracoesValePedagio {id}.", _arquivoLog);
                }
            }
        }
        private void execute_GerarIntegracoesCancelamentoValePedagio(int id)
        {
            try
            {
                lock (_IDsGerarIntegracoesCancelamentoValePedagio)
                {
                    if (!_IDsGerarIntegracoesCancelamentoValePedagio.Any(o => o == id))
                        _IDsGerarIntegracoesCancelamentoValePedagio.Add(id);
                    else
                        return;
                    Servicos.Log.TratarErro($"Iniciou thread para geração de GerarIntegracoesCancelamentoValePedagio {id}.", _arquivoLog);
                }

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unitOfWork);
                    servicoValePedagio.GerarIntegracoesCancelamentoValePedagioAsync(Program.TipoServicoMultisoftware, new List<int> { id }).GetAwaiter().GetResult();
                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, _arquivoLog);
            }
            finally
            {
                lock (_IDsGerarIntegracoesCancelamentoValePedagio)
                {
                    if (_IDsGerarIntegracoesCancelamentoValePedagio.Any(o => o == id))
                        _IDsGerarIntegracoesCancelamentoValePedagio.Remove(id);

                    Servicos.Log.TratarErro($"Finalizou thread para geração de GerarIntegracoesCancelamentoValePedagio {id}.", _arquivoLog);
                }
            }
        }
        private void execute_VerificarRetornosValePedagio(int id)
        {
            try
            {
                lock (_IDsVerificarRetornosValePedagio)
                {
                    if (!_IDsVerificarRetornosValePedagio.Any(o => o == id))
                        _IDsVerificarRetornosValePedagio.Add(id);
                    else
                        return;
                    Servicos.Log.TratarErro($"Iniciou thread para geração de VerificarRetornosValePedagio {id}.", _arquivoLog);
                }

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unitOfWork);
                    servicoValePedagio.VerificarRetornosValePedagioAsync(Program.TipoServicoMultisoftware, new List<int>() { id }).GetAwaiter().GetResult();
                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, _arquivoLog);
            }
            finally
            {
                lock (_IDsVerificarRetornosValePedagio)
                {
                    if (_IDsVerificarRetornosValePedagio.Any(o => o == id))
                        _IDsVerificarRetornosValePedagio.Remove(id);

                    Servicos.Log.TratarErro($"Finalizou thread para geração de VerificarRetornosValePedagio {id}.", _arquivoLog);
                }
            }
        }
        private void execute_ProcessarExtratoValePedagioPendentes(int id)
        {
            try
            {
                lock (_IDsProcessarExtratoValePedagioPendentes)
                {
                    if (!_IDsProcessarExtratoValePedagioPendentes.Any(o => o == id))
                        _IDsProcessarExtratoValePedagioPendentes.Add(id);
                    else
                        return;
                    Servicos.Log.TratarErro($"Iniciou thread para geração de ProcessarExtratoValePedagioPendentes {id}.", _arquivoLog);
                }

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Servicos.Embarcador.Integracao.SemParar.ValePedagio servicoValePedagioSemParar = new Servicos.Embarcador.Integracao.SemParar.ValePedagio();
                    servicoValePedagioSemParar.ProcessarExtratoValePedagioPendentes(unitOfWork, Program.TipoServicoMultisoftware, new List<int>() { id });
                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, _arquivoLog);
            }
            finally
            {
                lock (_IDsProcessarExtratoValePedagioPendentes)
                {
                    if (_IDsProcessarExtratoValePedagioPendentes.Any(o => o == id))
                        _IDsProcessarExtratoValePedagioPendentes.Remove(id);

                    Servicos.Log.TratarErro($"Finalizou thread para geração de ProcessarExtratoValePedagioPendentes {id}.", _arquivoLog);
                }
            }
        }

        private List<int> ObterConsultarValoresPedagioPendente()
        {
            List<int> codigosSemThread = new List<int>();
            
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
                List<int> ids = repCargaConsultaValePedagio.ConsultaIntegracaoAgIntegracao(50, 3, 5);
                lock (_IDsConsultarValoresPedagioPendente)
                {
                    foreach (var id in ids)
                        if (!_IDsConsultarValoresPedagioPendente.Any(o => o == id))
                            codigosSemThread.Add(id);
                }
            }
            
            return codigosSemThread;
        }
        private List<int> ObterPendentesGerarIntegracoesValePedagio()
        {
            List<int> codigosSemThread = new List<int>();
            
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                List<int> IDsPendentes = repCargaValePedagio.BuscarCargarAgIntegracaoValePedagio(100, 2, 5);

                lock (_IDsGerarIntegracoesValePedagio)
                {
                    foreach (var id in IDsPendentes)
                        if (!_IDsGerarIntegracoesValePedagio.Any(o => o == id))
                            codigosSemThread.Add(id);
                }
            }
            
            return codigosSemThread;
        }
        private List<int> ObterPendentesGerarIntegracoesCancelamentoValePedagio()
        {
            List<int> codigosSemThread = new List<int>();
            
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                List<int> IDsPendentes = repCargaValePedagio.BuscarPorCargaEmCancelamento().ToList();
                lock (_IDsGerarIntegracoesCancelamentoValePedagio)
                {
                    foreach (var id in IDsPendentes)
                        if (!_IDsGerarIntegracoesCancelamentoValePedagio.Any(o => o == id))
                            codigosSemThread.Add(id);
                }
            }

            return codigosSemThread;
        }
        private List<int> ObterVerificarRetornosValePedagio()
        {
            List<int> codigosSemThread = new List<int>();
            
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                List<int> ids = repositorioCargaValePedagio.BuscarValePedagioAgRetorno(100).ToList();
                lock (_IDsVerificarRetornosValePedagio)
                {
                    foreach (var id in ids)
                        if (!_IDsVerificarRetornosValePedagio.Any(o => o == id))
                            codigosSemThread.Add(id);
                }
            }
            
            return codigosSemThread;
        }
        private List<int> ObterProcessarExtratoValePedagioPendentes()
        {
            List<int> codigosSemThread = new List<int>();

            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar);
                if (integracao == null)
                    return new List<int>() { };

                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                List<int> ids = repositorioCargaIntegracaoValePedagio.BuscarValePedagioPorTipoIntegracao(integracao.Codigo, 100).Select(x => x.Codigo).ToList();

                lock (_IDsProcessarExtratoValePedagioPendentes)
                {
                    foreach (var id in ids)
                        if (!_IDsProcessarExtratoValePedagioPendentes.Any(o => o == id))
                            codigosSemThread.Add(id);
                }
            }

            return codigosSemThread;
        }


        private ConfiguracaoThread ObterConfiguracaoThread(string nome)
        {
            return _configuracoes.Where(x => x.Nome == nome).FirstOrDefault();
        }
        protected override void OnStop()
        {
            if (_jobs != null)
            {
                _jobs.ForEach(thread => thread.Abort());
                _jobs = null;
            }
            GC.Collect();
            Servicos.Log.TratarErro("Serviço parado.");
        }
    }
}
