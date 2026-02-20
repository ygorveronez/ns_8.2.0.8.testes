/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="Cliente.js" />
/// <reference path="TipoContainer.js" />

var BuscarLocalRetiradaContainer = function (knout, callbackRetorno, knoutContainerTipo) {
    var idDiv = guid();
    var gridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Local de Retirada de Container", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Locais de Retirada de Container", type: types.local });
        this.Local = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Local de Retirada", idBtnSearch: guid() });
        this.ContainerTipo = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Tipo do Container", idBtnSearch: guid(), visible: !Boolean(knoutContainerTipo) });
        this.MostraTodosLocais = PropertyEntity({ col: 2, text: "Mostrar locais indisponíveis", val: ko.observable(false), options: Global.ObterOpcoesBooleano(Localization.Resources.Gerais.Geral.Sim, Localization.Resources.Gerais.Geral.Nao), def: false, visible: true });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { gridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar" });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParametroDinamico = function () {
        if (knoutContainerTipo) {
            knoutOpcoes.ContainerTipo.codEntity(knoutContainerTipo.codEntity());
            knoutOpcoes.ContainerTipo.val(knoutContainerTipo.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, null, function () {
        new BuscarClientes(knoutOpcoes.Local);
        new BuscarTiposContainer(knoutOpcoes.ContainerTipo);
    });

    loadCargaRetiradaContainerDetalhes();

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    var menuOpcoes = divBusca.OpcaoPadrao(callback);

    menuOpcoes.tipo = TypeOptionMenu.list;

    var opcaoDetalhes = {
        descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), evento: "onclick", metodo: function (retorno) {
            ObterDetalhesLocalRetiradaContainerModalBusca(retorno)
        }, icone: ""
    };

    menuOpcoes.opcoes.push(opcaoDetalhes);

    gridConsulta = new GridView(idDiv + "_tabelaEntidades", "ControleContainer/PesquisaLocalRetiradaContainer", knoutOpcoes, menuOpcoes, null);

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Descricao.val(knout.val());

        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
                return;
            }

            divBusca.OpenModal();
        });
    });

    this.abrirBusca = function () {
        LimparCampos(knoutOpcoes);

        divBusca.UpdateGrid();
        divBusca.OpenModal();
    };
}
