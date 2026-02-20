using System;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class ServicoEnvioVencimentoCertificados
    {
        private static ServicoEnvioVencimentoCertificados Instance;
        private static Task Task;

        public static ServicoEnvioVencimentoCertificados GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoEnvioVencimentoCertificados();

            return Instance;
        }

        public void IniciarThread(string stringConexao)
        {
            if (Task == null)
            {
                Task = new Task(() =>
                {
                    while (true)
                    {
                        string horaConfigurada = System.Configuration.ConfigurationManager.AppSettings["HoraEnvioVencimentoCertificado"];
                        int.TryParse(horaConfigurada, out int hora);

                        DateTime dataProximaExecucao = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hora, 0, 0);

                        if ((dataProximaExecucao - DateTime.Now).TotalMilliseconds < 0)
                            dataProximaExecucao = dataProximaExecucao.AddDays(1);

                        Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao);

                        try
                        {
                            if (DateTime.Now.Hour == hora)
                                Servicos.Empresa.EnviarCertificados(unidadeDeTrabalho);                               
                            
                            System.Threading.Thread.Sleep((dataProximaExecucao - DateTime.Now));
                        }
                        catch (TaskCanceledException abort)
                        {
                            Servicos.Log.TratarErro(string.Concat("Task de envio de vencimento dos certificados cancelada: ", abort.ToString()), "EnvioVencimentoCertificados");
                            break;
                        }
                        catch (System.Threading.ThreadAbortException abortThread)
                        {
                            Servicos.Log.TratarErro(string.Concat("Thread de vencimento dos certificados cancelada: ", abortThread), "EnvioVencimentoCertificados");
                            break;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex, "EnvioVencimentoCertificados");

                            System.Threading.Thread.Sleep((dataProximaExecucao - DateTime.Now));
                        }
                        finally
                        {
                            unidadeDeTrabalho.Dispose();
                        }
                    }
                });

                Task.Start();
            }
        }
    }
}