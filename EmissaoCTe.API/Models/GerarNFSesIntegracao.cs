using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class GerarNFSesIntegracao
    {
        private int Tempo = 5000; //5 Segundos

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaGerarCargaCTesIntegrados;
        private static GerarNFSesIntegracao Instance;

        public static GerarNFSesIntegracao GetInstance()
        {
            if (Instance == null)
                Instance = new GerarNFSesIntegracao();

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
                            VerificarNFSesPendentes(unidadeDeTrabalho);
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
                        Servicos.Log.TratarErro(string.Concat("Task de geração NFes Integrados cancelada: ", abort.ToString()));
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread de geração NFes Integrados cancelada: ", abortThread));
                        break;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        System.Threading.Thread.Sleep(Tempo);
                    }
                }
            });

            if (ListaTasks.TryAdd(idEmpresa, task))
                task.Start();
            else
                Servicos.Log.TratarErro("Não foi possível adicionar a task de geração NFEs Integrados à fila.");
        }


        private void VerificarNFSesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.IntegracaoNFSe repIntegracaoNFSe = new Repositorio.IntegracaoNFSe(unitOfWork);

            List<int> listaIntegracaoNFSe = repIntegracaoNFSe.BuscarIntegracaoNFSeAguardandoGeracao(5);

            Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);

            for (var i = 0; i < listaIntegracaoNFSe.Count; i++)
            {
                Dominio.Entidades.IntegracaoNFSe integracaoNFse = repIntegracaoNFSe.BuscarPorCodigo(listaIntegracaoNFSe[i]);

                if (integracaoNFse != null)
                {
                    if (integracaoNFse.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Pendente)
                    {
                        try
                        {
                            Dominio.ObjetosDeValor.CTe.CTeNFSe documento = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.CTe.CTeNFSe>(integracaoNFse.Arquivo);

                            if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                                unitOfWork.Start(System.Data.IsolationLevel.Serializable);
                            else
                                unitOfWork.Start();

                            Dominio.Entidades.NFSe nfse = svcNFSe.GerarNFSePorObjetoObjetoCTe(documento, unitOfWork, Dominio.Enumeradores.StatusNFSe.Enviado, integracaoNFse.NFSe.Codigo);
                            svcNFSe.ObterRPS(ref nfse, unitOfWork);

                            unitOfWork.CommitChanges();

                            if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Enviado)
                            {
                                if (!svcNFSe.Emitir(nfse, unitOfWork))
                                    throw new Exception("NFSe" + nfse.Numero.ToString() + " da empresa " + nfse.Empresa.CNPJ + " foi salva, porem, ocorreu uma falha ao enviar-la a prefeitura.");

                                if (!svcNFSe.AdicionarNFSeNaFilaDeConsulta(nfse, unitOfWork))
                                    throw new Exception("NFSe " + nfse.Numero.ToString() + " da empresa " + nfse.Empresa.CNPJ + " foi salva, porem, nao foi possivel adiciona-la na fila de consulta.");

                                integracaoNFse.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                                repIntegracaoNFSe.Atualizar(integracaoNFse);
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Problema ao gerar NFSe codigo " + integracaoNFse.NFSe.Codigo + ": " + ex, "GerarNFSeIntegracao");

                            NotificarFalhaPorEmail(integracaoNFse.NFSe.Codigo, ex, unitOfWork);

                            unitOfWork.Rollback();

                            throw;
                        }
                    }
                    else
                    {
                        integracaoNFse.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                        repIntegracaoNFSe.Atualizar(integracaoNFse);
                    }
                }
                else
                    Servicos.Log.TratarErro("Integração codigo " + listaIntegracaoNFSe[i].ToString() + " não localzada.", "GerarNFSeIntegracao");

                unitOfWork.FlushAndClear();
            }
        }

        private void NotificarFalhaPorEmail(int codigoNFSe, Exception erro, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Email svcEmail = new Servicos.Email();

                string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                string assunto = ambiente + " - Problemas na geração de NFSe!";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<p>Atenção, problemas na emissão no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                sb.Append("Erro ao gerar NFSe código: " + codigoNFSe.ToString() + " ").Append(erro).Append("</p><br /> <br />");

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