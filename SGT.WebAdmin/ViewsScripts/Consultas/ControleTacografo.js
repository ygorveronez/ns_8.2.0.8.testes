/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarControleTacografo = function (knout, callbackRetorno, basicGrid, consultaAcerto) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;
    if (consultaAcerto == null)
        consultaAcerto = false;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Tacógrafo", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Tacógrafos", type: types.local });

        this.Codigo = PropertyEntity({ col: 6, text: "Código: " });        
        this.ConsultaAcerto = PropertyEntity({ col: 12, text: "Para Acerto: ", visible: false, val: ko.observable(consultaAcerto), def: ko.observable(consultaAcerto) });
        this.Excesso = PropertyEntity({ col: 12, text: "Excesso: ", visible: false, val: ko.observable(9), def: ko.observable(9) });
        this.Status = PropertyEntity({ col: 12, text: "Status: ", visible: false, val: ko.observable(1), def: ko.observable(1) });               

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ControleTacografo/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ControleTacografo/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Codigo.val(knout.val());
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