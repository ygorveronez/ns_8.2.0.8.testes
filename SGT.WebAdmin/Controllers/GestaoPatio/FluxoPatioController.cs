using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using Servicos.Extensions;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize(new string[] { "ObterCapacidadePorDoca", "ObterEtapasDisponiveis", "ObterMultiplasEtapasDisponiveis", "ConfiguracoesGestaoPatio", "ObterFluxoPatio", "BuscarPorCodigo", "ObterDetalhesFluxoPatio", "DefinirEtapaComoVisualizada" }, "GestaoPatio/FluxoPatio")]
    public class FluxoPatioController : BaseController
    {
        #region Construtores

        public FluxoPatioController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> ObterEtapasDisponiveis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilial = Request.GetIntParam("Filial");
                TipoFluxoGestaoPatio? tipo = Request.GetNullableEnumParam<TipoFluxoGestaoPatio>("Tipo");
                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repositorioConfiguracaoGestaoPatio.BuscarConfiguracao();
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork, configuracaoGestaoPatio);
                List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada> descricoesEtapas = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricoesEtapasAgrupadas(new List<int>() { codigoFilial }, tipo);

                foreach (Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada descricaoEtapa in descricoesEtapas)
                {
                    if (descricaoEtapa.Descricao.Length > 60)
                        descricaoEtapa.Descricao = $"{descricaoEtapa.Descricao.Left(57)}...";
                }

                return new JsonpResult(descricoesEtapas);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.GestaoPatio.FluxoPatio.OcorreuUmaFalhaAoBuscarFluxoDePatio);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterMultiplasEtapasDisponiveis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosFilial = Request.GetListParam<int>("Filiais");
                TipoFluxoGestaoPatio? tipo = Request.GetNullableEnumParam<TipoFluxoGestaoPatio>("Tipo");
                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repositorioConfiguracaoGestaoPatio.BuscarConfiguracao();
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork, configuracaoGestaoPatio);
                List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada> descricoesEtapas = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricoesEtapasAgrupadas(codigosFilial, tipo);

                foreach (Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada descricaoEtapa in descricoesEtapas)
                {
                    if (descricaoEtapa.Descricao.Length > 60)
                        descricaoEtapa.Descricao = $"{descricaoEtapa.Descricao.Left(57)}...";
                }

                return new JsonpResult(descricoesEtapas);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.GestaoPatio.FluxoPatio.OcorreuUmaFalhaAoBuscarFluxoDePatio);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConfirmarMensagemAlerta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio servicoMensagemAlerta = new Servicos.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio(unitOfWork, Auditado);

                servicoMensagemAlerta.ConfirmarPorCodigo(codigo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a confirmação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarObservacoesEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                EtapaFluxoGestaoPatio etapa = Request.GetEnumParam<EtapaFluxoGestaoPatio>("Etapa");
                string observacao = Request.GetStringParam("ObservacoesEtapa");
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                servicoFluxoGestaoPatio.SalvarObservacaoPorEtapa(codigoFluxoGestaoPatio, etapa, observacao);

                return new JsonpResult(true, Localization.Resources.GestaoPatio.FluxoPatio.ObservacaoAtualizadaComSucesso);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.GestaoPatio.FluxoPatio.OcorreuUmaFalhaAoSalvarObservacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarObservacoesEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                EtapaFluxoGestaoPatio etapa = Request.GetEnumParam<EtapaFluxoGestaoPatio>("Etapa");
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                string observacoes = servicoFluxoGestaoPatio.ObterObservacaoPorEtapa(codigoFluxoGestaoPatio, etapa);

                return new JsonpResult(new
                {
                    ObservacoesEtapa = observacoes
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.GestaoPatio.FluxoPatio.OcorreuUmaFalhaAoBuscarObservacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarEquipamentoFluxoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

                int codigoFluxoGestaoPatio = Request.GetIntParam("CodigoFluxoPatio");
                int codigoEquipamento = Request.GetIntParam("Equipamento");

                bool equipamentoEmUso = repFluxoGestaoPatio.ExisteEquipamentoEmUsoFluxoPatio(codigoEquipamento);

                if (equipamentoEmUso)
                    return new JsonpResult(false, true, "O equipamento já está sendo utilizado em outro fluxo de patio");

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoGestaoPatio);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                servicoFluxoGestaoPatio.InserirEquipamentoFluxoPatio(codigoEquipamento, fluxoGestaoPatio);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao informar o equipamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarEquipamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoGestaoPatio);

                if (fluxoGestaoPatio == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (fluxoGestaoPatio.Equipamento == null)
                {
                    Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);
                    Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPrimeiroPorCarga(fluxoGestaoPatio.Carga.Codigo);

                    if (filaCarregamentoVeiculo != null && filaCarregamentoVeiculo.Equipamento != null)
                        servicoFluxoGestaoPatio.InserirEquipamentoFluxoPatio(filaCarregamentoVeiculo.Equipamento.Codigo, fluxoGestaoPatio);
                }

                dynamic retorno =
                        new
                        {
                            fluxoGestaoPatio.Codigo,
                            Equipamento = new
                            {
                                Codigo = fluxoGestaoPatio.Equipamento?.Codigo,
                                Descricao = fluxoGestaoPatio.Equipamento != null ? $"{fluxoGestaoPatio.Equipamento?.Descricao} - {fluxoGestaoPatio.Equipamento?.Numero}" : ""
                            }
                        };
                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> BuscarObservacoesEtapas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas repositorioFluxoGestaoPatioEtapas = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas(unitOfWork);
                List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapas = repositorioFluxoGestaoPatioEtapas.BuscarPorGestao(codigoFluxoGestaoPatio);

                if (etapas.Count == 0)
                    return new JsonpResult(false, Localization.Resources.GestaoPatio.FluxoPatio.NaoFoiPossivelEncontrarAsObservacoesDasEtapas);

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = etapas.FirstOrDefault().FluxoGestaoPatio;
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada> descricoesEtapas = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricoesEtapas(fluxoGestaoPatio);
                List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> etapasComObservacoes = (from o in etapas where !string.IsNullOrWhiteSpace(o.Observacao) select o).ToList();

                dynamic etapasRetornar = (
                    from etapa in etapasComObservacoes
                    select new
                    {
                        etapa.Codigo,
                        etapa.Observacao,
                        Etapa = (from o in descricoesEtapas where o.Enumerador == etapa.EtapaFluxoGestaoPatio select o).FirstOrDefault()?.Descricao ?? ""
                    }
                ).ToList();

                return new JsonpResult(etapasRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.GestaoPatio.FluxoPatio.OcorreuUmaFalhaAoBuscarObservacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarFluxoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FluxoGestaoPatio_PermiteCancelarFluxo) && !(this.Usuario?.UsuarioAdministrador ?? false))
                    throw new ControllerException(Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

                int codigoFluxo = Request.GetIntParam("CodigoFluxo");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCodigo(codigoFluxo, false);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioDadosReiniciar fluxoGestaoPatioDadosReiniciar = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioDadosReiniciar()
                {
                    FluxoGestaoPatio = fluxoGestaoPatio,
                    Motivo = Request.GetStringParam("Motivo"),
                    RemoverVeiculoFilaCarregamento = Request.GetBoolParam("RemoverVeiculoFilaCarregamento"),
                    CodigoMotivoRetiradaFilaCarregamento = Request.GetIntParam("MotivoRetiradaFilaCarregamento")
                };

                servicoFluxoGestaoPatio.Reiniciar(fluxoGestaoPatioDadosReiniciar, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, true, Localization.Resources.GestaoPatio.FluxoPatio.FluxoFoiCanceladoComSucesso);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.GestaoPatio.FluxoPatio.OcorreuUmaFalhaAoCancelarFluxoDePatio);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarCancelamentoFluxoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPatio.FluxoPatioCancelamento repositorioFluxoPatioCancelamento = new Repositorio.Embarcador.GestaoPatio.FluxoPatioCancelamento(unitOfWork);

                int codigoFluxo = Request.GetIntParam("CodigoFluxo");

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioCancelamento fluxoPatioCancelamento = codigoFluxo > 0 ? repositorioFluxoPatioCancelamento.BuscarPorFluxoPatio(codigoFluxo) : null;

                if (fluxoPatioCancelamento == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                return new JsonpResult(new
                {
                    CodigoCancelamento = fluxoPatioCancelamento.Codigo,
                    fluxoPatioCancelamento.Motivo,
                    fluxoPatioCancelamento.RemoverVeiculoFilaCarregamento,
                    MotivoRetiradaFilaCarregamento = fluxoPatioCancelamento.MotivoRetiradaFilaCarregamento != null ? new
                    {
                        fluxoPatioCancelamento.MotivoRetiradaFilaCarregamento.Descricao,
                        fluxoPatioCancelamento.MotivoRetiradaFilaCarregamento.Codigo
                    } : null
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

        public async Task<IActionResult> ConfiguracoesGestaoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao servicoFluxoGestaoPatioConfiguracao = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracao = servicoFluxoGestaoPatioConfiguracao.ObterConfiguracao();
                bool permiteRemoverVeiculoFilaCarregamentoAoCancelarFluxoPatio = false;

                if (configuracao.PermiteCancelarFluxoPatioAtual)
                    permiteRemoverVeiculoFilaCarregamentoAoCancelarFluxoPatio = new Repositorio.Embarcador.Logistica.MotivoRetiradaFilaCarregamento(unitOfWork).ExisteAtivo();

                return new JsonpResult(new
                {
                    configuracao.InformarDocaCarregamentoDescricao,
                    configuracao.MontagemCargaDescricao,
                    configuracao.ChegadaVeiculoDescricao,
                    configuracao.GuaritaEntradaDescricao,
                    configuracao.CheckListDescricao,
                    configuracao.TravaChaveDescricao,
                    configuracao.ExpedicaoDescricao,
                    configuracao.LiberaChaveDescricao,
                    configuracao.FaturamentoDescricao,
                    configuracao.GuaritaSaidaDescricao,
                    configuracao.PosicaoDescricao,
                    configuracao.ChegadaLojaDescricao,
                    configuracao.DeslocamentoPatioDescricao,
                    configuracao.DocumentoFiscalDescricao,
                    configuracao.DocumentosTransporteDescricao,
                    configuracao.SaidaLojaDescricao,
                    configuracao.FimViagemDescricao,
                    configuracao.ExibirComprovanteSaida,
                    configuracao.InicioHigienizacaoDescricao,
                    configuracao.FimHigienizacaoDescricao,
                    configuracao.InicioCarregamentoDescricao,
                    configuracao.FimCarregamentoDescricao,
                    configuracao.SeparacaoMercadoriaDescricao,
                    configuracao.SolicitacaoVeiculoDescricao,
                    configuracao.InicioDescarregamentoDescricao,
                    configuracao.FimDescarregamentoDescricao,
                    configuracao.AvaliacaoDescargaDescricao,
                    configuracao.InformarDocaCarregamentoPermiteVoltar,
                    configuracao.MontagemCargaPermiteVoltar,
                    configuracao.ChegadaVeiculoPermiteVoltar,
                    configuracao.GuaritaEntradaPermiteVoltar,
                    configuracao.CheckListPermiteVoltar,
                    configuracao.UtilizarCategoriaDeReboqueConformeModeloVeicularCarga,
                    configuracao.TravaChavePermiteVoltar,
                    configuracao.ExpedicaoPermiteVoltar,
                    configuracao.LiberaChavePermiteVoltar,
                    configuracao.FaturamentoPermiteVoltar,
                    configuracao.GuaritaSaidaPermiteVoltar,
                    configuracao.PosicaoPermiteVoltar,
                    configuracao.ChegadaLojaPermiteVoltar,
                    configuracao.DeslocamentoPatioPermiteVoltar,
                    configuracao.DocumentoFiscalPermiteVoltar,
                    configuracao.DocumentosTransportePermiteVoltar,
                    configuracao.SaidaLojaPermiteVoltar,
                    configuracao.FimViagemPermiteVoltar,
                    configuracao.InicioHigienizacaoPermiteVoltar,
                    configuracao.FimHigienizacaoPermiteVoltar,
                    configuracao.InicioCarregamentoPermiteVoltar,
                    configuracao.FimCarregamentoPermiteVoltar,
                    configuracao.SeparacaoMercadoriaPermiteVoltar,
                    configuracao.SolicitacaoVeiculoPermiteVoltar,
                    configuracao.SolicitacaoVeiculoPermiteEnvioSMSMotorista,
                    configuracao.InicioDescarregamentoPermiteVoltar,
                    configuracao.FimDescarregamentoPermiteVoltar,
                    configuracao.AvaliacaoDescargaPermiteVoltar,
                    configuracao.GerarOcorrenciaPedidoEtapaDocaCarregamento,
                    configuracao.OcultarFluxoCarga,
                    configuracao.DocaDetalhada,
                    configuracao.OcultarTransportador,
                    configuracao.HabilitarPreCarga,
                    configuracao.ExibirTempoPrevistoERealizado,
                    configuracao.PermitirRejeicaoFluxo,
                    configuracao.IdentificacaoFluxoExibirOrigemXDestinos,
                    configuracao.IdentificacaoFluxoExibirTipoOperacao,
                    configuracao.IdentificacaoFluxoExibirModeloVeicularCargaVeiculo,
                    configuracao.SempreExibirPrevistoXRealizadoEDiferenca,
                    configuracao.SempreAtualizarDataPrevistaAoAlterarHorarioCarregamento,
                    configuracao.CheckListPermiteSalvarSemPreencher,
                    configuracao.InformarDocaCarregamentoUtilizarLocalCarregamento,
                    configuracao.IniciarViagemSemGuarita,
                    configuracao.HabilitarObservacaoEtapa,
                    configuracao.ExibirDetalhesIdentificacaoFluxo,
                    configuracao.PermiteCancelarFluxoPatioAtual,
                    PermiteRemoverVeiculoFilaCarregamentoAoCancelarFluxoPatio = permiteRemoverVeiculoFilaCarregamentoAoCancelarFluxoPatio,
                    configuracao.ObrigatorioInformarDataInicial,
                    configuracao.FaturamentoPermiteImprimirCapaViagem,
                    configuracao.DocumentoFiscalPermiteAvancarAutomaticamenteAposNotasFiscaisInseridas,
                    configuracao.ChecklistPermiteAntecipar,
                    configuracao.ChegadaVeiculoPermiteAntecipar,
                    configuracao.GuaritaEntradaPermiteAntecipar,
                    configuracao.MontagemCargaPermiteAntecipar,
                    configuracao.InformarDocaPermiteAntecipar,
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.GestaoPatio.FluxoPatio.OcorreuUmaFalhaAoBuscarFluxoDePatio);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterFluxoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaAreaVeiculo repositorioCargaAreaVeiculo = new Repositorio.Embarcador.Cargas.CargaAreaVeiculo(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta repFluxoGestaoPatioConfiguracaoAlerta = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta(unitOfWork);
                Repositorio.Embarcador.Filiais.GestaoPatioAlertaSla repositorioAlertasSlaGestaoPatio = new Repositorio.Embarcador.Filiais.GestaoPatioAlertaSla(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklistCarga = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork);

                Servicos.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio servicoMensagemAlerta = new Servicos.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio(unitOfWork);

                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repositorioConfiguracaoGestaoPatio.BuscarConfiguracao();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                filtrosPesquisa.ListarCargasCanceladas = configuracaoGestaoPatio.ListarCargasCanceladas;

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = ObterParametrosConsulta();
                int total = repositorioFluxoGestaoPatio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosGestaoPatio = total > 0 ? repositorioFluxoGestaoPatio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>();

                List<int> codigosFluxosGestaoPatio = (from o in fluxosGestaoPatio select o.Codigo).ToList();
                List<int> codigosCarga = (from o in fluxosGestaoPatio where o.Carga != null select o.Carga.Codigo).ToList();
                List<int> codigosFiliais = (from o in fluxosGestaoPatio where o.Filial != null select o.Filial.Codigo).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo> areasVeiculo = repositorioCargaAreaVeiculo.BuscarPorCargas(codigosCarga);
                List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta> configuracoesAlerta = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? repFluxoGestaoPatioConfiguracaoAlerta.BuscarPorUsuarioFiliais(Usuario.Codigo, filtrosPesquisa.CodigosFilial) : new List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta>();
                List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> sequenciasGestaoPatio = repositorioSequenciaGestaoPatio.BuscarTodosPorFiliais(codigosFiliais);
                List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> alertasSla = repositorioAlertasSlaGestaoPatio.BuscarPorFiliais(codigosFiliais);
                List<(int CodigoFluxo, int? Tempo)> listaTemposEtapaComparadoComDataAtual = ObterTemposEtapaAtualFluxosPatio(fluxosGestaoPatio, unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = repositorioCentroCarregamento.BuscarPorFiliais(codigosFiliais);
                List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga> listaChecklist = total > 0 ? repositorioChecklistCarga.BuscarPorFluxosGestaoPatio(fluxosGestaoPatio.Select(obj => obj.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga>();
                List<Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta> mensagensAlerta = servicoMensagemAlerta.ObterMensagensPorEntidades(codigosFluxosGestaoPatio);
                List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CategoriasCarga> categoriasPorCargas = repositorioFluxoGestaoPatio.BuscarCategoriasPorCargas(codigosCarga);

                List<dynamic> lista = (
                    from fluxoGestaoPatio in fluxosGestaoPatio
                    select ObterDetalhesFluxoPatio(
                        fluxoGestaoPatio,
                        configuracaoGestaoPatio,
                        configuracaoEmbarcador,
                        sequenciasGestaoPatio.Where(o => o.Filial.Codigo == fluxoGestaoPatio.Filial.Codigo && o.Tipo == fluxoGestaoPatio.Tipo && (o.TipoOperacao == null || o.TipoOperacao.Codigo == fluxoGestaoPatio.CargaBase.TipoOperacao?.Codigo)).OrderBy(o => o.TipoOperacao == null).FirstOrDefault(),
                        areasVeiculo,
                        configuracoesAlerta.Where(o => o.Filial.Codigo == fluxoGestaoPatio.Filial.Codigo).FirstOrDefault(),
                        alertasSla.Where(o => o.Filial.Codigo == fluxoGestaoPatio.Filial.Codigo).ToList(),
                        listaTemposEtapaComparadoComDataAtual,
                        centrosCarregamento.Where(o => o.Filial.Codigo == fluxoGestaoPatio.Filial.Codigo).FirstOrDefault(),
                        listaChecklist,
                        mensagensAlerta.Where(o => o.CodigoEntidade == fluxoGestaoPatio.Codigo).ToList(),
                        categoriasPorCargas.Find(c => c.CargaCodigo == fluxoGestaoPatio.Carga.Codigo),
                        unitOfWork
                    )).ToList();

                return new JsonpResult(lista, total);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.GestaoPatio.FluxoPatio.OcorreuUmaFalhaAoBuscarFluxoDePatio);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaAreaVeiculo repositorioCargaAreaVeiculo = new Repositorio.Embarcador.Cargas.CargaAreaVeiculo(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta repFluxoGestaoPatioConfiguracaoAlerta = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta(unitOfWork);
                Repositorio.Embarcador.Filiais.GestaoPatioAlertaSla repositorioAlertaSla = new Repositorio.Embarcador.Filiais.GestaoPatioAlertaSla(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioCheckListCarga = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repConfiguracaoGestaoPatio.BuscarConfiguracao();
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repFluxoGestaoPatio.BuscarPorCodigo(codigo, false);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork, configuracaoGestaoPatio);
                Servicos.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio servicoMensagemAlerta = new Servicos.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio);

                List<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo> areasVeiculo = repositorioCargaAreaVeiculo.BuscarPorCarga(fluxoGestaoPatio.Carga?.Codigo ?? 0);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta configuracaoAlerta = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? repFluxoGestaoPatioConfiguracaoAlerta.BuscarPorUsuarioFilial(Usuario.Codigo, fluxoGestaoPatio.Filial?.Codigo ?? 0) : null;
                List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> alertasSla = repositorioAlertaSla.BuscarPorFilial(fluxoGestaoPatio?.Filial?.Codigo ?? 0);
                List<(int CodigoFluxo, int? Tempo)> listaTemposEtapaComparadoComDataAtual = ObterTemposEtapaAtualFluxosPatio(new List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> { fluxoGestaoPatio }, unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = fluxoGestaoPatio.Filial != null ? repositorioCentroCarregamento.BuscarPorFilial(fluxoGestaoPatio.Filial.Codigo) : new Dominio.Entidades.Embarcador.Logistica.CentroCarregamento();
                List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga> listaChecklist = repositorioCheckListCarga.BuscarPorFluxosGestaoPatio(new List<int> { fluxoGestaoPatio.Codigo });
                List<Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta> mensagensAlerta = servicoMensagemAlerta.ObterMensagensPorEntidade(fluxoGestaoPatio.Codigo);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CategoriasCarga categoriaAgendamento = repFluxoGestaoPatio.BuscarCategoriasPorCodigoFluxoPatio(codigo);

                dynamic retorno = ObterDetalhesFluxoPatio(
                    fluxoGestaoPatio,
                    configuracaoGestaoPatio,
                    configuracaoEmbarcador,
                    sequenciaGestaoPatio,
                    areasVeiculo,
                    configuracaoAlerta,
                    alertasSla,
                    listaTemposEtapaComparadoComDataAtual,
                    centroCarregamento,
                    listaChecklist,
                    mensagensAlerta,
                    categoriaAgendamento,
                    unitOfWork
                );

                return new JsonpResult(retorno);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ComprovanteCargaInformada()
        {
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool descarga = Request.GetBoolParam("Descarga");
                string senhaAgendamento = Request.GetStringParam("SenhaAgendamento");

                byte[] pdf = ReportRequest.WithType(ReportType.ComprovanteCargaInformada)
                     .WithExecutionType(ExecutionType.Sync)
                     .AddExtraData("codigo", codigo.ToString())
                     .AddExtraData("descarga", descarga.ToString())
                     .AddExtraData("senhaAgendamento", senhaAgendamento.ToString())
                     .CallReport()
                     .GetContentFile();

                if (pdf == null)
                    return new JsonpResult(true, false, Localization.Resources.Gerais.Geral.NaoFoiPossivelGerarDocumento);

                return Arquivo(pdf, "application/pdf", Localization.Resources.GestaoPatio.FluxoPatio.ComprovanteDeCargaInformada + ".pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> SalvarConfiguracaoAlerta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    return new JsonpResult(false, true, Localization.Resources.GestaoPatio.FluxoPatio.VoceNaoTemPermissaoParaSalvarConfiguracaoDeAlerta);

                unitOfWork.Start();

                int codigoFilial = Request.GetIntParam("Filial");

                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta repFluxoGestaoPatioConfiguracaoAlerta = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlertaEtapa repFluxoGestaoPatioConfiguracaoAlertaEtapa = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlertaEtapa(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta configuracaoAlerta = repFluxoGestaoPatioConfiguracaoAlerta.BuscarPorUsuarioFilial(Usuario.Codigo, codigoFilial);
                if (configuracaoAlerta == null)
                {
                    configuracaoAlerta = new Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta();
                    configuracaoAlerta.DataCadastro = DateTime.Now;
                    configuracaoAlerta.Usuario = Usuario;
                    configuracaoAlerta.Filial = repFilial.BuscarPorCodigo(codigoFilial);
                    repFluxoGestaoPatioConfiguracaoAlerta.Inserir(configuracaoAlerta, Auditado);
                }

                //Salva as configurações das etapas
                dynamic dynEtapas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ConfiguracaoAlertaEtapas"));
                foreach (dynamic dynEtapa in dynEtapas)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapa = ((string)dynEtapa.CodigoEtapa).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio>();
                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlertaEtapa configuracaoAlertaEtapa = repFluxoGestaoPatioConfiguracaoAlertaEtapa.BuscarPorConfiguracaoEtapa(configuracaoAlerta.Codigo, etapa);
                    if (configuracaoAlertaEtapa == null)
                    {
                        configuracaoAlertaEtapa = new Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlertaEtapa();
                        configuracaoAlertaEtapa.EtapaFluxoGestaoPatio = etapa;
                        configuracaoAlertaEtapa.ConfiguracaoAlerta = configuracaoAlerta;
                    }

                    configuracaoAlertaEtapa.AlertaVisual = (bool)dynEtapa.AlertaVisual;
                    configuracaoAlertaEtapa.AlertaSonoro = (bool)dynEtapa.AlertaSonoro;

                    if (configuracaoAlertaEtapa.Codigo > 0)
                        repFluxoGestaoPatioConfiguracaoAlertaEtapa.Atualizar(configuracaoAlertaEtapa);
                    else
                        repFluxoGestaoPatioConfiguracaoAlertaEtapa.Inserir(configuracaoAlertaEtapa);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.GestaoPatio.FluxoPatio.OcorreuUmaFalhaAoSalvarConfiguracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarConfiguracaoAlertaFilialDoUsuario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoFilial = Request.GetIntParam("Filial");

                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta repFluxoGestaoPatioConfiguracaoAlerta = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta configuracaoAlerta = repFluxoGestaoPatioConfiguracaoAlerta.BuscarPorUsuarioFilial(Usuario.Codigo, codigoFilial);

                if (configuracaoAlerta == null)
                    return new JsonpResult(true);

                var retorno = new
                {
                    Etapas = (from obj in configuracaoAlerta.Etapas
                              select new
                              {
                                  obj.EtapaFluxoGestaoPatio,
                                  obj.AlertaVisual,
                                  obj.AlertaSonoro
                              }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.GestaoPatio.FluxoPatio.OcorreuUmaFalhaAoBuscarConfiguracaoDoUsuarioDaFilial);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DefinirEtapaComoVisualizada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                EtapaFluxoGestaoPatio etapaFluxoGestaoPatio = Request.GetEnumParam<EtapaFluxoGestaoPatio>("Etapa");

                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas repFluxoGestaoPatioEtapas = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta repFluxoGestaoPatioConfiguracaoAlerta = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta(unitOfWork);
                    
                await unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaPatio = repFluxoGestaoPatioEtapas.BuscarPorGestaoEEtapa(codigo, etapaFluxoGestaoPatio);

                if (etapaPatio == null)
                    return new JsonpResult(false, true, Localization.Resources.GestaoPatio.FluxoPatio.EtapaNaoFoiLocalizada);

                if (!etapaPatio.EtapaVisualizada)
                {
                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta configuracaoAlerta = repFluxoGestaoPatioConfiguracaoAlerta.BuscarPorUsuarioFilial(Usuario.Codigo, etapaPatio.FluxoGestaoPatio.Filial?.Codigo ?? 0);
                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlertaEtapa configuracaoAlertaEtapa = configuracaoAlerta?.Etapas.Where(o => o.EtapaFluxoGestaoPatio == etapaFluxoGestaoPatio && o.EtapaFluxoGestaoPatio == etapaPatio.FluxoGestaoPatio.EtapaFluxoGestaoPatioAtual)?.FirstOrDefault();


                    etapaPatio.EtapaVisualizada = true;
                    await repFluxoGestaoPatioEtapas.AtualizarAsync(etapaPatio);

                    await unitOfWork.CommitChangesAsync();

                    return new JsonpResult(new
                    {
                        AlertaSonoro = configuracaoAlertaEtapa != null && configuracaoAlertaEtapa.AlertaSonoro,
                        AlertaVisual = configuracaoAlertaEtapa != null && configuracaoAlertaEtapa.AlertaVisual
                    });
                }
                else
                    return new JsonpResult(new { AlertaSonoro = false, AlertaVisual = false });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, Localization.Resources.GestaoPatio.FluxoPatio.OcorreuUmaFalhaAoDefinirEtapaComoVisualizada);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterCapacidadePorDoca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosFilial = Request.GetListParam<int>("Filial");
                DateTime? dataInicial = Request.GetNullableDateTimeParam("DataInicial");
                DateTime? dataFinal = Request.GetNullableDateTimeParam("DataFinal");
                List<int> codigosTipoCarga = Request.GetListParam<int>("TipoCarga");

                if (codigosFilial.Count == 0 || codigosFilial.Count > 1)
                    return new JsonpResult(false);

                if (!dataInicial.HasValue || !dataFinal.HasValue || dataInicial.Value.Date != dataFinal.Value.Date)
                    return new JsonpResult(false);

                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = null;

                if (codigosTipoCarga.Count == 1)
                    centroCarregamento = repositorioCentroCarregamento.BuscarPorTipoCargaEFilial(codigosTipoCarga.FirstOrDefault(), codigosFilial.FirstOrDefault(), true);
                else
                    centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(codigosFilial.FirstOrDefault());

                if (centroCarregamento == null || centroCarregamento.LimiteCarregamentos != LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
                    return new JsonpResult(false);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoCarregamento repPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo repPeriodoCarregamentoTipoOperacaoSimultaneo = new Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork);

                var dataCarregamento = dataInicial.Value.Date;
                TimeSpan? periodoInicial = dataInicial.Value.TimeOfDay; ;
                TimeSpan? periodoFinal = dataFinal.Value.TimeOfDay;

                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento
                {
                    DataCarregamento = dataCarregamento,
                    InicioPeriodoInicial = periodoInicial,
                    InicioPeriodoFinal = periodoFinal,
                    CodigoCentroCarregamento = centroCarregamento.Codigo,
                    CodigoTransportador = 0,
                    CodigoTipoOperacao = 0
                };

                List<(int CodigoTipoOperacao, string TipoOperacao, decimal PesoCarga, bool PossuiVeiculo)> cargasJanelaCarregamentoPeriodo = repCargaJanelaCarregamento.BuscarPesoAlocadoEPercentualAgendado(filtrosPesquisa);
                List<int> tiposOperacao = new List<int>();
                int quantidadeDocasAlocadas = cargasJanelaCarregamentoPeriodo.Count();

                if (quantidadeDocasAlocadas > 0)
                    tiposOperacao = cargasJanelaCarregamentoPeriodo.Select(o => o.CodigoTipoOperacao).Distinct().ToList();
                else
                    return new JsonpResult(false);

                var capacidadePorTipoOperacao = (
                    from tipoOperacao in tiposOperacao
                    select new
                    {
                        CodigoTipoOperacao = tipoOperacao,
                        DescricaoTipoOperacao = cargasJanelaCarregamentoPeriodo.Where(o => o.CodigoTipoOperacao == tipoOperacao).Select(o => o.TipoOperacao).FirstOrDefault(),
                        PercentualAgendado = $"{(Convert.ToDecimal(cargasJanelaCarregamentoPeriodo.Where(o => o.CodigoTipoOperacao == tipoOperacao).ToList().Count * 100) / Convert.ToDecimal(quantidadeDocasAlocadas)):n2} %",
                        PesoTotalCargas = $"{(cargasJanelaCarregamentoPeriodo.Where(o => o.CodigoTipoOperacao == tipoOperacao).Sum(o => o.PesoCarga)):n2}",
                        CargasSemVeiculosInformados = cargasJanelaCarregamentoPeriodo.Where(o => o.CodigoTipoOperacao == tipoOperacao && !o.PossuiVeiculo).Count(),
                        CargasComVeiculosInformados = cargasJanelaCarregamentoPeriodo.Where(o => o.CodigoTipoOperacao == tipoOperacao && o.PossuiVeiculo).Count(),
                    });

                return new JsonpResult(new
                {
                    CapacidadePorTipoOperacao = capacidadePorTipoOperacao,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarAosDadosDeCapacidadeDeCarregamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadInicioCarregamentoViaCega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigoFetch(codigoCarga);

                Servicos.Embarcador.GestaoPatio.InicioHigienizacao servicoInicioHigienizacao = new Servicos.Embarcador.GestaoPatio.InicioHigienizacao(unitOfWork, Auditado);

                if (carga == null)
                    return new JsonpResult(true, false, "Não foi possível encontrar a carga para gerar a Via Cega.");

                byte[] pdf = servicoInicioHigienizacao.GerarPdfViaCega(carga);

                if (pdf == null)
                    return new JsonpResult(true, false, "Não foi possível gerar o relatório de Via Cega. Tente novamente.");

                return Arquivo(pdf, "application/pdf", "ViaCega.pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da Via Cega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadRomaneioTotalizador()
        {
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                byte[] pdf = ReportRequest.WithType(ReportType.RomaneioTotalizador)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoCarga", codigoCarga)
                    .CallReport()
                    .GetContentFile();

                if (pdf == null)
                    return new JsonpResult(true, false, Localization.Resources.Gerais.Geral.NaoFoiPossivelGerarDocumento);

                return Arquivo(pdf, "application/pdf", "RomaneioTotalizador.pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do Romaneio.");
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadRomaneioDetalhado()
        {
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                byte[] pdf = ReportRequest.WithType(ReportType.RomaneioDetalhado)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigoCarga", codigoCarga)
                    .CallReport()
                    .GetContentFile();

                if (pdf == null)
                    return new JsonpResult(true, false, Localization.Resources.Gerais.Geral.NaoFoiPossivelGerarDocumento);

                return Arquivo(pdf, "application/pdf", "RomaneioDetalhado.pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do Romaneio.");
            }
        }
        public async Task<IActionResult> DownloadRomaneioDetalhadoResumido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                byte[] pdf = ReportRequest.WithType(ReportType.RomaneioDetalhadoResumido)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigoCarga", codigoCarga)
                    .CallReport()
                    .GetContentFile();

                if (pdf == null)
                    return new JsonpResult(true, false, Localization.Resources.Gerais.Geral.NaoFoiPossivelGerarDocumento);

                return Arquivo(pdf, "application/pdf", "RomaneioDetalhadoResumido.pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do Romaneio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> EnviarNotificacaoMotoristaSMS()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoNotificacao = Request.GetIntParam("Mensagem");


                Repositorio.Embarcador.Configuracoes.NotificacaoMotoristaSMS repNotificacaoMotoristaSMS = new Repositorio.Embarcador.Configuracoes.NotificacaoMotoristaSMS(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repGestaoPatio.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS notificacaoMotoristaSMS = repNotificacaoMotoristaSMS.BuscarPorCodigo(codigoNotificacao);

                if (fluxoGestaoPatio.Carga.Motoristas == null)
                    return new JsonpResult(true, false, "Obrigatório informar um motorista");

                var isTerceiro = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);


                Servicos.SMS srvSMS = new Servicos.SMS(unitOfWork);
                string msgErro;
                string cpfMotorista = fluxoGestaoPatio?.Carga?.CPFPrimeiroMotorista ?? "";
                string nomeMotorista = fluxoGestaoPatio.Carga?.NomePrimeiroMotorista ?? "";
                string numeroCarga = fluxoGestaoPatio.Carga?.CodigoCargaEmbarcador ?? "";
                string placaVeiculo = fluxoGestaoPatio.Carga?.RetornarPlacas ?? "";
                string nomeTransportadora = (isTerceiro)
                                        ? fluxoGestaoPatio.Carga.Veiculo?.Proprietario.Empresa.RazaoSocial
                                        : fluxoGestaoPatio.Carga.Veiculo?.Empresa?.RazaoSocial;
                string cnpjTransportadora = (isTerceiro)
                                        ? fluxoGestaoPatio.Carga.Veiculo?.Proprietario?.CPF_CNPJ_Formatado
                                        : fluxoGestaoPatio.Carga.Veiculo?.Empresa?.CNPJ_Formatado;
                string doca = fluxoGestaoPatio.Carga?.NumeroDoca ?? "";

                string mensagem = notificacaoMotoristaSMS.Mensagem.Replace("#CpfMotorista", cpfMotorista)
                                                                  .Replace("#NomeMotorista", nomeMotorista)
                                                                  .Replace("#NumeroCarga", numeroCarga)
                                                                  .Replace("#PlacaVeiculo", placaVeiculo)
                                                                  .Replace("#NomeTransportadora", nomeTransportadora)
                                                                  .Replace("#CnpjTransportadora", cnpjTransportadora)
                                                                  .Replace("#Doca", doca)
                                                                  .Replace("#DataMensagem", DateTime.Now.ToString());

                if (!srvSMS.EnviarSMS(ConfiguracaoEmbarcador.TokenSMS, ConfiguracaoEmbarcador.SenderSMS, fluxoGestaoPatio.Carga.Motoristas.FirstOrDefault().Celular, mensagem, unitOfWork, out msgErro))
                {
                    return new JsonpResult(false, true, msgErro);
                }

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a notificação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterDetalhesFluxoPatio(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio, List<Dominio.Entidades.Embarcador.Cargas.CargaAreaVeiculo> areasVeiculo, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta configuracaoAlerta, List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> alertasSla, List<(int CodigoFluxo, int? Tempo)> listaTemposEtapaComparadoComDataAtual, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga> listaChecklist, List<Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta> mensagensAlerta, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.CategoriasCarga categoriasCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (sequenciaGestaoPatio == null)
                throw new ControllerException(Localization.Resources.GestaoPatio.FluxoPatio.FilialNaoPossuiUmaSequenciaParaEstaGestaoDePatio);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork, configuracaoGestaoPatio);
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada> etapasDescricao = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricoesEtapas(fluxoGestaoPatio);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = fluxoGestaoPatio.Carga;
            Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados cargaDadosSumarizados = carga?.DadosSumarizados;
            bool cargaPreCarga = carga?.CargaDePreCarga ?? false;

            if (!cargaPreCarga)
                cargaPreCarga = carga == null;

            string areaVeiculo = string.Join(", ", (from o in areasVeiculo where o.Carga.Codigo == (carga?.Codigo ?? 0) select o.AreaVeiculo.Descricao).ToList());
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlertaEtapa configuracaoAlertaEtapa = configuracaoAlerta?.Etapas.Where(o => o.EtapaFluxoGestaoPatio == fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual)?.FirstOrDefault();
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(fluxoGestaoPatio.Carga?.TipoOperacao?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = null;

            if (tipoOperacao != null && (tipoOperacao.ConfiguracaoAgendamentoColetaEntrega?.ConsiderarDataEntregaComoInicioDoFluxoPatio ?? false))
            {
                agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(fluxoGestaoPatio.Carga?.Codigo ?? 0);
            }

            var retorno = new
            {
                fluxoGestaoPatio.Codigo,
                PossuiCarga = fluxoGestaoPatio.Carga != null,
                NumeroCarga = fluxoGestaoPatio.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                CargaDePreCarga = cargaPreCarga,
                CargaCancelada = fluxoGestaoPatio.Carga != null && (fluxoGestaoPatio.Carga.SituacaoCarga == SituacaoCarga.Cancelada || fluxoGestaoPatio.Carga.SituacaoCarga == SituacaoCarga.Anulada),
                Carga = fluxoGestaoPatio.Carga?.Codigo ?? 0,
                PreCarga = fluxoGestaoPatio.PreCarga?.Codigo ?? 0,

                NumeroCarregamento = $"{(configuracaoGestaoPatio.IdentificacaoFluxoExibirCodigoIntegracaoFilial && !string.IsNullOrEmpty(fluxoGestaoPatio.Filial?.CodigoFilialEmbarcador) ? configuracaoGestaoPatio.ExibirSiglaFilial && !string.IsNullOrWhiteSpace(fluxoGestaoPatio.Filial.SiglaFilial) ? $"{fluxoGestaoPatio.Filial.SiglaFilial} - " : $"{fluxoGestaoPatio.Filial.CodigoFilialEmbarcador} - " : "")}{carga?.CodigoCargaEmbarcador ?? string.Empty}",

                fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio,
                fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual,
                fluxoGestaoPatio.EtapaAtual,
                Transportador = fluxoGestaoPatio.Carga != null ? fluxoGestaoPatio.Carga.Empresa?.NomeFantasia : fluxoGestaoPatio.PreCarga.Empresa?.NomeFantasia,

                TipoCarregamento = fluxoGestaoPatio?.Carga?.TipoCarregamento != null ? fluxoGestaoPatio?.Carga != null ? $"{(fluxoGestaoPatio?.Carga?.Motoristas?.FirstOrDefault()?.Nome ?? "")} / {(fluxoGestaoPatio?.Carga?.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm"))}" :
                $"{(fluxoGestaoPatio?.PreCarga?.Carga?.Motoristas?.FirstOrDefault()?.Nome ?? "")} / {(fluxoGestaoPatio?.PreCarga?.Carga?.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm"))}" : null,

                TipoOperacao = fluxoGestaoPatio.Carga != null ? fluxoGestaoPatio.Carga.TipoOperacao?.Descricao : fluxoGestaoPatio.PreCarga.TipoOperacao?.Descricao,
                Destinatario = configuracaoGestaoPatio.IdentificacaoFluxoExibirOrigemXDestinos ? (fluxoGestaoPatio.Carga != null ? fluxoGestaoPatio.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios : ConcatenarDestinatarios(fluxoGestaoPatio.PreCarga.Pedidos.ToList())) : "",
                Temperatura = fluxoGestaoPatio.Temperatura,

                //Descrições das etapas
                AvaliacaoDescargaDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.AvaliacaoDescarga, etapasDescricao),
                MontagemCargaDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.MontagemCarga, etapasDescricao),
                InformarDocaCarregamentoDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.InformarDoca, etapasDescricao),
                ChegadaVeiculoDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.ChegadaVeiculo, etapasDescricao),
                GuaritaEntradaDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.Guarita, etapasDescricao),
                CheckListDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.CheckList, etapasDescricao),
                TravaChaveDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.TravamentoChave, etapasDescricao),
                ExpedicaoDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.Expedicao, etapasDescricao),
                LiberaChaveDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.LiberacaoChave, etapasDescricao),
                FaturamentoDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.Faturamento, etapasDescricao),
                GuaritaSaidaDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.InicioViagem, etapasDescricao),
                PosicaoDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.Posicao, etapasDescricao),
                ChegadaLojaDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.ChegadaLoja, etapasDescricao),
                DeslocamentoPatioDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.DeslocamentoPatio, etapasDescricao),
                DocumentoFiscalDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.DocumentoFiscal, etapasDescricao),
                DocumentosTransporteDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.DocumentosTransporte, etapasDescricao),
                SaidaLojaDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.SaidaLoja, etapasDescricao),
                FimViagemDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.FimViagem, etapasDescricao),
                InicioHigienizacaoDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.InicioHigienizacao, etapasDescricao),
                FimHigienizacaoDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.FimHigienizacao, etapasDescricao),
                InicioCarregamentoDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.InicioCarregamento, etapasDescricao),
                FimCarregamentoDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.FimCarregamento, etapasDescricao),
                SeparacaoMercadoriaDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.SeparacaoMercadoria, etapasDescricao),
                SolicitacaoVeiculoDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.SolicitacaoVeiculo, etapasDescricao),
                InicioDescarregamentoDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.InicioDescarregamento, etapasDescricao),
                FimDescarregamentoDescricao = ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio.FimDescarregamento, etapasDescricao),

                DataDocaInformada = fluxoGestaoPatio.DataDocaInformada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataDocaInformadaPrevista = fluxoGestaoPatio.DataDocaInformadaPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataDocaInformadaReprogramada = fluxoGestaoPatio.DataDocaInformadaReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaDocaInformada,
                DocaInformadaLimitePermanencia = sequenciaGestaoPatio.InformarDocaCarregamentoTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaDocaInformada ?? 0) < sequenciaGestaoPatio.InformarDocaCarregamentoTempoPermanencia,

                DataChegadaVeiculo = fluxoGestaoPatio.DataChegadaVeiculo?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataChegadaVeiculoPrevista = fluxoGestaoPatio.DataChegadaVeiculoPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataChegadaVeiculoReprogramada = fluxoGestaoPatio.DataChegadaVeiculoReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaChegadaVeiculo,
                ChegadaVeiculoLimitePermanencia = sequenciaGestaoPatio.ChegadaVeiculoTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaChegadaVeiculo ?? 0) < sequenciaGestaoPatio.ChegadaVeiculoTempoPermanencia,

                DataEntregaGuarita = fluxoGestaoPatio.DataEntregaGuarita?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataEntregaGuaritaPrevista = fluxoGestaoPatio.DataEntregaGuaritaPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataEntregaGuaritaReprogramada = fluxoGestaoPatio.DataEntregaGuaritaReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesPesagem,
                sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesProdutor,
                sequenciaGestaoPatio.ChegadaVeiculoPermiteImprimirRelacaoDeProdutos,
                fluxoGestaoPatio.DiferencaEntregaGuarita,
                EntregaGuaritaLimitePermanencia = sequenciaGestaoPatio.GuaritaEntradaTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaEntregaGuarita ?? 0) < sequenciaGestaoPatio.GuaritaEntradaTempoPermanencia,

                sequenciaGestaoPatio.GuaritaSaidaPermiteInformacoesPesagem,

                DataFaturamento = fluxoGestaoPatio.DataFaturamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFaturamentoPrevista = fluxoGestaoPatio.DataFaturamentoPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFaturamentoReprogramada = fluxoGestaoPatio.DataFaturamentoReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaFaturamento,
                FaturamentoFinalizado = fluxoGestaoPatio?.Carga?.DataFinalizacaoEmissao.HasValue ?? false,
                FaturamentoLimitePermanencia = sequenciaGestaoPatio.FaturamentoTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaFaturamento ?? 0) < sequenciaGestaoPatio.FaturamentoTempoPermanencia,

                DataFimCheckList = fluxoGestaoPatio.DataFimCheckList?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimCheckListPrevista = fluxoGestaoPatio.DataFimCheckListPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimCheckListReprogramada = fluxoGestaoPatio.DataFimCheckListReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaFimCheckList,
                FimCheckListLimitePermanencia = sequenciaGestaoPatio.CheckListTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaFimCheckList ?? 0) < sequenciaGestaoPatio.CheckListTempoPermanencia,

                DataInicioCarregamentoPrevista = fluxoGestaoPatio.DataInicioCarregamentoPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioCarregamentoReprogramada = fluxoGestaoPatio.DataInicioCarregamentoReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioCarregamento = fluxoGestaoPatio.DataInicioCarregamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaInicioCarregamento,
                InicioCarregamentoLimitePermanencia = sequenciaGestaoPatio.InicioCarregamentoTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaInicioCarregamento ?? 0) < sequenciaGestaoPatio.InicioCarregamentoTempoPermanencia,

                DataFimCarregamento = fluxoGestaoPatio.DataFimCarregamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimCarregamentoPrevista = fluxoGestaoPatio.DataFimCarregamentoPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimCarregamentoReprogramada = fluxoGestaoPatio.DataFimCarregamentoReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaFimCarregamento,
                FimCarregamentoLimitePermanencia = sequenciaGestaoPatio.FimCarregamentoTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaFimCarregamento ?? 0) < sequenciaGestaoPatio.FimCarregamentoTempoPermanencia,

                DataInicioViagem = fluxoGestaoPatio.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioViagemPrevista = fluxoGestaoPatio.DataInicioViagemPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioViagemReprogramada = fluxoGestaoPatio.DataInicioViagemReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaInicioViagem,

                DataLiberacaoChave = fluxoGestaoPatio.DataLiberacaoChave?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataLiberacaoChavePrevista = fluxoGestaoPatio.DataLiberacaoChavePrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataLiberacaoChaveReprogramada = fluxoGestaoPatio.DataLiberacaoChaveReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaLiberacaoChave,
                LiberacaoChaveLimitePermanencia = sequenciaGestaoPatio.LiberaChaveTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaLiberacaoChave ?? 0) < sequenciaGestaoPatio.LiberaChaveTempoPermanencia,

                DataTravaChave = fluxoGestaoPatio.DataTravaChave?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataTravaChavePrevista = fluxoGestaoPatio.DataTravaChavePrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataTravaChaveReprogramada = fluxoGestaoPatio.DataTravaChaveReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaTravaChave,
                TravaChaveLimitePermanencia = sequenciaGestaoPatio.TravaChaveTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaTravaChave ?? 0) < sequenciaGestaoPatio.TravaChaveTempoPermanencia,

                DataPosicao = fluxoGestaoPatio.DataPosicao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataPosicaoPrevista = fluxoGestaoPatio.DataPosicaoPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataPosicaoReprogramada = fluxoGestaoPatio.DataPosicaoReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaPosicao,
                PosicaoLimitePermanencia = sequenciaGestaoPatio.PosicaoTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaPosicao ?? 0) < sequenciaGestaoPatio.PosicaoTempoPermanencia,

                DataChegadaLoja = fluxoGestaoPatio.DataChegadaLoja?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataChegadaLojaPrevista = (tipoOperacao.ConfiguracaoAgendamentoColetaEntrega?.ConsiderarDataEntregaComoInicioDoFluxoPatio ?? false) && agendamentoColeta != null ? agendamentoColeta.DataEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "" : fluxoGestaoPatio.DataChegadaLojaPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataChegadaLojaReprogramada = fluxoGestaoPatio.DataChegadaLojaReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaChegadaLoja,
                ChegadaLojaLimitePermanencia = sequenciaGestaoPatio.ChegadaLojaTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaChegadaLoja ?? 0) < sequenciaGestaoPatio.ChegadaLojaTempoPermanencia,

                DataDeslocamentoPatio = fluxoGestaoPatio.DataDeslocamentoPatio?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataDeslocamentoPatioPrevista = fluxoGestaoPatio.DataDeslocamentoPatioPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataDeslocamentoPatioReprogramada = fluxoGestaoPatio.DataDeslocamentoPatioReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaDeslocamentoPatio,
                DeslocamentoPatioLimitePermanencia = sequenciaGestaoPatio.DeslocamentoPatioTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaDeslocamentoPatio ?? 0) < sequenciaGestaoPatio.DeslocamentoPatioTempoPermanencia,
                sequenciaGestaoPatio.DeslocamentoPatioPermiteInformacoesPesagem,
                sequenciaGestaoPatio.DeslocamentoPatioPermiteInformacoesLoteInterno,
                sequenciaGestaoPatio.DeslocamentoPatioPermiteInformarQuantidade,

                DataSaidaLoja = fluxoGestaoPatio.DataSaidaLoja?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataSaidaLojaPrevista = fluxoGestaoPatio.DataSaidaLojaPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataSaidaLojaReprogramada = fluxoGestaoPatio.DataSaidaLojaReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaSaidaLoja,
                SaidaLojaLimitePermanencia = sequenciaGestaoPatio.SaidaLojaTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaSaidaLoja ?? 0) < sequenciaGestaoPatio.SaidaLojaTempoPermanencia,

                DataFimViagem = fluxoGestaoPatio.DataFimViagem?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimViagemPrevista = fluxoGestaoPatio.DataFimViagemPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimViagemReprogramada = fluxoGestaoPatio.DataFimViagemReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaFimViagem,
                FimViagemLimitePermanencia = sequenciaGestaoPatio.FimViagemTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaFimViagem ?? 0) < sequenciaGestaoPatio.FimViagemTempoPermanencia,

                DataInicioHigienizacao = fluxoGestaoPatio.DataInicioHigienizacao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioHigienizacaoPrevista = fluxoGestaoPatio.DataInicioHigienizacaoPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioHigienizacaoReprogramada = fluxoGestaoPatio.DataInicioHigienizacaoReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaInicioHigienizacao,
                InicioHigienizacaoLimitePermanencia = sequenciaGestaoPatio.InicioHigienizacaoTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaInicioHigienizacao ?? 0) < sequenciaGestaoPatio.InicioHigienizacaoTempoPermanencia,

                DataFimHigienizacao = fluxoGestaoPatio.DataFimHigienizacao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimHigienizacaoPrevista = fluxoGestaoPatio.DataFimHigienizacaoPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimHigienizacaoReprogramada = fluxoGestaoPatio.DataFimHigienizacaoReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaFimHigienizacao,
                FimHigienizacaoLimitePermanencia = sequenciaGestaoPatio.FimHigienizacaoTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaFimHigienizacao ?? 0) < sequenciaGestaoPatio.FimHigienizacaoTempoPermanencia,

                DataSeparacaoMercadoria = fluxoGestaoPatio.DataSeparacaoMercadoria?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataSeparacaoMercadoriaPrevista = fluxoGestaoPatio.DataSeparacaoMercadoriaPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataSeparacaoMercadoriaReprogramada = fluxoGestaoPatio.DataSeparacaoMercadoriaReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaSeparacaoMercadoria,
                SeparacaoMercadoriaLimitePermanencia = sequenciaGestaoPatio.SeparacaoMercadoriaTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaSeparacaoMercadoria ?? 0) < sequenciaGestaoPatio.SeparacaoMercadoriaTempoPermanencia,

                DataSolicitacaoVeiculo = fluxoGestaoPatio.DataSolicitacaoVeiculo?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataSolicitacaoVeiculoPrevista = fluxoGestaoPatio.DataSolicitacaoVeiculoPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataSolicitacaoVeiculoReprogramada = fluxoGestaoPatio.DataSolicitacaoVeiculoReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaSolicitacaoVeiculo,
                SolicitacaoVeiculoLimitePermanencia = sequenciaGestaoPatio.SolicitacaoVeiculoTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaSolicitacaoVeiculo ?? 0) < sequenciaGestaoPatio.SolicitacaoVeiculoTempoPermanencia,

                DataInicioDescarregamento = fluxoGestaoPatio.DataInicioDescarregamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioDescarregamentoPrevista = fluxoGestaoPatio.DataInicioDescarregamentoPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioDescarregamentoReprogramada = fluxoGestaoPatio.DataInicioDescarregamentoReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaInicioDescarregamento,
                InicioDescarregamentoLimitePermanencia = sequenciaGestaoPatio.InicioDescarregamentoTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaInicioDescarregamento ?? 0) < sequenciaGestaoPatio.InicioDescarregamentoTempoPermanencia,

                DataFimDescarregamento = fluxoGestaoPatio.DataFimDescarregamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimDescarregamentoPrevista = fluxoGestaoPatio.DataFimDescarregamentoPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimDescarregamentoReprogramada = fluxoGestaoPatio.DataFimDescarregamentoReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaFimDescarregamento,
                FimDescarregamentoLimitePermanencia = sequenciaGestaoPatio.FimDescarregamentoTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaFimDescarregamento ?? 0) < sequenciaGestaoPatio.FimDescarregamentoTempoPermanencia,

                DataFimAvaliacaoDescarga = fluxoGestaoPatio.DataFimDescarregamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimAvaliacaoDescargaPrevista = fluxoGestaoPatio.DataFimAvaliacaoDescargaPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimAvaliacaoDescargaReprogramada = fluxoGestaoPatio.DataFimAvaliacaoDescargaReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaFimAvaliacaoDescarga,
                FimAvaliacaoDescargaLimitePermanencia = sequenciaGestaoPatio.AvaliacaoDescargaTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaFimAvaliacaoDescarga ?? 0) < sequenciaGestaoPatio.AvaliacaoDescargaTempoPermanencia,

                DataDocumentoFiscal = fluxoGestaoPatio.DataDocumentoFiscal?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataDocumentoFiscalPrevista = fluxoGestaoPatio.DataDocumentoFiscalPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataDocumentoFiscalReprogramada = fluxoGestaoPatio.DataDocumentoFiscalReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaDocumentoFiscal,
                DocumentoFiscalLimitePermanencia = sequenciaGestaoPatio.DocumentoFiscalTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaDocumentoFiscal ?? 0) < sequenciaGestaoPatio.DocumentoFiscalTempoPermanencia,

                DataDocumentosTransporte = fluxoGestaoPatio.DataDocumentosTransporte?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataDocumentosTransportePrevista = fluxoGestaoPatio.DataDocumentosTransportePrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataDocumentosTransporteReprogramada = fluxoGestaoPatio.DataDocumentosTransporteReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaDocumentosTransporte,
                DocumentosTransporteLimitePermanencia = sequenciaGestaoPatio.DocumentosTransporteTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaDocumentosTransporte ?? 0) < sequenciaGestaoPatio.DocumentosTransporteTempoPermanencia,

                DataMontagemCarga = fluxoGestaoPatio.DataMontagemCarga?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataMontagemCargaPrevista = fluxoGestaoPatio.DataMontagemCargaPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataMontagemCargaReprogramada = fluxoGestaoPatio.DataMontagemCargaReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoPatio.DiferencaMontagemCarga,
                MontagemCargaLimitePermanencia = sequenciaGestaoPatio.MontagemCargaTempoPermanencia > 0 && (fluxoGestaoPatio.DiferencaMontagemCarga ?? 0) < sequenciaGestaoPatio.MontagemCargaTempoPermanencia,

                Placas = carga != null ? ObterPlacas(carga.Veiculo, carga.VeiculosVinculados, configuracaoGestaoPatio) : ObterPlacas(fluxoGestaoPatio.PreCarga.Veiculo, fluxoGestaoPatio.PreCarga.VeiculosVinculados, configuracaoGestaoPatio),
                NomeMotoristas = carga != null ? ObterNomeMotoristas(carga.Motoristas.ToList()) : ObterNomeMotoristas(fluxoGestaoPatio.PreCarga.Motoristas.ToList()),
                CPFMotoristas = carga != null ? ObterCPFMotoristas(carga.Motoristas.ToList()) : ObterCPFMotoristas(fluxoGestaoPatio.PreCarga.Motoristas.ToList()),
                Remetente = configuracaoGestaoPatio.IdentificacaoFluxoExibirCodigoIntegracaoFilial ? "" : cargaDadosSumarizados?.CodigoIntegracaoRemetentes ?? "",
                DataCarga = carga?.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Doca = !string.IsNullOrWhiteSpace(carga?.NumeroDocaEncosta) ? carga?.NumeroDocaEncosta : carga?.NumeroDoca ?? string.Empty,
                PercentualSeparacaoMercadoria = string.IsNullOrWhiteSpace(carga?.PercentualSeparacaoMercadoria.ToString()) ? "0%" : carga.PercentualSeparacaoMercadoria.ToString() + '%',
                carga?.SeparacaoMercadoriaConfirmada,
                ModeloVeicularCargaVeiculo = fluxoGestaoPatio.CargaBase.ModeloVeicularCargaVeiculo?.Descricao ?? "",
                AreaVeiculo = areaVeiculo,
                Equipamento = fluxoGestaoPatio.Equipamento?.Descricao ?? string.Empty,
                AntecipacaoICMS = cargaDadosSumarizados?.PossuiAntecipacaoICMS ?? false,
                Tipo = fluxoGestaoPatio.Tipo,
                LimiteCarregamentos = centroCarregamento?.LimiteCarregamentos,
                MensagensAlerta = mensagensAlerta,
                CategoriaAgendamento = categoriasCarga?.CategoriaAgendamento ?? string.Empty,
                Etapas = (
                    from etapa in fluxoGestaoPatio.GetEtapas()
                    select new
                    {
                        etapa.EtapaFluxoGestaoPatio,
                        etapa.EtapaLiberada,
                        PossibilidadePreencherEtapaBloqueada = ObterPermissaoDeEditarEtapaBloqueada(etapa, fluxoGestaoPatio, sequenciaGestaoPatio, configuracaoGestaoPatio),
                        AlertaVisual = VerificarSeEtapaPossuiAlertaVisual(configuracaoAlertaEtapa, etapa, alertasSla, fluxoGestaoPatio, listaTemposEtapaComparadoComDataAtual.Where(obj => obj.CodigoFluxo == fluxoGestaoPatio.Codigo).Select(obj => obj.Tempo).FirstOrDefault()),
                        AlertaSonoro = ConfiguracaoAlertaValida(configuracaoAlertaEtapa, etapa) ? configuracaoAlertaEtapa.AlertaSonoro : false,
                        Cor = ObterCorEtapa(alertasSla, fluxoGestaoPatio, listaTemposEtapaComparadoComDataAtual.Where(obj => obj.CodigoFluxo == fluxoGestaoPatio.Codigo).Select(obj => obj.Tempo).FirstOrDefault(), (from obj in listaChecklist where obj.FluxoGestaoPatio.Codigo == fluxoGestaoPatio.Codigo select obj).FirstOrDefault(), etapa),
                        ExibirAlerta = VerificarSeEtapaExibeAlerta(etapa, fluxoGestaoPatio)
                    }
                ).ToList(),

                EnvioNotificacaoSMS = fluxoGestaoPatio?.Carga?.Motoristas != null,
                fluxoGestaoPatio?.Filial.InformarEquipamentoFluxoPatio,
                PermiteEnviarNotificacaoSuperApp = (configuracaoIntegracao?.PossuiIntegracaoTrizy ?? false) && fluxoGestaoPatio?.Carga?.Motoristas != null && fluxoGestaoPatio.Carga.Motoristas.Count > 0,
            };

            return retorno;
        }

        private bool VerificarSeEtapaExibeAlerta(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (etapa.ExibirAlerta && (etapa.EtapaFluxoGestaoPatio == fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual) && !fluxoGestaoPatio.DataFinalizacaoFluxo.HasValue)
                return true;

            return false;
        }

        private bool VerificarSeEtapaPossuiAlertaVisual(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlertaEtapa configuracaoAlertaEtapa, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa, List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> gestaoPatioAlertaSla, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, int? tempoExcedido)
        {
            if (ConfiguracaoAlertaValida(configuracaoAlertaEtapa, etapa) && configuracaoAlertaEtapa.AlertaVisual)
                return true;

            Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla alertaSla = gestaoPatioAlertaSla.Where(obj => obj.Etapas.Any(e => e == fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual)).FirstOrDefault();

            if (alertaSla == null)
                return false;

            return tempoExcedido.HasValue && ((tempoExcedido > alertaSla.TempoExcedido) || (alertaSla.TempoFaltante > 0 && tempoExcedido > -alertaSla.TempoFaltante));
        }

        private string ObterCorEtapa(List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> gestaoPatioAlertaSla, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, int? tempoExcedido, Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa)
        {
            if (etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.CheckList && checklist.Reavaliada)
                return "#a5d793";

            if (etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.CheckList && checklist.EditadoRetroativo)
                return Cores.Laranja.Descricao();

            if (etapa != fluxoGestaoPatio.GetEtapaAtual())
                return string.Empty;

            if (fluxoGestaoPatio.DataFinalizacaoFluxo.HasValue)
                return string.Empty;

            if (!tempoExcedido.HasValue)
                return string.Empty;

            if (!gestaoPatioAlertaSla.Any(obj => obj.Etapas.Any(e => e == fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual)))
                return string.Empty;

            Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla alertaSla = gestaoPatioAlertaSla.Where(obj => obj.Etapas.Any(e => e == fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual)).FirstOrDefault();

            if (tempoExcedido > alertaSla.TempoExcedido)
                return alertaSla.CorAlertaTempoExcedido;

            if (alertaSla.TempoFaltante > 0 && tempoExcedido > -alertaSla.TempoFaltante)
                return alertaSla.CorAlertaTempoFaltante;

            return string.Empty;
        }

        private List<(int CodigoFluxo, int? Tempo)> ObterTemposEtapaAtualFluxosPatio(List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

            List<(int CodigoFluxo, int? Tempo)> listaRetorno = new List<(int CodigoFluxo, int? Tempo)>();

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio in fluxosGestaoPatio)
                listaRetorno.Add((fluxoGestaoPatio.Codigo, servicoFluxoGestaoPatio.ObterTempoExcedidoEtapaAtual(fluxoGestaoPatio)));

            return listaRetorno;
        }

        private string ConcatenarDestinatarios(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            return ConcatenarCodigosIntegradores(from o in pedidos select o.Destinatario.CodigoIntegracao);
        }

        private string ConcatenarCodigosIntegradores(IEnumerable<string> codigos)
        {
            return String.Join(" - ", (from o in codigos where !string.IsNullOrEmpty(o) select o));
        }

        private bool ConfiguracaoAlertaValida(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlertaEtapa configuracaoAlertaEtapa, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa)
        {
            return configuracaoAlertaEtapa != null && configuracaoAlertaEtapa.EtapaFluxoGestaoPatio == etapa.EtapaFluxoGestaoPatio && etapa.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aprovado && !etapa.EtapaVisualizada;
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio()
            {
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                CodigosAreaVeiculo = Request.GetListParam<int>("AreaVeiculo"),
                CodigosLocalCarregamento = Request.GetListParam<int>("LocalCarregamento"),
                CodigosTipoCarga = Request.GetListParam<int>("TipoCarga"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CpfCnpjDestinatario = Request.GetListParam<double>("Destinatario"),
                CpfCnpjRemetente = Request.GetListParam<double>("Remetente"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                EtapaFluxoGestaoPatio = Request.GetListEnumParam<EtapaFluxoGestaoPatio>("EtapaFluxoGestaoPatio"),
                NumeroCarga = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal"),
                NumeroNfProdutor = Request.GetIntParam("NumeroNfProdutor"),
                Pedido = Request.GetStringParam("Pedido"),
                Placa = Request.GetStringParam("Placa"),
                PreCarga = Request.GetStringParam("PreCarga"),
                Situacao = Request.GetNullableEnumParam<SituacaoEtapaFluxoGestaoPatio>("Situacao"),
                Tipo = Request.GetNullableEnumParam<TipoFluxoGestaoPatio>("Tipo"),
                CodigoTipoCarregamento = Request.GetIntParam("TipoCarregamento"),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                DataFinalChegadaVeiculo = Request.GetNullableDateTimeParam("DataFinalChegadaVeiculo"),
                DataInicialChegadaVeiculo = Request.GetNullableDateTimeParam("DataInicialChegadaVeiculo")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportador = this.Empresa.Codigo;
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                filtrosPesquisa.CodigoTransportador = new Repositorio.Empresa(unitOfWork).BuscarPorCNPJ(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato)?.Codigo ?? 0;

            int codigoFilial = Request.GetIntParam("Filial");//Para o fluxo em tabela
            List<int> codigosFilial = Request.GetListParam<int>("Filial");

            filtrosPesquisa.CodigosFilial = codigoFilial == 0 ? codigosFilial : new List<int>() { codigoFilial };

            if (filtrosPesquisa.CodigosTransportador == null || filtrosPesquisa.CodigosTransportador.Count == 0)
                filtrosPesquisa.CodigosTransportador = ObterListaCodigoTransportadorPermitidosOperadorLogistica(unitOfWork);

            return filtrosPesquisa;
        }

        private string ObterNomenclaturaEtapa(EtapaFluxoGestaoPatio etapaFluxoGestaoPatio, List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada> etapasDescricao)
        {
            return etapasDescricao?.Where(o => o.Enumerador == etapaFluxoGestaoPatio)?.FirstOrDefault()?.Descricao ?? string.Empty;
        }

        private Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta ObterParametrosConsulta()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoOrdenar = Request.GetStringParam("Ordenacao"),
                InicioRegistros = Request.GetIntParam("inicio"),
                LimiteRegistros = Request.GetIntParam("limite"),
                PropriedadeOrdenar = "Carga.DataCarregamentoCarga"
            };
        }

        private bool ObterPermissaoDeEditarEtapaBloqueada(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapa, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio, Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio)
        {
            if (fluxoGestaoPatio.EtapaAtual < etapa.Ordem)
                if ((etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.ChegadaVeiculo && configuracaoGestaoPatio.ChegadaVeiculoPermiteAntecipar) ||
                   (etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.MontagemCarga && configuracaoGestaoPatio.MontagemCargaPermiteAntecipar) ||
                   (etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.Guarita && configuracaoGestaoPatio.GuaritaEntradaPermiteAntecipar) ||
                   (etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.CheckList && configuracaoGestaoPatio.ChecklistPermiteAntecipar) ||
                   (etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.InformarDoca && configuracaoGestaoPatio.InformarDocaPermiteAntecipar))
                    return true;

            return etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.InicioViagem && fluxoGestaoPatio.EtapaAtual < etapa.Ordem && (sequenciaGestaoPatio?.GuaritaSaidaPermiteInformacoesPesagem ?? false);
        }

        private string ObterPlacas(Dominio.Entidades.Veiculo veiculo, IEnumerable<Dominio.Entidades.Veiculo> veiculosVinculados, Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio)
        {
            List<string> placas = new List<string>();

            if (veiculo != null && configuracaoGestaoPatio.VisualizarPlacaTracao)
                placas.Add(veiculo.Placa);

            if (configuracaoGestaoPatio.VisualizarPlacaReboque)
                placas.AddRange(veiculosVinculados.Select(o => o.Placa));

            return string.Join(", ", placas);
        }
        private string ObterCPFMotoristas(List<Dominio.Entidades.Usuario> motoristas)
        {
            return string.Join(", ", motoristas.Select(m => m.CPF).ToList());
        }
        private string ObterNomeMotoristas(List<Dominio.Entidades.Usuario> motoristas)
        {
            return string.Join(", ", motoristas.Select(m => m.Nome).ToList());
        }

        #endregion
    }
}
