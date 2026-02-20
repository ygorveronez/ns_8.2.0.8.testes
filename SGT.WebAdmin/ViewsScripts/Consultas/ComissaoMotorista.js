var BuscarComissoesMotoristas = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Comissões de Motoristas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Comissões de Motoristas", type: types.local });
        this.DataInicio = PropertyEntity({ col: 6, text: "Data Inicial: ", getType: typesKnockout.date });
        this.DataFim = PropertyEntity({ col: 6, text: "Data Final: ", getType: typesKnockout.date });
        this.SituacaoComissaoFuncionario = PropertyEntity({ val: ko.observable(EnumSituacaoComissaoFuncionario.Finalizada), def: EnumSituacaoComissaoFuncionario.Finalizada, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, false);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Descricao);
        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ComissaoFuncionario/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        divBusca.OpenModal();
    });
}