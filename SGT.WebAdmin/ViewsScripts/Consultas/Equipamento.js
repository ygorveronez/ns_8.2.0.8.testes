/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Veiculo.js" />

var BuscarEquipamentos = function (knout, callbackRetorno, menuOpcoes, knoutVeiculo, basicGrid) {
    
    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = basicGrid != null;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.Equipamento.BuscarEquipamentos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.Equipamento.Equipamentos, type: types.local });

        this.Codigo = PropertyEntity({ col: 2, text: Localization.Resources.Gerais.Geral.Codigo.getFieldDescription() });
        this.Descricao = PropertyEntity({ col: 6, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
        this.Numero = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.Equipamento.Numero.getFieldDescription(), visible: !IsMobile() });
        this.Chassi = PropertyEntity({ col: 2, text: Localization.Resources.Consultas.Equipamento.Chassi.getFieldDescription(), visible: !IsMobile() });
        this.Ativo = PropertyEntity({ val: ko.observable(1), options: _statusPesquisa, def: 1, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), visible: false });

        this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Consultas.Equipamento.Veiculo.getFieldDescription(), idBtnSearch: guid() });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    };

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutVeiculo != null) {
        knoutOpcoes.Veiculo.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Veiculo.codEntity(knoutVeiculo.codEntity());
            knoutOpcoes.Veiculo.val(knoutVeiculo.val());
        };
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, null, multiplaEscolha, function () {
        new BuscarVeiculos(knoutOpcoes.Veiculo);
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

    var opcoes = divBusca.OpcaoPadrao(callback, 22);
    if (menuOpcoes != null) {
        opcoes = menuOpcoes;
    }

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Equipamento/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
       
    } else {

        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "Equipamento/Pesquisa", knoutOpcoes, opcoes, null);
       
        

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

    this.CarregarReboqueVeiculoSelecionada = function () {
        if (knoutVeiculo != null) {
            LimparCampos(knoutOpcoes);
            funcaoParamentroDinamico();

            GridConsulta.CarregarGrid(function (lista) {
                if (lista.data.length == 1)
                    callback(lista.data[0]);
                else
                    LimparCampo(knout);
            });
        }
    };
};