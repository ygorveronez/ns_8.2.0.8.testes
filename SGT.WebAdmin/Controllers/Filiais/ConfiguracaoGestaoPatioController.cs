using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Filiais
{
    [CustomAuthorize("Filiais/ConfiguracaoGestaoPatio")]
    public class ConfiguracaoGestaoPatioController : BaseController
    {
        #region Construtores

        public ConfiguracaoGestaoPatioController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ConfiguracaoGestaoPatioPadrao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao servicoFluxoGestaoPatioConfiguracao = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracao = servicoFluxoGestaoPatioConfiguracao.ObterConfiguracao();

                return new JsonpResult(new
                {
                    configuracao.ViewFluxoPatioTabelado,

                    configuracao.CheckListDescricao,
                    configuracao.CheckListPermiteQRCode,
                    configuracao.CheckListPermiteVoltar,
                    configuracao.UtilizarCategoriaDeReboqueConformeModeloVeicularCarga,
                    configuracao.CheckListPermiteSalvarSemPreencher,
                    ChecklistTipoIntegracao = new { value = configuracao.ChecklistTipoIntegracao, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoHelper.ObterDescricao(configuracao.ChecklistTipoIntegracao) },
                    InformarDocaCarregamentoTipoIntegracao = new { value = configuracao.InformarDocaCarregamentoTipoIntegracao, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoHelper.ObterDescricao(configuracao.InformarDocaCarregamentoTipoIntegracao) },
                    InicioCarregamentoTipoIntegracao = new { value = configuracao.InicioCarregamentoTipoIntegracao, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoHelper.ObterDescricao(configuracao.InicioCarregamentoTipoIntegracao) },
                    FimCarregamentoTipoIntegracao = new { value = configuracao.FimCarregamentoTipoIntegracao, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoHelper.ObterDescricao(configuracao.FimCarregamentoTipoIntegracao) },

                    configuracao.ChegadaLojaDescricao,
                    configuracao.ChegadaLojaPermiteQRCode,
                    configuracao.ChegadaLojaPermiteVoltar,

                    configuracao.ChegadaVeiculoDescricao,
                    configuracao.ChegadaVeiculoPermiteQRCode,
                    configuracao.ChegadaVeiculoPermiteVoltar,
                    configuracao.ChegadaVeiculoPermiteAvancarAutomaticamenteAposInformarDadosTransporteCarga,
                    configuracao.ChegadaVeiculoPermiteInformarComEtapaBloqueada,
                    ChegadaVeiculoTipoIntegracao = new { value = configuracao.ChegadaVeiculoTipoIntegracao, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoHelper.ObterDescricao(configuracao.ChegadaVeiculoTipoIntegracao) },
                    configuracao.ChegadaVeiculoAction,
                    configuracao.PermiteGerarAtendimento,

                    configuracao.DeslocamentoPatioDescricao,
                    configuracao.DeslocamentoPatioPermiteQRCode,
                    configuracao.DeslocamentoPatioPermiteVoltar,

                    configuracao.DocumentoFiscalDescricao,
                    configuracao.DocumentoFiscalPermiteQRCode,
                    configuracao.DocumentoFiscalPermiteVoltar,
                    configuracao.DocumentoFiscalPermiteAvancarAutomaticamenteAposNotasFiscaisInseridas,

                    configuracao.DocumentosTransporteDescricao,
                    configuracao.DocumentosTransportePermiteQRCode,
                    configuracao.DocumentosTransportePermiteVoltar,
                    configuracao.DocumentosTransportePermiteAvancarAutomaticamenteAposGerarDocumentos,
                    DocumentosTransporteTipoIntegracao = new { value = configuracao.DocumentosTransporteTipoIntegracao, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoHelper.ObterDescricao(configuracao.DocumentosTransporteTipoIntegracao) },

                    configuracao.ExpedicaoDescricao,
                    configuracao.ExpedicaoPermiteQRCode,
                    configuracao.ExpedicaoPermiteVoltar,

                    configuracao.FaturamentoDescricao,
                    configuracao.FaturamentoPermiteAvancarAutomaticamenteAposGerarDocumentos,
                    configuracao.FaturamentoPermiteSolicitarNotasFiscaisEtapaBloqueada,
                    configuracao.FaturamentoPermiteQRCode,
                    configuracao.FaturamentoPermiteVoltar,

                    configuracao.FimCarregamentoDescricao,
                    configuracao.FimCarregamentoPermiteQRCode,
                    configuracao.FimCarregamentoPermiteVoltar,
                    configuracao.FimCarregamentoPermiteAvancarSomenteDadosTransporteInformados,

                    configuracao.FimDescarregamentoDescricao,
                    configuracao.FimDescarregamentoPermiteQRCode,
                    configuracao.FimDescarregamentoPermiteVoltar,

                    configuracao.FimHigienizacaoDescricao,
                    configuracao.FimHigienizacaoPermiteAvancarAutomaticamenteComVeiculosHigienizados,
                    configuracao.FimHigienizacaoPermiteQRCode,
                    configuracao.FimHigienizacaoPermiteVoltar,

                    configuracao.FimViagemDescricao,
                    configuracao.FimViagemPermiteQRCode,
                    configuracao.FimViagemPermiteVoltar,

                    configuracao.GuaritaEntradaDescricao,
                    configuracao.GuaritaEntradaPermiteQRCode,
                    configuracao.GuaritaEntradaPermiteVoltar,
                    configuracao.GuaritaEntradaAction,
                    GuaritaEntradaTipoIntegracao = new { value = configuracao.GuaritaEntradaTipoIntegracao, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoHelper.ObterDescricao(configuracao.GuaritaEntradaTipoIntegracao) },

                    configuracao.GuaritaSaidaDescricao,
                    configuracao.GuaritaSaidaPermiteQRCode,
                    configuracao.GuaritaSaidaPermiteVoltar,
                    GuaritaSaidaTipoIntegracao = new { value = configuracao.GuaritaSaidaTipoIntegracao, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoHelper.ObterDescricao(configuracao.GuaritaSaidaTipoIntegracao) },

                    configuracao.InformarDocaCarregamentoDescricao,
                    configuracao.InformarDocaCarregamentoPermiteQRCode,
                    configuracao.InformarDocaCarregamentoPermiteVoltar,
                    configuracao.InformarDocaCarregamentoUtilizarLocalCarregamento,
                    configuracao.NaoPermitirInformarMaisDeUmVeiculoPorVezNaDoca,
                    configuracao.GerarOcorrenciaPedidoEtapaDocaCarregamento,
                    TipoDeOcorrencia = new { Codigo = configuracao.TipoDeOcorrencia?.Codigo ?? 0, Descricao = configuracao.TipoDeOcorrencia?.Descricao ?? "" },

                    configuracao.InicioCarregamentoDescricao,
                    configuracao.InicioCarregamentoPermiteQRCode,
                    configuracao.InicioCarregamentoPermiteVoltar,

                    configuracao.InicioDescarregamentoDescricao,
                    configuracao.InicioDescarregamentoPermiteQRCode,
                    configuracao.InicioDescarregamentoPermiteVoltar,

                    configuracao.InicioHigienizacaoDescricao,
                    configuracao.InicioHigienizacaoPermiteAvancarAutomaticamenteComVeiculosHigienizados,
                    configuracao.InicioHigienizacaoPermiteQRCode,
                    configuracao.InicioHigienizacaoPermiteVoltar,

                    configuracao.LiberaChaveDescricao,
                    configuracao.LiberaChavePermiteQRCode,
                    configuracao.LiberaChavePermiteVoltar,

                    configuracao.MontagemCargaDescricao,
                    configuracao.MontagemCargaCodigoControle,
                    configuracao.MontagemCargaPermiteQRCode,
                    configuracao.MontagemCargaPermiteVoltar,

                    configuracao.PosicaoDescricao,
                    configuracao.PosicaoPermiteQRCode,
                    configuracao.PosicaoPermiteVoltar,

                    configuracao.SaidaLojaDescricao,
                    configuracao.SaidaLojaPermiteQRCode,
                    configuracao.SaidaLojaPermiteVoltar,

                    configuracao.SeparacaoMercadoriaDescricao,
                    configuracao.SeparacaoMercadoriaPermiteQRCode,
                    configuracao.SeparacaoMercadoriaPermiteVoltar,

                    configuracao.AvaliacaoDescargaDescricao,
                    configuracao.AvaliacaoDescargaPermiteVoltar,

                    configuracao.SolicitacaoVeiculoDescricao,
                    configuracao.SolicitacaoVeiculoPermiteQRCode,
                    configuracao.SolicitacaoVeiculoPermiteVoltar,
                    configuracao.SolicitacaoVeiculoPermiteEnvioSMSMotorista,
                    SolicitacaoVeiculoTipoIntegracao = new { value = configuracao.SolicitacaoVeiculoTipoIntegracao, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoHelper.ObterDescricao(configuracao.SolicitacaoVeiculoTipoIntegracao) },

                    configuracao.TravaChaveDescricao,
                    configuracao.TravaChavePermiteQRCode,
                    configuracao.TravaChavePermiteVoltar,

                    configuracao.ExibirComprovanteSaida,
                    configuracao.OcultarFluxoCarga,
                    configuracao.DocaDetalhada,
                    configuracao.OcultarTransportador,
                    configuracao.HabilitarPreCarga,
                    configuracao.ExibirTempoPrevistoERealizado,
                    configuracao.PermitirRejeicaoFluxo,
                    configuracao.ListarCargasCanceladas,
                    configuracao.IdentificacaoFluxoExibirOrigemXDestinos,
                    configuracao.IdentificacaoFluxoExibirTipoOperacao,
                    configuracao.IdentificacaoFluxoExibirCodigoIntegracaoFilial,
                    configuracao.ExibirSiglaFilial,
                    configuracao.IdentificacaoFluxoExibirModeloVeicularCargaVeiculo,
                    configuracao.SempreExibirPrevistoXRealizadoEDiferenca,
                    configuracao.SempreAtualizarDataPrevistaAoAlterarHorarioCarregamento,
                    configuracao.VisualizarPlacaReboque,
                    configuracao.VisualizarPlacaTracao,
                    configuracao.PermiteCancelarFluxoPatioAtual,
                    configuracao.AvancarCargaAgrupadaApenasComAsCargasFilhasAvancadas,
                    configuracao.IniciarFluxoPatioSomenteComCarregamentoAgendado,


                    MacroInicioViagem = configuracao.MacroInicioViagem != null ? new { configuracao.MacroInicioViagem.Codigo, configuracao.MacroInicioViagem.Descricao } : null,
                    MacroChegadaDestinatario = configuracao.MacroChegadaDestinatario != null ? new { configuracao.MacroChegadaDestinatario.Codigo, configuracao.MacroChegadaDestinatario.Descricao } : null,
                    MacroSaidaDestinatario = configuracao.MacroSaidaDestinatario != null ? new { configuracao.MacroSaidaDestinatario.Codigo, configuracao.MacroSaidaDestinatario.Descricao } : null,
                    MacroFimViagem = configuracao.MacroFimViagem != null ? new { configuracao.MacroFimViagem.Codigo, configuracao.MacroFimViagem.Descricao } : null,

                    configuracao.IniciarViagemSemGuarita,
                    configuracao.HabilitarObservacaoEtapa,
                    configuracao.ExibirDetalhesIdentificacaoFluxo,

                    configuracao.RelatorioFluxoHorarioQuantidadeBaixa,
                    configuracao.RelatorioFluxoHorarioQuantidadeNormal,
                    configuracao.RelatorioFluxoHorarioQuantidadeAlta,

                    configuracao.GerarFluxoDestinoAntesFinalizarOrigem,

                    // Notificações no app
                    configuracao.InformarDocaCarregamentoNotificarMotoristaApp,
                    configuracao.MontagemCargaNotificarMotoristaApp,
                    configuracao.MontagemCargaPermiteGerarAtendimento,
                    configuracao.ChegadaVeiculoNotificarMotoristaApp,
                    configuracao.GuaritaEntradaNotificarMotoristaApp,
                    configuracao.CheckListNotificarMotoristaApp,
                    configuracao.TravaChaveNotificarMotoristaApp,
                    configuracao.TravaChavePermiteGerarAtendimento,
                    configuracao.ExpedicaoNotificarMotoristaApp,
                    configuracao.LiberaChaveNotificarMotoristaApp,
                    configuracao.FaturamentoNotificarMotoristaApp,
                    configuracao.FaturamentoPermiteImprimirCapaViagem,
                    configuracao.FaturamentoMensagemCapaViagem,
                    configuracao.PosicaoNotificarMotoristaApp,
                    configuracao.ChegadaLojaNotificarMotoristaApp,
                    configuracao.DeslocamentoPatioNotificarMotoristaApp,
                    configuracao.DocumentoFiscalNotificarMotoristaApp,
                    configuracao.DocumentosTransporteNotificarMotoristaApp,
                    configuracao.SaidaLojaNotificarMotoristaApp,
                    configuracao.FimViagemNotificarMotoristaApp,
                    configuracao.InicioHigienizacaoNotificarMotoristaApp,
                    configuracao.FimHigienizacaoNotificarMotoristaApp,
                    configuracao.InicioCarregamentoNotificarMotoristaApp,
                    configuracao.FimCarregamentoNotificarMotoristaApp,
                    configuracao.SeparacaoMercadoriaNotificarMotoristaApp,
                    configuracao.SolicitacaoVeiculoNotificarMotoristaApp,
                    configuracao.GuaritaSaidaNotificarMotoristaApp,
                    configuracao.GuaritaSaidaIniciarViagemControleEntregaAoFinalizarEtapa,
                    configuracao.InicioDescarregamentoNotificarMotoristaApp,
                    configuracao.FimDescarregamentoNotificarMotoristaApp,
                    configuracao.ObrigatorioInformarDataInicial,
                    configuracao.IntegrarFluxoPatioWMS,
                    configuracao.UtilizarFluxoPatioCargaCanceladaAoReenviarCarga,

                    //Antecipar Etapa
                    configuracao.MontagemCargaPermiteAntecipar,
                    configuracao.ChegadaVeiculoPermiteAntecipar,
                    configuracao.GuaritaEntradaPermiteAntecipar,
                    configuracao.ChecklistPermiteAntecipar,
                    configuracao.InformarDocaPermiteAntecipar,
                    configuracao.UtilizarDataPrevistaEtapaAtualAtivarAlerta,
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repConfiguracaoGestaoPatio.BuscarConfiguracao();

                if (configuracaoGestaoPatio == null)
                    configuracaoGestaoPatio = new Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio();
                else
                    configuracaoGestaoPatio.Initialize();

                PreencheEntidade(configuracaoGestaoPatio, unitOfWork);

                unitOfWork.Start();

                if (configuracaoGestaoPatio.Codigo == 0)
                    repConfiguracaoGestaoPatio.Inserir(configuracaoGestaoPatio, Auditado);
                else
                    repConfiguracaoGestaoPatio.Atualizar(configuracaoGestaoPatio, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Filiais.ConfiguracaoGestaoPatio.OcorreuFalhaAoAtualizarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarTiposIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposExistentes = repositorioTipoIntegracao.BuscarTipos();

                List<dynamic> retorno = new List<dynamic>();
                int posicaoEnum = 0;

                foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposExistentes)
                {
                    retorno.Insert(posicaoEnum, new { value = tipoIntegracao, text = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoHelper.ObterDescricao(tipoIntegracao) });

                    posicaoEnum++;
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os tipos de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencheEntidade(Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.Macro repMacro = new Repositorio.Embarcador.Veiculos.Macro(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrenciaCte = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            configuracaoGestaoPatio.CheckListDescricao = Request.GetNullableStringParam("CheckListDescricao");
            configuracaoGestaoPatio.CheckListPermiteQRCode = Request.GetBoolParam("CheckListPermiteQRCode");
            configuracaoGestaoPatio.CheckListPermiteVoltar = Request.GetBoolParam("CheckListPermiteVoltar");
            configuracaoGestaoPatio.UtilizarCategoriaDeReboqueConformeModeloVeicularCarga = Request.GetBoolParam("UtilizarCategoriaDeReboqueConformeModeloVeicularCarga");
            configuracaoGestaoPatio.CheckListPermiteSalvarSemPreencher = Request.GetBoolParam("CheckListPermiteSalvarSemPreencher");
            configuracaoGestaoPatio.ChecklistTipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("ChecklistTipoIntegracao");

            configuracaoGestaoPatio.ChegadaLojaDescricao = Request.GetNullableStringParam("ChegadaLojaDescricao");
            configuracaoGestaoPatio.ChegadaLojaPermiteQRCode = Request.GetBoolParam("ChegadaLojaPermiteQRCode");
            configuracaoGestaoPatio.ChegadaLojaPermiteVoltar = Request.GetBoolParam("ChegadaLojaPermiteVoltar");

            configuracaoGestaoPatio.ChegadaVeiculoDescricao = Request.GetNullableStringParam("ChegadaVeiculoDescricao");
            configuracaoGestaoPatio.ChegadaVeiculoPermiteQRCode = Request.GetBoolParam("ChegadaVeiculoPermiteQRCode");
            configuracaoGestaoPatio.ChegadaVeiculoPermiteVoltar = Request.GetBoolParam("ChegadaVeiculoPermiteVoltar");
            configuracaoGestaoPatio.ChegadaVeiculoPermiteAvancarAutomaticamenteAposInformarDadosTransporteCarga = Request.GetBoolParam("ChegadaVeiculoPermiteAvancarAutomaticamenteAposInformarDadosTransporteCarga");
            configuracaoGestaoPatio.ChegadaVeiculoPermiteInformarComEtapaBloqueada = Request.GetBoolParam("ChegadaVeiculoPermiteInformarComEtapaBloqueada");
            configuracaoGestaoPatio.ChegadaVeiculoTipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("ChegadaVeiculoTipoIntegracao");
            configuracaoGestaoPatio.ChegadaVeiculoAction = Request.GetStringParam("ChegadaVeiculoAction");
            configuracaoGestaoPatio.PermiteGerarAtendimento = Request.GetBoolParam("PermiteGerarAtendimento");

            configuracaoGestaoPatio.DeslocamentoPatioDescricao = Request.GetNullableStringParam("DeslocamentoPatioDescricao");
            configuracaoGestaoPatio.DeslocamentoPatioPermiteQRCode = Request.GetBoolParam("DeslocamentoPatioPermiteQRCode");
            configuracaoGestaoPatio.DeslocamentoPatioPermiteVoltar = Request.GetBoolParam("DeslocamentoPatioPermiteVoltar");

            configuracaoGestaoPatio.DocumentoFiscalDescricao = Request.GetNullableStringParam("DocumentoFiscalDescricao");
            configuracaoGestaoPatio.DocumentoFiscalPermiteQRCode = Request.GetBoolParam("DocumentoFiscalPermiteQRCode");
            configuracaoGestaoPatio.DocumentoFiscalPermiteVoltar = Request.GetBoolParam("DocumentoFiscalPermiteVoltar");
            configuracaoGestaoPatio.DocumentoFiscalPermiteAvancarAutomaticamenteAposNotasFiscaisInseridas = Request.GetBoolParam("DocumentoFiscalPermiteAvancarAutomaticamenteAposNotasFiscaisInseridas");

            configuracaoGestaoPatio.DocumentosTransporteDescricao = Request.GetNullableStringParam("DocumentosTransporteDescricao");
            configuracaoGestaoPatio.DocumentosTransportePermiteQRCode = Request.GetBoolParam("DocumentosTransportePermiteQRCode");
            configuracaoGestaoPatio.DocumentosTransportePermiteVoltar = Request.GetBoolParam("DocumentosTransportePermiteVoltar");
            configuracaoGestaoPatio.DocumentosTransportePermiteAvancarAutomaticamenteAposGerarDocumentos = Request.GetBoolParam("DocumentosTransportePermiteAvancarAutomaticamenteAposGerarDocumentos");
            configuracaoGestaoPatio.DocumentosTransporteTipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("DocumentosTransporteTipoIntegracao");

            configuracaoGestaoPatio.ExpedicaoDescricao = Request.GetNullableStringParam("ExpedicaoDescricao");
            configuracaoGestaoPatio.ExpedicaoPermiteQRCode = Request.GetBoolParam("ExpedicaoPermiteQRCode");
            configuracaoGestaoPatio.ExpedicaoPermiteVoltar = Request.GetBoolParam("ExpedicaoPermiteVoltar");

            configuracaoGestaoPatio.FaturamentoDescricao = Request.GetNullableStringParam("FaturamentoDescricao");
            configuracaoGestaoPatio.FaturamentoPermiteAvancarAutomaticamenteAposGerarDocumentos = Request.GetBoolParam("FaturamentoPermiteAvancarAutomaticamenteAposGerarDocumentos");
            configuracaoGestaoPatio.FaturamentoPermiteSolicitarNotasFiscaisEtapaBloqueada = Request.GetBoolParam("FaturamentoPermiteSolicitarNotasFiscaisEtapaBloqueada");
            configuracaoGestaoPatio.FaturamentoPermiteQRCode = Request.GetBoolParam("FaturamentoPermiteQRCode");
            configuracaoGestaoPatio.FaturamentoPermiteVoltar = Request.GetBoolParam("FaturamentoPermiteVoltar");

            configuracaoGestaoPatio.FimCarregamentoDescricao = Request.GetNullableStringParam("FimCarregamentoDescricao");
            configuracaoGestaoPatio.FimCarregamentoPermiteQRCode = Request.GetBoolParam("FimCarregamentoPermiteQRCode");
            configuracaoGestaoPatio.FimCarregamentoPermiteVoltar = Request.GetBoolParam("FimCarregamentoPermiteVoltar");
            configuracaoGestaoPatio.FimCarregamentoPermiteAvancarSomenteDadosTransporteInformados = Request.GetBoolParam("FimCarregamentoPermiteAvancarSomenteDadosTransporteInformados");

            configuracaoGestaoPatio.FimDescarregamentoDescricao = Request.GetNullableStringParam("FimDescarregamentoDescricao");
            configuracaoGestaoPatio.FimDescarregamentoPermiteQRCode = Request.GetBoolParam("FimDescarregamentoPermiteQRCode");
            configuracaoGestaoPatio.FimDescarregamentoPermiteVoltar = Request.GetBoolParam("FimDescarregamentoPermiteVoltar");

            configuracaoGestaoPatio.FimHigienizacaoDescricao = Request.GetNullableStringParam("FimHigienizacaoDescricao");
            configuracaoGestaoPatio.FimHigienizacaoPermiteAvancarAutomaticamenteComVeiculosHigienizados = Request.GetBoolParam("FimHigienizacaoPermiteAvancarAutomaticamenteComVeiculosHigienizados");
            configuracaoGestaoPatio.FimHigienizacaoPermiteQRCode = Request.GetBoolParam("FimHigienizacaoPermiteQRCode");
            configuracaoGestaoPatio.FimHigienizacaoPermiteVoltar = Request.GetBoolParam("FimHigienizacaoPermiteVoltar");

            configuracaoGestaoPatio.FimViagemDescricao = Request.GetNullableStringParam("FimViagemDescricao");
            configuracaoGestaoPatio.FimViagemPermiteQRCode = Request.GetBoolParam("FimViagemPermiteQRCode");
            configuracaoGestaoPatio.FimViagemPermiteVoltar = Request.GetBoolParam("FimViagemPermiteVoltar");

            configuracaoGestaoPatio.GuaritaEntradaDescricao = Request.GetNullableStringParam("GuaritaEntradaDescricao");
            configuracaoGestaoPatio.GuaritaEntradaPermiteQRCode = Request.GetBoolParam("GuaritaEntradaPermiteQRCode");
            configuracaoGestaoPatio.GuaritaEntradaPermiteVoltar = Request.GetBoolParam("GuaritaEntradaPermiteVoltar");
            configuracaoGestaoPatio.GuaritaEntradaAction = Request.GetStringParam("GuaritaEntradaAction");
            configuracaoGestaoPatio.GuaritaEntradaTipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("GuaritaEntradaTipoIntegracao");

            configuracaoGestaoPatio.GuaritaSaidaDescricao = Request.GetNullableStringParam("GuaritaSaidaDescricao");
            configuracaoGestaoPatio.GuaritaSaidaPermiteQRCode = Request.GetBoolParam("GuaritaSaidaPermiteQRCode");
            configuracaoGestaoPatio.GuaritaSaidaPermiteVoltar = Request.GetBoolParam("GuaritaSaidaPermiteVoltar");
            configuracaoGestaoPatio.GuaritaSaidaTipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("GuaritaSaidaTipoIntegracao");

            configuracaoGestaoPatio.InformarDocaCarregamentoDescricao = Request.GetNullableStringParam("InformarDocaCarregamentoDescricao");
            configuracaoGestaoPatio.InformarDocaCarregamentoPermiteQRCode = Request.GetBoolParam("InformarDocaCarregamentoPermiteQRCode");
            configuracaoGestaoPatio.InformarDocaCarregamentoPermiteVoltar = Request.GetBoolParam("InformarDocaCarregamentoPermiteVoltar");
            configuracaoGestaoPatio.InformarDocaCarregamentoUtilizarLocalCarregamento = Request.GetBoolParam("InformarDocaCarregamentoUtilizarLocalCarregamento");
            configuracaoGestaoPatio.NaoPermitirInformarMaisDeUmVeiculoPorVezNaDoca = Request.GetBoolParam("NaoPermitirInformarMaisDeUmVeiculoPorVezNaDoca");
            configuracaoGestaoPatio.GerarOcorrenciaPedidoEtapaDocaCarregamento = Request.GetBoolParam("GerarOcorrenciaPedidoEtapaDocaCarregamento");
            configuracaoGestaoPatio.InformarDocaCarregamentoTipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("InformarDocaCarregamentoTipoIntegracao");
            configuracaoGestaoPatio.InformarDocaCarregamentoTipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("InformarDocaCarregamentoTipoIntegracao");
            configuracaoGestaoPatio.InicioCarregamentoTipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("InicioCarregamentoTipoIntegracao");
            configuracaoGestaoPatio.FimCarregamentoTipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("FimCarregamentoTipoIntegracao");

            configuracaoGestaoPatio.InicioCarregamentoDescricao = Request.GetNullableStringParam("InicioCarregamentoDescricao");
            configuracaoGestaoPatio.InicioCarregamentoPermiteQRCode = Request.GetBoolParam("InicioCarregamentoPermiteQRCode");
            configuracaoGestaoPatio.InicioCarregamentoPermiteVoltar = Request.GetBoolParam("InicioCarregamentoPermiteVoltar");

            configuracaoGestaoPatio.InicioDescarregamentoDescricao = Request.GetNullableStringParam("InicioDescarregamentoDescricao");
            configuracaoGestaoPatio.InicioDescarregamentoPermiteQRCode = Request.GetBoolParam("InicioDescarregamentoPermiteQRCode");
            configuracaoGestaoPatio.InicioDescarregamentoPermiteVoltar = Request.GetBoolParam("InicioDescarregamentoPermiteVoltar");

            configuracaoGestaoPatio.InicioHigienizacaoDescricao = Request.GetNullableStringParam("InicioHigienizacaoDescricao");
            configuracaoGestaoPatio.InicioHigienizacaoPermiteAvancarAutomaticamenteComVeiculosHigienizados = Request.GetBoolParam("InicioHigienizacaoPermiteAvancarAutomaticamenteComVeiculosHigienizados");
            configuracaoGestaoPatio.InicioHigienizacaoPermiteQRCode = Request.GetBoolParam("InicioHigienizacaoPermiteQRCode");
            configuracaoGestaoPatio.InicioHigienizacaoPermiteVoltar = Request.GetBoolParam("InicioHigienizacaoPermiteVoltar");

            configuracaoGestaoPatio.LiberaChaveDescricao = Request.GetNullableStringParam("LiberaChaveDescricao");
            configuracaoGestaoPatio.LiberaChavePermiteQRCode = Request.GetBoolParam("LiberaChavePermiteQRCode");
            configuracaoGestaoPatio.LiberaChavePermiteVoltar = Request.GetBoolParam("LiberaChavePermiteVoltar");

            configuracaoGestaoPatio.MontagemCargaDescricao = Request.GetNullableStringParam("MontagemCargaDescricao");
            configuracaoGestaoPatio.MontagemCargaCodigoControle = Request.GetNullableStringParam("MontagemCargaCodigoControle");
            configuracaoGestaoPatio.MontagemCargaPermiteQRCode = Request.GetBoolParam("MontagemCargaPermiteQRCode");
            configuracaoGestaoPatio.MontagemCargaPermiteVoltar = Request.GetBoolParam("MontagemCargaPermiteVoltar");

            configuracaoGestaoPatio.PosicaoDescricao = Request.GetNullableStringParam("PosicaoDescricao");
            configuracaoGestaoPatio.PosicaoPermiteQRCode = Request.GetBoolParam("PosicaoPermiteQRCode");
            configuracaoGestaoPatio.PosicaoPermiteVoltar = Request.GetBoolParam("PosicaoPermiteVoltar");

            configuracaoGestaoPatio.SaidaLojaDescricao = Request.GetNullableStringParam("SaidaLojaDescricao");
            configuracaoGestaoPatio.SaidaLojaPermiteQRCode = Request.GetBoolParam("SaidaLojaPermiteQRCode");
            configuracaoGestaoPatio.SaidaLojaPermiteVoltar = Request.GetBoolParam("SaidaLojaPermiteVoltar");

            configuracaoGestaoPatio.SeparacaoMercadoriaDescricao = Request.GetNullableStringParam("SeparacaoMercadoriaDescricao");
            configuracaoGestaoPatio.SeparacaoMercadoriaPermiteQRCode = Request.GetBoolParam("SeparacaoMercadoriaPermiteQRCode");
            configuracaoGestaoPatio.SeparacaoMercadoriaPermiteVoltar = Request.GetBoolParam("SeparacaoMercadoriaPermiteVoltar");

            configuracaoGestaoPatio.AvaliacaoDescargaDescricao = Request.GetNullableStringParam("AvaliacaoDescargaDescricao");
            configuracaoGestaoPatio.AvaliacaoDescargaPermiteVoltar = Request.GetBoolParam("AvaliacaoDescargaPermiteVoltar");

            configuracaoGestaoPatio.SolicitacaoVeiculoDescricao = Request.GetNullableStringParam("SolicitacaoVeiculoDescricao");
            configuracaoGestaoPatio.SolicitacaoVeiculoPermiteQRCode = Request.GetBoolParam("SolicitacaoVeiculoPermiteQRCode");
            configuracaoGestaoPatio.SolicitacaoVeiculoPermiteVoltar = Request.GetBoolParam("SolicitacaoVeiculoPermiteVoltar");
            configuracaoGestaoPatio.SolicitacaoVeiculoPermiteEnvioSMSMotorista = Request.GetBoolParam("SolicitacaoVeiculoPermiteEnvioSMSMotorista");
            configuracaoGestaoPatio.SolicitacaoVeiculoTipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("SolicitacaoVeiculoTipoIntegracao");

            configuracaoGestaoPatio.TravaChaveDescricao = Request.GetNullableStringParam("TravaChaveDescricao");
            configuracaoGestaoPatio.TravaChavePermiteQRCode = Request.GetBoolParam("TravaChavePermiteQRCode");
            configuracaoGestaoPatio.TravaChavePermiteVoltar = Request.GetBoolParam("TravaChavePermiteVoltar");

            configuracaoGestaoPatio.MacroInicioViagem = repMacro.BuscarPorCodigo(Request.GetIntParam("MacroInicioViagem"));
            configuracaoGestaoPatio.MacroChegadaDestinatario = repMacro.BuscarPorCodigo(Request.GetIntParam("MacroChegadaDestinatario"));
            configuracaoGestaoPatio.MacroSaidaDestinatario = repMacro.BuscarPorCodigo(Request.GetIntParam("MacroSaidaDestinatario"));
            configuracaoGestaoPatio.MacroFimViagem = repMacro.BuscarPorCodigo(Request.GetIntParam("MacroFimViagem"));
            configuracaoGestaoPatio.TipoDeOcorrencia = repTipoOcorrenciaCte.BuscarPorCodigo(Request.GetIntParam("TipoDeOcorrencia"));

            configuracaoGestaoPatio.RelatorioFluxoHorarioQuantidadeBaixa = Request.GetIntParam("RelatorioFluxoHorarioQuantidadeBaixa");
            configuracaoGestaoPatio.RelatorioFluxoHorarioQuantidadeNormal = Request.GetIntParam("RelatorioFluxoHorarioQuantidadeNormal");
            configuracaoGestaoPatio.RelatorioFluxoHorarioQuantidadeAlta = Request.GetIntParam("RelatorioFluxoHorarioQuantidadeAlta");

            configuracaoGestaoPatio.ViewFluxoPatioTabelado = Request.GetBoolParam("ViewFluxoPatioTabelado");
            configuracaoGestaoPatio.ExibirComprovanteSaida = Request.GetBoolParam("ExibirComprovanteSaida");
            configuracaoGestaoPatio.IniciarViagemSemGuarita = Request.GetBoolParam("IniciarViagemSemGuarita");
            configuracaoGestaoPatio.HabilitarObservacaoEtapa = Request.GetBoolParam("HabilitarObservacaoEtapa");
            configuracaoGestaoPatio.ExibirDetalhesIdentificacaoFluxo = Request.GetBoolParam("ExibirDetalhesIdentificacaoFluxo");

            configuracaoGestaoPatio.OcultarFluxoCarga = Request.GetBoolParam("OcultarFluxoCarga");
            configuracaoGestaoPatio.DocaDetalhada = Request.GetBoolParam("DocaDetalhada");
            configuracaoGestaoPatio.OcultarTransportador = Request.GetBoolParam("OcultarTransportador");
            configuracaoGestaoPatio.HabilitarPreCarga = Request.GetBoolParam("HabilitarPreCarga");
            configuracaoGestaoPatio.ExibirTempoPrevistoERealizado = Request.GetBoolParam("ExibirTempoPrevistoERealizado");
            configuracaoGestaoPatio.PermitirRejeicaoFluxo = Request.GetBoolParam("PermitirRejeicaoFluxo");
            configuracaoGestaoPatio.ListarCargasCanceladas = Request.GetBoolParam("ListarCargasCanceladas");
            configuracaoGestaoPatio.IdentificacaoFluxoExibirOrigemXDestinos = Request.GetBoolParam("IdentificacaoFluxoExibirOrigemXDestinos");
            configuracaoGestaoPatio.IdentificacaoFluxoExibirTipoOperacao = Request.GetBoolParam("IdentificacaoFluxoExibirTipoOperacao");
            configuracaoGestaoPatio.IdentificacaoFluxoExibirCodigoIntegracaoFilial = Request.GetBoolParam("IdentificacaoFluxoExibirCodigoIntegracaoFilial");
            configuracaoGestaoPatio.ExibirSiglaFilial = Request.GetBoolParam("ExibirSiglaFilial");
            configuracaoGestaoPatio.IdentificacaoFluxoExibirModeloVeicularCargaVeiculo = Request.GetBoolParam("IdentificacaoFluxoExibirModeloVeicularCargaVeiculo");
            configuracaoGestaoPatio.SempreExibirPrevistoXRealizadoEDiferenca = Request.GetBoolParam("SempreExibirPrevistoXRealizadoEDiferenca");
            configuracaoGestaoPatio.SempreAtualizarDataPrevistaAoAlterarHorarioCarregamento = Request.GetBoolParam("SempreAtualizarDataPrevistaAoAlterarHorarioCarregamento");
            configuracaoGestaoPatio.VisualizarPlacaReboque = Request.GetBoolParam("VisualizarPlacaReboque");
            configuracaoGestaoPatio.VisualizarPlacaTracao = Request.GetBoolParam("VisualizarPlacaTracao");
            configuracaoGestaoPatio.AvancarCargaAgrupadaApenasComAsCargasFilhasAvancadas = Request.GetBoolParam("AvancarCargaAgrupadaApenasComAsCargasFilhasAvancadas");
            configuracaoGestaoPatio.IniciarFluxoPatioSomenteComCarregamentoAgendado = Request.GetBoolParam("IniciarFluxoPatioSomenteComCarregamentoAgendado");
            configuracaoGestaoPatio.PermiteCancelarFluxoPatioAtual = Request.GetBoolParam("PermiteCancelarFluxoPatioAtual");

            // Notificações no app
            configuracaoGestaoPatio.InformarDocaCarregamentoNotificarMotoristaApp = Request.GetBoolParam("InformarDocaCarregamentoNotificarMotoristaApp");
            configuracaoGestaoPatio.MontagemCargaNotificarMotoristaApp = Request.GetBoolParam("MontagemCargaNotificarMotoristaApp");
            configuracaoGestaoPatio.MontagemCargaPermiteGerarAtendimento = Request.GetBoolParam("MontagemCargaPermiteGerarAtendimento");
            configuracaoGestaoPatio.ChegadaVeiculoNotificarMotoristaApp = Request.GetBoolParam("ChegadaVeiculoNotificarMotoristaApp");
            configuracaoGestaoPatio.GuaritaEntradaNotificarMotoristaApp = Request.GetBoolParam("GuaritaEntradaNotificarMotoristaApp");
            configuracaoGestaoPatio.CheckListNotificarMotoristaApp = Request.GetBoolParam("CheckListNotificarMotoristaApp");
            configuracaoGestaoPatio.TravaChaveNotificarMotoristaApp = Request.GetBoolParam("TravaChaveNotificarMotoristaApp");
            configuracaoGestaoPatio.TravaChavePermiteGerarAtendimento = Request.GetBoolParam("TravaChavePermiteGerarAtendimento");
            configuracaoGestaoPatio.ExpedicaoNotificarMotoristaApp = Request.GetBoolParam("ExpedicaoNotificarMotoristaApp");
            configuracaoGestaoPatio.LiberaChaveNotificarMotoristaApp = Request.GetBoolParam("LiberaChaveNotificarMotoristaApp");
            configuracaoGestaoPatio.FaturamentoNotificarMotoristaApp = Request.GetBoolParam("FaturamentoNotificarMotoristaApp");
            configuracaoGestaoPatio.FaturamentoPermiteImprimirCapaViagem = Request.GetBoolParam("FaturamentoPermiteImprimirCapaViagem");
            configuracaoGestaoPatio.FaturamentoMensagemCapaViagem = Request.GetStringParam("FaturamentoMensagemCapaViagem");
            configuracaoGestaoPatio.PosicaoNotificarMotoristaApp = Request.GetBoolParam("PosicaoNotificarMotoristaApp");
            configuracaoGestaoPatio.ChegadaLojaNotificarMotoristaApp = Request.GetBoolParam("ChegadaLojaNotificarMotoristaApp");
            configuracaoGestaoPatio.DeslocamentoPatioNotificarMotoristaApp = Request.GetBoolParam("DeslocamentoPatioNotificarMotoristaApp");
            configuracaoGestaoPatio.DocumentoFiscalNotificarMotoristaApp = Request.GetBoolParam("DocumentoFiscalNotificarMotoristaApp");
            configuracaoGestaoPatio.DocumentosTransporteNotificarMotoristaApp = Request.GetBoolParam("DocumentosTransporteNotificarMotoristaApp");
            configuracaoGestaoPatio.SaidaLojaNotificarMotoristaApp = Request.GetBoolParam("SaidaLojaNotificarMotoristaApp");
            configuracaoGestaoPatio.FimViagemNotificarMotoristaApp = Request.GetBoolParam("FimViagemNotificarMotoristaApp");
            configuracaoGestaoPatio.InicioHigienizacaoNotificarMotoristaApp = Request.GetBoolParam("InicioHigienizacaoNotificarMotoristaApp");
            configuracaoGestaoPatio.FimHigienizacaoNotificarMotoristaApp = Request.GetBoolParam("FimHigienizacaoNotificarMotoristaApp");
            configuracaoGestaoPatio.InicioCarregamentoNotificarMotoristaApp = Request.GetBoolParam("InicioCarregamentoNotificarMotoristaApp");
            configuracaoGestaoPatio.FimCarregamentoNotificarMotoristaApp = Request.GetBoolParam("FimCarregamentoNotificarMotoristaApp");
            configuracaoGestaoPatio.SeparacaoMercadoriaNotificarMotoristaApp = Request.GetBoolParam("SeparacaoMercadoriaNotificarMotoristaApp");
            configuracaoGestaoPatio.SolicitacaoVeiculoNotificarMotoristaApp = Request.GetBoolParam("SolicitacaoVeiculoNotificarMotoristaApp");
            configuracaoGestaoPatio.GuaritaSaidaNotificarMotoristaApp = Request.GetBoolParam("GuaritaSaidaNotificarMotoristaApp");
            configuracaoGestaoPatio.GuaritaSaidaIniciarViagemControleEntregaAoFinalizarEtapa = Request.GetBoolParam("GuaritaSaidaIniciarViagemControleEntregaAoFinalizarEtapa");
            configuracaoGestaoPatio.InicioDescarregamentoNotificarMotoristaApp = Request.GetBoolParam("InicioDescarregamentoNotificarMotoristaApp");
            configuracaoGestaoPatio.FimDescarregamentoNotificarMotoristaApp = Request.GetBoolParam("FimDescarregamentoNotificarMotoristaApp");
            configuracaoGestaoPatio.ObrigatorioInformarDataInicial = Request.GetBoolParam("ObrigatorioInformarDataInicial");
            configuracaoGestaoPatio.IntegrarFluxoPatioWMS = Request.GetBoolParam("IntegrarFluxoPatioWMS");
            configuracaoGestaoPatio.UtilizarFluxoPatioCargaCanceladaAoReenviarCarga = Request.GetBoolParam("UtilizarFluxoPatioCargaCanceladaAoReenviarCarga");
            configuracaoGestaoPatio.GerarFluxoDestinoAntesFinalizarOrigem = Request.GetBoolParam("GerarFluxoDestinoAntesFinalizarOrigem");

            //Antecipar Etapas
            configuracaoGestaoPatio.MontagemCargaPermiteAntecipar = Request.GetBoolParam("MontagemCargaPermiteAntecipar");
            configuracaoGestaoPatio.ChegadaVeiculoPermiteAntecipar = Request.GetBoolParam("ChegadaVeiculoPermiteAntecipar");
            configuracaoGestaoPatio.GuaritaEntradaPermiteAntecipar = Request.GetBoolParam("GuaritaEntradaPermiteAntecipar");
            configuracaoGestaoPatio.ChecklistPermiteAntecipar = Request.GetBoolParam("ChecklistPermiteAntecipar");
            configuracaoGestaoPatio.InformarDocaPermiteAntecipar = Request.GetBoolParam("InformarDocaPermiteAntecipar");
            configuracaoGestaoPatio.UtilizarDataPrevistaEtapaAtualAtivarAlerta = Request.GetBoolParam("UtilizarDataPrevistaEtapaAtualAtivarAlerta");
        }

        #endregion
    }
}
