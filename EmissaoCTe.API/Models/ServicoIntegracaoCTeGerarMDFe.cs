using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace EmissaoCTe.API
{
    public class ServicoIntegracaoCTeGerarMDFe
    {
        private int Tempo = 120000; //2 MINUTOS

        private ConcurrentDictionary<int, Task> ListaTasks;
        private static ServicoIntegracaoCTeGerarMDFe Instance;

        public static ServicoIntegracaoCTeGerarMDFe GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoIntegracaoCTeGerarMDFe();

            return Instance;
        }


        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

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
                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao))
                        {
                            VerificarIntegracoesPendentes(unidadeDeTrabalho);
                            unidadeDeTrabalho.Dispose();
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
                        Servicos.Log.TratarErro(string.Concat("Task de gerar MDFe Integracao CTe cancelada: ", abort.ToString()));
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task de gerar MDFe Integracao CTe cancelada: ", abortThread));
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
                Servicos.Log.TratarErro("Não foi possível adicionar a Task de gerar MDFe Integracao CTe à fila.");
        }


        private void VerificarIntegracoesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork);
            List<Dominio.Entidades.IntegracaoCTe> listaIntegracoes = repIntegracaoCTe.BuscarIntegracoesFinalizadora("A");

            Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);

            for (var i = 0; i < listaIntegracoes.Count(); i++)
            {
                try
                {
                    List<Dominio.Entidades.IntegracaoCTe> integracoesCarga = repIntegracaoCTe.BuscarIntegracoesPendentes(listaIntegracoes[i].NumeroDaCarga, listaIntegracoes[i].NumeroDaUnidade, "");
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = integracoesCarga.Where(o => o.CTe.Status == "A").Select(o => o.CTe).ToList(); 

                    if (integracoesCarga.Count() == listaCTes.Count())
                    {                       
                        unitOfWork.Start();

                        List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaMDFes = svcMDFe.GerarMDFePorCTesDestinosDiferentes(listaIntegracoes[i].CTe.Empresa, listaCTes, string.Empty, unitOfWork);

                        unitOfWork.CommitChanges();

                        if (listaMDFes != null && listaMDFes.Count > 0)
                        {
                            for (var j = 0; j < listaMDFes.Count(); j++)
                            {
                                if (svcMDFe.Emitir(listaMDFes[j], unitOfWork))
                                    if (!svcMDFe.AdicionarMDFeNaFilaDeConsulta(listaMDFes[j],unitOfWork))
                                        Servicos.Log.TratarErro("O MDF-e nº " + listaMDFes[j].Numero.ToString() + " da empresa " + listaMDFes[j].Empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ");
                            }

                            //Atualizar integrações como Finalizadas
                            for (var k = 0; k < integracoesCarga.Count(); k++)
                            {
                                integracoesCarga[k].FinalizouCarga = true;
                                repIntegracaoCTe.Atualizar(integracoesCarga[k]);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    //Verificar para fazer aviso, e para salvar na tabela o erro

                    Servicos.Log.TratarErro("Problema ao finalizar carga: " + listaIntegracoes[i].NumeroDaCarga + ": " + ex.Message);

                    unitOfWork.Rollback();
                }
            }
        }

    }
}