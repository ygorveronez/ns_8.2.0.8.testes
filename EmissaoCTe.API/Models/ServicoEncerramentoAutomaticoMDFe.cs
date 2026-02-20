using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class ServicoEncerramentoAutomaticoMDFe
    {
        private static ServicoEncerramentoAutomaticoMDFe Instance;
        private static Task Task;

        public static ServicoEncerramentoAutomaticoMDFe GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoEncerramentoAutomaticoMDFe();

            return Instance;
        }

        public void IniciarThread(string stringConexao)
        {
            string servicoEncerramentoAutomaticoMDFe = System.Configuration.ConfigurationManager.AppSettings["ServicoEncerramentoAutomaticoMDFe"];

            if (!string.IsNullOrWhiteSpace(servicoEncerramentoAutomaticoMDFe))
            {
                if (Task == null)
                {
                    Task = new Task(() =>
                    {
                        while (true)
                        {
                            DateTime dataProximaExecucao = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 40, 0);

                            if ((dataProximaExecucao - DateTime.Now).TotalMilliseconds < 0)
                                dataProximaExecucao = dataProximaExecucao.AddDays(1);

                            //Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao);
                            try
                            {
                                using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                                {
                                    if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 39 && DateTime.Now.Minute <= 55)
                                    {
                                        Servicos.Log.TratarErro("Iniciando encerramento automatico MDFe", "EncerramentoMDFeAutomatico");

                                        Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                                        Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                                        Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                                        List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.BuscarPorStatusEDataAutorizacao(Dominio.Enumeradores.StatusMDFe.Autorizado, DateTime.Now.AddDays(-25), 0, Dominio.Enumeradores.TipoAmbiente.Producao);

                                        Servicos.Log.TratarErro(mdfes.Count().ToString() + " pendentes de encerramento.", "EncerramentoMDFeAutomatico");
                                        foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfes)
                                        {
                                            if (mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.EncerramentoMDFeAutomatico != Dominio.Enumeradores.EncerramentoMDFeAutomatico.Nenhum)
                                            {
                                                if (mdfe.Empresa.DataFinalCertificado >= DateTime.Today)
                                                {
                                                    Servicos.Log.TratarErro("Encerrando MDFe " + mdfe.Chave, "EncerramentoMDFeAutomatico");

                                                    Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamentoMDFe = repMunicipioDescarregamento.BuscarPrimeiroPorMDFe(mdfe.Codigo);

                                                    mdfe.MunicipioEncerramento = (municipioDescarregamentoMDFe != null && municipioDescarregamentoMDFe.Municipio != null) ? municipioDescarregamentoMDFe.Municipio : mdfe.Empresa.Localidade;

                                                    if (mdfe.MunicipioEncerramento != null)
                                                    {
                                                        repMDFe.Atualizar(mdfe);

                                                        TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);

                                                        DateTime dataEncerramento = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);

                                                        if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).EncerrarMdfe(mdfe.Codigo, mdfe.Empresa.Codigo, dataEncerramento, unidadeDeTrabalho, dataEncerramento))
                                                        {
                                                            if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                                                                FilaConsultaCTe.GetInstance().QueueItem(5, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, stringConexao);
                                                            svcMDFe.SalvarLogEncerramentoMDFe(mdfe.Chave, mdfe.Protocolo, dataEncerramento, mdfe.Empresa, mdfe.Empresa.Localidade, "Encerramento automatico MDFe emitido com mais de 30 dias", unidadeDeTrabalho);
                                                        }
                                                    }
                                                }
                                                else
                                                    Servicos.Log.TratarErro("Encerrando MDFe " + mdfe.Chave + " Certificado digital vencido.", "EncerramentoMDFeAutomatico");
                                            }
                                        }

                                        //Comentado pois tem muitos MDFes pendentes de 5 dias e sistema fica travando durante o processo
                                        //List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes5dias = repMDFe.BuscarPorStatusEDataAutorizacao(Dominio.Enumeradores.StatusMDFe.Autorizado, DateTime.Now.AddDays(-5), 2);
                                        //foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfes5dias)
                                        //{
                                        //    mdfe.MunicipioEncerramento = repMunicipioDescarregamento.BuscarPrimeiroPorMDFe(mdfe.Codigo).Municipio;

                                        //    if (mdfe.MunicipioEncerramento != null)
                                        //    {
                                        //        repMDFe.Atualizar(mdfe);

                                        //        TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);

                                        //        DateTime dataEncerramento = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);

                                        //        if (svcMDFe.Encerrar(mdfe.Codigo, mdfe.Empresa.Codigo, dataEncerramento, unidadeDeTrabalho, dataEncerramento))
                                        //        {
                                        //            FilaConsultaCTe.GetInstance().QueueItem(mdfe.Empresa.Codigo, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, stringConexao);
                                        //        }
                                        //    }
                                        //}
                                    }
                                }

                                System.Threading.Thread.Sleep((dataProximaExecucao - DateTime.Now));
                            }
                            catch (TaskCanceledException abort)
                            {
                                Servicos.Log.TratarErro(string.Concat("Task de encerramento automatico de MDF-e cancelada: ", abort.ToString()), "EncerramentoMDFeAutomatico");
                                break;
                            }
                            catch (System.Threading.ThreadAbortException abortThread)
                            {
                                Servicos.Log.TratarErro(string.Concat("Thread de encerramento automatico de MDF-e cancelada: ", abortThread), "EncerramentoMDFeAutomatico");
                                break;
                            }
                            catch (Exception ex)
                            {
                                //unidadeDeTrabalho.Dispose();

                                Servicos.Log.TratarErro(ex, "EncerramentoMDFeAutomatico");

                                System.Threading.Thread.Sleep((dataProximaExecucao - DateTime.Now));
                            }
                            finally
                            {
                                //unidadeDeTrabalho.Dispose();
                            }
                        }
                    });

                    Task.Start();
                }
            }
        }
    }
}