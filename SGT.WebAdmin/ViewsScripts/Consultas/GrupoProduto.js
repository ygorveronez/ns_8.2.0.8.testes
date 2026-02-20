/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

const BuscarGruposProdutos = function (knout, callbackRetorno, basicGrid, somenteChecklist) {

    const idDiv = guid();
    let GridConsulta;

    let multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    const OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Grupos de Produtos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Grupos de Produtos", type: types.local });
        this.Descricao = PropertyEntity({ col: 8, text: "Descrição: ", maxlength: 250 });
        this.CodigoGrupoProdutoEmbarcador = PropertyEntity({ col: 4, maxlength: 50, text: "Código: " });
        this.SomenteChecklist = PropertyEntity({ val: ko.observable(Boolean(somenteChecklist)), def: Boolean(somenteChecklist), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    let knoutOpcoes = new OpcoesKnout();

    let divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha);

    let callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        const objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "GrupoProduto/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "GrupoProduto/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Descricao.val(knout.val());

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    })
}