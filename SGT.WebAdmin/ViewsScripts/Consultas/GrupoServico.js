/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />


var BuscarGrupoServico = function (knout, callbackRetorno, knoutTipo) {
    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.GrupoServico.BuscarGrupoServico, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.GrupoServico.DescricaoGrupoServico, type: types.local });

        this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), col: 12 });
        this.Status = PropertyEntity({ val: ko.observable(1), options: _statusPesquisa, def: 1, visible: false });
        this.Tipo = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, required: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutTipo != null) {
        funcaoParametroDinamico = function () {
            knoutOpcoes.Tipo.val(knoutTipo.codEntity());
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        };
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "GrupoServico/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Descricao.val(knout.val());
        }
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    });

    this.CarregarGrupoServicoFornecedor = function () {
        LimparCampos(knoutOpcoes);

        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                LimparCampo(knout);
        });
    };
};