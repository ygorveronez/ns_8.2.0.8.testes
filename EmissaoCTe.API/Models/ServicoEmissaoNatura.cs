using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace EmissaoCTe.API
{
    public class ServicoEmissaoNatura
    {
        private int Tempo = 300000; //5 minutos

        private static ServicoEmissaoNatura Instance;
        private static Task Task;

        public static ServicoEmissaoNatura GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoEmissaoNatura();

            return Instance;
        }

        public void IniciarThread(int idEmpresa, string stringConexao)
        {
            if (Task == null)
            {
                Task = new Task(() =>
                {
                    if (!string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["IntervaloConsultasNatura"]))
                    {
                        int.TryParse(System.Configuration.ConfigurationManager.AppSettings["IntervaloConsultasNatura"], out Tempo);
                        Tempo = Tempo * 60000;
                    }

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
                                ConsultarDTsNatura(unidadeDeTrabalho);
                                EmitirDTsNatura(unidadeDeTrabalho);
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


        private void ConsultarDTsNatura(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            List<Dominio.Entidades.Empresa> listaEmpresas = repEmpresa.BuscarEmpresasEmissaoNaturaAutomatico();

            Servicos.Natura svcNatura = new Servicos.Natura(unitOfWork);

            for (var i = 0; i < listaEmpresas.Count(); i++)
            {
                try
                {
                    DateTime dataInicio = DateTime.Today;
                    DateTime dataFim = DateTime.Today.AddDays(1);
#if DEBUG
                    dataInicio = DateTime.Today.AddDays(-2);
#endif
                    svcNatura.ConsultarDocumentosTransporte(listaEmpresas[i].Codigo, 0, dataInicio, dataFim, unitOfWork, 0, false, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.AguardandoEmissaoAutomatica);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Problema ao consultar DTs da empresa" + listaEmpresas[i].CNPJ + ": " + ex.Message,"EmissaoNatura");
                }
            }
        }

        private void EmitirDTsNatura(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentoTransporteNatura repDT = new Repositorio.DocumentoTransporteNatura(unitOfWork);
            List<Dominio.Entidades.DocumentoTransporteNatura> listaDTs = repDT.BuscarPorSituacao(0, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.AguardandoEmissaoAutomatica);

            Servicos.Natura svcNatura = new Servicos.Natura(unitOfWork);
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            foreach (Dominio.Entidades.DocumentoTransporteNatura documento in listaDTs)
            {
                try
                {
                    documento.ValorFrete = documento.NotasFiscais.Sum(nf => nf.ValorFrete);
                    repDT.Atualizar(documento);

                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = svcNatura.EmitirCTes(documento.Codigo, "", unitOfWork);

                    if (ctes != null && ctes.Count() > 0)
                    {
                        foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                            svcCTe.AdicionarCTeNaFilaDeConsulta(cte, unitOfWork);

                        var notasPendentes = (from obj in documento.NotasFiscais where obj.Status == Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente select obj).ToList();
                        if (notasPendentes != null && notasPendentes.Count == 0)
                        {
                            documento.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.AguardandoRetornoAutomatico;
                            repDT.Atualizar(documento);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Problema ao emitir documentos da DT "+ documento.NumeroDT + " da empresa" + documento.Empresa.CNPJ + ": " + ex.Message, "EmissaoNatura");
                }
            }
        }
    }
}