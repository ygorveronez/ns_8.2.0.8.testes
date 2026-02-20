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
/// <reference path="../../Consultas/DocumentoEntrada.js" />
/// <reference path="ContratoFinanciamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _contratoFinanciamentoDocumentoEntrada, _gridDocumentoEntradasContratoFinanciamento;

var ContratoFinanciamentoDocumentoEntrada = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.DocumentoEntrada = PropertyEntity({ type: types.event, text: "Adicionar Nota(s) Compra", idBtnSearch: guid(), enable: ko.observable(true) });
}

//*******EVENTOS*******

function loadContratoFinanciamentoDocumentoEntrada() {
    _contratoFinanciamentoDocumentoEntrada = new ContratoFinanciamentoDocumentoEntrada();
    KoBindings(_contratoFinanciamentoDocumentoEntrada, "knockoutDocumentoEntradaContratoFinanciamento");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirDocumentoEntradaClick(_contratoFinanciamentoDocumentoEntrada.DocumentoEntrada, data) } }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "10%" },
        { data: "Serie", title: "Série", width: "10%" },
        { data: "DataEmissao", title: "Data Emissão", width: "15%" },
        { data: "DataEntrada", title: "Data Entrada", width: "15%" },
        { data: "Chave", title: "Chave", width: "40%" }
    ];
    _gridDocumentoEntradasContratoFinanciamento = new BasicDataTable(_contratoFinanciamentoDocumentoEntrada.Grid.id, header, menuOpcoes);

    new BuscarDocumentoEntrada(_contratoFinanciamentoDocumentoEntrada.DocumentoEntrada, function (r) {
        if (r != null) {
            var bens = _gridDocumentoEntradasContratoFinanciamento.BuscarRegistros();
            for (var i = 0; i < r.length; i++)
                bens.push({
                    Codigo: r[i].Codigo,
                    Numero: r[i].Numero,
                    Serie: r[i].Serie,
                    DataEmissao: r[i].DataEmissao,
                    DataEntrada: r[i].DataEntrada,
                    Chave: r[i].Chave,
                });

            _gridDocumentoEntradasContratoFinanciamento.CarregarGrid(bens);
        }
    }, null, null, _gridDocumentoEntradasContratoFinanciamento);

    _contratoFinanciamentoDocumentoEntrada.DocumentoEntrada.basicTable = _gridDocumentoEntradasContratoFinanciamento;

    RecarregarGridContratoFinanciamentoDocumentoEntrada();
}

//*******MÉTODOS*******

function RecarregarGridContratoFinanciamentoDocumentoEntrada() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_contratoFinanciamento.DocumentosEntrada.val())) {

        $.each(_contratoFinanciamento.DocumentosEntrada.val(), function (i, documentoEntrada) {
            var documentoEntradaGrid = new Object();

            documentoEntradaGrid.Codigo = documentoEntrada.DOCUMENTOENTRADA.Codigo;
            documentoEntradaGrid.Numero = documentoEntrada.DOCUMENTOENTRADA.Numero;
            documentoEntradaGrid.Serie = documentoEntrada.DOCUMENTOENTRADA.Serie;
            documentoEntradaGrid.DataEmissao = documentoEntrada.DOCUMENTOENTRADA.DataEmissao;
            documentoEntradaGrid.DataEntrada = documentoEntrada.DOCUMENTOENTRADA.DataEntrada;
            documentoEntradaGrid.Chave = documentoEntrada.DOCUMENTOENTRADA.Chave;

            data.push(documentoEntradaGrid);
        });
    }

    _gridDocumentoEntradasContratoFinanciamento.CarregarGrid(data);
}

function ExcluirDocumentoEntradaClick(knoutDocumentoEntrada, data) {
    var documentoEntradaGrid = knoutDocumentoEntrada.basicTable.BuscarRegistros();

    for (var i = 0; i < documentoEntradaGrid.length; i++) {
        if (data.Codigo == documentoEntradaGrid[i].Codigo) {
            documentoEntradaGrid.splice(i, 1);
            break;
        }
    }

    knoutDocumentoEntrada.basicTable.CarregarGrid(documentoEntradaGrid);
}

function preencherListasSelecaoContratoFinanciamentoDocumentoEntrada() {
    var documentoEntradas = new Array();

    $.each(_contratoFinanciamentoDocumentoEntrada.DocumentoEntrada.basicTable.BuscarRegistros(), function (i, documentoEntrada) {
        documentoEntradas.push({ DOCUMENTOENTRADA: documentoEntrada });
    });

    _contratoFinanciamento.DocumentosEntrada.val(JSON.stringify(documentoEntradas));
}

function limparCamposContratoFinanciamentoDocumentoEntrada() {
    LimparCampos(_contratoFinanciamentoDocumentoEntrada);
    RecarregarGridContratoFinanciamentoDocumentoEntrada();
}