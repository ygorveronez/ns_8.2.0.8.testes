/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/MotivoAvaria.js" />
/// <reference path="../../Consultas/Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaAvarias;
var _gridAvaria;

var PesquisaAvarias = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Transportadora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportadora:", issue: 69, idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", issue: 70, idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiTMS) });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo:", issue: 943, idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid()});

    this.GerarLote = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: gerarLoteClick, text: "Gerar Lote", visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: ko.observable(), visible: ko.observable(true) });
    this.SelecionarTodos.val.subscribe(labelBtnMarcarTodas);

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarAvarias();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({ type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true) });
};

//*******EVENTOS*******


function loadLoteAvaria() {
    _pesquisaAvarias = new PesquisaAvarias();
    KoBindings(_pesquisaAvarias, "knockoutPesquisaAvarias");

    // Busca componentes pesquisa
    new BuscarTransportadores(_pesquisaAvarias.Transportadora);
    new BuscarFilial(_pesquisaAvarias.Filial);
    new BuscarMotivoAvaria(_pesquisaAvarias.Motivo, EnumFinalidadeMotivoAvaria.MotivoAvaria);
    new BuscarTiposOperacao(_pesquisaAvarias.TipoOperacao);

    labelBtnMarcarTodas();
    buscarAvarias();
}

function labelBtnMarcarTodas() {
    var txt = _pesquisaAvarias.SelecionarTodos.val() == true ? "Desmarcar Todas" : "Marcar Todas";
    _pesquisaAvarias.SelecionarTodos.text(txt);
}

//*******MÉTODOS*******

function buscarAvarias() {
    //-- Reseta
    _pesquisaAvarias.SelecionarTodos.val(false);
    _pesquisaAvarias.GerarLote.visible(false);

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaAvarias.SelecionarTodos,
        callbackNaoSelecionado: exibirMultiplasOpcoes,
        callbackSelecionado: exibirMultiplasOpcoes,
        callbackSelecionarTodos: exibirMultiplasOpcoes,
        somenteLeitura: false
    }

    var configExportacao = {
        url: "LoteAvarias/ExportarPesquisa",
        titulo: "Lote Avaria"
    };

    _gridAvaria = new GridViewExportacao(_pesquisaAvarias.Pesquisar.idGrid, "LoteAvarias/Pesquisa", _pesquisaAvarias, null, configExportacao, null, 20, multiplaescolha);

    _gridAvaria.CarregarGrid();
}

function exibirMultiplasOpcoes(e) {
    var possuiSelecionado = _gridAvaria.ObterMultiplosSelecionados().length > 0;
    var selecionadoTodos = _pesquisaAvarias.SelecionarTodos.val();

    if (possuiSelecionado || selecionadoTodos) {
        _pesquisaAvarias.GerarLote.visible(true);
    } else {
        _pesquisaAvarias.GerarLote.visible(false);
    }
}

function gerarLoteClick() {
    exibirConfirmacao("Confirmação", "Você realmente deseja gerar o lote com as avarias selecionadas?", function () {
        var dados = RetornarObjetoPesquisa(_pesquisaAvarias);

        dados.SelecionarTodos = _pesquisaAvarias.SelecionarTodos.val();
        dados.AvariasSelecionadas = JSON.stringify(_gridAvaria.ObterMultiplosSelecionados());
        dados.AvariasNaoSelecionadas = JSON.stringify(_gridAvaria.ObterMultiplosNaoSelecionados());

        executarReST("LoteAvarias/GerarLote", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (arg.Data.LotesGerados > 0)
                        exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Data.LotesGerados + " lote(s) gerado(s) com sucesso.");

                    // Verifica se alguma lote deu erro
                    var lotesFalhados = arg.Data.TotalLotes - arg.Data.LotesGerados;
                    if (lotesFalhados > 0)
                        exibirMensagem(tipoMensagem.atencao, "Falha em gerar lote", lotesFalhados + " lote(s) falharam.");
                    buscarAvarias();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    });
}