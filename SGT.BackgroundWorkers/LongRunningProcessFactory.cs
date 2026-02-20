using AdminMultisoftware.Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class LongRunningProcessFactory
    {
        #region Atributos


        private static readonly Lazy<LongRunningProcessFactory> _instance = new Lazy<LongRunningProcessFactory>(() => new LongRunningProcessFactory());
        private readonly object _proccessFactoryLock = new object();
        private readonly List<ILongRunningProcessBase> _processes = new List<ILongRunningProcessBase>();

        #endregion Atributos

        #region Construtores 

        public static LongRunningProcessFactory Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private LongRunningProcessFactory() { }

        #endregion Construtores

        #region Métodos Públicos

        public async Task AddProcessAsync<TProcess>(LongRunningProcessBase<TProcess> process, int codigoEmpresa, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, string stringConexao, string stringConexaoAdmin, bool utilizarLockNasThreads, int tempoExecucaoPadrao = 60000, bool? ativo = null, TipoExecucao tipoExecucao = TipoExecucao.Thread) where TProcess : LongRunningProcessBase<TProcess>, new()
        {
            bool iniciarProcesso = false;

            lock (_proccessFactoryLock)
            {
                process.SetParameters(codigoEmpresa, clienteURLAcesso, stringConexao, stringConexaoAdmin, tempoExecucaoPadrao, ativo, utilizarLockNasThreads);

                if (tipoExecucao == TipoExecucao.Hangfire)
                {
                    process.ExecuteHangfire();
                }
                else
                {
                    bool? useBackgroundThreads = Environment.GetEnvironmentVariable("useBackgroundThreads")?.ToBool();
                    if (useBackgroundThreads.HasValue && !useBackgroundThreads.Value)
                        return;

                    bool threadsIniciadas = Environment.GetEnvironmentVariable("threadsIniciadas")?.ToBool() ?? false;

                    if (threadsIniciadas && (_processes.Count == 0))
                        return;

                    if (_processes.Exists(o => o.GetType() == process.GetType()))
                        return;

                    if (!threadsIniciadas)
                        Environment.SetEnvironmentVariable("threadsIniciadas", "true");


                    _processes.Add(process);
                    iniciarProcesso = true;

                }
            }

            if (iniciarProcesso)
            {
                await process.StartAsync();
            }
        }

        public bool IsProccessActive(string name)
        {
            lock (_proccessFactoryLock)
            {
                ILongRunningProcessBase runningProccess = _processes.FirstOrDefault(o => o.GetProccessName() == name);

                if (runningProccess == null)
                    return false;

                return runningProccess.IsActive();
            }
        }

        public async Task RemoveProccessAsync(string name)
        {
            lock (_proccessFactoryLock)
            {
                List<ILongRunningProcessBase> runningProccesses = _processes.Where(o => o.GetProccessName() == name).ToList();

                if (runningProccesses.Count == 0)
                    return;

                for (int i = 0; i < runningProccesses.Count; i++)
                {
                    ILongRunningProcessBase runningProccess = runningProccesses[i];

                    _processes.Remove(runningProccess);

                    runningProccess.StopAsync().GetAwaiter().GetResult();
                }
            }
        }

        #endregion Métodos Públicos
    }
}
