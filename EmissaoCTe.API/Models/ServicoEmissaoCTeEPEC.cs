using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class ServicoEmissaoCTeEPEC
    {
        private static ServicoEmissaoCTeEPEC Instance;
        private static Task Task;

        public static ServicoEmissaoCTeEPEC GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoEmissaoCTeEPEC();

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
                        string horaConfigurada = System.Configuration.ConfigurationManager.AppSettings["HoraEnvioCTeEPEC"];
                        int.TryParse(horaConfigurada, out int hora);

                        DateTime dataProximaExecucao = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hora, 0, 0);

                        if ((dataProximaExecucao - DateTime.Now).TotalMilliseconds < 0)
                            dataProximaExecucao = dataProximaExecucao.AddDays(1);

                        Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao);

                        try
                        {
                            if (DateTime.Now.Hour == hora)
                                VerificarCTesEPEC(unidadeDeTrabalho, stringConexao);

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

        private void VerificarCTesEPEC(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = null;
            Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

            string configAdicionarCTesFilaConsulta = System.Configuration.ConfigurationManager.AppSettings["AdicionarCTesFilaConsulta"];
            if (configAdicionarCTesFilaConsulta == null || configAdicionarCTesFilaConsulta == "")
                configAdicionarCTesFilaConsulta = "SIM";

            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TentativasEnvioCTeEPEC"], out int tentativasReenvio);

            listaCTes = repCTe.BuscarCTesEPEC(tentativasReenvio > 0 ? tentativasReenvio : 5);
            
            for (var i = 0; i < listaCTes.Count; i++)
            {
                Servicos.Log.TratarErro("CT-e " + listaCTes[i].Chave + " EPEC Reenviado", "ReenvioCTeEPEC");

                if (servicoCTe.Emitir(listaCTes[i].Codigo, 0, unitOfWork))
                {
                    if (configAdicionarCTesFilaConsulta.Equals("SIM"))
                        servicoCTe.AdicionarCTeNaFilaDeConsulta(listaCTes[i], unitOfWork);
                    servicoCTe.AtualizarIntegracaoRetornoCTe(listaCTes[i], unitOfWork);
                }
            }


        }

    }
}