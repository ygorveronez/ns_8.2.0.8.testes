/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarMotivoRetiradaFilaCarregamento = function (knout, callbackRetorno, exibirMotivosMobile) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Motivo da Retirada da Fila de Carregamento", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Motivos da Retirada", type: types.local });
        this.Descricao = PropertyEntity({ text: "Descrição:", col: 12 });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), visible: false });
        this.Mobile = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(exibirMotivosMobile ? null : false), visible: false });
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoRetiradaFilaCarregamento/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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