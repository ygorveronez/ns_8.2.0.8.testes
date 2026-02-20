using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class Ocorrencia : LongRunningProcessBase<Ocorrencia>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Integracao.IntegracaoOcorrencia servicoIntegracaoOcorrencia = new Servicos.Embarcador.Integracao.IntegracaoOcorrencia(unitOfWork, _tipoServicoMultisoftware, cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoOcorrenciaCancelamento serIntegracaoOcorrenciaCancelamento = new Servicos.Embarcador.Integracao.IntegracaoOcorrenciaCancelamento(unitOfWork);
            Servicos.Embarcador.Integracao.IntegracaoEnvioProgramado servicoIntegracaoEnvioProgramado = new Servicos.Embarcador.Integracao.IntegracaoEnvioProgramado(unitOfWork);

            VerificarDocumentoComplementarPendenteEmissao(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware, _webServiceOracle);
            VerificarOcorrenciasEmCancelamento(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);

            await servicoIntegracaoOcorrencia.VerificarIntegracoesPendentesAsync(unitOfWork, _clienteMultisoftware, _clienteUrlAcesso, _tipoServicoMultisoftware);
            serIntegracaoOcorrenciaCancelamento.VerificarIntegracoesOcorrenciaPendentes(_tipoServicoMultisoftware, _clienteUrlAcesso);

            ProcessarOcorrenciaLote(unitOfWork, _auditado, _tipoServicoMultisoftware);
            BuscarAtendimentosComTemposExcendete(unitOfWork);
            VerificarGeracaoOcorrenciasPorMultaAtrasoRetirada(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware);

            servicoIntegracaoEnvioProgramado.VerificarIntegracaoesOcorrenciaPendentes();
        }

        #region Metodos Privados

        private void ProcessarOcorrenciaLote(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Ocorrencias.OcorrenciaLote repositorioOcorrenciaLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaLote(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Servicos.Embarcador.CargaOcorrencia.Ocorrencia servicoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

                List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote> ocorrenciaLotes = repositorioOcorrenciaLote.BuscarOcorrenciaLotePendentesGeracao();
                if (ocorrenciaLotes.Count == 0)
                    return;

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLote ocorrenciaLote in ocorrenciaLotes)
                    servicoOcorrencia.ProcessarOcorrenciaLote(ocorrenciaLote, configuracao, tipoServicoMultisoftware, _clienteMultisoftware, unitOfWork, auditado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void BuscarAtendimentosComTemposExcendete(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.NivelAtendimento repNivelAtendimento = new Repositorio.Embarcador.Chamados.NivelAtendimento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Servicos.Embarcador.Hubs.Chamado hubChamado = new Servicos.Embarcador.Hubs.Chamado();
            Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos repGatilhosMotivo = new Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos(unitOfWork);

            Servicos.Embarcador.Notificacao.Notificacao serNotificaocao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, _tipoServicoMultisoftware, "");
            Servicos.Embarcador.Chamado.Chamado serChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();

            int totalAtendimentosComNiveisExcedidos = repNivelAtendimento.CountBuscarNiveisAtendimentosExpirados();
            List<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento> listaAtendimentoComNiveisExcedidos;
            int skip = 0;
            int take = 10;

            while (skip < totalAtendimentosComNiveisExcedidos)
            {
                listaAtendimentoComNiveisExcedidos = repNivelAtendimento.BuscarNiveisAtendimentosExpirados(skip, take);

                if (listaAtendimentoComNiveisExcedidos.Count == 0)
                    break;

                foreach (Dominio.Entidades.Embarcador.Chamados.NivelAtendimento nivelAtendimento in listaAtendimentoComNiveisExcedidos)
                {
                    bool pasouDoTempo = DateTime.Compare(DateTime.Now, (nivelAtendimento.DataLimite ?? DateTime.MinValue)) > 0 ? true : false;

                    if (!pasouDoTempo)
                        continue;

                    serNotificaocao.GerarNotificacao(nivelAtendimento.Chamado.Autor, nivelAtendimento.Codigo, "Chamados/ChamadoOcorrencia", string.Format(Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciaExcedeuTempoLimiteNivelAtual, nivelAtendimento.Chamado.Numero), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta, _tipoServicoMultisoftware, unitOfWork);


                    //Atualizando situação para não ser gerada um nova notifição para este nivel.
                    nivelAtendimento.FoiNotificado = true;

                    if (configuracaoChamado.EscalarAutomaticamenteNivelExcederTempo)
                    {
                        Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);

                        EscalationList novoNivelEscalation = ObterProximoNivelAtendimento(nivelAtendimento.Nivel);
                        Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList gatilho = repGatilhosMotivo.BuscarNivelPorMotivoChamado(nivelAtendimento.Chamado?.MotivoChamado?.Codigo ?? 0, novoNivelEscalation);
                        Dominio.Entidades.Embarcador.Operacional.OperadorLogistica novoOperador = repOperadorLogistica.BuscarOperadorLogisticaPorNivelEscalation(novoNivelEscalation);

                        if (novoOperador == null)
                        {
                            Servicos.Log.TratarErro($"Notificação de e-mail não encontrou usuário com nível escalation compatível para chamado {nivelAtendimento.Chamado.Numero}");
                            continue;
                        }

                        if (gatilho != null)
                        {
                            nivelAtendimento.Nivel = novoNivelEscalation;
                            nivelAtendimento.Chamado.Nivel = novoNivelEscalation;
                            serChamado.EnviarEmailMudancaEscalationList(nivelAtendimento, novoOperador, _urlAcesso, unitOfWork);
                            nivelAtendimento.Chamado.Responsavel = novoOperador.Usuario;
                            nivelAtendimento.FoiNotificado = false;

                            if (gatilho.MotivoChamado.ConsiderarHorasDiasUteis)
                            {
                                nivelAtendimento.DataLimite = new Servicos.Embarcador.Chamado.Chamado(unitOfWork).AdicionarTempoDiaUtil(DateTime.Now, gatilho.Tempo);
                            }
                            else
                            {
                                nivelAtendimento.DataLimite = DateTime.Now.AddMinutes(gatilho.Tempo);
                            }
                        }

                        repChamado.Atualizar(nivelAtendimento.Chamado);
                        hubChamado.NotificarTempoExcedidoChamado(nivelAtendimento.Chamado);
                    }

                    repNivelAtendimento.Atualizar(nivelAtendimento);
                }

                skip += take;
            }

        }

        private void VerificarDocumentoComplementarPendenteEmissao(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceOracle)
        {
            unitOfWork.FlushAndClear();

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Servicos.Embarcador.Carga.Ocorrencia servicoCargaOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);

            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.VerificarDocumentoComplementarPendenteEmissao);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<int> codigoCargasOcorrencias  = servicoOrquestradorFila.Ordenar((limiteRegistros) => repositorioCargaOcorrencia.BuscarCodigosOcorrenciasEmEmissaoSituacaoDocumentosNaoGeradosNaoAptosParaAvancarEtapaOcorrencia(limiteRegistros));

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> cargasOcorrencias = repositorioCargaOcorrencia.BuscarPorCodigos(codigoCargasOcorrencias, false);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in cargasOcorrencias)
            {
                servicoCargaOcorrencia.ValidarEmissaoComplementosOcorrencia(ocorrencia, unitOfWork, webServiceConsultaCTe, tipoServicoMultisoftware, configuracao, true, _auditado, webServiceOracle, false, _clienteMultisoftware); // gera documentos 
                servicoCargaOcorrencia.ValidarEmissaoComplementosOcorrencia(ocorrencia, unitOfWork, webServiceConsultaCTe, tipoServicoMultisoftware, configuracao, false, _auditado, webServiceOracle, true, _clienteMultisoftware); // flega ocorrencias que geraram documentos 
            }

            cargasOcorrencias = repositorioCargaOcorrencia.BuscarOcorrenciasEmEmissaoSituacaoDocumentosGeradosAptosParaAvancarEtapaOcorrencia(5);
            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in cargasOcorrencias)
            {
                servicoCargaOcorrencia.ValidarEmissaoComplementosOcorrencia(ocorrencia, unitOfWork, webServiceConsultaCTe, tipoServicoMultisoftware, configuracao, false, _auditado, webServiceOracle, true, _clienteMultisoftware);// avança etapas da ocorrencia
            }

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> cargasOcorrenciasValorZerado = repositorioCargaOcorrencia.BuscarOcorrenciasEmEmissaoSituacao(30);
            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in cargasOcorrenciasValorZerado)
            {
                servicoCargaOcorrencia.ValidarEmissaoComplementosOcorrencia(ocorrencia, unitOfWork, webServiceConsultaCTe, tipoServicoMultisoftware, configuracao, true, _auditado, webServiceOracle, true, _clienteMultisoftware);
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Repositorio.Embarcador.Cargas.CargaComplementoFrete repCargaComplementoFrete = new Repositorio.Embarcador.Cargas.CargaComplementoFrete(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete> cargaComplementosFrete = repCargaComplementoFrete.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComplementoFrete.EmEmissaoCTeComplementar);
                Servicos.Embarcador.Carga.ComplementoFrete serComplementoFrete = new Servicos.Embarcador.Carga.ComplementoFrete(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete in cargaComplementosFrete)
                {
                    serComplementoFrete.ValidarEmissaoCargaComplementoFrete(cargaComplementoFrete, unitOfWork, webServiceConsultaCTe, tipoServicoMultisoftware);
                }
            }

            if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) || (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador))
            {
                Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
                List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete> fechamentos = repFechamentoFrete.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoFrete.EmEmissaoComplemento, 0, 2);
                foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento in fechamentos)
                {
                    new Servicos.Embarcador.Fechamento.FechamentoFrete(unitOfWork).ValidarEmissaoComplementosFechamentoFrete(fechamento, webServiceConsultaCTe, tipoServicoMultisoftware, webServiceOracle);
                }
            }
        }

        private void VerificarOcorrenciasEmCancelamento(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            unitOfWork.FlushAndClear();

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repositorioOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);
            List<int> codigosCancelamentosOcorrencia = repositorioOcorrenciaCancelamento.BuscarCancelamentosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.EmCancelamento, 100);
            Servicos.Embarcador.Carga.Ocorrencia servicoOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);

            for (int i = 0; i < codigosCancelamentosOcorrencia.Count; i++)
            {
                servicoOcorrencia.VerificarCancelamentoCTesComplementaresOcorrencia(codigosCancelamentosOcorrencia[i], unitOfWork, webServiceConsultaCTe, tipoServicoMultisoftware);
                unitOfWork.FlushAndClear();
            }
        }

        private void VerificarGeracaoOcorrenciasPorMultaAtrasoRetirada(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            unitOfWork.FlushAndClear();

            Servicos.Embarcador.GestaoPatio.MultaAtrasoRetirada servicoMultaAtrasoRetirada = new Servicos.Embarcador.GestaoPatio.MultaAtrasoRetirada(unitOfWork);

            servicoMultaAtrasoRetirada.VerificarGeracaoOcorrencias(tipoServicoMultisoftware, clienteMultisoftware);
        }

        private EscalationList ObterProximoNivelAtendimento(EscalationList escalationList)
        {
            switch (escalationList)
            {
                case EscalationList.Nivel1: return EscalationList.Nivel2;
                case EscalationList.Nivel2: return EscalationList.Nivel3;
                case EscalationList.Nivel3: return EscalationList.Nivel4;
                case EscalationList.Nivel4: return EscalationList.Nivel5;
                case EscalationList.Nivel5: return EscalationList.Nivel6;
                case EscalationList.Nivel6: return EscalationList.Nivel7;
                case EscalationList.Nivel7: return EscalationList.Nivel8;
                case EscalationList.Nivel8: return EscalationList.Nivel9;
                case EscalationList.Nivel9: return EscalationList.Nivel10;
                case EscalationList.Nivel10: return EscalationList.Nivel10;
                default: return escalationList;
            }
            ;
        }

        #endregion
    }
}