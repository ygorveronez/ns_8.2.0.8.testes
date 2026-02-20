/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumGrupoTipoIntegracao.js" />

var BuscarTipoIntegracao = function (knout, callbackRetorno, titulo, tituloGrid, grupoTipoIntegracaoDefault = null, mostrarGrupoTipoIntegracao = false, basicGrid) {
    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: (titulo != null ? titulo : "Buscar Tipo de Integração"), type: types.local });
        this.TituloGrid = PropertyEntity({ text: (tituloGrid != null ? tituloGrid : "Tipos de Integração"), type: types.local });
        this.Descricao = PropertyEntity({ text: "Descrição:", col: 8 });
        this.GrupoTipoIntegracao = PropertyEntity({ text: "Grupo Tipo Integração:", col: 4, visible: ko.observable(true), val: ko.observable(grupoTipoIntegracaoDefault), options: EnumGrupoTipoIntegracao.obterOpcoesPesquisa(), def: grupoTipoIntegracaoDefault });
        this.Ativo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), visible: false });
        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    };
    var knoutOpcoes = new OpcoesKnout();
    if (grupoTipoIntegracaoDefault > 0) {
        knoutOpcoes.GrupoTipoIntegracao.val(grupoTipoIntegracaoDefault);
    }
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha);

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

        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoIntegracao/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoIntegracao/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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