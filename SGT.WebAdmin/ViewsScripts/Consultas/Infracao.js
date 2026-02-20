/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="ModeloVeicularCarga.js" />
/// <reference path="Tranportador.js" />
/// <reference path="../../ViewsScripts/Configuracao/Sistema/ConfiguracaoTMS.js" />

var BuscarInfracoes = function (knout, callbackRetorno, basicGrid, knoutMotorista) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {

        this.Titulo = PropertyEntity({ text: "Busca de Infrações", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Infrações", type: types.local });
        this.Placa = PropertyEntity({ col: 5, text: "Placa: ", maxlength: 7 });
        this.NumeroAtuacao = PropertyEntity({ col: 4, text: "Nº Autuação: ", maxlength: 30, visible: true });
        this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: false });
        this.InfracoesPendentes = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: ko.observable(true), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametrosDinamicos = null;

    if (knoutMotorista != null) {
        funcaoParametrosDinamicos = function () {
            knoutOpcoes.Motorista.codEntity(knoutMotorista.codEntity());
            knoutOpcoes.Motorista.val(knoutMotorista.val());
        }
    }
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, multiplaEscolha);

    $("#" + knoutOpcoes.Placa.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });
    var callback = function (e) {
        var aprovado = true;
        if (aprovado) {
            knout.codEntity(e.Codigo);
            knout.val(e.Placa);
            knoutOpcoes.Placa.val(knoutOpcoes.Placa.def);

            knout.requiredClass("form-control");

            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        }
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Infracao/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Infracao/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Placa.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });

    this.AbrirBusca = function () {
        LimparCampos(knoutOpcoes);

        divBusca.UpdateGrid();
        divBusca.OpenModal();
    };
}
