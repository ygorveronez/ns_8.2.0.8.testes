using Servicos.Embarcador.Monitoramento;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;

namespace SGT.Monitoramento.Thread
{
    public class MonitorarThreads : AbstractThreadProcessamento
    {

        #region Atributos privados

        private static MonitorarThreads Instante;
        private static int tempoSleep = 10;
        private bool enable = true;

        private static System.Threading.Thread MonitorarThread;

        #endregion

        #region Métodos públicos

        // Singleton
        public static MonitorarThreads GetInstance(string stringConexao)
        {
            if (Instante == null)
                Instante = new MonitorarThreads(stringConexao);
            return Instante;
        }

        public System.Threading.Thread Iniciar(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (enable)
                MonitorarThread = base.IniciarThread(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware, null, tempoSleep);

            return MonitorarThread;

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
            ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;
            Log(currentThreads.Count + " threads no processo");
            foreach (ProcessThread thread in currentThreads)
            {
                Log(thread.GetType().Name + " " + thread.Id + " " + thread.ThreadState + " " + ((thread.ThreadState.ToString() == "Wait") ? thread.WaitReason.ToString() : ""));
            }
        }

        override protected void Parar()
        {
            if (MonitorarThread != null)
            {
                MonitorarThread.Abort();
                MonitorarThread = null;
            }
        }

        #endregion

        #region Construtor privado

        private MonitorarThreads(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = string.IsNullOrWhiteSpace(stringConexao) ? null : new Repositorio.UnitOfWork(stringConexao);
            try
            {
                tempoSleep = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarThreads().TempoSleepThread;
                enable = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarThreads().Ativo;
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

    }
}