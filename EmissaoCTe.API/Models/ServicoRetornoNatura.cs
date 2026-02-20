using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class ServicoRetornoNatura
    {
        private int Tempo = 60000; //1 minuto

        private static ServicoRetornoNatura Instance;
        private static Task Task;

        public static ServicoRetornoNatura GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoRetornoNatura();

            return Instance;
        }

        public void IniciarThread(int idEmpresa, string stringConexao)
        {
            if (Task == null)
            {
                Task = new Task(() =>
                {
#if DEBUG
                    System.Threading.Thread.Sleep(6666);
                    Tempo = 5000;
#endif

                    while (true)
                    {
                        try
                        {
                            using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao))
                            {
                                RetornoDTsNatura(unidadeDeTrabalho);
                            }

                            GC.Collect();

                            System.Threading.Thread.Sleep(Tempo);
                        }
                        catch (TaskCanceledException abort)
                        {
                            Servicos.Log.TratarErro(string.Concat("Task de finalização integracao CTe cancelada: ", abort.ToString()));
                            break;
                        }
                        catch (System.Threading.ThreadAbortException abortThread)
                        {
                            Servicos.Log.TratarErro(string.Concat("Task de finalização integracao CTe cancelada: ", abortThread));
                            break;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            System.Threading.Thread.Sleep(Tempo);
                        }
                    }

                });


                Task.Start();
            }
        }

        private void RetornoDTsNatura(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentoTransporteNatura repDT = new Repositorio.DocumentoTransporteNatura(unitOfWork);
            Repositorio.NotaFiscalDocumentoTransporteNatura repNotasDT = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);

            List<Dominio.Entidades.DocumentoTransporteNatura> listaDTs = repDT.BuscarPorSituacao(0, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.AguardandoRetornoAutomatico);

            Servicos.Natura svcNatura = new Servicos.Natura(unitOfWork);
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            foreach (Dominio.Entidades.DocumentoTransporteNatura documento in listaDTs)
            {
                try 
                {
                    List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> listaNotasDT = repNotasDT.BuscarPorDocumentoTransporte(documento.Codigo);

                    //Verifica se foi emitido em produção e só faz retorno se for produção
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (from o in listaNotasDT where o.CTe.Status == "A" select o.CTe).FirstOrDefault();
                    if (cte != null && cte.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                    {
                        bool autorizouTodos = (from o in listaNotasDT where o.CTe.Status == "A" select o).Count() == (from o in listaNotasDT where o.CTe != null select o).Count();
                        if (autorizouTodos)
                        {
                            Servicos.ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentResponseDados[] retorno = svcNatura.EnviarRetornoDocumentoTransporte(documento.Codigo, unitOfWork);

                            string numeroCTes = string.Empty;

                            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTEs = (from o in listaNotasDT where o.CTe.Status == "A" select o.CTe).Distinct().ToList();
                            for (int j = 0; j < listaCTEs.Count; j++)
                            {
                                if (listaCTEs != null)
                                {
                                    if (string.IsNullOrWhiteSpace(numeroCTes))
                                        numeroCTes = listaCTEs[j].Numero.ToString();
                                    else
                                        numeroCTes = string.Concat(numeroCTes, ", " + listaCTEs[j].Numero.ToString());
                                }
                            }

                            if (documento.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(documento.Empresa.Configuracao.EmailsNotificacaoNatura))
                            {
                                string assunto = "DT " + documento.NumeroDT + " retornada para a Natura";
                                System.Text.StringBuilder st = new System.Text.StringBuilder();

                                st.Append("DT emitida e retornada para a Natura").AppendLine().AppendLine();
                                st.Append("Transportador: " + documento.Empresa.CNPJ_Formatado + " " + documento.Empresa.RazaoSocial).AppendLine();
                                st.Append("DT Natura: " + documento.NumeroDT).AppendLine();
                                st.Append("Notas Fiscais: " + documento.NumeroNotas).AppendLine();
                                st.Append("CTes: " + numeroCTes).AppendLine().AppendLine();

                                if (retorno != null)
                                {
                                    for (int i = 0; i < retorno.Length; i++)
                                    {
                                        if (retorno[i].number != "100")
                                            st.Append("Retorno da Natura : " + retorno[i].number + " - " + retorno[i].message).AppendLine();
                                    }
                                }

                                svcNatura.NotificarEmail(assunto, st, documento.Empresa.Configuracao.EmailsNotificacaoNatura, unitOfWork);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Problema ao emitir documentos da DT " + documento.NumeroDT + " da empresa" + documento.Empresa.CNPJ + ": " + ex.Message, "EmissaoNatura");
                }
            }
        }
    }
}