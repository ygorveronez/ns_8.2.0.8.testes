/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />

var BuscarPlanoConta = function (knout, TituloOpcional, TituloGridOpcional, callbackRetorno, knoutTipoPlanoConta, knoutFormaTipoMovimento, filtraGrupoResultado, planoContaDebito, planoContaCredito, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    let multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: TituloOpcional != null ? TituloOpcional : Localization.Resources.Consultas.PlanoConta.PesquisaDePlacaDeConta, type: types.local });
        this.TituloGrid = PropertyEntity({ text: TituloGridOpcional != null ? TituloGridOpcional : Localization.Resources.Consultas.PlanoConta.PlanosDeContas, type: types.local });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });

        this.PlanoContaDebito = PropertyEntity({ val: ko.observable(0), visible: false });
        this.PlanoContaCredito = PropertyEntity({ val: ko.observable(0), visible: false });

        this.Plano = PropertyEntity({ col: 6, text: Localization.Resources.Consultas.PlanoConta.Plano.getFieldDescription() });
        this.Tipo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Tipo.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.FormaTipoMovimento = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Tipo.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
        this.ComGrupoResultado = PropertyEntity({ val: ko.observable("N"), def: "N", visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutTipoPlanoConta != null && knoutFormaTipoMovimento != null) {
        knoutOpcoes.Tipo.visible = false;
        knoutOpcoes.FormaTipoMovimento.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Tipo.codEntity(knoutTipoPlanoConta);
            knoutOpcoes.Tipo.val(knoutTipoPlanoConta);
            knoutOpcoes.FormaTipoMovimento.codEntity(knoutFormaTipoMovimento);
            knoutOpcoes.FormaTipoMovimento.val(knoutFormaTipoMovimento);
            knoutOpcoes.PlanoContaDebito.val(planoContaDebito != null ? planoContaDebito.val() : 0);
            knoutOpcoes.PlanoContaCredito.val(planoContaCredito != null ? planoContaCredito.val() : 0);
        }
    } else if (knoutTipoPlanoConta != null) {
        knoutOpcoes.Tipo.visible = false;
        knoutOpcoes.FormaTipoMovimento.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Tipo.codEntity(knoutTipoPlanoConta);
            knoutOpcoes.Tipo.val(knoutTipoPlanoConta);
            knoutOpcoes.PlanoContaDebito.val(planoContaDebito != null ? planoContaDebito.val() : 0);
            knoutOpcoes.PlanoContaCredito.val(planoContaCredito != null ? planoContaCredito.val() : 0);
        }
    } else if (knoutFormaTipoMovimento != null) {
        knoutOpcoes.Tipo.visible = false;
        knoutOpcoes.FormaTipoMovimento.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.FormaTipoMovimento.codEntity(knoutFormaTipoMovimento);
            knoutOpcoes.FormaTipoMovimento.val(knoutFormaTipoMovimento);
            knoutOpcoes.PlanoContaDebito.val(planoContaDebito != null ? planoContaDebito.val() : 0);
            knoutOpcoes.PlanoContaCredito.val(planoContaCredito != null ? planoContaCredito.val() : 0);
        }
    }

    if (filtraGrupoResultado != null) {
        knoutOpcoes.ComGrupoResultado.val(filtraGrupoResultado);
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    let url = "PlanoConta/Pesquisa";

    if (multiplaEscolha) {
        let objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 2, dir: orderDir.asc });
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
    })
}