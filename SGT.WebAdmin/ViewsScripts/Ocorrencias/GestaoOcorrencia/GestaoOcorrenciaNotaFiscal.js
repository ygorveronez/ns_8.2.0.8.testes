/// <reference path="../../Consultas/NotaFiscal.js" />
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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridNotaFiscal;
var _notaFiscal;

var GestaoOcorrenciaNotaFiscal = function () {
    this.GridNotaFiscal = PropertyEntity({ type: types.local });
    this.NFe = PropertyEntity({ type: types.event, text: "Notas Fiscais", idBtnSearch: guid(), enable: ko.observable(true) });
    this.OcorrenciaPorNotaFiscal = PropertyEntity({ def: false, getType: typesKnockout.bool, val: ko.observable(false) });
}


//*******EVENTOS*******

function loadGestaoOcorrenciaNotaFiscal() {
    _notaFiscal = new GestaoOcorrenciaNotaFiscal();
    KoBindings(_notaFiscal, "knockoutGestaoOcorrenciaNotaFiscal");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirNotaFiscalClick(_notaFiscal.NFe, data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: Localization.Resources.Gerais.Geral.Numero, width: "80%" },
        { data: "Emitente", title: Localization.Resources.Gerais.Geral.Emitente, width: "80%" },
        { data: "DataEmissao", title: Localization.Resources.Gerais.Geral.DataEmissao, width: "80%" },
    ];

    _gridNotaFiscal = new BasicDataTable(_notaFiscal.GridNotaFiscal.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridNotaFiscal.CarregarGrid([]);

    new BuscarXMLNotaFiscal(_notaFiscal.NFe, null, _gestaoOcorrencia.Carga, _gridNotaFiscal, null);
    _notaFiscal.NFe.basicTable = _gridNotaFiscal;

    if (_gestaoOcorrencia.CargaEntrega.val() == "")
        _notaFiscal.NFe.enable(false);

    recarregarGridNotaFiscal();
}


function preencherXMLNotasFiscais(data) {
    _gestaoOcorrencia.NotasFiscais.val(data);
    recarregarGridNotaFiscal();
}

function recarregarGridNotaFiscal() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_gestaoOcorrencia.NotasFiscais.val())) {

        $.each(_gestaoOcorrencia.NotasFiscais.val(), function (i, notaFiscal) {
            var notaFiscalGrid = new Object();

            notaFiscalGrid.Codigo = notaFiscal.Codigo;
            notaFiscalGrid.Numero = notaFiscal.Numero;
            notaFiscalGrid.Emitente = notaFiscal.Emitente;
            notaFiscalGrid.DataEmissao = notaFiscal.DataEmissao;


            data.push(notaFiscalGrid);
        });
    }

    _gridNotaFiscal.CarregarGrid(data);
}


function excluirNotaFiscalClick(knoutNotaFiscal, data) {
    var notaFiscalGrid = knoutNotaFiscal.basicTable.BuscarRegistros();

    for (var i = 0; i < notaFiscalGrid.length; i++) {
        if (data.Codigo == notaFiscalGrid[i].Codigo) {
            notaFiscalGrid.splice(i, 1);
            break;
        }
    }

    knoutNotaFiscal.basicTable.CarregarGrid(notaFiscalGrid);
}

function limparCamposNotaFiscal() {
    LimparCampos(_notaFiscal);
    LimparCampos(_gridNotaFiscal);
    _gridNotaFiscal.CarregarGrid([]);
}

function obterNotasFiscais() {
    let codigosNotasFiscais = new Array();

    $.each(_notaFiscal.NFe.basicTable.BuscarRegistros(), function (i, notaFiscal) {
        codigosNotasFiscais.push(notaFiscal.Codigo);
    });
    return JSON.stringify(codigosNotasFiscais);
}

function permitirSelecionarNotasFiscais() {
    _notaFiscal.NFe.enable(true);
}

function bloquearSelecionarNotasFiscais() {
    _notaFiscal.NFe.enable(false);
}