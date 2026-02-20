/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Consultas/Cliente.js" />
/// <reference path="../Enumeradores/EnumStatusCheque.js" />
/// <reference path="../Enumeradores/EnumTipoCheque.js" />

var BuscarCheque = function (knout, callbackRetorno, basicGrid, listaTipos) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = basicGrid != null;
    var listaTiposPesquisar = listaTipos || [];

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Cheque", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Cheques", type: types.local });
        this.Pessoa = PropertyEntity({ type: types.entity, col: 12, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid() });
        this.NumeroCheque = PropertyEntity({ text: "Nº Cheque:", col: 4 });
        this.Status = PropertyEntity({ text: "Status: ", col: 4, val: ko.observable(EnumStatusCheque.Todos), options: EnumStatusCheque.obterOpcoesPesquisa(), def: EnumStatusCheque.Todos });
        this.Tipo = PropertyEntity({ text: "Tipo: ", col: 4, val: ko.observable(EnumTipoCheque.Todos), options: EnumTipoCheque.obterOpcoesPesquisa(), def: EnumTipoCheque.Todos, visible: true });
        this.Tipos = PropertyEntity({ getType: typesKnockout.selectMultiple, val: ko.observable(listaTiposPesquisar), def: [], options: EnumTipoCheque.obterOpcoesPesquisa(), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();

    if (listaTiposPesquisar.length > 0) {
        knoutOpcoes.Tipo.visible = false;
        knoutOpcoes.NumeroCheque.col = 6;
        knoutOpcoes.Status.col = 6;
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        new BuscarClientes(knoutOpcoes.Pessoa);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };

        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Cheque/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Cheque/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroCheque.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}