using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EmissaoCTe.API
{
    public class GerarSubcontratacoes
    {
        private int Tempo = 60000; //60 segundos

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaGerarSubcontratacoes;
        private static GerarSubcontratacoes Instance;

        public static GerarSubcontratacoes GetInstance()
        {
            if (Instance == null)
                Instance = new GerarSubcontratacoes();

            return Instance;
        }


        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaGerarSubcontratacoes == null)
                ListaGerarSubcontratacoes = new ConcurrentQueue<int>();

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
                            VerificarSubcontratacoesPendentes(unidadeDeTrabalho);
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


        private void VerificarSubcontratacoesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Subcontratacao repSubcontratacao = new Repositorio.Subcontratacao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            List<int> listaSubcontratacoes = repSubcontratacao.BuscarPendentes(5);
            Servicos.Subcontratacao svcSubcontratacao = new Servicos.Subcontratacao(unitOfWork);
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            for (var i = 0; i < listaSubcontratacoes.Count; i++)
            {
                if (svcSubcontratacao.ValidarDadosSubcontratacao(listaSubcontratacoes[i], unitOfWork))
                {
                    Dominio.Entidades.Subcontratacao subcontratacao = repSubcontratacao.BuscarPorCodigo(listaSubcontratacoes[i]);

                    if (subcontratacao != null && subcontratacao.Situacao == Dominio.Enumeradores.SituacaoSubcontratacao.AgProcessamento)
                    {
                        if (subcontratacao.AgruparCTes)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteGerado = null;
                            try
                            {
                                Dominio.ObjetosDeValor.CTe.CTe cte = svcSubcontratacao.GerarObjetoCTe(subcontratacao, unitOfWork);

                                if (cte == null)
                                    throw new Exception("Não foi possível carregar dados para CTe de subcontratação.");

                                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cte.Emitente?.CNPJ));
                                if (transportador == null || transportador.TipoAmbiente != transportador.EmpresaPai.TipoAmbiente || transportador.Configuracao == null || transportador.Configuracao.SerieInterestadual == null || transportador.Configuracao.SerieIntraestadual == null)
                                {
                                    subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                                    subcontratacao.DescricaoFalha = "CTe não gerado";
                                }
                                if (transportador != null && cte.CodigoIBGECidadeInicioPrestacao == cte.CodigoIBGECidadeTerminoPrestacao && (!transportador.Configuracao?.GerarNFSeImportacoes ?? false)  )
                                {
                                    subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                                    subcontratacao.DescricaoFalha = "Transportador configurado para não gerar NFSe";
                                }
                                else
                                {
                                    unitOfWork.Start();
                                    cteGerado = svcCTe.GerarCTePorObjeto(cte, 0, unitOfWork, "1", 0, "E", null, 0, null);
                                    unitOfWork.CommitChanges();

                                    if (cteGerado == null)
                                    {
                                        subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                                        subcontratacao.DescricaoFalha = "CTe não gerado";
                                    }
                                    else
                                    {
                                        subcontratacao.DocumentoSubcontratacao = cteGerado;
                                        subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.EmitindoCTes;
                                        subcontratacao.DescricaoFalha = String.Empty;

                                        if (subcontratacao.DocumentoSubcontratacao.Status == "E")
                                        {
                                            if (!svcCTe.Emitir(ref cteGerado, unitOfWork))
                                                Servicos.Log.TratarErro("O CT-e " + cteGerado.Numero.ToString() + " da empresa " + cteGerado.Empresa.CNPJ + " foi salvo, porem, ocorreu uma falha ao enviar-lo ao Sefaz.", "GerarSubcontratacoes");

                                            if (!svcCTe.AdicionarCTeNaFilaDeConsulta(cteGerado, unitOfWork))
                                                Servicos.Log.TratarErro("O CT-e " + cteGerado.Numero.ToString() + " da empresa " + cteGerado.Empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-lo na fila de consulta.", "GerarSubcontratacoes");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);

                                unitOfWork.Rollback();

                                subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento;
                                subcontratacao.DescricaoFalha = ex.Message;
                            }

                            repSubcontratacao.Atualizar(subcontratacao);
                        }

                    }
                    else
                        Servicos.Log.TratarErro("Subcontratação codigo " + listaSubcontratacoes[i].ToString() + " não localzado.", "GerarCTeIntegracao");
                }

                unitOfWork.FlushAndClear();
            }
        }

        private void NotificarFalhaPorEmail(int codigoCTe, Exception erro, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Email svcEmail = new Servicos.Email();

                string ambiente = System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
                string assunto = ambiente + " - Problemas na geração de CTe subcontratacao!";

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