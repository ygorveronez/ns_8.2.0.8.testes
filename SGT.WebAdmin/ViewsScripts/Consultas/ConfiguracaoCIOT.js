/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Consultas/Empresa.js" />

var BuscarConfiguracaoCIOT = function (knout, titulo, tituloGrid, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: titulo || Localization.Resources.Consultas.ConfiguracaoCIOT.ConfigurcoesDeCIOT, type: types.local });
        this.TituloGrid = PropertyEntity({ text: tituloGrid || Localization.Resources.Consultas.ConfiguracaoCIOT.Configuracoes, type: types.local });
        this.Descricao = PropertyEntity({ text: Localization.Resources.Consultas.ConfiguracaoCIOT.Descricao.getFieldDescription(), col: 12 });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), text: Localization.Resources.Consultas.ConfiguracaoCIOT.Ativo, visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Consultas.ConfiguracaoCIOT.Pesquisar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, false, function () { });

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ConfiguracaoCIOT/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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