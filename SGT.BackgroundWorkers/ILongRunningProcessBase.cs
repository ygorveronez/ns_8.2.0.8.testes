using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public interface ILongRunningProcessBase
    {
        string GetProccessName();

        bool IsActive();

        void SetParameters(
            int codigoEmpresa,
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso,
            string stringConexao,
            string stringConexaoAdmin,
            int tempoAguardarProximaExecucao,
            bool? iniciarProcessoAtivo,
            bool utilizarLockNasThreads);

        Task StartAsync();

        Task StopAsync();
    }
}
