/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarCausasTipoOcorrencia = function (knout, knouTipoOcorrencia, callbackRetorno, basicGrid ) {
    var idDiv = guid();
    var GridConsulta;
 
    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Causas Tipo de Ocorrência", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Causas Tipo de Ocorrência", type: types.local });
        this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), col: 9 });
        this.CodigoTipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
        this.BuscarTodasCausasDesconsiderandoTipoOcorrencia = PropertyEntity({ val: ko.observable(false), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var funcaoParametrosDinamicos = function () {
        if (knouTipoOcorrencia != null) {
            knoutOpcoes.CodigoTipoOcorrencia.codEntity(knouTipoOcorrencia.codEntity());
            knoutOpcoes.CodigoTipoOcorrencia.val(knouTipoOcorrencia.val());
            knoutOpcoes.BuscarTodasCausasDesconsiderandoTipoOcorrencia.val(false);
        } else {
            knoutOpcoes.BuscarTodasCausasDesconsiderandoTipoOcorrencia.val(true);
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoOcorrencia/PesquisaCausasTipoOcorrencia", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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