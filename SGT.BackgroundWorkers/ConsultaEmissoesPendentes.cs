using Dominio.Entidades.Embarcador.Ocorrencias;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 5000)]

    public class ConsultaEmissoesPendentes : LongRunningProcessBase<ConsultaEmissoesPendentes>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarCargasEmCancelamento(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.Cancelamento.VerificarCargasAgCancelamentoMDFes(unitOfWork, _tipoServicoMultisoftware, _auditado);
            Servicos.Embarcador.Carga.Cancelamento.VerificarCargasAgCancelamentoAverbacoesMDFes(unitOfWork, _tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.Cancelamento.VerificarCargasAgCancelamentoCTes(unitOfWork, _tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.Cancelamento.VerificarCargasAgCancelamentoAverbacoesCTes(unitOfWork, _tipoServicoMultisoftware);
            new Servicos.Embarcador.Carga.Cancelamento().VerificarCargasAgCancelamentoCIOT(unitOfWork, _tipoServicoMultisoftware, _auditado);
            Servicos.Embarcador.Carga.Cancelamento.VerificarCargasAgIntegracoesCancelamento(unitOfWork, _tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.Cancelamento.VerificarCargasEmFinalizacaoCancelamento(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware, _auditado);
            Servicos.Embarcador.Carga.Cancelamento.VerificarCargasReenvioCancelamentoCTe(unitOfWork, _tipoServicoMultisoftware);
            new Servicos.Embarcador.Carga.CargaDistribuidor(unitOfWork, _tipoServicoMultisoftware).VerificarCargasPendentesGerarDistribuidor();
            new Servicos.Embarcador.Carga.CargaEspelho(unitOfWork, _tipoServicoMultisoftware).GerarCargaEspelho();


            new Servicos.Embarcador.Carga.Cancelamento().VerificarCargasAgIntegracoesDadosCancelamento(unitOfWork, _tipoServicoMultisoftware);
            new Servicos.Embarcador.Carga.Retornos.RetornoCarga(unitOfWork).VerificarCargasPendenteRetorno(_tipoServicoMultisoftware, _clienteMultisoftware, _auditado);
            new Servicos.Embarcador.Carga.Cancelamento().VerificarCTeSemCargaAgCancelamento(unitOfWork);

            VerificarNFSManualEmCancelamento(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);
            VerificarFretesPendentesAposRecebimentoDasNotas(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);
            VerificarMDFEsEmEncerramento(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
            VerificarCTesPendentesParaGeracaoDeMovimentos(_stringConexao, _tipoServicoMultisoftware, unitOfWork);
            VerificarDocumentoComplementarPendenteIntegracaoGPA(_stringConexao, _tipoServicoMultisoftware, unitOfWork);

            Servicos.Embarcador.ProdutorRural.FechamentoProdutorRural.VerificarFechamentosCargaEmEmissao(_tipoServicoMultisoftware, unitOfWork, _stringConexao);

            VerificarRelatorioGerados(unitOfWork, _stringConexao);

            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
            serPedido.GerarCargasAguardandoGeracaoPreCarga(_stringConexao, _tipoServicoMultisoftware, unitOfWork);

            new Servicos.Embarcador.Carga.CargaAgrupada(unitOfWork).AjustarCargasAgrupadasAtualizadas();

            Servicos.Embarcador.CTe.CTeAgrupado.ProcessarCTesAgrupadosEmEmissao(_tipoServicoMultisoftware, unitOfWork);
            Servicos.Embarcador.CTe.CTeAgrupado.ProcessarCTesAgrupadosEmCancelamento(_tipoServicoMultisoftware, unitOfWork);

            Servicos.Embarcador.Integracao.IntegracaoCargaCTeAgrupado serIntegracaoCargaCTeAgrupado = new Servicos.Embarcador.Integracao.IntegracaoCargaCTeAgrupado(unitOfWork, _tipoServicoMultisoftware);
            serIntegracaoCargaCTeAgrupado.VerificarIntegracoesPendentes();

            ProcessarVerificaoDeOcorrenciasParaAprovacaoAutomatica(unitOfWork);

            LiberarAprovacoesAutomaticamente(unitOfWork, unitOfWorkAdmin, _tipoServicoMultisoftware);

            VerificarCargasComTempoAlertaTipoOperacaoAcimaDoTempo(unitOfWork, _tipoServicoMultisoftware);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            servicoCarga.VerificarSalvarDadostransporteCargaAutomaticamente(unitOfWork);

            servicoCarga.VerificarAlertaInicioViagemMotorista(_auditado, unitOfWork);
        }

        private void ProcessarVerificaoDeOcorrenciasParaAprovacaoAutomatica(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork).BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork).BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
            Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia servicoAutorizacaoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia(unitOfWork, _auditado, _tipoServicoMultisoftware, configuracaoOcorrencia, configuracaoChamado, configuracaoTMS, repositorioCargaOcorrenciaAutorizacao);
            servicoAutorizacaoOcorrencia.VerificarOcorrenciasParaAprovacaoAutomatica();
        }

        private void VerificarMDFEsEmEncerramento(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            unitOfWork.FlushAndClear();
            //unitOfWork.Dispose();
            //unitOfWork = null;
            //unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFeEmEncerramento = repCargaMDFe.BuscarPorEmEncerramento();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFeEncerramento in cargaMDFeEmEncerramento)
            {
                try
                {
                    unitOfWork.Start();
                    if (cargaMDFeEncerramento.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                    {
                        cargaMDFeEncerramento.EmEncerramento = false;
                        repCargaMDFe.Atualizar(cargaMDFeEncerramento);

                        Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaMDFeEncerramento.Carga;
                        serCarga.ValidarCargasFinalizadas(ref carga, tipoServicoMultisoftware, null, unitOfWork);
                    }
                    else
                    {
                        if (cargaMDFeEncerramento.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                        {
                            cargaMDFeEncerramento.EmEncerramento = false;
                            repCargaMDFe.Atualizar(cargaMDFeEncerramento);
                        }
                    }
                    unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    unitOfWork.Rollback();
                }
            }
        }

        private void LiberarAprovacoesAutomaticamente(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);

            /// Dúvida: Essas configurações fariam sentido ir para um Cache? São muito acessadas e pouco alteradas?
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork).BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork).BuscarConfiguracaoPadrao();

            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, IdentificadorControlePosicaoThread.VerificarCargasOcorrenciaAutorizacaoPendentes);
            List<int> listaCodigosOcorrenciasParaAprovacao = servicoOrquestradorFila.Ordenar((limiteRegistros) =>
                repCargaOcorrenciaAutorizacao.BuscarPendentesAprovacaoAutomatica(DateTime.Now.Date, 0, limiteRegistros)
            );

            unitOfWork.Start();

            Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia servicoAutorizacaoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.AutorizacaoCargaOcorrencia(unitOfWork, _auditado, tipoServicoMultisoftware, configuracaoOcorrencia, configuracaoChamado, configuracaoTMS, repCargaOcorrenciaAutorizacao);

            for (var i = 0; i < listaCodigosOcorrenciasParaAprovacao.Count; i++)
            {
                var codigoOcorrenciaAutorizacao = listaCodigosOcorrenciasParaAprovacao[i];
                try
                {
                    /// TODO (LEONARDO CENTENARO) Testar a performance com o repCargaOcorrenciaAutorizacao.BuscarPorCodigoComFetch
                    CargaOcorrenciaAutorizacao ocorrenciaAutorizacaoCompleta = repCargaOcorrenciaAutorizacao.BuscarPorCodigo(codigoOcorrenciaAutorizacao);

                    servicoAutorizacaoOcorrencia.EfetuarAprovacao(ocorrenciaAutorizacaoCompleta, null);
                    servicoAutorizacaoOcorrencia.VerificarSituacaoOcorrencia(ocorrenciaAutorizacaoCompleta.CargaOcorrencia, null, null, 0, 0, null);

                    servicoOrquestradorFila.RegistroLiberadoComSucesso(codigoOcorrenciaAutorizacao);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    servicoOrquestradorFila.RegistroComFalha(codigoOcorrenciaAutorizacao, excecao.Message);
                }
            }
            unitOfWork.CommitChanges();
        }

        private void VerificarRelatorioGerados(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            try
            {
                //unitOfWork.Dispose();
                //unitOfWork = null;
                //unitOfWork = new Repositorio.UnitOfWork(stringConexao);
                unitOfWork.FlushAndClear();

                int diasParaExclusao = -4;
#if DEGUG
            diasParaExclusao = -1;
#endif
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repositorioRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> relatorioGerados = repositorioRelatorioControleGeracao.BuscarGeradosAnterioresAData(DateTime.Now.AddDays(diasParaExclusao));

                foreach (Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioGerado in relatorioGerados)
                {
                    if (relatorioGerado.SituacaoGeracaoRelatorio == SituacaoGeracaoRelatorio.Gerado)
                        servicoRelatorio.ExcluirArquivoRelatorio(relatorioGerado, unitOfWork);

                    repositorioRelatorioControleGeracao.Deletar(relatorioGerado);
                }

                List<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> relatoriosEmExecucao = repositorioRelatorioControleGeracao.BuscarRelatoriosEmExecucao();

                foreach (Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioEmExecucao in relatoriosEmExecucao)
                {
                    if (relatorioEmExecucao.Relatorio.TimeOutMinutos > 0 && relatorioEmExecucao.DataInicioGeracao < DateTime.Now.AddMinutes(-relatorioEmExecucao.Relatorio.TimeOutMinutos))
                        servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioEmExecucao, unitOfWork, "A geração do relatório excedeu o tempo limite pré determinado de " + relatorioEmExecucao.Relatorio.TimeOutMinutos + " minutos.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
            }
        }

        private void VerificarNFSManualEmCancelamento(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            unitOfWork.FlushAndClear();

            Repositorio.Embarcador.NFS.NFSManualCancelamento repNFSManualCancelamento = new Repositorio.Embarcador.NFS.NFSManualCancelamento(unitOfWork);

            List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> cancelamentos = repNFSManualCancelamento.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.EmCancelamento, 0, 10);
            Servicos.Embarcador.NFSe.NFSManual serNFSManual = new Servicos.Embarcador.NFSe.NFSManual(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento cancelamento in cancelamentos)
                serNFSManual.VerificarNFSEmCancelamento(cancelamento, webServiceConsultaCTe, tipoServicoMultisoftware, unitOfWork);
        }

        private void VerificarCargasEmCancelamento(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            //Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            List<int> cargasEmCancelamento = repCargaCancelamento.BuscarCodigosPorSituacaoETipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento);

            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            foreach (int codigoCargaCancelamento in cargasEmCancelamento)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                    //if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Cancelamento)
                    //{
                    //Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, stringConexao, tipoServicoMultisoftware);
                    Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCargaNovo(cargaCancelamento, unitOfWork, tipoServicoMultisoftware);
                    //}
                    //else if (cargaCancelamento.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoCarga.Anulacao)
                    //    Servicos.Embarcador.Carga.Cancelamento.AnularCarga(ref cargaCancelamento, unitOfWork, stringConexao, tipoServicoMultisoftware);
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }
            }
        }

        private void VerificarFretesPendentesAposRecebimentoDasNotas(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                //unitOfWork.Dispose();
                //unitOfWork = null;
                //unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                try
                {
                    unitOfWork.FlushAndClear();

                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                    Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                    Servicos.Embarcador.Carga.Frete serCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, tipoServicoMultisoftware);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarCargaPendentesCalculoFreteAposRecebimentoNFe(0, 2);
                    if (cargas.Count > 0)
                    {
                        Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                        Servicos.Embarcador.Carga.CTe serCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);

                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                        {
                            unitOfWork.Start();

                            carga.DataInicioCalculoFrete = DateTime.Now;
                            carga.CalculandoFrete = true;

                            repCarga.Atualizar(carga);

                            unitOfWork.CommitChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                //finally
                //{
                //    unitOfWork.Dispose();
                //}
            }

        }

        private void VerificarCTesPendentesParaGeracaoDeMovimentos(string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();
            //unitOfWork.Dispose();
            //unitOfWork = null;
            //unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Carga.Documentos svcDocumentos = new Servicos.Embarcador.Carga.Documentos(unitOfWork);
            Servicos.Embarcador.Carga.Cancelamento svcCancelamento = new Servicos.Embarcador.Carga.Cancelamento();
            Repositorio.Embarcador.CTe.ControleGeracaoMovimentoCTeManual repControleGeracaoMovimento = new Repositorio.Embarcador.CTe.ControleGeracaoMovimentoCTeManual(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.CTe.ControleGeracaoMovimentoCTeManual> ctesParaGeracao = repControleGeracaoMovimento.BuscarParaGeracao(0, 10);

            string erro = string.Empty;

            //try
            //{
            foreach (Dominio.Entidades.Embarcador.CTe.ControleGeracaoMovimentoCTeManual cteParaGeracao in ctesParaGeracao)
            {
                unitOfWork.Start();

                if (cteParaGeracao.CargaCTe.CTe.Status == "A")
                {
                    svcDocumentos.GerarTituloGNRECTeManual(cteParaGeracao.CargaCTe, unitOfWork, tipoServicoMultisoftware);
                    svcDocumentos.GerarMovimentoEmissaoCTe(cteParaGeracao.CargaCTe, tipoServicoMultisoftware, unitOfWork, false);

                    Servicos.Log.GravarInfo($"VerificarCTesPendentesParaGeracaoDeMovimentos inserindo documento faturamento - Carga {cteParaGeracao?.CargaCTe?.CargaOrigem?.Codigo ?? 0} -  CTe {cteParaGeracao?.CargaCTe?.CTe?.Codigo ?? 0}", "ControleDocumentoFaturamento");
                    Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorDocumento(cteParaGeracao.CargaCTe.CargaOrigem, cteParaGeracao.CargaCTe.CTe, null, null, null, null, false, false, false, configuracao, unitOfWork, tipoServicoMultisoftware);

                    Servicos.Embarcador.Escrituracao.DocumentoEscrituracao.AdicionarDocumentoParaEscrituracao(cteParaGeracao.CargaCTe, unitOfWork);
                }
                else if (cteParaGeracao.CargaCTe.CTe.Status == "C")
                {
                    if (!Servicos.Embarcador.Carga.Cancelamento.GerarMovimentoCancelamentoCTe(out erro, cteParaGeracao.CargaCTe, tipoServicoMultisoftware, unitOfWork, stringConexao))
                        throw new Exception(erro);

                    if (!Servicos.Embarcador.Carga.Cancelamento.ReverterItensEmAbertoAposCancelamentoCTe(out erro, cteParaGeracao.CargaCTe, tipoServicoMultisoftware, unitOfWork))
                        throw new Exception(erro);

                    Servicos.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento.AdicionarDocumentoParaEscrituracao(cteParaGeracao.CargaCTe, unitOfWork);
                }

                repControleGeracaoMovimento.Deletar(cteParaGeracao);

                unitOfWork.CommitChanges();
            }
            //}
            //catch
            //{
            //    unitOfWork.Rollback();
            //    throw;
            //}

            //unitOfWork.Dispose();
        }

        private void VerificarDocumentoComplementarPendenteIntegracaoGPA(string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho)
        {
            //Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(stringConexao);
            try
            {
                unidadeTrabalho.FlushAndClear();

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unidadeTrabalho);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repCargaOcorrencia.BuscarParaIntegracaoGPA();
                //Servicos.Log.TratarErro(ocorrencias.Count().ToString() + " pendentes para integrar.", "IntegracaoGPA");

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in ocorrencias)
                {
                    //unidadeTrabalho.Start();

                    Servicos.Log.TratarErro("Integrando NFSe ocorrência número " + ocorrencia.NumeroOcorrencia, "IntegracaoGPA");

                    Dominio.Entidades.Empresa empresa = ocorrencia.ObterEmitenteOcorrencia();
                    string endpoint = empresa?.EndpointIntegracaoGPA;

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> complementos = repCargaCTeComplementoInfo.BuscarPreCTesPorOcorrenciaPendenteIntegracapGPA(ocorrencia.Codigo);
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao aprovacao = repCargaOcorrenciaAutorizacao.BuscarUltimoAprovadorOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia);

                    // Se nao ha configuracao de integracao, pula ocorrencia
                    if (string.IsNullOrWhiteSpace(endpoint))
                    {
                        Servicos.Log.TratarErro("Ocorrência número " + ocorrencia.NumeroOcorrencia + ": Nenhum Endpoint configurado para a empresa.", "IntegracaoGPA");
                        ocorrencia.MensagemPendencia = "Nenhum Endpoint configurado para a empresa.";
                        ocorrencia.IntegradoComGPA = true;
                        ocorrencia.ErroIntegracaoComGPA = false;
                    }
                    else if (aprovacao == null)
                    {
                        Servicos.Log.TratarErro("Ocorrência número " + ocorrencia.NumeroOcorrencia + ": Nenhuma aprovação encontrada para gerar dados.", "IntegracaoGPA");
                        ocorrencia.MensagemPendencia = "Nenhuma aprovação encontrada para gerar dados.";
                        ocorrencia.IntegradoComGPA = true;
                        ocorrencia.ErroIntegracaoComGPA = false;
                    }
                    else if (empresa == null)
                    {
                        Servicos.Log.TratarErro("Ocorrência número " + ocorrencia.NumeroOcorrencia + ": Empresa não encontrada.", "IntegracaoGPA");
                        ocorrencia.MensagemPendencia = "Empresa não encontrada.";
                        ocorrencia.IntegradoComGPA = true;
                        ocorrencia.ErroIntegracaoComGPA = false;
                    }
                    else
                    {
                        bool todosComplementosIntegrados = true;
                        int totalDocumentosNaoIntegrados = 0;

                        Servicos.Log.TratarErro(complementos.Count().ToString() + " complementos para integrar", "IntegracaoGPA");

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complemento in complementos)
                        {
                            Servicos.Log.TratarErro("Integrando CargaCTeComplementoInfo codigo + " + complemento.Codigo, "IntegracaoGPA");

                            // Objeto de requisicao do GPA
                            Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EmitirNfse dadosNFSe = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EmitirNfse()
                            {
                                tipoMensagem = "emitirNfse",
                                Dados = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EmitirNfseDados()
                                {
                                    cnpjPrestador = empresa.CNPJ_SemFormato,
                                    numeroRpsOriginal = complemento.CargaCTeComplementado?.CTe?.RPS?.Numero.ToString() ?? "0",
                                    serieRPSOriginal = complemento.CargaCTeComplementado?.CTe?.RPS?.Serie ?? "0", // se for 0 enviar E
                                    valorServico = complemento.ValorComplemento.ToString().Replace(",", "."),
                                    nomeUsuarioAprov = aprovacao.Usuario.Nome,
                                    dataUsuarioAprov = aprovacao.Data?.ToString("dd/MM/yyyy") ?? string.Empty,
                                    horaUsuarioAprov = aprovacao.Data?.ToString("HH:mm") ?? string.Empty,
                                    numeroOcorrencia = ocorrencia.NumeroOcorrencia.ToString()
                                }
                            };
                            if (dadosNFSe.Dados.serieRPSOriginal == "0")
                                dadosNFSe.Dados.serieRPSOriginal = "E";
#if DEBUG
                            dadosNFSe.Dados.numeroRpsOriginal = "999999";
#endif

                            if (!Servicos.Embarcador.Integracao.GPA.IntegracaoGPA.EmitirNFSe(endpoint, dadosNFSe, unidadeTrabalho, out string erro))
                            {
                                Servicos.Log.TratarErro("CargaCTeComplementoInfo não integrada", "IntegracaoGPA");
                                // Não integrado
                                todosComplementosIntegrados = false;
                                totalDocumentosNaoIntegrados++;
                                complemento.MensagemPendencia = erro;
                                complemento.ErroIntegracaoComGPA = true;
                            }
                            else
                            {
                                Servicos.Log.TratarErro("CargaCTeComplementoInfo integrada", "IntegracaoGPA");

                                complemento.ErroIntegracaoComGPA = false;
                                complemento.IntegradoComGPA = true;
                            }

                            repCargaCTeComplementoInfo.Atualizar(complemento);
                        }


                        // Atualiza Ocorrencia
                        if (!todosComplementosIntegrados)
                        {
                            Servicos.Log.TratarErro(totalDocumentosNaoIntegrados.ToString() + " / " + complementos.Count + " não foram integrado(s)", "IntegracaoGPA");

                            ocorrencia.MensagemPendencia = totalDocumentosNaoIntegrados.ToString() + "/" + complementos.Count + " não foram integrado(s).";
                            ocorrencia.IntegradoComGPA = false;
                            ocorrencia.ErroIntegracaoComGPA = true;
                        }
                        else
                        {
                            Servicos.Log.TratarErro("Ocorrência número " + ocorrencia.NumeroOcorrencia + " integrada com sucesso.", "IntegracaoGPA");

                            ocorrencia.MensagemPendencia = string.Empty;
                            ocorrencia.IntegradoComGPA = true;
                            ocorrencia.ErroIntegracaoComGPA = false;
                        }
                    }

                    repCargaOcorrencia.Atualizar(ocorrencia);

                    //unidadeTrabalho.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoGPA");
                //unidadeTrabalho.Rollback();
                throw;
            }
            //finally
            //{
            //    unidadeTrabalho.Dispose();
            //}
        }

        private void VerificarCargasComTempoAlertaTipoOperacaoAcimaDoTempo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfiguracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfiguracaoEmail.BuscarEmailEnviaDocumentoAtivo();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasComTempoTipoDeOperacaoAlertaEmailExcedente();

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    string mensagemEmail = $"A carga ({carga.CodigoCargaEmbarcador}) está a mais de {(carga.DataCriacaoCarga.AddDays(-carga.TipoOperacao.ConfiguracaoCarga.TempoParaAlertarPorEmailResponsavelDaFilialDaCargaQueAindaNaoTeveOsDadosDeTransporteInformados)).Hour} horas aguardando o checkin, por favor verifique.";
                    string assuntoEmail = $"Carga ({carga.CodigoCargaEmbarcador}) está aguardando o Checkin";

                    Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, carga.Filial.Email, null, null, assuntoEmail, mensagemEmail, configuracaoEmail.Smtp, out string mensagemErro, configuracaoEmail.DisplayEmail, null, "", configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, unitOfWork);

                    carga.DataEnvioEmailTipoOperacaoAvisoResponsavelFilial = DateTime.Now;

                    repositorioCarga.Atualizar(carga);
                }

                unitOfWork.CommitChanges();
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }
    }
}