/// <reference path="../../Enumeradores/StatusDocumentoTransporte.js" />
/// <reference path="../../Consultas/Cliente.js" />

// #region Variaveis

var _documentoParaTransporte;
var _documentoTransporte;
var _gridDadoParaTransporte;
var _crudDocumentoTransporte;
// #endregion

var DadosParaTransporte = function () {
    this.CodigoAgendamento = PropertyEntity({ val: ko.observable(0) });

    this.Grid = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid(), idTab: guid() });
    this.DocumentosParaTrasporte = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0) });

    this.Adicionar = PropertyEntity({ eventClick: AbrirModalDocumentoTransporte, type: types.event, text: "Adicionar Documento", visible: ko.observable(true) });
    this.Salvar = PropertyEntity({ eventClick: SalvarDocumentoParaTransporte, type: types.event, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Avancar = PropertyEntity({ eventClick: FinalizarAgendamento, type: types.event, text: "Avançar", visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default botaoDentroSmartAdmimForm",
        UrlImportacao: "AgendamentoColeta/ObterDadosDocumentoTransportePlanilha",
        UrlConfiguracao: "AgendamentoColeta/ConfiguracaoImportacaoDocumentoTransporte",
        CodigoControleImportacao: EnumCodigoControleImportacao.O054_ImportacaoDadosTransporte,
        CallbackImportacao: function (arg) {
            RecarregarGridDocumentoTransporte(arg.Data.Retorno);
        },
        FecharModalSeSucesso: true
    });
}

var DocumentoTransporte = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.NumeroNFE = PropertyEntity({ text: "Nº NFe", val: ko.observable("") });
    this.NumeroCTE = PropertyEntity({ text: "Nº CTe", val: ko.observable("") });
    this.ChaveAcessoCTE = PropertyEntity({ text: "Chave Acesso CTe", val: ko.observable("") });
    this.ChaveAcessoNFE = PropertyEntity({ text: "Chave Acesso NFe", val: ko.observable("") });
    this.Peso = PropertyEntity({ text: "Peso", type: typesKnockout.int, val: ko.observable(0) });
    this.Volumen = PropertyEntity({ text: "Volumen", type: typesKnockout.int, val: ko.observable(0) });
    this.Status = PropertyEntity({ text: "Status", options: EnumStatusDocumentoTransporte.obterOpcoes(), def: EnumStatusDocumentoTransporte.OK, val: ko.observable(EnumStatusDocumentoTransporte.OK) });
    this.Observacao = PropertyEntity({ text: "Observação", val: ko.observable("") });
}

var CRUDDocumentoTransporte = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarDocumentoTransporte, type: types.event, text: "Adicionar ", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarDocumentoTransporte, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarEdicaoDocumentoTransporte, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//#region Funções Constructoras

function LoadDocumentoParaTransporte() {
    _documentoParaTransporte = new DadosParaTransporte();
    KoBindings(_documentoParaTransporte, "knockoutDocumentoParaTransporte");

    _documentoTransporte = new DocumentoTransporte();
    KoBindings(_documentoTransporte, "knockoutDocumentoTransporte");

    _crudDocumentoTransporte = new CRUDDocumentoTransporte();
    KoBindings(_crudDocumentoTransporte, "knockoutCRUDDocumentoTransporte");

    new BuscarClientes(_documentoTransporte.Fornecedor);

    LoadGridDocumentoTransporte();
    RecarregarGridDocumentoTransporte();
}

function LoadGridDocumentoTransporte() {
    var excluir = { descricao: "Remover", id: guid(), evento: "onclick", metodo: ExcluirDocumentoDaGrid, tamanho: "15", icone: "" };
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarDocumentoDaGrid, tamanho: "15", icone: "" };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "DT_RowClass", visible: false },
        { data: "CodigoFornecedor", visible: false },
        { data: "NumeroNFE", title: "Nº NF", width: "10%" },
        { data: "NumeroCTE", title: "Nº CTE", width: "10%" },
        { data: "Fornecedor", title: "Fornecedor", width: "25%" },
        { data: "ChaveAcessoCTE", title: "Chave Acesso CTe", width: "20%" },
        { data: "ChaveAcessoNFE", title: "Chave Acesso NFe", width: "20%" },
        { data: "Peso", title: "Peso", width: "10%" },
        { data: "Volumen", title: "Volumen", width: "10%" },
        { data: "Status", title: "Status", width: "10%" },
        { data: "Observacao", title: "Observações", width: "25%" },
    ];

    _gridDadoParaTransporte = new BasicDataTable(_documentoParaTransporte.Grid.idGrid, header, menuOpcoes);
}



//#endregion

//#region Funçoes Grid Documento Para Transporte

function RecarregarGridDocumentoTransporte(list) {

    if (list != null && list.length > 0 )
        _documentoParaTransporte.DocumentosParaTrasporte.list = [..._documentoParaTransporte.DocumentosParaTrasporte.list,...list];

    const listaData = new Array();
    $.each(_documentoParaTransporte.DocumentosParaTrasporte.list, (i, item) => {
        const data = new Object();
        data.Codigo = item.Codigo;
        data.NumeroNFE = item.NumeroNFE;
        data.NumeroCTE = item.NumeroCTE;
        data.Fornecedor = item.Fornecedor;
        data.CodigoFornecedor = item.CodigoFornecedor;
        data.ChaveAcessoCTE = item.ChaveAcessoCTE;
        data.ChaveAcessoNFE = item.ChaveAcessoNFE;
        data.Peso = item.Peso;
        data.Volumen = item.Volumen;
        data.Status = item.Status;
        data.Observacao = item.Observacao;
        data.DT_RowClass = !item.DT_RowClass ? "" : item.DT_RowClass;
        listaData.push(data);
    })
    _gridDadoParaTransporte.CarregarGrid(listaData);
}

function ExcluirDocumentoDaGrid(documento) {
    const tamanoList = _documentoParaTransporte.DocumentosParaTrasporte.list.length;

    for (var i = 0; i < tamanoList; i++) {
        const itemDocumento = _documentoParaTransporte.DocumentosParaTrasporte.list[i];

        if (itemDocumento.ChaveAcessoCTE == documento.ChaveAcessoCTE && itemDocumento.ChaveAcessoNFE == documento.ChaveAcessoNFE) {
            _documentoParaTransporte.DocumentosParaTrasporte.list.splice(i, 1);
            break;
        }
    }
    RecarregarGridDocumentoTransporte();
}

function EditarDocumentoDaGrid(documento) {
    LimparCamposDocumentoTransporte();

    _documentoTransporte.Codigo.val(documento.Codigo);
    _documentoTransporte.NumeroCTE.val(documento.NumeroCTE);
    _documentoTransporte.NumeroNFE.val(documento.NumeroNFE);
    _documentoTransporte.ChaveAcessoCTE.val(documento.ChaveAcessoCTE);
    _documentoTransporte.ChaveAcessoNFE.val(documento.ChaveAcessoNFE);
    _documentoTransporte.Fornecedor.val(documento.Fornecedor);
    _documentoTransporte.Fornecedor.codEntity(documento.CodigoFornecedor);
    _documentoTransporte.Peso.val(documento.Peso);
    _documentoTransporte.Volumen.val(documento.Volumen);
    _documentoTransporte.Status.val(documento.Status == "OK" ? EnumStatusDocumentoTransporte.OK : EnumStatusDocumentoTransporte.NAOOK);
    _documentoTransporte.Observacao.val(documento.Observacao);

    ControleBotoesCRUD(true);

    AbrirModalDocumentoTransporte();
}
//#endregion

//#region Funções CRUD Documento Transporte
function AdicionarDocumentoTransporte() {
    const novoDocumentoTransporte = new Object();

    novoDocumentoTransporte.Codigo = _documentoTransporte.Codigo.val();
    novoDocumentoTransporte.NumeroNFE = _documentoTransporte.NumeroNFE.val();
    novoDocumentoTransporte.NumeroCTE = _documentoTransporte.NumeroCTE.val();
    novoDocumentoTransporte.Fornecedor = _documentoTransporte.Fornecedor.val();
    novoDocumentoTransporte.CodigoFornecedor = _documentoTransporte.Fornecedor.codEntity();
    novoDocumentoTransporte.ChaveAcessoCTE = _documentoTransporte.ChaveAcessoCTE.val();
    novoDocumentoTransporte.ChaveAcessoNFE = _documentoTransporte.ChaveAcessoNFE.val();
    novoDocumentoTransporte.Peso = _documentoTransporte.Peso.val();
    novoDocumentoTransporte.Volumen = _documentoTransporte.Volumen.val();
    novoDocumentoTransporte.Status = _documentoTransporte.Status.val() == 1 ? "OK" : "NÃO OK";
    novoDocumentoTransporte.Observacao = _documentoTransporte.Observacao.val();

    _documentoParaTransporte.DocumentosParaTrasporte.list.push(novoDocumentoTransporte);

    RecarregarGridDocumentoTransporte();
    LimparCamposDocumentoTransporte();
    FecharModalDocumentoTransporte();
}

function AtualizarDocumentoTransporte() {
    $.each(_documentoParaTransporte.DocumentosParaTrasporte.list, (i, itemDocumento) => {

        if (itemDocumento.Codigo == _documentoTransporte.Codigo.val()
            && itemDocumento.ChaveAcessoCTE == _documentoTransporte.ChaveAcessoCTE.val()
            && itemDocumento.ChaveAcessoNFE == _documentoTransporte.ChaveAcessoNFE.val()) {

            itemDocumento.Codigo = _documentoTransporte.Codigo.val();
            itemDocumento.NumeroNFE = _documentoTransporte.NumeroNFE.val();
            itemDocumento.NumeroCTE = _documentoTransporte.NumeroCTE.val();
            itemDocumento.Fornecedor = _documentoTransporte.Fornecedor.val();
            itemDocumento.CodigoFornecedor = _documentoTransporte.Fornecedor.codEntity();
            itemDocumento.ChaveAcessoCTE = _documentoTransporte.ChaveAcessoCTE.val();
            itemDocumento.ChaveAcessoNFE = _documentoTransporte.ChaveAcessoNFE.val();
            itemDocumento.Peso = _documentoTransporte.Peso.val();
            itemDocumento.Volumen = _documentoTransporte.Volumen.val();
            itemDocumento.Status = _documentoTransporte.Status.val() == 1 ? "OK" : "NÃO OK";
            itemDocumento.Observacao = _documentoTransporte.Observacao.val();
            return false;
        }
    });
    RecarregarGridDocumentoTransporte();
    LimparCamposDocumentoTransporte();
    FecharModalDocumentoTransporte();
    ControleBotoesCRUD(false);
}

function CancelarEdicaoDocumentoTransporte() {
    LimparCamposDocumentoTransporte();
    ControleBotoesCRUD(false);
}

function ControleBotoesCRUD(visible) {
    _crudDocumentoTransporte.Atualizar.visible(visible);
    _crudDocumentoTransporte.Cancelar.visible(visible);
    _crudDocumentoTransporte.Adicionar.visible(!visible);
}
//#endregion

//#region Funções Auxiliares
function AbrirModalDocumentoTransporte() {
    Global.abrirModal("divModalDocumentoParaTransporte");
}
function FecharModalDocumentoTransporte() {
    Global.fecharModal("divModalDocumentoParaTransporte");
}
function LimparCamposDocumentoTransporte() {
    LimparCampos(_documentoTransporte);
    _documentoTransporte.Peso.val(0);
    _documentoTransporte.Volumen.val(0);
}
function ObterDocumentoParaTransporteGrid() {
    const lista = _gridDadoParaTransporte.BuscarRegistros();
    return JSON.stringify(lista);
}
function SalvarDocumentoParaTransporte() {
    executarReST("AgendamentoColeta/SalvarDocumentoParaTransporte", {
        Codigo: _agendamentoColeta.CodigoAgendamento.val(),
        DocumentoParaTransporte: ObterDocumentoParaTransporteGrid()
    }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                BuscarAgendamento({ Codigo: _agendamentoColeta.CodigoAgendamento.val() });
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Registros salvos com sucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}
function FinalizarAgendamento() {
    executarReST("AgendamentoColeta/FinalizarAgendamentoColeta", { Codigo: _agendamentoColeta.Codigo.val() }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Error", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Finalizado com sucesso.");
    });
}
//#endregion
