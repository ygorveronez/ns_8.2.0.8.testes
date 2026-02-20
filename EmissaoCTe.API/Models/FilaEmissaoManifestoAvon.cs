using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace EmissaoCTe.API
{
    public class FilaEmissaoManifestoAvon
    {
        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentDictionary<int, ConcurrentQueue<int>> ListaManifestoEmissao;
        private static FilaEmissaoManifestoAvon Instance;

        public static FilaEmissaoManifestoAvon GetInstance()
        {
            if (Instance == null)
                Instance = new FilaEmissaoManifestoAvon();

            return Instance;
        }

        public void QueueItem(int idEmpresa, int idManifesto, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaManifestoEmissao == null)
                ListaManifestoEmissao = new ConcurrentDictionary<int, ConcurrentQueue<int>>();

            if (!ListaTasks.ContainsKey(idEmpresa))
            {
                this.IniciarThread(idManifesto, idEmpresa, stringConexao);
            }
            else
            {
                ConcurrentQueue<int> lista = null;

                if (ListaManifestoEmissao.TryGetValue(idEmpresa, out lista))
                {
                    lista.Enqueue(idManifesto);
                }
                else
                {
                    Servicos.Log.TratarErro("Não foi possível obter a lista com os manifestos para emissão.");
                }
            }
        }

        private void IniciarThread(int idManifesto, int idEmpresa, string stringConexao)
        {
            var filaEmissao = new ConcurrentQueue<int>();

            filaEmissao.Enqueue(idManifesto);

            if (ListaManifestoEmissao.TryAdd(idEmpresa, filaEmissao))
            {
                Task task = new Task(() =>
                {
                    int idManifestoEmissao = 0;

                    if (!filaEmissao.TryDequeue(out idManifestoEmissao))
                        Servicos.Log.TratarErro("Falha ao obter o primeiro item da fila de manifestos para emissão.");

                    Servicos.CTe servicoCTe = null;
                    Servicos.Avon servicoAvon = null;

                    Repositorio.ManifestoAvon repManifestoAvon = null;
                    Repositorio.DocumentoManifestoAvon repDocumentoManifestoAvon = null;
                    Repositorio.Localidade repLocalidade = null;
                    Repositorio.Estado repEstado = null;

                    Dominio.Entidades.ManifestoAvon manifestoAvon = null;
                    List<Dominio.Entidades.DocumentoManifestoAvon> documentosManifesto = null;

                    bool sleep = false;

                    while (true)
                    {
                        using Repositorio.UnitOfWork unidadeDeTrabalho = new(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

                        try
                        {
                            if (idManifestoEmissao > 0)
                            {
                                repManifestoAvon = new Repositorio.ManifestoAvon(unidadeDeTrabalho);
                                repDocumentoManifestoAvon = new Repositorio.DocumentoManifestoAvon(unidadeDeTrabalho);
                                repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                                repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                                manifestoAvon = repManifestoAvon.BuscarPorCodigo(idManifestoEmissao);

                                if (manifestoAvon != null && manifestoAvon.Status == Dominio.Enumeradores.StatusManifestoAvon.Enviado)
                                {
                                    servicoAvon = new Servicos.Avon(unidadeDeTrabalho);
                                    servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                                    documentosManifesto = repDocumentoManifestoAvon.BuscarPorManifesto(manifestoAvon.Codigo, Dominio.Enumeradores.StatusDocumentoManifestoAvon.Enviado);

                                    int countProcessados = 0;

                                    foreach (Dominio.Entidades.DocumentoManifestoAvon documento in documentosManifesto)
                                    {
                                        if (countProcessados == 50)
                                        {
                                            countProcessados = 0;

                                            repDocumentoManifestoAvon = null;
                                            repEstado = null;
                                            repLocalidade = null;
                                            repManifestoAvon = null;

                                            GC.Collect();

                                            repDocumentoManifestoAvon = new Repositorio.DocumentoManifestoAvon(unidadeDeTrabalho);
                                            repEstado = new Repositorio.Estado(unidadeDeTrabalho);
                                            repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                                            repManifestoAvon = new Repositorio.ManifestoAvon(unidadeDeTrabalho);
                                        }

                                        countProcessados++;

                                        if (documento.Status == Dominio.Enumeradores.StatusDocumentoManifestoAvon.Enviado)
                                        {
                                            Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica nfe = new Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica(documento.Documento);

                                            if (repLocalidade.BuscarPorCodigoIBGE(nfe.Destinatario.CodigoMunicipio) == null)
                                            {
                                                Dominio.Entidades.Localidade localidade = new Dominio.Entidades.Localidade();

                                                localidade.Codigo = repLocalidade.BuscarPorMaiorCodigo() + 1;
                                                localidade.CodigoIBGE = nfe.Destinatario.CodigoMunicipio;
                                                localidade.DataAtualizacao = DateTime.Now;
                                                localidade.Estado = repEstado.BuscarPorSigla(nfe.Destinatario.UF);
                                                localidade.Descricao = nfe.Destinatario.DescricaoMunicipio;

                                                repLocalidade.Inserir(localidade);
                                            }

                                            if (repLocalidade.BuscarPorCodigoIBGE(nfe.Emitente.CodigoMunicipio) == null)
                                            {
                                                Dominio.Entidades.Localidade localidade = new Dominio.Entidades.Localidade();

                                                localidade.Codigo = repLocalidade.BuscarPorMaiorCodigo() + 1;
                                                localidade.CodigoIBGE = nfe.Emitente.CodigoMunicipio;
                                                localidade.DataAtualizacao = DateTime.Now;
                                                localidade.Estado = repEstado.BuscarPorSigla(nfe.Emitente.UF);
                                                localidade.Descricao = nfe.Emitente.DescricaoMunicipio;

                                                repLocalidade.Inserir(localidade);
                                            }

                                            try
                                            {
                                                unidadeDeTrabalho.Start();

                                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoAvon.GerarCTePorNFe(manifestoAvon.Empresa.Codigo, nfe, documento);

                                                documento.Status = Dominio.Enumeradores.StatusDocumentoManifestoAvon.Emitido;
                                                documento.CTe = cte;

                                                repDocumentoManifestoAvon.Atualizar(documento);

                                                unidadeDeTrabalho.CommitChanges();

                                                servicoCTe.Emitir(cte.Codigo, cte.Empresa.Codigo, unidadeDeTrabalho, "E");
                                            }
                                            catch
                                            {
                                                if (unidadeDeTrabalho != null)
                                                    unidadeDeTrabalho.Rollback();

                                                throw;
                                            }
                                        }
                                    }

                                    if (repDocumentoManifestoAvon.ContarPorManifesto(manifestoAvon.Codigo, Dominio.Enumeradores.StatusDocumentoManifestoAvon.Enviado) <= 0)
                                    {
                                        manifestoAvon.Status = Dominio.Enumeradores.StatusManifestoAvon.Emitido;

                                        repManifestoAvon.Atualizar(manifestoAvon);
                                    }
                                }

                                GC.Collect();

                                if (sleep)
                                    System.Threading.Thread.Sleep(5000);

                                sleep = false;
                            }

                            while (!filaEmissao.TryDequeue(out idManifestoEmissao))
                                System.Threading.Thread.Sleep(5000);

                        }
                        catch (TaskCanceledException abort)
                        {
                            Servicos.Log.TratarErro(string.Concat("Task de emissão de manifestos cancelada: ", abort.ToString()));

                            break;
                        }
                        catch (System.Threading.ThreadAbortException abortThread)
                        {
                            Servicos.Log.TratarErro(string.Concat("Thread de emissão de manifestos cancelada: ", abortThread));

                            break;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);

                            if (idManifestoEmissao > 0)
                                filaEmissao.Enqueue(idManifestoEmissao);

                            servicoCTe = null;
                            servicoAvon = null;
                            repDocumentoManifestoAvon = null;
                            repManifestoAvon = null;
                            repEstado = null;
                            repLocalidade = null;
                            manifestoAvon = null;
                            documentosManifesto = null;
                            idManifestoEmissao = 0;
                        }
                    }
                });

                if (ListaTasks.TryAdd(idEmpresa, task))
                    task.Start();
                else
                    Servicos.Log.TratarErro("Não foi possível adicionar a task de emissão de manifestos à fila.");
            }
            else
            {
                Servicos.Log.TratarErro("Não foi possível adicionar a fila de emissão de manifestos.");
            }
        }
    }
}