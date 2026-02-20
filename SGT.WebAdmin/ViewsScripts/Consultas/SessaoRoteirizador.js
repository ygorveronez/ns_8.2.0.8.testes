/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../Enumeradores/EnumSituacaoSessaoRoteirizador.js" />

var BuscarSessaoRoteirizador = function (knout, callbackRetorno, basicGrid) {

    var idDiv = guid();
    var GridConsulta;
    var multiplaEscolha = (basicGrid != null);

    var OpcoesKnout = function () {

        this.NumeroSessao = PropertyEntity({ col: 2, getType: typesKnockout.int, text: Localization.Resources.Consultas.SessaoRoteirizador.Sessao.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
        this.CodigoFilial = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), visible: true, text: Localization.Resources.Consultas.SessaoRoteirizador.Filial.getFieldDescription(), idBtnSearch: guid() });
        this.CodigoUsuario = PropertyEntity({ col: 4, type: types.entity, codEntity: ko.observable(0), visible: true, text: Localization.Resources.Consultas.SessaoRoteirizador.Usuario.getFieldDescription(), idBtnSearch: guid() });
        this.Situacao = PropertyEntity({ col: 2, val: ko.observable(EnumSituacaoSessaoRoteirizador.Iniciada), options: EnumSituacaoSessaoRoteirizador.obterOpcoesPesquisa(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), def: EnumSituacaoSessaoRoteirizador.Iniciada });
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.SessaoRoteirizador.BuscarSessaoRoteirizador, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.SessaoRoteirizador.SessoesRoteirizador, type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        new BuscarFilial(knoutOpcoes.CodigoFilial);
        new BuscarFuncionario(knoutOpcoes.CodigoUsuario);
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

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };

        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "SessaoRoteirizador/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "SessaoRoteirizador/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada)
            knoutOpcoes.Situacao.val(knout.val());
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1)
                callback(lista.data[0]);
            else
                divBusca.OpenModal();
        });
    })
}