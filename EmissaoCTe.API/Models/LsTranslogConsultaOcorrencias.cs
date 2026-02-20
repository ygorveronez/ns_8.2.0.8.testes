using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace EmissaoCTe.API
{
    public class LsTranslogConsultaOcorrencias
    {
        private int Tempo = 1200000; //20 Minutos

        private static LsTranslogConsultaOcorrencias Instance;
        private static Task Task;

        public static LsTranslogConsultaOcorrencias GetInstance()
        {
            if (Instance == null)
                Instance = new LsTranslogConsultaOcorrencias();

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
                        try
                        {
                            using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao))
                            {
                                VerificarIntegracoesPendentes(unidadeDeTrabalho);
                            }

                            GC.Collect();

                            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["LsTranslogIntervaloConsulta"], out int intervalo);

                            if (intervalo == 0)
                                intervalo = 17;

#if DEBUG
                            intervalo = 1;
#endif

                            System.Threading.Thread.Sleep(intervalo * 60000);
                        }
                        catch (TaskCanceledException abort)
                        {
                            Servicos.Log.TratarErro(string.Concat("Task de finalização consulta LSTrasnglog cancelada: ", abort.ToString()));
                            break;
                        }
                        catch (System.Threading.ThreadAbortException abortThread)
                        {
                            Servicos.Log.TratarErro(string.Concat("Task de finalização consulta LSTrasnglog cancelada: ", abortThread));
                            break;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            System.Threading.Thread.Sleep(Tempo);
                        }
                    }

                });


                Task.Start();
            }
        }


        private void VerificarIntegracoesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.IntegracaoLsTranslog repIntegracao = new Repositorio.IntegracaoLsTranslog(unitOfWork);

            try
            {
                List<int> listaIntegracoes = repIntegracao.BuscarIntegracoesPendentesConsulta();

                Servicos.LsTranslog svcLsTranslog = new Servicos.LsTranslog(unitOfWork);

                for (var i = 0; i < listaIntegracoes.Count(); i++)
                {
                    try
                    {
                        svcLsTranslog.ConsultarDocumento(listaIntegracoes[i], unitOfWork);

                        unitOfWork.FlushAndClear();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("VerificarIntegracoesPendentes Consulta: Codigo " + listaIntegracoes[i] + ": " + ex.Message, "LsTranslog");
                    }
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("VerificarIntegracoesPendentes: "+ ex.Message, "LsTranslog");
            }
        }

    }
}