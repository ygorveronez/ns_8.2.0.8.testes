/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumTipoTabelaFrete.js" />

var BuscarLancamentoNFSManualParaCancelamento = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar NFS Manual", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "NFS Manual", type: types.local });

        this.DataInicio = PropertyEntity({ text: "Data Inicial: ", col: 3, getType: typesKnockout.date, visible: true, val: ko.observable() });
        this.DataFim = PropertyEntity({ text: "Data Final: ", col: 3, getType: typesKnockout.date, visible: true });
        this.DataInicio.dateRangeLimit = this.DataFim;
        this.DataFim.dateRangeInit = this.DataInicio;
        this.Numero = PropertyEntity({ text: "Número NFS:", maxlength: 12, col: 3, enable: ko.observable(true), getType: typesKnockout.int });
        this.NumeroDOC = PropertyEntity({ text: "Numero NF-e:", maxlength: 12, col: 3, enable: ko.observable(true), getType: typesKnockout.int });
        this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), col: 12, text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
        
        this.BuscaAvancada = PropertyEntity({
            eventClick: function (e) {
                if (e.BuscaAvancada.visibleFade() == true) {
                    e.BuscaAvancada.visibleFade(false);
                    e.BuscaAvancada.icon("fal fa-plus");
                } else {
                    e.BuscaAvancada.visibleFade(true);
                    e.BuscaAvancada.icon("fal fa-minus");
                }
            }, buscaAvancada: true, type: types.event, text: "Avançada", idFade: guid(), cssClass: "btn btn-default", icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: true
        });

        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), col: 6, text: "Carga:", idBtnSearch: guid() });
        this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), col: 6, text: "Local da Prestação:", idBtnSearch: guid() });
        this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), col: 6, text: "Filial:", idBtnSearch: guid() });
        this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), col: 6, text: "Tomador:", idBtnSearch: guid() });
        
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        knoutOpcoes.Empresa.visible(false);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        knoutOpcoes.Empresa.text("Empresa/Filial: ");
        knoutOpcoes.Filial.visible = false;
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null, function () {
        new BuscarTransportadores(knoutOpcoes.Empresa);
        new BuscarCargas(knoutOpcoes.Carga);
        
        new BuscarLocalidades(knoutOpcoes.LocalidadePrestacao, null);
        new BuscarClientes(knoutOpcoes.Tomador, null);
        new BuscarFilial(knoutOpcoes.Filial);
    }, null, true);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "NFSManual/PesquisaParaCancelamento", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.Numero.val(knout.val());
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