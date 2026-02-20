using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration;

namespace EmissaoCTe.API
{
    public class GerarCTesIntegracao
    {
        private int Tempo = 1000; //1 segundo

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaGerarCargaCTesIntegrados;
        private static GerarCTesIntegracao Instance;

        public static GerarCTesIntegracao GetInstance()
        {
            if (Instance == null)
                Instance = new GerarCTesIntegracao();

            return Instance;
        }


        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaGerarCargaCTesIntegrados == null)
                ListaGerarCargaCTesIntegrados = new ConcurrentQueue<int>();

            if (!ListaTasks.ContainsKey(idEmpresa))
            {
                this.IniciarThread(idEmpresa, stringConexao);
            }
        }

        private void IniciarThread(int idEmpresa, string stringConexao)
        {
            var filaConsulta = new ConcurrentQueue<int>();

            filaConsulta.Enqueue(idEmpresa);

            Task task = new Task(() =>
            {
#if DEBUG
                System.Threading.Thread.Sleep(6666);
                Tempo = 5000;
#endif

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            VerificarCTesPendentes(unidadeDeTrabalho);
                        }

                        GC.Collect();

                        System.Threading.Thread.Sleep(Tempo);

                        if (!filaConsulta.TryDequeue(out idEmpresa))
                        {
                            Servicos.Log.TratarErro("Task parou a execução");
                            break;
                        }

                    }
                    catch (TaskCanceledException abort)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task de geração CTes Integrados cancelada: ", abort.ToString()));
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread de geração CTes Integrados cancelada: ", abortThread));
                        break;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            });

            if (ListaTasks.TryAdd(idEmpresa, task))
                task.Start();
            else
                Servicos.Log.TratarErro("Não foi possível adicionar a task de geração CTes Integrados à fila.");
        }


        private void VerificarCTesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork);
            List<int> listaIntegracaoCTe = repIntegracaoCTe.BuscarIntegracaoCTesPendentes(10);
            Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

            for (var i = 0; i < listaIntegracaoCTe.Count; i++)
            {
                Dominio.Entidades.IntegracaoCTe integracaoCTe = repIntegracaoCTe.BuscarPorCodigo(listaIntegracaoCTe[i]);
                integracaoCTe.Tentativas++;
                repIntegracaoCTe.Atualizar(integracaoCTe);

                if (integracaoCTe != null)
                {
                    if (integracaoCTe.CTe.Status == "P")
                    {
                        try
                        {
                            Dominio.ObjetosDeValor.CTe.CTe documentoCTe = null;
                            Dominio.ObjetosDeValor.CTe.CTeNFSe documentoCTeNFSe = null;
                            try
                            {
                                documentoCTe = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.CTe.CTe>(integracaoCTe.Arquivo);
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro("Problema ao converter objeto CTe codigo " + integracaoCTe.CTe.Codigo + ": " + ex, "GerarCTeIntegracao");

                                documentoCTeNFSe = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.CTe.CTeNFSe>(integracaoCTe.Arquivo);
                            }

                            if (documentoCTe == null && documentoCTeNFSe != null)
                                documentoCTe = servicoCTe.ConverteObjetoCTeNFSe(documentoCTeNFSe);

                            if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                                unitOfWork.Start(System.Data.IsolationLevel.Serializable);
                            else
                                unitOfWork.Start();

                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoCTe.GerarCTePorObjeto(documentoCTe, integracaoCTe.CTe.Codigo, unitOfWork, "1", 0, "E", null, 0, null);

                            unitOfWork.CommitChanges();

                            if (cte.Status == "E")
                            {
                                if (!servicoCTe.Emitir(ref cte, unitOfWork))
                                    throw new Exception("O CT-e " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porem, ocorreu uma falha ao enviar-lo ao Sefaz.");

                                if (!servicoCTe.AdicionarCTeNaFilaDeConsulta(cte, unitOfWork))
                                    throw new Exception("O CT-e " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-lo na fila de consulta.");

                                integracaoCTe.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                                repIntegracaoCTe.Atualizar(integracaoCTe);
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Problema ao gerar CTe codigo " + integracaoCTe.CTe.Codigo + ": " + ex, "GerarCTeIntegracao");

                            NotificarFalhaPorEmail(integracaoCTe.CTe.Codigo, ex, unitOfWork);

                            unitOfWork.Rollback();

                            throw;
                        }
                    }
                    else
                    {
                        integracaoCTe.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                        repIntegracaoCTe.Atualizar(integracaoCTe);
                    }
                }
                else
                    Servicos.Log.TratarErro("Integração codigo " + listaIntegracaoCTe[i].ToString() + " não localzada.", "GerarCTeIntegracao");

                unitOfWork.FlushAndClear();
            }
        }

        private void NotificarFalhaPorEmail(int codigoCTe, Exception erro, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Email svcEmail = new Servicos.Email();

                string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                string assunto = ambiente + " - Problemas na geração de CTe!";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<p>Atenção, problemas na emissão no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                sb.Append("Erro ao gerar CTe código: " + codigoCTe.ToString() + " ").Append(erro).Append("</p><br /> <br />");

                System.Text.StringBuilder ss = new System.Text.StringBuilder();
                ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte1@multisoftware.com.br", 0, unitOfWork, 0, true, null, false);
#else
                    svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte1@multisoftware.com.br", 0, unitOfWork, 0, true, null, false);
#endif
            }
            catch (Exception exptEmail)
            {
                Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
            }
        }
    }
}