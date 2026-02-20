/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarTipoOleo = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;
    var buscaProdutos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.TipoOleo.PesquisarTipoOleo, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.TipoOleo.TipoDeOleo, type: types.local });

        this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), col: 5 });
        this.TipoDeOleo = PropertyEntity({ text: "Tipo de Óleo:", required: false, getType: typesKnockout.string, val: ko.observable(""), col: 3});
        this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Material:", idBtnSearch: guid(), col: 4 });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null, function () {
        buscaProdutos = new BuscarProdutoTMS(knoutOpcoes.Produto);
    });

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Descricao);
        divBusca.CloseModal();
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoOleo/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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
};