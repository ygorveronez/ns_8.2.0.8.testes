/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../Enumeradores/EnumTipoMotivoRecusaCancelamento.js" />

var BuscarMotivoRecusaCancelamento = function (knout, TituloOpcional, TituloGridOpcional, cancelamento, recusa, callbackRetorno, basicGrid) {
    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var opcoes = [];
    if (cancelamento)
        opcoes = EnumTipoMotivoRecusaCancelamento.obterOpcoesCancelamento();
    else if (recusa)
        opcoes = EnumTipoMotivoRecusaCancelamento.obterOpcoesRecusa();

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : "Pesquisa Motivos Recusa/Cancelamento", type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : "Modelos", type: types.local });

        this.Descricao = PropertyEntity({ col: 8, text: "Descrição: " });
        this.TipoMotivoRecusaCancelamento = PropertyEntity({ col: 4, text: "Tipo: ", val: ko.observable(), options: ko.observable(opcoes) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoRecusaCancelamento/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "MotivoRecusaCancelamento/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
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
    });
};
