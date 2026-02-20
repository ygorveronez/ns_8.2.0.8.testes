/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarTipoInfracao = function (knout, callbackRetorno, titulo, tituloGrid) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: (titulo != null ? titulo : "Buscar Tipo de Ocorrência"), type: types.local });
        this.TituloGrid = PropertyEntity({ text: (tituloGrid != null ? tituloGrid : "Tipos de Ocorrência"), type: types.local });
        this.Descricao = PropertyEntity({ text: "Descrição:", col: 8 });
        this.CodigoCTB = PropertyEntity({ text: "Código C.T.B:", col: 4 });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoInfracao/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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