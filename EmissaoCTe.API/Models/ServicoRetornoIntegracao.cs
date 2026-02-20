using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class ServicoRetornoIntegracao
    {
        private int Tempo = 10000; //10 segundos

        private static ServicoRetornoIntegracao Instance;
        private static Task Task;

        public static ServicoRetornoIntegracao GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoRetornoIntegracao();

            return Instance;
        }

        public void IniciarThread(string stringConexao)
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
                            using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                            {
                                VerificarRetornosCTe(unidadeDeTrabalho);
                                VerificarRetornosMDFe(unidadeDeTrabalho);
                                VerificarRetornosNFSe(unidadeDeTrabalho);
                            }

                            GC.Collect();

                            System.Threading.Thread.Sleep(Tempo);
                        }
                        catch (TaskCanceledException abort)
                        {
                            Servicos.Log.TratarErro(string.Concat("Task de finalização integracao retorno cancelada: ", abort.ToString()));
                            break;
                        }
                        catch (System.Threading.ThreadAbortException abortThread)
                        {
                            Servicos.Log.TratarErro(string.Concat("Task de finalização integracao retorno cancelada: ", abortThread));
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

        private void VerificarRetornosCTe(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.CTeIntegracaoRetorno repCTeIntegracaoRetorno = new Repositorio.CTeIntegracaoRetorno(unitOfWork);

            List<int> integracoesCTeRetorno = repCTeIntegracaoRetorno.BuscarPendentesPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog, 100);
            List<int> integracoesCTeRetornoEscrituracao = repCTeIntegracaoRetorno.BuscarPendentesPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao, 100);
            List<int> integracoesCTeRetornoMultiEmbarcador = repCTeIntegracaoRetorno.BuscarPendentesPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador, 100);

            integracoesCTeRetorno.AddRange(integracoesCTeRetornoEscrituracao);
            integracoesCTeRetorno.AddRange(integracoesCTeRetornoMultiEmbarcador);

            for (var i = 0; i < integracoesCTeRetorno.Count; i++)
            {
                var cteIntegracaoRetorno = repCTeIntegracaoRetorno.BuscarPorCodigo(integracoesCTeRetorno[i]);
                try
                {
                    if (cteIntegracaoRetorno.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogEscrituracao)
                        Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarCTeParaEscrituracao(ref cteIntegracaoRetorno, unitOfWork);
                    else if (cteIntegracaoRetorno.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog)
                        Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarRetornoCTe(ref cteIntegracaoRetorno, unitOfWork);
                    else if (cteIntegracaoRetorno.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador)
                        Servicos.IntegracaoMultiEmbarcadorTMS.IntegrarCTe(ref cteIntegracaoRetorno, unitOfWork);
                    else
                    {
                        cteIntegracaoRetorno.ProblemaIntegracao = "Tipo de integração não disponível";
                        cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    }

                    cteIntegracaoRetorno.DataIntegracao = DateTime.Now;
                    repCTeIntegracaoRetorno.Atualizar(cteIntegracaoRetorno);

                    unitOfWork.FlushAndClear();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Problema ao retornar integração " + cteIntegracaoRetorno?.Codigo + ": " + ex, "RetornoIntegracao");

                    cteIntegracaoRetorno.ProblemaIntegracao = "Problema ao retornar integração";
                    cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    cteIntegracaoRetorno.DataIntegracao = DateTime.Now;
                    repCTeIntegracaoRetorno.Atualizar(cteIntegracaoRetorno);
                }
            }

        }

        private void VerificarRetornosMDFe(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.MDFeIntegracaoRetorno repMDFeIntegracaoRetorno = new Repositorio.MDFeIntegracaoRetorno(unitOfWork);

            var integracoesMDFeRetorno = repMDFeIntegracaoRetorno.BuscarPendentes(100);

            string endPoint = System.Configuration.ConfigurationManager.AppSettings["URLMDFeYamalog"]; //"https://jpra-dev.yamaha-motor.com.br/ords/ym_jpra/multisoftware/jpraeventosconfirmacaolote";
            string urlToken = System.Configuration.ConfigurationManager.AppSettings["URLYokenYamalog"]; //"https://jpra-dev.yamaha-motor.com.br/ords/ym_jpra/oauth/token";
            string clientID = System.Configuration.ConfigurationManager.AppSettings["ClientIDYamalog"]; //"84EToR0YVkAbYhM60XOQ5A..";
            string clientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecretYamalog"]; //"Fassnv5au_yh8nW23dHXWQ..";


            for (var i = 0; i < integracoesMDFeRetorno.Count; i++)
            {
                var mdfeIntegracaoRetorno = repMDFeIntegracaoRetorno.BuscarPorCodigo(integracoesMDFeRetorno[i]);
                try
                {
                    if (mdfeIntegracaoRetorno.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog)
                        Servicos.Embarcador.Integracao.Magalog.IntegracaoMagalog.IntegrarRetornoMDFe(ref mdfeIntegracaoRetorno, unitOfWork);
                    else if (mdfeIntegracaoRetorno.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador)
                        Servicos.IntegracaoMultiEmbarcadorTMS.IntegrarMDFe(ref mdfeIntegracaoRetorno, unitOfWork);
                    else if (mdfeIntegracaoRetorno.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Yamalog)
                        Servicos.IntegracaoYamalog.IntegrarMDFe(ref mdfeIntegracaoRetorno, endPoint, urlToken, clientID, clientSecret, unitOfWork);
                    else
                    {
                        mdfeIntegracaoRetorno.ProblemaIntegracao = "Tipo de integração não disponível";
                        mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    }

                    mdfeIntegracaoRetorno.DataIntegracao = DateTime.Now;
                    repMDFeIntegracaoRetorno.Atualizar(mdfeIntegracaoRetorno);

                    unitOfWork.FlushAndClear();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Problema ao retornar integração " + mdfeIntegracaoRetorno?.Codigo + ": " + ex, "RetornoIntegracao");

                    mdfeIntegracaoRetorno.ProblemaIntegracao = "Problema ao retornar integração";
                    mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    mdfeIntegracaoRetorno.DataIntegracao = DateTime.Now;
                    repMDFeIntegracaoRetorno.Atualizar(mdfeIntegracaoRetorno);
                }
            }

        }

        private void VerificarRetornosNFSe(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.NFSeIntegracaoRetorno repNFSeIntegracaoRetorno = new Repositorio.NFSeIntegracaoRetorno(unitOfWork);

            List<int> integracoesNFSeRetorno = repNFSeIntegracaoRetorno.BuscarPendentesPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador, 100);

            for (var i = 0; i < integracoesNFSeRetorno.Count; i++)
            {
                var nfseIntegracaoRetorno = repNFSeIntegracaoRetorno.BuscarPorCodigo(integracoesNFSeRetorno[i]);
                try
                {
                    if (nfseIntegracaoRetorno.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador)
                        Servicos.IntegracaoMultiEmbarcadorTMS.IntegrarNFSe(ref nfseIntegracaoRetorno, unitOfWork);
                    else
                    {
                        nfseIntegracaoRetorno.ProblemaIntegracao = "Tipo de integração não disponível";
                        nfseIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    }

                    nfseIntegracaoRetorno.DataIntegracao = DateTime.Now;
                    repNFSeIntegracaoRetorno.Atualizar(nfseIntegracaoRetorno);

                    unitOfWork.FlushAndClear();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Problema ao retornar integração " + nfseIntegracaoRetorno?.Codigo + ": " + ex, "RetornoIntegracao");

                    nfseIntegracaoRetorno.ProblemaIntegracao = "Problema ao retornar integração";
                    nfseIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    nfseIntegracaoRetorno.DataIntegracao = DateTime.Now;
                    repNFSeIntegracaoRetorno.Atualizar(nfseIntegracaoRetorno);
                }
            }
        }

    }
}