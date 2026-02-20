using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 1000)]

    public class ProcessarDocumentoTransporte : LongRunningProcessBase<ProcessarDocumentoTransporte>
    {
        #region Metodos Privados


        private void ProcessarCargasPendenteIntegracao(Repositorio.UnitOfWork unitOfWork, long codigoPedidoIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);
            Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integracao = repIntegradoraIntegracaoRetorno.BuscarPorCodigo(codigoPedidoIntegracao);

            try
            {
                if (integracao != null)
                {
                    if (integracao.ArquivoRequisicao == null)
                    {
                        integracao.Mensagem = "Arquivo de requisição não encontrado";
                        integracao.Situacao = SituacaoIntegracao.ProblemaIntegracao;
                        repIntegradoraIntegracaoRetorno.Atualizar(integracao);

                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarRertornoCarga(integracao);
                    }
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                        auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;
                        auditado.Integradora = integracao.Integradora;

                        string request = Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(integracao.ArquivoRequisicao);
                        Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporte = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte>(request);
                        
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaProcessar = new Repositorio.Embarcador.Cargas.Carga(unitOfWork).BuscarPorProtocoloFetchProcessarDocumento(integracao.Carga?.Codigo ?? 0);

                        string mensagem = string.Empty;

                        unitOfWork.Start();

                        Dominio.Entidades.Embarcador.Cargas.Carga carga = Servicos.Embarcador.Carga.DocumentoTransporte.ProcessarDocumentoTransporte.GerarCargaPorDocumentoTransporte(cargaProcessar, documentoTransporte, auditado, tipoServicoMultisoftware, _clienteUrlAcesso, _clienteMultisoftware, _stringConexaoAdmin, unitOfWork, configuracaoTMS);

                        Servicos.Log.TratarErro($"Finalizando e comitando GerarCargaPorDocumentoTransporte, referente a carga código: {integracao.Carga?.Codigo ?? 0}", "GerarCargaPorDocumentoTransporte_Rastreio");

                        integracao.Mensagem = "Carga integrada com sucesso.";
                        integracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        integracao.Sucesso = true;
                        repIntegradoraIntegracaoRetorno.Atualizar(integracao);

                        unitOfWork.CommitChanges();

                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarRertornoCarga(integracao);
                        new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(unitOfWork, TipoServicoMultisoftware.MultiEmbarcador).AdicionarIntegracaoIndividual(carga, EtapaCarga.SalvarDadosTransporte, $"SalvarDT Processado", new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal }, true);

                    }
                }
                unitOfWork.FlushAndClear();
            }
            catch (ServicoException se)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(se, "GerarCargaPorDocumentoTransporte_Rastreio");

                unitOfWork.Start();

                integracao.Sucesso = false;
                integracao.Mensagem = se.Message;
                integracao.Situacao = SituacaoIntegracao.ProblemaIntegracao;
                repIntegradoraIntegracaoRetorno.Atualizar(integracao);

                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();

                new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarRertornoCarga(integracao);
                new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(unitOfWork, TipoServicoMultisoftware.MultiEmbarcador).AdicionarIntegracaoIndividual(integracao.Carga, EtapaCarga.SalvarDadosTransporte, $"{ se.Message }", new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                unitOfWork.Start();
                integracao.Sucesso = false;
                integracao.Mensagem = "Problema ao tentar processar a integração";
                integracao.Situacao = SituacaoIntegracao.ProblemaIntegracao;

                repIntegradoraIntegracaoRetorno.Atualizar(integracao);

                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();

                new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarRertornoCarga(integracao);
                new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(unitOfWork, TipoServicoMultisoftware.MultiEmbarcador).AdicionarIntegracaoIndividual(integracao.Carga, EtapaCarga.SalvarDadosTransporte, $"Problemas no processamento do Salvar DT.", new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal });
            }
        }

        private void VerficarCargasPendenteIntegracao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Servicos.Log.TratarErro("Inicio Buscando Cargas pendentes", "IntegracaoCargaUnilever");

                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);
                List<long> listaIntegracoesAguardando = repIntegradoraIntegracaoRetorno.BuscarCargasPendenteIntegracao(10);

                //List<long> listaIntegracoesAguardando = new List<long>();
                //listaIntegracoesAguardando.Add(66068791);

                Servicos.Log.TratarErro("Cargas pendentes: " + listaIntegracoesAguardando.Count + "", "IntegracaoCargaUnilever");

                if (listaIntegracoesAguardando != null && listaIntegracoesAguardando.Count > 0)
                {
                    foreach (var codigoIntegracao in listaIntegracoesAguardando)
                        ProcessarCargasPendenteIntegracao(unitOfWork, codigoIntegracao, tipoServicoMultisoftware);
                }

                Servicos.Log.TratarErro("Fim Cargas pendentes", "IntegracaoCargaUnilever");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        #endregion

        #region Metodos Publicos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerficarCargasPendenteIntegracao(unitOfWork, _tipoServicoMultisoftware);
        }

        #endregion
    }
}