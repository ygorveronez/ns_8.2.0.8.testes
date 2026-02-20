using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class CargaCTe
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaCTe(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private bool EmitirCte(int codigoCTe)
        {
            Servicos.CTe servicoCte = new Servicos.CTe(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCte = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioCte.BuscarPorCodigo(codigoCTe);

            if ((cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Rejeitada) || (cte.SituacaoCTeSefaz == SituacaoCTeSefaz.EmDigitacao) || (cte.SituacaoCTeSefaz == SituacaoCTeSefaz.ContingenciaFSDA))
            {
                if (cte.DataEmissao.HasValue && cte.DataEmissao.Value < DateTime.Now.AddDays(-6))
                {
                    TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(cte.Empresa.FusoHorario);

                    cte.DataEmissao = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);

                    repositorioCte.Atualizar(cte);
                }
            }

            return servicoCte.Emitir(codigoCTe, codigoEmpresa: 0, unitOfWork: _unitOfWork);
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao AdicionarCargaCTeManualIntegracao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Enumeradores.StatusIntegracaoCTeManual status)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao()
            {
                Carga = carga,
                CTe = cte,
                TipoIntegracao = tipoIntegracao,
                Status = status,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                NumeroTentativas = 0,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao
            };

            repCargaCTeManualIntegracao.Inserir(cargaCTeManualIntegracao);

            return cargaCTeManualIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public async Task<string> SincronizarDocumentoEmProcessamentoAsync(int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, System.Threading.CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = await repositorioCTe.BuscarPorCodigoAsync(codigoCTe, true);
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = await repCargaCte.BuscarPorCTeAsync(codigoCTe, cancellationToken, codigoEmpresa: 0);

            if (cte == null)
                return "O Documento informado não foi localizado";

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
            {
                bool sucesso = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cte.SistemaEmissor).ConsultarCte(cte, Auditado, TipoServicoMultisoftware, _unitOfWork);

                if (!sucesso)
                    return "Não foi possível efetuar a sincronização do documento.";
            }
            else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe && cte.SistemaEmissor == TipoEmissorDocumento.Migrate)
            {
                Servicos.Embarcador.Integracao.Migrate.IntegracaoMigrate serMigrate = new Servicos.Embarcador.Integracao.Migrate.IntegracaoMigrate(_unitOfWork);
                serMigrate.ConsultarRetornoNFSe(cte, _unitOfWork);
            }
            else
                return "Ação não permitida para o tipo de documento selecionado.";

            if (cargaCTe != null)
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCTe, null, "Documento sincronizado.", _unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro, cancellationToken);

            await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cte, null, "Documento sincronizado.", _unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro, cancellationToken);
            return string.Empty;
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.SincronizacaoLoteResultado SincronizarLoteDocumentoEmProcessamento(int codigoCarga, int? codigoCancelamentoCarga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
            if (carga == null)
                return new Dominio.ObjetosDeValor.Embarcador.Carga.SincronizacaoLoteResultado
                {
                    Sucesso = false,
                    Mensagem = "Carga não localizada."
                };

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = null;
            if (codigoCancelamentoCarga.HasValue)
                cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCancelamentoCarga.Value);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCargaCTe.BuscarTodosCTesPorCarga(codigoCarga);
            Dominio.ObjetosDeValor.Embarcador.Carga.SincronizacaoLoteResultado resultado = new Dominio.ObjetosDeValor.Embarcador.Carga.SincronizacaoLoteResultado { Sucesso = true };

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                if (ObterHabilitarSincronizarDocumento(cte, cargaCancelamento))
                {
                    try
                    {
                        string mensagemErro = SincronizarDocumentoEmProcessamento(cte.Codigo, auditado, tipoServicoMultisoftware);
                        if (!string.IsNullOrEmpty(mensagemErro))
                            resultado.Erros.Add($"Documento {cte.Codigo} - {mensagemErro}");
                    }
                    catch (Dominio.Excecoes.Embarcador.ServicoException ex)
                    {
                        resultado.Erros.Add($"Documento {cte.Codigo} - ({ex.Message})");
                    }
                    catch (Exception)
                    {
                        resultado.Erros.Add($"Documento {cte.Codigo} - (Ocorreu uma falha ao tentar sincronizar o Documento)");
                    }
                }
            }

            if (resultado.Erros.Any())
            {
                resultado.Sucesso = false;
                resultado.Mensagem = string.Join(Environment.NewLine, resultado.Erros);
            }
            else
            {
                RetirarPendenciaCarga(carga, repCargaCTe, repCarga);
            }

            return resultado;
        }

        public string SincronizarDocumentoEmProcessamento(int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioCTe.BuscarPorCodigo(codigoCTe);
            if (cte == null)
                return "O Documento informado não foi localizado";

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCte.BuscarPorCTe(cte.Codigo);

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
            {
                bool sucesso = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService
                    .GetEmissorDocumentoCTe(cte.SistemaEmissor)
                    .ConsultarCte(cte, auditado, tipoServicoMultisoftware, _unitOfWork);

                if (!sucesso)
                    return "Não foi possível efetuar a sincronização do documento.";
            }
            else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe &&
                     cte.SistemaEmissor == TipoEmissorDocumento.Migrate)
            {
                Servicos.Embarcador.Integracao.Migrate.IntegracaoMigrate serMigrate =
                    new Servicos.Embarcador.Integracao.Migrate.IntegracaoMigrate(_unitOfWork);
                serMigrate.ConsultarRetornoNFSe(cte, _unitOfWork);
            }
            else
                return "Ação não permitida para o tipo de documento selecionado.";

            if (cargaCTe != null)
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaCTe, null, "Documento sincronizado.", _unitOfWork);

            Servicos.Auditoria.Auditoria.Auditar(auditado, cte, null, "Documento sincronizado.", _unitOfWork);
            return string.Empty;
        }

        public bool ObterHabilitarSincronizarDocumento( Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento)
        {
            if (cte.Status == "E" && cte.DataIntegracao != null && (DateTime.Now.AddMinutes(-30) > cte.DataIntegracao))
            {
                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                {
                    if (cte.SistemaEmissor == null || (cte.SistemaEmissor == TipoEmissorDocumento.Integrador && cte.CodigoCTeIntegrador != 0))
                        return true;
                    else if (cte.SistemaEmissor == TipoEmissorDocumento.NSTech)
                        return true;
                }
                else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe && cte.SistemaEmissor == TipoEmissorDocumento.Migrate)
                    return true;
            }
            else if (cte.Status == "K" && cargaCancelamento?.DataCancelamento != null && (DateTime.Now.AddMinutes(-30) > cargaCancelamento?.DataCancelamento) && cte.SistemaEmissor == TipoEmissorDocumento.NSTech)
                return true;

            return false;
        }

        public async Task RetirarPendenciaCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, System.Threading.CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = await repCargaCTe.BuscarPorCargaAsync(carga.Codigo);
            bool cargaPossuiCTeNaoAutorizado = cargaCTes.Any(c => c.CTe.Status != "A");

            if (carga.PossuiPendencia && !cargaPossuiCTeNaoAutorizado)
            {
                carga.PossuiPendencia = false;
                carga.problemaCTE = false;
                carga.MotivoPendencia = "";
                await repCarga.AtualizarAsync(carga);
            }
        }

        private void RetirarPendenciaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe, Repositorio.Embarcador.Cargas.Carga repCarga)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(carga.Codigo);
            bool cargaPossuiCTeNaoAutorizado = cargaCTes.Any(c => c.CTe.Status != "A");

            if (carga.PossuiPendencia && !cargaPossuiCTeNaoAutorizado)
            {
                carga.PossuiPendencia = false;
                carga.problemaCTE = false;
                carga.MotivoPendencia = "";
                repCarga.Atualizar(carga);
            }
        }

        public void EmitirCTes(int codigoCarga)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCte = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Servicos.NFSe servicoNfse = new Servicos.NFSe(_unitOfWork);
            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();

            try
            {
                List<int> codigosCTesEmitir = repositorioCargaCte.ObterCodigoCTeParaEmissao(codigoCarga);
                int count = 0;
                int quantidadeCTesEmitidos = 0;
                int quantidadeCTesTotal = codigosCTesEmitir.Count;
                int quantidadeCTesAtualizarCarga = (int)(quantidadeCTesTotal * 0.1);

                if (quantidadeCTesAtualizarCarga > 10)
                    quantidadeCTesAtualizarCarga = 10;
                else if (quantidadeCTesAtualizarCarga < 2)
                    quantidadeCTesAtualizarCarga = 2;

                foreach (var codigoCTe in codigosCTesEmitir)
                {
                    quantidadeCTesEmitidos += 1;

                    if (count == 25)
                    {
                        _unitOfWork.FlushAndClear();
                    }

                    bool sucesso;

                    if (repositorioCte.BuscarPorCodigoModeloCTe(codigoCTe))
                        sucesso = EmitirCte(codigoCTe);
                    else if (repositorioCte.BuscarPorCodigoModeloNFSe(codigoCTe))
                        sucesso = servicoNfse.EmitirNFSe(codigoCTe, _unitOfWork);
                    else
                    {
                        Servicos.CTe servicoCte = new Servicos.CTe(_unitOfWork);
                        DateTime dataAutorizacao = DateTime.Now;
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioCte.BuscarPorCodigo(codigoCTe);

                        _unitOfWork.Start();

                        cte.Status = "A";
                        cte.DataRetornoSefaz = dataAutorizacao;
                        cte.DataAutorizacao = dataAutorizacao;

                        repositorioCte.Atualizar(cte);
                        servicoCte.AjustarAverbacoesParaAutorizacao(cte.Codigo, _unitOfWork);

                        if (!string.IsNullOrWhiteSpace(cte.ChaveCTESubComp) && cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOriginal = repositorioCte.BuscarPorChave(cte.ChaveCTESubComp);
                            if (cteOriginal != null)
                            {
                                cteOriginal.PossuiCTeComplementar = true;
                                repositorioCte.Atualizar(cteOriginal);
                            }
                        }

                        _unitOfWork.CommitChanges();

                        sucesso = true;
                    }

                    if (!sucesso)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioCte.BuscarPorCodigo(codigoCTe);
                        servicoHubCarga.InformarQuantidadeDocumentosEnviadosSefaz(codigoCarga, quantidadeCTesTotal, quantidadeCTesEmitidos, true, "Ocorreu uma falha ao emitir o documento nº " + cte.Numero + " da empresa " + cte.Empresa.CNPJ + ".");
                        return;
                    }

                    count++;

                    if (quantidadeCTesEmitidos == 1 || (quantidadeCTesEmitidos % quantidadeCTesAtualizarCarga) == 0)
                        servicoHubCarga.InformarQuantidadeDocumentosEnviadosSefaz(codigoCarga, quantidadeCTesTotal, quantidadeCTesEmitidos, false, string.Empty);
                }

                servicoHubCarga.InformarQuantidadeDocumentosEnviadosSefaz(codigoCarga, quantidadeCTesTotal, quantidadeCTesEmitidos, false, string.Empty);
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                servicoHubCarga.InformarQuantidadeDocumentosEnviadosSefaz(codigoCarga, 0, 0, true, "Ocorreu uma falha ao emitir os documentos.");
                Log.TratarErro(excecao);
            }
            finally
            {
                _unitOfWork.Start();

                string situacaoEmDigitacao = "S";
                bool manterCTesEmDigitacao = repositorioCargaCte.ContarCTePorSituacao(codigoCarga, situacaoEmDigitacao) > 0;
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                carga.EmitindoCTes = false;
                carga.CTesEmDigitacao = manterCTesEmDigitacao;

                repositorioCarga.Atualizar(carga);

                Pedido.Pedido.AtualizarSituacaoPlanejamentoPedidoTMS(carga, null, SituacaoPlanejamentoPedidoTMS.CargaGerouDocumentacao, _unitOfWork);

                _unitOfWork.CommitChanges();
            }
        }

        public void EmitirCTesComplementaresRejeitados()
        {
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                if (configuracaoEmbarcador.NumeroTentativasReenvioCteRejeitado <= 0)
                    return;

                int tempoAguardarParaReenviarEmMinutos = configuracaoEmbarcador.TempoMinutosParaReenviarCancelamento > 0 ? configuracaoEmbarcador.TempoMinutosParaReenviarCancelamento : 5;
                DateTime dataLimiteReenvio = DateTime.Now.AddMinutes(-tempoAguardarParaReenviarEmMinutos);
                Servicos.NFSe servicoNfse = new Servicos.NFSe(_unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repositorioCte = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.CteComplementarReemissao> ctesComplementaresParaReemissao = repositorioCargaCTeComplementoInfo.ObterCtesComplementaresRejeitados(numeroMaximoRegistros: 10, configuracaoEmbarcador.NumeroTentativasReenvioCteRejeitado, dataLimiteReenvio);

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CteComplementarReemissao ctesComplementarParaReemissao in ctesComplementaresParaReemissao)
                {
                    _unitOfWork.Start();

                    if (repositorioCte.BuscarPorCodigoModeloCTe(ctesComplementarParaReemissao.CodigoCte))
                        EmitirCte(ctesComplementarParaReemissao.CodigoCte);
                    else if (repositorioCte.BuscarPorCodigoModeloNFSe(ctesComplementarParaReemissao.CodigoCte))
                        servicoNfse.EmitirNFSe(ctesComplementarParaReemissao.CodigoCte, _unitOfWork);

                    repositorioCargaOcorrencia.DefinirSituacaoOcorrenciaPorCodigo(ctesComplementarParaReemissao.CodigoOcorrencia, SituacaoOcorrencia.EmEmissaoCTeComplementar);

                    _unitOfWork.CommitChanges();
                }
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(excecao);
            }
        }

        public void ReemitirCTesComplementaresRejeitados()
        {
            try
            {
                Servicos.NFSe servicoNfse = new Servicos.NFSe(_unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repositorioCte = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.CteComplementarReemissao> ctesComplementaresParaReemissao = repositorioCargaCTeComplementoInfo.ObterCtesComplementaresParaReemissao(numeroMaximoRegistros: 10);

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CteComplementarReemissao ctesComplementarParaReemissao in ctesComplementaresParaReemissao)
                {
                    _unitOfWork.Start();

                    if (repositorioCte.BuscarPorCodigoModeloCTe(ctesComplementarParaReemissao.CodigoCte))
                        EmitirCte(ctesComplementarParaReemissao.CodigoCte);
                    else if (repositorioCte.BuscarPorCodigoModeloNFSe(ctesComplementarParaReemissao.CodigoCte))
                        servicoNfse.EmitirNFSe(ctesComplementarParaReemissao.CodigoCte, _unitOfWork);

                    repositorioCargaOcorrencia.DefinirSituacaoOcorrenciaPorCodigo(ctesComplementarParaReemissao.CodigoOcorrencia, SituacaoOcorrencia.EmEmissaoCTeComplementar);
                    repositorioCargaCTeComplementoInfo.DefinirCteComplementarComoReemitido(ctesComplementarParaReemissao.CodigoComplementoInfo);

                    _unitOfWork.CommitChanges();
                }
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(excecao);
            }
        }

        public void GerarIntegracoesCargaCTeManual(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool apenasCTeManual, Dominio.Entidades.Embarcador.Cargas.Carga carga = null)
        {
            if (apenasCTeManual && !cte.GeradoManualmente && !cte.CanceladoManualmente)
                return;

            if (cte.Status != "Z" && cte.Status != "C" && cte.Status != "A")
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            List<TipoIntegracao> tiposIntegracaoAutorizados = new List<TipoIntegracao> {
                TipoIntegracao.Intercab,
                TipoIntegracao.EMP,
                TipoIntegracao.NFTP,
                TipoIntegracao.SAP,
                TipoIntegracao.KMM,
                TipoIntegracao.CTePagamentoLoggi,
                TipoIntegracao.Globus,
                TipoIntegracao.PortalCabotagem
            };

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(tiposIntegracaoAutorizados);

            if (tiposIntegracao.Count == 0)
                return;

            if (carga == null)
                carga = repCargaCTe.BuscarCargaPorCTe(cte.Codigo);
            if (carga == null)
                return;

            if (carga.CargaRecebidaDeIntegracao)
                return;

            Dominio.Enumeradores.StatusIntegracaoCTeManual status = Dominio.Enumeradores.StatusIntegracaoCTeManual.EmitirCTeManual;
            if (cte.Status == "A")
            {
                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao)
                    status = Dominio.Enumeradores.StatusIntegracaoCTeManual.EmitirAnulacao;
                else if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto)
                    status = Dominio.Enumeradores.StatusIntegracaoCTeManual.EmitirSubstituicao;
            }
            else if (cte.Status == "Z")
                status = Dominio.Enumeradores.StatusIntegracaoCTeManual.AnularCTe;
            else if (cte.Status == "C")
                status = Dominio.Enumeradores.StatusIntegracaoCTeManual.CancelarCTeManual;

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                if (tipoIntegracao.Tipo == TipoIntegracao.Intercab)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                    if (integracaoIntercab == null)
                        continue;

                    if (!((integracaoIntercab?.PossuiIntegracaoIntercab ?? false) && (integracaoIntercab?.AtivarIntegracaoCteManual ?? false)))
                        continue;

                    AdicionarCargaCTeManualIntegracao(cte, carga, tipoIntegracao, status);
                }
                if (tipoIntegracao.Tipo == TipoIntegracao.PortalCabotagem)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem repIntegracaoPortalCabotagem = new Repositorio.Embarcador.Configuracoes.IntegracaoPortalCabotagem(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPortalCabotagem integracaoPortalCabotagem = repIntegracaoPortalCabotagem.BuscarPrimeiroRegistro();

                    if (integracaoPortalCabotagem == null)
                        continue;

                    if (!(integracaoPortalCabotagem.AtivarEnvioXMLCTE || integracaoPortalCabotagem.AtivarEnvioPDFCTE))
                        continue;

                    AdicionarCargaCTeManualIntegracao(cte, carga, tipoIntegracao, status);
                }
                if (tipoIntegracao.Tipo == TipoIntegracao.EMP)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();
                    if (integracaoEMP == null)
                        continue;

                    if (integracaoEMP.PossuiIntegracaoEMP && integracaoEMP.AtivarIntegracaoCTeManualEMP)
                        AdicionarCargaCTeManualIntegracao(cte, carga, tipoIntegracao, status);

                }
                if (tipoIntegracao.Tipo == TipoIntegracao.NFTP)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoEMP repIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP integracaoEMP = repIntegracaoEMP.Buscar();
                    if (integracaoEMP == null)
                        continue;

                    if (integracaoEMP.AtivarIntegracaoNFTPEMP && cte.Status != "Z")
                        AdicionarCargaCTeManualIntegracao(cte, carga, tipoIntegracao, status);

                }
                if (tipoIntegracao.Tipo == TipoIntegracao.SAP)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoSAP repIntegracaoAP = new Repositorio.Embarcador.Configuracoes.IntegracaoSAP(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP integracaoSAP = repIntegracaoAP.Buscar();

                    if (integracaoSAP == null)
                        continue;

                    if (integracaoSAP.PossuiIntegracao)
                        AdicionarCargaCTeManualIntegracao(cte, carga, tipoIntegracao, status);
                }
                if (tipoIntegracao.Tipo == TipoIntegracao.KMM)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoKMM repIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM integracaoKMM = repIntegracaoKMM.Buscar();

                    if (integracaoKMM == null)
                        continue;

                    if (integracaoKMM.PossuiIntegracao)
                        AdicionarCargaCTeManualIntegracao(cte, carga, tipoIntegracao, status);
                }
                if (tipoIntegracao.Tipo == TipoIntegracao.CTePagamentoLoggi)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoLoggi repIntegracaoLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoLoggi(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLoggi integracaoLoggi = repIntegracaoLoggi.Buscar();

                    if (integracaoLoggi == null)
                        continue;

                    if (integracaoLoggi.PossuiIntegracao)
                        AdicionarCargaCTeManualIntegracao(cte, carga, tipoIntegracao, status);
                }
                if (tipoIntegracao.Tipo == TipoIntegracao.Globus)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoGlobus repIntegracaoGlobus = new Repositorio.Embarcador.Configuracoes.IntegracaoGlobus(_unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus integracaoGlobus = repIntegracaoGlobus.Buscar();

                    if (integracaoGlobus == null)
                        continue;

                    if (integracaoGlobus.PossuiIntegracao)
                        AdicionarCargaCTeManualIntegracao(cte, carga, tipoIntegracao, status);
                }

            }


        }

        public void EnviarEmailPreviaDocumentosCargaCte(int codigosDaCarga, Dominio.Entidades.Embarcador.Cargas.CargaCTe precte = null)
        {

            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga respositorioConfiguracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracao = respositorioConfiguracao.BuscarPrimeiroRegistro();

            if (!(configuracao?.EnviarEmailPreviaCustoParaTransportadores ?? false))
                return;

            Servicos.Log.TratarErro("Inicio Envio Email Previa Custo para A carga: " + codigosDaCarga, "EmailPreviaCusto");
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Servicos.PreCTe svcPreCTe = new Servicos.PreCTe(_unitOfWork);
            Servicos.Email svcEmail = new Servicos.Email(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> existeCargaCte = repositorioCargaCte.BuscarCargaCtePorCarga(codigosDaCarga);

            if (precte != null && !existeCargaCte.Any(e => e.Codigo == precte.Codigo))
                existeCargaCte.Add(precte);

            Servicos.Log.TratarErro("Encontro Resgistros de Carga Cte " + existeCargaCte.Count, "EmailPreviaCusto");
            if (existeCargaCte.Count == 0 || existeCargaCte == null)
                return;

            Dictionary<string, byte[]> listaAnexos = new Dictionary<string, byte[]>();

            Servicos.Log.TratarErro("Inicio Processamento ", "EmailPreviaCusto");
            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
            foreach (var cargaCte in existeCargaCte)
            {
                if (cargaCte.PreCTe == null)
                    continue;

                byte[] previaCustos = new Servicos.Embarcador.Carga.Carga(_unitOfWork).ObterPreviaCustoCarga(cargaCte.Carga.Codigo, _unitOfWork);
                byte[] composicaoDoFrete = new Servicos.Embarcador.Carga.Frete(_unitOfWork).GeraComposicaoDeFrete(cargaCte.Carga, false);
                byte[] xmlPreCte = null;

                if (carga == null)
                    carga = cargaCte.Carga;

                if (cargaCte?.PreCTe?.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    xmlPreCte = System.Text.Encoding.Unicode.GetBytes(svcPreCTe.BuscarXMLPreCte(cargaCte?.PreCTe ?? null));

                string numeroArquivo = !string.IsNullOrWhiteSpace(cargaCte?.PreCTe?.NumeroRecibo ?? string.Empty) ? cargaCte.PreCTe.NumeroRecibo : cargaCte.Codigo.ToString();

                if (previaCustos != null)
                {
                    Servicos.Log.TratarErro("PreviaCustos " + numeroArquivo, "EmailPreviaCusto");
                    listaAnexos.Add($"PrevicaoCustos-{numeroArquivo}.csv", previaCustos);
                }

                if (composicaoDoFrete != null)
                {
                    Servicos.Log.TratarErro("ComposicaoDeFrete " + numeroArquivo, "EmailPreviaCusto");
                    listaAnexos.Add($"ComposicaoDeFrete-{numeroArquivo}.pdf", composicaoDoFrete);
                }

                if (xmlPreCte != null)
                {
                    Servicos.Log.TratarErro("XmlPrecte " + numeroArquivo, "EmailPreviaCusto");
                    listaAnexos.Add($"XmlPrecte-{numeroArquivo}.pdf", xmlPreCte);
                }
            }

            if (carga == null)
                return;

            List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>();

            MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(listaAnexos);

            if (arquivoCompactado != null)
            {
                Servicos.Log.TratarErro("Gerou arquivo compactado ", "EmailPreviaCusto");
                anexos.Add(new System.Net.Mail.Attachment(arquivoCompactado, $"Anexos_ {codigosDaCarga} {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}.zip"));
            }

            Servicos.Log.TratarErro("N Anexos: " + listaAnexos.Count(), "EmailPreviaCusto");

            System.Text.StringBuilder corpoEmail = new System.Text.StringBuilder();
            corpoEmail.Append($"<b>Numero da Carga: </b>{carga.CodigoCargaEmbarcador}<br/>");
            corpoEmail.Append($"<b>Data da carga: </b>{carga.DataCarregamentoCarga}<br/>");
            corpoEmail.Append($"<b>Transportador: </b>{carga.Empresa.RazaoSocial}<br/>");
            svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, carga.Empresa.Email, "", "", $"Carga Documentos #{carga.CodigoCargaEmbarcador}", corpoEmail.ToString(), string.Empty, anexos, string.Empty, false, string.Empty, 0, _unitOfWork);
            Servicos.Log.TratarErro("Enviou Email", "EmailPreviaCusto");
        }

        #endregion
    }
}
