/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarProgramacaoEspecialidade = function (knout, callbackRetorno, finalidadeProgramacaoEspecialidade) {

    var idDiv = guid();
    var GridConsulta;

    var finalidade = new Array();

    if (finalidadeProgramacaoEspecialidade !== null)
        finalidade = [].concat(finalidadeProgramacaoEspecialidade);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Especialidade", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Especialidades", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Descrição: ", maxlength: 250 });
        this.Finalidade = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(JSON.stringify(finalidade)), text: "Finalidade:", visible: false });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), text: "Ativo:", visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno !== null) {
        callback = function (e) {
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ProgramacaoEspecialidade/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length === 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};