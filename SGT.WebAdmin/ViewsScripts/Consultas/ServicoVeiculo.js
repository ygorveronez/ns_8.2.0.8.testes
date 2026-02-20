/// <reference path="GrupoServico.js" />

var BuscarServicoVeiculo = function (knout, callbackRetorno, basicGrid, knoutGrupoServico) {

    var idDiv = guid();
    var gridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Pesquisa de Serviços", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Serviços", type: types.local });

        this.Descricao = PropertyEntity({ col: 12, text: "Descrição:" });
        this.Ativo = PropertyEntity({ val: ko.observable(1), options: _statusPesquisa, def: 1, visible: false });
        this.GrupoServico = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Grupo de Serviço:", idBtnSearch: guid(), visible: ko.observable(true) });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                gridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParametroDinamico = null;

    if (knoutGrupoServico != null) {
        funcaoParametroDinamico = function () {
            if (knoutGrupoServico != null) {
                knoutOpcoes.GrupoServico.codEntity(knoutGrupoServico.codEntity());
                knoutOpcoes.GrupoServico.val(knoutGrupoServico.val());
            }
        };
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        knoutOpcoes.GrupoServico.visible(false);
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, multiplaEscolha, function () {
        new BuscarGrupoServico(knoutOpcoes.GrupoServico);
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

    var url = "ServicoVeiculo/Pesquisa";

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        gridConsulta = new GridView(idDiv + "_tabelaEntidades", url, knoutOpcoes, divBusca.OpcaoPadrao(callback), { column: 1, dir: orderDir.asc });
    }

    divBusca.AddEvents(gridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Descricao.val(knout.val());

        gridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length === 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
};