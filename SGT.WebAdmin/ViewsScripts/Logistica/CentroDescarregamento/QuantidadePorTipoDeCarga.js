/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Enumeradores/EnumDiaSemana.js" />

//#region Váriaveis Globais

var _quantidadePorTipoDeCarga;
var _gridQuantidadePorTipoDeCarga;
var _gridQuantidadePorTipoDeCargaTipoCargas;

//#endregion


//#region Mapeamento Knockout

var QuantidadePorTipoDeCarga = function () {
    this.Lista = PropertyEntity({ type: types.local });
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoDia = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tolerancia = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroDescarregamento.ToleranciaEmHoras, val: ko.observable(0), def: 0, configInt: { precision: 0, allowZero: true } });
    this.ToleranciaCancelamentoAgendaConfirmada = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroDescarregamento.ToleranciaCancelamentoAgendasConfirmadasEmHoras.getFieldDescription(), val: ko.observable(0), def: 0, configInt: { precision: 0, allowZero: true } });
    this.ToleranciaCancelamentoAgendaNaoConfirmada = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroDescarregamento.ToleranciaCancelamentoAgendasNaoConfirmadasEmHoras.getFieldDescription(), val: ko.observable(0), def: 0, configInt: { precision: 0, allowZero: true } });
    this.Dia = PropertyEntity({ type: types.map, getType: typesKnockout.int, required: true, text: Localization.Resources.Logistica.CentroDescarregamento.DiaSemana.getRequiredFieldDescription(), val: ko.observable(0), options: EnumDiaSemana.obterOpcoes() });
    this.Volumes = PropertyEntity({ type: types.map, getType: typesKnockout.int, required: true, text: Localization.Resources.Logistica.CentroDescarregamento.QtdCaixas.getRequiredFieldDescription(), val: ko.observable(""), def: "" });
    this.TiposCarga = PropertyEntity({ type: types.map, text: Localization.Resources.Logistica.CentroDescarregamento.AdicionarTipoCarga, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), idBtnSearch: guid(), idGrid: guid(), visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarQuantidadePorTipoDeCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarQuantidadePorTipoDeCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirQuantidadePorTipoDeCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarQuantidadePorTipoDeCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

function loadQuantidadePorTipoDeCarga() {
    _quantidadePorTipoDeCarga = new QuantidadePorTipoDeCarga();
    KoBindings(_quantidadePorTipoDeCarga, "knockoutQuantidadePorTipoDeCarga");

    loadGridQuantidadePorTipoDeCarga();
    loadGridQuantidadePorTipoDeCargaTiposCargas();
}

function loadGridQuantidadePorTipoDeCarga() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarQuantidadePorTipoDeCarga }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoDia", visible: false },
        { data: "TiposCarga", visible: false },
        { data: "DescricaoTipoCarga", title: Localization.Resources.Gerais.Geral.TiposCarga, width: "25%" },
        { data: "Volumes", title: Localization.Resources.Logistica.CentroDescarregamento.QtdCaixas, width: "10%" },
        { data: "Dia", title: Localization.Resources.Gerais.Geral.Dia, width: "15%" },
        { data: "Tolerancia", title: Localization.Resources.Logistica.CentroDescarregamento.Tolerancia, width: "10%" },
        { data: "ToleranciaCancelamentoAgendaConfirmada", title: Localization.Resources.Logistica.CentroDescarregamento.ToleranciaCancelamentoAgendaConfirmada, width: "20%" },
        { data: "ToleranciaCancelamentoAgendaNaoConfirmada", title: Localization.Resources.Logistica.CentroDescarregamento.ToleranciaCancelamentoAgendaNaoConfirmada, width: "20%" }
    ];

    _gridQuantidadePorTipoDeCarga = new BasicDataTable(_quantidadePorTipoDeCarga.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    recarregarGridQuantidadePorTipoDeCarga();
}

var loadGridQuantidadePorTipoDeCargaTiposCargas = function () {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerQuantidadePorTipoCargaTipoDeCarga, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "75%", className: "text-align-left" }
    ];

    _gridQuantidadePorTipoDeCargaTipoCargas = new BasicDataTable(_quantidadePorTipoDeCarga.TiposCarga.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);

    new BuscarTiposdeCarga(_quantidadePorTipoDeCarga.TiposCarga, null, null, _gridQuantidadePorTipoDeCargaTipoCargas);
    _quantidadePorTipoDeCarga.TiposCarga.basicTable = _gridQuantidadePorTipoDeCargaTipoCargas;

    _gridQuantidadePorTipoDeCargaTipoCargas.CarregarGrid([]);
}

//#endregion

//#region Métodos Públicos

function editarQuantidadePorTipoDeCarga(registroSelecionado) {
    for (var i = 0; i < _centroDescarregamento.QuantidadePorTipoDeCarga.list.length; i++) {
        var registroAtual = _centroDescarregamento.QuantidadePorTipoDeCarga.list[i];
        if (registroSelecionado.Codigo == registroAtual.Codigo.val) {
            _quantidadePorTipoDeCarga.Codigo.val(registroAtual.Codigo.val);
            _quantidadePorTipoDeCarga.TiposCarga.basicTable.CarregarGrid(obterGridQuantidadeTiposCargaTiposCarga(registroAtual.TiposCarga.list));
            _quantidadePorTipoDeCarga.Dia.val(registroAtual.CodigoDia.val);
            _quantidadePorTipoDeCarga.Volumes.val(registroAtual.Volumes.val);
            _quantidadePorTipoDeCarga.Tolerancia.val(registroAtual.Tolerancia.val);
            _quantidadePorTipoDeCarga.ToleranciaCancelamentoAgendaConfirmada.val(registroAtual.ToleranciaCancelamentoAgendaConfirmada.val);
            _quantidadePorTipoDeCarga.ToleranciaCancelamentoAgendaNaoConfirmada.val(registroAtual.ToleranciaCancelamentoAgendaNaoConfirmada.val);

            _quantidadePorTipoDeCarga.Adicionar.visible(false);
            _quantidadePorTipoDeCarga.Atualizar.visible(true);
            _quantidadePorTipoDeCarga.Cancelar.visible(true);
            _quantidadePorTipoDeCarga.Excluir.visible(true);
        }
    }
}

//#endregion

//#region Métodos Privados

function limparCamposQuantidadePorTipoDeCarga() {
    LimparCampos(_quantidadePorTipoDeCarga);
    _gridQuantidadePorTipoDeCargaTipoCargas.CarregarGrid([]);

    _quantidadePorTipoDeCarga.Adicionar.visible(true);
    _quantidadePorTipoDeCarga.Atualizar.visible(false);
    _quantidadePorTipoDeCarga.Cancelar.visible(false);
    _quantidadePorTipoDeCarga.Excluir.visible(false);
}

function recarregarGridQuantidadePorTipoDeCarga() {
    var data = new Array();

    $.each(_centroDescarregamento.QuantidadePorTipoDeCarga.list, function (i, quantidadePorTipoDeCarga) {
        var quantidadePorTipoDeCargaGrid = new Object();

        quantidadePorTipoDeCargaGrid.Codigo = quantidadePorTipoDeCarga.Codigo.val;
        quantidadePorTipoDeCargaGrid.Tolerancia = quantidadePorTipoDeCarga.Tolerancia.val;
        quantidadePorTipoDeCargaGrid.ToleranciaCancelamentoAgendaConfirmada = quantidadePorTipoDeCarga.ToleranciaCancelamentoAgendaConfirmada.val;
        quantidadePorTipoDeCargaGrid.ToleranciaCancelamentoAgendaNaoConfirmada = quantidadePorTipoDeCarga.ToleranciaCancelamentoAgendaNaoConfirmada.val;
        quantidadePorTipoDeCargaGrid.CodigoDia = quantidadePorTipoDeCarga.CodigoDia.val;
        quantidadePorTipoDeCargaGrid.Volumes = quantidadePorTipoDeCarga.Volumes.val;
        quantidadePorTipoDeCargaGrid.Dia = EnumDiaSemana.obterDescricaoSemConfiguracao(quantidadePorTipoDeCarga.CodigoDia.val);
        quantidadePorTipoDeCargaGrid.DescricaoTipoCarga = quantidadePorTipoDeCarga.DescricaoTipoCarga.val;
        quantidadePorTipoDeCargaGrid.TiposCarga = quantidadePorTipoDeCarga.TiposCarga.list;
        
        data.push(quantidadePorTipoDeCargaGrid);
    });

    _gridQuantidadePorTipoDeCarga.CarregarGrid(data);
}

function atualizarQuantidadePorTipoDeCargaPorCodigo(codigo, quantidadePorTipoDeCarga) {
    for (var i = 0; i < _centroDescarregamento.QuantidadePorTipoDeCarga.list.length; i++) {
        if (codigo == _centroDescarregamento.QuantidadePorTipoDeCarga.list[i].Codigo.val) {
            _centroDescarregamento.QuantidadePorTipoDeCarga.list[i] = quantidadePorTipoDeCarga;
            break;
        }
    }
}

function obterDescricaoTiposCarga() {
    var descricoesTipoCarga = [];

    _gridQuantidadePorTipoDeCargaTipoCargas.BuscarRegistros().forEach(function (reg) {
        descricoesTipoCarga.push(reg.Descricao);
    });
    
    return descricoesTipoCarga.join(', ');
}

function obterQuantidadeTipoCargaSalvar() {
    var quantidadeTipoCarga = SalvarListEntity(_quantidadePorTipoDeCarga);;

    quantidadeTipoCarga["TiposCarga"] = new PropertyEntity({ val: _quantidadePorTipoDeCarga.TiposCarga.val(), getType: _quantidadePorTipoDeCarga.TiposCarga.getType, list: obterQuantidadeTipoCargaTiposDeCargaSalvar(), type: types.listEntity });
    quantidadeTipoCarga["DescricaoTipoCarga"] = new PropertyEntity({ val: obterDescricaoTiposCarga(), getType: typesKnockout.string });

    return quantidadeTipoCarga;
}

function obterQuantidadeTipoCargaTiposDeCargaSalvar() {
    var listaTiposCarga = _gridQuantidadePorTipoDeCargaTipoCargas.BuscarRegistros();
    var listaRetornar = new Array();
    
    listaTiposCarga.forEach(function (tipoCarga) {
        listaRetornar.push({
            Codigo: { val: tipoCarga.Codigo, getType: "int", type: "map" },
            Descricao: { val: tipoCarga.Descricao, getType: "string", type: "map" }
        });
    });

    return listaRetornar;
}

function obterGridQuantidadeTiposCargaTiposCarga(lista) {
    var listaRetorno = [];
    lista.forEach(function (registro) {
        listaRetorno.push({ Codigo: registro.Codigo.val, Descricao: registro.Descricao.val });
    });
    
    return listaRetorno;
}

//#endregion

//#region Eventos

function adicionarQuantidadePorTipoDeCargaClick() {
    var valido = ValidarCamposObrigatorios(_quantidadePorTipoDeCarga);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    _quantidadePorTipoDeCarga.Codigo.val(guid());
    _quantidadePorTipoDeCarga.CodigoDia.val(_quantidadePorTipoDeCarga.Dia.val());
    _centroDescarregamento.QuantidadePorTipoDeCarga.list.push(obterQuantidadeTipoCargaSalvar());
    
    recarregarGridQuantidadePorTipoDeCarga();
    limparCamposQuantidadePorTipoDeCarga();
}

function atualizarQuantidadePorTipoDeCargaClick() {
    for (var i = 0; i < _centroDescarregamento.QuantidadePorTipoDeCarga.list.length; i++) {
        var registroAtual = _centroDescarregamento.QuantidadePorTipoDeCarga.list[i];
        if (registroAtual.Codigo.val == _quantidadePorTipoDeCarga.Codigo.val()) {

            _quantidadePorTipoDeCarga.CodigoDia.val(_quantidadePorTipoDeCarga.Dia.val());
            var quantidadeTipoCarga = obterQuantidadeTipoCargaSalvar();
            atualizarQuantidadePorTipoDeCargaPorCodigo(_quantidadePorTipoDeCarga.Codigo.val(), quantidadeTipoCarga);

            recarregarGridQuantidadePorTipoDeCarga();
            limparCamposQuantidadePorTipoDeCarga();

            break;
        }
    }
}

function excluirQuantidadePorTipoDeCargaClick() {
    for (var i = 0; i < _centroDescarregamento.QuantidadePorTipoDeCarga.list.length; i++) {
        var registroAtual = _centroDescarregamento.QuantidadePorTipoDeCarga.list[i];
        if (registroAtual.Codigo.val = _quantidadePorTipoDeCarga.Codigo.val()) {
            _centroDescarregamento.QuantidadePorTipoDeCarga.list.splice(i, 1);
            recarregarGridQuantidadePorTipoDeCarga();
            limparCamposQuantidadePorTipoDeCarga();
            break;
        }
    }
}

function cancelarQuantidadePorTipoDeCargaClick() {
    limparCamposQuantidadePorTipoDeCarga();
}

function removerQuantidadePorTipoCargaTipoDeCarga(registroSelecionado) {
    var tiposCarga = _gridQuantidadePorTipoDeCargaTipoCargas.BuscarRegistros();

    for (var i = 0; i < tiposCarga.length; i++) {
        if (registroSelecionado.Codigo == tiposCarga[i].Codigo) {
            tiposCarga.splice(i, 1);
            _gridQuantidadePorTipoDeCargaTipoCargas.CarregarGrid(tiposCarga);
            break;
        }
    }
}

//#endregion