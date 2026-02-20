/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridPagamento;
var _lancamentosContabeis;
var _CRUDPagamento;
var _pesquisaLancamentosContabeis;

var LancamentosContabeis = function () {
    this.Lancamentos = PropertyEntity({ val: ko.observableArray([]), def: [] });

    this.Chave = PropertyEntity({ text: "Chave NF-e: " });
    this.Numero = PropertyEntity({ text: "Número: " });
    this.Serie = PropertyEntity({ text: "Série: " });
    this.DataEmissao = PropertyEntity({ text: "Data Emissão: " });
    this.Emitente = PropertyEntity({ text: "Emitente: " });
    this.Destinatario = PropertyEntity({ text: "Destinatário: " });
};

var CRUDLancamentosContabeis = function () {
    this.Imprimir = PropertyEntity({ eventClick: imprimirLancamentosContabeisClick, type: types.event, text: "Exportar", idGrid: guid(), visible: ko.observable(false) });
};

var PesquisaLancamentosContabeis = function () {
    this.Nota = PropertyEntity({ text: "Nota Fiscal:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.CTe = PropertyEntity({ text: "CT-e Anterior:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
   
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            BuscarLancamentosContabeis();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
};



//*******EVENTOS*******
function loadLancamentosContabeis() {
    _lancamentosContabeis = new LancamentosContabeis();
    KoBindings(_lancamentosContabeis, "knockoutLancamentosContabeis");

    _CRUDLancamentosContabeis = new CRUDLancamentosContabeis();
    KoBindings(_CRUDLancamentosContabeis, "knockoutCRUD");

    _pesquisaLancamentosContabeis = new PesquisaLancamentosContabeis();
    KoBindings(_pesquisaLancamentosContabeis, "knockoutPesquisaDocumentos", false, _pesquisaLancamentosContabeis.Pesquisar.id);
    
    // Inicia as buscas
    new BuscarXMLNotaFiscal(_pesquisaLancamentosContabeis.Nota);
    new BuscarCTes(_pesquisaLancamentosContabeis.CTe);
}


function imprimirLancamentosContabeisClick(e, sender) {
    var dados = RetornarObjetoPesquisa(_pesquisaLancamentosContabeis);

    executarDownload("LancamentosContabeis/ExportarLancamentos", dados);
}


//*******MÉTODOS*******
function BuscarLancamentosContabeis() {
    var dados = RetornarObjetoPesquisa(_pesquisaLancamentosContabeis);

    executarReST("LancamentosContabeis/ObterLancamentos", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $(".container-lancamentos").removeClass("hide");
                PreencherObjetoKnout(_lancamentosContabeis, arg);
                _CRUDLancamentosContabeis.Imprimir.visible(true);
                LoadGrids();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function LoadGrids() {
    var header = [
        { data: "TipoContaContabilDescricao", title: "Tipo Conta Contábil", width: "10%" },
        { data: "ContaContabil", title: "Conta Contábil", width: "20%" },
        { data: "CentroCusto", title: "Centro Custo", width: "10%" },
        { data: "TipoContabilizacaoDescricao", title: "Tipo Contabilização", width: "10%" },
        { data: "ValorDescricao", title: "Valor", width: "10%", className: "text-right" },
        { data: "DataLancamento", title: "Data Lançamento", width: "10%", className: "text-center" },
    ];

    var lancamentos = _lancamentosContabeis.Lancamentos.val();

        for(var i = 0; i < lancamentos.length; i++) {
            var grid = new BasicDataTable("data-grid-" + i, header, null, { column: 5, dir: orderDir.asc }, null, null, false, false);
        grid.CarregarGrid(lancamentos[i].Lancamentos);
    }
}
