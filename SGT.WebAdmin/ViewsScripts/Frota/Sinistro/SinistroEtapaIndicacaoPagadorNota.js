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
/// <reference path="SinistroEtapaIndicacaoPagador.js" />
/// <reference path="../../Consultas/DocumentoEntrada.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDocumentoEntrada;
var _etapaIndicadorPagadorNota;

var IndicacaoPagadorSinistroNota = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.DocumentoEntrada = PropertyEntity({ type: types.event, text: "Adicionar Documento de Entrada", idBtnSearch: guid(), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadIndicacaoPagadorSinistroNota() {
    _etapaIndicadorPagadorNota = new IndicacaoPagadorSinistroNota();
    KoBindings(_etapaIndicadorPagadorNota, "knockoutOperadorIndicacaoPagadorSinistroNota");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { excluirDocumentoEntradaClick(data) } }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "15%" },
        { data: "Fornecedor", title: "Fornecedor", width: "45%" },
        { data: "DataEmissao", title: "Data Emissão", width: "15%" },
        { data: "ValorTotal", title: "Valor Total", width: "15%" },
    ];

    _gridDocumentoEntrada = new BasicDataTable(_etapaIndicadorPagadorNota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarDocumentoEntrada(_etapaIndicadorPagadorNota.DocumentoEntrada, null, null, null, _gridDocumentoEntrada);
    _etapaIndicadorPagadorNota.DocumentoEntrada.basicTable = _gridDocumentoEntrada;

    recarregarGridIndicacaoPagadorNota();
}

function recarregarGridIndicacaoPagadorNota() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_etapaIndicadorPagadorSinistro.Notas.val())) {
        $.each(_etapaIndicadorPagadorSinistro.Notas.val(), function (i, documentoEntrada) {
            var documentoEntradaGrid = new Object();

            documentoEntradaGrid.Codigo = documentoEntrada.Codigo;
            documentoEntradaGrid.Numero = documentoEntrada.Numero;
            documentoEntradaGrid.Fornecedor = documentoEntrada.Fornecedor;
            documentoEntradaGrid.DataEmissao = documentoEntrada.DataEmissao;
            documentoEntradaGrid.ValorTotal = documentoEntrada.ValorTotal;

            data.push(documentoEntradaGrid);
        });
    }

    _gridDocumentoEntrada.CarregarGrid(data);
}

function excluirDocumentoEntradaClick(data) {
    var documentoEntradaGrid = _etapaIndicadorPagadorNota.DocumentoEntrada.basicTable.BuscarRegistros();

    for (var i = 0; i < documentoEntradaGrid.length; i++) {
        if (data.Codigo == documentoEntradaGrid[i].Codigo) {
            documentoEntradaGrid.splice(i, 1);
            break;
        }
    }

    _etapaIndicadorPagadorNota.DocumentoEntrada.basicTable.CarregarGrid(documentoEntradaGrid);
}

function bloquearCamposIndicadorPagadorNota() {
    SetarEnableCamposKnockout(_etapaIndicadorPagadorNota, false);

    _gridDocumentoEntrada.DesabilitarOpcoes();
}

function limparCamposIndicacaoPagadorSinistroNota() {
    LimparCampos(_etapaIndicadorPagadorNota);
    _etapaIndicadorPagadorNota.DocumentoEntrada.basicTable.CarregarGrid(new Array());
    SetarEnableCamposKnockout(_etapaIndicadorPagadorNota, true);

    _gridDocumentoEntrada.HabilitarOpcoes();
}