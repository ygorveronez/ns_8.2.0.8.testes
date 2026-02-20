/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../Consultas/Cliente.js" />
/// <reference path="../Consultas/Filial.js" />
/// <reference path="../Consultas/MotivoChamado.js" />
/// <reference path="../Consultas/Tranportador.js" />
/// <reference path="../Consultas/Veiculo.js" />

var BuscarChamadosParaOcorrencia = function (knout, callbackRetorno, knoutCarga, somenteValoresPendentes, retornoMultiplosChamados) {

    var idDiv = guid();
    var GridConsulta;

    var visibleCarga = true;
    if (knoutCarga != null)
        visibleCarga = false;

    if (somenteValoresPendentes == null)
        somenteValoresPendentes = false;

    var OpcoesKnout = function () {
        var isTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS;

        this.Titulo = PropertyEntity({ text: "Buscar Chamados", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Chamados", type: types.local });
        this.NumeroInicial = PropertyEntity({ col: 3, text: "Número Inicial: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
        this.NumeroFinal = PropertyEntity({ col: 3, text: "Número Final: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
        this.Carga = PropertyEntity({ col: 3, text: "Número Carga:", visible: visibleCarga });
        this.NotaFiscal = PropertyEntity({ col: 3, text: "Nota Fiscal: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10, visible: visibleCarga });
        this.Transportador = PropertyEntity({ col: 6, type: types.multiplesEntities, codEntity: ko.observable(0), text: (isTMS ? "Empresa/Filial" : "Transportador:"), idBtnSearch: guid(), visible: visibleCarga });
        this.Filial = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: (!isTMS && visibleCarga) });
        this.Cliente = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
        this.MotivoChamado = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Motivo do Chamado:", idBtnSearch: guid() });
        this.Veiculo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: visibleCarga });

        this.CargaCodigo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });        
        this.SituacaoChamado = PropertyEntity({ col: 6, val: ko.observable(EnumSituacaoChamado.LiberadaOcorrencia), options: EnumSituacaoChamado.obterOpcoesPesquisa(), def: EnumSituacaoChamado.LiberadaOcorrencia, text: "Situação: ", visible: ko.observable(true) });
        this.SomenteValoresPendentes = PropertyEntity({ val: ko.observable(somenteValoresPendentes), def: somenteValoresPendentes, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        knoutOpcoes.visible = false;

    var funcaoParametrosDinamicos = function () {
        if (knoutCarga != null) {
            knoutOpcoes.CargaCodigo.codEntity(knoutCarga.codEntity());
            knoutOpcoes.CargaCodigo.val(knoutCarga.val());
        }
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, null, function () {
        new BuscarClientes(knoutOpcoes.Cliente);
        new BuscarFilial(knoutOpcoes.Filial)
        new BuscarMotivoChamado(knoutOpcoes.MotivoChamado);
        new BuscarTransportadores(knoutOpcoes.Transportador);
        new BuscarVeiculos(knoutOpcoes.Veiculo);
    }, undefined, true, null, retornoMultiplosChamados);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        };
    }

    var opcoes = divBusca.OpcaoPadrao(callback)

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ChamadoOcorrencia/PesquisaChamado", knoutOpcoes, opcoes, null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroInicial.val(knout.val());
            knoutOpcoes.NumeroFinal.val(knout.val());
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

var BuscarTodosChamadosParaOcorrencia = function (knout, callbackRetorno, knoutCarga, somenteValoresPendentes, retornoMultiplosChamados) {

    var idDiv = guid();
    var GridConsulta;

    var visibleCarga = true;
    if (knoutCarga != null)
        visibleCarga = false;

    if (somenteValoresPendentes == null)
        somenteValoresPendentes = false;

    var OpcoesKnout = function () {
        var isTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS;

        this.Titulo = PropertyEntity({ text: "Buscar Chamados", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Chamados", type: types.local });
        this.NumeroInicial = PropertyEntity({ col: 3, text: "Número Inicial: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
        this.NumeroFinal = PropertyEntity({ col: 3, text: "Número Final: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10 });
        this.Carga = PropertyEntity({ col: 3, text: "Número Carga:", visible: visibleCarga });

        this.NotaFiscal = PropertyEntity({ col: 3, text: "Nota Fiscal: ", getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, maxlength: 10, visible: visibleCarga });
        this.Transportador = PropertyEntity({ col: 6, type: types.multiplesEntities, codEntity: ko.observable(0), text: (isTMS ? "Empresa/Filial" : "Transportador:"), idBtnSearch: guid(), visible: visibleCarga });
        this.Filial = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: (!isTMS && visibleCarga) });
        this.Cliente = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
        this.MotivoChamado = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Motivo do Chamado:", idBtnSearch: guid() });
        this.Veiculo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: visibleCarga });

        this.CargaCodigo = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: false });
        this.SomenteValoresPendentes = PropertyEntity({ val: ko.observable(somenteValoresPendentes), def: somenteValoresPendentes, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        knoutOpcoes.visible = false;

    var funcaoParametrosDinamicos = function () {
        if (knoutCarga != null) {
            knoutOpcoes.CargaCodigo.codEntity(knoutCarga.codEntity());
            knoutOpcoes.CargaCodigo.val(knoutCarga.val());
        }
    };

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametrosDinamicos, null, null, function () {
        new BuscarClientes(knoutOpcoes.Cliente);
        new BuscarFilial(knoutOpcoes.Filial)
        new BuscarMotivoChamado(knoutOpcoes.MotivoChamado);
        new BuscarTransportadores(knoutOpcoes.Transportador);
        new BuscarVeiculos(knoutOpcoes.Veiculo);
    }, undefined, true, null, retornoMultiplosChamados);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
            Global.setarFocoProximoCampo(knout.id);
        };
    }

    var opcoes = divBusca.OpcaoPadrao(callback)

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ChamadoOcorrencia/PesquisaChamado", knoutOpcoes, opcoes, null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroInicial.val(knout.val());
            knoutOpcoes.NumeroFinal.val(knout.val());
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