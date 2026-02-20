/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarJustificativas = function (knout, callbackRetorno, tipoJustificativa, finalidadeJustificativa, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var tipo = "";
    var finalidade = new Array();

    if (finalidadeJustificativa != null)
        finalidade = [].concat(finalidadeJustificativa);

    if (tipoJustificativa != null)
        tipo = tipoJustificativa;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Justificativa.BuscarJustificativa, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Justificativa.Justificativas, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.Justificativa.Descricao.getFieldDescription(), maxlength: 250 });
        this.TipoJustificativa = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(tipo), text: Localization.Resources.Consultas.Justificativa.Tipo.getFieldDescription(), visible: false });
        this.FinalidadeJustificativa = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(JSON.stringify(finalidade)), text: Localization.Resources.Consultas.Justificativa.Finalidade.getFieldDescription(), visible: false });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), text: Localization.Resources.Consultas.Justificativa.Ativo.getFieldDescription(), visible: false });
        this.GerarPendenciaMotorista = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(true), text: Localization.Resources.Consultas.Justificativa.Ativo.getFieldDescription(), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Consultas.Justificativa.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha);

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Justificativa/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Justificativa/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}