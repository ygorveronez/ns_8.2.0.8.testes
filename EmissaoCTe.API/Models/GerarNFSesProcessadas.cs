using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class GerarNFSesProcessadas
    {
        private int Tempo = 60000; //60000 = 1 MINUTO //3600000 = 60 MINUTO

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaGerarNFSesProcessadas;
        private static GerarNFSesProcessadas Instance;

        public static GerarNFSesProcessadas GetInstance()
        {
            if (Instance == null)
                Instance = new GerarNFSesProcessadas();

            return Instance;
        }


        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaGerarNFSesProcessadas == null)
                ListaGerarNFSesProcessadas = new ConcurrentQueue<int>();

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
                if (!string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["IntervaloGeracaoNFSeProcessadas"]))
                {
                    int.TryParse(System.Configuration.ConfigurationManager.AppSettings["IntervaloGeracaoNFSeProcessadas"], out Tempo);
                    Tempo = Tempo * 60000;
                }

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            VerificarNFSesProcessadasPendentes(unidadeDeTrabalho);
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
                        Servicos.Log.TratarErro(string.Concat("Task de geração NFSes Processadas cancelada: ", abort.ToString()));
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread de geração NFSes Processadas cancelada: ", abortThread));
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
                Servicos.Log.TratarErro("Não foi possível adicionar a task de geração NFSes Processadas Integrados à fila.");
        }


        private void VerificarNFSesProcessadasPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork);
            Repositorio.IntegracaoNFSe repIntegracaoNFSe = new Repositorio.IntegracaoNFSe(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            int quantidadePendentes = repIntegracaoNFSe.ContarIntegracaoNFSeProcessadasPendentes();
            Servicos.Log.TratarErro("NFSes pendentes de geração: " + quantidadePendentes, "GerarNFSesProcessadas");

            if (quantidadePendentes > 0)
            {
                List<int> listaIntegracaoNFSe = repIntegracaoNFSe.BuscarIntegracaoNFSeProcessadasPendentes(50);

                Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);

                for (var i = 0; i < listaIntegracaoNFSe.Count; i++)
                {
                    Dominio.Entidades.IntegracaoNFSe integracaoNFSe = repIntegracaoNFSe.BuscarPorCodigo(listaIntegracaoNFSe[i]);
                    if (integracaoNFSe.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Pendente)
                    {
                        try
                        {
                            Dominio.ObjetosDeValor.NFSe.NFSeProcessada documentoNFSe = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.NFSe.NFSeProcessada>(integracaoNFSe.Arquivo);

                            Dominio.Entidades.Veiculo veiculo = null;
                            if (!string.IsNullOrWhiteSpace(documentoNFSe.Placa))
                            {
                                veiculo = repVeiculo.BuscarPorPlaca(integracaoNFSe.NFSe.Empresa.Codigo, documentoNFSe.Placa);

                                if (veiculo == null)
                                {
                                    veiculo = new Dominio.Entidades.Veiculo();
                                    veiculo.Placa = documentoNFSe.Placa;
                                    veiculo.Empresa = integracaoNFSe.NFSe.Empresa;
                                    veiculo.Ativo = true;
                                    veiculo.Estado = integracaoNFSe.NFSe.Empresa.Localidade.Estado;
                                    veiculo.Tipo = "P";
                                    veiculo.TipoRodado = "01";
                                    veiculo.TipoCarroceria = "00";

                                    repVeiculo.Inserir(veiculo);
                                }
                            }

                            if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                                unitOfWork.Start(System.Data.IsolationLevel.Serializable);
                            else
                                unitOfWork.Start();

                            Dominio.Entidades.NFSe nfse = servicoNFSe.GravarNFSeProcessada(documentoNFSe, integracaoNFSe.NFSe.Empresa, veiculo, integracaoNFSe.NFSe.Serie, unitOfWork, integracaoNFSe.NFSe.Codigo);

                            if (System.Configuration.ConfigurationManager.AppSettings["GerarCargaIntegracao"] == "SIM")
                            {
                                //Gera o CTe
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoNFSe.ConverterNFSeEmCTe(nfse, unitOfWork, false);

                                //Gera a integração do CTe para gerar a carga
                                Dominio.Entidades.IntegracaoCTe integracaoCTe = new Dominio.Entidades.IntegracaoCTe();
                                integracaoCTe.CTe = cte;
                                integracaoCTe.Tipo = Dominio.Enumeradores.TipoIntegracao.Emissao;
                                integracaoCTe.GerouCargaEmbarcador = false;
                                integracaoCTe.NumeroDaCarga = integracaoNFSe.NumeroDaCarga;
                                integracaoCTe.NumeroDaUnidade = integracaoNFSe.NumeroDaUnidade;
                                integracaoCTe.Arquivo = "NFSe codigo " + integracaoNFSe.NFSe.Codigo.ToString();
                                integracaoCTe.Status = Dominio.Enumeradores.StatusIntegracao.Integrado;
                                repIntegracaoCTe.Inserir(integracaoCTe);

                                integracaoNFSe.GerouCargaEmbarcador = true;
                            }
                            else
                                integracaoNFSe.GerouCargaEmbarcador = false;

                            integracaoNFSe.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                            repIntegracaoNFSe.Atualizar(integracaoNFSe);

                            unitOfWork.CommitChanges();

                            unitOfWork.FlushAndClear();
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Problema ao gerar NFSe Processada codigo " + integracaoNFSe.Codigo + ": " + ex, "GerarNFSesProcessadas");

                            NotificarFalhaPorEmail(integracaoNFSe.Codigo, ex, unitOfWork);

                            unitOfWork.Rollback();

                            throw;
                        }
                    }
                    else
                    {
                        integracaoNFSe.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                        repIntegracaoNFSe.Atualizar(integracaoNFSe);
                    }

                }
            }
        }

        private void NotificarFalhaPorEmail(int codigoCTe, Exception erro, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Email svcEmail = new Servicos.Email();

                string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                string assunto = ambiente + " - Problemas na geração de NFSe Processada!";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<p>Atenção, problemas na geração de NFSe Processada no ambiente ").Append(ambiente).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                sb.Append("Erro ao gerar CTe código: " + codigoCTe.ToString() + " ").Append(erro).Append("</p><br /> <br />");

                System.Text.StringBuilder ss = new System.Text.StringBuilder();
                ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

#if DEBUG
                svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte1@multisoftware.com.br", 0, unitOfWork);
#else
                    svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "infra@multisoftware.com.br", "", "", assunto, sb.ToString(), string.Empty, null, ss.ToString(), true, "cte1@multisoftware.com.br",0, unitOfWork);
#endif
            }
            catch (Exception exptEmail)
            {
                Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
            }
        }
    }
}