using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class ServicoEmailMDFesNaoEncerrados
    {
        private static ServicoEmailMDFesNaoEncerrados Instance;
        private static Task Task;

        public static ServicoEmailMDFesNaoEncerrados GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoEmailMDFesNaoEncerrados();

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
                        DateTime dataProximaExecucao = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 07, 00, 0);

                        if ((dataProximaExecucao - DateTime.Now).TotalMilliseconds < 0)
                            dataProximaExecucao = dataProximaExecucao.AddDays(1);

                        try
                        {
                            using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                            {
                                if (DateTime.Now.Hour == 07 && DateTime.Now.Minute >= 00 && DateTime.Now.Minute <= 15)
                                {
                                    Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                                    Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                                    string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                                    string urlSistema = System.Configuration.ConfigurationManager.AppSettings["WebServiceConsultaCTe"];

                                    List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes5dias = repMDFe.BuscarPendentesEncerramento(Dominio.Enumeradores.StatusMDFe.Autorizado, DateTime.Now.AddDays(-5), DateTime.Now.AddDays(-7));
                                    for (var i = 0; i < mdfes5dias.Count; i++)
                                        svcMDFe.EnviarEmailEncerramentoTransportador(mdfes5dias[i].Codigo, ambiente, urlSistema, unidadeDeTrabalho);
                                }
                            }

                            System.Threading.Thread.Sleep((dataProximaExecucao - DateTime.Now));
                        }
                        catch (TaskCanceledException abort)
                        {
                            Servicos.Log.TratarErro(string.Concat("Task de email encerramento MDFe: ", abort.ToString()), "EmailMDFesNaoEncerrados");
                            break;
                        }
                        catch (System.Threading.ThreadAbortException abortThread)
                        {
                            Servicos.Log.TratarErro(string.Concat("Thread de email encerramento MDFe: ", abortThread), "EmailMDFesNaoEncerrados");
                            break;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex, "EmailMDFesNaoEncerrados");

                            System.Threading.Thread.Sleep((dataProximaExecucao - DateTime.Now));
                        }
                    }
                });

                Task.Start();
            }
        }
    }
}