using Hangfire.Storage;
using Hangfire;
using SGT.BackgroundWorkers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGT.Hangfire.Threads
{
    public class JobRunner<TProccess> where TProccess : LongRunningProcessBase<TProccess>, new()
    {

        private readonly IStorageConnection _storageConnection;
        private string _key;
        public JobRunner()
        {
            _storageConnection = JobStorage.Current.GetConnection();
        }

        public void Run(int CodigoCliente, int tempoExecucaoPadrao)
        {
            string stringConexaoCliente = string.Empty;
            var jobName = typeof(TProccess).Name;
            var key = $"job:{jobName}:{CodigoCliente}";

            try
            {
                var adminDatabaseConnectionString = Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(new AdminMultisoftware.Repositorio.UnitOfWork(adminDatabaseConnectionString));
                var clienteHangfire = repClienteURLAcesso.BuscarPorCodigo(CodigoCliente);
                stringConexaoCliente = GerarStringConexao(clienteHangfire.Cliente.ClienteConfiguracao);
                LongRunningProcessFactory.Instance.AddProcessAsync(LongRunningProcessBase<TProccess>.Instance, clienteHangfire.Codigo, clienteHangfire, stringConexaoCliente, adminDatabaseConnectionString, false, tempoExecucaoPadrao, null, AdminMultisoftware.Dominio.Enumeradores.TipoExecucao.Hangfire).GetAwaiter().GetResult();
                HangfireExtensions.RemoverAgendamentoDoJob(key);
            }
            catch (Exception ex)
            {
                HangfireExtensions.RemoverAgendamentoDoJob(key);

                Console.WriteLine(ex.Message);

            }
            finally
            {
                if (HangfireExtensions.JobNaoEstaAgendado(key))
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexaoCliente, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        Repositorio.ControleThread repositorioControleThread = new Repositorio.ControleThread(unitOfWork);
                        Dominio.Entidades.ControleThread controleThread = repositorioControleThread.BuscarPorThread(jobName);

                        if (controleThread != null)
                        {
                            var jobId = BackgroundJob.Schedule<JobRunner<TProccess>>(jr => jr.Run(CodigoCliente, tempoExecucaoPadrao), TimeSpan.FromSeconds(controleThread.Tempo * 1000));

                            HangfireExtensions.AgendarJob(key);
                        }
                    }
                }
            }
        }
        private string GerarStringConexao(AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteConfiguracao configuracao)
        {
            if (configuracao.LoginPorAD)
                return $"Data Source={configuracao.DBServidor};database={configuracao.DBBase};Integrated Security=SSPI;persist security info=True;Max Pool Size=1000;";
            else
                return $"Data Source={configuracao.DBServidor};Initial Catalog={configuracao.DBBase};User Id={configuracao.DBUsuario};Password={configuracao.DBSenha};Max Pool Size=1000;";
        }
    }
}
