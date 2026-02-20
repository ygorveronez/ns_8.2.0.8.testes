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


//*******MAPEAMENTO KNOUCKOUT*******

var _parametros;
var _gridPreOcorrencias;
var _TabelaParametros = null;
var _valoresAlterados = [];

var Parametros = function () {
    // Propriedades
    this.Data = PropertyEntity({ text: "Data: ", getType: typesKnockout.date, visible: ko.observable(true), enable: ko.observable(true) });
    this.HoraInicial = PropertyEntity({ text: "Hora Inicial: ", getType: typesKnockout.time, visible: ko.observable(true), enable: ko.observable(true)  });
    this.HoraFinal = PropertyEntity({ text: "Hora Final: ", getType: typesKnockout.time, visible: ko.observable(true), enable: ko.observable(true) });
    this.KmInicial = PropertyEntity({ text: "KM Inicial: ", getType: typesKnockout.int, visible: ko.observable(true), enable: ko.observable(true) });
    this.KmFinal = PropertyEntity({ text: "KM Final: ", getType: typesKnockout.int, visible: ko.observable(true), enable: ko.observable(true) });

    this.TabelaParametros = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EnumTipoParametro = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ocorrencia = PropertyEntity({ idGrid: guid(), visible: ko.observable(false) });

    // CRUD
    this.VisulizarOcorrencias = PropertyEntity({ eventClick: RecalcularOcorrenciasGeradas, type: types.event, text: "Visualizar Ocorrências", visible: ko.observable(true) });
    this.ObservacaoCTe = PropertyEntity({ eventClick: AbrirModalObservacaoCTe, type: types.event, text: "Observação CT-e", visible: ko.observable(true) });
}


//*******EVENTOS*******
function LoadParametros() {
    _parametros = new Parametros();
    KoBindings(_parametros, "knockoutParametros");

    GridPreOcorrencias();
}



//*******MÉTODOS*******
function GridPreOcorrencias() {
    // Cabecalho
    var header = [
        { data: "Descricao", title: "Descrição", width: "60%", className: "text-align-left", orderable: false },
        { data: "DescricaoValor", title: "Valor", width: "20%", className: "text-align-right", orderable: false }
    ];

    var editarColuna = {
        permite: _ocorrenciaParametroOcorrencia.Codigo.val() == 0,
        atualizarRow: true,
        callback: AtualizarGridOcorrencias,
    };

    // Grid
    var ko_grid = {
        Codigo: _ocorrenciaParametroOcorrencia.Codigo,
        HoraInicial: _parametros.HoraInicial,
        HoraFinal: _parametros.HoraFinal,
        KmInicial: _parametros.KmInicial,
        KmFinal: _parametros.KmFinal,
        TabelaParametros: _parametros.TabelaParametros,
        EnumTipoParametro: _parametros.EnumTipoParametro,
        Carga: _ocorrenciaParametroOcorrencia.Carga
    };

    _gridPreOcorrencias = new GridView(_parametros.Ocorrencia.idGrid, "OcorrenciaParametroOcorrencia/PesquisaOcorrencias", ko_grid, null, null, null, null, null, null, null, null, editarColuna);
    _gridPreOcorrencias.CarregarGrid();
}

function limparCamposParametro() {
    LimparCampos(_parametros);
    _valoresAlterados = [];
    _parametros.VisulizarOcorrencias.visible(true);
}

function AtualizarGridOcorrencias(dataRow) {
    for (var i in _valoresAlterados) {
        if (_valoresAlterados[i].Codigo == dataRow.Codigo)
            return;
    }

    _valoresAlterados.push({
        Codigo: dataRow.Codigo,
        Valor: dataRow.Valor
    });
}

function FluxoOcorrenciaHoraExtra() {
    _parametros.Data.visible(true);
    _parametros.HoraInicial.visible(true);
    _parametros.HoraFinal.visible(true);
    _parametros.KmInicial.visible(false);
    _parametros.KmFinal.visible(false);
}

function FluxoOcorrenciaEstadia() {
    _parametros.Data.visible(true);
    _parametros.HoraInicial.visible(true);
    _parametros.HoraFinal.visible(true);
    _parametros.KmInicial.visible(false);
    _parametros.KmFinal.visible(false);
}

function FluxoOcorrenciaPernoite() {
    _parametros.Data.visible(true);
    _parametros.HoraInicial.visible(false);
    _parametros.HoraFinal.visible(false);
    _parametros.KmInicial.visible(false);
    _parametros.KmFinal.visible(false);
}

function FluxoOcorrenciaEscolta() {
    _parametros.Data.visible(true);
    _parametros.HoraInicial.visible(true);
    _parametros.HoraFinal.visible(true);
    _parametros.KmInicial.visible(true);
    _parametros.KmFinal.visible(true);
}


function RecalcularOcorrenciasGeradas() {
    _gridPreOcorrencias.CarregarGrid();
}
