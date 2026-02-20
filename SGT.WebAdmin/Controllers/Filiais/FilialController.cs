using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Security.Cryptography.X509Certificates;

namespace SGT.WebAdmin.Controllers.Filiais
{
    [CustomAuthorize(new string[] { "ConfiguracaoGestaoPatio" }, "Filiais/Filial", "Filiais/SequenciaGestaoPatio")]
    public class FilialController : BaseController
    {
        #region Construtores

        public FilialController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Filiais.Filial repFilial = new(unitOfWork);

                Dominio.Entidades.Embarcador.Filiais.Filial filial = new();

                PreencherEntidade(filial, unitOfWork);

                if (!string.IsNullOrWhiteSpace(filial.CodigoFilialEmbarcador) && repFilial.buscarPorCodigoEmbarcador(filial.CodigoFilialEmbarcador) != null)
                    throw new ControllerException(Localization.Resources.Filiais.Filial.JaExisteFilialCadastradaComCodigoEmbarcador);

                await repFilial.InserirAsync(filial, Auditado);

                SalvarTiposOperacao(ref filial, unitOfWork);
                SalvarOutrosCodigosIntegracao(ref filial, repFilial);
                SalvarSetores(filial, unitOfWork);
                SalvarIntegracaoSemParar(filial, unitOfWork);
                SalvarIntegracaoTarget(filial, unitOfWork);
                SalvarIntegracaoBuonny(filial, unitOfWork);
                AtualizarDescontos(filial, unitOfWork);
                AtualizarDescontosExcecao(filial, unitOfWork);
                AtualizarModelosVeicularesCarga(filial, null, unitOfWork);
                SalvarAlertasSla(filial, unitOfWork);
                SalvarBalancas(filial, unitOfWork);
                SalvarTiposOperacaoTrizy(filial, unitOfWork);
                SalvarEstadoDestinoEmpresaEmissora(ref filial, unitOfWork);

                AdicionarOuAtualizarCondicoesPagamento(filial, unitOfWork);
                AdicionarOuValePedagioTranportadores(filial, unitOfWork);
                AdicionarOuAtualizarArmazem(filial, unitOfWork);
                SalvarGestaoPatio(filial, TipoFluxoGestaoPatio.Origem, unitOfWork);
                SalvarGestaoPatio(filial, TipoFluxoGestaoPatio.Destino, unitOfWork);
                await SalvarTiposIntegracaoAsync(filial, unitOfWork, cancellationToken);

                await repFilial.AtualizarAsync(filial);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AdicionarSequenciaGestaoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoFilial = Request.GetIntParam("Filial");
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(codigoFilial);

                if (filial == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                if (tipoOperacao == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                if (new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(unitOfWork).ExistePorFilialETipoOperacao(filial.Codigo, tipoOperacao.Codigo))
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroDuplicadoMensagem);

                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio gestaoPatioOrigem = SalvarGestaoPatio(filial, tipoOperacao, TipoFluxoGestaoPatio.Origem, unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio gestaoPatioDestino = SalvarGestaoPatio(filial, tipoOperacao, TipoFluxoGestaoPatio.Destino, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, string.Format(Localization.Resources.Filiais.Filial.AdicionouSequenciaGestaoPatio, $" {Localization.Resources.Filiais.Filial.DoTipoOperacao} {tipoOperacao.Descricao}"), unitOfWork);

                if (gestaoPatioDestino != null)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, string.Format(Localization.Resources.Filiais.Filial.AdicionouSequenciaGestaoPatio, $" {Localization.Resources.Filiais.Filial.DeDestino} {Localization.Resources.Filiais.Filial.DoTipoOperacao} {tipoOperacao.Descricao}"), unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);

                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                if (filial == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                PreencherEntidade(filial, unitOfWork);

                if (!string.IsNullOrEmpty(filial.CodigoFilialEmbarcador) && repFilial.ValidarPorCodigoEmbarcador(filial.CodigoFilialEmbarcador, filial.Codigo))
                    throw new ControllerException(Localization.Resources.Filiais.Filial.JaExisteFilialCadastradaComCodigoEmbarcador);

                SalvarTiposOperacao(ref filial, unitOfWork);
                SalvarOutrosCodigosIntegracao(ref filial, repFilial);
                SalvarSetores(filial, unitOfWork);
                SalvarIntegracaoSemParar(filial, unitOfWork);
                SalvarIntegracaoTarget(filial, unitOfWork);
                SalvarIntegracaoBuonny(filial, unitOfWork);
                SalvarEstadoDestinoEmpresaEmissora(ref filial, unitOfWork);
                AtualizarDescontos(filial, unitOfWork);
                AtualizarDescontosExcecao(filial, unitOfWork);
                SalvarAlertasSla(filial, unitOfWork);
                SalvarBalancas(filial, unitOfWork);
                SalvarTiposOperacaoTrizy(filial, unitOfWork);
                ExcluirTanquesRemovidos(filial, unitOfWork);
                AtualizarTanques(filial, unitOfWork);
                AdicionarOuAtualizarArmazem(filial, unitOfWork);
                await SalvarTiposIntegracaoAsync(filial, unitOfWork, cancellationToken);

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repFilial.Atualizar(filial, Auditado);

                AdicionarOuAtualizarCondicoesPagamento(filial, unitOfWork);
                AdicionarOuValePedagioTranportadores(filial, unitOfWork);
                AtualizarModelosVeicularesCarga(filial, historico, unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio gestaoPatioOrigem = SalvarGestaoPatio(filial, TipoFluxoGestaoPatio.Origem, unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio gestaoPatioDestino = SalvarGestaoPatio(filial, TipoFluxoGestaoPatio.Destino, unitOfWork);
                Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(Auditado, filial, gestaoPatioOrigem.GetChanges(), string.Format(Localization.Resources.Filiais.Filial.AlterouSequenciaGestaoPatio, ""), unitOfWork);

                if (gestaoPatioDestino != null)
                    Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(Auditado, filial, gestaoPatioDestino.GetChanges(), string.Format(Localization.Resources.Filiais.Filial.AlterouSequenciaGestaoPatio, $" {Localization.Resources.Filiais.Filial.DeDestino}"), unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarSequenciaGestaoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoFilial = Request.GetIntParam("Filial");
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(codigoFilial);

                if (filial == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                if (tipoOperacao == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio gestaoPatioOrigem = SalvarGestaoPatio(filial, tipoOperacao, TipoFluxoGestaoPatio.Origem, unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio gestaoPatioDestino = SalvarGestaoPatio(filial, tipoOperacao, TipoFluxoGestaoPatio.Destino, unitOfWork);
                Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(Auditado, filial, gestaoPatioOrigem.GetChanges(), string.Format(Localization.Resources.Filiais.Filial.AlterouSequenciaGestaoPatio, $" {Localization.Resources.Filiais.Filial.DoTipoOperacao} {tipoOperacao.Descricao}"), unitOfWork);

                if (gestaoPatioDestino != null)
                    Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(Auditado, filial, gestaoPatioDestino.GetChanges(), string.Format(Localization.Resources.Filiais.Filial.AlterouSequenciaGestaoPatio, $" {Localization.Resources.Filiais.Filial.DeDestino} {Localization.Resources.Filiais.Filial.DoTipoOperacao} {tipoOperacao.Descricao}"), unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BaixarQrCodeEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilial = Request.GetIntParam("Filial");
                EtapaFluxoGestaoPatio etapa = Request.GetEnumParam<EtapaFluxoGestaoPatio>("Etapa");
                TipoFluxoGestaoPatio tipo = Request.GetEnumParam<TipoFluxoGestaoPatio>("Tipo");
                Servicos.Embarcador.Filiais.Filial servicoFilial = new Servicos.Embarcador.Filiais.Filial(unitOfWork);
                byte[] pdf = servicoFilial.ObterPdfQrCodeEtapa(codigoFilial, etapa, tipo);

                return Arquivo(pdf, "application/pdf", $"{Localization.Resources.Filiais.Filial.QRCodeEtapa} {etapa.ObterDescricao()}.pdf");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracaoSequenciaGestaoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilial = Request.GetIntParam("CodigoFilial");
                Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(unitOfWork);
                List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> sequenciasGestaoPatioDestino = (codigoFilial > 0) ? repositorioSequenciaGestaoPatio.BuscarTodosPorFilial(codigoFilial) : new List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio>();

                return new JsonpResult(new
                {
                    PermitirInformacoesProdutor = sequenciasGestaoPatioDestino.Any(o => o.GuaritaEntradaPermiteInformacoesPesagem && o.GuaritaEntradaPermiteInformacoesProdutor),
                    ExibirImprimirTicketBalanca = sequenciasGestaoPatioDestino.Any(o => o?.ImprimirTicketBalanca ?? false)
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Filiais.Filial repFilial = new(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.Filial filial = await repFilial.BuscarPorCodigoAsync(codigo);

                Repositorio.Embarcador.Tanques.FilialTanque repFilialTanque = new(unitOfWork);
                List<Dominio.Entidades.Embarcador.Filiais.FilialTanque> filialTanque = repFilialTanque.BuscarPorFilial(filial.Codigo);

                Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial repositorioValePedagioTransportador = new(unitOfWork);
                List<Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial> listaValePedagioTransportador = repositorioValePedagioTransportador.BuscarPorFilial(filial.Codigo);

                Repositorio.Embarcador.Filiais.FilialArmazem repositorioFilialArmazem = new(unitOfWork);
                List<Dominio.Entidades.Embarcador.Filiais.FilialArmazem> listaFilialArmazem = repositorioFilialArmazem.BuscarPorFilial(filial.Codigo);

                Repositorio.Embarcador.Filiais.FilialModeloVeicularCarga repositorioFilialModeloVeicularCarga = new(unitOfWork);
                List<Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga> modelosVeicularesCarga = repositorioFilialModeloVeicularCarga.BuscarPorFilial(filial.Codigo);
                Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatioOrigem = repositorioSequenciaGestaoPatio.BuscarPorFilialETipoSemTipoOperacao(filial.Codigo, TipoFluxoGestaoPatio.Origem);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatioDestino = repositorioSequenciaGestaoPatio.BuscarPorFilialETipoSemTipoOperacao(filial.Codigo, TipoFluxoGestaoPatio.Destino);

                Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora repositorioEstadoDestinoFilialEmissora = new(unitOfWork);
                List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> estadosDestinoEmpresaEmissora = repositorioEstadoDestinoFilialEmissora.BuscarPorFilial(filial.Codigo);

                Repositorio.Embarcador.GestaoPatio.TipoChecklistImpressao repositorioTipoChecklistImpressao = new(unitOfWork);

                Repositorio.Embarcador.Filiais.FilialTipoIntegracao repositorioFilialTipoIntegracao = new(unitOfWork);
                List<TipoIntegracao> tiposIntegracao = await repositorioFilialTipoIntegracao.BuscarTiposIntegracaoPorFilialAsync(codigo, cancellationToken);

                var dynFilial = new
                {
                    filial.Codigo,
                    filial.CodigoFilialEmbarcador,
                    filial.Descricao,
                    filial.NumeroUnidadeImpressao,
                    filial.TipoFilial,
                    filial.Email,
                    CNPJ = filial.CNPJ_Formatado,
                    Atividade = filial.Atividade != null ? filial.Atividade.Codigo : 1,
                    filial.Ativo,
                    filial.ControlaExpedicao,
                    filial.NaoAdicionarValorDescarga,
                    filial.EmiteMDFeFilialEmissora,
                    filial.EmiteMDFeFilialEmissoraPorEstadoDestino,
                    filial.EmitirMDFeManualmente,
                    filial.UtilizarCtesAnterioresComoCteFilialEmissora,
                    filial.ExigirPreCargaMontagemCarga,
                    filial.ExigirConfirmacaoTransporte,
                    filial.GerarCIOTParaTodasAsCargas,
                    filial.NaoPermitirAgruparCargaMesmaFilial,
                    filial.ExigeConfirmacaoFreteAntesEmissao,
                    filial.LiberarAutomaticamentePagamento,
                    filial.HorarioCorteParaCalculoLeadTime,
                    filial.HabilitarPreViagemTrizy,
                    filial.InformarEquipamentoFluxoPatio,
                    filial.HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos,
                    AlertarDiariaContainer = !string.IsNullOrEmpty(filial.EmailDiariaContainer),
                    filial.EmailDiariaContainer,
                    filial.NotificarContainerCargaCancelada,
                    filial.SiglaFilial,
                    filial.AccountNameVtex,
                    filial.NaoValidarVeiculoIntegracao,
                    ValorMedioMercadoria = filial.ValorMedioMercadoria > 0m ? filial.ValorMedioMercadoria.ToString("n2") : "",
                    TipoCarga = new { Codigo = filial.TipoDeCarga?.Codigo ?? 0, Descricao = filial.TipoDeCarga?.Descricao ?? "" },
                    TipoOperacaoRedespacho = new { Codigo = filial.TipoOperacaoRedespacho?.Codigo ?? 0, Descricao = filial.TipoOperacaoRedespacho?.Descricao ?? "" },
                    EmpresaEmissora = filial.EmpresaEmissora == null ? null : new { filial.EmpresaEmissora.Codigo, filial.EmpresaEmissora.Descricao },
                    Localidade = new { Codigo = filial.Localidade != null ? filial.Localidade.Codigo : 0, Descricao = filial.Localidade != null ? filial.Localidade.DescricaoCidadeEstado : "" },
                    DataInicialCertificado = filial.DataInicialCertificado != null ? filial.DataInicialCertificado.Value.ToString("dd/MM/yyyy") : "",
                    DataFinalCertificado = filial.DataFinalCertificado != null ? filial.DataFinalCertificado.Value.ToString("dd/MM/yyyy") : "",
                    filial.SerieCertificado,
                    filial.SenhaCertificado,
                    PossuiCertificado = !string.IsNullOrWhiteSpace(filial.NomeCertificado) ? Utilidades.IO.FileStorageService.Storage.Exists(filial.NomeCertificado) : false,
                    SetorAtendimento = new { Codigo = filial.SetorAtendimento?.Codigo ?? 0, Descricao = filial.SetorAtendimento?.Descricao ?? "" },
                    ConfiguracaoCIOT = new { Codigo = filial.ConfiguracaoCIOT?.Codigo ?? 0, Descricao = filial.ConfiguracaoCIOT?.Descricao ?? "" },
                    CondicaoPagamento = new
                    {
                        filial.AtivarCondicao,
                        CondicoesPagamento = (
                            from condicao in filial.CondicoesPagamento
                            select new
                            {
                                condicao.Codigo,
                                CodigoTipoCarga = condicao.TipoDeCarga?.Codigo ?? 0,
                                CodigoTipoOperacao = condicao.TipoOperacao?.Codigo ?? 0,
                                DescricaoTipoCarga = condicao.TipoDeCarga?.Descricao ?? "",
                                DescricaoTipoOperacao = condicao.TipoOperacao?.Descricao ?? "",
                                condicao.DiaEmissaoLimite,
                                DiaEmissaoLimiteDescricao = condicao.DiaEmissaoLimite > 0 ? condicao.DiaEmissaoLimite.Value.ToString() : Localization.Resources.Gerais.Geral.SemConfiguracao,
                                condicao.DiaMes,
                                DiaMesDescricao = condicao.DiaMes > 0 ? condicao.DiaMes.Value.ToString() : Localization.Resources.Gerais.Geral.SemConfiguracao,
                                condicao.DiasDePrazoPagamento,
                                DiasDePrazoPagamentoDescricao = condicao.DiasDePrazoPagamento > 0 ? condicao.DiasDePrazoPagamento.Value.ToString() : Localization.Resources.Gerais.Geral.SemConfiguracao,
                                condicao.DiaSemana,
                                DiaSemanaDescricao = condicao.DiaSemana.HasValue ? condicao.DiaSemana.Value.ObterDescricao() : Localization.Resources.Gerais.Geral.SemConfiguracao,
                                condicao.TipoPrazoPagamento,
                                TipoPrazoPagamentoDescricao = condicao.TipoPrazoPagamento.HasValue ? condicao.TipoPrazoPagamento.Value.ObterDescricao() : Localization.Resources.Gerais.Geral.SemConfiguracao,
                                condicao.VencimentoForaMes,
                                condicao.ConsiderarDiaUtilVencimento,
                                VencimentoForaMesDescricao = condicao.VencimentoForaMes ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao
                            }
                        ).ToList()
                    },
                    OutrosCodigosIntegracao = (
                        from obj in filial.OutrosCodigosIntegracao
                        select new
                        {
                            Codigo = obj,
                            CodigoIntegracao = obj
                        }
                    ).ToList(),
                    TiposOperacoes = (
                        from obj in filial.TipoOperacoesIsencaoValorDescargaCliente
                        orderby obj.Descricao
                        select new
                        {
                            Tipo = new
                            {
                                obj.Codigo,
                                obj.Descricao
                            }
                        }
                    ).ToList(),
                    SequenciaGestaoPatio = RetornaDynSequenciaGestaoPatio(sequenciaGestaoPatioOrigem, unitOfWork),
                    SequenciaGestaoPatioDestino = RetornaDynSequenciaGestaoPatio(sequenciaGestaoPatioDestino, unitOfWork),
                    OrdemGestaoPatio = RetornaDynOrdemGestaoPatio(sequenciaGestaoPatioOrigem),
                    OrdemGestaoPatioDestino = RetornaDynOrdemGestaoPatioDestino(sequenciaGestaoPatioDestino),
                    ExisteTipoCheckListImpressao = await repositorioTipoChecklistImpressao.VerificarExistenciaAsync(cancellationToken),
                    SetoresFilial = (
                        from setorFilial in filial.Setores
                        orderby setorFilial.Setor.Descricao
                        select new
                        {
                            setorFilial.Codigo,
                            Setor = new
                            {
                                setorFilial.Setor.Codigo,
                                setorFilial.Setor.Descricao
                            },
                            Turnos = (
                                from turno in setorFilial.Turnos
                                orderby turno.Descricao
                                select new
                                {
                                    turno.Codigo,
                                    turno.Descricao
                                }
                            ).ToList()
                        }
                    ).ToList(),
                    IntegracaoSemParar = RetornaDynIntegracaoSemParar(filial.IntegracaoSemParar),
                    IntegracaoTarget = RetornaDynIntegracaoTarget(filial.IntegracaoTarget),
                    IntegracaoBuonny = RetornaDynIntegracaoBuonny(filial.IntegracaoBuonny),
                    Descontos = (
                        from desconto in filial.Descontos
                        select new
                        {
                            desconto.Codigo,
                            CodigoModeloVeicular = desconto.ModeloVeicularCarga?.Codigo,
                            DescricaoModeloVeicular = desconto.ModeloVeicularCarga?.Descricao,
                            ValorDesconto = desconto.ValorDesconto.ToString("n2"),
                            CodigoTipoOperacao = desconto.TipoOperacao?.Codigo,
                            DescricaoTipoOperacao = desconto.TipoOperacao?.Descricao
                        }
                    ).ToList(),
                    DescontosExcecao = (
                        from desconto in filial.DescontosExcecao
                        select new
                        {
                            desconto.Codigo,
                            CodigoModeloVeicular = desconto.ModeloVeicularCarga?.Codigo,
                            DescricaoModeloVeicular = desconto.ModeloVeicularCarga?.Descricao,
                            CodigoTransportador = desconto.Transportador?.Codigo,
                            DescricaoTransportador = desconto.Transportador?.Descricao,
                            CodigoProduto = desconto.Produto?.Codigo,
                            DescricaoProduto = desconto.Produto?.Descricao,
                            HoraInicio = desconto.HoraInicio,
                            HoraFim = desconto.HoraFim
                        }
                    ).ToList(),
                    ValePedagioTransportadores = (
                        from o in listaValePedagioTransportador
                        select new
                        {
                            o.Codigo,
                            CodigoTransportador = o.Transportador.Codigo,
                            o.ComprarValePedagio,
                            TipoIntegracaoValePedagio = o.TipoIntegracaoValePedagio?.Tipo ?? TipoIntegracao.NaoInformada,
                            DescricaoTransportador = o.Transportador.Descricao,
                            DescricaoComprarValePedagio = o.ComprarValePedagio ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                            DescricaoTipoIntegracaoValePedagio = o.ComprarValePedagio ? (o.TipoIntegracaoValePedagio?.Tipo ?? TipoIntegracao.NaoInformada).ObterDescricao() : ""
                        }
                    ).ToList(),
                    Armazem = (
                        from o in listaFilialArmazem
                        select new
                        {
                            o.Codigo,
                            o.Descricao,
                            o.CodigoIntegracao,
                            o.Situacao,
                            SituacaoDescricao = o.Situacao ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo
                        }
                    ).ToList(),
                    ListaModeloVeicularCarga = (
                        from o in modelosVeicularesCarga
                        select new
                        {
                            o.Codigo,
                            o.IntegrarOrdemEmbarque,
                            ModeloVeicularCarga = new { o.ModeloVeicularCarga.Codigo, o.ModeloVeicularCarga.Descricao }
                        }
                    ).ToList(),
                    ListaAlertasSla = RetornarDynAlertasSla(filial, unitOfWork),
                    Balancas = RetornaDynBalanca(filial, unitOfWork),
                    DiasDeCortePlanejamentoDiario = filial.DiasDeCortePlanejamentoDiario,
                    HoraInicialPlanejamentoDiario = filial.HoraInicialPlanejamentoDiario.HasValue ? filial.HoraInicialPlanejamentoDiario.Value.ToString("HH:mm") : null,
                    HoraFinalPlanejamentoDiario = filial.HoraFinalPlanejamentoDiario.HasValue ? filial.HoraFinalPlanejamentoDiario.Value.ToString("HH:mm") : null,
                    GerarIntegracaoP44 = filial.GerarIntegracaoP44,
                    GerarIntegracaoKlios = filial.GerarIntegracaoKlios,
                    Tanques = (
                        from tanque in filialTanque
                        select new
                        {
                            tanque?.Codigo,
                            tanque?.Tanque?.Descricao,
                            tanque?.Tanque?.ID,
                            tanque?.Volume,
                            tanque?.Capacidade,
                            tanque?.Vazao,
                            tanque?.Ocupacao,
                            DataAtualizacao = tanque?.DataAtualizacao.ToString("dd/MM/yyyy HH:mm"),
                            tanque?.Status
                        }
                    ).ToList(),
                    TipoOperacao = (
                        from obj in filial.TipoOperacoesTrizy
                        select new
                        {
                            Tipo = new
                            {
                                obj.Codigo,
                                obj.Descricao
                            }
                        }
                    ).ToList(),
                    EstadoDestinoEmpresaEmissora = (
                    from obj in estadosDestinoEmpresaEmissora
                    select new
                    {
                        Codigo = obj.Estado.Sigla,
                        obj.Estado.Descricao,
                        EmpresaCodigo = obj.Empresa.Codigo,
                        EmpresaDescricao = obj.Empresa.Descricao
                    }).ToList(),
                    TipoIntegracao = tiposIntegracao.ToList(),
                };
                return new JsonpResult(dynFilial);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarSequenciaGestaoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilial = Request.GetIntParam("Filial");
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(codigoFilial);

                if (filial == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                if (tipoOperacao == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatioOrigem = repositorioSequenciaGestaoPatio.BuscarPorFilialTipoETipoOperacao(filial.Codigo, tipoOperacao.Codigo, TipoFluxoGestaoPatio.Origem);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatioDestino = repositorioSequenciaGestaoPatio.BuscarPorFilialTipoETipoOperacao(filial.Codigo, tipoOperacao.Codigo, TipoFluxoGestaoPatio.Destino);

                var dynFilial = new
                {
                    Codigo = sequenciaGestaoPatioDestino?.Codigo ?? sequenciaGestaoPatioOrigem.Codigo,
                    Filial = new { filial.Codigo, filial.Descricao, filial.GerarIntegracaoP44 },
                    TipoOperacao = new { tipoOperacao.Codigo, tipoOperacao.Descricao },
                    SequenciaGestaoPatio = RetornaDynSequenciaGestaoPatio(sequenciaGestaoPatioOrigem, unitOfWork),
                    SequenciaGestaoPatioDestino = RetornaDynSequenciaGestaoPatio(sequenciaGestaoPatioDestino, unitOfWork),
                    OrdemGestaoPatio = RetornaDynOrdemGestaoPatio(sequenciaGestaoPatioOrigem),
                    OrdemGestaoPatioDestino = RetornaDynOrdemGestaoPatioDestino(sequenciaGestaoPatioDestino)
                };

                return new JsonpResult(dynFilial);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoGestaoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                TipoFluxoGestaoPatio tipo = Request.GetEnumParam<TipoFluxoGestaoPatio>("Tipo");
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa> etapas = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterEtapas(tipo);

                return new JsonpResult(new
                {
                    Etapas = etapas
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("codigo");
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(codigo);

                if (filial == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(unitOfWork);
                List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> sequenciasGestaoPatio = repositorioSequenciaGestaoPatio.BuscarTodosPorFilial(filial.Codigo);

                foreach (Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio in sequenciasGestaoPatio)
                    repositorioSequenciaGestaoPatio.Deletar(sequenciaGestaoPatio, Auditado);

                filial.TipoOperacoesIsencaoValorDescargaCliente.Clear();
                repositorioFilial.Deletar(filial, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirSequenciaGestaoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Filial");
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(codigo);

                if (filial == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                if (tipoOperacao == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatioOrigem = repositorioSequenciaGestaoPatio.BuscarPorFilialTipoETipoOperacao(filial.Codigo, tipoOperacao.Codigo, TipoFluxoGestaoPatio.Origem);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatioDestino = repositorioSequenciaGestaoPatio.BuscarPorFilialTipoETipoOperacao(filial.Codigo, tipoOperacao.Codigo, TipoFluxoGestaoPatio.Destino);

                repositorioSequenciaGestaoPatio.Deletar(sequenciaGestaoPatioOrigem);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, string.Format(Localization.Resources.Filiais.Filial.RemoveuSequenciaGestaoPatio, $" {Localization.Resources.Filiais.Filial.DoTipoOperacao} {tipoOperacao.Descricao}"), unitOfWork);

                if (sequenciaGestaoPatioDestino != null)
                {
                    repositorioSequenciaGestaoPatio.Deletar(sequenciaGestaoPatioDestino);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, string.Format(Localization.Resources.Filiais.Filial.RemoveuSequenciaGestaoPatio, $" {Localization.Resources.Filiais.Filial.DeDestino} {Localization.Resources.Filiais.Filial.DoTipoOperacao} {tipoOperacao.Descricao}"), unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filial;

                int codigoFilial;
                int.TryParse(Request.Params("CodigoFilial"), out codigoFilial);

                string senha = Request.Params("SenhaCertificado");

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                filial = repFilial.BuscarPorCodigo(codigoFilial);

                if (filial == null)
                    return new JsonpResult(false, Localization.Resources.Filiais.Filial.FilialNaoEncontrada);

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, Localization.Resources.Filiais.Filial.SelecioneCertificadoEnvio);

                Servicos.DTO.CustomFile file = files[0];

                if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".pfx")
                    return new JsonpResult(false, string.Format(Localization.Resources.Gerais.Geral.ExtensaoArquivoInvalidaSelecioneArquivoComExtensao, ".pfx"));

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosEmpresas, Utilidades.String.OnlyNumbers(filial.CNPJ), "CertificadoFilial");

                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, filial.CNPJ + ".pfx");

                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    X509Certificate2 certificado = new X509Certificate2(binaryReader.ReadBytes((int)file.Length), senha);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                    file.SaveAs(caminho);

                    filial.NomeCertificado = caminho;
                    filial.SenhaCertificado = senha;
                    filial.SerieCertificado = certificado.SerialNumber;
                    filial.DataInicialCertificado = certificado.NotBefore;
                    filial.DataFinalCertificado = certificado.NotAfter;

                    repFilial.Atualizar(filial);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, Localization.Resources.Filiais.Filial.AtualizouCertificadoFilial, unitOfWork);
                }

                return new JsonpResult(new
                {
                    filial.SerieCertificado,
                    DataInicialCertificado = filial.DataInicialCertificado.Value.ToString("dd/MM/yyyy"),
                    DataFinalCertificado = filial.DataFinalCertificado.Value.ToString("dd/MM/yyyy"),
                    filial.SenhaCertificado
                });
            }
            catch (System.Security.Cryptography.CryptographicException ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Filiais.Filial.SenhaInvalidaNaoFoiPossivelExtrairInformacoesCertificado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoSalvar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFilial;
                int.TryParse(Request.Params("CodigoFilial"), out codigoFilial);

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(codigoFilial);

                if (filial == null)
                    return new JsonpResult(false, Localization.Resources.Filiais.Filial.FilialNaoEncontrada);

                if (!string.IsNullOrWhiteSpace(filial.NomeCertificado) && Utilidades.IO.FileStorageService.Storage.Exists(filial.NomeCertificado))
                    Utilidades.IO.FileStorageService.Storage.Delete(filial.NomeCertificado);

                filial.NomeCertificado = string.Empty;
                filial.DataInicialCertificado = null;
                filial.DataFinalCertificado = null;
                filial.SerieCertificado = string.Empty;
                filial.SenhaCertificado = string.Empty;

                repFilial.Atualizar(filial);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, Localization.Resources.Filiais.Filial.RemoveuCertificadoFilial, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFilial = 0;
                int.TryParse(Request.Params("CodigoFilial"), out codigoFilial);

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(codigoFilial);

                if (filial == null)
                    return new JsonpResult(false, Localization.Resources.Filiais.Filial.FilialNaoEncontrada);

                if (string.IsNullOrWhiteSpace(filial.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(filial.NomeCertificado))
                    return new JsonpResult(false, Localization.Resources.Filiais.Filial.CertificadoNaoEncontrado);

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(filial.NomeCertificado), "application/x-pkcs12", filial.CNPJ + ".pfx");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTodas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);

                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = repOperadorLogistica.BuscarPorUsuario(this.Usuario.Codigo);

                List<Dominio.Entidades.Embarcador.Filiais.Filial> listaFilial = null;
                if (operadorLogistica != null)
                {
                    if (operadorLogistica.Filiais.Count > 0)
                    {
                        listaFilial = (from obj in operadorLogistica.Filiais where obj.Filial.Ativo select obj.Filial).ToList();
                    }
                }

                if (listaFilial == null)
                    listaFilial = repFilial.ConsultarTodas();

                var retorno = (from obj in listaFilial select new { value = obj.Codigo, text = obj.Descricao }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", "Filial." + grid.extensaoCSV);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaFilialBalanca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialBalanca filtrosPesquisa = ObterFiltrosPesquisaFilialBalanca();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Marca", "MarcaBalanca", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo", "Descricao", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("IP Consulta", "HostConsultaBalanca", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Porta", "PortaBalanca", 20, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Filiais.FilialBalanca repFilialBalanca = new Repositorio.Embarcador.Filiais.FilialBalanca(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Filiais.FilialBalanca> balancas = repFilialBalanca.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repFilialBalanca.ContarConsulta(filtrosPesquisa));

                var lista = (from p in balancas
                             select new
                             {
                                 p.Codigo,
                                 p.MarcaBalanca,
                                 Descricao = p.ModeloBalanca,
                                 p.HostConsultaBalanca,
                                 p.PortaBalanca
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaFilialArmazem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialArmazem filtrosPesquisa = ObterFiltrosPesquisaFilialArmazem();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Filiais.Filial.DescricaoFilial, "Filial", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.CodigoIntegracao, "CodigoIntegracao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CodigoFilialEmbarcador", false);

                Repositorio.Embarcador.Filiais.FilialArmazem repFilialArmazen = new Repositorio.Embarcador.Filiais.FilialArmazem(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Filiais.FilialArmazem> armazens = repFilialArmazen.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repFilialArmazen.ContarConsulta(filtrosPesquisa));

                var lista = (from p in armazens
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 Filial = p.Filial.Descricao,
                                 p.CodigoIntegracao,
                                 p.Filial.CodigoFilialEmbarcador
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> VerificarCNPJCadastrado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                string cnpj = Utilidades.String.OnlyNumbers(Request.Params("CNPJ"));

                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCNPJ(cnpj);

                dynamic dynFilial = null;

                if (filial != null)
                {
                    dynFilial = new
                    {
                        filial.Codigo,
                        filial.Descricao
                    };
                }

                return new JsonpResult(dynFilial, true, "");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
            Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);

            filial.Descricao = Request.GetStringParam("Descricao");
            filial.CodigoFilialEmbarcador = Request.GetStringParam("CodigoFilialEmbarcador");
            filial.CNPJ = Utilidades.String.OnlyNumbers(Request.GetStringParam("CNPJ"));
            filial.NumeroUnidadeImpressao = Request.GetIntParam("NumeroUnidadeImpressao");
            filial.Email = Request.GetStringParam("Email");
            filial.SiglaFilial = Request.GetStringParam("SiglaFilial");
            filial.AccountNameVtex = Request.GetStringParam("AccountNameVtex");
            filial.Ativo = Request.GetBoolParam("Ativo");
            filial.TipoFilial = Request.GetEnumParam<TipoFilial>("TipoFilial");

            filial.Localidade = repLocalidade.BuscarPorCodigo(Request.GetIntParam("Localidade"));
            filial.Atividade = repAtividade.BuscarPorCodigo(Request.GetIntParam("Atividade"));
            filial.EmpresaEmissora = repEmpresa.BuscarPorCodigo(Request.GetIntParam("EmpresaEmissora"));
            filial.TipoDeCarga = repTipoDeCarga.BuscarPorCodigo(Request.GetIntParam("TipoCarga"));
            filial.TipoOperacaoRedespacho = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacaoRedespacho"));
            filial.SetorAtendimento = repSetor.BuscarPorCodigo(Request.GetIntParam("SetorAtendimento"));
            filial.ControlaExpedicao = Request.GetBoolParam("ControlaExpedicao");
            filial.NaoAdicionarValorDescarga = Request.GetBoolParam("NaoAdicionarValorDescarga");
            filial.EmiteMDFeFilialEmissora = Request.GetBoolParam("EmiteMDFeFilialEmissora");
            filial.EmiteMDFeFilialEmissoraPorEstadoDestino = Request.GetBoolParam("EmiteMDFeFilialEmissoraPorEstadoDestino");
            filial.EmitirMDFeManualmente = Request.GetBoolParam("EmitirMDFeManualmente");
            filial.UtilizarCtesAnterioresComoCteFilialEmissora = Request.GetBoolParam("UtilizarCtesAnterioresComoCteFilialEmissora");
            filial.ExigirPreCargaMontagemCarga = Request.GetBoolParam("ExigirPreCargaMontagemCarga");
            filial.ExigirConfirmacaoTransporte = Request.GetBoolParam("ExigirConfirmacaoTransporte");
            filial.GerarCIOTParaTodasAsCargas = Request.GetBoolParam("GerarCIOTParaTodasAsCargas");
            filial.NaoPermitirAgruparCargaMesmaFilial = Request.GetBoolParam("NaoPermitirAgruparCargaMesmaFilial");
            filial.ExigeConfirmacaoFreteAntesEmissao = Request.GetBoolParam("ExigeConfirmacaoFreteAntesEmissao");
            filial.AtivarCondicao = Request.GetBoolParam("AtivarCondicao");
            filial.LiberarAutomaticamentePagamento = Request.GetBoolParam("LiberarAutomaticamentePagamento");
            filial.ValorMedioMercadoria = Request.GetDecimalParam("ValorMedioMercadoria");
            filial.NaoValidarVeiculoIntegracao = Request.GetBoolParam("NaoValidarVeiculoIntegracao");
            filial.GerarIntegracaoP44 = Request.GetBoolParam("GerarIntegracaoP44");
            filial.DiasDeCortePlanejamentoDiario = Request.GetIntParam("DiasDeCortePlanejamentoDiario");
            filial.IntegradoERP = false;
            filial.HorarioCorteParaCalculoLeadTime = Request.GetBoolParam("HorarioCorteParaCalculoLeadTime");
            filial.HabilitarPreViagemTrizy = Request.GetBoolParam("HabilitarPreViagemTrizy");
            filial.InformarEquipamentoFluxoPatio = Request.GetBoolParam("InformarEquipamentoFluxoPatio");
            filial.HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos = Request.GetNullableTimeParam("HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos");
            filial.GerarIntegracaoKlios = Request.GetBoolParam("GerarIntegracaoKlios");
            filial.NotificarContainerCargaCancelada = Request.GetBoolParam("NotificarContainerCargaCancelada");

            filial.HoraInicialPlanejamentoDiario = Request.GetTimeParam("HoraInicialPlanejamentoDiario").ToDateTime();
            if (filial.HoraInicialPlanejamentoDiario == DateTime.MinValue)
                filial.HoraInicialPlanejamentoDiario = null;

            filial.HoraFinalPlanejamentoDiario = Request.GetTimeParam("HoraFinalPlanejamentoDiario").ToDateTime();
            if (filial.HoraFinalPlanejamentoDiario == DateTime.MinValue)
                filial.HoraFinalPlanejamentoDiario = null;

            bool aletarDiariaContainer = Request.GetBoolParam("AlertarDiariaContainer");

            if (aletarDiariaContainer)
                filial.EmailDiariaContainer = Request.GetStringParam("EmailDiariaContainer");
            else
                filial.EmailDiariaContainer = "";

            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(filial.CNPJ));
            if (cliente != null)
            {
                cliente.Initialize();
                cliente.Atividade = filial.Atividade;
                repCliente.Atualizar(cliente, Auditado);
            }
            filial.ConfiguracaoCIOT = repConfiguracaoCIOT.BuscarPorCodigo(Request.GetIntParam("ConfiguracaoCIOT"));
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilial filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.CodigoIntegracao, "CodigoFilialEmbarcador", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Filial.Localidade, "Localidade", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Filial.TipoFilial, "RetornaTipoFilial", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("TipoChecklistImpressao", false);
                grid.AdicionarCabecalho("CheckListAtivado", false);
                grid.AdicionarCabecalho("AvaliacaoDescargaAtivado", false);
                grid.AdicionarCabecalho("GerarIntegracaoP44", false);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(unitOfWork);

                List<Dominio.Entidades.Embarcador.Filiais.Filial> listaFilial = repositorioFilial.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                List<int> codigosFiliais = listaFilial.Select(o => o.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> listaSequenciaGestaoPatio = repositorioSequenciaGestaoPatio.BuscarTodosPorFiliais(codigosFiliais);
                grid.setarQuantidadeTotal(repositorioFilial.ContarConsulta(filtrosPesquisa));

                var lista = (
                    from filial in listaFilial
                    select RetornarFilial(filial, listaSequenciaGestaoPatio)
                ).ToList();

                bool existeSiglaFiliais = lista.Any(f => f.SiglaFilial != string.Empty);

                if (existeSiglaFiliais)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Filial.SiglaFilial, "SiglaFilial", 15, Models.Grid.Align.left, false);

                grid.AdicionaRows(lista);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void AdicionarOuAtualizarCondicoesPagamento(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic condicoesPagamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CondicoesPagamento"));

            ExcluirCondicoesPagamentoRemovidas(filial, condicoesPagamento, unitOfWork);
            InserirCondicoesPagamentoAdicionadas(filial, condicoesPagamento, unitOfWork);
        }

        private void ExcluirCondicoesPagamentoRemovidas(Dominio.Entidades.Embarcador.Filiais.Filial filial, dynamic condicoesPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            if (filial.CondicoesPagamento?.Count > 0)
            {
                Repositorio.Embarcador.Filiais.CondicaoPagamentoFilial repositorioCondicaoPagamentoFilial = new Repositorio.Embarcador.Filiais.CondicaoPagamentoFilial(unitOfWork);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var condicaoPagamento in condicoesPagamento)
                {
                    int? codigo = ((string)condicaoPagamento.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Filiais.CondicaoPagamentoFilial> condicoesPagamentoRemover = (from condicao in filial.CondicoesPagamento where !listaCodigosAtualizados.Contains(condicao.Codigo) select condicao).ToList();

                foreach (var condicaoPagamento in condicoesPagamentoRemover)
                {
                    repositorioCondicaoPagamentoFilial.Deletar(condicaoPagamento);
                }

                if (condicoesPagamentoRemover.Count > 0)
                {
                    string descricaoAcao = condicoesPagamentoRemover.Count == 1 ? Localization.Resources.Filiais.Filial.CondicaoPagamentoRemovida : Localization.Resources.Filiais.Filial.MultiplasCondicoesPagamentoRemovidas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void InserirCondicoesPagamentoAdicionadas(Dominio.Entidades.Embarcador.Filiais.Filial filial, dynamic condicoesPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.CondicaoPagamentoFilial repositorioCondicaoPagamentoFilial = new Repositorio.Embarcador.Filiais.CondicaoPagamentoFilial(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            int totalCondicoesPagamentoAdicionadasOuAtualizadas = 0;

            foreach (var condicaoPagamento in condicoesPagamento)
            {
                int? codigo = ((string)condicaoPagamento.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Filiais.CondicaoPagamentoFilial condicaoPagamentoFilial;

                if (codigo.HasValue)
                    condicaoPagamentoFilial = repositorioCondicaoPagamentoFilial.BuscarPorCodigo(codigo.Value, auditavel: false) ?? throw new ControllerException(Localization.Resources.Filiais.Filial.CondicaoPagamentoNaoEncontrada);
                else
                    condicaoPagamentoFilial = new Dominio.Entidades.Embarcador.Filiais.CondicaoPagamentoFilial();

                condicaoPagamentoFilial.Filial = filial;
                condicaoPagamentoFilial.DiaEmissaoLimite = ((string)condicaoPagamento.DiaEmissaoLimite).ToNullableInt();
                condicaoPagamentoFilial.DiaMes = ((string)condicaoPagamento.DiaMes).ToNullableInt();
                condicaoPagamentoFilial.DiaSemana = ((string)condicaoPagamento.DiaSemana).ToNullableEnum<DiaSemana>();
                condicaoPagamentoFilial.DiasDePrazoPagamento = ((string)condicaoPagamento.DiasDePrazoPagamento).ToNullableInt();
                condicaoPagamentoFilial.TipoPrazoPagamento = ((string)condicaoPagamento.TipoPrazoPagamento).ToNullableEnum<TipoPrazoPagamento>();
                condicaoPagamentoFilial.VencimentoForaMes = (bool)condicaoPagamento.VencimentoForaMes;
                condicaoPagamentoFilial.ConsiderarDiaUtilVencimento = (bool)condicaoPagamento.ConsiderarDiaUtilVencimento;

                int codigoTipoCarga = ((string)condicaoPagamento.CodigoTipoCarga).ToInt();
                condicaoPagamentoFilial.TipoDeCarga = codigoTipoCarga > 0 ? repositorioTipoDeCarga.BuscarPorCodigo(codigoTipoCarga) ?? throw new ControllerException(Localization.Resources.Filiais.Filial.TipoCargaNaoEncontrado) : null;

                int codigoTipoOperacao = ((string)condicaoPagamento.CodigoTipoOperacao).ToInt();
                condicaoPagamentoFilial.TipoOperacao = codigoTipoOperacao > 0 ? repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) ?? throw new ControllerException(Localization.Resources.Filiais.Filial.TipoOperacaoNaoEncontrado) : null;

                if (codigo.HasValue)
                    repositorioCondicaoPagamentoFilial.Atualizar(condicaoPagamentoFilial);
                else
                    repositorioCondicaoPagamentoFilial.Inserir(condicaoPagamentoFilial);

                totalCondicoesPagamentoAdicionadasOuAtualizadas++;
            }

            if (filial.IsInitialized() && (totalCondicoesPagamentoAdicionadasOuAtualizadas > 0))
            {
                string descricaoAcao = totalCondicoesPagamentoAdicionadasOuAtualizadas == 1 ? Localization.Resources.Filiais.Filial.CondicaoPagamentoAdicionadaAtualizada : Localization.Resources.Filiais.Filial.MultiplasCondicoesPagamentoAdicionadasAtualizadas;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private void AdicionarOuValePedagioTranportadores(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic valePedagioTranportadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ValePedagioTransportadores"));

            ExcluirValePedagioTranportadoresRemovidos(filial, valePedagioTranportadores, unitOfWork);
            InserirValePedagioTranportadoresAdicionados(filial, valePedagioTranportadores, unitOfWork);
        }

        private void ExcluirValePedagioTranportadoresRemovidos(Dominio.Entidades.Embarcador.Filiais.Filial filial, dynamic valePedagioTranportadores, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial repositorioValePedagioTransportador = new Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial(unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial> listaValePedagioTransportador = repositorioValePedagioTransportador.BuscarPorFilial(filial.Codigo);

            if (listaValePedagioTransportador.Count == 0)
                return;

            List<int> listaCodigosAtualizados = new List<int>();

            foreach (var valePedagioTranportador in valePedagioTranportadores)
            {
                int? codigo = ((string)valePedagioTranportador.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    listaCodigosAtualizados.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial> listaValePedagioTransportadorRemover = (from o in listaValePedagioTransportador where !listaCodigosAtualizados.Contains(o.Codigo) select o).ToList();

            foreach (Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial valePedagioTransportador in listaValePedagioTransportadorRemover)
                repositorioValePedagioTransportador.Deletar(valePedagioTransportador);
        }

        private void InserirValePedagioTranportadoresAdicionados(Dominio.Entidades.Embarcador.Filiais.Filial filial, dynamic valePedagioTranportadores, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial repositorioValePedagioTransportador = new Repositorio.Embarcador.Filiais.ValePedagioTransportadorFilial(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            foreach (var valePedagioTranportador in valePedagioTranportadores)
            {
                int? codigo = ((string)valePedagioTranportador.Codigo).ToNullableInt();
                int codigoTransportador = ((string)valePedagioTranportador.CodigoTransportador).ToInt();
                TipoIntegracao tipoIntegracaoValePedagio = ((string)valePedagioTranportador.TipoIntegracaoValePedagio).ToEnum<TipoIntegracao>();
                Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial valePedagioTransportadorSalvar;

                if (codigo.HasValue)
                    valePedagioTransportadorSalvar = repositorioValePedagioTransportador.BuscarPorCodigo(codigo.Value, auditavel: false) ?? throw new ControllerException(Localization.Resources.Filiais.Filial.TransportadorValePedagioNaoEncontrado);
                else
                    valePedagioTransportadorSalvar = new Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial();

                valePedagioTransportadorSalvar.Filial = filial;
                valePedagioTransportadorSalvar.ComprarValePedagio = ((string)valePedagioTranportador.ComprarValePedagio).ToBool();
                valePedagioTransportadorSalvar.Transportador = (codigoTransportador > 0) ? repositorioEmpresa.BuscarPorCodigo(codigoTransportador) ?? throw new ControllerException(Localization.Resources.Filiais.Filial.TransportadorNaoEncontrado) : null;
                valePedagioTransportadorSalvar.TipoIntegracaoValePedagio = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracaoValePedagio) ?? throw new ControllerException(Localization.Resources.Filiais.Filial.OperadoraValePedagioNaoEncontrada);

                if (codigo.HasValue)
                    repositorioValePedagioTransportador.Atualizar(valePedagioTransportadorSalvar);
                else
                    repositorioValePedagioTransportador.Inserir(valePedagioTransportadorSalvar);
            }
        }

        private void AdicionarOuAtualizarArmazem(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic armamens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Armazem"));

            ExcluirArmazemRemovidos(filial, armamens, unitOfWork);
            InserirArmazemAdicionados(filial, armamens, unitOfWork);
        }

        private void ExcluirArmazemRemovidos(Dominio.Entidades.Embarcador.Filiais.Filial filial, dynamic armazens, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.FilialArmazem repositorioFilialArmazem = new Repositorio.Embarcador.Filiais.FilialArmazem(unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.FilialArmazem> listaFilialArmazem = repositorioFilialArmazem.BuscarPorFilial(filial.Codigo);

            if (listaFilialArmazem.Count == 0)
                return;

            List<int> listaCodigosAtualizados = new List<int>();
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> valoresAlterados = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            foreach (var armazen in armazens)
            {
                int? codigo = ((string)armazen.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    listaCodigosAtualizados.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Filiais.FilialArmazem> listaFilialArmazemRemover = (from o in listaFilialArmazem where !listaCodigosAtualizados.Contains(o.Codigo) select o).ToList();

            foreach (Dominio.Entidades.Embarcador.Filiais.FilialArmazem filialArmazen in listaFilialArmazemRemover)
            {
                valoresAlterados.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    De = filialArmazen.Descricao,
                    Para = "",
                    Propriedade = $"Armazém {filialArmazen.Codigo}"
                });
                repositorioFilialArmazem.Deletar(filialArmazen);
            }

            filial.SetExternalChanges(valoresAlterados);
        }

        private void InserirArmazemAdicionados(Dominio.Entidades.Embarcador.Filiais.Filial filial, dynamic filialArmazens, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.FilialArmazem repositorioValePedagioTransportador = new Repositorio.Embarcador.Filiais.FilialArmazem(unitOfWork);

            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> valoresAlterados = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            foreach (dynamic armazem in filialArmazens)
            {
                int? codigo = ((string)armazem.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Filiais.FilialArmazem filialArmazem;

                if (codigo.HasValue)
                    filialArmazem = repositorioValePedagioTransportador.BuscarPorCodigo(codigo.Value, auditavel: false) ?? throw new ControllerException(Localization.Resources.Filiais.Filial.ArmazemNaoEncontrado);
                else
                    filialArmazem = new Dominio.Entidades.Embarcador.Filiais.FilialArmazem();

                filialArmazem.Initialize();

                filialArmazem.Filial = filial;
                filialArmazem.Descricao = ((string)armazem.Descricao);
                filialArmazem.CodigoIntegracao = ((string)armazem.CodigoIntegracao);
                filialArmazem.Situacao = ((string)armazem.Situacao).ToBool();

                if (codigo.HasValue)
                    repositorioValePedagioTransportador.Atualizar(filialArmazem);
                else
                    repositorioValePedagioTransportador.Inserir(filialArmazem);

                foreach (Dominio.Entidades.Auditoria.HistoricoPropriedade alteracao in filialArmazem.GetChanges())
                    valoresAlterados.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        De = alteracao.De,
                        Para = alteracao.Para,
                        Propriedade = $"Armazém {filialArmazem.Codigo} - {alteracao.Propriedade}"
                    });
            }

            filial.SetExternalChanges(valoresAlterados);
            filial.SetChanges();
        }

        private Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilial ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilial filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilial()
            {
                Ativo = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Descricao = Request.GetStringParam("Descricao"),
                DescricaoOuCodigoIntegracao = Request.GetStringParam("DescricaoOuCodigoIntegracao"),
                SomenteFiliaisComSolicitacaoDeGas = Request.GetBoolParam("SomenteFiliaisComSolicitacaoDeGas"),
                SomenteLiberadasParaFilaCarregamento = Request.GetBoolParam("SomenteLiberadasParaFilaCarregamento"),
            };

            if (Request.GetBoolParam("FiltrarPorConfiguracaoOperadorLogistica", valorPadrao: true))
                filtrosPesquisa.ListaCodigoFilialPermitidas = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);

            if (Request.GetBoolParam("FiltrarPorConfiguracaoOperadorLogisticaFilialVenda"))
                filtrosPesquisa.ListaCodigoFiliaisVendasPermitidas = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);

            return filtrosPesquisa;
        }

        private List<Dominio.Entidades.Embarcador.Filiais.SetorFilial> ObterSetores(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            var listaSetoresFilial = new List<Dominio.Entidades.Embarcador.Filiais.SetorFilial>();
            var setores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("SetoresFilial"));
            var repositorioSetor = new Repositorio.Setor(unidadeDeTrabalho);
            var repositorioTurno = new Repositorio.Embarcador.Filiais.Turno(unidadeDeTrabalho);

            foreach (var setorFilial in setores)
            {
                var setor = repositorioSetor.BuscarPorCodigo(((string)setorFilial.Setor.Codigo).ToInt());
                var turnos = new List<Dominio.Entidades.Embarcador.Filiais.Turno>();

                if (setor == null)
                    throw new Exception(Localization.Resources.Filiais.Filial.SetorNaoEncontrado);

                foreach (var setorFilialTurno in setorFilial.Turnos)
                {
                    var turno = repositorioTurno.BuscarPorCodigo(((string)setorFilialTurno.Codigo).ToInt());

                    if (turno == null)
                        throw new Exception(Localization.Resources.Filiais.Filial.TurnoNaoEncontrado);

                    turnos.Add(turno);
                }

                listaSetoresFilial.Add(
                    new Dominio.Entidades.Embarcador.Filiais.SetorFilial()
                    {
                        Codigo = ((string)setorFilial.Codigo).ToInt(),
                        Setor = setor,
                        Turnos = turnos.Count > 0 ? turnos : null
                    }
                );
            }

            return listaSetoresFilial;
        }

        private dynamic RetornarFilial(Dominio.Entidades.Embarcador.Filiais.Filial filial, List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> listaSequenciaGestaoPatio)
        {
            List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> listaSequenciaGestaoPatioPorFilial = listaSequenciaGestaoPatio.Where(sequencia => sequencia.Filial.Codigo == filial.Codigo && sequencia.TipoOperacao == null).ToList();

            return new
            {
                filial.Codigo,
                filial.Descricao,
                filial.CodigoFilialEmbarcador,
                filial.RetornaTipoFilial,
                filial.DescricaoAtivo,
                SiglaFilial = filial?.SiglaFilial ?? string.Empty,
                CheckListAtivado = listaSequenciaGestaoPatioPorFilial.Any(sequencia => sequencia.CheckList),
                AvaliacaoDescargaAtivado = listaSequenciaGestaoPatioPorFilial.Any(sequencia => sequencia.Tipo == TipoFluxoGestaoPatio.Destino && sequencia.AvaliacaoDescarga),
                Localidade = filial.Localidade != null ? filial.Localidade.DescricaoCidadeEstado : "",
                TipoChecklistImpressao = listaSequenciaGestaoPatioPorFilial.Where(sequencia => sequencia.TipoChecklistImpressao != null).Select(sequencia => sequencia.TipoChecklistImpressao.Codigo).FirstOrDefault(),
                GerarIntegracaoP44 = filial?.GerarIntegracaoP44 ?? false,
            };
        }

        private dynamic RetornarDynAlertasSla(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.GestaoPatioAlertaSla repositorioAlertaSla = new Repositorio.Embarcador.Filiais.GestaoPatioAlertaSla(unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> alertasSla = repositorioAlertaSla.BuscarPorFilial(filial.Codigo);

            return (from o in alertasSla
                    select new
                    {
                        o.Codigo,
                        o.CorAlertaTempoExcedido,
                        o.CorAlertaTempoFaltante,
                        o.Emails,
                        o.NomeAlerta,
                        o.TempoExcedido,
                        o.TempoExcedidoTransportador,
                        o.TempoFaltante,
                        o.TempoFaltanteTransportador,
                        o.AlertarTransportadorPorEmail,
                        Etapas = o.Etapas.Select(obj => obj).ToList(),
                        AlertasEnviarEmail = o.TiposAlertaEmail.Select(obj => obj).ToList()
                    }).ToList();
        }

        private dynamic RetornaDynOrdemGestaoPatio(Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio)
        {
            if (sequenciaGestaoPatio == null)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> ordenacaoEtapas = new List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao>();

            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.CheckList,
                Ordem = sequenciaGestaoPatio.OrdemCheckList
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.Expedicao,
                Ordem = sequenciaGestaoPatio.OrdemExpedicao
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.Faturamento,
                Ordem = sequenciaGestaoPatio.OrdemFaturamento
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.ChegadaVeiculo,
                Ordem = sequenciaGestaoPatio.OrdemChegadaVeiculo
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.Guarita,
                Ordem = sequenciaGestaoPatio.OrdemGuaritaEntrada
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.InformarDoca,
                Ordem = sequenciaGestaoPatio.OrdemInformarDocaCarregamento
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.InicioViagem,
                Ordem = sequenciaGestaoPatio.OrdemGuaritaSaida
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.LiberacaoChave,
                Ordem = sequenciaGestaoPatio.OrdemLiberaChave
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.TravamentoChave,
                Ordem = sequenciaGestaoPatio.OrdemTravaChave
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.Posicao,
                Ordem = sequenciaGestaoPatio.OrdemPosicao
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.ChegadaLoja,
                Ordem = sequenciaGestaoPatio.OrdemChegadaLoja
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.DeslocamentoPatio,
                Ordem = sequenciaGestaoPatio.OrdemDeslocamentoPatio
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.SaidaLoja,
                Ordem = sequenciaGestaoPatio.OrdemSaidaLoja
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.FimViagem,
                Ordem = sequenciaGestaoPatio.OrdemFimViagem
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.InicioHigienizacao,
                Ordem = sequenciaGestaoPatio.OrdemInicioHigienizacao
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.FimHigienizacao,
                Ordem = sequenciaGestaoPatio.OrdemFimHigienizacao
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.InicioCarregamento,
                Ordem = sequenciaGestaoPatio.OrdemInicioCarregamento
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.FimCarregamento,
                Ordem = sequenciaGestaoPatio.OrdemFimCarregamento
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.InicioDescarregamento,
                Ordem = sequenciaGestaoPatio.OrdemInicioDescarregamento
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.FimDescarregamento,
                Ordem = sequenciaGestaoPatio.OrdemFimDescarregamento
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.SeparacaoMercadoria,
                Ordem = sequenciaGestaoPatio.OrdemSeparacaoMercadoria
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.SolicitacaoVeiculo,
                Ordem = sequenciaGestaoPatio.OrdemSolicitacaoVeiculo
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.DocumentoFiscal,
                Ordem = sequenciaGestaoPatio.OrdemDocumentoFiscal
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.DocumentosTransporte,
                Ordem = sequenciaGestaoPatio.OrdemDocumentosTransporte
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.MontagemCarga,
                Ordem = sequenciaGestaoPatio.OrdemMontagemCarga
            });

            return (from o in ordenacaoEtapas orderby o.Ordem ascending select o).ToList();
        }

        private dynamic RetornaDynOrdemGestaoPatioDestino(Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio)
        {
            if (sequenciaGestaoPatio == null)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> ordenacaoEtapas = new List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao>();

            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.AvaliacaoDescarga,
                Ordem = sequenciaGestaoPatio.OrdemAvaliacaoDescarga
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.CheckList,
                Ordem = sequenciaGestaoPatio.OrdemCheckList
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.Faturamento,
                Ordem = sequenciaGestaoPatio.OrdemFaturamento
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.ChegadaVeiculo,
                Ordem = sequenciaGestaoPatio.OrdemChegadaVeiculo
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.Guarita,
                Ordem = sequenciaGestaoPatio.OrdemGuaritaEntrada
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.InformarDoca,
                Ordem = sequenciaGestaoPatio.OrdemInformarDocaCarregamento
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.InicioViagem,
                Ordem = sequenciaGestaoPatio.OrdemGuaritaSaida
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.LiberacaoChave,
                Ordem = sequenciaGestaoPatio.OrdemLiberaChave
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.TravamentoChave,
                Ordem = sequenciaGestaoPatio.OrdemTravaChave
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.ChegadaLoja,
                Ordem = sequenciaGestaoPatio.OrdemChegadaLoja
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.DeslocamentoPatio,
                Ordem = sequenciaGestaoPatio.OrdemDeslocamentoPatio
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.SaidaLoja,
                Ordem = sequenciaGestaoPatio.OrdemSaidaLoja
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.FimViagem,
                Ordem = sequenciaGestaoPatio.OrdemFimViagem
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.InicioHigienizacao,
                Ordem = sequenciaGestaoPatio.OrdemInicioHigienizacao
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.FimHigienizacao,
                Ordem = sequenciaGestaoPatio.OrdemFimHigienizacao
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.SolicitacaoVeiculo,
                Ordem = sequenciaGestaoPatio.OrdemSolicitacaoVeiculo
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.InicioDescarregamento,
                Ordem = sequenciaGestaoPatio.OrdemInicioDescarregamento
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.FimDescarregamento,
                Ordem = sequenciaGestaoPatio.OrdemFimDescarregamento
            });
            ordenacaoEtapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
            {
                Etapa = EtapaFluxoGestaoPatio.AvaliacaoDescarga,
                Ordem = sequenciaGestaoPatio.OrdemAvaliacaoDescarga
            });

            return (from o in ordenacaoEtapas orderby o.Ordem ascending select o).ToList();
        }

        private dynamic RetornaDynSequenciaGestaoPatio(Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            if (sequenciaGestaoPatio == null)
                return null;

            return new
            {
                sequenciaGestaoPatio.MontagemCarga,
                sequenciaGestaoPatio.MontagemCargaHabilitarIntegracao,
                sequenciaGestaoPatio.MontagemCargaCodigoIntegracao,
                sequenciaGestaoPatio.MontagemCargaTempo,
                sequenciaGestaoPatio.MontagemCargaTempoPermanencia,
                sequenciaGestaoPatio.MontagemCargaDescricao,
                sequenciaGestaoPatio.MontagemCargaInformarDoca,
                sequenciaGestaoPatio.MontagemCargaPermiteInformarQuantidadeCaixas,
                sequenciaGestaoPatio.MontagemCargaPermiteInformarQuantidadeItens,
                sequenciaGestaoPatio.MontagemCargaPermiteInformarQuantidadePallets,
                sequenciaGestaoPatio.MontagemCargaGerarIntegracaoP44,

                sequenciaGestaoPatio.InformarDocaCarregamento,
                sequenciaGestaoPatio.InformarDocaCarregamentoHabilitarIntegracao,
                sequenciaGestaoPatio.InformarDocaCarregamentoCodigoIntegracao,
                sequenciaGestaoPatio.InformarDocaCarregamentoTempo,
                sequenciaGestaoPatio.InformarDocaCarregamentoTempoPermanencia,
                sequenciaGestaoPatio.InformarDocaCarregamentoDescricao,
                sequenciaGestaoPatio.InformarDocaCarregamentoPermiteTransportadorLancarHorarios,
                sequenciaGestaoPatio.InformarDocaCarregamentoPermiteInformarDadosLaudo,
                sequenciaGestaoPatio.InformarDocaCarregamentoGerarIntegracaoP44,
                sequenciaGestaoPatio.InformarDocaCarregamentoTipoIntegracao,

                sequenciaGestaoPatio.ChegadaVeiculo,
                sequenciaGestaoPatio.ChegadaVeiculoHabilitarIntegracao,
                sequenciaGestaoPatio.ChegadaVeiculoPermiteImprimirRelacaoDeProdutos,
                sequenciaGestaoPatio.ChegadaVeiculoPreencherDataSaida,
                sequenciaGestaoPatio.ChegadaVeiculoCodigoIntegracao,
                sequenciaGestaoPatio.ChegadaVeiculoTempo,
                sequenciaGestaoPatio.ChegadaVeiculoTempoPermanencia,
                sequenciaGestaoPatio.ChegadaVeiculoDescricao,
                sequenciaGestaoPatio.ChegadaVeiculoPermiteTransportadorLancarHorarios,
                sequenciaGestaoPatio.ChegadaVeiculoImprimirComprovanteModeloColetaOutbound,
                sequenciaGestaoPatio.ChegadaVeiculoGerarIntegracaoP44,

                sequenciaGestaoPatio.GuaritaEntrada,
                sequenciaGestaoPatio.GuaritaEntradaHabilitarIntegracao,
                sequenciaGestaoPatio.GuaritaEntradaCodigoIntegracao,
                sequenciaGestaoPatio.GuaritaEntradaTempo,
                sequenciaGestaoPatio.GuaritaEntradaTempoPermanencia,
                sequenciaGestaoPatio.GuaritaEntradaDescricao,
                sequenciaGestaoPatio.GuaritaEntradaInformarDoca,
                sequenciaGestaoPatio.GuaritaEntradaExibirHorarioExato,
                sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesPesagem,
                sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesProdutor,
                sequenciaGestaoPatio.GuaritaEntradaPermiteInformarAnexoPesagem,
                sequenciaGestaoPatio.GuaritaEntradaPermiteInformarPressaoPesagem,
                sequenciaGestaoPatio.GuaritaEntradaPermiteInformarQuantidadeCaixasPesagem,
                sequenciaGestaoPatio.GuaritaEntradaPermiteDenegarChegada,
                sequenciaGestaoPatio.GuaritaEntradaPermiteTransportadorLancarHorarios,
                sequenciaGestaoPatio.GuaritaEntradaPermiteInformarDadosDevolucao,
                sequenciaGestaoPatio.GuaritaEntradaGerarIntegracaoP44,
                sequenciaGestaoPatio.ImprimirTicketBalanca,
                BalancaGuaritaEntrada = new { Codigo = sequenciaGestaoPatio.BalancaGuaritaEntrada?.Codigo ?? 0, Descricao = sequenciaGestaoPatio.BalancaGuaritaEntrada?.ModeloBalanca ?? string.Empty },
                sequenciaGestaoPatio.GuaritaEntradaTipoIntegracaoBalanca,

                sequenciaGestaoPatio.CheckList,
                sequenciaGestaoPatio.CheckListHabilitarIntegracao,
                sequenciaGestaoPatio.CheckListCodigoIntegracao,
                sequenciaGestaoPatio.CheckListPermiteImpressaoApenasComCheckListFinalizada,
                sequenciaGestaoPatio.CheckListPermitePreencherCheckListAntesDeChegarNaEtapa,
                sequenciaGestaoPatio.CheckListInformarDoca,
                sequenciaGestaoPatio.CheckListTempo,
                sequenciaGestaoPatio.CheckListTempoPermanencia,
                sequenciaGestaoPatio.CheckListDescricao,
                sequenciaGestaoPatio.CheckListPermiteSalvarSemPreencher,
                sequenciaGestaoPatio.CheckListCancelarPatioAoReprovar,
                sequenciaGestaoPatio.CheckListNaoExigeObservacaoAoReprovar,
                sequenciaGestaoPatio.CheckListNotificarPorEmailReprovacao,
                sequenciaGestaoPatio.CheckListEmails,
                sequenciaGestaoPatio.CheckListPermiteTransportadorLancarHorarios,
                sequenciaGestaoPatio.CheckListAssinaturaMotorista,
                sequenciaGestaoPatio.CheckListAssinaturaCarregador,
                sequenciaGestaoPatio.CheckListAssinaturaResponsavelAprovacao,
                sequenciaGestaoPatio.CheckListGerarIntegracaoP44,
                sequenciaGestaoPatio.CheckListGerarNovoPedidoAoTerminoFluxo,
                sequenciaGestaoPatio.CheckListExigirAnexo,
                sequenciaGestaoPatio.CheckListUtilizarVigencia,
                CheckListTipoOperacao = new { Codigo = sequenciaGestaoPatio.CheckListTipoOperacao?.Codigo ?? 0, Descricao = sequenciaGestaoPatio.CheckListTipoOperacao?.Descricao ?? string.Empty },
                CheckListDestinatario = new { Codigo = sequenciaGestaoPatio.CheckListDestinatario?.Codigo ?? 0, Descricao = sequenciaGestaoPatio.CheckListDestinatario?.Nome ?? string.Empty },
                TipoCheckListImpressao = sequenciaGestaoPatio.TipoChecklistImpressao != null,

                sequenciaGestaoPatio.TravaChave,
                sequenciaGestaoPatio.TravaChaveHabilitarIntegracao,
                sequenciaGestaoPatio.TravaChaveCodigoIntegracao,
                sequenciaGestaoPatio.TravaChaveInformarDoca,
                sequenciaGestaoPatio.TravaChaveTempo,
                sequenciaGestaoPatio.TravaChaveTempoPermanencia,
                sequenciaGestaoPatio.TravaChaveDescricao,
                sequenciaGestaoPatio.TravaChavePermiteTransportadorLancarHorarios,
                sequenciaGestaoPatio.TravaChaveGerarIntegracaoP44,

                sequenciaGestaoPatio.Faturamento,
                sequenciaGestaoPatio.FaturamentoHabilitarIntegracao,
                sequenciaGestaoPatio.FaturamentoCodigoIntegracao,
                sequenciaGestaoPatio.FaturamentoTempo,
                sequenciaGestaoPatio.FaturamentoTempoPermanencia,
                sequenciaGestaoPatio.FaturamentoDescricao,
                sequenciaGestaoPatio.FaturamentoGerarIntegracaoP44,

                sequenciaGestaoPatio.Expedicao,
                sequenciaGestaoPatio.ExpedicaoHabilitarIntegracao,
                sequenciaGestaoPatio.ExpedicaoCodigoIntegracao,
                sequenciaGestaoPatio.ExpedicaoInformarDoca,
                sequenciaGestaoPatio.ExpedicaoInformarInicioCarregamento,
                sequenciaGestaoPatio.ExpedicaoInformarTerminoCarregamento,
                sequenciaGestaoPatio.ExpedicaoConfirmarPlaca,
                sequenciaGestaoPatio.ExpedicaoTempo,
                sequenciaGestaoPatio.ExpedicaoDescricao,
                sequenciaGestaoPatio.ExpedicaoGerarIntegracaoP44,

                sequenciaGestaoPatio.LiberaChave,
                sequenciaGestaoPatio.LiberaChaveHabilitarIntegracao,
                sequenciaGestaoPatio.LiberaChaveCodigoIntegracao,
                sequenciaGestaoPatio.LiberaChaveTempo,
                sequenciaGestaoPatio.LiberaChaveTempoPermanencia,
                sequenciaGestaoPatio.LiberaChaveDescricao,
                sequenciaGestaoPatio.LiberaChaveBloquearLiberacaoEtapaAnterior,
                sequenciaGestaoPatio.LiberaChaveInformarNumeroDePaletes,
                sequenciaGestaoPatio.LiberaChaveSolicitarAssinaturaMotorista,
                sequenciaGestaoPatio.LiberaChaveGerarIntegracaoP44,
                sequenciaGestaoPatio.LiberaChaveExigirAnexo,

                sequenciaGestaoPatio.GuaritaSaida,
                sequenciaGestaoPatio.GuaritaSaidaHabilitarIntegracao,
                sequenciaGestaoPatio.GuaritaSaidaCodigoIntegracao,
                sequenciaGestaoPatio.GuaritaSaidaTempo,
                sequenciaGestaoPatio.GuaritaSaidaDescricao,
                sequenciaGestaoPatio.GuaritaSaidaPermiteInformacoesPesagem,
                sequenciaGestaoPatio.GuaritaSaidaPermiteInformarLacrePesagem,
                sequenciaGestaoPatio.GuaritaSaidaPermiteInformarPercentualRefugoPesagem,
                sequenciaGestaoPatio.GuaritaSaidaIniciarEmissaoDocumentosTransporte,
                sequenciaGestaoPatio.GuaritaSaidaPermiteAnexosPesagem,
                sequenciaGestaoPatio.GuaritaSaidaGerarIntegracaoP44,
                sequenciaGestaoPatio.GuaritaSaidaExigirAnexo,
                BalancaGuaritaSaida = new { Codigo = sequenciaGestaoPatio.BalancaGuaritaSaida?.Codigo ?? 0, Descricao = sequenciaGestaoPatio.BalancaGuaritaSaida?.ModeloBalanca ?? string.Empty },
                sequenciaGestaoPatio.GuaritaSaidaTipoIntegracaoBalanca,

                sequenciaGestaoPatio.Posicao,
                sequenciaGestaoPatio.PosicaoHabilitarIntegracao,
                sequenciaGestaoPatio.PosicaoCodigoIntegracao,
                sequenciaGestaoPatio.PosicaoTempo,
                sequenciaGestaoPatio.PosicaoTempoPermanencia,
                sequenciaGestaoPatio.PosicaoDescricao,
                sequenciaGestaoPatio.PosicaoGerarIntegracaoP44,

                sequenciaGestaoPatio.ChegadaLoja,
                sequenciaGestaoPatio.ChegadaLojaHabilitarIntegracao,
                sequenciaGestaoPatio.ChegadaLojaCodigoIntegracao,
                sequenciaGestaoPatio.ChegadaLojaTempo,
                sequenciaGestaoPatio.ChegadaLojaTempoPermanencia,
                sequenciaGestaoPatio.ChegadaLojaDescricao,
                sequenciaGestaoPatio.ChegadaLojaGerarIntegracaoP44,

                sequenciaGestaoPatio.DeslocamentoPatio,
                sequenciaGestaoPatio.DeslocamentoPatioHabilitarIntegracao,
                sequenciaGestaoPatio.DeslocamentoPatioCodigoIntegracao,
                sequenciaGestaoPatio.DeslocamentoPatioTempo,
                sequenciaGestaoPatio.DeslocamentoPatioTempoPermanencia,
                sequenciaGestaoPatio.DeslocamentoPatioDescricao,
                sequenciaGestaoPatio.DeslocamentoPatioPermiteInformacoesPesagem,
                sequenciaGestaoPatio.DeslocamentoPatioPermiteInformacoesLoteInterno,
                sequenciaGestaoPatio.DeslocamentoPatioPermiteInformarQuantidade,
                sequenciaGestaoPatio.DeslocamentoPatioGerarIntegracaoP44,

                sequenciaGestaoPatio.SaidaLoja,
                sequenciaGestaoPatio.SaidaLojaHabilitarIntegracao,
                sequenciaGestaoPatio.SaidaLojaCodigoIntegracao,
                sequenciaGestaoPatio.SaidaLojaTempo,
                sequenciaGestaoPatio.SaidaLojaTempoPermanencia,
                sequenciaGestaoPatio.SaidaLojaDescricao,
                sequenciaGestaoPatio.SaidaLojaGerarIntegracaoP44,

                sequenciaGestaoPatio.FimViagem,
                sequenciaGestaoPatio.FimViagemHabilitarIntegracao,
                sequenciaGestaoPatio.FimViagemCodigoIntegracao,
                sequenciaGestaoPatio.FimViagemTempo,
                sequenciaGestaoPatio.FimViagemTempoPermanencia,
                sequenciaGestaoPatio.FimViagemDescricao,
                sequenciaGestaoPatio.FimViagemGerarIntegracaoP44,

                sequenciaGestaoPatio.InicioHigienizacao,
                sequenciaGestaoPatio.InicioHigienizacaoHabilitarIntegracao,
                sequenciaGestaoPatio.InicioHigienizacaoCodigoIntegracao,
                sequenciaGestaoPatio.InicioHigienizacaoTempo,
                sequenciaGestaoPatio.InicioHigienizacaoTempoPermanencia,
                sequenciaGestaoPatio.InicioHigienizacaoDescricao,
                sequenciaGestaoPatio.InicioHigienizacaoGerarIntegracaoP44,

                sequenciaGestaoPatio.FimHigienizacao,
                sequenciaGestaoPatio.FimHigienizacaoHabilitarIntegracao,
                sequenciaGestaoPatio.FimHigienizacaoCodigoIntegracao,
                sequenciaGestaoPatio.FimHigienizacaoTempo,
                sequenciaGestaoPatio.FimHigienizacaoTempoPermanencia,
                sequenciaGestaoPatio.FimHigienizacaoDescricao,
                sequenciaGestaoPatio.FimHigienizacaoGerarIntegracaoP44,

                sequenciaGestaoPatio.InicioCarregamento,
                sequenciaGestaoPatio.InicioCarregamentoHabilitarIntegracao,
                sequenciaGestaoPatio.InicioCarregamentoCodigoIntegracao,
                sequenciaGestaoPatio.InicioCarregamentoTempo,
                sequenciaGestaoPatio.InicioCarregamentoTempoPermanencia,
                sequenciaGestaoPatio.InicioCarregamentoDescricao,
                sequenciaGestaoPatio.InicioCarregamentoPermiteInformarPesagem,
                sequenciaGestaoPatio.InicioCarregamentoGerarIntegracaoP44,

                sequenciaGestaoPatio.FimCarregamento,
                sequenciaGestaoPatio.FimCarregamentoHabilitarIntegracao,
                sequenciaGestaoPatio.FimCarregamentoCodigoIntegracao,
                sequenciaGestaoPatio.FimCarregamentoTempo,
                sequenciaGestaoPatio.FimCarregamentoTempoPermanencia,
                sequenciaGestaoPatio.FimCarregamentoDescricao,
                sequenciaGestaoPatio.FimCarregamentoPermiteInformarPesagem,
                sequenciaGestaoPatio.FimCarregamentoGerarIntegracaoP44,

                sequenciaGestaoPatio.SolicitacaoVeiculo,
                sequenciaGestaoPatio.SolicitacaoVeiculoHabilitarIntegracao,
                sequenciaGestaoPatio.SolicitacaoVeiculoCodigoIntegracao,
                sequenciaGestaoPatio.SolicitacaoVeiculoTempo,
                sequenciaGestaoPatio.SolicitacaoVeiculoTempoPermanencia,
                sequenciaGestaoPatio.SolicitacaoVeiculoDescricao,
                sequenciaGestaoPatio.SolicitacaoVeiculoGerarIntegracaoP44,
                sequenciaGestaoPatio.NaoPermitirEnviarSMS,
                sequenciaGestaoPatio.SolicitacaoVeiculoHabilitarIntegracaoPager,
                sequenciaGestaoPatio.SolicitacaoVeiculoPermitirInformarDadosTransporteCarga,

                sequenciaGestaoPatio.InicioDescarregamento,
                sequenciaGestaoPatio.InicioDescarregamentoHabilitarIntegracao,
                sequenciaGestaoPatio.InicioDescarregamentoCodigoIntegracao,
                sequenciaGestaoPatio.InicioDescarregamentoTempo,
                sequenciaGestaoPatio.InicioDescarregamentoTempoPermanencia,
                sequenciaGestaoPatio.InicioDescarregamentoDescricao,
                sequenciaGestaoPatio.InicioDescarregamentoPermiteInformarPesagem,
                sequenciaGestaoPatio.InicioDescarregamentoGerarIntegracaoP44,

                sequenciaGestaoPatio.FimDescarregamento,
                sequenciaGestaoPatio.FimDescarregamentoHabilitarIntegracao,
                sequenciaGestaoPatio.FimDescarregamentoCodigoIntegracao,
                sequenciaGestaoPatio.FimDescarregamentoTempo,
                sequenciaGestaoPatio.FimDescarregamentoTempoPermanencia,
                sequenciaGestaoPatio.FimDescarregamentoDescricao,
                sequenciaGestaoPatio.FimDescarregamentoPermiteInformarPesagem,
                sequenciaGestaoPatio.FimDescarregamentoGerarIntegracaoP44,

                sequenciaGestaoPatio.DocumentoFiscal,
                sequenciaGestaoPatio.DocumentoFiscalHabilitarIntegracao,
                sequenciaGestaoPatio.DocumentoFiscalCodigoIntegracao,
                sequenciaGestaoPatio.DocumentoFiscalTempo,
                sequenciaGestaoPatio.DocumentoFiscalTempoPermanencia,
                sequenciaGestaoPatio.DocumentoFiscalDescricao,
                sequenciaGestaoPatio.DocumentoFiscalVincularNotaFiscal,
                sequenciaGestaoPatio.DocumentoFiscalGerarIntegracaoP44,

                sequenciaGestaoPatio.DocumentosTransporte,
                sequenciaGestaoPatio.DocumentosTransporteHabilitarIntegracao,
                sequenciaGestaoPatio.DocumentosTransporteCodigoIntegracao,
                sequenciaGestaoPatio.DocumentosTransporteTempo,
                sequenciaGestaoPatio.DocumentosTransporteTempoPermanencia,
                sequenciaGestaoPatio.DocumentosTransporteDescricao,
                sequenciaGestaoPatio.DocumentosTransporteGerarIntegracaoP44,

                sequenciaGestaoPatio.SeparacaoMercadoria,
                sequenciaGestaoPatio.SeparacaoMercadoriaHabilitarIntegracao,
                sequenciaGestaoPatio.SeparacaoMercadoriaCodigoIntegracao,
                sequenciaGestaoPatio.SeparacaoMercadoriaTempo,
                sequenciaGestaoPatio.SeparacaoMercadoriaTempoPermanencia,
                sequenciaGestaoPatio.SeparacaoMercadoriaDescricao,
                sequenciaGestaoPatio.SeparacaoMercadoriaPermiteInformarDadosCarregadores,
                sequenciaGestaoPatio.SeparacaoMercadoriaPermiteInformarDadosSeparadores,
                sequenciaGestaoPatio.SeparacaoMercadoriaGerarIntegracaoP44,

                sequenciaGestaoPatio.AvaliacaoDescarga,
                sequenciaGestaoPatio.AvaliacaoDescargaHabilitarIntegracao,
                sequenciaGestaoPatio.AvaliacaoDescargaCodigoIntegracao,
                sequenciaGestaoPatio.AvaliacaoDescargaPermiteImpressaoApenasComAvaliacaoDescargaFinalizada,
                sequenciaGestaoPatio.AvaliacaoDescargaPermitePreencherAvaliacaoDescargaAntesDeChegarNaEtapa,
                sequenciaGestaoPatio.AvaliacaoDescargaInformarDoca,
                sequenciaGestaoPatio.AvaliacaoDescargaTempo,
                sequenciaGestaoPatio.AvaliacaoDescargaTempoPermanencia,
                sequenciaGestaoPatio.AvaliacaoDescargaDescricao,
                sequenciaGestaoPatio.AvaliacaoDescargaPermiteSalvarSemPreencher,
                sequenciaGestaoPatio.AvaliacaoDescargaCancelarPatioAoReprovar,
                sequenciaGestaoPatio.AvaliacaoDescargaNaoExigeObservacaoAoReprovar,
                sequenciaGestaoPatio.AvaliacaoDescargaNotificarPorEmailReprovacao,
                sequenciaGestaoPatio.AvaliacaoDescargaEmails,
                sequenciaGestaoPatio.AvaliacaoDescargaPermiteTransportadorLancarHorarios,
                sequenciaGestaoPatio.AvaliacaoDescargaAssinaturaMotorista,
                sequenciaGestaoPatio.AvaliacaoDescargaAssinaturaCarregador,
                sequenciaGestaoPatio.AvaliacaoDescargaAssinaturaResponsavelAprovacao,
            };
        }

        private dynamic RetornaDynIntegracaoSemParar(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar)
        {
            if (integracaoSemParar == null)
                return null;

            return new
            {
                integracaoSemParar.Codigo,
                integracaoSemParar.Usuario,
                integracaoSemParar.Senha,
                FornecedorValePedagio = integracaoSemParar.FornecedorValePedagio != null ? new { integracaoSemParar.FornecedorValePedagio.Codigo, integracaoSemParar.FornecedorValePedagio.Descricao } : null,
                integracaoSemParar.TipoRota,
                integracaoSemParar.CNPJ,
                integracaoSemParar.DiasPrazo,
                integracaoSemParar.Observacao1,
                integracaoSemParar.Observacao2,
                integracaoSemParar.Observacao3,
                integracaoSemParar.Observacao4,
                integracaoSemParar.Observacao5,
                integracaoSemParar.Observacao6,
                integracaoSemParar.NomeRpt,
                integracaoSemParar.UtilizarModeoVeicularCarga
            };
        }

        private dynamic RetornaDynIntegracaoTarget(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget integracaoTarget)
        {
            if (integracaoTarget == null)
                return null;

            return new
            {
                integracaoTarget.Codigo,
                integracaoTarget.Usuario,
                integracaoTarget.Senha,
                integracaoTarget.Token,
                FornecedorValePedagio = integracaoTarget.FornecedorValePedagio != null ? new { integracaoTarget.FornecedorValePedagio.Codigo, integracaoTarget.FornecedorValePedagio.Descricao } : null,
                integracaoTarget.DiasPrazo,
                integracaoTarget.CodigoCentroCusto,
                integracaoTarget.CadastrarRotaPorIBGE,
                integracaoTarget.CadastrarRotaPorCoordenadas,
                integracaoTarget.NaoBuscarCartaoMotoristaTarget
            };
        }

        private dynamic RetornaDynIntegracaoBuonny(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBuonny integracaoBuonny)
        {
            if (integracaoBuonny == null)
                return null;

            return new
            {
                integracaoBuonny.Codigo,
                integracaoBuonny.CNPJCliente,
                integracaoBuonny.Token,
            };
        }

        private dynamic RetornaDynBalanca(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.FilialBalanca repositorioFilialBalanca = new Repositorio.Embarcador.Filiais.FilialBalanca(unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.FilialBalanca> balancas = repositorioFilialBalanca.BuscarPorFilial(filial.Codigo);

            return (from o in balancas
                    select new
                    {
                        o.Codigo,
                        o.MarcaBalanca,
                        o.ModeloBalanca,
                        o.HostConsultaBalanca,
                        o.PortaBalanca,
                        o.TamanhoRetornoBalanca,
                        o.PosicaoInicioPesoBalanca,
                        o.PosicaoFimPesoBalanca,
                        o.CasasDecimaisPesoBalanca,
                        o.QuantidadeConsultasPesoBalanca,
                        PercentualToleranciaPesoBalanca = o.PercentualToleranciaPesoBalanca.ToString("n2"),
                        PercentualToleranciaPesagemEntrada = o.PercentualToleranciaPesagemEntrada.ToString("n2"),
                        PercentualToleranciaPesagemSaida = o.PercentualToleranciaPesagemSaida.ToString("n2"),
                    }).ToList();
        }

        private Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio SalvarGestaoPatio(Dominio.Entidades.Embarcador.Filiais.Filial filial, TipoFluxoGestaoPatio tipo, Repositorio.UnitOfWork unitOfWork)
        {
            return SalvarGestaoPatio(filial, tipoOperacao: null, tipo, unitOfWork);
        }

        private Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio SalvarGestaoPatio(Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, TipoFluxoGestaoPatio tipo, Repositorio.UnitOfWork unitOfWork)
        {
            if ((tipo == TipoFluxoGestaoPatio.Destino) && !ConfiguracaoEmbarcador.GerarFluxoPatioDestino)
                return null;

            Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.TipoChecklistImpressao repositorioTipoChecklistImpressao = new Repositorio.Embarcador.GestaoPatio.TipoChecklistImpressao(unitOfWork);
            Repositorio.Embarcador.Filiais.FilialBalanca repositorioFilialBalanca = new Repositorio.Embarcador.Filiais.FilialBalanca(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Cliente repositorioDestinatario = new Repositorio.Cliente(unitOfWork);

            dynamic sequenciaGestaoPatio = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(tipo == TipoFluxoGestaoPatio.Origem ? Request.Params("SequenciaGestaoPatio") : Request.Params("SequenciaGestaoPatioDestino"));
            List<dynamic> ordemSequenciaGestaoPatio;

            try
            {
                ordemSequenciaGestaoPatio = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(tipo == TipoFluxoGestaoPatio.Origem ? Request.Params("OrdemGestaoPatio") : Request.Params("OrdemGestaoPatioDestino"));
            }
            catch (Exception)
            {
                ordemSequenciaGestaoPatio = new List<dynamic>();
            }

            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatioSalvar;

            if (tipoOperacao != null)
                sequenciaGestaoPatioSalvar = repositorioSequenciaGestaoPatio.BuscarPorFilialTipoETipoOperacao(filial.Codigo, tipoOperacao.Codigo, tipo);
            else
                sequenciaGestaoPatioSalvar = repositorioSequenciaGestaoPatio.BuscarPorFilialETipoSemTipoOperacao(filial.Codigo, tipo);

            if (sequenciaGestaoPatioSalvar == null)
                sequenciaGestaoPatioSalvar = new Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio()
                {
                    Filial = filial,
                    TipoOperacao = tipoOperacao,
                    Tipo = tipo
                };
            else
                sequenciaGestaoPatioSalvar.Initialize();

            sequenciaGestaoPatioSalvar.CheckList = (bool)sequenciaGestaoPatio.CheckList;
            sequenciaGestaoPatioSalvar.CheckListInformarDoca = (bool)sequenciaGestaoPatio.CheckListInformarDoca;
            sequenciaGestaoPatioSalvar.CheckListTempo = (int)sequenciaGestaoPatio.CheckListTempo;
            sequenciaGestaoPatioSalvar.CheckListTempoPermanencia = (int)sequenciaGestaoPatio.CheckListTempoPermanencia;
            sequenciaGestaoPatioSalvar.CheckListDescricao = (string)sequenciaGestaoPatio.CheckListDescricao;
            sequenciaGestaoPatioSalvar.CheckListHabilitarIntegracao = (bool)sequenciaGestaoPatio.CheckListHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.CheckListCodigoIntegracao = sequenciaGestaoPatioSalvar.CheckListHabilitarIntegracao ? (string)sequenciaGestaoPatio.CheckListCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.CheckListPermiteImpressaoApenasComCheckListFinalizada = (bool)sequenciaGestaoPatio.CheckListPermiteImpressaoApenasComCheckListFinalizada;
            sequenciaGestaoPatioSalvar.CheckListPermitePreencherCheckListAntesDeChegarNaEtapa = (bool)sequenciaGestaoPatio.CheckListPermitePreencherCheckListAntesDeChegarNaEtapa;
            sequenciaGestaoPatioSalvar.CheckListNotificarPorEmailReprovacao = (bool)sequenciaGestaoPatio.CheckListNotificarPorEmailReprovacao;
            sequenciaGestaoPatioSalvar.CheckListEmails = (string)sequenciaGestaoPatio.CheckListEmails;
            sequenciaGestaoPatioSalvar.CheckListCancelarPatioAoReprovar = (bool)sequenciaGestaoPatio.CheckListCancelarPatioAoReprovar;
            sequenciaGestaoPatioSalvar.CheckListNaoExigeObservacaoAoReprovar = (bool)sequenciaGestaoPatio.CheckListNaoExigeObservacaoAoReprovar;
            sequenciaGestaoPatioSalvar.CheckListPermiteSalvarSemPreencher = (bool)sequenciaGestaoPatio.CheckListPermiteSalvarSemPreencher;
            sequenciaGestaoPatioSalvar.CheckListPermiteTransportadorLancarHorarios = ((string)sequenciaGestaoPatio.CheckListPermiteTransportadorLancarHorarios).ToBool();
            sequenciaGestaoPatioSalvar.CheckListAssinaturaMotorista = ((string)sequenciaGestaoPatio.CheckListAssinaturaMotorista).ToBool();
            sequenciaGestaoPatioSalvar.CheckListAssinaturaCarregador = ((string)sequenciaGestaoPatio.CheckListAssinaturaCarregador).ToBool();
            sequenciaGestaoPatioSalvar.CheckListAssinaturaResponsavelAprovacao = ((string)sequenciaGestaoPatio.CheckListAssinaturaResponsavelAprovacao).ToBool();
            sequenciaGestaoPatioSalvar.CheckListGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.CheckListGerarIntegracaoP44).ToBool();
            sequenciaGestaoPatioSalvar.CheckListGerarNovoPedidoAoTerminoFluxo = ((string)sequenciaGestaoPatio.CheckListGerarNovoPedidoAoTerminoFluxo).ToBool();
            sequenciaGestaoPatioSalvar.CheckListExigirAnexo = ((string)sequenciaGestaoPatio.CheckListExigirAnexo).ToBool();
            int codigoTipoOperacao = ((string)sequenciaGestaoPatio.CheckListTipoOperacao).ToInt();
            sequenciaGestaoPatioSalvar.CheckListTipoOperacao = codigoTipoOperacao > 0 ? repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao, false) : null;
            double codigoDestinatario = ((string)sequenciaGestaoPatio.CheckListDestinatario).ToDouble();
            sequenciaGestaoPatioSalvar.CheckListDestinatario = codigoDestinatario > 0 ? repositorioDestinatario.BuscarPorCPFCNPJ(codigoDestinatario) : null;
            sequenciaGestaoPatioSalvar.CheckListUtilizarVigencia = ((string)sequenciaGestaoPatio.CheckListUtilizarVigencia).ToBool();

            sequenciaGestaoPatioSalvar.TipoChecklistImpressao = ((bool?)sequenciaGestaoPatio.TipoCheckListImpressao ?? false) ? repositorioTipoChecklistImpressao.BuscarPrimeiroRegistro() : null;

            sequenciaGestaoPatioSalvar.ChegadaLoja = (bool)sequenciaGestaoPatio.ChegadaLoja;
            sequenciaGestaoPatioSalvar.ChegadaLojaTempo = (int)sequenciaGestaoPatio.ChegadaLojaTempo;
            sequenciaGestaoPatioSalvar.ChegadaLojaTempoPermanencia = (int)sequenciaGestaoPatio.ChegadaLojaTempoPermanencia;
            sequenciaGestaoPatioSalvar.ChegadaLojaDescricao = (string)sequenciaGestaoPatio.ChegadaLojaDescricao;
            sequenciaGestaoPatioSalvar.ChegadaLojaHabilitarIntegracao = (bool)sequenciaGestaoPatio.ChegadaLojaHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.ChegadaLojaCodigoIntegracao = sequenciaGestaoPatioSalvar.ChegadaLojaHabilitarIntegracao ? (string)sequenciaGestaoPatio.ChegadaLojaCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.ChegadaLojaGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.ChegadaLojaGerarIntegracaoP44).ToBool();

            sequenciaGestaoPatioSalvar.ChegadaVeiculo = (bool)sequenciaGestaoPatio.ChegadaVeiculo;
            sequenciaGestaoPatioSalvar.ChegadaVeiculoTempo = (int)sequenciaGestaoPatio.ChegadaVeiculoTempo;
            sequenciaGestaoPatioSalvar.ChegadaVeiculoTempoPermanencia = (int)sequenciaGestaoPatio.ChegadaVeiculoTempoPermanencia;
            sequenciaGestaoPatioSalvar.ChegadaVeiculoDescricao = (string)sequenciaGestaoPatio.ChegadaVeiculoDescricao;
            sequenciaGestaoPatioSalvar.ChegadaVeiculoHabilitarIntegracao = (bool)sequenciaGestaoPatio.ChegadaVeiculoHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.ChegadaVeiculoPermiteImprimirRelacaoDeProdutos = (bool)sequenciaGestaoPatio.ChegadaVeiculoPermiteImprimirRelacaoDeProdutos;
            sequenciaGestaoPatioSalvar.ChegadaVeiculoPreencherDataSaida = (bool)sequenciaGestaoPatio.ChegadaVeiculoPreencherDataSaida;
            sequenciaGestaoPatioSalvar.ChegadaVeiculoCodigoIntegracao = sequenciaGestaoPatioSalvar.ChegadaVeiculoHabilitarIntegracao ? (string)sequenciaGestaoPatio.ChegadaVeiculoCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.ChegadaVeiculoPermiteTransportadorLancarHorarios = ((string)sequenciaGestaoPatio.ChegadaVeiculoPermiteTransportadorLancarHorarios).ToBool();
            sequenciaGestaoPatioSalvar.ChegadaVeiculoImprimirComprovanteModeloColetaOutbound = ((string)sequenciaGestaoPatio.ChegadaVeiculoImprimirComprovanteModeloColetaOutbound).ToBool();
            sequenciaGestaoPatioSalvar.ChegadaVeiculoGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.ChegadaVeiculoGerarIntegracaoP44).ToBool();

            sequenciaGestaoPatioSalvar.DeslocamentoPatio = ((string)sequenciaGestaoPatio.DeslocamentoPatio).ToBool();
            sequenciaGestaoPatioSalvar.DeslocamentoPatioTempo = ((string)sequenciaGestaoPatio.DeslocamentoPatioTempo).ToInt();
            sequenciaGestaoPatioSalvar.DeslocamentoPatioTempoPermanencia = ((string)sequenciaGestaoPatio.DeslocamentoPatioTempoPermanencia).ToInt();
            sequenciaGestaoPatioSalvar.DeslocamentoPatioDescricao = (string)sequenciaGestaoPatio.DeslocamentoPatioDescricao;
            sequenciaGestaoPatioSalvar.DeslocamentoPatioHabilitarIntegracao = ((string)sequenciaGestaoPatio.DeslocamentoPatioHabilitarIntegracao).ToBool();
            sequenciaGestaoPatioSalvar.DeslocamentoPatioCodigoIntegracao = sequenciaGestaoPatioSalvar.DeslocamentoPatioHabilitarIntegracao ? (string)sequenciaGestaoPatio.DeslocamentoPatioCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.DeslocamentoPatioPermiteInformacoesPesagem = ((string)sequenciaGestaoPatio.DeslocamentoPatioPermiteInformacoesPesagem).ToBool();
            sequenciaGestaoPatioSalvar.DeslocamentoPatioPermiteInformacoesLoteInterno = ((string)sequenciaGestaoPatio.DeslocamentoPatioPermiteInformacoesLoteInterno).ToBool();
            sequenciaGestaoPatioSalvar.DeslocamentoPatioPermiteInformarQuantidade = ((string)sequenciaGestaoPatio.DeslocamentoPatioPermiteInformarQuantidade).ToBool();
            sequenciaGestaoPatioSalvar.DeslocamentoPatioGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.DeslocamentoPatioGerarIntegracaoP44).ToBool();

            sequenciaGestaoPatioSalvar.Faturamento = (bool)sequenciaGestaoPatio.Faturamento;
            sequenciaGestaoPatioSalvar.FaturamentoTempo = (int)sequenciaGestaoPatio.FaturamentoTempo;
            sequenciaGestaoPatioSalvar.FaturamentoTempoPermanencia = (int)sequenciaGestaoPatio.FaturamentoTempoPermanencia;
            sequenciaGestaoPatioSalvar.FaturamentoDescricao = (string)sequenciaGestaoPatio.FaturamentoDescricao;
            sequenciaGestaoPatioSalvar.FaturamentoHabilitarIntegracao = (bool)sequenciaGestaoPatio.FaturamentoHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.FaturamentoCodigoIntegracao = sequenciaGestaoPatioSalvar.FaturamentoHabilitarIntegracao ? (string)sequenciaGestaoPatio.FaturamentoCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.FaturamentoGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.FaturamentoGerarIntegracaoP44).ToBool();

            sequenciaGestaoPatioSalvar.FimDescarregamento = (bool)sequenciaGestaoPatio.FimDescarregamento;
            sequenciaGestaoPatioSalvar.FimDescarregamentoTempo = (int)sequenciaGestaoPatio.FimDescarregamentoTempo;
            sequenciaGestaoPatioSalvar.FimDescarregamentoTempoPermanencia = (int)sequenciaGestaoPatio.FimDescarregamentoTempoPermanencia;
            sequenciaGestaoPatioSalvar.FimDescarregamentoDescricao = (string)sequenciaGestaoPatio.FimDescarregamentoDescricao;
            sequenciaGestaoPatioSalvar.FimDescarregamentoHabilitarIntegracao = (bool)sequenciaGestaoPatio.FimDescarregamentoHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.FimDescarregamentoCodigoIntegracao = sequenciaGestaoPatioSalvar.FimDescarregamentoHabilitarIntegracao ? (string)sequenciaGestaoPatio.FimDescarregamentoCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.FimDescarregamentoPermiteInformarPesagem = ((string)sequenciaGestaoPatio.FimDescarregamentoPermiteInformarPesagem).ToBool();
            sequenciaGestaoPatioSalvar.FimDescarregamentoGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.FimDescarregamentoGerarIntegracaoP44).ToBool();

            sequenciaGestaoPatioSalvar.FimHigienizacao = (bool)sequenciaGestaoPatio.FimHigienizacao;
            sequenciaGestaoPatioSalvar.FimHigienizacaoTempo = (int)sequenciaGestaoPatio.FimHigienizacaoTempo;
            sequenciaGestaoPatioSalvar.FimHigienizacaoTempoPermanencia = (int)sequenciaGestaoPatio.FimHigienizacaoTempoPermanencia;
            sequenciaGestaoPatioSalvar.FimHigienizacaoDescricao = (string)sequenciaGestaoPatio.FimHigienizacaoDescricao;
            sequenciaGestaoPatioSalvar.FimHigienizacaoHabilitarIntegracao = (bool)sequenciaGestaoPatio.FimHigienizacaoHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.FimHigienizacaoCodigoIntegracao = sequenciaGestaoPatioSalvar.FimHigienizacaoHabilitarIntegracao ? (string)sequenciaGestaoPatio.FimHigienizacaoCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.FimHigienizacaoGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.FimHigienizacaoGerarIntegracaoP44).ToBool();

            sequenciaGestaoPatioSalvar.FimViagem = (bool)sequenciaGestaoPatio.FimViagem;
            sequenciaGestaoPatioSalvar.FimViagemTempo = (int)sequenciaGestaoPatio.FimViagemTempo;
            sequenciaGestaoPatioSalvar.FimViagemTempoPermanencia = (int)sequenciaGestaoPatio.FimViagemTempoPermanencia;
            sequenciaGestaoPatioSalvar.FimViagemDescricao = (string)sequenciaGestaoPatio.FimViagemDescricao;
            sequenciaGestaoPatioSalvar.FimViagemHabilitarIntegracao = (bool)sequenciaGestaoPatio.FimViagemHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.FimViagemCodigoIntegracao = sequenciaGestaoPatioSalvar.FimViagemHabilitarIntegracao ? (string)sequenciaGestaoPatio.FimViagemCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.FimViagemGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.FimViagemGerarIntegracaoP44).ToBool();

            sequenciaGestaoPatioSalvar.GuaritaEntrada = ((string)sequenciaGestaoPatio.GuaritaEntrada).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaEntradaTempo = ((string)sequenciaGestaoPatio.GuaritaEntradaTempo).ToInt();
            sequenciaGestaoPatioSalvar.GuaritaEntradaTempoPermanencia = ((string)sequenciaGestaoPatio.GuaritaEntradaTempoPermanencia).ToInt();
            sequenciaGestaoPatioSalvar.GuaritaEntradaDescricao = (string)sequenciaGestaoPatio.GuaritaEntradaDescricao;
            sequenciaGestaoPatioSalvar.GuaritaEntradaHabilitarIntegracao = ((string)sequenciaGestaoPatio.GuaritaEntradaHabilitarIntegracao).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaEntradaCodigoIntegracao = sequenciaGestaoPatioSalvar.GuaritaEntradaHabilitarIntegracao ? (string)sequenciaGestaoPatio.GuaritaEntradaCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.GuaritaEntradaInformarDoca = ((string)sequenciaGestaoPatio.GuaritaEntradaInformarDoca).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaEntradaExibirHorarioExato = ((string)sequenciaGestaoPatio.GuaritaEntradaExibirHorarioExato).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaEntradaPermiteInformacoesPesagem = ((string)sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesPesagem).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaEntradaPermiteInformacoesProdutor = ((string)sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesProdutor).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaEntradaPermiteInformarAnexoPesagem = ((string)sequenciaGestaoPatio.GuaritaEntradaPermiteInformarAnexoPesagem).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaEntradaPermiteInformarPressaoPesagem = ((string)sequenciaGestaoPatio.GuaritaEntradaPermiteInformarPressaoPesagem).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaEntradaPermiteInformarQuantidadeCaixasPesagem = ((string)sequenciaGestaoPatio.GuaritaEntradaPermiteInformarQuantidadeCaixasPesagem).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaEntradaPermiteDenegarChegada = ((string)sequenciaGestaoPatio.GuaritaEntradaPermiteDenegarChegada).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaEntradaPermiteTransportadorLancarHorarios = ((string)sequenciaGestaoPatio.GuaritaEntradaPermiteTransportadorLancarHorarios).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaEntradaPermiteInformarDadosDevolucao = ((string)sequenciaGestaoPatio.GuaritaEntradaPermiteInformarDadosDevolucao).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaEntradaGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.GuaritaEntradaGerarIntegracaoP44).ToBool();
            sequenciaGestaoPatioSalvar.ImprimirTicketBalanca = ((string)sequenciaGestaoPatio.ImprimirTicketBalanca).ToBool();
            int codigoBalancaGuaritaEntrada = ((string)sequenciaGestaoPatio.BalancaGuaritaEntrada).ToInt();
            sequenciaGestaoPatioSalvar.BalancaGuaritaEntrada = codigoBalancaGuaritaEntrada > 0 ? repositorioFilialBalanca.BuscarPorCodigo(codigoBalancaGuaritaEntrada, false) : null;
            if (sequenciaGestaoPatioSalvar.BalancaGuaritaEntrada != null && sequenciaGestaoPatioSalvar.BalancaGuaritaEntrada.Filial.Codigo != filial.Codigo)
                throw new ControllerException(Localization.Resources.Filiais.Filial.BalancaSelecionadaNaoEDaMesmaFilial);
            sequenciaGestaoPatioSalvar.GuaritaEntradaTipoIntegracaoBalanca = ((string)sequenciaGestaoPatio.GuaritaEntradaTipoIntegracaoBalanca).ToNullableEnum<TipoIntegracao>();

            sequenciaGestaoPatioSalvar.GuaritaSaida = ((string)sequenciaGestaoPatio.GuaritaSaida).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaSaidaTempo = ((string)sequenciaGestaoPatio.GuaritaSaidaTempo).ToInt();
            sequenciaGestaoPatioSalvar.GuaritaSaidaDescricao = (string)sequenciaGestaoPatio.GuaritaSaidaDescricao;
            sequenciaGestaoPatioSalvar.GuaritaSaidaHabilitarIntegracao = ((string)sequenciaGestaoPatio.GuaritaSaidaHabilitarIntegracao).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaSaidaCodigoIntegracao = sequenciaGestaoPatioSalvar.GuaritaSaidaHabilitarIntegracao ? (string)sequenciaGestaoPatio.GuaritaSaidaCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.GuaritaSaidaPermiteInformacoesPesagem = ((string)sequenciaGestaoPatio.GuaritaSaidaPermiteInformacoesPesagem).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaSaidaPermiteInformarLacrePesagem = ((string)sequenciaGestaoPatio.GuaritaSaidaPermiteInformarLacrePesagem).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaSaidaPermiteInformarPercentualRefugoPesagem = ((string)sequenciaGestaoPatio.GuaritaSaidaPermiteInformarPercentualRefugoPesagem).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaSaidaIniciarEmissaoDocumentosTransporte = ((string)sequenciaGestaoPatio.GuaritaSaidaIniciarEmissaoDocumentosTransporte).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaSaidaPermiteAnexosPesagem = ((string)sequenciaGestaoPatio.GuaritaSaidaPermiteAnexosPesagem).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaSaidaGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.GuaritaSaidaGerarIntegracaoP44).ToBool();
            sequenciaGestaoPatioSalvar.GuaritaSaidaExigirAnexo = ((string)sequenciaGestaoPatio.GuaritaSaidaExigirAnexo).ToBool();
            int codigoBalancaGuaritaSaida = ((string)sequenciaGestaoPatio.BalancaGuaritaSaida).ToInt();
            sequenciaGestaoPatioSalvar.BalancaGuaritaSaida = codigoBalancaGuaritaSaida > 0 ? repositorioFilialBalanca.BuscarPorCodigo(codigoBalancaGuaritaSaida, false) : null;
            if (sequenciaGestaoPatioSalvar.BalancaGuaritaSaida != null && sequenciaGestaoPatioSalvar.BalancaGuaritaSaida.Filial.Codigo != filial.Codigo)
                throw new ControllerException(Localization.Resources.Filiais.Filial.BalancaSelecionadaNaoEDaMesmaFilial);
            sequenciaGestaoPatioSalvar.GuaritaSaidaTipoIntegracaoBalanca = ((string)sequenciaGestaoPatio.GuaritaSaidaTipoIntegracaoBalanca).ToNullableEnum<TipoIntegracao>();

            sequenciaGestaoPatioSalvar.InformarDocaCarregamento = (bool)sequenciaGestaoPatio.InformarDocaCarregamento;
            sequenciaGestaoPatioSalvar.InformarDocaCarregamentoTempo = (int)sequenciaGestaoPatio.InformarDocaCarregamentoTempo;
            sequenciaGestaoPatioSalvar.InformarDocaCarregamentoTempoPermanencia = (int)sequenciaGestaoPatio.InformarDocaCarregamentoTempoPermanencia;
            sequenciaGestaoPatioSalvar.InformarDocaCarregamentoDescricao = (string)sequenciaGestaoPatio.InformarDocaCarregamentoDescricao;
            sequenciaGestaoPatioSalvar.InformarDocaCarregamentoHabilitarIntegracao = (bool)sequenciaGestaoPatio.InformarDocaCarregamentoHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.InformarDocaCarregamentoCodigoIntegracao = sequenciaGestaoPatioSalvar.InformarDocaCarregamentoHabilitarIntegracao ? (string)sequenciaGestaoPatio.InformarDocaCarregamentoCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.InformarDocaCarregamentoPermiteTransportadorLancarHorarios = ((string)sequenciaGestaoPatio.InformarDocaCarregamentoPermiteTransportadorLancarHorarios).ToBool();
            sequenciaGestaoPatioSalvar.InformarDocaCarregamentoPermiteInformarDadosLaudo = ((string)sequenciaGestaoPatio.InformarDocaCarregamentoPermiteInformarDadosLaudo).ToBool();
            sequenciaGestaoPatioSalvar.InformarDocaCarregamentoGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.InformarDocaCarregamentoGerarIntegracaoP44).ToBool();
            sequenciaGestaoPatioSalvar.InformarDocaCarregamentoTipoIntegracao = ((string)sequenciaGestaoPatio.InformarDocaCarregamentoTipoIntegracao).ToEnum<TipoIntegracao>();

            sequenciaGestaoPatioSalvar.InicioDescarregamento = (bool)sequenciaGestaoPatio.InicioDescarregamento;
            sequenciaGestaoPatioSalvar.InicioDescarregamentoTempo = (int)sequenciaGestaoPatio.InicioDescarregamentoTempo;
            sequenciaGestaoPatioSalvar.InicioDescarregamentoTempoPermanencia = (int)sequenciaGestaoPatio.InicioDescarregamentoTempoPermanencia;
            sequenciaGestaoPatioSalvar.InicioDescarregamentoDescricao = (string)sequenciaGestaoPatio.InicioDescarregamentoDescricao;
            sequenciaGestaoPatioSalvar.InicioDescarregamentoHabilitarIntegracao = (bool)sequenciaGestaoPatio.InicioDescarregamentoHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.InicioDescarregamentoCodigoIntegracao = sequenciaGestaoPatioSalvar.InicioDescarregamentoHabilitarIntegracao ? (string)sequenciaGestaoPatio.InicioDescarregamentoCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.InicioDescarregamentoPermiteInformarPesagem = ((string)sequenciaGestaoPatio.InicioDescarregamentoPermiteInformarPesagem).ToBool();
            sequenciaGestaoPatioSalvar.InicioDescarregamentoGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.InicioDescarregamentoGerarIntegracaoP44).ToBool();

            sequenciaGestaoPatioSalvar.InicioHigienizacao = (bool)sequenciaGestaoPatio.InicioHigienizacao;
            sequenciaGestaoPatioSalvar.InicioHigienizacaoTempo = (int)sequenciaGestaoPatio.InicioHigienizacaoTempo;
            sequenciaGestaoPatioSalvar.InicioHigienizacaoTempoPermanencia = (int)sequenciaGestaoPatio.InicioHigienizacaoTempoPermanencia;
            sequenciaGestaoPatioSalvar.InicioHigienizacaoDescricao = (string)sequenciaGestaoPatio.InicioHigienizacaoDescricao;
            sequenciaGestaoPatioSalvar.InicioHigienizacaoHabilitarIntegracao = (bool)sequenciaGestaoPatio.InicioHigienizacaoHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.InicioHigienizacaoCodigoIntegracao = sequenciaGestaoPatioSalvar.InicioHigienizacaoHabilitarIntegracao ? (string)sequenciaGestaoPatio.InicioHigienizacaoCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.InicioHigienizacaoGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.InicioHigienizacaoGerarIntegracaoP44).ToBool();

            sequenciaGestaoPatioSalvar.LiberaChave = (bool)sequenciaGestaoPatio.LiberaChave;
            sequenciaGestaoPatioSalvar.LiberaChaveTempo = (int)sequenciaGestaoPatio.LiberaChaveTempo;
            sequenciaGestaoPatioSalvar.LiberaChaveTempoPermanencia = (int)sequenciaGestaoPatio.LiberaChaveTempoPermanencia;
            sequenciaGestaoPatioSalvar.LiberaChaveDescricao = (string)sequenciaGestaoPatio.LiberaChaveDescricao;
            sequenciaGestaoPatioSalvar.LiberaChaveHabilitarIntegracao = (bool)sequenciaGestaoPatio.LiberaChaveHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.LiberaChaveBloquearLiberacaoEtapaAnterior = ((string)sequenciaGestaoPatio.LiberaChaveBloquearLiberacaoEtapaAnterior).ToBool();
            sequenciaGestaoPatioSalvar.LiberaChaveCodigoIntegracao = sequenciaGestaoPatioSalvar.LiberaChaveHabilitarIntegracao ? (string)sequenciaGestaoPatio.LiberaChaveCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.LiberaChaveInformarNumeroDePaletes = ((string)sequenciaGestaoPatio.LiberaChaveInformarNumeroDePaletes).ToBool();
            sequenciaGestaoPatioSalvar.LiberaChaveSolicitarAssinaturaMotorista = ((string)sequenciaGestaoPatio.LiberaChaveSolicitarAssinaturaMotorista).ToBool();
            sequenciaGestaoPatioSalvar.LiberaChaveGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.LiberaChaveGerarIntegracaoP44).ToBool();
            sequenciaGestaoPatioSalvar.LiberaChaveExigirAnexo = ((string)sequenciaGestaoPatio.LiberaChaveExigirAnexo).ToBool();

            sequenciaGestaoPatioSalvar.SaidaLoja = (bool)sequenciaGestaoPatio.SaidaLoja;
            sequenciaGestaoPatioSalvar.SaidaLojaTempo = (int)sequenciaGestaoPatio.SaidaLojaTempo;
            sequenciaGestaoPatioSalvar.SaidaLojaTempoPermanencia = (int)sequenciaGestaoPatio.SaidaLojaTempoPermanencia;
            sequenciaGestaoPatioSalvar.SaidaLojaDescricao = (string)sequenciaGestaoPatio.SaidaLojaDescricao;
            sequenciaGestaoPatioSalvar.SaidaLojaHabilitarIntegracao = (bool)sequenciaGestaoPatio.SaidaLojaHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.SaidaLojaCodigoIntegracao = sequenciaGestaoPatioSalvar.SaidaLojaHabilitarIntegracao ? (string)sequenciaGestaoPatio.SaidaLojaCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.SaidaLojaGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.SaidaLojaGerarIntegracaoP44).ToBool();

            sequenciaGestaoPatioSalvar.SolicitacaoVeiculo = (bool)sequenciaGestaoPatio.SolicitacaoVeiculo;
            sequenciaGestaoPatioSalvar.SolicitacaoVeiculoTempo = (int)sequenciaGestaoPatio.SolicitacaoVeiculoTempo;
            sequenciaGestaoPatioSalvar.SolicitacaoVeiculoTempoPermanencia = (int)sequenciaGestaoPatio.SolicitacaoVeiculoTempoPermanencia;
            sequenciaGestaoPatioSalvar.SolicitacaoVeiculoDescricao = (string)sequenciaGestaoPatio.SolicitacaoVeiculoDescricao;
            sequenciaGestaoPatioSalvar.SolicitacaoVeiculoHabilitarIntegracao = (bool)sequenciaGestaoPatio.SolicitacaoVeiculoHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.SolicitacaoVeiculoCodigoIntegracao = sequenciaGestaoPatioSalvar.SolicitacaoVeiculoHabilitarIntegracao ? (string)sequenciaGestaoPatio.SolicitacaoVeiculoCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.SolicitacaoVeiculoGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.SolicitacaoVeiculoGerarIntegracaoP44).ToBool();
            sequenciaGestaoPatioSalvar.NaoPermitirEnviarSMS = ((string)sequenciaGestaoPatio.NaoPermitirEnviarSMS).ToBool();
            sequenciaGestaoPatioSalvar.SolicitacaoVeiculoHabilitarIntegracaoPager = ((string)sequenciaGestaoPatio.SolicitacaoVeiculoHabilitarIntegracaoPager).ToBool();
            sequenciaGestaoPatioSalvar.SolicitacaoVeiculoPermitirInformarDadosTransporteCarga = ((string)sequenciaGestaoPatio.SolicitacaoVeiculoPermitirInformarDadosTransporteCarga).ToBool();

            sequenciaGestaoPatioSalvar.TravaChave = (bool)sequenciaGestaoPatio.TravaChave;
            sequenciaGestaoPatioSalvar.TravaChaveInformarDoca = (bool)sequenciaGestaoPatio.TravaChaveInformarDoca;
            sequenciaGestaoPatioSalvar.TravaChaveTempo = (int)sequenciaGestaoPatio.TravaChaveTempo;
            sequenciaGestaoPatioSalvar.TravaChaveTempoPermanencia = (int)sequenciaGestaoPatio.TravaChaveTempoPermanencia;
            sequenciaGestaoPatioSalvar.TravaChaveDescricao = (string)sequenciaGestaoPatio.TravaChaveDescricao;
            sequenciaGestaoPatioSalvar.TravaChaveHabilitarIntegracao = (bool)sequenciaGestaoPatio.TravaChaveHabilitarIntegracao;
            sequenciaGestaoPatioSalvar.TravaChaveCodigoIntegracao = sequenciaGestaoPatioSalvar.TravaChaveHabilitarIntegracao ? (string)sequenciaGestaoPatio.TravaChaveCodigoIntegracao : string.Empty;
            sequenciaGestaoPatioSalvar.TravaChavePermiteTransportadorLancarHorarios = ((string)sequenciaGestaoPatio.TravaChavePermiteTransportadorLancarHorarios).ToBool();
            sequenciaGestaoPatioSalvar.TravaChaveGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.TravaChaveGerarIntegracaoP44).ToBool();

            if (tipo == TipoFluxoGestaoPatio.Origem)
            {
                sequenciaGestaoPatioSalvar.DocumentoFiscal = (bool)sequenciaGestaoPatio.DocumentoFiscal;
                sequenciaGestaoPatioSalvar.DocumentoFiscalTempo = (int)sequenciaGestaoPatio.DocumentoFiscalTempo;
                sequenciaGestaoPatioSalvar.DocumentoFiscalTempoPermanencia = (int)sequenciaGestaoPatio.DocumentoFiscalTempoPermanencia;
                sequenciaGestaoPatioSalvar.DocumentoFiscalDescricao = (string)sequenciaGestaoPatio.DocumentoFiscalDescricao;
                sequenciaGestaoPatioSalvar.DocumentoFiscalVincularNotaFiscal = ((string)sequenciaGestaoPatio.DocumentoFiscalVincularNotaFiscal).ToBool();
                sequenciaGestaoPatioSalvar.DocumentoFiscalHabilitarIntegracao = (bool)sequenciaGestaoPatio.DocumentoFiscalHabilitarIntegracao;
                sequenciaGestaoPatioSalvar.DocumentoFiscalCodigoIntegracao = sequenciaGestaoPatioSalvar.DocumentoFiscalHabilitarIntegracao ? (string)sequenciaGestaoPatio.DocumentoFiscalCodigoIntegracao : string.Empty;
                sequenciaGestaoPatioSalvar.DocumentoFiscalGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.DocumentoFiscalGerarIntegracaoP44).ToBool();

                sequenciaGestaoPatioSalvar.DocumentosTransporte = (bool)sequenciaGestaoPatio.DocumentosTransporte;
                sequenciaGestaoPatioSalvar.DocumentosTransporteTempo = (int)sequenciaGestaoPatio.DocumentosTransporteTempo;
                sequenciaGestaoPatioSalvar.DocumentosTransporteTempoPermanencia = (int)sequenciaGestaoPatio.DocumentosTransporteTempoPermanencia;
                sequenciaGestaoPatioSalvar.DocumentosTransporteDescricao = (string)sequenciaGestaoPatio.DocumentosTransporteDescricao;
                sequenciaGestaoPatioSalvar.DocumentosTransporteHabilitarIntegracao = (bool)sequenciaGestaoPatio.DocumentosTransporteHabilitarIntegracao;
                sequenciaGestaoPatioSalvar.DocumentosTransporteCodigoIntegracao = sequenciaGestaoPatioSalvar.DocumentosTransporteHabilitarIntegracao ? (string)sequenciaGestaoPatio.DocumentosTransporteCodigoIntegracao : string.Empty;
                sequenciaGestaoPatioSalvar.DocumentosTransporteGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.DocumentosTransporteGerarIntegracaoP44).ToBool();

                sequenciaGestaoPatioSalvar.Expedicao = (bool)sequenciaGestaoPatio.Expedicao;
                sequenciaGestaoPatioSalvar.ExpedicaoInformarDoca = (bool)sequenciaGestaoPatio.ExpedicaoInformarDoca;
                sequenciaGestaoPatioSalvar.ExpedicaoInformarInicioCarregamento = (bool)sequenciaGestaoPatio.ExpedicaoInformarInicioCarregamento;
                sequenciaGestaoPatioSalvar.ExpedicaoInformarTerminoCarregamento = (bool)sequenciaGestaoPatio.ExpedicaoInformarTerminoCarregamento;
                sequenciaGestaoPatioSalvar.ExpedicaoTempo = (int)sequenciaGestaoPatio.ExpedicaoTempo;
                sequenciaGestaoPatioSalvar.ExpedicaoDescricao = (string)sequenciaGestaoPatio.ExpedicaoDescricao;
                sequenciaGestaoPatioSalvar.ExpedicaoHabilitarIntegracao = (bool)sequenciaGestaoPatio.ExpedicaoHabilitarIntegracao;
                sequenciaGestaoPatioSalvar.ExpedicaoCodigoIntegracao = sequenciaGestaoPatioSalvar.ExpedicaoHabilitarIntegracao ? (string)sequenciaGestaoPatio.ExpedicaoCodigoIntegracao : string.Empty;
                sequenciaGestaoPatioSalvar.ExpedicaoGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.ExpedicaoGerarIntegracaoP44).ToBool();

                sequenciaGestaoPatioSalvar.FimCarregamento = (bool)sequenciaGestaoPatio.FimCarregamento;
                sequenciaGestaoPatioSalvar.FimCarregamentoTempo = (int)sequenciaGestaoPatio.FimCarregamentoTempo;
                sequenciaGestaoPatioSalvar.FimCarregamentoTempoPermanencia = (int)sequenciaGestaoPatio.FimCarregamentoTempoPermanencia;
                sequenciaGestaoPatioSalvar.FimCarregamentoDescricao = (string)sequenciaGestaoPatio.FimCarregamentoDescricao;
                sequenciaGestaoPatioSalvar.FimCarregamentoHabilitarIntegracao = (bool)sequenciaGestaoPatio.FimCarregamentoHabilitarIntegracao;
                sequenciaGestaoPatioSalvar.FimCarregamentoCodigoIntegracao = sequenciaGestaoPatioSalvar.FimCarregamentoHabilitarIntegracao ? (string)sequenciaGestaoPatio.FimCarregamentoCodigoIntegracao : string.Empty;
                sequenciaGestaoPatioSalvar.FimCarregamentoPermiteInformarPesagem = ((string)sequenciaGestaoPatio.FimCarregamentoPermiteInformarPesagem).ToBool();
                sequenciaGestaoPatioSalvar.FimCarregamentoGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.FimCarregamentoGerarIntegracaoP44).ToBool();

                sequenciaGestaoPatioSalvar.InicioCarregamento = (bool)sequenciaGestaoPatio.InicioCarregamento;
                sequenciaGestaoPatioSalvar.InicioCarregamentoTempo = (int)sequenciaGestaoPatio.InicioCarregamentoTempo;
                sequenciaGestaoPatioSalvar.InicioCarregamentoTempoPermanencia = (int)sequenciaGestaoPatio.InicioCarregamentoTempoPermanencia;
                sequenciaGestaoPatioSalvar.InicioCarregamentoDescricao = (string)sequenciaGestaoPatio.InicioCarregamentoDescricao;
                sequenciaGestaoPatioSalvar.InicioCarregamentoHabilitarIntegracao = (bool)sequenciaGestaoPatio.InicioCarregamentoHabilitarIntegracao;
                sequenciaGestaoPatioSalvar.InicioCarregamentoCodigoIntegracao = sequenciaGestaoPatioSalvar.InicioCarregamentoHabilitarIntegracao ? (string)sequenciaGestaoPatio.InicioCarregamentoCodigoIntegracao : string.Empty;
                sequenciaGestaoPatioSalvar.InicioCarregamentoPermiteInformarPesagem = ((string)sequenciaGestaoPatio.InicioCarregamentoPermiteInformarPesagem).ToBool();
                sequenciaGestaoPatioSalvar.InicioCarregamentoGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.InicioCarregamentoGerarIntegracaoP44).ToBool();
                sequenciaGestaoPatioSalvar.DeslocamentoPatioPermiteInformarQuantidade = ((string)sequenciaGestaoPatio.DeslocamentoPatioPermiteInformarQuantidade).ToBool();

                sequenciaGestaoPatioSalvar.MontagemCarga = (bool)sequenciaGestaoPatio.MontagemCarga;
                sequenciaGestaoPatioSalvar.MontagemCargaTempo = (int)sequenciaGestaoPatio.MontagemCargaTempo;
                sequenciaGestaoPatioSalvar.MontagemCargaTempoPermanencia = (int)sequenciaGestaoPatio.MontagemCargaTempoPermanencia;
                sequenciaGestaoPatioSalvar.MontagemCargaDescricao = (string)sequenciaGestaoPatio.MontagemCargaDescricao;
                sequenciaGestaoPatioSalvar.MontagemCargaHabilitarIntegracao = (bool)sequenciaGestaoPatio.MontagemCargaHabilitarIntegracao;
                sequenciaGestaoPatioSalvar.MontagemCargaCodigoIntegracao = sequenciaGestaoPatioSalvar.MontagemCargaHabilitarIntegracao ? (string)sequenciaGestaoPatio.MontagemCargaCodigoIntegracao : string.Empty;
                sequenciaGestaoPatioSalvar.MontagemCargaInformarDoca = (bool)sequenciaGestaoPatio.MontagemCargaInformarDoca;
                sequenciaGestaoPatioSalvar.MontagemCargaPermiteInformarQuantidadeCaixas = ((string)sequenciaGestaoPatio.MontagemCargaPermiteInformarQuantidadeCaixas).ToBool();
                sequenciaGestaoPatioSalvar.MontagemCargaPermiteInformarQuantidadeItens = ((string)sequenciaGestaoPatio.MontagemCargaPermiteInformarQuantidadeItens).ToBool();
                sequenciaGestaoPatioSalvar.MontagemCargaPermiteInformarQuantidadePallets = ((string)sequenciaGestaoPatio.MontagemCargaPermiteInformarQuantidadePallets).ToBool();
                sequenciaGestaoPatioSalvar.MontagemCargaGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.MontagemCargaGerarIntegracaoP44).ToBool();

                sequenciaGestaoPatioSalvar.Posicao = (bool)sequenciaGestaoPatio.Posicao;
                sequenciaGestaoPatioSalvar.PosicaoTempo = (int)sequenciaGestaoPatio.PosicaoTempo;
                sequenciaGestaoPatioSalvar.PosicaoTempoPermanencia = (int)sequenciaGestaoPatio.PosicaoTempoPermanencia;
                sequenciaGestaoPatioSalvar.PosicaoDescricao = (string)sequenciaGestaoPatio.PosicaoDescricao;
                sequenciaGestaoPatioSalvar.PosicaoHabilitarIntegracao = (bool)sequenciaGestaoPatio.PosicaoHabilitarIntegracao;
                sequenciaGestaoPatioSalvar.PosicaoCodigoIntegracao = sequenciaGestaoPatioSalvar.PosicaoHabilitarIntegracao ? (string)sequenciaGestaoPatio.PosicaoCodigoIntegracao : string.Empty;
                sequenciaGestaoPatioSalvar.PosicaoGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.PosicaoGerarIntegracaoP44).ToBool();

                sequenciaGestaoPatioSalvar.SeparacaoMercadoria = (bool)sequenciaGestaoPatio.SeparacaoMercadoria;
                sequenciaGestaoPatioSalvar.SeparacaoMercadoriaTempo = (int)sequenciaGestaoPatio.SeparacaoMercadoriaTempo;
                sequenciaGestaoPatioSalvar.SeparacaoMercadoriaTempoPermanencia = (int)sequenciaGestaoPatio.SeparacaoMercadoriaTempoPermanencia;
                sequenciaGestaoPatioSalvar.SeparacaoMercadoriaDescricao = (string)sequenciaGestaoPatio.SeparacaoMercadoriaDescricao;
                sequenciaGestaoPatioSalvar.SeparacaoMercadoriaHabilitarIntegracao = (bool)sequenciaGestaoPatio.SeparacaoMercadoriaHabilitarIntegracao;
                sequenciaGestaoPatioSalvar.SeparacaoMercadoriaCodigoIntegracao = sequenciaGestaoPatioSalvar.SeparacaoMercadoriaHabilitarIntegracao ? (string)sequenciaGestaoPatio.SeparacaoMercadoriaCodigoIntegracao : string.Empty;
                sequenciaGestaoPatioSalvar.SeparacaoMercadoriaPermiteInformarDadosCarregadores = ((string)sequenciaGestaoPatio.SeparacaoMercadoriaPermiteInformarDadosCarregadores).ToBool();
                sequenciaGestaoPatioSalvar.SeparacaoMercadoriaPermiteInformarDadosSeparadores = ((string)sequenciaGestaoPatio.SeparacaoMercadoriaPermiteInformarDadosSeparadores).ToBool();
                sequenciaGestaoPatioSalvar.SeparacaoMercadoriaGerarIntegracaoP44 = ((string)sequenciaGestaoPatio.SeparacaoMercadoriaGerarIntegracaoP44).ToBool();
            }
            else
            {
                sequenciaGestaoPatioSalvar.AvaliacaoDescarga = (bool)sequenciaGestaoPatio.AvaliacaoDescarga;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaInformarDoca = (bool)sequenciaGestaoPatio.AvaliacaoDescargaInformarDoca;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaTempo = (int)sequenciaGestaoPatio.AvaliacaoDescargaTempo;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaTempoPermanencia = (int)sequenciaGestaoPatio.AvaliacaoDescargaTempoPermanencia;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaDescricao = (string)sequenciaGestaoPatio.AvaliacaoDescargaDescricao;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaHabilitarIntegracao = (bool)sequenciaGestaoPatio.AvaliacaoDescargaHabilitarIntegracao;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaCodigoIntegracao = sequenciaGestaoPatioSalvar.AvaliacaoDescargaHabilitarIntegracao ? (string)sequenciaGestaoPatio.AvaliacaoDescargaCodigoIntegracao : string.Empty;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaPermiteImpressaoApenasComAvaliacaoDescargaFinalizada = (bool)sequenciaGestaoPatio.AvaliacaoDescargaPermiteImpressaoApenasComAvaliacaoDescargaFinalizada;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaPermitePreencherAvaliacaoDescargaAntesDeChegarNaEtapa = (bool)sequenciaGestaoPatio.AvaliacaoDescargaPermitePreencherAvaliacaoDescargaAntesDeChegarNaEtapa;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaNotificarPorEmailReprovacao = (bool)sequenciaGestaoPatio.AvaliacaoDescargaNotificarPorEmailReprovacao;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaEmails = (string)sequenciaGestaoPatio.AvaliacaoDescargaEmails;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaCancelarPatioAoReprovar = (bool)sequenciaGestaoPatio.AvaliacaoDescargaCancelarPatioAoReprovar;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaNaoExigeObservacaoAoReprovar = (bool)sequenciaGestaoPatio.AvaliacaoDescargaNaoExigeObservacaoAoReprovar;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaPermiteSalvarSemPreencher = (bool)sequenciaGestaoPatio.AvaliacaoDescargaPermiteSalvarSemPreencher;
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaPermiteTransportadorLancarHorarios = ((string)sequenciaGestaoPatio.AvaliacaoDescargaPermiteTransportadorLancarHorarios).ToBool();
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaAssinaturaMotorista = ((string)sequenciaGestaoPatio.AvaliacaoDescargaAssinaturaMotorista).ToBool();
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaAssinaturaCarregador = ((string)sequenciaGestaoPatio.AvaliacaoDescargaAssinaturaCarregador).ToBool();
                sequenciaGestaoPatioSalvar.AvaliacaoDescargaAssinaturaResponsavelAprovacao = ((string)sequenciaGestaoPatio.AvaliacaoDescargaAssinaturaResponsavelAprovacao).ToBool();
            }

            int informarDocaAtivo = 0;

            if (sequenciaGestaoPatioSalvar.MontagemCargaInformarDoca) informarDocaAtivo++;
            if (sequenciaGestaoPatioSalvar.GuaritaEntradaInformarDoca) informarDocaAtivo++;
            if (sequenciaGestaoPatioSalvar.AvaliacaoDescargaInformarDoca) informarDocaAtivo++;
            if (sequenciaGestaoPatioSalvar.TravaChaveInformarDoca) informarDocaAtivo++;
            if (sequenciaGestaoPatioSalvar.ExpedicaoInformarDoca) informarDocaAtivo++;

            if (informarDocaAtivo > 1)
                throw new ControllerException(Localization.Resources.Filiais.Filial.SomentePossivelSelecionarUmaSequenciaParaInformarDoca);

            foreach (dynamic ordem in ordemSequenciaGestaoPatio)
            {
                EtapaFluxoGestaoPatio etapa = (EtapaFluxoGestaoPatio)ordem.Etapa;

                switch (etapa)
                {
                    case EtapaFluxoGestaoPatio.AvaliacaoDescarga:
                        sequenciaGestaoPatioSalvar.OrdemAvaliacaoDescarga = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.CheckList:
                        sequenciaGestaoPatioSalvar.OrdemCheckList = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.Expedicao:
                        sequenciaGestaoPatioSalvar.OrdemExpedicao = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.Faturamento:
                        sequenciaGestaoPatioSalvar.OrdemFaturamento = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.ChegadaVeiculo:
                        sequenciaGestaoPatioSalvar.OrdemChegadaVeiculo = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.Guarita:
                        sequenciaGestaoPatioSalvar.OrdemGuaritaEntrada = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.InformarDoca:
                        sequenciaGestaoPatioSalvar.OrdemInformarDocaCarregamento = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.InicioViagem:
                        sequenciaGestaoPatioSalvar.OrdemGuaritaSaida = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.LiberacaoChave:
                        sequenciaGestaoPatioSalvar.OrdemLiberaChave = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.TravamentoChave:
                        sequenciaGestaoPatioSalvar.OrdemTravaChave = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.Posicao:
                        sequenciaGestaoPatioSalvar.OrdemPosicao = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.ChegadaLoja:
                        sequenciaGestaoPatioSalvar.OrdemChegadaLoja = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.DeslocamentoPatio:
                        sequenciaGestaoPatioSalvar.OrdemDeslocamentoPatio = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.SaidaLoja:
                        sequenciaGestaoPatioSalvar.OrdemSaidaLoja = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.FimViagem:
                        sequenciaGestaoPatioSalvar.OrdemFimViagem = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.InicioCarregamento:
                        sequenciaGestaoPatioSalvar.OrdemInicioCarregamento = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.InicioHigienizacao:
                        sequenciaGestaoPatioSalvar.OrdemInicioHigienizacao = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.FimCarregamento:
                        sequenciaGestaoPatioSalvar.OrdemFimCarregamento = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.FimHigienizacao:
                        sequenciaGestaoPatioSalvar.OrdemFimHigienizacao = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.SeparacaoMercadoria:
                        sequenciaGestaoPatioSalvar.OrdemSeparacaoMercadoria = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.SolicitacaoVeiculo:
                        sequenciaGestaoPatioSalvar.OrdemSolicitacaoVeiculo = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.InicioDescarregamento:
                        sequenciaGestaoPatioSalvar.OrdemInicioDescarregamento = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.FimDescarregamento:
                        sequenciaGestaoPatioSalvar.OrdemFimDescarregamento = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.DocumentoFiscal:
                        sequenciaGestaoPatioSalvar.OrdemDocumentoFiscal = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.DocumentosTransporte:
                        sequenciaGestaoPatioSalvar.OrdemDocumentosTransporte = (int)ordem.Ordem;
                        break;
                    case EtapaFluxoGestaoPatio.MontagemCarga:
                        sequenciaGestaoPatioSalvar.OrdemMontagemCarga = (int)ordem.Ordem;
                        break;
                }
            }

            if (sequenciaGestaoPatioSalvar.Codigo > 0)
            {
                if (tipo == TipoFluxoGestaoPatio.Origem)
                    repositorioSequenciaGestaoPatio.Atualizar(sequenciaGestaoPatioSalvar, Auditado, null, string.Format(Localization.Resources.Filiais.Filial.AlterouSequenciaGestaoPatio, ""));
                else
                    repositorioSequenciaGestaoPatio.Atualizar(sequenciaGestaoPatioSalvar, Auditado, null, string.Format(Localization.Resources.Filiais.Filial.AlterouSequenciaGestaoPatio, $" {Localization.Resources.Filiais.Filial.DeDestino}"));
            }
            else
            {
                if (tipo == TipoFluxoGestaoPatio.Origem)
                    repositorioSequenciaGestaoPatio.Inserir(sequenciaGestaoPatioSalvar, Auditado, null, "Adicionou uma nova Gestão de Pátio");
                else
                    repositorioSequenciaGestaoPatio.Inserir(sequenciaGestaoPatioSalvar, Auditado, null, "Adicionou uma nova Gestão de Pátio de Destino");
            }

            return sequenciaGestaoPatioSalvar;
        }

        private void SalvarOutrosCodigosIntegracao(ref Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.Embarcador.Filiais.Filial repFilial)
        {
            dynamic outrosCodigosIntegracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("OutrosCodigosIntegracao"));

            if (filial.OutrosCodigosIntegracao == null)
                filial.OutrosCodigosIntegracao = new List<string>();

            filial.OutrosCodigosIntegracao.Clear();

            foreach (var outroCodigo in outrosCodigosIntegracao)
            {
                if (!string.IsNullOrEmpty((string)outroCodigo.CodigoIntegracao) && repFilial.ValidarPorCodigoEmbarcador((string)outroCodigo.CodigoIntegracao, filial.Codigo))
                    throw new ControllerException(string.Format(Localization.Resources.Filiais.Filial.JaExisteFilialComCodigoIntegracaoCadastrada, (string)outroCodigo.CodigoIntegracao));

                filial.OutrosCodigosIntegracao.Add((string)outroCodigo.CodigoIntegracao);
            }
        }

        private void SalvarSetores(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            var listaSetoresFilial = ObterSetores(unidadeDeTrabalho);
            var listaSetoresFilialAdicionar = (from setorFilial in listaSetoresFilial where setorFilial.Codigo == 0 select setorFilial);

            if (filial.Setores?.Count() > 0)
            {
                var listaSetoresFilialAtualizar = (from setorFilial in listaSetoresFilial where setorFilial.Codigo > 0 select setorFilial);
                var listaSetoresFilialRemover = new List<Dominio.Entidades.Embarcador.Filiais.SetorFilial>();

                foreach (var setor in filial.Setores)
                {
                    var setorAtualizar = (from setorFilial in listaSetoresFilial where setorFilial.Codigo == setor.Codigo select setorFilial).FirstOrDefault();

                    if (setorAtualizar == null)
                        listaSetoresFilialRemover.Add(setor);
                    else
                    {
                        setor.Setor = setorAtualizar.Setor;
                        setor.Turnos = setorAtualizar.Turnos;
                    }
                }

                foreach (var setorRemover in listaSetoresFilialRemover)
                {
                    filial.Setores.Remove(setorRemover);
                }
            }

            if (listaSetoresFilialAdicionar.Count() > 0)
            {
                if (filial.Setores == null)
                    filial.Setores = new List<Dominio.Entidades.Embarcador.Filiais.SetorFilial>();

                foreach (var setorAdicionar in listaSetoresFilialAdicionar)
                {
                    filial.Setores.Add(setorAdicionar);
                }
            }
        }

        private void SalvarTiposOperacao(ref Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);

            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacoes"));

            if (filial.TipoOperacoesIsencaoValorDescargaCliente == null)
            {
                filial.TipoOperacoesIsencaoValorDescargaCliente = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            }
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoOperacao in tiposOperacao)
                    codigos.Add((int)tipoOperacao.Tipo.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposDeletar = filial.TipoOperacoesIsencaoValorDescargaCliente.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoDeletar in tiposDeletar)
                    filial.TipoOperacoesIsencaoValorDescargaCliente.Remove(tipoOperacaoDeletar);
            }

            foreach (var tipoOperacao in tiposOperacao)
                if (!filial.TipoOperacoesIsencaoValorDescargaCliente.Any(o => o.Codigo == (int)tipoOperacao.Tipo.Codigo))
                    filial.TipoOperacoesIsencaoValorDescargaCliente.Add(repTipoOperacao.BuscarPorCodigo((int)tipoOperacao.Tipo.Codigo));
        }

        private void SalvarIntegracaoBuonny(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoBuonny repIntegracaoBuonny = new Repositorio.Embarcador.Configuracoes.IntegracaoBuonny(unidadeDeTrabalho);

            dynamic dadosIntegracaoBuonny = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("IntegracaoBuonny"));
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBuonny integracaoBuonny = filial.IntegracaoBuonny ?? new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBuonny();

            if (dadosIntegracaoBuonny != null && !string.IsNullOrWhiteSpace((string)dadosIntegracaoBuonny.CNPJCliente))
            {
                integracaoBuonny.CNPJCliente = Utilidades.String.OnlyNumbers((string)dadosIntegracaoBuonny.CNPJCliente);
                integracaoBuonny.Token = (string)dadosIntegracaoBuonny.Token;

                if (integracaoBuonny.Codigo > 0)
                    repIntegracaoBuonny.Atualizar(integracaoBuonny, Auditado);
                else
                    repIntegracaoBuonny.Inserir(integracaoBuonny, Auditado);

                filial.IntegracaoBuonny = integracaoBuonny;
            }
            else if (filial.IntegracaoBuonny != null)
            {
                filial.IntegracaoBuonny = null;
                repIntegracaoBuonny.Deletar(integracaoBuonny, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, string.Format(Localization.Resources.Filiais.Filial.RemoveuConfiguracoesIntegracao, "Buonny"), unidadeDeTrabalho);
            }
        }

        private void SalvarEstadoDestinoEmpresaEmissora(ref Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora repositorioEstadoDestinoFilialEmissora = new Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora(unitOfWork);
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            dynamic dynEstadosDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EstadosDestino"));

            List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> estadosDestino = repositorioEstadoDestinoFilialEmissora.BuscarPorFilial(filial.Codigo);

            if (estadosDestino == null || estadosDestino.Count == 0)
            {
                estadosDestino = new List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>();
            }
            else
            {
                List<string> codigos = new List<string>();

                foreach (dynamic dynEstadoDestino in dynEstadosDestino)
                    codigos.Add((string)dynEstadoDestino.Codigo);

                List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> estadosDestinoDeletar = estadosDestino.Where(o => !codigos.Contains(o.Estado.Sigla)).ToList();

                foreach (Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora estadoDestinoDeletar in estadosDestinoDeletar)
                    repositorioEstadoDestinoFilialEmissora.Deletar(estadoDestinoDeletar);
            }

            foreach (var estadoDestino in dynEstadosDestino)
            {
                string codigo = string.Empty;
                int codigoEmpresa = 0;
                codigo = estadoDestino.Codigo;
                codigoEmpresa = estadoDestino.EmpresaCodigo;

                Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora empresaEmissoraSalva = repositorioEstadoDestinoFilialEmissora.BuscarPorSiglaEEmpresa(codigo, codigoEmpresa);


                if (empresaEmissoraSalva == null && estadosDestino.Any(e => e.Estado.Sigla.Equals(codigo)) && estadosDestino.Any(o => o.Empresa.Codigo == codigoEmpresa))
                    throw new ControllerException("Esse estado já está cadastrado na Filial.");

                if (empresaEmissoraSalva == null && estadosDestino.Any(e => e.Filial.Codigo.Equals(codigoEmpresa)))
                    throw new ControllerException("Não é possível cadastrar a mesma empresa da Filial.");


                if (empresaEmissoraSalva == null)
                {
                    Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora estadoDestinoAdicionar = new Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora();
                    Dominio.Entidades.Estado estado = repositorioEstado.BuscarPorSigla(codigo);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                    estadoDestinoAdicionar.Empresa = empresa;
                    estadoDestinoAdicionar.Estado = estado;
                    estadoDestinoAdicionar.Filial = filial;
                    repositorioEstadoDestinoFilialEmissora.Inserir(estadoDestinoAdicionar);

                }
            }


        }

        private void SalvarIntegracaoSemParar(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            dynamic semParar = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("IntegracaoSemParar"));
            if (semParar != null && !string.IsNullOrWhiteSpace((string)semParar.Usuario) && !string.IsNullOrWhiteSpace((string)semParar.Senha) && !string.IsNullOrWhiteSpace((string)semParar.CNPJ))
            {
                int.TryParse((string)semParar.DiasPrazo, out int diasPrazo);
                int.TryParse((string)semParar.Codigo, out int codigo);
                double.TryParse((string)semParar.FornecedorValePedagio, out double fornecedor);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar tipoRota;
                Enum.TryParse((string)semParar.TipoRota, out tipoRota);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = codigo > 0 ? repIntegracaoSemParar.BuscarPorCodigo(codigo, true) : null;
                if (integracaoSemParar == null)
                    integracaoSemParar = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar();

                integracaoSemParar.Usuario = (string)semParar.Usuario;
                integracaoSemParar.Senha = (string)semParar.Senha;
                integracaoSemParar.CNPJ = Utilidades.String.OnlyNumbers((string)semParar.CNPJ);
                integracaoSemParar.DiasPrazo = diasPrazo;
                integracaoSemParar.NomeRpt = (string)semParar.NomeRpt;
                integracaoSemParar.Observacao1 = (string)semParar.Observacao1;
                integracaoSemParar.Observacao2 = (string)semParar.Observacao2;
                integracaoSemParar.Observacao3 = (string)semParar.Observacao3;
                integracaoSemParar.Observacao4 = (string)semParar.Observacao4;
                integracaoSemParar.Observacao5 = (string)semParar.Observacao5;
                integracaoSemParar.Observacao6 = (string)semParar.Observacao6;
                integracaoSemParar.FornecedorValePedagio = fornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(fornecedor) : null;
                integracaoSemParar.TipoRota = tipoRota;
                integracaoSemParar.UtilizarModeoVeicularCarga = (bool)semParar.UtilizarModeoVeicularCarga;

                if (integracaoSemParar.Codigo > 0)
                    repIntegracaoSemParar.Atualizar(integracaoSemParar, Auditado);
                else
                    repIntegracaoSemParar.Inserir(integracaoSemParar, Auditado);

                filial.IntegracaoSemParar = integracaoSemParar;
            }
            else
            {
                if (filial.IntegracaoSemParar != null)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = filial.IntegracaoSemParar;
                    filial.IntegracaoSemParar = null;
                    repIntegracaoSemParar.Deletar(integracaoSemParar, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, string.Format(Localization.Resources.Filiais.Filial.RemoveuConfiguracoesIntegracao, "Sem Parar"), unidadeDeTrabalho);
                }
            }
        }

        private void ExcluirDescontosRemovidas(Dominio.Entidades.Embarcador.Filiais.Filial filial, dynamic descontos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (filial.Descontos != null)
            {
                Repositorio.Embarcador.Filiais.FilialDesconto repFilialDesconto = new Repositorio.Embarcador.Filiais.FilialDesconto(unidadeDeTrabalho);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var desconto in descontos)
                {
                    int? codigo = ((string)desconto.Codigo).ToNullableInt();

                    if (codigo.HasValue && codigo > 0)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Filiais.FilialDesconto> listaDescontosRemover = (from desc in filial.Descontos where !listaCodigosAtualizados.Contains(desc.Codigo) select desc).ToList();

                foreach (var desconto in listaDescontosRemover)
                {
                    repFilialDesconto.Deletar(desconto);
                }

                if (listaDescontosRemover.Count > 0)
                {
                    string descricaoAcao = listaDescontosRemover.Count == 1 ? Localization.Resources.Filiais.Filial.DescontoRemovido : Localization.Resources.Filiais.Filial.MultiplosDescontosRemovidos;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void PreencherDadosDesconto(Dominio.Entidades.Embarcador.Filiais.FilialDesconto filialDesconto, dynamic desconto, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(((string)desconto.CodigoModeloVeicular).ToInt()) ?? null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(((string)desconto.CodigoTipoOperacao).ToInt()) ?? null;

            filialDesconto.ValorDesconto = ((string)desconto.ValorDesconto).ToDecimal();
            filialDesconto.ModeloVeicularCarga = modeloVeicularCarga;
            filialDesconto.TipoOperacao = tipoOperacao;
        }

        private void SalvarDescontosAdicionadasOuAtualizadas(Dominio.Entidades.Embarcador.Filiais.Filial filial, dynamic descontos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Filiais.FilialDesconto repFilialDesconto = new Repositorio.Embarcador.Filiais.FilialDesconto(unidadeDeTrabalho);
            int totalRegistrosAdicionados = 0;
            int totalRegistrosAtualizados = 0;

            foreach (var desconto in descontos)
            {
                Dominio.Entidades.Embarcador.Filiais.FilialDesconto filialDesconto;
                int codigo = desconto.Codigo;

                if (codigo > 0)
                    filialDesconto = repFilialDesconto.BuscarPorCodigo(codigo, auditavel: true) ?? throw new Dominio.Excecoes.Embarcador.ControllerException(Localization.Resources.Filiais.Filial.DescontoNaoEncontrado);
                else
                    filialDesconto = new Dominio.Entidades.Embarcador.Filiais.FilialDesconto { Filial = filial };

                PreencherDadosDesconto(filialDesconto, desconto, unidadeDeTrabalho);

                if (codigo > 0)
                {
                    totalRegistrosAtualizados += filialDesconto.GetChanges().Count > 0 ? 1 : 0;
                    repFilialDesconto.Atualizar(filialDesconto);
                }
                else
                {
                    totalRegistrosAdicionados += 1;
                    repFilialDesconto.Inserir(filialDesconto);
                }
            }

            if (filial.IsInitialized())
            {
                if (totalRegistrosAtualizados > 0)
                {
                    string descricaoAcao = totalRegistrosAtualizados == 1 ? Localization.Resources.Filiais.Filial.DescontoAtualizado : Localization.Resources.Filiais.Filial.MultiplosDescontosAtualizados;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }

                if (totalRegistrosAdicionados > 0)
                {
                    string descricaoAcao = totalRegistrosAdicionados == 1 ? Localization.Resources.Filiais.Filial.DescontoAdicionado : Localization.Resources.Filiais.Filial.MultiplosDescontosAdicionados;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void AtualizarDescontos(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic descontos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Descontos"));

            ExcluirDescontosRemovidas(filial, descontos, unidadeDeTrabalho);
            SalvarDescontosAdicionadasOuAtualizadas(filial, descontos, unidadeDeTrabalho);
        }

        private void ExcluirDescontosExcecaoRemovidas(Dominio.Entidades.Embarcador.Filiais.Filial filial, dynamic descontosExcecao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (filial.DescontosExcecao != null)
            {
                Repositorio.Embarcador.Filiais.FilialDescontoExcecao repFilialDescontoExcecao = new Repositorio.Embarcador.Filiais.FilialDescontoExcecao(unidadeDeTrabalho);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var descontoExcecao in descontosExcecao)
                {
                    int? codigo = ((string)descontoExcecao.Codigo).ToNullableInt();

                    if (codigo.HasValue && codigo > 0)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Filiais.FilialDescontoExcecao> listaDescontosExcecaoRemover = (from desc in filial.DescontosExcecao where !listaCodigosAtualizados.Contains(desc.Codigo) select desc).ToList();

                foreach (var descontoExcecao in listaDescontosExcecaoRemover)
                    repFilialDescontoExcecao.Deletar(descontoExcecao);

                if (listaDescontosExcecaoRemover.Count > 0)
                {
                    string descricaoAcao = listaDescontosExcecaoRemover.Count == 1 ? Localization.Resources.Filiais.Filial.ExcecaoRemovida : Localization.Resources.Filiais.Filial.MultiplasExcecoesRemovidas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void PreencherDadosDescontoExcecao(Dominio.Entidades.Embarcador.Filiais.FilialDescontoExcecao filialDescontoExcecao, dynamic descontoExcecao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(((string)descontoExcecao.CodigoModeloVeicular).ToInt()) ?? null;
            Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo(((string)descontoExcecao.CodigoTransportador).ToInt()) ?? null;
            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repProduto.BuscarPorCodigo(((string)descontoExcecao.CodigoProduto).ToInt()) ?? null;

            filialDescontoExcecao.ModeloVeicularCarga = modeloVeicularCarga;
            filialDescontoExcecao.Transportador = transportador;
            filialDescontoExcecao.Produto = produto;
            filialDescontoExcecao.HoraInicio = (string)descontoExcecao.HoraInicio;
            filialDescontoExcecao.HoraFim = (string)descontoExcecao.HoraFim;
        }

        private void SalvarDescontosExcecaoAdicionadasOuAtualizadas(Dominio.Entidades.Embarcador.Filiais.Filial filial, dynamic descontosExcecao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Filiais.FilialDescontoExcecao repFilialDescontoExcecao = new Repositorio.Embarcador.Filiais.FilialDescontoExcecao(unidadeDeTrabalho);
            int totalRegistrosAdicionados = 0;
            int totalRegistrosAtualizados = 0;

            foreach (var descontoExcecao in descontosExcecao)
            {
                Dominio.Entidades.Embarcador.Filiais.FilialDescontoExcecao filialDescontoExcecao;
                int codigo = descontoExcecao.Codigo;

                if (codigo > 0)
                    filialDescontoExcecao = repFilialDescontoExcecao.BuscarPorCodigo(codigo, auditavel: true) ?? throw new Dominio.Excecoes.Embarcador.ControllerException(Localization.Resources.Filiais.Filial.ExcecaoNaoEncontrada);
                else
                    filialDescontoExcecao = new Dominio.Entidades.Embarcador.Filiais.FilialDescontoExcecao { Filial = filial };

                PreencherDadosDescontoExcecao(filialDescontoExcecao, descontoExcecao, unidadeDeTrabalho);

                if (codigo > 0)
                {
                    totalRegistrosAtualizados += filialDescontoExcecao.GetChanges().Count > 0 ? 1 : 0;
                    repFilialDescontoExcecao.Atualizar(filialDescontoExcecao);
                }
                else
                {
                    totalRegistrosAdicionados += 1;
                    repFilialDescontoExcecao.Inserir(filialDescontoExcecao);
                }
            }

            if (filial.IsInitialized())
            {
                if (totalRegistrosAtualizados > 0)
                {
                    string descricaoAcao = totalRegistrosAtualizados == 1 ? Localization.Resources.Filiais.Filial.ExcecaoAtualizada : Localization.Resources.Filiais.Filial.MultiplasExcecoesAtualizadas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }

                if (totalRegistrosAdicionados > 0)
                {
                    string descricaoAcao = totalRegistrosAdicionados == 1 ? Localization.Resources.Filiais.Filial.ExcecaoAdicionada : Localization.Resources.Filiais.Filial.MultiplasExcecoesAdicionadas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void AtualizarDescontosExcecao(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic descontosExcecao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("DescontosExcecao"));

            ExcluirDescontosExcecaoRemovidas(filial, descontosExcecao, unidadeDeTrabalho);
            SalvarDescontosExcecaoAdicionadasOuAtualizadas(filial, descontosExcecao, unidadeDeTrabalho);
        }

        private void SalvarIntegracaoTarget(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTarget repIntegracaoTarget = new Repositorio.Embarcador.Configuracoes.IntegracaoTarget(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            dynamic target = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("IntegracaoTarget"));
            if (target != null && !string.IsNullOrWhiteSpace((string)target.Usuario) && !string.IsNullOrWhiteSpace((string)target.Senha))
            {
                int.TryParse((string)target.DiasPrazo, out int diasPrazo);
                int.TryParse((string)target.Codigo, out int codigo);
                int.TryParse((string)target.CodigoCentroCusto, out int codigoCentroCusto);

                double.TryParse((string)target.FornecedorValePedagio, out double fornecedor);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget integracaoTarget = codigo > 0 ? repIntegracaoTarget.BuscarPorCodigo(codigo, true) : null;
                if (integracaoTarget == null)
                    integracaoTarget = new Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget();

                integracaoTarget.Usuario = (string)target.Usuario;
                integracaoTarget.Senha = (string)target.Senha;
                integracaoTarget.Token = (string)target.Token;
                integracaoTarget.DiasPrazo = diasPrazo;
                integracaoTarget.CodigoCentroCusto = codigoCentroCusto;
                integracaoTarget.FornecedorValePedagio = fornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(fornecedor) : null;
                integracaoTarget.CadastrarRotaPorIBGE = (bool)target.CadastrarRotaPorIBGE;
                integracaoTarget.CadastrarRotaPorCoordenadas = (bool)target.CadastrarRotaPorCoordenadas;
                integracaoTarget.NaoBuscarCartaoMotoristaTarget = (bool)target.NaoBuscarCartaoMotoristaTarget;

                if (integracaoTarget.Codigo > 0)
                    repIntegracaoTarget.Atualizar(integracaoTarget, Auditado);
                else
                    repIntegracaoTarget.Inserir(integracaoTarget, Auditado);

                filial.IntegracaoTarget = integracaoTarget;
            }
            else
            {
                if (filial.IntegracaoTarget != null)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget integracaoTarget = filial.IntegracaoTarget;
                    filial.IntegracaoTarget = null;
                    repIntegracaoTarget.Deletar(integracaoTarget, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, string.Format(Localization.Resources.Filiais.Filial.RemoveuConfiguracoesIntegracao, "Target"), unidadeDeTrabalho);
                }
            }
        }

        private void SalvarBalancas(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Filiais.FilialBalanca repFilialBalanca = new Repositorio.Embarcador.Filiais.FilialBalanca(unidadeDeTrabalho);

            dynamic dynBalancas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Balancas"));

            List<Dominio.Entidades.Embarcador.Filiais.FilialBalanca> balancas = repFilialBalanca.BuscarPorFilial(filial.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (balancas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic dynBalanca in dynBalancas)
                    if (dynBalanca.Codigo != null)
                        codigos.Add((int)dynBalanca.Codigo);

                List<Dominio.Entidades.Embarcador.Filiais.FilialBalanca> balancasDeletar = (from obj in balancas where !codigos.Contains(obj.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Filiais.FilialBalanca balancaDeletar in balancasDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Balancas",
                        De = $"{balancaDeletar.MarcaBalanca} - {balancaDeletar.ModeloBalanca}",
                        Para = ""
                    });

                    repFilialBalanca.Deletar(balancaDeletar);
                }
            }

            foreach (dynamic dynBalanca in dynBalancas)
            {
                int codigoFilialBalanca = ((string)dynBalanca.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Filiais.FilialBalanca filialBalanca = codigoFilialBalanca > 0 ? balancas.Where(o => o.Codigo == codigoFilialBalanca).FirstOrDefault() : null;

                if (filialBalanca == null)
                {
                    filialBalanca = new Dominio.Entidades.Embarcador.Filiais.FilialBalanca();
                    filialBalanca.Filial = filial;
                }
                else
                    filialBalanca.Initialize();

                filialBalanca.MarcaBalanca = (string)dynBalanca.MarcaBalanca;
                filialBalanca.ModeloBalanca = (string)dynBalanca.ModeloBalanca;
                filialBalanca.HostConsultaBalanca = (string)dynBalanca.HostConsultaBalanca;
                filialBalanca.PortaBalanca = ((string)dynBalanca.PortaBalanca).ToInt();
                filialBalanca.TamanhoRetornoBalanca = ((string)dynBalanca.TamanhoRetornoBalanca).ToInt();
                filialBalanca.PosicaoInicioPesoBalanca = ((string)dynBalanca.PosicaoInicioPesoBalanca).ToInt();
                filialBalanca.PosicaoFimPesoBalanca = ((string)dynBalanca.PosicaoFimPesoBalanca).ToInt();
                filialBalanca.CasasDecimaisPesoBalanca = ((string)dynBalanca.CasasDecimaisPesoBalanca).ToInt();
                filialBalanca.QuantidadeConsultasPesoBalanca = ((string)dynBalanca.QuantidadeConsultasPesoBalanca).ToInt();
                filialBalanca.PercentualToleranciaPesoBalanca = ((string)dynBalanca.PercentualToleranciaPesoBalanca).ToDecimal();
                filialBalanca.PercentualToleranciaPesagemEntrada = ((string)dynBalanca.PercentualToleranciaPesagemEntrada).ToDecimal();
                filialBalanca.PercentualToleranciaPesagemSaida = ((string)dynBalanca.PercentualToleranciaPesagemSaida).ToDecimal();

                if (filialBalanca.PosicaoInicioPesoBalanca > filialBalanca.PosicaoFimPesoBalanca)
                    throw new ControllerException("Balança - Posição inicio não pode ser maior que a de fim");

                if (filialBalanca.PosicaoFimPesoBalanca > filialBalanca.TamanhoRetornoBalanca && filialBalanca.TamanhoRetornoBalanca > 0)
                    throw new ControllerException("Balança - Posição fim não pode ser maior que o tamanho do retorno");

                if (filialBalanca.QuantidadeConsultasPesoBalanca > 1 && filialBalanca.PercentualToleranciaPesoBalanca == 0)
                    throw new ControllerException("Balança - A tolerância de peso está zerada, quando a quantidade de consultas for maior que 1, é obrigatório informar");

                if (filialBalanca.Codigo > 0)
                {
                    repFilialBalanca.Atualizar(filialBalanca);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoesBalanca = filialBalanca.GetChanges();
                    if (alteracoesBalanca.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, alteracoesBalanca, $"Alterou dados da balança {filialBalanca.MarcaBalanca} - {filialBalanca.ModeloBalanca} da filial {filial.Descricao}", unidadeDeTrabalho);
                }
                else
                {
                    repFilialBalanca.Inserir(filialBalanca);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Balancas",
                        De = "",
                        Para = $"{filialBalanca.MarcaBalanca} - {filialBalanca.ModeloBalanca}"
                    });
                }
            }

            filial.SetExternalChanges(alteracoes);
        }

        private Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialBalanca ObterFiltrosPesquisaFilialBalanca()
        {
            Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialBalanca filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialBalanca()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                Descricao = Request.GetStringParam("Descricao")
            };

            return filtrosPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialArmazem ObterFiltrosPesquisaFilialArmazem()
        {
            Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialArmazem filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialArmazem()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigosFiliais = Request.GetListParam<int>("Filiais"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Descricao = Request.GetStringParam("Descricao")
            };

            return filtrosPesquisa;
        }

        private void SalvarTiposOperacaoTrizy(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);

            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoOperacao"));

            if (filial.TipoOperacoesTrizy == null)
            {
                filial.TipoOperacoesTrizy = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            }
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoOperacao in tiposOperacao)
                    codigos.Add((int)tipoOperacao.TiposOperacao.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposDeletar = filial.TipoOperacoesTrizy.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoDeletar in tiposDeletar)
                    filial.TipoOperacoesTrizy.Remove(tipoOperacaoDeletar);
            }

            foreach (var tipoOperacao in tiposOperacao)
                if (!filial.TipoOperacoesTrizy.Any(o => o.Codigo == (int)tipoOperacao.TiposOperacao.Codigo))
                    filial.TipoOperacoesTrizy.Add(repTipoOperacao.BuscarPorCodigo((int)tipoOperacao.TiposOperacao.Codigo));
        }

        private async Task SalvarTiposIntegracaoAsync(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Filiais.FilialTipoIntegracao repFilialTipoIntegracao = new(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new(unitOfWork);

            List<TipoIntegracao> tiposIntegraoes = Request.GetListEnumParam<TipoIntegracao>("TipoIntegracao");

            List<Dominio.Entidades.Embarcador.Filiais.FilialTipoIntegracao> filiaisTiposIntegracao = await repFilialTipoIntegracao.BuscarPorFilialAsync(filial.Codigo, cancellationToken);

            List<Dominio.Entidades.Embarcador.Filiais.FilialTipoIntegracao> filiaisTipoIntegracaoDeletar = [.. filiaisTiposIntegracao.Where(obj => !tiposIntegraoes.Contains(obj.TipoIntegracao.Tipo))];
            if (filiaisTipoIntegracaoDeletar.Count > 0)
            {
                repFilialTipoIntegracao.Deletar(filiaisTipoIntegracaoDeletar);

                IEnumerable<TipoIntegracao> tiposIntegracoesDeletar = filiaisTipoIntegracaoDeletar.Select(x => x.TipoIntegracao.Tipo);
                string descricoesTiposIntegracoesDeletar = string.Join(", ", tiposIntegracoesDeletar.Select(x => x.ObterDescricao()));
                Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, string.Format(Localization.Resources.Filiais.Filial.ExcluiuIntegracoes, descricoesTiposIntegracoesDeletar), unitOfWork);
            }

            IEnumerable<Dominio.Entidades.Embarcador.Filiais.FilialTipoIntegracao> filiaisTiposIntegracaoQueRestaram = filiaisTiposIntegracao.Except(filiaisTipoIntegracaoDeletar);
            IEnumerable<TipoIntegracao> enumsTipoIntegracaoInserir = tiposIntegraoes.Where(enumTipoIntegracao => !filiaisTiposIntegracaoQueRestaram.Select(x => x.TipoIntegracao.Tipo).Contains(enumTipoIntegracao));
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracoesInserir = repTipoIntegracao.BuscarPorTipos([.. enumsTipoIntegracaoInserir]);

            List<Dominio.Entidades.Embarcador.Filiais.FilialTipoIntegracao> filiaisTiposIntegracoesAdicionar = tiposIntegracoesInserir
                .Select(integracao => new Dominio.Entidades.Embarcador.Filiais.FilialTipoIntegracao
                {
                    Filial = filial,
                    TipoIntegracao = integracao
                })
                .ToList();

            if (filiaisTiposIntegracoesAdicionar.Count > 0)
            {
                repFilialTipoIntegracao.Inserir(filiaisTiposIntegracoesAdicionar);

                IEnumerable<TipoIntegracao> tiposIntegracoesAdicionar = filiaisTiposIntegracoesAdicionar.Select(x => x.TipoIntegracao.Tipo);
                string descricoesTiposIntegracoesAdicionar = string.Join(", ", tiposIntegracoesAdicionar.Select(x => x.ObterDescricao()));
                Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, string.Format(Localization.Resources.Filiais.Filial.AdicionouIntegracoes, descricoesTiposIntegracoesAdicionar), unitOfWork);
            }
        }

        #endregion

        #region Métodos Privados - Modelos Veiculares de Carga

        private void AtualizarModelosVeicularesCarga(Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.FilialModeloVeicularCarga repositorioFilialModeloVeicularCarga = new Repositorio.Embarcador.Filiais.FilialModeloVeicularCarga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga> modelosVeicularesCarga = repositorioFilialModeloVeicularCarga.BuscarPorFilial(filial.Codigo);
            dynamic modelosVeicularesCargaAdicionadosOuAtualizados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaModeloVeicularCarga"));

            ExcluirModelosVeicularesCargaRemovidos(filial, modelosVeicularesCarga, modelosVeicularesCargaAdicionadosOuAtualizados, historico, unitOfWork);
            SalvarModelosVeicularesCargaAdicionadosOuAtualizados(filial, modelosVeicularesCarga, modelosVeicularesCargaAdicionadosOuAtualizados, historico, unitOfWork);
        }

        private void ExcluirModelosVeicularesCargaRemovidos(Dominio.Entidades.Embarcador.Filiais.Filial filial, List<Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga> modelosVeicularesCarga, dynamic modelosVeicularesCargaAdicionadosOuAtualizados, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            if ((modelosVeicularesCarga == null) || (modelosVeicularesCarga.Count == 0))
                return;

            Repositorio.Embarcador.Filiais.FilialModeloVeicularCarga repositorioFilialModeloVeicularCarga = new Repositorio.Embarcador.Filiais.FilialModeloVeicularCarga(unitOfWork);
            List<int> listaCodigosAtualizados = new List<int>();

            foreach (var modeloVeicularCarga in modelosVeicularesCargaAdicionadosOuAtualizados)
            {
                int? codigo = ((string)modeloVeicularCarga.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    listaCodigosAtualizados.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga> listaModeloVeicularCargaRemover = (from o in modelosVeicularesCarga where !listaCodigosAtualizados.Contains(o.Codigo) select o).ToList();

            foreach (Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga modeloVeicularCarga in listaModeloVeicularCargaRemover)
                repositorioFilialModeloVeicularCarga.Deletar(modeloVeicularCarga, (historico != null ? Auditado : null), historico);
        }

        private void SalvarModelosVeicularesCargaAdicionadosOuAtualizados(Dominio.Entidades.Embarcador.Filiais.Filial filial, List<Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga> modelosVeicularesCarga, dynamic modelosVeicularesCargaAdicionadosOuAtualizados, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.FilialModeloVeicularCarga repositorioFilialModeloVeicularCarga = new Repositorio.Embarcador.Filiais.FilialModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga> modelosVeicularesCargaCadastradosOuAtualizados = new List<Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga>();

            foreach (var modeloVeicularCarga in modelosVeicularesCargaAdicionadosOuAtualizados)
            {
                Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga modeloVeicularCargaSalvar;
                int? codigo = ((string)modeloVeicularCarga.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    modeloVeicularCargaSalvar = repositorioFilialModeloVeicularCarga.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException(Localization.Resources.Filiais.Filial.ConfiguracaoModeloVeicularCargaNaoEncontrada);
                else
                    modeloVeicularCargaSalvar = new Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga() { Filial = filial };

                modeloVeicularCargaSalvar.ModeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(((string)modeloVeicularCarga.ModeloVeicularCarga).ToInt()) ?? throw new ControllerException(Localization.Resources.Filiais.Filial.ModeloVeicularCargaNaoEncontrado);
                modeloVeicularCargaSalvar.IntegrarOrdemEmbarque = ((string)modeloVeicularCarga.IntegrarOrdemEmbarque).ToBool();

                ValidarDadosModelosVeicularesCargaDuplicado(modelosVeicularesCargaCadastradosOuAtualizados, modeloVeicularCargaSalvar);

                modelosVeicularesCargaCadastradosOuAtualizados.Add(modeloVeicularCargaSalvar);

                if (codigo.HasValue)
                    repositorioFilialModeloVeicularCarga.Atualizar(modeloVeicularCargaSalvar, (historico != null ? Auditado : null), historico);
                else
                    repositorioFilialModeloVeicularCarga.Inserir(modeloVeicularCargaSalvar, (historico != null ? Auditado : null), historico);
            }
        }

        private void ValidarDadosModelosVeicularesCargaDuplicado(List<Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga> modelosVeicularesCargaCadastradosOuAtualizados, Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga modeloVeicularCargaSalvar)
        {
            Dominio.Entidades.Embarcador.Filiais.FilialModeloVeicularCarga modeloVeicularCargaDuplicado = (
                from o in modelosVeicularesCargaCadastradosOuAtualizados
                where o.ModeloVeicularCarga.Codigo == modeloVeicularCargaSalvar.ModeloVeicularCarga.Codigo
                select o
            ).FirstOrDefault();

            if (modeloVeicularCargaDuplicado != null)
                throw new ControllerException(string.Format(Localization.Resources.Filiais.Filial.ModeloVeicularCargaDuplicado, modeloVeicularCargaDuplicado.ModeloVeicularCarga.Descricao));
        }

        #endregion

        #region Métodos Privados - Alertas SLA

        private void SalvarAlertasSla(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.GestaoPatioAlertaSla repositorioGestaoPatioAlertaSla = new Repositorio.Embarcador.Filiais.GestaoPatioAlertaSla(unitOfWork);

            List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> gestaoPatioAlertasSla = repositorioGestaoPatioAlertaSla.BuscarPorFilial(filial.Codigo);

            dynamic dynGestaoPatioAlertasSla = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAlertasSln"));

            List<int> codigosNaoRemovidos = new List<int>();

            int codigo = 0;

            foreach (dynamic alerta in dynGestaoPatioAlertasSla)
            {
                codigo = ((string)alerta.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla alertaSla = null;

                if (codigo > 0)
                {
                    codigosNaoRemovidos.Add(codigo);
                    alertaSla = gestaoPatioAlertasSla.Where(obj => obj.Codigo == codigo).FirstOrDefault();
                }

                alertaSla = alertaSla ?? new Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla();

                alertaSla.NomeAlerta = ((string)alerta.NomeAlerta).ToString();
                alertaSla.TempoExcedido = ((string)alerta.TempoExcedido).ToInt();
                alertaSla.TempoExcedidoTransportador = ((string)alerta.TempoExcedidoTransportador).ToInt();
                alertaSla.TempoFaltante = ((string)alerta.TempoFaltante).ToInt();
                alertaSla.TempoFaltanteTransportador = ((string)alerta.TempoFaltanteTransportador).ToInt();
                alertaSla.CorAlertaTempoExcedido = ((string)alerta.CorAlertaTempoExcedido).ToString();
                alertaSla.CorAlertaTempoFaltante = ((string)alerta.CorAlertaTempoFaltante).ToString();
                alertaSla.Filial = filial;
                alertaSla.Emails = ((string)alerta.Emails).ToString();
                alertaSla.AlertarTransportadorPorEmail = ((string)alerta.AlertarTransportadorPorEmail).ToBool();
                alertaSla.Etapas = alertaSla.Etapas ?? new List<EtapaFluxoGestaoPatio>();
                alertaSla.TiposAlertaEmail = alertaSla.TiposAlertaEmail ?? new List<TipoAlertaSlnEmail>();
                alertaSla.Etapas.Clear();
                alertaSla.TiposAlertaEmail.Clear();

                foreach (dynamic etapa in alerta.Etapas)
                    alertaSla.Etapas.Add(((string)etapa).ToEnum<EtapaFluxoGestaoPatio>());

                foreach (var tipoAlertaSln in alerta.AlertasEnviarEmail)
                    alertaSla.TiposAlertaEmail.Add(((string)tipoAlertaSln).ToEnum<TipoAlertaSlnEmail>());

                if (alertaSla.Codigo > 0)
                    repositorioGestaoPatioAlertaSla.Atualizar(alertaSla);
                else
                    repositorioGestaoPatioAlertaSla.Inserir(alertaSla);
            }

            ExcluirAlertasSla(gestaoPatioAlertasSla.Where(obj => !codigosNaoRemovidos.Contains(obj.Codigo)).ToList(), unitOfWork);
        }

        private void ExcluirAlertasSla(List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> gestaoPatioAlertasSlaRemover, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.GestaoPatioAlertaSla repositorioGestaoPatioAlertaSla = new Repositorio.Embarcador.Filiais.GestaoPatioAlertaSla(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla gestaoPatioAlerta in gestaoPatioAlertasSlaRemover)
            {
                gestaoPatioAlerta.Etapas.Clear();
                gestaoPatioAlerta.TiposAlertaEmail.Clear();
                repositorioGestaoPatioAlertaSla.Deletar(gestaoPatioAlerta);
            }
        }

        #endregion

        #region Método Privado - Tanques

        private void ExcluirTanquesRemovidos(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic dynamicTanquesRemover = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TanquesRemover"));

            if (dynamicTanquesRemover != null)
            {
                List<int> CodigosTanquesRemover = new List<int>();
                foreach (var dynBalanca in dynamicTanquesRemover)
                    CodigosTanquesRemover.Add((int)dynBalanca);

                Repositorio.Embarcador.Tanques.FilialTanque repositorioFilialTanque = new Repositorio.Embarcador.Tanques.FilialTanque(unidadeDeTrabalho);
                Repositorio.Embarcador.Tanques.Tanque repositorioTanque = new Repositorio.Embarcador.Tanques.Tanque(unidadeDeTrabalho);
                List<Dominio.Entidades.Embarcador.Filiais.FilialTanque> ListaFilialTanque = repositorioFilialTanque.BuscarPorFilial(filial.Codigo);

                List<Dominio.Entidades.Embarcador.Filiais.FilialTanque> ListaFilialTanqueRemover = (from o in ListaFilialTanque where CodigosTanquesRemover.Contains(o.Codigo) select o).ToList();

                foreach (var filialTanqueRemover in ListaFilialTanqueRemover)
                {
                    repositorioFilialTanque.Deletar(filialTanqueRemover);
                }
            }
        }

        private void AtualizarTanques(Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic dynamicTanquesAdicionar = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TanquesAdicionar"));

            if (dynamicTanquesAdicionar != null)
            {
                List<int> CodigosTanquesAdicionar = new List<int>();
                foreach (var dynCodigo in dynamicTanquesAdicionar)
                {
                    CodigosTanquesAdicionar.Add((int)dynCodigo.Codigo);
                    Repositorio.Embarcador.Tanques.FilialTanque repositorioFilialTanque = new Repositorio.Embarcador.Tanques.FilialTanque(unidadeDeTrabalho);
                    Repositorio.Embarcador.Tanques.Tanque repositorioTanque = new Repositorio.Embarcador.Tanques.Tanque(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Filiais.FilialTanque tanque = repositorioFilialTanque.BuscarPorCodigo((int)dynCodigo.Codigo, false);

                    tanque.Capacidade = (decimal)dynCodigo.Capacidade;

                    repositorioFilialTanque.Atualizar(tanque);


                }
            }
        }

        #endregion
    }
}
