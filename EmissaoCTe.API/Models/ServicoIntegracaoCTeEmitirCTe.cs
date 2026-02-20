using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace EmissaoCTe.API
{
    public class ServicoIntegracaoCTeEmitirCte
    {
        private int Tempo = 5000; //5 segundos

        private static ServicoIntegracaoCTeEmitirCte Instance;
        private static Task Task;

        public static ServicoIntegracaoCTeEmitirCte GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoIntegracaoCTeEmitirCte();

            return Instance;
        }

        public void IniciarThread(int idEmpresa, string stringConexao)
        {
            if (Task == null)
            {
                Task = new Task(() =>
                {
                    while (true)
                    {
                        try
                        {
                            using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao))
                            {
                                VerificarIntegracoesPendentes(unidadeDeTrabalho);
                                unidadeDeTrabalho.Dispose();
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


        private void VerificarIntegracoesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork);
            List<Dominio.Entidades.IntegracaoCTe> listaIntegracoes = repIntegracaoCTe.BuscarIntegracoesFinalizadora("Y");

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            for (var i = 0; i < listaIntegracoes.Count(); i++)
            {
                try
                {
                    List<Dominio.Entidades.IntegracaoCTe> integracoesCarga = repIntegracaoCTe.BuscarIntegracoesPendentes(listaIntegracoes[i].NumeroDaCarga, listaIntegracoes[i].NumeroDaUnidade, "Y");
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = integracoesCarga.Select(o => o.CTe).ToList();

                    Servicos.Log.TratarErro("Quantidade de CTes: " + listaCTes.Count(),"FinalizacaoCarga");

                    unitOfWork.Start();
                    if (svcCTe.RatearValorFreteEntreCTes(listaCTes, listaIntegracoes[i].CTe.ValorFrete, Dominio.Enumeradores.TipoRateioTabelaFreteValor.Peso, unitOfWork))
                    {
                        unitOfWork.CommitChanges();

                        for (var j = 0; j < listaCTes.Count(); j++)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = listaCTes[j];
                            cte.TipoEnvio = 1;

                            if (svcCTe.Emitir(ref cte, unitOfWork))
                                if (!svcCTe.AdicionarCTeNaFilaDeConsulta(cte, unitOfWork))
                                    Servicos.Log.TratarErro("O CTe-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, não foi possível adicioná-lo na fila de consulta. ");
                        }

                    }
                    else
                        unitOfWork.Rollback();

                }
                catch (Exception ex)
                {
                    //Verificar para fazer aviso, e para salvar na tabela o erro

                    Servicos.Log.TratarErro("Problema ao ratear valores e emitir CTes carga: " + listaIntegracoes[i].NumeroDaCarga + ": " + ex.Message);

                    unitOfWork.Rollback();
                }
            }
        }

    }
}