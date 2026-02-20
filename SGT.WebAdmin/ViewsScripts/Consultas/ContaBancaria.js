/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Cliente.js" />

var BuscarContasBancarias = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;
    var buscaCliente;


    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Busca de Conta Bancária", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Conta Bancária", type: types.local });
        this.Descricao = PropertyEntity({ col: 8, text: "Descrição: ", visible: false });
        this.NumeroConta = PropertyEntity({ text: Localization.Resources.Consultas.GrupoPessoa.NumeroConta, type: types.local });
        this.ClientePortadorConta = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Pessoa.getFieldDescription(), idBtnSearch: guid(), visible: true });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        buscaCliente = new BuscarClientes(knoutOpcoes.ClientePortadorConta);
    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha)
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContaBancaria/Pesquisa", knoutOpcoes, null, null, null, null, null, null, { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar });
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ContaBancaria/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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

    this.Destroy = function () {
        if (buscaCliente)
            buscaCliente.Destroy();

        divBusca.Destroy();
    };
}