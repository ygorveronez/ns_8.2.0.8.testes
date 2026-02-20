/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoAcompanhamento.js" />
/// <reference path="../../../ViewsScripts/CTe/CTe/CTe.js" />

var _pesquisaMovimentacaoContaPagar;
var _gridMovimentacaoContaPaga;
var _modalVinculacaoManualDocumento;
var _modalVinculacaoReprocessamentoRegistro;

var PesquisaMovimentacaoContaPagar = function () {
    this.DataDocInicial = PropertyEntity({ text: "Data Doc Inicial:", enable: false, getType: typesKnockout.date, val: ko.observable(""), enable: false });
    this.DataDocFinal = PropertyEntity({ text: "Data Doc Final:", enable: false, getType: typesKnockout.date, val: ko.observable(""), enable: false });
    this.NumeroTermoQuitacao = PropertyEntity({ text: "Nº Termo Quitação:", getType: typesKnockout.int, val: ko.observable(0) });
    this.DataCompensacaoInicial = PropertyEntity({ text: "Data Compensação Inicial:", enable: false, getType: typesKnockout.date, val: ko.observable(""), enable: false });
    this.DataCompensacaoFinal = PropertyEntity({ text: "Data Compensação Final:", enable: false, getType: typesKnockout.date, val: ko.observable(""), enable: false });
    this.Transportador = PropertyEntity({ text: "Transportador: ", enable: false, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.TipoArquivo = PropertyEntity({ text: "Tipo Arquivo: ", val: ko.observable(EnumTipoDocumentoAcompanhamento.SemTipo), getType: typesKnockout.select, options: EnumTipoDocumentoAcompanhamento.obterOpcoes(), def: EnumTipoDocumentoAcompanhamento.SemTipo });
    this.SituacaoProcessamento = PropertyEntity({ text: "Situação Processamento: ", val: ko.observable(EnumSituacaoProcessamento.Todos), getType: typesKnockout.select, options: EnumSituacaoProcessamento.obterOpcoes(), def: EnumSituacaoProcessamento.Todos });
    this.SituacaoDocumento = PropertyEntity({ text: "Situação Documento: ", val: ko.observable(EnumSituacaoDocumento.Todos), getType: typesKnockout.select, options: EnumSituacaoDocumento.obterOpcoes(), def: EnumSituacaoDocumento.Todos });
    this.TodasFiliaisTransportador = PropertyEntity({ text: "Todas as Filiais Transportador: ", val: ko.observable(false), getType: typesKnockout.bool, def: false });
    this.ReprocessarMovimentosContaPagar = PropertyEntity({ eventClick: reprocessarDocumentosSemMiro, type: types.event, text: "Reprocessamento Conta Pagar", idGrid: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMovimentacaoContaPaga.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: "grid-movimentacao-contas-pagar", visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: verificarManualmenteOsRegistros, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(false) });

}

var ModalVinculacaoManualDocumento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) })
    this.CTe = PropertyEntity({ text: "CT-e: ", enable: false, type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarCteaoDocumento, type: types.event, text: "Adicionar", idGrid: guid(), visible: ko.observable(true) });
}

var ModalVinculacaoReprocessamentoRegistro = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", enable: false, getType: typesKnockout.date, val: ko.observable(""), enable: false });
    this.DataFinal = PropertyEntity({ text: "Data Final:", enable: false, getType: typesKnockout.date, val: ko.observable(""), enable: false });

    this.Adicionar = PropertyEntity({ eventClick: reprocessarDocumentos, type: types.event, text: "Reprocessar ", idGrid: guid(), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: () => { Global.fecharModal("divModalReprocessarVinculo") }, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(true) });
}

function loadMovimentacaoContaPagar() {
    _pesquisaMovimentacaoContaPagar = new PesquisaMovimentacaoContaPagar();
    KoBindings(_pesquisaMovimentacaoContaPagar, "knockoutPesquisaMovimentacaoContaPagar");

    _modalVinculacaoManualDocumento = new ModalVinculacaoManualDocumento();
    KoBindings(_modalVinculacaoManualDocumento, "knockoutVincularDocumento");

    _modalVinculacaoReprocessamentoRegistro = new ModalVinculacaoReprocessamentoRegistro();
    KoBindings(_modalVinculacaoReprocessamentoRegistro, "knockoutReprocessarVinculo");

    new BuscarCTes(_modalVinculacaoManualDocumento.CTe);
    new BuscarEmpresa(_pesquisaMovimentacaoContaPagar.Transportador);
    loadGridMovimentacaoContaPagar();
}

function loadGridMovimentacaoContaPagar() {
    //var multiplaescolha = {
    //    basicGrid: null,
    //    eventos: function () { },
    //    selecionados: new Array(),
    //    naoSelecionados: new Array(),
    //    SelecionarTodosKnout: _pesquisaMovimentacaoContaPagar.SelecionarTodos,
    //    callbackNaoSelecionado: () => { },
    //    callbackSelecionado: () => { },
    //    callbackSelecionarTodos: () => { },
    //    somenteLeitura: false
    //};

    var configExportacao = {
        url: "MovimentacaoContaPagar/ExportarPesquisa",
        titulo: "Movimentação Conta a Pagar"
    };
    var vincularDocumento = { descricao: "Vincular Manualmente", id: guid(), evento: "onclick", metodo: abrirModalVinculacaoCte, tamanho: "8", icone: "" };
    var detalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: detalhesCTeMovimentacaoClick, tamanho: "8", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [ detalhes], tamanho: 8, };
    _gridMovimentacaoContaPaga = new GridView(_pesquisaMovimentacaoContaPagar.Pesquisar.idGrid, "MovimentacaoContaPagar/Pesquisa", _pesquisaMovimentacaoContaPagar, menuOpcoes, null, 25, null, null, null, null, null, null, configExportacao);
    _gridMovimentacaoContaPaga.SetSalvarPreferenciasGrid(true);
    _gridMovimentacaoContaPaga.SetPermitirEdicaoColunas(true);
    _gridMovimentacaoContaPaga.CarregarGrid();
}

function reprocessarDocumentosSemMiro() {
    Global.abrirModal("divModalReprocessarVinculo");
}

function abrirModalVinculacaoCte(e) {
    _modalVinculacaoManualDocumento.Codigo.val(e.Codigo);
    Global.abrirModal("divModalVincularDocumento");
}

function reprocessarDocumentos(e) {
    var data = {
        DataInicio: _modalVinculacaoReprocessamentoRegistro.DataInicial.val(),
        DataFim: _modalVinculacaoReprocessamentoRegistro.DataFinal.val(),
    }
    executarReST("MovimentacaoContaPagar/ReprocessamentoContaPagar", data, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg)

        exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg)
        _gridMovimentacaoContaPaga.CarregarGrid();
        Global.fecharModal("divModalReprocessarVinculo")

    })
}

function adicionarCteaoDocumento(e) {

    var data = {
        Cte: e.CTe.codEntity(),
        Codigo: e.Codigo.val()
    }
    executarReST("MovimentacaoContaPagar/VincularCTeManual", data, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg)

        exibirMensagem(tipoMensagem.ok, "Sucesso", arg.Msg)
        _gridMovimentacaoContaPaga.CarregarGrid();
        Global.fecharModal("divModalVincularDocumento");

    })
}

function verificarManualmenteOsRegistros() {
 
    let selecionouTodos = _pesquisaMovimentacaoContaPagar.SelecionarTodos.val();
    let selecionados = selecionouTodos ? [] : _gridMovimentacaoContaPaga.ObterMultiplosSelecionados();
    let codigosSelecionados = selecionouTodos ? JSON.stringify([]) : JSON.stringify(selecionados.map(x => x.Codigo));

    if (!selecionouTodos && codigosSelecionados.length == 0)
        return exibirMensagem(tipoMensagem.aviso, "Atenção", "Por favor selecione as movimentações que deseja verificar manualmente");

    executarReST("MovimentacaoContaPagar/VerificarManualmente", { Codigos: codigosSelecionados, SelecionouTodos: selecionouTodos }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Registros Verificados Com sucesso");
        _gridMovimentacaoContaPaga.CarregarGrid();
    })
}


function detalhesCTeMovimentacaoClick(e, sender) {
    if (e.CodigoCTe == 0)
        return exibirMensagem(tipoMensagem.aviso, "Atenção", "Não existem documentos vinculados para este registro");


    var codigo = parseInt(e.CodigoCTe);
    var permissoesSomenteLeituraCTe = new Array();
    permissoesSomenteLeituraCTe.push(EnumPermissoesEdicaoCTe.Nenhuma);
    var instancia = new EmissaoCTe(codigo, function () {
        instancia.CRUDCTe.Emitir.visible(false);
        instancia.CRUDCTe.Salvar.visible(false);
        instancia.CRUDCTe.Salvar.eventClick = function () {
       /*     var objetoCTe = ObterObjetoCTe(instancia);*/
           /* SalvarCTe(objetoCTe, e.Codigo, instancia);*/
        }
    }, permissoesSomenteLeituraCTe);
}