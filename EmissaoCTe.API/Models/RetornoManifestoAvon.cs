using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
namespace EmissaoCTe.API
{
    public class RetornoManifestoAvon
    {
        private int Tempo = 300000;
        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaConsultaEmissoes;
        private static RetornoManifestoAvon Instance;

        public static RetornoManifestoAvon GetInstance()
        {
            if (Instance == null)
                Instance = new RetornoManifestoAvon();

            return Instance;
        }

        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaConsultaEmissoes == null)
                ListaConsultaEmissoes = new ConcurrentQueue<int>();

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
#endif

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            EnviarDocumentosManifestoAvon(idEmpresa, unitOfWork);
                            SetarMinutasNaoFinalizadas(unitOfWork);
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
                        Servicos.Log.TratarErro(string.Concat("Task de consulta de objetos cancelada: ", abort.ToString()));
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread de consulta de objetos cancelada: ", abortThread));
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
                Servicos.Log.TratarErro("Não foi possível adicionar a task à fila.");
        }

        private void EnviarDocumentosManifestoAvon(int codEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            string quantidadeCTeRetornoAvon = System.Configuration.ConfigurationManager.AppSettings["QuantidadeCTeRetornoAvon"];
            if (string.IsNullOrWhiteSpace(quantidadeCTeRetornoAvon))
                quantidadeCTeRetornoAvon = "50";

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codEmpresa);
            Servicos.Avon serAvon = new Servicos.Avon(unitOfWork);
            Repositorio.DocumentoManifestoAvon repDocumentoManifestoAvon = new Repositorio.DocumentoManifestoAvon(unitOfWork);
            //Servicos.Log.TratarErro("Carregando CTes para retorno Avon.");
            List<Dominio.Entidades.DocumentoManifestoAvon> documentos = repDocumentoManifestoAvon.ConsultarDocumentosParaRetorno(0, int.Parse(quantidadeCTeRetornoAvon));//faz o envio de 10 por vez
            if(documentos.Count > 0)
            {
                Tempo = 200;
                serAvon.EnviarRetornoAvon(documentos, empresa);
            }
            else
            {
                Tempo = 5000;
            }
        }

        private void SetarMinutasNaoFinalizadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ManifestoAvon repManifestoAvon = new Repositorio.ManifestoAvon(unitOfWork);
            Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(unitOfWork);

            List<Dominio.Entidades.ManifestoAvon> manifestos = repManifestoAvon.BuscarPorStatusValidarEnvioRetorno();
            foreach (Dominio.Entidades.ManifestoAvon manifesto in manifestos)
            {
                if (repDocumento.ContarPorManifestoEStatusDiff(manifesto.Codigo, Dominio.Enumeradores.StatusDocumentoManifestoAvon.Finalizado) <= 0) {
                    manifesto.Status = Dominio.Enumeradores.StatusManifestoAvon.Finalizado;
                    repManifestoAvon.Atualizar(manifesto);
                }
                else
                {
                    if(manifesto.Status != Dominio.Enumeradores.StatusManifestoAvon.FalhaNoRetorno && repDocumento.ContarPorManifestoEStatusProblema(manifesto.Codigo) > 0)
                    {
                        manifesto.Status = Dominio.Enumeradores.StatusManifestoAvon.FalhaNoRetorno;
                        repManifestoAvon.Atualizar(manifesto);
                    }
                }
            }
        }
    }
}