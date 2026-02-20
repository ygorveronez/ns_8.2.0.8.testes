/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/MotivoChamado.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamadoTMS.js" />
/// <reference path="Abertura.js" />
/// <reference path="Etapas.js" />
/// <reference path="AutorizacaoCliente.js" />
/// <reference path="Analise.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridChamadosTMS;
var _chamadoTMS;
var _CRUDChamadoTMS;
var _pesquisaChamadosTMS;
var _motivoChamadoConfiguracao = {};

var ChamadoTMS = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.PossuiResponsavel = PropertyEntity({ val: ko.observable(false), def: false });
    this.PodeEditar = PropertyEntity({ val: ko.observable(false), def: false });
    this.Responsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Responsável:" }); // Uso Apenas informativo
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoChamadoTMS.Todos), def: EnumSituacaoChamadoTMS.Todos, text: "Situação: " });
};

var CRUDChamadoTMS = function () {
    this.Limpar = PropertyEntity({ eventClick: limparChamadoClick, type: types.event, text: "Limpar (Gerar novo Chamado)", idGrid: guid(), visible: ko.observable(false) });
    this.AssumirChamado = PropertyEntity({ eventClick: assumirChamadoClick, type: types.event, text: "Assumir Chamado", idGrid: guid(), visible: ko.observable(false) });
    this.Reabrir = PropertyEntity({ eventClick: reabrirClick, type: types.event, text: "Reabrir", visible: ko.observable(false) });
    this.IniciarAnalise = PropertyEntity({ eventClick: iniciarAnaliseClick, type: types.event, text: "Iniciar Análise", visible: ko.observable(false) });
};

var PesquisaChamadosTMS = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, val: ko.observable(Global.PrimeiraDataDoMesAtual()) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Carga = PropertyEntity({ text: "Número Carga:" });
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
    this.NumeroFinal = PropertyEntity({ text: "Número Final: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
    this.SituacaoChamado = PropertyEntity({ val: ko.observable(EnumSituacaoChamadoTMS.Todos), options: EnumSituacaoChamadoTMS.obterOpcoesPesquisa(), def: EnumSituacaoChamadoTMS.Todos, text: "Situação: ", visible: ko.observable(true) });
    this.MotivoChamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo do Chamado:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridChamadosTMS.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
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
};

//*******EVENTOS*******

function loadChamadoTMS() {
    _chamadoTMS = new ChamadoTMS();

    _CRUDChamadoTMS = new CRUDChamadoTMS();
    KoBindings(_CRUDChamadoTMS, "knockoutCRUD");

    _pesquisaChamadosTMS = new PesquisaChamadosTMS();
    KoBindings(_pesquisaChamadosTMS, "knockoutPesquisaChamadosTMS", false, _pesquisaChamadosTMS.Pesquisar.id);

    new BuscarMotivoChamado(_pesquisaChamadosTMS.MotivoChamado);
    new BuscarMotorista(_pesquisaChamadosTMS.Motorista);
    new BuscarVeiculos(_pesquisaChamadosTMS.Veiculo);

    loadEtapasChamado();
    loadAbertura();
    loadAutorizacaoCliente();
    loadAnalise();

    BuscarChamadosTMS();
}

function limparChamadoClick(e, sender) {
    LimparCamposChamadoTMS();
}

function assumirChamadoClick() {
    executarReST("ChamadoTMS/AssumirChamado", { Codigo: _chamadoTMS.Codigo.val() }, function (arg) {
        if (arg.Data) {
            exibirMensagem(tipoMensagem.ok, "Chamado Assumido", "O responsável pelo chamado passa a ser você.");
            _chamadoTMS.PodeEditar.val(true);
            _CRUDChamadoTMS.AssumirChamado.visible(false);
            AvaliarRegras();
            limparAnaliseClick();
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, null);
}

function reabrirClick() {
    executarReST("ChamadoTMS/ReabrirChamado", { Codigo: _chamadoTMS.Codigo.val() }, function (arg) {
        if (arg.Data) {
            exibirMensagem(tipoMensagem.ok, "Chamado Reaberto", "O chamado agora está Aberto.");
            BuscarChamadoTMSPorCodigo(_chamadoTMS.Codigo.val());
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, null);
}

function iniciarAnaliseClick() {
    executarReST("ChamadoTMS/IniciarEtapaAnalise", { Codigo: _chamadoTMS.Codigo.val() }, function (arg) {
        if (arg.Data) {
            exibirMensagem(tipoMensagem.ok, "Chamado em Análise", "Análise iniciada com sucesso.");
            BuscarChamadoTMSPorCodigo(_chamadoTMS.Codigo.val());
            recarregarGridPesquisaChamadosTMS();
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function recarregarGridPesquisaChamadosTMS() {
    _gridChamadosTMS.CarregarGrid();
}

function BuscarChamadosTMS() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarChamadosTMS, tamanho: 15, icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridChamadosTMS = new GridView(_pesquisaChamadosTMS.Pesquisar.idGrid, "ChamadoTMS/Pesquisa", _pesquisaChamadosTMS, menuOpcoes, null, 10);
    _gridChamadosTMS.CarregarGrid();
}

function ObterConfiguracaoMotivoChamado(codigo, cb) {
    executarReST("MotivoChamado/ConfiguracaoDoMotivo", { Codigo: codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _motivoChamadoConfiguracao = arg.Data;

                if (cb)
                    cb();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function editarChamadosTMS(itemGrid) {
    _pesquisaChamadosTMS.ExibirFiltros.visibleFade(false);
    BuscarChamadoTMSPorCodigo(itemGrid.Codigo);
}

function BuscarChamadoTMSPorCodigo(codigo, cb) {
    LimparCamposChamadoTMS();

    executarReST("ChamadoTMS/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            ObterConfiguracaoMotivoChamado(arg.Data.MotivoChamado, function () {
                EditarChamadoTMS(arg.Data);
                EditarAbertura(arg.Data);
                EditarEtapa2(arg.Data);
                EditarAnalise(arg.Data);
                SetarEtapaChamado();

                if (cb) cb();
            });
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function EditarChamadoTMS(data) {
    _chamadoTMS.Codigo.val(data.Codigo);
    _chamadoTMS.Situacao.val(data.Situacao);
    _chamadoTMS.PodeEditar.val(data.PodeEditar);
    _chamadoTMS.PossuiResponsavel.val(data.PossuiResponsavel);

    if (data.Responsavel != null) {
        _chamadoTMS.Responsavel.val(data.Responsavel.Descricao);
        _chamadoTMS.Responsavel.codEntity(data.Responsavel.Codigo);
    }

    _CRUDChamadoTMS.Limpar.visible(true);

    if (_chamadoTMS.PossuiResponsavel.val()) {
        var permitirAssumirChamadoDeOutroResponsavel = (_chamadoTMS.Situacao.val() === EnumSituacaoChamadoTMS.Aberto) && (data.CodigoResponsavel != _CONFIGURACAO_TMS.CodigoUsuarioLogado) && (_CONFIGURACAO_TMS.PermitirAssumirChamadoDeOutroResponsavel || data.PermiteAssumirChamadoMesmoSetor);
        _CRUDChamadoTMS.AssumirChamado.visible(permitirAssumirChamadoDeOutroResponsavel);
    }
    else
        _CRUDChamadoTMS.AssumirChamado.visible(true);

    if (_chamadoTMS.Situacao.val() === EnumSituacaoChamadoTMS.Cancelado)
        _CRUDChamadoTMS.Reabrir.visible(true);
    if (_chamadoTMS.Situacao.val() === EnumSituacaoChamadoTMS.Aberto)
        _CRUDChamadoTMS.IniciarAnalise.visible(true);
}

function LimparCamposChamadoTMS() {
    LimparCampos(_chamadoTMS);
    _CRUDChamadoTMS.Limpar.visible(false);
    _CRUDChamadoTMS.AssumirChamado.visible(false);
    _CRUDChamadoTMS.Reabrir.visible(false);
    _CRUDChamadoTMS.IniciarAnalise.visible(false);
    SetarEtapaInicioChamado();

    LimparCamposAbertura();
    limparCamposAutorizacaoCliente();
    LimparCamposAnalise();
    Global.ResetarAbas();
}