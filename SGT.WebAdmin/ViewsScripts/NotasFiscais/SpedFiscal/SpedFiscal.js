/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumStatusArquivoSpedFiscal.js" />
/// <reference path="../../Enumeradores/EnumTipoMovimentoSpedFiscal.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/DocumentoEntrada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridSpedFiscal;
var _spedFiscal;
var _pesquisaSpedFiscal;
var _gridDocumentos;

var _tipoSpedFiscal = [
    { text: "Todos", value: EnumTipoMovimentoSpedFiscal.Todos },
    { text: "Entrada", value: EnumTipoMovimentoSpedFiscal.Entrada },
    { text: "Saída", value: EnumTipoMovimentoSpedFiscal.Saida }
];


var _SituacaoDocumentoSPEDFiscal = [
    { text: "Todos documentos", value: EnumSituacaoDocumentoSPEDFiscal.TodosDocumentos },
    { text: "Somente documentos SEM arquivo SPED gerado", value: EnumSituacaoDocumentoSPEDFiscal.SomenteDocumentosNaoGerados },
    { text: "Somente documentos COM arquivo SPED gerado", value: EnumSituacaoDocumentoSPEDFiscal.SomenteDocumentosGerados }
];

var PesquisaSpedFiscal = function () {

    var _statusPesquisaSpedFiscal = [
        { text: "Todos", value: 0 },
        { text: "Aguardando Geração", value: EnumStatusArquivoSpedFiscal.AguardandoGeracao },
        { text: "Gerado", value: EnumStatusArquivoSpedFiscal.Gerado },
        { text: "Em Processo", value: EnumStatusArquivoSpedFiscal.EmProcesso },
        { text: "Erro de Validação", value: EnumStatusArquivoSpedFiscal.ErroValidacao }
    ];

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusPesquisaSpedFiscal, def: 0, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSpedFiscal.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var SpedFiscal = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataInicial = PropertyEntity({ text: "*Data Emissão de: ", getType: typesKnockout.date, required: false, val: ko.observable("") });
    this.DataFinal = PropertyEntity({ text: "*Até: ", getType: typesKnockout.date, required: false, val: ko.observable("") });
    this.DataInventario = PropertyEntity({ text: "Data Inventário: ", getType: typesKnockout.date, enable: ko.observable(false), required: ko.observable(false) });
    this.DataInicialFinalizacao = PropertyEntity({ text: "Data Finalização de: ", getType: typesKnockout.date, required: false });
    this.DataFinalFinalizacao = PropertyEntity({ text: "Até: ", getType: typesKnockout.date, required: false });
    this.DataInicialEntrada = PropertyEntity({ text: "*Data Entrada de: ", getType: typesKnockout.date, required: false, val: ko.observable("") });
    this.DataFinalEntrada = PropertyEntity({ text: "*Até: ", getType: typesKnockout.date, required: false, val: ko.observable("") });

    this.ComExtensaoCFOP = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Com Extensão da CFOP?", def: false });
    this.ComNFSePropria = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Com NFS-e Própria?", def: false });
    this.ComInventario = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Com Inventário?", def: false, eventChange: ComInventarioChange });
    this.ComBlocoK = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Com Bloco K?", def: false, eventChange: ComInventarioChange });    
    this.ComRessarcimentoICMS = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Com Ressarcimento de ICMS - C176?", def: false });
    this.ComD160 = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Com D160 e derivados?", def: false });
    this.ComD170 = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Com registro de Pedágio no D170?", def: false });

    this.TipoMovimento = PropertyEntity({ val: ko.observable(EnumTipoMovimentoSpedFiscal.Todos), options: _tipoSpedFiscal, def: EnumTipoMovimentoSpedFiscal.Todos, text: "*Tipo Movimento: ", enable: ko.observable(true), required: true });
    this.SituacaoDocumentoSPEDFiscal = PropertyEntity({ val: ko.observable(EnumSituacaoDocumentoSPEDFiscal.TodosDocumentos), options: _SituacaoDocumentoSPEDFiscal, def: EnumSituacaoDocumentoSPEDFiscal.TodosDocumentos, text: "Documento gerado em SPED: ", enable: ko.observable(true), required: false });

    this.DocumentoEntrada = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Documento Entrada:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });
    this.Grid = PropertyEntity({ type: types.local });
    this.Documentos = PropertyEntity({ type: types.event, text: "Adicionar Nota Entrada", idBtnSearch: guid() });
    this.DocumentosEntrada = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Solicitar Geração", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadSpedFiscal() {

    _pesquisaSpedFiscal = new PesquisaSpedFiscal();
    KoBindings(_pesquisaSpedFiscal, "knockoutPesquisaSpedFiscal", false, _pesquisaSpedFiscal.Pesquisar.id);

    _spedFiscal = new SpedFiscal();
    KoBindings(_spedFiscal, "knockoutCadastroSpedFiscal");

    HeaderAuditoria("SpedFiscal", _spedFiscal);

    new BuscarEmpresa(_spedFiscal.Empresa);
    //new BuscarDocumentoEntrada(_spedFiscal.DocumentoEntrada, RetornoDocumentoEntrada, null, null, null, _spedFiscal.DataInicial, _spedFiscal.DataFinal, _spedFiscal.DataInicialEntrada, _spedFiscal.DataFinalEntrada, _spedFiscal.SituacaoDocumentoSPEDFiscal);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _spedFiscal.Empresa.visible(true);
        _spedFiscal.Empresa.required(true);
    }

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDocumentoEntradaClick(_spedFiscal.Documentos, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "10%" },
        { data: "Serie", title: "Série", width: "5%" },
        { data: "DataEmissao", title: "Emissão", width: "10%" },
        { data: "DataEntrada", title: "Entrada", width: "10%" },
        { data: "Chave", title: "Chave", width: "45%" }
    ];

    _gridDocumentos = new BasicDataTable(_spedFiscal.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarDocumentoEntrada(_spedFiscal.Documentos, function (r) {
        if (r !== null) {
            var documentos = _gridDocumentos.BuscarRegistros();
            for (var i = 0; i < r.length; i++)
                documentos.push({
                    Codigo: r[i].Codigo,
                    Numero: r[i].Numero,
                    Serie: r[i].Serie,
                    DataEmissao: r[i].DataEmissao,
                    DataEntrada: r[i].DataEntrada,
                    Chave: r[i].Chave
                });

            _gridDocumentos.CarregarGrid(documentos);
        }
    }, null, null, _gridDocumentos, _spedFiscal.DataInicial, _spedFiscal.DataFinal, _spedFiscal.DataInicialEntrada, _spedFiscal.DataFinalEntrada, _spedFiscal.SituacaoDocumentoSPEDFiscal);

    _spedFiscal.Documentos.basicTable = _gridDocumentos;

    RecarregarGridDocumentos();
    buscarSpedFiscals();
}

function preencherListasSelecao() {
    var documentos = new Array();

    $.each(_spedFiscal.Documentos.basicTable.BuscarRegistros(), function (i, doc) {
        documentos.push({ Documento: doc });
    });

    _spedFiscal.DocumentosEntrada.val(JSON.stringify(documentos));
}

function RecarregarGridDocumentos() {

    var data = new Array();

    _gridDocumentos.CarregarGrid(data);
}

function ExcluirDocumentoEntradaClick(knoutDocumento, data) {
    var documentoGrid = knoutDocumento.basicTable.BuscarRegistros();

    for (var i = 0; i < documentoGrid.length; i++) {
        if (data.Codigo === documentoGrid[i].Codigo) {
            documentoGrid.splice(i, 1);
            break;
        }
    }
    knoutDocumento.basicTable.CarregarGrid(documentoGrid);
}

function RetornoDocumentoEntrada(data) {
    _spedFiscal.DocumentoEntrada.codEntity(data.Codigo);
    _spedFiscal.DocumentoEntrada.val(data.Numero);
}

function ComInventarioChange(e, sender) {
    if ($("#" + _spedFiscal.ComInventario.id).prop("checked") || $("#" + _spedFiscal.ComBlocoK.id).prop("checked")) {
        _spedFiscal.DataInventario.enable(true);
        _spedFiscal.DataInventario.required(true);
    } else {
        _spedFiscal.DataInventario.enable(false);
        _spedFiscal.DataInventario.required(false);
        _spedFiscal.DataInventario.val("");
    }
}

function adicionarClick(e, sender) {
    preencherListasSelecao();
    Salvar(e, "SpedFiscal/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Geração do Sped solicitada com sucesso");
                _gridSpedFiscal.CarregarGrid();
                limparCamposSpedFiscal();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposSpedFiscal();
    _spedFiscal.DataInventario.enable(false);
    _spedFiscal.DataInventario.required(false);
}

//*******MÉTODOS*******


function buscarSpedFiscals() {
    var baixar = { descricao: "Baixar", id: "clasBaixar", evento: "onclick", metodo: baixarTXTSpedFiscal, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(baixar);

    _gridSpedFiscal = new GridView(_pesquisaSpedFiscal.Pesquisar.idGrid, "SpedFiscal/Pesquisa", _pesquisaSpedFiscal, menuOpcoes, null);
    _gridSpedFiscal.CarregarGrid();
}

function baixarTXTSpedFiscal(spedFiscalGrid) {
    if (spedFiscalGrid.CodigoStatus === EnumStatusArquivoSpedFiscal.Gerado) {
        var dados = { Codigo: spedFiscalGrid.Codigo }
        executarDownload("SpedFiscal/DownloadTXT", dados);
    } else
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Só é possível baixar o arquivo txt do Sped Fiscal com status Gerado", 60000);
}

function limparCamposSpedFiscal() {
    LimparCampos(_spedFiscal);
    RecarregarGridDocumentos();
}