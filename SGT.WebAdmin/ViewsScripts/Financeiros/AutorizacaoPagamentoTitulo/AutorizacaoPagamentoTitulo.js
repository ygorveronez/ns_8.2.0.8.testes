/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAutorizacao.js" />
/// <reference path="../../Enumeradores/EnumTipoTituloNegociacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoBoletoTitulo.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoPesquisaTitulo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridAutorizacaoPagamentoTitulo;
var _autorizacaoPagamentoTitulo;

var PesquisaAutorizacaoPagamentoTitulo = function () {
    this.DataEmissaoInicial = PropertyEntity({ text: "Emissão Inicial: ", getType: typesKnockout.date, required: false, enable: ko.observable(true), val: ko.observable("") });
    this.DataEmissaoFinal = PropertyEntity({ text: "Emissão Final: ", getType: typesKnockout.date, required: false, enable: ko.observable(true), val: ko.observable("") });
    this.DataVencimentoInicial = PropertyEntity({ text: "Vencimento Inicial: ", getType: typesKnockout.date, required: false, enable: ko.observable(true), val: ko.observable("") });
    this.DataVencimentoFinal = PropertyEntity({ text: "Vencimento Final: ", getType: typesKnockout.date, required: false, enable: ko.observable(true), val: ko.observable("") });
    this.NumeroTitulo = PropertyEntity({ text: "Nº Título: ", getType: typesKnockout.int, required: false, maxlength: 10 });
    this.NumeroDocumento = PropertyEntity({ text: "Nº Documento: ", required: false, maxlength: 150 });

    this.SituacaoAutorizacao = PropertyEntity({ text: "Situação da Autorização: ", val: ko.observable(EnumSituacaoAutorizacao.Todos), options: EnumSituacaoAutorizacao.obterOpcoesPesquisa(), def: EnumSituacaoAutorizacao.Todos });
    this.TipoTituloNegociacao = PropertyEntity({ text: "Situação do Título: ", val: ko.observable(EnumTipoTituloNegociacao.Todos), options: EnumTipoTituloNegociacao.obterOpcoesPesquisa(), def: EnumTipoTituloNegociacao.Todos });
    this.SituacaoBoletoTitulo = PropertyEntity({ text: "Situação do Boleto: ", val: ko.observable(EnumSituacaoBoletoTitulo.Todos), options: EnumSituacaoBoletoTitulo.obterOpcoesPesquisa(), def: EnumSituacaoBoletoTitulo.Todos });
    this.TipoDocumento = PropertyEntity({ text: "Tipo de Documento:", getType: typesKnockout.selectMultiple, val: ko.observable(new Array()), def: new Array(), options: EnumTipoDocumentoPesquisaTitulo.obterOpcoes() });

    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor: ", idBtnSearch: guid() });
    this.TipoMovimento = PropertyEntity({ text: "Tipo de Movimento:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ text: "Centro de Resultado:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.GrupoPessoas = PropertyEntity({ text: "Grupo de Pessoas:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.Titulos = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid(), idTab: guid(), enable: ko.observable(false) });
    this.ListaTitulos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.QuantidadeTitulosSelecionados = PropertyEntity({ text: "Qtd. selecionado: ", def: "0", val: ko.observable("0") });
    this.ValorOriginalTitulosSelecionados = PropertyEntity({ text: "Valor Total selecionado: R$ ", def: "0,00", val: ko.observable("0,00") });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAutorizacaoPagamentoTitulo.CarregarGrid();
            AtualizarTotalizadoresAutorizacao();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.DataAutorizacao = PropertyEntity({ text: "Data da Autorização: ", getType: typesKnockout.date, required: false, enable: ko.observable(true), val: ko.observable(""), def: ko.observable("") });
    this.InformarData = PropertyEntity({ eventClick: InformarDataClick, type: types.event, text: "Atualizar Data Autorização", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadAutorizacaoPagamentoTitulo() {
    _autorizacaoPagamentoTitulo = new PesquisaAutorizacaoPagamentoTitulo();
    KoBindings(_autorizacaoPagamentoTitulo, "knockoutPesquisaAutorizacaoPagamentoTitulo", false, _autorizacaoPagamentoTitulo.Pesquisar.id);

    new BuscarClientes(_autorizacaoPagamentoTitulo.Fornecedor);
    new BuscarTipoMovimento(_autorizacaoPagamentoTitulo.TipoMovimento);
    new BuscarCentroResultado(_autorizacaoPagamentoTitulo.CentroResultado);
    new BuscarGruposPessoas(_autorizacaoPagamentoTitulo.GrupoPessoas);
    CriarGridAutorizacaoPagamentoTitulo();
}

function CriarGridAutorizacaoPagamentoTitulo() {
    var somenteLeitura = false;

    _autorizacaoPagamentoTitulo.SelecionarTodos.visible(true);
    _autorizacaoPagamentoTitulo.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _autorizacaoPagamentoTitulo.SelecionarTodos,
        somenteLeitura: somenteLeitura,
        callbackSelecionado: function () {
            AtualizarTotalizadoresAutorizacao();
        },
        callbackNaoSelecionado: function () {
            AtualizarTotalizadoresAutorizacao();
        },
    };

    var configuracoesExportacao = { url: "AutorizacaoPagamentoTitulo/ExportarPesquisa", titulo: "Configurações de Alerta" };

    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: true };
    _gridAutorizacaoPagamentoTitulo = new GridViewExportacao(_autorizacaoPagamentoTitulo.Titulos.idGrid, "AutorizacaoPagamentoTitulo/Pesquisa", _autorizacaoPagamentoTitulo, null, configuracoesExportacao, null, 15, multiplaescolha, 1000, editarColuna);

    _gridAutorizacaoPagamentoTitulo.CarregarGrid();
}

function buscarTitulos() {
    _autorizacaoPagamentoTitulo.SelecionarTodos.visible(true);
    _autorizacaoPagamentoTitulo.SelecionarTodos.val(false);

    _gridAutorizacaoPagamentoTitulo.CarregarGrid();
    _gridAutorizacaoPagamentoTitulo.AtualizarRegistrosSelecionados([]);
}

//*******MÉTODOS*******

function InformarDataClick(e) {
    var msgAviso = "Realmente deseja salvar a data de autorização nos títulos selecionados?";
    if (_autorizacaoPagamentoTitulo.DataAutorizacao.val() === null || _autorizacaoPagamentoTitulo.DataAutorizacao.val() === "" || _autorizacaoPagamentoTitulo.DataAutorizacao.val() === "  /  /    ")
        msgAviso = "Realmente deseja REMOVER a data de autorização nos títulos selecionados?";

    exibirConfirmacao("Confirmação", msgAviso, function () {
        var titulosSelecionados = null;
        titulosSelecionados = _gridAutorizacaoPagamentoTitulo.ObterMultiplosSelecionados();

        var codigosTitulos = new Array();
        for (var i = 0; i < titulosSelecionados.length; i++)
            codigosTitulos.push(titulosSelecionados[i].DT_RowId);

        _autorizacaoPagamentoTitulo.ListaTitulos.val(JSON.stringify(codigosTitulos));

        executarReST("AutorizacaoPagamentoTitulo/AtualizarAutorizacao", RetornarObjetoPesquisa(_autorizacaoPagamentoTitulo), function (r) {
            if (r.Success) {
                if (r.Data) {
                    LimparCamposAutorizacao();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Títulos salvos com sucesso");
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function callbackEditarColuna(dataRow, row, head, callbackTabPress) {
    var dataEnvio = {
        Codigo: dataRow.Codigo,
        Valor: dataRow.Observacao,
    };

    executarReST("AutorizacaoPagamentoTitulo/AlterarObservacao", dataEnvio, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, arg.Data);
                _gridAutorizacaoPagamentoTitulo.AtualizarDataRow(row, dataRow, callbackTabPress);
            } else {
                ExibirErroDataRow(row, arg.Msg, tipoMensagem.aviso, "Aviso");
            }
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, "Falha");
        }
    });
}

function AtualizarTotalizadoresAutorizacao() {
    var titulosSelecionados = null;

    if (_autorizacaoPagamentoTitulo.SelecionarTodos.val())
        titulosSelecionados = _gridAutorizacaoPagamentoTitulo.ObterMultiplosNaoSelecionados();
    else
        titulosSelecionados = _gridAutorizacaoPagamentoTitulo.ObterMultiplosSelecionados();

    var codigosTitulos = new Array();
    for (var i = 0; i < titulosSelecionados.length; i++)
        codigosTitulos.push(titulosSelecionados[i].DT_RowId);

    if (codigosTitulos && (codigosTitulos.length > 0 || _autorizacaoPagamentoTitulo.SelecionarTodos.val()))
        _autorizacaoPagamentoTitulo.ListaTitulos.val(JSON.stringify(codigosTitulos));
    else
        _autorizacaoPagamentoTitulo.ListaTitulos.val("");

    if (!string.IsNullOrWhiteSpace(_autorizacaoPagamentoTitulo.ListaTitulos.val())) {
        executarReST("AutorizacaoPagamentoTitulo/BuscarDocumentosSelecionados", RetornarObjetoPesquisa(_autorizacaoPagamentoTitulo), function (r) {
            if (r.Success) {
                if (r.Data) {
                    var data = r.Data;
                    _autorizacaoPagamentoTitulo.QuantidadeTitulosSelecionados.val(data.QuantidadeTitulosSelecionados);
                    _autorizacaoPagamentoTitulo.ValorOriginalTitulosSelecionados.val(data.ValorOriginalTitulosSelecionados);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    } else {
        _autorizacaoPagamentoTitulo.QuantidadeTitulosSelecionados.val("0");
        _autorizacaoPagamentoTitulo.ValorOriginalTitulosSelecionados.val("0,00");
    }
}

function LimparCamposAutorizacao() {
    LimparCampos(_autorizacaoPagamentoTitulo);
    _autorizacaoPagamentoTitulo.DataAutorizacao.val("");
    buscarTitulos();
}
