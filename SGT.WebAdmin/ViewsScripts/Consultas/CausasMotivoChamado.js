/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarCausasMotivoChamado = function (knout, knouMotivoAtendimento, callbackRetorno, basicGrid ) {
    var idDiv = guid();
    var GridConsulta;
 
    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Causas Motivo Atendimento", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Causas Motivo Atendimento", type: types.local });
        this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), col: 9 });
        this.CodigoMotivoChamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
        this.BuscarTodasCausasDesconsiderandoMotivoChamado = PropertyEntity({ val: ko.observable(false), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var funcaoParametrosDinamicos = function () {
        if (knouMotivoAtendimento != null) {
            knoutOpcoes.CodigoMotivoChamado.codEntity(knouMotivoAtendimento.codEntity());
            knoutOpcoes.CodigoMotivoChamado.val(knouMotivoAtendimento.val());
            knoutOpcoes.BuscarTodasCausasDesconsiderandoMotivoChamado.val(false);
        } else {
            knoutOpcoes.BuscarTodasCausasDesconsiderandoMotivoChamado.val(true);
        }
    };

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoChamado/PesquisaCausasMotivoChamado", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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
    });
}