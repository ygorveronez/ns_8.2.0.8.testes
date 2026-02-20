using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class ServicoEncerramentoMDFe
    {
        private static ServicoEncerramentoMDFe Instance;
        private static Task Task;

        public static ServicoEncerramentoMDFe GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoEncerramentoMDFe();

            return Instance;
        }

        public void IniciarThread(string stringConexao)
        {
            string servicoEncerramentoMDFe = System.Configuration.ConfigurationManager.AppSettings["ServicoEncerramentoMDFe"];

            if (!string.IsNullOrWhiteSpace(servicoEncerramentoMDFe))
            {
                if (Task == null)
                {
                    Task = new Task(() =>
                    {
                        while (true)
                        {
                            using Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

                            try
                            {
                                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                                Repositorio.IntegracaoMDFe repIntegracaoMDFe = new Repositorio.IntegracaoMDFe(unidadeDeTrabalho);
                                Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);

                                List<Dominio.Entidades.IntegracaoMDFe> integracoes = repIntegracaoMDFe.BuscarParaEncerramento(DateTime.Now);

                                foreach (Dominio.Entidades.IntegracaoMDFe integracao in integracoes)
                                {
                                    if (integracao.MDFe.Empresa.Configuracao == null || integracao.MDFe.Empresa.Configuracao.EncerramentoMDFeAutomatico != Dominio.Enumeradores.EncerramentoMDFeAutomatico.Nenhum)
                                    {
                                        integracao.MDFe.MunicipioEncerramento = repMunicipioDescarregamento.BuscarPrimeiroPorMDFe(integracao.MDFe.Codigo).Municipio;

                                        repIntegracaoMDFe.Atualizar(integracao);

                                        if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(integracao.MDFe.SistemaEmissor).EncerrarMdfe(integracao.MDFe.Codigo, integracao.MDFe.Empresa.Codigo, integracao.DataEncerramento.Value, unidadeDeTrabalho))
                                        {
                                            if (integracao.MDFe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                                                FilaConsultaCTe.GetInstance().QueueItem(5, integracao.MDFe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, stringConexao);
                                            svcMDFe.SalvarLogEncerramentoMDFe(integracao.MDFe.Chave, integracao.MDFe.Protocolo, integracao.DataEncerramento.Value, integracao.MDFe.Empresa, integracao.MDFe.Empresa.Localidade, "Encerramento automatico MDFe integrados com data de encerramento", unidadeDeTrabalho);
                                        }
                                    }
                                }

                                System.Threading.Thread.Sleep(900000);
                            }
                            catch (TaskCanceledException abort)
                            {
                                Servicos.Log.TratarErro(string.Concat("Task de encerramento de MDF-e cancelada: ", abort.ToString()));
                                break;
                            }
                            catch (System.Threading.ThreadAbortException abortThread)
                            {
                                Servicos.Log.TratarErro(string.Concat("Thread de encerramento de MDF-e cancelada: ", abortThread));
                                break;
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);

                                System.Threading.Thread.Sleep(900000);
                            }
                        }
                    });

                    Task.Start();
                }
            }
        }
    }
}