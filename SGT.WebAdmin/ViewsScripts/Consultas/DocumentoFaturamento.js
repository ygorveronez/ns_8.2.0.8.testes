/// <reference path="../Enumeradores/EnumTipoCTe.js" />

var BuscarDocumentosFaturamentoParaFatura = function (knout, callbackRetorno, basicGrid, knoutFatura, exibirFiltroContainer) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = (basicGrid != null);

    var _tipoDocumentoCTe = [
        { text: "Todos", value: EnumTipoCTe.Todos },
        { text: "Normal", value: EnumTipoCTe.Normal },
        { text: "Complementar", value: EnumTipoCTe.Complementar },
        { text: "Substitutos", value: EnumTipoCTe.Substituicao }
    ];

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Consulta de Documentos para Fatura", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Documentos", type: types.local });
        this.Fatura = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });
        this.NumeroDocumento = PropertyEntity({ col: 2, text: "Documento: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' } });
        this.Carga = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: true });
        this.DataInicial = PropertyEntity({ col: 2, getType: typesKnockout.date, visible: true, text: "Data Inicial:" });
        this.DataFinal = PropertyEntity({ col: 2, getType: typesKnockout.date, visible: true, text: "Data Final:" });
        this.Serie = PropertyEntity({ col: 3, getType: typesKnockout.int, visible: true, text: "Série:" });
        this.NumeroDocumentoInicial = PropertyEntity({ col: 2, getType: typesKnockout.int, visible: true, text: "Numero Documento Inicial:" });
        this.NumeroDocumentoFinal = PropertyEntity({ col: 2, getType: typesKnockout.int, visible: true, text: "Numero Documento Final:" });
        this.TipoDocumento = PropertyEntity({ col: 3, val: ko.observable(EnumTipoCTe.Todos), options: _tipoDocumentoCTe, def: -1, text: "Tipo de Documento CT-e: " });
        this.Notas = PropertyEntity({ col: 2, text: "Notas: ", getType: typesKnockout.string, });
        this.Container = PropertyEntity({ col: 3, text: "Container: ", getType: typesKnockout.string, visible: ko.observable(false) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }
    var knoutOpcoes = new OpcoesKnout();

    if (exibirFiltroContainer != null)
        knoutOpcoes.Container.visible = exibirFiltroContainer;

    var funcaoParamentroDinamico = null;

    var url = "FaturaDocumento/PesquisaDocumentosParaFatura";

    funcaoParamentroDinamico = function () {
        if (knoutFatura != null) {
            knoutOpcoes.Fatura.codEntity(knoutFatura.val());
            knoutOpcoes.Fatura.val(knoutFatura.val());
        }
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarCargas(knoutOpcoes.Carga, null, null, [EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.AgIntegracao, EnumSituacoesCarga.EmTransporte]);
    }, null, true);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, { column: 0, dir: orderDir.desc }, null, null, null, null, objetoBasicGrid, 2000);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 0, dir: orderDir.desc });
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroDocumento.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    })
}

var BuscarDocumentosFaturamentoEmAberto = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Consulta de Documentos para Faturamento", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Documentos", type: types.local });
        this.NumeroDocumento = PropertyEntity({ col: 4, text: "Documento: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' } });
        this.GrupoPessoas = PropertyEntity({ col: 8, type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid() });
        this.Tomador = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = null;

    var url = "DocumentoFaturamento/PesquisaDocumentosEmAberto";

    funcaoParamentroDinamico = function () {
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarGruposPessoas(knoutOpcoes.GrupoPessoas);
        new BuscarClientes(knoutOpcoes.Tomador);
    });

    var callback = function (e) {
        knout.val(e.Documento);
        knout.codEntity(e.Codigo);
        divBusca.CloseModal();
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.NumeroDocumento.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    })
}