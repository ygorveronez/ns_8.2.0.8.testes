/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarServicoTMS = function (knout, callbackRetorno, knoutPessoa, knoutTipoEmissao, knoutNaturezaOperacao) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Serviços", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Serviços", type: types.local });
        this.Descricao = PropertyEntity({ col: 8, text: "Descrição: ", maxlength: 250 });
        this.CodigoProdutoEmbarcador = PropertyEntity({ col: 4, maxlength: 50, text: "Código: " });
        this.Status = PropertyEntity({ col: 12, text: "Status: ", visible: false, val: ko.observable(true), def: ko.observable(true) });
        this.Pessoa = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Pessoas:", idBtnSearch: guid(), visible: false });
        this.TipoEmissao = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Tipo Emissão:", idBtnSearch: guid(), visible: false });
        this.NaturezaOperacao = PropertyEntity({ col: 6, type: types.entity, codEntity: ko.observable(0), text: "Tipo Emissão:", idBtnSearch: guid(), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutPessoa != null && knoutTipoEmissao != null && knoutNaturezaOperacao != null) {
        knoutOpcoes.Pessoa.visible = false;
        knoutOpcoes.TipoEmissao.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Pessoa.codEntity(knoutPessoa.codEntity());
            knoutOpcoes.Pessoa.val(knoutPessoa.val());
            knoutOpcoes.TipoEmissao.codEntity(knoutTipoEmissao.val());
            knoutOpcoes.TipoEmissao.val(knoutTipoEmissao.val());
            knoutOpcoes.NaturezaOperacao.codEntity(knoutNaturezaOperacao.codEntity());
            knoutOpcoes.NaturezaOperacao.val(knoutNaturezaOperacao.val());
        }
    } else if (knoutPessoa != null) {
        knoutOpcoes.Pessoa.visible = false;
        knoutOpcoes.TipoEmissao.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Pessoa.codEntity(knoutPessoa.codEntity());
            knoutOpcoes.Pessoa.val(knoutPessoa.val());
        }
    } else if (knoutTipoEmissao != null) {
        knoutOpcoes.Pessoa.visible = false;
        knoutOpcoes.TipoEmissao.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.TipoEmissao.codEntity(knoutTipoEmissao.val());
            knoutOpcoes.TipoEmissao.val(knoutTipoEmissao.val());
        }
    } else if (knoutNaturezaOperacao != null) {
        knoutOpcoes.Pessoa.visible = false;
        knoutOpcoes.TipoEmissao.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.NaturezaOperacao.codEntity(knoutNaturezaOperacao.codEntity());
            knoutOpcoes.NaturezaOperacao.val(knoutNaturezaOperacao.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "ServicoNotaFiscal/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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
var BuscarNBS = function (knout, callbackRetorno) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Pesquisar NBS", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "NBS", type: types.local });
        this.Descricao = PropertyEntity({ col: 4, text: "NBS", maxlength: 8 });
        this.CodigoNBS = PropertyEntity({ col: 8, text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), maxlength: 200 });
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
            callbackRetorno(e);
            divBusca.CloseModal();
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "NotaFiscalServico/PesquisaNBS", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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