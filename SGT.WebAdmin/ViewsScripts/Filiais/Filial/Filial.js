/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/SetorFuncionario.js" />
/// <reference path="../../Consultas/ConfiguracaoCIOT.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="CondicaoPagamento.js" />
/// <reference path="EmpresaEmissora.js" />
/// <reference path="EstadoDestinoEmpresaEmissora.js" />
/// <reference path="GestaPatio.js" />
/// <reference path="OutrosCodigosIntegracao.js" />
/// <reference path="SetorFilial.js" />
/// <reference path="TipoOperacao.js" />
/// <reference path="FilialSemParar.js" />
/// <reference path="Certificado.js" />
/// <reference path="FilialBalanca.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridFilial;
var _filial;
var _pesquisaFilial;
var _CRUDFilial;
var _tiposIntegracaoValePedagio;

//var _TipoFilial = [
//    { text: "Filial", value: 1 },
//    { text: "Matriz", value: 2 },
//    { text: "Agência", value: 3 }
//];

//var _Atividade = [
//    { text: "1 - Serviços da Mesma", value: 1 },
//    { text: "2 - Industrial", value: 2 },
//    { text: "3 - Comercial", value: 3 },
//    { text: "4 - Prestadora de Serviço", value: 4 },
//    { text: "5 - Distribuidora de Ene", value: 5 },
//    { text: "6 - Produtor Rural", value: 6 },
//    { text: "7 - Não Contribuínte", value: 7 }
//];

/*
 * Declaração das Classes
 */

var PesquisaFilial = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), maxlength: 50 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFilial.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var Filial = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), issue: 586, required: true });
    this.CodigoFilialEmbarcador = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), issue: 15, maxlength: 50 });
    this.CNPJ = PropertyEntity({ text: Localization.Resources.Filiais.Filial.CNPJ.getRequiredFieldDescription(), maxlength: 50, required: true, getType: typesKnockout.cnpj });
    this.TipoFilial = PropertyEntity({ val: ko.observable(1), options: EnumTipoFilial.obterOpcoes(), text: Localization.Resources.Filiais.Filial.TipoFilial.getRequiredFieldDescription(), issue: 733, def: 1 });
    this.Atividade = PropertyEntity({ val: ko.observable(1), options: EnumAtividade.obterOpcoes(), text: Localization.Resources.Filiais.Filial.AtividadeFilial.getRequiredFieldDescription(), issue: 47, def: 1 });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Filiais.Filial.Localidade.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 557 });
    this.NumeroUnidadeImpressao = PropertyEntity({ text: Localization.Resources.Filiais.Filial.NumeroUnidadeImpressao.getFieldDescription(), issue: 734, required: false, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' }, val: ko.observable(0.00), maxlength: 10 });
    this.ControlaExpedicao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ControlarExpedicaoFilial, def: false });
    this.EmitirMDFeManualmente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.MDFesSeraoEmitidosManualmente, issue: 1534, def: false });
    this.ExigirPreCargaMontagemCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ExigePreCargaMontagemCarga, def: false });
    this.ValorMedioMercadoria = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Filiais.Filial.ValorMedioMercadoria.getFieldDescription(), required: false, getType: typesKnockout.decimal, maxlength: 18 });

    this.ExigirConfirmacaoTransporte = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ExigeConfirmacaoDadosTransporte, def: false });
    this.LiberarAutomaticamentePagamento = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.LiberarAutomaticamentePagamentoDocumentosEmitidosFilial, def: false, visible: ko.observable(_CONFIGURACAO_TMS.GerarPagamentoBloqueado) });
    this.NaoAdicionarValorDescarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.NaoAdicionarValorDescarga, issue: 736, def: false, visible: ko.observable(false) });
    this.GerarCIOTParaTodasAsCargas = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.GerarCIOTTodasCargasFilial, def: false });
    this.NaoPermitirAgruparCargaMesmaFilial = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.NaoPermitirAgruparCargasDestaMesmaFilial, def: false });
    this.ExigeConfirmacaoFreteAntesEmissao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.ExigeConfirmacaoFreteAntesGeracaoCTes, def: false });
    this.AlertarDiariaContainer = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.AlertarDiariaContainer, def: false });
    this.NaoValidarVeiculoIntegracao = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.NaoValidarVeiculoIntegracao, def: false, visible: ko.observable(false) });
    this.GerarIntegracaoP44 = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.GerarIntegracaoP44, def: false, visible: ko.observable(false) });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Filiais.Filial.TipoCargaPadrao.getFieldDescription(), idBtnSearch: guid() });
    this.SetorAtendimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Filiais.Filial.SetorAtendimento.getFieldDescription(), idBtnSearch: guid() });
    this.ConfiguracaoCIOT = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Filiais.Filial.ConfiguracaoCIOT.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Email = PropertyEntity({ text: Localization.Resources.Filiais.Filial.Email.getFieldDescription() });
    this.EmailDiariaContainer = PropertyEntity({ text: Localization.Resources.Filiais.Filial.EmailDiariaContainer.getFieldDescription() });
    this.HorarioCorteParaCalculoLeadTime = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HorarioCorteParaCalculoLeadTime, def: false });
    this.HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos = PropertyEntity({ text: Localization.Resources.Filiais.Filial.HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.time });
    this.NotificarContainerCargaCancelada = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.NotificarContainerCargaCancelada, def: false });
    this.InformarEquipamentoFluxoPatio = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.InformarEquipamentoFluxoPatio, def: false, visible: ko.observable(true) });
    this.HorarioCorteParaCalculoLeadTime.val.subscribe(function (novoValor) {
        LimparCampo(_filial.HoraCorteRecalcularPrazoEntregaAposEmissaoDocumentos)
    });

    var dataAtual = moment().format("HH:mm");

    this.DiasDeCortePlanejamentoDiario = PropertyEntity({ val: ko.observable(0), def: 0, text: "Dias de Corte Planejamento Diário:", getType: typesKnockout.int, required: false, visible: ko.observable(false) });

    this.HoraInicialPlanejamentoDiario = PropertyEntity({ val: ko.observable(dataAtual), def: dataAtual, text: "Hora Inicial Planejamento Diário", getType: typesKnockout.time, required: false, visible: ko.observable(false) });
    this.HoraFinalPlanejamentoDiario = PropertyEntity({ val: ko.observable(dataAtual), def: dataAtual, text: "Hora Final Planejamento Diário", getType: typesKnockout.time, required: false, visible: ko.observable(false) });

    this.OutrosCodigosIntegracao = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.TiposOperacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.EmpresaEmissora = _empresaEmissora.EmpresaEmissora;
    this.EmiteMDFeFilialEmissora = _empresaEmissora.EmiteMDFeFilialEmissora;
    this.EmiteMDFeFilialEmissoraPorEstadoDestino = _estadoDestino.EmiteMDFeFilialEmissoraPorEstadoDestino;
    this.UtilizarCtesAnterioresComoCteFilialEmissora = _empresaEmissora.UtilizarCtesAnterioresComoCteFilialEmissora;
    this.SequenciaGestaoPatio = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: null, val: GetSetGestaoPatio });
    this.SequenciaGestaoPatioDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: null, val: GetSetGestaoPatioDestino });
    this.OrdemGestaoPatio = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: null, val: GetSetOrdemGestaoPatio });
    this.OrdemGestaoPatioDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, def: null, val: GetSetOrdemGestaoPatioDestino });
    this.SetoresFilial = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idGrid: guid() });
    this.IntegracaoSemParar = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.IntegracaoBuonny = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.IntegracaoTarget = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Descontos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.DescontosExcecao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.Balancas = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });
    this.EstadosDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.TipoOperacaoRedespacho = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, visible: ko.observable(false), text: Localization.Resources.Filiais.Filial.TipoOperacaoPadraoRedespachos.getFieldDescription(), idBtnSearch: guid() });

    this.PossuiCertificado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Filiais.Filial.PossuiCertificado });
    this.DataInicialCertificado = PropertyEntity({ text: Localization.Resources.Filiais.Filial.DataInicialCertificado.getRequiredFieldDescription(), getType: typesKnockout.date, required: false });
    this.DataFinalCertificado = PropertyEntity({ text: Localization.Resources.Filiais.Filial.DataFinalCertificado.getRequiredFieldDescription(), getType: typesKnockout.date, required: false });
    this.SerieCertificado = PropertyEntity({ text: Localization.Resources.Filiais.Filial.SerieCertificado.getRequiredFieldDescription(), required: false });
    this.SenhaCertificado = PropertyEntity({ text: Localization.Resources.Filiais.Filial.SenhaCertificado.getRequiredFieldDescription(), required: false });
    this.SiglaFilial = PropertyEntity({ text: Localization.Resources.Filiais.Filial.SiglaFilial.getFieldDescription() });
    this.TipoIntegracao = PropertyEntity({ text: Localization.Resources.Filiais.Filial.IntegracaoFatura, getType: typesKnockout.selectMultiple, options: ko.observable([]), url: "TipoIntegracao/BuscarTodos", val: ko.observable(""), def: [], issue: 0, visible: ko.observable(true) });
    this.AccountNameVtex = PropertyEntity({ text: Localization.Resources.Filiais.Filial.AccountNameVtex.getFieldDescription(), required: false, visible: ko.observable(_possuiIntegracaoVtex) });
    this.GerarIntegracaoKlios = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.GerarIntegracaoKlios, def: false, visible: ko.observable(false) });
    this.HabilitarPreViagemTrizy = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Filiais.Filial.HabilitarPreViagemTrizy, def: false, visible: ko.observable(false) });
    this.HabilitarPreViagemTrizy.val.subscribe(function (novoValor) {
        if (novoValor)
            $("#liTabTiposOperacaoTrizy").show();
        else
            $("#liTabTiposOperacaoTrizy").hide();
    });
    this.TanquesRemover = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TanquesAdicionar = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
}

var CRUDFilial = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadFilial() {
    carregarTiposIntegracaoValePedagio(function () {
        loadEmpresaEmissora();
        loadEstadoDestino();

        _filial = new Filial();
        KoBindings(_filial, "knockoutCadastroFilial");

        HeaderAuditoria("Filial", _filial);

        _CRUDFilial = new CRUDFilial();
        KoBindings(_CRUDFilial, "knockoutCRUDFilial");

        _pesquisaFilial = new PesquisaFilial();
        KoBindings(_pesquisaFilial, "knockoutPesquisaFilial", false, _pesquisaFilial.Pesquisar.id);

        if (_possuiValorDescargaCliente) {
            _filial.NaoAdicionarValorDescarga.visible(true);
            $("#liIsencaoValorDescargaPorTipoOperacao").show();
        }

        if (_habilitarCadastroArmazem) {
            $("#liTabArmazem").show();
        }

        $("#liTabCertificado").show();

        alterarEstadoCadastroCertificado();

        ObterIntegracoesHabilitadas();
        loadOutroCodigoIntegracao();
        loadTipoOperacao();
        loadTipoOperacaoTrizy();
        loadGestaoPatio();
        loadGestaoPatioDestino();
        loadSetorFilial();
        loadCondicaoPagamento();
        loadFilialSemParar();
        loadFilialTarget();
        loadFilialBuonny();
        loadFilialDesconto();
        loadFilialDescontoExcecao();
        loadValePedagio();
        loadArmazem();
        loadFilialModeloVeicularCarga();
        loadAlertaSln();
        loadFilialBalanca();
        LoadTanques();

        new BuscarTiposdeCarga(_filial.TipoCarga);
        new BuscarLocalidades(_filial.Localidade);
        new BuscarSetorFuncionario(_filial.SetorAtendimento);
        new BuscarTiposOperacao(_filial.TipoOperacaoRedespacho);
        BuscarConfiguracaoCIOT(_filial.ConfiguracaoCIOT,
            Localization.Resources.Filiais.Filial.ConsultaDeOperadorasDeCIOT,
            Localization.Resources.Filiais.Filial.ConfiguracaoCIOT,
            RetornoConsultaConfiguracaoCIOT);

        buscarFilials();

        $("#" + _filial.NaoAdicionarValorDescarga.id).click(naoAdicionarValorDescargaClick);

    });
}
function RetornoConsultaConfiguracaoCIOT(data) {
    _filial.ConfiguracaoCIOT.val(data.Descricao);
    _filial.ConfiguracaoCIOT.codEntity(data.Codigo);
}

function verificarSeExisteConfiguracaoParaGeracaoDeFrotaAutomatizada(filialGrid, arg) {
    if (!filialGrid || !filialGrid.Codigo) return;
    executarReST("GeracaoFrotaAutomatizada/VerificarSeExisteConfiguracaoCadastrada", { CodigoFilial: filialGrid.Codigo }, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _filial.DiasDeCortePlanejamentoDiario.visible(true);
            _filial.HoraInicialPlanejamentoDiario.visible(true);
            _filial.HoraFinalPlanejamentoDiario.visible(true);
            _filial.DiasDeCortePlanejamentoDiario.val(arg.Data.DiasDeCortePlanejamentoDiario);
            _filial.HoraInicialPlanejamentoDiario.val(arg.Data.HoraInicialPlanejamentoDiario);
            _filial.HoraFinalPlanejamentoDiario.val(arg.Data.HoraFinalPlanejamentoDiario);
        } else {
            _filial.DiasDeCortePlanejamentoDiario.visible(false);
            _filial.HoraInicialPlanejamentoDiario.visible(false);
            _filial.HoraFinalPlanejamentoDiario.visible(false);
            _filial.DiasDeCortePlanejamentoDiario.val(0);
            _filial.HoraInicialPlanejamentoDiario.val('');
            _filial.HoraFinalPlanejamentoDiario.val('');
        }
    });
}

function ObterIntegracoesHabilitadas() {
    _integracaoSemParar = false;

    executarReST("Integracao/ObterIntegracoesConfiguradas", {}, function (retorno) {
        if (retorno.Success && retorno.Data) {
            if (retorno.Data.TiposExistentes != null && retorno.Data.TiposExistentes.length > 0) {
                if (retorno.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Ortec; }))
                    _filial.TipoOperacaoRedespacho.visible(true);

                if (retorno.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.SemParar || o == EnumTipoIntegracao.Target; }))
                    $("#liTabValePedagio").show();

                if (retorno.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Marfrig; }))
                    $("#liFilialModeloVeicularCarga").show();

                if (retorno.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Buonny; }))
                    $("#liIntegracaoBuonny").show();

                if (retorno.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Deca; })) {
                    $("#liTabBalanca").show();
                    _gestaoPatio.BalancaGuaritaSaida.visible(true);
                    _gestaoPatio.BalancaGuaritaEntrada.visible(true);
                    _gestaoPatioDestino.BalancaGuaritaSaida.visible(true);
                    _gestaoPatioDestino.BalancaGuaritaEntrada.visible(true);
                }

                if (retorno.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Qbit || o == EnumTipoIntegracao.BalancaKIKI; })) {
                    _gestaoPatio.GuaritaEntradaTipoIntegracaoBalanca.visible(true);
                    _gestaoPatio.GuaritaSaidaTipoIntegracaoBalanca.visible(true);
                    _gestaoPatioDestino.GuaritaEntradaTipoIntegracaoBalanca.visible(true);
                    _gestaoPatioDestino.GuaritaSaidaTipoIntegracaoBalanca.visible(true);
                }

                if (retorno.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Ultragaz; }))
                    _filial.NaoValidarVeiculoIntegracao.visible(true);

                if (retorno.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.P44; }))
                    _filial.GerarIntegracaoP44.visible(true);

                if (retorno.Data.TiposExistentes.some(function (o) { return o == EnumTipoIntegracao.Klios; }))
                    _filial.GerarIntegracaoKlios.visible(true);

                _filial.HabilitarPreViagemTrizy.visible(retorno.Data.PossuiIntegracaoTrizy);
            }
        }
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    if (ValidarCamposObrigatorios(_filial)) {
        executarReST("Filial/Adicionar", obterFilialSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                    _gridFilial.CarregarGrid();
                    limparCamposFilial();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function atualizarClick() {
    if (ValidarCamposObrigatorios(_filial)) {
        executarReST("Filial/Atualizar", obterFilialSalvar(), function (retorno) {

            if (_gestaoPatio.CheckListGerarNovoPedidoAoTerminoFluxo.val() == true) {
                if (_gestaoPatio.CheckListTipoOperacao.val() == 0 && _gestaoPatio.CheckListDestinatario.val() == 0) {
                    _gestaoPatio.CheckListTipoOperacao.required(true);
                    _gestaoPatio.CheckListDestinatario.required(true);
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.TipoOperacaoeDestinatarioObrigatorios);
                    return;
                }

            }

            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);
                    _gridFilial.CarregarGrid();
                    limparCamposFilial();
                }
                else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                    _filial.SetoresFilial.val(JSON.parse(_filial.SetoresFilial.val()));
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                _filial.SetoresFilial.val(JSON.parse(_filial.SetoresFilial.val()));
            }
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function cancelarClick() {
    limparCamposFilial();
}

function excluirClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaExcluirRegistro, function () {
        ExcluirPorCodigo(_filial, "Filial/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridFilial.CarregarGrid();
                    limparCamposFilial();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

/*
 * Declaração das Funções
 */

function buscarFilials() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: editarFilial, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "Filial/ExportarPesquisa",
        btnText: "Exportar Excel",
        funcaoObterParametros: function () {
            return { Codigo: _filial.Codigo.val() };
        }
    }

    _gridFilial = new GridView(_pesquisaFilial.Pesquisar.idGrid, "Filial/Pesquisa", _pesquisaFilial, menuOpcoes, null, 10, null, null, null, null, null, null, configExportacao);
    _gridFilial.CarregarGrid();
}

function carregarTiposIntegracaoValePedagio(callback) {
    executarReST("TipoIntegracao/BuscarTodos", { Tipos: JSON.stringify([EnumTipoIntegracao.NaoInformada, EnumTipoIntegracao.SemParar, EnumTipoIntegracao.Target]) }, function (retorno) {
        if (retorno.Success) {
            _tiposIntegracaoValePedagio = new Array();

            for (var i = 0; i < retorno.Data.length; i++)
                _tiposIntegracaoValePedagio.push({ value: retorno.Data[i].Codigo, text: retorno.Data[i].Descricao });
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

        callback();
    });
}

function editarFilial(filialGrid) {
    limparCamposFilial();
    _filial.Codigo.val(filialGrid.Codigo);
    BuscarPorCodigo(_filial, "Filial/BuscarPorCodigo", function (arg) {
        _pesquisaFilial.ExibirFiltros.visibleFade(false);

        verificarSeExisteConfiguracaoParaGeracaoDeFrotaAutomatizada(filialGrid, arg);
        recarregarGridOutroCodigoIntegracao();
        recarregarGridTipoOperacao();
        recarregarGridEstadosDestino(arg.Data.EstadoDestinoEmpresaEmissora);
        recarregarGridTipoOperacaoTrizy();
        recarregarGridSetoresFilial();
        recarregarGridFilialDesconto();
        recarregarGridFilialDescontoExcecao();
        RecarregarGridFilialBalanca();
        preencherCondicaoPagamento(arg.Data.CondicaoPagamento);
        preencherValePedagio(arg.Data.ValePedagioTransportadores);
        preencherArmazem(arg.Data.Armazem);
        preencherFilialModeloVeicularCarga(arg.Data.ListaModeloVeicularCarga);
        preencherAlertasSla(arg.Data.ListaAlertasSla);

        _tanques.Tanques(arg.Data.Tanques);

        _CRUDFilial.Atualizar.visible(true);
        _CRUDFilial.Cancelar.visible(true);
        _CRUDFilial.Excluir.visible(true);
        _CRUDFilial.Adicionar.visible(false);

        if (_possuiValorDescargaCliente)
            naoAdicionarValorDescargaClick();

        if (arg.Data.IntegracaoSemParar !== null && arg.Data.IntegracaoSemParar !== undefined)
            PreencherObjetoKnout(_filialSemParar, { Data: arg.Data.IntegracaoSemParar });

        if (arg.Data.IntegracaoTarget !== null && arg.Data.IntegracaoTarget !== undefined)
            PreencherObjetoKnout(_filialTarget, { Data: arg.Data.IntegracaoTarget });

        if (arg.Data.IntegracaoBuonny)
            PreencherObjetoKnout(_filialBuonny, { Data: arg.Data.IntegracaoBuonny });

        alterarEstadoCadastroCertificado();

        _gestaoPatio.TipoCheckListImpressao.visible(arg.Data.ExisteTipoCheckListImpressao);
        _gestaoPatioDestino.TipoCheckListImpressao.visible(arg.Data.ExisteTipoCheckListImpressao);

    }, null);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
}

function limparCamposFilial() {
    _CRUDFilial.Atualizar.visible(false);
    _CRUDFilial.Cancelar.visible(false);
    _CRUDFilial.Excluir.visible(false);
    _CRUDFilial.Adicionar.visible(true);

    _filial.DiasDeCortePlanejamentoDiario.visible(false);
    _filial.HoraInicialPlanejamentoDiario.visible(false);
    _filial.HoraFinalPlanejamentoDiario.visible(false);

    LimparCampos(_filial);
    limparCamposOutroCodigoIntegracao();
    limparCamposTipoOperacao();
    limparCamposTipoOperacaoTrizy();
    limparCamposEmpresaEmissora();
    limparCamposEstadoDestino();
    limparCamposGestaoPatio();
    limparCamposGestaoPatioDestino();
    limparCamposSetorFilial();
    limparCamposCondicaoPagamento();
    limparCamposFilialSemParar();
    limparCamposFilialTarget();
    limparCamposFilialBuonny();
    limparCamposValePedagio();
    limparCamposArmazem();
    limparCamposFilialModeloVeicularCarga();
    limparCamposAlertasSln();
    limparCamposFilialBalanca();

    recarregarGridOutroCodigoIntegracao();
    recarregarGridTipoOperacao();
    recarregarGridTipoOperacaoTrizy();
    recarregarGridSetoresFilial();
    recarregarGridFilialDesconto();
    recarregarGridFilialDescontoExcecao();
    alterarEstadoCadastroCertificado();

    _opcoesChecklistImpressao = [{
        value: 0,
        text: Localization.Resources.Gerais.Geral.Padrao
    }];

    if (_possuiValorDescargaCliente)
        $("#liIsencaoValorDescargaPorTipoOperacao").show();

    Global.ResetarAbas();
}

function naoAdicionarValorDescargaClick() {
    if (_filial.NaoAdicionarValorDescarga.val())
        $("#liIsencaoValorDescargaPorTipoOperacao").hide();
    else
        $("#liIsencaoValorDescargaPorTipoOperacao").show();
}

function isCadastroEmEdicao() {
    return _filial.Codigo.val() > 0;
}

function obterFilialSalvar() {
    _filial.TiposOperacoes.val(obterTipoOperacaoSalvar());
    _filial.TipoOperacao.val(obterTipoOperacaoTrizySalvar());
    _filial.Descontos.val(obterFilialDesconto());
    _filial.DescontosExcecao.val(obterFilialDescontoExcecao());
    _filial.TanquesRemover.val(ObterTanquesRemover());
    _filial.TanquesAdicionar.val(ObterTanquesAdicionar());
    _filial.EstadosDestino.val(obterEstadosDestino());

    preencherListaSetorFilialSalvar();

    _filial.IntegracaoSemParar.val(JSON.stringify(RetornarObjetoPesquisa(_filialSemParar)));
    _filial.IntegracaoTarget.val(JSON.stringify(RetornarObjetoPesquisa(_filialTarget)));
    _filial.IntegracaoBuonny.val(JSON.stringify(RetornarObjetoPesquisa(_filialBuonny)));

    var filial = RetornarObjetoPesquisa(_filial);

    preencherCondicaoPagamentoSalvar(filial);
    preencherValePedagioSalvar(filial);
    preencherArmazemSalvar(filial);
    preencherFilialModeloVeicularCargaSalvar(filial);
    preencherFilialAlertasSlnSalvar(filial);


    return filial;
}

function obterFilialDesconto() {
    return JSON.stringify(_gridFilialDesconto.BuscarRegistros());
}

function recarregarGridFilialDesconto() {
    _gridFilialDesconto.CarregarGrid(_filial.Descontos.val() || []);
}

function obterFilialDescontoExcecao() {
    return JSON.stringify(_gridFilialDescontoExcecao.BuscarRegistros());
}

function obterEstadosDestino() {
    return JSON.stringify(_gridEstadoDestino.BuscarRegistros());
}

function recarregarGridFilialDescontoExcecao() {
    _gridFilialDescontoExcecao.CarregarGrid(_filial.DescontosExcecao.val() || []);
}
