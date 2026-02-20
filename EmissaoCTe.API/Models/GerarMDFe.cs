using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Configuration;



namespace EmissaoCTe.API
{
    public class GerarMDFe
    {
        private int Tempo = 120000; //2 MINUTOS

        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaGeracaoMDFe;
        private static GerarMDFe Instance;

        public static GerarMDFe GetInstance()
        {
            if (Instance == null)
                Instance = new GerarMDFe();

            return Instance;
        }

        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaGeracaoMDFe == null)
                ListaGeracaoMDFe = new ConcurrentQueue<int>();

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
                Tempo = 500;
#endif

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            //EmitirCTesAguardandoEmissao(unidadeDeTrabalho, stringConexao);
                            GerarMDFes(unidadeDeTrabalho, stringConexao);
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

        //private void EmitirCTesAguardandoEmissao(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        //{
        //    Repositorio.ConfiguracaoEmissaoEmail repConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unitOfWork);
        //    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
        //    Dominio.Entidades.ConfiguracaoEmissaoEmail configuracaoEmissaoEmail = null;
        //    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.BuscarPorStatusEPeriodo(0, new string[] { "M" }, DateTime.Today.AddDays(-1), DateTime.Today);

        //    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

        //    for (var i = 0; i < listaCTes.Count; i++)
        //    {
        //        configuracaoEmissaoEmail = repConfiguracaoEmissaoEmail.Buscar(0, listaCTes[i].Empresa.Codigo, double.Parse(listaCTes[i].Remetente.CPF_CNPJ));

        //        if (configuracaoEmissaoEmail == null)
        //            configuracaoEmissaoEmail = repConfiguracaoEmissaoEmail.Buscar(0, listaCTes[i].Empresa.Codigo, 0);

        //        if (listaCTes[i].ValorAReceber > 0 && configuracaoEmissaoEmail != null && configuracaoEmissaoEmail.Emitir == Dominio.Enumeradores.OpcaoSimNao.Sim)
        //        {
        //            if (svcCTe.Emitir(listaCTes[i].Codigo, listaCTes[i].Empresa.Codigo, unitOfWork))
        //            {
        //                FilaConsultaCTe.GetInstance().QueueItem(4, listaCTes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, unitOfWork.StringConexao);

        //                if (configuracaoEmissaoEmail.GerarMDFe == Dominio.Enumeradores.OpcaoSimNao.Sim)
        //                {
        //                    Repositorio.GerarMDFe repGerarMDFe = new Repositorio.GerarMDFe(unitOfWork);
        //                    Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);
        //                    List<Dominio.Entidades.VeiculoCTE> velculosCTe = repVeiculoCTe.BuscarPorCTe(listaCTes[i].Empresa.Codigo, listaCTes[i].Codigo);

        //                    if (velculosCTe != null && velculosCTe.Count > 0)
        //                    {
        //                        Dominio.Entidades.GerarMDFe gerarMDFe = new Dominio.Entidades.GerarMDFe();
        //                        gerarMDFe.Status = Dominio.Enumeradores.StatusMDFe.Pendente;

        //                        if (gerarMDFe.CTEs == null)
        //                            gerarMDFe.CTEs = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

        //                        gerarMDFe.CTEs.Add(listaCTes[i]);

        //                        repGerarMDFe.Inserir(gerarMDFe);
        //                    }
        //                    else
        //                    {
        //                        Servicos.Log.TratarErro("CT-e " + listaCTes[i].Codigo + ": sem veículo para gerar MDFe.", "GerarMDFe");

        //                        Dominio.Entidades.GerarMDFe gerarMDFe = new Dominio.Entidades.GerarMDFe();
        //                        gerarMDFe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
        //                        gerarMDFe.Mensagem = "CT - e " + listaCTes[i].Numero + ": sem veículo para gerar MDFe.";
        //                        if (gerarMDFe.CTEs == null)
        //                            gerarMDFe.CTEs = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
        //                        gerarMDFe.CTEs.Add(listaCTes[i]);

        //                        repGerarMDFe.Inserir(gerarMDFe);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Servicos.Log.TratarErro("CT-e " + listaCTes[i].Codigo + ": ocorreu uma falha e não foi possível emitir.", "GerarMDFe");
        //            }
        //        }
        //        else
        //        {
        //            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(listaCTes[i].Codigo);
        //            if (cte != null)
        //            {
        //                cte.Status = "S";
        //                repCTe.Atualizar(cte);
        //            }
        //        }
        //    }
        //}

        private void GerarMDFes(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);

            Repositorio.GerarMDFe repGerarMDFe = new Repositorio.GerarMDFe(unitOfWork);
            List<Dominio.Entidades.GerarMDFe> listaGerarMDFe = repGerarMDFe.BuscarPorStatus(Dominio.Enumeradores.StatusMDFe.Pendente);

            foreach (Dominio.Entidades.GerarMDFe gerarMDFe in listaGerarMDFe)
            {
                try
                {
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = gerarMDFe.CTEs.Where(o => o.Status == "A").Select(o => o).ToList();
                    if (listaCTes.Count() > 0 && listaCTes.Count() == gerarMDFe.CTEs.Count())
                    {
                        if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                            unitOfWork.Start(System.Data.IsolationLevel.Serializable);
                        else
                            unitOfWork.Start();

                        List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaMDFes = svcMDFe.GerarMDFePorCTesDestinosDiferentes(listaCTes.FirstOrDefault().Empresa, listaCTes, string.Empty, unitOfWork);

                        for (var i = 0; i < listaMDFes.Count(); i++)
                        {
                            if (gerarMDFe.MDFEs == null)
                                gerarMDFe.MDFEs = new List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
                            gerarMDFe.MDFEs.Add(listaMDFes[i]);
                            gerarMDFe.Status = Dominio.Enumeradores.StatusMDFe.Autorizado;
                            repGerarMDFe.Atualizar(gerarMDFe);
                        }

                        unitOfWork.CommitChanges();

                        for (var i = 0; i < listaMDFes.Count(); i++)
                        {
                            if (svcMDFe.Emitir(listaMDFes[i], unitOfWork))
                                if (!svcMDFe.AdicionarMDFeNaFilaDeConsulta(listaMDFes[i], unitOfWork))
                                    Servicos.Log.TratarErro("O MDF-e nº " + listaMDFes[i].Numero.ToString() + " da empresa " + listaMDFes[i].Empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta.", "GerarMDFe");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Problema ao gerar MDFe: " + gerarMDFe.Codigo + ": " + ex.Message, "GerarMDFe");

                    unitOfWork.Rollback();

                    gerarMDFe.Mensagem = ex.Message;
                    gerarMDFe.Status = Dominio.Enumeradores.StatusMDFe.Rejeicao;
                    repGerarMDFe.Atualizar(gerarMDFe);
                }
            }
        }
    }
}
