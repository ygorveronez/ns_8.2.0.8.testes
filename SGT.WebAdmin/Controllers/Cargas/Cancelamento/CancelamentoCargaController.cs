using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Servicos;
using SGTAdmin.Controllers;
using System.Text;
using System.Web;

namespace SGT.WebAdmin.Controllers.Cargas.Cancelamento
{
    [CustomAuthorize(new string[] { "ObterDadosGerais", "ValidarCarga" }, "Cargas/CancelamentoCarga")]
    public class CancelamentoCargaController : BaseController
    {
        #region Construtores

        public CancelamentoCargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unidadeTrabalho);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unidadeTrabalho, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCarga()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.CargaNaoEncontrada);

                TipoCancelamentoCarga tipoCancelamento = TipoCancelamentoCarga.Cancelamento;
                bool ctePermite = true, mdfePermite = true, averbacaoPermite = true, ciotPermite = true, emitidoNoEmbarcador = false;

                if (carga.CargaMDFes.Any(o => o.MDFe != null && o.SistemaEmissor != SistemaEmissor.MultiCTe))
                    emitidoNoEmbarcador = true;

                if (carga.CargaMDFes.Any(cargaMDFe => cargaMDFe.MDFe != null && cargaMDFe.SistemaEmissor == SistemaEmissor.MultiCTe && ((cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && cargaMDFe.MDFe.DataAutorizacao < DateTime.Now.AddDays(-1)) || cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)))
                {
                    tipoCancelamento = TipoCancelamentoCarga.Anulacao;
                    mdfePermite = false;
                }

                if (tipoCancelamento == TipoCancelamentoCarga.Cancelamento)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManualCarga = repCargaMDFeManual.BuscarPorCarga(carga.Codigo);
                    if (cargaMDFeManualCarga != null)
                    {
                        tipoCancelamento = TipoCancelamentoCarga.Anulacao;
                        mdfePermite = false;
                    }
                }

                if (!carga.CargaTransbordo)
                {
                    if (carga.CargaCTes.Any(cargaCTe => cargaCTe.CTe != null && cargaCTe.SistemaEmissor == SistemaEmissor.MultiCTe && cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe
                        && cargaCTe.CTe.Status == "A" && (cargaCTe.CTe.DataRetornoSefaz < DateTime.Now.AddDays(-7)) && cargaCTe.CargaCTeTrechoAnterior == null))
                    {
                        tipoCancelamento = TipoCancelamentoCarga.Anulacao;
                        ctePermite = false;
                    }
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> nfses = carga.CargaCTes.Where(cargaCTe => cargaCTe.CTe != null && cargaCTe.SistemaEmissor == SistemaEmissor.MultiCTe
                            && cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe && cargaCTe.CTe.Status == "A" && cargaCTe.CargaCTeTrechoAnterior == null).ToList();

                        if (nfses.Count > 0)
                        {
                            bool permiteAnularNFSe = true;

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in nfses)
                            {
                                //Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, cargaCTe.CTe.LocalidadeInicioPrestacao.Codigo) ?? repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, 0);
                                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, cargaCTe.CTe.LocalidadeInicioPrestacao.Codigo, cargaCTe.CTe.TomadorPagador.Localidade?.Estado?.Sigla ?? "", cargaCTe.CTe.TomadorPagador.GrupoPessoas?.Codigo ?? 0, cargaCTe.CTe.TomadorPagador.Localidade?.Codigo ?? 0);
                                if (configuracaoNFSe == null)
                                    configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, cargaCTe.CTe.LocalidadeInicioPrestacao.Codigo, cargaCTe.CTe.TomadorPagador.Localidade?.Estado?.Sigla ?? "", 0, 0);
                                if (configuracaoNFSe == null)
                                    configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, cargaCTe.CTe.LocalidadeInicioPrestacao.Codigo, "", cargaCTe.CTe.TomadorPagador.GrupoPessoas?.Codigo ?? 0, 0);
                                if (configuracaoNFSe == null)
                                    configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, 0, cargaCTe.CTe.TomadorPagador.Localidade?.Estado?.Sigla ?? "", cargaCTe.CTe.TomadorPagador.GrupoPessoas?.Codigo ?? 0, 0);
                                if (configuracaoNFSe == null)
                                    configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, 0, "", 0, cargaCTe.CTe.TomadorPagador.Localidade?.Codigo ?? 0);
                                if (configuracaoNFSe == null)
                                    configuracaoNFSe = repConfiguracaoNFSe.BuscarPorEmpresaELocalidade(cargaCTe.CTe.Empresa.Codigo, 0, "", 0, 0);

                                if (configuracaoNFSe == null || !configuracaoNFSe.PermiteAnular || cargaCTe.CTe.DataRetornoSefaz > DateTime.Now.AddDays(-configuracaoNFSe.PrazoCancelamento))
                                {
                                    permiteAnularNFSe = false;
                                    break;
                                }
                            }

                            if (permiteAnularNFSe)
                            {
                                tipoCancelamento = TipoCancelamentoCarga.Anulacao;
                                ctePermite = false;
                            }
                        }
                    }

                    if (carga.CargaCTes.Any(o => o.CTe != null && o.SistemaEmissor != SistemaEmissor.MultiCTe))
                        emitidoNoEmbarcador = true;
                }

                bool cancelamentoUnitario = ((carga.TipoOperacao?.ConfiguracaoCarga?.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.Documentos)
                    && carga.SituacaoCarga != SituacaoCarga.Encerrada
                    && carga.SituacaoCarga != SituacaoCarga.Nova
                    && carga.SituacaoCarga != SituacaoCarga.CalculoFrete
                    && carga.SituacaoCarga != SituacaoCarga.AgNFe
                    && carga.SituacaoCarga != SituacaoCarga.AgTransportador
                    && carga.SituacaoCarga != SituacaoCarga.Anulada
                    && carga.SituacaoCarga != SituacaoCarga.Cancelada;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    Servicos.Embarcador.Carga.Cancelamento.ValidarDocumentosCargaCancelamento(carga, out ctePermite, out mdfePermite, out averbacaoPermite, out ciotPermite, unidadeTrabalho);
                }

                return new JsonpResult(new
                {
                    TipoCancelamento = tipoCancelamento,
                    CTePermiteCancelamento = ctePermite,
                    MDFePermiteCancelamento = mdfePermite,
                    DocumentoEmitidoNoEmbarcador = emitidoNoEmbarcador,
                    SituacaoPermiteCancelarApenasDocumentos = carga.SituacaoCarga.IsSituacaoCargaEmitida(),
                    TipoCancelamentoCarga = carga.TipoOperacao?.ConfiguracaoCarga?.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga,
                    AverbacaoPermiteCancelamento = averbacaoPermite,
                    CiotPermiteCancelamento = ciotPermite,
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.FalhaAoValidar);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CancelamentoCarga");

                int codigoCargaCancelamento = await AdicionarCancelamentoAsync(unidadeTrabalho, permissoesPersonalizadas);

                return new JsonpResult(codigoCargaCancelamento);
            }
            catch (BaseException excecao)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaGerarCancelamento);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarComoCancelamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CancelamentoCarga");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaCancelamento_AdicionarComoCancelamento))
                    return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.NaoPossuiPermissãoAdicionarCancelamento);

                int codigoCargaCancelamento = await AdicionarCancelamentoAsync(unidadeTrabalho, permissoesPersonalizadas, TipoCancelamentoCarga.Cancelamento);

                return new JsonpResult(codigoCargaCancelamento);
            }
            catch (BaseException excecao)
            {
                await unidadeTrabalho.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unidadeTrabalho.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaGerarCancelamento);
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> ReenviarCancelamentoComoAnulacao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CancelamentoCarga");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaCancelamento_ReenviarCancelamentoComoAnulacao))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.NaoPossuiPermissão);

                int codigoCancelamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCancelamento);

                if (!cargaCancelamento.Situacao.IspermitirReenvio())
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.SituacaoNaoPermiteReenvio);

                if (cargaCancelamento.Tipo != TipoCancelamentoCarga.Cancelamento)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.TipoCancelamentoNaoPermite);

                Servicos.Embarcador.Carga.CargaCancelamentoAprovacao servicoCargaCancelamentoAprovacao = new Servicos.Embarcador.Carga.CargaCancelamentoAprovacao(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao repCargaCancelamentoSolicitacao = new Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao(unidadeTrabalho);

                await unidadeTrabalho.StartAsync();

                cargaCancelamento.Tipo = TipoCancelamentoCarga.Anulacao;
                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;
                cargaCancelamento.EnviouCTesParaCancelamento = false;
                cargaCancelamento.EnviouMDFesParaCancelamento = false;
                cargaCancelamento.EnviouAverbacoesCTesParaCancelamento = false;
                cargaCancelamento.EnviouValePedagiosParaCancelamento = false;
                cargaCancelamento.EnviouCIOTCancelamento = false;

                await GerarLogCancelamentoAsync(cargaCancelamento, TipoLogCargaCancelamento.ReenvioComoAnulacao, unidadeTrabalho);

                if (!repCargaCancelamentoSolicitacao.ExisteAprovadaPorCargaCancelamento(cargaCancelamento.Codigo))
                    servicoCargaCancelamentoAprovacao.CriarAprovacao(cargaCancelamento, TipoServicoMultisoftware);

                await repCargaCancelamento.AtualizarAsync(cargaCancelamento);

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCancelamento, null, Localization.Resources.Cargas.CancelamentoCarga.ReenviouCancelamentoCargaAnulacao, unidadeTrabalho);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCancelamento.Carga, null, Localization.Resources.Cargas.CancelamentoCarga.ReenviouCancelamentoCargaAnulacao, unidadeTrabalho);

                await unidadeTrabalho.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                await unidadeTrabalho.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unidadeTrabalho.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.FalhaReenviarCancelamento);
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> Aprovar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCancelamento;
                int.TryParse(Request.Params("Codigo"), out codigoCancelamento);

                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCancelamento);

                if (cargaCancelamento.Tipo == TipoCancelamentoCarga.Anulacao)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.AnulaçãoIndisponívelMultiembarcador);

                if (cargaCancelamento.Situacao != SituacaoCancelamentoCarga.AgConfirmacao)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.SituaçãoCancelamentoNaoPermite);

                await unidadeTrabalho.StartAsync();

                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;

                await repCargaCancelamento.AtualizarAsync(cargaCancelamento);

                await GerarLogCancelamentoAsync(cargaCancelamento, TipoLogCargaCancelamento.Aprovacao, unidadeTrabalho);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCancelamento, null, Localization.Resources.Cargas.CancelamentoCarga.AprovouCancelamentoCarga, unidadeTrabalho);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCancelamento.Carga, null, Localization.Resources.Cargas.CancelamentoCarga.AprovouCancelamentoCarga, unidadeTrabalho);
                await unidadeTrabalho.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unidadeTrabalho.RollbackAsync();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaAprovarCancelamento);
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> Reprovar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCancelamento;
                int.TryParse(Request.Params("Codigo"), out codigoCancelamento);

                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCancelamento);

                if (cargaCancelamento.Situacao != SituacaoCancelamentoCarga.AgConfirmacao)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.FalhaAprovarCancelamento);

                await unidadeTrabalho.StartAsync();

                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.Reprovada;

                await repCargaCancelamento.AtualizarAsync(cargaCancelamento);

                await GerarLogCancelamentoAsync(cargaCancelamento, TipoLogCargaCancelamento.Reprovacao, unidadeTrabalho);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCancelamento, null, Localization.Resources.Cargas.CancelamentoCarga.ReprovouCancelamentoCarga, unidadeTrabalho);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCancelamento.Carga, null, Localization.Resources.Cargas.CancelamentoCarga.ReprovouCancelamentoCarga, unidadeTrabalho);
                await unidadeTrabalho.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unidadeTrabalho.RollbackAsync();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaReprovarCancelamento);
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCancelamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = await repCargaCancelamento.BuscarPorCodigoAsync(codigoCancelamento, true);

                if (!cargaCancelamento.Situacao.IspermitirReenvio())
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.SituacaoNaoPermiteReenvio);

                Servicos.Embarcador.Carga.CargaCancelamentoAprovacao servicoCargaCancelamentoAprovacao = new Servicos.Embarcador.Carga.CargaCancelamentoAprovacao(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao repCargaCancelamentoSolicitacao = new Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao> cargaCancelamentosIntegracao = repCargaCancelamentoIntegracao.BuscarPorCargaCancelamento(codigoCancelamento, SituacaoIntegracao.ProblemaIntegracao, null);

                await unidadeTrabalho.StartAsync();

                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;
                cargaCancelamento.EnviouCTesParaCancelamento = false;
                cargaCancelamento.EnviouMDFesParaCancelamento = false;
                cargaCancelamento.EnviouAverbacoesCTesParaCancelamento = false;
                cargaCancelamento.EnviouValePedagiosParaCancelamento = false;
                cargaCancelamento.EnviouCIOTCancelamento = false;

                await GerarLogCancelamentoAsync(cargaCancelamento, TipoLogCargaCancelamento.Reenvio, unidadeTrabalho);

                if (!repCargaCancelamentoSolicitacao.ExisteAprovadaPorCargaCancelamento(cargaCancelamento.Codigo))
                    servicoCargaCancelamentoAprovacao.CriarAprovacao(cargaCancelamento, TipoServicoMultisoftware);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao in cargaCancelamentosIntegracao)
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = "";
                    await repCargaCancelamentoIntegracao.AtualizarAsync(cargaCancelamentoIntegracao);
                }

                if (cargaCancelamento.CTe != null)
                {
                    cargaCancelamento.CTe.UsuarioEmissaoCTe = Usuario;
                    await repositorioCTe.AtualizarAsync(cargaCancelamento.CTe);
                }

                await repCargaCancelamento.AtualizarAsync(cargaCancelamento);

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCancelamento, null, Localization.Resources.Cargas.CancelamentoCarga.ReenviouCancelamentoCarga, unidadeTrabalho);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCancelamento.Carga, null, Localization.Resources.Cargas.CancelamentoCarga.ReenviouCancelamentoCarga, unidadeTrabalho);

                await unidadeTrabalho.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                await unidadeTrabalho.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unidadeTrabalho.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaReenviarCancelamento);
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterDadosGerais()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var retorno = new
                {
                    Usuario = Usuario.Nome,
                    CodigoUsuario = Usuario.Codigo
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaObterDadosGeraisCancelamento);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI repCargaCancelamentoIntegracaoEDI = new Repositorio.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unidadeTrabalho);
                Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeTrabalho);
                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigo);

                if (cargaCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoNaoEncontrado);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracaoEDI> integracoes = repCargaCancelamentoIntegracaoEDI.BuscarPorCancelamento(codigo, null);

                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

                SituacaoCancelamentoDocumentoCarga situacaoMDFes = SituacaoCancelamentoDocumentoCarga.Sucesso;
                SituacaoCancelamentoDocumentoCarga situacaoAverbacaoMDFes = SituacaoCancelamentoDocumentoCarga.Sucesso;
                SituacaoCancelamentoDocumentoCarga situacaoCTes = SituacaoCancelamentoDocumentoCarga.Sucesso;
                SituacaoCancelamentoDocumentoCarga situacaoAverbacaoCTes = SituacaoCancelamentoDocumentoCarga.Sucesso;
                SituacaoCancelamentoDocumentoCarga situacaoIntegracoes = SituacaoCancelamentoDocumentoCarga.Sucesso;
                SituacaoCancelamentoDocumentoCarga situacaoValePedagio = SituacaoCancelamentoDocumentoCarga.Sucesso;
                SituacaoCancelamentoDocumentoCarga situacaoInutilizacao = SituacaoCancelamentoDocumentoCarga.Sucesso;
                SituacaoCancelamentoDocumentoCarga situacaoCIOT = SituacaoCancelamentoDocumentoCarga.Sucesso;

                if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.AgCancelamentoMDFe)
                    situacaoMDFes = SituacaoCancelamentoDocumentoCarga.Cancelando;
                else if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                {
                    Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes;

                    if ((cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos) || (cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.TodosDocumentos))
                        cargaMDFes = repositorioCargaMDFe.BuscarPorCargaECancelamento(cargaCancelamento.Carga.Codigo, cargaCancelamento.Codigo);
                    else
                        cargaMDFes = repositorioCargaMDFe.BuscarPorCarga(cargaCancelamento.Carga.Codigo);

                    if (cargaMDFes.Any(o => o.MDFe != null && o.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado && o.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado && o.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao && o.SistemaEmissor != SistemaEmissor.OutrosEmissores))
                        situacaoMDFes = SituacaoCancelamentoDocumentoCarga.Rejeicao;
                }

                if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.AgIntegracaoCancelamentoCIOT)
                    situacaoCIOT = SituacaoCancelamentoDocumentoCarga.Cancelando;
                else if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento && cargaCIOT != null && cargaCIOT.CIOT.Situacao != SituacaoCIOT.Cancelado)
                    situacaoCIOT = SituacaoCancelamentoDocumentoCarga.Rejeicao;

                if (situacaoMDFes == SituacaoCancelamentoDocumentoCarga.Sucesso)
                {
                    if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.AgCancelamentoAverbacaoMDFe)
                    {
                        situacaoAverbacaoMDFes = SituacaoCancelamentoDocumentoCarga.Cancelando;
                        situacaoMDFes = SituacaoCancelamentoDocumentoCarga.Cancelando;
                    }
                    else if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento &&
                             !cargaCancelamento.LiberarCancelamentoComAverbacaoMDFeRejeitada && repAverbacaoMDFe.ExistePorCargaEStatus(cargaCancelamento.Carga.Codigo, Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso))
                    {
                        situacaoAverbacaoMDFes = SituacaoCancelamentoDocumentoCarga.Rejeicao;
                        situacaoMDFes = SituacaoCancelamentoDocumentoCarga.Rejeicao;
                    }
                }

                if (!cargaCancelamento.Carga.CargaTransbordo)
                {
                    if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.AgCancelamentoCTe)
                        situacaoCTes = SituacaoCancelamentoDocumentoCarga.Cancelando;
                    else if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento
                        && cargaCancelamento.Carga.CargaCTes.Any(o => o.CTe != null
                        && o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Cancelada
                        && o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Inutilizada
                        && o.CTe.SituacaoCTeSefaz != SituacaoCTeSefaz.Anulado
                        && o.SistemaEmissor == SistemaEmissor.MultiCTe))
                    {

                        if (cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos && cargaCancelamento.CTe != null && cargaCancelamento.CTe.SituacaoCTeSefaz == SituacaoCTeSefaz.Cancelada && repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Unilever))
                            situacaoCTes = SituacaoCancelamentoDocumentoCarga.Sucesso;
                        else
                            situacaoCTes = SituacaoCancelamentoDocumentoCarga.Rejeicao;

                        if (!cargaCancelamento.LiberarCancelamentoComCTeNaoInutilizado && cargaCancelamento.Carga.CargaCTes.Any(o => o.CTe != null && o.CTe.SituacaoCTeSefaz == SituacaoCTeSefaz.Rejeitada && o.CTe.MensagemStatus != null && o.CTe.MensagemStatus.PermiteLiberarSemInutilizacao))
                            situacaoInutilizacao = SituacaoCancelamentoDocumentoCarga.Rejeicao;

                        if (cargaCancelamento.LiberarCancelamentoComCTeNaoInutilizado && cargaCancelamento.Carga.CargaCTes.Any(o => o.CTe != null && o.CTe.SituacaoCTeSefaz == SituacaoCTeSefaz.Rejeitada && o.CTe.MensagemStatus != null && o.CTe.MensagemStatus.CodigoDoErro == 652))
                            situacaoInutilizacao = SituacaoCancelamentoDocumentoCarga.Rejeicao;

                        if (!cargaCancelamento.LiberarCancelamentoComCTeNaoInutilizado && cargaCancelamento.Carga.CargaCTes.Any(o => o.CTe != null && o.CTe.SituacaoCTeSefaz == SituacaoCTeSefaz.Rejeitada && !string.IsNullOrWhiteSpace(o.CTe.MensagemRetornoSefaz) && o.CTe.MensagemRetornoSefaz.Contains("652")))
                            situacaoInutilizacao = SituacaoCancelamentoDocumentoCarga.Rejeicao;

                        if (cargaCancelamento.LiberarCancelamentoComCTeNaoInutilizado && cargaCancelamento.Carga.CargaCTes.Any(o => o.CTe != null && o.CTe.SituacaoCTeSefaz == SituacaoCTeSefaz.Rejeitada && !string.IsNullOrWhiteSpace(o.CTe.MensagemRetornoSefaz) && o.CTe.MensagemRetornoSefaz.Contains("Data de Validade do Certificado já expirou")))
                            situacaoInutilizacao = SituacaoCancelamentoDocumentoCarga.Rejeicao;

                        if (!cargaCancelamento.LiberarCancelamentoComCTeNaoInutilizado && cargaCancelamento.Carga.CargaCTes.Any(o => o.CTe != null && o.CTe.SituacaoCTeSefaz == SituacaoCTeSefaz.Rejeitada && !string.IsNullOrWhiteSpace(o.CTe.MensagemRetornoSefaz) && o.CTe.MensagemRetornoSefaz.Contains("Data de Validade do Certificado já expirou")))
                            situacaoInutilizacao = SituacaoCancelamentoDocumentoCarga.Rejeicao;

                        if (!cargaCancelamento.LiberarCancelamentoComCTeNaoInutilizado && cargaCancelamento.Carga.CargaCTes.Any(o => o.CTe != null && o.CTe.SituacaoCTeSefaz == SituacaoCTeSefaz.Rejeitada && !string.IsNullOrWhiteSpace(o.CTe.MensagemRetornoSefaz) && o.CTe.MensagemRetornoSefaz.Contains("Falha ao assinar Inutilização de numeração. DadosPFX, ArquivoPFX ou NumeroSerie não especificados !")))
                            situacaoInutilizacao = SituacaoCancelamentoDocumentoCarga.Rejeicao;
                    }

                    if (situacaoCTes == SituacaoCancelamentoDocumentoCarga.Sucesso)
                    {
                        if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.AgCancelamentoAverbacaoCTe)
                        {
                            situacaoAverbacaoCTes = SituacaoCancelamentoDocumentoCarga.Cancelando;
                            situacaoCTes = SituacaoCancelamentoDocumentoCarga.Cancelando;
                        }
                        else
                        {
                            if (!cargaCancelamento.LiberarCancelamentoComAverbacaoCTeRejeitada && repAverbacaoCTe.ExistePorCargaEStatus(cargaCancelamento.Carga.Codigo, Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso))
                            {
                                situacaoAverbacaoCTes = SituacaoCancelamentoDocumentoCarga.Rejeicao;
                                situacaoCTes = SituacaoCancelamentoDocumentoCarga.Rejeicao;
                            }
                            else if (!cargaCancelamento.LiberarCancelamentoComValePedagioRejeitado)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaIntegracoesValePedagio = repCargaIntegracaoValePedagio.BuscarPorCarga(cargaCancelamento.Carga.Codigo, true);

                                foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio in cargaIntegracoesValePedagio)
                                {
                                    if ((cargaIntegracaoValePedagio.SituacaoValePedagio == SituacaoValePedagio.Confirmada) ||
                                        (cargaIntegracaoValePedagio.SituacaoValePedagio == SituacaoValePedagio.Comprada && cargaIntegracaoValePedagio.TipoIntegracao.Tipo.IntegraCancelamentoValePedagio()))
                                    {
                                        situacaoValePedagio = SituacaoCancelamentoDocumentoCarga.Rejeicao;
                                        situacaoCTes = SituacaoCancelamentoDocumentoCarga.Rejeicao;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.AgIntegracao)
                    situacaoIntegracoes = SituacaoCancelamentoDocumentoCarga.Cancelando;
                else if (integracoes.Any(o => o.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao) ||
                         repCargaCancelamentoCargaIntegracao.ContarPorSituacaoConsulta(cargaCancelamento.Codigo, SituacaoIntegracao.ProblemaIntegracao) > 0)
                    situacaoIntegracoes = SituacaoCancelamentoDocumentoCarga.Rejeicao;

                var retorno = new
                {
                    cargaCancelamento.Codigo,
                    Carga = new
                    {
                        cargaCancelamento.Carga.Codigo,
                        cargaCancelamento.Carga.CodigoCargaEmbarcador,
                        Remetente = cargaCancelamento.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                        Origem = cargaCancelamento.Carga.DadosSumarizados?.Origens ?? string.Empty,
                        Destinatario = cargaCancelamento.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                        Destino = cargaCancelamento.Carga.DadosSumarizados?.Destinos ?? string.Empty,
                        Situacao = cargaCancelamento.Carga.DescricaoSituacaoCarga
                    },
                    UsuarioSolicitou = cargaCancelamento.Usuario?.Nome ?? cargaCancelamento.UsuarioERPSolicitouCancelamento,
                    Motivo = cargaCancelamento.MotivoCancelamento,
                    MotivoEscape = cargaCancelamento.MotivoCancelamento,
                    Justificativa = cargaCancelamento.Justificativa != null ? new { Codigo = cargaCancelamento.Justificativa?.Codigo ?? 0, Descricao = cargaCancelamento.Justificativa?.Descricao ?? "" } : null,
                    JustificativaCancelamentoCarga = cargaCancelamento.JustificativaCancelamentoCarga != null ? new { Codigo = cargaCancelamento.JustificativaCancelamentoCarga?.Codigo ?? 0, Descricao = cargaCancelamento.JustificativaCancelamentoCarga?.Descricao ?? "" } : null,
                    OperadorResponsavel = cargaCancelamento.OperadorResponsavel != null ? new { Codigo = cargaCancelamento.OperadorResponsavel?.Codigo ?? 0, Descricao = cargaCancelamento.OperadorResponsavel?.Descricao ?? "" } : null,
                    cargaCancelamento.DescricaoTipo,
                    cargaCancelamento.DescricaoSituacao,
                    cargaCancelamento.Tipo,
                    cargaCancelamento.TipoCancelamentoCargaDocumento,
                    cargaCancelamento.Situacao,
                    cargaCancelamento.MensagemRejeicaoCancelamento,
                    cargaCancelamento.DuplicarCarga,
                    DataCancelamento = cargaCancelamento.DataCancelamento.Value.ToString("dd/MM/yyyy"),
                    SituacaoMDFes = situacaoMDFes,
                    SituacaoAverbacaoMDFes = situacaoAverbacaoMDFes,
                    SituacaoCTes = situacaoCTes,
                    SituacaoAverbacaoCTes = situacaoAverbacaoCTes,
                    SituacaoIntegracoes = situacaoIntegracoes,
                    cargaCancelamento.GerouIntegracao,
                    NaoDuplicarCarga = !cargaCancelamento.DuplicarCarga,
                    SituacaoValePedagio = situacaoValePedagio,
                    SituacaoInutilizacao = situacaoInutilizacao,
                    CTeParaCancelamento = cargaCancelamento?.CTe?.Numero.ToString() ?? "",
                    SituacaoCIOT = situacaoCIOT,
                    Anexos = (from anexo in cargaCancelamento.Anexos
                              select new
                              {
                                  anexo.Codigo,
                                  anexo.Descricao,
                                  anexo.NomeArquivo,
                              }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaObterDetalhesCancelamento);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterCargasVinculadas()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.CargaVinculada repositorioCargaVinculada = new Repositorio.Embarcador.Cargas.CargaVinculada(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasVinculadas = repositorioCargaVinculada.BuscarCargasPorCarga(codigoCarga);

                if (cargasVinculadas.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.SemCargasViculadas);

                string numerosCargasVinculadas = string.Join(", ", cargasVinculadas.Select(carga => carga.CodigoCargaEmbarcador));

                var retorno = new
                {
                    CargasVinculadas = numerosCargasVinculadas
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.FalhaObterCargasVinculadas);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterFaturasFechadas()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);

                int codigoFaturaFechada = repFatura.ExisteFaturaFechadaPorCarga(codigoCarga);
                bool ContemFaturaComOutrasCargas = repFatura.ExisteOutrasCargasVinculadasNaFatura(codigoCarga, codigoFaturaFechada);

                var retorno = new
                {
                    ContemFaturasFechadas = codigoFaturaFechada > 0,
                    FaturaComOutrasCargas = ContemFaturaComOutrasCargas
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.FalhaObterCargasVinculadas);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCargaTelaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                var retorno = new
                {
                    Carga = new { Codigo = carga?.Codigo ?? 0, Descricao = carga?.CodigoCargaEmbarcador ?? string.Empty }
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaObterDadosGeraisCancelamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> UploadDesacordoCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count == 0)
                    return new JsonpResult(false, "Nenhum arquivo recebido.");

                int codigoCancelamento = Request.GetIntParam("Codigo");
                int codigoCte = Request.GetIntParam("CodigoCte");

                Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repostorioCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.XMLCTe repostorioXMLCte = new Repositorio.XMLCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repositorioCargaCancelamento.BuscarPorCodigo(codigoCancelamento);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repostorioCte.BuscarPorCodigo(codigoCte);
                Dominio.Entidades.XMLCTe xmlCte = repostorioXMLCte.BuscarPorCTe(codigoCte, Dominio.Enumeradores.TipoXMLCTe.Desacordo);

                if (cargaCancelamento == null)
                    return new JsonpResult(false, $"Não foi possivel achar registro com o codigo {codigoCte}");

                if (cte == null)
                    return new JsonpResult(false, $"CT-e não encontrado com o codigo {codigoCte}");

                if (xmlCte != null)
                    return new JsonpResult(false, $"Cte já foi validado com o xml de desacordo");

                Servicos.DTO.CustomFile file = files[0];
                StreamReader leitor = new StreamReader(file.InputStream);
                string conteudoXML = leitor.ReadToEnd();
                leitor.Close();

                if (System.IO.Path.GetExtension(file.FileName).ToLower() != ".xml")
                    return new JsonpResult(true, "Formato de arquivo não valido");

                if (!ValidarXmlDescordo(conteudoXML))
                    return new JsonpResult(true, "O xml enviado não é valido");

                xmlCte = new Dominio.Entidades.XMLCTe();
                xmlCte.CTe = cte;
                xmlCte.XML = conteudoXML;
                xmlCte.Tipo = Dominio.Enumeradores.TipoXMLCTe.Desacordo;
                xmlCte.XMLArmazenadoEmArquivo = false;
                repostorioXMLCte.Inserir(xmlCte);

                cargaCancelamento.AguardandoXmlDesacordo = false;
                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.AgCancelamentoCTe;

                GerarIntegracoesCancelamento(cargaCancelamento, unitOfWork);
                repositorioCargaCancelamento.Atualizar(cargaCancelamento);

                return new JsonpResult(true, "XML processado com sucesso em alguns instantes continuaremos com o fluxo");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu um problema ao tentar carregar o xml");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unidadeTrabalho, bool exportacao = false)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfiguracaoGeral.BuscarPrimeiroRegistro();

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCarga filtrosPesquisa = ObterFiltrosPesquisa(unidadeTrabalho);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);

            if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
            {
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Data, "DataCancelamento", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Carga, "Carga", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Remetente, "Remetente", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Destinatario, "Destinatario", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Situacao, "Situacao", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Solicitante, "Solicitante", 7, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.MotivoRejeicao, "MensagemRejeicaoCancelamento", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.PortoOrigem, "PortoOrigem", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.PortoDestino, "PortoDestino", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.NavioViagemDirecao, "Viagem", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Origens, "Origens", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Destinos, "Destinos", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("TipoCancelamentoCargaDocumento", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.MotivoCancelamento, "MotivoCancelamento", 15, Models.Grid.Align.left, true);
            }
            else
            {
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Data, "DataCancelamento", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Carga, "Carga", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Remetente, "Remetente", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Destinatario, "Destinatario", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Situacao, "Situacao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Solicitante, "Solicitante", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("MensagemRejeicaoCancelamento", false);
                grid.AdicionarCabecalho("PortoOrigem", false);
                grid.AdicionarCabecalho("PortoDestino", false);
                grid.AdicionarCabecalho("Viagem", false);
                grid.AdicionarCabecalho("Origens", false);
                grid.AdicionarCabecalho("Destinos", false);
                if (configuracaoGeralCarga.PermitirCancelarDocumentosCargaPeloCancelamentoCarga)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.TipoCancelamento, "TipoCancelamentoCargaDocumento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.MotivoCancelamento, "MotivoCancelamento", 15, Models.Grid.Align.left, true);
            }

            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

            int totalRegistros = repCargaCancelamento.ContarConsulta(filtrosPesquisa);
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.Cancelamento.CancelamentoCarga> cargasCancelamento = totalRegistros > 0 ? repCargaCancelamento.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Carga.Cancelamento.CancelamentoCarga>();

            var retorno = (from obj in cargasCancelamento
                           select new
                           {
                               obj.Codigo,
                               DataCancelamento = obj.DataCancelamento.HasValue ? obj.DataCancelamento.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                               Carga = obj.Carga,
                               Remetente = obj.Remetente,
                               Destinatario = obj.Destinatario,
                               Situacao = obj.SituacaoFormatada,
                               Solicitante = obj.Usuario,
                               obj.MensagemRejeicaoCancelamento,
                               PortoOrigem = obj.PortoOrigem,
                               PortoDestino = obj.PortoDestino,
                               Viagem = obj.Viagem,
                               Origens = obj.Origens,
                               Destinos = obj.Destinos,
                               MotivoCancelamento = obj.MotivoCancelamento,
                               TipoCancelamentoCargaDocumento = obj.TipoCancelamentoCargaDocumentoFormatada,
                           }).ToList();

            grid.AdicionaRows(retorno);
            grid.setarQuantidadeTotal(totalRegistros);

            if (exportacao)
            {
                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            else
                return new JsonpResult(grid);
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCarga ObterFiltrosPesquisa(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCarga filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCarga()
            {
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CodigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao"),
                CodigoTerminalOrigem = Request.GetIntParam("TerminalOrigem"),
                CodigoTerminalDestino = Request.GetIntParam("TerminalDestino"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigosEmpresas = (this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null ? this.Usuario.Empresas.Select(c => c.Codigo).ToList() : null,
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                TiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null,
                Situacao = Request.GetNullableEnumParam<SituacaoCancelamentoCarga>("Situacao"),
                Tipo = Request.GetNullableEnumParam<TipoCancelamentoCarga>("Tipo"),
                TipoCancelamentoCargaDocumento = Request.GetNullableEnumParam<TipoCancelamentoCargaDocumento>("TipoCancelamentoCargaDocumento"),
                CpfCnpjPessoa = Request.GetDoubleParam("Pessoa"),
                CargasLiberadasParaCancelamentoComRejeicaoIntegracao = Request.GetBoolParam("CargasLiberadasParaCancelamentoComRejeicaoIntegracao"),
            };

            filtroPesquisa.CodigosFilial = ObterListaCodigoFilialPermitidasOperadorLogistica(unidadeTrabalho);
            filtroPesquisa.CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unidadeTrabalho);
            filtroPesquisa.CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unidadeTrabalho);
            filtroPesquisa.CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unidadeTrabalho);

            return filtroPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Carga")
                propriedadeOrdenar = "Carga.CodigoCargaEmbarcador";

            return propriedadeOrdenar;
        }

        private async Task GerarLogCancelamentoAsync(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, TipoLogCargaCancelamento tipo, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoLog repCargaCancelamentoLog = new Repositorio.Embarcador.Cargas.CargaCancelamentoLog(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoLog log = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoLog()
            {
                Acao = tipo,
                CargaCancelamento = cargaCancelamento,
                Data = DateTime.Now,
                Usuario = Usuario
            };

            await repCargaCancelamentoLog.InserirAsync(log);
        }

        public void GerarIntegracoesDadosCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            if (new Servicos.Embarcador.Integracao.IntegracaoCargaCancelamentoDadosCancelamento().AdicionarIntegracoesDadosCancelamento(cargaCancelamento, unidadeTrabalho, TipoServicoMultisoftware))
            {
                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.AgIntegracaoDadosCancelamento;
            }
            else
            {
                if (configuracaoGeralCarga.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga)
                {
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgCancelamentoMDFe;
                }
                else
                {
                    cargaCancelamento.Situacao = SituacaoCancelamentoCarga.AgIntegracaoCancelamentoCIOT;
                }
            }
        }

        private void GerarIntegracoesCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (Servicos.Embarcador.Integracao.IntegracaoCargaCancelamento.AdicionarIntegracoesCarga(cargaCancelamento, unidadeTrabalho, TipoServicoMultisoftware))
                return;

            if (cargaCancelamento.Carga.EmpresaFilialEmissora != null && cargaCancelamento.Carga.EmpresaFilialEmissora.TransportadorLayoutsEDI.Any(obj => obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO))
                cargaCancelamento.GerouIntegracao = true;
            else if (cargaCancelamento.Carga.Empresa != null && cargaCancelamento.Carga.Empresa.TransportadorLayoutsEDI.Any(obj => obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTNC_CANCELAMENTO))
                cargaCancelamento.GerouIntegracao = true;
            else
                cargaCancelamento.GerouIntegracao = false;
        }

        private async Task<int> AdicionarCancelamentoAsync(Repositorio.UnitOfWork unidadeTrabalho, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas, TipoCancelamentoCarga? tipoCancelamentoFixo = null)
        {
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unidadeTrabalho);
            Servicos.Embarcador.Carga.CargaCancelamentoAprovacao servicoCargaCancelamentoAprovacao = new Servicos.Embarcador.Carga.CargaCancelamentoAprovacao(unidadeTrabalho);
            Servicos.Embarcador.Carga.Cancelamento servicoCargaCancelamento = new Servicos.Embarcador.Carga.Cancelamento();
            Servicos.Embarcador.Frete.ContratoFreteCliente.ContratoFreteCliente servicoContratoFreteCliente = new Servicos.Embarcador.Frete.ContratoFreteCliente.ContratoFreteCliente(unidadeTrabalho);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga repJustificativaCancelamentoCarga = new Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repConfigEmbarcador.BuscarConfiguracaoPadraoAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistroAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = await repositorioConfiguracaoFinanceiro.BuscarPrimeiroRegistroAsync();
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao existeTipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(TipoIntegracao.Unilever);

            int codigoCarga;
            int.TryParse(Request.Params("Carga"), out codigoCarga);

            int codigoCTeCancelamentoUnitario = Request.GetIntParam("CTeParaCancelamento");
            int codigoUsuarioSolicitou = Request.GetIntParam("UsuarioSolicitou");

            bool liberarCancelamentoCargaBloqueada = false;
            if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaCancelamento_CancelarCargaBloqueada))
                liberarCancelamentoCargaBloqueada = true;

            string motivo = Request.GetStringParam("MotivoEscape");
            motivo = HttpUtility.UrlDecode(motivo, System.Text.Encoding.UTF8);
            motivo = Utilidades.String.RemoveDiacritics(motivo).Trim();
            motivo = motivo.Replace("  ", "");

            bool cancelarDocumentosEmitidosNoEmbarcador = false;
            TipoCancelamentoCarga tipoCancelamento;
            if (tipoCancelamentoFixo.HasValue)
            {
                tipoCancelamento = tipoCancelamentoFixo.Value;
                cancelarDocumentosEmitidosNoEmbarcador = true;
            }
            else
                Enum.TryParse(Request.Params("Tipo"), out tipoCancelamento);

            if (tipoCancelamento == TipoCancelamentoCarga.Anulacao && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaCancelamento_Anular))
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.NaoPossuiPermissaoRealizarAnulacaoCarga);

            if ((string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length <= 20) && !configuracaoEmbarcador.UtilizaEmissaoMultimodal)
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.MotivoPossuirMaisCaracteres);

            if ((string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length <= 15) && configuracaoEmbarcador.UtilizaEmissaoMultimodal)
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.MotivoDevePossuirMaisQuinzeCaracteres);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga repPagamentoProvedorCarga = new Repositorio.Embarcador.Financeiro.PagamentoProvedorCarga(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);

            bool ctePermite = false, mdfePermite = false, averbacaoPermite = false, ciotPermite = false;


            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (!Servicos.Embarcador.Carga.Cancelamento.ValidarDocumentosCargaCancelamento(carga, out ctePermite, out mdfePermite, out averbacaoPermite, out ciotPermite, unidadeTrabalho))
                    throw new ControllerException($"Não é possível cancelar a carga se houver documentos com status Enviado, Pendente ou Ag. Integração !");
            }

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Cancelar) && repCarga.ExisteCargaComSituacaoNFEDocumentoEmitido(codigoCarga))
                    throw new ControllerException(Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);
            }

            if (carga.CargaBloqueadaParaEdicaoIntegracao || (carga.TipoOperacao?.CargaBloqueadaParaEdicaoIntegracao ?? false))
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.CargaBloqueadaNaoPermiteAlteracao);

            if (servicoCargaCancelamento.ValidaCargaEmProcessamento(carga, configuracaoEmbarcador, unidadeTrabalho, TipoServicoMultisoftware))
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.NaoPossivelGeraroCancelamentoEnquantoCargaEstiverProcessandoInformacoes);

            if (ValidaCargaRejeicaoNaoPermiteCancelamento(carga, configuracaoEmbarcador, out List<int> numeroCtes, TipoServicoMultisoftware, unidadeTrabalho))
                throw new ControllerException(string.Format(Localization.Resources.Cargas.CancelamentoCarga.NaoPossivelGerarCancelamentoCTePossuiRejeicaoNaoPermiteCancelamento, String.Join(",", numeroCtes)));

            if (configuracaoFinanceiro.ValidarDataPrevisaoPagamentoEDataPagamentoNoCancelamentoDosCTes && serCarga.ValidaPagamentoCTes(carga, out List<int> numeroCtesComPagamento))
                throw new ControllerException(string.Format(Localization.Resources.Cargas.CancelamentoCarga.NaoPossivelCancelarCargaCTePossuiTituloComDataPagamento, String.Join(", ", numeroCtesComPagamento)));

            bool duplicarCarga = !Request.GetBoolParam("NaoDuplicarCarga");

            if (!duplicarCarga && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaCancelamento_NaoDuplicarCarga))
                throw new ControllerException($"Usuário sem permissão para marcar a opção \"{(configuracaoEmbarcador.TrocarPreCargaPorCarga ? "Não Duplicar a Carga?" : "Não retornar para pré carga")}\".");

            if (duplicarCarga)
            {
                if (carga.CargaSVM || carga.CargaTakeOrPay)
                    duplicarCarga = false;
                else if (!configuracaoEmbarcador.TrocarPreCargaPorCarga)
                {
                    if (carga.CargaDePreCarga)
                        throw new ControllerException("Não é possível retornar uma pré carga para pré carga. Por favor, marque a opção \"Não retornar para pré carga\" para prosseguir!");

                    if (!carga.CargaDePreCargaFechada)
                        throw new ControllerException("Não é possível retornar uma carga que nunca foi pré carga para pré carga. Por favor, marque a opção \"Não retornar para pré carga\" para prosseguir!");

                    if (carga.CargaDePreCargaEmFechamento)
                        throw new ControllerException("Não é possível retornar uma carga durante o processo de fechamento para pré carga. Por favor, marque a opção \"Não retornar para pré carga\" para prosseguir!");
                }
                else
                {
                    bool permitirDuplicarCarga = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) || configuracaoEmbarcador.SempreDuplicarCargaCancelada;

                    if (!permitirDuplicarCarga)
                        duplicarCarga = false;
                }
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && duplicarCarga && repCargaPedido.ExisteCTeEmitidoNoEmbarcador(carga.Codigo) && repCargaDocumentoParaEmissaoNFSManual.ExistePorCarga(carga.Codigo))
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.NaoPossivelEfetuarCancelamentoAnulacao);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && repCargaCTe.ExisteCTeComTituloPorCarga(carga.Codigo))
                Servicos.Embarcador.Financeiro.Titulo.AnularTituloCancelamentoCargaAnulacao(carga.Codigo, unidadeTrabalho, Auditado);

            if (carga.CargaMDFes.Any(o => o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento || o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento))
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.CargaBloqueada);

            if (repCargaCancelamento.ExisteCargaPendenteDeCancelamento(carga?.Codigo ?? 0))
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.CargaJaCancelada);

            if (!liberarCancelamentoCargaBloqueada && carga.BloquearCancelamentoCarga)
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.CargaBloqueadaDocumentacao);

            if (carga.SituacaoCarga == SituacaoCarga.Cancelada)
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.CargaJaCancelada);

            if (carga.CargaRecebidaDeIntegracao)
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.CargaRecebidaDeIntegracao);

            if (Servicos.Embarcador.Carga.Carga.IsCargaBloqueada(carga, unidadeTrabalho))
                throw new ServicoException(Localization.Resources.Cargas.CancelamentoCarga.CargaBloqueadaNaoPermiteAlteracao);

            if (tipoCancelamento == TipoCancelamentoCarga.Cancelamento && carga.Ocorrencias.Any(o => o.SituacaoOcorrencia != SituacaoOcorrencia.Cancelada && o.SituacaoOcorrencia != SituacaoOcorrencia.Rejeitada && o.SituacaoOcorrencia != SituacaoOcorrencia.RejeitadaEtapaEmissao && (o.ValorOcorrencia > 0 || o.ValorICMS > 0)))
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.OcorrenciasNaoCanceladasParaCarga);

            if (existeTipoIntegracao != null)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = repositorioStage.BuscarStagesPorCarga(carga.Codigo);

                if (stages.Count > 0 && stages.Any(s => s.Cancelado == false && !string.IsNullOrEmpty(s.NumeroFolha)))
                    throw new ControllerException("Já existe FRS para a DT autorizada, é necessário cancelar a FRS para cancelar a carga.");
            }

            int quantidade = repCargaDocumentoParaEmissaoNFSManual.ContarPorCargaVinculadasNFSManual(carga.Codigo);
            if (quantidade > 0)
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.ExistemNFsNaoCanceladasCarga);

            if (tipoCancelamento == TipoCancelamentoCarga.Anulacao)
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (carga.CargaMDFes.Any(o => o.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado && o.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado))
                        throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.NaoPermiteAnulacaoMDFes);
                }
            }
            else if (tipoCancelamento == TipoCancelamentoCarga.Cancelamento)
            {
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in carga.CargaMDFes.ToList())
                {
                    if (cargaMDFe.MDFe != null && cargaMDFe.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorMDFe(cargaMDFe.MDFe.Codigo);
                        if (cargaMDFeManual != null)
                            throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.MDFesNaoCanceladasNaoPermiteCancelamento);
                    }
                }

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManualCarga = repCargaMDFeManual.BuscarPorCarga(carga.Codigo);
                if (cargaMDFeManualCarga != null)
                    throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.MDFesNaoCanceladasNaoPermiteCancelamento);

                if (repCargaIntegracaoValePedagio.VerificarSeExisteValePedagioPorStatus(carga.Codigo, SituacaoValePedagio.Encerrada))
                    throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.JaExistemValePedagiosCarga);

                if (repPagamentoProvedorCarga.VerificarSeExisteCargaEmPagamentoProvedor(carga.Codigo))
                    throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.ExistemPagamentoProvedorComCargaEmAberto);

                if (configuracaoEmbarcador.UtilizaEmissaoMultimodal)
                {
                    List<string> cargasVinculadas = repPedidoCTeParaSubContratacao.ContemCargaSubcontratada(carga.Codigo);
                    if (cargasVinculadas != null && cargasVinculadas.Count > 0)
                        throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.ExistemCargasVinculadasQueNaoEstaoCanceladas + string.Join(", ", cargasVinculadas));
                }

                if (!carga.CargaTransbordo && repCargaCTe.ExisteCTeVinculadoATransbordo(carga.Codigo))
                    throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.ExistemCTesDestaCargaVinculadosTransbordo);
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCargaNaoReprovada(codigoCarga);

            if (cargaCancelamento != null)
                throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.JaExisteUmaSolicitacaoDeCancelamentoCarga);

            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await ObterOperadorLogisticaAsync(unidadeTrabalho);

            if (operadorLogistica != null)
            {
                if (!(await serCarga.VerificarSeCargaEstaNaLogisticaAsync(carga, TipoServicoMultisoftware) || configuracaoEmbarcador.PermitirCancelamentoTotalCarga || (carga.SituacaoCarga == SituacaoCarga.AgNFe && operadorLogistica.SupervisorLogistica)))
                    throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.NaoEPossivelCancelarDepoisFaturamento);
                else
                {
                    if (carga.SituacaoCarga == SituacaoCarga.AgNFe && carga.ExigeNotaFiscalParaCalcularFrete && carga.TipoOperacao != null && !carga.TipoOperacao.PermiteImportarDocumentosManualmente && !operadorLogistica.SupervisorLogistica)
                        throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.NaoEPossivelCancelarDepoisFaturamento);
                }
            }

            if (carga.FreteDeTerceiro && !configuracaoGeralCarga.RealizarIntegracaoDadosCancelamentoCarga && !configuracaoGeralCarga.CancelarCIOTAutomaticamenteFluxoCancelamentoCarga)
            {
                if (carga.CargaCTes.Any(o => o.CIOTs.Any(c => !c.CIOT.CIOTPorPeriodo && (c.CIOT.Situacao == SituacaoCIOT.Aberto || c.CIOT.Situacao == SituacaoCIOT.AgLiberarViagem))))
                    throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.CargaPossuiCiotAberto);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && carga.CargaCTes.Any(o => o.CIOTs.Any(c => c.CIOT.Situacao == SituacaoCIOT.Encerrado)))
                    throw new ControllerException("A carga possui um CIOT encerrado, não sendo possível o cancelamento.");
            }

            if (carga.EmpresaFilialEmissora != null)
            {
                if (carga.Pedidos.Any(obj => obj.CargaPedidoProximoTrecho != null
                && (obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == SituacaoCarga.EmTransporte
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == SituacaoCarga.Encerrada
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == SituacaoCarga.AgIntegracao
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos
                || obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == SituacaoCarga.LiberadoPagamento
                )))
                {
                    throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.CargaPossuiRedespachoAutorizado);
                }
                else
                {
                    //se faltam apenas 5 minutos para emissão não permite cancelar sem cancelar o proximo trecho
                    if (carga.Pedidos.Any(obj => obj.CargaPedidoProximoTrecho != null && obj.CargaPedidoProximoTrecho.Carga.SituacaoCarga == SituacaoCarga.AgNFe
                        && (obj.CargaPedidoProximoTrecho.Carga.DataEnvioUltimaNFe.HasValue && obj.CargaPedidoProximoTrecho.Carga.DataEnvioUltimaNFe.Value >= DateTime.Now.AddMinutes(-5))))
                        throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.RedespachoEmitidoDaquiAlgunsMinutos);
                }
            }

            string erro = "";
            if (!Servicos.Embarcador.Carga.Cancelamento.VerificarSeJaIntegradoComERP(carga, out erro, Localization.Resources.Cargas.CancelamentoCarga.Cancelada, configuracaoEmbarcador, unidadeTrabalho))
                throw new ControllerException(erro);

            new Servicos.Embarcador.Integracao.Carrefour.IntegracaoCarrefour(unidadeTrabalho).ValidarPermissaoCancelarCarga(carga, motivo);
            new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unidadeTrabalho).ValidarPermissaoCancelarCarga(carga);

            await unidadeTrabalho.StartAsync();

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = codigoCTeCancelamentoUnitario > 0 ? repositorioCTe.BuscarPorCodigo(codigoCTeCancelamentoUnitario) : null;
            Dominio.Entidades.Usuario usuarioSolicitou = codigoUsuarioSolicitou > 0 ? await repUsuario.BuscarPorCodigoAsync(codigoUsuarioSolicitou) : null;

            Servicos.Embarcador.Fatura.Fatura servicoFatura = new Servicos.Embarcador.Fatura.Fatura(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);

            SituacaoCancelamentoCarga situacaoCancelamento = SituacaoCancelamentoCarga.EmCancelamento;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfigGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracao = await repositorioConfigGeralCarga.BuscarPrimeiroRegistroAsync();

            if (configuracao?.AtivarCancelamentoDeFaturaETituloAoFluxoDeCancelamentoNaCarga ?? false && !(carga?.CargaTransbordo ?? false))
            {
                if (cargaCancelamento != null)
                    situacaoCancelamento = SituacaoCancelamentoCarga.AgCancelamentoFatura;

                List<Dominio.Entidades.Embarcador.Fatura.Fatura> faturas = repFatura.BuscarPorCarga(codigoCarga);

                foreach (var fatura in faturas)
                {
                    try
                    {
                        servicoFatura.IniciarCancelamentoFatura(fatura.Codigo, motivo, Usuario, ConfiguracaoEmbarcador, Auditado, false, DateTime.Now);

                        if (cargaCancelamento != null)
                            fatura.CargaCancelamento = cargaCancelamento;

                        await repFatura.AtualizarAsync(fatura);
                    }
                    catch (Exception ex)
                    {
                        throw new ControllerException(ex.Message);
                    }
                }
            }

            cargaCancelamento = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamento
            {
                Carga = carga,
                DataCancelamento = DateTime.Now,
                DuplicarCarga = duplicarCarga,
                MotivoCancelamento = motivo,
                Tipo = tipoCancelamento,
                Usuario = Usuario,
                EnviouAverbacoesCTesParaCancelamento = true,
                SituacaoCargaNoCancelamento = carga.SituacaoCarga,
                CancelarDocumentosEmitidosNoEmbarcador = cancelarDocumentosEmitidosNoEmbarcador,
                Situacao = situacaoCancelamento,
                TipoCancelamentoCargaDocumento = carga.TipoOperacao?.ConfiguracaoCarga?.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga,
                CTe = cte,
                UsuarioSolicitouCancelamento = usuarioSolicitou
            };

            if (cte != null)
            {
                cte.UsuarioEmissaoCTe = Usuario;
                await repositorioCTe.AtualizarAsync(cte);
            }

            int codigoJustificativa = Request.GetIntParam("Justificativa");
            cargaCancelamento.Justificativa = codigoJustificativa > 0 ? repJustificativa.BuscarPorCodigo(codigoJustificativa) : null;

            long codigoJustificativaCancelamentoCarga = Request.GetLongParam("JustificativaCancelamentoCarga");
            cargaCancelamento.JustificativaCancelamentoCarga = codigoJustificativaCancelamentoCarga > 0 ? await repJustificativaCancelamentoCarga.BuscarPorCodigoAsync(codigoJustificativaCancelamentoCarga, false) : null;

            int codigoOperadorResponsavel = Request.GetIntParam("OperadorResponsavel");
            cargaCancelamento.OperadorResponsavel = codigoOperadorResponsavel > 0 ? await repUsuario.BuscarPorCodigoAsync(codigoOperadorResponsavel) : null;

            if (existeTipoIntegracao != null && configuracaoEmbarcador.LiberarPedidosParaMontagemCargaCancelada && (cargaCancelamento.Carga?.Carregamento != null || cargaCancelamento.Carga.CargaGeradaViaDocumentoTransporte))
                cargaCancelamento.LiberarPedidosParaMontagemCarga = true;
            else if (configuracaoEmbarcador.LiberarPedidosParaMontagemCargaCancelada && cargaCancelamento.Carga.Carregamento != null && !cargaCancelamento.Carga.CargaDePreCargaFechada && !carga.CargaDePreCargaEmFechamento)
                cargaCancelamento.LiberarPedidosParaMontagemCarga = !cargaCancelamento.DuplicarCarga;

            await repCargaCancelamento.InserirAsync(cargaCancelamento);

            await GerarLogCancelamentoAsync(cargaCancelamento, TipoLogCargaCancelamento.Emissao, unidadeTrabalho);
            GerarIntegracoesCancelamento(cargaCancelamento, unidadeTrabalho);

            if (configuracaoGeralCarga != null && configuracaoGeralCarga.RealizarIntegracaoDadosCancelamentoCarga)
                GerarIntegracoesDadosCancelamento(cargaCancelamento, unidadeTrabalho);

            servicoCargaCancelamentoAprovacao.CriarAprovacao(cargaCancelamento, TipoServicoMultisoftware);

            await repCargaCancelamento.AtualizarAsync(cargaCancelamento);

            string tipo = Localization.Resources.Cargas.CancelamentoCarga.Cancelamento;
            if (tipoCancelamento == TipoCancelamentoCarga.Anulacao)
            {
                tipo = Localization.Resources.Cargas.CancelamentoCarga.Anulacao;

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(unidadeTrabalho).Buscar();
                if (configuracaoIntegracaoUnilever?.PossuiIntegracaoUnilever ?? false)
                    Servicos.Embarcador.Carga.Cancelamento.RemoverDocumentosCargaComDesacordo(carga, codigoCTeCancelamentoUnitario, unidadeTrabalho);
            }

            string mensagemAuditoriaNaoDuplicarCarga = cargaCancelamento.DuplicarCarga ? string.Empty : $" ({Localization.Resources.Cargas.CancelamentoCarga.MarcouParametro} {(configuracaoEmbarcador.TrocarPreCargaPorCarga ? Localization.Resources.Cargas.CancelamentoCarga.NaoDuplicarCarga : Localization.Resources.Cargas.CancelamentoCarga.NaoRetornarPreCarga)})";
            string mensagemAuditoria = $"{Localization.Resources.Cargas.CancelamentoCarga.Adicionou} {tipo} {Localization.Resources.Cargas.CancelamentoCarga.DaCarga}{mensagemAuditoriaNaoDuplicarCarga}.";

            await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCancelamento, null, mensagemAuditoria, unidadeTrabalho);
            await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaCancelamento.Carga, null, mensagemAuditoria, unidadeTrabalho);

            servicoContratoFreteCliente.ExtornaSaldo(carga);
            Servicos.Embarcador.Escrituracao.CancelamentoPagamento.GerarCancelamentoPagamentoAutomatico(cargaCancelamento, unidadeTrabalho);

            await unidadeTrabalho.CommitChangesAsync();

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaVinculadaJanelaCarregamentoTransportador = repCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(cargaCancelamento.Carga.Codigo, cargaCancelamento.Carga.Empresa?.Codigo ?? 0);

            if (cargaVinculadaJanelaCarregamentoTransportador != null && cargaVinculadaJanelaCarregamentoTransportador.UsuarioResponsavel != null && cargaVinculadaJanelaCarregamentoTransportador.CargaJanelaCarregamento != null)
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, this.Cliente, TipoServicoMultisoftware, _conexao.AdminStringConexao);
                string mensagem = Localization.Resources.Cargas.CancelamentoCarga.SolicitadoCancelamentoCarga + cargaCancelamento.Carga.CodigoCargaEmbarcador;
                serNotificacao.GerarNotificacao(cargaVinculadaJanelaCarregamentoTransportador.UsuarioResponsavel, cargaVinculadaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Codigo, "Logistica/JanelaCarregamento", mensagem, IconesNotificacao.rejeitado, TipoNotificacao.janelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unidadeTrabalho);
            }

            if (configuracaoGeralCarga.AlertarTransportadorCancelamentoCarga)
            {
                Servicos.Embarcador.Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Servicos.Embarcador.Notificacao.NotificacaoEmpresa(unidadeTrabalho);

                StringBuilder mensagem = new StringBuilder();

                mensagem
                    .AppendLine($"{Localization.Resources.Cargas.CancelamentoCarga.Ola} ({cargaCancelamento.Carga.Empresa.CNPJ_Formatado}) {cargaCancelamento.Carga.Empresa.Descricao},").AppendLine()
                    .AppendLine($"{Localization.Resources.Cargas.CancelamentoCarga.ACarga} {cargaCancelamento.Carga.CodigoCargaEmbarcador} {Localization.Resources.Cargas.CancelamentoCarga.FoiCancelada}.").AppendLine();

                Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                {
                    AssuntoEmail = Localization.Resources.Cargas.CancelamentoCarga.CancelamentoDaCarga,
                    Empresa = cargaCancelamento.Carga.Empresa,
                    Mensagem = mensagem.ToString(),
                    NotificarSomenteEmailPrincipal = true
                };

                servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmpresa);
            }

            return cargaCancelamento.Codigo;
        }

        private bool ValidarXmlDescordo(string conteudoXML)
        {
            int codigoRespostaEsperado = 135;
            int.TryParse(Utilidades.XML.ObterConteudoTag(conteudoXML, "cStat"), out int codigoXml);

            string motivoEsperado = "Evento registrado e vinculado a CT-e";
            string motivoXml = Utilidades.XML.ObterConteudoTag(conteudoXML, "xMotivo");

            if (!codigoRespostaEsperado.Equals(codigoXml) || !motivoEsperado.Equals(motivoXml))
                return false;

            return true;
        }

        private bool ValidaCargaRejeicaoNaoPermiteCancelamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, out List<int> numeroCtes, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCte = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            numeroCtes = null;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                numeroCtes = repCte.BuscarPorCargaValidaRejeicaoCteNaoPermiteCancelamento(carga);

                if (numeroCtes != null && numeroCtes.Count() > 0)
                    return true;
            }

            return false;
        }

        #endregion
    }
}
