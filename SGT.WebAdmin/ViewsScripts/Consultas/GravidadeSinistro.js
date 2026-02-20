var BuscarGravidadeSinistro = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: 'Busca de Gravidade de Sinistro', type: types.local });
        this.TituloGrid = PropertyEntity({ text: 'Gravidade de Sinistros', type: types.local });

        this.Descricao = PropertyEntity({ col: 5, text: 'Descricao:' });
        this.Status = PropertyEntity({ col: 3, text: 'Status:', val: ko.observable(1), options: _statusPesquisa, def: 1, visible: true });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParamentroDinamico = null;

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);

    var callback = function (e) {
        knout.codEntity(e.Codigo);
        knout.val(e.Descricao);
        knoutOpcoes.Descricao.val(knoutOpcoes.Descricao.def);

        knout.requiredClass("form-control");

        divBusca.CloseModal();
        Global.setarFocoProximoCampo(knout.id);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "GravidadeSinistro/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback));

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
};