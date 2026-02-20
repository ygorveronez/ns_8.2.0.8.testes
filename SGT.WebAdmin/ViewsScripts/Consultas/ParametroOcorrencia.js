/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumTipoParametroOcorrencia.js" />

var BuscarParametroOcorrencia = function (knout, callbackRetorno, tipo, knoutFiltrarParametrosPeriodo) {
    var idDiv = guid();
    var GridConsulta;
    var tipoParametroOcorrencia = tipo || EnumTipoParametroOcorrencia.Todos;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Parâmetros de Ocorrência", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Componentes de Parâmetros de Ocorrência", type: types.local });
        this.Descricao = PropertyEntity({ text: "Descrição: ", col: 12 });
        this.Tipo = PropertyEntity({ text: "Tipo: ", val: ko.observable(tipoParametroOcorrencia), options: EnumTipoParametroOcorrencia.obterOpcoesPesquisa(), def: EnumTipoParametroOcorrencia.Todos, visible: false });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), visible: false });
        this.FiltrarParametrosPeriodo = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutFiltrarParametrosPeriodo != null) {
        funcaoParamentroDinamico = function () {
            knoutOpcoes.FiltrarParametrosPeriodo.val(knoutFiltrarParametrosPeriodo.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ParametroOcorrencia/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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