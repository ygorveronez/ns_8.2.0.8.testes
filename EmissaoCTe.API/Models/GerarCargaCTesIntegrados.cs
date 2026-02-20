using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class GerarCargaCTesIntegrados
    {
        private int Tempo = 150000; //2,5 MINUTOS

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaGerarCargaCTesIntegrados;
        private static GerarCargaCTesIntegrados Instance;

        public static GerarCargaCTesIntegrados GetInstance()
        {
            if (Instance == null)
                Instance = new GerarCargaCTesIntegrados();

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
                if (!string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["IntervaloGeracaoCargaMultiCTe"]))
                {
                    int.TryParse(System.Configuration.ConfigurationManager.AppSettings["IntervaloGeracaoCargaMultiCTe"], out Tempo);
                    Tempo = Tempo * 60000;
                }

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            VerificarCTesPendentes(stringConexao, unidadeDeTrabalho);

                            unidadeDeTrabalho.Dispose();
                        }

                        GC.Collect();

                        System.Threading.Thread.Sleep(Tempo);

                        if (!filaConsulta.TryDequeue(out idEmpresa))
                        {
                            Servicos.Log.TratarErro("Task parou a execução", "GerarCargaCTes");
                            break;
                        }

                    }
                    catch (TaskCanceledException abort)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task de geração de carga de CTes Integrados cancelada: ", abort.ToString()), "GerarCargaCTes");
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread de geração de carga de CTes Integrados cancelada: ", abortThread), "GerarCargaCTes");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "GerarCargaCTes");
                        System.Threading.Thread.Sleep(Tempo / 2);
                    }
                }
            });

            if (ListaTasks.TryAdd(idEmpresa, task))
                task.Start();
            else
                Servicos.Log.TratarErro("Não foi possível adicionar a task de geração de carga de CTes Integrados à fila.", "GerarCargaCTes");
        }


        private void VerificarCTesPendentes(string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork);

            Servicos.Log.TratarErro("Consultando CTes pendentes de geração de carga", "GerarCargaCTes");
            List<int> listaIntegracaoCTe = repIntegracaoCTe.BuscarPendentesIntegracao(50);
            List<int> listaIntegracaoCTeTotais = repIntegracaoCTe.BuscarPendentesIntegracao(0);
            Servicos.Log.TratarErro("CTes pendentes de geração de carga: " + listaIntegracaoCTeTotais.Count(), "GerarCargaCTes");

            Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

            for (var i = 0; i < listaIntegracaoCTe.Count; i++)
            {
                Dominio.Entidades.IntegracaoCTe integracaoCTe = repIntegracaoCTe.BuscarPorCodigo(listaIntegracaoCTe[i]);
                if (integracaoCTe != null && integracaoCTe.CTe.Status == "A")
                {
                    try
                    {
                        string gerarNumeracaoCarga = System.Configuration.ConfigurationManager.AppSettings["GerarNumeracaoCarga"];
                        string numeroCarga = integracaoCTe.NumeroDaCarga > 0 ? integracaoCTe.NumeroDaCarga.ToString() : "";
                        if (gerarNumeracaoCarga == "SIM")
                        {
                            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                            numeroCarga = repCarga.ObterProximoCodigo().ToString();
                        }

                        string tipoVeiculo = "";
                        try
                        {
                            if (integracaoCTe.CTe.ObservacoesGerais != null && integracaoCTe.CTe.ObservacoesGerais.Contains("TIPO VEICULO:"))
                            {
                                int posicao = integracaoCTe.CTe.ObservacoesGerais.IndexOf("TIPO VEICULO:");
                                int posicaoFim = posicao + 13 + 8;
                                if (posicao > -1 && posicaoFim > -1)
                                {
                                    int inicio = posicao + 13;
                                    int tamanho = posicaoFim - (posicao + 13);
                                    tipoVeiculo = integracaoCTe.CTe.ObservacoesGerais.Substring(inicio, tamanho).Replace(" ", "");
                                }
                            }
                            else if (integracaoCTe.CTe.ObservacoesGerais != null && integracaoCTe.CTe.ObservacoesGerais.Contains("TIPO: VEICULO"))
                            { 
                                int posicao = integracaoCTe.CTe.ObservacoesGerais.IndexOf("TIPO: VEICULO");
                                int posicaoFim = posicao + 13 + 8;
                                if (posicao > -1 && posicaoFim > -1)
                                    tipoVeiculo = integracaoCTe.CTe.ObservacoesGerais.Substring(posicao + 13, posicaoFim - (posicao + 13)).Replace(" ", "");
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Problema ao gerar definir tipi de veiculo: " + ex.Message, "GerarCargaCTes");
                        }

                        Servicos.Log.TratarErro("Gerando carga CTe: " + integracaoCTe.CTe.Codigo, "GerarCargaCTes");
                        if (servicoCTe.GerarCargaCTe(integracaoCTe.CTe.Codigo, integracaoCTe.NumeroDaUnidade.ToString(), numeroCarga, tipoVeiculo, "Todas", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte, stringConexao, unitOfWork) > 0)
                        {
                            integracaoCTe.GerouCargaEmbarcador = true;
                            repIntegracaoCTe.Atualizar(integracaoCTe);
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Problema ao gerar carga CTe codigo : " + integracaoCTe.CTe.Codigo, "GerarCargaCTes");

                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Problema ao gerar carga CTe codigo " + integracaoCTe.CTe.Codigo + ": " + ex.Message, "GerarCargaCTes");

                        throw;
                    }
                }
                unitOfWork.FlushAndClear();
            }

#if !DEBUG            
            List<Dominio.Entidades.IntegracaoCTe> listaIntegracoesSemCarga = repIntegracaoCTe.BuscarIntegracoesSemCarga();
            foreach (Dominio.Entidades.IntegracaoCTe integracao in listaIntegracoesSemCarga)
            {
                integracao.GerouCargaEmbarcador = false;
                repIntegracaoCTe.Atualizar(integracao);
            }
#endif
            if (listaIntegracaoCTeTotais.Count() > listaIntegracaoCTe.Count())
                Tempo = Tempo / 2;

        }

        
    }
}