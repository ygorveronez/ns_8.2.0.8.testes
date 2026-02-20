/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="Localidade.js" />

var BuscarNaturezaNFSe = function (knout, knoutLocalidade, callbackRetorno, knoutPessoa, knoutDentroForaEstado, knoutTipoEntradaSaida) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), col: 12 });
        this.Localidade = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.NaturezaNFSe.Localidade.getFieldDescription(), idBtnSearch: guid() });
        this.Ativo = PropertyEntity({ val: ko.observable(1), def: 1, visible: false, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });

        this.Pessoa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Consultas.NaturezaNFSe.Pessoas.getFieldDescription(), idBtnSearch: guid(), visible: false });
        this.DentroForaEstado = PropertyEntity({ col: 12, text: Localization.Resources.Consultas.NaturezaNFSe.DentroFora.getFieldDescription(), visible: false, val: ko.observable(""), def: ko.observable("") });
        this.Tipo = PropertyEntity({ col: 12, text: Localization.Resources.Gerais.Geral.Tipo, visible: false, val: ko.observable(""), def: ko.observable("") });

        this.Titulo = PropertyEntity({ text: Localization.Resources.Consultas.NaturezaNFSe.BuscarNaturezaDosServioos, type: types.local });
        this.TituloGrid = PropertyEntity({ text: Localization.Resources.Consultas.NaturezaNFSe.NaturezaDosServicos, type: types.local });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutLocalidade != null || knoutPessoa != null || knoutDentroForaEstado != null || knoutTipoEntradaSaida != null) {
        knoutOpcoes.Localidade.visible = false;
        funcaoParamentroDinamico = function () {
            if (knoutLocalidade != null) {
                knoutOpcoes.Localidade.codEntity(knoutLocalidade.codEntity());
                knoutOpcoes.Localidade.val(knoutLocalidade.val());
            }
            if (knoutPessoa != null) {
                knoutOpcoes.Pessoa.codEntity(knoutPessoa.codEntity());
                knoutOpcoes.Pessoa.val(knoutPessoa.val());
            }
            if (knoutDentroForaEstado != null) {
                knoutOpcoes.DentroForaEstado.val(knoutDentroForaEstado)
            }
            if (knoutTipoEntradaSaida != null) {
                knoutOpcoes.Tipo.val(knoutTipoEntradaSaida);
            }
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico, function () {
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


    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "NaturezaNFSe/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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