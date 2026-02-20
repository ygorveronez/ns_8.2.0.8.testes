/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarSetorFuncionario = function (knout, callbackRetorno, knoutFilial, basicGrid, irregularidade) {

    var idDiv = guid();
    var GridConsulta;

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.SetorFuncionario.PesquisarSetores, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.SetorFuncionario.Setor, type: types.local });
        this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), col: 12 });
        this.Status = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(1), text: Localization.Resources.Gerais.Geral.Ativo.getFieldDescription(), visible: false });
        this.Filial = PropertyEntity({ col: 0, type: types.entity, codEntity: ko.observable(0), visible: false });
        this.CodigoIrregularidade = PropertyEntity({ col: 0, getType: typesKnockout.int, val: ko.observable(0), visible: false });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true });
    }

    var arrayOpcoes = [];
    var funcaoParamentroDinamico = null;
    var knoutOpcoes = new OpcoesKnout();

    if (knoutFilial) {
        arrayOpcoes.push({ knout: knoutFilial, propriedade: "Filial" });
    }

    if (arrayOpcoes.length > 0) {
        funcaoParamentroDinamico = function () {
            arrayOpcoes.forEach(function (opcao) {
                knoutOpcoes[opcao.propriedade].codEntity(opcao.knout.codEntity());
                knoutOpcoes[opcao.propriedade].val(opcao.knout.val());
            });
        }
    }

    if (irregularidade) {
        knoutOpcoes.CodigoIrregularidade.val(irregularidade);
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
    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "SetorFuncionario/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    }
    else
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "SetorFuncionario/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);

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
}