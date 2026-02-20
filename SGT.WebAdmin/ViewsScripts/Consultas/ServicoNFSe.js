/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Localidade.js" />

var BuscarServicoNFSe = function (knout, knoutLocalidade, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao, col: 12 });
        this.Localidade = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.ServicoNFSe.Localidade, idBtnSearch: guid() });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: Localization.Resources.Gerais.Geral.Situacao });
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.ServicoNFSe.BuscarServicos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.ServicoNFSe.Servicos, type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var funcaoParametroDinamico = null;
    if (knoutLocalidade != null) {
        knoutOpcoes.Localidade.visible = false;
        funcaoParametroDinamico = function () {
            knoutOpcoes.Localidade.codEntity(knoutLocalidade.codEntity());
            knoutOpcoes.Localidade.val(knoutLocalidade.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParametroDinamico, null, false, function () {
       new BuscarLocalidades(knoutOpcoes.Localidade);
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


    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ServicoNFSe/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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