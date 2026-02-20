/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../ViewsScripts/Enumeradores/EnumTipoMotivoRejeicaoAlteracaoPedido.js" />

var BuscarMotivoRejeicaoAlteracaoPedido = function (knout, callbackRetorno, tipoMotivoRejeicao) {
    var idDiv = guid();
    var GridConsulta;
    var tipo = EnumTipoMotivoRejeicaoAlteracaoPedido.Todos;

    if (tipoMotivoRejeicao)
        tipo = tipoMotivoRejeicao;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Motivo de Rejeição daAlteração de Pedido", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Motivos de Rejeição", type: types.local });
        this.Descricao = PropertyEntity({ text: "Descrição:", col: 12 });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), text: "Ativo:", visible: false });
        this.Tipo = PropertyEntity({ val: ko.observable(tipo), options: EnumTipoMotivoRejeicaoAlteracaoPedido.obterOpcoes(), visible: false });
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

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoRejeicaoAlteracaoPedido/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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