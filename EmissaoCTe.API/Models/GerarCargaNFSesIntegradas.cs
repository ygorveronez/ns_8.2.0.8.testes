using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EmissaoCTe.API
{
    public class GerarCargaNFSesIntegrados
    {
        private int Tempo = 300000; //5 MINUTOS

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaGerarCargaNFSesIntegrados;
        private static GerarCargaNFSesIntegrados Instance;

        public static GerarCargaNFSesIntegrados GetInstance()
        {
            if (Instance == null)
                Instance = new GerarCargaNFSesIntegrados();

            return Instance;
        }


        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaGerarCargaNFSesIntegrados == null)
                ListaGerarCargaNFSesIntegrados = new ConcurrentQueue<int>();

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
                    Tempo = Tempo * 6000;
                }

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            VerificarNFSesPendentes(stringConexao, unidadeDeTrabalho);
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
                        Servicos.Log.TratarErro(string.Concat("Task de geração de carga de NFSes Integrados cancelada: ", abort.ToString()));
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread de geração de carga de NFSes Integrados cancelada: ", abortThread));
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
                Servicos.Log.TratarErro("Não foi possível adicionar a task de geração de carga de NFSEs Integrados à fila.");
        }


        private void VerificarNFSesPendentes(string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.IntegracaoNFSe repIntegracaoNFSe = new Repositorio.IntegracaoNFSe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<int> listaIntegracaoNFSe = repIntegracaoNFSe.BuscarPendentesIntegracao();

            Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);
            Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

            for (var i = 0; i < listaIntegracaoNFSe.Count; i++)
            {
                Dominio.Entidades.IntegracaoNFSe integracaoNFSe = repIntegracaoNFSe.BuscarPorCodigo(listaIntegracaoNFSe[i]);
                if (integracaoNFSe != null && integracaoNFSe.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado || integracaoNFSe.NFSe.Status == Dominio.Enumeradores.StatusNFSe.AgGeracaoNFSeManual)
                {
                    try
                    {
                        string gerarNumeracaoCarga = System.Configuration.ConfigurationManager.AppSettings["GerarNumeracaoCarga"];
                        string numeroCarga = integracaoNFSe.NumeroDaCarga > 0 ? integracaoNFSe.NumeroDaCarga.ToString() : "";
                        if (gerarNumeracaoCarga == "SIM")
                        {
                            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                            numeroCarga = repCarga.ObterProximoCodigo().ToString();
                        }
                        string status = integracaoNFSe.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ? "A" : (integracaoNFSe.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Rejeicao ? "R" : (integracaoNFSe.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Cancelado ? "C" : "P"));
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarNFSePorEmpresaNumeroESerie(integracaoNFSe.NFSe.Empresa.Codigo, integracaoNFSe.NFSe.Numero, integracaoNFSe.NFSe.Serie.Numero, status, integracaoNFSe.NFSe.DataEmissao);

                        if (cte == null)
                            cte = servicoNFSe.ConverterNFSeEmCTe(integracaoNFSe.NFSe, unitOfWork);

                        if (cte != null)
                        {
                            string tipoVeiculo = "";
                            if (cte.ObservacoesGerais != null && cte.ObservacoesGerais.Contains("TIPO VEICULO:"))
                            {
                                int posicao = cte.ObservacoesGerais.IndexOf("TIPO VEICULO:");
                                int posicaoFim = posicao + 8;
                                if (posicao > -1 && posicaoFim > -1)
                                    tipoVeiculo = cte.ObservacoesGerais.Substring(posicao + 13, posicaoFim - (posicao + 13)).Replace(" ", "");
                            }
                            else if (cte.ObservacoesGerais != null && cte.ObservacoesGerais.Contains("TIPO: VEICULO"))
                            {
                                int posicao = cte.ObservacoesGerais.IndexOf("TIPO: VEICULO");
                                int posicaoFim = posicao + 13 + 8;
                                if (posicao > -1 && posicaoFim > -1)
                                    tipoVeiculo = cte.ObservacoesGerais.Substring(posicao + 13, posicaoFim - (posicao + 13)).Replace(" ", "");
                            }

                            int numeroOcorrencia = 0;
                            if (System.Configuration.ConfigurationManager.AppSettings["VincularNFSeProcessadaOcorrencia"] == "SIM")
                            {                                
                                if (!string.IsNullOrWhiteSpace(cte.ObservacoesGerais) && cte.ObservacoesGerais.ToUpper().Contains("OCORRENCIA:"))
                                {
                                    Regex regex = new Regex("OCORRENCIA:(.*?);");
                                    string obs = regex.Match(cte.ObservacoesGerais.ToUpper()).Groups.Count > 0 ? regex.Match(cte.ObservacoesGerais.ToUpper()).Groups[1].Value : string.Empty;

                                    int.TryParse(Utilidades.String.OnlyNumbers(obs), out numeroOcorrencia);
                                }
                            }

                            //Se possui numero de ocorrência vincula a NFSe a ocorrência
                            if (numeroOcorrencia > 0)
                            {
                                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorNumero(numeroOcorrencia);
                                if (ocorrencia != null)
                                {
                                    if (!Servicos.Embarcador.Carga.PreCTe.EnviarCTeParaComplementoInfo(cte, numeroOcorrencia, unitOfWork, out string erro))
                                        Servicos.Log.TratarErro("Problema ao vincular NFSe (CTe cod. " + cte.Codigo.ToString() + ") a Ocorrencia " + numeroOcorrencia + " : " + erro, "GerarCargasNFSe");
         
                                    Servicos.Embarcador.Carga.PreCTe.VerificarEnviouTodosPreDocumentos(ocorrencia, unitOfWork);

                                    integracaoNFSe.GerouCargaEmbarcador = true;
                                    repIntegracaoNFSe.Atualizar(integracaoNFSe);
                                }
                                else
                                    Servicos.Log.TratarErro("Problema ao vincular NFSe cod. " + integracaoNFSe.NFSe.Codigo.ToString() + " a Ocorrencia " + numeroOcorrencia + " não localizada.", "GerarCargasNFSe");

                            } //Caso contrário gera uma nova carga
                            else if (servicoCTe.GerarCargaCTe(cte.Codigo, integracaoNFSe.NumeroDaUnidade.ToString(), numeroCarga, tipoVeiculo, "Todas", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte, stringConexao, unitOfWork) > 0)
                            {
                                integracaoNFSe.GerouCargaEmbarcador = true;
                                repIntegracaoNFSe.Atualizar(integracaoNFSe);

                                if(integracaoNFSe.NFSe.Status == Dominio.Enumeradores.StatusNFSe.AgGeracaoNFSeManual)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);
                                    if((cargaCTe?.Carga ?? null) != null)
                                        Servicos.Embarcador.NFSe.NFSManual.GerarRegistrosLancamentoManual(integracaoNFSe.NFSe, cargaCTe.Carga, unitOfWork);
                                }
                            }
                            else Servicos.Log.TratarErro("Problema ao gerar carga NFSe (CTe codigo : " + cte.Codigo +"): Carga não gerada.", "GerarCargasNFSe");

                        }


                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Problema ao gerar carga NFSe codigo " + integracaoNFSe.NFSe.Codigo + ": " + ex.Message, "GerarCargasNFSe");
                    }
                }
            }
        }

    }
}