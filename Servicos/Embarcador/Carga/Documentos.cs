using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Carga
{
    public class Documentos : ServicoBase
    {
        public Documentos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        private int TempoLimiteParEmissaoEmMinutos = 25;

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.Carga VerificarPendeciasEmissaoDocumentosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, int codEmpresaPai, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Repositorio.UnitOfWork unitOfWork)
        {
            bool emitirMDFePeloMultiCTe = true;
            bool emitiuTodosCte = true;
            bool problemaEmissao = false;
            bool excedeuTempoLimiteEnviado = false;

            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.CTe serCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaNFS repCargaNFS = new Repositorio.Embarcador.Cargas.CargaNFS(unitOfWork);
            Repositorio.NFSe repNFSs = new Repositorio.NFSe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega repFluxoColetaEntrega = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega = repFluxoColetaEntrega.BuscarPorCarga(carga.Codigo);
            //int codigoEmpresa = carga.Empresa.Codigo;

            bool ctesSubContratacaoFilialEmissora = false;
            bool pendenciaTempoNFSe = false;
            if (carga.EmpresaFilialEmissora != null && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                ctesSubContratacaoFilialEmissora = true; //codigoEmpresa = carga.EmpresaFilialEmissora.Codigo;

            Servicos.Embarcador.Carga.Carga serCarga = new Carga(unitOfWork);
            Servicos.Embarcador.Carga.CTe serCargaCTe = new CTe(unitOfWork);

            if (repCargaCte.ContarCTePorListaSituacao(carga.Codigo, ctesSubContratacaoFilialEmissora, new string[] { "R", "D", "I", "L" }) > 0)
            {
                new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(unitOfWork, tipoServicoMultisoftware).AdicionarIntegracaoIndividual(carga, EtapaCarga.NotaFiscal, "Problema na emissão do(s) CT-e(s).", new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal });

                carga.MotivoPendencia = "Problema na emissão do(s) CT-e(s)";
                problemaEmissao = true;
                carga.problemaCTE = true;
            }

            if (repCargaCte.ContarCTeEnviadoSemCodigoOracle(carga.Codigo) > 0)
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCargaCte.ObterCTeEnviadoSemCodigoOracle(carga.Codigo);
                for (int i = 0; i < ctes.Count; i++)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[i];
                    cte.Status = "R";
                    cte.MensagemRetornoSefaz = "888 - Falha ao conectar com o Sefaz.";
                    repCTe.Atualizar(cte);
                }
            }

            if (repCargaCte.ContarCTePorSituacaoDiff(carga.Codigo, ctesSubContratacaoFilialEmissora, new string[] { "A", "C", "K", "N", "F", "Z" }) > 0)
                emitiuTodosCte = false;

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesExcederamLimiteTempo = null;
            if (carga.ControlaTempoParaEmissao)
            {
                ctesExcederamLimiteTempo = repCargaCte.BuscarCTesPorCargaETempoLimiteEmissao(carga.Codigo, ctesSubContratacaoFilialEmissora, BuscarTempoLimiteEmissao());
                if (ctesExcederamLimiteTempo.Count > 0)
                {
                    problemaEmissao = true;
                    carga.problemaCTE = true;
                    excedeuTempoLimiteEnviado = true;

                    pendenciaTempoNFSe = ((from obj in ctesExcederamLimiteTempo where obj.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe select obj).Count() > 0);
                }
            }

            carga.ContingenciaFSDA = repCargaCte.ContarCTePorSituacao(carga.Codigo, "F") > 0;

            if (!problemaEmissao)
            {
                if (emitiuTodosCte)
                {
                    if (carga.EmitindoCRT)
                    {
                        //RETORNA A CARGA PARA ETAPA DE NOTA PARA VINCULAR XMLs E EMITIR O CTE
                        carga.EmitindoCRT = false;
                        carga.SituacaoCarga = SituacaoCarga.AgNFe;
                        repCargaPedido.InformarCTesNaoEmitidos(carga.Codigo);
                        carga.CTesEmDigitacao = !serCTe.VerificarSePodeEmitirAutomaticamente(tipoServicoMultisoftware, carga, configuracaoEmbarcador.AtivarAutorizacaoAutomaticaDeTodasCargas);
                        repCarga.Atualizar(carga);
                        serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                        return carga;
                    }

                    if (carga.RealizandoOperacaoContainer)
                    {
                        if (AverbarCTeCarga(carga, configuracaoEmbarcador, unitOfWork, true) && IntegrouValePedagioCarga(carga, unitOfWork, configuracaoEmbarcador) && ValidarGerouAdiantamentoPagamentoTerceiroContainer(carga))
                        {
                            carga.RealizandoOperacaoContainer = false;
                            carga.SituacaoCarga = SituacaoCarga.AgNFe;
                            carga.LiberarComProblemaAverbacao = false;
                            carga.LiberadoComProblemaValePedagio = false;
                            carga.problemaAverbacaoCTe = false;

                            carga.CTesEmDigitacao = !serCTe.VerificarSePodeEmitirAutomaticamente(tipoServicoMultisoftware, carga, configuracaoEmbarcador.AtivarAutorizacaoAutomaticaDeTodasCargas);
                            repCarga.Atualizar(carga);
                        }
                        return carga;
                    }

                    if (carga.EmpresaFilialEmissora != null && carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    {
                        if (fluxoColetaEntrega != null && !fluxoColetaEntrega.DataEmissaoCTeSubContratacao.HasValue)
                            Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTeSubcontratacao, unitOfWork);
                    }
                    else
                    {
                        if (fluxoColetaEntrega != null && !fluxoColetaEntrega.DataEmissaoCTe.HasValue && !carga.AgImportacaoCTe)
                            Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.CTe, unitOfWork);
                    }

                    bool finalizarEmissoes = true;
                    bool possuiMDFe = Servicos.Embarcador.Carga.Carga.VerificarSePossuiMDFe(carga, unitOfWork, tipoServicoMultisoftware);
                    // Segunda parte de emissão:
                    // Após emitir todos documentos principais (CTe/NFSe)
                    // Inicia-se o processo segundario (Averbação dos CTes)
                    // Só depois de todos CT-es emitidos e todas aberbações processadas
                    // É iniciado a emissão de MDF-e
                    if (!EmitirNFeRemessa(carga, configuracaoEmbarcador, unitOfWork))
                        finalizarEmissoes = false;
                    else if (!AverbarCTeCarga(carga, configuracaoEmbarcador, unitOfWork))
                        finalizarEmissoes = false;
                    else if (!IntegrouValePedagioCarga(carga, unitOfWork, configuracaoEmbarcador))
                        finalizarEmissoes = false;
                    else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && !IntegrouPagamentoMotoristaCarga(carga, unitOfWork, configuracaoEmbarcador))
                        finalizarEmissoes = false;
                    else if (!IntegrouCIOTCarga(carga, possuiMDFe, tipoServicoMultisoftware, unitOfWork, configuracaoEmbarcador))
                        finalizarEmissoes = false;
                    else if (!IntegrouGNRE(carga, unitOfWork, configuracaoEmbarcador))
                        finalizarEmissoes = false;
                    else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !Integracao.Diaria.DiariaMotorista.GerarPagamentoMotoristaTerceiro(carga, unitOfWork))
                        finalizarEmissoes = false;
                    else if (possuiMDFe)
                    {
                        if (!EmissaoMDFeCarga(ref excedeuTempoLimiteEnviado, emitirMDFePeloMultiCTe, carga, tipoServicoMultisoftware, configuracaoEmbarcador, codEmpresaPai, webServiceConsultaCTe, unitOfWork))
                            finalizarEmissoes = false;
                        else if (!AverbarMDFeCarga(carga, configuracaoEmbarcador, unitOfWork))
                            finalizarEmissoes = false;
                    }

                    if (finalizarEmissoes)
                    {
                        if (!possuiMDFe)
                        {
                            if (fluxoColetaEntrega != null && !fluxoColetaEntrega.DataEmissaoMDFe.HasValue)
                                Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.MDFe, unitOfWork);
                        }

                        FinalizarEmissoes(ref carga, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, Auditado);
                    }
                }
            }
            else
            {
                if (excedeuTempoLimiteEnviado)
                {
                    if (!carga.PossuiPendencia && !pendenciaTempoNFSe)
                    {
                        System.Text.StringBuilder stBuilder = MontaHTMLCTeParado(carga.RetornarCodigoCargaParaVisualizacao, ctesExcederamLimiteTempo);

                        if (carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                        {
                            //Task.Factory.StartNew(() => enviarEmailDocumentosParadosNaFilaDeEnvio(stBuilder.ToString(), "CT-e parado na fila de envio.", unitOfWork));
                            string assunto = "CT-e parado na fila de envio.";
                            string ambiente = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().IdentificacaoAmbiente;
                            if (!string.IsNullOrWhiteSpace(ambiente))
                                assunto = "CT-e parado na fila de envio - " + ambiente + ".";

                            enviarEmailDocumentosParadosNaFilaDeEnvio(stBuilder.ToString(), assunto, unitOfWork);
                        }

                        if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                            Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarSituacaoDiversas(carga.CodigoCargaEmbarcador, "warning", 8888, "Sefaz indisponivel temporariamente", "Sefaz", unitOfWork);


#if DEBUG
                        //Task.Factory.StartNew(() => enviarEmailDocumentosParadosNaFilaDeEnvio(stBuilder.ToString(), "CT-e parado na fila de envio.", unitOfWork));
                        enviarEmailDocumentosParadosNaFilaDeEnvio(stBuilder.ToString(), "CT-e parado na fila de envio.", unitOfWork);
#endif
                    }

                    if (!pendenciaTempoNFSe)
                        carga.MotivoPendencia = "Sefaz não está respondendo ao envio do(s) CT-e(s) ";
                    else
                        carga.MotivoPendencia = "Prefeitura não está respondendo ao envio da(s) NFS-e(s), favor aguardar";

                    if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Piracanjuba) != null)
                        new Servicos.Embarcador.Integracao.Piracanjuba.IntegracaoPiracanjuba(unitOfWork, tipoServicoMultisoftware).IntegrarMDFeComPendencia(carga);
                }

                carga.PossuiPendencia = true;
                Servicos.Embarcador.Integracao.ControleIntegracaoCargaEDI.AtualizarSituacaoCargaControleEDI(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI.ProblemaEmissao, unitOfWork);

                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, StringConexao);

                if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                    serCarga.NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.MotivoPendenciaCarga, carga.MotivoPendencia, carga.CodigoCargaEmbarcador), unitOfWork);
            }

            repCarga.Atualizar(carga);
            return carga;
        }

        public bool VerificarMDFeExcedeuTempoLimiteEmissao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {

#if DEBUG
            TempoLimiteParEmissaoEmMinutos = 3;
#endif

            if (mdfe.DataIntegracao != null && mdfe.DataIntegracao.Value.AddMinutes(TempoLimiteParEmissaoEmMinutos) < DateTime.Now && mdfe.Status == Dominio.Enumeradores.StatusMDFe.Enviado)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool VerificarCTeExcedeuTempoLimiteEmissao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (cte.DataIntegracao != null && cte.DataIntegracao.Value.AddMinutes(TempoLimiteParEmissaoEmMinutos) < DateTime.Now && cte.Status == "E")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int BuscarTempoLimiteEmissao()
        {
            return TempoLimiteParEmissaoEmMinutos;
        }

        public void AutorizarEmissaoProximosTrechos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaProximosTrechos = repCargaPedido.BuscarCargasProximoTrecho(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaProximoTrecho in cargaProximosTrechos)
            {
                bool podeLiberar = true;
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasTrechoAnterior = repCargaPedido.BuscarCargasAnteriores(cargaProximoTrecho.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaTrechoAnterior in cargasTrechoAnterior)
                {
                    if (cargaTrechoAnterior.Codigo != carga.Codigo)
                    {
                        if (serCarga.VerificarSeCargaEstaNaLogistica(cargaTrechoAnterior, tipoServicoMultisoftware) ||
                                           cargaTrechoAnterior.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe ||
                                          cargaTrechoAnterior.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos)
                        {
                            podeLiberar = false;
                        }
                    }
                }

                if (podeLiberar)
                {
                    cargaProximoTrecho.AguardandoEmissaoDocumentoAnterior = false;

                    if (cargaProximoTrecho.Empresa != null && (cargaProximoTrecho.DataEnvioUltimaNFe.HasValue || !carga.ExigeConfirmacaoAntesEmissao))
                        cargaProximoTrecho.DataEnvioUltimaNFe = DateTime.Now.AddHours(cargaProximoTrecho.Empresa.TempoDelayHorasParaIniciarEmissao);

                    repCarga.Atualizar(cargaProximoTrecho);
                }
            }
        }

        public void FinalizarEmissoes(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();

            unitOfWork.Start();

            if (carga.EmpresaFilialEmissora == null || carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
            {
                Retornos.RetornoCarga servicoRetornoCarga = new Retornos.RetornoCarga(unitOfWork);

                if (carga.DataFinalizacaoEmissao == null)
                    carga.DataFinalizacaoEmissao = DateTime.Now;

                carga.MotivoPendencia = "";
                carga.PossuiPendencia = false;
                carga.AutorizouTodosCTes = true;
                carga.problemaCTE = false;
                carga.problemaNFS = false;
                carga.problemaMDFe = false;
                carga.problemaAverbacaoCTe = false;
                carga.FinalizandoProcessoEmissao = true;

                if (!carga.CargaEmitidaParcialmente)
                {
                    FinalizarEDIIntegracao(carga, unitOfWork);
                    servicoRetornoCarga.CriarRetorno(carga);
                }

                if (carga.Veiculo != null)
                    serCarga.IniciaViagemVeiculo(carga.Veiculo.Codigo, carga, unitOfWork, Auditado);
            }
            else
            {
                //Manda para integração para gerar a integração da filial emissora.
                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao;
                carga = serCarga.AtualizarStatusCustoExtra(carga, servicoHubCarga, repCarga);
                carga.GerandoIntegracoes = true;
                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
            }

            repCarga.Atualizar(carga);

            unitOfWork.CommitChanges();
        }

        public static void LiberarEmissaoFilialEmissora(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos;
            carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = true;
            carga.EmEmissaoCTeSubContratacaoFilialEmissora = false;
            carga.AgGeracaoCTesAnteriorFilialEmissora = true;
            carga.AutorizouTodosCTes = false;
            carga.PossuiPendencia = false;
            carga.MotivoPendencia = string.Empty;

            repCarga.Atualizar(carga);
        }

        private void FinalizarEDIIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.ControleIntegracaoCargaEDI.AtualizarSituacaoCargaControleEDI(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI.Ok, unitOfWork);

        }

        public void FinalizarCargaEmFinalizacao(int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfigVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Logistica.PrevisaoCarregamento repositorioPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoFluxoGestaoPatio configuracaoFluxoGestaoPatio = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoFluxoGestaoPatio()
            {
                LiberarComMensagemSemComfirmacao = true
            };

            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.Carga servicoCarga = new Carga(unitOfWork);
            Servicos.Embarcador.Carga.RateioProduto serRateioProduto = new Servicos.Embarcador.Carga.RateioProduto(unitOfWork);
            Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Credito.CreditoMovimentacao(unitOfWork);
            Servicos.Embarcador.Carga.Impressao servicoImpressao = new Impressao(unitOfWork);
            GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new GestaoPatio.FluxoGestaoPatio(unitOfWork, configuracaoFluxoGestaoPatio);
            Servicos.Embarcador.Financeiro.TituloAPagar serTituloAPagar = new Servicos.Embarcador.Financeiro.TituloAPagar(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
            Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal servicoNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, Auditado);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(carga.Codigo);

            if (!carga.FinalizandoProcessoEmissao)
                return;

            bool existeCIOTAbertoPorCarga = repCargaCIOT.ExisteCIOTPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from o in cargaPedidos select o.Pedido).Distinct().ToList();
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFreteTerceiro.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoEmissaoDocumentos = repConfiguracaoEmissaoDocumento.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configVeiculo = repConfigVeiculo.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configAgendamentoColeta = repConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();

            if (carga.EmpresaFilialEmissora == null || carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
            {
                if (!carga.CargaEmitidaParcialmente)
                {
                    RecalcularDataPrevisaoEntregaLeadTime(carga, configuracaoControleEntrega, unitOfWork);
                    CriarAprovacaoAutorizacaoIntegracaoCTe(carga, tipoServicoMultisoftware, configuracaoEmbarcador, unitOfWork);
                    CriarAprovacaoLiberacaoEscrituracaoPagamento(carga, tipoServicoMultisoftware, configuracaoEmbarcador, unitOfWork);
                    GerarMovimentosAutorizacaoCarga(ref carga, unitOfWork, tipoServicoMultisoftware, true, true);
                    GerarTitulosAutorizacaoCarga(ref carga, unitOfWork, configuracaoEmbarcador, tipoServicoMultisoftware, Auditado);
                    GerarTitulosGNRECarga(ref carga, unitOfWork, configuracaoFinanceiro, tipoServicoMultisoftware);
                    GerarControleFaturamentoDocumentos(ref carga, unitOfWork, configuracaoEmbarcador, tipoServicoMultisoftware);
                    GerarFaturamentoTakeOrPay(ref carga, unitOfWork, configuracaoEmbarcador, tipoServicoMultisoftware);
                    servicoNotaFiscal.VincularNotasFiscaisAosPedidosDaCarga(carga, cargaPedidos, tipoServicoMultisoftware, Auditado);
                    GerarCanhotosCTes(ref carga, cargaPedidos, unitOfWork, tipoServicoMultisoftware);
                    FinalizarEDIIntegracao(carga, unitOfWork);
                    GerarNFeAnteriorCargaDeColeta(carga, tipoServicoMultisoftware, configuracaoEmbarcador, unitOfWork);
                    Servicos.Embarcador.Carga.Ocorrencia.GerarOcorrenciaAutomaticaTabelaFreteMinima(carga, unitOfWork, tipoServicoMultisoftware);
                    Servicos.Embarcador.Pedido.Pedido.GerarOcorrenciasColetaEntrega(pedidos, carga, carga.TipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGatilhoPedidoOcorrenciaColetaEntrega.FinalizacaoEmissaoCarga, configuracaoEmbarcador, null, unitOfWork);
                    Servicos.Embarcador.Carga.PedidoVinculado.VerificarSeExistePraAgrupamentoPedidoEncaixe(carga, unitOfWork);
                    AdicionarIndicadorIntegracaoCTe(carga, unitOfWork);
                    Servicos.Embarcador.Pedido.SeparacaoPedido.EncaminharPedidosParaSeparacao(carga, unitOfWork);
                    GerarAnuenciaTransportador(carga, unitOfWork);
                    AtualizarSumarizacaoViagem(carga, unitOfWork);
                    AtualizarSituacaoEnvioEmailDocumentacaoCarga(carga, unitOfWork);
                    GerarComprovantesCarga(carga, unitOfWork);
                    GerarPendenciaValorParaContratoFreteFuturo(carga, contratoFrete, unitOfWork);
                    servCargaJanelaDescarregamento.AtualizarSituacao(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaDescarregamentoCadastrada.EmTransito);
                    new Servicos.Embarcador.Carga.CargaCTe(unitOfWork).EnviarEmailPreviaDocumentosCargaCte(carga.Codigo);

                    if (carga.TipoOperacao?.ConfiguracaoMobile?.PermiteBaixarOsDocumentosDeTransporte ?? false)
                    {
                        Servicos.Embarcador.Notificacao.NotificacaoMTrack serNotificacaoMTrack = new Servicos.Embarcador.Notificacao.NotificacaoMTrack(unitOfWork);
                        serNotificacaoMTrack.NotificarMudancaCarga(carga, carga.Motoristas.ToList(), AdminMultisoftware.Dominio.Enumeradores.MobileHubs.DocumentosDeTransporteEmitidos);
                    }
                }

                Servicos.Embarcador.Monitoramento.Monitoramento.IniciarMonitoramento(carga, DateTime.Now, configuracaoEmbarcador, null, unitOfWork);

                if (cargaJanelaCarregamento?.CentroCarregamento != null && !cargaJanelaCarregamento.Excedente && cargaJanelaCarregamento.TerminoCarregamento.Date > DateTime.Now.Date && repositorioPrevisaoCarregamento.VerificarSePossuiRestricaoPorCentroCarregamento(cargaJanelaCarregamento.CentroCarregamento.Codigo))
                {
                    cargaJanelaCarregamento.TerminoCarregamento = DateTime.Now;
                    cargaJanelaCarregamento.InicioCarregamento = cargaJanelaCarregamento.TerminoCarregamento.AddMinutes(-cargaJanelaCarregamento.TempoCarregamento);
                    repCargaJanelaCarregamento.Atualizar(cargaJanelaCarregamento);
                }

                Servicos.Log.TratarErro($"Iniciou transacao - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                if (carga.CargaAgrupada && carga.OcultarNoPatio)
                    cargas = repCarga.BuscarCargasOriginais(carga.Codigo);
                else
                    cargas.Add(carga);

                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repositorioConfiguracaoGestaoPatio.BuscarConfiguracao();

                Servicos.Log.TratarErro($"Fluxo Patio - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargas)
                {
                    bool avancaFluxo = true;

                    if (carga.CargaEmitidaParcialmente && !repCargaCTe.ExisteAutorizadoPorCarga(cargaOrigem.Codigo))
                        avancaFluxo = false;

                    if (avancaFluxo)
                    {
                        if (cargaOrigem.EtapaFaturamentoLiberado)
                        {
                            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(cargaOrigem);

                            if (fluxoGestaoPatio != null)
                            {
                                if ((configuracaoGestaoPatio?.FaturamentoPermiteAvancarAutomaticamenteAposGerarDocumentos ?? true) && !fluxoGestaoPatio.DataFaturamento.HasValue)
                                    servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.Faturamento);

                                if ((configuracaoGestaoPatio?.DocumentosTransportePermiteAvancarAutomaticamenteAposGerarDocumentos ?? true) && !fluxoGestaoPatio.DataDocumentosTransporte.HasValue)
                                    servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.DocumentosTransporte);
                            }
                        }
                        else
                            cargaOrigem.EtapaFaturamentoLiberado = true;

                        repCarga.Atualizar(cargaOrigem);
                    }
                }

                if (!configuracaoEmbarcador.NaoRatearValorFreteProtudos && !carga.CargaEmitidaParcialmente)
                {
                    Servicos.Log.TratarErro($"Rateio frete produtos - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                    serRateioProduto.RatearFretePorProduto(carga, tipoServicoMultisoftware, unitOfWork);
                }

                carga.SituacaoCarga = configuracaoEmbarcador.SituacaoCargaAposEmissaoDocumentos;
                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos && (carga.TipoOperacao?.NaoNecessarioConfirmarImpressaoDocumentos ?? false))
                {
                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte;
                    carga = servicoCarga.AtualizarStatusCustoExtra(carga, servicoHubCarga, repCarga);
                    carga.DataMudouSituacaoParaEmTransporte = DateTime.Now;
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte.ObterDescricao()}", unitOfWork);
                }

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao)
                {
                    Servicos.Log.TratarErro($"Integrações - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAMDFe);
                    if (tipoIntegracao != null)
                    {
                        Servicos.Embarcador.Integracao.IntegracaoCarga svcIntegracaoCarga = new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork);
                        svcIntegracaoCarga.InformarIntegracaoCarga(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAMDFe, unitOfWork);

                        Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaParaIntegracao(carga, tipoIntegracao, unitOfWork, false, false);
                    }

                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoEscrituracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAEscrituracaoCTe);
                    if (tipoIntegracaoEscrituracao != null)
                    {
                        Servicos.Embarcador.Integracao.IntegracaoCarga svcIntegracaoCarga = new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork);
                        svcIntegracaoCarga.InformarIntegracaoCarga(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAEscrituracaoCTe, unitOfWork);

                        //Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaParaIntegracao(carga, tipoIntegracaoEscrituracao, unitOfWork);
                    }

                    if (carga.FreteDeTerceiro && (carga.TipoOperacao?.Integracoes?.Any(integracao => integracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DPACiot) ?? false))
                    {
                        Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoDPACiot = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DPACiot);
                        if (tipoIntegracaoDPACiot != null)
                        {
                            Servicos.Embarcador.Integracao.IntegracaoCarga svcIntegracaoCarga = new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork);
                            svcIntegracaoCarga.InformarIntegracaoCarga(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DPACiot, unitOfWork);

                            Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaParaIntegracao(carga, tipoIntegracaoDPACiot, unitOfWork, false, false);
                        }
                    }

                    carga.GerandoIntegracoes = true;
                }

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                {
                    new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(unitOfWork).GerarIntegracaoNotificacao(carga, TipoNotificacaoApp.MotoristaPodeSeguirViagem);
                    if (configuracaoEmbarcador.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoInformarVeiculoNaCargaECargaEmTransporte)
                        Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciar(carga, configuracaoEmbarcador, null, "Carga em transporte", unitOfWork);
                }

                if (!carga.CargaEmitidaParcialmente)
                {
                    Servicos.Log.TratarErro($"Canhotos - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosProximoTrecho = null;
                    if (cargaPedidos.Any(obj => obj.CargaPedidoProximoTrecho != null))
                    {
                        cargaPedidosProximoTrecho = cargaPedidos.ToList();
                        AutorizarEmissaoProximosTrechos(carga, cargaPedidosProximoTrecho, unitOfWork, tipoServicoMultisoftware);
                    }

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && (cargaPedidosProximoTrecho != null || cargaPedidos.Any(obj => obj.Recebedor != null)) && (carga.TipoOperacao?.ConfiguracaoCanhoto?.NaoGerarCanhotoAvulsoEmCargasComAoMenosUmRecebedor == false))
                    {
                        if (cargaPedidosProximoTrecho == null)
                            cargaPedidosProximoTrecho = cargaPedidos.ToList();

                        Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Canhotos.Canhoto(unitOfWork);
                        serCanhoto.CriarCanhotoAvulso(carga, cargaPedidosProximoTrecho, unitOfWork);
                    }

                    Servicos.Embarcador.Escrituracao.Pagamento.LiberarPagamentoReentrega(carga, configuracaoEmbarcador, configuracaoFinanceiro, unitOfWork);
                    Servicos.Embarcador.Escrituracao.DocumentoEscrituracao.AdicionarDocumentoParaEscrituracao(carga, tipoServicoMultisoftware, unitOfWork);
                    Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(carga, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork);
                }

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Servicos.Log.TratarErro($"Janela carregamento - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita cargaJanelaCarregamentoGuarita = repCargaJanelaCarregamentoGuarita.BuscarPorCarga(carga.Codigo);
                    if (cargaJanelaCarregamentoGuarita != null)
                    {
                        cargaJanelaCarregamentoGuarita.DataLiberacaoVeiculo = DateTime.Now;

                        if (!cargaJanelaCarregamentoGuarita.DataFinalCarregamento.HasValue)
                            cargaJanelaCarregamentoGuarita.DataFinalCarregamento = cargaJanelaCarregamentoGuarita.DataLiberacaoVeiculo;

                        repCargaJanelaCarregamentoGuarita.Atualizar(cargaJanelaCarregamentoGuarita);
                    }

                    serCreditoMovimentacao.VerificarSeOperadorObtveCreditoNaCarga(carga, unitOfWork);
                }

                if (configuracaoEmbarcador.EnviarDocumentosAutomaticamenteParaImpressao)
                {
                    Servicos.Log.TratarErro($"Impressão CTe - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                    servicoImpressao.EnviarDocumentosParaImpressao(carga);
                }

                if (configuracaoEmbarcador.EnviarMDFeAutomaticamenteParaImpressao)
                {
                    Servicos.Log.TratarErro($"Impressão MDFe - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                    servicoImpressao.EnviarMDFeParaImpressao(carga);
                }

                if (carga.TipoOperacao?.EnviarEmailPlanoViagemFinalizarCarga ?? false)
                {
                    Servicos.Log.TratarErro($"Plano viagem envio e-mail - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                    servicoImpressao.EnviarPlanoViagemParaDestinatariosPorEmail(carga, "Plano de Viagem");
                }

                if (!carga.CargaEmitidaParcialmente)
                {
                    Servicos.Log.TratarErro($"Pallets - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                    Servicos.Embarcador.Carga.ComplementoFrete serComplementoFrete = new ComplementoFrete(unitOfWork);
                    serComplementoFrete.VerificarCargaComplementoFretePendentesCTesComplementares(carga, unitOfWork, tipoServicoMultisoftware);
                    CriarDevolucaoPallets(unitOfWork, carga, tipoServicoMultisoftware);
                }

                if (carga.CargaAgrupada)
                {
                    Servicos.Log.TratarErro($"Atualizar situação cargas agrupadas - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
                    repCarga.AtualizarSituacaoCargasAgrupadas(carga.Codigo, carga.SituacaoCarga);
                }
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (!configVeiculo.NaoAlterarCentroResultadoVeiculosEmissaoCargas)
                    AjustarCentroResultadoCarga(carga, configuracaoEmissaoDocumentos, unitOfWork, usuario);

                if (contratoFrete != null && configuracaoEmbarcador.AjustarDataContratoIgualDataFinalizacaoCarga)
                {
                    contratoFrete.DataEmissaoContrato = carga.DataFinalizacaoEmissao ?? DateTime.Now;

                    repContratoFreteTerceiro.Atualizar(contratoFrete);
                }

                if (carga.FreteDeTerceiro && existeCIOTAbertoPorCarga)
                {
                    Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTerceiro.BuscarPorPessoa(contratoFrete.TransportadorTerceiro.CPF_CNPJ);
                    if (!modalidadeTransportadoraPessoas.GerarPagamentoTerceiro)
                    {
                        if (contratoFrete.ConfiguracaoCIOT?.GerarTitulosContratoFrete ?? false)
                        {
                            contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado;

                            repContratoFreteTerceiro.Atualizar(contratoFrete);

                            if (!serTituloAPagar.AtualizarTitulos(contratoFrete, unitOfWork, tipoServicoMultisoftware, out string erroTitulos, carga.Empresa.TipoAmbiente, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada))
                            {
                                unitOfWork.Rollback();
                                throw new Exception(erroTitulos);
                            }
                        }
                    }
                }
            }

            if (repCargaPedido.PossuiMontagemContainerNaCarga(carga.Codigo))
                servicoPedido.AtualizarSituacaoMontagemContainer(carga.Codigo, StatusMontagemContainer.Expedido, unitOfWork);

            carga.FinalizandoProcessoEmissao = false;
            carga.AguardandoEnvioDocumentacaoLote = configuracaoEmissaoDocumentos.AtivarEnvioDocumentacaoFinalizacaoCarga;

            if (carga.TipoOperacao?.GerarControleColetaEntregaAposEmissaoDocumentos ?? false)
                carga.GerandoControleColetaEntrega = true;

            if (carga.TipoOperacao?.NaoDisponibilizarCargaParaIntegracaoERP ?? false)
                carga.CargaIntegradaEmbarcador = true;

            repCarga.Atualizar(carga);

            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repAgendamentoColeta.BuscarPorCarga(carga.Codigo);
            if (agendamentoColeta != null)
            {
                if (configAgendamentoColeta?.CalcularDataDeEntregaPorTempoDeDescargaDaRota ?? false)
                {
                    Servicos.Embarcador.Carga.RotaFrete serRotaFrete = new Servicos.Embarcador.Carga.RotaFrete(unitOfWork);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico primeiroCTe = repCargaCTe.BuscarPrimeiroCTePorCarga(carga.Codigo);

                    if (primeiroCTe != null)
                    {
                        int tempoDescarga = (int?)carga.Rota?.TempoDescarga.TotalMinutes ?? configAgendamentoColeta.TempoPadraoDeDescargaMinutos;
                        DateTime? dataCalculada = primeiroCTe.DataEmissao?.AddMinutes(tempoDescarga) ?? agendamentoColeta.DataEntrega;

                        if (carga.Rota != null)
                            dataCalculada = serRotaFrete.ObtemDataEntregaComRestricao(carga.Rota, dataCalculada, carga.ModeloVeicularCarga?.Codigo ?? 0, carga.TipoDeCarga?.Codigo ?? 0);

                        agendamentoColeta.DataEntrega = dataCalculada;

                        servCargaJanelaDescarregamento.DefinirHorarioPorAgendamentoColeta(agendamentoColeta);

                        repAgendamentoColeta.Atualizar(agendamentoColeta);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, agendamentoColeta, null, $"Data de entrega alterada para {agendamentoColeta.DataEntrega.Value:dd/MM/yyyy HH:mm} após emissão do CTE.", unitOfWork);
                    }
                }

                if (configuracaoEmbarcador.ControlarAgendamentoSKU)
                {
                    agendamentoColeta.Situacao = SituacaoAgendamentoColeta.Finalizado;
                    agendamentoColeta.EtapaAgendamentoColeta = EtapaAgendamentoColeta.Emissao;

                    repAgendamentoColeta.Atualizar(agendamentoColeta);

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                    {
                        TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                        OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
                    };

                    Servicos.Auditoria.Auditoria.Auditar(auditado, agendamentoColeta, null, $"Finalizou o agendamento pela emissão da carga.", unitOfWork);
                }
            }

            if ((carga.TipoOperacao?.GerarRedespachoAutomaticamentePorPedidoAposEmissaoDocumentos ?? false) || (carga.TipoOperacao?.ConfiguracaoCarga?.GerarRedespachoAutomaticamenteAposEmissaoDocumentos ?? false))
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    if (!cargaPedido.PendenteGerarCargaDistribuidor && cargaPedido.Recebedor != null)
                    {
                        cargaPedido.PendenteGerarCargaDistribuidor = true;
                        repCargaPedido.Atualizar(cargaPedido);
                    }
                }

                carga.PendenteGerarCargaDistribuidor = true;
            }

            if (carga.TipoOperacao?.ConfiguracaoCarga?.GerarCargaEspelhoAutomaticamenteAoFinalizarCarga ?? false)
                carga.PendenteGerarCargaEspelho = true;

            bool gerarRetornoAutomatico = (carga.TipoOperacao?.ConfiguracaoCarga?.GerarRetornoAutomaticoMomento ?? GerarRetornoAutomaticoMomento.Nenhum) == GerarRetornoAutomaticoMomento.FinalizarEmissaoDocumentos;
            if (gerarRetornoAutomatico)
            {
                Servicos.Embarcador.Carga.Retornos.RetornoCarga serRetornos = new Servicos.Embarcador.Carga.Retornos.RetornoCarga(unitOfWork);

                serRetornos.GerarCargaRetorno(carga, carga.TipoOperacao.TipoRetornoCarga?.Codigo ?? 0, 0, carga.Veiculo?.Codigo ?? 0, carga.VeiculosVinculados?.FirstOrDefault()?.Codigo ?? 0, 0, false, null);
            }

            unitOfWork.CommitChanges();
            Servicos.Log.TratarErro($"Commit transacao - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                unitOfWork.FlushAndClear();

                carga = repCarga.BuscarPorCodigo(carga.Codigo);
                contratoFrete = repContratoFreteTerceiro.BuscarPorCarga(carga.Codigo);

                if (carga.FreteDeTerceiro && !existeCIOTAbertoPorCarga && contratoFrete != null)
                {
                    unitOfWork.Start();

                    Servicos.Embarcador.Terceiros.ContratoFrete.AlcadasContratoFrete(contratoFrete, unitOfWork, tipoServicoMultisoftware);

                    if (!Servicos.Embarcador.Terceiros.ContratoFrete.ProcessarContratoAprovado(contratoFrete, tipoServicoMultisoftware, carga.Empresa?.TipoAmbiente ?? Dominio.Enumeradores.TipoAmbiente.Producao, null, unitOfWork, StringConexao, out string erro))
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(erro);
                    }
                    else
                    {
                        unitOfWork.CommitChanges();
                    }
                }
            }

            Servicos.Log.TratarErro($"Hub informar carga atualizada - {codigoCarga} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff")}", "FinalizarCargaEmFinalizacao");
            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

            if (configuracaoEmbarcador.HabilitarEnvioAbastecimentoExterno)
            {
                Servicos.Embarcador.Logistica.RotaFreteAbastecimento serRotaFreteAbastecimento = new Logistica.RotaFreteAbastecimento(unitOfWork);
                serRotaFreteAbastecimento.GerarRequisicaoAbastecimento(carga, unitOfWork);
            }
        }

        public void AtualizarSumarizacaoViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, bool cancelamento = false)
        {
            if (carga.PedidoViagemNavio == null)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador reposotorioConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = reposotorioConfiguracaoTransportador.BuscarConfiguracaoPadrao();

            if (!configuracaoTransportador.AtivarControleCarregamentoNavio)
                return;

            if (carga.PedidoViagemNavio == null || carga.CargaSVM)
                return;

            //if (!repCargaCTe.ContemCTeAquaviario(carga.Codigo))
            //    return;

            decimal qtdPlugs = 0m;
            decimal qtdTEUS = 0m;
            decimal qtdTONS = 0m;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repCargaPedido.BuscarPedidosPorCarga(carga.Codigo);

            if (pedidos == null || pedidos.Count == 0)
                return;

            qtdTEUS = pedidos.Where(c => c.Container != null && c.Container.ContainerTipo != null && (c.Container.ContainerTipo.TipoPes == TipoPes.vintepes || c.Container.ContainerTipo.TipoPes == TipoPes.quarentapes))?.Sum(c => c.Container.ContainerTipo.TEU.ToInt()) ?? 0m;
            qtdPlugs = pedidos.Where(c => c.Container != null && c.Container.TipoCarregamentoNavio == TipoCarregamentoNavio.Reefer)?.Count() ?? 0m;
            qtdTONS = carga.DadosSumarizados.PesoTotal / 1000;
            if (qtdTONS < 0)
                qtdTONS = 0;

            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = repViagem.BuscarPorCodigo(carga.PedidoViagemNavio.Codigo);
            viagem.Initialize();
            if (!cancelamento)
            {
                viagem.ConsumoPlugs += qtdPlugs;
                viagem.ConsumoTeus += qtdTEUS;
                viagem.ConsumoTons += qtdTONS;
            }
            else
            {
                viagem.ConsumoPlugs -= qtdPlugs;
                viagem.ConsumoTeus -= qtdTEUS;
                viagem.ConsumoTons -= qtdTONS;
            }

            if (viagem.ConsumoPlugs < 0)
                viagem.ConsumoPlugs = 0;
            if (viagem.ConsumoTeus < 0)
                viagem.ConsumoTeus = 0;
            if (viagem.ConsumoTons < 0)
                viagem.ConsumoTons = 0;

            repViagem.Atualizar(viagem);
            if (!cancelamento)
                Servicos.Auditoria.Auditoria.Auditar(auditado, viagem, null, $"Consumo de viagem alterado pela finalização da carga {carga.CodigoCargaEmbarcador}.", unitOfWork);
            else
                Servicos.Auditoria.Auditoria.Auditar(auditado, viagem, null, $"Consumo de viagem alterado pelo cancelamento da carga {carga.CodigoCargaEmbarcador}.", unitOfWork);
        }

        public void GerarComprovantesContainerCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante tipoComprovante, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprovanteCarga situacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga repComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga comprovante;

            comprovante = PreencherComprovante(carga, tipoComprovante, situacao);
            repComprovante.Inserir(comprovante);

        }

        private void GerarComprovantesCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga repComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido = carga.Pedidos?.FirstOrDefault();
            Dominio.Entidades.Cliente tomador = pedido?.ObterTomador();
            if (tomador == null)
                return;

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = tomador.GrupoPessoas;

            Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga comprovante;
            ICollection<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante> tiposComprovantes;

            if (carga.TipoOperacao?.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa ?? false)
                tiposComprovantes = carga.TipoOperacao?.TiposComprovante;
            else if (tomador.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa)
                tiposComprovantes = tomador?.TiposComprovante;
            else
                tiposComprovantes = tomador.GrupoPessoas?.TiposComprovante;

            if (tiposComprovantes != null)
                foreach (Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante tipo in tiposComprovantes)
                {
                    comprovante = PreencherComprovante(carga, tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprovanteCarga.Pendente);
                    repComprovante.Inserir(comprovante);
                }
        }

        private void GerarPendenciaValorParaContratoFreteFuturo(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.PendenciaContratoFreteFuturo repPendenciaContratoFreteFuturo = new Repositorio.Embarcador.Terceiros.PendenciaContratoFreteFuturo(unitOfWork);

            if (contratoFrete != null && (carga.TipoOperacao?.ConfiguracaoTerceiro?.AdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato ?? false))
            {
                Dominio.Entidades.Embarcador.Terceiros.PendenciaContratoFreteFuturo pendenciaContratoFreteFuturo = new Dominio.Entidades.Embarcador.Terceiros.PendenciaContratoFreteFuturo();
                pendenciaContratoFreteFuturo.Ativo = true;
                pendenciaContratoFreteFuturo.ContratoFreteOrigem = contratoFrete;
                pendenciaContratoFreteFuturo.Justificativa = carga.TipoOperacao?.ConfiguracaoTerceiro?.JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato;
                pendenciaContratoFreteFuturo.Veiculo = carga.Veiculo;
                pendenciaContratoFreteFuturo.Valor = contratoFrete.ValorFreteSubcontratacao;
                pendenciaContratoFreteFuturo.TransportadorTerceiro = contratoFrete.TransportadorTerceiro;
                repPendenciaContratoFreteFuturo.Inserir(pendenciaContratoFreteFuturo);

            }
        }

        private Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga PreencherComprovante(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante tipoComprovante, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComprovanteCarga situacao)
        {
            Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga comprovante = new Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga();
            comprovante.Carga = carga;
            comprovante.TipoComprovante = tipoComprovante;
            comprovante.Situacao = situacao;

            return comprovante;
        }

        private void AjustarCentroResultadoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo repHistoricoVeiculoVinculo = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado repHistoricoVeiculoVinculoCentroResultado = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };

            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = repCargaPedido.BuscarCentroResultadoPorCarga(carga.Codigo);

            if (centroResultado == null)
                return;

            if (carga.Veiculo != null)
            {
                if (carga.Veiculo.CentroResultado == null || carga.Veiculo.CentroResultado.Codigo != centroResultado.Codigo)
                {
                    carga.Veiculo.Initialize();

                    Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoAntigo = carga.Veiculo.CentroResultado;

                    carga.Veiculo.CentroResultado = centroResultado;

                    repVeiculo.Atualizar(carga.Veiculo);

                    if (centroResultadoAntigo != centroResultado)
                    {
                        Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo();
                        historicoVeiculoVinculo = repHistoricoVeiculoVinculo.BuscarPorVeiculo(carga.Veiculo.Codigo);

                        if (historicoVeiculoVinculo == null)
                        {
                            historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                            {
                                Veiculo = carga.Veiculo,
                                DataHora = DateTime.Now,
                                Usuario = usuario,
                                KmRodado = carga.Veiculo.KilometragemAtual,
                                KmAtualModificacao = 0,
                                DiasVinculado = 0
                            };
                            repHistoricoVeiculoVinculo.Inserir(historicoVeiculoVinculo);
                        }

                        Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado
                        {
                            HistoricoVeiculoVinculo = historicoVeiculoVinculo,
                            CentroResultado = carga.Veiculo.CentroResultado,
                            DataHora = DateTime.Now
                        };

                        repHistoricoVeiculoVinculoCentroResultado.Inserir(historicoVeiculoVinculoCentroResultado, auditado);
                    }

                    Servicos.Auditoria.Auditoria.Auditar(auditado, carga.Veiculo, carga.Veiculo.GetChanges(), $"Centro de resultado ajustado pela emissão da carga {carga.CodigoCargaEmbarcador}.", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);
                }

                foreach (Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento in carga.Veiculo.Equipamentos)
                {
                    if (equipamento.CentroResultado == null || equipamento.CentroResultado.Codigo != centroResultado.Codigo)
                    {
                        equipamento.Initialize();

                        equipamento.CentroResultado = centroResultado;

                        repEquipamento.Atualizar(equipamento);

                        Servicos.Auditoria.Auditoria.Auditar(auditado, equipamento, equipamento.GetChanges(), $"Centro de resultado ajustado pela emissão da carga {carga.CodigoCargaEmbarcador}.", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);
                    }
                }
            }

            foreach (Dominio.Entidades.Veiculo veiculo in carga.VeiculosVinculados)
            {
                if (veiculo.CentroResultado == null || veiculo.CentroResultado.Codigo != centroResultado.Codigo)
                {
                    veiculo.Initialize();

                    Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoAntigo = carga.Veiculo.CentroResultado;

                    veiculo.CentroResultado = centroResultado;

                    repVeiculo.Atualizar(veiculo);

                    if (centroResultadoAntigo != centroResultado)
                    {
                        Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo();
                        historicoVeiculoVinculo = repHistoricoVeiculoVinculo.BuscarPorVeiculo(carga.Veiculo.Codigo);

                        if (historicoVeiculoVinculo == null)
                        {
                            historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                            {
                                Veiculo = carga.Veiculo,
                                DataHora = DateTime.Now,
                                Usuario = usuario,
                                KmRodado = carga.Veiculo.KilometragemAtual,
                                KmAtualModificacao = 0,
                                DiasVinculado = 0
                            };
                            repHistoricoVeiculoVinculo.Inserir(historicoVeiculoVinculo);
                        }

                        Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado
                        {
                            HistoricoVeiculoVinculo = historicoVeiculoVinculo,
                            CentroResultado = carga.Veiculo.CentroResultado,
                            DataHora = DateTime.Now,
                        };

                        repHistoricoVeiculoVinculoCentroResultado.Inserir(historicoVeiculoVinculoCentroResultado, auditado);
                    }

                    Servicos.Auditoria.Auditoria.Auditar(auditado, veiculo, veiculo.GetChanges(), $"Centro de resultado ajustado pela emissão da carga {carga.CodigoCargaEmbarcador}.", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);
                }

                foreach (Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento in veiculo.Equipamentos)
                {
                    if (equipamento.CentroResultado == null || equipamento.CentroResultado.Codigo != centroResultado.Codigo)
                    {
                        equipamento.Initialize();

                        equipamento.CentroResultado = centroResultado;

                        repEquipamento.Atualizar(equipamento);

                        Servicos.Auditoria.Auditoria.Auditar(auditado, equipamento, equipamento.GetChanges(), $"Centro de resultado ajustado pela emissão da carga {carga.CodigoCargaEmbarcador}.", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);
                    }
                }
            }

            if (!(configuracaoCargaEmissaoDocumento?.NaoAlterarCentroResultadoMotorista ?? false))
            {
                foreach (Dominio.Entidades.Usuario motorista in carga.Motoristas)
                {
                    if (motorista.CentroResultado == null || motorista.CentroResultado.Codigo != centroResultado.Codigo)
                    {
                        motorista.Initialize();

                        motorista.CentroResultado = centroResultado;

                        repMotorista.Atualizar(motorista);

                        Servicos.Auditoria.Auditoria.Auditar(auditado, motorista, motorista.GetChanges(), $"Centro de resultado ajustado pela emissão da carga {carga.CodigoCargaEmbarcador}.", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);
                    }
                }
            }
        }

        private void AtualizarTipoContratacao(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro)
            {
                if (cargaPedido.Expedidor != null && cargaPedido.Recebedor != null)
                {
                    if (cargaPedido.Carga.GrupoPessoaPrincipal != null && cargaPedido.Carga.GrupoPessoaPrincipal.EmitirSempreComoRedespacho)
                        cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                    else
                        cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;
                }
                else if (cargaPedido.Expedidor != null || cargaPedido.Recebedor != null)
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                else
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                if (cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Subcontratacao || (cargaPedido.Carga.TipoOperacao?.SempreEmitirSubcontratacao ?? false))
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

                cargaPedido.CargaPedidoFilialEmissora = false;

            }

            repCargaPedido.Atualizar(cargaPedido);

        }

        public void GerarNFeAnteriorCargaDeColeta(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.CargaColeta || carga.Empresa == null || !carga.Empresa.PermiteEmitirSubcontratacao)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repCargaPedido.BuscarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDeColeta = (from pedidos in cargasPedido where pedidos.Pedido != null && pedidos.Pedido.DisponibilizarPedidoParaColeta == true select pedidos.Pedido).ToList();

            Servicos.Cliente serCliente = new Cliente();


            if (pedidosDeColeta.Count == 0)
                return;

            List<int> codigosPedidosDeColeta = (from pedidocoleta in pedidosDeColeta select pedidocoleta.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDeColeta = repCargaPedido.BuscarCargasPedidoDeColetaPorPedidos(codigosPedidosDeColeta);

            Servicos.Embarcador.Carga.FilialEmissora serFilialEmissora = new Embarcador.Carga.FilialEmissora();

            Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(double.Parse(carga.Empresa.CNPJ_SemFormato));
            if (tomador != null)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoDeColeta in cargaPedidosDeColeta)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoAnterior = carga.Pedidos.Where(o => o.Pedido.Codigo == cargaPedidoDeColeta.Pedido.Codigo).FirstOrDefault();

                    serFilialEmissora.GerarCTesAnterioresDaFilialEmissoraRetorno(cargaPedidoAnterior, cargaPedidoDeColeta, tipoServicoMultisoftware, configuracao, unitOfWork, true, true);
                    cargaPedidoDeColeta.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                    cargaPedidoDeColeta.Tomador = tomador;
                    AtualizarTipoContratacao(cargaPedidoDeColeta, unitOfWork);
                }
            }
        }

        public void GerarMovimentoEmissaoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, bool gerarParaCancelamentosEAnulacoes)
        {
            string erro = string.Empty;

            if (!GerarMovimentoAutorizacaoCTe(out erro, cargaCTe, tipoServicoMultisoftware, unidadeTrabalho, gerarParaCancelamentosEAnulacoes))
                throw new Exception(erro);
        }

        public bool GerarMovimentoEmissaoCTe(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, bool gerarParaCancelamentosEAnulacoes)
        {
            return GerarMovimentoAutorizacaoCTe(out erro, cargaCTe, tipoServicoMultisoftware, unidadeTrabalho, gerarParaCancelamentosEAnulacoes);
        }

        public static void AdicionarCTeManualParaGeracaoDeMovimento(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, string situacaoCTeGerar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.CTe.ControleGeracaoMovimentoCTeManual repControleGeracaoMovimento = new Repositorio.Embarcador.CTe.ControleGeracaoMovimentoCTeManual(unidadeTrabalho);

            if (cargaCTe.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte || cargaCTe.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada)
            {
                Dominio.Entidades.Embarcador.CTe.ControleGeracaoMovimentoCTeManual controleGeracaoMovimento = repControleGeracaoMovimento.BuscarPorCargaCTe(cargaCTe.Codigo);

                if (controleGeracaoMovimento == null)
                {
                    controleGeracaoMovimento = new Dominio.Entidades.Embarcador.CTe.ControleGeracaoMovimentoCTeManual()
                    {
                        CargaCTe = cargaCTe
                    };
                }

                controleGeracaoMovimento.SituacaoCTeGerar = situacaoCTeGerar;

                if (controleGeracaoMovimento.Codigo <= 0)
                    repControleGeracaoMovimento.Inserir(controleGeracaoMovimento);
                else
                    repControleGeracaoMovimento.Atualizar(controleGeracaoMovimento);
            }
        }

        public void GerarTituloGNRECTeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cargaCTe.Carga.TipoOperacao != null && (cargaCTe.Carga?.TipoOperacao?.NaoGerarTituloGNREAutomatico ?? false))
                return;

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || cargaCTe.Carga.CargaTransbordo)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE repConfiguracaoFinanceiraGNRE = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE configuracaoFinanceiraGNRE = repConfiguracaoFinanceiraGNRE.BuscarPrimeiroRegistro();

            if (configuracaoFinanceiraGNRE == null || !configuracaoFinanceiraGNRE.GerarGNREParaCTesEmitidos || !configuracaoFinanceiraGNRE.GerarGNREAutomaticamente)
                return;

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            if (repCargaCTe.ContarPorCargaQueGeraFaturamento(cargaCTe.Carga.Codigo) > 0)
            {
                if (cargaCTe.GerouTituloGNREAutorizacao)
                    return;

                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro repConfiguracaoFinanceiraGNRERegistro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro> configuracaoFinanceiraGNRERegistros = repConfiguracaoFinanceiraGNRERegistro.BuscarPorConfiguracao(configuracaoFinanceiraGNRE.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaCTe.Carga.Codigo);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                Servicos.Embarcador.Financeiro.Titulo.GerarTituloGNRE(cargaCTe, cargaPedido, configuracaoFinanceiraGNRERegistros, unitOfWork, tipoServicoMultisoftware, configuracaoFinanceiro);

                cargaCTe.GerouTituloGNREAutorizacao = true;

                repCargaCTe.Atualizar(cargaCTe);

                unitOfWork.CommitChanges();

                unitOfWork.FlushAndClear();
            }
        }

        #endregion

        #region Métodos Privados

        private void CriarAprovacaoLiberacaoEscrituracaoPagamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Escrituracao.LiberacaoEscrituracaoPagamento servicoLiberacaoEscrituracaoPagamento = new Escrituracao.LiberacaoEscrituracaoPagamento(unitOfWork, configuracaoEmbarcador);

            servicoLiberacaoEscrituracaoPagamento.CriarAprovacao(carga, tipoServicoMultisoftware);
        }

        private void CriarAprovacaoAutorizacaoIntegracaoCTe(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Embarcador.CTe.AutorizacaoIntegracaoCTe servicoAutorizacaoIntegracaoCTe = new Embarcador.CTe.AutorizacaoIntegracaoCTe(unitOfWork);

            servicoAutorizacaoIntegracaoCTe.CriarAprovacao(carga, tipoServicoMultisoftware);
        }

        private void CriarDevolucaoPallets(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            new Pallets.DevolucaoPallets(unitOfWork).Adicionar(carga, tipoServicoMultisoftware);
        }

        private bool GerarMovimentoAutorizacaoCTe(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, bool gerarParaCancelamentosEAnulacoes)
        {
            if (gerarParaCancelamentosEAnulacoes && cargaCTe.CTe.Status != "A" && cargaCTe.CTe.Status != "C" && cargaCTe.CTe.Status != "Z")
            {
                erro = string.Empty;
                return true;
            }

            if (!gerarParaCancelamentosEAnulacoes && cargaCTe.CTe.Status != "A")
            {
                erro = string.Empty;
                return true;
            }

            if (cargaCTe.Carga.CargaTransbordo)
            {
                erro = string.Empty;
                return true;
            }

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimento = new Financeiro.ProcessoMovimento(StringConexao);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentes = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unidadeTrabalho);

            DateTime dataMovimentacao = cargaCTe.CTe.DataAutorizacao != null ? cargaCTe.CTe.DataAutorizacao.Value : cargaCTe.CTe.DataEmissao.Value;
            string observacaoMovimentacao = "Movimento gerado à partir do " + cargaCTe.CTe.ModeloDocumentoFiscal.Abreviacao + " " + cargaCTe.CTe.Numero + "-" + cargaCTe.CTe.Serie.Numero + ".";

            if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoEmissao, dataMovimentacao, cargaCTe.CTe.ValorAReceber, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                return false;

            if (cargaCTe.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaValorLiquido)
            {
                if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoPropriaNacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoAgregadoNacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoTerceiroNacional != null &&
                    cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoPropriaInternacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoAgregadoInternacional != null && cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoTerceiroInternacional != null)
                {
                    bool cteNacional = cargaCTe.CTe.LocalidadeInicioPrestacao.Pais == null || cargaCTe.CTe.LocalidadeTerminoPrestacao.Pais == null && (cargaCTe.CTe.LocalidadeInicioPrestacao.Pais.Sigla == "01058" && cargaCTe.CTe.LocalidadeTerminoPrestacao.Pais.Sigla == "01058");
                    bool cteProprio = cargaCTe.Carga.Terceiro == null;
                    bool cteAgregado = false;
                    bool cteTerceiro = false;
                    if (!cteProprio)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(cargaCTe.Carga.Terceiro, unidadeTrabalho);

                        cteAgregado = modalidadeTerceiro != null && modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado;
                        cteTerceiro = modalidadeTerceiro == null || modalidadeTerceiro.TipoTransportador != TipoProprietarioVeiculo.TACAgregado;
                    }
                    if (cteNacional && cteProprio)
                    {
                        if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoPropriaNacional, dataMovimentacao, cargaCTe.CTe.ValorFrete, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                            return false;
                    }
                    else if (!cteNacional && cteProprio)
                    {
                        if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoPropriaInternacional, dataMovimentacao, cargaCTe.CTe.ValorFrete, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                            return false;
                    }
                    else if (cteNacional && cteAgregado)
                    {
                        if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoAgregadoNacional, dataMovimentacao, cargaCTe.CTe.ValorFrete, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                            return false;
                    }
                    else if (!cteNacional && cteAgregado)
                    {
                        if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoAgregadoInternacional, dataMovimentacao, cargaCTe.CTe.ValorFrete, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                            return false;
                    }
                    else if (cteNacional && cteTerceiro)
                    {
                        if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoTerceiroNacional, dataMovimentacao, cargaCTe.CTe.ValorFrete, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                            return false;
                    }
                    else if (!cteNacional && cteTerceiro)
                    {
                        if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoTerceiroInternacional, dataMovimentacao, cargaCTe.CTe.ValorFrete, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                            return false;
                    }

                }
                else
                {
                    if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissao, dataMovimentacao, cargaCTe.CTe.ValorFrete, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                        return false;
                }
            }

            decimal valorImposto = cargaCTe.CTe.ValorICMS;
            string tipoImposto = "ICMS.";
            if (cargaCTe.CTe.ModeloDocumentoFiscal != null && (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS))
            {
                valorImposto = cargaCTe.CTe.ValorISS;
                tipoImposto = "ISS.";
            }

            if (cargaCTe.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaImpostos)
                if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoImpostoEmissao, dataMovimentacao, valorImposto, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: " + tipoImposto, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                    return false;

            if (cargaCTe.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaPIS)
                if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoPISEmissao, dataMovimentacao, cargaCTe.CTe.ValorPIS, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: PIS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                    return false;

            if (cargaCTe.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS)
                if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoCOFINSEmissao, dataMovimentacao, cargaCTe.CTe.ValorCOFINS, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: COFINS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                    return false;

            if (cargaCTe.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaIR)
                if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoIREmissao, dataMovimentacao, cargaCTe.CTe.ValorIR, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: IR.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                    return false;

            if (cargaCTe.CTe.ModeloDocumentoFiscal.DiferenciarMovimentosParaCSLL)
                if (!svcMovimento.GerarMovimentacao(out erro, cargaCTe.CTe.ModeloDocumentoFiscal.TipoMovimentoCSLLEmissao, dataMovimentacao, cargaCTe.CTe.ValorCSLL, cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: CSLL.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                    return false;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> componentesFrete = repCargaCTeComponentes.BuscarPorCargaCTeQueGeraMovimentacao(cargaCTe.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete componente in componentesFrete)
                if (componente.ComponenteFrete.GerarMovimentoAutomatico)
                    if (!svcMovimento.GerarMovimentacao(out erro, componente.ComponenteFrete.TipoMovimentoEmissao, dataMovimentacao, Math.Abs(componente.ValorComponente), cargaCTe.CTe.Numero.ToString() + "-" + cargaCTe.CTe.Serie.Numero.ToString(), observacaoMovimentacao + " Componente: " + componente.ComponenteFrete.Descricao + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cargaCTe.CTe.TomadorPagador?.Cliente, cargaCTe.CTe.TomadorPagador?.GrupoPessoas, null, null, null, cargaCTe.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                        return false;

            erro = string.Empty;
            return true;
        }

        public bool GerarMovimentoAutorizacaoCTe(out string erro, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, bool gerarParaCancelamentosEAnulacoes)
        {
            if (gerarParaCancelamentosEAnulacoes && cte.Status != "A" && cte.Status != "C" && cte.Status != "Z")
            {
                erro = string.Empty;
                return true;
            }

            if (!gerarParaCancelamentosEAnulacoes && cte.Status != "A")
            {
                erro = string.Empty;
                return true;
            }

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimento = new Financeiro.ProcessoMovimento(StringConexao);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);

            DateTime dataMovimentacao = cte.DataAutorizacao != null ? cte.DataAutorizacao.Value : cte.DataEmissao.Value;
            string observacaoMovimentacao = "Movimento gerado à partir do " + cte.ModeloDocumentoFiscal.Abreviacao + " " + cte.Numero + "-" + cte.Serie.Numero + ".";

            if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoEmissao, dataMovimentacao, cte.ValorAReceber, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                return false;

            if (cte.ModeloDocumentoFiscal.DiferenciarMovimentosParaValorLiquido)
            {
                if (cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoPropriaNacional != null && cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoAgregadoNacional != null && cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoTerceiroNacional != null &&
                    cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoPropriaInternacional != null && cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoAgregadoInternacional != null && cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoTerceiroInternacional != null)
                {
                    Dominio.Entidades.Veiculo veiculoPrincipal = repCTe.BuscarPrimeiroVeiculo(cte.Codigo);
                    bool cteNacional = cte.LocalidadeInicioPrestacao.Pais == null || cte.LocalidadeTerminoPrestacao.Pais == null && (cte.LocalidadeInicioPrestacao.Pais.Sigla == "01058" && cte.LocalidadeTerminoPrestacao.Pais.Sigla == "01058");
                    bool cteProprio = veiculoPrincipal == null || veiculoPrincipal.Tipo == "P";
                    bool cteAgregado = false;
                    bool cteTerceiro = false;
                    if (!cteProprio)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(veiculoPrincipal?.Proprietario ?? null, unidadeTrabalho);

                        cteAgregado = modalidadeTerceiro != null && modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado;
                        cteTerceiro = modalidadeTerceiro == null || modalidadeTerceiro.TipoTransportador != TipoProprietarioVeiculo.TACAgregado;
                    }
                    if (cteNacional && cteProprio)
                    {
                        if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoPropriaNacional, dataMovimentacao, cte.ValorFrete, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                            return false;
                    }
                    else if (!cteNacional && cteProprio)
                    {
                        if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoPropriaInternacional, dataMovimentacao, cte.ValorFrete, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                            return false;
                    }
                    else if (cteNacional && cteAgregado)
                    {
                        if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoAgregadoNacional, dataMovimentacao, cte.ValorFrete, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                            return false;
                    }
                    else if (!cteNacional && cteAgregado)
                    {
                        if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoAgregadoInternacional, dataMovimentacao, cte.ValorFrete, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                            return false;
                    }
                    else if (cteNacional && cteTerceiro)
                    {
                        if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoTerceiroNacional, dataMovimentacao, cte.ValorFrete, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                            return false;
                    }
                    else if (!cteNacional && cteTerceiro)
                    {
                        if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissaoTerceiroInternacional, dataMovimentacao, cte.ValorFrete, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                            return false;
                    }
                }
                else
                {
                    if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissao, dataMovimentacao, cte.ValorFrete, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                        return false;
                }
            }

            decimal valorImposto = cte.ValorICMS;
            string imposto = "ICMS";

            if (cte.ModeloDocumentoFiscal != null && (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe))
            {
                valorImposto = cte.ValorISS;
                imposto = "ISS";
            }

            if (cte.ModeloDocumentoFiscal.DiferenciarMovimentosParaImpostos)
                if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoImpostoEmissao, dataMovimentacao, valorImposto, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: " + imposto + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                    return false;

            if (cte.ModeloDocumentoFiscal.DiferenciarMovimentosParaPIS)
                if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoPISEmissao, dataMovimentacao, cte.ValorPIS, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: PIS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                    return false;

            if (cte.ModeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS)
                if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoCOFINSEmissao, dataMovimentacao, cte.ValorCOFINS, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: COFINS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                    return false;

            if (cte.ModeloDocumentoFiscal.DiferenciarMovimentosParaIR)
                if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoIREmissao, dataMovimentacao, cte.ValorIR, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: IR.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                    return false;

            if (cte.ModeloDocumentoFiscal.DiferenciarMovimentosParaCSLL)
                if (!svcMovimento.GerarMovimentacao(out erro, cte.ModeloDocumentoFiscal.TipoMovimentoCSLLEmissao, dataMovimentacao, cte.ValorCSLL, cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Imposto: CSLL.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                    return false;

            if (cte.IncluirISSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && cte.ValorISS > 0m)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS);

                if (componenteFrete != null && componenteFrete.GerarMovimentoAutomatico)
                    if (!svcMovimento.GerarMovimentacao(out erro, componenteFrete.TipoMovimentoEmissao, dataMovimentacao, Math.Abs(cte.ValorISS), cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Componente: " + componenteFrete.Descricao + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                        return false;
            }
            else if (cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && cte.ValorICMS > 0m && cte.CST != "60")
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);

                if (componenteFrete != null && componenteFrete.GerarMovimentoAutomatico)
                    if (!svcMovimento.GerarMovimentacao(out erro, componenteFrete.TipoMovimentoEmissao, dataMovimentacao, Math.Abs(cte.ValorICMS), cte.Numero.ToString() + "-" + cte.Serie.Numero.ToString(), observacaoMovimentacao + " Componente: " + componenteFrete.Descricao + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe, tipoServicoMultisoftware, 0, null, null, 0, null, cte.TomadorPagador?.Cliente, cte.TomadorPagador?.GrupoPessoas, null, null, null, cte, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao.Emissao))
                        return false;
            }

            erro = string.Empty;
            return true;
        }

        private void enviarEmailDocumentosParadosNaFilaDeEnvio(string mensagem, string assunto, Repositorio.UnitOfWork unidadeTrabalho)
        {
            try
            {
                Servicos.Email serEmail = new Servicos.Email(unidadeTrabalho);
                Repositorio.Embarcador.Email.EmailAlerta repositorioEmailAlerta = new Repositorio.Embarcador.Email.EmailAlerta(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Email.EmailAlerta emailAlerta = repositorioEmailAlerta.BuscarPorTipoAlerta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaEmail.IntegradoresCTeMDFe);

                string cc = emailAlerta.EmailsCopia;
#if DEBUG
                cc = string.Empty;
#endif

                serEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, emailAlerta.EmailPrincipal, string.Empty, cc, assunto, mensagem, string.Empty, null, string.Empty, true, string.Empty, 0, unidadeTrabalho, 0, true, null, false);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

        }

        private void GerarControleFaturamentoDocumentos(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (carga.CargaTransbordo || carga.GerouControleFaturamento)
                return;

            if (carga?.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga)
                return;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            if (Carga.VerificarSeGeraMovimentacaoAgrupadaPorPedido(carga, unidadeTrabalho))
            {
                unidadeTrabalho.Start();
                Servicos.Log.TratarErro($"Inicio Geração Documento Faturamento por Carga ({carga.CodigoCargaEmbarcador})", "DocumentoFaturamento");
                Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorCarga(carga, unidadeTrabalho);

                carga.GerouControleFaturamento = true;

                repCarga.Atualizar(carga);

                unidadeTrabalho.CommitChanges();
            }
            else
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

                List<int> codigosCargaCTes = null;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    codigosCargaCTes = repCargaCTe.BuscarPorCargaSemComplementaresComFaturamentoEQueNaoGeraramControleFaturamento(carga.Codigo);
                else
                    codigosCargaCTes = repCargaCTe.BuscarPorCargaSemComplementaresComFaturamentoQueGeram(carga.Codigo);

                foreach (int codigoCargaCTe in codigosCargaCTes)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                    unidadeTrabalho.Start();

                    bool provisonarPorNotaFiscal = false;
                    if (cargaCTe.CargaCTeFilialEmissora != null || carga.CargaGeradaComCTeAnteriorFilialEmissora)
                        provisonarPorNotaFiscal = true;

                    bool cteFilialEmissora = false;
                    if (cargaCTe.CargaCTeSubContratacaoFilialEmissora != null)
                        cteFilialEmissora = true;


                    Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorDocumento(cargaCTe.CargaOrigem, cargaCTe.CTe, null, null, null, null, provisonarPorNotaFiscal, cteFilialEmissora, carga.SituacaoLiberacaoEscrituracaoPagamentoCarga, false, configuracao, unidadeTrabalho, tipoServicoMultisoftware);

                    if (documentoFaturamento != null && (cargaCTe.PreCTe?.PagamentoLiberado ?? false))
                    {
                        documentoFaturamento.PagamentoDocumentoBloqueado = false;
                        documentoFaturamento.DataLiberacaoPagamento = cargaCTe.PreCTe.DataLiberacaoPagamento;
                        repDocumentoFaturamento.Atualizar(documentoFaturamento);
                    }

                    cargaCTe.GerouControleFaturamento = true;

                    repCargaCTe.Atualizar(cargaCTe);

                    unidadeTrabalho.CommitChanges();
                    unidadeTrabalho.FlushAndClear();
                }

                carga = repCarga.BuscarPorCodigo(carga.Codigo);

                carga.GerouControleFaturamento = true;

                repCarga.Atualizar(carga);
            }
        }

        public void GerarTitulosAutorizacaoCarga(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            if (carga.CargaTransbordo || carga.GerouTituloAutorizacao)
                return;

            if (!Carga.VerificarSeGeraTituloAutomaticamente(carga, unidadeTrabalho, out bool gerarFaturamentoAVista, out bool gerarBoletoAutomaticamente, out int codigoBoletoConfiguracao, out bool enviarBoletoPorEmailAutomaticamente, out bool enviarDocumentacaoFaturamentoCTe))
                return;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unidadeTrabalho);

            if (repCargaCTe.ContarPorCargaQueGeraFaturamento(carga.Codigo) > 0)
            {
                bool pagamentoBloqueado = configuracao.GerarPagamentoBloqueado;

                if (pagamentoBloqueado && (carga?.Filial?.LiberarAutomaticamentePagamento ?? true))
                    pagamentoBloqueado = false;

                if (Carga.VerificarSeGeraMovimentacaoAgrupadaPorPedido(carga, unidadeTrabalho))
                {
                    unidadeTrabalho.Start();

                    servicoTitulo.GerarTituloPorCarga(carga, tipoServicoMultisoftware, gerarFaturamentoAVista, gerarBoletoAutomaticamente, codigoBoletoConfiguracao, enviarBoletoPorEmailAutomaticamente, enviarDocumentacaoFaturamentoCTe, pagamentoBloqueado, Auditado);

                    carga.GerouTituloAutorizacao = true;

                    repCarga.Atualizar(carga);

                    unidadeTrabalho.CommitChanges();
                }
                else
                {
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura repConfiguracaoFinanceiraFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura(unidadeTrabalho);
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFinanceiraFatura = repConfiguracaoFinanceiraFatura.BuscarPrimeiroRegistro();
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

                    List<int> codigosCargaCTes = repCargaCTe.BuscarCodigosPorCargaSemComplementaresComFaturamentoEQueNaoGeraramTitulos(carga.Codigo);

                    foreach (int codigoCargaCTe in codigosCargaCTes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                        if (cargaCTe.GerouTituloAutorizacao)
                            continue;

                        Dominio.Entidades.Cliente tomador = cargaCTe.CTe.TomadorPagador.Cliente;
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = cargaCTe.CTe.TomadorPagador?.GrupoPessoas;

                        unidadeTrabalho.Start();

                        servicoTitulo.GerarTituloPorDocumento(cargaCTe.CTe, cargaPedido, grupoPessoas, tomador, configuracaoFinanceiraFatura, tipoServicoMultisoftware, gerarFaturamentoAVista, gerarBoletoAutomaticamente, pagamentoBloqueado, codigoBoletoConfiguracao, enviarBoletoPorEmailAutomaticamente, enviarDocumentacaoFaturamentoCTe, Auditado, 0);

                        cargaCTe.GerouTituloAutorizacao = true;

                        repCargaCTe.Atualizar(cargaCTe);

                        unidadeTrabalho.CommitChanges();

                        unidadeTrabalho.FlushAndClear();
                    }

                    carga = repCarga.BuscarPorCodigo(carga.Codigo);

                    carga.GerouTituloAutorizacao = true;

                    repCarga.Atualizar(carga);
                }
            }
        }

        private void GerarFaturamentoTakeOrPay(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ||
                !carga.CargaTakeOrPay ||
                carga.CargaTransbordo ||
                carga.GerouFaturamentoTakeOrPay)
                return;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unidadeTrabalho);

            if (repCargaCTe.ContarPorCargaQueGeraFaturamento(carga.Codigo) > 0)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = new Dominio.Entidades.Embarcador.Fatura.FaturamentoLote()
                {
                    Cliente = null,
                    DataFatura = DateTime.Now.Date,
                    DataFinal = null,
                    DataGeracao = DateTime.Now,
                    DataInicial = null,
                    Destino = null,
                    GrupoPessoas = null,
                    NumeroBooking = string.Empty,
                    Observacao = string.Empty,
                    Origem = null,
                    PedidoViagemNavio = carga.PedidoViagemNavio,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Aguardando,
                    TerminalDestino = carga.TerminalDestino,
                    TerminalOrigem = carga.TerminalOrigem,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoLote.Faturamento,
                    TipoOperacao = null,
                    TipoPessoa = TipoPessoa.Pessoa,
                    Usuario = null,
                    Empresa = carga.PortoOrigem?.Empresa ?? null,
                    FaturamentoAutomatico = true,
                    NotificadoOperador = true,
                    DataAFaturar = DateTime.Now,
                    Carga = carga,
                    TipoCTe = Dominio.Enumeradores.TipoCTE.Normal
                };

                if (faturamentoLote.TipoPropostaMultimodal == null)
                    faturamentoLote.TipoPropostaMultimodal = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>();

                faturamentoLote.TipoPropostaMultimodal.Add(TipoPropostaMultimodal.TAkePayCabotagem);
                faturamentoLote.TipoPropostaMultimodal.Add(TipoPropostaMultimodal.TakePayFeeder);
                faturamentoLote.TipoPropostaMultimodal.Add(TipoPropostaMultimodal.NoShowCabotagem);
                faturamentoLote.TipoPropostaMultimodal.Add(TipoPropostaMultimodal.FaturamentoContabilidade);
                faturamentoLote.TipoPropostaMultimodal.Add(TipoPropostaMultimodal.DemurrageCabotagem);
                faturamentoLote.TipoPropostaMultimodal.Add(TipoPropostaMultimodal.DetentionCabotagem);

                repFaturamentoLote.Inserir(faturamentoLote);

                carga = repCarga.BuscarPorCodigo(carga.Codigo);

                carga.GerouTituloGNREAutorizacao = true;

                repCarga.Atualizar(carga);
            }
        }

        private void GerarTitulosGNRECarga(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (carga.TipoOperacao != null && (carga?.TipoOperacao?.NaoGerarTituloGNREAutomatico ?? false))
                return;
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ||
                carga.CargaTransbordo ||
                carga.GerouTituloGNREAutorizacao)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE repConfiguracaoFinanceiraGNRE = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE configuracaoFinanceiraGNRE = repConfiguracaoFinanceiraGNRE.BuscarPrimeiroRegistro();

            if (configuracaoFinanceiraGNRE == null || !configuracaoFinanceiraGNRE.GerarGNREParaCTesEmitidos || !configuracaoFinanceiraGNRE.GerarGNREAutomaticamente)
                return;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            if (repCargaCTe.ContarPorCargaQueGeraFaturamento(carga.Codigo) > 0)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro repConfiguracaoFinanceiraGNRERegistro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro> configuracaoFinanceiraGNRERegistros = repConfiguracaoFinanceiraGNRERegistro.BuscarPorConfiguracao(configuracaoFinanceiraGNRE.Codigo);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

                List<int> codigosCargaCTes = repCargaCTe.BuscarCodigosPorCargaSemComplementaresComFaturamentoEQueNaoGeraramTitulosGNRE(carga.Codigo);

                foreach (int codigoCargaCTe in codigosCargaCTes)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                    if (cargaCTe.GerouTituloGNREAutorizacao)
                        continue;

                    unidadeTrabalho.Start();

                    Servicos.Embarcador.Financeiro.Titulo.GerarTituloGNRE(cargaCTe, cargaPedido, configuracaoFinanceiraGNRERegistros, unidadeTrabalho, tipoServicoMultisoftware, configuracaoFinanceiro);

                    cargaCTe.GerouTituloGNREAutorizacao = true;

                    repCargaCTe.Atualizar(cargaCTe);

                    unidadeTrabalho.CommitChanges();

                    unidadeTrabalho.FlushAndClear();
                }

                carga = repCarga.BuscarPorCodigo(carga.Codigo);

                carga.GerouTituloGNREAutorizacao = true;

                repCarga.Atualizar(carga);
            }
        }

        public void GerarMovimentosAutorizacaoCarga(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool gerarParaCancelamentosEAnulacoes, bool controlarTransacao)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            if (carga.CargaTransbordo || carga.GerouMovimentacaoAutorizacao)
                return;

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

            if (repCargaCTe.ContarPorCarga(carga.Codigo) > 0) // carga.CargaCTes.Count > 0)
            {
                if (Servicos.Embarcador.Carga.Carga.VerificarSeGeraMovimentacaoAgrupadaPorPedido(carga, unidadeTrabalho))
                {
                    if (controlarTransacao)
                        unidadeTrabalho.Start();

                    GerarMovimentoEmissaoCarga(carga, tipoServicoMultisoftware, unidadeTrabalho, gerarParaCancelamentosEAnulacoes);

                    carga.GerouMovimentacaoAutorizacao = true;

                    repCarga.Atualizar(carga);

                    if (controlarTransacao)
                        unidadeTrabalho.CommitChanges();
                }
                else
                {
                    List<int> codigosCargaCTes = repCargaCTe.BuscarCodigosPorCargaSemComplementaresEQueNaoGeraramMovimentosAutorizacao(carga.Codigo);

                    foreach (int codigoCargaCTe in codigosCargaCTes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                        if (cargaCTe.GerouMovimentacaoAutorizacao)
                            continue;

                        if (controlarTransacao)
                            unidadeTrabalho.Start();

                        GerarMovimentoEmissaoCTe(cargaCTe, tipoServicoMultisoftware, unidadeTrabalho, gerarParaCancelamentosEAnulacoes);

                        cargaCTe.GerouMovimentacaoAutorizacao = true;

                        repCargaCTe.Atualizar(cargaCTe);

                        if (controlarTransacao)
                        {
                            unidadeTrabalho.CommitChanges();

                            unidadeTrabalho.FlushAndClear();
                        }
                    }

                    if (controlarTransacao)
                        carga = repCarga.BuscarPorCodigo(carga.Codigo);

                    carga.GerouMovimentacaoAutorizacao = true;

                    repCarga.Atualizar(carga);
                }
            }
        }

        private void GerarMovimentoEmissaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, bool gerarParaCancelamentosEAnulacoes)
        {
            if (carga.CargaTransbordo)
                return;

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcMovimento = new Financeiro.ProcessoMovimento(StringConexao);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentes = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModeloDocumento = new Repositorio.ModeloDocumentoFiscal(unidadeTrabalho);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeTrabalho);

            string[] situacoesCTes = null;

            if (gerarParaCancelamentosEAnulacoes)
                situacoesCTes = new string[] { "A", "C", "Z" };
            else
                situacoesCTes = new string[] { "A" };

            List<DateTime> datasMovimentacoes = repCargaCTe.BuscarDatasAutorizacaoPorCarga(carga.Codigo, situacoesCTes);

            List<int> codigosComponentesFrete = repCargaCTeComponentes.BuscarCodigoComponenteFreteQueGeraMovimentacaoPorCarga(carga.Codigo, situacoesCTes);
            List<int> codigosModelosDocumento = repCargaCTe.BuscarCodigoModeloDocumentoPorCarga(carga.Codigo, situacoesCTes);

            foreach (int codigoModeloDocumento in codigosModelosDocumento)
            {
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumento = repModeloDocumento.BuscarPorId(codigoModeloDocumento);

                foreach (DateTime dataMovimentacao in datasMovimentacoes)
                {
                    decimal valorMovimentacao = repCargaCTe.BuscarValorTotalReceberPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, situacoesCTes, dataMovimentacao, null, null);
                    string observacaoMovimentacao = "Movimento gerado à partir dos documentos " + modeloDocumento.Abreviacao + " da carga " + carga.CodigoCargaEmbarcador + ".";

                    svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoEmissao, dataMovimentacao, valorMovimentacao, carga.CodigoCargaEmbarcador, observacaoMovimentacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);

                    if (modeloDocumento.DiferenciarMovimentosParaValorLiquido)
                    {
                        decimal valorFreteLiquido = repCargaCTe.BuscarValorFreteLiquidoPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, situacoesCTes, dataMovimentacao, null, null);

                        if (modeloDocumento.TipoMovimentoValorLiquidoEmissaoPropriaNacional != null && modeloDocumento.TipoMovimentoValorLiquidoEmissaoAgregadoNacional != null && modeloDocumento.TipoMovimentoValorLiquidoEmissaoTerceiroNacional != null &&
                            modeloDocumento.TipoMovimentoValorLiquidoEmissaoPropriaInternacional != null && modeloDocumento.TipoMovimentoValorLiquidoEmissaoAgregadoInternacional != null && modeloDocumento.TipoMovimentoValorLiquidoEmissaoTerceiroInternacional != null)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico primeiroCTe = repCargaCTe.BuscarPrimeiroCTePorCarga(carga.Codigo);
                            bool cteNacional = primeiroCTe != null && primeiroCTe.LocalidadeInicioPrestacao.Pais == null || primeiroCTe.LocalidadeTerminoPrestacao.Pais == null && (primeiroCTe.LocalidadeInicioPrestacao.Pais.Sigla == "01058" && primeiroCTe.LocalidadeTerminoPrestacao.Pais.Sigla == "01058");
                            bool cteProprio = carga.Terceiro == null;
                            bool cteAgregado = false;
                            bool cteTerceiro = false;
                            if (!cteProprio)
                            {
                                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(carga?.Terceiro ?? null, unidadeTrabalho);

                                cteAgregado = modalidadeTerceiro != null && modalidadeTerceiro.TipoTransportador == TipoProprietarioVeiculo.TACAgregado;
                                cteTerceiro = modalidadeTerceiro == null || modalidadeTerceiro.TipoTransportador != TipoProprietarioVeiculo.TACAgregado;
                            }
                            if (cteNacional && cteProprio)
                                svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoValorLiquidoEmissaoPropriaNacional, dataMovimentacao, valorFreteLiquido, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);
                            else if (!cteNacional && cteProprio)
                                svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoValorLiquidoEmissaoPropriaInternacional, dataMovimentacao, valorFreteLiquido, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);
                            else if (cteNacional && cteAgregado)
                                svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoValorLiquidoEmissaoAgregadoNacional, dataMovimentacao, valorFreteLiquido, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);
                            else if (!cteNacional && cteAgregado)
                                svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoValorLiquidoEmissaoAgregadoInternacional, dataMovimentacao, valorFreteLiquido, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);
                            else if (cteNacional && cteTerceiro)
                                svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoValorLiquidoEmissaoTerceiroNacional, dataMovimentacao, valorFreteLiquido, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);
                            else if (!cteNacional && cteTerceiro)
                                svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoValorLiquidoEmissaoTerceiroInternacional, dataMovimentacao, valorFreteLiquido, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);


                        }
                        else
                        {
                            svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoValorLiquidoEmissao, dataMovimentacao, valorFreteLiquido, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Valor do frete líquido.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);
                        }
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaImpostos)
                    {
                        decimal valorMovimentacaoImpostos = repCargaCTe.BuscarValorICMSPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, situacoesCTes, dataMovimentacao, null, null);

                        svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoImpostoEmissao, dataMovimentacao, valorMovimentacaoImpostos, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: ICMS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaPIS)
                    {
                        decimal valorMovimentacaoPIS = repCargaCTe.BuscarValorPISPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, situacoesCTes, dataMovimentacao, null, null);

                        svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoPISEmissao, dataMovimentacao, valorMovimentacaoPIS, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: PIS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaCOFINS)
                    {
                        decimal valorMovimentacaoCOFINS = repCargaCTe.BuscarValorCOFINSPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, situacoesCTes, dataMovimentacao, null, null);

                        svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoCOFINSEmissao, dataMovimentacao, valorMovimentacaoCOFINS, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: COFINS.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaIR)
                    {
                        decimal valorMovimentacaoIR = repCargaCTe.BuscarValorIRPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, situacoesCTes, dataMovimentacao, null, null);

                        svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoIREmissao, dataMovimentacao, valorMovimentacaoIR, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: IR.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);
                    }

                    if (modeloDocumento.DiferenciarMovimentosParaCSLL)
                    {
                        decimal valorMovimentacaoCSLL = repCargaCTe.BuscarValorCSLLPorCargaEModeloDocumento(carga.Codigo, codigoModeloDocumento, situacoesCTes, dataMovimentacao, null, null);

                        svcMovimento.GerarMovimentacao(modeloDocumento.TipoMovimentoCSLLEmissao, dataMovimentacao, valorMovimentacaoCSLL, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Imposto: CSLL.", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);
                    }

                    foreach (int codigoComponenteFrete in codigosComponentesFrete)
                    {
                        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo(codigoComponenteFrete);

                        decimal valorMovimentacaoComponente = Math.Abs(repCargaCTeComponentes.BuscarValorComponentePorCargaEModeloDocumento(carga.Codigo, codigoComponenteFrete, codigoModeloDocumento, situacoesCTes, dataMovimentacao, null, null));

                        svcMovimento.GerarMovimentacao(componenteFrete.TipoMovimentoEmissao, dataMovimentacao, valorMovimentacaoComponente, carga.CodigoCargaEmbarcador, observacaoMovimentacao + " Componente: " + componenteFrete.Descricao + ".", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Carga, tipoServicoMultisoftware);
                    }
                }
            }
        }

        public bool IntegrouCIOTCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool cargaPossuiMDFe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(carga.Codigo);

            bool integrouCIOT = true;
            SituacaoRetornoCIOT retAbrirCIOT = SituacaoRetornoCIOT.Autorizado;

            if (cargaCIOT != null)
            {
                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.CIOT.CIOT svcCIOT = new Servicos.Embarcador.CIOT.CIOT();

                string motivoRejeicaoCIOT = null;

                if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia)
                {
                    if (!cargaCIOT.Carga.LiberadoComProblemaCIOT)
                        retAbrirCIOT = SituacaoRetornoCIOT.ProblemaIntegracao;
                }
                else if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao)
                {
                    retAbrirCIOT = svcCIOT.AbrirCIOT(cargaCIOT.CIOT, cargaCIOT, tipoServicoMultisoftware, unitOfWork);
                }
                else if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto ||
                         cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem)
                {
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (!cargaCIOT.Carga.LiberadoComProblemaCIOT && cargaCIOT.CIOT.CIOTPorPeriodo && !cargaCIOT.CargaAdicionadaAoCIOT)
                        {
                            if (cargaCIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia)
                                retAbrirCIOT = SituacaoRetornoCIOT.ProblemaIntegracao;
                            else if ((cargaCIOT.Situacao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao) == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao)
                                retAbrirCIOT = svcCIOT.AdicionarViagem(cargaCIOT, tipoServicoMultisoftware, unitOfWork, out motivoRejeicaoCIOT);
                        }
                    }
                    else
                    {
                        if (!cargaCIOT.Carga.LiberadoComProblemaCIOT && !cargaCIOT.CargaAdicionadaAoCIOT)
                        {
                            if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia)
                                retAbrirCIOT = SituacaoRetornoCIOT.ProblemaIntegracao;
                            else if ((cargaCIOT.Situacao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao) == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao)
                                retAbrirCIOT = svcCIOT.AdicionarViagem(cargaCIOT, tipoServicoMultisoftware, unitOfWork, out motivoRejeicaoCIOT);
                        }
                    }
                }

                if (retAbrirCIOT == SituacaoRetornoCIOT.ProblemaIntegracao)
                {
                    if (!string.IsNullOrWhiteSpace(motivoRejeicaoCIOT))
                        carga.MotivoPendencia = motivoRejeicaoCIOT;
                    else
                        carga.MotivoPendencia = "Falha ao abrir o CIOT.";

                    carga.PossuiPendencia = true;
                    carga.ProblemaIntegracaoCIOT = true;
                    carga.LiberadoComProblemaCIOT = false;

                    integrouCIOT = false;
                    repCarga.Atualizar(carga);

                    serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                    if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                        serCarga.NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.FalhaAbrirCIOTCarga, carga.CodigoCargaEmbarcador), unitOfWork);
                }
                else if (retAbrirCIOT == SituacaoRetornoCIOT.Autorizado)
                {
                    carga.IntegrandoCIOT = false;
                    repCarga.Atualizar(carga);

                    integrouCIOT = true;
                }
                else
                {
                    integrouCIOT = false;
                }
            }
            else if (cargaPossuiMDFe && (carga.CargaCTes?.Any() ?? false) && (carga.EmpresaFilialEmissora == null || carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora) && !carga.LiberadoComProblemaCIOT && carga.Veiculo != null && (carga.Veiculo.Proprietario != null || carga.Veiculo.Empresa.EquiparadoTAC))
            {
                bool gerouCIOT = AtualizarInformacoesCIOT(carga, unitOfWork);

                if (!gerouCIOT && (configuracaoGeralCarga.ObrigatoriedadeCIOTEmissaoMDFe || (carga.Empresa?.Configuracao?.ObrigatoriedadeCIOTEmissaoMDFe ?? false)))
                {
                    carga.MotivoPendencia = "Para emitir o MDF-e, é necessário informar o número do CIOT e os dados de pagamento. Clique na aba CIOT para preencher ou validar as informações.";
                    carga.PossuiPendencia = true;
                    carga.ProblemaIntegracaoCIOT = true;
                    carga.LiberadoComProblemaCIOT = false;
                    integrouCIOT = false;
                    repCarga.Atualizar(carga);
                }
            }

            return integrouCIOT;
        }

        public bool IntegrouValePedagioCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagioIntegracao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaValePedagios = repCargaValePedagio.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.Stage stagesCarga = repositorioStage.BuscarPrimeiraPorCarga(carga.Codigo);

            bool integrouValePedagio = true;
            bool integracaoRejeitada = false;
            if (cargaValePedagios.Count > 0 && (carga.EmpresaFilialEmissora == null || carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora))
            {
                //if(carga.IntegrandoValePedagio == false)
                //{
                if (cargaValePedagios.Any(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao))
                    integracaoRejeitada = true;
                else if (cargaValePedagios.Any(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno))
                    integrouValePedagio = false;
                else if (cargaValePedagios.Any(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao))
                {
                    integrouValePedagio = false;
                    carga.IntegrandoValePedagio = true;
                    carga.ProblemaIntegracaoValePedagio = false;
                    repCarga.Atualizar(carga);
                }
                //}

                if (integracaoRejeitada && !carga.LiberadoComProblemaValePedagio)
                {
                    carga.MotivoPendencia = "Falha ao Integrar o Vale Pedágio.";
                    carga.PossuiPendencia = true;
                    carga.ProblemaIntegracaoValePedagio = true;
                    integrouValePedagio = false;
                    repCarga.Atualizar(carga);

                    serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                    if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                        serCarga.NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.FalhaIntegrarValePedagioCarga, carga.CodigoCargaEmbarcador), unitOfWork);
                }
            }

            if (cargaValePedagios.Count == 0 && (carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.NaoPermitirLiberarSemValePedagio ?? false)
                && !carga.LiberadoComProblemaValePedagio)
            {
                carga.MotivoPendencia = "Vale Pedágio Não recebido Via integração";
                carga.PossuiPendencia = true;
                carga.ProblemaIntegracaoValePedagio = true;
                integrouValePedagio = false;
                repCarga.Atualizar(carga);

                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
            }
            else if (cargaValePedagios.Count == 0 && (carga?.TipoOperacao?.ConfiguracaoEmissaoDocumento?.NaoPermitirLiberarSemValePedagio ?? false)
              && !carga.LiberadoComProblemaValePedagio && serCarga.VerificarCargaSubTrechoTransferenciaEntrega(carga, unitOfWork))
            {
                carga.MotivoPendencia = "Vale Pedágio Não recebido Via integração";
                carga.PossuiPendencia = true;
                carga.ProblemaIntegracaoValePedagio = true;
                integrouValePedagio = false;
                repCarga.Atualizar(carga);

                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
            }


            return integrouValePedagio || carga.LiberadoComProblemaValePedagio;
        }

        public bool AtualizarInformacoesCIOT(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            bool gerouCIOT = false;

            Servicos.Embarcador.CIOT.CIOT servicoCIOT = new Servicos.Embarcador.CIOT.CIOT();
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork).BuscarPrimeiroPorCarga(carga.Codigo);
            Dominio.Entidades.Global.PedidoInformacoesBancarias informacaoBancariaPedido = new Repositorio.Embarcador.Pedidos.PedidoInformacoesBancarias(unitOfWork).BuscarPorPedido(pedido.Codigo);


            if (!string.IsNullOrWhiteSpace(pedido.CIOT) && informacaoBancariaPedido != null)
            {
                gerouCIOT = servicoCIOT.GerarCIOTAutomatico(
                    carga,
                    informacaoBancariaPedido.TipoPagamento,
                    informacaoBancariaPedido.ValorFrete,
                    informacaoBancariaPedido.ValorAdiantamento,
                    informacaoBancariaPedido.DataVencimentoCIOT,
                    informacaoBancariaPedido.TipoInformacaoBancaria,
                    informacaoBancariaPedido.Ipef,
                    informacaoBancariaPedido.Agencia,
                    informacaoBancariaPedido.Conta,
                    informacaoBancariaPedido.ChavePIX,
                    pedido.CIOT,
                    pedido.CIOT,
                    _tipoServicoMultisoftware,
                    unitOfWork
                );
            }

            if (!gerouCIOT && !string.IsNullOrEmpty(carga.Veiculo.CIOT))
            {
                Dominio.Entidades.Veiculo cargaVeiculo = carga.Veiculo;
                string cnpjIpef = cargaVeiculo.CNPJInstituicaoPagamentoCIOT ?? cargaVeiculo.Proprietario?.CnpjIpef ?? ""; 
                string agencia = cargaVeiculo.AgenciaCIOT ?? cargaVeiculo.Proprietario?.Agencia ?? ""; 
                string banco = cargaVeiculo.ContaCIOT ?? cargaVeiculo.Proprietario?.Banco?.Numero.ToString() ?? "";
                string chavePix = cargaVeiculo.ChavePIXCIOT ?? cargaVeiculo.Proprietario?.ChavePix ?? "";

                gerouCIOT = servicoCIOT.GerarCIOTAutomatico(carga, cargaVeiculo.FormaPagamentoCIOT, cargaVeiculo.ValorFreteCIOT, cargaVeiculo.ValorAdiantamentoCIOT, cargaVeiculo.DataVencimentoCIOT, cargaVeiculo.TipoPagamentoCIOT, cnpjIpef, agencia, banco, chavePix, cargaVeiculo.CIOT, cargaVeiculo.CIOT, _tipoServicoMultisoftware, unitOfWork);
            }

            return gerouCIOT;
        }

        private bool IntegrouPagamentoMotoristaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repositorioPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Hubs.Carga serHubCarga = new Hubs.Carga();
            Carga serCarga = new Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> pagamentos = repositorioPagamentoMotorista.BuscarPorCarga(carga.Codigo);

            bool integrouPagamento = true;
            bool integracaoRejeitada = false;

            if (pagamentos.Count > 0)
            {

                if (pagamentos.Any(obj => obj.PagamentoLiberado == false))
                {
                    for (int i = 0; i < pagamentos.Count(); i++)
                    {
                        if (pagamentos[i].PagamentoLiberado == false)
                        {
                            pagamentos[i].PagamentoLiberado = true;
                            repositorioPagamentoMotorista.Atualizar(pagamentos[i]);
                        }
                    }
                    carga.AgIntegracaoPagamentoMotorista = true;
                    repCarga.Atualizar(carga);
                }

                if (pagamentos.Any(obj => obj.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FalhaIntegracao))
                    integracaoRejeitada = true;
                else if (pagamentos.Any(obj => obj.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Finalizada || obj.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento))
                {
                    integrouPagamento = true;
                    //carga.IntegrandoPagamentoMotorista = true;
                    carga.ProblemaIntegracaoPagamentoMotorista = false;
                    repCarga.Atualizar(carga);
                }
                if (integracaoRejeitada && !carga.LiberadoComProblemaPagamentoMotorista)
                {
                    carga.MotivoPendencia = "Falha ao Integrar o Pagamento do Motorista.";
                    carga.PossuiPendencia = true;
                    carga.ProblemaIntegracaoPagamentoMotorista = true;
                    integrouPagamento = false;
                    repCarga.Atualizar(carga);

                    serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                    if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                        serCarga.NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.FalhaIntegrarPagamentoMotoristaCarga, carga.CodigoCargaEmbarcador), unitOfWork);
                }
            }

            return integrouPagamento || carga.LiberadoComProblemaPagamentoMotorista;
        }

        private void GerarCanhotosCTes(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (carga.GerouCanhoto)
                return;

            if (carga.TipoOperacao?.NaoGerarCanhoto ?? false)
                return;

            Dominio.Entidades.Cliente tomador = cargaPedidos.FirstOrDefault()?.ObterTomador();

            if (carga.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
            {
                if (!carga.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.ArmazenaCanhotoFisicoCTe ?? false)
                    return;
            }
            else if (tomador?.NaoUsarConfiguracaoFaturaGrupo ?? false)
            {
                if (!tomador.ArmazenaCanhotoFisicoCTe.HasValue || tomador.ArmazenaCanhotoFisicoCTe.Value == false)
                    return;
            }
            else if (carga.GrupoPessoaPrincipal == null || !carga.GrupoPessoaPrincipal.ArmazenaCanhotoFisicoCTe.HasValue || carga.GrupoPessoaPrincipal.ArmazenaCanhotoFisicoCTe.Value == false)
                return;

            Servicos.Embarcador.Canhotos.Canhoto svcCanhoto = new Canhotos.Canhoto(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<int> codigosCargaCTes = repCargaCTe.BuscarCodigosPorCargaSemComplementaresEQueNaoGeraramCanhotos(carga.Codigo);

            foreach (int codigoCargaCTe in codigosCargaCTes)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                if (cargaCTe.GerouCanhoto)
                    continue;

                unitOfWork.Start();

                svcCanhoto.SalvarCanhotoCargaCTe(cargaCTe, tipoServicoMultisoftware, unitOfWork);

                cargaCTe.GerouCanhoto = true;

                repCargaCTe.Atualizar(cargaCTe);

                unitOfWork.CommitChanges();

                unitOfWork.FlushAndClear();
            }

            carga = repCarga.BuscarPorCodigo(carga.Codigo);

            carga.GerouCanhoto = true;

            repCarga.Atualizar(carga);

            return;
        }

        private void AdicionarIndicadorIntegracaoCTe(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            bool transacaoIniciada = unitOfWork.IsActiveTransaction();

            try
            {
                if (!transacaoIniciada)
                    unitOfWork.Start();

                Integracao.IndicadorIntegracaoCTe servicoIndicadorIntegracaoCTe = new Integracao.IndicadorIntegracaoCTe(unitOfWork);

                servicoIndicadorIntegracaoCTe.Adicionar(carga);

                if (!transacaoIniciada)
                    unitOfWork.CommitChanges();
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                if (!transacaoIniciada)
                    unitOfWork.Rollback();
            }
        }

        private bool EmitirNFeRemessa(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            if (!configuracaoEmbarcador.EmitirNFeRemessaNaCarga)
                return true;

            // Repositorios
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaNFe repCargaNFe = new Repositorio.Embarcador.Cargas.CargaNFe(unitOfWork);
            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            bool autorizouTodasNotas = true;
            bool notaRejeitada = false;
            bool processandoNotas = false;
            bool contemNotasAguardandoEmissao = false;

            if (repCargaNFe.ContarPorCargaEStatus(carga.Codigo, Dominio.Enumeradores.StatusNFe.Emitido) > 0)
            {
                autorizouTodasNotas = false;
                contemNotasAguardandoEmissao = true;
            }

            if (repCargaNFe.ContarPorCargaEStatus(carga.Codigo, Dominio.Enumeradores.StatusNFe.Rejeitado) > 0)
                notaRejeitada = true;

            if (repCargaNFe.ContarPorCargaEStatus(carga.Codigo, Dominio.Enumeradores.StatusNFe.Emitido) > 0)
                processandoNotas = true;

            if (repCargaNFe.ContarPorCargaEStatus(carga.Codigo, Dominio.Enumeradores.StatusNFe.EmProcessamento) > 0)
                processandoNotas = true;

            // Atualzia erro de averbacao
            if (notaRejeitada && !contemNotasAguardandoEmissao)
            {
                carga.MotivoPendencia = "Erro ao gerar notas de remessa da carga.";
                carga.PossuiPendencia = true;
                carga.EmitindoNFeRemessa = true;
                carga.problemaEmissaoNFeRemessa = true;

                autorizouTodasNotas = false;
                processandoNotas = false;
            }
            else
            {
                carga.problemaEmissaoNFeRemessa = false;
            }

            // Atuliza processo
            if (processandoNotas)
            {
                carga.EmitindoNFeRemessa = true;

                autorizouTodasNotas = false;
            }
            else if (autorizouTodasNotas)
            {
                carga.EmitindoNFeRemessa = false;
            }

            // Atualiza carga
            repCarga.Atualizar(carga);

            if (notaRejeitada)
            {
                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                    serCarga.NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.ErroGerarNotasRemessaCarga, carga.CodigoCargaEmbarcador), unitOfWork);
            }

            return autorizouTodasNotas;
        }

        private bool AverbarCTeCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, bool averbacaoOperacaoContainer = false)
        {
            // Quando há uma averbação com erro:
            // Seta a flag problema averbacao como true
            //
            // Quando há uma averbacao enviada
            // Seta a flag Averbando cte como true

            // Repositorios
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAverbacao repPedidoAverbacao = new Repositorio.Embarcador.Pedidos.PedidoAverbacao(unitOfWork);

            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            bool bloquerCargaComRejeicaoAverbacao = false;

            if (configuracaoEmbarcador.NaoPermiteEmitirCargaSemAverbacao && !(carga.TipoOperacao?.PermitirCargaSemAverbacao ?? false))
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao> listaAverbacaoPedidos = repPedidoAverbacao.BuscarPorCarga(carga.Codigo);

                bloquerCargaComRejeicaoAverbacao = true;
                Dominio.Entidades.Empresa empresa = carga.Empresa;

                if (carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    empresa = carga.EmpresaFilialEmissora;

                if (empresa != null && empresa.LiberarEmissaoSemAverbacao)
                    bloquerCargaComRejeicaoAverbacao = false;

                if (carga.LiberarComProblemaAverbacao)
                    bloquerCargaComRejeicaoAverbacao = false;

                if (listaAverbacaoPedidos != null && listaAverbacaoPedidos.Count > 0)
                    bloquerCargaComRejeicaoAverbacao = false;
            }

            //todo: se precisar criar regras que bloqueiam a carga se não averbar deve faze-la aqui.
            //if (carga.Empresa.EmpresaPai != null && carga.Empresa.EmpresaPai.Configuracao.VersaoMDFe == "3.0") 
            //bloquerCargaComRejeicaoAverbacao = true;

            bool averbouTodosCTe = true;
            bool averbacaoRejeitada = false;
            bool averbandoCTes = false;

            Dominio.Enumeradores.TipoAverbacaoCTe tipoAverbacao = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;

            if (repAverbacaoCTe.ContarPorCargaTipoEStatus(carga.Codigo, tipoAverbacao, new Dominio.Enumeradores.StatusAverbacaoCTe[] { Dominio.Enumeradores.StatusAverbacaoCTe.Pendente, Dominio.Enumeradores.StatusAverbacaoCTe.AgEmissao }, averbacaoOperacaoContainer) > 0)
                averbouTodosCTe = false;

            if (bloquerCargaComRejeicaoAverbacao && repAverbacaoCTe.ContarPorCargaTipoEStatus(carga.Codigo, tipoAverbacao, Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao, averbacaoOperacaoContainer) > 0)
                averbacaoRejeitada = true;

            if (repAverbacaoCTe.ContarPorCargaTipoEStatus(carga.Codigo, tipoAverbacao, Dominio.Enumeradores.StatusAverbacaoCTe.Enviado, averbacaoOperacaoContainer) > 0)
                averbandoCTes = true;

            // Atualzia erro de averbacao
            if (averbacaoRejeitada)
            {
                carga.MotivoPendencia = "Erro ao Averbar CT-e da carga.";
                carga.PossuiPendencia = true;
                carga.problemaAverbacaoCTe = true;

                averbouTodosCTe = false;
                averbandoCTes = false;
            }
            else
            {
                carga.problemaAverbacaoCTe = false;
            }

            // Atuliza processo
            if (averbandoCTes)
            {
                carga.AverbandoCTes = true;

                averbouTodosCTe = false;
            }
            else if (averbouTodosCTe)
            {
                carga.AverbandoCTes = false;
            }

            // Atualiza carga
            repCarga.Atualizar(carga);

            if (averbacaoRejeitada)
            {
                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                    serCarga.NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.ErroAverbarCTeCarga, carga.CodigoCargaEmbarcador), unitOfWork);
            }

            return averbouTodosCTe;
        }

        private bool AverbarMDFeCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            // Quando há uma averbação com erro:
            // Seta a flag problema averbacao como true
            //
            // Quando há uma averbacao enviada
            // Seta a flag Averbando cte como true

            // Repositorios
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAverbacao repPedidoAverbacao = new Repositorio.Embarcador.Pedidos.PedidoAverbacao(unitOfWork);

            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            bool bloquerCargaComRejeicaoAverbacao = false;

            if (configuracaoEmbarcador.NaoPermiteEmitirCargaSemAverbacao && !(carga.TipoOperacao?.PermitirCargaSemAverbacao ?? false))
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao> listaAverbacaoPedidos = repPedidoAverbacao.BuscarPorCarga(carga.Codigo);

                bloquerCargaComRejeicaoAverbacao = true;
                Dominio.Entidades.Empresa empresa = carga.Empresa;

                if (carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    empresa = carga.EmpresaFilialEmissora;

                if (empresa != null && empresa.LiberarEmissaoSemAverbacao)
                    bloquerCargaComRejeicaoAverbacao = false;

                if (carga.LiberarComProblemaAverbacaoMDFe)
                    bloquerCargaComRejeicaoAverbacao = false;

                if (listaAverbacaoPedidos != null && listaAverbacaoPedidos.Count > 0)
                    bloquerCargaComRejeicaoAverbacao = false;
            }

            //todo: se precisar criar regras que bloqueiam a carga se não averbar deve faze-la aqui.
            //if (carga.Empresa.EmpresaPai != null && carga.Empresa.EmpresaPai.Configuracao.VersaoMDFe == "3.0") 
            //bloquerCargaComRejeicaoAverbacao = true;

            bool averbouTodosMDFe = true;
            bool averbacaoRejeitada = false;

            Dominio.Enumeradores.TipoAverbacaoMDFe tipoAverbacao = Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao;

            if (repAverbacaoMDFe.ExistePorCargaTipoEStatus(carga.Codigo, tipoAverbacao, new Dominio.Enumeradores.StatusAverbacaoMDFe[] { Dominio.Enumeradores.StatusAverbacaoMDFe.Pendente, Dominio.Enumeradores.StatusAverbacaoMDFe.AgEmissao }))
                averbouTodosMDFe = false;

            if (bloquerCargaComRejeicaoAverbacao && repAverbacaoMDFe.ExistePorCargaTipoEStatus(carga.Codigo, tipoAverbacao, Dominio.Enumeradores.StatusAverbacaoMDFe.Rejeicao))
                averbacaoRejeitada = true;

            // Atualzia erro de averbacao
            if (averbacaoRejeitada)
            {
                carga.MotivoPendencia = "Problemas ao averbar o(s) MDF-e(s) da carga.";
                carga.PossuiPendencia = true;
                carga.ProblemaAverbacaoMDFe = true;

                averbouTodosMDFe = false;

                if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                    serCarga.NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.ProblemasAverbarMDFesCarga, carga.CodigoCargaEmbarcador), unitOfWork);
            }
            else
            {
                carga.ProblemaAverbacaoMDFe = false;
            }

            // Atualiza carga
            repCarga.Atualizar(carga);

            if (carga.DadosSumarizados != null)
            {
                carga.DadosSumarizados.PossuiAverbacaoMDFe = true;
                repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
            }

            if (averbacaoRejeitada)
                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

            return averbouTodosMDFe;
        }

        private bool EmissaoMDFeCarga(ref bool excedeuTempoLimiteEnviado, bool emitirMDFePeloMultiCTe, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, int codEmpresaPai, string webServiceConsultaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            // Repositorios
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega repFluxoColetaEntrega = new Repositorio.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            bool filtrarMDFePorStatus = configuracaoTMS.PermiteEmitirCargaDiferentesOrigensParcialmente || (carga.TipoOperacao?.PermiteEmitirCargaDiferentesOrigensParcialmente ?? false);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFEs = repCargaMDFe.BuscarPorCargaSemParcial(carga.Codigo, filtrarMDFePorStatus);
            bool emitiuTodoMDFe = true;
            bool problemaMDFe = false;

            if (cargaMDFEs.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> mdfeExcederamLimiteTempo = new List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFEs)
                {
                    if (cargaMDFe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe)
                    {
                        if (cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao)
                        {
                            problemaMDFe = true;
                            emitiuTodoMDFe = false;
                        }
                        else if (carga.ControlaTempoParaEmissao && VerificarMDFeExcedeuTempoLimiteEmissao(cargaMDFe.MDFe))
                        {
                            emitiuTodoMDFe = false;
                            problemaMDFe = true;
                            excedeuTempoLimiteEnviado = true;
                            mdfeExcederamLimiteTempo.Add(cargaMDFe);
                        }
                        else if (cargaMDFe.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado && cargaMDFe.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                        {
                            emitiuTodoMDFe = false;
                        }

                    }
                    else if (cargaMDFe.MDFeManual == null || cargaMDFe.MDFeManual.MDFeInformado == false)
                        emitiuTodoMDFe = false;
                }

                if (problemaMDFe)
                {
                    if (excedeuTempoLimiteEnviado)
                    {
                        carga.MotivoPendencia = "Sefaz não está respondendo ao envio do(s) MDF-e(s)";
                        if (!carga.PossuiPendencia)
                        {
                            System.Text.StringBuilder stBuilder = MontaHTMLMDFeParado(carga.RetornarCodigoCargaParaVisualizacao, mdfeExcederamLimiteTempo, unitOfWork);

                            if (carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                            {
                                //Task.Factory.StartNew(() => enviarEmailDocumentosParadosNaFilaDeEnvio(stBuilder.ToString(), "MDF-e parado na fila de envio.", unitOfWork));
                                enviarEmailDocumentosParadosNaFilaDeEnvio(stBuilder.ToString(), "MDF-e parado na fila de envio.", unitOfWork);
                            }


#if DEBUG
                            //Task.Factory.StartNew(() => enviarEmailDocumentosParadosNaFilaDeEnvio(stBuilder.ToString(), "MDF-e parado na fila de envio.", unitOfWork));
                            enviarEmailDocumentosParadosNaFilaDeEnvio(stBuilder.ToString(), "MDF-e parado na fila de envio.", unitOfWork);
#endif
                        }
                    }
                    else
                        carga.MotivoPendencia = "Problema na Emissão do(s) MDF-e(s)";

                    carga.PossuiPendencia = true;
                    carga.problemaMDFe = true;
                    repCarga.Atualizar(carga);

                    Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

                    serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                    if (configuracaoTMS.NotificarAlteracaoCargaAoOperador)
                        serCarga.NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.MotivoPendenciaCarga, carga.MotivoPendencia, carga.CodigoCargaEmbarcador), unitOfWork);

                    if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Piracanjuba) != null)
                        new Servicos.Embarcador.Integracao.Piracanjuba.IntegracaoPiracanjuba(unitOfWork, tipoServicoMultisoftware).IntegrarMDFeComPendencia(carga);
                }
                else if (emitiuTodoMDFe)
                {
                    carga.PossuiPendencia = false;
                    carga.MotivoPendencia = "";
                }
            }
            else
            {
                Servicos.Embarcador.Carga.MDFe serMDFe = new MDFe(unitOfWork);

                if (carga.EmpresaFilialEmissora == null || carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora || carga.EmiteMDFeFilialEmissora)
                    carga.AutorizouTodosCTes = true;

                carga.problemaCTE = false;
                carga.problemaNFS = false;
                carga.PossuiPendencia = false;

                repCarga.Atualizar(carga);

                if (emitirMDFePeloMultiCTe)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                    string mensagemRetorno = serMDFe.EmitirMDFe(carga, configuracaoTMS, cargaPedidos, tipoServicoMultisoftware, codEmpresaPai, webServiceConsultaCTe, unitOfWork);

                    Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                    serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                    carga = repCarga.BuscarPorCodigo(carga.Codigo);

                    if (mensagemRetorno == "NaoPossuiMDFe")
                    {
                        carga.PossuiPendencia = false;
                        carga.MotivoPendencia = "";
                        if (unitOfWork.IsActiveTransaction())
                            unitOfWork.CommitChanges();
                    }
                    else
                    {
                        emitiuTodoMDFe = false;
                        if (!string.IsNullOrEmpty(mensagemRetorno))
                        {
                            carga.MotivoPendencia = mensagemRetorno;
                            carga.PossuiPendencia = true;
                            carga.problemaMDFe = true;
                            repCarga.Atualizar(carga);

                            if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Piracanjuba) != null)
                                new Servicos.Embarcador.Integracao.Piracanjuba.IntegracaoPiracanjuba(unitOfWork, tipoServicoMultisoftware).IntegrarMDFeComPendencia(carga);

                            if (unitOfWork.IsActiveTransaction())
                                unitOfWork.CommitChanges();

                            if (configuracaoTMS.NotificarAlteracaoCargaAoOperador)
                                serCarga.NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.NaoFoiPossivelEmitirMDFesCarga, carga.CodigoCargaEmbarcador), unitOfWork);
                        }
                        else
                        {
                            carga.MotivoPendencia = "";
                            carga.PossuiPendencia = false;
                            carga.problemaMDFe = false;
                            repCarga.Atualizar(carga);
                        }
                    }
                }
                else
                {
                    carga.AgImportacaoMDFe = true;
                    serMDFe.GerarMDFeManual(carga, unitOfWork);
                    repCarga.Atualizar(carga);
                }
            }

            if (emitiuTodoMDFe)
            {
                Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega = repFluxoColetaEntrega.BuscarPorCarga(carga.Codigo);
                if (fluxoColetaEntrega != null && !fluxoColetaEntrega.DataEmissaoMDFe.HasValue)
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.SetarProximaEtapa(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.MDFe, unitOfWork);
            }

            return emitiuTodoMDFe;
        }

        private System.Text.StringBuilder MontaHTMLMDFeParado(string codigoCargaEmbarcador, List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> mdfeExcederamLimiteTempo, Repositorio.UnitOfWork unitOfWork)
        {
            // Repositorio cliente
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            // Inicia str builder
            StringBuilder stBuilder = new StringBuilder();

            // Monta cabeçalho
            stBuilder
                .Append("Os seguintes MDF-e(s) estão em processamento a mais de " + BuscarTempoLimiteEmissao() + " minutos no Multi Embarcador (carga: " + codigoCargaEmbarcador + ").")
                .AppendLine() // Quebra de linha
                .AppendLine();

            // Monta informação de cada MDFe
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in mdfeExcederamLimiteTempo)
            {
                Dominio.Entidades.Cliente cliente = null;
                if (cargaMDFe.Carga.Filial != null)
                    cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(cargaMDFe.Carga.Filial.CNPJ));

                stBuilder.Append("<b>DADOS do MDF-e</b>").AppendLine();
                if (cliente != null)
                    stBuilder.Append("Remetente: " + cliente.Nome + " (" + cliente.CPF_CNPJ_Formatado + ")").AppendLine();
                stBuilder.Append("Emissor: " + cargaMDFe.MDFe.Empresa.RazaoSocial + " (" + cargaMDFe.MDFe.Empresa.CNPJ_Formatado + ")").AppendLine();
                stBuilder.Append("Número: " + cargaMDFe.MDFe.Numero).AppendLine();
                stBuilder.Append("Série: " + cargaMDFe.MDFe.Serie.Numero).AppendLine();
                stBuilder.Append("<hr/>");
            }

            return stBuilder;
        }

        private System.Text.StringBuilder MontaHTMLCTeParado(string codigoCargaEmbarcador, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesExcederamLimiteTempo)
        {
            // Inicia str builder
            StringBuilder stBuilder = new StringBuilder();

            // Monta cabeçalho
            stBuilder
                .Append("Os seguintes CT-e(s) estão em processamento a mais de " + BuscarTempoLimiteEmissao() + " minutos no Multi Embarcador (carga: " + codigoCargaEmbarcador + ").")
                .AppendLine() // Quebra de linha
                .AppendLine();

            // Monta informação de cada MDFe
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctesExcederamLimiteTempo)
            {
                stBuilder.Append("<b>DADOS do CT-e</b>").AppendLine();
                stBuilder.Append("Remetente: " + cte.Remetente.Nome + " (" + cte.Remetente.CPF_CNPJ_Formatado + ")").AppendLine();
                stBuilder.Append("Emissor: " + cte.Empresa.RazaoSocial + " (" + cte.Empresa.CNPJ_Formatado + ")").AppendLine();
                stBuilder.Append("Número: " + cte.Numero).AppendLine();
                stBuilder.Append("Série: " + cte.Serie.Numero).AppendLine();
                stBuilder.Append("<hr/>");
            }

            return stBuilder;
        }

        private void GerarAnuenciaTransportador(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            var serConciliacaoTransportador = new Servicos.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
            serConciliacaoTransportador.AdicionarCargaEmConciliacaoTransportador(carga);
        }

        private Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas ObterModalidadeTransportador(Dominio.Entidades.Cliente terceiro, Repositorio.UnitOfWork unitOfWork)
        {
            if (terceiro == null)
                return null;

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = repModalidadeTerceiro.BuscarPorPessoa(terceiro.CPF_CNPJ);
            return modalidadeTerceiro;
        }

        private void AtualizarSituacaoEnvioEmailDocumentacaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfigGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configGeralCarga = repConfigGeralCarga.BuscarPrimeiroRegistro();

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioEmailDocumentacaoCarga? situacao = null;

            if (configGeralCarga?.HabilitarEnvioDocumentacaoCargaPorEmail ?? false)
                situacao = SituacaoEnvioEmailDocumentacaoCarga.PendenteDeEnvio;

            carga.SituacaoEnvioEmailDocumentacaoCarga = situacao;

            repCarga.Atualizar(carga);
        }

        private void RecalcularDataPrevisaoEntregaLeadTime(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.Filial == null || carga.TipoOperacao == null || carga.TipoDeCarga == null || !carga.Filial.HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos.HasValue)
                return;

            Repositorio.Embarcador.Localidades.RegiaoPrazoEntrega repRegiaoPrazoEntrega = new Repositorio.Embarcador.Localidades.RegiaoPrazoEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);

            DateTime dataEmbarque = DateTime.Now;

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(carga.Codigo);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaEntrega.Pedidos?.FirstOrDefault().CargaPedido;
                if (cargaPedido != null && cargaPedido.CargaPedidoTrechoAnterior != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedidoTrechoAnterior = repCargaEntregaPedido.BuscarPorCargaPedido(cargaPedido.CargaPedidoTrechoAnterior.Codigo);

                    if (cargaEntregaPedidoTrechoAnterior != null && cargaEntregaPedidoTrechoAnterior.CargaEntrega != null)
                        dataEmbarque = cargaEntregaPedidoTrechoAnterior.CargaEntrega.DataPrevista.HasValue ? cargaEntregaPedidoTrechoAnterior.CargaEntrega.DataPrevista.Value : DateTime.Now;
                }

                Dominio.Entidades.Embarcador.Localidades.Regiao regiaoDestino = cargaEntrega.Pedidos?.FirstOrDefault()?.CargaPedido?.Pedido?.RegiaoDestino;
                if (regiaoDestino == null)
                    continue;

                Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega regiaoPrazoEntrega = repRegiaoPrazoEntrega.BuscarPorFilialTipoOperacaoTipoCarga(regiaoDestino.Codigo, carga.Filial.Codigo, carga.TipoOperacao.Codigo, carga.TipoDeCarga.Codigo);
                if (regiaoPrazoEntrega == null)
                    continue;

                cargaEntrega.DataPrevista = ObterDataPrevisaoEntregaLeadTime(dataEmbarque, carga.Filial.HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos.Value, configuracaoControleEntrega.HoraFimPadraoEntrega, regiaoPrazoEntrega, unitOfWork);
                repCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork, configControleEntrega);
            }
        }

        private DateTime ObterDataPrevisaoEntregaLeadTime(DateTime dataEmbarque, TimeSpan horaCorte, TimeSpan? horaPadraoEntrega, Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega regiaoPrazoEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Configuracoes.Feriado servicoFeriado = new Configuracoes.Feriado(unitOfWork);

            DateTime dataPrevisaoEntrega;

            if (regiaoPrazoEntrega.PadraoTempo == PadraoTempoDiasMinutos.Dias)
                dataPrevisaoEntrega = dataEmbarque.AddDays(regiaoPrazoEntrega.TempoDeViagemEmMinutos / 1440);
            else
                dataPrevisaoEntrega = dataEmbarque.AddMinutes(regiaoPrazoEntrega.TempoDeViagemEmMinutos);

            if (dataPrevisaoEntrega.TimeOfDay > horaCorte)
                dataPrevisaoEntrega = dataPrevisaoEntrega.AddDays(1);

            if (horaPadraoEntrega.HasValue)
            {
                if (dataPrevisaoEntrega.TimeOfDay > horaPadraoEntrega.Value)
                    dataPrevisaoEntrega = dataPrevisaoEntrega.AddDays(1);

                dataPrevisaoEntrega = dataPrevisaoEntrega.Date.Add(horaPadraoEntrega.Value);
            }

            int diasBuscarFeriado = Math.Max(regiaoPrazoEntrega.TempoDeViagemEmMinutos / 1440 * 2, 30);
            List<DateTime> datasComFeriado = servicoFeriado.ObterDatasComFeriado(dataEmbarque, dataEmbarque.AddDays(diasBuscarFeriado));

            List<DateTime> datasPeriodo = Utilidades.DateTime.GetDatesBetween(dataEmbarque.Date, dataPrevisaoEntrega.Date);

            for (int i = 0; i < datasPeriodo.Count; i++)
            {
                DateTime data = datasPeriodo[i];

                if (datasComFeriado.Contains(data))
                {
                    dataPrevisaoEntrega = dataPrevisaoEntrega.AddDays(1);
                    datasPeriodo.Add(dataPrevisaoEntrega.Date);
                    continue;
                }

                if (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
                {
                    dataPrevisaoEntrega = dataPrevisaoEntrega.AddDays(1);
                    datasPeriodo.Add(dataPrevisaoEntrega.Date);
                    continue;
                }
            }

            return dataPrevisaoEntrega;
        }

        private bool IntegrouGNRE(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.Carga servicoCarga = new Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> guiasNacionalRecolhimentoTributoEstadual = repositorioGNRE.BuscarPorCarga(carga.Codigo);

            bool integrouGNRE = true;
            bool integracaoRejeitada = false;
            if (guiasNacionalRecolhimentoTributoEstadual.Count > 0)
            {
                if (guiasNacionalRecolhimentoTributoEstadual.Any(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao))
                    integracaoRejeitada = true;
                else if (guiasNacionalRecolhimentoTributoEstadual.Any(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno))
                    integrouGNRE = false;
                else if (guiasNacionalRecolhimentoTributoEstadual.Any(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) && !(carga?.LiberadoComProblemaGNRE ?? false))
                {
                    integrouGNRE = false;
                    carga.IntegrandoGNRE = true;
                    carga.ProblemaIntegracaoGNRE = false;
                    repositorioCarga.Atualizar(carga);
                }

                if (integracaoRejeitada && !(carga?.LiberadoComProblemaGNRE ?? false))
                {
                    carga.MotivoPendencia = "Falha ao Integrar GNRE.";
                    carga.PossuiPendencia = true;
                    carga.ProblemaIntegracaoGNRE = true;
                    integrouGNRE = false;
                    repositorioCarga.Atualizar(carga);

                    servicoHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                    if (configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                        servicoCarga.NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.FalhaIntegrarValePedagioCarga, carga.CodigoCargaEmbarcador), unitOfWork);
                }

                if (carga.LiberadoComProblemaGNRE)
                    integrouGNRE = true;
            }

            return integrouGNRE || integracaoRejeitada;
        }

        private bool ValidarGerouAdiantamentoPagamentoTerceiroContainer(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!carga.FreteDeTerceiro)
                return true;

            if (!carga.GerouAdiantamentoTerceiroContainer && (carga.TipoOperacao?.ConfiguracaoContainer?.GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer ?? false))
                return false;

            return true;
        }

        #endregion
    }
}