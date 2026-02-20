/// <reference path="../../js/Global/Buscas.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/libs/jquery-2.1.1.js" />

var BuscarConhecimentosFatura = function (knout, callbackRetorno, knoutFatura) {

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Conhecimentos", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Conhecimentos", type: types.local });
        this.Descricao = PropertyEntity({ col: 12, text: "Número: ", maxlength: 250, getType: typesKnockout.int });
        this.Fatura = PropertyEntity({ col: 12, type: types.entity, codEntity: ko.observable(0), text: "Fatura:", idBtnSearch: guid(), visible: false });
        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });
    }

    var knoutOpcoes = new OpcoesKnout();
    var funcaoParamentroDinamico = null;

    if (knoutFatura != null) {
        knoutOpcoes.Fatura.visible = false;
        funcaoParamentroDinamico = function () {
            knoutOpcoes.Fatura.codEntity(knoutFatura.val());
            knoutOpcoes.Fatura.val(knoutFatura.val());
        }
    }

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, funcaoParamentroDinamico);

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "FaturaCarga/PesquisaConhecimentosFatura", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
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

var BuscarConhecimentoNotaReferencia = function (knout, callbackRetorno, basicGrid) {

    var opcoesSituacaoLiquidacaoCTe = [
        { text: "Todos", value: "" },
        { text: "Pendente", value: EnumTipoLiquidacaoRelatorioDocumentoFaturamento.Pendente },
        { text: "Liquidado", value: EnumTipoLiquidacaoRelatorioDocumentoFaturamento.Liquidado }
    ];

    var multiplaEscolha = false;
    if (basicGrid != null)
        multiplaEscolha = true;

    var idDiv = guid();
    var GridConsulta;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Conhecimentos de Frete", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Conhecimentos de Frete", type: types.local });

        this.NumeroInicial = PropertyEntity({ col: 3, text: "Número Inicial:", getType: typesKnockout.int });
        this.NumeroFinal = PropertyEntity({ col: 3, text: "Número Final:", getType: typesKnockout.int });
        this.ValorInicial = PropertyEntity({ col: 3, text: "Valor Inicial:", getType: typesKnockout.decimal });
        this.ValorFinal = PropertyEntity({ col: 3, text: "Valor Final:", getType: typesKnockout.decimal });
        this.Serie = PropertyEntity({ col: 3, text: "Série:" });
        this.Chave = PropertyEntity({ col: 9, text: "Chave:" });

        this.Pesquisar = PropertyEntity({
            eventClick: function (e) {
                GridConsulta.CarregarGrid();
            }, type: types.event, text: "Pesquisar", visible: true
        });

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

        this.ModeloDocumento = PropertyEntity({ col: 6, text: "Modelo de Documento:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
        this.Empresa = PropertyEntity({ col: 6, text: "Empresa/Filial:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
        this.GrupoPessoas = PropertyEntity({ col: 6, text: "Grupo de Pessoas:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
        this.SituacaoLiquidacao = PropertyEntity({ col: 6, text: "Situação Liquidação:", options: opcoesSituacaoLiquidacaoCTe, val: ko.observable(""), def: "" });
    }

    var knoutOpcoes = new OpcoesKnout();

    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, multiplaEscolha, function () {
        new BuscarModeloDocumentoFiscal(knoutOpcoes.ModeloDocumento, null, null, null, null, true);
        new BuscarTransportadores(knoutOpcoes.Empresa);
        new BuscarGruposPessoas(knoutOpcoes.GrupoPessoas);
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

    if (multiplaEscolha) {
        var objetoBasicGrid = { basicGrid: basicGrid, callback: callbackRetorno, eventos: knoutOpcoes.Pesquisar };
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CTe/Pesquisa", knoutOpcoes, null, null, null, null, null, null, objetoBasicGrid);
    } else {
        GridConsulta = new GridView(idDiv + "_tabelaEntidades", "CTe/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    }

    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        if (outraPressionada) {
            knoutOpcoes.NumeroInicial.val(knout.val());
            knoutOpcoes.NumeroFinal.val(knout.val());
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