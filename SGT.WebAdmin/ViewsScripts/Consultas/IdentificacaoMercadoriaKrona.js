/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/Globais.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/Mensagem.js" />
/// <reference path="../../js/Global/Rest.js" />

var BuscarIdentificacaoMercadoriaKrona = function (knout, callbackRetorno) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Identificação de Mercadoria da Krona", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Identificações de Mercadorias da Krona", type: types.local });
        this.Identificador = PropertyEntity({ text: "Identificador:", col: 4, getType: typesKnockout.int });
        this.IdentificadorDescricao = PropertyEntity({ text: "Descrição:", col: 8 });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), text: "Ativo:", visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "IdentificacaoMercadoriaKrona/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.IdentificadorDescricao.val(knout.val());
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