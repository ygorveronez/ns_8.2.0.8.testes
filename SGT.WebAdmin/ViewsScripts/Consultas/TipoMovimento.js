/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="PlanoConta.js" />
/// <reference path="../Enumeradores/EnumAnaliticoSintetico.js" />

var BuscarTipoMovimento = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno, knoutFormaTipoMovimento, finalidadeTipoMovimento, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = basicGrid != null;

    var finalidade = 0;
    if (finalidadeTipoMovimento != null)
        finalidade = finalidadeTipoMovimento;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : Localization.Resources.Consultas.TipoMovimento.PesquisaDeTipoDeMovimento, type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : Localization.Resources.Consultas.TipoMovimento.TiposDeMovimento, type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
        this.FormaTipoMovimento = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.TipoMovimento.Tipo.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.FinalidadeTipoMovimento = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(finalidade), text: Localization.Resources.Consultas.TipoMovimento.Finalidade.getFieldDescription(), visible: false });
        this.PlanoDebito = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.TipoMovimento.PlanoDeEntrada.getFieldDescription(), idBtnSearch: guid(), val: ko.observable(""), visible: true });
        this.PlanoCredito = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.TipoMovimento.PlanoDeSaida.getFieldDescription(), idBtnSearch: guid(), val: ko.observable(""), visible: true });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutFormaTipoMovimento != null) {
        knoutOpcoes.FormaTipoMovimento.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.FormaTipoMovimento.codEntity(knoutFormaTipoMovimento);
            knoutOpcoes.FormaTipoMovimento.val(knoutFormaTipoMovimento);
        };
    }    

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarPlanoConta(knoutOpcoes.PlanoDebito, "", "", null, EnumAnaliticoSintetico.Analitico);
        new BuscarPlanoConta(knoutOpcoes.PlanoCredito, "", "", null, EnumAnaliticoSintetico.Analitico);
    });

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
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoMovimento/Pesquisa", knoutOpcoes, null, { column: 2, dir: orderDir.asc }, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "TipoMovimento/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 2, dir: orderDir.asc });
    }

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