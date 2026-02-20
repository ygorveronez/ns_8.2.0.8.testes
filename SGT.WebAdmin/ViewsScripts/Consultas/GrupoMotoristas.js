/// <reference path="../../wwwroot/js/Global/Buscas.js" />
/// <reference path="../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../wwwroot/js/libs/jquery-2.1.1.js" />

var BuscarGrupoMotoristas = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;

    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Grupos de Motoristas", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Grupos de Motoristas", type: types.local });

        this.Descricao = PropertyEntity({ text: "Descrição:", maxlength: 100, col: 5 });
        this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração", maxlength: 100, col: 5 });
        this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoGrupoMotoristas.Finalizado), options: EnumSituacaoGrupoMotoristas.obterOpcoes(), def: EnumSituacaoGrupoMotoristas.Finalizado, text: "Situação", visible: ko.observable(true), col: 2 });
        this.Ativo = PropertyEntity({ val: ko.observable(true), def: true, visible: false });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    if (multiplaEscolha)
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "GrupoMotoristas/Pesquisar", knoutOpcoes, null, null, null, null, null, null, { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar });
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "GrupoMotoristas/Pesquisar", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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

    this.Destroy = function () {
        if (buscaCliente)
            buscaCliente.Destroy();

        divBusca.Destroy();
    };
}