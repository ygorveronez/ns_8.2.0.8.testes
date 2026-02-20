/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarModalidade = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar modalidades", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Modalidade", type: types.local });

        this.Descricao = PropertyEntity({ text: "Descrição:", col: 8 });
        this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _statusPesquisa, def: true, col: 4 });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }
    var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ModalidadeContratoFinanciamento/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    divBusca.AddEvents(GridConsulta);

}