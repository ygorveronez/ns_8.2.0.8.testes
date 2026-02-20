/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Motorista.js" />

var BuscarAdiantamentosAgregado = function (knout, callbackRetorno, basicGrid, knoutCliente, knoutDataInicial, knoutDataFinal, knoutPagamentoAgregado) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Adiantamentos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Adiantamentos", type: types.local });
        this.Numero = PropertyEntity({ text: "Número: ", col: 3 });
        this.DataPagamento = PropertyEntity({ text: "Data Pagamento: ", col: 3, getType: typesKnockout.date, visible: true });
        this.Motorista = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: true });

        this.PagamentoAgregado = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Pagento Agregado:", idBtnSearch: guid(), visible: false });;
        this.Cliente = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: false });;
        this.DataInicial = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Data Inicial:", idBtnSearch: guid(), visible: false });;
        this.DataFinal = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Data Final:", idBtnSearch: guid(), visible: false });;
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutCliente != null && knoutDataInicial != null && knoutDataFinal != null && knoutPagamentoAgregado != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.PagamentoAgregado.codEntity(knoutPagamentoAgregado.val());
            knoutOpcoes.PagamentoAgregado.val(knoutPagamentoAgregado.val());

            knoutOpcoes.Cliente.codEntity(knoutCliente.codEntity());
            knoutOpcoes.Cliente.val(knoutCliente.val());

            knoutOpcoes.DataInicial.codEntity(knoutDataInicial.val());
            knoutOpcoes.DataInicial.val(knoutDataInicial.val());

            knoutOpcoes.DataFinal.codEntity(knoutDataFinal.val());
            knoutOpcoes.DataFinal.val(knoutDataFinal.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarMotoristas(knoutOpcoes.Motorista);
    });

    var callback = function (e) {
        preecherRetornoSelecao(knout, e, idDiv, knoutOpcoes);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PagamentoMotoristaTMS/PesquisaAdiantamentoAgregado", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "PagamentoMotoristaTMS/PesquisaAdiantamentoAgregado", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Numero.val(knout.val());
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