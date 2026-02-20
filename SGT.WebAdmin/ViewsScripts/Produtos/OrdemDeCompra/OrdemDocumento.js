/// <reference path="../../Consultas/OrdemDocumento.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="OrdemDeCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridOrdemDocumento;
var _ordemDocumento;
var _pesquisaHistoricoMovimentacoes;
var _gridHistoricoMovimentacoes;

var OrdemDocumento = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Documento = PropertyEntity({ type: types.event, text: "Ordem Documento", idBtnSearch: guid(), click: abrirModalOrdemDocumento });
}

var PesquisaHistoricoMovimentacoes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.Historico = PropertyEntity({ type: types.event, text: "Ordem Documento", idGrid: guid(), click: abrilModalHistoricoMovimentacoes });
    this.TotalValorHistorico = PropertyEntity({ text: "Total Valor:  ", val: ko.observable(0) });
    this.Tolerancia = PropertyEntity({ text: "Tolerancia:  ", val: ko.observable(0) });
}

var ModalOrdemDocumento = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid() });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor: ", idBtnSearch: guid() });
    this.NumeroOrdem = PropertyEntity({ text: 'Número Ordem:', getType: typesKnockout.string, required: false, val: ko.observable("") });
    this.InicioValidade = PropertyEntity({ text: "Início Validade:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.FimValidade = PropertyEntity({ text: "Fim Validade:", getType: typesKnockout.date, val: ko.observable(""), def: "" });

    this.Adicionar = PropertyEntity({ eventClick: adicionarDocumentoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadOrdemDocumento() {

    _ordemDocumento = new OrdemDocumento();
    KoBindings(_ordemDocumento, "knockoutOrdemDocumento");

    //Sera utilizado depois para inserir movimentação manualmente
    //_modalOrdemDocumento = new ModalOrdemDocumento();
    //KoBindings(_modalOrdemDocumento, "divModalOrdemDocumento");
    //new BuscarFilial(_modalOrdemDocumento.Filial);
    //new BuscarClientes(_modalOrdemDocumento.Fornecedor);

    _pesquisaHistoricoMovimentacoes = new PesquisaHistoricoMovimentacoes();
    KoBindings(_pesquisaHistoricoMovimentacoes, "knockouModalHistoricoMovimentacoesOrdemCompra");

    loadGridDocumentos()
    recarregarGridOrdemDocumento();
}

function recarregarGridOrdemDocumento() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_ordemDeCompra.OrdemItem.val()))
        data = _ordemDeCompra.OrdemItem.val();

    _gridOrdemDocumento.CarregarGrid(data);
}


function excluirOrdemDocumentoClick(knoutOrdemDocumento, data) {
    var ordemDocumentoGrid = knoutOrdemDocumento.basicTable.BuscarRegistros();

    for (var i = 0; i < ordemDocumentoGrid.length; i++) {
        if (data.Codigo == ordemDocumentoGrid[i].Codigo) {
            ordemDocumentoGrid.splice(i, 1);
            break;
        }
    }

    knoutOrdemDocumento.basicTable.CarregarGrid(ordemDocumentoGrid);
}

function limparCamposOrdemDocumento() {
    LimparCampos(_ordemDocumento);
    LimparCampos(_gridOrdemDocumento);
}
function abrirModalOrdemDocumento() {
    LimparCampos(_ordemDocumento);
    Global.abrirModal('divModalOrdemDocumento');
}

function adicionarDocumentoClick(e, sender) {

    let listaDocumentos = BuscarRegistroGridDocumento();

    let novoDocumento = {
        Codigo: 0,
        CodigoFilial: e.Filial.codEntity(),
        Filial: e.Filial.val(),
        CodigoFornecedor: e.Fornecedor.codEntity(),
        Fornecedor: e.Fornecedor.val(),
        NumeroOrdem: e.NumeroOrdem.val(),
        InicioValidade: e.InicioValidade.val(),
        FimValidade: e.FimValidade.val()
    }

    listaDocumentos.push(novoDocumento);
    _gridOrdemDocumento.CarregarGrid(listaDocumentos);

    Global.fecharModal("divModalOrdemDocumento");
    LimparCampos(_modalOrdemDocumento);
}

function BuscarRegistroGridDocumento() {
    return _gridOrdemDocumento.BuscarRegistros();
}

function loadGridDocumentos() {
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [] };
    //menuOpcoes.opcoes.push({ descricao: "Remover", id: guid(), metodo: removerItemDocumento, tamanho: "20", icone: "" });
    menuOpcoes.opcoes.push({ descricao: "Historico", id: guid(), metodo: abrilModalHistoricoMovimentacoes, tamanho: "20", icone: "" });
    //var configuracoesExportacao = { url: "OrdemDeCompra/ExportarPesquisa", titulo: "Ordem de Compra" };

    var header = [
        { data: "Codigo", visible: false },
        { data: "TotalItemsDisponiveis", visible: false },
        { data: "Tolerancia", visible: false },
        { data: "DataAlteracao", title: "Data Alteração", width: "20%" },
        { data: "NumeroItemDocumento", title: "Numero Item Documento", width: "20%" },
        { data: "CodigoProdutoEmbarcador", title: "Codigo Produto Embarcador", width: "20%" },
        { data: "EntregaConcluida", title: "Entrega Concluida", width: "10%" },
        { data: "ProdutoProduzidoInternamente", title: "Produto Produzido Internamente", width: "10%" },
        { data: "QuantidadeOrdemCompra", title: "Quantidade", width: "10%" },
        { data: "LimiteTolerancia", title: "Limite Tolerancia", width: "10%" },
    ];

    _gridOrdemDocumento = new BasicDataTable(_ordemDocumento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, null, null, null, null, null, null, null);
    _ordemDocumento.Documento.basicTable = _gridOrdemDocumento;
}

function PreecherListaDocumentoOrdems() {
    let lista = _ordemDocumento.Documento.basicTable.BuscarRegistros();
    let listaRetorno = [];

    for (var i = 0; i < lista.length; i++) {
        //let documento = lista[i];
        //listaRetorno.push({
        //    Codigo: documento.Codigo,
        //    : documento.CodigoFilial,
        //    : documento.CodigoFornecedor,
        //    : documento.InicioValidade,
        //    : documento.FimValidade,
        //    : documento.NumeroOrdem
        //});
    }
    _ordemDeCompra.OrdemItem.val(JSON.stringify(listaRetorno));
}


function removerItemDocumento(e) {
    let listaDocumentos = BuscarRegistroGridDocumento();

    for (var i = 0; i < listaDocumentos.length; i++) {
        let item = listaDocumentos[i];
        if (!(item.NumeroItemDocumento == e.NumeroItemDocumento))
            continue;
        listaDocumentos.splice(i, 1);
        break;
    }
    _ordemDocumento.Documento.basicTable.CarregarGrid(listaDocumentos);
}

function loadGridHistoricoMovimentacoesOrdemCompra(e) {
    _pesquisaHistoricoMovimentacoes.Codigo.val(e.Codigo);
    _gridHistoricoMovimentacoes = new GridView(_pesquisaHistoricoMovimentacoes.Historico.idGrid, "OrdemDeCompra/ConsultarHistoricoMovimentacoes", _pesquisaHistoricoMovimentacoes);
    _gridHistoricoMovimentacoes.CarregarGrid();
    _pesquisaHistoricoMovimentacoes.TotalValorHistorico.val(Globalize.parseFloat(e.TotalItemsDisponiveis));
    _pesquisaHistoricoMovimentacoes.Tolerancia.val(Globalize.parseInt(e.Tolerancia) + "%");
}

function abrilModalHistoricoMovimentacoes(e) {
    loadGridHistoricoMovimentacoesOrdemCompra(e);
    Global.abrirModal("divModalHistoricoMovimentacoesOrdemCompra");
}
