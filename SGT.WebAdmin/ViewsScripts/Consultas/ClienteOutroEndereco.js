/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Tranportador.js" />
/// <reference path="../../js/Global/Validacao.js" />

var BuscarClienteOutroEndereco = function (knout, callbackRetorno, knoutCliente, filtrarCliente) {

    var idDiv = guid();
    var GridConsulta;

    if (filtrarCliente == null)
        filtrarCliente = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca outros endereços", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Outros Endereços", type: types.local });
        this.Cliente = PropertyEntity({ col: 12, type: types.entity, text: "Cliente:", codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
        this.Localidade = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Localidade:", idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;
    if (knoutCliente != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Cliente.codEntity(knoutCliente.codEntity());
            knoutOpcoes.Cliente.val(knoutCliente.val());
        }
    }

    if (filtrarCliente)
        knoutOpcoes.Cliente.visible(true);

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, null, function () {
        new BuscarLocalidades(knoutOpcoes.Localidade);
        new BuscarClientes(knoutOpcoes.Cliente);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ClienteOutroEndereco/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    })
}