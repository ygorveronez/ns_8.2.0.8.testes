/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarMotivoDesacordo = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Motivo de Desacordo", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Motivos", type: types.local });

        this.Descricao = PropertyEntity({ col: 3, text: "Descrição:", getType: typesKnockout.string, val: ko.observable("") });
        this.Situacao = PropertyEntity({ col: 3, text: "Situação: ", val: ko.observable(0), options: _statusFemPesquisa, def: 0 });
        this.SubstituiCTe = PropertyEntity({ col: 3, text: "Substitui CT-e: ", val: ko.observable(true), options: EnumSimNao.obterOpcoes(), def: true });
        this.Irregularidade = PropertyEntity({ col: 3, type: types.entity, codEntity: ko.observable(0), text: "Irregularidade:", idBtnSearch: guid() });


        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), text: Localization.Resources.Consultas.Justificativa.Ativo.getFieldDescription(), visible: false });
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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoDesacordo/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoDesacordo/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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