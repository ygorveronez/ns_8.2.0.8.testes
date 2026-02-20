/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarModelosVeicularesCarga = function (knout, callbackRetorno, idTipoCarga, knouTipoCarga, tipos, knouTipoVeiculo, basicGrid, selectTipoCarga, knoutGrupoPessoas, knoutOuCodigoCarga, filtrarModelosTipoCargaCentroCarregamento, knoutDestinatario) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null) {
        multiplaEscolha = true;
    }

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ModeloVeicularCarga.PesquisarModelosVeicularesCarga, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ModeloVeicularCarga.ModelosVeicularesCarga, type: types.local });

        this.TipoCarga = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });
        this.Destinatario = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });
        this.GrupoPessoa = PropertyEntity({ type: types.entity, visible: false, codEntity: ko.observable(0) });
        this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, visible: false });
        this.Descricao = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.CapacidadePesoTransporte = PropertyEntity({ col: 4, getType: typesKnockout.decimal, maxlength: 10, text: Localization.Resources.Consultas.ModeloVeicularCarga.CapacidadeCarga.getFieldDescription() });
        this.Tipos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(JSON.stringify(tipos)), idGrid: guid(), visible: false });
        this.TipoVeiculo = PropertyEntity({ val: ko.observable(3), def: 3, visible: false });
        this.FiltrarModelosTipoCargaCentroCarregamento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    
    if (filtrarModelosTipoCargaCentroCarregamento == true)
        knoutOpcoes.FiltrarModelosTipoCargaCentroCarregamento.val(true);

    var funcaoParamentroDinamico = null;
    var url = "ModeloVeicularCarga/Pesquisa";
    var knoutCarga;

    if (knoutOuCodigoCarga instanceof Object)
        knoutCarga = knoutOuCodigoCarga;
    else if (knoutOuCodigoCarga > 0)
        knoutOpcoes.Carga.val(knoutOuCodigoCarga);

    funcaoParamentroDinamico = function () {
        if (knoutCarga)
            knoutOpcoes.Carga.val(knoutCarga.val());

        if (knoutGrupoPessoas != null) {
            knoutOpcoes.GrupoPessoa.codEntity(knoutGrupoPessoas.codEntity());
            knoutOpcoes.GrupoPessoa.val(knoutGrupoPessoas.val());
        }

        if (knouTipoCarga != null) {
            knoutOpcoes.TipoCarga.codEntity(knouTipoCarga.codEntity());
            knoutOpcoes.TipoCarga.val(knouTipoCarga.val());
        }

        if (knouTipoVeiculo != null)
            knoutOpcoes.TipoVeiculo.val(knouTipoVeiculo.val());

        if (knoutDestinatario != null) {
            knoutOpcoes.Destinatario.codEntity(knoutDestinatario.val());
            knoutOpcoes.Destinatario.val(knoutDestinatario.val());
        }

        if (knoutGrupoPessoas != null) {
            knoutOpcoes.GrupoPessoa.codEntity(knoutGrupoPessoas.codEntity());
            knoutOpcoes.GrupoPessoa.val(knoutGrupoPessoas.val());
        }

        if (selectTipoCarga != null) {
            knoutOpcoes.TipoCarga.codEntity(selectTipoCarga.val());
            knoutOpcoes.TipoCarga.val(selectTipoCarga.val());
        }
    };

    if (idTipoCarga != null) {
        knoutOpcoes.TipoCarga.codEntity(idTipoCarga);
        knoutOpcoes.TipoCarga.val("Tipo Carga");
        url = "TipoCarga/PesquisaModelosVeiculares";
    }
    else if (knouTipoCarga != null) {
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
            url = "TipoCarga/PesquisaModelosVeiculares";
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha);

    var callback = function (e) {
        divBusca.DefCallback(e);
    };

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        };
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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