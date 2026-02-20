/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumTipoTabelaFrete.js" />

var BuscarCargaMDFeManualParaCancelamento = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar MDF-e Manual", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "MDF-e Manual", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Descrição: " });
        this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), col: 4, text: "Carga:", idBtnSearch: guid() });
        this.MDFe = PropertyEntity({ text: "Número MDF-e:", maxlength: 12, col: 4, enable: ko.observable(true) });
        this.CTe = PropertyEntity({ text: "Número CT-e:", maxlength: 12, col: 4, enable: ko.observable(true) });

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

        this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), col: 12, text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
        this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), col: 6, text: "Origem:", idBtnSearch: guid() });
        this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), col: 6, text: "Destino:", idBtnSearch: guid() });
        this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), col: 6, text: "Veículo:", idBtnSearch: guid() });
        this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), col: 6, text: "Motorista:", idBtnSearch: guid() });

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
    }
    
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, null, function () {
        new BuscarTransportadores(knoutOpcoes.Empresa);
        new BuscarCargas(knoutOpcoes.Carga);
        new BuscarVeiculos(knoutOpcoes.Veiculo, null, knoutOpcoes.Empresa);
        new BuscarMotoristas(knoutOpcoes.Motorista, null, knoutOpcoes.Empresa);
        new BuscarLocalidades(knoutOpcoes.Origem, null);
        new BuscarLocalidades(knoutOpcoes.Destino, null);
    });
    
    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CargaMDFeManual/PesquisaParaCancelamento", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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